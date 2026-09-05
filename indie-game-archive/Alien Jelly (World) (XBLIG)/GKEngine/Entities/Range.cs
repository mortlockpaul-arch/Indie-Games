using System;

namespace GKEngine.Entities;

public class Range
{
	public float from;

	public float to;

	public float max;

	public float min;

	public float length => to - from;

	public float distance => Math.Abs(length);

	public float random => from + (to - from) * (float)GameEngine.random.NextDouble();

	public Range()
	{
		from = 0f;
		to = 0f;
	}

	public Range(float xFrom, float xTo)
	{
		from = xFrom;
		to = xTo;
		max = ((from > to) ? from : to);
		min = ((from > to) ? to : from);
	}

	public float Lerp(float xRatio)
	{
		return from + xRatio * (to - from);
	}

	public float Clamp(float xValue)
	{
		return Math.Max(Math.Min(xValue, max), min);
	}

	public bool In(float xValue)
	{
		if (xValue >= min)
		{
			return xValue <= max;
		}
		return false;
	}

	public bool Between(float xValue)
	{
		if (xValue > min)
		{
			return xValue < max;
		}
		return false;
	}

	public float Ratio(float xValue)
	{
		return (Clamp(xValue) - from) / length;
	}

	public bool Intersect(float xFrom, float xTo)
	{
		if (!(xFrom > max))
		{
			return !(xTo < min);
		}
		return false;
	}

	public override string ToString()
	{
		return "Range (" + from + ", " + to + ")";
	}
}
