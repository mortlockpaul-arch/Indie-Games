using System;
using System.Collections.Generic;
using _0004;
using _0014;
using _0017;
using BEPUphysics.DataStructures;
using I;
using Microsoft.Xna.Framework;
using N;
using P;
using l;
using p;
using r;
using s;
using y;

namespace I
{
	internal static class _0006
	{
		public static void GetLocalTransform(ref N._0006 transformA, ref N._0006 transformB, out N._0006 localTransformB)
		{
			Quaternion.Conjugate(ref transformA.Orientation, out var result);
			Quaternion.Concatenate(ref transformB.Orientation, ref result, out localTransformB.Orientation);
			Vector3.Subtract(ref transformB.Position, ref transformA.Position, out localTransformB.Position);
			Vector3.Transform(ref localTransformB.Position, ref result, out localTransformB.Position);
		}

		public static void GetLocalMinkowskiExtremePoint(y.h shapeA, y.h shapeB, ref Vector3 direction, ref N._0006 localTransformB, out Vector3 extremePoint)
		{
			shapeA.GetLocalExtremePointWithoutMargin(ref direction, out extremePoint);
			Vector3.Negate(ref direction, out var result);
			shapeB.GetExtremePointWithoutMargin(result, ref localTransformB, out var extremePoint2);
			Vector3.Subtract(ref extremePoint, ref extremePoint2, out extremePoint);
			ExpandMinkowskiSum(shapeA.collisionMargin, shapeB.collisionMargin, ref direction, out extremePoint2);
			Vector3.Add(ref extremePoint, ref extremePoint2, out extremePoint);
		}

		public static void GetLocalMinkowskiExtremePoint(y.h shapeA, y.h shapeB, ref Vector3 direction, ref N._0006 localTransformB, out Vector3 extremePointA, out Vector3 extremePoint)
		{
			shapeA.GetLocalExtremePointWithoutMargin(ref direction, out extremePointA);
			Vector3.Negate(ref direction, out var result);
			shapeB.GetExtremePointWithoutMargin(result, ref localTransformB, out var extremePoint2);
			ExpandMinkowskiSum(shapeA.collisionMargin, shapeB.collisionMargin, direction, ref extremePointA, ref extremePoint2);
			Vector3.Subtract(ref extremePointA, ref extremePoint2, out extremePoint);
		}

		public static void GetLocalMinkowskiExtremePoint(y.h shapeA, y.h shapeB, ref Vector3 direction, ref N._0006 localTransformB, out Vector3 extremePointA, out Vector3 extremePointB, out Vector3 extremePoint)
		{
			shapeA.GetLocalExtremePointWithoutMargin(ref direction, out extremePointA);
			Vector3.Negate(ref direction, out var result);
			shapeB.GetExtremePointWithoutMargin(result, ref localTransformB, out extremePointB);
			ExpandMinkowskiSum(shapeA.collisionMargin, shapeB.collisionMargin, direction, ref extremePointA, ref extremePointB);
			Vector3.Subtract(ref extremePointA, ref extremePointB, out extremePoint);
		}

		public static void GetLocalMinkowskiExtremePointWithoutMargin(y.h shapeA, y.h shapeB, ref Vector3 direction, ref N._0006 localTransformB, out Vector3 extremePoint)
		{
			shapeA.GetLocalExtremePointWithoutMargin(ref direction, out extremePoint);
			Vector3.Negate(ref direction, out var result);
			shapeB.GetExtremePointWithoutMargin(result, ref localTransformB, out var extremePoint2);
			Vector3.Subtract(ref extremePoint, ref extremePoint2, out extremePoint);
		}

		public static void ExpandMinkowskiSum(float marginA, float marginB, ref Vector3 direction, out Vector3 contribution)
		{
			float num = direction.LengthSquared();
			if (num > 1E-07f)
			{
				Vector3.Multiply(ref direction, (marginA + marginB) / (float)Math.Sqrt(num), out contribution);
			}
			else
			{
				contribution = default(Vector3);
			}
		}

		public static void ExpandMinkowskiSum(float marginA, float marginB, Vector3 direction, ref Vector3 toExpandA, ref Vector3 toExpandB)
		{
			float num = direction.LengthSquared();
			if (num > 1E-07f)
			{
				num = 1f / (float)Math.Sqrt(num);
				Vector3.Multiply(ref direction, marginA * num, out var result);
				Vector3.Add(ref toExpandA, ref result, out toExpandA);
				Vector3.Multiply(ref direction, marginB * num, out result);
				Vector3.Subtract(ref toExpandB, ref result, out toExpandB);
			}
		}
	}
	internal class _0018 : X
	{
		internal enum _00065h
		{
			Plane,
			ExternalSeparated,
			ExternalNear,
			Deep
		}

		private new const int a5h = 10;

		internal y.h a5b;

		internal _00065h a56;

		private int a5a;

		private Vector3 a57;

		public override bool ShouldCorrectContactNormal => a56 == _00065h.Deep;

		public override bool GenerateContactCandidate(out l.W<_0004.b> contactList)
		{
			switch (a56)
			{
			case _00065h.Plane:
				return _6_000F(out contactList);
			case _00065h.ExternalSeparated:
				return _6Z(out contactList);
			case _00065h.ExternalNear:
				return _6u(out contactList);
			case _00065h.Deep:
				return _6v(out contactList);
			default:
				contactList = default(l.W<_0004.b>);
				return false;
			}
		}

