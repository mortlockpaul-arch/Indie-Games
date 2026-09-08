using System;
using d;
using l;
using r;

namespace m;

internal class a : _6
{
	private l._7<h> a5h = new l._7<h>();

	private l._7<b> a5b = new l._7<b>();

	private l._7<b> a56 = new l._7<b>();

	public static int MultithreadingThreshold = 100;

	private Action<int> a5a;

	private Action<int> a57;

	private Action<int> a5_0006;

	public a(r.B timeStepSettings)
		: base(timeStepSettings)
	{
		a5a = a_0006;
		a57 = av;
		a5_0006 = aB;
	}

	public a(r.B timeStepSettings, d.b threadManager)
		: base(timeStepSettings, threadManager)
	{
		a5a = a_0006;
		a57 = av;
		a5_0006 = aB;
	}

	private void a_0006(int P_0)
	{
		if (P_0 >= a5h.a5h)
		{
			P_0 -= a5h.a5h;
			if (P_0 >= a5b.a5h)
			{
				P_0 -= a5b.a5h;
				if (a56.Elements[P_0].IsActive)
				{
					a56.Elements[P_0].PreUpdatePosition(timeStepSettings.TimeStepDuration);
				}
			}
			else if (a5b.Elements[P_0].IsActive)
			{
				a5b.Elements[P_0].PreUpdatePosition(timeStepSettings.TimeStepDuration);
			}
		}
		else if (a5h.Elements[P_0].IsActive)
		{
			a5h.Elements[P_0].PreUpdatePosition(timeStepSettings.TimeStepDuration);
		}
	}

	private void av(int P_0)
	{
		a56.Elements[P_0].UpdateTimeOfImpacts(timeStepSettings.TimeStepDuration);
	}

	private void aB(int P_0)
	{
		if (P_0 < a5b.a5h)
		{
			if (a5b.Elements[P_0].IsActive)
			{
				a5b.Elements[P_0].UpdatePositionContinuously(timeStepSettings.TimeStepDuration);
			}
		}
		else if (a56.Elements[P_0 - a5b.a5h].IsActive)
		{
			a56.Elements[P_0 - a5b.a5h].UpdatePositionContinuously(timeStepSettings.TimeStepDuration);
		}
	}

	protected override void UpdateMultithreaded()
	{
		int endIndex = a5h.a5h + a5b.a5h + a56.a5h;
		base.ThreadManager.ForLoop(0, endIndex, a5a);
		if (a56.a5h > MultithreadingThreshold)
		{
			base.ThreadManager.ForLoop(0, a56.a5h, a57);
		}
		else
		{
			for (int i = 0; i < a56.a5h; i++)
			{
				av(i);
			}
		}
		endIndex = a5b.a5h + a56.a5h;
		if (endIndex > MultithreadingThreshold)
		{
			base.ThreadManager.ForLoop(0, endIndex, a5_0006);
			return;
		}
		for (int j = 0; j < endIndex; j++)
		{
			aB(j);
		}
	}

	protected override void UpdateSingleThreaded()
	{
		int num = a5h.a5h + a5b.a5h + a56.a5h;
		for (int i = 0; i < num; i++)
		{
			a_0006(i);
		}
		for (int j = 0; j < a56.a5h; j++)
		{
			av(j);
		}
		num = a5b.a5h + a56.a5h;
		for (int k = 0; k < num; k++)
		{
			aB(k);
		}
	}

	public void UpdateableModeChanged(b updateable, _7 previousMode)
	{
		switch (previousMode)
		{
		case _7.Discrete:
			a5h.Remove(updateable);
			break;
		case _7.Passive:
			a5b.Remove(updateable);
			break;
		case _7.Continuous:
			a56.Remove(updateable);
			break;
		}
		switch (updateable.PositionUpdateMode)
		{
		case _7.Discrete:
			a5h.Add(updateable);
			break;
		case _7.Passive:
			a5b.Add(updateable);
			break;
		case _7.Continuous:
			a56.Add(updateable);
			break;
		}
	}

	public override void Add(h updateable)
	{
		if (updateable.PositionUpdater == null)
		{
			updateable.PositionUpdater = this;
			if (updateable is b b2)
			{
				switch (b2.PositionUpdateMode)
				{
				case _7.Discrete:
					a5h.Add(updateable);
					break;
				case _7.Passive:
					a5b.Add(b2);
					break;
				case _7.Continuous:
					a56.Add(b2);
					break;
				}
			}
			else
			{
				a5h.Add(updateable);
			}
			return;
		}
		throw new Exception("Cannot add object to Integrator; it already belongs to one.");
	}

	public override void Remove(h updateable)
	{
		if (updateable.PositionUpdater == this)
		{
			updateable.PositionUpdater = null;
			if (updateable is b b2)
			{
				switch (b2.PositionUpdateMode)
				{
				case _7.Discrete:
					a5h.Remove(updateable);
					break;
				case _7.Passive:
					a5b.Remove(b2);
					break;
				case _7.Continuous:
					a56.Remove(b2);
					break;
				}
			}
			else
			{
				a5h.Remove(updateable);
			}
			return;
		}
		throw new Exception("Cannot remove object from this Integrator.  The object doesn't belong to it.");
	}
}
