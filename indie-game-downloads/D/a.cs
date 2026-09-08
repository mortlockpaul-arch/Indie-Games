using System;
using System.Threading;
using _0014;
using _0017;
using Microsoft.Xna.Framework;
using P;
using Y;
using m;
using s;

namespace D
{
	internal abstract class a : D._6
	{
		public override void Initialize(global::Y.a entryA, global::Y.a entryB)
		{
			UpdateMaterialProperties();
			base.Initialize(entryA, entryB);
		}

		public override void UpdateTimeOfImpact(P.h requester, float dt)
		{
			s.v v2 = CollidableA as s.v;
			s.v v3 = CollidableB as s.v;
			global::Y._7 broadPhaseOverlap = base.BroadPhaseOverlap;
			if ((broadPhaseOverlap.a5h.IsActive || broadPhaseOverlap.a5b.IsActive) && ((v2.entity.PositionUpdateMode == global::m._7.Continuous && v3.entity.PositionUpdateMode == global::m._7.Continuous && broadPhaseOverlap.a5h == requester) || ((v2.entity.PositionUpdateMode == global::m._7.Continuous) ^ (v3.entity.PositionUpdateMode == global::m._7.Continuous))))
			{
				Vector3 result;
				if (v2.entity.PositionUpdateMode == global::m._7.Discrete)
				{
					result = v3.entity.a5a;
				}
				else if (v3.entity.PositionUpdateMode == global::m._7.Discrete)
				{
					Vector3.Negate(ref v2.entity.a5a, out result);
				}
				else
				{
					Vector3.Subtract(ref v3.entity.a5a, ref v2.entity.a5a, out result);
				}
				Vector3.Multiply(ref result, dt, out result);
				float num = result.LengthSquared();
				float num2 = v2.Shape.minimumRadius * _0014._6.CoreShapeScaling;
				timeOfImpact = 1f;
				if (num2 * num2 < num && _0017.h.CCDSphereCast(new Ray(v2.worldTransform.Position, -result), num2, v3.Shape, ref v3.worldTransform, timeOfImpact, out var hit))
				{
					timeOfImpact = hit.T;
				}
				float num3 = v3.Shape.minimumRadius * _0014._6.CoreShapeScaling;
				if (num3 * num3 < num && _0017.h.CCDSphereCast(new Ray(v3.worldTransform.Position, result), num3, v2.Shape, ref v2.worldTransform, timeOfImpact, out var hit2))
				{
					timeOfImpact = hit2.T;
				}
				if (timeOfImpact == 0f)
				{
					timeOfImpact = 1f;
				}
			}
		}
	}
}
namespace d
{
	internal class a : IDisposable
	{
		private readonly _6 a5h;

		internal bool a5b;

		internal object a56 = new object();

		internal int a5a;

		internal AutoResetEvent a57;

		private object a5_0006;

		internal int a5v;

		private Thread a5B;

		private Action<object> a5X;

		internal a(_6 P_0, Action<object> P_1, object P_2)
		{
			a5h = P_0;
			a5X = P_1;
			a5_0006 = P_2;
			a57 = new AutoResetEvent(initialState: false);
			a5B = new Thread(a_0001);
			a5B.IsBackground = true;
			a5B.Start();
		}

		~a()
		{
			Dispose();
		}

		public void Dispose()
		{
			lock (a56)
			{
				if (!a5b)
				{
					a5b = true;
					a57.Close();
					a57 = null;
					a5B = null;
					GC.SuppressFinalize(this);
				}
			}
		}

		internal void a_0001()
		{
			if (a5X != null)
			{
				a5X(a5_0006);
			}
			a5X = null;
			a5_0006 = null;
			while (true)
			{
				a57.WaitOne();
				if (a5h.a5_0006 == null)
				{
					break;
				}
				while (a5h.a5_0018 <= a5h.a5W)
				{
					int num = Interlocked.Increment(ref a5h.a5_0018);
					int num2 = num * a5v;
					int num3 = num2 - a5v;
					for (int i = num3; i < num2 && i < a5a; i++)
					{
						a5h.a5_0006(i);
					}
				}
				a5h.ar();
			}
			a5h.ar();
		}
	}
}
