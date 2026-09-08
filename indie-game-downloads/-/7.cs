using System.Collections.Generic;
using _0004;
using D;
using Microsoft.Xna.Framework;
using N;
using Y;
using l;
using r;
using y;

namespace _000F
{
	internal enum _7
	{
		Accepted,
		Rejected,
		TooDeep,
		Obstructed,
		HeadObstructed,
		NoHit
	}
}
namespace _0002
{
	internal struct _7
	{
		internal D.h a5h;

		internal Y.a a5b;

		internal _7(Y.a P_0, D.h P_1)
		{
			a5b = P_0;
			a5h = P_1;
		}
	}
}
namespace _000E
{
	internal struct _7
	{
		public N._7 VolumeDistribution;

		public Vector3 Center;

		public float Volume;
	}
}
namespace _0017
{
	internal struct _7
	{
		public static float ProgressionEpsilon = 1E-08f;

		public static float DistanceConvergenceEpsilon = 1E-07f;

		public a SimplexA;

		public a SimplexB;

		public Vector3 A;

		public Vector3 B;

		public Vector3 C;

		public Vector3 D;

		public b State;

		public float U;

		public float V;

		public float W;

		public N._0006 LocalTransformB;

		internal float a5h;

		private float a5b;

		public float ErrorTolerance => a5h;

		private _7(ref N._0006 P_0)
		{
			a5b = float.MaxValue;
			a5h = 0f;
			LocalTransformB = P_0;
			State = b.Point;
			SimplexA = default(a);
			SimplexB = new a
			{
				A = P_0.Position
			};
			Vector3.Negate(ref P_0.Position, out A);
			B = default(Vector3);
			C = default(Vector3);
			D = default(Vector3);
			U = 0f;
			V = 0f;
			W = 0f;
		}

		public _7(ref _6 cachedSimplex, ref N._0006 localTransformB)
		{
			a5b = float.MaxValue;
			a5h = 0f;
			LocalTransformB = localTransformB;
			State = cachedSimplex.State;
			SimplexA = cachedSimplex.LocalSimplexA;
			SimplexB = default(a);
			U = 0f;
			V = 0f;
			W = 0f;
			N._7 result;
			switch (State)
			{
			case b.Point:
				Vector3.Transform(ref cachedSimplex.LocalSimplexB.A, ref LocalTransformB.Orientation, out SimplexB.A);
				Vector3.Add(ref SimplexB.A, ref LocalTransformB.Position, out SimplexB.A);
				Vector3.Subtract(ref SimplexA.A, ref SimplexB.A, out A);
				B = default(Vector3);
				C = default(Vector3);
				D = default(Vector3);
				break;
			case b.Segment:
				N._7.CreateFromQuaternion(ref localTransformB.Orientation, out result);
				N._7.Transform(ref cachedSimplex.LocalSimplexB.A, ref result, out SimplexB.A);
				N._7.Transform(ref cachedSimplex.LocalSimplexB.B, ref result, out SimplexB.B);
				Vector3.Add(ref SimplexB.A, ref LocalTransformB.Position, out SimplexB.A);
				Vector3.Add(ref SimplexB.B, ref LocalTransformB.Position, out SimplexB.B);
				Vector3.Subtract(ref SimplexA.A, ref SimplexB.A, out A);
				Vector3.Subtract(ref SimplexA.B, ref SimplexB.B, out B);
				C = default(Vector3);
				D = default(Vector3);
				break;
			case b.Triangle:
				N._7.CreateFromQuaternion(ref localTransformB.Orientation, out result);
				N._7.Transform(ref cachedSimplex.LocalSimplexB.A, ref result, out SimplexB.A);
				N._7.Transform(ref cachedSimplex.LocalSimplexB.B, ref result, out SimplexB.B);
				N._7.Transform(ref cachedSimplex.LocalSimplexB.C, ref result, out SimplexB.C);
				Vector3.Add(ref SimplexB.A, ref LocalTransformB.Position, out SimplexB.A);
				Vector3.Add(ref SimplexB.B, ref LocalTransformB.Position, out SimplexB.B);
				Vector3.Add(ref SimplexB.C, ref LocalTransformB.Position, out SimplexB.C);
				Vector3.Subtract(ref SimplexA.A, ref SimplexB.A, out A);
				Vector3.Subtract(ref SimplexA.B, ref SimplexB.B, out B);
				Vector3.Subtract(ref SimplexA.C, ref SimplexB.C, out C);
				D = default(Vector3);
				break;
			case b.Tetrahedron:
				N._7.CreateFromQuaternion(ref localTransformB.Orientation, out result);
				N._7.Transform(ref cachedSimplex.LocalSimplexB.A, ref result, out SimplexB.A);
				N._7.Transform(ref cachedSimplex.LocalSimplexB.B, ref result, out SimplexB.B);
				N._7.Transform(ref cachedSimplex.LocalSimplexB.C, ref result, out SimplexB.C);
				N._7.Transform(ref cachedSimplex.LocalSimplexB.D, ref result, out SimplexB.D);
				Vector3.Add(ref SimplexB.A, ref LocalTransformB.Position, out SimplexB.A);
				Vector3.Add(ref SimplexB.B, ref LocalTransformB.Position, out SimplexB.B);
				Vector3.Add(ref SimplexB.C, ref LocalTransformB.Position, out SimplexB.C);
				Vector3.Add(ref SimplexB.D, ref LocalTransformB.Position, out SimplexB.D);
				Vector3.Subtract(ref SimplexA.A, ref SimplexB.A, out A);
				Vector3.Subtract(ref SimplexA.B, ref SimplexB.B, out B);
				Vector3.Subtract(ref SimplexA.C, ref SimplexB.C, out C);
				Vector3.Subtract(ref SimplexA.D, ref SimplexB.D, out D);
				break;
			default:
				A = default(Vector3);
				B = default(Vector3);
				C = default(Vector3);
				D = default(Vector3);
				break;
			}
		}

