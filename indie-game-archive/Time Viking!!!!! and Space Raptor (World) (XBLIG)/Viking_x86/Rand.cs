using System;
using Microsoft.Xna.Framework;

namespace Viking_x86;

internal class Rand
{
	public static Random rand;

	private static int zeroCount;

	public static int GetRandomInt(int min, int max)
	{
		return min + (int)(rand.NextDouble() * (double)(max - min));
	}

	public static Vector2 GetRandomVec2(float xMin, float xMax, float yMin, float yMax)
	{
		return new Vector2(GetRandomFloat(xMin, xMax), GetRandomFloat(yMin, yMax));
	}

	public static Vector3 GetRandomVec3(float xMin, float xMax, float yMin, float yMax, float zMin, float zMax)
	{
		return new Vector3(GetRandomFloat(xMin, xMax), GetRandomFloat(yMin, yMax), GetRandomFloat(zMin, zMax));
	}

	public static Vector4 GetRandomVec4(float xMin, float xMax, float yMin, float yMax, float zMin, float zMax, float wMin, float wMax)
	{
		return new Vector4(GetRandomFloat(xMin, xMax), GetRandomFloat(yMin, yMax), GetRandomFloat(zMin, zMax), GetRandomFloat(wMin, wMax));
	}

	public static float GetRandomFloat(float min, float max)
	{
		return min + (float)(rand.NextDouble() * (double)(max - min));
	}

	public static double GetRandomDouble(double min, double max)
	{
		return min + rand.NextDouble() * (max - min);
	}

	public static bool CoinToss(float chanceToSucceed)
	{
		if (GetRandomFloat(0f, 1f) < chanceToSucceed)
		{
			return true;
		}
		return false;
	}
}
