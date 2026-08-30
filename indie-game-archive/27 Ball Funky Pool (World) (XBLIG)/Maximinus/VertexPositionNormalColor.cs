using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public struct VertexPositionNormalColor(Vector3 position, Vector3 color) : IVertexType
{
	public Vector3 Position = position;

	public Vector3 Normal = Vector3.Up;

	public Vector3 Color = color;

	public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0), new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.Color, 0));

	VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
}
