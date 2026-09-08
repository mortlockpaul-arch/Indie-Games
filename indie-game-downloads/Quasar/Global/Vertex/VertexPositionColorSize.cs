using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quasar.Global.Vertex;

public struct VertexPositionColorSize
{
	public Vector3 Position;

	public float Size;

	public Color Color;

	public static VertexElement[] Elements = new VertexElement[3]
	{
		new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
		new VertexElement(12, VertexElementFormat.Single, VertexElementUsage.PointSize, 0),
		new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 0)
	};

	public static int SizeBytes => 20;

	public VertexPositionColorSize(Vector3 position, Color color, float size)
	{
		Position = position;
		Color = color;
		Size = size;
	}
}
