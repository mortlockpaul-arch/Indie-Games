using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RenegadeEngine;

public struct VertexColorInstanceWorld(Vector4 diffuseColor, Matrix world) : IVertexType
{
	public Vector4 DiffuseColor = diffuseColor;

	public Matrix World = world;

	public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Color, 0), new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0), new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1), new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2), new VertexElement(64, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3));

	VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
}
