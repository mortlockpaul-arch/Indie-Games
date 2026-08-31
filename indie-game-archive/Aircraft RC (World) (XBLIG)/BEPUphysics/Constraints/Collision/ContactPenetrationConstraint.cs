using System;
using BEPUphysics.CollisionTests;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.Settings;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.Collision;

/// <summary>
/// Computes the forces necessary to keep two entities from going through each other at a contact point.
/// </summary>
public class ContactPenetrationConstraint : EntitySolverUpdateable
{
	internal Contact contact;

	internal float accumulatedImpulse;

	internal float angularAX;

	internal float angularAY;

	internal float angularAZ;

	internal float angularBX;

	internal float angularBY;

	internal float angularBZ;

	private float bias;

	private float linearAX;

	private float linearAY;

	private float linearAZ;

	private Entity entityA;

	private Entity entityB;

	private bool entityADynamic;

	private bool entityBDynamic;

	internal float velocityToImpulse;

	private ContactManifoldConstraint contactManifoldConstraint;

	internal Vector3 ra;

	internal Vector3 rb;

	/// <summary>
	///  Gets the contact associated with this penetration constraint.
	/// </summary>
	public Contact Contact => contact;

	/// <summary>
	/// Gets the total normal impulse applied by this penetration constraint to maintain the separation of the involved entities.
	/// </summary>
	public float NormalImpulse => accumulatedImpulse;

	/// <summary>
	///  Gets the relative velocity between the associated entities at the contact point along the contact normal.
	/// </summary>
	public float RelativeVelocity
	{
		get
		{
			float num = 0f;
			if (entityA != null)
			{
				num = entityA.linearVelocity.X * linearAX + entityA.linearVelocity.Y * linearAY + entityA.linearVelocity.Z * linearAZ + entityA.angularVelocity.X * angularAX + entityA.angularVelocity.Y * angularAY + entityA.angularVelocity.Z * angularAZ;
			}
			if (entityB != null)
			{
				num += (0f - entityB.linearVelocity.X) * linearAX - entityB.linearVelocity.Y * linearAY - entityB.linearVelocity.Z * linearAZ + entityB.angularVelocity.X * angularBX + entityB.angularVelocity.Y * angularBY + entityB.angularVelocity.Z * angularBZ;
			}
			return num;
		}
	}

	/// <summary>
	///  Constructs a new penetration constraint.
	/// </summary>
	public ContactPenetrationConstraint()
	{
		isActive = false;
	}

	/// <summary>
	///  Configures the penetration constraint.
	/// </summary>
	/// <param name="contactManifoldConstraint">Owning manifold constraint.</param>
	/// <param name="contact">Contact associated with the penetration constraint.</param>
	public void Setup(ContactManifoldConstraint contactManifoldConstraint, Contact contact)
	{
		this.contactManifoldConstraint = contactManifoldConstraint;
		this.contact = contact;
		isActive = true;
		entityA = contactManifoldConstraint.EntityA;
		entityB = contactManifoldConstraint.EntityB;
	}

	/// <summary>
	///  Cleans up the constraint.
	/// </summary>
	public void CleanUp()
	{
		accumulatedImpulse = 0f;
		contactManifoldConstraint = null;
		contact = null;
		entityA = null;
		entityB = null;
		isActive = false;
	}

