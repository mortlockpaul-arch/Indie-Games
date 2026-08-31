using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace u;

internal struct D : IVertexType
{
	public Vector3 Position;

	public Vector3 Normal;

	public Vector2 TextureCoordinate;

	public Vector2 Binormal;

	public Vector2 Tangent;

	public static readonly VertexElement[] VertexElements;

	public static readonly VertexDeclaration VertexDeclaration;

	public static int SizeInBytes => 48;

	VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

	static D()
	{
		VertexElements = new VertexElement[5]
		{
			new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
			new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
			new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
			new VertexElement(32, VertexElementFormat.Vector2, VertexElementUsage.Binormal, 0),
			new VertexElement(40, VertexElementFormat.Vector2, VertexElementUsage.Tangent, 0)
		};
		VertexDeclaration = new VertexDeclaration(VertexElements)
		{
			Name = "VertexSprite.VertexDeclaration"
		};
	}
}
