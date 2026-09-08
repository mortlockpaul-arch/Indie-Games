using System;
using _0004;
using Microsoft.Xna.Framework;
using N;
using P;
using l;
using r;
using s;
using y;

namespace I
{
	internal static class v
	{
		public static int InnerIterationLimit = 15;

		public static int OuterIterationLimit = 15;

		private static float a5h = 1E-07f;

		private static float a5b = 0.0001f;

		private static float a56 = 1E-09f;

		private static int a5a = 3;

		public static float SurfaceEpsilon
		{
			get
			{
				return a5h;
			}
			set
			{
				if (value > 0f)
				{
					a5h = value;
					return;
				}
				throw new Exception("Epsilon must be positive.");
			}
		}

		public static float DepthRefinementEpsilon
		{
			get
			{
				return a5b;
			}
			set
			{
				if (value > 0f)
				{
					a5b = value;
					return;
				}
				throw new Exception("Epsilon must be positive.");
			}
		}

		public static float RayCastSurfaceEpsilon
		{
			get
			{
				return a56;
			}
			set
			{
				if (value > 0f)
				{
					a56 = value;
					return;
				}
				throw new Exception("Epsilon must be positive.");
			}
		}

		public static int MaximumDepthRefinementIterations
		{
			get
			{
				return a5a;
			}
			set
			{
				if (value > 0)
				{
					a5a = value;
					return;
				}
				throw new Exception("Iteration count must be positive.");
			}
		}

		public static bool GetOverlapPosition(y.h shapeA, y.h shapeB, ref N._0006 transformA, ref N._0006 transformB, out Vector3 position)
		{
			_0006.GetLocalTransform(ref transformA, ref transformB, out var localTransformB);
			bool localOverlapPosition = GetLocalOverlapPosition(shapeA, shapeB, ref localTransformB, out position);
			N._0006.Transform(ref position, ref transformA, out position);
			return localOverlapPosition;
		}

		public static bool GetLocalOverlapPosition(y.h shapeA, y.h shapeB, ref N._0006 localTransformB, out Vector3 position)
		{
			return _6W(shapeA, shapeB, ref localTransformB.Position, ref localTransformB, out position);
		}

		internal static bool _6W(y.h P_0, y.h P_1, ref Vector3 P_2, ref N._0006 P_3, out Vector3 P_4)
		{
			if (P_2.LengthSquared() < 1E-07f)
			{
				P_4 = default(Vector3);
				return true;
			}
			Vector3.Negate(ref P_2, out var result);
			Vector3 direction = P_2;
			_0006.GetLocalMinkowskiExtremePoint(P_0, P_1, ref direction, ref P_3, out var extremePointA, out var extremePointB, out var extremePoint);
			Vector3.Cross(ref extremePoint, ref result, out direction);
			if (direction.LengthSquared() < 1E-07f)
			{
				Vector3.Dot(ref extremePoint, ref P_2, out var result2);
				if (result2 < 0f)
				{
					P_4 = default(Vector3);
					return false;
				}
				Vector3.Dot(ref result, ref P_2, out var result3);
				float scaleFactor = (0f - result3) / (result2 - result3);
				Vector3.Multiply(ref extremePointA, scaleFactor, out P_4);
				return true;
			}
			_0006.GetLocalMinkowskiExtremePoint(P_0, P_1, ref direction, ref P_3, out var extremePointA2, out var extremePointB2, out var extremePoint2);
			Vector3.Subtract(ref extremePoint, ref result, out var result4);
			Vector3.Subtract(ref extremePoint2, ref result, out var result5);
			Vector3.Cross(ref result4, ref result5, out direction);
			int num = 0;
			Vector3 extremePointA3;
			Vector3 extremePoint3;
			while (true)
			{
				_0006.GetLocalMinkowskiExtremePoint(P_0, P_1, ref direction, ref P_3, out extremePointA3, out var extremePointB3, out extremePoint3);
				if (num > OuterIterationLimit)
				{
					break;
				}
				num++;
				Vector3.Cross(ref extremePoint, ref extremePoint3, out result4);
				Vector3.Dot(ref result4, ref result, out var result6);
				if (result6 < 0f)
				{
					extremePoint2 = extremePoint3;
					extremePointA2 = extremePointA3;
					extremePointB2 = extremePointB3;
					Vector3.Subtract(ref extremePoint, ref result, out result4);
					Vector3.Subtract(ref extremePoint3, ref result, out result5);
					Vector3.Cross(ref result4, ref result5, out direction);
					continue;
				}
				Vector3.Cross(ref extremePoint3, ref extremePoint2, out result4);
				Vector3.Dot(ref result4, ref result, out result6);
				if (!(result6 < 0f))
				{
					break;
				}
				extremePoint = extremePoint3;
				extremePointA = extremePointA3;
				extremePointB = extremePointB3;
				Vector3.Subtract(ref extremePoint2, ref result, out result4);
				Vector3.Subtract(ref extremePoint3, ref result, out result5);
				Vector3.Cross(ref result4, ref result5, out direction);
			}
			while (true)
			{
				Vector3.Subtract(ref extremePoint3, ref extremePoint2, out result4);
				Vector3.Subtract(ref extremePoint, ref extremePoint2, out result5);
				Vector3.Cross(ref result4, ref result5, out direction);
				Vector3.Dot(ref direction, ref extremePoint, out var result7);
				if (result7 >= 0f)
				{
					Vector3.Subtract(ref extremePoint, ref result, out result4);
					Vector3.Subtract(ref extremePoint2, ref result, out result5);
					Vector3.Subtract(ref extremePoint3, ref result, out var result8);
					Vector3.Cross(ref result4, ref result5, out var result9);
					Vector3.Dot(ref result9, ref result8, out var result10);
					Vector3.Cross(ref extremePoint, ref extremePoint2, out result9);
					Vector3.Dot(ref result9, ref extremePoint3, out var result11);
					Vector3.Cross(ref P_2, ref result5, out result9);
					Vector3.Dot(ref result9, ref result8, out var result12);
					Vector3.Cross(ref result4, ref P_2, out result9);
					Vector3.Dot(ref result9, ref result8, out var result13);
					if (result10 > 1E-09f)
					{
						float num2 = 1f / result10;
						float num3 = result11 * num2;
						float num4 = result12 * num2;
						float num5 = result13 * num2;
						float num6 = 1f - num3 - num4 - num5;
						P_4 = num4 * extremePointA + num5 * extremePointA2 + num6 * extremePointA3;
					}
					else
					{
						P_4 = default(Vector3);
					}
					return true;
				}
				_0006.GetLocalMinkowskiExtremePoint(P_0, P_1, ref direction, ref P_3, out var extremePointA4, out var extremePointB4, out var extremePoint4);
				Vector3.Dot(ref extremePoint4, ref direction, out var result14);
				if (result14 < 0f)
				{
					P_4 = default(Vector3);
					return false;
				}
				if (result14 - result7 < a5h || num > InnerIterationLimit)
				{
					break;
				}
				num++;
				Vector3.Cross(ref extremePoint4, ref result, out result4);
				Vector3.Dot(ref extremePoint, ref result4, out result7);
				if (result7 >= 0f)
				{
					Vector3.Dot(ref extremePoint2, ref result4, out result7);
					if (result7 >= 0f)
					{
						extremePoint = extremePoint4;
						extremePointA = extremePointA4;
						extremePointB = extremePointB4;
					}
					else
					{
						extremePoint3 = extremePoint4;
						extremePointA3 = extremePointA4;
						Vector3 extremePointB3 = extremePointB4;
					}
				}
				else
				{
					Vector3.Dot(ref extremePoint3, ref result4, out result7);
					if (result7 >= 0f)
					{
						extremePoint2 = extremePoint4;
						extremePointA2 = extremePointA4;
						extremePointB2 = extremePointB4;
					}
					else
					{
						extremePoint = extremePoint4;
						extremePointA = extremePointA4;
						extremePointB = extremePointB4;
					}
				}
			}
			P_4 = default(Vector3);
			return false;
		}

