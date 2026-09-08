using System;
using System.Collections.Generic;
using _0004;
using _0010;
using BEPUphysics.DataStructures;
using J;
using L;
using P;
using T;
using Y;
using l;
using q;

namespace D;

internal abstract class X : b, B
{
	private new _0010._6 a5h;

	private Dictionary<P._6, b> a5b = new Dictionary<P._6, b>();

	private BEPUphysics.DataStructures.HashSet<P._6> a56 = new BEPUphysics.DataStructures.HashSet<P._6>();

	private l._7<P._6> a5a = new l._7<P._6>();

	private int a57;

	public l.v<P._6, b> ChildPairs => new l.v<P._6, b>(a5b);

	protected internal override int ContactCount => a57;

	public X()
	{
		a5h = new _0010._6();
	}

	public override void UpdateMaterialProperties(global::T._6 a, global::T._6 b)
	{
		foreach (b value in a5b.Values)
		{
			value.UpdateMaterialProperties(a, b);
		}
	}

	public override void UpdateMaterialProperties(global::T.b properties)
	{
		foreach (b value in a5b.Values)
		{
			value.UpdateMaterialProperties(properties);
		}
	}

	public override void Initialize(global::Y.a entryA, global::Y.a entryB)
	{
		a5h.Initialize(EntityA, EntityB);
		base.Initialize(entryA, entryB);
	}

	public override void CleanUp()
	{
		foreach (b value in a5b.Values)
		{
			value.CleanUp();
			value.Factory.GiveBack(value);
		}
		a5b.Clear();
		base.CleanUp();
	}

	protected void TryToAdd(P.h a, P.h b)
	{
		TryToAdd(a, b, null, null);
	}

	protected void TryToAdd(P.h a, P.h b, global::T._6 materialA)
	{
		TryToAdd(a, b, materialA, null);
	}

	protected void TryToAdd(P.h a, P.h b, global::T._6 materialA, global::T._6 materialB)
	{
		global::q.a a2;
		if ((a2 = global::q._7.a5v(((global::Y.a)a).a56, ((global::Y.a)b).a56)) >= global::q.a.NoNarrowPhasePair)
		{
			return;
		}
		if (a2 < base.CollisionRule)
		{
			a2 = base.CollisionRule;
		}
		P._6 pair = new P._6(a, b);
		if (!a5b.ContainsKey(pair))
		{
			b pairHandler = J.a.GetPairHandler(ref pair, a2);
			if (pairHandler != null)
			{
				pairHandler.UpdateMaterialProperties(materialA, materialB);
				pairHandler.Parent = this;
				a5b.Add(pair, pairHandler);
			}
		}
		a56.Add(pair);
	}

	protected abstract void UpdateContainedPairs();

	protected virtual void UpdateContacts(float dt)
	{
		UpdateContainedPairs();
		foreach (P._6 key in a5b.Keys)
		{
			if (!a56.Contains(key))
			{
				a5a.Add(key);
			}
		}
		for (int i = 0; i < a5a.a5h; i++)
		{
			b b2 = a5b[a5a.Elements[i]];
			a5b.Remove(a5a.Elements[i]);
			b2.CleanUp();
			b2.Factory.GiveBack(b2);
		}
		a56.Clear();
		a5a.Clear();
		foreach (b value in a5b.Values)
		{
			if (value.BroadPhaseOverlap.a56 < global::q.a.NoNarrowPhaseUpdate)
			{
				value.UpdateCollision(dt);
			}
		}
	}

	public override void UpdateCollision(float dt)
	{
		if (!suppressEvents)
		{
			CollidableA.EventTriggerer.OnPairUpdated(CollidableB, this);
			CollidableB.EventTriggerer.OnPairUpdated(CollidableA, this);
		}
		UpdateContacts(dt);
		if (a57 > 0)
		{
			if (!suppressEvents)
			{
				CollidableA.EventTriggerer.OnPairTouching(CollidableB, this);
				CollidableB.EventTriggerer.OnPairTouching(CollidableA, this);
			}
			if (previousContactCount == 0)
			{
				CollidableA.EventTriggerer.OnInitialCollisionDetected(CollidableB, this);
				CollidableB.EventTriggerer.OnInitialCollisionDetected(CollidableA, this);
			}
		}
		else if (previousContactCount > 0 && !suppressEvents)
		{
			CollidableA.EventTriggerer.OnCollisionEnded(CollidableB, this);
			CollidableB.EventTriggerer.OnCollisionEnded(CollidableA, this);
		}
		previousContactCount = a57;
	}

	public override void UpdateTimeOfImpact(P.h requester, float dt)
	{
		timeOfImpact = 1f;
		foreach (b value in a5b.Values)
		{
			if (base.BroadPhaseOverlap.a5h == requester)
			{
				value.UpdateTimeOfImpact((P.h)value.BroadPhaseOverlap.a5h, dt);
			}
			else
			{
				value.UpdateTimeOfImpact((P.h)value.BroadPhaseOverlap.a5b, dt);
			}
			if (value.timeOfImpact < timeOfImpact)
			{
				timeOfImpact = value.timeOfImpact;
			}
		}
	}

	protected internal override void GetContactInformation(int index, out _0001 info)
	{
		foreach (b value in a5b.Values)
		{
			int count = value.Contacts.Count;
			if (index - count < 0)
			{
				value.GetContactInformation(index, out info);
				return;
			}
			index -= count;
		}
		throw new IndexOutOfRangeException("Contact index is not present in the pair.");
	}

	void B.AddSolverUpdateable(L.h addedItem)
	{
		a5h.Add(addedItem);
		if (a5h.SolverUpdateables.Count == 1)
		{
			if (base.Parent != null)
			{
				base.Parent.AddSolverUpdateable(a5h);
			}
			else if (base.NarrowPhase != null)
			{
				base.NarrowPhase.NotifyUpdateableAdded(a5h);
			}
		}
	}

	void B.RemoveSolverUpdateable(L.h removedItem)
	{
		a5h.Remove(removedItem);
		if (a5h.SolverUpdateables.Count == 0)
		{
			if (base.Parent != null)
			{
				base.Parent.RemoveSolverUpdateable(a5h);
			}
			else if (base.NarrowPhase != null)
			{
				base.NarrowPhase.NotifyUpdateableRemoved(a5h);
			}
		}
	}

	void B.OnContactAdded(_0004.h contact)
	{
		a57++;
		OnContactAdded(contact);
	}

	void B.OnContactRemoved(_0004.h contact)
	{
		a57--;
		OnContactRemoved(contact);
	}

	public override void ClearContacts()
	{
		foreach (b value in a5b.Values)
		{
			value.ClearContacts();
		}
		base.ClearContacts();
	}
}
