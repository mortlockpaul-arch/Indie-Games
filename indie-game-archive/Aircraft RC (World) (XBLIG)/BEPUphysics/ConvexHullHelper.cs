using System;
using System.Collections.Generic;
using BEPUphysics.DataStructures;
using BEPUphysics.ResourceManagement;
using Microsoft.Xna.Framework;

namespace BEPUphysics;

/// <summary>
/// Processes vertex data into convex hulls.
/// </summary>
public static class ConvexHullHelper
{
	/// <summary>
	/// Represents a cell in space which is already occupied by a point.  Any other points which resolve to the same cell are considered redundant.
	/// </summary>
	public struct BlockedCell : IEquatable<BlockedCell>
	{
		public int X;

		public int Y;

		public int Z;

		public override int GetHashCode()
		{
			return (int)((long)X * 961748927L + (long)Y * 961748941L + (long)Z * 982451653L);
		}

		public override bool Equals(object obj)
		{
			return Equals((BlockedCell)obj);
		}

		public bool Equals(BlockedCell other)
		{
			if (other.X == X && other.Y == Y)
			{
				return other.Z == Z;
			}
			return false;
		}
	}

	/// <summary>
	/// Contains and manufactures cell sets used by the redundant point remover.  To minimize memory usage, this can be cleared
	/// after using the RemoveRedundantPoints if it isn't going to be used again.
	/// </summary>
	public static LockingResourcePool<BEPUphysics.DataStructures.HashSet<BlockedCell>> BlockedCellSets = new LockingResourcePool<BEPUphysics.DataStructures.HashSet<BlockedCell>>();

	/// <summary>
	/// Removes redundant points.  Two points are redundant if they occupy the same hash grid cell of size 0.001.
	/// </summary>
	/// <param name="points">List of points to prune.</param>
	public static void RemoveRedundantPoints(IList<Vector3> points)
	{
		RemoveRedundantPoints(points, 0.001);
	}

	/// <summary>
	/// Removes redundant points.  Two points are redundant if they occupy the same hash grid cell.
	/// </summary>
	/// <param name="points">List of points to prune.</param>
	/// <param name="cellSize">Size of cells to determine redundancy.</param>
	public static void RemoveRedundantPoints(IList<Vector3> points, double cellSize)
	{
		RawList<Vector3> vectorList = Resources.GetVectorList();
		vectorList.AddRange(points);
		RemoveRedundantPoints(vectorList, cellSize);
		points.Clear();
		for (int i = 0; i < vectorList.count; i++)
		{
			points.Add(vectorList.Elements[i]);
		}
		Resources.GiveBack(vectorList);
	}

	/// <summary>
	/// Removes redundant points.  Two points are redundant if they occupy the same hash grid cell of size 0.001.
	/// </summary>
	/// <param name="points">List of points to prune.</param>
	public static void RemoveRedundantPoints(RawList<Vector3> points)
	{
		RemoveRedundantPoints(points, 0.001);
	}

	/// <summary>
	/// Removes redundant points.  Two points are redundant if they occupy the same hash grid cell.
	/// </summary>
	/// <param name="points">List of points to prune.</param>
	/// <param name="cellSize">Size of cells to determine redundancy.</param>
	public static void RemoveRedundantPoints(RawList<Vector3> points, double cellSize)
	{
		BEPUphysics.DataStructures.HashSet<BlockedCell> hashSet = BlockedCellSets.Take();
		for (int num = points.count - 1; num >= 0; num--)
		{
			Vector3 vector = points.Elements[num];
			BlockedCell item = new BlockedCell
			{
				X = (int)Math.Floor((double)vector.X / cellSize),
				Y = (int)Math.Floor((double)vector.Y / cellSize),
				Z = (int)Math.Floor((double)vector.Z / cellSize)
			};
			if (hashSet.Contains(item))
			{
				points.FastRemoveAt(num);
			}
			else
			{
				hashSet.Add(item);
			}
		}
		hashSet.Clear();
		BlockedCellSets.GiveBack(hashSet);
	}

	/// <summary>
	/// Identifies the indices of points in a set which are on the outer convex hull of the set.
	/// </summary>
	/// <param name="points">List of points in the set.</param>
	/// <param name="indices">List of indices composing the triangulated surface of the convex hull.
	/// Each group of 3 indices represents a triangle on the surface of the hull.</param>
	public static void GetConvexHull(IList<Vector3> points, IList<int> indices)
	{
		RawList<Vector3> vectorList = Resources.GetVectorList();
		RawList<int> intList = Resources.GetIntList();
		vectorList.AddRange(points);
		GetConvexHull(vectorList, intList);
		Resources.GiveBack(vectorList);
		for (int i = 0; i < intList.count; i++)
		{
			indices.Add(intList[i]);
		}
		Resources.GiveBack(intList);
	}

