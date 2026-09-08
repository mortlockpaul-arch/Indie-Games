using System;
using Microsoft.Xna.Framework;

namespace Quasar.Trails;

public class Trail
{
	public struct PointData(Vector3 point, float progress)
	{
		public Vector3 Point = point;

		public float Progress = progress;
	}

	private PointData[] points;

	private int pointCount;

	private TrailSystem system;

	private float pointProgress;

	private Transform transform;

	private int trailIndex;

	private bool enabled;

	private float lastGeneration;

	private float currentTrailLength;

	public PointData[] Points => points;

	public int PointCount => pointCount;

	public float PointProgress => pointProgress;

	public int Index => trailIndex;

	public bool Enabled => enabled;

	public float CurrentTrailLength => currentTrailLength;

	public void SetIndex(int index)
	{
		trailIndex = index;
	}

	public Trail(TrailSystem system, int index)
	{
		trailIndex = index;
		this.system = system;
		pointCount = 0;
		points = new PointData[system.TrailLength];
	}

	public void Disable()
	{
		enabled = false;
	}

	public void Init(Transform transform)
	{
		this.transform = transform;
		enabled = true;
		pointCount = 0;
	}

	public void Update()
	{
		if (enabled)
		{
			ref PointData reference = ref points[0];
			reference = new PointData(transform.WorldTranslation, 0f);
			float num = system.Timer.TimeSince(lastGeneration);
			if (num >= system.GenerationPeriod)
			{
				if (pointCount == 0 || Vector3.Distance(points[1].Point, points[0].Point) > system.MinTrailDistance)
				{
					Generate();
				}
				lastGeneration = system.Timer.TotalTimeSeconds;
			}
		}
		float progress = system.Timer.LastIntervalSeconds / system.TrailDuration;
		for (int i = 0; i < pointCount; i++)
		{
			IncreaseProgress(ref points[i], progress);
		}
		while (pointCount > 1 && points[pointCount - 2].Progress >= 1f)
		{
			pointCount--;
		}
		if (pointCount == 0)
		{
			currentTrailLength = 0f;
		}
		else
		{
			currentTrailLength = Math.Min(1f, points[pointCount - 1].Progress);
		}
	}

	private void IncreaseProgress(ref PointData data, float progress)
	{
		data.Progress += progress;
	}

	private void Generate()
	{
		for (int num = pointCount - 1; num >= 0; num--)
		{
			if (num < points.Length - 1)
			{
				ref PointData reference = ref points[num + 1];
				reference = points[num];
			}
		}
		pointCount = Math.Min(pointCount + 1, points.Length);
	}

	public void Destroy()
	{
		system.RemoveTrail(this);
	}
}
