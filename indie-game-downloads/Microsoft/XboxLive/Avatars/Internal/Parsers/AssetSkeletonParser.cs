using System.IO;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal.Parsers;

public class AssetSkeletonParser
{
	public class SkeletonDataParser : DataUnpackerGeneric<Joint>
	{
		public SmallDataUnpacker<byte> m_Parent = new SmallDataUnpacker<byte>(1);

		public Vector3dDataUnpacker m_BindPosePosition = new Vector3dDataUnpacker();

		public QuaternionDataUnpacker m_BindPoseRotation = new QuaternionDataUnpacker();

		public override void UnpackHeader(BitStream bitStream)
		{
			m_Parent.UnpackHeader(bitStream);
			m_BindPosePosition.UnpackHeader(bitStream);
			m_BindPoseRotation.UnpackHeader(bitStream);
		}

		public override void UnpackData(BitStream bitStream, out Joint data)
		{
			data = default(Joint);
			m_Parent.UnpackData(bitStream, out byte data2);
			m_BindPosePosition.UnpackData(bitStream, out data.BindPosition);
			m_BindPoseRotation.UnpackData(bitStream, out data.BindRotation);
			data.Parent = data2;
		}
	}

	public CoordinateSystem m_CoordSys;

	public AssetSkeletonParser(CoordinateSystem coordSys)
	{
		m_CoordSys = coordSys;
	}

	public static void RecalculateChildren(Joint[] joints)
	{
		int num = joints.Length;
		int i;
		for (i = 0; i < num; i++)
		{
			joints[i].Child = -1;
			joints[i].Sibling = -1;
		}
		i = num;
		while (--i >= 0)
		{
			int parent = joints[i].Parent;
			if (parent != 255)
			{
				if (joints[parent].Child == -1)
				{
					joints[parent].Child = i;
					continue;
				}
				joints[i].Sibling = joints[parent].Child;
				joints[parent].Child = i;
			}
		}
	}

	public static void RecalculateLocalTransforms(Joint[] joints)
	{
		int num = joints.Length;
		while (--num >= 1)
		{
			Matrix matrixA = Matrix.CreateFromQuaternion(joints[num].BindRotation);
			matrixA.Translation = joints[num].BindPosition;
			int parent = joints[num].Parent;
			Matrix matrix = Matrix.CreateFromQuaternion(joints[parent].BindRotation);
			matrix.Translation = joints[parent].BindPosition;
			Matrix matrixB = MatrixMath.InvertEuclidean(matrix);
			Matrix matrix2 = Matrix.Multiply(matrixA, matrixB);
			joints[num].Local.position = matrix2.Translation;
			joints[num].Local.rotation = Quaternion.CreateFromRotationMatrix(matrix2);
			joints[num].Local.scale = new Vector3(1f, 1f, 1f);
		}
		joints[0].Local.position = joints[0].BindPosition;
		joints[0].Local.rotation = joints[0].BindRotation;
		joints[0].Local.scale = new Vector3(1f, 1f, 1f);
	}

	public Skeleton Parse(Stream stream)
	{
		Skeleton skeleton = new Skeleton();
		ByteStreamUnpacker<Joint> byteStreamUnpacker = new ByteStreamUnpacker<Joint>(stream, new SkeletonDataParser());
		byteStreamUnpacker.Unpack(out skeleton.Joints);
		if (skeleton.Joints.Length > 72)
		{
			Logger.Log(new DebugLog(this, Resources.SkeletonParserError1));
			throw new AvatarException(Resources.SkeletonParserError1);
		}
		if (m_CoordSys == CoordinateSystem.LeftHanded)
		{
			int num = skeleton.Joints.Length;
			for (int i = 0; i < num; i++)
			{
				skeleton.Joints[i].BindPosition.Z = 0f - skeleton.Joints[i].BindPosition.Z;
				skeleton.Joints[i].BindRotation.X = 0f - skeleton.Joints[i].BindRotation.X;
				skeleton.Joints[i].BindRotation.Y = 0f - skeleton.Joints[i].BindRotation.Y;
			}
		}
		RecalculateChildren(skeleton.Joints);
		RecalculateLocalTransforms(skeleton.Joints);
		return skeleton;
	}
}