	/// <summary>
	/// Identifies the points on the surface of hull.
	/// </summary>
	/// <param name="points">List of points in the set.</param>
	/// <param name="outputSurfacePoints">Unique points on the surface of the convex hull.</param>
	public static void GetConvexHull(IList<Vector3> points, IList<Vector3> outputSurfacePoints)
	{
		RawList<Vector3> vectorList = Resources.GetVectorList();
		vectorList.AddRange(points);
		GetConvexHull(vectorList, outputSurfacePoints);
		Resources.GiveBack(vectorList);
	}

	/// <summary>
	/// Identifies the points on the surface of hull.
	/// </summary>
	/// <param name="points">List of points in the set.</param>
	/// <param name="outputSurfacePoints">Unique points on the surface of the convex hull.</param>
	public static void GetConvexHull(RawList<Vector3> points, IList<Vector3> outputSurfacePoints)
	{
		RawList<int> intList = Resources.GetIntList();
		GetConvexHull(points, intList, outputSurfacePoints);
		Resources.GiveBack(intList);
	}

	/// <summary>
	/// Identifies the points on the surface of hull.
	/// </summary>
	/// <param name="points">List of points in the set.</param>
	/// <param name="outputIndices">List of indices composing the triangulated surface of the convex hull.
	/// Each group of 3 indices represents a triangle on the surface of the hull.</param>
	/// <param name="outputSurfacePoints">Unique points on the surface of the convex hull.</param>
	public static void GetConvexHull(IList<Vector3> points, IList<int> outputIndices, IList<Vector3> outputSurfacePoints)
	{
		RawList<Vector3> vectorList = Resources.GetVectorList();
		RawList<int> intList = Resources.GetIntList();
		vectorList.AddRange(points);
		GetConvexHull(vectorList, intList, outputSurfacePoints);
		Resources.GiveBack(vectorList);
		for (int i = 0; i < intList.count; i++)
		{
			outputIndices.Add(intList[i]);
		}
		Resources.GiveBack(intList);
	}

	/// <summary>
	/// Identifies the points on the surface of hull.
	/// </summary>
	/// <param name="points">List of points in the set.</param>
	/// <param name="outputIndices">List of indices composing the triangulated surface of the convex hull.
	/// Each group of 3 indices represents a triangle on the surface of the hull.</param>
	/// <param name="outputSurfacePoints">Unique points on the surface of the convex hull.</param>
	public static void GetConvexHull(RawList<Vector3> points, RawList<int> outputIndices, IList<Vector3> outputSurfacePoints)
	{
		GetConvexHull(points, outputIndices);
		BEPUphysics.DataStructures.HashSet<int> intSet = Resources.GetIntSet();
		for (int num = outputIndices.Count - 1; num >= 0; num--)
		{
			int num2 = outputIndices[num];
			if (!intSet.Contains(num2))
			{
				outputSurfacePoints.Add(points[num2]);
				intSet.Add(num2);
			}
		}
		Resources.GiveBack(intSet);
	}

	/// <summary>
	/// Identifies the indices of points in a set which are on the outer convex hull of the set.
	/// </summary>
	/// <param name="points">List of points in the set.</param>
	/// <param name="indices">List of indices composing the triangulated surface of the convex hull.
	/// Each group of 3 indices represents a triangle on the surface of the hull.</param>
	public static void GetConvexHull(RawList<Vector3> points, RawList<int> triangleIndices)
	{
		if (points.count == 0)
		{
			throw new Exception("Point set must have volume.");
		}
		RawList<int> intList = Resources.GetIntList();
		if (intList.Capacity < points.count)
		{
			intList.Capacity = points.count;
		}
		for (int i = 0; i < points.count; i++)
		{
			intList.Add(i);
		}
		ComputeInitialTetrahedron(points, intList, triangleIndices, out var centroid);
		RemoveInsidePoints(points, triangleIndices, intList);
		RawList<int> intList2 = Resources.GetIntList();
		RawList<int> intList3 = Resources.GetIntList();
		RawList<int> intList4 = Resources.GetIntList();
		while (intList.Count > 0)
		{
			for (int j = 0; j < triangleIndices.count; j += 3)
			{
				FindNormal(triangleIndices, points, j, out var normal);
				int extremePoint = GetExtremePoint(ref normal, points, intList);
				int num = intList.Elements[extremePoint];
				Vector3 value = points.Elements[num];
				Vector3.Subtract(ref value, ref points.Elements[triangleIndices.Elements[j]], out var result);
				Vector3.Dot(ref normal, ref result, out var result2);
				if (!(result2 > 0f))
				{
					continue;
				}
				intList.FastRemoveAt(extremePoint);
				intList2.Clear();
				intList3.Clear();
				for (int num2 = triangleIndices.count - 3; num2 >= 0; num2 -= 3)
				{
					if (IsTriangleVisibleFromPoint(triangleIndices, points, num2, ref value))
					{
						MaintainEdge(triangleIndices[num2], triangleIndices[num2 + 1], intList2);
						MaintainEdge(triangleIndices[num2], triangleIndices[num2 + 2], intList2);
						MaintainEdge(triangleIndices[num2 + 1], triangleIndices[num2 + 2], intList2);
						triangleIndices.FastRemoveAt(num2 + 2);
						triangleIndices.FastRemoveAt(num2 + 1);
						triangleIndices.FastRemoveAt(num2);
					}
				}
				for (int k = 0; k < intList2.Count; k += 2)
				{
					intList4.Add(intList2[k]);
					intList4.Add(intList2[k + 1]);
					intList4.Add(num);
				}
				VerifyWindings(intList4, points, ref centroid);
				triangleIndices.AddRange(intList4);
				intList4.Clear();
				RemoveInsidePoints(points, triangleIndices, intList);
				break;
			}
		}
		Resources.GiveBack(intList);
		Resources.GiveBack(intList2);
		Resources.GiveBack(intList3);
		Resources.GiveBack(intList4);
	}

