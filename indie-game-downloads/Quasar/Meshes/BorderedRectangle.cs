using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Global.Vertex;
using Quasar.Meshes.Generic;
using Quasar.Shaders;

namespace Quasar.Meshes;

public class BorderedRectangle : UserIndexVertexMesh<VertexPositionUV>
{
	private float[] coordBorderSize = new float[4];

	private float[] uvBorderSize = new float[4];

	private float zValue;

	private bool dirty;

	private bool useXZCoords;

	private Vector2 size;

	private Vector2 offset = Vector2.Zero;

	private static short[] indices = new short[54]
	{
		0, 1, 2, 2, 1, 3, 1, 5, 3, 1,
		4, 5, 4, 6, 5, 6, 7, 5, 7, 8,
		5, 7, 9, 8, 9, 11, 8, 9, 10, 11,
		8, 13, 12, 8, 11, 13, 12, 14, 15, 12,
		13, 14, 3, 15, 2, 3, 12, 15, 5, 12,
		3, 5, 8, 12
	};

	private static short[] flippedIndices = new short[54]
	{
		0, 2, 1, 2, 3, 1, 1, 3, 5, 1,
		5, 4, 4, 5, 6, 6, 5, 7, 7, 5,
		8, 7, 8, 9, 9, 8, 11, 9, 11, 10,
		8, 12, 13, 8, 13, 11, 12, 15, 14, 12,
		14, 13, 3, 2, 15, 3, 15, 12, 5, 3,
		12, 5, 12, 8
	};

	public Vector2 Size
	{
		get
		{
			return size;
		}
		set
		{
			size = value;
			dirty = true;
		}
	}

