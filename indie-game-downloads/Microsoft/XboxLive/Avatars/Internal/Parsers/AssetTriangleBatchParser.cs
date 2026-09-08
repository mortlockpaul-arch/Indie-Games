using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal.Parsers;

public class AssetTriangleBatchParser
{
	public class TriangleIndicesParser
	{
		public IntegerDataUnpacker m_idx = new IntegerDataUnpacker(16);

		public void UnpackHeader(BitStream bitStream)
		{
			m_idx.UnpackHeader(bitStream);
		}

		public void UnpackDataRightHanded(BitStream bitStream, out IndexedTriangle data)
		{
			data = default(IndexedTriangle);
			m_idx.UnpackData(bitStream, out data.i1);
			m_idx.UnpackData(bitStream, out data.i2);
			m_idx.UnpackData(bitStream, out data.i3);
		}

		public void UnpackDataLeftHanded(BitStream bitStream, out IndexedTriangle data)
		{
			data = default(IndexedTriangle);
			m_idx.UnpackData(bitStream, out data.i1);
			m_idx.UnpackData(bitStream, out data.i3);
			m_idx.UnpackData(bitStream, out data.i2);
		}
	}

	public class BatchStreamParser
	{
		public uint m_uvcnt;

		public Vector3dDataUnpacker m_position = new Vector3dDataUnpacker();

		public IntegerDataUnpacker m_normal = new IntegerDataUnpacker();

		public IntegerDataUnpacker m_weights = new IntegerDataUnpacker();

		public IntegerDataUnpacker m_bindings = new IntegerDataUnpacker();

		public IntegerDataUnpacker m_color = new IntegerDataUnpacker();

		public SmallDataUnpacker<short>[] m_U;

		public SmallDataUnpacker<short>[] m_V;

		public BatchStreamParser(uint UVscnt)
		{
			m_uvcnt = UVscnt;
			m_U = new SmallDataUnpacker<short>[m_uvcnt];
			m_V = new SmallDataUnpacker<short>[m_uvcnt];
		}

		public void UnpackHeader(BitStream bitStream)
		{
			m_position.UnpackHeader(bitStream);
			m_normal.UnpackHeader(bitStream);
			m_weights.UnpackHeader(bitStream);
			m_bindings.UnpackHeader(bitStream);
			m_color.UnpackHeader(bitStream);
			for (int i = 0; i < m_uvcnt; i++)
			{
				m_U[i] = new SmallDataUnpacker<short>(2);
				m_V[i] = new SmallDataUnpacker<short>(2);
				m_U[i].UnpackHeader(bitStream);
				m_V[i].UnpackHeader(bitStream);
			}
		}

		public static Colorf Int2Colorf(int i)
		{
			return new Colorf
			{
				blue = (float)(i & 0xFF) / 255f,
				green = (float)((i >> 8) & 0xFF) / 255f,
				red = (float)((i >> 16) & 0xFF) / 255f,
				alpha = (float)((i >> 24) & 0xFF) / 255f
			};
		}

		public static float HalfToFloat(ushort x)
		{
			uint num = (uint)(x & 0x7C00);
			uint num2 = (uint)(x & 0x3FF);
			switch (num)
			{
			case 0u:
				if (num2 != 0)
				{
					num = 947912704u;
					do
					{
						num -= 8388608;
						num2 <<= 1;
					}
					while ((num2 & 0x400) == 0);
					num2 &= 0x3FF;
				}
				break;
			case 31744u:
				num = 2139095040u;
				break;
			default:
				num = (num << 13) + 939524096;
				break;
			}
			EndianSwapper endianSwapper = new EndianSwapper
			{
				aUint = ((uint)((x & 0x8000) << 16) | num | (num2 << 13))
			};
			return endianSwapper.aFloat;
		}