		private bool _6_000F(out l.W<_0004.b> P_0)
		{
			Vector3.Subtract(ref base.a5h.a5b, ref base.a5h.a5h, out var result);
			Vector3.Subtract(ref base.a5h.a56, ref base.a5h.a5h, out var result2);
			Vector3.Cross(ref result2, ref result, out var result3);
			Vector3.Dot(ref base.a5h.a5h, ref result3, out var result4);
			P_0 = default(l.W<_0004.b>);
			switch (base.a5h.a5a)
			{
			case y._0006.DoubleSided:
				if (result4 < 0f)
				{
					Vector3.Negate(ref result3, out result3);
					result4 = 0f - result4;
				}
				break;
			case y._0006.Counterclockwise:
				Vector3.Negate(ref result3, out result3);
				result4 = 0f - result4;
				break;
			}
			a5b.GetLocalExtremePointWithoutMargin(ref result3, out var extremePoint);
			if (_6Y(ref extremePoint) != _0004._0006.ABC)
			{
				a56 = _00065h.ExternalSeparated;
				return _6Z(out P_0);
			}
			Vector3.Dot(ref extremePoint, ref result3, out var result5);
			float num = (result4 - result5) / result3.LengthSquared();
			Vector3.Multiply(ref result3, num, out var result6);
			float num2 = result6.LengthSquared();
			float num3 = base.a5h.collisionMargin + a5b.collisionMargin;
			if (num <= 0f || num2 < num3 * num3)
			{
				_0004.b item = default(_0004.b);
				if (num3 > 1E-07f)
				{
					Vector3.Multiply(ref result6, a5b.collisionMargin / num3, out item.Position);
				}
				else
				{
					item.Position = default(Vector3);
				}
				Vector3.Add(ref extremePoint, ref item.Position, out item.Position);
				float num4 = result3.Length();
				Vector3.Divide(ref result3, num4, out item.Normal);
				float num5 = num4 * num;
				item.PenetrationDepth = num3 - num5;
				if (item.PenetrationDepth > num3)
				{
					if (_6z(out var item2))
					{
						P_0.Add(ref item2);
					}
					_00065h obj = a56;
					a56 = _00065h.ExternalNear;
					if (!_6u(out var w))
					{
						a56 = obj;
						return false;
					}
					w.Get(0, out item2);
					if (item2.PenetrationDepth + 0.01f < item.PenetrationDepth)
					{
						P_0.Add(ref item2);
					}
					else
					{
						P_0.Add(ref item);
						a56 = obj;
					}
				}
				else
				{
					P_0.Add(ref item);
				}
				return true;
			}
			return false;
		}

		private bool _6Z(out l.W<_0004.b> P_0)
		{
			if (_0017.h.AreShapesIntersecting(a5b, base.a5h, ref r.X.RigidIdentity, ref r.X.RigidIdentity, ref a57))
			{
				a56 = _00065h.ExternalNear;
				return _6u(out P_0);
			}
			_6L();
			P_0 = default(l.W<_0004.b>);
			return false;
		}

		private bool _6u(out l.W<_0004.b> P_0)
		{
			Vector3.Add(ref base.a5h.a5h, ref base.a5h.a5b, out var result);
			Vector3.Add(ref result, ref base.a5h.a56, out result);
			Vector3.Multiply(ref result, 1f / 3f, out result);
			_0017._6 cachedSimplex = new _0017._6
			{
				State = _0017.b.Point,
				LocalSimplexB = 
				{
					A = result
				}
			};
			if (_0017.h.GetClosestPoints(a5b, base.a5h, ref r.X.RigidIdentity, ref r.X.RigidIdentity, ref cachedSimplex, out var closestPointA, out var closestPointB))
			{
				a56 = _00065h.Deep;
				return _6v(out P_0);
			}
			Vector3.Subtract(ref closestPointB, ref closestPointA, out var result2);
			float num = result2.LengthSquared();
			float num2 = a5b.collisionMargin + base.a5h.collisionMargin;
			P_0 = default(l.W<_0004.b>);
			if (num < num2 * num2)
			{
				_0004.b item = default(_0004.b);
				if (base.a5h.a5a != y._0006.DoubleSided)
				{
					Vector3.Subtract(ref base.a5h.a5b, ref base.a5h.a5h, out var result3);
					Vector3.Subtract(ref base.a5h.a56, ref base.a5h.a5h, out var result4);
					Vector3.Cross(ref result3, ref result4, out var result5);
					Vector3.Dot(ref result5, ref result2, out var result6);
					if (base.a5h.a5a == y._0006.Clockwise && result6 > 0f)
					{
						return false;
					}
					if (base.a5h.a5a == y._0006.Counterclockwise && result6 < 0f)
					{
						return false;
					}
				}
				if (num2 > 1E-07f)
				{
					Vector3.Multiply(ref result2, a5b.collisionMargin / num2, out item.Position);
				}
				else
				{
					item.Position = default(Vector3);
				}
				Vector3.Add(ref closestPointA, ref item.Position, out item.Position);
				item.Normal = result2;
				float num3 = (float)Math.Sqrt(num);
				Vector3.Divide(ref item.Normal, num3, out item.Normal);
				item.PenetrationDepth = num2 - num3;
				P_0.Add(ref item);
				_6L(ref item.Position);
				return true;
			}
			a56 = _00065h.ExternalSeparated;
			return false;
		}

