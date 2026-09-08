using System;
using System.IO;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal.Parsers;

public class AssetVertexOverrideParser
{
	public struct VertexOverride
	{
		public int originalIndex;

		public Vector3 position;

		public Vector3 normal;

		public Vector4b skinWeights;

		public Vector4b skinBindings;

		public Colorb vertexColor;
	}

	public class VertexOverrideParser
	{
		public IntegerDataUnpacker m_OriginalIndexPacker = new IntegerDataUnpacker();

		public Vector3dDataUnpacker m_PositionPacker = new Vector3dDataUnpacker();

		public IntegerDataUnpacker m_NormalPacker = new IntegerDataUnpacker();

		public IntegerDataUnpacker m_SkinWeightsPacker = new IntegerDataUnpacker();

		public IntegerDataUnpacker m_SkinBindingsPacker = new IntegerDataUnpacker();

		public IntegerDataUnpacker m_ColorPacker = new IntegerDataUnpacker();

		public void UnpackHeader(BitStream bitStream)
		{
			m_OriginalIndexPacker.UnpackHeader(bitStream);
			m_PositionPacker.UnpackHeader(bitStream);
			m_NormalPacker.UnpackHeader(bitStream);
			m_SkinWeightsPacker.UnpackHeader(bitStream);
			m_SkinBindingsPacker.UnpackHeader(bitStream);
			m_ColorPacker.UnpackHeader(bitStream);
		}

		public void UnpackDataRightHanded(BitStream bitStream, out VertexOverride data)
		{
			m_OriginalIndexPacker.UnpackData(bitStream, out data.originalIndex);
			m_PositionPacker.UnpackData(bitStream, out data.position);
			m_NormalPacker.UnpackData(bitStream, out var data2);
			data.normal = VectorMath.CreateFromPackedNormal(data2);
			m_SkinWeightsPacker.UnpackData(bitStream, out data2);
			data.skinWeights = new Vector4b(data2);
			m_SkinBindingsPacker.UnpackData(bitStream, out data2);
			data.skinBindings = new Vector4b(data2);
			m_ColorPacker.UnpackData(bitStream, out data2);
			data.vertexColor = new Colorb(data2);
		}

		public void UnpackDataLeftHanded(BitStream bitStream, out VertexOverride data)
		{
			m_OriginalIndexPacker.UnpackData(bitStream, out data.originalIndex);
			m_PositionPacker.UnpackData(bitStream, out data.position);
			m_NormalPacker.UnpackData(bitStream, out var data2);
			data.normal = VectorMath.CreateFromPackedNormalFlipCoordinates(data2);
			m_SkinWeightsPacker.UnpackData(bitStream, out data2);
			data.skinWeights = new Vector4b(data2);
			m_SkinBindingsPacker.UnpackData(bitStream, out data2);
			data.skinBindings = new Vector4b(data2);
			m_ColorPacker.UnpackData(bitStream, out data2);
			data.vertexColor = new Colorb(data2);
		}

		public void FlipCoordinateSystem()
		{
			m_PositionPacker.InvertCoordinateSystem();
		}
	}

	public int m_GlobalVertexBufferSize;

	public Guid m_OriginalAssetId;

	public CoordinateSystem m_CoordinateSystem;

	public VertexOverride[] m_VertexData;

	public AssetVertexOverrideParser(CoordinateSystem coordinateSystem)
	{
		m_CoordinateSystem = coordinateSystem;
	}

	public Guid GetOriginalAssetId()
	{
		return m_OriginalAssetId;
	}

	public void Parse(Stream stream)
	{
		BitStream bitStream = new BitStream(stream);
		int num = bitStream.ReadInt(32);
		if ((long)num > 8192L)
		{
			Logger.Log(new DebugLog(this, Resources.VertexOverrideError1));
			throw new AvatarException(Resources.VertexOverrideError1);
		}
		m_GlobalVertexBufferSize = bitStream.ReadInt(32);
		m_OriginalAssetId = new Guid(bitStream.ReadByteArray(16));
		VertexOverrideParser vertexOverrideParser = new VertexOverrideParser();
		vertexOverrideParser.UnpackHeader(bitStream);
		m_VertexData = new VertexOverride[num];
		if (m_CoordinateSystem == CoordinateSystem.RightHanded)
		{
			for (int i = 0; i < num; i++)
			{
				vertexOverrideParser.UnpackDataRightHanded(bitStream, out m_VertexData[i]);
			}
			return;
		}
		vertexOverrideParser.FlipCoordinateSystem();
		for (int j = 0; j < num; j++)
		{
			vertexOverrideParser.UnpackDataLeftHanded(bitStream, out m_VertexData[j]);
		}
	}

	public void Apply(AvatarComponent avatarComponent)
	{
		int num = avatarComponent.m_Batches.Length;
		int[] array = new int[num + 1];
		int[] array2 = new int[num];
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			int num3 = avatarComponent.m_Batches[i].Vertices.TextureChannelCount * 4 + 28;
			array[i] = num2;
			array2[i] = num3;
			num2 += avatarComponent.m_Batches[i].Vertices.Positions.Length * num3;
		}
		array[num] = num2;
		int num4 = m_VertexData.Length;
		for (int j = 0; j < num4; j++)
		{
			int originalIndex = m_VertexData[j].originalIndex;
			int num5 = 1;
			while (true)
			{
				if (num5 <= num)
				{
					if (originalIndex < array[num5])
					{
						break;
					}
					num5++;
					continue;
				}
				Logger.Log(new DebugLog(this, Resources.VertexOverrideError2));
				throw new AvatarException(Resources.VertexOverrideError2);
			}
			num5--;
			originalIndex = (originalIndex - array[num5]) / array2[num5];
			ref Vector3 reference = ref avatarComponent.m_Batches[num5].Vertices.Positions[originalIndex];
			reference = m_VertexData[j].position;
			ref Vector3 reference2 = ref avatarComponent.m_Batches[num5].Vertices.Normals[originalIndex];
			reference2 = m_VertexData[j].normal;
			ref Vector3 reference3 = ref avatarComponent.m_Batches[num5].Vertices.RawPositions[originalIndex];
			reference3 = m_VertexData[j].position;
			ref Vector3 reference4 = ref avatarComponent.m_Batches[num5].Vertices.RawNormals[originalIndex];
			reference4 = m_VertexData[j].normal;
			ref Vector4b reference5 = ref avatarComponent.m_Batches[num5].Vertices.SkinBindings[originalIndex];
			reference5 = m_VertexData[j].skinBindings;
			ref Vector4b reference6 = ref avatarComponent.m_Batches[num5].Vertices.SkinWeights[originalIndex];
			reference6 = m_VertexData[j].skinWeights;
			avatarComponent.m_Batches[num5].Vertices.Color0[originalIndex].red = (float)(int)m_VertexData[j].vertexColor.red / 255f;
			avatarComponent.m_Batches[num5].Vertices.Color0[originalIndex].green = (float)(int)m_VertexData[j].vertexColor.green / 255f;
			avatarComponent.m_Batches[num5].Vertices.Color0[originalIndex].blue = (float)(int)m_VertexData[j].vertexColor.blue / 255f;
			avatarComponent.m_Batches[num5].Vertices.Color0[originalIndex].alpha = (float)(int)m_VertexData[j].vertexColor.alpha / 255f;
		}
	}

	public int GetMemoryUsage()
	{
		return m_VertexData.Length * 32 + 64;
	}
}
