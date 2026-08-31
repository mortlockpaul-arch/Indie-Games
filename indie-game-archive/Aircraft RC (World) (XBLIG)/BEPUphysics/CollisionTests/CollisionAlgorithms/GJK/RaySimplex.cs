using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionTests.CollisionAlgorithms.GJK;

/// <summary>
///  GJK simplex supporting ray-based tests.
/// </summary>
public struct RaySimplex
{
	/// <summary>
	///  First vertex in the simplex.
	/// </summary>
	public Vector3 A;

	/// <summary>
	/// Second vertex in the simplex.
	/// </summary>
	public Vector3 B;

	/// <summary>
	/// Third vertex in the simplex.
	/// </summary>
	public Vector3 C;

	/// <summary>
	/// Fourth vertex in the simplex.
	/// </summary>
	public Vector3 D;

	/// <summary>
	/// Current state of the simplex.
	/// </summary>
	public SimplexState State;

	/// <summary>
	///  Gets the point on the simplex that is closest to the origin.
	/// </summary>
	/// <param name="simplex">Simplex to test.</param>
	/// <param name="point">Closest point on the simplex.</param>
	/// <returns>Whether or not the simplex contains the origin.</returns>
	public bool GetPointClosestToOrigin(ref RaySimplex simplex, out Vector3 point)
	{
		switch (State)
		{
		case SimplexState.Point:
			point = A;
			break;
		case SimplexState.Segment:
			GetPointOnSegmentClosestToOrigin(ref simplex, out point);
			break;
		case SimplexState.Triangle:
			GetPointOnTriangleClosestToOrigin(ref simplex, out point);
			break;
		case SimplexState.Tetrahedron:
			return GetPointOnTetrahedronClosestToOrigin(ref simplex, out point);
		default:
			point = Toolbox.ZeroVector;
			break;
		}
		return false;
	}

	/// <summary>
	///  Finds the point on the segment to the origin.
	/// </summary>
	/// <param name="simplex">Simplex to test.</param>
	/// <param name="point">Closest point.</param>
	public void GetPointOnSegmentClosestToOrigin(ref RaySimplex simplex, out Vector3 point)
	{
		Vector3.Subtract(ref B, ref A, out var result);
		Vector3.Dot(ref result, ref A, out var result2);
		if (result2 > 0f)
		{
			simplex.State = SimplexState.Point;
			point = A;
			return;
		}
		Vector3.Dot(ref result, ref B, out var result3);
		if (result3 > 0f)
		{
			float scaleFactor = (0f - result2) / result.LengthSquared();
			Vector3.Multiply(ref result, scaleFactor, out point);
			Vector3.Add(ref point, ref A, out point);
		}
		else
		{
			simplex.A = simplex.B;
			simplex.State = SimplexState.Point;
			point = A;
		}
	}

	/// <summary>
	///  Gets the point on the triangle that is closest to the origin.
	/// </summary>
	/// <param name="simplex">Simplex to test.</param>
	/// <param name="point">Closest point to origin.</param>
	public void GetPointOnTriangleClosestToOrigin(ref RaySimplex simplex, out Vector3 point)
	{
		Vector3.Subtract(ref B, ref A, out var result);
		Vector3.Subtract(ref C, ref A, out var result2);
		Vector3.Dot(ref result, ref A, out var result3);
		Vector3.Dot(ref result2, ref A, out var result4);
		result3 = 0f - result3;
		result4 = 0f - result4;
		if (result4 <= 0f && result3 <= 0f)
		{
			simplex.State = SimplexState.Point;
			point = A;
			return;
		}
		Vector3.Dot(ref result, ref B, out var result5);
		Vector3.Dot(ref result2, ref B, out var result6);
		result5 = 0f - result5;
		result6 = 0f - result6;
		if (result5 >= 0f && result6 <= result5)
		{
			simplex.State = SimplexState.Point;
			simplex.A = simplex.B;
			point = B;
			return;
		}
		float num = result3 * result6 - result5 * result4;
		if (num <= 0f && result3 > 0f && result5 < 0f)
		{
			simplex.State = SimplexState.Segment;
			float scaleFactor = result3 / (result3 - result5);
			Vector3.Multiply(ref result, scaleFactor, out point);
			Vector3.Add(ref point, ref A, out point);
			return;
		}
		Vector3.Dot(ref result, ref C, out var result7);
		Vector3.Dot(ref result2, ref C, out var result8);
		result7 = 0f - result7;
		result8 = 0f - result8;
		if (result8 >= 0f && result7 <= result8)
		{
			simplex.State = SimplexState.Point;
			simplex.A = simplex.C;
			point = A;
			return;
		}
		float num2 = result7 * result4 - result3 * result8;
		if (num2 <= 0f && result4 > 0f && result8 < 0f)
		{
			simplex.State = SimplexState.Segment;
			simplex.B = simplex.C;
			float scaleFactor2 = result4 / (result4 - result8);
			Vector3.Multiply(ref result2, scaleFactor2, out point);
			Vector3.Add(ref point, ref A, out point);
			return;
		}
		float num3 = result5 * result8 - result7 * result6;
		float num4;
		float num5;
		if (num3 <= 0f && (num4 = result6 - result5) > 0f && (num5 = result7 - result8) > 0f)
		{
			simplex.State = SimplexState.Segment;
			simplex.A = simplex.C;
			float scaleFactor3 = num4 / (num4 + num5);
			Vector3.Subtract(ref C, ref B, out var result9);
			Vector3.Multiply(ref result9, scaleFactor3, out point);
			Vector3.Add(ref point, ref B, out point);
		}
		else
		{
			float num6 = 1f / (num3 + num2 + num);
			float scaleFactor4 = num2 * num6;
			float scaleFactor5 = num * num6;
			Vector3.Multiply(ref result, scaleFactor4, out point);
			Vector3.Multiply(ref result2, scaleFactor5, out var result10);
			Vector3.Add(ref A, ref point, out point);
			Vector3.Add(ref point, ref result10, out point);
		}
	}

