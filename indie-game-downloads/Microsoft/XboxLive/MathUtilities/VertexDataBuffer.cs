namespace Microsoft.XboxLive.MathUtilities;

public class VertexDataBuffer
{
	private Vector3[] m_Positions;

	private Vector3[] m_Normals;

	private Colorf[][] m_Colors;

	private Vector2[][] m_TextureCoordinates;

	private Vector4b[] m_SkinBindings;

	private Vector4b[] m_SkinWeights;

	private Vector3[] m_RawPositions;

	private Vector3[] m_RawNormals;

	public Vector3[] Positions
	{
		get
		{
			return m_Positions;
		}
		set
		{
			m_Positions = value;
		}
	}

	public Vector3[] Normals
	{
		get
		{
			return m_Normals;
		}
		set
		{
			m_Normals = value;
		}
	}

	public Vector4b[] SkinWeights
	{
		get
		{
			return m_SkinWeights;
		}
		set
		{
			m_SkinWeights = value;
		}
	}

	public Vector4b[] SkinBindings
	{
		get
		{
			return m_SkinBindings;
		}
		set
		{
			m_SkinBindings = value;
		}
	}

	public Vector3[] RawPositions
	{
		get
		{
			return m_RawPositions;
		}
		set
		{
			m_RawPositions = value;
		}
	}

	public Vector3[] RawNormals
	{
		get
		{
			return m_RawNormals;
		}
		set
		{
			m_RawNormals = value;
		}
	}

	public int TextureChannelCount => m_TextureCoordinates.Length;

	public int ColorChannelCount => m_Colors.Length;

	public Vector2[][] TextureCoordinates
	{
		get
		{
			return m_TextureCoordinates;
		}
		set
		{
			m_TextureCoordinates = value;
		}
	}

	public Vector2[] TextureCoordinateChannel0
	{
		get
		{
			return m_TextureCoordinates[0];
		}
		set
		{
			m_TextureCoordinates[0] = value;
		}
	}

	public Vector2[] TextureCoordinateChannel1
	{
		get
		{
			return m_TextureCoordinates[1];
		}
		set
		{
			m_TextureCoordinates[1] = value;
		}
	}

	public Vector2[] TextureCoordinateChannel2
	{
		get
		{
			return m_TextureCoordinates[2];
		}
		set
		{
			m_TextureCoordinates[2] = value;
		}
	}

	public Vector2[] TextureCoordinateChannel3
	{
		get
		{
			return m_TextureCoordinates[3];
		}
		set
		{
			m_TextureCoordinates[3] = value;
		}
	}

	public Vector2[] TextureCoordinateChannel4
	{
		get
		{
			return m_TextureCoordinates[4];
		}
		set
		{
			m_TextureCoordinates[4] = value;
		}
	}

	public Vector2[] TextureCoordinateChannel5
	{
		get
		{
			return m_TextureCoordinates[5];
		}
		set
		{
			m_TextureCoordinates[5] = value;
		}
	}

	public Vector2[] TextureCoordinateChannel6
	{
		get
		{
			return m_TextureCoordinates[6];
		}
		set
		{
			m_TextureCoordinates[6] = value;
		}
	}

	public Vector2[] TextureCoordinateChannel7
	{
		get
		{
			return m_TextureCoordinates[7];
		}
		set
		{
			m_TextureCoordinates[7] = value;
		}
	}

	public Colorf[] Color0
	{
		get
		{
			return m_Colors[0];
		}
		set
		{
			m_Colors[0] = value;
		}
	}

	public Colorf[] Color1
	{
		get
		{
			return m_Colors[1];
		}
		set
		{
			m_Colors[1] = value;
		}
	}

	public Colorf[] Color2
	{
		get
		{
			return m_Colors[2];
		}
		set
		{
			m_Colors[2] = value;
		}
	}

	public Colorf[] Color3
	{
		get
		{
			return m_Colors[3];
		}
		set
		{
			m_Colors[3] = value;
		}
	}

