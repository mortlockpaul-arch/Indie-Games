using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace EGEngine;

public struct VS_PosTexStruct : IVertexType
{
	public Vector3 position;

	public HalfVector2 textureCoord;

	public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.HalfVector2, VertexElementUsage.TextureCoordinate, 0));

	VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

	public VS_PosTexStruct(Vector3 pos, HalfVector2 uv)
	{
		position = pos;
		textureCoord = uv;
	}
}
