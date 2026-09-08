using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global.Vertex;
using Quasar.Meshes.Generic;
using Quasar.Shaders;

namespace Quasar.Meshes;

public class RectangleListMesh : UserIndexVertexMesh<VertexPositionColorUV>
{
	public struct RectangleData
	{
		public Vector2 Size;

		public Vector3 Position;

		public float Rotation;

		public Vector4 Diffuse;

		internal bool uvInitialized;

		internal Vector2 minUV;

		internal Vector2 maxUV;

		public void SetTile(int tileIndex, int cols, int rows)
		{
			uvInitialized = true;
			int num = tileIndex / cols;
			int num2 = tileIndex % cols;
			float num3 = 1f / (float)cols;
			float num4 = 1f / (float)rows;
			minUV = new Vector2(num3 * (float)num2, num4 * (float)num);
			maxUV = new Vector2(num3 * (float)(num2 + 1), num4 * (float)(num + 1));
		}
	}

	private bool flipX;

	private bool flipY;

	private bool useXZCoords;

	private float zValue;

	private RectangleData[] rectangles;

	private int rectangleCount;

	public bool FlipX
	{
		get
		{
			return flipX;
		}
		set
		{
			flipX = value;
		}
	}

	public bool FlipY
	{
		get
		{
			return flipY;
		}
		set
		{
			flipY = value;
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
		}
	}

	public RectangleData[] Rectangles => rectangles;

	public int RectangleCount => rectangleCount;

	public int MaxRectangles => rectangles.Length;

	private void initializeBuffer(int maxRectangles)
	{
		rectangles = new RectangleData[maxRectangles];
		rectangleCount = 0;
		initMesh(maxRectangles * 4, maxRectangles * 6);
		base.PrimitiveCount = 0;
		fillBuffer(maxRectangles);
	}

	private void fillBuffer(int maxRectangles)
	{
		for (int i = 0; i < maxRectangles; i++)
		{
			int num = 6 * i;
			int num2 = 4 * i;
			indicesBuffer[num] = (short)num2;
			indicesBuffer[num + 1] = (short)(num2 + 1);
			indicesBuffer[num + 2] = (short)(num2 + 2);
			indicesBuffer[num + 3] = (short)(num2 + 2);
			indicesBuffer[num + 4] = (short)(num2 + 1);
			indicesBuffer[num + 5] = (short)(num2 + 3);
		}
	}

	private void generateVertices()
	{
		for (int i = 0; i < rectangleCount; i++)
		{
			int num = i * 4;
			int num2 = num;
			int num3 = num + 1;
			int num4 = num + 2;
			int num5 = num + 3;
			RectangleData rectangleData = rectangles[i];
			Vector2 vector = rectangleData.Size / 2f;
			if (rectangleData.Rotation != 0f)
			{
				float num6 = (float)Math.Sin(rectangleData.Rotation);
				float num7 = (float)Math.Cos(rectangleData.Rotation);
				float num8 = vector.X * num7;
				float num9 = vector.Y * num7;
				float num10 = vector.X * num6;
				float num11 = vector.Y * num6;
				verticesBuffer[num2].Position = new Vector3(0f - num8 - num11, num9 - num10, zValue);
				verticesBuffer[num3].Position = new Vector3(num8 - num11, num9 + num10, zValue);
				verticesBuffer[num4].Position = new Vector3(0f - num8 + num11, 0f - num9 - num10, zValue);
				verticesBuffer[num5].Position = new Vector3(num8 + num11, 0f - num9 + num10, zValue);
			}
			else
			{
				verticesBuffer[num2].Position = new Vector3(0f - vector.X, vector.Y, 0f);
				verticesBuffer[num3].Position = new Vector3(vector.X, vector.Y, 0f);
				verticesBuffer[num4].Position = new Vector3(0f - vector.X, 0f - vector.Y, 0f);
				verticesBuffer[num5].Position = new Vector3(vector.X, 0f - vector.Y, 0f);
			}
			verticesBuffer[num2].UV = new Vector2(flipX ? rectangleData.maxUV.X : rectangleData.minUV.X, flipY ? rectangleData.maxUV.Y : rectangleData.minUV.Y);
			verticesBuffer[num3].UV = new Vector2(flipX ? rectangleData.minUV.X : rectangleData.maxUV.X, flipY ? rectangleData.maxUV.Y : rectangleData.minUV.Y);
			verticesBuffer[num4].UV = new Vector2(flipX ? rectangleData.maxUV.X : rectangleData.minUV.X, flipY ? rectangleData.minUV.Y : rectangleData.maxUV.Y);
			verticesBuffer[num5].UV = new Vector2(flipX ? rectangleData.minUV.X : rectangleData.maxUV.X, flipY ? rectangleData.minUV.Y : rectangleData.maxUV.Y);
			verticesBuffer[num2].Color = (verticesBuffer[num3].Color = (verticesBuffer[num4].Color = (verticesBuffer[num5].Color = new Color(rectangleData.Diffuse))));
			if (useXZCoords)
			{
				verticesBuffer[num2].Position = new Vector3(verticesBuffer[num2].Position.X, verticesBuffer[num2].Position.Z, verticesBuffer[num2].Position.Y);
				verticesBuffer[num3].Position = new Vector3(verticesBuffer[num3].Position.X, verticesBuffer[num3].Position.Z, verticesBuffer[num3].Position.Y);
				verticesBuffer[num4].Position = new Vector3(verticesBuffer[num4].Position.X, verticesBuffer[num4].Position.Z, verticesBuffer[num4].Position.Y);
				verticesBuffer[num5].Position = new Vector3(verticesBuffer[num5].Position.X, verticesBuffer[num5].Position.Z, verticesBuffer[num5].Position.Y);
				VertexPositionColorUV vertexPositionColorUV = verticesBuffer[num3];
				ref VertexPositionColorUV reference = ref verticesBuffer[num3];
				reference = verticesBuffer[num4];
				verticesBuffer[num4] = vertexPositionColorUV;
			}
			verticesBuffer[num2].Position += rectangleData.Position;
			verticesBuffer[num3].Position += rectangleData.Position;
			verticesBuffer[num4].Position += rectangleData.Position;
			verticesBuffer[num5].Position += rectangleData.Position;
		}
		base.PrimitiveCount = rectangleCount * 2;
	}

	public override void Dispose()
	{
		base.Dispose();
	}

	public RectangleListMesh(int maxRectangles, Texture2D backgroundTexture)
	{
		initializeBuffer(maxRectangles);
		Material material = new Material();
		shader = ShaderManager.Shaders["SimpleVertexColor"];
		material.Textures.Add(backgroundTexture);
		materials.Add(material);
	}

	public RectangleListMesh(int maxRectangles)
	{
		initializeBuffer(maxRectangles);
		Material item = new Material();
		shader = ShaderManager.Shaders["VertexColor"];
		materials.Add(item);
	}

	public void ClearRectangles()
	{
		rectangleCount = 0;
	}

	public void AddRectangle(ref RectangleData data)
	{
		if (rectangleCount < rectangles.Length)
		{
			if (!data.uvInitialized)
			{
				data.minUV = Vector2.Zero;
				data.maxUV = Vector2.One;
			}
			ref RectangleData reference = ref rectangles[rectangleCount];
			reference = data;
			rectangleCount++;
		}
	}

	public void UpdateData()
	{
		generateVertices();
	}

	public override void Render(Transform motion)
	{
		base.Render(motion);
	}
}