	private static void MaintainEdge(int a, int b, RawList<int> edges)
	{
		bool flag = false;
		int num = 0;
		for (int i = 0; i < edges.Count; i += 2)
		{
			if ((edges[i] == a && edges[i + 1] == b) || (edges[i] == b && edges[i + 1] == a))
			{
				flag = true;
				num = i;
			}
		}
		if (!flag)
		{
			edges.Add(a);
			edges.Add(b);
		}
		else
		{
			edges.FastRemoveAt(num + 1);
			edges.FastRemoveAt(num);
		}
	}

	private static int GetExtremePoint(ref Vector3 direction, RawList<Vector3> points, RawList<int> outsidePoints)
	{
		float num = float.MinValue;
		int result = 0;
		for (int i = 0; i < outsidePoints.count; i++)
		{
			Vector3.Dot(ref points.Elements[outsidePoints[i]], ref direction, out var result2);
			if (result2 > num)
			{
				num = result2;
				result = i;
			}
		}
		return result;
	}

	private static void GetExtremePoints(ref Vector3 direction, RawList<Vector3> points, out float maximumDot, out float minimumDot, out int maximumIndex, out int minimumIndex)
	{
		maximumIndex = 0;
		minimumIndex = 0;
		Vector3.Dot(ref points.Elements[0], ref direction, out var result);
		minimumDot = result;
		maximumDot = result;
		for (int i = 1; i < points.count; i++)
		{
			Vector3.Dot(ref points.Elements[i], ref direction, out result);
			if (result > maximumDot)
			{
				maximumDot = result;
				maximumIndex = i;
			}
			else if (result < minimumDot)
			{
				minimumDot = result;
				minimumIndex = i;
			}
		}
	}

