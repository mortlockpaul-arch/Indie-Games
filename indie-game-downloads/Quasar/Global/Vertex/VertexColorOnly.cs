using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quasar.Global.Vertex;

public struct VertexColorOnly
{
	public Color Color;

	public static VertexElement[] Elements = new VertexElement[1]
	{
		new VertexElement(0, VertexElementFormat.Color, VertexElementUsage.Color, 0)
	};

	public static int SizeBytes => 4;

	public VertexColorOnly(Color color)
	{
		Color = color;
	}
}
