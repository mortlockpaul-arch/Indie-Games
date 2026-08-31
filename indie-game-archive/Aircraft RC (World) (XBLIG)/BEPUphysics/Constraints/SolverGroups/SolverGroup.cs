using System;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.SolverSystems;

namespace BEPUphysics.Constraints.SolverGroups;

/// <summary>
/// Superclass of constraints that are composed of multiple subconstraints.
/// </summary>
public abstract class SolverGroup : EntitySolverUpdateable
{
	internal readonly RawList<EntitySolverUpdateable> solverUpdateables = new RawList<EntitySolverUpdateable>();

	/// <summary>
	/// Gets the solver updateables managed by this solver group.
	/// </summary>
	public ReadOnlyList<EntitySolverUpdateable> SolverUpdateables => new ReadOnlyList<EntitySolverUpdateable>(solverUpdateables);

	/// <summary>
	///  Gets the solver to which the solver updateable belongs.
	/// </summary>
	public override Solver Solver
	{
		get
		{
			return solver;
		}
		internal set
		{
			base.Solver = value;
			for (int i = 0; i < solverUpdateables.count; i++)
			{
				solverUpdateables.Elements[i].Solver = value;
			}
		}
	}

	/// <summary>
	/// Collects the entities which are affected by the solver group and updates the internal listing.
	/// </summary>
	protected internal override void CollectInvolvedEntities(RawList<Entity> outputInvolvedEntities)
	{
		foreach (EntitySolverUpdateable solverUpdateable in solverUpdateables)
		{
			for (int i = 0; i < solverUpdateable.involvedEntities.count; i++)
			{
				if (!outputInvolvedEntities.Contains(solverUpdateable.involvedEntities.Elements[i]))
				{
					outputInvolvedEntities.Add(solverUpdateable.involvedEntities.Elements[i]);
				}
			}
		}
	}

	/// <summary>
	/// Sets the activity state of the constraint based on the activity state of its connections.
	/// Called automatically by the space owning a constaint.  If a constraint is a sub-constraint that hasn't been directly added to the space,
	/// this may need to be called alongside the preStep from within the parent constraint.
	/// </summary>
	public override void UpdateSolverActivity()
	{
		if (isActive)
		{
			isActiveInSolver = false;
			for (int i = 0; i < solverUpdateables.count; i++)
			{
				EntitySolverUpdateable entitySolverUpdateable = solverUpdateables.Elements[i];
				entitySolverUpdateable.UpdateSolverActivity();
				isActiveInSolver |= entitySolverUpdateable.isActiveInSolver;
			}
		}
		else
		{
			isActiveInSolver = false;
		}
	}

	protected void UpdateUpdateable(EntitySolverUpdateable item, float dt)
	{
		item.SolverSettings.currentIterations = 0;
		item.SolverSettings.iterationsAtZeroImpulse = 0;
		if (item.isActiveInSolver)
		{
			item.Update(dt);
		}
	}

	protected void ExclusiveUpdateUpdateable(EntitySolverUpdateable item)
	{
		if (item.isActiveInSolver)
		{
			item.ExclusiveUpdate();
		}
	}

	/// <summary>
	///  Performs the frame's configuration step.
	/// </summary>
	/// <param name="dt">Timestep duration.</param>
	public override void Update(float dt)
	{
		for (int i = 0; i < solverUpdateables.count; i++)
		{
			UpdateUpdateable(solverUpdateables.Elements[i], dt);
		}
	}

	/// <summary>
	/// Performs any pre-solve iteration work that needs exclusive
	/// access to the members of the solver updateable.
	/// Usually, this is used for applying warmstarting impulses.
	/// </summary>
	public override void ExclusiveUpdate()
	{
		for (int i = 0; i < solverUpdateables.count; i++)
		{
			ExclusiveUpdateUpdateable(solverUpdateables.Elements[i]);
		}
	}

