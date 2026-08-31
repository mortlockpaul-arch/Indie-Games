using System;
using System.Collections.Generic;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using BEPUphysics.ResourceManagement;
using Microsoft.Xna.Framework;

namespace BEPUphysics;

/// <summary>
/// Helper class with many algorithms for intersection testing and 3D math.
/// </summary>
public static class Toolbox
{
	/// <summary>
	/// Large tolerance value.
	/// </summary>
	public const float BigEpsilon = 1E-05f;

	/// <summary>
	/// Tolerance value.
	/// </summary>
	public const float Epsilon = 1E-07f;

	/// <summary>
	/// Represents an invalid Vector3.
	/// </summary>
	public static readonly Vector3 NoVector = new Vector3(float.MinValue, float.MinValue, float.MinValue);

	/// <summary>
	/// Reference for a vector with dimensions (0,0,1).
	/// </summary>
	public static Vector3 BackVector = Vector3.Backward;

	/// <summary>
	/// Reference for a vector with dimensions (0,-1,0).
	/// </summary>
	public static Vector3 DownVector = Vector3.Down;

	/// <summary>
	/// Reference for a vector with dimensions (0,0,-1).
	/// </summary>
	public static Vector3 ForwardVector = Vector3.Forward;

	/// <summary>
	/// Refers to the identity quaternion.
	/// </summary>
	public static Quaternion IdentityOrientation = Quaternion.Identity;

	/// <summary>
	/// Reference for a vector with dimensions (-1,0,0).
	/// </summary>
	public static Vector3 LeftVector = Vector3.Left;

	/// <summary>
	/// Reference for a vector with dimensions (1,0,0).
	/// </summary>
	public static Vector3 RightVector = Vector3.Right;

	/// <summary>
	/// Reference for a vector with dimensions (0,1,0).
	/// </summary>
	public static Vector3 UpVector = Vector3.Up;

	/// <summary>
	/// Matrix containing zeroes for every element.
	/// </summary>
	public static Matrix ZeroMatrix = new Matrix(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);

	/// <summary>
	/// Reference for a vector with dimensions (0,0,0).
	/// </summary>
	public static Vector3 ZeroVector = Vector3.Zero;

	/// <summary>
	/// Refers to the rigid identity transformation.
	/// </summary>
	public static RigidTransform RigidIdentity = RigidTransform.Identity;

	/// <summary>
	/// Determines the intersection between a ray and a triangle.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	/// <param name="maximumLength">Maximum length to travel in units of the direction's length.</param>
	/// <param name="a">First vertex of the triangle.</param>
	/// <param name="b">Second vertex of the triangle.</param>
	/// <param name="c">Third vertex of the triangle.</param>
	/// <param name="hitClockwise">True if the the triangle was hit on the clockwise face, false otherwise.</param>
	/// <param name="hit">Hit data of the ray, if any</param>
	/// <returns>Whether or not the ray and triangle intersect.</returns>
	public static bool FindRayTriangleIntersection(ref Ray ray, float maximumLength, ref Vector3 a, ref Vector3 b, ref Vector3 c, out bool hitClockwise, out RayHit hit)
	{
		hitClockwise = false;
		hit = default(RayHit);
		Vector3.Subtract(ref b, ref a, out var result);
		Vector3.Subtract(ref c, ref a, out var result2);
		Vector3.Cross(ref result, ref result2, out hit.Normal);
		if (hit.Normal.LengthSquared() < 1E-07f)
		{
			return false;
		}
		Vector3.Dot(ref ray.Direction, ref hit.Normal, out var result3);
		result3 = 0f - result3;
		hitClockwise = result3 >= 0f;
		Vector3.Subtract(ref ray.Position, ref a, out var result4);
		Vector3.Dot(ref result4, ref hit.Normal, out hit.T);
		hit.T /= result3;
		if (hit.T < 0f || hit.T > maximumLength)
		{
			return false;
		}
		Vector3.Multiply(ref ray.Direction, hit.T, out hit.Location);
		Vector3.Add(ref ray.Position, ref hit.Location, out hit.Location);
		Vector3.Subtract(ref hit.Location, ref a, out result4);
		Vector3.Dot(ref result, ref result, out var result5);
		Vector3.Dot(ref result, ref result2, out var result6);
		Vector3.Dot(ref result, ref result4, out var result7);
		Vector3.Dot(ref result2, ref result2, out var result8);
		Vector3.Dot(ref result2, ref result4, out var result9);
		float num = 1f / (result5 * result8 - result6 * result6);
		float num2 = (result8 * result7 - result6 * result9) * num;
		float num3 = (result5 * result9 - result6 * result7) * num;
		if (num2 >= -1E-05f && num3 >= -1E-05f)
		{
			return num2 + num3 <= 1.00001f;
		}
		return false;
	}

	/// <summary>
	/// Determines the intersection between a ray and a triangle.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	/// <param name="maximumLength">Maximum length to travel in units of the direction's length.</param>
	/// <param name="sidedness">Sidedness of the triangle to test.</param>
	/// <param name="a">First vertex of the triangle.</param>
	/// <param name="b">Second vertex of the triangle.</param>
	/// <param name="c">Third vertex of the triangle.</param>
	/// <param name="hit">Hit data of the ray, if any</param>
	/// <returns>Whether or not the ray and triangle intersect.</returns>
	public static bool FindRayTriangleIntersection(ref Ray ray, float maximumLength, TriangleSidedness sidedness, ref Vector3 a, ref Vector3 b, ref Vector3 c, out RayHit hit)
	{
		hit = default(RayHit);
		Vector3.Subtract(ref b, ref a, out var result);
		Vector3.Subtract(ref c, ref a, out var result2);
		Vector3.Cross(ref result, ref result2, out hit.Normal);
		if (hit.Normal.LengthSquared() < 1E-07f)
		{
			return false;
		}
		Vector3.Dot(ref ray.Direction, ref hit.Normal, out var result3);
		result3 = 0f - result3;
		switch (sidedness)
		{
		case TriangleSidedness.DoubleSided:
			if (result3 <= 0f)
			{
				Vector3.Negate(ref hit.Normal, out hit.Normal);
				result3 = 0f - result3;
			}
			break;
		case TriangleSidedness.Clockwise:
			if (result3 <= 0f)
			{
				return false;
			}
			break;
		case TriangleSidedness.Counterclockwise:
			if (result3 >= 0f)
			{
				return false;
			}
			Vector3.Negate(ref hit.Normal, out hit.Normal);
			result3 = 0f - result3;
			break;
		}
		Vector3.Subtract(ref ray.Position, ref a, out var result4);
		Vector3.Dot(ref result4, ref hit.Normal, out hit.T);
		hit.T /= result3;
		if (hit.T < 0f || hit.T > maximumLength)
		{
			return false;
		}
		Vector3.Multiply(ref ray.Direction, hit.T, out hit.Location);
		Vector3.Add(ref ray.Position, ref hit.Location, out hit.Location);
		Vector3.Subtract(ref hit.Location, ref a, out result4);
		Vector3.Dot(ref result, ref result, out var result5);
		Vector3.Dot(ref result, ref result2, out var result6);
		Vector3.Dot(ref result, ref result4, out var result7);
		Vector3.Dot(ref result2, ref result2, out var result8);
		Vector3.Dot(ref result2, ref result4, out var result9);
		float num = 1f / (result5 * result8 - result6 * result6);
		float num2 = (result8 * result7 - result6 * result9) * num;
		float num3 = (result5 * result9 - result6 * result7) * num;
		if (num2 >= -1E-05f && num3 >= -1E-05f)
		{
			return num2 + num3 <= 1.00001f;
		}
		return false;
	}

	/// <summary>
	/// Finds the intersection between the given segment and the given plane defined by three points.
	/// </summary>
	/// <param name="a">First endpoint of segment.</param>
	/// <param name="b">Second endpoint of segment.</param>
	/// <param name="d">First vertex of a triangle which lies on the plane.</param>
	/// <param name="e">Second vertex of a triangle which lies on the plane.</param>
	/// <param name="f">Third vertex of a triangle which lies on the plane.</param>
	/// <param name="q">Intersection point.</param>
	/// <returns>Whether or not the segment intersects the plane.</returns>
	public static bool GetSegmentPlaneIntersection(Vector3 a, Vector3 b, Vector3 d, Vector3 e, Vector3 f, out Vector3 q)
	{
		Plane p = default(Plane);
		p.Normal = Vector3.Cross(e - d, f - d);
		p.D = Vector3.Dot(p.Normal, d);
		float t;
		return GetSegmentPlaneIntersection(a, b, p, out t, out q);
	}

