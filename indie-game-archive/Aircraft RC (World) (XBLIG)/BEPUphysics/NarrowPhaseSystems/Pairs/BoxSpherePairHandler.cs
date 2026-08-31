using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests.Manifolds;
using BEPUphysics.Constraints.Collision;
using BEPUphysics.Entities;
using Microsoft.Xna.Framework;

namespace BEPUphysics.NarrowPhaseSystems.Pairs;

/// <summary>
///  Handles a box and sphere in a collision.
/// </summary>
public class BoxSpherePairHandler : ConvexPairHandler
{
	private ConvexCollidable<BoxShape> box;

	private ConvexCollidable<SphereShape> sphere;

	private BoxSphereContactManifold contactManifold = new BoxSphereContactManifold();

	private NonConvexContactManifoldConstraint contactConstraint = new NonConvexContactManifoldConstraint();

	public override Collidable CollidableA => box;

	public override Collidable CollidableB => sphere;

	/// <summary>
	/// Gets the contact constraint used by the pair handler.
	/// </summary>
	public override ContactManifoldConstraint ContactConstraint => contactConstraint;

	/// <summary>
	/// Gets the contact manifold used by the pair handler.
	/// </summary>
	public override ContactManifold ContactManifold => contactManifold;

	public override Entity EntityA => box.entity;

	public override Entity EntityB => sphere.entity;

	/// <summary>
	///  Initializes the pair handler.
	/// </summary>
	/// <param name="entryA">First entry in the pair.</param>
	/// <param name="entryB">Second entry in the pair.</param>
	public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
	{
		box = entryA as ConvexCollidable<BoxShape>;
		sphere = entryB as ConvexCollidable<SphereShape>;
		if (box == null || sphere == null)
		{
			box = entryB as ConvexCollidable<BoxShape>;
			sphere = entryA as ConvexCollidable<SphereShape>;
			if (box == null || sphere == null)
			{
				throw new Exception("Inappropriate types used to initialize pair.");
			}
		}
		broadPhaseOverlap.entryA = box;
		broadPhaseOverlap.entryB = sphere;
		base.Initialize(entryA, entryB);
	}

	/// <summary>
	///  Cleans up the pair handler.
	/// </summary>
	public override void CleanUp()
	{
		base.CleanUp();
		box = null;
		sphere = null;
	}

	protected internal override void GetContactInformation(int index, out ContactInformation info)
	{
		info.Contact = ContactManifold.contacts.Elements[index];
		info.FrictionImpulse = 0f;
		info.NormalImpulse = 0f;
		for (int i = 0; i < contactConstraint.frictionConstraints.count; i++)
		{
			if (contactConstraint.frictionConstraints.Elements[i].PenetrationConstraint.contact == info.Contact)
			{
				info.FrictionImpulse = contactConstraint.frictionConstraints.Elements[i].accumulatedImpulse;
				info.NormalImpulse = contactConstraint.frictionConstraints.Elements[i].PenetrationConstraint.accumulatedImpulse;
				break;
			}
		}
		Vector3 result;
		if (EntityA != null)
		{
			Vector3.Subtract(ref info.Contact.Position, ref EntityA.position, out result);
			Vector3.Cross(ref EntityA.angularVelocity, ref result, out result);
			Vector3.Add(ref result, ref EntityA.linearVelocity, out info.RelativeVelocity);
		}
		else
		{
			info.RelativeVelocity = default(Vector3);
		}
		if (EntityB != null)
		{
			Vector3.Subtract(ref info.Contact.Position, ref EntityB.position, out result);
			Vector3.Cross(ref EntityB.angularVelocity, ref result, out result);
			Vector3.Add(ref result, ref EntityB.linearVelocity, out result);
			Vector3.Subtract(ref info.RelativeVelocity, ref result, out info.RelativeVelocity);
		}
		info.Pair = this;
	}
}
