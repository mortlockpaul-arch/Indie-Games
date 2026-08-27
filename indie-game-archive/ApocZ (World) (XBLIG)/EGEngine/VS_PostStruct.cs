using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public struct VS_PostStruct : IVertexType
{
	public Vector3 position;

	public Vector3 textureCoord;

	public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0));

	VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

	public VS_PostStruct(Vector3 pos, Vector2 uv, float corner)
	{
		position = pos;
		textureCoord = Vector3.Zero;
		textureCoord.X = uv.X;
		textureCoord.Y = uv.Y;
		textureCoord.Z = corner;
	}
}
