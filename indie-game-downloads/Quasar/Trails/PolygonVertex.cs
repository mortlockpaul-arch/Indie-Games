using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global.Vertex;

namespace Quasar.Trails;

public struct PolygonVertex : IPositionVertex, IVertexType
{
	public Vector3 Position;

	public Vector2 TrailData;

	public Vector2 UV;

	public Vector3 Color;

	public float Alpha;

	public Vector3 Normal;

	public static VertexElement[] Elements = new VertexElement[5]
	{
		new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
		new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.PointSize, 0),
		new VertexElement(20, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
		new VertexElement(28, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
		new VertexElement(44, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 1)
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

	public PolygonVertex(Vector3 position, Vector3 color, float alpha, Vector2 UV, Vector2 trailData, Vector3 normal)
	{
		Position = position;
		Color = color;
		Alpha = alpha;
		TrailData = trailData;
		this.UV = UV;
		Normal = normal;
	}
}
