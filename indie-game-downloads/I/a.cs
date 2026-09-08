using System;
using _0004;
using _0014;
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
	internal static class a
	{
		public static bool AreShapesColliding(y._6 box, y._7 sphere, ref N._0006 boxTransform, ref Vector3 spherePosition, out _0004.b contact)
		{
			contact = default(_0004.b);
			N._0006.TransformByInverse(ref spherePosition, ref boxTransform, out var result);
			Vector3 position = new Vector3
			{
				X = MathHelper.Clamp(result.X, 0f - box.a5h, box.a5h),
				Y = MathHelper.Clamp(result.Y, 0f - box.a5b, box.a5b),
				Z = MathHelper.Clamp(result.Z, 0f - box.a56, box.a56)
			};
			N._0006.Transform(ref position, ref boxTransform, out contact.Position);
			Vector3.Subtract(ref spherePosition, ref contact.Position, out var result2);
			float num = result2.LengthSquared();
			if (num > (sphere.collisionMargin + _0014.h.a5b) * (sphere.collisionMargin + _0014.h.a5b))
			{
				return false;
			}
			if (num > 1E-07f)
			{
				num = (float)Math.Sqrt(num);
				Vector3.Divide(ref result2, num, out contact.Normal);
				contact.PenetrationDepth = sphere.collisionMargin - num;
			}
			else
			{
				Vector3 vector = default(Vector3);
				vector.X = ((position.X < 0f) ? (position.X + box.a5h) : (box.a5h - position.X));
				vector.Y = ((position.Y < 0f) ? (position.Y + box.a5b) : (box.a5b - position.Y));
				vector.Z = ((position.Z < 0f) ? (position.Z + box.a56) : (box.a56 - position.Z));
				if (vector.X < vector.Y && vector.X < vector.Z)
				{
					contact.Normal = ((position.X > 0f) ? r.X.RightVector : r.X.LeftVector);
					contact.PenetrationDepth = vector.X;
				}
				else if (vector.Y < vector.Z)
				{
					contact.Normal = ((position.Y > 0f) ? r.X.UpVector : r.X.DownVector);
					contact.PenetrationDepth = vector.Y;
				}
				else
				{
					contact.Normal = ((position.Z > 0f) ? r.X.BackVector : r.X.ForwardVector);
					contact.PenetrationDepth = vector.X;
				}
				contact.PenetrationDepth += sphere.collisionMargin;
				Vector3.Transform(ref contact.Normal, ref boxTransform.Orientation, out contact.Normal);
			}
			return true;
		}
	}
}
namespace i
{
	internal class a : h
	{
		private l._0006<_0004._7> a5h = new l._0006<_0004._7>(4);

		private I._7 a5b;

		protected s.v collidableA;

		protected s.v collidableB;

		public I._7 PairTester => a5b;

		public s.v CollidableA => collidableA;

		public s.v CollidableB => collidableB;

		public a()
		{
			contacts = new l._7<_0004.h>(4);
			unusedContacts = new p.a<_0004.h>(4);
			contactIndicesToRemove = new l._7<int>(4);
			a5b = new I._7();
		}

		public override void Update(float dt)
		{
			_0004.a.ContactRefresh(contacts, a5h, ref collidableA.worldTransform, ref collidableB.worldTransform, contactIndicesToRemove);
			RemoveQueuedContacts();
			if (a5b.GenerateContactCandidate(out var contact))
			{
				if (!_6m(ref contact))
				{
					return;
				}
				if (contacts.a5h == 4)
				{
					_0004._6.ReduceContacts(contacts, ref contact, contactIndicesToRemove, out var addCandidate);
					RemoveQueuedContacts();
					if (addCandidate)
					{
						Add(ref contact);
					}
				}
				else
				{
					Add(ref contact);
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
			N._0006.TransformByInverse(ref contactCandidate.Position, ref collidableA.worldTransform, out item.LocalOffsetA);
			N._0006.TransformByInverse(ref contactCandidate.Position, ref collidableB.worldTransform, out item.LocalOffsetB);
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
					N._0006.TransformByInverse(ref P_0.Position, ref collidableA.worldTransform, out a5h.Elements[i].LocalOffsetA);
					N._0006.TransformByInverse(ref P_0.Position, ref collidableB.worldTransform, out a5h.Elements[i].LocalOffsetB);
					return false;
				}
			}
			return true;
		}

		public override void Initialize(P.h newCollidableA, P.h newCollidableB)
		{
			collidableA = newCollidableA as s.v;
			collidableB = newCollidableB as s.v;
			a5b.Initialize(newCollidableA, newCollidableB);
			if (collidableA == null || collidableB == null)
			{
				throw new Exception("Inappropriate types used to initialize pair tester.");
			}
		}

		public override void CleanUp()
		{
			a5h.Clear();
			contacts.Clear();
			collidableA = null;
			collidableB = null;
			a5b.CleanUp();
			base.CleanUp();
		}
	}
}
