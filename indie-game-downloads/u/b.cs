using System;
using System.Runtime.CompilerServices;
using L;
using Z;
using d;
using l;
using r;

namespace u;

internal class b : r.b
{
	private l._7<h> a5h = new l._7<h>();

	internal int a5b = 10;

	protected internal r.B timeStepSettings;

	private Action<int> a56;

	private int a5a;

	private static long[] a57 = new long[50]
	{
		472882049L, 492876847L, 492876863L, 512927357L, 512927377L, 533000389L, 533000401L, 553105243L, 553105253L, 573259391L,
		573259433L, 593441843L, 593441861L, 613651349L, 613651369L, 633910099L, 633910111L, 654188383L, 654188429L, 674506081L,
		674506111L, 694847533L, 694847539L, 715225739L, 715225741L, 735632791L, 735632797L, 756065159L, 756065179L, 776531401L,
		776531419L, 797003413L, 797003437L, 817504243L, 817504253L, 838041641L, 838041647L, 858599503L, 858599509L, 879190747L,
		879190841L, 899809343L, 899809363L, 920419813L, 920419823L, 941083981L, 941083987L, 961748927L, 961748941L, 982451653L
	};

	private long a5_0006;

	private Action<int> a5v;

	[CompilerGenerated]
	private Z._6 a5B;

	public int IterationLimit
	{
		get
		{
			return a5b;
		}
		set
		{
			a5b = Math.Max(value, 0);
		}
	}

	public l.X<h> SolverUpdateables => new l.X<h>(a5h);

	public r.B TimeStepSettings
	{
		get
		{
			return timeStepSettings;
		}
		set
		{
			timeStepSettings = value;
		}
	}

	public Z._6 DeactivationManager
	{
		[CompilerGenerated]
		get
		{
			return a5B;
		}
		[CompilerGenerated]
		set
		{
			a5B = value;
		}
	}

	public int PermutationIndex
	{
		get
		{
			return a5a;
		}
		set
		{
			a5a = value % a57.Length;
		}
	}

	public b(r.B timeStepSettings, Z._6 deactivationManager)
	{
		TimeStepSettings = timeStepSettings;
		DeactivationManager = deactivationManager;
		a56 = aX;
		a5v = aW;
		Enabled = true;
	}

	public b(r.B timeStepSettings, Z._6 deactivationManager, d.b threadManager)
		: this(timeStepSettings, deactivationManager)
	{
		base.ThreadManager = threadManager;
		base.AllowMultithreading = true;
	}

	public void Add(h item)
	{
		if (item.Solver == null)
		{
			item.Solver = this;
			item.a5h = a5h.a5h;
			a5h.Add(item);
			DeactivationManager.Add(item.simulationIslandConnection);
			item.OnAdditionToSolver(this);
			return;
		}
		throw new ArgumentException("Solver updateable already belongs to something; it can't be added.", "item");
	}

	public void Remove(h item)
	{
		if (item.Solver == this)
		{
			item.Solver = null;
			a5h.a5h--;
			if (item.a5h < a5h.a5h)
			{
				a5h.Elements[item.a5h] = a5h.Elements[a5h.a5h];
				a5h.Elements[item.a5h].a5h = item.a5h;
			}
			a5h.Elements[a5h.a5h] = null;
			DeactivationManager.Remove(item.simulationIslandConnection);
			item.OnRemovalFromSolver(this);
			return;
		}
		throw new ArgumentException("Solver updateable doesn't belong to this solver; it can't be removed.", "item");
	}

	private void aX(int P_0)
	{
		h h2 = a5h.Elements[P_0];
		h2.UpdateSolverActivity();
		if (h2.isActiveInSolver)
		{
			h2.SolverSettings.a5h = 0;
			h2.SolverSettings.a57 = 0;
			h2.Update(timeStepSettings.TimeStepDuration);
			h2.EnterLock();
			try
			{
				h2.ExclusiveUpdate();
			}
			finally
			{
				h2.ExitLock();
			}
		}
	}

	private void a_0018()
	{
		a5_0006 = a57[a5a = (a5a + 1) % a57.Length];
	}

	private void aW(int P_0)
	{
		h h2 = a5h.Elements[P_0 * a5_0006 % a5h.a5h];
		L.b solverSettings = h2.solverSettings;
		if (!h2.isActiveInSolver)
		{
			return;
		}
		int num = -1;
		h2.EnterLock();
		if (h2.isActiveInSolver)
		{
			if (h2.SolveIteration() < solverSettings.a5a)
			{
				solverSettings.a57++;
				if (solverSettings.a57 > solverSettings.a56)
				{
					h2.isActiveInSolver = false;
				}
			}
			else
			{
				solverSettings.a57 = 0;
			}
			num = solverSettings.a5h++;
		}
		h2.ExitLock();
		if (num > a5b || num > solverSettings.a5b)
		{
			h2.isActiveInSolver = false;
		}
	}

	protected override void UpdateMultithreaded()
	{
		base.ThreadManager.ForLoop(0, a5h.a5h, a56);
		a_0018();
		base.ThreadManager.ForLoop(0, a5b * a5h.a5h, a5v);
	}

	protected override void UpdateSingleThreaded()
	{
		int num = a5h.a5h;
		for (int i = 0; i < num; i++)
		{
			UnsafePrestep(a5h.Elements[i]);
		}
		int num2 = a5b * num;
		a_0018();
		for (int j = 0; j < num2; j++)
		{
			UnsafeSolveIteration(a5h.Elements[j * a5_0006 % num]);
		}
	}

	protected internal void UnsafePrestep(h updateable)
	{
		updateable.UpdateSolverActivity();
		if (updateable.isActiveInSolver)
		{
			L.b solverSettings = updateable.solverSettings;
			solverSettings.a5h = 0;
			solverSettings.a57 = 0;
			updateable.Update(timeStepSettings.TimeStepDuration);
			updateable.ExclusiveUpdate();
		}
	}

	protected internal void UnsafeSolveIteration(h updateable)
	{
		if (!updateable.isActiveInSolver)
		{
			return;
		}
		L.b solverSettings = updateable.solverSettings;
		solverSettings.a5h++;
		if (solverSettings.a5h <= a5b && solverSettings.a5h <= solverSettings.a5b)
		{
			if (updateable.SolveIteration() < solverSettings.a5a)
			{
				solverSettings.a57++;
				if (solverSettings.a57 > solverSettings.a56)
				{
					updateable.isActiveInSolver = false;
				}
			}
			else
			{
				solverSettings.a57 = 0;
			}
		}
		else
		{
			updateable.isActiveInSolver = false;
		}
	}
}
