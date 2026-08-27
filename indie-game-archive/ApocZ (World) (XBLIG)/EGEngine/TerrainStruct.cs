using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public struct TerrainStruct
{
	public int sizeX;

	public int sizeZ;

	public int numVertices;

	public int numPrimitives;

	public int scale;

	public int numTiles;

	public bool[,,] tileRender;

	public Vector3[] tileOffset;

	public Vector3[,,] curOffset;

	public Matrix transform;

	public IndexBuffer indexBuffer;

	public VertexBuffer vertexBuffer;

	public BoundingBox aabb;
}
