using System;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using BEPUphysics.Settings;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.Collision;

/// <summary>
/// Computes the forces to slow down and stop sliding motion between two entities when centralized friction is active.
/// </summary>
public class SlidingFrictionTwoAxis : EntitySolverUpdateable
{
	private ConvexContactManifoldConstraint contactManifoldConstraint;

	internal Vector2 accumulatedImpulse;

	internal Matrix2X3 angularA;

	internal Matrix2X3 angularB;

	private int contactCount;

	private float friction;

	internal Matrix2X3 linearA;

	private Entity entityA;

	private Entity entityB;

	private bool entityADynamic;

	private bool entityBDynamic;

	private Vector3 ra;

	private Vector3 rb;

	private Matrix2X2 velocityToImpulse;

	internal Vector3 manifoldCenter;

	internal Vector3 relativeVelocity;

	/// <summary>
	///  Gets the contact manifold constraint that owns this constraint.
	/// </summary>
	public ConvexContactManifoldConstraint ContactManifoldConstraint => contactManifoldConstraint;

	/// <summary>
	/// Gets the first direction in which the friction force acts.
	/// This is one of two directions that are perpendicular to each other and the normal of a collision between two entities.
	/// </summary>
	public Vector3 FrictionDirectionX => new Vector3(linearA.M11, linearA.M12, linearA.M13);

	/// <summary>
	/// Gets the second direction in which the friction force acts.
	/// This is one of two directions that are perpendicular to each other and the normal of a collision between two entities.
	/// </summary>
	public Vector3 FrictionDirectionY => new Vector3(linearA.M21, linearA.M22, linearA.M23);

	/// <summary>
	/// Gets the total impulse applied by sliding friction in the last time step.
	/// The X component of this vector is the force applied along the frictionDirectionX,
	/// while the Y component is the force applied along the frictionDirectionY.
	/// </summary>
	public Vector2 TotalImpulse => accumulatedImpulse;

