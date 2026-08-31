using System;
using BEPUphysics.Constraints.TwoEntity.Motors;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.SingleEntity;

/// <summary>
/// Constraint which attempts to restrict the relative angular velocity of two entities to some value.
/// Can use a target relative orientation to apply additional force.
/// </summary>
public class SingleEntityAngularMotor : SingleEntityConstraint, I3DImpulseConstraintWithError, I3DImpulseConstraint
{
	private readonly JointBasis3D basis = new JointBasis3D();

	private readonly MotorSettingsOrientation settings;

	private Vector3 accumulatedImpulse;

	private float angle;

	private Vector3 axis;

	private Vector3 biasVelocity;

	private Matrix3X3 effectiveMassMatrix;

	private float maxForceDt;

	private float maxForceDtSquared;

	private float usedSoftness;

	/// <summary>
	/// Gets the basis attached to the entity.
	/// The target velocity/orientation of this motor is transformed by the basis.
	/// </summary>
	public JointBasis3D Basis => basis;

	/// <summary>
	/// Gets the motor's velocity and servo settings.
	/// </summary>
	public MotorSettingsOrientation Settings => settings;

	/// <summary>
	/// Gets the current relative velocity with respect to the constraint.
	/// For single entity constraints, this is pretty straightforward.  It is taken directly from the 
	/// entity.
	/// </summary>
	public Vector3 RelativeVelocity => -Entity.AngularVelocity;

	/// <summary>
	/// Gets the total impulse applied by this constraint.
	/// </summary>
	public Vector3 TotalImpulse => accumulatedImpulse;

	/// <summary>
	/// Gets the current constraint error.
	/// If the motor is in velocity only mode, error is zero.
	/// </summary>
	public Vector3 Error => axis * angle;

	/// <summary>
	/// Constructs a new constraint which attempts to restrict the relative angular velocity of two entities to some value.
	/// </summary>
	/// <param name="entity">Affected entity.</param>
	public SingleEntityAngularMotor(Entity entity)
	{
		Entity = entity;
		settings = new MotorSettingsOrientation(this)
		{
			servo = 
			{
				goal = base.entity.orientation
			}
		};
	}

	/// <summary>
	/// Constructs a new constraint which attempts to restrict the relative angular velocity of two entities to some value.
	/// This constructor will make the angular motor start with isActive set to false.
	/// </summary>
	public SingleEntityAngularMotor()
	{
		settings = new MotorSettingsOrientation(this);
		base.IsActive = false;
	}

	/// <summary>
	/// Applies the corrective impulses required by the constraint.
	/// </summary>
	public override float SolveIteration()
	{
		Vector3 v = default(Vector3);
		Vector3 angularVelocity = entity.angularVelocity;
		v.X = 0f - angularVelocity.X + biasVelocity.X - usedSoftness * accumulatedImpulse.X;
		v.Y = 0f - angularVelocity.Y + biasVelocity.Y - usedSoftness * accumulatedImpulse.Y;
		v.Z = 0f - angularVelocity.Z + biasVelocity.Z - usedSoftness * accumulatedImpulse.Z;
		Matrix3X3.Transform(ref v, ref effectiveMassMatrix, out v);
		Vector3 vector = accumulatedImpulse;
		accumulatedImpulse.X += v.X;
		accumulatedImpulse.Y += v.Y;
		accumulatedImpulse.Z += v.Z;
		float num = accumulatedImpulse.LengthSquared();
		if (num > maxForceDtSquared)
		{
			float num2 = maxForceDt / (float)Math.Sqrt(num);
			accumulatedImpulse.X *= num2;
			accumulatedImpulse.Y *= num2;
			accumulatedImpulse.Z *= num2;
			v.X = accumulatedImpulse.X - vector.X;
			v.Y = accumulatedImpulse.Y - vector.Y;
			v.Z = accumulatedImpulse.Z - vector.Z;
		}
		entity.ApplyAngularImpulse(ref v);
		return Math.Abs(v.X) + Math.Abs(v.Y) + Math.Abs(v.Z);
	}

	/// <summary>
	/// Initializes the constraint for the current frame.
	/// </summary>
	/// <param name="dt">Time between frames.</param>
	public override void Update(float dt)
	{
		basis.rotationMatrix = entity.orientationMatrix;
		basis.ComputeWorldSpaceAxes();
		if (settings.mode == MotorMode.Servomechanism)
		{
			Matrix matrix = Matrix3X3.ToMatrix4X4(basis.WorldTransform);
			Quaternion.CreateFromRotationMatrix(ref matrix, out var result);
			Quaternion.Conjugate(ref result, out var result2);
			Quaternion.Multiply(ref settings.servo.goal, ref result2, out result2);
			settings.servo.springSettings.ComputeErrorReductionAndSoftness(dt, out var errorReduction, out usedSoftness);
			Toolbox.GetAxisAngleFromQuaternion(ref result2, out axis, out angle);
			if (angle > 1E-05f)
			{
				float num = MathHelper.Min(settings.servo.baseCorrectiveSpeed, angle / dt) + angle * errorReduction;
				biasVelocity.X = axis.X * num;
				biasVelocity.Y = axis.Y * num;
				biasVelocity.Z = axis.Z * num;
				float num2 = biasVelocity.LengthSquared();
				if (num2 > settings.servo.maxCorrectiveVelocitySquared)
				{
					float num3 = settings.servo.maxCorrectiveVelocity / (float)Math.Sqrt(num2);
					biasVelocity.X *= num3;
					biasVelocity.Y *= num3;
					biasVelocity.Z *= num3;
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
			angle = 0f;
			Matrix3X3 matrix2 = basis.WorldTransform;
			Matrix3X3.Transform(ref settings.velocityMotor.goalVelocity, ref matrix2, out biasVelocity);
		}
		effectiveMassMatrix = entity.inertiaTensorInverse;
		effectiveMassMatrix.M11 += usedSoftness;
		effectiveMassMatrix.M22 += usedSoftness;
		effectiveMassMatrix.M33 += usedSoftness;
		Matrix3X3.Invert(ref effectiveMassMatrix, out effectiveMassMatrix);
		ComputeMaxForces(settings.maximumForce, dt);
	}

	/// <summary>
	/// Performs any pre-solve iteration work that needs exclusive
	/// access to the members of the solver updateable.
	/// Usually, this is used for applying warmstarting impulses.
	/// </summary>
	public override void ExclusiveUpdate()
	{
		entity.ApplyAngularImpulse(ref accumulatedImpulse);
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