	/// <summary>
	/// Finds the intersection between the given segment and the given plane.
	/// </summary>
	/// <param name="a">First endpoint of segment.</param>
	/// <param name="b">Second enpoint of segment.</param>
	/// <param name="p">Plane for comparison.</param>
	/// <param name="q">Intersection point.</param>
	/// <returns>Whether or not the segment intersects the plane.</returns>
	public static bool GetSegmentPlaneIntersection(Vector3 a, Vector3 b, Plane p, out Vector3 q)
	{
		if (GetLinePlaneIntersection(ref a, ref b, ref p, out var t, out q) && t >= 0f)
		{
			return t <= 1f;
		}
		return false;
	}

	/// <summary>
	/// Finds the intersection between the given segment and the given plane.
	/// </summary>
	/// <param name="a">First endpoint of segment.</param>
	/// <param name="b">Second endpoint of segment.</param>
	/// <param name="p">Plane for comparison.</param>
	/// <param name="t">Interval along segment to intersection.</param>
	/// <param name="q">Intersection point.</param>
	/// <returns>Whether or not the segment intersects the plane.</returns>
	public static bool GetSegmentPlaneIntersection(Vector3 a, Vector3 b, Plane p, out float t, out Vector3 q)
	{
		if (GetLinePlaneIntersection(ref a, ref b, ref p, out t, out q) && t >= 0f)
		{
			return t <= 1f;
		}
		return false;
	}

	/// <summary>
	/// Finds the intersection between the given line and the given plane.
	/// </summary>
	/// <param name="a">First endpoint of segment defining the line.</param>
	/// <param name="b">Second endpoint of segment defining the line.</param>
	/// <param name="p">Plane for comparison.</param>
	/// <param name="t">Interval along line to intersection (A + t * AB).</param>
	/// <param name="q">Intersection point.</param>
	/// <returns>Whether or not the line intersects the plane.  If false, the line is parallel to the plane's surface.</returns>
	public static bool GetLinePlaneIntersection(ref Vector3 a, ref Vector3 b, ref Plane p, out float t, out Vector3 q)
	{
		Vector3.Subtract(ref b, ref a, out var result);
		Vector3.Dot(ref p.Normal, ref result, out var result2);
		if (result2 < 1E-07f && result2 > -1E-07f)
		{
			q = default(Vector3);
			t = float.MaxValue;
			return false;
		}
		Vector3.Dot(ref p.Normal, ref a, out var result3);
		t = (p.D - result3) / result2;
		Vector3.Multiply(ref result, t, out q);
		Vector3.Add(ref a, ref q, out q);
		return true;
	}

	/// <summary>
	/// Finds the intersection between the given ray and the given plane.
	/// </summary>
	/// <param name="ray">Ray to test against the plane.</param>
	/// <param name="p">Plane for comparison.</param>
	/// <param name="t">Interval along line to intersection (A + t * AB).</param>
	/// <param name="q">Intersection point.</param>
	/// <returns>Whether or not the line intersects the plane.  If false, the line is parallel to the plane's surface.</returns>
	public static bool GetRayPlaneIntersection(ref Ray ray, ref Plane p, out float t, out Vector3 q)
	{
		Vector3.Dot(ref p.Normal, ref ray.Direction, out var result);
		if (result < 1E-07f && result > -1E-07f)
		{
			q = default(Vector3);
			t = float.MaxValue;
			return false;
		}
		Vector3.Dot(ref p.Normal, ref ray.Position, out var result2);
		t = (p.D - result2) / result;
		Vector3.Multiply(ref ray.Direction, t, out q);
		Vector3.Add(ref ray.Position, ref q, out q);
		return t >= 0f;
	}

	/// <summary>
	/// Determines the closest point on a triangle given by points a, b, and c to point p.
	/// </summary>
	/// <param name="a">First vertex of triangle.</param>
	/// <param name="b">Second vertex of triangle.</param>
	/// <param name="c">Third vertex of triangle.</param>
	/// <param name="p">Point for comparison.</param>
	/// <param name="closestPoint">Closest point on tetrahedron to point.</param>
	/// <returns>Voronoi region containing the closest point.</returns>
	public static VoronoiRegion GetClosestPointOnTriangleToPoint(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 p, out Vector3 closestPoint)
	{
		Vector3.Subtract(ref b, ref a, out var result);
		Vector3.Subtract(ref c, ref a, out var result2);
		Vector3.Subtract(ref p, ref a, out var result3);
		Vector3.Dot(ref result, ref result3, out var result4);
		Vector3.Dot(ref result2, ref result3, out var result5);
		if (result4 <= 0f && result5 < 0f)
		{
			closestPoint = a;
			return VoronoiRegion.A;
		}
		Vector3.Subtract(ref p, ref b, out var result6);
		Vector3.Dot(ref result, ref result6, out var result7);
		Vector3.Dot(ref result2, ref result6, out var result8);
		if (result7 >= 0f && result8 <= result7)
		{
			closestPoint = b;
			return VoronoiRegion.B;
		}
		float num = result4 * result8 - result7 * result5;
		float scaleFactor;
		if (num <= 0f && result4 >= 0f && result7 <= 0f)
		{
			scaleFactor = result4 / (result4 - result7);
			Vector3.Multiply(ref result, scaleFactor, out closestPoint);
			Vector3.Add(ref closestPoint, ref a, out closestPoint);
			return VoronoiRegion.AB;
		}
		Vector3.Subtract(ref p, ref c, out var result9);
		Vector3.Dot(ref result, ref result9, out var result10);
		Vector3.Dot(ref result2, ref result9, out var result11);
		if (result11 >= 0f && result10 <= result11)
		{
			closestPoint = c;
			return VoronoiRegion.C;
		}
		float num2 = result10 * result5 - result4 * result11;
		float scaleFactor2;
		if (num2 <= 0f && result5 >= 0f && result11 <= 0f)
		{
			scaleFactor2 = result5 / (result5 - result11);
			Vector3.Multiply(ref result2, scaleFactor2, out closestPoint);
			Vector3.Add(ref closestPoint, ref a, out closestPoint);
			return VoronoiRegion.AC;
		}
		float num3 = result7 * result11 - result10 * result8;
		if (num3 <= 0f && result8 - result7 >= 0f && result10 - result11 >= 0f)
		{
			scaleFactor2 = (result8 - result7) / (result8 - result7 + (result10 - result11));
			Vector3.Subtract(ref c, ref b, out closestPoint);
			Vector3.Multiply(ref closestPoint, scaleFactor2, out closestPoint);
			Vector3.Add(ref closestPoint, ref b, out closestPoint);
			return VoronoiRegion.BC;
		}
		float num4 = 1f / (num3 + num2 + num);
		scaleFactor = num2 * num4;
		scaleFactor2 = num * num4;
		Vector3.Multiply(ref result, scaleFactor, out var result12);
		Vector3.Multiply(ref result2, scaleFactor2, out var result13);
		Vector3.Add(ref a, ref result12, out closestPoint);
		Vector3.Add(ref closestPoint, ref result13, out closestPoint);
		return VoronoiRegion.ABC;
	}