		public void UpdateCachedSimplex(ref _6 simplex)
		{
			simplex.LocalSimplexA = SimplexA;
			N._7 result;
			switch (State)
			{
			case b.Point:
			{
				Vector3.Subtract(ref SimplexB.A, ref LocalTransformB.Position, out simplex.LocalSimplexB.A);
				Quaternion.Conjugate(ref LocalTransformB.Orientation, out var result2);
				Vector3.Transform(ref simplex.LocalSimplexB.A, ref result2, out simplex.LocalSimplexB.A);
				break;
			}
			case b.Segment:
				Vector3.Subtract(ref SimplexB.A, ref LocalTransformB.Position, out simplex.LocalSimplexB.A);
				Vector3.Subtract(ref SimplexB.B, ref LocalTransformB.Position, out simplex.LocalSimplexB.B);
				N._7.CreateFromQuaternion(ref LocalTransformB.Orientation, out result);
				N._7.TransformTranspose(ref simplex.LocalSimplexB.A, ref result, out simplex.LocalSimplexB.A);
				N._7.TransformTranspose(ref simplex.LocalSimplexB.B, ref result, out simplex.LocalSimplexB.B);
				break;
			case b.Triangle:
				Vector3.Subtract(ref SimplexB.A, ref LocalTransformB.Position, out simplex.LocalSimplexB.A);
				Vector3.Subtract(ref SimplexB.B, ref LocalTransformB.Position, out simplex.LocalSimplexB.B);
				Vector3.Subtract(ref SimplexB.C, ref LocalTransformB.Position, out simplex.LocalSimplexB.C);
				N._7.CreateFromQuaternion(ref LocalTransformB.Orientation, out result);
				N._7.TransformTranspose(ref simplex.LocalSimplexB.A, ref result, out simplex.LocalSimplexB.A);
				N._7.TransformTranspose(ref simplex.LocalSimplexB.B, ref result, out simplex.LocalSimplexB.B);
				N._7.TransformTranspose(ref simplex.LocalSimplexB.C, ref result, out simplex.LocalSimplexB.C);
				break;
			case b.Tetrahedron:
				Vector3.Subtract(ref SimplexB.A, ref LocalTransformB.Position, out simplex.LocalSimplexB.A);
				Vector3.Subtract(ref SimplexB.B, ref LocalTransformB.Position, out simplex.LocalSimplexB.B);
				Vector3.Subtract(ref SimplexB.C, ref LocalTransformB.Position, out simplex.LocalSimplexB.C);
				Vector3.Subtract(ref SimplexB.D, ref LocalTransformB.Position, out simplex.LocalSimplexB.D);
				N._7.CreateFromQuaternion(ref LocalTransformB.Orientation, out result);
				N._7.TransformTranspose(ref simplex.LocalSimplexB.A, ref result, out simplex.LocalSimplexB.A);
				N._7.TransformTranspose(ref simplex.LocalSimplexB.B, ref result, out simplex.LocalSimplexB.B);
				N._7.TransformTranspose(ref simplex.LocalSimplexB.C, ref result, out simplex.LocalSimplexB.C);
				N._7.TransformTranspose(ref simplex.LocalSimplexB.D, ref result, out simplex.LocalSimplexB.D);
				break;
			}
			simplex.State = State;
		}

