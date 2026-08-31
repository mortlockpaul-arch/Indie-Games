using System;
using System.Collections.Generic;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using BEPUphysics.ResourceManagement;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionShapes.ConvexShapes;

/// <summary>
///  Convex wrapping around a point set.
/// </summary>
public class ConvexHullShape : ConvexShape
{
	private RawList<Vector3> vertices;

	/// <summary>
	///  Gets the point set of the convex hull.
	/// </summary>
	public ReadOnlyList<Vector3> Vertices => new ReadOnlyList<Vector3>(vertices);

	/// <summary>
	///  Constructs a new convex hull shape.
	///  The point set will be recentered on the local origin.
	///  If that offset is needed, use the other constructor which outputs the computed center.
	/// </summary>
	/// <param name="vertices">Point set to use to construct the convex hull.</param>
	/// <exception cref="T:System.ArgumentException">Thrown when the point set is empty.</exception>
	public ConvexHullShape(IList<Vector3> vertices)
	{
		if (vertices.Count == 0)
		{
			throw new ArgumentException("Vertices list used to create a ConvexHullShape cannot be empty.");
		}
		RawList<Vector3> vectorList = Resources.GetVectorList();
		ComputeCenter(vertices, vectorList);
		this.vertices = new RawList<Vector3>(vectorList);
		Resources.GiveBack(vectorList);
		OnShapeChanged();
	}

	/// <summary>
	///  Constructs a new convex hull shape.
	///  The point set will be recentered on the local origin.
	/// </summary>
	/// <param name="vertices">Point set to use to construct the convex hull.</param>
	///  <param name="center">Computed center of the convex hull shape prior to recentering.</param>
	/// <exception cref="T:System.ArgumentException">Thrown when the point set is empty.</exception>
	public ConvexHullShape(IList<Vector3> vertices, out Vector3 center)
	{
		if (vertices.Count == 0)
		{
			throw new ArgumentException("Vertices list used to create a ConvexHullShape cannot be empty.");
		}
		RawList<Vector3> vectorList = Resources.GetVectorList();
		center = ComputeCenter(vertices, vectorList);
		this.vertices = new RawList<Vector3>(vectorList);
		Resources.GiveBack(vectorList);
		OnShapeChanged();
	}

	/// <summary>
	///  Constructs a new convex hull shape.
	///  The point set will be recentered on the local origin.
	/// </summary>
	/// <param name="vertices">Point set to use to construct the convex hull.</param>
	///  <param name="center">Computed center of the convex hull shape prior to recentering.</param>
	///  <param name="outputHullTriangleIndices">Triangle indices computed on the surface of the point set.</param>
	///  <param name="outputUniqueSurfaceVertices">Unique vertices on the surface of the convex hull.</param>
	/// <exception cref="T:System.ArgumentException">Thrown when the point set is empty.</exception>
	public ConvexHullShape(IList<Vector3> vertices, out Vector3 center, IList<int> outputHullTriangleIndices, IList<Vector3> outputUniqueSurfaceVertices)
	{
		if (vertices.Count == 0)
		{
			throw new ArgumentException("Vertices list used to create a ConvexHullShape cannot be empty.");
		}
		center = ComputeCenter(vertices, outputHullTriangleIndices, outputUniqueSurfaceVertices);
		this.vertices = new RawList<Vector3>(outputUniqueSurfaceVertices);
		OnShapeChanged();
	}

