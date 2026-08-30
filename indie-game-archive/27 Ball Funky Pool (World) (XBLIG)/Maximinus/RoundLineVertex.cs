using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

internal struct RoundLineVertex(Vector3 pos, Vector2 norm, Vector2 tex, float index)
{
	public Vector3 pos = pos;

	public Vector2 rhoTheta = norm;

	public Vector2 scaleTrans = tex;

	public float index = index;

	public static int SizeInBytes = 32;

	public static VertexElement[] VertexElements = new VertexElement[4]
	{
		new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
		new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.Normal, 0),
		new VertexElement(20, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
		new VertexElement(28, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1)
	};
}
