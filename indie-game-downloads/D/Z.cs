using System;
using _0010;
using E;
using Microsoft.Xna.Framework;
using P;
using Y;
using i;
using s;
using y;

namespace D;

internal class Z : a
{
	private new s.B<global::y._7> a5h;

	private s.B<global::y._7> a5b;

	private i._7 a56 = new i._7();

	private _0010._0006 a5a = new _0010._0006();

	protected override P.h CollidableA => a5h;

	protected override P.h CollidableB => a5b;

	protected override E.h EntityA => a5h.entity;

	protected override E.h EntityB => a5b.entity;

	public override _0010.b ContactConstraint => a5a;

	public override i.h ContactManifold => a56;

	public override void Initialize(global::Y.a entryA, global::Y.a entryB)
	{
		a5h = entryA as s.B<global::y._7>;
		a5b = entryB as s.B<global::y._7>;
		if (a5h == null || a5b == null)
		{
			throw new Exception("Inappropriate types used to initialize pair.");
		}
		base.Initialize(entryA, entryB);
	}

	public override void CleanUp()
	{
		base.CleanUp();
		a5h = null;
		a5b = null;
	}

	protected internal override void GetContactInformation(int index, out _0001 info)
	{
		info.Contact = ContactManifold.contacts.Elements[index];
		info.FrictionForce = 0f;
		info.NormalForce = 0f;
		for (int i = 0; i < a5a.a56.a5h; i++)
		{
			if (a5a.a56.Elements[i].PenetrationConstraint.a5h == info.Contact)
			{
				info.FrictionForce = a5a.a56.Elements[i].a56;
				info.NormalForce = a5a.a56.Elements[i].PenetrationConstraint.a5b;
				break;
			}
		}
		Vector3 result;
		if (EntityA != null)
		{
			Vector3.Subtract(ref info.Contact.Position, ref EntityA.a5h, out result);
			Vector3.Cross(ref EntityA.a5_0006, ref result, out result);
			Vector3.Add(ref result, ref EntityA.a5a, out info.RelativeVelocity);
		}
		else
		{
			info.RelativeVelocity = default(Vector3);
		}
		if (EntityB != null)
		{
			Vector3.Subtract(ref info.Contact.Position, ref EntityB.a5h, out result);
			Vector3.Cross(ref EntityB.a5_0006, ref result, out result);
			Vector3.Add(ref result, ref EntityB.a5a, out result);
			Vector3.Subtract(ref info.RelativeVelocity, ref result, out info.RelativeVelocity);
		}
		info.Pair = this;
	}
}
internal class z : u
{
	private new i.X a5h = new i.X();

	protected override i.v MeshManifold => a5h;
}
