using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionTests.CollisionAlgorithms.GJK;

/// <summary>
///  GJK simplex supporting boolean intersection tests.
/// </summary>
public struct SimpleSimplex
{
	/// <summary>
	///  First vertex of the simplex.
	/// </summary>
	public Vector3 A;

	/// <summary>
	///  Second vertex of the simplex.
	/// </summary>
	public Vector3 B;

	/// <summary>
	///  Third vertex of the simplex.
	/// </summary>
	public Vector3 C;

	/// <summary>
	///  Fourth vertex of the simplex.
	/// </summary>
	public Vector3 D;

	/// <summary>
	///  Current state of the simplex.
	/// </summary>
	public SimplexState State;

	/// <summary>
	///  Gets the point on the simplex closest to the origin.
	/// </summary>
	/// <param name="point">Closest point to the origin.</param>
	/// <returns>Whether or not the simplex encloses the origin.</returns>
	public bool GetPointClosestToOrigin(out Vector3 point)
	{
		switch (State)
		{
		case SimplexState.Point:
			point = A;
			break;
		case SimplexState.Segment:
			GetPointOnSegmentClosestToOrigin(out point);
			break;
		case SimplexState.Triangle:
			GetPointOnTriangleClosestToOrigin(out point);
			break;
		case SimplexState.Tetrahedron:
			return GetPointOnTetrahedronClosestToOrigin(out point);
		default:
			point = Toolbox.ZeroVector;
			break;
		}
		return false;
	}

	/// <summary>
	///  Gets the closest point on the segment to the origin.
	/// </summary>
	/// <param name="point">Closest point.</param>
	public void GetPointOnSegmentClosestToOrigin(out Vector3 point)
	{
		Vector3.Subtract(ref B, ref A, out var result);
		Vector3.Dot(ref result, ref A, out var result2);
		float scaleFactor = (0f - result2) / result.LengthSquared();
		Vector3.Multiply(ref result, scaleFactor, out point);
		Vector3.Add(ref point, ref A, out point);
	}

	/// <summary>
	///  Gets the closest point on the triangle to the origin.
	/// </summary>
	/// <param name="point">Closest point.</param>
	public void GetPointOnTriangleClosestToOrigin(out Vector3 point)
	{
		Vector3.Subtract(ref B, ref A, out var result);
		Vector3.Subtract(ref C, ref A, out var result2);
		Vector3.Dot(ref result, ref C, out var result3);
		Vector3.Dot(ref result2, ref C, out var result4);
		result3 = 0f - result3;
		result4 = 0f - result4;
		if (result4 >= 0f && result3 <= result4)
		{
			State = SimplexState.Point;
			A = C;
			point = A;
			return;
		}
		Vector3.Dot(ref result, ref A, out var result5);
		Vector3.Dot(ref result2, ref A, out var result6);
		result5 = 0f - result5;
		result6 = 0f - result6;
		float num = result3 * result6 - result5 * result4;
		if (num <= 0f && result6 > 0f && result4 < 0f)
		{
			State = SimplexState.Segment;
			B = C;
			float scaleFactor = result6 / (result6 - result4);
			Vector3.Multiply(ref result2, scaleFactor, out point);
			Vector3.Add(ref point, ref A, out point);
			return;
		}
		Vector3.Dot(ref result, ref B, out var result7);
		Vector3.Dot(ref result2, ref B, out var result8);
		result7 = 0f - result7;
		result8 = 0f - result8;
		float num2 = result7 * result4 - result3 * result8;
		float num3;
		float num4;
		if (num2 <= 0f && (num3 = result8 - result7) > 0f && (num4 = result3 - result4) > 0f)
		{
			State = SimplexState.Segment;
			A = C;
			float scaleFactor2 = num3 / (num3 + num4);
			Vector3.Subtract(ref C, ref B, out var result9);
			Vector3.Multiply(ref result9, scaleFactor2, out point);
			Vector3.Add(ref point, ref B, out point);
		}
		else
		{
			float num5 = result5 * result8 - result7 * result6;
			float num6 = 1f / (num2 + num + num5);
			float scaleFactor3 = num * num6;
			float scaleFactor4 = num5 * num6;
			Vector3.Multiply(ref result, scaleFactor3, out point);
			Vector3.Multiply(ref result2, scaleFactor4, out var result10);
			Vector3.Add(ref A, ref point, out point);
			Vector3.Add(ref point, ref result10, out point);
		}
	}

