using System;
using E;
using P;
using Y;
using l;
using p;
using s;

namespace D;

internal class W : D._0018
{
	private new s.v a5h;

	protected override P.h CollidableB => a5h;

	protected override E.h EntityB => a5h.entity;

	public override void Initialize(global::Y.a entryA, global::Y.a entryB)
	{
		a5h = entryA as s.v;
		if (a5h == null)
		{
			a5h = entryB as s.v;
			if (a5h == null)
			{
				throw new Exception("Inappropriate types used to initialize pair.");
			}
		}
		base.Initialize(entryA, entryB);
	}

	public override void CleanUp()
	{
		base.CleanUp();
		a5h = null;
	}

	protected override void UpdateContainedPairs()
	{
		l._7<s._7> compoundChildList = p._6.GetCompoundChildList();
		compoundInfo.a5b.Tree.GetOverlaps(a5h.boundingBox, compoundChildList);
		for (int i = 0; i < compoundChildList.a5h; i++)
		{
			TryToAdd(compoundChildList.Elements[i].CollisionInformation, CollidableB, compoundChildList.Elements[i].Material);
		}
		p._6.GiveBack(compoundChildList);
	}
}
