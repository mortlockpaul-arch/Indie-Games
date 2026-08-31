using System;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.Settings;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.Collision;

/// <summary>
/// Computes the forces necessary to slow down and stop twisting motion in a collision between two entities.
/// </summary>
public class TwistFrictionConstraint : EntitySolverUpdateable
{
	private readonly float[] leverArms = new float[4];

	private ConvexContactManifoldConstraint contactManifoldConstraint;

	internal float accumulatedImpulse;

	private float angularX;

	private float angularY;

	private float angularZ;

	private int contactCount;

	private float friction;

	private Entity entityA;

	private Entity entityB;

	private bool entityADynamic;

	private bool entityBDynamic;

	private float velocityToImpulse;

	/// <summary>
	///  Gets the contact manifold constraint that owns this constraint.
	/// </summary>
	public ConvexContactManifoldConstraint ContactManifoldConstraint => contactManifoldConstraint;

	/// <summary>
	/// Gets the torque applied by twist friction.
	/// </summary>
	public float TotalTorque => accumulatedImpulse;

	/// <summary>
	///  Gets the angular velocity between the associated entities.
	/// </summary>
	public float RelativeVelocity
	{
		get
		{
			float num = 0f;
			if (entityA != null)
			{
				num = entityA.angularVelocity.X * angularX + entityA.angularVelocity.Y * angularY + entityA.angularVelocity.Z * angularZ;
			}
			if (entityB != null)
			{
				num -= entityB.angularVelocity.X * angularX + entityB.angularVelocity.Y * angularY + entityB.angularVelocity.Z * angularZ;
			}
			return num;
		}
	}

	/// <summary>
	///  Constructs a new twist friction constraint.
	/// </summary>
	public TwistFrictionConstraint()
	{
		isActive = false;
	}

	/// <summary>
	/// Computes one iteration of the constraint to meet the solver updateable's goal.
	/// </summary>
	/// <returns>The rough applied impulse magnitude.</returns>
	public override float SolveIteration()
	{
		float relativeVelocity = RelativeVelocity;
		relativeVelocity *= velocityToImpulse;
		float num = accumulatedImpulse;
		float num2 = 0f;
		for (int i = 0; i < contactCount; i++)
		{
			num2 += leverArms[i] * contactManifoldConstraint.penetrationConstraints.Elements[i].accumulatedImpulse;
		}
		num2 *= friction;
		accumulatedImpulse = MathHelper.Clamp(accumulatedImpulse + relativeVelocity, 0f - num2, num2);
		relativeVelocity = accumulatedImpulse - num;
		Vector3 impulse = new Vector3
		{
			X = relativeVelocity * angularX,
			Y = relativeVelocity * angularY,
			Z = relativeVelocity * angularZ
		};
		if (entityADynamic)
		{
			entityA.ApplyAngularImpulse(ref impulse);
		}
		if (entityBDynamic)
		{
			impulse.X = 0f - impulse.X;
			impulse.Y = 0f - impulse.Y;
			impulse.Z = 0f - impulse.Z;
			entityB.ApplyAngularImpulse(ref impulse);
		}
		return Math.Abs(relativeVelocity);
	}

	/// <summary>
	///  Performs the frame's configuration step.
	/// </summary>
	/// <param name="dt">Timestep duration.</param>
	public override void Update(float dt)
	{
		entityADynamic = entityA != null && entityA.isDynamic;
		entityBDynamic = entityB != null && entityB.isDynamic;
		Vector3 normal = contactManifoldConstraint.penetrationConstraints.Elements[0].contact.Normal;
		angularX = normal.X;
		angularY = normal.Y;
		angularZ = normal.Z;
		float num4;
		if (entityADynamic)
		{
			float num = angularX * entityA.inertiaTensorInverse.M11 + angularY * entityA.inertiaTensorInverse.M21 + angularZ * entityA.inertiaTensorInverse.M31;
			float num2 = angularX * entityA.inertiaTensorInverse.M12 + angularY * entityA.inertiaTensorInverse.M22 + angularZ * entityA.inertiaTensorInverse.M32;
			float num3 = angularX * entityA.inertiaTensorInverse.M13 + angularY * entityA.inertiaTensorInverse.M23 + angularZ * entityA.inertiaTensorInverse.M33;
			num4 = num * angularX + num2 * angularY + num3 * angularZ + entityA.inverseMass;
		}
		else
		{
			num4 = 0f;
		}
		float num5;
		if (entityBDynamic)
		{
			float num = angularX * entityB.inertiaTensorInverse.M11 + angularY * entityB.inertiaTensorInverse.M21 + angularZ * entityB.inertiaTensorInverse.M31;
			float num2 = angularX * entityB.inertiaTensorInverse.M12 + angularY * entityB.inertiaTensorInverse.M22 + angularZ * entityB.inertiaTensorInverse.M32;
			float num3 = angularX * entityB.inertiaTensorInverse.M13 + angularY * entityB.inertiaTensorInverse.M23 + angularZ * entityB.inertiaTensorInverse.M33;
			num5 = num * angularX + num2 * angularY + num3 * angularZ + entityB.inverseMass;
		}
		else
		{
			num5 = 0f;
		}
		velocityToImpulse = -1f / (num4 + num5);
		float relativeVelocity = RelativeVelocity;
		Vector3 relativeVelocity2 = contactManifoldConstraint.SlidingFriction.relativeVelocity;
		friction = ((Math.Abs(relativeVelocity) > CollisionResponseSettings.StaticFrictionVelocityThreshold || Math.Abs(relativeVelocity2.X) + Math.Abs(relativeVelocity2.Y) + Math.Abs(relativeVelocity2.Z) > CollisionResponseSettings.StaticFrictionVelocityThreshold) ? contactManifoldConstraint.materialInteraction.KineticFriction : contactManifoldConstraint.materialInteraction.StaticFriction);
		friction *= CollisionResponseSettings.TwistFrictionFactor;
		contactCount = contactManifoldConstraint.penetrationConstraints.count;
		for (int i = 0; i < contactCount; i++)
		{
			Vector3.Subtract(ref contactManifoldConstraint.penetrationConstraints.Elements[i].contact.Position, ref contactManifoldConstraint.SlidingFriction.manifoldCenter, out var result);
			leverArms[i] = result.Length();
		}
	}

	/// <summary>
	/// Performs any pre-solve iteration work that needs exclusive
	/// access to the members of the solver updateable.
	/// Usually, this is used for applying warmstarting impulses.
	/// </summary>
	public override void ExclusiveUpdate()
	{
		Vector3 impulse = new Vector3
		{
			X = accumulatedImpulse * angularX,
			Y = accumulatedImpulse * angularY,
			Z = accumulatedImpulse * angularZ
		};
		if (entityADynamic)
		{
			entityA.ApplyAngularImpulse(ref impulse);
		}
		if (entityBDynamic)
		{
			impulse.X = 0f - impulse.X;
			impulse.Y = 0f - impulse.Y;
			impulse.Z = 0f - impulse.Z;
			entityB.ApplyAngularImpulse(ref impulse);
		}
	}

	internal void Setup(ConvexContactManifoldConstraint contactManifoldConstraint)
	{
		this.contactManifoldConstraint = contactManifoldConstraint;
		isActive = true;
		entityA = contactManifoldConstraint.EntityA;
		entityB = contactManifoldConstraint.EntityB;
	}

	internal void CleanUp()
	{
		accumulatedImpulse = 0f;
		contactManifoldConstraint = null;
		entityA = null;
		entityB = null;
		isActive = false;
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
}
