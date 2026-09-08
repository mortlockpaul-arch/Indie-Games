using System.IO;
using Microsoft.XboxLive.Avatars.Internal.Animations;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal.Parsers;

public class AssetAnimationParser
{
	public class SkeletonPosePacker : DataUnpackerGeneric<Pose>
	{
		public Vector3dDataUnpacker position = new Vector3dDataUnpacker();

		public QuaternionDataUnpacker rotation = new QuaternionDataUnpacker();

		public Vector3dDataUnpacker scale = new Vector3dDataUnpacker();

		public override int GetHeaderBitCount()
		{
			return position.GetHeaderBitCount() + rotation.GetHeaderBitCount() + scale.GetHeaderBitCount();
		}

		public override int GetPerDataBitCount()
		{
			return position.GetPerDataBitCount() + rotation.GetPerDataBitCount() + scale.GetPerDataBitCount();
		}

		public override void UnpackHeader(BitStream bitStream)
		{
			position.UnpackHeader(bitStream);
			rotation.UnpackHeader(bitStream);
			scale.UnpackHeader(bitStream);
		}

		public override void UnpackData(BitStream bitStream, out Pose pose)
		{
			position.UnpackData(bitStream, out pose.position);
			rotation.UnpackData(bitStream, out pose.rotation);
			scale.UnpackData(bitStream, out pose.scale);
		}
	}

	public class AvatarExpressionPacker : DataUnpackerGeneric<AvatarExpression>
	{
		public IntegerDataUnpacker MouthLayer = new IntegerDataUnpacker();

		public IntegerDataUnpacker LeftEyebrowLayer = new IntegerDataUnpacker();

		public IntegerDataUnpacker RightEyebrowLayer = new IntegerDataUnpacker();

		public IntegerDataUnpacker LeftEyeLayer = new IntegerDataUnpacker();

		public IntegerDataUnpacker RightEyeLayer = new IntegerDataUnpacker();

		public override void UnpackHeader(BitStream bitStream)
		{
			int num = bitStream.ReadInt(32);
			if (num != 5)
			{
				Logger.Log(new DebugLog(this, Resources.InvalidAvatarExpressionFormatText));
				throw new AvatarException(Resources.InvalidAvatarExpressionFormatText);
			}
			RightEyeLayer.UnpackHeader(bitStream);
			LeftEyeLayer.UnpackHeader(bitStream);
			RightEyebrowLayer.UnpackHeader(bitStream);
			LeftEyebrowLayer.UnpackHeader(bitStream);
			MouthLayer.UnpackHeader(bitStream);
		}

		public override void UnpackData(BitStream bitStream, out AvatarExpression exp)
		{
			MouthLayer.UnpackData(bitStream, out exp.MouthLayer);
			if (exp.MouthLayer > 13)
			{
				throw new AvatarException(Resources.InvalidAvatarMouthTextureIndex);
			}
			LeftEyebrowLayer.UnpackData(bitStream, out exp.LeftEyebrowLayer);
			if (exp.LeftEyebrowLayer > 4)
			{
				throw new AvatarException(Resources.InvalidAvatarEyebrowTextureIndex);
			}
			RightEyebrowLayer.UnpackData(bitStream, out exp.RightEyebrowLayer);
			if (exp.RightEyebrowLayer > 4)
			{
				throw new AvatarException(Resources.InvalidAvatarEyebrowTextureIndex);
			}
			LeftEyeLayer.UnpackData(bitStream, out exp.LeftEyeLayer);
			if (exp.LeftEyeLayer > 13)
			{
				Logger.Log(new DebugLog(this, Resources.InvalidAvatarEyeTextureIndex + " error has occured in left eye layer."));
				exp.LeftEyeLayer = 12;
			}
			RightEyeLayer.UnpackData(bitStream, out exp.RightEyeLayer);
			if (exp.RightEyeLayer > 13)
			{
				Logger.Log(new DebugLog(this, Resources.InvalidAvatarEyeTextureIndex + " error has occured in right eye layer."));
				exp.RightEyeLayer = 12;
			}
		}
	}

	public float m_FramesPerSecond;

	public int m_SecondaryOffset;

	public int m_TexturesOffset;

	public int m_CompressedSize;

