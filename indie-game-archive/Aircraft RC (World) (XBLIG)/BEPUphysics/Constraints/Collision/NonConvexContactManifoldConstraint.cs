using System.Collections.Generic;
using BEPUphysics.CollisionTests;
using BEPUphysics.DataStructures;

namespace BEPUphysics.Constraints.Collision;

/// <summary>
///  Collision constraint for non-convex manifolds.  These manifolds are usually used in cases
///  where the contacts are coming from multiple objects or from non-convex objects.  The normals
///  will likely face more than one direction.
/// </summary>
public class NonConvexContactManifoldConstraint : ContactManifoldConstraint
{
	internal RawList<ContactPenetrationConstraint> penetrationConstraints;

	private Stack<ContactPenetrationConstraint> penetrationConstraintPool = new Stack<ContactPenetrationConstraint>(4);

	internal RawList<ContactFrictionConstraint> frictionConstraints;

	private Stack<ContactFrictionConstraint> frictionConstraintPool = new Stack<ContactFrictionConstraint>(4);

	/// <summary>
	///  Gets the penetration constraints in the manifold.
	/// </summary>
	public ReadOnlyList<ContactPenetrationConstraint> ContactPenetrationConstraints => new ReadOnlyList<ContactPenetrationConstraint>(penetrationConstraints);

	/// <summary>
	///  Gets the friction constraints in the manifold.
	/// </summary>
	public ReadOnlyList<ContactFrictionConstraint> ContactFrictionConstraints => new ReadOnlyList<ContactFrictionConstraint>(frictionConstraints);

	/// <summary>
	///  Constructs a new nonconvex manifold constraint.
	/// </summary>
	public NonConvexContactManifoldConstraint()
	{
		penetrationConstraints = new RawList<ContactPenetrationConstraint>(4);
		frictionConstraints = new RawList<ContactFrictionConstraint>(4);
		for (int i = 0; i < 4; i++)
		{
			ContactPenetrationConstraint contactPenetrationConstraint = new ContactPenetrationConstraint();
			penetrationConstraintPool.Push(contactPenetrationConstraint);
			Add(contactPenetrationConstraint);
			ContactFrictionConstraint contactFrictionConstraint = new ContactFrictionConstraint();
			frictionConstraintPool.Push(contactFrictionConstraint);
			Add(contactFrictionConstraint);
		}
	}

	/// <summary>
	///  Cleans up the constraint.
	/// </summary>
	public override void CleanUp()
	{
		for (int num = penetrationConstraints.count - 1; num >= 0; num--)
		{
			ContactPenetrationConstraint contactPenetrationConstraint = penetrationConstraints.Elements[num];
			contactPenetrationConstraint.CleanUp();
			penetrationConstraints.RemoveAt(num);
			penetrationConstraintPool.Push(contactPenetrationConstraint);
		}
		for (int num2 = frictionConstraints.count - 1; num2 >= 0; num2--)
		{
			ContactFrictionConstraint contactFrictionConstraint = frictionConstraints.Elements[num2];
			contactFrictionConstraint.CleanUp();
			frictionConstraints.RemoveAt(num2);
			frictionConstraintPool.Push(contactFrictionConstraint);
		}
	}

	/// <summary>
	///  Adds a contact to be managed by the constraint.
	/// </summary>
	/// <param name="contact">Contact to add.</param>
	public override void AddContact(Contact contact)
	{
		ContactPenetrationConstraint contactPenetrationConstraint = penetrationConstraintPool.Pop();
		contactPenetrationConstraint.Setup(this, contact);
		penetrationConstraints.Add(contactPenetrationConstraint);
		ContactFrictionConstraint contactFrictionConstraint = frictionConstraintPool.Pop();
		contactFrictionConstraint.Setup(this, contactPenetrationConstraint);
		frictionConstraints.Add(contactFrictionConstraint);
	}

	/// <summary>
	///  Removes a contact from the constraint.
	/// </summary>
	/// <param name="contact">Contact to remove.</param>
	public override void RemoveContact(Contact contact)
	{
		ContactPenetrationConstraint contactPenetrationConstraint = null;
		for (int i = 0; i < penetrationConstraints.count; i++)
		{
			if ((contactPenetrationConstraint = penetrationConstraints.Elements[i]).contact == contact)
			{
				contactPenetrationConstraint.CleanUp();
				penetrationConstraints.RemoveAt(i);
				penetrationConstraintPool.Push(contactPenetrationConstraint);
				break;
			}
		}
		for (int num = frictionConstraints.count - 1; num >= 0; num--)
		{
			ContactFrictionConstraint contactFrictionConstraint = frictionConstraints[num];
			if (contactFrictionConstraint.PenetrationConstraint == contactPenetrationConstraint)
			{
				contactFrictionConstraint.CleanUp();
				frictionConstraints.RemoveAt(num);
				frictionConstraintPool.Push(contactFrictionConstraint);
				break;
			}
		}
	}

	/// <summary>
	///  Performs the frame's configuration step.
	/// </summary>
	/// <param name="dt">Timestep duration.</param>
	public sealed override void Update(float dt)
	{
		for (int i = 0; i < penetrationConstraints.count; i++)
		{
			UpdateUpdateable(penetrationConstraints.Elements[i], dt);
		}
		for (int j = 0; j < frictionConstraints.count; j++)
		{
			UpdateUpdateable(frictionConstraints.Elements[j], dt);
		}
	}

	/// <summary>
	/// Performs any pre-solve iteration work that needs exclusive
	/// access to the members of the solver updateable.
	/// Usually, this is used for applying warmstarting impulses.
	/// </summary>
	public sealed override void ExclusiveUpdate()
	{
		for (int i = 0; i < penetrationConstraints.count; i++)
		{
			ExclusiveUpdateUpdateable(penetrationConstraints.Elements[i]);
		}
		for (int j = 0; j < frictionConstraints.count; j++)
		{
			ExclusiveUpdateUpdateable(frictionConstraints.Elements[j]);
		}
	}

	/// <summary>
	/// Computes one iteration of the constraint to meet the solver updateable's goal.
	/// </summary>
	/// <returns>The rough applied impulse magnitude.</returns>
	public sealed override float SolveIteration()
	{
		int activeConstraints = 0;
		for (int i = 0; i < penetrationConstraints.count; i++)
		{
			SolveUpdateable(penetrationConstraints.Elements[i], ref activeConstraints);
		}
		for (int j = 0; j < frictionConstraints.count; j++)
		{
			SolveUpdateable(frictionConstraints.Elements[j], ref activeConstraints);
		}
		isActiveInSolver = activeConstraints > 0;
		return solverSettings.minimumImpulse + 1f;
	}
}
