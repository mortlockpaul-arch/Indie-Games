using Microsoft.Xna.Framework;
using Quasar.Global.Vertex;
using Quasar.Meshes.Generic;

namespace Quasar.Meshes;

public class Grid : UserIndexVertexMesh<VertexPositionUVNormal>
{
	private Vector2 cellSize = new Vector2(4f);

	private short dimension = 128;

	public Vector2 CellSize
	{
		get
		{
			return cellSize;
		}
		set
		{
			cellSize = value;
		}
	}

	public short Dimension
	{
		get
		{
			return dimension;
		}
		set
		{
			dimension = value;
		}
	}

	public Grid(Vector2 cellSize, short dimension)
	{
		this.cellSize = cellSize;
		this.dimension = dimension;
		GenerateStructures();
		materials.Add(new Material());
	}

	public void GenerateStructures()
	{
		initMesh((dimension + 1) * (dimension + 1), dimension * dimension * 6);
		for (int i = 0; i < dimension + 1; i++)
		{
			for (int j = 0; j < dimension + 1; j++)
			{
				VertexPositionUVNormal vertexPositionUVNormal = new VertexPositionUVNormal
				{
					Position = new Vector3(((float)i - (float)dimension / 2f) * cellSize.X, 0f, ((float)j - (float)dimension / 2f) * cellSize.Y),
					Normal = Vector3.Up,
					UV = new Vector2((float)i / (float)dimension, (float)j / (float)dimension)
				};
				verticesBuffer[i * (dimension + 1) + j] = vertexPositionUVNormal;
			}
		}
		for (int k = 0; k < dimension; k++)
		{
			for (int l = 0; l < dimension; l++)
			{
				indicesBuffer[6 * (k * dimension + l)] = (short)(k * (dimension + 1) + l);
				indicesBuffer[6 * (k * dimension + l) + 1] = (short)(k * (dimension + 1) + l + 1);
				indicesBuffer[6 * (k * dimension + l) + 2] = (short)((k + 1) * (dimension + 1) + l + 1);
				indicesBuffer[6 * (k * dimension + l) + 3] = (short)(k * (dimension + 1) + l);
				indicesBuffer[6 * (k * dimension + l) + 4] = (short)((k + 1) * (dimension + 1) + l + 1);
				indicesBuffer[6 * (k * dimension + l) + 5] = (short)((k + 1) * (dimension + 1) + l);
			}
		}
	}
}
