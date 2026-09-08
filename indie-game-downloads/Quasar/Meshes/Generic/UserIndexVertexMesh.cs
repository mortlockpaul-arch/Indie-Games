using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Global.Vertex;

namespace Quasar.Meshes.Generic;

public abstract class UserIndexVertexMesh<VertexStruct> : GenericMesh<VertexStruct> where VertexStruct : struct, IPositionVertex
{
	protected int[] longIndicesBuffer;

	protected short[] indicesBuffer;

	private bool useLongIndices;

	protected bool hasSimpleLayout;

	public int[] LongIndices => longIndicesBuffer;

	public short[] Indices => indicesBuffer;

	public UserIndexVertexMesh()
	{
	}

	protected void initMesh(int vertexCount, int indexCount)
	{
		initMesh(vertexCount, indexCount, useLongIndices: false);
	}

	protected void initMesh(int vertexCount, int indexCount, bool useLongIndices)
	{
		initMesh(vertexCount);
		this.useLongIndices = useLongIndices;
		if (useLongIndices)
		{
			longIndicesBuffer = new int[indexCount];
		}
		else
		{
			indicesBuffer = new short[indexCount];
		}
	}

	protected override void RenderPrimitives()
	{
		int numVertices = (hasSimpleLayout ? (4 * (primitiveCount / 2) + 3 * (primitiveCount % 2)) : verticesBuffer.Length);
		if (useLongIndices)
		{
			Engine.Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, verticesBuffer, 0, numVertices, longIndicesBuffer, 0, primitiveCount);
		}
		else
		{
			Engine.Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, verticesBuffer, 0, numVertices, indicesBuffer, 0, primitiveCount);
		}
	}
}
