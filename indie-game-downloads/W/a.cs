using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using d;
using r;
using s;

namespace W;

internal class a : r.b
{
	private List<s.h> a5h = new List<s.h>();

	private r.B a5b;

	private Action<int> a56;

	[CompilerGenerated]
	private r.B a5a;

	public r.B TimeStepSettings
	{
		[CompilerGenerated]
		get
		{
			return a5a;
		}
		[CompilerGenerated]
		set
		{
			a5a = value;
		}
	}

	public a(r.B timeStepSettings)
	{
		a56 = aa;
		Enabled = true;
		a5b = timeStepSettings;
	}

	public a(r.B timeStepSettings, d.b threadManager)
		: this(timeStepSettings)
	{
		base.ThreadManager = threadManager;
		base.AllowMultithreading = true;
	}

	private void aa(int P_0)
	{
		if (a5h[P_0].IsActive)
		{
			a5h[P_0].UpdateBoundingBox(a5b.TimeStepDuration);
		}
	}

	public void Add(s.h entry)
	{
		a5h.Add(entry);
	}

	public void Remove(s.h entry)
	{
		a5h.Remove(entry);
	}

	protected override void UpdateMultithreaded()
	{
		base.ThreadManager.ForLoop(0, a5h.Count, a56);
	}

	protected override void UpdateSingleThreaded()
	{
		foreach (s.h item in a5h)
		{
			if (item.IsActive)
			{
				item.UpdateBoundingBox(a5b.TimeStepDuration);
			}
		}
	}
}
