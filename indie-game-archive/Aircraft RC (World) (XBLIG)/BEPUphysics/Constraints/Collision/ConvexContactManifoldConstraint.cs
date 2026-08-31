using System.Collections.Generic;
using BEPUphysics.CollisionTests;
using BEPUphysics.DataStructures;

namespace BEPUphysics.Constraints.Collision;

/// <summary>
///  Contact manifold constraint that is used by manifolds whose normals are assumed to be
///  essentially the same.  This assumption can only be maintained between two convex objects.
/// </summary>
public class ConvexContactManifoldConstraint : ContactManifoldConstraint
{
	internal TwistFrictionConstraint twistFriction;

	internal SlidingFrictionTwoAxis slidingFriction;

	internal RawList<ContactPenetrationConstraint> penetrationConstraints;

	private Stack<ContactPenetrationConstraint> penetrationConstraintPool = new Stack<ContactPenetrationConstraint>(4);

	/// <summary>
	///  Gets the twist friction constraint used by the manifold.
	/// </summary>
	public TwistFrictionConstraint TwistFriction => twistFriction;

	/// <summary>
	///  Gets the sliding friction constraint used by the manifold.
	/// </summary>
	public SlidingFrictionTwoAxis SlidingFriction => slidingFriction;

	/// <summary>
	///  Gets the penetration constraints used by the manifold.
	/// </summary>
	public ReadOnlyList<ContactPenetrationConstraint> ContactPenetrationConstraints => new ReadOnlyList<ContactPenetrationConstraint>(penetrationConstraints);

	/// <summary>
	///  Constructs a new convex contact manifold constraint.
	/// </summary>
	public ConvexContactManifoldConstraint()
	{
		penetrationConstraints = new RawList<ContactPenetrationConstraint>(4);
		for (int i = 0; i < 4; i++)
		{
			ContactPenetrationConstraint contactPenetrationConstraint = new ContactPenetrationConstraint();
			Add(contactPenetrationConstraint);
			contactPenetrationConstraint.Tag = i;
			penetrationConstraintPool.Push(contactPenetrationConstraint);
		}
		slidingFriction = new SlidingFrictionTwoAxis();
		Add(slidingFriction);
		twistFriction = new TwistFrictionConstraint();
		Add(twistFriction);
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
		if (twistFriction.isActive)
		{
			twistFriction.CleanUp();
			slidingFriction.CleanUp();
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
		if (penetrationConstraints.count == 1)
		{
			twistFriction.Setup(this);
			slidingFriction.Setup(this);
		}
	}

	/// <summary>
	///  Removes a contact from the constraint.
	/// </summary>
	/// <param name="contact">Contact to remove.</param>
	public override void RemoveContact(Contact contact)
	{
		for (int i = 0; i < penetrationConstraints.count; i++)
		{
			ContactPenetrationConstraint contactPenetrationConstraint;
			if ((contactPenetrationConstraint = penetrationConstraints.Elements[i]).contact == contact)
			{
				contactPenetrationConstraint.CleanUp();
				penetrationConstraints.RemoveAt(i);
				penetrationConstraintPool.Push(contactPenetrationConstraint);
				break;
			}
		}
		if (penetrationConstraints.count == 0)
		{
			twistFriction.CleanUp();
			slidingFriction.CleanUp();
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
		UpdateUpdateable(slidingFriction, dt);
		UpdateUpdateable(twistFriction, dt);
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
		ExclusiveUpdateUpdateable(slidingFriction);
		ExclusiveUpdateUpdateable(twistFriction);
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
		SolveUpdateable(slidingFriction, ref activeConstraints);
		SolveUpdateable(twistFriction, ref activeConstraints);
		isActiveInSolver = activeConstraints > 0;
		return solverSettings.minimumImpulse + 1f;
	}
}