	private static void ComputeInitialTetrahedron(RawList<Vector3> points, RawList<int> outsidePoints, RawList<int> triangleIndices, out Vector3 centroid)
	{
		float num = float.MaxValue;
		float num2 = float.MinValue;
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < points.count; i++)
		{
			Vector3 vector = points.Elements[i];
			if (vector.X > num2)
			{
				num2 = vector.X;
				num4 = i;
			}
			else if (vector.X < num)
			{
				num = vector.X;
				num3 = i;
			}
		}
		int num5 = num3;
		int num6 = num4;
		if (num5 == num6)
		{
			throw new Exception("Point set is degenerate; convex hulls must have volume.");
		}
		Vector3.Subtract(ref points.Elements[num6], ref points.Elements[num5], out var result);
		Vector3.Cross(ref result, ref Toolbox.UpVector, out var result2);
		if (result2.LengthSquared() < 1E-07f)
		{
			Vector3.Cross(ref result, ref Toolbox.RightVector, out result2);
		}
		GetExtremePoints(ref result2, points, out var maximumDot, out var minimumDot, out var maximumIndex, out var minimumIndex);
		Vector3.Dot(ref result2, ref points.Elements[num5], out var result3);
		int num7 = ((!(Math.Abs(result3 - minimumDot) > Math.Abs(result3 - maximumDot))) ? maximumIndex : minimumIndex);
		if (num5 == num7 || num6 == num7)
		{
			throw new Exception("Point set is degenerate; convex hulls must have volume.");
		}
		Vector3.Subtract(ref points.Elements[num7], ref points.Elements[num5], out var result4);
		Vector3.Cross(ref result, ref result4, out result2);
		GetExtremePoints(ref result2, points, out maximumDot, out minimumDot, out maximumIndex, out minimumIndex);
		Vector3.Dot(ref result2, ref points.Elements[num5], out result3);
		int num8 = ((!(Math.Abs(result3 - minimumDot) > Math.Abs(result3 - maximumDot))) ? maximumIndex : minimumIndex);
		if (num5 == num8 || num6 == num8 || num7 == num8)
		{
			throw new Exception("Point set is degenerate; convex hulls must have volume.");
		}
		triangleIndices.Add(num5);
		triangleIndices.Add(num6);
		triangleIndices.Add(num7);
		triangleIndices.Add(num5);
		triangleIndices.Add(num6);
		triangleIndices.Add(num8);
		triangleIndices.Add(num5);
		triangleIndices.Add(num7);
		triangleIndices.Add(num8);
		triangleIndices.Add(num6);
		triangleIndices.Add(num7);
		triangleIndices.Add(num8);
		Vector3.Add(ref points.Elements[num5], ref points.Elements[num6], out centroid);
		Vector3.Add(ref centroid, ref points.Elements[num7], out centroid);
		Vector3.Add(ref centroid, ref points.Elements[num8], out centroid);
		Vector3.Multiply(ref centroid, 0.25f, out centroid);
		for (int j = 0; j < triangleIndices.count; j += 3)
		{
			Vector3 value = points.Elements[triangleIndices.Elements[j]];
			Vector3 value2 = points.Elements[triangleIndices.Elements[j + 1]];
			Vector3 value3 = points.Elements[triangleIndices.Elements[j + 2]];
			Vector3.Subtract(ref value2, ref value, out result);
			Vector3.Subtract(ref value3, ref value, out result4);
			Vector3.Cross(ref result4, ref result, out var result5);
			Vector3.Subtract(ref value, ref centroid, out var result6);
			Vector3.Dot(ref result6, ref result5, out var result7);
			if (Math.Abs(result7) < 1E-05f)
			{
				throw new Exception("Point set is degenerate; convex hulls must have volume.");
			}
			if (result7 < 0f)
			{
				int num9 = triangleIndices.Elements[j];
				triangleIndices.Elements[j] = triangleIndices.Elements[j + 1];
				triangleIndices.Elements[j + 1] = num9;
			}
		}
	}

	private static void RemoveInsidePoints(RawList<Vector3> points, RawList<int> triangleIndices, RawList<int> outsidePoints)
	{
		RawList<int> intList = Resources.GetIntList();
		intList.AddRange(outsidePoints);
		outsidePoints.Clear();
		for (int i = 0; i < triangleIndices.count; i += 3)
		{
			if (intList.count <= 0)
			{
				break;
			}
			FindNormal(triangleIndices, points, i, out var normal);
			Vector3 value = points.Elements[triangleIndices.Elements[i]];
			for (int num = intList.count - 1; num >= 0; num--)
			{
				Vector3.Subtract(ref points.Elements[intList.Elements[num]], ref value, out var result);
				Vector3.Dot(ref result, ref normal, out var result2);
				if (result2 > 0f)
				{
					outsidePoints.Add(intList.Elements[num]);
					intList.FastRemoveAt(num);
				}
			}
		}
		Resources.GiveBack(intList);
	}

	private static void FindNormal(RawList<int> indices, RawList<Vector3> points, int triangleIndex, out Vector3 normal)
	{
		Vector3 value = points.Elements[indices.Elements[triangleIndex]];
		Vector3.Subtract(ref points.Elements[indices.Elements[triangleIndex + 1]], ref value, out var result);
		Vector3.Subtract(ref points.Elements[indices.Elements[triangleIndex + 2]], ref value, out var result2);
		Vector3.Cross(ref result2, ref result, out normal);
	}

	private static bool IsTriangleVisibleFromPoint(RawList<int> indices, RawList<Vector3> points, int triangleIndex, ref Vector3 point)
	{
		Vector3 value = points.Elements[indices.Elements[triangleIndex]];
		Vector3.Subtract(ref points.Elements[indices.Elements[triangleIndex + 1]], ref value, out var result);
		Vector3.Subtract(ref points.Elements[indices.Elements[triangleIndex + 2]], ref value, out var result2);
		Vector3.Cross(ref result2, ref result, out var result3);
		Vector3.Subtract(ref point, ref value, out var result4);
		Vector3.Dot(ref result4, ref result3, out var result5);
		return result5 >= 0f;
	}

	private static void VerifyWindings(RawList<int> newIndices, RawList<Vector3> points, ref Vector3 centroid)
	{
		for (int i = 0; i < newIndices.Count; i += 3)
		{
			if (IsTriangleVisibleFromPoint(newIndices, points, i, ref centroid))
			{
				int value = newIndices[i + 1];
				newIndices[i + 1] = newIndices[i + 2];
				newIndices[i + 2] = value;
			}
		}
	}
}
