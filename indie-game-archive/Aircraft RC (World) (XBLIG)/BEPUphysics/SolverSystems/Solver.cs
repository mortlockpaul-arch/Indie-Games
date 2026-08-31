using System;
using BEPUphysics.Constraints;
using BEPUphysics.DataStructures;
using BEPUphysics.DeactivationManagement;
using BEPUphysics.Threading;

namespace BEPUphysics.SolverSystems;

/// <summary>
///  Iteratively solves solver updateables, converging to a solution for simulated joints and collision pair contact constraints.
/// </summary>
public class Solver : MultithreadedProcessingStage
{
	private RawList<SolverUpdateable> solverUpdateables = new RawList<SolverUpdateable>();

	internal int iterationLimit = 10;

	protected internal TimeStepSettings timeStepSettings;

	private Action<int> multithreadedPrestepDelegate;

	private int primeIndex;

	private static long[] primes = new long[50]
	{
		472882049L, 492876847L, 492876863L, 512927357L, 512927377L, 533000389L, 533000401L, 553105243L, 553105253L, 573259391L,
		573259433L, 593441843L, 593441861L, 613651349L, 613651369L, 633910099L, 633910111L, 654188383L, 654188429L, 674506081L,
		674506111L, 694847533L, 694847539L, 715225739L, 715225741L, 735632791L, 735632797L, 756065159L, 756065179L, 776531401L,
		776531419L, 797003413L, 797003437L, 817504243L, 817504253L, 838041641L, 838041647L, 858599503L, 858599509L, 879190747L,
		879190841L, 899809343L, 899809363L, 920419813L, 920419823L, 941083981L, 941083987L, 961748927L, 961748941L, 982451653L
	};

	private long prime;

	private Action<int> multithreadedIterationDelegate;

	/// <summary>
	///  Gets or sets the maximum number of iterations the solver will attempt to use to solve the simulation's constraints.
	/// </summary>
	public int IterationLimit
	{
		get
		{
			return iterationLimit;
		}
		set
		{
			iterationLimit = Math.Max(value, 0);
		}
	}

	/// <summary>
	///  Gets the list of solver updateables in the solver.
	/// </summary>
	public ReadOnlyList<SolverUpdateable> SolverUpdateables => new ReadOnlyList<SolverUpdateable>(solverUpdateables);

	/// <summary>
	///  Gets or sets the time step settings used by the solver.
	/// </summary>
	public TimeStepSettings TimeStepSettings
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

	/// <summary>
	///  Gets or sets the deactivation manager used by the solver.
	///  When constraints are added and removed, the deactivation manager
	///  gains and loses simulation island connections that affect simulation islands
	///  and activity states.
	/// </summary>
	public DeactivationManager DeactivationManager { get; set; }

	/// <summary>
	/// Gets or sets the permutation index used by the solver.  If the simulation is restarting from a given frame,
	/// setting this index to be consistent is required for deterministic results.
	/// </summary>
	public int PermutationIndex
	{
		get
		{
			return primeIndex;
		}
		set
		{
			primeIndex = value % primes.Length;
		}
	}

	/// <summary>
	///  Constructs a Solver.
	/// </summary>
	/// <param name="timeStepSettings">Time step settings used by the solver.</param>
	/// <param name="deactivationManager">Deactivation manager used by the solver.</param>
	public Solver(TimeStepSettings timeStepSettings, DeactivationManager deactivationManager)
	{
		TimeStepSettings = timeStepSettings;
		DeactivationManager = deactivationManager;
		multithreadedPrestepDelegate = MultithreadedPrestep;
		multithreadedIterationDelegate = MultithreadedIteration;
		Enabled = true;
	}

	/// <summary>
	///  Constructs a Solver.
	/// </summary>
	/// <param name="timeStepSettings">Time step settings used by the solver.</param>
	/// <param name="deactivationManager">Deactivation manager used by the solver.</param>
	///  <param name="threadManager">Thread manager used by the solver.</param>
	public Solver(TimeStepSettings timeStepSettings, DeactivationManager deactivationManager, IThreadManager threadManager)
		: this(timeStepSettings, deactivationManager)
	{
		base.ThreadManager = threadManager;
		base.AllowMultithreading = true;
	}

	/// <summary>
	///  Adds a solver updateable to the solver.
	/// </summary>
	/// <param name="item">Updateable to add.</param>
	/// <exception cref="T:System.ArgumentException">Thrown when the item already belongs to a solver.</exception>
	public void Add(SolverUpdateable item)
	{
		if (item.Solver == null)
		{
			item.Solver = this;
			item.solverIndex = solverUpdateables.count;
			solverUpdateables.Add(item);
			DeactivationManager.Add(item.simulationIslandConnection);
			item.OnAdditionToSolver(this);
			return;
		}
		throw new ArgumentException("Solver updateable already belongs to something; it can't be added.", "item");
	}

