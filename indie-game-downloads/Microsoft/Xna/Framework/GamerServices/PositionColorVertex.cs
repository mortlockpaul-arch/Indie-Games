using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.GamerServices;

internal struct PositionColorVertex : IVertexType
{
	public const int SizeInBytes = 16;

	public Vector3 position;

	public Color color;

	private static readonly VertexElement[] vertexElements = new VertexElement[2]
	{
		new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
		new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0)
	};

	private static VertexDeclaration declaration = new VertexDeclaration(vertexElements);

	VertexDeclaration IVertexType.VertexDeclaration => declaration;

	public PositionColorVertex(Vector3 position, Color color)
	{
		declaration = new VertexDeclaration(vertexElements);
		this.position = position;
		this.color = color;
	}
}