		public bool GetPointClosestToOrigin(out Vector3 point)
		{
			switch (State)
			{
			case b.Point:
				point = A;
				U = 1f;
				break;
			case b.Segment:
				GetPointOnSegmentClosestToOrigin(out point);
				break;
			case b.Triangle:
				GetPointOnTriangleClosestToOrigin(out point);
				break;
			case b.Tetrahedron:
				return GetPointOnTetrahedronClosestToOrigin(out point);
			default:
				point = r.X.ZeroVector;
				break;
			}
			return false;
		}

		public void GetPointOnSegmentClosestToOrigin(out Vector3 point)
		{
			Vector3.Subtract(ref B, ref A, out var result);
			Vector3.Dot(ref result, ref A, out var result2);
			if (result2 > 0f)
			{
				State = b.Point;
				U = 1f;
				point = A;
				return;
			}
			Vector3.Dot(ref result, ref B, out var result3);
			if (result3 > 0f)
			{
				U = result3 / result.LengthSquared();
				V = 1f - U;
				Vector3.Multiply(ref result, V, out point);
				Vector3.Add(ref point, ref A, out point);
			}
			else
			{
				A = B;
				SimplexA.A = SimplexA.B;
				SimplexB.A = SimplexB.B;
				State = b.Point;
				U = 1f;
				point = A;
			}
		}

		public void GetPointOnTriangleClosestToOrigin(out Vector3 point)
		{
			Vector3.Subtract(ref B, ref A, out var result);
			Vector3.Subtract(ref C, ref A, out var result2);
			Vector3.Dot(ref result, ref A, out var result3);
			Vector3.Dot(ref result2, ref A, out var result4);
			result3 = 0f - result3;
			result4 = 0f - result4;
			if (result4 <= 0f && result3 <= 0f)
			{
				State = b.Point;
				U = 1f;
				point = A;
				return;
			}
			Vector3.Dot(ref result, ref B, out var result5);
			Vector3.Dot(ref result2, ref B, out var result6);
			result5 = 0f - result5;
			result6 = 0f - result6;
			if (result5 >= 0f && result6 <= result5)
			{
				State = b.Point;
				A = B;
				U = 1f;
				SimplexA.A = SimplexA.B;
				SimplexB.A = SimplexB.B;
				point = B;
				return;
			}
			float num = result3 * result6 - result5 * result4;
			if (num <= 0f && result3 > 0f && result5 < 0f)
			{
				State = b.Segment;
				V = result3 / (result3 - result5);
				U = 1f - V;
				Vector3.Multiply(ref result, V, out point);
				Vector3.Add(ref point, ref A, out point);
				return;
			}
			Vector3.Dot(ref result, ref C, out var result7);
			Vector3.Dot(ref result2, ref C, out var result8);
			result7 = 0f - result7;
			result8 = 0f - result8;
			if (result8 >= 0f && result7 <= result8)
			{
				State = b.Point;
				A = C;
				SimplexA.A = SimplexA.C;
				SimplexB.A = SimplexB.C;
				U = 1f;
				point = A;
				return;
			}
			float num2 = result7 * result4 - result3 * result8;
			if (num2 <= 0f && result4 > 0f && result8 < 0f)
			{
				State = b.Segment;
				B = C;
				SimplexA.B = SimplexA.C;
				SimplexB.B = SimplexB.C;
				V = result4 / (result4 - result8);
				U = 1f - V;
				Vector3.Multiply(ref result2, V, out point);
				Vector3.Add(ref point, ref A, out point);
				return;
			}
			float num3 = result5 * result8 - result7 * result6;
			float num4;
			float num5;
			if (num3 <= 0f && (num4 = result6 - result5) > 0f && (num5 = result7 - result8) > 0f)
			{
				State = b.Segment;
				A = C;
				SimplexA.A = SimplexA.C;
				SimplexB.A = SimplexB.C;
				U = num4 / (num4 + num5);
				V = 1f - U;
				Vector3.Subtract(ref C, ref B, out var result9);
				Vector3.Multiply(ref result9, U, out point);
				Vector3.Add(ref point, ref B, out point);
			}
			else
			{
				float num6 = 1f / (num3 + num2 + num);
				V = num2 * num6;
				W = num * num6;
				U = 1f - V - W;
				Vector3.Multiply(ref result, V, out point);
				Vector3.Multiply(ref result2, W, out var result10);
				Vector3.Add(ref A, ref point, out point);
				Vector3.Add(ref point, ref result10, out point);
			}
		}