	/// <summary>
	///  Gets the point closest to the origin on the tetrahedron.
	/// </summary>
	/// <param name="simplex">Simplex to test.</param>
	/// <param name="point">Closest point.</param>
	/// <returns>Whether or not the tetrahedron encloses the origin.</returns>
	public bool GetPointOnTetrahedronClosestToOrigin(ref RaySimplex simplex, out Vector3 point)
	{
		RaySimplex raySimplex = default(RaySimplex);
		point = default(Vector3);
		float num = float.MaxValue;
		if (TryTetrahedronTriangle(ref A, ref C, ref D, ref simplex.A, ref simplex.C, ref simplex.D, ref B, out var simplex2, out var point2))
		{
			point = point2;
			raySimplex = simplex2;
			num = point2.LengthSquared();
		}
		float num2;
		if (TryTetrahedronTriangle(ref C, ref B, ref D, ref simplex.C, ref simplex.B, ref simplex.D, ref A, out simplex2, out point2) && (num2 = point2.LengthSquared()) < num)
		{
			point = point2;
			raySimplex = simplex2;
			num = num2;
		}
		if (TryTetrahedronTriangle(ref B, ref A, ref D, ref simplex.B, ref simplex.A, ref simplex.D, ref C, out simplex2, out point2) && (num2 = point2.LengthSquared()) < num)
		{
			point = point2;
			raySimplex = simplex2;
			num = num2;
		}
		if (TryTetrahedronTriangle(ref A, ref B, ref C, ref simplex.A, ref simplex.B, ref simplex.C, ref D, out simplex2, out point2) && (num2 = point2.LengthSquared()) < num)
		{
			point = point2;
			raySimplex = simplex2;
			num = num2;
		}
		if (num < float.MaxValue)
		{
			simplex = raySimplex;
			return false;
		}
		return true;
	}

	private static bool TryTetrahedronTriangle(ref Vector3 A, ref Vector3 B, ref Vector3 C, ref Vector3 simplexA, ref Vector3 simplexB, ref Vector3 simplexC, ref Vector3 otherPoint, out RaySimplex simplex, out Vector3 point)
	{
		simplex = default(RaySimplex);
		point = default(Vector3);
		Vector3.Subtract(ref B, ref A, out var result);
		Vector3.Subtract(ref C, ref A, out var result2);
		Vector3.Cross(ref result, ref result2, out var result3);
		Vector3.Subtract(ref otherPoint, ref A, out var result4);
		Vector3.Dot(ref A, ref result3, out var result5);
		Vector3.Dot(ref result4, ref result3, out var result6);
		if (result5 * result6 >= 0f)
		{
			Vector3.Dot(ref result, ref A, out var result7);
			Vector3.Dot(ref result2, ref A, out var result8);
			result7 = 0f - result7;
			result8 = 0f - result8;
			if (result8 <= 0f && result7 <= 0f)
			{
				simplex.State = SimplexState.Point;
				simplex.A = simplexA;
				point = A;
				return true;
			}
			Vector3.Dot(ref result, ref B, out var result9);
			Vector3.Dot(ref result2, ref B, out var result10);
			result9 = 0f - result9;
			result10 = 0f - result10;
			if (result9 >= 0f && result10 <= result9)
			{
				simplex.State = SimplexState.Point;
				simplex.A = simplexB;
				point = B;
				return true;
			}
			float num = result7 * result10 - result9 * result8;
			if (num <= 0f && result7 > 0f && result9 < 0f)
			{
				simplex.State = SimplexState.Segment;
				simplex.A = simplexA;
				simplex.B = simplexB;
				float scaleFactor = result7 / (result7 - result9);
				Vector3.Multiply(ref result, scaleFactor, out point);
				Vector3.Add(ref point, ref A, out point);
				return true;
			}
			Vector3.Dot(ref result, ref C, out var result11);
			Vector3.Dot(ref result2, ref C, out var result12);
			result11 = 0f - result11;
			result12 = 0f - result12;
			if (result12 >= 0f && result11 <= result12)
			{
				simplex.State = SimplexState.Point;
				simplex.A = simplexC;
				point = C;
				return true;
			}
			float num2 = result11 * result8 - result7 * result12;
			if (num2 <= 0f && result8 > 0f && result12 < 0f)
			{
				simplex.State = SimplexState.Segment;
				simplex.A = simplexA;
				simplex.B = simplexC;
				float scaleFactor2 = result8 / (result8 - result12);
				Vector3.Multiply(ref result2, scaleFactor2, out point);
				Vector3.Add(ref point, ref A, out point);
				return true;
			}
			float num3 = result9 * result12 - result11 * result10;
			float num4;
			float num5;
			if (num3 <= 0f && (num4 = result10 - result9) > 0f && (num5 = result11 - result12) > 0f)
			{
				simplex.State = SimplexState.Segment;
				simplex.A = simplexB;
				simplex.B = simplexC;
				float scaleFactor3 = num4 / (num4 + num5);
				Vector3.Subtract(ref C, ref B, out var result13);
				Vector3.Multiply(ref result13, scaleFactor3, out point);
				Vector3.Add(ref point, ref B, out point);
				return true;
			}
			simplex.State = SimplexState.Triangle;
			simplex.A = simplexA;
			simplex.B = simplexB;
			simplex.C = simplexC;
			float num6 = 1f / (num3 + num2 + num);
			float scaleFactor4 = num * num6;
			float scaleFactor5 = num2 * num6;
			Vector3.Multiply(ref result, scaleFactor5, out point);
			Vector3.Multiply(ref result2, scaleFactor4, out var result14);
			Vector3.Add(ref A, ref point, out point);
			Vector3.Add(ref point, ref result14, out point);
			return true;
		}
		return false;
	}

