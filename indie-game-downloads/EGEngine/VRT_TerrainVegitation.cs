using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public struct VRT_TerrainVegitation : IVertexType
{
	public Vector3 Position;

	public Color Texcoord;

	public static readonly VertexDeclaration VertexDeclaration;

	VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

	public static int SizeInBytes => 16;

	static VRT_TerrainVegitation()
	{
		VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.TextureCoordinate, 0));
	}
}
