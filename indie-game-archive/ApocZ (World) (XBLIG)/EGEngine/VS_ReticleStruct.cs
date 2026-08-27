using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public struct VS_ReticleStruct : IVertexType
{
	public Vector3 pos;

	public Vector3 data;

	public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0));

	VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

	public VS_ReticleStruct(Vector3 position, float Index, Vector2 TexCoord)
	{
		pos = Vector3.Zero;
		data = Vector3.Zero;
		pos = position;
		data.X = TexCoord.X;
		data.Y = TexCoord.Y;
		data.Z = Index;
	}
}
