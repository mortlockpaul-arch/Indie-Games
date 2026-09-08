using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global.Vertex;

namespace Quasar.Particles;

public struct BillboardVertex : IPositionVertex, IVertexType
{
	public Vector3 Position;

	public Vector3 ParticleData;

	public Vector2 UV;

	public Vector3 Color;

	public float Alpha;

	public int Tile;

	public static VertexElement[] Elements = new VertexElement[5]
	{
		new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
		new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.PointSize, 0),
		new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
		new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
		new VertexElement(48, VertexElementFormat.Short2, VertexElementUsage.TextureCoordinate, 1)
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

	public BillboardVertex(Vector3 position, Vector3 color, float alpha, Vector2 UV, Vector3 particleData, int tile)
	{
		Position = position;
		Color = color;
		Alpha = alpha;
		ParticleData = particleData;
		this.UV = UV;
		Tile = tile;
	}
}
