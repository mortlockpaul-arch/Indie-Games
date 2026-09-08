using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Global.Vertex;

namespace Quasar.Meshes.Generic;

public class IndexVertexMesh<VertexStruct> : GenericMesh<VertexStruct> where VertexStruct : struct, IPositionVertex
{
	private VertexBuffer vertices;

	private IndexBuffer indices;

	protected short[] indicesBuffer;

	protected short[] Indices => indicesBuffer;

	protected void initMesh(int vertexCount, int indexCount)
	{
		initMesh(vertexCount);
		vertices = new VertexBuffer(Engine.Device, typeof(VertexStruct), vertexCount, BufferUsage.WriteOnly);
		indices = new IndexBuffer(Engine.Device, typeof(short), indexCount, BufferUsage.WriteOnly);
		indicesBuffer = new short[indexCount];
	}

	protected void updateMesh()
	{
		vertices.SetData(verticesBuffer);
		indices.SetData(indicesBuffer);
		updateBoundingSphere(verticesBuffer.Length);
	}

	protected override void PrepareRender()
	{
		Engine.Device.SetVertexBuffer(vertices);
		Engine.Device.Indices = indices;
	}

	protected override void RenderPrimitives()
	{
		Engine.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, verticesBuffer.Length, 0, primitiveCount);
	}

	public override void Dispose()
	{
		if (indices != null)
		{
			indices.Dispose();
			indices = null;
		}
		if (vertices != null)
		{
			vertices.Dispose();
			vertices = null;
		}
		base.Dispose();
	}
}
