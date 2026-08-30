using System;
using Microsoft.Xna.Framework;

namespace Viking_x86;

public class VScroll
{
	public static float zoom = 1f;

	public static Vector2 screenSize;

	public static Vector2 scroll;

	public static float angle;

	public static Vector2 xVec;

	public static Vector2 yVec;

	public static Vector2 scrollDif;

	private static Vector2 pScroll;

	public static Vector2 GetRealLoc(Vector2 loc, float layer)
	{
		return (loc - screenSize / 2f) / (zoom * layer) + scroll;
	}

	public static Vector2 GetScreenLoc(Vector2 loc, float layer)
	{
		Vector2 vector = (loc - scroll) * zoom * layer + screenSize / 2f;
		Vector2 vector2 = vector - screenSize / 2f;
		return screenSize / 2f + new Vector2(xVec.X * vector2.X, xVec.Y * vector2.X) + new Vector2(yVec.X * (0f - vector2.Y), yVec.Y * (0f - vector2.Y));
	}

	public static Vector2 GetRotatedVec2(Vector2 v)
	{
		return new Vector2(xVec.X * v.X, xVec.Y * v.X) + new Vector2(yVec.X * (0f - v.Y), yVec.Y * (0f - v.Y));
	}

	internal static void Bake()
	{
		scrollDif = scroll - pScroll;
		pScroll = scroll;
		xVec.X = (float)Math.Cos(angle);
		xVec.Y = (float)Math.Sin(angle);
		yVec.X = xVec.Y;
		yVec.Y = 0f - xVec.X;
	}
}
