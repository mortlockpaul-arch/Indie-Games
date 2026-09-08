using System;
using _0004;
using _0014;
using I;
using Microsoft.Xna.Framework;
using P;
using l;
using p;
using r;
using s;
using y;

namespace I
{
	internal struct b
	{
		public h D1;

		public h D2;

		public h D3;

		public h D4;

		public h D5;

		public h D6;

		public h D7;

		public h D8;

		public byte Count;
	}
	internal static class B
	{
		public static bool AreSpheresColliding(y._7 a, y._7 b, ref Vector3 positionA, ref Vector3 positionB, out _0004.b contact)
		{
			contact = default(_0004.b);
			float num = a.collisionMargin + b.collisionMargin;
			Vector3.Subtract(ref positionB, ref positionA, out var result);
			float num2 = result.LengthSquared();
			if (num2 < (num + _0014.h.a5b) * (num + _0014.h.a5b))
			{
				if (num > 1E-07f)
				{
					Vector3.Multiply(ref result, a.collisionMargin / num, out contact.Position);
				}
				else
				{
					contact.Position = default(Vector3);
				}
				Vector3.Add(ref contact.Position, ref positionA, out contact.Position);
				num2 = (float)Math.Sqrt(num2);
				if (num2 > 1E-05f)
				{
					Vector3.Divide(ref result, num2, out contact.Normal);
				}
				else
				{
					contact.Normal = r.X.UpVector;
				}
				contact.PenetrationDepth = num - num2;
				return true;
			}
			return false;
		}
	}
}
namespace i
{
	internal class b : h
	{
		protected s.B<y._6> boxA;

		protected s.B<y._6> boxB;

		public s.B<y._6> CollidableA => boxA;

		public s.B<y._6> CollidableB => boxB;

		public b()
		{
			contacts = new l._7<_0004.h>(4);
			unusedContacts = new p.a<_0004.h>(4);
			contactIndicesToRemove = new l._7<int>(4);
		}

		public override void Update(float dt)
		{
			l.W<I.h> contactData = default(l.W<I.h>);
			if (I._6.AreBoxesColliding(boxA.Shape, boxB.Shape, ref boxA.worldTransform, ref boxB.worldTransform, out var _, out var axis, out contactData))
			{
				Vector3.Negate(ref axis, out axis);
				l._0018<int> obj = default(l._0018<int>);
				I.h item;
				for (int i = 0; i < contacts.a5h; i++)
				{
					bool flag = false;
					for (int num = contactData.Count - 1; num >= 0; num--)
					{
						contactData.Get(num, out item);
						if (contacts.Elements[i].Id == item.Id)
						{
							flag = true;
							contacts.Elements[i].Position = item.Position;
							contacts.Elements[i].PenetrationDepth = 0f - item.Depth;
							contacts.Elements[i].Normal = axis;
							contactData.RemoveAt(num);
							break;
						}
					}
					if (!flag)
					{
						obj.Add(i);
					}
				}
				for (int num2 = obj.Count - 1; num2 >= 0; num2--)
				{
					Remove(obj[num2]);
				}
				for (int j = 0; j < contactData.Count; j++)
				{
					contactData.Get(j, out item);
					_0004.b contactCandidate = new _0004.b
					{
						Position = item.Position,
						PenetrationDepth = 0f - item.Depth,
						Normal = axis,
						Id = item.Id
					};
					Add(ref contactCandidate);
				}
			}
			else
			{
				for (int num3 = contacts.a5h - 1; num3 >= 0; num3--)
				{
					Remove(num3);
				}
			}
		}

		public override void Initialize(P.h newCollidableA, P.h newCollidableB)
		{
			boxA = (s.B<y._6>)newCollidableA;
			boxB = (s.B<y._6>)newCollidableB;
			if (boxA == null || boxB == null)
			{
				throw new Exception("Inappropriate types used to initialize pair tester.");
			}
		}

		public override void CleanUp()
		{
			contacts.Clear();
			boxA = null;
			boxB = null;
			base.CleanUp();
		}
	}
	internal class B : v
	{
		private new p.a<I._0018> a5h = new p.a<I._0018>();

		protected override void GiveBackTester(I.X tester)
		{
			a5h.GiveBack((I._0018)tester);
		}

		protected override I.X GetTester()
		{
			return a5h.Take();
		}
	}
}
