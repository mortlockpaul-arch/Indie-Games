using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar;
using Quasar.Global;
using Quasar.Global.Vertex;
using Quasar.Meshes.Generic;
using Quasar.Shaders;

namespace AvatarFarmOnline.Items.Game;

public class TileMesh : UserIndexVertexMesh<VertexPositionUV>
{
	private int cols;

	private int rows;

	private int tileCols;

	private int tileRows;

	private Vector2 cellSize;

	private Vector2 tileSize;

	private Vector2 textureSize;

	public int[] TileIndices;

	public TileMesh(Int2 size, Vector2 cellSize, Int2 tileCount, Texture2D source)
	{
		tileCols = tileCount.X;
		tileRows = tileCount.Y;
		tileSize = new Vector2(1f / (float)tileCount.X, 1f / (float)tileCount.Y);
		cols = size.X;
		rows = size.Y;
		this.cellSize = cellSize;
		textureSize = new Vector2(source.Width, source.Height);
		initializeBuffer();
		shader = ShaderManager.Shaders["FarmTiles"];
		Material material = new Material();
		material.Textures.Add(source);
		material.SetForcedAlpha(alpha: false);
		material.AlphaTest = 0.5f;
		materials.Add(material);
	}

	public void Resize(Int2 newSize)
	{
		cols = newSize.X;
		rows = newSize.Y;
		initializeBuffer();
	}

	protected void initializeBuffer()
	{
		int vertexCount = cols * rows * 4;
		initMesh(vertexCount, cols * rows * 6, useLongIndices: false);
		PrimitiveCount = 2 * rows * cols;
		TileIndices = new int[rows * cols];
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				int num = (i * cols + j) * 6;
				short num2 = (short)((i * cols + j) * 4);
				float x = cellSize.X * (float)j;
				float x2 = cellSize.X * (float)(j + 1);
				float z = cellSize.Y * (float)(i + 1);
				float z2 = cellSize.Y * (float)i;
				verticesBuffer[num2].Position = new Vector3(x, 0f, z);
				verticesBuffer[num2 + 2].Position = new Vector3(x2, 0f, z);
				verticesBuffer[num2 + 1].Position = new Vector3(x, 0f, z2);
				verticesBuffer[num2 + 3].Position = new Vector3(x2, 0f, z2);
				indicesBuffer[num] = num2;
				indicesBuffer[num + 1] = (short)(num2 + 1);
				indicesBuffer[num + 2] = (short)(num2 + 2);
				indicesBuffer[num + 3] = (short)(num2 + 2);
				indicesBuffer[num + 4] = (short)(num2 + 1);
				indicesBuffer[num + 5] = (short)(num2 + 3);
			}
		}
		UpdateTileData();
	}

	public void UpdateTileData()
	{
		float num = 0.1f / textureSize.X;
		float num2 = 0.1f / textureSize.Y;
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				int num3 = i * cols + j;
				int num4 = TileIndices[num3];
				int num5 = num4 % tileCols;
				int num6 = num4 / tileCols;
				verticesBuffer[4 * num3].UV = new Vector2(num + (float)num5 * tileSize.X, 0f - num2 + (float)(num6 + 1) * tileSize.Y);
				verticesBuffer[4 * num3 + 1].UV = new Vector2(num + (float)num5 * tileSize.X, num2 + (float)num6 * tileSize.Y);
				verticesBuffer[4 * num3 + 2].UV = new Vector2(0f - num + (float)(num5 + 1) * tileSize.X, 0f - num2 + (float)(num6 + 1) * tileSize.Y);
				verticesBuffer[4 * num3 + 3].UV = new Vector2(0f - num + (float)(num5 + 1) * tileSize.X, num2 + (float)num6 * tileSize.Y);
			}
		}
	}
}