		public static bool AreShapesOverlapping(y.h shapeA, y.h shapeB, ref N._0006 transformA, ref N._0006 transformB)
		{
			_0006.GetLocalTransform(ref transformA, ref transformB, out var localTransformB);
			return AreLocalShapesOverlapping(shapeA, shapeB, ref localTransformB);
		}

		public static bool AreLocalShapesOverlapping(y.h shapeA, y.h shapeB, ref N._0006 localTransformB)
		{
			return _6_0002(shapeA, shapeB, ref localTransformB.Position, ref localTransformB);
		}

		internal static bool _6_0002(y.h P_0, y.h P_1, ref Vector3 P_2, ref N._0006 P_3)
		{
			if (P_2.LengthSquared() < 1E-07f)
			{
				return true;
			}
			Vector3.Negate(ref P_2, out var result);
			Vector3 direction = P_2;
			_0006.GetLocalMinkowskiExtremePoint(P_0, P_1, ref direction, ref P_3, out var extremePoint);
			Vector3.Cross(ref extremePoint, ref result, out direction);
			if (direction.LengthSquared() < 1E-07f)
			{
				Vector3.Dot(ref extremePoint, ref P_2, out var result2);
				if (result2 < 0f)
				{
					return false;
				}
				return true;
			}
			_0006.GetLocalMinkowskiExtremePoint(P_0, P_1, ref direction, ref P_3, out var extremePoint2);
			Vector3.Subtract(ref extremePoint, ref result, out var result3);
			Vector3.Subtract(ref extremePoint2, ref result, out var result4);
			Vector3.Cross(ref result3, ref result4, out direction);
			int num = 0;
			Vector3 extremePoint3;
			while (true)
			{
				_0006.GetLocalMinkowskiExtremePoint(P_0, P_1, ref direction, ref P_3, out extremePoint3);
				if (num > OuterIterationLimit)
				{
					break;
				}
				num++;
				Vector3.Cross(ref extremePoint, ref extremePoint3, out result3);
				Vector3.Dot(ref result3, ref result, out var result5);
				if (result5 < 0f)
				{
					extremePoint2 = extremePoint3;
					Vector3.Subtract(ref extremePoint, ref result, out result3);
					Vector3.Subtract(ref extremePoint3, ref result, out result4);
					Vector3.Cross(ref result3, ref result4, out direction);
					continue;
				}
				Vector3.Cross(ref extremePoint3, ref extremePoint2, out result3);
				Vector3.Dot(ref result3, ref result, out result5);
				if (!(result5 < 0f))
				{
					break;
				}
				extremePoint = extremePoint3;
				Vector3.Subtract(ref extremePoint2, ref result, out result3);
				Vector3.Subtract(ref extremePoint3, ref result, out result4);
				Vector3.Cross(ref result3, ref result4, out direction);
			}
			while (true)
			{
				Vector3.Subtract(ref extremePoint3, ref extremePoint2, out result3);
				Vector3.Subtract(ref extremePoint, ref extremePoint2, out result4);
				Vector3.Cross(ref result3, ref result4, out direction);
				Vector3.Dot(ref direction, ref extremePoint, out var result6);
				if (result6 >= 0f)
				{
					return true;
				}
				_0006.GetLocalMinkowskiExtremePoint(P_0, P_1, ref direction, ref P_3, out var extremePoint4);
				Vector3.Dot(ref extremePoint4, ref direction, out var result7);
				if (result7 < 0f)
				{
					return false;
				}
				if (result7 - result6 < a5h || num > InnerIterationLimit)
				{
					break;
				}
				num++;
				Vector3.Cross(ref extremePoint4, ref result, out result3);
				Vector3.Dot(ref extremePoint, ref result3, out result6);
				if (result6 >= 0f)
				{
					Vector3.Dot(ref extremePoint2, ref result3, out result6);
					if (result6 >= 0f)
					{
						extremePoint = extremePoint4;
					}
					else
					{
						extremePoint3 = extremePoint4;
					}
				}
				else
				{
					Vector3.Dot(ref extremePoint3, ref result3, out result6);
					if (result6 >= 0f)
					{
						extremePoint2 = extremePoint4;
					}
					else
					{
						extremePoint = extremePoint4;
					}
				}
			}
			return false;
		}

