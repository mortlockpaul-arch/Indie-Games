using System;
using Microsoft.Xna.Framework;

namespace MathTools;

public static class VectorTools
{
	public static Vector2 Rotate(Vector2 vec, float rot)
	{
		return new Vector2
		{
			X = (float)(Math.Cos(rot) * (double)vec.X - Math.Sin(rot) * (double)vec.Y),
			Y = (float)(Math.Sin(rot) * (double)vec.X + Math.Cos(rot) * (double)vec.Y)
		};
	}

	public static float GetAngleFromVector(Vector2 vec)
	{
		if (vec.X != 0f)
		{
			float num = (float)Math.Atan(vec.Y / vec.X);
			if (vec.X > 0f)
			{
				return num;
			}
			return num + (float)Math.PI;
		}
		if (vec.Y > 0f)
		{
			return (float)Math.PI / 2f;
		}
		return -(float)Math.PI / 2f;
	}
}