	/// <summary>
	/// Gets the bounding box of the shape given a transform.
	/// </summary>
	/// <param name="shapeTransform">Transform to use.</param>
	/// <param name="boundingBox">Bounding box of the transformed shape.</param>
	public override void GetBoundingBox(ref RigidTransform shapeTransform, out BoundingBox boundingBox)
	{
		boundingBox = default(BoundingBox);
		Matrix3X3.CreateFromQuaternion(ref shapeTransform.Orientation, out var result);
		Vector3 vector = new Vector3(result.M11, result.M21, result.M31);
		Vector3 vector2 = new Vector3(result.M12, result.M22, result.M32);
		Vector3 vector3 = new Vector3(result.M13, result.M23, result.M33);
		Vector3.Dot(ref vertices.Elements[0], ref vector, out var result2);
		float num = result2;
		Vector3.Dot(ref vertices.Elements[0], ref vector2, out var result3);
		float num2 = result3;
		Vector3.Dot(ref vertices.Elements[0], ref vector3, out var result4);
		float num3 = result4;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		int num8 = 0;
		int num9 = 0;
		for (int i = 1; i < vertices.count; i++)
		{
			Vector3.Dot(ref vertices.Elements[i], ref vector, out var result5);
			if (result5 < num)
			{
				num = result5;
				num4 = i;
			}
			else if (result5 > result2)
			{
				result2 = result5;
				num5 = i;
			}
			Vector3.Dot(ref vertices.Elements[i], ref vector2, out result5);
			if (result5 < num2)
			{
				num2 = result5;
				num6 = i;
			}
			else if (result5 > result3)
			{
				result3 = result5;
				num7 = i;
			}
			Vector3.Dot(ref vertices.Elements[i], ref vector3, out result5);
			if (result5 < num3)
			{
				num3 = result5;
				num8 = i;
			}
			else if (result5 > result4)
			{
				result4 = result5;
				num9 = i;
			}
		}
		Matrix3X3.Transform(ref vertices.Elements[num4], ref result, out var result6);
		Matrix3X3.Transform(ref vertices.Elements[num5], ref result, out var result7);
		Matrix3X3.Transform(ref vertices.Elements[num6], ref result, out var result8);
		Matrix3X3.Transform(ref vertices.Elements[num7], ref result, out var result9);
		Matrix3X3.Transform(ref vertices.Elements[num8], ref result, out var result10);
		Matrix3X3.Transform(ref vertices.Elements[num9], ref result, out var result11);
		boundingBox.Max.X = shapeTransform.Position.X + collisionMargin + result7.X;
		boundingBox.Max.Y = shapeTransform.Position.Y + collisionMargin + result9.Y;
		boundingBox.Max.Z = shapeTransform.Position.Z + collisionMargin + result11.Z;
		boundingBox.Min.X = shapeTransform.Position.X - collisionMargin + result6.X;
		boundingBox.Min.Y = shapeTransform.Position.Y - collisionMargin + result8.Y;
		boundingBox.Min.Z = shapeTransform.Position.Z - collisionMargin + result10.Z;
	}

	public override void GetLocalExtremePointWithoutMargin(ref Vector3 direction, out Vector3 extremePoint)
	{
		Vector3.Dot(ref vertices.Elements[0], ref direction, out var result);
		int num = 0;
		for (int i = 1; i < vertices.count; i++)
		{
			Vector3.Dot(ref vertices.Elements[i], ref direction, out var result2);
			if (result2 > result)
			{
				result = result2;
				num = i;
			}
		}
		extremePoint = vertices.Elements[num];
	}

	/// <summary>
	/// Computes the center of the shape.  This can be considered its 
	/// center of mass.
	/// </summary>
	/// <returns>Center of the shape.</returns>
	public override Vector3 ComputeCenter()
	{
		return ComputeCenter(vertices);
	}

	/// <summary>
	/// Computes the center of the shape.  This can be considered its 
	/// center of mass.  This calculation is often associated with the 
	/// volume calculation, which is given by this method as well.
	/// </summary>
	/// <param name="volume">Volume of the shape.</param>
	/// <returns>Center of the shape.</returns>
	public override Vector3 ComputeCenter(out float volume)
	{
		return ComputeCenter(vertices, out volume);
	}

	/// <summary>
	/// Computes the volume of the shape.
	/// </summary>
	/// <returns>Volume of the shape.</returns>
	public override float ComputeVolume()
	{
		ComputeCenter(out var volume);
		return volume;
	}

	/// <summary>
	///  Computes the center, volume, and surface triangles of the convex hull shape.
	/// </summary>
	/// <param name="volume">Volume of the hull.</param>
	/// <param name="outputSurfaceTriangles">Surface triangles of the hull.</param>
	/// <param name="outputLocalSurfaceVertices">Surface vertices recentered on the center of volume. </param>
	/// <returns>Center of the hull.</returns>
	public Vector3 ComputeCenter(out float volume, IList<int> outputSurfaceTriangles, IList<Vector3> outputLocalSurfaceVertices)
	{
		return ComputeCenter(vertices, out volume, outputSurfaceTriangles, outputLocalSurfaceVertices);
	}