		private bool _6v(out l.W<_0004.b> P_0)
		{
			Vector3.Add(ref base.a5h.a5h, ref base.a5h.a5b, out var result);
			Vector3.Add(ref result, ref base.a5h.a56, out result);
			Vector3.Multiply(ref result, 1f / 3f, out result);
			P_0 = default(l.W<_0004.b>);
			_0004.b item = default(_0004.b);
			if (v._6_0002(a5b, base.a5h, ref result, ref r.X.RigidIdentity))
			{
				Vector3.Subtract(ref base.a5h.a5b, ref base.a5h.a5h, out var result2);
				Vector3.Subtract(ref base.a5h.a56, ref base.a5h.a5h, out var result3);
				Vector3.Cross(ref result2, ref result3, out var result4);
				float num = result4.LengthSquared();
				float result11;
				if (num < 1E-09f)
				{
					v.LocalSurfaceCast(a5b, base.a5h, ref r.X.RigidIdentity, ref result, out item.PenetrationDepth, out item.Normal, out item.Position);
				}
				else
				{
					Vector3.Divide(ref result4, (float)Math.Sqrt(num), out result4);
					Vector3.Subtract(ref result, ref base.a5h.a5h, out var result5);
					Vector3.Subtract(ref result, ref base.a5h.a5b, out var result6);
					Vector3.Subtract(ref result, ref base.a5h.a56, out var result7);
					Vector3.Subtract(ref base.a5h.a5b, ref base.a5h.a5h, out var result8);
					Vector3.Subtract(ref base.a5h.a56, ref base.a5h.a5b, out var result9);
					Vector3.Subtract(ref base.a5h.a5h, ref base.a5h.a56, out var result10);
					Vector3.Dot(ref result5, ref result8, out result11);
					Vector3.Multiply(ref result8, result11 / result8.LengthSquared(), out var result12);
					Vector3.Subtract(ref result5, ref result12, out result12);
					result12.Normalize();
					Vector3.Dot(ref result6, ref result9, out result11);
					Vector3.Multiply(ref result9, result11 / result9.LengthSquared(), out var result13);
					Vector3.Subtract(ref result6, ref result13, out result13);
					result13.Normalize();
					Vector3.Dot(ref result7, ref result10, out result11);
					Vector3.Multiply(ref result10, result11 / result10.LengthSquared(), out var result14);
					Vector3.Subtract(ref result7, ref result14, out result14);
					result14.Normalize();
					v.LocalSurfaceCast(a5b, base.a5h, ref r.X.RigidIdentity, ref result12, out item.PenetrationDepth, out item.Normal);
					Vector3.Dot(ref result4, ref item.Normal, out result11);
					if ((base.a5h.a5a == y._0006.Clockwise && result11 > 0f) || (base.a5h.a5a == y._0006.Counterclockwise && result11 < 0f))
					{
						Vector3 vector = item.Normal;
						Vector3.Dot(ref item.Normal, ref result4, out result11);
						Vector3.Multiply(ref item.Normal, result11, out var result15);
						Vector3.Subtract(ref item.Normal, ref result15, out item.Normal);
						float num2 = item.Normal.LengthSquared();
						if (num2 > 1E-07f)
						{
							Vector3.Divide(ref item.Normal, (float)Math.Sqrt(num2), out item.Normal);
							Vector3.Dot(ref item.Normal, ref vector, out result11);
							item.PenetrationDepth *= result11;
						}
						else
						{
							item.PenetrationDepth = float.MaxValue;
							item.Normal = default(Vector3);
						}
					}
					v.LocalSurfaceCast(a5b, base.a5h, ref r.X.RigidIdentity, ref result13, out var num3, out var normal);
					Vector3.Dot(ref result4, ref normal, out result11);
					if ((base.a5h.a5a == y._0006.Clockwise && result11 > 0f) || (base.a5h.a5a == y._0006.Counterclockwise && result11 < 0f))
					{
						Vector3 vector2 = normal;
						Vector3.Dot(ref normal, ref result4, out result11);
						Vector3.Multiply(ref normal, result11, out var result16);
						Vector3.Subtract(ref normal, ref result16, out normal);
						float num4 = normal.LengthSquared();
						if (num4 > 1E-07f)
						{
							Vector3.Divide(ref normal, (float)Math.Sqrt(num4), out normal);
							Vector3.Dot(ref normal, ref vector2, out result11);
							num3 *= result11;
						}
						else
						{
							item.PenetrationDepth = float.MaxValue;
							item.Normal = default(Vector3);
						}
					}
					if (num3 < item.PenetrationDepth)
					{
						item.Normal = normal;
						item.PenetrationDepth = num3;
					}
					v.LocalSurfaceCast(a5b, base.a5h, ref r.X.RigidIdentity, ref result14, out num3, out normal);
					Vector3.Dot(ref result4, ref normal, out result11);
					if ((base.a5h.a5a == y._0006.Clockwise && result11 > 0f) || (base.a5h.a5a == y._0006.Counterclockwise && result11 < 0f))
					{
						Vector3 vector3 = normal;
						Vector3.Dot(ref normal, ref result4, out result11);
						Vector3.Multiply(ref normal, result11, out var result17);
						Vector3.Subtract(ref normal, ref result17, out normal);
						float num5 = normal.LengthSquared();
						if (num5 > 1E-07f)
						{
							Vector3.Divide(ref normal, (float)Math.Sqrt(num5), out normal);
							Vector3.Dot(ref normal, ref vector3, out result11);
							num3 *= result11;
						}
						else
						{
							item.PenetrationDepth = float.MaxValue;
							item.Normal = default(Vector3);
						}
					}
					if (num3 < item.PenetrationDepth)
					{
						item.Normal = normal;
						item.PenetrationDepth = num3;
					}
					if (base.a5h.a5a != y._0006.Clockwise)
					{
						v.LocalSurfaceCast(a5b, base.a5h, ref r.X.RigidIdentity, ref result4, out num3, out normal);
						if (num3 < item.PenetrationDepth)
						{
							item.Normal = normal;
							item.PenetrationDepth = num3;
						}
					}
					if (base.a5h.a5a != y._0006.Counterclockwise)
					{
						Vector3.Negate(ref result4, out result4);
						v.LocalSurfaceCast(a5b, base.a5h, ref r.X.RigidIdentity, ref result4, out num3, out normal);
						if (num3 < item.PenetrationDepth)
						{
							item.Normal = normal;
							item.PenetrationDepth = num3;
						}
					}
				}
				v.RefinePenetration(a5b, base.a5h, ref r.X.RigidIdentity, item.PenetrationDepth, ref item.Normal, out item.PenetrationDepth, out item.Normal, out item.Position);
				if (base.a5h.a5a != y._0006.DoubleSided)
				{
					Vector3.Dot(ref result4, ref item.Normal, out result11);
					if (result11 < 0f)
					{
						return false;
					}
				}
				item.Id = -1;
				if (item.PenetrationDepth < a5b.collisionMargin + base.a5h.collisionMargin)
				{
					a56 = _00065h.ExternalNear;
				}
				P_0.Add(ref item);
			}
			if (_6z(out item))
			{
				P_0.Add(ref item);
			}
			if (P_0.a5X > 0)
			{
				return true;
			}
			a56 = _00065h.ExternalSeparated;
			return false;
		}

		private void _6L()
		{
			if (++a5a == 10)
			{
				a5a = 0;
				a56 = _00065h.Plane;
			}
		}

		private void _6L(ref Vector3 P_0)
		{
			if (++a5a == 10 && _6Y(ref P_0) == _0004._0006.ABC)
			{
				a5a = 0;
				a56 = _00065h.Plane;
			}
		}

		private bool _6z(out _0004.b P_0)
		{
			r.X.GetClosestPointOnTriangleToPoint(ref base.a5h.a5h, ref base.a5h.a5b, ref base.a5h.a56, ref r.X.ZeroVector, out var closestPoint);
			float num = closestPoint.LengthSquared();
			float num2 = a5b.minimumRadius * (_0014._6.CoreShapeScaling + 0.01f);
			if (num < num2 * num2)
			{
				Vector3.Subtract(ref base.a5h.a5b, ref base.a5h.a5h, out var result);
				Vector3.Subtract(ref base.a5h.a56, ref base.a5h.a5h, out var result2);
				Vector3.Cross(ref result, ref result2, out var result3);
				Vector3.Dot(ref closestPoint, ref result3, out var result4);
				if ((base.a5h.a5a == y._0006.Clockwise && result4 > 0f) || (base.a5h.a5a == y._0006.Counterclockwise && result4 < 0f))
				{
					P_0 = default(_0004.b);
					return false;
				}
				num = (float)Math.Sqrt(num);
				P_0.Position = closestPoint;
				if (num > 1E-07f)
				{
					Vector3.Divide(ref closestPoint, num, out P_0.Normal);
				}
				else
				{
					float num3 = result3.LengthSquared();
					if (!(result3.LengthSquared() > 1E-07f))
					{
						P_0 = default(_0004.b);
						return false;
					}
					Vector3.Divide(ref result3, (float)Math.Sqrt(num3), out result3);
					if (base.a5h.a5a == y._0006.Clockwise)
					{
						P_0.Normal = result3;
					}
					else
					{
						Vector3.Negate(ref result3, out P_0.Normal);
					}
				}
				P_0.PenetrationDepth = a5b.minimumRadius - num;
				P_0.Id = -1;
				return true;
			}
			P_0 = default(_0004.b);
			return false;
		}