	/// <summary>
	///  Gets the tangential relative velocity between the associated entities at the contact point.
	/// </summary>
	public Vector2 RelativeVelocity
	{
		get
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			if (entityA != null)
			{
				num = entityA.linearVelocity.X + entityA.angularVelocity.Y * ra.Z - entityA.angularVelocity.Z * ra.Y;
				num2 = entityA.linearVelocity.Y + entityA.angularVelocity.Z * ra.X - entityA.angularVelocity.X * ra.Z;
				num3 = entityA.linearVelocity.Z + entityA.angularVelocity.X * ra.Y - entityA.angularVelocity.Y * ra.X;
			}
			if (entityB != null)
			{
				num += 0f - entityB.linearVelocity.X - entityB.angularVelocity.Y * rb.Z + entityB.angularVelocity.Z * rb.Y;
				num2 += 0f - entityB.linearVelocity.Y - entityB.angularVelocity.Z * rb.X + entityB.angularVelocity.X * rb.Z;
				num3 += 0f - entityB.linearVelocity.Z - entityB.angularVelocity.X * rb.Y + entityB.angularVelocity.Y * rb.X;
			}
			return new Vector2
			{
				X = num * linearA.M11 + num2 * linearA.M12 + num3 * linearA.M13,
				Y = num * linearA.M21 + num2 * linearA.M22 + num3 * linearA.M23
			};
		}
	}

	/// <summary>
	///  Constructs a new sliding friction constraint.
	/// </summary>
	public SlidingFrictionTwoAxis()
	{
		isActive = false;
	}

	/// <summary>
	/// Computes one iteration of the constraint to meet the solver updateable's goal.
	/// </summary>
	/// <returns>The rough applied impulse magnitude.</returns>
	public override float SolveIteration()
	{
		Vector2 vector = RelativeVelocity;
		float x = vector.X;
		vector.X = x * velocityToImpulse.M11 + vector.Y * velocityToImpulse.M21;
		vector.Y = x * velocityToImpulse.M12 + vector.Y * velocityToImpulse.M22;
		Vector2 vector2 = accumulatedImpulse;
		accumulatedImpulse.X += vector.X;
		accumulatedImpulse.Y += vector.Y;
		float num = accumulatedImpulse.LengthSquared();
		float num2 = 0f;
		for (int i = 0; i < contactCount; i++)
		{
			num2 += contactManifoldConstraint.penetrationConstraints.Elements[i].accumulatedImpulse;
		}
		num2 *= friction;
		if (num > num2 * num2)
		{
			num = num2 / (float)Math.Sqrt(num);
			accumulatedImpulse.X *= num;
			accumulatedImpulse.Y *= num;
		}
		vector.X = accumulatedImpulse.X - vector2.X;
		vector.Y = accumulatedImpulse.Y - vector2.Y;
		Vector3 impulse = default(Vector3);
		Vector3 impulse2 = default(Vector3);
		impulse.X = vector.X * linearA.M11 + vector.Y * linearA.M21;
		impulse.Y = vector.X * linearA.M12 + vector.Y * linearA.M22;
		impulse.Z = vector.X * linearA.M13 + vector.Y * linearA.M23;
		if (entityADynamic)
		{
			impulse2.X = vector.X * angularA.M11 + vector.Y * angularA.M21;
			impulse2.Y = vector.X * angularA.M12 + vector.Y * angularA.M22;
			impulse2.Z = vector.X * angularA.M13 + vector.Y * angularA.M23;
			entityA.ApplyLinearImpulse(ref impulse);
			entityA.ApplyAngularImpulse(ref impulse2);
		}
		if (entityBDynamic)
		{
			impulse.X = 0f - impulse.X;
			impulse.Y = 0f - impulse.Y;
			impulse.Z = 0f - impulse.Z;
			impulse2.X = vector.X * angularB.M11 + vector.Y * angularB.M21;
			impulse2.Y = vector.X * angularB.M12 + vector.Y * angularB.M22;
			impulse2.Z = vector.X * angularB.M13 + vector.Y * angularB.M23;
			entityB.ApplyLinearImpulse(ref impulse);
			entityB.ApplyAngularImpulse(ref impulse2);
		}
		return Math.Abs(vector.X) + Math.Abs(vector.Y);
	}

	/// <summary>
	///  Performs the frame's configuration step.
	/// </summary>
	/// <param name="dt">Timestep duration.</param>
	public override void Update(float dt)
	{
		entityADynamic = entityA != null && entityA.isDynamic;
		entityBDynamic = entityB != null && entityB.isDynamic;
		contactCount = contactManifoldConstraint.penetrationConstraints.count;
		switch (contactCount)
		{
		case 1:
			manifoldCenter = contactManifoldConstraint.penetrationConstraints.Elements[0].contact.Position;
			break;
		case 2:
			Vector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[0].contact.Position, ref contactManifoldConstraint.penetrationConstraints.Elements[1].contact.Position, out manifoldCenter);
			manifoldCenter.X *= 0.5f;
			manifoldCenter.Y *= 0.5f;
			manifoldCenter.Z *= 0.5f;
			break;
		case 3:
			Vector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[0].contact.Position, ref contactManifoldConstraint.penetrationConstraints.Elements[1].contact.Position, out manifoldCenter);
			Vector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[2].contact.Position, ref manifoldCenter, out manifoldCenter);
			manifoldCenter.X *= 1f / 3f;
			manifoldCenter.Y *= 1f / 3f;
			manifoldCenter.Z *= 1f / 3f;
			break;
		case 4:
			Vector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[0].contact.Position, ref contactManifoldConstraint.penetrationConstraints.Elements[1].contact.Position, out manifoldCenter);
			Vector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[2].contact.Position, ref manifoldCenter, out manifoldCenter);
			Vector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[3].contact.Position, ref manifoldCenter, out manifoldCenter);
			manifoldCenter.X *= 0.25f;
			manifoldCenter.Y *= 0.25f;
			manifoldCenter.Z *= 0.25f;
			break;
		default:
			manifoldCenter = Toolbox.NoVector;
			break;
		}
		Vector3 result;
		if (entityA != null)
		{
			Vector3.Subtract(ref manifoldCenter, ref entityA.position, out ra);
			Vector3.Cross(ref entityA.angularVelocity, ref ra, out result);
			Vector3.Add(ref result, ref entityA.linearVelocity, out result);
		}
		else
		{
			result = default(Vector3);
		}
		Vector3 result2;
		if (entityB != null)
		{
			Vector3.Subtract(ref manifoldCenter, ref entityB.position, out rb);
			Vector3.Cross(ref entityB.angularVelocity, ref rb, out result2);
			Vector3.Add(ref result2, ref entityB.linearVelocity, out result2);
		}
		else
		{
			result2 = default(Vector3);
		}
		Vector3.Subtract(ref result, ref result2, out relativeVelocity);
		Vector3 vector = contactManifoldConstraint.penetrationConstraints.Elements[0].contact.Normal;
		float num = vector.X * relativeVelocity.X + vector.Y * relativeVelocity.Y + vector.Z * relativeVelocity.Z;
		relativeVelocity.X -= num * vector.X;
		relativeVelocity.Y -= num * vector.Y;
		relativeVelocity.Z -= num * vector.Z;
		float num2 = relativeVelocity.LengthSquared();
		if (num2 > 1E-07f)
		{
			num2 = (float)Math.Sqrt(num2);
			float num3 = 1f / num2;
			linearA.M11 = relativeVelocity.X * num3;
			linearA.M12 = relativeVelocity.Y * num3;
			linearA.M13 = relativeVelocity.Z * num3;
			friction = ((num2 > CollisionResponseSettings.StaticFrictionVelocityThreshold) ? contactManifoldConstraint.materialInteraction.KineticFriction : contactManifoldConstraint.materialInteraction.StaticFriction);
		}
		else
		{
			friction = contactManifoldConstraint.materialInteraction.StaticFriction;
			if (linearA.M11 == 0f && linearA.M12 == 0f && linearA.M13 == 0f)
			{
				Vector3.Cross(ref vector, ref Toolbox.RightVector, out var result3);
				num2 = result3.LengthSquared();
				if (num2 > 1E-07f)
				{
					num2 = (float)Math.Sqrt(num2);
					float num4 = 1f / num2;
					linearA.M11 = result3.X * num4;
					linearA.M12 = result3.Y * num4;
					linearA.M13 = result3.Z * num4;
				}
				else
				{
					Vector3.Cross(ref vector, ref Toolbox.UpVector, out result3);
					result3.Normalize();
					linearA.M11 = result3.X;
					linearA.M12 = result3.Y;
					linearA.M13 = result3.Z;
				}
			}
		}
		linearA.M21 = linearA.M12 * vector.Z - linearA.M13 * vector.Y;
		linearA.M22 = linearA.M13 * vector.X - linearA.M11 * vector.Z;
		linearA.M23 = linearA.M11 * vector.Y - linearA.M12 * vector.X;
		if (entityA != null)
		{
			angularA.M11 = ra.Y * linearA.M13 - ra.Z * linearA.M12;
			angularA.M12 = ra.Z * linearA.M11 - ra.X * linearA.M13;
			angularA.M13 = ra.X * linearA.M12 - ra.Y * linearA.M11;
			angularA.M21 = ra.Y * linearA.M23 - ra.Z * linearA.M22;
			angularA.M22 = ra.Z * linearA.M21 - ra.X * linearA.M23;
			angularA.M23 = ra.X * linearA.M22 - ra.Y * linearA.M21;
		}
		if (entityB != null)
		{
			angularB.M11 = linearA.M12 * rb.Z - linearA.M13 * rb.Y;
			angularB.M12 = linearA.M13 * rb.X - linearA.M11 * rb.Z;
			angularB.M13 = linearA.M11 * rb.Y - linearA.M12 * rb.X;
			angularB.M21 = linearA.M22 * rb.Z - linearA.M23 * rb.Y;
			angularB.M22 = linearA.M23 * rb.X - linearA.M21 * rb.Z;
			angularB.M23 = linearA.M21 * rb.Y - linearA.M22 * rb.X;
		}
		Matrix2X3 result4;
		Matrix3X2 result5;
		Matrix2X2 result6;
		if (entityADynamic)
		{
			Matrix2X3.Multiply(ref angularA, ref entityA.inertiaTensorInverse, out result4);
			Matrix2X3.Transpose(ref angularA, out result5);
			Matrix2X2.Multiply(ref result4, ref result5, out result6);
			result6.M11 += entityA.inverseMass;
			result6.M22 += entityA.inverseMass;
		}
		else
		{
			result6 = default(Matrix2X2);
		}
		Matrix2X2 result7;
		if (entityBDynamic)
		{
			Matrix2X3.Multiply(ref angularB, ref entityB.inertiaTensorInverse, out result4);
			Matrix2X3.Transpose(ref angularB, out result5);
			Matrix2X2.Multiply(ref result4, ref result5, out result7);
			result7.M11 += entityB.inverseMass;
			result7.M22 += entityB.inverseMass;
		}
		else
		{
			result7 = default(Matrix2X2);
		}
		velocityToImpulse.M11 = 0f - result6.M11 - result7.M11;
		velocityToImpulse.M12 = 0f - result6.M12 - result7.M12;
		velocityToImpulse.M21 = 0f - result6.M21 - result7.M21;
		velocityToImpulse.M22 = 0f - result6.M22 - result7.M22;
		Matrix2X2.Invert(ref velocityToImpulse, out velocityToImpulse);
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
		impulse.X = accumulatedImpulse.X * linearA.M11 + accumulatedImpulse.Y * linearA.M21;
		impulse.Y = accumulatedImpulse.X * linearA.M12 + accumulatedImpulse.Y * linearA.M22;
		impulse.Z = accumulatedImpulse.X * linearA.M13 + accumulatedImpulse.Y * linearA.M23;
		if (entityADynamic)
		{
			impulse2.X = accumulatedImpulse.X * angularA.M11 + accumulatedImpulse.Y * angularA.M21;
			impulse2.Y = accumulatedImpulse.X * angularA.M12 + accumulatedImpulse.Y * angularA.M22;
			impulse2.Z = accumulatedImpulse.X * angularA.M13 + accumulatedImpulse.Y * angularA.M23;
			entityA.ApplyLinearImpulse(ref impulse);
			entityA.ApplyAngularImpulse(ref impulse2);
		}
		if (entityBDynamic)
		{
			impulse.X = 0f - impulse.X;
			impulse.Y = 0f - impulse.Y;
			impulse.Z = 0f - impulse.Z;
			impulse2.X = accumulatedImpulse.X * angularB.M11 + accumulatedImpulse.Y * angularB.M21;
			impulse2.Y = accumulatedImpulse.X * angularB.M12 + accumulatedImpulse.Y * angularB.M22;
			impulse2.Z = accumulatedImpulse.X * angularB.M13 + accumulatedImpulse.Y * angularB.M23;
			entityB.ApplyLinearImpulse(ref impulse);
			entityB.ApplyAngularImpulse(ref impulse2);
		}
	}

	internal void Setup(ConvexContactManifoldConstraint contactManifoldConstraint)
	{
		this.contactManifoldConstraint = contactManifoldConstraint;
		isActive = true;
		linearA = default(Matrix2X3);
		entityA = contactManifoldConstraint.EntityA;
		entityB = contactManifoldConstraint.EntityB;
	}

	internal void CleanUp()
	{
		accumulatedImpulse = default(Vector2);
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
