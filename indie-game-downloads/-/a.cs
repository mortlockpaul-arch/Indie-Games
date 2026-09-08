using System;
using System.Collections.Generic;
using _0004;
using _0014;
using D;
using E;
using J;
using L;
using Microsoft.Xna.Framework;
using N;
using P;
using Y;
using l;
using q;
using r;
using s;
using y;

namespace _000F
{
	internal class a
	{
		private l._7<_0004.b> a5h = new l._7<_0004.b>();

		private l._7<_0004.b> a5b = new l._7<_0004.b>();

		private l._7<_0004.b> a56 = new l._7<_0004.b>();

		private l._7<_0004.b> a5a = new l._7<_0004.b>();

		private l._7<_0004.b> a57 = new l._7<_0004.b>();

		private s.b a5_0006;

		private s.b a5v;

		private s.b a5B;

		private h a5X;

		private Func<Y.a, bool> a5_0018;

		public l._7<_0004.b> Contacts => a5h;

		public l._7<_0004.b> SupportContacts => a5b;

		public l._7<_0004.b> TractionContacts => a56;

		public l._7<_0004.b> SideContacts => a5a;

		public l._7<_0004.b> HeadContacts => a57;

		public a(h character)
		{
			a5X = character;
			a5B = new s.B<global::y.b>(character.Body.CollisionInformation.Shape);
			a5_0006 = new s.B<global::y.b>(new global::y.b(character.StanceManager.StandingHeight, character.Body.Radius));
			a5v = new s.B<global::y.b>(new global::y.b(character.StanceManager.CrouchingHeight, character.Body.Radius));
			a5B.CollisionRules = character.Body.CollisionInformation.CollisionRules;
			a5_0006.CollisionRules = character.Body.CollisionInformation.CollisionRules;
			a5v.CollisionRules = character.Body.CollisionInformation.CollisionRules;
			a5_0018 = bW;
		}

		private bool bW(Y.a P_0)
		{
			return q._7.CollisionRuleCalculator(P_0.CollisionRules, a5X.Body.CollisionInformation.CollisionRules) == q.a.Normal;
		}

		public bool RayCast(Ray ray, float length, out global::r._0006 earliestHit)
		{
			earliestHit = default(global::r._0006);
			earliestHit.T = float.MaxValue;
			foreach (P.h overlappedCollidable in a5X.Body.CollisionInformation.OverlappedCollidables)
			{
				float? num = ray.Intersects(overlappedCollidable.BoundingBox);
				if (num.HasValue)
				{
					float? num2 = num;
					if (num2.GetValueOrDefault() < length && num2.HasValue && overlappedCollidable.RayCast(ray, length, a5_0018, out var rayHit) && rayHit.T < earliestHit.T)
					{
						earliestHit = rayHit;
					}
				}
			}
			if (earliestHit.T == float.MaxValue)
			{
				return false;
			}
			return true;
		}

		public bool RayCast(Ray ray, float length, out global::r._0006 earliestHit, out P.h hitObject)
		{
			earliestHit = default(global::r._0006);
			earliestHit.T = float.MaxValue;
			hitObject = null;
			foreach (P.h overlappedCollidable in a5X.Body.CollisionInformation.OverlappedCollidables)
			{
				float? num = ray.Intersects(overlappedCollidable.BoundingBox);
				if (num.HasValue)
				{
					float? num2 = num;
					if (num2.GetValueOrDefault() < length && num2.HasValue && overlappedCollidable.RayCast(ray, length, a5_0018, out var rayHit) && rayHit.T < earliestHit.T)
					{
						earliestHit = rayHit;
						hitObject = overlappedCollidable;
					}
				}
			}
			if (earliestHit.T == float.MaxValue)
			{
				return false;
			}
			return true;
		}