		private _0004._0006 _6Y(ref Vector3 P_0)
		{
			Vector3.Subtract(ref base.a5h.a5b, ref base.a5h.a5h, out var result);
			Vector3.Subtract(ref base.a5h.a56, ref base.a5h.a5h, out var result2);
			Vector3.Subtract(ref P_0, ref base.a5h.a5h, out var result3);
			Vector3.Dot(ref result3, ref result, out var result4);
			Vector3.Dot(ref result3, ref result2, out var result5);
			if (result5 <= 0f && result4 <= 0f)
			{
				return _0004._0006.A;
			}
			Vector3.Subtract(ref P_0, ref base.a5h.a5b, out var result6);
			Vector3.Dot(ref result, ref result6, out var result7);
			Vector3.Dot(ref result2, ref result6, out var result8);
			if (result7 >= 0f && result8 <= result7)
			{
				return _0004._0006.B;
			}
			float num = result4 * result8 - result7 * result5;
			if (num <= 0f && result4 > 0f && result7 < 0f)
			{
				return _0004._0006.AB;
			}
			Vector3.Subtract(ref P_0, ref base.a5h.a56, out var result9);
			Vector3.Dot(ref result, ref result9, out var result10);
			Vector3.Dot(ref result2, ref result9, out var result11);
			if (result11 >= 0f && result10 <= result11)
			{
				return _0004._0006.C;
			}
			float num2 = result10 * result5 - result4 * result11;
			if (num2 <= 0f && result5 > 0f && result11 < 0f)
			{
				return _0004._0006.AC;
			}
			float num3 = result7 * result11 - result10 * result8;
			if (num3 <= 0f && result8 - result7 > 0f && result10 - result11 > 0f)
			{
				return _0004._0006.BC;
			}
			return _0004._0006.ABC;
		}

		public override void Initialize(y.h convex, y.v triangle)
		{
			a5b = convex;
			base.a5h = triangle;
		}

		public override void CleanUp()
		{
			base.a5h = null;
			a5b = null;
			a56 = _00065h.Plane;
			a5a = 0;
			a57 = default(Vector3);
			Updated = false;
		}

		public override _0004._0006 GetRegion(ref _0004.b contact)
		{
			Vector3.Dot(ref base.a5h.a5h, ref contact.Normal, out var result);
			Vector3.Dot(ref base.a5h.a5b, ref contact.Normal, out var result2);
			Vector3.Dot(ref base.a5h.a56, ref contact.Normal, out var result3);
			result = 0f - result;
			result2 = 0f - result2;
			result3 = 0f - result3;
			float num = 0.01f;
			Vector3 result4;
			float result5;
			if (result > result2 && result > result3)
			{
				if (result2 > result3)
				{
					if (Math.Abs(result - result3) < num)
					{
						return _0004._0006.ABC;
					}
					Vector3.Subtract(ref base.a5h.a5b, ref base.a5h.a5h, out result4);
					Vector3.Dot(ref result4, ref contact.Normal, out result5);
					if (result5 * result5 < result4.LengthSquared() * 0.01f)
					{
						return _0004._0006.AB;
					}
					return _0004._0006.A;
				}
				if (Math.Abs(result - result2) < num)
				{
					return _0004._0006.ABC;
				}
				Vector3.Subtract(ref base.a5h.a56, ref base.a5h.a5h, out result4);
				Vector3.Dot(ref result4, ref contact.Normal, out result5);
				if (result5 * result5 < result4.LengthSquared() * 0.01f)
				{
					return _0004._0006.AC;
				}
				return _0004._0006.A;
			}
			if (result2 > result3)
			{
				if (result3 > result)
				{
					if (Math.Abs(result2 - result) < num)
					{
						return _0004._0006.ABC;
					}
					Vector3.Subtract(ref base.a5h.a56, ref base.a5h.a5b, out result4);
					Vector3.Dot(ref result4, ref contact.Normal, out result5);
					if (result5 * result5 < result4.LengthSquared() * 0.01f)
					{
						return _0004._0006.BC;
					}
					return _0004._0006.B;
				}
				if (Math.Abs(result2 - result3) < num)
				{
					return _0004._0006.ABC;
				}
				Vector3.Subtract(ref base.a5h.a5h, ref base.a5h.a5b, out result4);
				Vector3.Dot(ref result4, ref contact.Normal, out result5);
				if (result5 * result5 < result4.LengthSquared() * 0.01f)
				{
					return _0004._0006.AB;
				}
				return _0004._0006.B;
			}
			if (result > result2)
			{
				if (Math.Abs(result3 - result2) < num)
				{
					return _0004._0006.ABC;
				}
				Vector3.Subtract(ref base.a5h.a5h, ref base.a5h.a56, out result4);
				Vector3.Dot(ref result4, ref contact.Normal, out result5);
				if (result5 * result5 < result4.LengthSquared() * 0.01f)
				{
					return _0004._0006.AC;
				}
				return _0004._0006.C;
			}
			if (Math.Abs(result3 - result) < num)
			{
				return _0004._0006.ABC;
			}
			Vector3.Subtract(ref base.a5h.a5b, ref base.a5h.a56, out result4);
			Vector3.Dot(ref result4, ref contact.Normal, out result5);
			if (result5 * result5 < result4.LengthSquared() * 0.01f)
			{
				return _0004._0006.BC;
			}
			return _0004._0006.C;
		}
	}
}
namespace i
{
	internal abstract class _0006 : h
	{
		private struct _00065h(int a, int b) : IEquatable<_00065h>
		{
			private int a5h = a;

			private int a5b = b;

			public override int GetHashCode()
			{
				return a5h + a5b;
			}

			public bool Equals(_00065h edge)
			{
				if (edge.a5h != a5h || edge.a5b != a5b)
				{
					if (edge.a5h == a5b)
					{
						return edge.a5b == a5h;
					}
					return false;
				}
				return true;
			}
		}

		internal struct _00065b : IEquatable<_00065b>
		{
			public int A;