		public static void LocalSurfaceCast(y.h shapeA, y.h shapeB, ref N._0006 localTransformB, ref Vector3 direction, out float t, out Vector3 normal)
		{
			Vector3 direction2 = direction;
			_0006.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction2, ref localTransformB, out var extremePoint);
			Vector3.Cross(ref direction, ref extremePoint, out direction2);
			if (direction2.LengthSquared() < 1E-07f)
			{
				float num = direction.LengthSquared();
				if (num > 1E-09f)
				{
					Vector3.Divide(ref direction, (float)Math.Sqrt(num), out normal);
				}
				else
				{
					normal = default(Vector3);
				}
				Vector3.Dot(ref normal, ref direction, out var result);
				Vector3.Dot(ref normal, ref extremePoint, out var result2);
				if (result > 0f)
				{
					t = result2 / result;
				}
				else
				{
					t = 0f;
				}
				return;
			}
			_0006.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction2, ref localTransformB, out var extremePoint2);
			Vector3.Cross(ref extremePoint, ref extremePoint2, out direction2);
			Vector3.Dot(ref direction2, ref direction, out var result3);
			Vector3 result4;
			if (result3 > 0f)
			{
				Vector3.Negate(ref direction2, out direction2);
				result4 = extremePoint;
				extremePoint = extremePoint2;
				extremePoint2 = result4;
			}
			int num2 = 0;
			Vector3 extremePoint3;
			while (true)
			{
				_0006.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction2, ref localTransformB, out extremePoint3);
				if (num2 > OuterIterationLimit)
				{
					t = float.MaxValue;
					normal = r.X.UpVector;
					return;
				}
				num2++;
				Vector3.Cross(ref extremePoint, ref extremePoint3, out result4);
				Vector3.Dot(ref result4, ref direction, out result3);
				if (result3 < 0f)
				{
					extremePoint2 = extremePoint3;
					Vector3.Cross(ref extremePoint, ref extremePoint3, out direction2);
					continue;
				}
				Vector3.Cross(ref extremePoint3, ref extremePoint2, out result4);
				Vector3.Dot(ref result4, ref direction, out result3);
				if (!(result3 < 0f))
				{
					break;
				}
				extremePoint = extremePoint3;
				Vector3.Cross(ref extremePoint2, ref extremePoint3, out direction2);
			}
			num2 = 0;
			float result6;
			while (true)
			{
				Vector3.Subtract(ref extremePoint, ref extremePoint2, out result4);
				Vector3.Subtract(ref extremePoint3, ref extremePoint2, out var result5);
				Vector3.Cross(ref result4, ref result5, out direction2);
				_0006.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction2, ref localTransformB, out var extremePoint4);
				Vector3.Dot(ref direction2, ref extremePoint, out result3);
				Vector3.Dot(ref extremePoint4, ref direction2, out result6);
				if (result6 - result3 < a5h || num2 > InnerIterationLimit)
				{
					break;
				}
				Vector3.Cross(ref extremePoint4, ref direction, out result4);
				Vector3.Dot(ref extremePoint, ref result4, out result3);
				if (result3 >= 0f)
				{
					Vector3.Dot(ref extremePoint2, ref result4, out result3);
					if (result3 >= 0f)
					{
						extremePoint = extremePoint4;
					}
					else
					{
						extremePoint3 = extremePoint4;
					}
				}
				else
				{
					Vector3.Dot(ref extremePoint3, ref result4, out result3);
					if (result3 >= 0f)
					{
						extremePoint2 = extremePoint4;
					}
					else
					{
						extremePoint = extremePoint4;
					}
				}
				num2++;
			}
			float num3 = direction2.LengthSquared();
			if (num3 > 1E-09f)
			{
				Vector3.Divide(ref direction2, (float)Math.Sqrt(num3), out normal);
				Vector3.Dot(ref normal, ref direction, out result3);
				Vector3.Dot(ref normal, ref extremePoint, out result6);
				if (result3 > 0f)
				{
					t = result6 / result3;
				}
				else
				{
					t = 0f;
				}
			}
			else
			{
				normal = Vector3.Up;
				t = 0f;
			}
		}

		public static void LocalSurfaceCast(y.h shapeA, y.h shapeB, ref N._0006 localTransformB, ref Vector3 direction, out float t, out Vector3 normal, out Vector3 position)
		{
			Vector3 direction2 = direction;
			_0006.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction2, ref localTransformB, out var extremePointA, out var extremePoint);
			Vector3.Cross(ref direction, ref extremePoint, out direction2);
			if (direction2.LengthSquared() < 1E-07f)
			{
				float num = direction.LengthSquared();
				if (num > 1E-09f)
				{
					Vector3.Divide(ref direction, (float)Math.Sqrt(num), out normal);
				}
				else
				{
					normal = default(Vector3);
				}
				Vector3.Dot(ref normal, ref direction, out var result);
				Vector3.Dot(ref normal, ref extremePoint, out var result2);
				if (result > 0f)
				{
					t = result2 / result;
				}
				else
				{
					t = 0f;
				}
				position = extremePointA;
				return;
			}
			_0006.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction2, ref localTransformB, out var extremePointA2, out var extremePoint2);
			Vector3.Cross(ref extremePoint, ref extremePoint2, out direction2);
			Vector3.Dot(ref direction2, ref direction, out var result3);
			Vector3 vector;
			if (result3 > 0f)
			{
				Vector3.Negate(ref direction2, out direction2);
				vector = extremePoint;
				extremePoint = extremePoint2;
				extremePoint2 = vector;
				vector = extremePointA;
				extremePointA = extremePointA2;
				extremePointA2 = vector;
			}
			int num2 = 0;
			Vector3 extremePointA3;
			Vector3 extremePoint3;
			while (true)
			{
				_0006.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction2, ref localTransformB, out extremePointA3, out extremePoint3);
				if (num2 > OuterIterationLimit)
				{
					t = float.MaxValue;
					normal = r.X.UpVector;
					position = default(Vector3);
					return;
				}
				num2++;
				Vector3.Cross(ref extremePoint, ref extremePoint3, out vector);
				Vector3.Dot(ref vector, ref direction, out result3);
				if (result3 < 0f)
				{
					extremePoint2 = extremePoint3;
					extremePointA2 = extremePointA3;
					Vector3.Cross(ref extremePoint, ref extremePoint3, out direction2);
					continue;
				}
				Vector3.Cross(ref extremePoint3, ref extremePoint2, out vector);
				Vector3.Dot(ref vector, ref direction, out result3);
				if (!(result3 < 0f))
				{
					break;
				}
				extremePoint = extremePoint3;
				extremePointA = extremePointA3;
				Vector3.Cross(ref extremePoint2, ref extremePoint3, out direction2);
			}
			num2 = 0;
			float result5;
			while (true)
			{
				Vector3.Subtract(ref extremePoint, ref extremePoint2, out vector);
				Vector3.Subtract(ref extremePoint3, ref extremePoint2, out var result4);
				Vector3.Cross(ref vector, ref result4, out direction2);
				_0006.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction2, ref localTransformB, out var extremePointA4, out var extremePoint4);
				Vector3.Dot(ref direction2, ref extremePoint, out result3);
				Vector3.Dot(ref extremePoint4, ref direction2, out result5);
				if (result5 - result3 < a5h || num2 > InnerIterationLimit)
				{
					break;
				}
				Vector3.Cross(ref extremePoint4, ref direction, out vector);
				Vector3.Dot(ref extremePoint, ref vector, out result3);
				if (result3 >= 0f)
				{
					Vector3.Dot(ref extremePoint2, ref vector, out result3);
					if (result3 >= 0f)
					{
						extremePoint = extremePoint4;
						extremePointA = extremePointA4;
					}
					else
					{
						extremePoint3 = extremePoint4;
						extremePointA3 = extremePointA4;
					}
				}
				else
				{
					Vector3.Dot(ref extremePoint3, ref vector, out result3);
					if (result3 >= 0f)
					{
						extremePoint2 = extremePoint4;
						extremePointA2 = extremePointA4;
					}
					else
					{
						extremePoint = extremePoint4;
						extremePointA = extremePointA4;
					}
				}
				num2++;
			}
			float num3 = direction2.LengthSquared();
			if (num3 > 1E-09f)
			{
				Vector3.Divide(ref direction2, (float)Math.Sqrt(num3), out normal);
				Vector3.Dot(ref normal, ref direction, out result3);
				Vector3.Dot(ref normal, ref extremePoint, out result5);
				if (result3 > 0f)
				{
					t = result5 / result3;
				}
				else
				{
					t = 0f;
				}
			}
			else
			{
				normal = Vector3.Up;
				t = 0f;
			}
			Vector3.Multiply(ref direction, t, out position);
			r.X.GetBarycentricCoordinates(ref position, ref extremePoint, ref extremePoint2, ref extremePoint3, out var aWeight, out var bWeight, out var cWeight);
			Vector3.Multiply(ref extremePointA, aWeight, out position);
			Vector3.Multiply(ref extremePointA2, bWeight, out var result6);
			Vector3.Add(ref result6, ref position, out position);
			Vector3.Multiply(ref extremePointA3, cWeight, out result6);
			Vector3.Add(ref result6, ref position, out position);
		}

		private static bool _6_000E(ref Vector3 P_0, ref Vector3 P_1, ref Vector3 P_2, ref Vector3 P_3, ref Vector3 P_4)
		{
			Vector3 vector = Vector3.Cross(P_0 - P_1, P_3 - P_1);
			float num = Vector3.Dot(vector, P_4);
			vector = Vector3.Cross(P_0 - P_3, P_2 - P_3);
			float num2 = Vector3.Dot(vector, P_4);
			vector = Vector3.Cross(P_0 - P_2, P_1 - P_2);
			float num3 = Vector3.Dot(vector, P_4);
			if (!(num <= 0f) || !(num2 <= 0f) || !(num3 <= 0f))
			{
				if (num >= 0f && num2 >= 0f)
				{
					return num3 >= 0f;
				}
				return false;
			}
			return true;
		}

		public static bool GetContact(y.h shapeA, y.h shapeB, ref N._0006 transformA, ref N._0006 transformB, ref Vector3 penetrationAxis, out _0004.b contact)
		{
			_0006.GetLocalTransform(ref transformA, ref transformB, out var localTransformB);
			if (AreLocalShapesOverlapping(shapeA, shapeB, ref localTransformB))
			{
				float num = penetrationAxis.LengthSquared();
				Vector3 result;
				if (num > 1E-07f)
				{
					Vector3.Divide(ref penetrationAxis, (float)Math.Sqrt(num), out result);
					LocalSurfaceCast(shapeA, shapeB, ref localTransformB, ref result, out contact.PenetrationDepth, out contact.Normal);
				}
				else
				{
					contact.PenetrationDepth = float.MaxValue;
					contact.Normal = r.X.UpVector;
				}
				num = localTransformB.Position.LengthSquared();
				if (num > 1E-07f)
				{
					Vector3.Divide(ref localTransformB.Position, (float)Math.Sqrt(num), out result);
					LocalSurfaceCast(shapeA, shapeB, ref localTransformB, ref result, out var num2, out var normal);
					if (num2 < contact.PenetrationDepth)
					{
						contact.Normal = normal;
						contact.PenetrationDepth = num2;
					}
				}
				RefinePenetration(shapeA, shapeB, ref localTransformB, contact.PenetrationDepth, ref contact.Normal, out contact.PenetrationDepth, out contact.Normal, out contact.Position);
				contact.Id = -1;
				N._7.CreateFromQuaternion(ref transformA.Orientation, out var result2);
				N._7.Transform(ref contact.Normal, ref result2, out contact.Normal);
				N._7.Transform(ref contact.Position, ref result2, out contact.Position);
				Vector3.Add(ref contact.Position, ref transformA.Position, out contact.Position);
				return true;
			}
			contact = default(_0004.b);
			return false;
		}

		public static void RefinePenetration(y.h shapeA, y.h shapeB, ref N._0006 localTransformB, float initialDepth, ref Vector3 initialNormal, out float penetrationDepth, out Vector3 refinedNormal, out Vector3 position)
		{
			int num = 0;
			refinedNormal = initialNormal;
			penetrationDepth = initialDepth;
			float num2;
			while (true)
			{
				LocalSurfaceCast(shapeA, shapeB, ref localTransformB, ref refinedNormal, out num2, out var normal, out position);
				if (penetrationDepth - num2 <= a5b || ++num >= a5a)
				{
					break;
				}
				penetrationDepth = num2;
				refinedNormal = normal;
			}
			penetrationDepth = num2;
		}

		public static bool Sweep(y.h shapeA, y.h shapeB, ref Vector3 sweepA, ref Vector3 sweepB, ref N._0006 transformA, ref N._0006 transformB, out r._0006 hit)
		{
			Vector3.Subtract(ref sweepA, ref sweepB, out var result);
			Quaternion.Conjugate(ref transformA.Orientation, out var result2);
			Vector3.Transform(ref result, ref result2, out var result3);
			_0006.GetLocalTransform(ref transformA, ref transformB, out var localTransformB);
			float num = result3.LengthSquared();
			float result4;
			if (num > 1E-09f)
			{
				Vector3.Dot(ref localTransformB.Position, ref result3, out result4);
				result4 /= num;
				result4 += (shapeA.maximumRadius + shapeB.maximumRadius) / (float)Math.Sqrt(num);
			}
			else
			{
				num = 0f;
				result4 = 0f;
			}
			bool flag;
			if (flag = result4 < 0f)
			{
				result4 = 0f;
			}
			Vector3.Multiply(ref result3, result4, out var result5);
			if (!AreSweptShapesIntersecting(shapeA, shapeB, ref result5, ref localTransformB, out hit.Location))
			{
				hit.T = float.MaxValue;
				hit.Normal = default(Vector3);
				hit.Location = default(Vector3);
				return false;
			}
			if (flag)
			{
				hit.T = 0f;
				Vector3.Normalize(ref result3, out hit.Normal);
				Vector3.Transform(ref hit.Normal, ref transformA.Orientation, out hit.Normal);
				Vector3.Transform(ref hit.Location, ref transformA.Orientation, out hit.Location);
				Vector3.Add(ref hit.Location, ref transformA.Position, out hit.Location);
				hit.Location += sweepA * hit.T;
				return true;
			}
			if (_6y(shapeA, shapeB, result4, num, ref result3, ref result5, ref localTransformB, out hit))
			{
				Vector3 vector = (0f - hit.T) * result3;
				_6r(shapeA, shapeB, ref localTransformB, ref vector, out hit.Location);
				N._0006.Transform(ref hit.Location, ref transformA, out hit.Location);
				Vector3.Transform(ref hit.Normal, ref transformA.Orientation, out hit.Normal);
				Vector3.Multiply(ref sweepA, hit.T, out var result6);
				Vector3.Add(ref result6, ref hit.Location, out hit.Location);
				return true;
			}
			return false;
		}

		private static bool _6y(y.h P_0, y.h P_1, float P_2, float P_3, ref Vector3 P_4, ref Vector3 P_5, ref N._0006 P_6, out r._0006 P_7)
		{
			Vector3 result = P_4;
			_6_0001(P_0, P_1, ref P_6, ref P_5, ref result, out var vector, out var vector2);
			Vector3.Cross(ref P_4, ref vector2, out result);
			P_7.Location = default(Vector3);
			if (result.LengthSquared() < 1E-09f)
			{
				if (P_3 > 1E-09f)
				{
					Vector3.Divide(ref P_4, (float)Math.Sqrt(P_3), out P_7.Normal);
				}
				else
				{
					P_7.Normal = default(Vector3);
				}
				Vector3.Dot(ref P_7.Normal, ref P_4, out var result2);
				Vector3.Dot(ref P_7.Normal, ref vector2, out var result3);
				if (result2 > 0f)
				{
					P_7.T = P_2 - result3 / result2;
				}
				else
				{
					P_7.T = P_2;
				}
				if (P_7.T < 0f)
				{
					P_7.T = 0f;
				}
				return P_7.T <= 1f;
			}
			_6_0001(P_0, P_1, ref P_6, ref P_5, ref result, out var vector3, out var vector4);
			Vector3.Cross(ref vector2, ref vector4, out result);
			Vector3.Dot(ref result, ref P_4, out var result4);
			Vector3 vector5;
			if (result4 > 0f)
			{
				Vector3.Negate(ref result, out result);
				vector5 = vector2;
				vector2 = vector4;
				vector4 = vector5;
				vector5 = vector;
				vector = vector3;
				vector3 = vector5;
			}
			int num = 0;
			Vector3 vector7;
			while (true)
			{
				_6_0001(P_0, P_1, ref P_6, ref P_5, ref result, out var vector6, out vector7);
				if (num > OuterIterationLimit)
				{
					P_7.T = float.MaxValue;
					P_7.Normal = default(Vector3);
					P_7.Location = default(Vector3);
					return false;
				}
				num++;
				Vector3.Cross(ref vector2, ref vector7, out vector5);
				Vector3.Dot(ref vector5, ref P_4, out result4);
				if (result4 < 0f)
				{
					vector4 = vector7;
					vector3 = vector6;
					Vector3.Cross(ref vector2, ref vector7, out result);
					continue;
				}
				Vector3.Cross(ref vector7, ref vector4, out vector5);
				Vector3.Dot(ref vector5, ref P_4, out result4);
				if (!(result4 < 0f))
				{
					break;
				}
				vector2 = vector7;
				vector = vector6;
				Vector3.Cross(ref vector4, ref vector7, out result);
			}
			num = 0;
			float result6;
			while (true)
			{
				Vector3.Subtract(ref vector2, ref vector4, out vector5);
				Vector3.Subtract(ref vector7, ref vector4, out var result5);
				Vector3.Cross(ref vector5, ref result5, out result);
				_6_0001(P_0, P_1, ref P_6, ref P_5, ref result, out var vector8, out var vector9);
				Vector3.Dot(ref result, ref vector2, out result4);
				Vector3.Dot(ref vector9, ref result, out result6);
				if (result6 - result4 < a56 || num > InnerIterationLimit)
				{
					break;
				}
				Vector3.Cross(ref vector9, ref P_4, out vector5);
				Vector3.Dot(ref vector2, ref vector5, out result4);
				if (result4 >= 0f)
				{
					Vector3.Dot(ref vector4, ref vector5, out result4);
					if (result4 >= 0f)
					{
						vector2 = vector9;
						vector = vector8;
					}
					else
					{
						vector7 = vector9;
						Vector3 vector6 = vector8;
					}
				}
				else
				{
					Vector3.Dot(ref vector7, ref vector5, out result4);
					if (result4 >= 0f)
					{
						vector4 = vector9;
						vector3 = vector8;
					}
					else
					{
						vector2 = vector9;
						vector = vector8;
					}
				}
				num++;
			}
			float num2 = result.LengthSquared();
			if (num2 > 1E-12f)
			{
				Vector3.Divide(ref result, (float)Math.Sqrt(num2), out P_7.Normal);
				Vector3.Dot(ref P_7.Normal, ref P_4, out result4);
				Vector3.Dot(ref P_7.Normal, ref vector2, out result6);
				if (result4 > 0f)
				{
					_ = result6 / result4;
				}
				P_7.T = P_2 - result6 / result4;
			}
			else
			{
				Vector3.Normalize(ref P_4, out P_7.Normal);
				P_7.T = P_2;
			}
			if (P_7.T < 0f)
			{
				P_7.T = 0f;
			}
			return P_7.T <= 1f;
		}

		internal static void _6r(y.h P_0, y.h P_1, ref N._0006 P_2, ref Vector3 P_3, out Vector3 P_4)
		{
			Vector3.Add(ref P_3, ref P_2.Position, out var result);
			if (result.LengthSquared() < 1E-07f)
			{
				P_4 = default(Vector3);
				return;
			}
			Vector3.Negate(ref P_2.Position, out var result2);
			Vector3 direction = result;
			_0006.GetLocalMinkowskiExtremePoint(P_0, P_1, ref direction, ref P_2, out var extremePointA, out var extremePointB, out var extremePoint);
			Vector3.Cross(ref extremePoint, ref result2, out direction);
			if (direction.LengthSquared() < 1E-07f)
			{
				float num = Vector3.Dot(extremePoint - P_3, result);
				float num2 = Vector3.Dot(result2 - P_3, result);
				float scaleFactor = (0f - num2) / (num - num2);
				Vector3.Multiply(ref extremePointA, scaleFactor, out P_4);
				Vector3.Subtract(ref extremePointB, ref P_2.Position, out var _);
				return;
			}
			_0006.GetLocalMinkowskiExtremePoint(P_0, P_1, ref direction, ref P_2, out var extremePointA2, out var extremePointB2, out var extremePoint2);
			Vector3.Subtract(ref extremePoint, ref result2, out var result4);
			Vector3.Subtract(ref extremePoint2, ref result2, out var result5);
			Vector3.Cross(ref result4, ref result5, out direction);
			Vector3.Subtract(ref result2, ref P_3, out var result6);
			int num3 = 0;
			Vector3 extremePointA3;
			Vector3 extremePoint3;
			Vector3 result8;
			while (true)
			{
				_0006.GetLocalMinkowskiExtremePoint(P_0, P_1, ref direction, ref P_2, out extremePointA3, out var extremePointB3, out extremePoint3);
				if (num3 > OuterIterationLimit)
				{
					break;
				}
				num3++;
				Vector3.Subtract(ref extremePoint, ref result2, out result4);
				Vector3.Subtract(ref extremePoint3, ref result2, out var result7);
				Vector3.Cross(ref result4, ref result7, out result8);
				Vector3.Dot(ref result8, ref result6, out var result9);
				if (result9 < 0f)
				{
					extremePoint2 = extremePoint3;
					extremePointA2 = extremePointA3;
					extremePointB2 = extremePointB3;
					Vector3.Cross(ref result4, ref result7, out direction);
					continue;
				}
				Vector3.Subtract(ref extremePoint2, ref result2, out result5);
				Vector3.Cross(ref result7, ref result5, out result8);
				Vector3.Dot(ref result8, ref result6, out result9);
				if (!(result9 < 0f))
				{
					break;
				}
				extremePoint = extremePoint3;
				extremePointA = extremePointA3;
				extremePointB = extremePointB3;
				Vector3.Cross(ref result5, ref result7, out direction);
			}
			while (true)
			{
				Vector3.Subtract(ref extremePoint3, ref extremePoint2, out var result10);
				Vector3.Subtract(ref extremePoint, ref extremePoint2, out var result11);
				Vector3.Cross(ref result10, ref result11, out direction);
				Vector3.Subtract(ref extremePoint, ref P_3, out var result12);
				Vector3.Dot(ref result12, ref direction, out var result13);
				_0006.GetLocalMinkowskiExtremePoint(P_0, P_1, ref direction, ref P_2, out var extremePointA4, out var extremePointB4, out var extremePoint4);
				Vector3.Subtract(ref extremePoint4, ref P_3, out var result14);
				Vector3.Dot(ref result14, ref direction, out var result15);
				if (result15 - result13 < a56 || num3 > InnerIterationLimit)
				{
					break;
				}
				num3++;
				Vector3.Cross(ref result14, ref result6, out result8);
				Vector3.Dot(ref result12, ref result8, out result13);
				if (result13 >= 0f)
				{
					Vector3.Subtract(ref extremePoint2, ref P_3, out var result16);
					Vector3.Dot(ref result16, ref result8, out result13);
					if (result13 >= 0f)
					{
						extremePoint = extremePoint4;
						extremePointA = extremePointA4;
						extremePointB = extremePointB4;
					}
					else
					{
						extremePoint3 = extremePoint4;
						extremePointA3 = extremePointA4;
						Vector3 extremePointB3 = extremePointB4;
					}
				}
				else
				{
					Vector3.Subtract(ref extremePoint3, ref P_3, out var result17);
					Vector3.Dot(ref result17, ref result8, out result13);
					if (result13 >= 0f)
					{
						extremePoint2 = extremePoint4;
						extremePointA2 = extremePointA4;
						extremePointB2 = extremePointB4;
					}
					else
					{
						extremePoint = extremePoint4;
						extremePointA = extremePointA4;
						extremePointB = extremePointB4;
					}
				}
			}
			r.X.GetBarycentricCoordinates(ref P_3, ref extremePoint, ref extremePoint2, ref extremePoint3, out var aWeight, out var bWeight, out var cWeight);
			Vector3.Multiply(ref extremePointA, aWeight, out P_4);
			Vector3.Multiply(ref extremePointA2, bWeight, out extremePointA2);
			Vector3.Multiply(ref extremePointA3, cWeight, out extremePointA3);
			Vector3.Add(ref extremePointA2, ref P_4, out P_4);
			Vector3.Add(ref extremePointA3, ref P_4, out P_4);
		}

		public static bool AreSweptShapesIntersecting(y.h shapeA, y.h shapeB, ref Vector3 sweep, ref N._0006 localTransformB, out Vector3 position)
		{
			if (localTransformB.Position.LengthSquared() < 1E-07f)
			{
				position = default(Vector3);
				return true;
			}
			Vector3.Negate(ref localTransformB.Position, out var result);
			Vector3 result2 = localTransformB.Position;
			_6_0001(shapeA, shapeB, ref localTransformB, ref sweep, ref result2, out var value, out var vector);
			Vector3.Cross(ref vector, ref result, out result2);
			if (result2.LengthSquared() < 1E-07f)
			{
				Vector3.Dot(ref vector, ref localTransformB.Position, out var result3);
				if (result3 < 0f)
				{
					position = default(Vector3);
					return false;
				}
				Vector3.Dot(ref result, ref localTransformB.Position, out var result4);
				float scaleFactor = (0f - result4) / (result3 - result4);
				Vector3.Multiply(ref value, scaleFactor, out position);
				return true;
			}
			_6_0001(shapeA, shapeB, ref localTransformB, ref sweep, ref result2, out var vector2, out var value2);
			Vector3.Subtract(ref vector, ref result, out var result5);
			Vector3.Subtract(ref value2, ref result, out var result6);
			Vector3.Cross(ref result5, ref result6, out result2);
			int num = 0;
			Vector3 vector3;
			Vector3 vector4;
			while (true)
			{
				_6_0001(shapeA, shapeB, ref localTransformB, ref sweep, ref result2, out vector3, out vector4);
				if (num > OuterIterationLimit)
				{
					break;
				}
				num++;
				Vector3.Cross(ref vector, ref vector4, out result5);
				Vector3.Dot(ref result5, ref result, out var result7);
				if (result7 < 0f)
				{
					value2 = vector4;
					vector2 = vector3;
					Vector3.Subtract(ref vector, ref result, out result5);
					Vector3.Subtract(ref vector4, ref result, out result6);
					Vector3.Cross(ref result5, ref result6, out result2);
					continue;
				}
				Vector3.Cross(ref vector4, ref value2, out result5);
				Vector3.Dot(ref result5, ref result, out result7);
				if (!(result7 < 0f))
				{
					break;
				}
				vector = vector4;
				value = vector3;
				Vector3.Subtract(ref value2, ref result, out result5);
				Vector3.Subtract(ref vector4, ref result, out result6);
				Vector3.Cross(ref result5, ref result6, out result2);
			}
			while (true)
			{
				Vector3.Subtract(ref vector4, ref value2, out result5);
				Vector3.Subtract(ref vector, ref value2, out result6);
				Vector3.Cross(ref result5, ref result6, out result2);
				Vector3.Dot(ref result2, ref vector, out var result8);
				if (result8 >= 0f)
				{
					Vector3.Subtract(ref vector, ref result, out result5);
					Vector3.Subtract(ref value2, ref result, out result6);
					Vector3.Subtract(ref vector4, ref result, out var result9);
					Vector3.Cross(ref result5, ref result6, out var result10);
					Vector3.Dot(ref result10, ref result9, out var result11);
					Vector3.Cross(ref vector, ref value2, out result10);
					Vector3.Dot(ref result10, ref vector4, out var result12);
					Vector3.Cross(ref localTransformB.Position, ref result6, out result10);
					Vector3.Dot(ref result10, ref result9, out var result13);
					Vector3.Cross(ref result5, ref localTransformB.Position, out result10);
					Vector3.Dot(ref result10, ref result9, out var result14);
					float num2 = 1f / result11;
					float num3 = result12 * num2;
					float num4 = result13 * num2;
					float num5 = result14 * num2;
					float num6 = 1f - num3 - num4 - num5;
					position = num4 * value + num5 * vector2 + num6 * vector3;
					return true;
				}
				_6_0001(shapeA, shapeB, ref localTransformB, ref sweep, ref result2, out var vector5, out var vector6);
				Vector3.Dot(ref vector6, ref result2, out var result15);
				if (result15 < 0f)
				{
					position = default(Vector3);
					return false;
				}
				if (result15 - result8 < a5h || num > InnerIterationLimit)
				{
					break;
				}
				num++;
				Vector3.Cross(ref vector6, ref result, out result5);
				Vector3.Dot(ref vector, ref result5, out result8);
				if (result8 >= 0f)
				{
					Vector3.Dot(ref value2, ref result5, out result8);
					if (result8 >= 0f)
					{
						vector = vector6;
						value = vector5;
					}
					else
					{
						vector4 = vector6;
						vector3 = vector5;
					}
				}
				else
				{
					Vector3.Dot(ref vector4, ref result5, out result8);
					if (result8 >= 0f)
					{
						value2 = vector6;
						vector2 = vector5;
					}
					else
					{
						vector = vector6;
						value = vector5;
					}
				}
			}
			position = default(Vector3);
			return false;
		}

		private static void _6_0001(y.h P_0, y.h P_1, ref N._0006 P_2, ref Vector3 P_3, ref Vector3 P_4, out Vector3 P_5)
		{
			_0006.GetLocalMinkowskiExtremePoint(P_0, P_1, ref P_4, ref P_2, out P_5);
			Vector3.Dot(ref P_4, ref P_3, out var result);
			if (result > 0f)
			{
				Vector3.Add(ref P_5, ref P_3, out P_5);
			}
		}

		private static void _6_0001(y.h P_0, y.h P_1, ref N._0006 P_2, ref Vector3 P_3, ref Vector3 P_4, out Vector3 P_5, out Vector3 P_6)
		{
			_0006.GetLocalMinkowskiExtremePoint(P_0, P_1, ref P_4, ref P_2, out P_5, out var _, out P_6);
			Vector3.Dot(ref P_4, ref P_3, out var result);
			if (result > 0f)
			{
				Vector3.Add(ref P_6, ref P_3, out P_6);
			}
		}
	}
}
namespace i
{
	internal abstract class v : _0006
	{
		protected P._7 mesh;