	public byte[] m_CompressedAnim;

	public CoordinateSystem m_CoordinateSystem;

	public AvatarGender m_bodyTypeMask;

	public AssetAnimationParser(CoordinateSystem coordSys, AvatarGender bodyMask)
	{
		m_CoordinateSystem = coordSys;
		m_bodyTypeMask = bodyMask;
	}

	public static void InvertCoordinateSystem(InterleavedDataUnpacker<Pose, SkeletonPosePacker> unpacker)
	{
		SkeletonPosePacker[] unpackers = unpacker.Unpackers;
		int num = unpackers.Length;
		for (int i = 0; i < num; i++)
		{
			unpackers[i].position.InvertCoordinateSystem();
			unpackers[i].rotation.InvertCoordinateSystem();
		}
	}

	public AvatarAnimation Parse(Stream stream)
	{
		BitStream bitStream = new BitStream(stream);
		bitStream.ReadInt(32);
		m_FramesPerSecond = bitStream.ReadFloat();
		bitStream.ReadInt(32);
		bitStream.ReadInt(32);
		bitStream.ReadInt(32);
		bitStream.ReadInt(32);
		m_SecondaryOffset = bitStream.ReadInt(32);
		m_TexturesOffset = bitStream.ReadInt(32);
		bitStream.ReadInt(32);
		m_CompressedSize = bitStream.ReadInt(32);
		m_CompressedAnim = new byte[m_CompressedSize];
		stream.Read(m_CompressedAnim, 0, m_CompressedSize);
		MemoryStream memoryStream = new MemoryStream(m_CompressedAnim);
		memoryStream.Seek(0L, SeekOrigin.Begin);
		InterleavedDataUnpacker<Pose, SkeletonPosePacker> unpacker = new InterleavedDataUnpacker<Pose, SkeletonPosePacker>(72);
		ByteStreamUnpacker<Pose[]> byteStreamUnpacker = new ByteStreamUnpacker<Pose[]>(memoryStream, unpacker);
		byteStreamUnpacker.UnpackHeader();
		if (m_CoordinateSystem == CoordinateSystem.LeftHanded)
		{
			InvertCoordinateSystem(unpacker);
		}
		byteStreamUnpacker.UnpackData(out var result);
		memoryStream.Seek(m_SecondaryOffset, SeekOrigin.Begin);
		InterleavedDataUnpacker<Pose, SkeletonPosePacker> unpacker2 = new InterleavedDataUnpacker<Pose, SkeletonPosePacker>(72);
		ByteStreamUnpacker<Pose[]> byteStreamUnpacker2 = new ByteStreamUnpacker<Pose[]>(memoryStream, unpacker2);
		byteStreamUnpacker2.UnpackHeader();
		if (m_CoordinateSystem == CoordinateSystem.LeftHanded)
		{
			InvertCoordinateSystem(unpacker2);
		}
		byteStreamUnpacker2.UnpackData(out var result2);
		if (result2[0].Length == 0)
		{
			result2 = null;
		}
		memoryStream.Seek(m_TexturesOffset, SeekOrigin.Begin);
		ByteStreamUnpacker<AvatarExpression> byteStreamUnpacker3 = new ByteStreamUnpacker<AvatarExpression>(memoryStream, new AvatarExpressionPacker());
		byteStreamUnpacker3.Unpack(out var result3);
		return new AvatarAnimation(result, result2, result3, m_FramesPerSecond, m_bodyTypeMask);
	}

	public static void NormalizeKeyframesPolarity(Pose[][] keyframes)
	{
		int num = keyframes.Length;
		int num2 = keyframes[0].Length;
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num - 1; j++)
			{
				Quaternion rotation = keyframes[j][i].rotation;
				Quaternion rotation2 = keyframes[j + 1][i].rotation;
				if (rotation.X * rotation2.X + rotation.Y * rotation2.Y + rotation.Z * rotation2.Z + rotation.W * rotation2.W < 0f)
				{
					rotation2.X = 0f - rotation2.X;
					rotation2.Y = 0f - rotation2.Y;
					rotation2.Z = 0f - rotation2.Z;
					rotation2.W = 0f - rotation2.W;
					keyframes[j + 1][i].rotation = rotation2;
				}
			}
		}
	}
}