			public int B;

			public int C;

			public override int GetHashCode()
			{
				return A + B + C;
			}

			public bool Equals(_00065b other)
			{
				if (A == other.A && B == other.B)
				{
					return C == other.C;
				}
				return false;
			}
		}

		private struct _000656
		{
			public bool ShouldCorrect;

			public Vector3 CorrectedNormal;

			public _00065h Edge;

			public _0004.b ContactData;
		}

		private struct _00065a
		{
			public bool ShouldCorrect;

			public Vector3 CorrectedNormal;

			public int Vertex;

			public _0004.b ContactData;
		}

		protected l._0006<_0004._7> supplementData = new l._0006<_0004._7>(4);

		private Dictionary<_00065b, I.X> a5h = new Dictionary<_00065b, I.X>(4);

		private l._0006<_0004.b> a5b;

		private l._0006<_0004.b> a56 = new l._0006<_0004.b>();

		protected y.v localTriangleShape = new y.v();

		private BEPUphysics.DataStructures.HashSet<int> a5a = new BEPUphysics.DataStructures.HashSet<int>();

		private BEPUphysics.DataStructures.HashSet<_00065h> a57 = new BEPUphysics.DataStructures.HashSet<_00065h>();

		private l._0006<_000656> a5_0006 = new l._0006<_000656>();

		private l._0006<_00065a> a5v = new l._0006<_00065a>();

		protected s.v convex;

		public s.v ConvexCollidable => convex;

		protected virtual N._0006 MeshTransform => N._0006.Identity;

		protected abstract bool UseImprovedBoundaryHandling { get; }

		protected abstract I.X GetTester();

		protected abstract void GiveBackTester(I.X tester);

		protected _0006()
		{
			contacts = new l._7<_0004.h>(4);
			unusedContacts = new p.a<_0004.h>(4);
			contactIndicesToRemove = new l._7<int>(4);
			a5b = new l._0006<_0004.b>(1);
		}

		protected internal abstract int FindOverlappingTriangles(float dt);

		protected abstract bool ConfigureTriangle(int i, out _00065b indices);

		protected internal abstract void CleanUpOverlappingTriangles();

		public override void Update(float dt)
		{
			N._0006 transformB = MeshTransform;
			_0004.a.ContactRefresh(contacts, supplementData, ref convex.worldTransform, ref transformB, contactIndicesToRemove);
			RemoveQueuedContacts();
			CleanUpOverlappingTriangles();
			int num = FindOverlappingTriangles(dt);
			N._7.CreateFromQuaternion(ref convex.worldTransform.Orientation, out var result);
			for (int i = 0; i < num; i++)
			{
				if (!ConfigureTriangle(i, out var indices))
				{
					continue;
				}
				if (!a5h.TryGetValue(indices, out var value))
				{
					value = GetTester();
					value.Initialize(convex.Shape, localTriangleShape);
					a5h.Add(indices, value);
				}
				value.Updated = true;
				Vector3.Subtract(ref localTriangleShape.a5h, ref convex.worldTransform.Position, out localTriangleShape.a5h);
				Vector3.Subtract(ref localTriangleShape.a5b, ref convex.worldTransform.Position, out localTriangleShape.a5b);
				Vector3.Subtract(ref localTriangleShape.a56, ref convex.worldTransform.Position, out localTriangleShape.a56);
				N._7.TransformTranspose(ref localTriangleShape.a5h, ref result, out localTriangleShape.a5h);
				N._7.TransformTranspose(ref localTriangleShape.a5b, ref result, out localTriangleShape.a5b);
				N._7.TransformTranspose(ref localTriangleShape.a56, ref result, out localTriangleShape.a56);
				if (!value.GenerateContactCandidate(out var contactList))
				{
					continue;
				}
				for (int j = 0; j < contactList.a5X; j++)
				{
					contactList.Get(j, out var item);
					if (UseImprovedBoundaryHandling)
					{
						if (_6q(ref indices, value, ref item))
						{
							_6T(ref item, ref result);
						}
					}
					else
					{
						_6T(ref item, ref result);
					}
				}
			}
			if (UseImprovedBoundaryHandling)
			{
				for (int k = 0; k < a5_0006.a5h; k++)
				{
					if (!a57.Contains(a5_0006.Elements[k].Edge))
					{
						_6T(ref a5_0006.Elements[k].ContactData, ref result);
					}
					else if (a5_0006.Elements[k].ShouldCorrect)
					{
						a5_0006.Elements[k].CorrectedNormal.Normalize();
						Vector3.Dot(ref a5_0006.Elements[k].CorrectedNormal, ref a5_0006.Elements[k].ContactData.Normal, out var result2);
						a5_0006.Elements[k].ContactData.Normal = a5_0006.Elements[k].CorrectedNormal;
						a5_0006.Elements[k].ContactData.PenetrationDepth *= MathHelper.Max(0f, result2);
						_6T(ref a5_0006.Elements[k].ContactData, ref result);
					}
				}
				for (int m = 0; m < a5v.a5h; m++)
				{
					if (!a5a.Contains(a5v.Elements[m].Vertex))
					{
						_6T(ref a5v.Elements[m].ContactData, ref result);
					}
					else if (a5v.Elements[m].ShouldCorrect)
					{
						a5v.Elements[m].CorrectedNormal.Normalize();
						Vector3.Dot(ref a5v.Elements[m].CorrectedNormal, ref a5v.Elements[m].ContactData.Normal, out var result3);
						a5v.Elements[m].ContactData.Normal = a5v.Elements[m].CorrectedNormal;
						a5v.Elements[m].ContactData.PenetrationDepth *= MathHelper.Max(0f, result3);
						_6T(ref a5v.Elements[m].ContactData, ref result);
					}
				}
				a57.Clear();
				a5a.Clear();
				a5v.Clear();
				a5_0006.Clear();
			}
			l._0018<_00065b> obj = default(l._0018<_00065b>);
			foreach (KeyValuePair<_00065b, I.X> item2 in a5h)
			{
				if (!item2.Value.Updated)
				{
					if (!obj.Add(item2.Key))
					{
						break;
					}
				}
				else
				{
					item2.Value.Updated = false;
				}
			}
			for (int num2 = obj.a5X - 1; num2 >= 0; num2--)
			{
				I.X x2 = a5h[obj[num2]];
				x2.CleanUp();
				GiveBackTester(x2);
				a5h.Remove(obj[num2]);
			}
			ProcessCandidates(a5b);
			if (contacts.a5h + a5b.a5h > 4)
			{
				_0004._6.ReduceContacts(contacts, a5b, contactIndicesToRemove, a56);
				RemoveQueuedContacts();
				for (int num3 = a56.a5h - 1; num3 >= 0; num3--)
				{
					Add(ref a56.Elements[num3]);
					a56.RemoveAt(num3);
				}
			}
			else if (a5b.a5h > 0)
			{
				for (int n = 0; n < a5b.a5h; n++)
				{
					Add(ref a5b.Elements[n]);
				}
			}
			a5b.Clear();
		}

