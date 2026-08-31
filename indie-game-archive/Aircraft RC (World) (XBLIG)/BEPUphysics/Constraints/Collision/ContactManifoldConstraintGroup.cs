using System;
using BEPUphysics.Constraints.SolverGroups;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;

namespace BEPUphysics.Constraints.Collision;

/// <summary>
///  Constraint group containing multiple contact manifold constraints.
///  Used by some pairs which manage multiple sub-pairs.
/// </summary>
public class ContactManifoldConstraintGroup : SolverGroup
{
	protected Entity entityA;

	protected Entity entityB;

	/// <summary>
	///  Gets the first entity in the pair.
	/// </summary>
	public Entity EntityA => entityA;

	/// <summary>
	///  Gets the second entity in the pair.
	/// </summary>
	public Entity EntityB => entityB;

	/// <summary>
	///  Adds a constraint to the group.
	/// </summary>
	/// <param name="manifoldConstraint">Constraint to add.</param>
	public new void Add(EntitySolverUpdateable manifoldConstraint)
	{
		if (manifoldConstraint.solver == null)
		{
			if (manifoldConstraint.SolverGroup == null)
			{
				solverUpdateables.Add(manifoldConstraint);
				manifoldConstraint.SolverGroup = this;
				manifoldConstraint.Solver = solver;
				return;
			}
			throw new InvalidOperationException("Cannot add SolverUpdateable to SolverGroup; it already belongs to a SolverGroup.");
		}
		throw new InvalidOperationException("Cannot add SolverUpdateable to SolverGroup; it already belongs to a solver.");
	}

	/// <summary>
	///  Removes a constraint from the group.
	/// </summary>
	/// <param name="manifoldConstraint">Constraint to remove.</param>
	public new void Remove(EntitySolverUpdateable manifoldConstraint)
	{
		if (manifoldConstraint.SolverGroup == this)
		{
			solverUpdateables.Remove(manifoldConstraint);
			manifoldConstraint.SolverGroup = null;
			manifoldConstraint.Solver = null;
			return;
		}
		throw new InvalidOperationException("Cannot remove SolverUpdateable from SolverGroup; it doesn't belong to this SolverGroup.");
	}

	protected internal override void CollectInvolvedEntities(RawList<Entity> outputInvolvedEntities)
	{
		if (entityA != null)
		{
			outputInvolvedEntities.Add(entityA);
		}
		if (entityB != null)
		{
			outputInvolvedEntities.Add(entityB);
		}
	}

	protected internal override void OnInvolvedEntitiesChanged()
	{
		CollectInvolvedEntities();
	}

	/// <summary>
	///  Initializes the constraint group.
	/// </summary>
	/// <param name="a">First entity of the pair.</param>
	/// <param name="b">Second entity of the pair.</param>
	public virtual void Initialize(Entity a, Entity b)
	{
		entityA = a;
		entityB = b;
		OnInvolvedEntitiesChanged();
	}

	/// <summary>
	///  Cleans up the constraint group.
	/// </summary>
	public virtual void CleanUp()
	{
		entityA = null;
		entityB = null;
		OnInvolvedEntitiesChanged();
	}
}
