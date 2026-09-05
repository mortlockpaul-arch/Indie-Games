using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Renderer;

public struct VertexPositionColoredNBTTextured : IVertexType
{
	private Vector3 vertexPosition;

	private Color vertexColor;

	private Vector3 vertexNormal;

	private Vector3 vertexBiNormal;

	private Vector3 vertexTangent;

	private Vector2 vertexTextureCoordinate;

	public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0), new VertexElement(16, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0), new VertexElement(28, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0), new VertexElement(40, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0), new VertexElement(52, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0));

	public static int SizeInBytes => 60;

	VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

	public Vector3 Position
	{
		get
		{
			return vertexPosition;
		}
		set
		{
			vertexPosition = value;
		}
	}

	public Color Color
	{
		get
		{
			return vertexColor;
		}
		set
		{
			vertexColor = value;
		}
	}

	public Vector3 Normal
	{
		get
		{
			return vertexNormal;
		}
		set
		{
			vertexNormal = value;
		}
	}

	public Vector3 Binormal
	{
		get
		{
			return vertexBiNormal;
		}
		set
		{
			vertexBiNormal = value;
		}
	}

	public Vector3 Tangent
	{
		get
		{
			return vertexTangent;
		}
		set
		{
			vertexTangent = value;
		}
	}

	public Vector2 TexCoord
	{
		get
		{
			return vertexTextureCoordinate;
		}
		set
		{
			vertexTextureCoordinate = value;
		}
	}

	public VertexPositionColoredNBTTextured(Vector3 pos, Color color, Vector3 normal, Vector3 binormal, Vector3 tangent, Vector2 texCoord)
	{
		vertexPosition = pos;
		vertexColor = color;
		vertexNormal = normal;
		vertexBiNormal = binormal;
		vertexTangent = tangent;
		vertexTextureCoordinate = texCoord;
	}
}
