using System;
using _0004;
using _0017;
using I;
using Microsoft.Xna.Framework;
using P;
using l;
using s;
using y;

namespace I
{
	internal class _7
	{
		private enum _00065h
		{
			Separated,
			ShallowContact,
			DeepContact
		}

		public static bool UseSimplexCaching;

		private _00065h a5h;

		private _00065h a5b;

		private Vector3 a56;

		private _0017._6 a5a;

		protected internal s.v collidableA;

		protected internal s.v collidableB;

		private Vector3 a57;

		public s.v CollidableA => collidableA;

		public s.v CollidableB => collidableB;

		public bool GenerateContactCandidate(out _0004.b contact)
		{
			a5b = a5h;
			switch (a5h)
			{
			case _00065h.Separated:
				if (_0017.h.AreShapesIntersecting(collidableA.Shape, collidableB.Shape, ref collidableA.worldTransform, ref collidableB.worldTransform, ref a56))
				{
					a5h = _00065h.ShallowContact;
					return _6_0006(out contact);
				}
				contact = default(_0004.b);
				return false;
			case _00065h.ShallowContact:
				return _6_0006(out contact);
			case _00065h.DeepContact:
				return _6v(out contact);
			default:
				contact = default(_0004.b);
				return false;
			}
		}

		private bool _6_0006(out _0004.b P_0)
		{
			bool closestPoints;
			Vector3 closestPointA;
			Vector3 closestPointB;
			if (UseSimplexCaching)
			{
				closestPoints = _0017.h.GetClosestPoints(collidableA.Shape, collidableB.Shape, ref collidableA.worldTransform, ref collidableB.worldTransform, ref a5a, out closestPointA, out closestPointB);
			}
			else
			{
				_0017._6 cachedSimplex = a5a;
				closestPoints = _0017.h.GetClosestPoints(collidableA.Shape, collidableB.Shape, ref collidableA.worldTransform, ref collidableB.worldTransform, ref cachedSimplex, out closestPointA, out closestPointB);
			}
			Vector3.Subtract(ref closestPointB, ref closestPointA, out var result);
			if (closestPoints)
			{
				a5h = _00065h.DeepContact;
				return _6v(out P_0);
			}
			a57 = result;
			float num = result.LengthSquared();
			float num2 = collidableA.Shape.collisionMargin + collidableB.Shape.collisionMargin;
			if (num < num2 * num2)
			{
				P_0 = default(_0004.b);
				if (num2 > 1E-07f)
				{
					Vector3.Multiply(ref result, collidableA.Shape.collisionMargin / num2, out P_0.Position);
				}
				else
				{
					P_0.Position = default(Vector3);
				}
				Vector3.Add(ref closestPointA, ref P_0.Position, out P_0.Position);
				P_0.Normal = result;
				float num3 = (float)Math.Sqrt(num);
				Vector3.Divide(ref P_0.Normal, num3, out P_0.Normal);
				P_0.PenetrationDepth = num2 - num3;
				return true;
			}
			a5h = _00065h.Separated;
			P_0 = default(_0004.b);
			return false;
		}

		private bool _6v(out _0004.b P_0)
		{
			if (a5b == _00065h.Separated)
			{
				if (collidableA.entity != null && collidableB.entity != null)
				{
					Vector3.Subtract(ref collidableA.entity.a5a, ref collidableB.entity.a5a, out a57);
				}
				else
				{
					a57 = a56;
				}
				if (a57.LengthSquared() < 1E-07f)
				{
					a57 = Vector3.Up;
				}
			}
			if (v.GetContact(collidableA.Shape, collidableB.Shape, ref collidableA.worldTransform, ref collidableB.worldTransform, ref a57, out P_0))
			{
				if (P_0.PenetrationDepth < collidableA.Shape.collisionMargin + collidableB.Shape.collisionMargin)
				{
					a5h = _00065h.ShallowContact;
				}
				return true;
			}
			a5h = _00065h.Separated;
			return false;
		}

		public void Initialize(P.h shapeA, P.h shapeB)
		{
			collidableA = (s.v)shapeA;
			collidableB = (s.v)shapeB;
			a5a = new _0017._6
			{
				State = _0017.b.Point
			};
		}

		public void CleanUp()
		{
			a5h = _00065h.Separated;
			a5b = _00065h.Separated;
			a5a = default(_0017._6);
			a56 = default(Vector3);
			collidableA = null;
			collidableB = null;
		}
	}
}
namespace i
{
	internal class _7 : h
	{
		protected s.B<y._7> sphereA;

		protected s.B<y._7> sphereB;

		private _0004.h a5h = new _0004.h();

		private bool a5b;

		public s.B<y._7> CollidableA => sphereA;

		public s.B<y._7> CollidableB => sphereB;

		public _7()
		{
			contacts = new l._7<_0004.h>(1);
		}

		public override void Update(float dt)
		{
			bool flag = false;
			if (I.B.AreSpheresColliding(sphereA.Shape, sphereB.Shape, ref sphereA.worldTransform.Position, ref sphereB.worldTransform.Position, out var contact))
			{
				if (!a5b && contact.PenetrationDepth >= 0f)
				{
					Add(ref contact);
					flag = true;
				}
				else if (a5b)
				{
					a5h.Normal = contact.Normal;
					a5h.PenetrationDepth = contact.PenetrationDepth;
					a5h.Position = contact.Position;
					flag = true;
				}
			}
			else if (a5b)
			{
				Remove(0);
			}
			a5b = flag;
		}

		protected override void Add(ref _0004.b contactCandidate)
		{
			a5h.Normal = contactCandidate.Normal;
			a5h.PenetrationDepth = contactCandidate.PenetrationDepth;
			a5h.Position = contactCandidate.Position;
			contacts.Add(a5h);
			OnAdded(a5h);
		}

		protected override void Remove(int index)
		{
			contacts.RemoveAt(index);
			OnRemoved(a5h);
		}

		public override void Initialize(P.h newCollidableA, P.h newCollidableB)
		{
			sphereA = (s.B<y._7>)newCollidableA;
			sphereB = (s.B<y._7>)newCollidableB;
			if (sphereA == null || sphereB == null)
			{
				throw new Exception("Inappropriate types used to initialize pair.");
			}
		}

		public override void CleanUp()
		{
			contacts.Clear();
			sphereA = null;
			sphereB = null;
			a5b = false;
			base.CleanUp();
		}

		public override void ClearContacts()
		{
			a5b = false;
			base.ClearContacts();
		}
	}
}