		public bool GetPointOnTetrahedronClosestToOrigin(out Vector3 point)
		{
			_7 obj = default(_7);
			point = default(Vector3);
			float num = float.MaxValue;
			if (_6X(ref A, ref C, ref D, ref SimplexA.A, ref SimplexA.C, ref SimplexA.D, ref SimplexB.A, ref SimplexB.C, ref SimplexB.D, a5h, ref B, out var obj2, out var vector))
			{
				point = vector;
				obj = obj2;
				num = vector.LengthSquared();
			}
			float num2;
			if (_6X(ref B, ref D, ref C, ref SimplexA.B, ref SimplexA.D, ref SimplexA.C, ref SimplexB.B, ref SimplexB.D, ref SimplexB.C, a5h, ref A, out obj2, out vector) && (num2 = vector.LengthSquared()) < num)
			{
				point = vector;
				obj = obj2;
				num = num2;
			}
			if (_6X(ref A, ref D, ref B, ref SimplexA.A, ref SimplexA.D, ref SimplexA.B, ref SimplexB.A, ref SimplexB.D, ref SimplexB.B, a5h, ref C, out obj2, out vector) && (num2 = vector.LengthSquared()) < num)
			{
				point = vector;
				obj = obj2;
				num = num2;
			}
			if (_6X(ref A, ref B, ref C, ref SimplexA.A, ref SimplexA.B, ref SimplexA.C, ref SimplexB.A, ref SimplexB.B, ref SimplexB.C, a5h, ref D, out obj2, out vector) && (num2 = vector.LengthSquared()) < num)
			{
				point = vector;
				obj = obj2;
				num = num2;
			}
			if (num < float.MaxValue)
			{
				obj.LocalTransformB = LocalTransformB;
				obj.a5b = a5b;
				obj.a5h = a5h;
				this = obj;
				return false;
			}
			return true;
		}