	public VertexDataBuffer()
	{
		m_TextureCoordinates = new Vector2[8][];
		m_Colors = new Colorf[8][];
	}

	public VertexDataBuffer Clone()
	{
		int textureChannelCount = TextureChannelCount;
		int colorChannelCount = ColorChannelCount;
		VertexDataBuffer vertexDataBuffer = new VertexDataBuffer(textureChannelCount, colorChannelCount);
		if (m_Positions != null)
		{
			vertexDataBuffer.m_Positions = new Vector3[m_Positions.Length];
			m_Positions.CopyTo(vertexDataBuffer.m_Positions, 0);
		}
		for (int i = 0; i < textureChannelCount; i++)
		{
			if (m_TextureCoordinates[i] != null)
			{
				vertexDataBuffer.m_TextureCoordinates[i] = new Vector2[m_TextureCoordinates[i].Length];
				m_TextureCoordinates[i].CopyTo(vertexDataBuffer.m_TextureCoordinates[i], 0);
			}
		}
		for (int j = 0; j < colorChannelCount; j++)
		{
			if (m_Colors[j] != null)
			{
				vertexDataBuffer.m_Colors[j] = new Colorf[m_Colors[j].Length];
				m_Colors[j].CopyTo(vertexDataBuffer.m_Colors[j], 0);
			}
		}
		if (m_Normals != null)
		{
			vertexDataBuffer.m_Normals = new Vector3[m_Normals.Length];
			m_Normals.CopyTo(vertexDataBuffer.m_Normals, 0);
		}
		if (m_RawPositions != null)
		{
			vertexDataBuffer.m_RawPositions = new Vector3[m_RawPositions.Length];
			m_RawPositions.CopyTo(vertexDataBuffer.m_RawPositions, 0);
		}
		if (m_RawNormals != null)
		{
			vertexDataBuffer.m_RawNormals = new Vector3[m_RawNormals.Length];
			m_RawNormals.CopyTo(vertexDataBuffer.m_RawNormals, 0);
		}
		if (m_SkinBindings != null)
		{
			vertexDataBuffer.m_SkinBindings = new Vector4b[m_SkinBindings.Length];
			m_SkinBindings.CopyTo(vertexDataBuffer.m_SkinBindings, 0);
		}
		if (m_SkinWeights != null)
		{
			vertexDataBuffer.m_SkinWeights = new Vector4b[m_SkinWeights.Length];
			m_SkinWeights.CopyTo(vertexDataBuffer.m_SkinWeights, 0);
		}
		return vertexDataBuffer;
	}

	public VertexDataBuffer(int textureStreamsCount, int colorStreamsCount)
	{
		m_TextureCoordinates = new Vector2[textureStreamsCount][];
		m_Colors = new Colorf[colorStreamsCount][];
	}

	public Vector2[] GetTextureChannelByIndex(int index)
	{
		return m_TextureCoordinates[index];
	}

	public void SetTextureChannelByIndex(int index, Vector2[] textureCoordinates)
	{
		m_TextureCoordinates[index] = textureCoordinates;
	}

	public Colorf[] GetColors(int index)
	{
		return m_Colors[index];
	}

	public void SetColors(int index, Colorf[] colors)
	{
		m_Colors[index] = colors;
	}

	public void AllocateVertexCount(int vertexCount)
	{
		int textureChannelCount = TextureChannelCount;
		int colorChannelCount = ColorChannelCount;
		m_Positions = new Vector3[vertexCount];
		m_Normals = new Vector3[vertexCount];
		for (int i = 0; i < colorChannelCount; i++)
		{
			m_Colors[i] = new Colorf[vertexCount];
		}
		for (int j = 0; j < textureChannelCount; j++)
		{
			m_TextureCoordinates[j] = new Vector2[vertexCount];
		}
		m_SkinBindings = new Vector4b[vertexCount];
		m_SkinWeights = new Vector4b[vertexCount];
		m_RawPositions = new Vector3[vertexCount];
		m_RawNormals = new Vector3[vertexCount];
	}
}
