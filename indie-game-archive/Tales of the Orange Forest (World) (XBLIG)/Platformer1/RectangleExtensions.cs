using System;
using Microsoft.Xna.Framework;

namespace Platformer1;

public static class RectangleExtensions
{
	public static Vector2 GetIntersectionDepth(this Rectangle rectA, Rectangle rectB)
	{
		float num = (float)rectA.Width / 2f;
		float num2 = (float)rectA.Height / 2f;
		float num3 = (float)rectB.Width / 2f;
		float num4 = (float)rectB.Height / 2f;
		Vector2 vector = new Vector2((float)rectA.Left + num, (float)rectA.Top + num2);
		Vector2 vector2 = new Vector2((float)rectB.Left + num3, (float)rectB.Top + num4);
		float num5 = vector.X - vector2.X;
		float num6 = vector.Y - vector2.Y;
		float num7 = num + num3;
		float num8 = num2 + num4;
		if (Math.Abs(num5) >= num7 || Math.Abs(num6) >= num8)
		{
			return Vector2.Zero;
		}
		float x = ((num5 > 0f) ? (num7 - num5) : (0f - num7 - num5));
		float y = ((num6 > 0f) ? (num8 - num6) : (0f - num8 - num6));
		return new Vector2(x, y);
	}

	public static Vector2 GetBottomCenter(this Rectangle rect)
	{
		return new Vector2((float)rect.X + (float)rect.Width / 2f, rect.Bottom);
	}

	public static Vector2 GetCenter(this Rectangle rect)
	{
		return new Vector2((float)rect.X + (float)rect.Width / 2f, (float)rect.Y + (float)rect.Height / 2f);
	}
}