	/// <summary>
	///  Computes the center of a convex hull defined by the point set.
	/// </summary>
	/// <param name="vertices">Point set defining the convex hull.</param>
	/// <returns>Center of the convex hull.</returns>
	public static Vector3 ComputeCenter(IList<Vector3> vertices)
	{
		float volume;
		return ComputeCenter(vertices, out volume);
	}

	/// <summary>
	///  Computes the center and volume of a convex hull defined by a pointset.
	/// </summary>
	/// <param name="vertices">Point set defining the convex hull.</param>
	/// <param name="volume">Volume of the convex hull.</param>
	/// <returns>Center of the convex hull.</returns>
	public static Vector3 ComputeCenter(IList<Vector3> vertices, out float volume)
	{
		RawList<Vector3> vectorList = Resources.GetVectorList();
		RawList<int> intList = Resources.GetIntList();
		Vector3 result = ComputeCenter(vertices, out volume, intList, vectorList);
		Resources.GiveBack(vectorList);
		Resources.GiveBack(intList);
		return result;
	}

	/// <summary>
	///  Computes the center and surface triangles of a convex hull defined by a point set.
	/// </summary>
	/// <param name="vertices">Point set defining the convex hull.</param>
	/// <param name="outputLocalSurfaceVertices">Local positions of vertices on the convex hull.</param>
	/// <returns>Center of the convex hull.</returns>
	public static Vector3 ComputeCenter(IList<Vector3> vertices, IList<Vector3> outputLocalSurfaceVertices)
	{
		RawList<int> intList = Resources.GetIntList();
		Vector3 result = ComputeCenter(vertices, out var _, intList, outputLocalSurfaceVertices);
		Resources.GiveBack(intList);
		return result;
	}

	/// <summary>
	///  Computes the center and surface triangles of a convex hull defined by a point set.
	/// </summary>
	/// <param name="vertices">Point set defining the convex hull.</param>
	/// <param name="outputSurfaceTriangles">Indices of surface triangles of the convex hull.</param>
	/// <param name="outputLocalSurfaceVertices">Local positions of vertices on the convex hull.</param>
	/// <returns>Center of the convex hull.</returns>
	public static Vector3 ComputeCenter(IList<Vector3> vertices, IList<int> outputSurfaceTriangles, IList<Vector3> outputLocalSurfaceVertices)
	{
		float volume;
		return ComputeCenter(vertices, out volume, outputSurfaceTriangles, outputLocalSurfaceVertices);
	}

	/// <summary>
	///  Computes the center, volume, and surface triangles of a convex hull defined by a point set.
	/// </summary>
	/// <param name="vertices">Point set defining the convex hull.</param>
	/// <param name="volume">Volume of the convex hull.</param>
	/// <param name="outputSurfaceTriangles">Indices of surface triangles of the convex hull.</param>
	/// <param name="outputLocalSurfaceVertices">Local positions of vertices on the convex hull.</param>
	/// <returns>Center of the convex hull.</returns>
	public static Vector3 ComputeCenter(IList<Vector3> vertices, out float volume, IList<int> outputSurfaceTriangles, IList<Vector3> outputLocalSurfaceVertices)
	{
		Vector3 zeroVector = Toolbox.ZeroVector;
		for (int i = 0; i < vertices.Count; i++)
		{
			zeroVector += vertices[i];
		}
		zeroVector /= (float)vertices.Count;
		ConvexHullHelper.GetConvexHull(vertices, outputSurfaceTriangles, outputLocalSurfaceVertices);
		volume = 0f;
		RawList<float> floatList = Resources.GetFloatList();
		RawList<Vector3> vectorList = Resources.GetVectorList();
		for (int j = 0; j < outputSurfaceTriangles.Count; j += 3)
		{
			floatList.Add(Vector3.Dot(Vector3.Cross(vertices[outputSurfaceTriangles[j + 1]] - vertices[outputSurfaceTriangles[j]], vertices[outputSurfaceTriangles[j + 2]] - vertices[outputSurfaceTriangles[j]]), zeroVector - vertices[outputSurfaceTriangles[j]]));
			volume += floatList[j / 3];
			vectorList.Add((vertices[outputSurfaceTriangles[j]] + vertices[outputSurfaceTriangles[j + 1]] + vertices[outputSurfaceTriangles[j + 2]] + zeroVector) / 4f);
		}
		Vector3 zeroVector2 = Toolbox.ZeroVector;
		for (int k = 0; k < vectorList.Count; k++)
		{
			zeroVector2 += vectorList[k] * (floatList[k] / volume);
		}
		volume /= 6f;
		for (int l = 0; l < outputLocalSurfaceVertices.Count; l++)
		{
			outputLocalSurfaceVertices[l] -= zeroVector2;
		}
		Resources.GiveBack(vectorList);
		Resources.GiveBack(floatList);
		return zeroVector2;
	}