	/// <summary>
	///  Gets the closest point on the tetrahedron to the origin.
	/// </summary>
	/// <param name="point">Closest point.</param>
	/// <returns>Whether or not the simplex encloses the origin.</returns>
	public bool GetPointOnTetrahedronClosestToOrigin(out Vector3 point)
	{
		SimpleSimplex simpleSimplex = default(SimpleSimplex);
		point = default(Vector3);
		float num = float.MaxValue;
		if (TryTetrahedronTriangle(ref A, ref C, ref D, ref B, out var simplex, out var point2))
		{
			point = point2;
			simpleSimplex = simplex;
			num = point2.LengthSquared();
		}
		float num2;
		if (TryTetrahedronTriangle(ref C, ref B, ref D, ref A, out simplex, out point2) && (num2 = point2.LengthSquared()) < num)
		{
			point = point2;
			simpleSimplex = simplex;
			num = num2;
		}
		if (TryTetrahedronTriangle(ref B, ref A, ref D, ref C, out simplex, out point2) && (num2 = point2.LengthSquared()) < num)
		{
			point = point2;
			simpleSimplex = simplex;
			num = num2;
		}
		if (num < float.MaxValue)
		{
			this = simpleSimplex;
			return false;
		}
		return true;
	}

	private static bool TryTetrahedronTriangle(ref Vector3 A, ref Vector3 B, ref Vector3 C, ref Vector3 otherPoint, out SimpleSimplex simplex, out Vector3 point)
	{
		simplex = default(SimpleSimplex);
		point = default(Vector3);
		Vector3.Subtract(ref B, ref A, out var result);
		Vector3.Subtract(ref C, ref A, out var result2);
		Vector3.Cross(ref result, ref result2, out var result3);
		Vector3.Subtract(ref otherPoint, ref A, out var result4);
		Vector3.Dot(ref A, ref result3, out var result5);
		Vector3.Dot(ref result4, ref result3, out var result6);
		if (result5 * result6 > 0f)
		{
			Vector3.Dot(ref result, ref C, out var result7);
			Vector3.Dot(ref result2, ref C, out var result8);
			result7 = 0f - result7;
			result8 = 0f - result8;
			if (result8 >= 0f && result7 <= result8)
			{
				simplex.State = SimplexState.Point;
				simplex.A = C;
				point = C;
				return true;
			}
			Vector3.Dot(ref result, ref A, out var result9);
			Vector3.Dot(ref result2, ref A, out var result10);
			result9 = 0f - result9;
			result10 = 0f - result10;
			float num = result7 * result10 - result9 * result8;
			if (num <= 0f && result10 > 0f && result8 < 0f)
			{
				simplex.State = SimplexState.Segment;
				simplex.A = A;
				simplex.B = C;
				float scaleFactor = result10 / (result10 - result8);
				Vector3.Multiply(ref result2, scaleFactor, out point);
				Vector3.Add(ref point, ref A, out point);
				return true;
			}
			Vector3.Dot(ref result, ref B, out var result11);
			Vector3.Dot(ref result2, ref B, out var result12);
			result11 = 0f - result11;
			result12 = 0f - result12;
			float num2 = result11 * result8 - result7 * result12;
			float num3;
			float num4;
			if (num2 <= 0f && (num3 = result12 - result11) > 0f && (num4 = result7 - result8) > 0f)
			{
				simplex.State = SimplexState.Segment;
				simplex.A = B;
				simplex.B = C;
				float scaleFactor2 = num3 / (num3 + num4);
				Vector3.Subtract(ref C, ref B, out var result13);
				Vector3.Multiply(ref result13, scaleFactor2, out point);
				Vector3.Add(ref point, ref B, out point);
				return true;
			}
			float num5 = result9 * result12 - result11 * result10;
			simplex.A = A;
			simplex.B = B;
			simplex.C = C;
			simplex.State = SimplexState.Triangle;
			float num6 = 1f / (num2 + num + num5);
			float scaleFactor3 = num5 * num6;
			float scaleFactor4 = num * num6;
			Vector3.Multiply(ref result, scaleFactor4, out point);
			Vector3.Multiply(ref result2, scaleFactor3, out var result14);
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
	public void AddNewSimplexPoint(ref Vector3 point)
	{
		switch (State)
		{
		case SimplexState.Empty:
			State = SimplexState.Point;
			A = point;
			break;
		case SimplexState.Point:
			State = SimplexState.Segment;
			B = point;
			break;
		case SimplexState.Segment:
			State = SimplexState.Triangle;
			C = point;
			break;
		case SimplexState.Triangle:
			State = SimplexState.Tetrahedron;
			D = point;
			break;
		}
	}

	/// <summary>
	///  Gets the error tolerance of the simplex.
	/// </summary>
	/// <returns>Error tolerance of the simplex.</returns>
	public float GetErrorTolerance()
	{
		return State switch
		{
			SimplexState.Point => A.LengthSquared(), 
			SimplexState.Segment => MathHelper.Max(A.LengthSquared(), B.LengthSquared()), 
			SimplexState.Triangle => MathHelper.Max(A.LengthSquared(), MathHelper.Max(B.LengthSquared(), C.LengthSquared())), 
			SimplexState.Tetrahedron => MathHelper.Max(A.LengthSquared(), MathHelper.Max(B.LengthSquared(), MathHelper.Max(C.LengthSquared(), D.LengthSquared()))), 
			_ => 1f, 
		};
	}
}
