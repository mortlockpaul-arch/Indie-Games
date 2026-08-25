using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RenegadeEngine.Graphics;

public class Sphere : PrimitiveShape
{
	private Matrix scaled;

	private Matrix translation;

	public Sphere(GraphicsDevice device, int tessellation, float scale)
	{
		if (tessellation < 3)
		{
			tessellation = 3;
		}
		List<VertexPositionNormal> list = new List<VertexPositionNormal>();
		int num = tessellation;
		int num2 = tessellation * 2;
		float num3 = 0.5f;
		list.Add(new VertexPositionNormal(Vector3.Down * num3, Vector3.Down));
		for (int i = 0; i < num - 1; i++)
		{
			float num4 = (float)(i + 1) * (float)Math.PI / (float)num - (float)Math.PI / 2f;
			float y = (float)Math.Sin(num4);
			float num5 = (float)Math.Cos(num4);
			for (int j = 0; j < num2; j++)
			{
				float num6 = (float)j * ((float)Math.PI * 2f) / (float)num2;
				float x = (float)Math.Cos(num6) * num5;
				float z = (float)Math.Sin(num6) * num5;
				Vector3 vector = new Vector3(x, y, z);
				list.Add(new VertexPositionNormal(vector * num3, vector));
			}
		}
		list.Add(new VertexPositionNormal(Vector3.Up * num3, Vector3.Up));
		vertexCount = list.Count;
		vertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormal), list.Count, BufferUsage.None);
		vertexBuffer.SetData(list.ToArray());
		List<int> list2 = new List<int>();
		for (int k = 0; k < num2; k++)
		{
			list2.Add(0);
			list2.Add(1 + (k + 1) % num2);
			list2.Add(1 + k);
		}
		for (int l = 0; l < num - 2; l++)
		{
			for (int m = 0; m < num2; m++)
			{
				int num7 = l + 1;
				int num8 = (m + 1) % num2;
				list2.Add(1 + l * num2 + m);
				list2.Add(1 + l * num2 + num8);
				list2.Add(1 + num7 * num2 + m);
				list2.Add(1 + l * num2 + num8);
				list2.Add(1 + num7 * num2 + num8);
				list2.Add(1 + num7 * num2 + m);
			}
		}
		for (int n = 0; n < num2; n++)
		{
			list2.Add(list.Count - 1);
			list2.Add(list.Count - 2 - (n + 1) % num2);
			list2.Add(list.Count - 2 - n);
		}
		triangleCount = list2.Count / 3;
		meshIndexBuffer = new IndexBuffer(device, typeof(int), list2.Count, BufferUsage.None);
		meshIndexBuffer.SetData(list2.ToArray());
		world.M11 = scale;
		world.M22 = scale;
		world.M33 = scale;
		world.M44 = 1f;
		InitializeEffect(device);
	}

	public override void Update()
	{
		world.M41 = position.X;
		world.M42 = position.Y;
		world.M43 = position.Z;
	}

	public Sphere(BasicEffect effect, VertexBuffer vertBuff, IndexBuffer indBuff, int vertCount, int numPrimitives, float scale)
	{
		vertexBuffer = vertBuff;
		meshIndexBuffer = indBuff;
		base.effect = effect;
		vertexCount = vertCount;
		triangleCount = numPrimitives;
		world.M11 = scale;
		world.M22 = scale;
		world.M33 = scale;
		world.M44 = 1f;
	}
}
