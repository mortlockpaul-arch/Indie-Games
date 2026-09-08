using System;
using _0014;
using _0017;
using E;
using Microsoft.Xna.Framework;
using P;
using Y;
using i;
using m;
using s;
using y;

namespace D;

internal class q : _7
{
	private new s.B<global::y.v> a5h;

	private s.v a5b;

	private i._000E a56 = new i._000E();

	protected override P.h CollidableA => a5b;

	protected override P.h CollidableB => a5h;

	protected override E.h EntityA => a5b.entity;

	protected override E.h EntityB => a5h.entity;

	public override i.h ContactManifold => a56;

	public override void Initialize(global::Y.a entryA, global::Y.a entryB)
	{
		a5h = entryA as s.B<global::y.v>;
		a5b = entryB as s.v;
		if (a5h == null || a5b == null)
		{
			a5h = entryB as s.B<global::y.v>;
			a5b = entryA as s.v;
			if (a5h == null || a5b == null)
			{
				throw new Exception("Inappropriate types used to initialize pair.");
			}
		}
		base.a5h.a5h = a5b;
		base.a5h.a5b = a5h;
		base.Initialize(entryA, entryB);
	}

	public override void CleanUp()
	{
		base.CleanUp();
		a5h = null;
		a5b = null;
	}

	public override void UpdateTimeOfImpact(P.h requester, float dt)
	{
		global::Y._7 broadPhaseOverlap = base.BroadPhaseOverlap;
		if ((!broadPhaseOverlap.a5h.IsActive && !broadPhaseOverlap.a5b.IsActive) || ((a5b.entity.PositionUpdateMode != global::m._7.Continuous || a5h.entity.PositionUpdateMode != global::m._7.Continuous || broadPhaseOverlap.a5h != requester) && !((a5b.entity.PositionUpdateMode == global::m._7.Continuous) ^ (a5h.entity.PositionUpdateMode == global::m._7.Continuous))))
		{
			return;
		}
		Vector3.Subtract(ref a5h.entity.a5a, ref a5b.entity.a5a, out var result);
		Vector3.Multiply(ref result, dt, out result);
		float num = result.LengthSquared();
		float num2 = a5b.Shape.minimumRadius * _0014._6.CoreShapeScaling;
		timeOfImpact = 1f;
		if (num2 * num2 < num && _0017.h.CCDSphereCast(new Ray(a5b.worldTransform.Position, -result), num2, a5h.Shape, ref a5h.worldTransform, timeOfImpact, out var hit))
		{
			if (a5h.Shape.a5a != global::y._0006.DoubleSided)
			{
				Vector3.Subtract(ref a5h.Shape.a5b, ref a5h.Shape.a5h, out var result2);
				Vector3.Subtract(ref a5h.Shape.a56, ref a5h.Shape.a5h, out var result3);
				Vector3.Cross(ref result2, ref result3, out var result4);
				Vector3.Dot(ref hit.Normal, ref result4, out var result5);
				if ((a5h.Shape.a5a == global::y._0006.Counterclockwise && result5 < 0f) || (a5h.Shape.a5a == global::y._0006.Clockwise && result5 > 0f))
				{
					timeOfImpact = hit.T;
				}
			}
			else
			{
				timeOfImpact = hit.T;
			}
		}
		if (timeOfImpact == 0f)
		{
			timeOfImpact = 1f;
		}
	}
}
