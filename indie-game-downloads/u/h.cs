using System.Runtime.CompilerServices;
using L;
using Z;
using p;
using r;

namespace u;

internal abstract class h : Z.h, r.h
{
	internal int a5h;

	protected internal b solver;

	protected internal L.b solverSettings = new L.b();

	protected internal bool isActive = true;

	protected internal bool isActiveInSolver = true;

	protected internal r.a space;

	protected internal Z._7 simulationIslandConnection;

	[CompilerGenerated]
	private object a5b;

	public virtual b Solver
	{
		get
		{
			return solver;
		}
		internal set
		{
			solver = b2;
		}
	}

	public L.b SolverSettings => solverSettings;

	public bool IsActive
	{
		get
		{
			return isActive;
		}
		set
		{
			isActive = value;
		}
	}

	public bool IsActiveInSolver => isActiveInSolver;

	r.a r.h.Space
	{
		get
		{
			return space;
		}
		set
		{
			space = value;
		}
	}

	public object Tag
	{
		[CompilerGenerated]
		get
		{
			return a5b;
		}
		[CompilerGenerated]
		set
		{
			a5b = value;
		}
	}

	public Z._7 SimulationIslandConnection => simulationIslandConnection;

	protected h()
	{
		simulationIslandConnection = p._6.GetSimulationIslandConnection();
		simulationIslandConnection.Owner = this;
	}

	public abstract void Update(float dt);

	public abstract void ExclusiveUpdate();

	public abstract float SolveIteration();

	public abstract bool TryEnterLock();

	public abstract void EnterLock();

	public abstract void ExitLock();

	public virtual void UpdateSolverActivity()
	{
		if (isActive)
		{
			for (int i = 0; i < simulationIslandConnection.a5h.a5h; i++)
			{
				Z.a simulationIsland = simulationIslandConnection.a5h.Elements[i].SimulationIsland;
				if (simulationIsland != null && simulationIsland.a56)
				{
					isActiveInSolver = true;
					return;
				}
			}
		}
		isActiveInSolver = false;
	}

	public virtual void OnAdditionToSpace(r.a newSpace)
	{
	}

	public virtual void OnRemovalFromSpace(r.a oldSpace)
	{
	}

	public virtual void OnAdditionToSolver(b newSolver)
	{
	}

	public virtual void OnRemovalFromSolver(b oldSolver)
	{
	}
}