	/// <summary>
	/// Determines the closest point on a triangle given by points a, b, and c to point p and provides the subsimplex whose voronoi region contains the point.
	/// </summary>
	/// <param name="a">First vertex of triangle.</param>
	/// <param name="b">Second vertex of triangle.</param>
	/// <param name="c">Third vertex of triangle.</param>
	/// <param name="p">Point for comparison.</param>
	/// <param name="subsimplex">The source of the voronoi region which contains the point.</param>
	/// <param name="closestPoint">Closest point on tetrahedron to point.</param>
	[Obsolete("Used for simplex tests; consider using the PairSimplex and its variants instead for simplex-related testing.")]
	public static void GetClosestPointOnTriangleToPoint(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 p, RawList<Vector3> subsimplex, out Vector3 closestPoint)
	{
		subsimplex.Clear();
		Vector3.Subtract(ref b, ref a, out var result);
		Vector3.Subtract(ref c, ref a, out var result2);
		Vector3.Subtract(ref p, ref a, out var result3);
		Vector3.Dot(ref result, ref result3, out var result4);
		Vector3.Dot(ref result2, ref result3, out var result5);
		if (result4 <= 0f && result5 < 0f)
		{
			subsimplex.Add(a);
			closestPoint = a;
			return;
		}
		Vector3.Subtract(ref p, ref b, out var result6);
		Vector3.Dot(ref result, ref result6, out var result7);
		Vector3.Dot(ref result2, ref result6, out var result8);
		if (result7 >= 0f && result8 <= result7)
		{
			subsimplex.Add(b);
			closestPoint = b;
			return;
		}
		float num = result4 * result8 - result7 * result5;
		if (num <= 0f && result4 >= 0f && result7 <= 0f)
		{
			subsimplex.Add(a);
			subsimplex.Add(b);
			float scaleFactor = result4 / (result4 - result7);
			Vector3.Multiply(ref result, scaleFactor, out closestPoint);
			Vector3.Add(ref closestPoint, ref a, out closestPoint);
			return;
		}
		Vector3.Subtract(ref p, ref c, out var result9);
		Vector3.Dot(ref result, ref result9, out var result10);
		Vector3.Dot(ref result2, ref result9, out var result11);
		if (result11 >= 0f && result10 <= result11)
		{
			subsimplex.Add(c);
			closestPoint = c;
			return;
		}
		float num2 = result10 * result5 - result4 * result11;
		if (num2 <= 0f && result5 >= 0f && result11 <= 0f)
		{
			subsimplex.Add(a);
			subsimplex.Add(c);
			float scaleFactor2 = result5 / (result5 - result11);
			Vector3.Multiply(ref result2, scaleFactor2, out closestPoint);
			Vector3.Add(ref closestPoint, ref a, out closestPoint);
			return;
		}
		float num3 = result7 * result11 - result10 * result8;
		if (num3 <= 0f && result8 - result7 >= 0f && result10 - result11 >= 0f)
		{
			subsimplex.Add(b);
			subsimplex.Add(c);
			float scaleFactor2 = (result8 - result7) / (result8 - result7 + (result10 - result11));
			Vector3.Subtract(ref c, ref b, out closestPoint);
			Vector3.Multiply(ref closestPoint, scaleFactor2, out closestPoint);
			Vector3.Add(ref closestPoint, ref b, out closestPoint);
		}
		else
		{
			subsimplex.Add(a);
			subsimplex.Add(b);
			subsimplex.Add(c);
			float num4 = 1f / (num3 + num2 + num);
			float scaleFactor = num2 * num4;
			float scaleFactor2 = num * num4;
			Vector3.Multiply(ref result, scaleFactor, out var result12);
			Vector3.Multiply(ref result2, scaleFactor2, out var result13);
			Vector3.Add(ref a, ref result12, out closestPoint);
			Vector3.Add(ref closestPoint, ref result13, out closestPoint);
		}
	}

	/// <summary>
	/// Determines the closest point on a triangle given by points a, b, and c to point p and provides the subsimplex whose voronoi region contains the point.
	/// </summary>
	/// <param name="q">Simplex containing triangle for testing.</param>
	/// <param name="i">Index of first vertex of triangle.</param>
	/// <param name="j">Index of second vertex of triangle.</param>
	/// <param name="k">Index of third vertex of triangle.</param>
	/// <param name="p">Point for comparison.</param>
	/// <param name="subsimplex">The source of the voronoi region which contains the point, enumerated as a = 0, b = 1, c = 2.</param>
	/// <param name="baryCoords">Barycentric coordinates of the point on the triangle.</param>
	/// <param name="closestPoint">Closest point on tetrahedron to point.</param>
	[Obsolete("Used for simplex tests; consider using the PairSimplex and its variants instead for simplex-related testing.")]
	public static void GetClosestPointOnTriangleToPoint(RawList<Vector3> q, int i, int j, int k, ref Vector3 p, RawList<int> subsimplex, RawList<float> baryCoords, out Vector3 closestPoint)
	{
		subsimplex.Clear();
		baryCoords.Clear();
		Vector3 value = q[i];
		Vector3 value2 = q[j];
		Vector3 value3 = q[k];
		Vector3.Subtract(ref value2, ref value, out var result);
		Vector3.Subtract(ref value3, ref value, out var result2);
		Vector3.Subtract(ref p, ref value, out var result3);
		Vector3.Dot(ref result, ref result3, out var result4);
		Vector3.Dot(ref result2, ref result3, out var result5);
		if (result4 <= 0f && result5 < 0f)
		{
			subsimplex.Add(i);
			baryCoords.Add(1f);
			closestPoint = value;
			return;
		}
		Vector3.Subtract(ref p, ref value2, out var result6);
		Vector3.Dot(ref result, ref result6, out var result7);
		Vector3.Dot(ref result2, ref result6, out var result8);
		if (result7 >= 0f && result8 <= result7)
		{
			subsimplex.Add(j);
			baryCoords.Add(1f);
			closestPoint = value2;
			return;
		}
		float num = result4 * result8 - result7 * result5;
		if (num <= 0f && result4 >= 0f && result7 <= 0f)
		{
			subsimplex.Add(i);
			subsimplex.Add(j);
			float num2 = result4 / (result4 - result7);
			baryCoords.Add(1f - num2);
			baryCoords.Add(num2);
			Vector3.Multiply(ref result, num2, out closestPoint);
			Vector3.Add(ref closestPoint, ref value, out closestPoint);
			return;
		}
		Vector3.Subtract(ref p, ref value3, out var result9);
		Vector3.Dot(ref result, ref result9, out var result10);
		Vector3.Dot(ref result2, ref result9, out var result11);
		if (result11 >= 0f && result10 <= result11)
		{
			subsimplex.Add(k);
			baryCoords.Add(1f);
			closestPoint = value3;
			return;
		}
		float num3 = result10 * result5 - result4 * result11;
		if (num3 <= 0f && result5 >= 0f && result11 <= 0f)
		{
			subsimplex.Add(i);
			subsimplex.Add(k);
			float num4 = result5 / (result5 - result11);
			baryCoords.Add(1f - num4);
			baryCoords.Add(num4);
			Vector3.Multiply(ref result2, num4, out closestPoint);
			Vector3.Add(ref closestPoint, ref value, out closestPoint);
			return;
		}
		float num5 = result7 * result11 - result10 * result8;
		if (num5 <= 0f && result8 - result7 >= 0f && result10 - result11 >= 0f)
		{
			subsimplex.Add(j);
			subsimplex.Add(k);
			float num4 = (result8 - result7) / (result8 - result7 + (result10 - result11));
			baryCoords.Add(1f - num4);
			baryCoords.Add(num4);
			Vector3.Subtract(ref value3, ref value2, out closestPoint);
			Vector3.Multiply(ref closestPoint, num4, out closestPoint);
			Vector3.Add(ref closestPoint, ref value2, out closestPoint);
		}
		else
		{
			subsimplex.Add(i);
			subsimplex.Add(j);
			subsimplex.Add(k);
			float num6 = 1f / (num5 + num3 + num);
			float num2 = num3 * num6;
			float num4 = num * num6;
			baryCoords.Add(1f - num2 - num4);
			baryCoords.Add(num2);
			baryCoords.Add(num4);
			Vector3.Multiply(ref result, num2, out var result12);
			Vector3.Multiply(ref result2, num4, out var result13);
			Vector3.Add(ref value, ref result12, out closestPoint);
			Vector3.Add(ref closestPoint, ref result13, out closestPoint);
		}
	}

	/// <summary>
	/// Determines if supplied point is within the triangle as defined by the provided vertices.
	/// </summary>
	/// <param name="vA">A vertex of the triangle.</param>
	/// <param name="vB">A vertex of the triangle.</param>
	/// <param name="vC">A vertex of the triangle.</param>
	/// <param name="p">The point for comparison against the triangle.</param>
	/// <returns>Whether or not the point is within the triangle.</returns>
	public static bool IsPointInsideTriangle(ref Vector3 vA, ref Vector3 vB, ref Vector3 vC, ref Vector3 p)
	{
		GetBarycentricCoordinates(ref p, ref vA, ref vB, ref vC, out var aWeight, out var bWeight, out var cWeight);
		if (aWeight > -1E-07f && bWeight > -1E-07f)
		{
			return cWeight > -1E-07f;
		}
		return false;
	}

	/// <summary>
	/// Determines if supplied point is within the triangle as defined by the provided vertices.
	/// </summary>
	/// <param name="vA">A vertex of the triangle.</param>
	/// <param name="vB">A vertex of the triangle.</param>
	/// <param name="vC">A vertex of the triangle.</param>
	/// <param name="p">The point for comparison against the triangle.</param>
	/// <param name="margin">Extra area on the edges of the triangle to include.  Can be negative.</param>
	/// <returns>Whether or not the point is within the triangle.</returns>
	public static bool IsPointInsideTriangle(ref Vector3 vA, ref Vector3 vB, ref Vector3 vC, ref Vector3 p, float margin)
	{
		GetBarycentricCoordinates(ref p, ref vA, ref vB, ref vC, out var aWeight, out var bWeight, out var cWeight);
		if (aWeight > 0f - margin && bWeight > 0f - margin)
		{
			return cWeight > 0f - margin;
		}
		return false;
	}

