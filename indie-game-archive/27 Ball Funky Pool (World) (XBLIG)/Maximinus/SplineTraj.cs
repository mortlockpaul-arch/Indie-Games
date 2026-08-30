using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Maximinus;

public class SplineTraj
{
	private List<Vector3> points;

	private int nbParts;

	private float length;

	private List<float> weights;

	public float Length => length;

	public SplineTraj(List<Vector3> points)
	{
		UpdatePoints(points);
	}

	public SplineTraj(List<Vector2> points)
	{
		List<Vector3> list = new List<Vector3>();
		foreach (Vector2 point in points)
		{
			list.Add(new Vector3(point.X, point.Y, 0f));
		}
		UpdatePoints(list);
	}

	public SplineTraj(Point[] points)
	{
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < points.Length; i++)
		{
			Point point = points[i];
			list.Add(new Vector3(point.X, point.Y, 0f));
		}
		UpdatePoints(list);
	}

	public SplineTraj(List<Point> points)
	{
		List<Vector3> list = new List<Vector3>();
		foreach (Point point in points)
		{
			list.Add(new Vector3(point.X, point.Y, 0f));
		}
		UpdatePoints(list);
	}

	public void UpdatePoints(List<Vector3> points)
	{
		this.points = points;
		if (points.Count < 4)
		{
			throw new Exception("not enough points : " + points.Count);
		}
		nbParts = points.Count - 3;
		weights = new List<float>();
		weights.Add(0f);
		length = 0f;
		for (int i = 1; i < points.Count - 2; i++)
		{
			float num = DistanceSimpleSpline(points[i - 1], points[i], points[i + 1], points[i + 2]);
			weights.Add(num + weights[weights.Count - 1]);
			length += num;
		}
		for (int j = 0; j < weights.Count; j++)
		{
			weights[j] /= weights[weights.Count - 1];
		}
	}

	private static float DistanceSimpleSpline(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		float num = 0f;
		for (int i = 1; i < 20; i++)
		{
			num += Vector3.Distance(Utils.CatmullRom3D(p0, p1, p2, p3, (float)i / 20f), Utils.CatmullRom3D(p0, p1, p2, p3, (float)(i - 1) / 20f));
		}
		return num;
	}

	public Vector2 GetByRatio2D(float ratio)
	{
		Vector3 byRatio = GetByRatio(ratio);
		return new Vector2(byRatio.X, byRatio.Y);
	}

	public Vector3 GetByRatio(float ratio)
	{
		if (ratio >= 1f)
		{
			return points[points.Count - 2];
		}
		if (ratio <= 0f)
		{
			return points[1];
		}
		int i;
		for (i = 1; weights[i] < ratio; i++)
		{
		}
		float amount = (ratio - weights[i - 1]) / (weights[i] - weights[i - 1]);
		return Utils.CatmullRom3D(points[i - 1], points[i], points[i + 1], points[i + 2], amount);
	}
}