	/// <summary>
	/// Computes the volume distribution of the shape as well as its volume.
	/// The volume distribution can be used to compute inertia tensors when
	/// paired with mass and other tuning factors.
	/// </summary>
	/// <param name="volume">Volume of the shape.</param>
	/// <returns>Volume distribution of the shape.</returns>
	public override Matrix3X3 ComputeVolumeDistribution(out float volume)
	{
		RawList<int> intList = Resources.GetIntList();
		RawList<Vector3> vectorList = Resources.GetVectorList();
		ComputeCenter(out volume, intList, vectorList);
		Matrix3X3 result = ComputeVolumeDistribution(volume, intList);
		Resources.GiveBack(intList);
		Resources.GiveBack(vectorList);
		return result;
	}

	/// <summary>
	///  Computes the volume distribution of the convex hull, its volume, and its surface triangles.
	/// </summary>
	/// <param name="volume">Volume of the convex hull.</param>
	/// <param name="localSurfaceTriangles">Surface triangles of the convex hull.</param>
	/// <returns>Volume distribution of the convex hull.</returns>
	public Matrix3X3 ComputeVolumeDistribution(float volume, IList<int> localSurfaceTriangles)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 1f / volume;
		float num8 = num7 / 60f;
		float num9 = (0f - num7) / 120f;
		for (int i = 0; i < localSurfaceTriangles.Count; i += 3)
		{
			Vector3 vector = vertices[localSurfaceTriangles[i]];
			Vector3 vector2 = vertices[localSurfaceTriangles[i + 1]];
			Vector3 vector3 = vertices[localSurfaceTriangles[i + 2]];
			float num10 = Math.Abs(vector.X * (vector2.Y * vector3.Z - vector2.Z * vector3.Y) - vector2.X * (vector.Y * vector3.Z - vector.Z * vector3.Y) + vector3.X * (vector.Y * vector2.Z - vector.Z * vector2.Y));
			num += num10 * (vector.Y * vector.Y + vector.Y * vector2.Y + vector2.Y * vector2.Y + vector.Y * vector3.Y + vector2.Y * vector3.Y + vector3.Y * vector3.Y + vector.Z * vector.Z + vector.Z * vector2.Z + vector2.Z * vector2.Z + vector.Z * vector3.Z + vector2.Z * vector3.Z + vector3.Z * vector3.Z);
			num2 += num10 * (vector.X * vector.X + vector.X * vector2.X + vector2.X * vector2.X + vector.X * vector3.X + vector2.X * vector3.X + vector3.X * vector3.X + vector.Z * vector.Z + vector.Z * vector2.Z + vector2.Z * vector2.Z + vector.Z * vector3.Z + vector2.Z * vector3.Z + vector3.Z * vector3.Z);
			num3 += num10 * (vector.X * vector.X + vector.X * vector2.X + vector2.X * vector2.X + vector.X * vector3.X + vector2.X * vector3.X + vector3.X * vector3.X + vector.Y * vector.Y + vector.Y * vector2.Y + vector2.Y * vector2.Y + vector.Y * vector3.Y + vector2.Y * vector3.Y + vector3.Y * vector3.Y);
			num4 += num10 * (2f * vector.Y * vector.Z + vector2.Y * vector.Z + vector3.Y * vector.Z + vector.Y * vector2.Z + 2f * vector2.Y * vector2.Z + vector3.Y * vector2.Z + vector.Y * vector3.Z + vector2.Y * vector3.Z + 2f * vector3.Y * vector3.Z);
			num5 += num10 * (2f * vector.X * vector.Z + vector2.X * vector.Z + vector3.X * vector.Z + vector.X * vector2.Z + 2f * vector2.X * vector2.Z + vector3.X * vector2.Z + vector.X * vector3.Z + vector2.X * vector3.Z + 2f * vector3.X * vector3.Z);
			num6 += num10 * (2f * vector.X * vector.Y + vector2.X * vector.Y + vector3.X * vector.Y + vector.X * vector2.Y + 2f * vector2.X * vector2.Y + vector3.X * vector2.Y + vector.X * vector3.Y + vector2.X * vector3.Y + 2f * vector3.X * vector3.Y);
		}
		num *= num8;
		num2 *= num8;
		num3 *= num8;
		num4 *= num9;
		num5 *= num9;
		num6 *= num9;
		return new Matrix3X3(num, num5, num6, num5, num2, num4, num6, num4, num3);
	}

	/// <summary>
	/// Computes the maximum radius of the shape.
	/// This is often larger than the actual maximum radius;
	/// it is simply an approximation that avoids underestimating.
	/// </summary>
	/// <returns>Maximum radius of the shape.</returns>
	public override float ComputeMaximumRadius()
	{
		float num = 0f;
		for (int i = 0; i < vertices.count; i++)
		{
			float num2 = vertices.Elements[i].Length();
			if (num < num2)
			{
				num = num2;
			}
		}
		return num + collisionMargin;
	}

	/// <summary>
	///  Computes the minimum radius of the shape.
	///  This is often smaller than the actual minimum radius;
	///  it is simply an approximation that avoids overestimating.
	/// </summary>
	/// <returns>Minimum radius of the shape.</returns>
	public override float ComputeMinimumRadius()
	{
		Vector3 direction = new Vector3(1f, 1f, 1f);
		GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint);
		direction = new Vector3(-1f, -1f, 1f);
		GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint2);
		direction = new Vector3(-1f, 1f, -1f);
		GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint3);
		direction = new Vector3(1f, -1f, -1f);
		GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint4);
		Vector3.Subtract(ref extremePoint2, ref extremePoint, out var result);
		Vector3.Subtract(ref extremePoint2, ref extremePoint3, out var result2);
		Vector3.Subtract(ref extremePoint3, ref extremePoint, out var result3);
		Vector3.Subtract(ref extremePoint4, ref extremePoint, out var result4);
		Vector3.Subtract(ref extremePoint4, ref extremePoint3, out var result5);
		Vector3.Cross(ref result3, ref result, out var result6);
		Vector3.Cross(ref result5, ref result2, out var result7);
		Vector3.Cross(ref result4, ref result3, out var result8);
		Vector3.Cross(ref result, ref result4, out var result9);
		Vector3.Dot(ref extremePoint, ref result6, out var result10);
		Vector3.Dot(ref extremePoint3, ref result7, out var result11);
		Vector3.Dot(ref extremePoint, ref result8, out var result12);
		Vector3.Dot(ref extremePoint, ref result9, out var result13);
		result10 /= result6.Length();
		result11 /= result7.Length();
		result12 /= result8.Length();
		result13 /= result9.Length();
		return collisionMargin + Math.Min(result10, Math.Min(result11, Math.Min(result12, result13)));
	}

	/// <summary>
	/// Retrieves an instance of an EntityCollidable that uses this EntityShape.  Mainly used by compound bodies.
	/// </summary>
	/// <returns>EntityCollidable that uses this shape.</returns>
	public override EntityCollidable GetCollidableInstance()
	{
		return new ConvexCollidable<ConvexHullShape>(this);
	}
}