		private void _6T(ref _0004.b P_0, ref N._7 P_1)
		{
			N._7.Transform(ref P_0.Position, ref P_1, out P_0.Position);
			Vector3.Add(ref P_0.Position, ref convex.worldTransform.Position, out P_0.Position);
			N._7.Transform(ref P_0.Normal, ref P_1, out P_0.Normal);
			if (_6m(ref P_0))
			{
				a5b.Add(ref P_0);
			}
		}

		protected void GetNormal(ref Vector3 uncorrectedNormal, out Vector3 normal)
		{
			Vector3.Subtract(ref localTriangleShape.a5b, ref localTriangleShape.a5h, out var result);
			Vector3.Subtract(ref localTriangleShape.a56, ref localTriangleShape.a5h, out var result2);
			switch (localTriangleShape.a5a)
			{
			case y._0006.DoubleSided:
			{
				Vector3.Cross(ref result, ref result2, out normal);
				Vector3.Dot(ref normal, ref uncorrectedNormal, out var result3);
				if (result3 < 0f)
				{
					Vector3.Negate(ref normal, out normal);
				}
				break;
			}
			case y._0006.Clockwise:
				Vector3.Cross(ref result2, ref result, out normal);
				break;
			default:
				Vector3.Cross(ref result, ref result2, out normal);
				break;
			}
		}

		private bool _6q(ref _00065b P_0, I.X P_1, ref _0004.b P_2)
		{
			_00065a item2 = default(_00065a);
			_000656 item = default(_000656);
			switch (P_1.GetRegion(ref P_2))
			{
			case _0004._0006.A:
				item2.ContactData = P_2;
				item2.Vertex = P_0.A;
				item2.ShouldCorrect = P_1.ShouldCorrectContactNormal;
				if (item2.ShouldCorrect)
				{
					GetNormal(ref P_2.Normal, out item2.CorrectedNormal);
				}
				else
				{
					item2.CorrectedNormal = default(Vector3);
				}
				a5v.Add(ref item2);
				a57.Add(new _00065h(P_0.A, P_0.B));
				a57.Add(new _00065h(P_0.B, P_0.C));
				a57.Add(new _00065h(P_0.A, P_0.C));
				a5a.Add(P_0.B);
				a5a.Add(P_0.C);
				break;
			case _0004._0006.B:
				item2.ContactData = P_2;
				item2.Vertex = P_0.B;
				item2.ShouldCorrect = P_1.ShouldCorrectContactNormal;
				if (item2.ShouldCorrect)
				{
					GetNormal(ref P_2.Normal, out item2.CorrectedNormal);
				}
				else
				{
					item2.CorrectedNormal = default(Vector3);
				}
				a5v.Add(ref item2);
				a57.Add(new _00065h(P_0.A, P_0.B));
				a57.Add(new _00065h(P_0.B, P_0.C));
				a57.Add(new _00065h(P_0.A, P_0.C));
				a5a.Add(P_0.A);
				a5a.Add(P_0.C);
				break;
			case _0004._0006.C:
				item2.ContactData = P_2;
				item2.Vertex = P_0.C;
				item2.ShouldCorrect = P_1.ShouldCorrectContactNormal;
				if (item2.ShouldCorrect)
				{
					GetNormal(ref P_2.Normal, out item2.CorrectedNormal);
				}
				else
				{
					item2.CorrectedNormal = default(Vector3);
				}
				a5v.Add(ref item2);
				a57.Add(new _00065h(P_0.A, P_0.B));
				a57.Add(new _00065h(P_0.B, P_0.C));
				a57.Add(new _00065h(P_0.A, P_0.C));
				a5a.Add(P_0.A);
				a5a.Add(P_0.B);
				break;
			case _0004._0006.AB:
				item.Edge = new _00065h(P_0.A, P_0.B);
				item.ContactData = P_2;
				item.ShouldCorrect = P_1.ShouldCorrectContactNormal;
				if (item.ShouldCorrect)
				{
					GetNormal(ref P_2.Normal, out item.CorrectedNormal);
				}
				else
				{
					item.CorrectedNormal = default(Vector3);
				}
				a5_0006.Add(ref item);
				a57.Add(new _00065h(P_0.B, P_0.C));
				a57.Add(new _00065h(P_0.A, P_0.C));
				a5a.Add(P_0.A);
				a5a.Add(P_0.B);
				a5a.Add(P_0.C);
				break;
			case _0004._0006.AC:
				item.Edge = new _00065h(P_0.A, P_0.C);
				item.ContactData = P_2;
				item.ShouldCorrect = P_1.ShouldCorrectContactNormal;
				if (item.ShouldCorrect)
				{
					GetNormal(ref P_2.Normal, out item.CorrectedNormal);
				}
				else
				{
					item.CorrectedNormal = default(Vector3);
				}
				a5_0006.Add(ref item);
				a57.Add(new _00065h(P_0.A, P_0.B));
				a57.Add(new _00065h(P_0.B, P_0.C));
				a5a.Add(P_0.A);
				a5a.Add(P_0.B);
				a5a.Add(P_0.C);
				break;
			case _0004._0006.BC:
				item.Edge = new _00065h(P_0.B, P_0.C);
				item.ContactData = P_2;
				item.ShouldCorrect = P_1.ShouldCorrectContactNormal;
				if (item.ShouldCorrect)
				{
					GetNormal(ref P_2.Normal, out item.CorrectedNormal);
				}
				else
				{
					item.CorrectedNormal = default(Vector3);
				}
				a5_0006.Add(ref item);
				a57.Add(new _00065h(P_0.A, P_0.B));
				a57.Add(new _00065h(P_0.A, P_0.C));
				a5a.Add(P_0.A);
				a5a.Add(P_0.B);
				a5a.Add(P_0.C);
				break;
			default:
				a57.Add(new _00065h(P_0.A, P_0.B));
				a57.Add(new _00065h(P_0.B, P_0.C));
				a57.Add(new _00065h(P_0.A, P_0.C));
				a5a.Add(P_0.A);
				a5a.Add(P_0.B);
				a5a.Add(P_0.C);
				return true;
			}
			return false;
		}

