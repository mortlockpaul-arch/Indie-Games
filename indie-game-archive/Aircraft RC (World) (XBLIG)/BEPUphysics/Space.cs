using System;
using System.Collections.Generic;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseSystems;
using BEPUphysics.BroadPhaseSystems.Hierarchies;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.DataStructures;
using BEPUphysics.DeactivationManagement;
using BEPUphysics.Entities;
using BEPUphysics.EntityStateManagement;
using BEPUphysics.NarrowPhaseSystems;
using BEPUphysics.OtherSpaceStages;
using BEPUphysics.PositionUpdating;
using BEPUphysics.ResourceManagement;
using BEPUphysics.SolverSystems;
using BEPUphysics.Threading;
using BEPUphysics.UpdateableSystems;
using Microsoft.Xna.Framework;

namespace BEPUphysics;

/// <summary>
///  Main simulation class of BEPUphysics.  Contains various updating stages addition/removal methods for getting objects into the simulation.
/// </summary>
public class Space : ISpace, IDisposable
{
	private TimeStepSettings timeStepSettings;

	private IThreadManager threadManager;

	private BroadPhase broadPhase;

	private bool disposed;

	/// <summary>
	///  Gets or sets the time step settings used by the space.
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
			DeactivationManager.TimeStepSettings = value;
			ForceUpdater.TimeStepSettings = value;
			BoundingBoxUpdater.TimeStepSettings = value;
			Solver.TimeStepSettings = value;
			PositionUpdater.TimeStepSettings = value;
		}
	}

	/// <summary>
	///  Gets or sets the thread manager used by the space.
	/// </summary>
	public IThreadManager ThreadManager
	{
		get
		{
			return threadManager;
		}
		set
		{
			threadManager = value;
			DeactivationManager.ThreadManager = value;
			ForceUpdater.ThreadManager = value;
			BoundingBoxUpdater.ThreadManager = value;
			BroadPhase.ThreadManager = value;
			NarrowPhase.ThreadManager = value;
			Solver.ThreadManager = value;
			PositionUpdater.ThreadManager = value;
			DuringForcesUpdateables.ThreadManager = value;
			BeforeNarrowPhaseUpdateables.ThreadManager = value;
			EndOfTimeStepUpdateables.ThreadManager = value;
			EndOfFrameUpdateables.ThreadManager = value;
		}
	}

	/// <summary>
	///  Gets or sets the space object buffer used by the space.
	///  The space object buffer allows objects to be safely asynchronously
	///  added to and removed from the space.
	/// </summary>
	public SpaceObjectBuffer SpaceObjectBuffer { get; set; }

	/// <summary>
	///  Gets or sets the entity state write buffer used by the space.
	///  The write buffer contains buffered writes to entity states that are
	///  flushed each frame when the buffer is updated.
	/// </summary>
	public EntityStateWriteBuffer EntityStateWriteBuffer { get; set; }

	/// <summary>
	///  Gets or sets the deactivation manager used by the space.
	///  The deactivation manager controls the activity state objects, putting them
	///  to sleep and managing the connections between objects and simulation islands.
	/// </summary>
	public DeactivationManager DeactivationManager { get; set; }

	/// <summary>
	///  Gets or sets the force updater used by the space.
	///  The force updater applies forces to all dynamic objects in the space each frame.
	/// </summary>
	public ForceUpdater ForceUpdater { get; set; }

	/// <summary>
	///  Gets or sets the bounding box updater used by the space.
	///  The bounding box updater updates the bounding box of mobile collidables each frame.
	/// </summary>
	public BoundingBoxUpdater BoundingBoxUpdater { get; set; }

	/// <summary>
	/// Gets or sets the broad phase used by the space.
	/// The broad phase finds overlaps between broad phase entries and passes
	/// them off to the narrow phase for processing.
	/// </summary>
	public BroadPhase BroadPhase
	{
		get
		{
			return broadPhase;
		}
		set
		{
			broadPhase = value;
			if (NarrowPhase != null)
			{
				if (value != null)
				{
					NarrowPhase.BroadPhaseOverlaps = broadPhase.Overlaps;
				}
				else
				{
					NarrowPhase.BroadPhaseOverlaps = null;
				}
			}
		}
	}

	/// <summary>
	///  Gets or sets the narrow phase used by the space.
	///  The narrow phase uses overlaps found by the broad phase
	///  to create pair handlers.  Those pair handlers can go on to 
	///  create things like contacts and constraints.
	/// </summary>
	public NarrowPhase NarrowPhase { get; set; }

	/// <summary>
	///  Gets or sets the solver used by the space.
	///  The solver iteratively finds a solution to the constraints in the simulation.
	/// </summary>
	public Solver Solver { get; set; }

	/// <summary>
	///  Gets or sets the position updater used by the space.
	///  The position updater moves everything around each frame.
	/// </summary>
	public PositionUpdater PositionUpdater { get; set; }

	/// <summary>
	///  Gets or sets the buffered states manager used by the space.
	///  The buffered states manager keeps track of read buffered entity states
	///  and also interpolated states based on the time remaining from internal
	///  time steps.
	/// </summary>
	public BufferedStatesManager BufferedStates { get; set; }

	/// <summary>
	///  Gets or sets the deferred event dispatcher used by the space.
	///  The event dispatcher gathers up deferred events created
	///  over the course of a timestep and dispatches them sequentially at the end.
	/// </summary>
	public DeferredEventDispatcher DeferredEventDispatcher { get; set; }

	/// <summary>
	///  Gets or sets the updateable manager that handles updateables that update during force application.
	/// </summary>
	public DuringForcesUpdateableManager DuringForcesUpdateables { get; set; }

	/// <summary>
	///  Gets or sets the updateable manager that handles updateables that update before the narrow phase.
	/// </summary>
	public BeforeNarrowPhaseUpdateableManager BeforeNarrowPhaseUpdateables { get; set; }

	/// <summary>
	///  Gets or sets the updateable manager that handles updateables that update before the solver
	/// </summary>
	public BeforeSolverUpdateableManager BeforeSolverUpdateables { get; set; }

	/// <summary>
	///  Gets or sets the updateable manager that handles updateables that update right before the position update phase.
	/// </summary>
	public BeforePositionUpdateUpdateableManager BeforePositionUpdateUpdateables { get; set; }

	/// <summary>
	///  Gets or sets the updateable manager that handles updateables that update at the end of a timestep.
	/// </summary>
	public EndOfTimeStepUpdateableManager EndOfTimeStepUpdateables { get; set; }

	/// <summary>
	///  Gets or sets the updateable manager that handles updateables that update at the end of a frame.
	/// </summary>
	public EndOfFrameUpdateableManager EndOfFrameUpdateables { get; set; }

	/// <summary>
	///  Gets the list of entities in the space.
	/// </summary>
	public ReadOnlyList<Entity> Entities => BufferedStates.Entities;

	/// <summary>
	///  Constructs a new space for things to live in.
	///  Uses the SpecializedThreadManager.
	/// </summary>
	public Space()
		: this(new SpecializedThreadManager())
	{
	}

	/// <summary>
	///  Constructs a new space for things to live in.
	/// </summary>
	/// <param name="threadManager">Thread manager to use with the space.</param>
	public Space(IThreadManager threadManager)
	{
		timeStepSettings = new TimeStepSettings();
		this.threadManager = threadManager;
		SpaceObjectBuffer = new SpaceObjectBuffer(this);
		EntityStateWriteBuffer = new EntityStateWriteBuffer();
		DeactivationManager = new DeactivationManager(TimeStepSettings, ThreadManager);
		ForceUpdater = new ForceUpdater(TimeStepSettings, ThreadManager);
		BoundingBoxUpdater = new BoundingBoxUpdater(TimeStepSettings, ThreadManager);
		BroadPhase = new DynamicHierarchy(ThreadManager);
		NarrowPhase = new NarrowPhase(TimeStepSettings, BroadPhase.Overlaps, ThreadManager);
		Solver = new Solver(TimeStepSettings, DeactivationManager, ThreadManager);
		NarrowPhase.Solver = Solver;
		PositionUpdater = new ContinuousPositionUpdater(TimeStepSettings, ThreadManager);
		BufferedStates = new BufferedStatesManager(ThreadManager);
		DeferredEventDispatcher = new DeferredEventDispatcher();
		DuringForcesUpdateables = new DuringForcesUpdateableManager(timeStepSettings, ThreadManager);
		BeforeNarrowPhaseUpdateables = new BeforeNarrowPhaseUpdateableManager(timeStepSettings, ThreadManager);
		BeforeSolverUpdateables = new BeforeSolverUpdateableManager(timeStepSettings, ThreadManager);
		BeforePositionUpdateUpdateables = new BeforePositionUpdateUpdateableManager(timeStepSettings, ThreadManager);
		EndOfTimeStepUpdateables = new EndOfTimeStepUpdateableManager(timeStepSettings, ThreadManager);
		EndOfFrameUpdateables = new EndOfFrameUpdateableManager(timeStepSettings, ThreadManager);
	}

	/// <summary>
	///  Adds a space object to the simulation.
	/// </summary>
	/// <param name="spaceObject">Space object to add.</param>
	public void Add(ISpaceObject spaceObject)
	{
		if (spaceObject.Space != null)
		{
			throw new ArgumentException("The object belongs to some Space already; cannot add it again.");
		}
		spaceObject.Space = this;
		if (spaceObject is SimulationIslandMember simulationIslandMember)
		{
			DeactivationManager.Add(simulationIslandMember);
		}
		if (spaceObject is ISimulationIslandMemberOwner simulationIslandMemberOwner)
		{
			DeactivationManager.Add(simulationIslandMemberOwner.ActivityInformation);
		}
		if (spaceObject is IForceUpdateable forceUpdateable)
		{
			ForceUpdater.Add(forceUpdateable);
		}
		if (spaceObject is MobileCollidable entry)
		{
			BoundingBoxUpdater.Add(entry);
		}
		if (spaceObject is BroadPhaseEntry entry2)
		{
			BroadPhase.Add(entry2);
		}
		if (spaceObject is IBroadPhaseEntryOwner broadPhaseEntryOwner)
		{
			BroadPhase.Add(broadPhaseEntryOwner.Entry);
			if (broadPhaseEntryOwner.Entry is MobileCollidable entry3)
			{
				BoundingBoxUpdater.Add(entry3);
			}
		}
		if (spaceObject is SolverUpdateable item)
		{
			Solver.Add(item);
		}
		if (spaceObject is IPositionUpdateable updateable)
		{
			PositionUpdater.Add(updateable);
		}
		if (spaceObject is Entity e)
		{
			BufferedStates.Add(e);
		}
		if (spaceObject is IDeferredEventCreator creator)
		{
			DeferredEventDispatcher.AddEventCreator(creator);
		}
		if (spaceObject is IDeferredEventCreatorOwner deferredEventCreatorOwner)
		{
			DeferredEventDispatcher.AddEventCreator(deferredEventCreatorOwner.EventCreator);
		}
		if (spaceObject is IDuringForcesUpdateable updateable2)
		{
			DuringForcesUpdateables.Add(updateable2);
		}
		if (spaceObject is IBeforeNarrowPhaseUpdateable updateable3)
		{
			BeforeNarrowPhaseUpdateables.Add(updateable3);
		}
		if (spaceObject is IBeforeSolverUpdateable updateable4)
		{
			BeforeSolverUpdateables.Add(updateable4);
		}
		if (spaceObject is IBeforePositionUpdateUpdateable updateable5)
		{
			BeforePositionUpdateUpdateables.Add(updateable5);
		}
		if (spaceObject is IEndOfTimeStepUpdateable updateable6)
		{
			EndOfTimeStepUpdateables.Add(updateable6);
		}
		if (spaceObject is IEndOfFrameUpdateable updateable7)
		{
			EndOfFrameUpdateables.Add(updateable7);
		}
		spaceObject.OnAdditionToSpace(this);
	}

	/// <summary>
	///  Removes a space object from the simulation.
	/// </summary>
	/// <param name="spaceObject">Space object to remove.</param>
	public void Remove(ISpaceObject spaceObject)
	{
		if (spaceObject.Space != this)
		{
			throw new ArgumentException("The object does not belong to this space; cannot remove it.");
		}
		if (spaceObject is SimulationIslandMember simulationIslandMember)
		{
			DeactivationManager.Remove(simulationIslandMember);
		}
		if (spaceObject is ISimulationIslandMemberOwner simulationIslandMemberOwner)
		{
			DeactivationManager.Remove(simulationIslandMemberOwner.ActivityInformation);
		}
		if (spaceObject is IForceUpdateable forceUpdateable)
		{
			ForceUpdater.Remove(forceUpdateable);
		}
		if (spaceObject is MobileCollidable entry)
		{
			BoundingBoxUpdater.Remove(entry);
		}
		if (spaceObject is BroadPhaseEntry entry2)
		{
			BroadPhase.Remove(entry2);
		}
		if (spaceObject is IBroadPhaseEntryOwner broadPhaseEntryOwner)
		{
			BroadPhase.Remove(broadPhaseEntryOwner.Entry);
			if (broadPhaseEntryOwner.Entry is MobileCollidable entry3)
			{
				BoundingBoxUpdater.Remove(entry3);
			}
		}
		if (spaceObject is SolverUpdateable item)
		{
			Solver.Remove(item);
		}
		if (spaceObject is IPositionUpdateable updateable)
		{
			PositionUpdater.Remove(updateable);
		}
		if (spaceObject is Entity e)
		{
			BufferedStates.Remove(e);
		}
		if (spaceObject is IDeferredEventCreator creator)
		{
			DeferredEventDispatcher.RemoveEventCreator(creator);
		}
		if (spaceObject is IDeferredEventCreatorOwner deferredEventCreatorOwner)
		{
			DeferredEventDispatcher.RemoveEventCreator(deferredEventCreatorOwner.EventCreator);
		}
		if (spaceObject is IDuringForcesUpdateable updateable2)
		{
			DuringForcesUpdateables.Remove(updateable2);
		}
		if (spaceObject is IBeforeNarrowPhaseUpdateable updateable3)
		{
			BeforeNarrowPhaseUpdateables.Remove(updateable3);
		}
		if (spaceObject is IBeforeSolverUpdateable updateable4)
		{
			BeforeSolverUpdateables.Remove(updateable4);
		}
		if (spaceObject is IBeforePositionUpdateUpdateable updateable5)
		{
			BeforePositionUpdateUpdateables.Remove(updateable5);
		}
		if (spaceObject is IEndOfTimeStepUpdateable updateable6)
		{
			EndOfTimeStepUpdateables.Remove(updateable6);
		}
		if (spaceObject is IEndOfFrameUpdateable updateable7)
		{
			EndOfFrameUpdateables.Remove(updateable7);
		}
		spaceObject.Space = null;
		spaceObject.OnRemovalFromSpace(this);
	}

	private void DoTimeStep()
	{
		SpaceObjectBuffer.Update();
		EntityStateWriteBuffer.Update();
		DeactivationManager.Update();
		ForceUpdater.Update();
		DuringForcesUpdateables.Update();
		BoundingBoxUpdater.Update();
		BroadPhase.Update();
		BeforeNarrowPhaseUpdateables.Update();
		NarrowPhase.Update();
		BeforeSolverUpdateables.Update();
		Solver.Update();
		BeforePositionUpdateUpdateables.Update();
		PositionUpdater.Update();
		BufferedStates.ReadBuffers.Update();
		DeferredEventDispatcher.Update();
		EndOfTimeStepUpdateables.Update();
	}

	/// <summary>
	///  Performs a single timestep.
	/// </summary>
	public void Update()
	{
		DoTimeStep();
		EndOfFrameUpdateables.Update();
	}

	/// <summary>
	/// Performs as many timesteps as necessary to get as close to the elapsed time as possible.
	/// </summary>
	/// <param name="dt">Elapsed time from the previous frame.</param>
	public void Update(float dt)
	{
		TimeStepSettings.AccumulatedTime += dt;
		for (int i = 0; i < TimeStepSettings.MaximumTimeStepsPerFrame; i++)
		{
			if (!(TimeStepSettings.AccumulatedTime >= TimeStepSettings.TimeStepDuration))
			{
				break;
			}
			TimeStepSettings.AccumulatedTime -= TimeStepSettings.TimeStepDuration;
			DoTimeStep();
		}
		BufferedStates.InterpolatedStates.BlendAmount = TimeStepSettings.AccumulatedTime / TimeStepSettings.TimeStepDuration;
		BufferedStates.InterpolatedStates.Update();
		EndOfFrameUpdateables.Update();
	}

	/// <summary>
	/// Tests a ray against the space.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	/// <param name="result">Hit data of the ray, if any.</param>
	/// <returns>Whether or not the ray hit anything.</returns>
	public bool RayCast(Ray ray, out RayCastResult result)
	{
		return RayCast(ray, float.MaxValue, out result);
	}

	/// <summary>
	/// Tests a ray against the space.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	/// <param name="filter">Delegate to prune out hit candidates before performing a ray cast against them.</param>
	/// <param name="result">Hit data of the ray, if any.</param>
	/// <returns>Whether or not the ray hit anything.</returns>
	public bool RayCast(Ray ray, Func<BroadPhaseEntry, bool> filter, out RayCastResult result)
	{
		return RayCast(ray, float.MaxValue, filter, out result);
	}

	/// <summary>
	/// Tests a ray against the space.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	/// <param name="maximumLength">Maximum length of the ray in units of the ray direction's length.</param>
	/// <param name="result">Hit data of the ray, if any.</param>
	/// <returns>Whether or not the ray hit anything.</returns>
	public bool RayCast(Ray ray, float maximumLength, out RayCastResult result)
	{
		RawList<RayCastResult> rayCastResultList = Resources.GetRayCastResultList();
		bool result2 = RayCast(ray, maximumLength, rayCastResultList);
		result = rayCastResultList.Elements[0];
		for (int i = 1; i < rayCastResultList.count; i++)
		{
			RayCastResult rayCastResult = rayCastResultList.Elements[i];
			if (rayCastResult.HitData.T < result.HitData.T)
			{
				result = rayCastResult;
			}
		}
		Resources.GiveBack(rayCastResultList);
		return result2;
	}

	/// <summary>
	/// Tests a ray against the space.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	/// <param name="maximumLength">Maximum length of the ray in units of the ray direction's length.</param>
	/// <param name="filter">Delegate to prune out hit candidates before performing a ray cast against them.</param>
	/// <param name="result">Hit data of the ray, if any.</param>
	/// <returns>Whether or not the ray hit anything.</returns>
	public bool RayCast(Ray ray, float maximumLength, Func<BroadPhaseEntry, bool> filter, out RayCastResult result)
	{
		RawList<RayCastResult> rayCastResultList = Resources.GetRayCastResultList();
		bool result2 = RayCast(ray, maximumLength, filter, rayCastResultList);
		result = rayCastResultList.Elements[0];
		for (int i = 1; i < rayCastResultList.count; i++)
		{
			RayCastResult rayCastResult = rayCastResultList.Elements[i];
			if (rayCastResult.HitData.T < result.HitData.T)
			{
				result = rayCastResult;
			}
		}
		Resources.GiveBack(rayCastResultList);
		return result2;
	}

	/// <summary>
	/// Tests a ray against the space, possibly returning multiple hits.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	/// <param name="maximumLength">Maximum length of the ray in units of the ray direction's length.</param>
	/// <param name="outputRayCastResults">Hit data of the ray, if any.</param>
	/// <returns>Whether or not the ray hit anything.</returns>
	public bool RayCast(Ray ray, float maximumLength, IList<RayCastResult> outputRayCastResults)
	{
		RawList<BroadPhaseEntry> broadPhaseEntryList = Resources.GetBroadPhaseEntryList();
		if (BroadPhase.QueryAccelerator.RayCast(ray, maximumLength, broadPhaseEntryList))
		{
			for (int i = 0; i < broadPhaseEntryList.count; i++)
			{
				BroadPhaseEntry broadPhaseEntry = broadPhaseEntryList.Elements[i];
				if (broadPhaseEntry.RayCast(ray, maximumLength, out var rayHit))
				{
					outputRayCastResults.Add(new RayCastResult(rayHit, broadPhaseEntry));
				}
			}
		}
		Resources.GiveBack(broadPhaseEntryList);
		return outputRayCastResults.Count > 0;
	}

	/// <summary>
	/// Tests a ray against the space, possibly returning multiple hits.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	/// <param name="maximumLength">Maximum length of the ray in units of the ray direction's length.</param>
	/// <param name="filter">Delegate to prune out hit candidates before performing a ray cast against them.</param>
	/// <param name="outputRayCastResults">Hit data of the ray, if any.</param>
	/// <returns>Whether or not the ray hit anything.</returns>
	public bool RayCast(Ray ray, float maximumLength, Func<BroadPhaseEntry, bool> filter, IList<RayCastResult> outputRayCastResults)
	{
		RawList<BroadPhaseEntry> broadPhaseEntryList = Resources.GetBroadPhaseEntryList();
		if (BroadPhase.QueryAccelerator.RayCast(ray, maximumLength, broadPhaseEntryList))
		{
			for (int i = 0; i < broadPhaseEntryList.count; i++)
			{
				BroadPhaseEntry broadPhaseEntry = broadPhaseEntryList.Elements[i];
				if (broadPhaseEntry.RayCast(ray, maximumLength, filter, out var rayHit))
				{
					outputRayCastResults.Add(new RayCastResult(rayHit, broadPhaseEntry));
				}
			}
		}
		Resources.GiveBack(broadPhaseEntryList);
		return outputRayCastResults.Count > 0;
	}

	/// <summary>
	/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
	/// </summary>
	/// <filterpriority>2</filterpriority>
	public void Dispose()
	{
		if (!disposed)
		{
			disposed = true;
			ThreadManager.Dispose();
		}
	}
}