	/// <summary>
	/// Determines the closest point on the provided segment ab to point p.
	/// </summary>
	/// <param name="a">First endpoint of segment.</param>
	/// <param name="b">Second endpoint of segment.</param>
	/// <param name="p">Point for comparison.</param>
	/// <param name="closestPoint">Closest point on the edge to p.</param>
	public static void GetClosestPointOnSegmentToPoint(ref Vector3 a, ref Vector3 b, ref Vector3 p, out Vector3 closestPoint)
	{
		Vector3.Subtract(ref b, ref a, out var result);
		Vector3.Subtract(ref p, ref a, out var result2);
		Vector3.Dot(ref result2, ref result, out var result3);
		if (result3 <= 0f)
		{
			closestPoint = a;
			return;
		}
		float num = result.X * result.X + result.Y * result.Y + result.Z * result.Z;
		if (result3 >= num)
		{
			closestPoint = b;
			return;
		}
		result3 /= num;
		Vector3.Multiply(ref result, result3, out var result4);
		Vector3.Add(ref a, ref result4, out closestPoint);
	}

	/// <summary>
	/// Determines the closest point on the provided segment ab to point p.
	/// </summary>
	/// <param name="a">First endpoint of segment.</param>
	/// <param name="b">Second endpoint of segment.</param>
	/// <param name="p">Point for comparison.</param>
	/// <param name="subsimplex">The source of the voronoi region which contains the point.</param>
	/// <param name="closestPoint">Closest point on the edge to p.</param>
	[Obsolete("Used for simplex tests; consider using the PairSimplex and its variants instead for simplex-related testing.")]
	public static void GetClosestPointOnSegmentToPoint(ref Vector3 a, ref Vector3 b, ref Vector3 p, List<Vector3> subsimplex, out Vector3 closestPoint)
	{
		subsimplex.Clear();
		Vector3.Subtract(ref b, ref a, out var result);
		Vector3.Subtract(ref p, ref a, out var result2);
		Vector3.Dot(ref result2, ref result, out var result3);
		if (result3 <= 0f)
		{
			subsimplex.Add(a);
			closestPoint = a;
			return;
		}
		float num = result.X * result.X + result.Y * result.Y + result.Z * result.Z;
		if (result3 >= num)
		{
			subsimplex.Add(b);
			closestPoint = b;
			return;
		}
		result3 /= num;
		subsimplex.Add(a);
		subsimplex.Add(b);
		Vector3.Multiply(ref result, result3, out var result4);
		Vector3.Add(ref a, ref result4, out closestPoint);
	}

	/// <summary>
	/// Determines the closest point on the provided segment ab to point p.
	/// </summary>
	/// <param name="q">List of points in the containing simplex.</param>
	/// <param name="i">Index of first endpoint of segment.</param>
	/// <param name="j">Index of second endpoint of segment.</param>
	/// <param name="p">Point for comparison.</param>
	/// <param name="subsimplex">The source of the voronoi region which contains the point, enumerated as a = 0, b = 1.</param>
	/// <param name="baryCoords">Barycentric coordinates of the point.</param>
	/// <param name="closestPoint">Closest point on the edge to p.</param>
	[Obsolete("Used for simplex tests; consider using the PairSimplex and its variants instead for simplex-related testing.")]
	public static void GetClosestPointOnSegmentToPoint(List<Vector3> q, int i, int j, ref Vector3 p, List<int> subsimplex, List<float> baryCoords, out Vector3 closestPoint)
	{
		Vector3 value = q[i];
		Vector3 value2 = q[j];
		subsimplex.Clear();
		baryCoords.Clear();
		Vector3.Subtract(ref value2, ref value, out var result);
		Vector3.Subtract(ref p, ref value, out var result2);
		Vector3.Dot(ref result2, ref result, out var result3);
		if (result3 <= 0f)
		{
			subsimplex.Add(i);
			baryCoords.Add(1f);
			closestPoint = value;
			return;
		}
		float num = result.X * result.X + result.Y * result.Y + result.Z * result.Z;
		if (result3 >= num)
		{
			subsimplex.Add(j);
			baryCoords.Add(1f);
			closestPoint = value2;
			return;
		}
		result3 /= num;
		subsimplex.Add(i);
		subsimplex.Add(j);
		baryCoords.Add(1f - result3);
		baryCoords.Add(result3);
		Vector3.Multiply(ref result, result3, out var result4);
		Vector3.Add(ref value, ref result4, out closestPoint);
	}

	/// <summary>
	/// Determines the shortest squared distance from the point to the line.
	/// </summary>
	/// <param name="p">Point to check against the line.</param>
	/// <param name="a">First point on the line.</param>
	/// <param name="b">Second point on the line.</param>
	/// <returns>Shortest squared distance from the point to the line.</returns>
	public static float GetSquaredDistanceFromPointToLine(ref Vector3 p, ref Vector3 a, ref Vector3 b)
	{
		Vector3.Subtract(ref p, ref a, out var result);
		Vector3.Subtract(ref b, ref a, out var result2);
		Vector3.Dot(ref result, ref result2, out var result3);
		return result.LengthSquared() - result3 * result3 / result2.LengthSquared();
	}

	/// <summary>
	/// Computes closest points c1 and c2 betwen segments p1q1 and p2q2.
	/// </summary>
	/// <param name="p1">First point of first segment.</param>
	/// <param name="q1">Second point of first segment.</param>
	/// <param name="p2">First point of second segment.</param>
	/// <param name="q2">Second point of second segment.</param>
	/// <param name="c1">Closest point on first segment.</param>
	/// <param name="c2">Closest point on second segment.</param>
	public static void GetClosestPointsBetweenSegments(Vector3 p1, Vector3 q1, Vector3 p2, Vector3 q2, out Vector3 c1, out Vector3 c2)
	{
		GetClosestPointsBetweenSegments(ref p1, ref q1, ref p2, ref q2, out var _, out var _, out c1, out c2);
	}

	/// <summary>
	/// Computes closest points c1 and c2 betwen segments p1q1 and p2q2.
	/// </summary>
	/// <param name="p1">First point of first segment.</param>
	/// <param name="q1">Second point of first segment.</param>
	/// <param name="p2">First point of second segment.</param>
	/// <param name="q2">Second point of second segment.</param>
	/// <param name="s">Distance along the line to the point for first segment.</param>
	/// <param name="t">Distance along the line to the point for second segment.</param>
	/// <param name="c1">Closest point on first segment.</param>
	/// <param name="c2">Closest point on second segment.</param>
	public static void GetClosestPointsBetweenSegments(ref Vector3 p1, ref Vector3 q1, ref Vector3 p2, ref Vector3 q2, out float s, out float t, out Vector3 c1, out Vector3 c2)
	{
		Vector3.Subtract(ref q1, ref p1, out var result);
		Vector3.Subtract(ref q2, ref p2, out var result2);
		Vector3.Subtract(ref p1, ref p2, out var result3);
		float num = result.LengthSquared();
		float num2 = result2.LengthSquared();
		Vector3.Dot(ref result2, ref result3, out var result4);
		if (num <= 1E-07f && num2 <= 1E-07f)
		{
			s = (t = 0f);
			c1 = p1;
			c2 = p2;
			return;
		}
		if (num <= 1E-07f)
		{
			s = 0f;
			t = MathHelper.Clamp(result4 / num2, 0f, 1f);
		}
		else
		{
			float num3 = Vector3.Dot(result, result3);
			if (num2 <= 1E-07f)
			{
				t = 0f;
				s = MathHelper.Clamp((0f - num3) / num, 0f, 1f);
			}
			else
			{
				float num4 = Vector3.Dot(result, result2);
				float num5 = num * num2 - num4 * num4;
				if (num5 != 0f)
				{
					s = MathHelper.Clamp((num4 * result4 - num3 * num2) / num5, 0f, 1f);
				}
				else
				{
					s = 0.5f;
				}
				t = (num4 * s + result4) / num2;
				if (t < 0f)
				{
					t = 0f;
					s = MathHelper.Clamp((0f - num3) / num, 0f, 1f);
				}
				else if (t > 1f)
				{
					t = 1f;
					s = MathHelper.Clamp((num4 - num3) / num, 0f, 1f);
				}
			}
		}
		Vector3.Multiply(ref result, s, out c1);
		Vector3.Add(ref c1, ref p1, out c1);
		Vector3.Multiply(ref result2, t, out c2);
		Vector3.Add(ref c2, ref p2, out c2);
	}

