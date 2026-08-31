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
///  Handles a sphere-sphere collision pair.
/// </summary>
public class SpherePairHandler : ConvexPairHandler
{
	private ConvexCollidable<SphereShape> sphereA;

	private ConvexCollidable<SphereShape> sphereB;

	private SphereContactManifold contactManifold = new SphereContactManifold();

	private NonConvexContactManifoldConstraint contactConstraint = new NonConvexContactManifoldConstraint();

	public override Collidable CollidableA => sphereA;

	public override Collidable CollidableB => sphereB;

	public override Entity EntityA => sphereA.entity;

	public override Entity EntityB => sphereB.entity;

	/// <summary>
	/// Gets the contact constraint used by the pair handler.
	/// </summary>
	public override ContactManifoldConstraint ContactConstraint => contactConstraint;

	/// <summary>
	/// Gets the contact manifold used by the pair handler.
	/// </summary>
	public override ContactManifold ContactManifold => contactManifold;

	/// <summary>
	///  Initializes the pair handler.
	/// </summary>
	/// <param name="entryA">First entry in the pair.</param>
	/// <param name="entryB">Second entry in the pair.</param>
	public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
	{
		sphereA = entryA as ConvexCollidable<SphereShape>;
		sphereB = entryB as ConvexCollidable<SphereShape>;
		if (sphereA == null || sphereB == null)
		{
			throw new Exception("Inappropriate types used to initialize pair.");
		}
		base.Initialize(entryA, entryB);
	}

	/// <summary>
	///  Cleans up the pair handler.
	/// </summary>
	public override void CleanUp()
	{
		base.CleanUp();
		sphereA = null;
		sphereB = null;
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
