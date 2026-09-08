using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Global.Vertex;
using Quasar.Render;
using Quasar.Shaders;

namespace Quasar.Meshes;

public class TileMesh : Mesh
{
	protected VertexBuffer planeBuffer;

	protected VertexBuffer UVBuffer;

	protected IndexBuffer indexBuffer;

	private int triangleNumber;

	private int vertexNumber;

	private int cols;

	private int rows;

	private int tileCols;

	private int tileRows;

	private Vector2 cellSize;

	private Vector2 tileSize;

	private Vector2 textureSize;

	private int[] tileIndices;

	private VertexUVOnly[] tileData;

	private bool useXZCoords;

	private bool dirty = true;

	private Vector2 margin = Vector2.Zero;

	private Vector2 offset = Vector2.Zero;

	private VertexBufferBinding[] vertexBuffers = new VertexBufferBinding[2];

	public int[] TileIndices
	{
		get
		{
			return tileIndices;
		}
		set
		{
			if (value != null && value.Length >= tileIndices.Length)
			{
				Array.Copy(value, tileIndices, tileIndices.Length);
				dirty = true;
			}
		}
	}

	public bool UseXZCoords
	{
		get
		{
			return useXZCoords;
		}
		set
		{
			useXZCoords = value;
			dirty = true;
		}
	}

	public Vector2 Margin
	{
		get
		{
			return margin;
		}
		set
		{
			margin = value;
			dirty = true;
		}
	}

	public Vector2 Offset
	{
		get
		{
			return offset;
		}
		set
		{
			offset = value;
			dirty = true;
		}
	}

	protected void updateBuffer()
	{
		int[] array = new int[3 * triangleNumber];
		VertexPosition[] array2 = new VertexPosition[vertexNumber];
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				int num = (i * cols + j) * 6;
				int num2 = (i * cols + j) * 4;
				float x = cellSize.X * (float)j - margin.X;
				float x2 = cellSize.X * (float)(j + 1) + margin.X;
				float num3 = cellSize.Y * (float)(rows - i - 1) - margin.Y;
				float num4 = cellSize.Y * (float)(rows - i) + margin.Y;
				if (!useXZCoords)
				{
					array2[num2].Position = new Vector3(x, num3, 0f);
					array2[num2 + 1].Position = new Vector3(x, num4, 0f);
					array2[num2 + 2].Position = new Vector3(x2, num3, 0f);
					array2[num2 + 3].Position = new Vector3(x2, num4, 0f);
					array2[num2].Position += new Vector3(offset.X, offset.Y, 0f);
					array2[num2 + 1].Position += new Vector3(offset.X, offset.Y, 0f);
					array2[num2 + 2].Position += new Vector3(offset.X, offset.Y, 0f);
					array2[num2 + 3].Position += new Vector3(offset.X, offset.Y, 0f);
				}
				else
				{
					array2[num2].Position = new Vector3(x, 0f, 0f - num3);
					array2[num2 + 1].Position = new Vector3(x, 0f, 0f - num4);
					array2[num2 + 2].Position = new Vector3(x2, 0f, 0f - num3);
					array2[num2 + 3].Position = new Vector3(x2, 0f, 0f - num4);
					array2[num2].Position += new Vector3(offset.X, 0f, offset.Y);
					array2[num2 + 1].Position += new Vector3(offset.X, 0f, offset.Y);
					array2[num2 + 2].Position += new Vector3(offset.X, 0f, offset.Y);
					array2[num2 + 3].Position += new Vector3(offset.X, 0f, offset.Y);
				}
				array[num] = num2;
				array[num + 1] = num2 + 1;
				array[num + 2] = num2 + 2;
				array[num + 3] = num2 + 2;
				array[num + 4] = num2 + 1;
				array[num + 5] = num2 + 3;
			}
		}
		planeBuffer.SetData(array2);
		indexBuffer.SetData(array);
	}

	protected void initializeBuffer()
	{
		vertexNumber = 4 * cols * rows;
		triangleNumber = 2 * cols * rows;
		tileIndices = new int[rows * cols];
		tileData = new VertexUVOnly[vertexNumber];
		planeBuffer = new VertexBuffer(Engine.Device, typeof(VertexPosition), vertexNumber, BufferUsage.WriteOnly);
		indexBuffer = new IndexBuffer(Engine.Device, typeof(int), 3 * triangleNumber, BufferUsage.WriteOnly);
		UVBuffer = new VertexBuffer(Engine.Device, typeof(VertexUVOnly), vertexNumber, BufferUsage.WriteOnly);
		updateBuffer();
		UpdateTileData();
		ref VertexBufferBinding reference = ref vertexBuffers[0];
		reference = planeBuffer;
		ref VertexBufferBinding reference2 = ref vertexBuffers[1];
		reference2 = UVBuffer;
	}

	public void UpdateTileData()
	{
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				int num3 = i * cols + j;
				int num4 = tileIndices[num3];
				int num5 = num4 % tileCols;
				int num6 = num4 / tileCols;
				tileData[4 * num3].UV = new Vector2(num + (float)num5 * tileSize.X, num2 + (float)(num6 + 1) * tileSize.Y - 0.002f);
				tileData[4 * num3 + 1].UV = new Vector2(num + (float)num5 * tileSize.X, num2 + (float)num6 * tileSize.Y);
				tileData[4 * num3 + 2].UV = new Vector2(num + (float)(num5 + 1) * tileSize.X - 0.002f, num2 + (float)(num6 + 1) * tileSize.Y - 0.002f);
				tileData[4 * num3 + 3].UV = new Vector2(num + (float)(num5 + 1) * tileSize.X - 0.002f, num2 + (float)num6 * tileSize.Y);
			}
		}
		UVBuffer.SetData(tileData);
	}

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
		shader = ShaderManager.Shaders["Tile"];
		Material item = new Material
		{
			Textures = { (Texture)source }
		};
		materials.Add(item);
	}

	public override void Render(Transform motion)
	{
		if (dirty)
		{
			updateBuffer();
			UpdateTileData();
			dirty = false;
		}
		SceneRenderData.CurrentRenderData.Sorter.Add(this, motion, 0);
	}

	public override void RenderMaterial(Transform motion, int material)
	{
		shader.BeginRender(motion, materials[material]);
		Engine.Device.SetVertexBuffers(vertexBuffers);
		Engine.Device.Indices = indexBuffer;
		int num = shader.PassNumber(materials[material]);
		for (int i = 0; i < num; i++)
		{
			shader.ApplyPass(i);
			Engine.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexNumber, 0, triangleNumber);
		}
		shader.EndRender();
	}

	public override void Dispose()
	{
		if (planeBuffer != null)
		{
			planeBuffer.Dispose();
		}
		planeBuffer = null;
		if (UVBuffer != null)
		{
			UVBuffer.Dispose();
		}
		UVBuffer = null;
		if (indexBuffer != null)
		{
			indexBuffer.Dispose();
		}
		indexBuffer = null;
		base.Dispose();
	}
}
