using System;
using _0004;
using I;
using Microsoft.Xna.Framework;
using l;
using p;
using r;
using y;

namespace I
{
	internal sealed class W : X
	{
		internal new y._7 a5h;

		private _0004._0006 a5b;

		public override bool ShouldCorrectContactNormal => false;

		public override bool GenerateContactCandidate(out l.W<_0004.b> contactList)
		{
			contactList = default(l.W<_0004.b>);
			Vector3.Subtract(ref base.a5h.a5b, ref base.a5h.a5h, out var result);
			Vector3.Subtract(ref base.a5h.a56, ref base.a5h.a5h, out var result2);
			Vector3.Cross(ref result, ref result2, out var result3);
			if (result3.LengthSquared() < 1E-09f)
			{
				Vector3.Add(ref base.a5h.a5h, ref base.a5h.a5b, out result3);
				Vector3.Add(ref result3, ref base.a5h.a56, out result3);
				Vector3.Multiply(ref result3, 1f / 3f, out result3);
				if (result3.LengthSquared() < 1E-09f)
				{
					result3 = r.X.UpVector;
				}
			}
			Vector3.Dot(ref result3, ref base.a5h.a5h, out var result4);
			switch (base.a5h.a5a)
			{
			case y._0006.DoubleSided:
				if (result4 < 0f)
				{
					Vector3.Negate(ref result3, out result3);
				}
				break;
			case y._0006.Clockwise:
				if (result4 > 0f)
				{
					return false;
				}
				break;
			case y._0006.Counterclockwise:
				if (result4 < 0f)
				{
					return false;
				}
				break;
			}
			a5b = r.X.GetClosestPointOnTriangleToPoint(ref base.a5h.a5h, ref base.a5h.a5b, ref base.a5h.a56, ref r.X.ZeroVector, out var closestPoint);
			float num = closestPoint.LengthSquared();
			float num2 = base.a5h.collisionMargin + a5h.collisionMargin;
			if (num <= num2 * num2)
			{
				_0004.b item = default(_0004.b);
				if (num < 1E-07f)
				{
					Vector3.Negate(ref result3, out item.Normal);
					item.Normal.Normalize();
					item.PenetrationDepth = num2;
					contactList.Add(ref item);
					return true;
				}
				num = (float)Math.Sqrt(num);
				Vector3.Divide(ref closestPoint, num, out item.Normal);
				item.PenetrationDepth = num2 - num;
				item.Position = closestPoint;
				contactList.Add(ref item);
				return true;
			}
			return false;
		}

		public override _0004._0006 GetRegion(ref _0004.b contact)
		{
			return a5b;
		}

		public override void Initialize(y.h convex, y.v triangle)
		{
			a5h = (y._7)convex;
			base.a5h = triangle;
		}

		public override void CleanUp()
		{
			base.a5h = null;
			a5h = null;
			Updated = false;
		}
	}
}
namespace i
{
	internal class W : _0018
	{
		private new p.a<I._0018> a5h = new p.a<I._0018>();

		protected override I.X GetTester()
		{
			return a5h.Take();
		}

		protected override void GiveBackTester(I.X tester)
		{
			a5h.GiveBack((I._0018)tester);
		}
	}
}