		private static bool _6X(ref Vector3 P_0, ref Vector3 P_1, ref Vector3 P_2, ref Vector3 P_3, ref Vector3 P_4, ref Vector3 P_5, ref Vector3 P_6, ref Vector3 P_7, ref Vector3 P_8, float P_9, ref Vector3 P_10, out _7 P_11, out Vector3 P_12)
		{
			P_11 = default(_7);
			P_12 = default(Vector3);
			Vector3.Subtract(ref P_1, ref P_0, out var result);
			Vector3.Subtract(ref P_2, ref P_0, out var result2);
			Vector3.Cross(ref result, ref result2, out var result3);
			Vector3.Subtract(ref P_10, ref P_0, out var result4);
			Vector3.Dot(ref P_0, ref result3, out var result5);
			Vector3.Dot(ref result4, ref result3, out var result6);
			if (result5 * result6 >= -1E-07f * P_9)
			{
				Vector3.Dot(ref result, ref P_0, out var result7);
				Vector3.Dot(ref result2, ref P_0, out var result8);
				result7 = 0f - result7;
				result8 = 0f - result8;
				if (result8 <= 0f && result7 <= 0f)
				{
					P_11.State = b.Point;
					P_11.A = P_0;
					P_11.U = 1f;
					P_11.SimplexA.A = P_3;
					P_11.SimplexB.A = P_6;
					P_12 = P_0;
					return true;
				}
				Vector3.Dot(ref result, ref P_1, out var result9);
				Vector3.Dot(ref result2, ref P_1, out var result10);
				result9 = 0f - result9;
				result10 = 0f - result10;
				if (result9 >= 0f && result10 <= result9)
				{
					P_11.State = b.Point;
					P_11.A = P_1;
					P_11.U = 1f;
					P_11.SimplexA.A = P_4;
					P_11.SimplexB.A = P_7;
					P_12 = P_1;
					return true;
				}
				float num = result7 * result10 - result9 * result8;
				if (num <= 0f && result7 > 0f && result9 < 0f)
				{
					P_11.State = b.Segment;
					P_11.V = result7 / (result7 - result9);
					P_11.U = 1f - P_11.V;
					P_11.A = P_0;
					P_11.B = P_1;
					P_11.SimplexA.A = P_3;
					P_11.SimplexB.A = P_6;
					P_11.SimplexA.B = P_4;
					P_11.SimplexB.B = P_7;
					Vector3.Multiply(ref result, P_11.V, out P_12);
					Vector3.Add(ref P_12, ref P_0, out P_12);
					return true;
				}
				Vector3.Dot(ref result, ref P_2, out var result11);
				Vector3.Dot(ref result2, ref P_2, out var result12);
				result11 = 0f - result11;
				result12 = 0f - result12;
				if (result12 >= 0f && result11 <= result12)
				{
					P_11.State = b.Point;
					P_11.A = P_2;
					P_11.U = 1f;
					P_11.SimplexA.A = P_5;
					P_11.SimplexB.A = P_8;
					P_12 = P_2;
					return true;
				}
				float num2 = result11 * result8 - result7 * result12;
				if (num2 <= 0f && result8 > 0f && result12 < 0f)
				{
					P_11.State = b.Segment;
					P_11.A = P_0;
					P_11.B = P_2;
					P_11.SimplexA.A = P_3;
					P_11.SimplexA.B = P_5;
					P_11.SimplexB.A = P_6;
					P_11.SimplexB.B = P_8;
					P_11.V = result8 / (result8 - result12);
					P_11.U = 1f - P_11.V;
					Vector3.Multiply(ref result2, P_11.V, out P_12);
					Vector3.Add(ref P_12, ref P_0, out P_12);
					return true;
				}
				float num3 = result9 * result12 - result11 * result10;
				float num4;
				float num5;
				if (num3 <= 0f && (num4 = result10 - result9) > 0f && (num5 = result11 - result12) > 0f)
				{
					P_11.State = b.Segment;
					P_11.A = P_1;
					P_11.B = P_2;
					P_11.SimplexA.A = P_4;
					P_11.SimplexA.B = P_5;
					P_11.SimplexB.A = P_7;
					P_11.SimplexB.B = P_8;
					P_11.V = num4 / (num4 + num5);
					P_11.U = 1f - P_11.V;
					Vector3.Subtract(ref P_2, ref P_1, out var result13);
					Vector3.Multiply(ref result13, P_11.V, out P_12);
					Vector3.Add(ref P_12, ref P_1, out P_12);
					return true;
				}
				P_11.A = P_0;
				P_11.B = P_1;
				P_11.C = P_2;
				P_11.SimplexA.A = P_3;
				P_11.SimplexA.B = P_4;
				P_11.SimplexA.C = P_5;
				P_11.SimplexB.A = P_6;
				P_11.SimplexB.B = P_7;
				P_11.SimplexB.C = P_8;
				P_11.State = b.Triangle;
				float num6 = 1f / (num3 + num2 + num);
				P_11.W = num * num6;
				P_11.V = num2 * num6;
				P_11.U = 1f - P_11.V - P_11.W;
				Vector3.Multiply(ref result, P_11.V, out P_12);
				Vector3.Multiply(ref result2, P_11.W, out var result14);
				Vector3.Add(ref P_0, ref P_12, out P_12);
				Vector3.Add(ref P_12, ref result14, out P_12);
				return true;
			}
			return false;
		}

