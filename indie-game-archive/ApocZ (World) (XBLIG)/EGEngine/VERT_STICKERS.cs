using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public struct VERT_STICKERS : IVertexType
{
	public Vector3 pos;

	public Vector3 norm;

	public Vector2 texCoord;

	public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0), new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0));

	VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

	public VERT_STICKERS(Vector3 p, Vector3 n, Vector2 uv)
	{
		pos = p;
		norm = n;
		texCoord = uv;
	}
}
