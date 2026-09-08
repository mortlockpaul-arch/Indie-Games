using System;
using System.Collections.Generic;
using _0004;
using BEPUphysics.DataStructures;
using E;
using Microsoft.Xna.Framework;
using N;
using l;
using p;
using y;

namespace r;

internal static class X
{
	public const float BigEpsilon = 1E-05f;

	public const float Epsilon = 1E-07f;

	public static readonly Vector3 NoVector = new Vector3(float.MinValue, float.MinValue, float.MinValue);

	public static Vector3 BackVector = Vector3.Backward;

	public static Vector3 DownVector = Vector3.Down;

	public static Vector3 ForwardVector = Vector3.Forward;

	public static Quaternion IdentityOrientation = Quaternion.Identity;

	public static Vector3 LeftVector = Vector3.Left;

	public static Vector3 RightVector = Vector3.Right;

	public static Vector3 UpVector = Vector3.Up;

	public static Matrix ZeroMatrix = new Matrix(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);

	public static Vector3 ZeroVector = Vector3.Zero;

	public static N._0006 RigidIdentity = N._0006.Identity;

	public static bool FindRayTriangleIntersection(ref Ray ray, float maximumLength, y._0006 sidedness, ref Vector3 a, ref Vector3 b, ref Vector3 c, out _0006 hit)
	{
		hit = default(_0006);
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
		case y._0006.DoubleSided:
			if (result3 <= 0f)
			{
				Vector3.Negate(ref hit.Normal, out hit.Normal);
				result3 = 0f - result3;
			}
			break;
		case y._0006.Clockwise:
			if (result3 <= 0f)
			{
				return false;
			}
			break;
		case y._0006.Counterclockwise:
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

	public static bool GetSegmentPlaneIntersection(Vector3 a, Vector3 b, Vector3 d, Vector3 e, Vector3 f, out Vector3 q)
	{
		Plane p = default(Plane);
		p.Normal = Vector3.Cross(e - d, f - d);
		p.D = Vector3.Dot(p.Normal, d);
		float num;
		return GetSegmentPlaneIntersection(a, b, p, out num, out q);
	}

	public static bool GetSegmentPlaneIntersection(Vector3 a, Vector3 b, Plane p, out Vector3 q)
	{
		if (GetLinePlaneIntersection(ref a, ref b, ref p, out var num, out q) && num >= 0f)
		{
			return num <= 1f;
		}
		return false;
	}

	public static bool GetSegmentPlaneIntersection(Vector3 a, Vector3 b, Plane p, out float t, out Vector3 q)
	{
		if (GetLinePlaneIntersection(ref a, ref b, ref p, out t, out q) && t >= 0f)
		{
			return t <= 1f;
		}
		return false;
	}

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

	public static _0004._0006 GetClosestPointOnTriangleToPoint(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 p, out Vector3 closestPoint)
	{
		Vector3.Subtract(ref b, ref a, out var result);
		Vector3.Subtract(ref c, ref a, out var result2);
		Vector3.Subtract(ref p, ref a, out var result3);
		Vector3.Dot(ref result, ref result3, out var result4);
		Vector3.Dot(ref result2, ref result3, out var result5);
		if (result4 <= 0f && result5 < 0f)
		{
			closestPoint = a;
			return _0004._0006.A;
		}
		Vector3.Subtract(ref p, ref b, out var result6);
		Vector3.Dot(ref result, ref result6, out var result7);
		Vector3.Dot(ref result2, ref result6, out var result8);
		if (result7 >= 0f && result8 <= result7)
		{
			closestPoint = b;
			return _0004._0006.B;
		}
		float num = result4 * result8 - result7 * result5;
		float scaleFactor;
		if (num <= 0f && result4 >= 0f && result7 <= 0f)
		{
			scaleFactor = result4 / (result4 - result7);
			Vector3.Multiply(ref result, scaleFactor, out closestPoint);
			Vector3.Add(ref closestPoint, ref a, out closestPoint);
			return _0004._0006.AB;
		}
		Vector3.Subtract(ref p, ref c, out var result9);
		Vector3.Dot(ref result, ref result9, out var result10);
		Vector3.Dot(ref result2, ref result9, out var result11);
		if (result11 >= 0f && result10 <= result11)
		{
			closestPoint = c;
			return _0004._0006.C;
		}
		float num2 = result10 * result5 - result4 * result11;
		float scaleFactor2;
		if (num2 <= 0f && result5 >= 0f && result11 <= 0f)
		{
			scaleFactor2 = result5 / (result5 - result11);
			Vector3.Multiply(ref result2, scaleFactor2, out closestPoint);
			Vector3.Add(ref closestPoint, ref a, out closestPoint);
			return _0004._0006.AC;
		}
		float num3 = result7 * result11 - result10 * result8;
		if (num3 <= 0f && result8 - result7 >= 0f && result10 - result11 >= 0f)
		{
			scaleFactor2 = (result8 - result7) / (result8 - result7 + (result10 - result11));
			Vector3.Subtract(ref c, ref b, out closestPoint);
			Vector3.Multiply(ref closestPoint, scaleFactor2, out closestPoint);
			Vector3.Add(ref closestPoint, ref b, out closestPoint);
			return _0004._0006.BC;
		}
		float num4 = 1f / (num3 + num2 + num);
		scaleFactor = num2 * num4;
		scaleFactor2 = num * num4;
		Vector3.Multiply(ref result, scaleFactor, out var result12);
		Vector3.Multiply(ref result2, scaleFactor2, out var result13);
		Vector3.Add(ref a, ref result12, out closestPoint);
		Vector3.Add(ref closestPoint, ref result13, out closestPoint);
		return _0004._0006.ABC;
	}

	[Obsolete("Used for simplex tests; consider using the PairSimplex and its variants instead for simplex-related testing.")]
	public static void GetClosestPointOnTriangleToPoint(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 p, l._7<Vector3> subsimplex, out Vector3 closestPoint)
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

	[Obsolete("Used for simplex tests; consider using the PairSimplex and its variants instead for simplex-related testing.")]
	public static void GetClosestPointOnTriangleToPoint(l._7<Vector3> q, int i, int j, int k, ref Vector3 p, l._7<int> subsimplex, l._7<float> baryCoords, out Vector3 closestPoint)
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

	public static bool IsPointInsideTriangle(ref Vector3 vA, ref Vector3 vB, ref Vector3 vC, ref Vector3 p)
	{
		GetBarycentricCoordinates(ref p, ref vA, ref vB, ref vC, out var aWeight, out var bWeight, out var cWeight);
		if (aWeight > -1E-07f && bWeight > -1E-07f)
		{
			return cWeight > -1E-07f;
		}
		return false;
	}

	public static bool IsPointInsideTriangle(ref Vector3 vA, ref Vector3 vB, ref Vector3 vC, ref Vector3 p, float margin)
	{
		GetBarycentricCoordinates(ref p, ref vA, ref vB, ref vC, out var aWeight, out var bWeight, out var cWeight);
		if (aWeight > 0f - margin && bWeight > 0f - margin)
		{
			return cWeight > 0f - margin;
		}
		return false;
	}

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

	public static float GetSquaredDistanceFromPointToLine(ref Vector3 p, ref Vector3 a, ref Vector3 b)
	{
		Vector3.Subtract(ref p, ref a, out var result);
		Vector3.Subtract(ref b, ref a, out var result2);
		Vector3.Dot(ref result, ref result2, out var result3);
		return result.LengthSquared() - result3 * result3 / result2.LengthSquared();
	}

	public static void GetClosestPointsBetweenSegments(Vector3 p1, Vector3 q1, Vector3 p2, Vector3 q2, out Vector3 c1, out Vector3 c2)
	{
		GetClosestPointsBetweenSegments(ref p1, ref q1, ref p2, ref q2, out var _, out var _, out c1, out c2);
	}

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

	public static float GetDistancePointToPlane(ref Vector3 point, ref Vector3 normal, ref Vector3 pointOnPlane)
	{
		Vector3.Subtract(ref point, ref pointOnPlane, out var result);
		Vector3.Dot(ref normal, ref result, out var result2);
		return result2 / normal.LengthSquared();
	}

	public static void GetPointProjectedOnPlane(ref Vector3 point, ref Vector3 normal, ref Vector3 pointOnPlane, out Vector3 projectedPoint)
	{
		Vector3.Dot(ref normal, ref point, out var result);
		Vector3.Dot(ref pointOnPlane, ref normal, out var result2);
		float scaleFactor = (result - result2) / normal.LengthSquared();
		Vector3.Multiply(ref normal, scaleFactor, out var result3);
		Vector3.Subtract(ref point, ref result3, out projectedPoint);
	}

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

	[Obsolete("This method was used for older GJK simplex tests.  If you need simplex tests, consider the PairSimplex class and its variants.")]
	public static void GetClosestPointOnTetrahedronToPoint(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 d, ref Vector3 p, l._7<Vector3> subsimplex, out Vector3 closestPoint)
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

	[Obsolete("This method was used for older GJK simplex tests.  If you need simplex tests, consider the PairSimplex class and its variants.")]
	public static void GetClosestPointOnTetrahedronToPoint(l._7<Vector3> tetrahedron, ref Vector3 p, l._7<int> subsimplex, l._7<float> baryCoords, out Vector3 closestPoint)
	{
		l._7<int> intList = global::p._6.GetIntList();
		l._7<float> floatList = global::p._6.GetFloatList();
		Vector3 p2 = tetrahedron[0];
		Vector3 p3 = tetrahedron[1];
		Vector3 p4 = tetrahedron[2];
		Vector3 p5 = tetrahedron[3];
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
		if (ArePointsOnOppositeSidesOfPlane(ref p, ref p5, ref p2, ref p3, ref p4))
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
		if (ArePointsOnOppositeSidesOfPlane(ref p, ref p3, ref p2, ref p4, ref p5))
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
		if (ArePointsOnOppositeSidesOfPlane(ref p, ref p4, ref p2, ref p5, ref p3))
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
		if (ArePointsOnOppositeSidesOfPlane(ref p, ref p2, ref p3, ref p5, ref p4))
		{
			GetClosestPointOnTriangleToPoint(tetrahedron, 1, 3, 2, ref p, intList, floatList, out closestPoint2);
			Vector3.Subtract(ref closestPoint2, ref p, out result);
			float num5 = result.LengthSquared();
			if (num5 < num)
			{
				closestPoint = closestPoint2;
				subsimplex.Clear();
				baryCoords.Clear();
				for (int m = 0; m < intList.Count; m++)
				{
					subsimplex.Add(intList[m]);
					baryCoords.Add(floatList[m]);
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
		global::p._6.GiveBack(intList);
		global::p._6.GiveBack(floatList);
	}

	private static Vector3 am(Vector3 P_0, l._7<int> P_1, l._7<Vector3> P_2, out int P_3)
	{
		float num = float.MinValue;
		Vector3 result = ZeroVector;
		P_3 = 0;
		for (int i = 0; i < P_1.a5h; i++)
		{
			Vector3.Dot(ref P_0, ref P_2.Elements[P_1.Elements[i]], out var result2);
			if (result2 > num)
			{
				num = result2;
				result = P_2[P_1[i]];
				P_3 = P_1[i];
			}
		}
		return result;
	}

	private static void aT(Vector3 P_0, l._7<Vector3> P_1, out int P_2, out int P_3)
	{
		float num = float.MinValue;
		float num2 = float.MaxValue;
		P_2 = 0;
		P_3 = 0;
		for (int i = 0; i < P_1.Count; i++)
		{
			Vector3.Dot(ref P_1.Elements[i], ref P_0, out var result);
			if (result > num)
			{
				num = result;
				P_3 = i;
			}
			if (result < num2)
			{
				num2 = result;
				P_2 = i;
			}
		}
	}

	private static void aT(Vector3 P_0, l._7<Vector3> P_1, out int P_2, out int P_3, out float P_4, out float P_5)
	{
		P_5 = float.MinValue;
		P_4 = float.MaxValue;
		P_2 = 0;
		P_3 = 0;
		for (int i = 0; i < P_1.Count; i++)
		{
			Vector3.Dot(ref P_1.Elements[i], ref P_0, out var result);
			if (result > P_5)
			{
				P_5 = result;
				P_3 = i;
			}
			if (result < P_4)
			{
				P_4 = result;
				P_2 = i;
			}
		}
	}

	private static void aq(int P_0, int P_1, l._7<int> P_2)
	{
		bool flag = false;
		int index = 0;
		for (int i = 0; i < P_2.Count; i += 2)
		{
			if ((P_2[i] == P_0 && P_2[i + 1] == P_1) || (P_2[i] == P_1 && P_2[i + 1] == P_0))
			{
				flag = true;
				index = i;
			}
		}
		if (!flag)
		{
			P_2.Add(P_0);
			P_2.Add(P_1);
		}
		else
		{
			P_2.RemoveAt(index);
			P_2.RemoveAt(index);
		}
	}

	private static void aE(l._7<int> P_0, l._7<Vector3> P_1, l._7<int> P_2)
	{
		l._7<int> intList = p._6.GetIntList();
		for (int i = 0; i < P_0.Count; i++)
		{
			intList.Add(P_0[i]);
		}
		P_0.Clear();
		for (int j = 0; j < P_2.Count; j += 3)
		{
			if (intList.Count <= 0)
			{
				break;
			}
			a_0013(P_2, P_1, j, out var vector);
			for (int k = 0; k < intList.Count; k++)
			{
				Vector3.Subtract(ref P_1.Elements[intList.Elements[k]], ref P_1.Elements[P_2.Elements[j]], out var result);
				Vector3.Dot(ref result, ref vector, out var result2);
				if (result2 >= 0f)
				{
					P_0.Add(intList[k]);
					intList.RemoveAt(k);
					k--;
				}
			}
		}
		p._6.GiveBack(intList);
	}

	private static void a_0013(l._7<int> P_0, l._7<Vector3> P_1, int P_2, out Vector3 P_3)
	{
		Vector3 value = P_1.Elements[P_0.Elements[P_2]];
		Vector3.Subtract(ref P_1.Elements[P_0.Elements[P_2 + 1]], ref value, out var result);
		Vector3.Subtract(ref P_1.Elements[P_0.Elements[P_2 + 2]], ref value, out var result2);
		Vector3.Cross(ref result2, ref result, out P_3);
	}

	private static bool ax(l._7<int> P_0, l._7<Vector3> P_1, int P_2, ref Vector3 P_3)
	{
		Vector3 value = P_1.Elements[P_0.Elements[P_2]];
		Vector3.Subtract(ref P_1.Elements[P_0.Elements[P_2 + 1]], ref value, out var result);
		Vector3.Subtract(ref P_1.Elements[P_0.Elements[P_2 + 2]], ref value, out var result2);
		Vector3.Cross(ref result2, ref result, out var result3);
		Vector3.Subtract(ref P_3, ref value, out var result4);
		Vector3.Dot(ref result4, ref result3, out var result5);
		return result5 >= 0f;
	}

	private static void aQ(l._7<int> P_0, l._7<Vector3> P_1)
	{
		Vector3 zeroVector = ZeroVector;
		for (int i = 0; i < P_0.Count; i++)
		{
			zeroVector += P_1[P_0[i]];
		}
		zeroVector /= (float)P_0.Count;
		for (int j = 0; j < P_0.Count; j += 3)
		{
			if (ax(P_0, P_1, j, ref zeroVector))
			{
				int value = P_0[j + 1];
				P_0[j + 1] = P_0[j + 2];
				P_0[j + 2] = value;
			}
		}
	}

	public static void GetConvexHull(IList<Vector3> points, IList<int> indices)
	{
		l._7<Vector3> vectorList = p._6.GetVectorList();
		l._7<int> intList = p._6.GetIntList();
		vectorList.AddRange(points);
		GetConvexHull(vectorList, intList);
		p._6.GiveBack(vectorList);
		for (int i = 0; i < intList.a5h; i++)
		{
			indices.Add(intList[i]);
		}
		p._6.GiveBack(intList);
	}

	public static void GetConvexHull(l._7<Vector3> points, l._7<int> indices)
	{
		l._7<int> intList = p._6.GetIntList();
		l._7<int> intList2 = p._6.GetIntList();
		l._7<int> intList3 = p._6.GetIntList();
		for (int i = 0; i < points.Count; i++)
		{
			intList.Add(i);
		}
		l._7<int> intList4 = p._6.GetIntList();
		aT(Vector3.Up, points, out var num, out var num2);
		if (num == num2)
		{
			throw new ArgumentException("Point set is degenerate.");
		}
		intList4.Add(num);
		intList4.Add(num2);
		Vector3 vector = NoVector;
		for (int j = 0; j < points.Count; j++)
		{
			if (j != num && j != num2)
			{
				vector = Vector3.Cross(points[num] - points[j], points[num2] - points[j]);
				if (vector.LengthSquared() > 1E-05f)
				{
					break;
				}
			}
		}
		float num3 = Vector3.Dot(vector, points[num]);
		aT(vector, points, out num, out num2, out var num4, out var num5);
		if (Math.Abs(num4 - num3) < 1E-05f)
		{
			if (Math.Abs(num5 - num3) < 1E-05f)
			{
				throw new ArgumentException("Point set is degenerate.");
			}
			intList4.Add(num2);
		}
		else
		{
			intList4.Add(num);
		}
		vector = Vector3.Cross(points[intList4[1]] - points[intList4[0]], points[intList4[2]] - points[intList4[0]]);
		num3 = Vector3.Dot(vector, points[intList4[0]]);
		aT(vector, points, out num, out num2, out num4, out num5);
		if (Math.Abs(num4 - num3) < 1E-05f)
		{
			if (Math.Abs(num5 - num3) < 1E-05f)
			{
				throw new ArgumentException("Point set is degenerate.");
			}
			intList4.Add(num2);
		}
		else
		{
			intList4.Add(num);
		}
		if (intList4.Count == 4)
		{
			indices.Add(intList4[0]);
			indices.Add(intList4[1]);
			indices.Add(intList4[2]);
			indices.Add(intList4[1]);
			indices.Add(intList4[2]);
			indices.Add(intList4[3]);
			indices.Add(intList4[2]);
			indices.Add(intList4[3]);
			indices.Add(intList4[0]);
			indices.Add(intList4[3]);
			indices.Add(intList4[0]);
			indices.Add(intList4[1]);
			for (int k = 0; k < 4; k++)
			{
				intList.Remove(intList4[k]);
			}
			aQ(indices, points);
			aE(intList, points, indices);
			p._6.GiveBack(intList4);
			while (intList.Count > 0)
			{
				for (int m = 0; m < indices.Count; m += 3)
				{
					a_0013(indices, points, m, out var vector2);
					Vector3 value = am(vector2, intList, points, out var item);
					Vector3.Subtract(ref value, ref points.Elements[indices.Elements[m]], out var result);
					Vector3.Dot(ref vector2, ref result, out var result2);
					if (!(result2 >= 0f))
					{
						continue;
					}
					intList.Remove(item);
					intList2.Clear();
					intList3.Clear();
					for (int n = 0; n < indices.Count; n += 3)
					{
						if (ax(indices, points, n, ref value))
						{
							aq(indices[n], indices[n + 1], intList2);
							aq(indices[n], indices[n + 2], intList2);
							aq(indices[n + 1], indices[n + 2], intList2);
							indices.RemoveAt(n);
							indices.RemoveAt(n);
							indices.RemoveAt(n);
							n -= 3;
						}
					}
					for (int num6 = 0; num6 < intList2.Count; num6 += 2)
					{
						indices.Add(intList2[num6]);
						indices.Add(intList2[num6 + 1]);
						indices.Add(item);
					}
					aQ(indices, points);
					aE(intList, points, indices);
					break;
				}
			}
			p._6.GiveBack(intList);
			p._6.GiveBack(intList2);
			p._6.GiveBack(intList3);
			return;
		}
		throw new ArgumentException("Could not form an initial tetrahedron from the input points; ensure that the input point set has volume.");
	}

	public static void GetConvexHull(IList<Vector3> points, IList<Vector3> outputSurfacePoints)
	{
		l._7<Vector3> vectorList = p._6.GetVectorList();
		vectorList.AddRange(points);
		GetConvexHull(vectorList, outputSurfacePoints);
		p._6.GiveBack(vectorList);
	}

	public static void GetConvexHull(l._7<Vector3> points, l._7<Vector3> outputSurfacePoints)
	{
		l._7<int> intList = p._6.GetIntList();
		GetConvexHull(points, intList, outputSurfacePoints);
		p._6.GiveBack(intList);
	}

	public static void GetConvexHull(IList<Vector3> points, IList<int> outputIndices, IList<Vector3> outputSurfacePoints)
	{
		l._7<Vector3> vectorList = p._6.GetVectorList();
		l._7<int> intList = p._6.GetIntList();
		vectorList.AddRange(points);
		GetConvexHull(vectorList, intList, outputSurfacePoints);
		p._6.GiveBack(vectorList);
		for (int i = 0; i < intList.a5h; i++)
		{
			outputIndices.Add(intList[i]);
		}
		p._6.GiveBack(intList);
	}

	public static void GetConvexHull(l._7<Vector3> points, l._7<int> outputIndices, IList<Vector3> outputSurfacePoints)
	{
		GetConvexHull(points, outputIndices);
		BEPUphysics.DataStructures.HashSet<int> intSet = p._6.GetIntSet();
		for (int num = outputIndices.Count - 1; num >= 0; num--)
		{
			int num2 = outputIndices[num];
			if (!intSet.Contains(num2))
			{
				outputSurfacePoints.Add(points[num2]);
				intSet.Add(num2);
			}
		}
		p._6.GiveBack(intSet);
	}

	public static bool RayCastSphere(ref Ray ray, ref Vector3 spherePosition, float radius, float maximumLength, out _0006 hit)
	{
		float num = ray.Direction.Length();
		Vector3.Divide(ref ray.Direction, num, out var result);
		maximumLength *= num;
		hit = default(_0006);
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

	public static void GetExpandedBoundingBox(ref y.h shape, ref N._0006 transform, ref Vector3 sweep, out BoundingBox boundingBox)
	{
		shape.GetBoundingBox(ref transform, out boundingBox);
		ExpandBoundingBox(ref boundingBox, ref sweep);
	}

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

	public static float GetAngleFromQuaternion(ref Quaternion q)
	{
		float num = Math.Abs(q.W);
		if (num > 1f)
		{
			return 0f;
		}
		return 2f * (float)Math.Acos(num);
	}

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

	public static Vector3 GetVelocityOfPoint(Vector3 p, E.h entity)
	{
		GetVelocityOfPoint(ref p, entity, out var velocity);
		return velocity;
	}

	public static void GetVelocityOfPoint(ref Vector3 p, E.h entity, out Vector3 velocity)
	{
		Vector3.Subtract(ref p, ref entity.a5h, out var result);
		Vector3.Cross(ref entity.a5_0006, ref result, out velocity);
		Vector3.Add(ref entity.a5a, ref velocity, out velocity);
	}

	internal static void a_0012(ref Quaternion P_0, ref N._7 P_1, ref Vector3 P_2, float P_3, out Quaternion P_4)
	{
		ag(ref P_0, ref P_1, ref P_2, out var quaternion);
		Quaternion.Multiply(ref quaternion, P_3 * 0.5f, out var result);
		Quaternion.Add(ref P_0, ref result, out result);
		ag(ref result, ref P_1, ref P_2, out var quaternion2);
		Quaternion.Multiply(ref quaternion2, P_3 * 0.5f, out var result2);
		Quaternion.Add(ref P_0, ref result2, out result2);
		ag(ref result2, ref P_1, ref P_2, out var quaternion3);
		Quaternion.Multiply(ref quaternion3, P_3, out var result3);
		Quaternion.Add(ref P_0, ref result3, out result3);
		ag(ref result3, ref P_1, ref P_2, out var quaternion4);
		Quaternion.Multiply(ref quaternion, P_3 / 6f, out quaternion);
		Quaternion.Multiply(ref quaternion2, P_3 / 3f, out quaternion2);
		Quaternion.Multiply(ref quaternion3, P_3 / 3f, out quaternion3);
		Quaternion.Multiply(ref quaternion4, P_3 / 6f, out quaternion4);
		Quaternion.Add(ref P_0, ref quaternion, out var result4);
		Quaternion.Add(ref result4, ref quaternion2, out result4);
		Quaternion.Add(ref result4, ref quaternion3, out result4);
		Quaternion.Add(ref result4, ref quaternion4, out result4);
		Quaternion.Normalize(ref result4, out P_4);
	}

	internal static void ag(ref Quaternion P_0, ref N._7 P_1, ref Vector3 P_2, out Quaternion P_3)
	{
		Quaternion.Normalize(ref P_0, out var result);
		N._7.CreateFromQuaternion(ref result, out var result2);
		N._7.MultiplyTransposed(ref result2, ref P_1, out var result3);
		N._7.Multiply(ref result3, ref result2, out result3);
		N._7.Transform(ref P_2, ref result3, out var result4);
		Vector3.Multiply(ref result4, 0.5f, out result4);
		Quaternion quaternion = new Quaternion(result4.X, result4.Y, result4.Z, 0f);
		Quaternion.Multiply(ref quaternion, ref result, out P_3);
	}

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
