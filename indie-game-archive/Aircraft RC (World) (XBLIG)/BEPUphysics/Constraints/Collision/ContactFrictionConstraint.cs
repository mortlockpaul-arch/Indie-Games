using System;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.Settings;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.Collision;

/// <summary>
/// Computes the friction force for a contact when central friction cannot be used.
/// </summary>
public class ContactFrictionConstraint : EntitySolverUpdateable
{
	private ContactManifoldConstraint contactManifoldConstraint;

	private ContactPenetrationConstraint penetrationConstraint;

	internal float accumulatedImpulse;

	private float angularAX;

	private float angularAY;

	private float angularAZ;

	private float angularBX;

	private float angularBY;

	private float angularBZ;

	private float friction;

	internal float linearAX;

	internal float linearAY;

	internal float linearAZ;

	private Entity entityA;

	private Entity entityB;

	private bool entityAIsDynamic;

	private bool entityBIsDynamic;

	private float velocityToImpulse;

	/// <summary>
	///  Gets the manifold constraint associated with this friction constraint.
	/// </summary>
	public ContactManifoldConstraint ContactManifoldConstraint => contactManifoldConstraint;

	/// <summary>
	///  Gets the penetration constraint associated with this friction constraint.
	/// </summary>
	public ContactPenetrationConstraint PenetrationConstraint => penetrationConstraint;

	/// <summary>
	/// Gets the direction in which the friction force acts.
	/// </summary>
	public Vector3 FrictionDirection => new Vector3(linearAX, linearAY, linearAZ);

	/// <summary>
	/// Gets the total impulse applied by this friction constraint in the last time step.
	/// </summary>
	public float TotalImpulse => accumulatedImpulse;

	/// <summary>
	///  Gets the relative velocity of the constraint.  This is the velocity along the tangent movement direction.
	/// </summary>
	public float RelativeVelocity
	{
		get
		{
			float num = 0f;
			if (entityA != null)
			{
				num += entityA.linearVelocity.X * linearAX + entityA.linearVelocity.Y * linearAY + entityA.linearVelocity.Z * linearAZ + entityA.angularVelocity.X * angularAX + entityA.angularVelocity.Y * angularAY + entityA.angularVelocity.Z * angularAZ;
			}
			if (entityB != null)
			{
				num += (0f - entityB.linearVelocity.X) * linearAX - entityB.linearVelocity.Y * linearAY - entityB.linearVelocity.Z * linearAZ + entityB.angularVelocity.X * angularBX + entityB.angularVelocity.Y * angularBY + entityB.angularVelocity.Z * angularBZ;
			}
			return num;
		}
	}

	/// <summary>
	///  Constructs a new friction constraint.
	/// </summary>
	public ContactFrictionConstraint()
	{
		isActive = false;
	}

	/// <summary>
	///  Configures the friction constraint for a new contact.
	/// </summary>
	/// <param name="contactManifoldConstraint">Manifold to which the constraint belongs.</param>
	/// <param name="penetrationConstraint">Penetration constraint associated with this friction constraint.</param>
	public void Setup(ContactManifoldConstraint contactManifoldConstraint, ContactPenetrationConstraint penetrationConstraint)
	{
		this.contactManifoldConstraint = contactManifoldConstraint;
		this.penetrationConstraint = penetrationConstraint;
		base.IsActive = true;
		linearAX = 0f;
		linearAY = 0f;
		linearAZ = 0f;
		entityA = contactManifoldConstraint.EntityA;
		entityB = contactManifoldConstraint.EntityB;
	}

	/// <summary>
	///  Cleans upt he friction constraint.
	/// </summary>
	public void CleanUp()
	{
		accumulatedImpulse = 0f;
		contactManifoldConstraint = null;
		penetrationConstraint = null;
		entityA = null;
		entityB = null;
		base.IsActive = false;
	}

