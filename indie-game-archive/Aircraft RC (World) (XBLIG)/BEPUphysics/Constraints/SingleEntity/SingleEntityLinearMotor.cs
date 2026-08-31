using System;
using BEPUphysics.Constraints.TwoEntity.Motors;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.SingleEntity;

/// <summary>
/// Constraint which tries to push an entity to a desired location.
/// </summary>
public class SingleEntityLinearMotor : SingleEntityConstraint, I3DImpulseConstraintWithError, I3DImpulseConstraint
{
	private readonly MotorSettings3D settings;

	/// <summary>
	/// Sum of forces applied to the constraint in the past.
	/// </summary>
	private Vector3 accumulatedImpulse = Vector3.Zero;

	private Vector3 biasVelocity;

	private Matrix3X3 effectiveMassMatrix;

	/// <summary>
	/// Maximum impulse that can be applied in a single frame.
	/// </summary>
	private float maxForceDt;

	/// <summary>
	/// Maximum impulse that can be applied in a single frame, squared.
	/// This is computed in the prestep to avoid doing extra multiplies in the more-often called applyImpulse method.
	/// </summary>
	private float maxForceDtSquared;

	private Vector3 error;

	private Vector3 localPoint;

	private Vector3 worldPoint;

	private Vector3 r;

	private float usedSoftness;

	/// <summary>
	/// Gets or sets the entity affected by the constraint.
	/// </summary>
	public override Entity Entity
	{
		get
		{
			return base.Entity;
		}
		set
		{
			if (Entity != value)
			{
				accumulatedImpulse = default(Vector3);
			}
			base.Entity = value;
		}
	}

	/// <summary>
	/// Point attached to the entity in its local space that is motorized.
	/// </summary>
	public Vector3 LocalPoint
	{
		get
		{
			return localPoint;
		}
		set
		{
			localPoint = value;
			Matrix3X3.Transform(ref localPoint, ref entity.orientationMatrix, out worldPoint);
			Vector3.Add(ref worldPoint, ref entity.position, out worldPoint);
		}
	}

	/// <summary>
	/// Point attached to the entity in world space that is motorized.
	/// </summary>
	public Vector3 Point
	{
		get
		{
			return worldPoint;
		}
		set
		{
			worldPoint = value;
			Vector3.Subtract(ref worldPoint, ref entity.position, out localPoint);
			Matrix3X3.TransformTranspose(ref localPoint, ref entity.orientationMatrix, out localPoint);
		}
	}

	/// <summary>
	/// Gets the motor's velocity and servo settings.
	/// </summary>
	public MotorSettings3D Settings => settings;

	/// <summary>
	/// Gets the current relative velocity between the connected entities with respect to the constraint.
	/// </summary>
	public Vector3 RelativeVelocity
	{
		get
		{
			Vector3.Cross(ref r, ref entity.angularVelocity, out var result);
			Vector3.Subtract(ref result, ref entity.linearVelocity, out result);
			return result;
		}
	}

	/// <summary>
	/// Gets the total impulse applied by this constraint.
	/// </summary>
	public Vector3 TotalImpulse => accumulatedImpulse;

	/// <summary>
	/// Gets the current constraint error.
	/// If the motor is in velocity only mode, error is zero.
	/// </summary>
	public Vector3 Error => error;

	/// <summary>
	/// Constructs a new single body linear motor.  This motor will try to move a single entity to a goal velocity or to a goal position.
	/// </summary>
	/// <param name="entity">Entity to affect.</param>
	/// <param name="point">Point in world space attached to the entity that will be motorized.</param>
	public SingleEntityLinearMotor(Entity entity, Vector3 point)
	{
		Entity = entity;
		Point = point;
		settings = new MotorSettings3D(this)
		{
			servo = 
			{
				goal = point
			}
		};
	}

	/// <summary>
	/// Constructs a new single body linear motor.  This motor will try to move a single entity to a goal velocity or to a goal position.
	/// This constructor will start the motor with isActive = false.
	/// </summary>
	public SingleEntityLinearMotor()
	{
		settings = new MotorSettings3D(this);
		base.IsActive = false;
	}

