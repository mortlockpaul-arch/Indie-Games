using System;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common;

public struct Mat22
{
	public Vector2 col1;

	public Vector2 col2;

	public float Angle => (float)Math.Atan2(col1.Y, col1.X);

	public Mat22 Inverse
	{
		get
		{
			float x = col1.X;
			float x2 = col2.X;
			float y = col1.Y;
			float y2 = col2.Y;
			float num = x * y2 - x2 * y;
			if (num != 0f)
			{
				num = 1f / num;
			}
			return new Mat22(new Vector2(num * y2, (0f - num) * y), new Vector2((0f - num) * x2, num * x));
		}
	}

	public Mat22(Vector2 c1, Vector2 c2)
	{
		col1 = c1;
		col2 = c2;
	}

	public Mat22(float a11, float a12, float a21, float a22)
	{
		col1 = new Vector2(a11, a21);
		col2 = new Vector2(a12, a22);
	}

	public Mat22(float angle)
	{
		float num = (float)Math.Cos(angle);
		float num2 = (float)Math.Sin(angle);
		col1 = new Vector2(num, num2);
		col2 = new Vector2(0f - num2, num);
	}

	public void Set(Vector2 c1, Vector2 c2)
	{
		col1 = c1;
		col2 = c2;
	}

	public void Set(float angle)
	{
		float num = (float)Math.Cos(angle);
		float num2 = (float)Math.Sin(angle);
		col1.X = num;
		col2.X = 0f - num2;
		col1.Y = num2;
		col2.Y = num;
	}

	public void SetIdentity()
	{
		col1.X = 1f;
		col2.X = 0f;
		col1.Y = 0f;
		col2.Y = 1f;
	}

	public void SetZero()
	{
		col1.X = 0f;
		col2.X = 0f;
		col1.Y = 0f;
		col2.Y = 0f;
	}

	public Vector2 Solve(Vector2 b)
	{
		float x = col1.X;
		float x2 = col2.X;
		float y = col1.Y;
		float y2 = col2.Y;
		float num = x * y2 - x2 * y;
		if (num != 0f)
		{
			num = 1f / num;
		}
		return new Vector2(num * (y2 * b.X - x2 * b.Y), num * (x * b.Y - y * b.X));
	}

	public static void Add(ref Mat22 A, ref Mat22 B, out Mat22 R)
	{
		R = new Mat22(A.col1 + B.col1, A.col2 + B.col2);
	}
}