	public Layout2D.LayoutData Layout
	{
		get
		{
			return new Layout2D.LayoutData(offset, size);
		}
		set
		{
			size = value.size;
			offset = value.position;
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

	public float ZValue
	{
		get
		{
			return zValue;
		}
		set
		{
			zValue = value;
			dirty = true;
		}
	}

	public bool RemoveInnerRectangle
	{
		set
		{
			base.PrimitiveCount = (value ? 16 : 18);
		}
	}

	private void fillBuffer()
	{
		generateVertices();
		updateBoundingSphere(16);
		dirty = false;
	}

	private void initMesh()
	{
		initMesh(16, 54);
		base.PrimitiveCount = 18;
		indices.CopyTo(indicesBuffer, 0);
		fillBuffer();
	}

	private void generateVertices()
	{
		float x = (0f - size.X) / 2f + offset.X;
		float x2 = Math.Min(0f, (0f - size.X) / 2f + coordBorderSize[0]) + offset.X;
		float x3 = size.X / 2f + offset.X;
		float x4 = Math.Max(0f, size.X / 2f - coordBorderSize[2]) + offset.X;
		float y = (0f - size.Y) / 2f + offset.Y;
		float y2 = Math.Min(0f, (0f - size.Y) / 2f + coordBorderSize[3]) + offset.Y;
		float y3 = size.Y / 2f + offset.Y;
		float y4 = Math.Max(0f, size.Y / 2f - coordBorderSize[1]) + offset.Y;
		verticesBuffer[0].Position = new Vector3(x, y3, zValue);
		verticesBuffer[0].UV = new Vector2(0f, 0f);
		verticesBuffer[1].Position = new Vector3(x2, y3, zValue);
		verticesBuffer[1].UV = new Vector2(uvBorderSize[0], 0f);
		verticesBuffer[2].Position = new Vector3(x, y4, zValue);
		verticesBuffer[2].UV = new Vector2(0f, uvBorderSize[1]);
		verticesBuffer[3].Position = new Vector3(x2, y4, zValue);
		verticesBuffer[3].UV = new Vector2(uvBorderSize[0], uvBorderSize[1]);
		verticesBuffer[4].Position = new Vector3(x4, y3, zValue);
		verticesBuffer[4].UV = new Vector2(1f - uvBorderSize[2], 0f);
		verticesBuffer[5].Position = new Vector3(x4, y4, zValue);
		verticesBuffer[5].UV = new Vector2(1f - uvBorderSize[2], uvBorderSize[1]);
		verticesBuffer[6].Position = new Vector3(x3, y3, zValue);
		verticesBuffer[6].UV = new Vector2(1f, 0f);
		verticesBuffer[7].Position = new Vector3(x3, y4, zValue);
		verticesBuffer[7].UV = new Vector2(1f, uvBorderSize[1]);
		verticesBuffer[8].Position = new Vector3(x4, y2, zValue);
		verticesBuffer[8].UV = new Vector2(1f - uvBorderSize[2], 1f - uvBorderSize[3]);
		verticesBuffer[9].Position = new Vector3(x3, y2, zValue);
		verticesBuffer[9].UV = new Vector2(1f, 1f - uvBorderSize[3]);
		verticesBuffer[10].Position = new Vector3(x3, y, zValue);
		verticesBuffer[10].UV = new Vector2(1f, 1f);
		verticesBuffer[11].Position = new Vector3(x4, y, zValue);
		verticesBuffer[11].UV = new Vector2(1f - uvBorderSize[2], 1f);
		verticesBuffer[12].Position = new Vector3(x2, y2, zValue);
		verticesBuffer[12].UV = new Vector2(uvBorderSize[0], 1f - uvBorderSize[3]);
		verticesBuffer[13].Position = new Vector3(x2, y, zValue);
		verticesBuffer[13].UV = new Vector2(uvBorderSize[0], 1f);
		verticesBuffer[14].Position = new Vector3(x, y, zValue);
		verticesBuffer[14].UV = new Vector2(0f, 1f);
		verticesBuffer[15].Position = new Vector3(x, y2, zValue);
		verticesBuffer[15].UV = new Vector2(0f, 1f - uvBorderSize[3]);
		if (useXZCoords)
		{
			for (int i = 0; i < 16; i++)
			{
				verticesBuffer[i].Position = new Vector3(verticesBuffer[i].Position.X, verticesBuffer[i].Position.Z, verticesBuffer[i].Position.Y);
			}
			flippedIndices.CopyTo(indicesBuffer, 0);
		}
		else
		{
			indices.CopyTo(indicesBuffer, 0);
		}
	}

	public override void Render(Transform motion)
	{
		if (dirty)
		{
			fillBuffer();
		}
		base.Render(motion);
	}

	public BorderedRectangle(Texture2D texture, Vector2 size, float[] coordBorder)
		: this(texture, size, Vector2.Zero, coordBorder)
	{
	}

	public BorderedRectangle(Texture2D texture, Layout2D.LayoutData layoutData, float[] coordBorder)
		: this(texture, layoutData.size, layoutData.position, coordBorder)
	{
	}

	public BorderedRectangle(Texture2D texture, Vector2 size, Vector2 offset, float[] coordBorder)
	{
		this.size = size;
		this.offset = offset;
		coordBorderSize = coordBorder;
		uvBorderSize = new float[4];
		for (int i = 0; i < 4; i++)
		{
			uvBorderSize[i] = coordBorder[i] / (float)((i % 2 == 0) ? texture.Width : texture.Height);
		}
		initMesh();
		base.Materials.Add(new Material());
		base.Materials[0].Texture = texture;
		shader = ShaderManager.Shaders["GUI"];
	}

	public BorderedRectangle(Texture2D texture, Vector2 size, float[] coordBorder, float[] uvBorder)
	{
		this.size = size;
		coordBorderSize = coordBorder;
		uvBorderSize = new float[4];
		for (int i = 0; i < 4; i++)
		{
			uvBorderSize[i] = uvBorder[i] / (float)((i % 2 == 0) ? texture.Width : texture.Height);
		}
		initMesh();
		base.Materials.Add(new Material());
		base.Materials[0].Texture = texture;
		shader = ShaderManager.Shaders["GUI"];
	}

	public BorderedRectangle(Texture2D texture, Vector2 size, Vector2 offset, float[] coordBorder, float[] uvBorder)
	{
		this.size = size;
		this.offset = offset;
		coordBorderSize = coordBorder;
		uvBorderSize = new float[4];
		for (int i = 0; i < 4; i++)
		{
			uvBorderSize[i] = uvBorder[i] / (float)((i % 2 == 0) ? texture.Width : texture.Height);
		}
		initMesh();
		base.Materials.Add(new Material());
		base.Materials[0].Texture = texture;
		shader = ShaderManager.Shaders["GUI"];
	}

	public override void Dispose()
	{
		base.Dispose();
	}
}