	/// <summary>
	/// Computes one iteration of the constraint to meet the solver updateable's goal.
	/// </summary>
	/// <returns>The rough applied impulse magnitude.</returns>
	public override float SolveIteration()
	{
		Vector3.Cross(ref r, ref entity.angularVelocity, out var result);
		Vector3.Subtract(ref result, ref entity.linearVelocity, out result);
		Vector3.Add(ref biasVelocity, ref result, out result);
		Vector3.Multiply(ref accumulatedImpulse, usedSoftness, out var result2);
		Vector3.Subtract(ref result, ref result2, out result);
		Matrix3X3.Transform(ref result, ref effectiveMassMatrix, out result);
		Vector3 vector = accumulatedImpulse;
		accumulatedImpulse += result;
		float num = accumulatedImpulse.LengthSquared();
		if (num > maxForceDtSquared)
		{
			accumulatedImpulse *= maxForceDt / (float)Math.Sqrt(num);
			result = accumulatedImpulse - vector;
		}
		entity.ApplyLinearImpulse(ref result);
		Vector3.Cross(ref r, ref result, out var result3);
		entity.ApplyAngularImpulse(ref result3);
		return Math.Abs(result.X) + Math.Abs(result.Y) + Math.Abs(result.Z);
	}

	/// <summary>
	///  Performs the frame's configuration step.
	/// </summary>
	/// <param name="dt">Timestep duration.</param>
	public override void Update(float dt)
	{
		Matrix3X3.Transform(ref localPoint, ref entity.orientationMatrix, out r);
		Vector3.Add(ref r, ref entity.position, out worldPoint);
		if (settings.mode == MotorMode.Servomechanism)
		{
			Vector3.Subtract(ref settings.servo.goal, ref worldPoint, out error);
			float num = error.Length();
			if (num > 1E-05f)
			{
				settings.servo.springSettings.ComputeErrorReductionAndSoftness(dt, out var errorReduction, out usedSoftness);
				float num2 = MathHelper.Min(settings.servo.baseCorrectiveSpeed, num / dt) + num * errorReduction;
				Vector3.Multiply(ref error, num2 / num, out biasVelocity);
				float num3 = biasVelocity.LengthSquared();
				if (num3 > settings.servo.maxCorrectiveVelocitySquared)
				{
					float num4 = settings.servo.maxCorrectiveVelocity / (float)Math.Sqrt(num3);
					biasVelocity.X *= num4;
					biasVelocity.Y *= num4;
					biasVelocity.Z *= num4;
				}
			}
			else
			{
				biasVelocity = default(Vector3);
			}
		}
		else
		{
			usedSoftness = settings.velocityMotor.softness / dt;
			biasVelocity = settings.velocityMotor.goalVelocity;
			error = Vector3.Zero;
		}
		ComputeMaxForces(settings.maximumForce, dt);
		Matrix3X3.CreateScale(entity.inverseMass, out var matrix);
		Matrix3X3.CreateCrossProduct(ref r, out var result);
		Matrix3X3.Multiply(ref result, ref entity.inertiaTensorInverse, out var result2);
		Matrix3X3.Multiply(ref result2, ref result, out result2);
		Matrix3X3.Subtract(ref matrix, ref result2, out effectiveMassMatrix);
		effectiveMassMatrix.M11 += usedSoftness;
		effectiveMassMatrix.M22 += usedSoftness;
		effectiveMassMatrix.M33 += usedSoftness;
		Matrix3X3.Invert(ref effectiveMassMatrix, out effectiveMassMatrix);
	}

	/// <summary>
	/// Performs any pre-solve iteration work that needs exclusive
	/// access to the members of the solver updateable.
	/// Usually, this is used for applying warmstarting impulses.
	/// </summary>
	public override void ExclusiveUpdate()
	{
		entity.ApplyLinearImpulse(ref accumulatedImpulse);
		Vector3.Cross(ref r, ref accumulatedImpulse, out var result);
		entity.ApplyAngularImpulse(ref result);
	}

	/// <summary>
	/// Computes the maxForceDt and maxForceDtSquared fields.
	/// </summary>
	private void ComputeMaxForces(float maxForce, float dt)
	{
		if (maxForce < float.MaxValue)
		{
			maxForceDt = maxForce * dt;
			maxForceDtSquared = maxForceDt * maxForceDt;
		}
		else
		{
			maxForceDt = float.MaxValue;
			maxForceDtSquared = float.MaxValue;
		}
	}
}