	/// <summary>
	/// Computes closest points c1 and c2 betwen lines p1q1 and p2q2.
	/// </summary>
	/// <param name="p1">First point of first segment.</param>
	/// <param name="q1">Second point of first segment.</param>
	/// <param name="p2">First point of second segment.</param>
	/// <param name="q2">Second point of second segment.</param>
	/// <param name="s">Distance along the line to the point for first segment.</param>
	/// <param name="t">Distance along the line to the point for second segment.</param>
	/// <param name="c1">Closest point on first segment.</param>
	/// <param name="c2">Closest point on second segment.</param>
	public static void GetClosestPointsBetweenLines(ref Vector3 p1, ref Vector3 q1, ref Vector3 p2, ref Vector3 q2, out float s, out float t, out Vector3 c1, out Vector3 c2)
	{
		Vector3.Subtract(ref q1, ref p1, out var result);
		Vector3.Subtract(ref q2, ref p2, out var result2);
		Vector3.Subtract(ref p1, ref p2, out var result3);
		float num = result.LengthSquared();
		float num2 = result2.LengthSquared();
		Vector3.Dot(ref result2, ref result3, out var result4);
		if (num <= 1E-07f && num2 <= 1E-07f)
		{
			s = (t = 0f);
			c1 = p1;
			c2 = p2;
			return;
		}
		if (num <= 1E-07f)
		{
			s = 0f;
			t = MathHelper.Clamp(result4 / num2, 0f, 1f);
		}
		else
		{
			float num3 = Vector3.Dot(result, result3);
			if (num2 <= 1E-07f)
			{
				t = 0f;
				s = MathHelper.Clamp((0f - num3) / num, 0f, 1f);
			}
			else
			{
				float num4 = Vector3.Dot(result, result2);
				float num5 = num * num2 - num4 * num4;
				if (num5 != 0f)
				{
					s = (num4 * result4 - num3 * num2) / num5;
				}
				else
				{
					s = 0.5f;
				}
				t = (num4 * s + result4) / num2;
			}
		}
		Vector3.Multiply(ref result, s, out c1);
		Vector3.Add(ref c1, ref p1, out c1);
		Vector3.Multiply(ref result2, t, out c2);
		Vector3.Add(ref c2, ref p2, out c2);
	}

