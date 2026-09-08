using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using D;
using Y;
using d;
using l;
using q;
using r;
using u;

namespace J;

internal class b : r.b
{
	private l._7<Y._7> a5h;

	private Dictionary<Y._7, D.h> a5b = new Dictionary<Y._7, D.h>();

	private l._7<D.h> a56 = new l._7<D.h>();

	private d.h<D.h> a5a = new d.h<D.h>();

	private Action<int> a57;

	private Action<D.h> a5_0006;

	private Action<D.h> a5v;

	private d.h<u._6> a5B = new d.h<u._6>();

	[CompilerGenerated]
	private r.B a5X;

	[CompilerGenerated]
	private u.b a5_0018;

	public l._7<Y._7> BroadPhaseOverlaps
	{
		get
		{
			return a5h;
		}
		set
		{
			a5h = value;
		}
	}

	public l.X<D.h> Pairs => new l.X<D.h>(a56);

	public r.B TimeStepSettings
	{
		[CompilerGenerated]
		get
		{
			return a5X;
		}
		[CompilerGenerated]
		set
		{
			a5X = value;
		}
	}

	public u.b Solver
	{
		[CompilerGenerated]
		get
		{
			return a5_0018;
		}
		[CompilerGenerated]
		set
		{
			a5_0018 = value;
		}
	}

	public event Action<D.h> CreatingPair
	{
		add
		{
			Action<D.h> action = a5_0006;
			Action<D.h> action2;
			do
			{
				action2 = action;
				Action<D.h> value2 = (Action<D.h>)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange(ref a5_0006, value2, action2);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<D.h> action = a5_0006;
			Action<D.h> action2;
			do
			{
				action2 = action;
				Action<D.h> value2 = (Action<D.h>)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange(ref a5_0006, value2, action2);
			}
			while ((object)action != action2);
		}
	}

	public event Action<D.h> RemovingPair
	{
		add
		{
			Action<D.h> action = a5v;
			Action<D.h> action2;
			do
			{
				action2 = action;
				Action<D.h> value2 = (Action<D.h>)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange(ref a5v, value2, action2);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<D.h> action = a5v;
			Action<D.h> action2;
			do
			{
				action2 = action;
				Action<D.h> value2 = (Action<D.h>)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange(ref a5v, value2, action2);
			}
			while ((object)action != action2);
		}
	}

	public b(r.B timeStepSettings)
	{
		TimeStepSettings = timeStepSettings;
		a57 = _6K;
		Enabled = true;
	}

	public b(r.B timeStepSettings, l._7<Y._7> overlaps)
		: this(timeStepSettings)
	{
		a5h = overlaps;
	}

	public b(r.B timeStepSettings, l._7<Y._7> overlaps, d.b threadManager)
		: this(timeStepSettings, overlaps)
	{
		base.ThreadManager = threadManager;
		base.AllowMultithreading = true;
	}

	private void _6K(int P_0)
	{
		Y._7 pair = a5h.Elements[P_0];
		if (pair.a56 >= q.a.NoNarrowPhasePair)
		{
			return;
		}
		if (!a5b.TryGetValue(pair, out var value))
		{
			value = a.GetPair(ref pair);
			if (value != null)
			{
				value.NarrowPhase = this;
				a5a.Enqueue(value);
			}
		}
		if (value != null)
		{
			if (value.BroadPhaseOverlap.a56 < q.a.NoNarrowPhaseUpdate)
			{
				value.UpdateCollision(TimeStepSettings.TimeStepDuration);
			}
			value.NeedsUpdate = false;
		}
	}

	protected override void UpdateMultithreaded()
	{
		base.ThreadManager.ForLoop(0, a5h.Count, a57);
		_6O();
		_63();
	}

	protected override void UpdateSingleThreaded()
	{
		int count = a5h.Count;
		for (int i = 0; i < count; i++)
		{
			_6K(i);
		}
		_6O();
		_63();
	}

	private void _6O()
	{
		for (int num = a56.a5h - 1; num >= 0; num--)
		{
			D.h h2 = a56.Elements[num];
			if (h2.NeedsUpdate && (h2.BroadPhaseOverlap.a5h.IsActive || h2.BroadPhaseOverlap.a5b.IsActive))
			{
				a56.FastRemoveAt(num);
				OnRemovePair(h2);
			}
			else
			{
				h2.NeedsUpdate = true;
			}
		}
	}

	private void _63()
	{
		D.h item;
		while (a5a.TryUnsafeDequeueFirst(out item))
		{
			a56.Add(item);
			OnCreatePair(item);
		}
	}

	public D.h GetPair(Y.a entryA, Y.a entryB)
	{
		a5b.TryGetValue(new Y._7(entryA, entryB), out var value);
		return value;
	}

	protected void OnCreatePair(D.h pair)
	{
		a5b.Add(pair.BroadPhaseOverlap, pair);
		pair.OnAddedToNarrowPhase();
		if (a5_0006 != null)
		{
			a5_0006(pair);
		}
	}

	protected void OnRemovePair(D.h pair)
	{
		a5b.Remove(pair.BroadPhaseOverlap);
		pair.CleanUp();
		pair.Factory.GiveBack(pair);
		if (a5v != null)
		{
			a5v(pair);
		}
	}

	public void NotifyUpdateableAdded(u.h addedItem)
	{
		a5B.Enqueue(new u._6(shouldAdd: true, addedItem));
	}

	public void NotifyUpdateableRemoved(u.h removedItem)
	{
		a5B.Enqueue(new u._6(shouldAdd: false, removedItem));
	}

	public void FlushGeneratedSolverUpdateables()
	{
		u._6 item;
		while (a5B.TryUnsafeDequeueFirst(out item))
		{
			if (item.ShouldAdd)
			{
				if (item.Item.solver == null)
				{
					Solver.Add(item.Item);
				}
			}
			else if (item.Item.solver != null)
			{
				Solver.Remove(item.Item);
			}
		}
	}
}