		internal l._7<int> a5h = new l._7<int>(4);

		public P._7 Mesh => mesh;

		protected override bool UseImprovedBoundaryHandling => mesh.a56;

		protected internal override int FindOverlappingTriangles(float dt)
		{
			mesh.Mesh.Tree.GetOverlaps(convex.boundingBox, a5h);
			return a5h.a5h;
		}

		protected override bool ConfigureTriangle(int i, out _00065b indices)
		{
			int num = a5h.Elements[i];
			mesh.Mesh.Data.GetTriangle(num, out localTriangleShape.a5h, out localTriangleShape.a5b, out localTriangleShape.a56);
			localTriangleShape.a5a = mesh.a5b;
			localTriangleShape.collisionMargin = 0f;
			indices = default(_00065b);
			indices.A = mesh.Mesh.Data.a5h[num];
			indices.B = mesh.Mesh.Data.a5h[num + 1];
			indices.C = mesh.Mesh.Data.a5h[num + 2];
			return true;
		}

		protected internal override void CleanUpOverlappingTriangles()
		{
			a5h.Clear();
		}

		public override void CleanUp()
		{
			mesh = null;
			convex = null;
			base.CleanUp();
		}

		public override void Initialize(P.h newCollidableA, P.h newCollidableB)
		{
			convex = newCollidableA as s.v;
			mesh = newCollidableB as P._7;
			if (convex == null || mesh == null)
			{
				convex = newCollidableB as s.v;
				mesh = newCollidableA as P._7;
				if (convex == null || mesh == null)
				{
					throw new Exception("Inappropriate types used to initialize contact manifold.");
				}
			}
		}
	}
}