	/// <summary>
	/// Determines if vectors o and p are on opposite sides of the plane defined by a, b, and c.
	/// </summary>
	/// <param name="o">First point for comparison.</param>
	/// <param name="p">Second point for comparison.</param>
	/// <param name="a">First vertex of the plane.</param>
	/// <param name="b">Second vertex of plane.</param>
	/// <param name="c">Third vertex of plane.</param>
	/// <returns>Whether or not vectors o and p reside on opposite sides of the plane.</returns>
	public static bool ArePointsOnOppositeSidesOfPlane(ref Vector3 o, ref Vector3 p, ref Vector3 a, ref Vector3 b, ref Vector3 c)
	{
		Vector3.Subtract(ref b, ref a, out var result);
		Vector3.Subtract(ref c, ref a, out var result2);
		Vector3.Subtract(ref p, ref a, out var result3);
		Vector3.Subtract(ref o, ref a, out var result4);
		Vector3.Cross(ref result, ref result2, out var result5);
		Vector3.Dot(ref result3, ref result5, out var result6);
		Vector3.Dot(ref result4, ref result5, out var result7);
		if (result6 * result7 <= 0f)
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Determines the distance between a point and a plane..
	/// </summary>
	/// <param name="point">Point to project onto plane.</param>
	/// <param name="normal">Normal of the plane.</param>
	/// <param name="pointOnPlane">Point located on the plane.</param>
	/// <returns>Distance from the point to the plane.</returns>
	public static float GetDistancePointToPlane(ref Vector3 point, ref Vector3 normal, ref Vector3 pointOnPlane)
	{
		Vector3.Subtract(ref point, ref pointOnPlane, out var result);
		Vector3.Dot(ref normal, ref result, out var result2);
		return result2 / normal.LengthSquared();
	}

	/// <summary>
	/// Determines the location of the point when projected onto the plane defined by the normal and a point on the plane.
	/// </summary>
	/// <param name="point">Point to project onto plane.</param>
	/// <param name="normal">Normal of the plane.</param>
	/// <param name="pointOnPlane">Point located on the plane.</param>
	/// <param name="projectedPoint">Projected location of point onto plane.</param>
	public static void GetPointProjectedOnPlane(ref Vector3 point, ref Vector3 normal, ref Vector3 pointOnPlane, out Vector3 projectedPoint)
	{
		Vector3.Dot(ref normal, ref point, out var result);
		Vector3.Dot(ref pointOnPlane, ref normal, out var result2);
		float scaleFactor = (result - result2) / normal.LengthSquared();
		Vector3.Multiply(ref normal, scaleFactor, out var result3);
		Vector3.Subtract(ref point, ref result3, out projectedPoint);
	}

	/// <summary>
	/// Determines if a point is within a set of planes defined by the edges of a triangle.
	/// </summary>
	/// <param name="point">Point for comparison.</param>
	/// <param name="planes">Edge planes.</param>
	/// <param name="centroid">A point known to be inside of the planes.</param>
	/// <returns>Whether or not the point is within the edge planes.</returns>
	public static bool IsPointWithinFaceExtrusion(Vector3 point, List<Plane> planes, Vector3 centroid)
	{
		foreach (Plane plane in planes)
		{
			plane.DotCoordinate(ref centroid, out var result);
			plane.DotCoordinate(ref point, out var result2);
			if ((!(result <= 1E-07f) || !(result2 <= 1E-07f)) && (!(result >= -1E-07f) || !(result2 >= -1E-07f)))
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Determines the closest point on a tetrahedron to a provided point p.
	/// </summary>
	/// <param name="a">First vertex of the tetrahedron.</param>
	/// <param name="b">Second vertex of the tetrahedron.</param>
	/// <param name="c">Third vertex of the tetrahedron.</param>
	/// <param name="d">Fourth vertex of the tetrahedron.</param>
	/// <param name="p">Point for comparison.</param>
	/// <param name="closestPoint">Closest point on the tetrahedron to the point.</param>
	[Obsolete("This method was used for older GJK simplex tests.  If you need simplex tests, consider the PairSimplex class and its variants.")]
	public static void GetClosestPointOnTetrahedronToPoint(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 d, ref Vector3 p, out Vector3 closestPoint)
	{
		closestPoint = p;
		float num = float.MaxValue;
		Vector3 closestPoint2;
		Vector3 result;
		if (ArePointsOnOppositeSidesOfPlane(ref p, ref d, ref a, ref b, ref c))
		{
			GetClosestPointOnTriangleToPoint(ref a, ref b, ref c, ref p, out closestPoint2);
			Vector3.Subtract(ref closestPoint2, ref p, out result);
			float num2 = result.X * result.X + result.Y * result.Y + result.Z * result.Z;
			if (num2 < num)
			{
				num = num2;
				closestPoint = closestPoint2;
			}
		}
		if (ArePointsOnOppositeSidesOfPlane(ref p, ref b, ref a, ref c, ref d))
		{
			GetClosestPointOnTriangleToPoint(ref a, ref c, ref d, ref p, out closestPoint2);
			Vector3.Subtract(ref closestPoint2, ref p, out result);
			float num3 = result.X * result.X + result.Y * result.Y + result.Z * result.Z;
			if (num3 < num)
			{
				num = num3;
				closestPoint = closestPoint2;
			}
		}
		if (ArePointsOnOppositeSidesOfPlane(ref p, ref c, ref a, ref d, ref b))
		{
			GetClosestPointOnTriangleToPoint(ref a, ref d, ref b, ref p, out closestPoint2);
			Vector3.Subtract(ref closestPoint2, ref p, out result);
			float num4 = result.X * result.X + result.Y * result.Y + result.Z * result.Z;
			if (num4 < num)
			{
				num = num4;
				closestPoint = closestPoint2;
			}
		}
		if (ArePointsOnOppositeSidesOfPlane(ref p, ref a, ref b, ref d, ref c))
		{
			GetClosestPointOnTriangleToPoint(ref b, ref d, ref c, ref p, out closestPoint2);
			Vector3.Subtract(ref closestPoint2, ref p, out result);
			float num5 = result.X * result.X + result.Y * result.Y + result.Z * result.Z;
			if (num5 < num)
			{
				closestPoint = closestPoint2;
			}
		}
	}

	/// <summary>
	/// Determines the closest point on a tetrahedron to a provided point p.
	/// </summary>
	/// <param name="a">First vertex of the tetrahedron.</param>
	/// <param name="b">Second vertex of the tetrahedron.</param>
	/// <param name="c">Third vertex of the tetrahedron.</param>
	/// <param name="d">Fourth vertex of the tetrahedron.</param>
	/// <param name="p">Point for comparison.</param>
	/// <param name="subsimplex">The source of the voronoi region which contains the point.</param>
	/// <param name="closestPoint">Closest point on the tetrahedron to the point.</param>
	[Obsolete("This method was used for older GJK simplex tests.  If you need simplex tests, consider the PairSimplex class and its variants.")]
	public static void GetClosestPointOnTetrahedronToPoint(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 d, ref Vector3 p, RawList<Vector3> subsimplex, out Vector3 closestPoint)
	{
		subsimplex.Clear();
		subsimplex.Add(a);
		subsimplex.Add(b);
		subsimplex.Add(c);
		subsimplex.Add(d);
		closestPoint = p;
		float num = float.MaxValue;
		Vector3 closestPoint2;
		Vector3 result;
		if (ArePointsOnOppositeSidesOfPlane(ref p, ref d, ref a, ref b, ref c))
		{
			GetClosestPointOnTriangleToPoint(ref a, ref b, ref c, ref p, subsimplex, out closestPoint2);
			Vector3.Subtract(ref closestPoint2, ref p, out result);
			float num2 = result.X * result.X + result.Y * result.Y + result.Z * result.Z;
			if (num2 < num)
			{
				num = num2;
				closestPoint = closestPoint2;
			}
		}
		if (ArePointsOnOppositeSidesOfPlane(ref p, ref b, ref a, ref c, ref d))
		{
			GetClosestPointOnTriangleToPoint(ref a, ref c, ref d, ref p, subsimplex, out closestPoint2);
			Vector3.Subtract(ref closestPoint2, ref p, out result);
			float num3 = result.X * result.X + result.Y * result.Y + result.Z * result.Z;
			if (num3 < num)
			{
				num = num3;
				closestPoint = closestPoint2;
			}
		}
		if (ArePointsOnOppositeSidesOfPlane(ref p, ref c, ref a, ref d, ref b))
		{
			GetClosestPointOnTriangleToPoint(ref a, ref d, ref b, ref p, subsimplex, out closestPoint2);
			Vector3.Subtract(ref closestPoint2, ref p, out result);
			float num4 = result.X * result.X + result.Y * result.Y + result.Z * result.Z;
			if (num4 < num)
			{
				num = num4;
				closestPoint = closestPoint2;
			}
		}
		if (ArePointsOnOppositeSidesOfPlane(ref p, ref a, ref b, ref d, ref c))
		{
			GetClosestPointOnTriangleToPoint(ref b, ref d, ref c, ref p, subsimplex, out closestPoint2);
			Vector3.Subtract(ref closestPoint2, ref p, out result);
			float num5 = result.X * result.X + result.Y * result.Y + result.Z * result.Z;
			if (num5 < num)
			{
				closestPoint = closestPoint2;
			}
		}
	}

	/// <summary>
	/// Determines the closest point on a tetrahedron to a provided point p.
	/// </summary>
	/// <param name="tetrahedron">List of 4 points composing the tetrahedron.</param>
	/// <param name="p">Point for comparison.</param>
	/// <param name="subsimplex">The source of the voronoi region which contains the point, enumerated as a = 0, b = 1, c = 2, d = 3.</param>
	/// <param name="baryCoords">Barycentric coordinates of p on the tetrahedron.</param>
	/// <param name="closestPoint">Closest point on the tetrahedron to the point.</param>
	[Obsolete("This method was used for older GJK simplex tests.  If you need simplex tests, consider the PairSimplex class and its variants.")]
	public static void GetClosestPointOnTetrahedronToPoint(RawList<Vector3> tetrahedron, ref Vector3 p, RawList<int> subsimplex, RawList<float> baryCoords, out Vector3 closestPoint)
	{
		RawList<int> intList = Resources.GetIntList();
		RawList<float> floatList = Resources.GetFloatList();
		Vector3 a = tetrahedron[0];
		Vector3 b = tetrahedron[1];
		Vector3 c = tetrahedron[2];
		Vector3 p2 = tetrahedron[3];
		closestPoint = p;
		float num = float.MaxValue;
		subsimplex.Clear();
		subsimplex.Add(0);
		subsimplex.Add(1);
		subsimplex.Add(2);
		subsimplex.Add(3);
		baryCoords.Clear();
		bool flag = false;
		Vector3 closestPoint2;
		Vector3 result;
		if (ArePointsOnOppositeSidesOfPlane(ref p, ref p2, ref a, ref b, ref c))
		{
			GetClosestPointOnTriangleToPoint(tetrahedron, 0, 1, 2, ref p, intList, floatList, out closestPoint2);
			Vector3.Subtract(ref closestPoint2, ref p, out result);
			float num2 = result.LengthSquared();
			if (num2 < num)
			{
				num = num2;
				closestPoint = closestPoint2;
				subsimplex.Clear();
				baryCoords.Clear();
				for (int i = 0; i < intList.Count; i++)
				{
					subsimplex.Add(intList[i]);
					baryCoords.Add(floatList[i]);
				}
				flag = true;
			}
		}
		if (ArePointsOnOppositeSidesOfPlane(ref p, ref b, ref a, ref c, ref p2))
		{
			GetClosestPointOnTriangleToPoint(tetrahedron, 0, 2, 3, ref p, intList, floatList, out closestPoint2);
			Vector3.Subtract(ref closestPoint2, ref p, out result);
			float num3 = result.LengthSquared();
			if (num3 < num)
			{
				num = num3;
				closestPoint = closestPoint2;
				subsimplex.Clear();
				baryCoords.Clear();
				for (int j = 0; j < intList.Count; j++)
				{
					subsimplex.Add(intList[j]);
					baryCoords.Add(floatList[j]);
				}
				flag = true;
			}
		}
		if (ArePointsOnOppositeSidesOfPlane(ref p, ref c, ref a, ref p2, ref b))
		{
			GetClosestPointOnTriangleToPoint(tetrahedron, 0, 3, 1, ref p, intList, floatList, out closestPoint2);
			Vector3.Subtract(ref closestPoint2, ref p, out result);
			float num4 = result.LengthSquared();
			if (num4 < num)
			{
				num = num4;
				closestPoint = closestPoint2;
				subsimplex.Clear();
				baryCoords.Clear();
				for (int k = 0; k < intList.Count; k++)
				{
					subsimplex.Add(intList[k]);
					baryCoords.Add(floatList[k]);
				}
				flag = true;
			}
		}
		if (ArePointsOnOppositeSidesOfPlane(ref p, ref a, ref b, ref p2, ref c))
		{
			GetClosestPointOnTriangleToPoint(tetrahedron, 1, 3, 2, ref p, intList, floatList, out closestPoint2);
			Vector3.Subtract(ref closestPoint2, ref p, out result);
			float num5 = result.LengthSquared();
			if (num5 < num)
			{
				closestPoint = closestPoint2;
				subsimplex.Clear();
				baryCoords.Clear();
				for (int l = 0; l < intList.Count; l++)
				{
					subsimplex.Add(intList[l]);
					baryCoords.Add(floatList[l]);
				}
				flag = true;
			}
		}
		if (!flag)
		{
			float num6 = new Matrix(tetrahedron[0].X, tetrahedron[0].Y, tetrahedron[0].Z, 1f, tetrahedron[1].X, tetrahedron[1].Y, tetrahedron[1].Z, 1f, tetrahedron[2].X, tetrahedron[2].Y, tetrahedron[2].Z, 1f, tetrahedron[3].X, tetrahedron[3].Y, tetrahedron[3].Z, 1f).Determinant();
			float num7 = new Matrix(p.X, p.Y, p.Z, 1f, tetrahedron[1].X, tetrahedron[1].Y, tetrahedron[1].Z, 1f, tetrahedron[2].X, tetrahedron[2].Y, tetrahedron[2].Z, 1f, tetrahedron[3].X, tetrahedron[3].Y, tetrahedron[3].Z, 1f).Determinant();
			float num8 = new Matrix(tetrahedron[0].X, tetrahedron[0].Y, tetrahedron[0].Z, 1f, p.X, p.Y, p.Z, 1f, tetrahedron[2].X, tetrahedron[2].Y, tetrahedron[2].Z, 1f, tetrahedron[3].X, tetrahedron[3].Y, tetrahedron[3].Z, 1f).Determinant();
			float num9 = new Matrix(tetrahedron[0].X, tetrahedron[0].Y, tetrahedron[0].Z, 1f, tetrahedron[1].X, tetrahedron[1].Y, tetrahedron[1].Z, 1f, p.X, p.Y, p.Z, 1f, tetrahedron[3].X, tetrahedron[3].Y, tetrahedron[3].Z, 1f).Determinant();
			num6 = 1f / num6;
			baryCoords.Add(num7 * num6);
			baryCoords.Add(num8 * num6);
			baryCoords.Add(num9 * num6);
			baryCoords.Add(1f - baryCoords[0] - baryCoords[1] - baryCoords[2]);
		}
		Resources.GiveBack(intList);
		Resources.GiveBack(floatList);
	}

	/// <summary>
	///  Tests a ray against a sphere.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	/// <param name="spherePosition">Position of the sphere.</param>
	/// <param name="radius">Radius of the sphere.</param>
	/// <param name="maximumLength">Maximum length of the ray in units of the ray direction's length.</param>
	/// <param name="hit">Hit data of the ray, if any.</param>
	/// <returns>Whether or not the ray hits the sphere.</returns>
	public static bool RayCastSphere(ref Ray ray, ref Vector3 spherePosition, float radius, float maximumLength, out RayHit hit)
	{
		float num = ray.Direction.Length();
		Vector3.Divide(ref ray.Direction, num, out var result);
		maximumLength *= num;
		hit = default(RayHit);
		Vector3.Subtract(ref ray.Position, ref spherePosition, out var result2);
		float num2 = Vector3.Dot(result2, result);
		float num3 = result2.LengthSquared() - radius * radius;
		if (num3 > 0f && num2 > 0f)
		{
			return false;
		}
		float num4 = num2 * num2 - num3;
		if (num4 < 0f)
		{
			return false;
		}
		hit.T = 0f - num2 - (float)Math.Sqrt(num4);
		if (hit.T < 0f)
		{
			hit.T = 0f;
		}
		if (hit.T > maximumLength)
		{
			return false;
		}
		hit.T /= num;
		Vector3.Multiply(ref result, hit.T, out hit.Location);
		Vector3.Add(ref hit.Location, ref ray.Position, out hit.Location);
		Vector3.Subtract(ref hit.Location, ref spherePosition, out hit.Normal);
		hit.Normal.Normalize();
		return true;
	}

	/// <summary>
	/// Computes a bounding box and expands it.
	/// </summary>
	/// <param name="shape">Shape to compute the bounding box of.</param>
	/// <param name="transform">Transform to use to position the shape.</param>
	/// <param name="sweep">Extra to add to the bounding box.</param>
	/// <param name="boundingBox">Expanded bounding box.</param>
	public static void GetExpandedBoundingBox(ref ConvexShape shape, ref RigidTransform transform, ref Vector3 sweep, out BoundingBox boundingBox)
	{
		shape.GetBoundingBox(ref transform, out boundingBox);
		ExpandBoundingBox(ref boundingBox, ref sweep);
	}

	/// <summary>
	/// Expands a bounding box by the given sweep.
	/// </summary>
	/// <param name="boundingBox">Bounding box to expand.</param>
	/// <param name="sweep">Sweep to expand the bounding box with.</param>
	public static void ExpandBoundingBox(ref BoundingBox boundingBox, ref Vector3 sweep)
	{
		if (sweep.X > 0f)
		{
			boundingBox.Max.X += sweep.X;
		}
		else
		{
			boundingBox.Min.X += sweep.X;
		}
		if (sweep.Y > 0f)
		{
			boundingBox.Max.Y += sweep.Y;
		}
		else
		{
			boundingBox.Min.Y += sweep.Y;
		}
		if (sweep.Z > 0f)
		{
			boundingBox.Max.Z += sweep.Z;
		}
		else
		{
			boundingBox.Min.Z += sweep.Z;
		}
	}

	/// <summary>
	/// Computes the bounding box of three points.
	/// </summary>
	/// <param name="a">First vertex of the triangle.</param>
	/// <param name="b">Second vertex of the triangle.</param>
	/// <param name="c">Third vertex of the triangle.</param>
	/// <param name="aabb">Bounding box of the triangle.</param>
	public static void GetTriangleBoundingBox(ref Vector3 a, ref Vector3 b, ref Vector3 c, out BoundingBox aabb)
	{
		aabb = default(BoundingBox);
		if (a.X > b.X && a.X > c.X)
		{
			aabb.Max.X = a.X;
			if (b.X > c.X)
			{
				aabb.Min.X = c.X;
			}
			else
			{
				aabb.Min.X = b.X;
			}
		}
		else if (b.X > c.X)
		{
			aabb.Max.X = b.X;
			if (a.X > c.X)
			{
				aabb.Min.X = c.X;
			}
			else
			{
				aabb.Min.X = a.X;
			}
		}
		else
		{
			aabb.Max.X = c.X;
			if (a.X > b.X)
			{
				aabb.Min.X = b.X;
			}
			else
			{
				aabb.Min.X = a.X;
			}
		}
		if (a.Y > b.Y && a.Y > c.Y)
		{
			aabb.Max.Y = a.Y;
			if (b.Y > c.Y)
			{
				aabb.Min.Y = c.Y;
			}
			else
			{
				aabb.Min.Y = b.Y;
			}
		}
		else if (b.Y > c.Y)
		{
			aabb.Max.Y = b.Y;
			if (a.Y > c.Y)
			{
				aabb.Min.Y = c.Y;
			}
			else
			{
				aabb.Min.Y = a.Y;
			}
		}
		else
		{
			aabb.Max.Y = c.Y;
			if (a.Y > b.Y)
			{
				aabb.Min.Y = b.Y;
			}
			else
			{
				aabb.Min.Y = a.Y;
			}
		}
		if (a.Z > b.Z && a.Z > c.Z)
		{
			aabb.Max.Z = a.Z;
			if (b.Z > c.Z)
			{
				aabb.Min.Z = c.Z;
			}
			else
			{
				aabb.Min.Z = b.Z;
			}
		}
		else if (b.Z > c.Z)
		{
			aabb.Max.Z = b.Z;
			if (a.Z > c.Z)
			{
				aabb.Min.Z = c.Z;
			}
			else
			{
				aabb.Min.Z = a.Z;
			}
		}
		else
		{
			aabb.Max.Z = c.Z;
			if (a.Z > b.Z)
			{
				aabb.Min.Z = b.Z;
			}
			else
			{
				aabb.Min.Z = a.Z;
			}
		}
	}

	/// <summary>
	/// Computes the angle change represented by a normalized quaternion.
	/// </summary>
	/// <param name="q">Quaternion to be converted.</param>
	/// <returns>Angle around the axis represented by the quaternion.</returns>
	public static float GetAngleFromQuaternion(ref Quaternion q)
	{
		float num = Math.Abs(q.W);
		if (num > 1f)
		{
			return 0f;
		}
		return 2f * (float)Math.Acos(num);
	}

	/// <summary>
	/// Computes the axis angle representation of a normalized quaternion.
	/// </summary>
	/// <param name="q">Quaternion to be converted.</param>
	/// <param name="axis">Axis represented by the quaternion.</param>
	/// <param name="angle">Angle around the axis represented by the quaternion.</param>
	public static void GetAxisAngleFromQuaternion(ref Quaternion q, out Vector3 axis, out float angle)
	{
		axis = default(Vector3);
		float num = q.X;
		float num2 = q.Y;
		float num3 = q.Z;
		float num4 = q.W;
		if (num4 < 0f)
		{
			num = 0f - num;
			num2 = 0f - num2;
			num3 = 0f - num3;
			num4 = 0f - num4;
		}
		if ((double)num4 > 0.999999999999)
		{
			axis = UpVector;
			angle = 0f;
			return;
		}
		angle = 2f * (float)Math.Acos(num4);
		float num5 = 1f / (float)Math.Sqrt(1f - num4 * num4);
		axis.X = num * num5;
		axis.Y = num2 * num5;
		axis.Z = num3 * num5;
	}

	/// <summary>
	/// Computes the quaternion rotation between two normalized vectors.
	/// </summary>
	/// <param name="v1">First unit-length vector.</param>
	/// <param name="v2">Second unit-length vector.</param>
	/// <param name="q">Quaternion representing the rotation from v1 to v2.</param>
	public static void GetQuaternionBetweenNormalizedVectors(ref Vector3 v1, ref Vector3 v2, out Quaternion q)
	{
		Vector3.Dot(ref v1, ref v2, out var result);
		Vector3.Cross(ref v1, ref v2, out var result2);
		if (result < -0.9999f)
		{
			q = new Quaternion(0f - v1.Z, v1.Y, v1.X, 0f);
		}
		else
		{
			q = new Quaternion(result2.X, result2.Y, result2.Z, result + 1f);
		}
		q.Normalize();
	}

	/// <summary>
	/// Finds the velocity of a world space point as if it were connected to the given entity.
	/// </summary>
	/// <param name="p">Location of point in world space.</param>
	/// <param name="entity">Entity with which to measure the velocity.</param>
	/// <returns>Velocity of the point on the entity.</returns>
	public static Vector3 GetVelocityOfPoint(Vector3 p, Entity entity)
	{
		GetVelocityOfPoint(ref p, entity, out var velocity);
		return velocity;
	}

	/// <summary>
	/// Finds the velocity of a world space point as if it were connected to the given entity.
	/// </summary>
	/// <param name="p">Location of point in world space.</param>
	/// <param name="entity">Entity with which to measure the velocity.</param>
	/// <param name="velocity">Velocity of the point on the entity.</param>
	public static void GetVelocityOfPoint(ref Vector3 p, Entity entity, out Vector3 velocity)
	{
		Vector3.Subtract(ref p, ref entity.position, out var result);
		Vector3.Cross(ref entity.angularVelocity, ref result, out velocity);
		Vector3.Add(ref entity.linearVelocity, ref velocity, out velocity);
	}

	/// <summary>
	/// Updates the quaternion using RK4 integration.
	/// </summary>
	/// <param name="q">Quaternion to update.</param>
	/// <param name="localInertiaTensorInverse">Local-space inertia tensor of the object being updated.</param>
	/// <param name="angularMomentum">Angular momentum of the object.</param>
	/// <param name="dt">Time since last frame, in seconds.</param>
	/// <param name="newOrientation">New orientation quaternion.</param>
	internal static void UpdateOrientationRK4(ref Quaternion q, ref Matrix3X3 localInertiaTensorInverse, ref Vector3 angularMomentum, float dt, out Quaternion newOrientation)
	{
		DifferentiateQuaternion(ref q, ref localInertiaTensorInverse, ref angularMomentum, out var orientationChange);
		Quaternion.Multiply(ref orientationChange, dt * 0.5f, out var result);
		Quaternion.Add(ref q, ref result, out result);
		DifferentiateQuaternion(ref result, ref localInertiaTensorInverse, ref angularMomentum, out var orientationChange2);
		Quaternion.Multiply(ref orientationChange2, dt * 0.5f, out var result2);
		Quaternion.Add(ref q, ref result2, out result2);
		DifferentiateQuaternion(ref result2, ref localInertiaTensorInverse, ref angularMomentum, out var orientationChange3);
		Quaternion.Multiply(ref orientationChange3, dt, out var result3);
		Quaternion.Add(ref q, ref result3, out result3);
		DifferentiateQuaternion(ref result3, ref localInertiaTensorInverse, ref angularMomentum, out var orientationChange4);
		Quaternion.Multiply(ref orientationChange, dt / 6f, out orientationChange);
		Quaternion.Multiply(ref orientationChange2, dt / 3f, out orientationChange2);
		Quaternion.Multiply(ref orientationChange3, dt / 3f, out orientationChange3);
		Quaternion.Multiply(ref orientationChange4, dt / 6f, out orientationChange4);
		Quaternion.Add(ref q, ref orientationChange, out var result4);
		Quaternion.Add(ref result4, ref orientationChange2, out result4);
		Quaternion.Add(ref result4, ref orientationChange3, out result4);
		Quaternion.Add(ref result4, ref orientationChange4, out result4);
		Quaternion.Normalize(ref result4, out newOrientation);
	}

	/// <summary>
	/// Finds the change in the rotation state quaternion provided the local inertia tensor and angular velocity.
	/// </summary>
	/// <param name="orientation">Orienatation of the object.</param>
	/// <param name="localInertiaTensorInverse">Local-space inertia tensor of the object being updated.</param>
	/// <param name="angularMomentum">Angular momentum of the object.</param>
	///  <param name="orientationChange">Change in quaternion.</param>
	internal static void DifferentiateQuaternion(ref Quaternion orientation, ref Matrix3X3 localInertiaTensorInverse, ref Vector3 angularMomentum, out Quaternion orientationChange)
	{
		Quaternion.Normalize(ref orientation, out var result);
		Matrix3X3.CreateFromQuaternion(ref result, out var result2);
		Matrix3X3.MultiplyTransposed(ref result2, ref localInertiaTensorInverse, out var result3);
		Matrix3X3.Multiply(ref result3, ref result2, out result3);
		Matrix3X3.Transform(ref angularMomentum, ref result3, out var result4);
		Vector3.Multiply(ref result4, 0.5f, out result4);
		Quaternion quaternion = new Quaternion(result4.X, result4.Y, result4.Z, 0f);
		Quaternion.Multiply(ref quaternion, ref result, out orientationChange);
	}

	/// <summary>
	/// Gets the barycentric coordinates of the point with respect to a triangle's vertices.
	/// </summary>
	/// <param name="p">Point to compute the barycentric coordinates of.</param>
	/// <param name="a">First vertex in the triangle.</param>
	/// <param name="b">Second vertex in the triangle.</param>
	/// <param name="c">Third vertex in the triangle.</param>
	/// <param name="aWeight">Weight of the first vertex.</param>
	/// <param name="bWeight">Weight of the second vertex.</param>
	/// <param name="cWeight">Weight of the third vertex.</param>
	public static void GetBarycentricCoordinates(ref Vector3 p, ref Vector3 a, ref Vector3 b, ref Vector3 c, out float aWeight, out float bWeight, out float cWeight)
	{
		Vector3.Subtract(ref b, ref a, out var result);
		Vector3.Subtract(ref c, ref a, out var result2);
		Vector3.Cross(ref result, ref result2, out var result3);
		float num = ((result3.X < 0f) ? (0f - result3.X) : result3.X);
		float num2 = ((result3.Y < 0f) ? (0f - result3.Y) : result3.Y);
		float num3 = ((result3.Z < 0f) ? (0f - result3.Z) : result3.Z);
		float num4;
		float num5;
		float num6;
		if (num >= num2 && num >= num3)
		{
			num4 = (p.Y - b.Y) * (b.Z - c.Z) - (b.Y - c.Y) * (p.Z - b.Z);
			num5 = (p.Y - c.Y) * (c.Z - a.Z) - (c.Y - a.Y) * (p.Z - c.Z);
			num6 = result3.X;
		}
		else if (num2 >= num3)
		{
			num4 = (p.X - b.X) * (b.Z - c.Z) - (b.X - c.X) * (p.Z - b.Z);
			num5 = (p.X - c.X) * (c.Z - a.Z) - (c.X - a.X) * (p.Z - c.Z);
			num6 = 0f - result3.Y;
		}
		else
		{
			num4 = (p.X - b.X) * (b.Y - c.Y) - (b.X - c.X) * (p.Y - b.Y);
			num5 = (p.X - c.X) * (c.Y - a.Y) - (c.X - a.X) * (p.Y - c.Y);
			num6 = result3.Z;
		}
		if ((double)num6 < -1E-09 || (double)num6 > 1E-09)
		{
			num6 = 1f / num6;
			aWeight = num4 * num6;
			bWeight = num5 * num6;
			cWeight = 1f - aWeight - bWeight;
			return;
		}
		Vector3.DistanceSquared(ref p, ref a, out var result4);
		Vector3.DistanceSquared(ref p, ref b, out var result5);
		Vector3.DistanceSquared(ref p, ref c, out var result6);
		if (result4 < result5 && result4 < result6)
		{
			aWeight = 1f;
			bWeight = 0f;
			cWeight = 0f;
		}
		else if (result5 < result6)
		{
			aWeight = 0f;
			bWeight = 1f;
			cWeight = 0f;
		}
		else
		{
			aWeight = 0f;
			bWeight = 0f;
			cWeight = 1f;
		}
	}
}
