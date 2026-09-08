using System;
using _0014;
using E;
using L;
using Microsoft.Xna.Framework;
using N;
using P;
using d;
using l;
using r;
using s;

namespace _000F
{
	internal class r : L.h
	{
		private new h a5h;

		private y a5b;

		private float a56 = 5000f;

		private float a5a;

		private float a57 = 1f;

		private float a5_0006;

		private E.h a5v;

		private Vector3 a5B;

		private Vector3 a5X;

		private Vector3 a5_0018;

		private float a5W;

		private float a5_0002;

		public y SupportData
		{
			get
			{
				return a5b;
			}
			set
			{
				P.h supportObject = a5b.SupportObject;
				a5b = value;
				if (supportObject != a5b.SupportObject)
				{
					OnInvolvedEntitiesChanged();
				}
			}
		}

		public float MaximumGlueForce
		{
			get
			{
				return a56;
			}
			set
			{
				if (a56 < 0f)
				{
					throw new Exception("Value must be nonnegative.");
				}
				a56 = value;
			}
		}

		public float SupportForceFactor
		{
			get
			{
				return a57;
			}
			set
			{
				if (value < 0f)
				{
					throw new Exception("Value must be nonnegative.");
				}
				a57 = value;
			}
		}

		public float EffectiveMass => a5_0006;

		public float RelativeVelocity
		{
			get
			{
				Vector3 vector = a5h.Body.LinearVelocity;
				Vector3.Dot(ref a5B, ref vector, out var result);
				if (a5v != null)
				{
					Vector3 vector2 = a5v.LinearVelocity;
					Vector3 vector3 = a5v.AngularVelocity;
					Vector3.Dot(ref a5X, ref vector2, out var result2);
					result += result2;
					Vector3.Dot(ref a5_0018, ref vector3, out result2);
					return result + result2;
				}
				return result;
			}
		}

		public r(h characterController)
		{
			a5h = characterController;
		}

		protected internal override void CollectInvolvedEntities(l._7<E.h> outputInvolvedEntities)
		{
			if (a5b.SupportObject is s.b b2)
			{
				outputInvolvedEntities.Add(b2.Entity);
			}
			outputInvolvedEntities.Add(a5h.Body);
		}

		public override void UpdateSolverActivity()
		{
			if (a5b.HasTraction)
			{
				base.UpdateSolverActivity();
			}
			else
			{
				isActiveInSolver = false;
			}
		}

		public override void Update(float dt)
		{
			if (a5b.SupportObject != null)
			{
				if (a5b.SupportObject is s.b b2)
				{
					a5v = b2.Entity;
				}
				else
				{
					a5v = null;
				}
			}
			else
			{
				a5v = null;
			}
			a5a = a56 * dt;
			if (a5b.Depth > 0f)
			{
				a5_0002 = _0014.b.MaximumPositionCorrectionSpeed;
			}
			else
			{
				a5_0002 = 0f;
			}
			a5B = a5b.Normal;
			Vector3.Negate(ref a5B, out a5X);
			a5_0006 = a5h.Body.InverseMass;
			if (a5v != null)
			{
				Vector3 vector = a5b.Position - a5v.Position;
				Vector3.Cross(ref vector, ref a5X, out a5_0018);
				if (a5v.IsDynamic)
				{
					N._7 matrix = a5v.InertiaTensorInverse;
					N._7.Transform(ref a5_0018, ref matrix, out var result);
					Vector3.Dot(ref result, ref a5_0018, out var result2);
					a5_0006 += a57 * (result2 + a5v.InverseMass);
				}
			}
			a5_0006 = 1f / a5_0006;
		}

		public override void ExclusiveUpdate()
		{
			Vector3 result = default(Vector3);
			Vector3 result2 = default(Vector3);
			Vector3.Multiply(ref a5B, a5W, out result);
			a5h.Body.ApplyLinearImpulse(ref result);
			if (a5v != null && a5v.IsDynamic)
			{
				Vector3.Multiply(ref result, 0f - a57, out result);
				Vector3.Multiply(ref a5_0018, a5W * a57, out result2);
				a5v.ApplyLinearImpulse(ref result);
				a5v.ApplyAngularImpulse(ref result2);
			}
		}

		public override float SolveIteration()
		{
			float num = RelativeVelocity + a5_0002;
			float num2 = (0f - num) * a5_0006;
			float num3 = a5W;
			a5W = MathHelper.Clamp(a5W + num2, 0f, a5a);
			num2 = a5W - num3;
			Vector3 result = default(Vector3);
			Vector3 result2 = default(Vector3);
			Vector3.Multiply(ref a5B, num2, out result);
			a5h.Body.ApplyLinearImpulse(ref result);
			if (a5v != null && a5v.IsDynamic)
			{
				Vector3.Multiply(ref result, 0f - a57, out result);
				Vector3.Multiply(ref a5_0018, num2 * a57, out result2);
				a5v.ApplyLinearImpulse(ref result);
				a5v.ApplyAngularImpulse(ref result2);
			}
			return Math.Abs(num2);
		}
	}
}
namespace _0001
{
	internal class r : global::_0001._0018<B>
	{
		public r(global::r.B timeStepSettings)
			: base(timeStepSettings)
		{
		}

		public r(global::r.B timeStepSettings, d.b threadManager)
			: base(timeStepSettings, threadManager)
		{
		}

		protected override void MultithreadedUpdate(int i)
		{
			if (simultaneouslyUpdatedUpdateables[i].IsUpdating)
			{
				simultaneouslyUpdatedUpdateables[i].Update(timeStepSettings.TimeStepDuration);
			}
		}

		protected override void SequentialUpdate(int i)
		{
			if (sequentiallyUpdatedUpdateables[i].IsUpdating)
			{
				sequentiallyUpdatedUpdateables[i].Update(timeStepSettings.TimeStepDuration);
			}
		}
	}
}