	/// <summary>
	/// Solves a child updateable.  Some children may override the group's update method;
	/// this avoids code repeat.
	/// </summary>
	/// <param name="item"></param>
	/// <param name="activeConstraints"> </param>
	protected void SolveUpdateable(EntitySolverUpdateable item, ref int activeConstraints)
	{
		if (!item.isActiveInSolver)
		{
			return;
		}
		SolverSettings solverSettings = item.solverSettings;
		solverSettings.currentIterations++;
		if (solverSettings.currentIterations <= solver.iterationLimit && solverSettings.currentIterations <= solverSettings.maximumIterations)
		{
			if (item.SolveIteration() < solverSettings.minimumImpulse)
			{
				solverSettings.iterationsAtZeroImpulse++;
				if (solverSettings.iterationsAtZeroImpulse > solverSettings.minimumIterations)
				{
					item.isActiveInSolver = false;
				}
				else
				{
					activeConstraints++;
				}
			}
			else
			{
				solverSettings.iterationsAtZeroImpulse = 0;
				activeConstraints++;
			}
		}
		else
		{
			item.isActiveInSolver = false;
		}
	}

	/// <summary>
	/// Computes one iteration of the constraint to meet the solver updateable's goal.
	/// </summary>
	/// <returns>The rough applied impulse magnitude.</returns>
	public override float SolveIteration()
	{
		int activeConstraints = 0;
		for (int i = 0; i < solverUpdateables.count; i++)
		{
			SolveUpdateable(solverUpdateables.Elements[i], ref activeConstraints);
		}
		isActiveInSolver = activeConstraints > 0;
		return solverSettings.minimumImpulse + 1f;
	}

	/// <summary>
	/// Adds a solver updateable to the group.
	/// </summary>
	/// <param name="solverUpdateable">Solver updateable to add.</param>
	/// <exception cref="T:System.InvalidOperationException">Thrown when the SolverUpdateable to add to the SolverGroup already belongs to another SolverGroup or to a Space.</exception>
	protected void Add(EntitySolverUpdateable solverUpdateable)
	{
		if (solverUpdateable.solver == null)
		{
			if (solverUpdateable.SolverGroup == null)
			{
				solverUpdateables.Add(solverUpdateable);
				solverUpdateable.SolverGroup = this;
				solverUpdateable.Solver = solver;
				OnInvolvedEntitiesChanged();
				return;
			}
			throw new InvalidOperationException("Cannot add SolverUpdateable to SolverGroup; it already belongs to a SolverGroup.");
		}
		throw new InvalidOperationException("Cannot add SolverUpdateable to SolverGroup; it already belongs to a solver.");
	}

	/// <summary>
	/// Removes a solver updateable from the group.
	/// </summary>
	/// <param name="solverUpdateable">Solver updateable to remove.</param>
	/// <exception cref="T:System.InvalidOperationException">Thrown when the SolverUpdateable to remove from the SolverGroup doesn't actually belong to this SolverGroup.</exception>
	protected void Remove(EntitySolverUpdateable solverUpdateable)
	{
		if (solverUpdateable.SolverGroup == this)
		{
			solverUpdateables.Remove(solverUpdateable);
			solverUpdateable.SolverGroup = null;
			solverUpdateable.Solver = null;
			OnInvolvedEntitiesChanged();
			return;
		}
		throw new InvalidOperationException("Cannot remove SolverUpdateable from SolverGroup; it doesn't belong to this SolverGroup.");
	}

	/// <summary>
	/// Called after the object is added to a space.
	/// </summary>
	/// <param name="newSpace"></param>
	public override void OnAdditionToSpace(ISpace newSpace)
	{
		for (int i = 0; i < solverUpdateables.Count; i++)
		{
			solverUpdateables[i].OnAdditionToSpace(newSpace);
		}
	}

	/// <summary>
	/// Called before an object is removed from its space.
	/// </summary>
	public override void OnRemovalFromSpace(ISpace oldSpace)
	{
		for (int i = 0; i < solverUpdateables.Count; i++)
		{
			solverUpdateables[i].OnRemovalFromSpace(oldSpace);
		}
	}
}