	/// <summary>
	/// Computes one iteration of the constraint to meet the solver updateable's goal.
	/// </summary>
	/// <returns>The rough applied impulse magnitude.</returns>
	public override float SolveIteration()
	{
		float num = RelativeVelocity * velocityToImpulse;
		float num2 = accumulatedImpulse;
		float num3 = friction * penetrationConstraint.accumulatedImpulse;
		accumulatedImpulse = MathHelper.Clamp(accumulatedImpulse + num, 0f - num3, num3);
		num = accumulatedImpulse - num2;
		Vector3 impulse = default(Vector3);
		Vector3 impulse2 = default(Vector3);
		impulse.X = num * linearAX;
		impulse.Y = num * linearAY;
		impulse.Z = num * linearAZ;
		if (entityAIsDynamic)
		{
			impulse2.X = num * angularAX;
			impulse2.Y = num * angularAY;
			impulse2.Z = num * angularAZ;
			entityA.ApplyLinearImpulse(ref impulse);
			entityA.ApplyAngularImpulse(ref impulse2);
		}
		if (entityBIsDynamic)
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

	/// <summary>
	/// Initializes the constraint for this frame.
	/// </summary>
	/// <param name="dt">Time since the last frame.</param>
	public override void Update(float dt)
	{
		entityAIsDynamic = entityA != null && entityA.isDynamic;
		entityBIsDynamic = entityB != null && entityB.isDynamic;
		Vector3 result = default(Vector3);
		Vector3 result2 = default(Vector3);
		Vector3 vector = penetrationConstraint.ra;
		Vector3 vector2 = penetrationConstraint.rb;
		if (entityA != null)
		{
			Vector3.Cross(ref entityA.angularVelocity, ref vector, out result);
			Vector3.Add(ref result, ref entityA.linearVelocity, out result);
		}
		if (entityB != null)
		{
			Vector3.Cross(ref entityB.angularVelocity, ref vector2, out result2);
			Vector3.Add(ref result2, ref entityB.linearVelocity, out result2);
		}
		Vector3.Subtract(ref result, ref result2, out var result3);
		Vector3 normal = penetrationConstraint.contact.Normal;
		float num = normal.X * result3.X + normal.Y * result3.Y + normal.Z * result3.Z;
		result3.X -= num * normal.X;
		result3.Y -= num * normal.Y;
		result3.Z -= num * normal.Z;
		float num2 = result3.LengthSquared();
		if (num2 > 1E-07f)
		{
			num2 = (float)Math.Sqrt(num2);
			linearAX = result3.X / num2;
			linearAY = result3.Y / num2;
			linearAZ = result3.Z / num2;
			friction = ((num2 > CollisionResponseSettings.StaticFrictionVelocityThreshold) ? contactManifoldConstraint.materialInteraction.KineticFriction : contactManifoldConstraint.materialInteraction.StaticFriction);
		}
		else
		{
			if (linearAX == 0f && linearAY == 0f && linearAZ == 0f)
			{
				isActiveInSolver = false;
				return;
			}
			friction = contactManifoldConstraint.materialInteraction.StaticFriction;
		}
		angularAX = vector.Y * linearAZ - vector.Z * linearAY;
		angularAY = vector.Z * linearAX - vector.X * linearAZ;
		angularAZ = vector.X * linearAY - vector.Y * linearAX;
		angularBX = linearAY * vector2.Z - linearAZ * vector2.Y;
		angularBY = linearAZ * vector2.X - linearAX * vector2.Z;
		angularBZ = linearAX * vector2.Y - linearAY * vector2.X;
		float num6;
		if (entityAIsDynamic)
		{
			float num3 = angularAX * entityA.inertiaTensorInverse.M11 + angularAY * entityA.inertiaTensorInverse.M21 + angularAZ * entityA.inertiaTensorInverse.M31;
			float num4 = angularAX * entityA.inertiaTensorInverse.M12 + angularAY * entityA.inertiaTensorInverse.M22 + angularAZ * entityA.inertiaTensorInverse.M32;
			float num5 = angularAX * entityA.inertiaTensorInverse.M13 + angularAY * entityA.inertiaTensorInverse.M23 + angularAZ * entityA.inertiaTensorInverse.M33;
			num6 = num3 * angularAX + num4 * angularAY + num5 * angularAZ + entityA.inverseMass;
		}
		else
		{
			num6 = 0f;
		}
		float num7;
		if (entityBIsDynamic)
		{
			float num3 = angularBX * entityB.inertiaTensorInverse.M11 + angularBY * entityB.inertiaTensorInverse.M21 + angularBZ * entityB.inertiaTensorInverse.M31;
			float num4 = angularBX * entityB.inertiaTensorInverse.M12 + angularBY * entityB.inertiaTensorInverse.M22 + angularBZ * entityB.inertiaTensorInverse.M32;
			float num5 = angularBX * entityB.inertiaTensorInverse.M13 + angularBY * entityB.inertiaTensorInverse.M23 + angularBZ * entityB.inertiaTensorInverse.M33;
			num7 = num3 * angularBX + num4 * angularBY + num5 * angularBZ + entityB.inverseMass;
		}
		else
		{
			num7 = 0f;
		}
		velocityToImpulse = -1f / (num6 + num7);
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
		if (entityAIsDynamic)
		{
			impulse2.X = accumulatedImpulse * angularAX;
			impulse2.Y = accumulatedImpulse * angularAY;
			impulse2.Z = accumulatedImpulse * angularAZ;
			entityA.ApplyLinearImpulse(ref impulse);
			entityA.ApplyAngularImpulse(ref impulse2);
		}
		if (entityBIsDynamic)
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
