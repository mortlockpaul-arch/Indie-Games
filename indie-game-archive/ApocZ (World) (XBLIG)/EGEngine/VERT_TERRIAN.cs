using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public struct VERT_TERRIAN : IVertexType
{
	public Vector3 Position;

	public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0));

	VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

	public static int SizeInBytes => 12;
}
