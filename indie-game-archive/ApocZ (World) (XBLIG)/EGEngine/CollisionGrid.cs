using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PropModel;

namespace EGEngine;

public struct CollisionGrid
{
	public const int xGridSize = 1024;

	public const int zGridSize = 1024;

	private static BoundingBox bbox = default(BoundingBox);

	public Vector3 Min;

	public Vector3 Max;

	public int nXGrid;

	public int nZGrid;

	public List<MeshInstanceData>[,] collisionGrid;

	public void Create(Vector3 min, Vector3 max)
	{
		Min = min;
		Max = max;
		Min.Y = -1000f;
		Max.Y = 30000f;
		nXGrid = (int)((Max.X - Min.X) / 1024f);
		nZGrid = (int)((Max.Z - Min.Z) / 1024f);
		collisionGrid = new List<MeshInstanceData>[nXGrid, nZGrid];
		for (int i = 0; i < nXGrid; i++)
		{
			for (int j = 0; j < nZGrid; j++)
			{
				collisionGrid[i, j] = new List<MeshInstanceData>();
			}
		}
	}

	public void Add(MeshInstanceData e, BoundingBox b)
	{
		for (int i = 0; i < nXGrid; i++)
		{
			for (int j = 0; j < nZGrid; j++)
			{
				bbox.Min = Min;
				bbox.Min.X += i * 1024;
				bbox.Min.Z += j * 1024;
				bbox.Max = bbox.Min;
				bbox.Max.X += 1024f;
				bbox.Max.Z += 1024f;
				if (b.Max.X >= bbox.Min.X && b.Min.X <= bbox.Max.X && b.Max.Z >= bbox.Min.Z && b.Min.Z <= bbox.Max.Z)
				{
					collisionGrid[i, j].Add(e);
				}
			}
		}
	}

	public bool GetSearchExtents(ref BoundingSphere e, ref Vector4 extents)
	{
		float num = e.Center.X - e.Radius;
		float num2 = e.Center.Z - e.Radius;
		float num3 = e.Center.X + e.Radius;
		float num4 = e.Center.Z + e.Radius;
		if (num3 >= Min.X && num <= Max.X && num4 >= Min.Z && num2 <= Max.Z)
		{
			num -= Min.X;
			num2 -= Min.Z;
			num3 -= Min.X;
			num4 -= Min.Z;
			int num5 = (int)(num / 1024f);
			int num6 = (int)(num2 / 1024f);
			int num7 = (int)(num3 / 1024f);
			int num8 = (int)(num4 / 1024f);
			extents.X = ((num5 > 0) ? num5 : 0);
			extents.Y = ((num6 > 0) ? num6 : 0);
			extents.Z = ((num7 < nXGrid) ? num7 : (nXGrid - 1));
			extents.W = ((num8 < nZGrid) ? num8 : (nZGrid - 1));
			return true;
		}
		return false;
	}
}
