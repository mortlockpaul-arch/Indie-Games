using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public struct TerrainVegitationHighResStruct
{
	public int sizeX;

	public int sizeZ;

	public int[] startVertice;

	public int[] numVertices;

	public int[] numPrimitives;

	public int scale;

	public int numBillboards;

	public int totalVertices;

	public int totalPrimitives;

	public bool[,,] tileRender;

	public Vector3[] tileOffset;

	public Vector3[,,] curOffset;

	public Matrix transform;

	public IndexBuffer indexBuffer;

	public Vector3[] vertexOffset;

	public VertexBuffer[] vertexBuffer;

	public BoundingBox aabb;
}
