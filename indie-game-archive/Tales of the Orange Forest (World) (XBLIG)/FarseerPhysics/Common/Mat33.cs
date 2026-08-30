using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common;

public struct Mat33(Vector3 c1, Vector3 c2, Vector3 c3)
{
	public Vector3 col1 = c1;

	public Vector3 col2 = c2;

	public Vector3 col3 = c3;

	public void SetZero()
	{
		col1 = Vector3.Zero;
		col2 = Vector3.Zero;
		col3 = Vector3.Zero;
	}

	public Vector3 Solve33(Vector3 b)
	{
		float num = Vector3.Dot(col1, Vector3.Cross(col2, col3));
		if (num != 0f)
		{
			num = 1f / num;
		}
		return new Vector3(num * Vector3.Dot(b, Vector3.Cross(col2, col3)), num * Vector3.Dot(col1, Vector3.Cross(b, col3)), num * Vector3.Dot(col1, Vector3.Cross(col2, b)));
	}

	public Vector2 Solve22(Vector2 b)
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
}