	/// <summary>
	///  Adds a new point to the simplex.
	/// </summary>
	/// <param name="point">Point to add.</param>
	/// <param name="hitLocation">Current ray hit location.</param>
	/// <param name="shiftedSimplex">Simplex shifted with the hit location.</param>
	public void AddNewSimplexPoint(ref Vector3 point, ref Vector3 hitLocation, out RaySimplex shiftedSimplex)
	{
		shiftedSimplex = default(RaySimplex);
		switch (State)
		{
		case SimplexState.Empty:
			State = SimplexState.Point;
			A = point;
			Vector3.Subtract(ref hitLocation, ref A, out shiftedSimplex.A);
			break;
		case SimplexState.Point:
			State = SimplexState.Segment;
			B = point;
			Vector3.Subtract(ref hitLocation, ref A, out shiftedSimplex.A);
			Vector3.Subtract(ref hitLocation, ref B, out shiftedSimplex.B);
			break;
		case SimplexState.Segment:
			State = SimplexState.Triangle;
			C = point;
			Vector3.Subtract(ref hitLocation, ref A, out shiftedSimplex.A);
			Vector3.Subtract(ref hitLocation, ref B, out shiftedSimplex.B);
			Vector3.Subtract(ref hitLocation, ref C, out shiftedSimplex.C);
			break;
		case SimplexState.Triangle:
			State = SimplexState.Tetrahedron;
			D = point;
			Vector3.Subtract(ref hitLocation, ref A, out shiftedSimplex.A);
			Vector3.Subtract(ref hitLocation, ref B, out shiftedSimplex.B);
			Vector3.Subtract(ref hitLocation, ref C, out shiftedSimplex.C);
			Vector3.Subtract(ref hitLocation, ref D, out shiftedSimplex.D);
			break;
		}
		shiftedSimplex.State = State;
	}

	/// <summary>
	/// Gets the error tolerance for the simplex.
	/// </summary>
	/// <param name="rayOrigin">Origin of the ray.</param>
	/// <returns>Error tolerance of the simplex.</returns>
	public float GetErrorTolerance(ref Vector3 rayOrigin)
	{
		float result;
		float result2;
		float result3;
		switch (State)
		{
		case SimplexState.Point:
			Vector3.DistanceSquared(ref A, ref rayOrigin, out result);
			return result;
		case SimplexState.Segment:
			Vector3.DistanceSquared(ref A, ref rayOrigin, out result);
			Vector3.DistanceSquared(ref B, ref rayOrigin, out result2);
			return MathHelper.Max(result, result2);
		case SimplexState.Triangle:
			Vector3.DistanceSquared(ref A, ref rayOrigin, out result);
			Vector3.DistanceSquared(ref B, ref rayOrigin, out result2);
			Vector3.DistanceSquared(ref C, ref rayOrigin, out result3);
			return MathHelper.Max(result, MathHelper.Max(result2, result3));
		case SimplexState.Tetrahedron:
		{
			Vector3.DistanceSquared(ref A, ref rayOrigin, out result);
			Vector3.DistanceSquared(ref B, ref rayOrigin, out result2);
			Vector3.DistanceSquared(ref C, ref rayOrigin, out result3);
			Vector3.DistanceSquared(ref D, ref rayOrigin, out var result4);
			return MathHelper.Max(result, MathHelper.Max(result2, MathHelper.Max(result3, result4)));
		}
		default:
			return 0f;
		}
	}
}