	/// <summary>
	///  Removes a solver updateable from the solver.
	/// </summary>
	/// <param name="item">Updateable to remove.</param>
	/// <exception cref="T:System.ArgumentException">Thrown when the item does not belong to the solver.</exception>
	public void Remove(SolverUpdateable item)
	{
		if (item.Solver == this)
		{
			item.Solver = null;
			solverUpdateables.count--;
			if (item.solverIndex < solverUpdateables.count)
			{
				solverUpdateables.Elements[item.solverIndex] = solverUpdateables.Elements[solverUpdateables.count];
				solverUpdateables.Elements[item.solverIndex].solverIndex = item.solverIndex;
			}
			solverUpdateables.Elements[solverUpdateables.count] = null;
			DeactivationManager.Remove(item.simulationIslandConnection);
			item.OnRemovalFromSolver(this);
			return;
		}
		throw new ArgumentException("Solver updateable doesn't belong to this solver; it can't be removed.", "item");
	}

	private void MultithreadedPrestep(int i)
	{
		SolverUpdateable solverUpdateable = solverUpdateables.Elements[i];
		solverUpdateable.UpdateSolverActivity();
		if (solverUpdateable.isActiveInSolver)
		{
			solverUpdateable.SolverSettings.currentIterations = 0;
			solverUpdateable.SolverSettings.iterationsAtZeroImpulse = 0;
			solverUpdateable.Update(timeStepSettings.TimeStepDuration);
			solverUpdateable.EnterLock();
			try
			{
				solverUpdateable.ExclusiveUpdate();
			}
			finally
			{
				solverUpdateable.ExitLock();
			}
		}
	}

	private void ComputeIterationCoefficient()
	{
		prime = primes[primeIndex = (primeIndex + 1) % primes.Length];
	}

	private void MultithreadedIteration(int i)
	{
		SolverUpdateable solverUpdateable = solverUpdateables.Elements[i * prime % solverUpdateables.count];
		SolverSettings solverSettings = solverUpdateable.solverSettings;
		if (!solverUpdateable.isActiveInSolver)
		{
			return;
		}
		int num = -1;
		solverUpdateable.EnterLock();
		if (solverUpdateable.isActiveInSolver)
		{
			if (solverUpdateable.SolveIteration() < solverSettings.minimumImpulse)
			{
				solverSettings.iterationsAtZeroImpulse++;
				if (solverSettings.iterationsAtZeroImpulse > solverSettings.minimumIterations)
				{
					solverUpdateable.isActiveInSolver = false;
				}
			}
			else
			{
				solverSettings.iterationsAtZeroImpulse = 0;
			}
			num = solverSettings.currentIterations++;
		}
		solverUpdateable.ExitLock();
		if (num > iterationLimit || num > solverSettings.maximumIterations)
		{
			solverUpdateable.isActiveInSolver = false;
		}
	}

	protected override void UpdateMultithreaded()
	{
		base.ThreadManager.ForLoop(0, solverUpdateables.count, multithreadedPrestepDelegate);
		ComputeIterationCoefficient();
		base.ThreadManager.ForLoop(0, iterationLimit * solverUpdateables.count, multithreadedIterationDelegate);
	}

	protected override void UpdateSingleThreaded()
	{
		int count = solverUpdateables.count;
		for (int i = 0; i < count; i++)
		{
			UnsafePrestep(solverUpdateables.Elements[i]);
		}
		int num = iterationLimit * count;
		ComputeIterationCoefficient();
		for (int j = 0; j < num; j++)
		{
			UnsafeSolveIteration(solverUpdateables.Elements[j * prime % count]);
		}
	}

	protected internal void UnsafePrestep(SolverUpdateable updateable)
	{
		updateable.UpdateSolverActivity();
		if (updateable.isActiveInSolver)
		{
			SolverSettings solverSettings = updateable.solverSettings;
			solverSettings.currentIterations = 0;
			solverSettings.iterationsAtZeroImpulse = 0;
			updateable.Update(timeStepSettings.TimeStepDuration);
			updateable.ExclusiveUpdate();
		}
	}

	protected internal void UnsafeSolveIteration(SolverUpdateable updateable)
	{
		if (!updateable.isActiveInSolver)
		{
			return;
		}
		SolverSettings solverSettings = updateable.solverSettings;
		solverSettings.currentIterations++;
		if (solverSettings.currentIterations <= iterationLimit && solverSettings.currentIterations <= solverSettings.maximumIterations)
		{
			if (updateable.SolveIteration() < solverSettings.minimumImpulse)
			{
				solverSettings.iterationsAtZeroImpulse++;
				if (solverSettings.iterationsAtZeroImpulse > solverSettings.minimumIterations)
				{
					updateable.isActiveInSolver = false;
				}
			}
			else
			{
				solverSettings.iterationsAtZeroImpulse = 0;
			}
		}
		else
		{
			updateable.isActiveInSolver = false;
		}
	}
}
