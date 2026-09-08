using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DataContent;

public class eMeshPart
{
	[ContentSerializer]
	public string Name;

	[ContentSerializer]
	public object Tag;

	[ContentSerializer]
	public CullMode Culling;

	[ContentSerializer]
	public ShaderEffect ShaderTecnique;

	[ContentSerializer]
	public ShaderOpacity Opacity;

	public uint UserFlags;

	public int PrimitiveCount;

	public int NumVertices;

	public int StartIndex;

	public int VertexOffset;

	public VertexBuffer VertexBuffer;

	public IndexBuffer IndexBuffer;

	[ContentSerializer(SharedResource = true)]
	public Effect Effect;
}