		public void UnpackDataRightHanded(BitStream bitStream, ref VertexDataBuffer data, int idx)
		{
			m_position.UnpackData(bitStream, out data.RawPositions[idx]);
			m_normal.UnpackData(bitStream, out var data2);
			m_weights.UnpackData(bitStream, out var data3);
			m_bindings.UnpackData(bitStream, out var data4);
			m_color.UnpackData(bitStream, out var data5);
			for (int i = 0; i < m_uvcnt; i++)
			{
				m_U[i].UnpackData(bitStream, out short data6);
				m_V[i].UnpackData(bitStream, out short data7);
				data.TextureCoordinates[i][idx].X = HalfToFloat((ushort)data6);
				data.TextureCoordinates[i][idx].Y = HalfToFloat((ushort)data7);
			}
			ref Vector3 reference = ref data.RawNormals[idx];
			reference = VectorMath.CreateFromPackedNormal(data2);
			ref Vector4b reference2 = ref data.SkinBindings[idx];
			reference2 = new Vector4b(data4);
			ref Vector4b reference3 = ref data.SkinWeights[idx];
			reference3 = new Vector4b(data3);
			ref Colorf reference4 = ref data.Color0[idx];
			reference4 = Int2Colorf(data5);
		}

		public void UnpackDataLeftHanded(BitStream bitStream, ref VertexDataBuffer data, int idx)
		{
			m_position.UnpackData(bitStream, out data.RawPositions[idx]);
			m_normal.UnpackData(bitStream, out var data2);
			m_weights.UnpackData(bitStream, out var data3);
			m_bindings.UnpackData(bitStream, out var data4);
			m_color.UnpackData(bitStream, out var data5);
			for (int i = 0; i < m_uvcnt; i++)
			{
				m_U[i].UnpackData(bitStream, out short data6);
				m_V[i].UnpackData(bitStream, out short data7);
				data.TextureCoordinates[i][idx].X = HalfToFloat((ushort)data6);
				data.TextureCoordinates[i][idx].Y = HalfToFloat((ushort)data7);
			}
			ref Vector3 reference = ref data.RawNormals[idx];
			reference = VectorMath.CreateFromPackedNormalFlipCoordinates(data2);
			ref Vector4b reference2 = ref data.SkinBindings[idx];
			reference2 = new Vector4b(data4);
			ref Vector4b reference3 = ref data.SkinWeights[idx];
			reference3 = new Vector4b(data3);
			ref Colorf reference4 = ref data.Color0[idx];
			reference4 = Int2Colorf(data5);
		}

		public void FlipCoordinateSystem()
		{
			m_position.InvertCoordinateSystem();
		}
	}

	public TriangleBatch m_Batch;

	public ShaderInstance m_Shader;

	public CoordinateSystem m_CoordinateSystem;

	public uint m_vertexGpuOfs;

	public uint m_vertexSize;

	public TriangleBatch TriangleBatch => m_Batch;

	public ShaderInstance ShaderInstance => m_Shader;

	public uint VertexGpuOffset => m_vertexGpuOfs;

	public uint VertexSize => m_vertexSize;

	public AssetTriangleBatchParser(CoordinateSystem coordinateSystem)
	{
		m_Batch = default(TriangleBatch);
		m_Shader = default(ShaderInstance);
		m_CoordinateSystem = coordinateSystem;
	}

	public void Parse(EndianStream stream)
	{
		BitStream bitStream = new BitStream(stream);
		m_Shader.ShaderId = (ShaderId)bitStream.ReadInt(32);
		uint num = bitStream.ReadUint(5);
		uint num2 = bitStream.ReadUint(32);
		uint num3 = bitStream.ReadUint(32);
		uint num4 = bitStream.ReadUint(32);
		uint num5 = bitStream.ReadUint(32);
		bitStream.ReadUint(32);
		uint vertexGpuOfs = bitStream.ReadUint(32);
		bitStream.ReadUint(32);
		if (num4 > 6)
		{
			Logger.Log(new DebugLog(this, Resources.TriangleParserError1));
			throw new AvatarException(Resources.TriangleParserError1);
		}
		if (num2 > 8192)
		{
			Logger.Log(new DebugLog(this, Resources.TriangleParserError2));
			throw new AvatarException(Resources.TriangleParserError2);
		}
		if (num3 > 8192)
		{
			Logger.Log(new DebugLog(this, Resources.TriangleParserError3));
			throw new AvatarException(Resources.TriangleParserError3);
		}
		m_vertexGpuOfs = vertexGpuOfs;
		m_vertexSize = num5;
		if (num5 != 32 + 4 * (num4 - 1))
		{
			Logger.Log(new DebugLog(this, Resources.TriangleParserError4));
			throw new AvatarException(Resources.TriangleParserError4);
		}
		m_Batch.Triangles = new IndexedTriangle[num2];
		m_Batch.Vertices = new VertexDataBuffer();
		m_Shader.ShaderParameters = new ShaderParameter[num];
		for (int i = 0; i < num; i++)
		{
			ReadShaderParameter(stream, i);
		}
		UnpackVertices(stream, num3, num4);
		UnpackTriangleData(stream, num2);
	}

