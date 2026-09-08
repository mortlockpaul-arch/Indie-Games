using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global.Vertex;
using Quasar.Meshes.Generic;
using Quasar.Shaders;

namespace Quasar.Meshes;

public class RectangleList3DMesh : UserIndexVertexMesh<VertexPositionColorUV>
{
	public struct RectangleData
	{
		public Vector2 Size;

		public Vector3 Position;

		public Quaternion Rotation;

		public Vector4 Diffuse;
	}

	private bool flipX;

	private bool flipY;

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
			verticesBuffer[num2].Position = Vector3.Transform(new Vector3(0f - vector.X, 0f, vector.Y), rectangleData.Rotation);
			verticesBuffer[num4].Position = Vector3.Transform(new Vector3(vector.X, 0f, vector.Y), rectangleData.Rotation);
			verticesBuffer[num3].Position = -verticesBuffer[num4].Position;
			verticesBuffer[num5].Position = -verticesBuffer[num2].Position;
			verticesBuffer[num2].UV = new Vector2(flipX ? 1 : 0, flipY ? 1 : 0);
			verticesBuffer[num3].UV = new Vector2((!flipX) ? 1 : 0, flipY ? 1 : 0);
			verticesBuffer[num4].UV = new Vector2(flipX ? 1 : 0, (!flipY) ? 1 : 0);
			verticesBuffer[num5].UV = new Vector2((!flipX) ? 1 : 0, (!flipY) ? 1 : 0);
			verticesBuffer[num2].Color = (verticesBuffer[num3].Color = (verticesBuffer[num4].Color = (verticesBuffer[num5].Color = new Color(rectangleData.Diffuse))));
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

	public RectangleList3DMesh(int maxRectangles, Texture2D backgroundTexture)
	{
		initializeBuffer(maxRectangles);
		Material material = new Material();
		shader = ShaderManager.Shaders["SimpleVertexColor"];
		material.Textures.Add(backgroundTexture);
		materials.Add(material);
	}

	public RectangleList3DMesh(int maxRectangles)
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
