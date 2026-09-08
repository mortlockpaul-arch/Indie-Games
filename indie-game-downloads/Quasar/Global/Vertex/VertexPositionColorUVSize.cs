using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quasar.Global.Vertex;

public struct VertexPositionColorUVSize : IPositionVertex, IVertexType
{
	public Vector3 Position;

	public float Size;

	public Vector2 UV;

	public Color Color;

	public static VertexElement[] Elements = new VertexElement[4]
	{
		new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
		new VertexElement(12, VertexElementFormat.Single, VertexElementUsage.PointSize, 0),
		new VertexElement(16, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
		new VertexElement(24, VertexElementFormat.Color, VertexElementUsage.Color, 0)
	};

	private static VertexDeclaration declaration = null;

	public static int SizeBytes => 28;

	public static VertexDeclaration Declaration
	{
		get
		{
			if (declaration == null)
			{
				declaration = new VertexDeclaration(Elements);
			}
			return declaration;
		}
	}

	Vector3 IPositionVertex.Position => Position;

	public VertexDeclaration VertexDeclaration => Declaration;

	public VertexPositionColorUVSize(Vector3 position, Color color, Vector2 UV, float size)
	{
		Position = position;
		Color = color;
		Size = size;
		this.UV = UV;
	}
}
