using System;
using Microsoft.Xna.Framework;

namespace ZP2K9;

internal class Rand
{
	public static Random rand;

	public static int GetRandomInt(int min, int max)
	{
		return min + (int)(rand.NextDouble() * (double)(max - min));
	}

	public static Vector2 GetRandomVec2(float xMin, float xMax, float yMin, float yMax)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(GetRandomFloat(xMin, xMax), GetRandomFloat(yMin, yMax));
	}

	public static Vector2 GetRandomVec2(float distance)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		float randomRadian = GetRandomRadian();
		return new Vector2((float)Math.Cos(randomRadian), (float)Math.Sin(randomRadian)) * distance;
	}

	public static float GetRandomFloat(float min, float max)
	{
		return min + (float)(rand.NextDouble() * (double)(max - min));
	}

	public static float GetRandomRadian()
	{
		return GetRandomFloat(0f, 6.28f);
	}

	public static double GetRandomDouble(double min, double max)
	{
		return min + rand.NextDouble() * (max - min);
	}

	public static bool CointToss(float chanceToSucceed)
	{
		if (GetRandomFloat(0f, 1f) < chanceToSucceed)
		{
			return true;
		}
		return false;
	}
}
