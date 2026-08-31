using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.DataStructures;
using BEPUphysics.Threading;

namespace BEPUphysics.BroadPhaseSystems;

/// <summary>
///  Superclass of all broad phases.  Broad phases collect overlapping broad phase entry pairs.
/// </summary>
public abstract class BroadPhase : MultithreadedProcessingStage
{
	private readonly SpinLock overlapAddLock = new SpinLock();

	private readonly RawList<BroadPhaseOverlap> overlaps = new RawList<BroadPhaseOverlap>();

	/// <summary>
	///  Gets the object which is locked by the broadphase during synchronized update processes.
	/// </summary>
	public object Locker { get; protected set; }

	/// <summary>
	/// Gets the list of overlaps identified in the previous broad phase update.
	/// </summary>
	public RawList<BroadPhaseOverlap> Overlaps => overlaps;

	/// <summary>
	///  Gets an interface to the broad phase's support for volume-based queries.
	/// </summary>
	public IQueryAccelerator QueryAccelerator { get; protected set; }

	protected BroadPhase()
	{
		Locker = new object();
		Enabled = true;
	}

	protected BroadPhase(IThreadManager threadManager)
		: this()
	{
		base.ThreadManager = threadManager;
		base.AllowMultithreading = true;
	}

	/// <summary>
	/// Adds an entry to the broad phase.
	/// </summary>
	/// <param name="entry">Entry to add.</param>
	public virtual void Add(BroadPhaseEntry entry)
	{
		if (entry.BroadPhase == null)
		{
			entry.BroadPhase = this;
			return;
		}
		throw new Exception("Cannot add entry; it already belongs to a broad phase.");
	}

	/// <summary>
	/// Removes an entry from the broad phase.
	/// </summary>
	/// <param name="entry">Entry to remove.</param>
	public virtual void Remove(BroadPhaseEntry entry)
	{
		if (entry.BroadPhase == this)
		{
			entry.BroadPhase = null;
			return;
		}
		throw new Exception("Cannot remove entry; it does not belong to this broad phase.");
	}

	protected internal void AddOverlap(BroadPhaseOverlap overlap)
	{
		overlapAddLock.Enter();
		overlaps.Add(overlap);
		overlapAddLock.Exit();
	}

	/// <summary>
	/// Adds a broad phase overlap if the collision rules permit it.
	/// </summary>
	/// <param name="entryA">First entry of the overlap.</param>
	/// <param name="entryB">Second entry of the overlap.</param>
	protected internal void TryToAddOverlap(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
	{
		CollisionRule collisionRule;
		if ((collisionRule = GetCollisionRule(entryA, entryB)) < CollisionRule.NoBroadPhase)
		{
			overlapAddLock.Enter();
			overlaps.Add(new BroadPhaseOverlap(entryA, entryB, collisionRule));
			overlapAddLock.Exit();
		}
	}

	protected internal CollisionRule GetCollisionRule(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
	{
		if (entryA.IsActive || entryB.IsActive)
		{
			return CollisionRules.collisionRuleCalculator(entryA, entryB);
		}
		return CollisionRule.NoBroadPhase;
	}
}
