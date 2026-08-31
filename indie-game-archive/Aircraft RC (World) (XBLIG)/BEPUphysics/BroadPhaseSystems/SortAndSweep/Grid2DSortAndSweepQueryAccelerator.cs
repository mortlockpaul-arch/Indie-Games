using System;
using System.Collections.Generic;
using BEPUphysics.BroadPhaseEntries;
using Microsoft.Xna.Framework;

namespace BEPUphysics.BroadPhaseSystems.SortAndSweep;

public class Grid2DSortAndSweepQueryAccelerator : IQueryAccelerator
{
	private Grid2DSortAndSweep owner;

	/// <summary>
	/// Gets the broad phase associated with this query accelerator.
	/// </summary>
	public BroadPhase BroadPhase => owner;

	public Grid2DSortAndSweepQueryAccelerator(Grid2DSortAndSweep owner)
	{
		this.owner = owner;
	}

	public bool RayCast(Ray ray, IList<BroadPhaseEntry> outputIntersections)
	{
		throw new NotSupportedException("The Grid2DSortAndSweep broad phase cannot accelerate infinite ray casts.  Consider specifying a maximum length or using a broad phase which supports infinite ray casts.");
	}

	public bool RayCast(Ray ray, float maximumLength, IList<BroadPhaseEntry> outputIntersections)
	{
		if (maximumLength == float.MaxValue)
		{
			throw new NotSupportedException("The Grid2DSortAndSweep broad phase cannot accelerate infinite ray casts.  Consider specifying a maximum length or using a broad phase which supports infinite ray casts.");
		}
		float num = 0f;
		Vector3 v = ray.Position;
		Grid2DSortAndSweep.ComputeCell(ref v, out var cell);
		while (true)
		{
			float num2 = 1f / Grid2DSortAndSweep.cellSizeInverse;
			float num3 = ((ray.Direction.Y > 0f) ? (((float)(cell.Y + 1) * num2 - v.Y) / ray.Direction.Y) : ((!(ray.Direction.Y < 0f)) ? 1E+11f : (((float)cell.Y * num2 - v.Y) / ray.Direction.Y)));
			float num4 = ((ray.Direction.Z > 0f) ? (((float)(cell.Z + 1) * num2 - v.Z) / ray.Direction.Z) : ((!(ray.Direction.Z < 0f)) ? 1E+11f : (((float)cell.Z * num2 - v.Z) / ray.Direction.Z)));
			bool flag = num3 < num4;
			float num5 = (flag ? num3 : num4);
			if (owner.cellSet.TryGetCell(ref cell, out var cell2))
			{
				float num6 = ((!(ray.Direction.X < 0f)) ? (v.X + ray.Direction.X * num5) : v.X);
				for (int i = 0; i < cell2.entries.count && cell2.entries.Elements[i].item.boundingBox.Min.X <= num6; i++)
				{
					BroadPhaseEntry item = cell2.entries.Elements[i].item;
					ray.Intersects(ref item.boundingBox, out var result);
					if (result.HasValue)
					{
						float? num7 = result;
						if (num7.GetValueOrDefault() < maximumLength && num7.HasValue && !outputIntersections.Contains(item))
						{
							outputIntersections.Add(item);
						}
					}
				}
			}
			num += num5;
			if (num > maximumLength)
			{
				break;
			}
			Vector3.Multiply(ref ray.Direction, num5, out var result2);
			Vector3.Add(ref result2, ref v, out v);
			if (flag)
			{
				if (ray.Direction.Y < 0f)
				{
					cell.Y--;
				}
				else
				{
					cell.Y++;
				}
			}
			else if (ray.Direction.Z < 0f)
			{
				cell.Z--;
			}
			else
			{
				cell.Z++;
			}
		}
		return outputIntersections.Count > 0;
	}

	public void GetEntries(BoundingBox boundingShape, IList<BroadPhaseEntry> overlaps)
	{
		Grid2DSortAndSweep.ComputeCell(ref boundingShape.Min, out var cell);
		Grid2DSortAndSweep.ComputeCell(ref boundingShape.Max, out var cell2);
		Int2 cellIndex = default(Int2);
		for (int i = cell.Y; i <= cell2.Y; i++)
		{
			for (int j = cell.Z; j <= cell2.Z; j++)
			{
				cellIndex.Y = i;
				cellIndex.Z = j;
				if (!owner.cellSet.TryGetCell(ref cellIndex, out var cell3))
				{
					continue;
				}
				for (int k = 0; k < cell3.entries.count && cell3.entries.Elements[k].item.boundingBox.Min.X <= boundingShape.Max.X; k++)
				{
					BroadPhaseEntry item = cell3.entries.Elements[k].item;
					boundingShape.Intersects(ref item.boundingBox, out var result);
					if (result && !overlaps.Contains(item))
					{
						overlaps.Add(item);
					}
				}
			}
		}
	}

	public void GetEntries(BoundingSphere boundingShape, IList<BroadPhaseEntry> overlaps)
	{
		Vector3 value = default(Vector3);
		value.X = boundingShape.Radius;
		value.Y = value.X;
		value.Z = value.Y;
		BoundingBox boundingBox = default(BoundingBox);
		Vector3.Add(ref boundingShape.Center, ref value, out boundingBox.Max);
		Vector3.Subtract(ref boundingShape.Center, ref value, out boundingBox.Min);
		Grid2DSortAndSweep.ComputeCell(ref boundingBox.Min, out var cell);
		Grid2DSortAndSweep.ComputeCell(ref boundingBox.Max, out var cell2);
		Int2 cellIndex = default(Int2);
		for (int i = cell.Y; i <= cell2.Y; i++)
		{
			for (int j = cell.Z; j <= cell2.Z; j++)
			{
				cellIndex.Y = i;
				cellIndex.Z = j;
				if (!owner.cellSet.TryGetCell(ref cellIndex, out var cell3))
				{
					continue;
				}
				for (int k = 0; k < cell3.entries.count && cell3.entries.Elements[k].item.boundingBox.Min.X <= boundingBox.Max.X; k++)
				{
					BroadPhaseEntry item = cell3.entries.Elements[k].item;
					boundingShape.Intersects(ref item.boundingBox, out var result);
					if (result && !overlaps.Contains(item))
					{
						overlaps.Add(item);
					}
				}
			}
		}
	}

	public void GetEntries(BoundingFrustum boundingShape, IList<BroadPhaseEntry> overlaps)
	{
		throw new NotSupportedException("The Grid2DSortAndSweep broad phase cannot accelerate frustum tests.  Consider using a broad phase which supports frustum tests or using a custom solution.");
	}
}