		public bool RayCastHitAnything(Ray ray, float length)
		{
			foreach (P.h overlappedCollidable in a5X.Body.CollisionInformation.OverlappedCollidables)
			{
				float? num = ray.Intersects(overlappedCollidable.BoundingBox);
				if (num.HasValue)
				{
					float? num2 = num;
					if (num2.GetValueOrDefault() < length && num2.HasValue && overlappedCollidable.RayCast(ray, length, a5_0018, out var _))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void b_0002()
		{
			a5h.Clear();
			a5b.Clear();
			a56.Clear();
			a5a.Clear();
			a57.Clear();
		}

		public void QueryContacts(Vector3 position)
		{
			b_000E(position, a5B);
		}

		public void QueryContacts(Vector3 position, v stance)
		{
			b_000E(position, (stance == v.Standing) ? a5_0006 : a5v);
		}

		private void b_000E(Vector3 P_0, s.b P_1)
		{
			b_0002();
			N._0006 transform = default(N._0006);
			transform.Position = P_0;
			transform.Orientation = a5X.Body.Orientation;
			P_1.UpdateBoundingBoxForTransform(ref transform, 0f);
			_0004.b item = default(_0004.b);
			foreach (P.h overlappedCollidable in a5X.Body.CollisionInformation.OverlappedCollidables)
			{
				if (!overlappedCollidable.BoundingBox.Intersects(P_1.BoundingBox))
				{
					continue;
				}
				P._6 pair = new P._6(overlappedCollidable, P_1);
				D.b pairHandler = J.a.GetPairHandler(ref pair);
				if (pairHandler.CollisionRule == q.a.Normal)
				{
					pairHandler.UpdateCollision(0f);
					foreach (D._0001 contact in pairHandler.Contacts)
					{
						if (contact.Pair.CollisionRule == q.a.Normal)
						{
							item.Position = contact.Contact.Position;
							item.Normal = contact.Contact.Normal;
							item.Id = contact.Contact.Id;
							item.PenetrationDepth = contact.Contact.PenetrationDepth;
							a5h.Add(item);
						}
					}
				}
				pairHandler.CleanUp();
				pairHandler.Factory.GiveBack(pairHandler);
			}
			by(ref P_0);
		}

		private void by(ref Vector3 P_0)
		{
			Vector3 vector = a5X.Body.OrientationMatrix.Down;
			for (int i = 0; i < a5h.Count; i++)
			{
				Vector3.Subtract(ref a5h.Elements[i].Position, ref P_0, out var result);
				Vector3.Dot(ref a5h.Elements[i].Normal, ref result, out var result2);
				_0004.b item = a5h.Elements[i];
				if (result2 < 0f)
				{
					result2 = 0f - result2;
					Vector3.Negate(ref item.Normal, out item.Normal);
				}
				Vector3.Dot(ref item.Normal, ref vector, out result2);
				if (result2 > X.a5h)
				{
					a5b.Add(item);
					if (result2 > a5X.SupportFinder.a5B)
					{
						a56.Add(item);
					}
					else
					{
						a5a.Add(item);
					}
				}
				else if (result2 < 0f - X.a5h)
				{
					a57.Add(item);
				}
				else
				{
					a5a.Add(item);
				}
			}
		}

		internal bool br(out bool P_0, out _7 P_1, out _0004.b P_2)
		{
			float num = float.MinValue;
			int num2 = -1;
			if (a56.Count > 0)
			{
				for (int i = 0; i < a56.Count; i++)
				{
					if (a56.Elements[i].PenetrationDepth > num)
					{
						num = a56.Elements[i].PenetrationDepth;
						num2 = i;
					}
				}
				P_0 = true;
				P_2 = a56.Elements[num2];
			}
			else
			{
				if (a5b.Count <= 0)
				{
					P_0 = false;
					P_1 = _7.NoHit;
					P_2 = default(_0004.b);
					return false;
				}
				for (int j = 0; j < a5b.Count; j++)
				{
					if (a5b.Elements[j].PenetrationDepth > num)
					{
						num = a5b.Elements[j].PenetrationDepth;
						num2 = j;
					}
				}
				P_0 = false;
				P_2 = a5b.Elements[num2];
			}
			if (num > _0014.h.AllowedPenetration)
			{
				P_1 = _7.TooDeep;
			}
			else if (num < 0f)
			{
				P_1 = _7.NoHit;
			}
			else
			{
				P_1 = _7.Accepted;
			}
			return true;
		}
	}
}
namespace _0002
{
	internal interface a
	{
		void OnPairTouched(P.h sender, P.h other, D.b pair);
	}
}
namespace _0001
{
	internal interface a : h, global::r.h
	{
		void Update(float dt);
	}
}
namespace _000E
{
	internal class a : b
	{
		internal l._7<_6> a5h;

		public l.X<_6> Shapes => new l.X<_6>(a5h);

		public a(IList<_6> shapes, out Vector3 center)
		{
			center = ComputeCenter(shapes);
			if (shapes.Count > 0)
			{
				a5h = new l._7<_6>(shapes);
				for (int i = 0; i < a5h.a5h; i++)
				{
					a5h.Elements[i].LocalTransform.Position -= center;
				}
				return;
			}
			throw new Exception("Compound shape must have at least 1 subshape.");
		}

		public a(IList<_6> shapes)
		{
			Vector3 vector = ComputeCenter(shapes);
			if (shapes.Count > 0)
			{
				a5h = new l._7<_6>(shapes);
				for (int i = 0; i < a5h.a5h; i++)
				{
					a5h.Elements[i].LocalTransform.Position -= vector;
				}
				return;
			}
			throw new Exception("Compound shape must have at least 1 subshape.");
		}

		public override Vector3 ComputeCenter()
		{
			float num = 0f;
			Vector3 value = default(Vector3);
			for (int i = 0; i < a5h.a5h; i++)
			{
				num += a5h.Elements[i].Weight;
				Vector3.Multiply(ref a5h.Elements[i].LocalTransform.Position, a5h.Elements[i].Weight, out var result);
				Vector3.Add(ref value, ref result, out value);
			}
			Vector3.Multiply(ref value, 1f / num, out value);
			return value;
		}

		public static Vector3 ComputeCenter(IList<s.a> childData)
		{
			Vector3 value = default(Vector3);
			float num = 0f;
			for (int i = 0; i < childData.Count; i++)
			{
				float num2 = childData[i].Entry.Shape.ComputeVolume();
				num += num2;
				value += childData[i].Entry.LocalTransform.Position * num2;
			}
			Vector3.Divide(ref value, num, out value);
			return value;
		}

		public static Vector3 ComputeCenter(IList<_6> childData)
		{
			Vector3 value = default(Vector3);
			float num = 0f;
			for (int i = 0; i < childData.Count; i++)
			{
				float weight = childData[i].Weight;
				num += weight;
				value += childData[i].LocalTransform.Position * weight;
			}
			Vector3.Divide(ref value, num, out value);
			return value;
		}

		public override float ComputeVolume()
		{
			float num = 0f;
			for (int i = 0; i < a5h.a5h; i++)
			{
				num += a5h.Elements[i].Shape.ComputeVolume();
			}
			return num;
		}

		public override N._7 ComputeVolumeDistribution(out float volume)
		{
			volume = ComputeVolume();
			return ComputeVolumeDistribution();
		}

		public override N._7 ComputeVolumeDistribution()
		{
			N._7 result = default(N._7);
			float num = 0f;
			for (int i = 0; i < a5h.a5h; i++)
			{
				num += a5h.Elements[i].Weight;
				GetContribution(a5h.Elements[i].Shape, ref a5h.Elements[i].LocalTransform, ref r.X.ZeroVector, a5h.Elements[i].Weight, out var contribution);
				N._7.Add(ref contribution, ref result, out result);
			}
			N._7.Multiply(ref result, 1f / num, out result);
			return result;
		}

		public static N._7 ComputeVolumeDistribution(IList<_6> entries, out Vector3 center)
		{
			center = default(Vector3);
			float num = 0f;
			for (int i = 0; i < entries.Count; i++)
			{
				center += entries[i].LocalTransform.Position * entries[i].Weight;
				num += entries[i].Weight;
			}
			center /= num;
			N._7 result = default(N._7);
			for (int j = 0; j < entries.Count; j++)
			{
				N._0006 transform = entries[j].LocalTransform;
				GetContribution(entries[j].Shape, ref transform, ref center, entries[j].Weight, out var contribution);
				N._7.Add(ref result, ref contribution, out result);
			}
			return result;
		}

		public static void GetContribution(b shape, ref N._0006 transform, ref Vector3 center, float weight, out N._7 contribution)
		{
			contribution = shape.ComputeVolumeDistribution();
			TransformContribution(ref transform, ref center, ref contribution, weight, out contribution);
		}

		public static void TransformContribution(ref N._0006 transform, ref Vector3 center, ref N._7 baseContribution, float weight, out N._7 contribution)
		{
			N._7.CreateFromQuaternion(ref transform.Orientation, out var result);
			N._7.MultiplyTransposed(ref result, ref baseContribution, out var result2);
			N._7.Multiply(ref result2, ref result, out result2);
			contribution = result2;
			Vector3.Subtract(ref transform.Position, ref center, out var result3);
			N._7.CreateScale(result3.LengthSquared(), out var matrix);
			N._7.CreateOuterProduct(ref result3, ref result3, out var result4);
			N._7.Subtract(ref matrix, ref result4, out result2);
			N._7.Add(ref contribution, ref result2, out contribution);
			N._7.Multiply(ref contribution, weight, out contribution);
		}

		public override s.b GetCollidableInstance()
		{
			return new s._6(this);
		}

		public override Vector3 ComputeCenter(out float volume)
		{
			volume = ComputeVolume();
			return ComputeCenter();
		}

		public override void ComputeDistributionInformation(out _7 shapeInfo)
		{
			shapeInfo.VolumeDistribution = ComputeVolumeDistribution(out shapeInfo.Volume);
			shapeInfo.Center = ComputeCenter();
		}

		public _7[] ComputeChildContributions()
		{
			_7[] array = new _7[a5h.a5h];
			for (int i = 0; i < a5h.a5h; i++)
			{
				a5h.Elements[i].Shape.ComputeDistributionInformation(out array[i]);
			}
			return array;
		}
	}
}
namespace _0017
{
	internal struct a
	{
		public Vector3 A;

		public Vector3 B;

		public Vector3 C;

		public Vector3 D;
	}
}
namespace _0004
{
	internal class a
	{
		public static void ContactRefresh(l._7<h> contacts, l._0006<_7> supplementData, ref N._0006 transformA, ref N._0006 transformB, l._7<int> toRemove)
		{
			for (int i = 0; i < contacts.a5h; i++)
			{
				_7 obj = supplementData.Elements[i];
				N._0006.Transform(ref obj.LocalOffsetA, ref transformA, out var result);
				N._0006.Transform(ref obj.LocalOffsetB, ref transformB, out var result2);
				Vector3.Subtract(ref result2, ref result, out var result3);
				Vector3.Dot(ref result3, ref contacts.Elements[i].Normal, out var result4);
				Vector3.Multiply(ref contacts.Elements[i].Normal, result4, out var result5);
				Vector3.Subtract(ref result3, ref result5, out result5);
				result4 = result5.LengthSquared();
				if (result4 > _0014.h.ContactInvalidationLengthSquared)
				{
					toRemove.Add(i);
					continue;
				}
				Vector3.Dot(ref result3, ref contacts.Elements[i].Normal, out result4);
				contacts.Elements[i].PenetrationDepth = obj.BasePenetrationDepth - result4;
				if (contacts.Elements[i].PenetrationDepth < 0f - _0014.h.a5b)
				{
					toRemove.Add(i);
					continue;
				}
				Vector3.Add(ref result2, ref result, out var result6);
				Vector3.Multiply(ref result6, 0.5f, out result6);
				contacts.Elements[i].Position = result6;
			}
		}
	}
}
namespace _0010
{
	internal class a : L.h
	{
		internal new _0004.h a5h;

		internal float a5b;

		internal float a56;

		internal float a5a;

		internal float a57;

		internal float a5_0006;

		internal float a5v;

		internal float a5B;

		private float a5X;

		private float a5_0018;

		private float a5W;

		private float a5_0002;

		private E.h a5_000E;

		private E.h a5y;

		private bool a5r;

		private bool a5_0001;

		internal float a5_000F;

		private b a5Z;

		internal Vector3 a5u;

		internal Vector3 a5L;

		public _0004.h Contact => a5h;

		public float NormalForce => a5b;

		public float RelativeVelocity
		{
			get
			{
				float num = 0f;
				if (a5_000E != null)
				{
					num = a5_000E.a5a.X * a5_0018 + a5_000E.a5a.Y * a5W + a5_000E.a5a.Z * a5_0002 + a5_000E.a5_0006.X * a56 + a5_000E.a5_0006.Y * a5a + a5_000E.a5_0006.Z * a57;
				}
				if (a5y != null)
				{
					num += (0f - a5y.a5a.X) * a5_0018 - a5y.a5a.Y * a5W - a5y.a5a.Z * a5_0002 + a5y.a5_0006.X * a5_0006 + a5y.a5_0006.Y * a5v + a5y.a5_0006.Z * a5B;
				}
				return num;
			}
		}

		public a()
		{
			isActive = false;
		}

		public void Setup(b contactManifoldConstraint, _0004.h contact)
		{
			a5Z = contactManifoldConstraint;
			a5h = contact;
			isActive = true;
			a5_000E = contactManifoldConstraint.EntityA;
			a5y = contactManifoldConstraint.EntityB;
			a5r = a5_000E != null && a5_000E.a5B;
			a5_0001 = a5y != null && a5y.a5B;
		}

		public void CleanUp()
		{
			a5b = 0f;
			a5Z = null;
			a5h = null;
			a5_000E = null;
			a5y = null;
			isActive = false;
		}

		public override void Update(float dt)
		{
			a5_0018 = 0f - a5h.Normal.X;
			a5W = 0f - a5h.Normal.Y;
			a5_0002 = 0f - a5h.Normal.Z;
			if (a5_000E != null)
			{
				Vector3.Subtract(ref a5h.Position, ref a5_000E.a5h, out a5u);
				a56 = a5u.Y * a5_0002 - a5u.Z * a5W;
				a5a = a5u.Z * a5_0018 - a5u.X * a5_0002;
				a57 = a5u.X * a5W - a5u.Y * a5_0018;
			}
			if (a5y != null)
			{
				Vector3.Subtract(ref a5h.Position, ref a5y.a5h, out a5L);
				a5_0006 = a5W * a5L.Z - a5_0002 * a5L.Y;
				a5v = a5_0002 * a5L.X - a5_0018 * a5L.Z;
				a5B = a5_0018 * a5L.Y - a5W * a5L.X;
			}
			float num4;
			if (a5r)
			{
				float num = a56 * a5_000E.a5_0018.M11 + a5a * a5_000E.a5_0018.M21 + a57 * a5_000E.a5_0018.M31;
				float num2 = a56 * a5_000E.a5_0018.M12 + a5a * a5_000E.a5_0018.M22 + a57 * a5_000E.a5_0018.M32;
				float num3 = a56 * a5_000E.a5_0018.M13 + a5a * a5_000E.a5_0018.M23 + a57 * a5_000E.a5_0018.M33;
				num4 = num * a56 + num2 * a5a + num3 * a57 + a5_000E.a5r;
			}
			else
			{
				num4 = 0f;
			}
			float num5;
			if (a5_0001)
			{
				float num = a5_0006 * a5y.a5_0018.M11 + a5v * a5y.a5_0018.M21 + a5B * a5y.a5_0018.M31;
				float num2 = a5_0006 * a5y.a5_0018.M12 + a5v * a5y.a5_0018.M22 + a5B * a5y.a5_0018.M32;
				float num3 = a5_0006 * a5y.a5_0018.M13 + a5v * a5y.a5_0018.M23 + a5B * a5y.a5_0018.M33;
				num5 = num * a5_0006 + num2 * a5v + num3 * a5B + a5y.a5r;
			}
			else
			{
				num5 = 0f;
			}
			a5_000F = -1f / (num4 + num5);
			if (a5h.PenetrationDepth >= 0f)
			{
				a5X = MathHelper.Min(MathHelper.Max(0f, a5h.PenetrationDepth - _0014.h.AllowedPenetration) * _0014.b.PenetrationRecoveryStiffness / dt, _0014.b.MaximumPositionCorrectionSpeed);
				if (a5Z.a5h.Bounciness > 0f)
				{
					float num6 = 0f - RelativeVelocity;
					if (num6 > _0014.b.BouncinessVelocityThreshold)
					{
						a5X = MathHelper.Max(num6 * a5Z.a5h.Bounciness, a5X);
					}
				}
			}
			else
			{
				a5X = a5h.PenetrationDepth / dt;
			}
		}

		public override void ExclusiveUpdate()
		{
			Vector3 impulse = default(Vector3);
			Vector3 impulse2 = default(Vector3);
			impulse.X = a5b * a5_0018;
			impulse.Y = a5b * a5W;
			impulse.Z = a5b * a5_0002;
			if (a5r)
			{
				impulse2.X = a5b * a56;
				impulse2.Y = a5b * a5a;
				impulse2.Z = a5b * a57;
				a5_000E.ApplyLinearImpulse(ref impulse);
				a5_000E.ApplyAngularImpulse(ref impulse2);
			}
			if (a5_0001)
			{
				impulse.X = 0f - impulse.X;
				impulse.Y = 0f - impulse.Y;
				impulse.Z = 0f - impulse.Z;
				impulse2.X = a5b * a5_0006;
				impulse2.Y = a5b * a5v;
				impulse2.Z = a5b * a5B;
				a5y.ApplyLinearImpulse(ref impulse);
				a5y.ApplyAngularImpulse(ref impulse2);
			}
		}

		public override float SolveIteration()
		{
			float num = (RelativeVelocity - a5X) * a5_000F;
			float num2 = a5b;
			a5b = MathHelper.Max(0f, a5b + num);
			num = a5b - num2;
			Vector3 impulse = default(Vector3);
			Vector3 impulse2 = default(Vector3);
			impulse.X = num * a5_0018;
			impulse.Y = num * a5W;
			impulse.Z = num * a5_0002;
			if (a5r)
			{
				impulse2.X = num * a56;
				impulse2.Y = num * a5a;
				impulse2.Z = num * a57;
				a5_000E.ApplyLinearImpulse(ref impulse);
				a5_000E.ApplyAngularImpulse(ref impulse2);
			}
			if (a5_0001)
			{
				impulse.X = 0f - impulse.X;
				impulse.Y = 0f - impulse.Y;
				impulse.Z = 0f - impulse.Z;
				impulse2.X = num * a5_0006;
				impulse2.Y = num * a5v;
				impulse2.Z = num * a5B;
				a5y.ApplyLinearImpulse(ref impulse);
				a5y.ApplyAngularImpulse(ref impulse2);
			}
			return Math.Abs(num);
		}

		protected internal override void CollectInvolvedEntities(l._7<E.h> outputInvolvedEntities)
		{
			if (a5_000E != null)
			{
				outputInvolvedEntities.Add(a5_000E);
			}
			if (a5y != null)
			{
				outputInvolvedEntities.Add(a5y);
			}
		}
	}
}
