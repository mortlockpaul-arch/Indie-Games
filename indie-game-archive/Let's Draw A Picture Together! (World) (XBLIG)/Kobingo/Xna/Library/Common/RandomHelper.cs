using System;
using Microsoft.Xna.Framework;

namespace Kobingo.Xna.Library.Common;

public class RandomHelper
{
	private static Random m_Random = new Random();

	public static int GetInt32(int min, int max)
	{
		return m_Random.Next(min, max);
	}

	public static float GetSingle(float min, float max)
	{
		return (float)m_Random.NextDouble() * (max - min) + min;
	}

	public static float GetSingle()
	{
		return (float)m_Random.NextDouble();
	}

	public static Vector2 GetVector2(Vector2 min, Vector2 max)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(GetSingle(min.X, max.X), GetSingle(min.Y, max.Y));
	}

	public static float GetAngle()
	{
		return GetSingle(0f, (float)Math.PI * 2f);
	}
}
