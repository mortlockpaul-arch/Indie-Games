using System;
using Microsoft.Xna.Framework;

namespace RenegadeEngine;

public static class Rand
{
	private static Random rand = new Random();

	public static void Unseeded()
	{
		rand = new Random();
	}

	public static void Seed(int seed)
	{
		rand = new Random(seed);
	}

	public static int Next()
	{
		return rand.Next();
	}

	public static int Next(int min, int max)
	{
		return rand.Next(min, max);
	}

	public static double NextDouble()
	{
		return rand.NextDouble();
	}

	public static double NextDouble(double min, double max)
	{
		return rand.NextDouble() * (max - min) + min;
	}

	public static void NextBytes(ref byte[] buffer)
	{
		rand.NextBytes(buffer);
	}

	public static float NextFloat()
	{
		return (float)rand.NextDouble();
	}

	public static double RandomBinomial(double scale)
	{
		return (rand.NextDouble() - rand.NextDouble()) * scale;
	}

	public static Vector3 RandomVector(Vector3 scale)
	{
		return new Vector3((float)NextDouble(0f - scale.X, scale.X), (float)NextDouble(0f - scale.Y, scale.Y), (float)NextDouble(0f - scale.Z, scale.Z));
	}

	public static Vector3 RandomVector(Vector3 min, Vector3 max)
	{
		return new Vector3((float)NextDouble(min.X, max.X), (float)NextDouble(min.Y, max.Y), (float)NextDouble(min.Z, max.Z));
	}

	public static Quaternion RandomQuaternion()
	{
		return Quaternion.Normalize(new Quaternion((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble()));
	}
}
