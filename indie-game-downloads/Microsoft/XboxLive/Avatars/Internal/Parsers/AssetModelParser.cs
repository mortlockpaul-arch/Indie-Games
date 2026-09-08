using System;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal.Parsers;

public class AssetModelParser
{
	public AvatarComponent m_Model;

	public CoordinateSystem m_CoordinateSystem;

	public AssetModelParser(AvatarComponent model, CoordinateSystem coordinateSystem)
	{
		m_Model = model;
		m_CoordinateSystem = coordinateSystem;
	}

	public void Parse(EndianStream stream, IResourceFactory resourceFactory)
	{
		bool littleEndian = stream.LittleEndian;
		stream.LittleEndian = true;
		stream.ReadInt();
		stream.ReadInt();
		stream.ReadInt();
		stream.ReadInt();
		stream.ReadInt();
		uint num = stream.ReadUInt();
		uint num2 = stream.ReadUInt();
		uint num3 = stream.ReadUInt();
		stream.ReadInt();
		stream.ReadInt();
		stream.ReadInt();
		stream.ReadInt();
		if (num > 16)
		{
			Logger.Log(new DebugLog(this, Resources.ModelParserError1));
			throw new AvatarException(Resources.ModelParserError1);
		}
		if (num2 > 18)
		{
			Logger.Log(new DebugLog(this, Resources.ModelParserError2));
			throw new AvatarException(Resources.ModelParserError2);
		}
		m_Model.m_Batches = new TriangleBatch[num];
		m_Model.m_ShaderInstance = new ShaderInstance[num];
		m_Model.m_Textures = new IBaseTextureAnimated[num2];
		uint[] array = new uint[num];
		uint[] array2 = new uint[num];
		for (int i = 0; i < num; i++)
		{
			AssetTriangleBatchParser assetTriangleBatchParser = new AssetTriangleBatchParser(m_CoordinateSystem);
			assetTriangleBatchParser.Parse(stream);
			ref TriangleBatch reference = ref m_Model.m_Batches[i];
			reference = assetTriangleBatchParser.TriangleBatch;
			ref ShaderInstance reference2 = ref m_Model.m_ShaderInstance[i];
			reference2 = assetTriangleBatchParser.ShaderInstance;
			array[i] = assetTriangleBatchParser.VertexGpuOffset - num3;
			array2[i] = assetTriangleBatchParser.VertexSize;
		}
		for (int j = 0; j < num2; j++)
		{
			stream.LittleEndian = true;
			stream.ReadInt();
			stream.ReadInt();
			stream.LittleEndian = littleEndian;
			AssetTextureParser assetTextureParser = new AssetTextureParser();
			assetTextureParser.Parse(stream, resourceFactory);
			m_Model.m_Textures[j] = assetTextureParser.Texture;
		}
		ReadVertexPairs(stream, array, array2);
	}

	public static void GetBatchAndIndex(uint gpuOffset, uint[] offsetsTable, uint[] vertexSizesTable, out uint batchIdx, out uint vertexIdx)
	{
		batchIdx = 0u;
		while (batchIdx < offsetsTable.Length && gpuOffset >= offsetsTable[batchIdx])
		{
			batchIdx++;
		}
		batchIdx--;
		gpuOffset -= offsetsTable[batchIdx];
		vertexIdx = gpuOffset / vertexSizesTable[batchIdx];
	}

	public void ReadVertexPairs(EndianStream stream, uint[] offsetsTable, uint[] vertexSizesTable)
	{
		IntegerDataUnpacker integerDataUnpacker = new IntegerDataUnpacker();
		try
		{
			while (true)
			{
				bool flag = true;
				BitStream bitStream = new BitStream(stream);
				int num = bitStream.ReadInt(32);
				if ((long)num > 400L)
				{
					Logger.Log(new DebugLog(this, Resources.ModelParserError3));
					throw new AvatarException(Resources.ModelParserError3);
				}
				if (num % 2 > 0)
				{
					break;
				}
				integerDataUnpacker.UnpackHeader(bitStream);
				for (int i = 0; i < num; i += 2)
				{
					integerDataUnpacker.UnpackData(bitStream, out var data);
					integerDataUnpacker.UnpackData(bitStream, out var data2);
					GetBatchAndIndex((uint)data, offsetsTable, vertexSizesTable, out var batchIdx, out var vertexIdx);
					GetBatchAndIndex((uint)data2, offsetsTable, vertexSizesTable, out var batchIdx2, out var vertexIdx2);
					ref Vector3 reference = ref m_Model.m_Batches[batchIdx2].Vertices.RawPositions[vertexIdx2];
					reference = m_Model.m_Batches[batchIdx].Vertices.RawPositions[vertexIdx];
					ref Vector4b reference2 = ref m_Model.m_Batches[batchIdx2].Vertices.SkinWeights[vertexIdx2];
					reference2 = m_Model.m_Batches[batchIdx].Vertices.SkinWeights[vertexIdx];
					ref Vector4b reference3 = ref m_Model.m_Batches[batchIdx2].Vertices.SkinBindings[vertexIdx2];
					reference3 = m_Model.m_Batches[batchIdx].Vertices.SkinBindings[vertexIdx];
				}
			}
		}
		catch (IndexOutOfRangeException)
		{
			Logger.Log(new DebugLog(this, string.Format("An error \"{0}\" occured while parsing asset {1}", Resources.ModelParserError4, m_Model.AssetId, ToString())));
			throw new AvatarException(Resources.ModelParserError4);
		}
	}
}