	/// <summary>
	///  Performs the frame's configuration step.
	/// </summary>
	/// <param name="dt">Timestep duration.</param>
	public override void Update(float dt)
	{
		entityADynamic = entityA != null && entityA.isDynamic;
		entityBDynamic = entityB != null && entityB.isDynamic;
		linearAX = 0f - contact.Normal.X;
		linearAY = 0f - contact.Normal.Y;
		linearAZ = 0f - contact.Normal.Z;
		if (entityA != null)
		{
			Vector3.Subtract(ref contact.Position, ref entityA.position, out ra);
			angularAX = ra.Y * linearAZ - ra.Z * linearAY;
			angularAY = ra.Z * linearAX - ra.X * linearAZ;
			angularAZ = ra.X * linearAY - ra.Y * linearAX;
		}
		if (entityB != null)
		{
			Vector3.Subtract(ref contact.Position, ref entityB.position, out rb);
			angularBX = linearAY * rb.Z - linearAZ * rb.Y;
			angularBY = linearAZ * rb.X - linearAX * rb.Z;
			angularBZ = linearAX * rb.Y - linearAY * rb.X;
		}
		float num4;
		if (entityADynamic)
		{
			float num = angularAX * entityA.inertiaTensorInverse.M11 + angularAY * entityA.inertiaTensorInverse.M21 + angularAZ * entityA.inertiaTensorInverse.M31;
			float num2 = angularAX * entityA.inertiaTensorInverse.M12 + angularAY * entityA.inertiaTensorInverse.M22 + angularAZ * entityA.inertiaTensorInverse.M32;
			float num3 = angularAX * entityA.inertiaTensorInverse.M13 + angularAY * entityA.inertiaTensorInverse.M23 + angularAZ * entityA.inertiaTensorInverse.M33;
			num4 = num * angularAX + num2 * angularAY + num3 * angularAZ + entityA.inverseMass;
		}
		else
		{
			num4 = 0f;
		}
		float num5;
		if (entityBDynamic)
		{
			float num = angularBX * entityB.inertiaTensorInverse.M11 + angularBY * entityB.inertiaTensorInverse.M21 + angularBZ * entityB.inertiaTensorInverse.M31;
			float num2 = angularBX * entityB.inertiaTensorInverse.M12 + angularBY * entityB.inertiaTensorInverse.M22 + angularBZ * entityB.inertiaTensorInverse.M32;
			float num3 = angularBX * entityB.inertiaTensorInverse.M13 + angularBY * entityB.inertiaTensorInverse.M23 + angularBZ * entityB.inertiaTensorInverse.M33;
			num5 = num * angularBX + num2 * angularBY + num3 * angularBZ + entityB.inverseMass;
		}
		else
		{
			num5 = 0f;
		}
		velocityToImpulse = -1f / (num4 + num5);
		if (contact.PenetrationDepth >= 0f)
		{
			bias = MathHelper.Min(MathHelper.Max(0f, contact.PenetrationDepth - CollisionDetectionSettings.AllowedPenetration) * CollisionResponseSettings.PenetrationRecoveryStiffness / dt, CollisionResponseSettings.MaximumPenetrationCorrectionSpeed);
			if (contactManifoldConstraint.materialInteraction.Bounciness > 0f)
			{
				float num6 = 0f - RelativeVelocity;
				if (num6 > CollisionResponseSettings.BouncinessVelocityThreshold)
				{
					bias = MathHelper.Max(num6 * contactManifoldConstraint.materialInteraction.Bounciness, bias);
				}
			}
		}
		else
		{
			bias = contact.PenetrationDepth / dt;
		}
	}

	/// <summary>
	/// Performs any pre-solve iteration work that needs exclusive
	/// access to the members of the solver updateable.
	/// Usually, this is used for applying warmstarting impulses.
	/// </summary>
	public override void ExclusiveUpdate()
	{
		Vector3 impulse = default(Vector3);
		Vector3 impulse2 = default(Vector3);
		impulse.X = accumulatedImpulse * linearAX;
		impulse.Y = accumulatedImpulse * linearAY;
		impulse.Z = accumulatedImpulse * linearAZ;
		if (entityADynamic)
		{
			impulse2.X = accumulatedImpulse * angularAX;
			impulse2.Y = accumulatedImpulse * angularAY;
			impulse2.Z = accumulatedImpulse * angularAZ;
			entityA.ApplyLinearImpulse(ref impulse);
			entityA.ApplyAngularImpulse(ref impulse2);
		}
		if (entityBDynamic)
		{
			impulse.X = 0f - impulse.X;
			impulse.Y = 0f - impulse.Y;
			impulse.Z = 0f - impulse.Z;
			impulse2.X = accumulatedImpulse * angularBX;
			impulse2.Y = accumulatedImpulse * angularBY;
			impulse2.Z = accumulatedImpulse * angularBZ;
			entityB.ApplyLinearImpulse(ref impulse);
			entityB.ApplyAngularImpulse(ref impulse2);
		}
	}

	/// <summary>
	/// Computes and applies an impulse to keep the colliders from penetrating.
	/// </summary>
	/// <returns>Impulse applied.</returns>
	public override float SolveIteration()
	{
		float num = (RelativeVelocity - bias) * velocityToImpulse;
		float num2 = accumulatedImpulse;
		accumulatedImpulse = MathHelper.Max(0f, accumulatedImpulse + num);
		num = accumulatedImpulse - num2;
		Vector3 impulse = default(Vector3);
		Vector3 impulse2 = default(Vector3);
		impulse.X = num * linearAX;
		impulse.Y = num * linearAY;
		impulse.Z = num * linearAZ;
		if (entityADynamic)
		{
			impulse2.X = num * angularAX;
			impulse2.Y = num * angularAY;
			impulse2.Z = num * angularAZ;
			entityA.ApplyLinearImpulse(ref impulse);
			entityA.ApplyAngularImpulse(ref impulse2);
		}
		if (entityBDynamic)
		{
			impulse.X = 0f - impulse.X;
			impulse.Y = 0f - impulse.Y;
			impulse.Z = 0f - impulse.Z;
			impulse2.X = num * angularBX;
			impulse2.Y = num * angularBY;
			impulse2.Z = num * angularBZ;
			entityB.ApplyLinearImpulse(ref impulse);
			entityB.ApplyAngularImpulse(ref impulse2);
		}
		return Math.Abs(num);
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