		public bool GetNewSimplexPoint(y.h shapeA, y.h shapeB, int iterationCount, ref Vector3 closestPoint)
		{
			Vector3.Negate(ref closestPoint, out var result);
			shapeA.GetLocalExtremePointWithoutMargin(ref result, out var extremePoint);
			shapeB.GetExtremePointWithoutMargin(closestPoint, ref LocalTransformB, out var extremePoint2);
			Vector3.Subtract(ref extremePoint, ref extremePoint2, out var result2);
			Vector3.Dot(ref result2, ref result, out var result3);
			float num = closestPoint.LengthSquared();
			float num2 = result3 + num;
			if (iterationCount > h.HighGJKIterations && num - a5b < DistanceConvergenceEpsilon * a5h)
			{
				return true;
			}
			if (num < a5b)
			{
				a5b = num;
			}
			switch (State)
			{
			case b.Point:
				if (num2 <= (a5h = MathHelper.Max(A.LengthSquared(), result2.LengthSquared())) * ProgressionEpsilon)
				{
					return true;
				}
				State = b.Segment;
				B = result2;
				SimplexA.B = extremePoint;
				SimplexB.B = extremePoint2;
				return false;
			case b.Segment:
				if (num2 <= (a5h = MathHelper.Max(MathHelper.Max(A.LengthSquared(), B.LengthSquared()), result2.LengthSquared())) * ProgressionEpsilon)
				{
					return true;
				}
				State = b.Triangle;
				C = result2;
				SimplexA.C = extremePoint;
				SimplexB.C = extremePoint2;
				return false;
			case b.Triangle:
				if (num2 <= (a5h = MathHelper.Max(MathHelper.Max(A.LengthSquared(), B.LengthSquared()), MathHelper.Max(C.LengthSquared(), result2.LengthSquared()))) * ProgressionEpsilon)
				{
					return true;
				}
				State = b.Tetrahedron;
				D = result2;
				SimplexA.D = extremePoint;
				SimplexB.D = extremePoint2;
				return false;
			default:
				return false;
			}
		}

		public void GetClosestPoints(out Vector3 closestPointA, out Vector3 closestPointB)
		{
			Vector3 result;
			switch (State)
			{
			case b.Point:
				closestPointA = SimplexA.A;
				closestPointB = SimplexB.A;
				break;
			case b.Segment:
				Vector3.Multiply(ref SimplexA.A, U, out closestPointA);
				Vector3.Multiply(ref SimplexA.B, V, out result);
				Vector3.Add(ref closestPointA, ref result, out closestPointA);
				Vector3.Multiply(ref SimplexB.A, U, out closestPointB);
				Vector3.Multiply(ref SimplexB.B, V, out result);
				Vector3.Add(ref closestPointB, ref result, out closestPointB);
				break;
			case b.Triangle:
				Vector3.Multiply(ref SimplexA.A, U, out closestPointA);
				Vector3.Multiply(ref SimplexA.B, V, out result);
				Vector3.Add(ref closestPointA, ref result, out closestPointA);
				Vector3.Multiply(ref SimplexA.C, W, out result);
				Vector3.Add(ref closestPointA, ref result, out closestPointA);
				Vector3.Multiply(ref SimplexB.A, U, out closestPointB);
				Vector3.Multiply(ref SimplexB.B, V, out result);
				Vector3.Add(ref closestPointB, ref result, out closestPointB);
				Vector3.Multiply(ref SimplexB.C, W, out result);
				Vector3.Add(ref closestPointB, ref result, out closestPointB);
				break;
			default:
				closestPointA = r.X.ZeroVector;
				closestPointB = r.X.ZeroVector;
				break;
			}
		}

