using System;
using Microsoft.Xna.Framework;

namespace ProjectMercury;

internal static class RandomHelper
{
	private static readonly object Padlock;

	private static Random Random { get; set; }

	static RandomHelper()
	{
		Padlock = new object();
		Random = new Random();
	}

	public static int NextInt()
	{
		lock (Padlock)
		{
			return Random.Next();
		}
	}

	public static int NextInt(int max)
	{
		lock (Padlock)
		{
			return Random.Next(max);
		}
	}

	public static int NextInt(int min, int max)
	{
		lock (Padlock)
		{
			return Random.Next(min, max);
		}
	}

	public static float NextFloat()
	{
		lock (Padlock)
		{
			return (float)Random.NextDouble();
		}
	}

	public static float NextFloat(float max)
	{
		return max * NextFloat();
	}

	public static float NextFloat(float min, float max)
	{
		return (max - min) * NextFloat() + min;
	}

	public static float NextFloat(Range range)
	{
		return (range.Maximum - range.Minimum) * NextFloat() + range.Minimum;
	}

	public static byte NextByte()
	{
		return (byte)NextInt(255);
	}

	public static bool NextBool()
	{
		return NextInt(2) == 1;
	}

	public static Vector2 NextUnitVector()
	{
		lock (Padlock)
		{
			float value = NextFloat(-3.141593f, 3.141593f);
			return new Vector2
			{
				X = Calculator.Cos(value),
				Y = Calculator.Sin(value)
			};
		}
	}

	public static float Variation(float value, float variation)
	{
		float min = value - variation;
		float max = value + variation;
		return NextFloat(min, max);
	}

	public static int ChooseOne(params int[] values)
	{
		int num = NextInt(values.Length);
		return values[num];
	}

	public unsafe static int* ChooseOne(int* valuesArray, int length)
	{
		int num = NextInt(length);
		return valuesArray + num;
	}

	public static float ChooseOne(params float[] values)
	{
		int num = NextInt(values.Length);
		return values[num];
	}

	public unsafe static float* ChooseOne(float* valuesArray, int length)
	{
		int num = NextInt(length);
		return valuesArray + num;
	}

	public static T ChooseOne<T>(params T[] values)
	{
		int num = NextInt(values.Length);
		return values[num];
	}
}
