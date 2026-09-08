using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using s;
using y;

namespace z;

internal static class y
{
	public static int NumSides = 24;

	public static void GetShapeMeshData(s.b collidable, List<VertexPositionNormalTexture> vertices, List<uint> indices)
	{
		if (!(collidable.Shape is global::y._7 obj))
		{
			throw new ArgumentException("Wrong shape type");
		}
		Vector3 vector = default(Vector3);
		float num = (float)Math.PI * 2f / (float)NumSides;
		float radius = obj.Radius;
		vertices.Add(new VertexPositionNormalTexture(new Vector3(0f, radius, 0f), Vector3.Up, Vector2.Zero));
		for (int i = 1; i < NumSides / 2; i++)
		{
			float num2 = (float)Math.PI / 2f - (float)i * num;
			float num3 = (float)Math.Sin(num2);
			float num4 = (float)Math.Cos(num2);
			for (int j = 0; j < NumSides; j++)
			{
				float num5 = (float)j * num;
				vector.X = (float)Math.Cos(num5) * num4;
				vector.Y = num3;
				vector.Z = (float)Math.Sin(num5) * num4;
				vertices.Add(new VertexPositionNormalTexture(vector * radius, vector, Vector2.Zero));
			}
		}
		vertices.Add(new VertexPositionNormalTexture(new Vector3(0f, 0f - radius, 0f), Vector3.Down, Vector2.Zero));
		for (int k = 0; k < NumSides; k++)
		{
			indices.Add((uint)(vertices.Count - 1));
			indices.Add((uint)(vertices.Count - 2 - k));
			indices.Add((uint)(vertices.Count - 2 - (k + 1) % NumSides));
		}
		for (int l = 0; l < NumSides / 2 - 2; l++)
		{
			for (int m = 0; m < NumSides; m++)
			{
				int num6 = (m + 1) % NumSides;
				indices.Add((uint)(l * NumSides + num6 + 1));
				indices.Add((uint)(l * NumSides + m + 1));
				indices.Add((uint)((l + 1) * NumSides + m + 1));
				indices.Add((uint)((l + 1) * NumSides + num6 + 1));
				indices.Add((uint)(l * NumSides + num6 + 1));
				indices.Add((uint)((l + 1) * NumSides + m + 1));
			}
		}
		for (int n = 0; n < NumSides; n++)
		{
			indices.Add(0u);
			indices.Add((uint)(n + 1));
			indices.Add((uint)((n + 1) % NumSides + 1));
		}
	}
}
