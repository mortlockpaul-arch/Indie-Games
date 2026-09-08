using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global.Vertex;

namespace Quasar.Animation;

public struct SpriteAnimationVertex : IPositionVertex, IVertexType
{
	public Vector3 Position;

	public Vector2 UV;

	public int Tile;

	public static VertexElement[] Elements = new VertexElement[3]
	{
		new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
		new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
		new VertexElement(20, VertexElementFormat.Short2, VertexElementUsage.TextureCoordinate, 1)
	};

	private static VertexDeclaration declaration = null;

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

	public SpriteAnimationVertex(Vector3 position, Vector2 UV, int tile)
	{
		Position = position;
		this.UV = UV;
		Tile = tile;
	}
}
