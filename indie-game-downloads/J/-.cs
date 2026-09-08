using System;
using D;
using p;

namespace J;

internal class _0006<T> : _7 where T : D.h, new()
{
	private p.b<T> a5h = new p.b<T>();

	public override int Count
	{
		get
		{
			return a5h.Count;
		}
		set
		{
			a5h.Initialize(value);
		}
	}

	public override D.h GetNarrowPhasePair()
	{
		if (!allowOnDemandConstruction && a5h.Count == 0)
		{
			throw new Exception("Cannot request additional resources from this factory; it is exhausted.  Consider specifying a greater number of initial resources or setting AllowOnDemandConstruction to true.");
		}
		return a5h.Take();
	}

	public override void GiveBack(D.h pair)
	{
		a5h.GiveBack((T)pair);
	}

	public override void Clear()
	{
		a5h.Clear();
	}
}
