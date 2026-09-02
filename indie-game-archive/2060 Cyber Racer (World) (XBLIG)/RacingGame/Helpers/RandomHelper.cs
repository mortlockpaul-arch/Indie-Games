using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RacingGame.Helpers;

public static class RandomHelper
{
	public static Random globalRandomGenerator = GenerateNewRandomGenerator();

	public static Color RandomColor
	{
		get
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			return new Color(new Vector3(GetRandomFloat(0.25f, 1f), GetRandomFloat(0.25f, 1f), GetRandomFloat(0.25f, 1f)));
		}
	}

	public static Vector3 RandomNormalVector3
	{
		get
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			Vector3 result = default(Vector3);
			((Vector3)(ref result))._002Ector(GetRandomFloat(-1f, 1f), GetRandomFloat(-1f, 1f), GetRandomFloat(-1f, 1f));
			((Vector3)(ref result)).Normalize();
			return result;
		}
	}

	public static Random GenerateNewRandomGenerator()
	{
		globalRandomGenerator = new Random((int)DateTime.Now.Ticks);
		return globalRandomGenerator;
	}

	public static int GetRandomInt(int max)
	{
		return globalRandomGenerator.Next(max);
	}

	public static float GetRandomFloat(float min, float max)
	{
		return (float)globalRandomGenerator.NextDouble() * (max - min) + min;
	}

	public static byte GetRandomByte(byte min, byte max)
	{
		return (byte)globalRandomGenerator.Next(min, max);
	}

	public static Vector2 GetRandomVector2(float min, float max)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(GetRandomFloat(min, max), GetRandomFloat(min, max));
	}

	public static Vector3 GetRandomVector3(float min, float max)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(GetRandomFloat(min, max), GetRandomFloat(min, max), GetRandomFloat(min, max));
	}
}
