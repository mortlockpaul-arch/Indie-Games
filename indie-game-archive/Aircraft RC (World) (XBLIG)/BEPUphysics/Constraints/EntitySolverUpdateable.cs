using System.Collections.Generic;
using BEPUphysics.Constraints.SolverGroups;
using BEPUphysics.DataStructures;
using BEPUphysics.DeactivationManagement;
using BEPUphysics.Entities;
using BEPUphysics.ResourceManagement;
using BEPUphysics.SolverSystems;

namespace BEPUphysics.Constraints;

/// <summary>
/// Superclass of objects types which require solving by the velocity solver.
/// These are updated within the internal iterative solver when owned by a space.
/// </summary>
public abstract class EntitySolverUpdateable : SolverUpdateable
{
	private class EntityComparer : IComparer<Entity>
	{
		int IComparer<Entity>.Compare(Entity x, Entity y)
		{
			if (x.InstanceId > y.InstanceId)
			{
				return 1;
			}
			if (x.InstanceId < y.InstanceId)
			{
				return -1;
			}
			return 0;
		}
	}

	/// <summary>
	/// List of all entities affected by this updateable.
	/// </summary>
	protected internal readonly RawList<Entity> involvedEntities = new RawList<Entity>(2);

	/// <summary>
	/// Number of entities used in the solver updateable.
	/// Note that this is set automatically by the sortInvolvedEntities method
	/// if it is called.
	/// </summary>
	protected internal int numberOfInvolvedEntities;

	private static EntityComparer comparer = new EntityComparer();

	/// <summary>
	///  Gets the entities that this solver updateable is involved with.
	/// </summary>
	public ReadOnlyList<Entity> InvolvedEntities => new ReadOnlyList<Entity>(involvedEntities);

	/// <summary>
	/// Gets the solver group that manages this solver updateable, if any.
	/// Null if not owned by a solver group.
	/// </summary>
	public SolverGroup SolverGroup { get; protected internal set; }

	/// <summary>
	/// Acquires exclusive access to all entities involved in the solver updateable.
	/// </summary>
	public override void EnterLock()
	{
		for (int i = 0; i < numberOfInvolvedEntities; i++)
		{
			if (involvedEntities.Elements[i].isDynamic)
			{
				involvedEntities.Elements[i].locker.Enter();
			}
		}
	}

	/// <summary>
	/// Releases exclusive access to the updateable's entities.
	/// This should be called within a 'finally' block following a 'try' block containing the locked operations.
	/// </summary>
	public override void ExitLock()
	{
		for (int num = numberOfInvolvedEntities - 1; num >= 0; num--)
		{
			if (involvedEntities.Elements[num].isDynamic)
			{
				involvedEntities.Elements[num].locker.Exit();
			}
		}
	}

	/// <summary>
	/// Attempts to acquire exclusive access to all entities involved in the solver updateable.
	/// </summary>
	/// <returns>Whether or not the lock was entered successfully.</returns>
	public override bool TryEnterLock()
	{
		for (int i = 0; i < numberOfInvolvedEntities; i++)
		{
			if (!involvedEntities.Elements[i].isDynamic || involvedEntities.Elements[i].locker.TryEnter())
			{
				continue;
			}
			for (i--; i >= 0; i--)
			{
				if (involvedEntities[i].isDynamic)
				{
					involvedEntities.Elements[i].locker.Exit();
				}
			}
			return false;
		}
		return true;
	}

	/// <summary>
	/// Handle any bookkeeping needed when the entities involved in this SolverUpdateable change.
	/// </summary>
	protected internal virtual void OnInvolvedEntitiesChanged()
	{
		bool flag = false;
		RawList<Entity> entityRawList = Resources.GetEntityRawList();
		CollectInvolvedEntities(entityRawList);
		if (entityRawList.count == involvedEntities.count)
		{
			for (int i = 0; i < entityRawList.Count; i++)
			{
				if (entityRawList.Elements[i] != involvedEntities.Elements[i])
				{
					flag = true;
					break;
				}
			}
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			for (int j = 0; j < involvedEntities.count; j++)
			{
				Entity entity = involvedEntities.Elements[j];
				if (entity.isDynamic)
				{
					entity.activityInformation.Activate();
					break;
				}
			}
			CollectInvolvedEntities();
			if (SolverGroup != null)
			{
				SolverGroup.OnInvolvedEntitiesChanged();
			}
			for (int k = 0; k < involvedEntities.count; k++)
			{
				Entity entity2 = involvedEntities.Elements[k];
				if (entity2.isDynamic)
				{
					entity2.activityInformation.Activate();
					break;
				}
			}
		}
		Resources.GiveBack(entityRawList);
	}

	/// <summary>
	/// Collects the entities involved in a solver updateable and sets up the internal listings.
	/// </summary>
	protected internal void CollectInvolvedEntities()
	{
		involvedEntities.Clear();
		CollectInvolvedEntities(involvedEntities);
		SortInvolvedEntities();
		UpdateConnectedMembers();
	}

	/// <summary>
	/// Adds entities associated with the solver item to the involved entities list.
	/// This allows the non-batched multithreading system to lock properly.
	/// </summary>
	protected internal abstract void CollectInvolvedEntities(RawList<Entity> outputInvolvedEntities);

	/// <summary>
	/// Sorts the involved entities according to their hashcode to allow non-batched multithreading to avoid deadlocks.
	/// </summary>
	protected internal void SortInvolvedEntities()
	{
		numberOfInvolvedEntities = involvedEntities.Count;
		involvedEntities.Sort(comparer);
	}

	private void UpdateConnectedMembers()
	{
		DeactivationManager deactivationManager = simulationIslandConnection.DeactivationManager;
		if (deactivationManager != null)
		{
			simulationIslandConnection.Owner = null;
			deactivationManager.Remove(simulationIslandConnection);
		}
		else if (!simulationIslandConnection.SlatedForRemoval)
		{
			Resources.GiveBack(simulationIslandConnection);
		}
		simulationIslandConnection = Resources.GetSimulationIslandConnection();
		for (int i = 0; i < involvedEntities.count; i++)
		{
			simulationIslandConnection.Add(involvedEntities.Elements[i].activityInformation);
		}
		simulationIslandConnection.Owner = this;
		deactivationManager?.Add(simulationIslandConnection);
	}
}
