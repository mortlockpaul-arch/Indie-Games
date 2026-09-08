using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Global.Vertex;

namespace Quasar.Meshes.Generic;

public class VertexMesh<VertexStruct> : GenericMesh<VertexStruct> where VertexStruct : struct, IPositionVertex
{
	private VertexBuffer vertices;

	protected new void initMesh(int vertexCount)
	{
		base.initMesh(vertexCount);
		vertices = new VertexBuffer(Engine.Device, typeof(VertexStruct), vertexCount, BufferUsage.WriteOnly);
	}

	protected void updateMesh()
	{
		vertices.SetData(verticesBuffer);
		updateBoundingSphere(verticesBuffer.Length);
	}

	protected override void PrepareRender()
	{
		Engine.Device.SetVertexBuffer(vertices);
	}

	protected override void RenderPrimitives()
	{
		Engine.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, primitiveCount);
	}

	public override void Dispose()
	{
		if (vertices != null)
		{
			vertices.Dispose();
			vertices = null;
		}
		base.Dispose();
	}
}