	public void UnpackTriangleData(EndianStream stream, uint triangleCount)
	{
		BitStream bitStream = new BitStream(stream);
		int num = bitStream.ReadInt(32);
		if (num != 3 * triangleCount)
		{
			Logger.Log(new DebugLog(this, Resources.TriangleParserError5));
			throw new AvatarException(Resources.TriangleParserError5);
		}
		TriangleIndicesParser triangleIndicesParser = new TriangleIndicesParser();
		triangleIndicesParser.UnpackHeader(bitStream);
		m_Batch.Triangles = new IndexedTriangle[triangleCount];
		if (m_CoordinateSystem == CoordinateSystem.RightHanded)
		{
			for (uint num2 = 0u; num2 < triangleCount; num2++)
			{
				triangleIndicesParser.UnpackDataRightHanded(bitStream, out m_Batch.Triangles[num2]);
			}
		}
		else
		{
			for (uint num3 = 0u; num3 < triangleCount; num3++)
			{
				triangleIndicesParser.UnpackDataLeftHanded(bitStream, out m_Batch.Triangles[num3]);
			}
		}
	}

	public void UnpackVertices(EndianStream stream, uint VertexCount, uint VertexUvCount)
	{
		BitStream bitStream = new BitStream(stream);
		uint num = bitStream.ReadUint(32);
		if (num != VertexCount)
		{
			Logger.Log(new DebugLog(this, Resources.TriangleParserError6));
			throw new AvatarException(Resources.TriangleParserError6);
		}
		BatchStreamParser batchStreamParser = new BatchStreamParser(VertexUvCount);
		batchStreamParser.UnpackHeader(bitStream);
		m_Batch.Vertices = new VertexDataBuffer((int)VertexUvCount, 1);
		m_Batch.Vertices.AllocateVertexCount((int)VertexCount);
		if (m_CoordinateSystem == CoordinateSystem.RightHanded)
		{
			for (uint num2 = 0u; num2 < num; num2++)
			{
				batchStreamParser.UnpackDataRightHanded(bitStream, ref m_Batch.Vertices, (int)num2);
			}
			return;
		}
		batchStreamParser.FlipCoordinateSystem();
		for (uint num3 = 0u; num3 < num; num3++)
		{
			batchStreamParser.UnpackDataLeftHanded(bitStream, ref m_Batch.Vertices, (int)num3);
		}
	}

	public void ReadShaderParameter(EndianStream stream, int index)
	{
		bool littleEndian = stream.LittleEndian;
		stream.LittleEndian = true;
		m_Shader.ShaderParameters[index].type = (ShaderParameterType)stream.ReadInt();
		m_Shader.ShaderParameters[index].usage = (ShaderParameterUsage)stream.ReadInt();
		if (m_Shader.ShaderParameters[index].type == ShaderParameterType.Texture)
		{
			m_Shader.ShaderParameters[index].data.Texture.TextureIndex = stream.ReadShort();
			m_Shader.ShaderParameters[index].data.Texture.UvLayer = stream.ReadShort();
			m_Shader.ShaderParameters[index].data.Texture.textureWrapMode = (TextureWrapModes)stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
		}
		else
		{
			if (m_Shader.ShaderParameters[index].type != ShaderParameterType.PixelConstant && m_Shader.ShaderParameters[index].type == ShaderParameterType.VertexConstant)
			{
				Logger.Log(new DebugLog(this, Resources.TriangleParserError7));
				throw new AvatarException(Resources.TriangleParserError7);
			}
			m_Shader.ShaderParameters[index].data.ConstantInt.i0 = stream.ReadInt();
			m_Shader.ShaderParameters[index].data.ConstantInt.i1 = stream.ReadInt();
			m_Shader.ShaderParameters[index].data.ConstantInt.i2 = stream.ReadInt();
			m_Shader.ShaderParameters[index].data.ConstantInt.i3 = stream.ReadInt();
		}
		stream.LittleEndian = littleEndian;
	}
}