		protected override void Add(ref _0004.b contactCandidate)
		{
			_0004._7 item = default(_0004._7);
			item.BasePenetrationDepth = contactCandidate.PenetrationDepth;
			N._0006.TransformByInverse(ref contactCandidate.Position, ref convex.worldTransform, out item.LocalOffsetA);
			N._0006 transform = MeshTransform;
			N._0006.TransformByInverse(ref contactCandidate.Position, ref transform, out item.LocalOffsetB);
			supplementData.Add(ref item);
			base.Add(ref contactCandidate);
		}

		protected override void Remove(int contactIndex)
		{
			supplementData.RemoveAt(contactIndex);
			base.Remove(contactIndex);
		}

		private bool _6m(ref _0004.b P_0)
		{
			N._0006 transform = MeshTransform;
			float result;
			for (int i = 0; i < contacts.a5h; i++)
			{
				Vector3.DistanceSquared(ref contacts.Elements[i].Position, ref P_0.Position, out result);
				if (result < _0014.h.ContactMinimumSeparationDistanceSquared)
				{
					Vector3.Dot(ref contacts.Elements[i].Normal, ref P_0.Normal, out result);
					if (Math.Abs(result) >= _0014.h.a5h)
					{
						contacts.Elements[i].Normal = P_0.Normal;
						contacts.Elements[i].Position = P_0.Position;
						contacts.Elements[i].PenetrationDepth = P_0.PenetrationDepth;
						supplementData.Elements[i].BasePenetrationDepth = P_0.PenetrationDepth;
						N._0006.TransformByInverse(ref P_0.Position, ref convex.worldTransform, out supplementData.Elements[i].LocalOffsetA);
						N._0006.TransformByInverse(ref P_0.Position, ref transform, out supplementData.Elements[i].LocalOffsetB);
						return false;
					}
				}
			}
			for (int j = 0; j < a5b.a5h; j++)
			{
				Vector3.DistanceSquared(ref a5b.Elements[j].Position, ref P_0.Position, out result);
				if (result < _0014.h.ContactMinimumSeparationDistanceSquared)
				{
					Vector3.Dot(ref a5b.Elements[j].Normal, ref P_0.Normal, out result);
					if (Math.Abs(result) >= _0014.h.a5h)
					{
						return false;
					}
				}
			}
			return true;
		}

		protected virtual void ProcessCandidates(l._0006<_0004.b> candidates)
		{
		}

		public override void CleanUp()
		{
			supplementData.Clear();
			contacts.Clear();
			convex = null;
			foreach (KeyValuePair<_00065b, I.X> item in a5h)
			{
				item.Value.CleanUp();
				GiveBackTester(item.Value);
			}
			a5h.Clear();
			CleanUpOverlappingTriangles();
			base.CleanUp();
		}
	}
	internal abstract class _0018 : _0006
	{
		protected P._0006 terrain;

		internal l._7<_00065b> a5h = new l._7<_00065b>(4);

		public P._0006 Terrain => terrain;

		protected override bool UseImprovedBoundaryHandling => terrain.a5b;

		protected internal override int FindOverlappingTriangles(float dt)
		{
			convex.Shape.GetLocalBoundingBox(ref convex.worldTransform, ref terrain.a5h, out var boundingBox);
			if (convex.entity != null)
			{
				N._7.Invert(ref terrain.a5h.LinearTransform, out var result);
				N._7.Transform(ref convex.entity.a5a, ref result, out var result2);
				Vector3.Multiply(ref result2, dt, out result2);
				if (result2.X > 0f)
				{
					boundingBox.Max.X += result2.X;
				}
				else
				{
					boundingBox.Min.X += result2.X;
				}
				if (result2.Y > 0f)
				{
					boundingBox.Max.Y += result2.Y;
				}
				else
				{
					boundingBox.Min.Y += result2.Y;
				}
				if (result2.Z > 0f)
				{
					boundingBox.Max.Z += result2.Z;
				}
				else
				{
					boundingBox.Min.Z += result2.Z;
				}
			}
			terrain.Shape.GetOverlaps(boundingBox, a5h);
			return a5h.a5h;
		}

		protected override bool ConfigureTriangle(int i, out _00065b indices)
		{
			indices = a5h.Elements[i];
			terrain.Shape.GetTriangle(ref indices, ref terrain.a5h, out localTriangleShape.a5h, out localTriangleShape.a5b, out localTriangleShape.a56);
			localTriangleShape.collisionMargin = 0f;
			Vector3.Subtract(ref localTriangleShape.a5b, ref localTriangleShape.a5h, out var result);
			Vector3.Subtract(ref localTriangleShape.a56, ref localTriangleShape.a5h, out var result2);
			Vector3.Cross(ref result, ref result2, out var result3);
			Vector3 vector = new Vector3(terrain.a5h.LinearTransform.M21, terrain.a5h.LinearTransform.M22, terrain.a5h.LinearTransform.M23);
			Vector3.Dot(ref vector, ref result3, out var result4);
			if (result4 > 0f)
			{
				localTriangleShape.a5a = y._0006.Clockwise;
			}
			else
			{
				localTriangleShape.a5a = y._0006.Counterclockwise;
			}
			return true;
		}

		protected internal override void CleanUpOverlappingTriangles()
		{
			a5h.Clear();
		}

		protected override void ProcessCandidates(l._0006<_0004.b> candidates)
		{
			if (!((candidates.a5h == 0) & (terrain.a5a > 0f)))
			{
				return;
			}
			Ray ray = new Ray
			{
				Position = convex.worldTransform.Position,
				Direction = terrain.a5h.LinearTransform.Up
			};
			ray.Direction.Normalize();
			if (!terrain.Shape.RayCast(ref ray, terrain.a5a, ref terrain.a5h, y._0006.DoubleSided, out var hit))
			{
				return;
			}
			hit.Normal.Normalize();
			Vector3.Dot(ref ray.Direction, ref hit.Normal, out var result);
			_0004.b item = new _0004.b
			{
				Normal = hit.Normal,
				Position = convex.worldTransform.Position,
				Id = 2,
				PenetrationDepth = (0f - hit.T) * result + convex.Shape.minimumRadius
			};
			bool flag = false;
			for (int i = 0; i < contacts.a5h; i++)
			{
				if (contacts.Elements[i].Id == 2)
				{
					contacts.Elements[i].Normal = item.Normal;
					contacts.Elements[i].Position = item.Position;
					contacts.Elements[i].PenetrationDepth = item.PenetrationDepth;
					supplementData.Elements[i].BasePenetrationDepth = item.PenetrationDepth;
					supplementData.Elements[i].LocalOffsetA = default(Vector3);
					supplementData.Elements[i].LocalOffsetB = ray.Position;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				candidates.Add(ref item);
			}
		}

		public override void CleanUp()
		{
			terrain = null;
			convex = null;
			base.CleanUp();
		}

		public override void Initialize(P.h newCollidableA, P.h newCollidableB)
		{
			convex = newCollidableA as s.v;
			terrain = newCollidableB as P._0006;
			if (convex == null || terrain == null)
			{
				convex = newCollidableB as s.v;
				terrain = newCollidableA as P._0006;
				if (convex == null || terrain == null)
				{
					throw new Exception("Inappropriate types used to initialize contact manifold.");
				}
			}
		}
	}
	internal class _0002 : _0018
	{
		private new p.a<I.W> a5h = new p.a<I.W>();

