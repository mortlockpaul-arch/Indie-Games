using System;
using _0010;
using _0014;
using _0017;
using E;
using Microsoft.Xna.Framework;
using P;
using Y;
using i;
using m;
using p;
using r;
using s;
using y;

namespace D;

internal abstract class u : D._6
{
	private new P._7 a5h;

	private s.v a5b;

	private _0010._0006 a56 = new _0010._0006();

	protected override P.h CollidableA => a5b;

	protected override P.h CollidableB => a5h;

	protected override E.h EntityA => a5b.entity;

	protected override E.h EntityB => null;

	public override _0010.b ContactConstraint => a56;

	public override i.h ContactManifold => MeshManifold;

	protected abstract i.v MeshManifold { get; }

	public override void Initialize(global::Y.a entryA, global::Y.a entryB)
	{
		a5h = entryA as P._7;
		a5b = entryB as s.v;
		if (a5h == null || a5b == null)
		{
			a5h = entryB as P._7;
			a5b = entryA as s.v;
			if (a5h == null || a5b == null)
			{
				throw new Exception("Inappropriate types used to initialize pair.");
			}
		}
		base.a5h.a5h = a5b;
		base.a5h.a5b = a5h;
		UpdateMaterialProperties((a5b.entity != null) ? a5b.entity.a5Z : null, a5h.a5a);
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
		if (!a5b.IsActive || a5b.entity.PositionUpdateMode != global::m._7.Continuous)
		{
			return;
		}
		Vector3.Multiply(ref a5b.entity.a5a, dt, out var result);
		float num = result.LengthSquared();
		float num2 = a5b.Shape.minimumRadius * _0014._6.CoreShapeScaling;
		timeOfImpact = 1f;
		if (!(num2 * num2 < num))
		{
			return;
		}
		global::y.v triangle = p._6.GetTriangle();
		triangle.collisionMargin = 0f;
		for (int i = 0; i < MeshManifold.a5h.a5h; i++)
		{
			a5h.Shape.TriangleMeshData.GetTriangle(MeshManifold.a5h.Elements[i], out triangle.a5h, out triangle.a5b, out triangle.a56);
			Vector3.Subtract(ref triangle.a5h, ref a5b.worldTransform.Position, out triangle.a5h);
			Vector3.Subtract(ref triangle.a5b, ref a5b.worldTransform.Position, out triangle.a5b);
			Vector3.Subtract(ref triangle.a56, ref a5b.worldTransform.Position, out triangle.a56);
			if (!_0017.h.CCDSphereCast(new Ray(global::r.X.ZeroVector, result), num2, triangle, ref global::r.X.RigidIdentity, timeOfImpact, out var hit) || !(hit.T > 1E-05f))
			{
				continue;
			}
			if (a5h.a5b != global::y._0006.DoubleSided)
			{
				Vector3.Subtract(ref triangle.a5b, ref triangle.a5h, out var result2);
				Vector3.Subtract(ref triangle.a56, ref triangle.a5h, out var result3);
				Vector3.Cross(ref result2, ref result3, out var result4);
				Vector3.Dot(ref result4, ref hit.Normal, out var result5);
				if ((a5h.a5b == global::y._0006.Counterclockwise && result5 < 0f) || (a5h.a5b == global::y._0006.Clockwise && result5 > 0f))
				{
					timeOfImpact = hit.T;
				}
			}
			else
			{
				timeOfImpact = hit.T;
			}
		}
		p._6.GiveBack(triangle);
	}

	protected internal override void GetContactInformation(int index, out _0001 info)
	{
		info.Contact = MeshManifold.contacts.Elements[index];
		info.FrictionForce = 0f;
		info.NormalForce = 0f;
		for (int i = 0; i < a56.a56.a5h; i++)
		{
			if (a56.a56.Elements[i].PenetrationConstraint.a5h == info.Contact)
			{
				info.FrictionForce = a56.a56.Elements[i].a56;
				info.NormalForce = a56.a56.Elements[i].PenetrationConstraint.a5b;
				break;
			}
		}
		if (a5b.entity != null)
		{
			Vector3.Subtract(ref info.Contact.Position, ref a5b.entity.a5h, out var result);
			Vector3.Cross(ref a5b.entity.a5_0006, ref result, out result);
			Vector3.Add(ref result, ref a5b.entity.a5a, out info.RelativeVelocity);
		}
		else
		{
			info.RelativeVelocity = default(Vector3);
		}
		info.Pair = this;
	}
}
