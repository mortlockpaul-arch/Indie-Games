using Microsoft.Xna.Framework.Graphics;

namespace GKEngine.Entities;

public class ModelPart
{
	public string name;

	public int triangleCount;

	public int vertexCount;

	public int vertexStride;

	public VertexDeclaration vertexDeclaration;

	public VertexBuffer vertexBuffer;

	public IndexBuffer indexBuffer;

	public Effect effect;

	public virtual void Dispose()
	{
		effect.Dispose();
		vertexBuffer.Dispose();
		vertexDeclaration.Dispose();
		indexBuffer.Dispose();
	}

	public virtual void Flush()
	{
		effect.Dispose();
	}
}