		protected override I.X GetTester()
		{
			return a5h.Take();
		}

		protected override void GiveBackTester(I.X tester)
		{
			a5h.GiveBack((I.W)tester);
		}
	}
	internal class _000E : h
	{
		private l._0006<_0004._7> a5h = new l._0006<_0004._7>(4);

		private I._0018 a5b;

		private y.v a56 = new y.v();

		protected s.v convex;

		protected s.B<y.v> triangle;

		public I._0018 PairTester => a5b;

		public s.v Convex => convex;

		public s.B<y.v> Triangle => triangle;

		public _000E()
		{
			contacts = new l._7<_0004.h>(4);
			unusedContacts = new p.a<_0004.h>(4);
			contactIndicesToRemove = new l._7<int>(4);
			a5b = new I._0018();
		}

		public override void Update(float dt)
		{
			_0004.a.ContactRefresh(contacts, a5h, ref convex.worldTransform, ref triangle.worldTransform, contactIndicesToRemove);
			RemoveQueuedContacts();
			a56.collisionMargin = triangle.Shape.collisionMargin;
			a56.a5a = triangle.Shape.a5a;
			N._7.CreateFromQuaternion(ref triangle.worldTransform.Orientation, out var result);
			N._7.Transform(ref triangle.Shape.a5h, ref result, out a56.a5h);
			N._7.Transform(ref triangle.Shape.a5b, ref result, out a56.a5b);
			N._7.Transform(ref triangle.Shape.a56, ref result, out a56.a56);
			Vector3.Add(ref a56.a5h, ref triangle.worldTransform.Position, out a56.a5h);
			Vector3.Add(ref a56.a5b, ref triangle.worldTransform.Position, out a56.a5b);
			Vector3.Add(ref a56.a56, ref triangle.worldTransform.Position, out a56.a56);
			Vector3.Subtract(ref a56.a5h, ref convex.worldTransform.Position, out a56.a5h);
			Vector3.Subtract(ref a56.a5b, ref convex.worldTransform.Position, out a56.a5b);
			Vector3.Subtract(ref a56.a56, ref convex.worldTransform.Position, out a56.a56);
			N._7.CreateFromQuaternion(ref convex.worldTransform.Orientation, out result);
			N._7.TransformTranspose(ref a56.a5h, ref result, out a56.a5h);
			N._7.TransformTranspose(ref a56.a5b, ref result, out a56.a5b);
			N._7.TransformTranspose(ref a56.a56, ref result, out a56.a56);
			if (a5b.GenerateContactCandidate(out var contactList))
			{
				for (int i = 0; i < contactList.a5X; i++)
				{
					contactList.Get(i, out var item);
					N._7.Transform(ref item.Position, ref result, out item.Position);
					Vector3.Add(ref item.Position, ref convex.worldTransform.Position, out item.Position);
					N._7.Transform(ref item.Normal, ref result, out item.Normal);
					if (!_6m(ref item))
					{
						continue;
					}
					if (contacts.a5h == 4)
					{
						_0004._6.ReduceContacts(contacts, ref item, contactIndicesToRemove, out var addCandidate);
						RemoveQueuedContacts();
						if (addCandidate)
						{
							Add(ref item);
						}
					}
					else
					{
						Add(ref item);
					}
				}
			}
			else
			{
				for (int num = contacts.a5h - 1; num >= 0; num--)
				{
					Remove(num);
				}
			}
		}

		protected override void Add(ref _0004.b contactCandidate)
		{
			_0004._7 item = default(_0004._7);
			item.BasePenetrationDepth = contactCandidate.PenetrationDepth;
			N._0006.TransformByInverse(ref contactCandidate.Position, ref convex.worldTransform, out item.LocalOffsetA);
			N._0006.TransformByInverse(ref contactCandidate.Position, ref triangle.worldTransform, out item.LocalOffsetB);
			a5h.Add(ref item);
			base.Add(ref contactCandidate);
		}

		protected override void Remove(int contactIndex)
		{
			a5h.RemoveAt(contactIndex);
			base.Remove(contactIndex);
		}

		private bool _6m(ref _0004.b P_0)
		{
			for (int i = 0; i < contacts.a5h; i++)
			{
				Vector3.DistanceSquared(ref contacts.Elements[i].Position, ref P_0.Position, out var result);
				if (result < _0014.h.ContactMinimumSeparationDistanceSquared)
				{
					contacts.Elements[i].Normal = P_0.Normal;
					contacts.Elements[i].Position = P_0.Position;
					contacts.Elements[i].PenetrationDepth = P_0.PenetrationDepth;
					a5h.Elements[i].BasePenetrationDepth = P_0.PenetrationDepth;
					N._0006.TransformByInverse(ref P_0.Position, ref convex.worldTransform, out a5h.Elements[i].LocalOffsetA);
					N._0006.TransformByInverse(ref P_0.Position, ref triangle.worldTransform, out a5h.Elements[i].LocalOffsetB);
					return false;
				}
			}
			return true;
		}

		public override void Initialize(P.h newCollidableA, P.h newCollidableB)
		{
			convex = newCollidableA as s.v;
			triangle = newCollidableB as s.B<y.v>;
			if (convex == null || triangle == null)
			{
				convex = newCollidableB as s.v;
				triangle = newCollidableA as s.B<y.v>;
				if (convex == null || triangle == null)
				{
					throw new Exception("Inappropriate types used to initialize contact manifold.");
				}
			}
			a5b.Initialize(convex.Shape, a56);
		}

		public override void CleanUp()
		{
			a5h.Clear();
			contacts.Clear();
			convex = null;
			triangle = null;
			a5b.CleanUp();
			base.CleanUp();
		}
	}
}