		internal void _6_0018()
		{
			switch (State)
			{
			case b.Point:
				if (!(Vector3.Distance(SimplexA.A - SimplexB.A, A) > 0.0001f))
				{
				}
				break;
			case b.Segment:
				Vector3.Distance(SimplexA.A - SimplexB.A, A);
				_ = 0.0001f;
				if (!(Vector3.Distance(SimplexA.B - SimplexB.B, B) > 0.0001f))
				{
				}
				break;
			case b.Triangle:
				Vector3.Distance(SimplexA.A - SimplexB.A, A);
				_ = 0.0001f;
				Vector3.Distance(SimplexA.B - SimplexB.B, B);
				_ = 0.0001f;
				if (!(Vector3.Distance(SimplexA.C - SimplexB.C, C) > 0.0001f))
				{
				}
				break;
			case b.Tetrahedron:
				Vector3.Distance(SimplexA.A - SimplexB.A, A);
				_ = 0.0001f;
				Vector3.Distance(SimplexA.B - SimplexB.B, B);
				_ = 0.0001f;
				Vector3.Distance(SimplexA.C - SimplexB.C, C);
				_ = 0.0001f;
				Vector3.Distance(SimplexA.D - SimplexB.D, D);
				_ = 0.0001f;
				break;
			}
		}
	}
}
namespace _0004
{
	internal struct _7
	{
		public Vector3 LocalOffsetA;

		public Vector3 LocalOffsetB;

		public float BasePenetrationDepth;
	}
}
namespace _0010
{
	internal class _7 : b
	{
		internal new B a5h;

		internal v a5b;

		internal l._7<a> a56;

		private Stack<a> a5a = new Stack<a>(4);

		public B TwistFriction => a5h;

		public v SlidingFriction => a5b;

		public l.X<a> ContactPenetrationConstraints => new l.X<a>(a56);

		public _7()
		{
			a56 = new l._7<a>(4);
			for (int i = 0; i < 4; i++)
			{
				a a2 = new a();
				Add(a2);
				a2.Tag = i;
				a5a.Push(a2);
			}
			a5b = new v();
			Add(a5b);
			a5h = new B();
			Add(a5h);
		}

		public override void CleanUp()
		{
			for (int num = a56.a5h - 1; num >= 0; num--)
			{
				a a2 = a56.Elements[num];
				a2.CleanUp();
				a56.RemoveAt(num);
				a5a.Push(a2);
			}
			if (a5h.isActive)
			{
				a5h.bV();
				a5b.bV();
			}
		}

		public override void AddContact(_0004.h contact)
		{
			a a2 = a5a.Pop();
			a2.Setup(this, contact);
			a56.Add(a2);
			if (a56.a5h == 1)
			{
				a5h._6E(this);
				a5b._6E(this);
			}
		}

		public override void RemoveContact(_0004.h contact)
		{
			for (int i = 0; i < a56.a5h; i++)
			{
				a a2;
				if ((a2 = a56.Elements[i]).a5h == contact)
				{
					a2.CleanUp();
					a56.RemoveAt(i);
					a5a.Push(a2);
					break;
				}
			}
			if (a56.a5h == 0)
			{
				a5h.bV();
				a5b.bV();
			}
		}

		public sealed override void Update(float dt)
		{
			for (int i = 0; i < a56.a5h; i++)
			{
				UpdateUpdateable(a56.Elements[i], dt);
			}
			UpdateUpdateable(a5b, dt);
			UpdateUpdateable(a5h, dt);
		}

		public sealed override void ExclusiveUpdate()
		{
			for (int i = 0; i < a56.a5h; i++)
			{
				ExclusiveUpdateUpdateable(a56.Elements[i]);
			}
			ExclusiveUpdateUpdateable(a5b);
			ExclusiveUpdateUpdateable(a5h);
		}

		public sealed override float SolveIteration()
		{
			int activeConstraints = 0;
			for (int i = 0; i < a56.a5h; i++)
			{
				SolveUpdateable(a56.Elements[i], ref activeConstraints);
			}
			SolveUpdateable(a5b, ref activeConstraints);
			SolveUpdateable(a5h, ref activeConstraints);
			isActiveInSolver = activeConstraints > 0;
			return solverSettings.a5a + 1f;
		}
	}
}
namespace _0001
{
	internal interface _7 : h, global::r.h
	{
		void Update(float dt);
	}
}
