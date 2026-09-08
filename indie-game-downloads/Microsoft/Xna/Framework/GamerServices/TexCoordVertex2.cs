using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.GamerServices;

internal struct TexCoordVertex2 : IVertexType
{
	public const int SizeInBytes = 8;

	public Vector2 texCoord;

	private static readonly VertexElement[] vertexElements = new VertexElement[1]
	{
		new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1)
	};

	private static VertexDeclaration declaration = new VertexDeclaration(vertexElements);

	VertexDeclaration IVertexType.VertexDeclaration => declaration;

	public TexCoordVertex2(Vector2 texCoord)
	{
		this.texCoord = texCoord;
	}
}
