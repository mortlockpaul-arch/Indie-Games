using System;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.TwoEntity.Motors;

/// <summary>
/// Constraint which attempts to restrict the relative angular motion of two entities.
/// Can use a target relative orientation to apply additional force.
/// </summary>
public class AngularMotor : Motor, I3DImpulseConstraintWithError, I3DImpulseConstraint, I3DJacobianConstraint
{
	private readonly JointBasis3D basis = new JointBasis3D();

	private readonly MotorSettingsOrientation settings;

	private Vector3 accumulatedImpulse;

	private float angle;

	private Vector3 axis;

	private Vector3 biasVelocity;

	private Matrix3X3 effectiveMassMatrix;

	/// <summary>
	/// Gets the basis attached to entity A.
	/// The target velocity/orientation of this motor is transformed by the basis.
	/// </summary>
	public JointBasis3D Basis => basis;

	/// <summary>
	/// Gets the motor's velocity and servo settings.
	/// </summary>
	public MotorSettingsOrientation Settings => settings;

	/// <summary>
	/// Gets the current relative velocity between the connected entities with respect to the constraint.
	/// </summary>
	public Vector3 RelativeVelocity => connectionA.angularVelocity - connectionB.angularVelocity;

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
	/// Constructs a new constraint which attempts to restrict the relative angular motion of two entities.
	/// To finish the initialization, specify the connections (ConnectionA and ConnectionB).
	/// This constructor sets the constraint's IsActive property to false by default.
	/// </summary>
	public AngularMotor()
	{
		base.IsActive = false;
		settings = new MotorSettingsOrientation(this);
	}

	/// <summary>
	/// Constructs a new constraint which attempts to restrict the relative angular motion of two entities.
	/// </summary>
	/// <param name="connectionA">First connection of the pair.</param>
	/// <param name="connectionB">Second connection of the pair.</param>
	public AngularMotor(Entity connectionA, Entity connectionB)
	{
		base.ConnectionA = connectionA;
		base.ConnectionB = connectionB;
		settings = new MotorSettingsOrientation(this);
		Quaternion.Conjugate(ref base.connectionB.orientation, out var result);
		Quaternion.Multiply(ref base.connectionA.orientation, ref result, out settings.servo.goal);
	}

	/// <summary>
	/// Gets the linear jacobian entry for the first connected entity.
	/// </summary>
	/// <param name="jacobianX">First linear jacobian entry for the first connected entity.</param>
	/// <param name="jacobianY">Second linear jacobian entry for the first connected entity.</param>
	/// <param name="jacobianZ">Third linear jacobian entry for the first connected entity.</param>
	public void GetLinearJacobianA(out Vector3 jacobianX, out Vector3 jacobianY, out Vector3 jacobianZ)
	{
		jacobianX = Toolbox.ZeroVector;
		jacobianY = Toolbox.ZeroVector;
		jacobianZ = Toolbox.ZeroVector;
	}

	/// <summary>
	/// Gets the linear jacobian entry for the second connected entity.
	/// </summary>
	/// <param name="jacobianX">First linear jacobian entry for the second connected entity.</param>
	/// <param name="jacobianY">Second linear jacobian entry for the second connected entity.</param>
	/// <param name="jacobianZ">Third linear jacobian entry for the second connected entity.</param>
	public void GetLinearJacobianB(out Vector3 jacobianX, out Vector3 jacobianY, out Vector3 jacobianZ)
	{
		jacobianX = Toolbox.ZeroVector;
		jacobianY = Toolbox.ZeroVector;
		jacobianZ = Toolbox.ZeroVector;
	}

	/// <summary>
	/// Gets the angular jacobian entry for the first connected entity.
	/// </summary>
	/// <param name="jacobianX">First angular jacobian entry for the first connected entity.</param>
	/// <param name="jacobianY">Second angular jacobian entry for the first connected entity.</param>
	/// <param name="jacobianZ">Third angular jacobian entry for the first connected entity.</param>
	public void GetAngularJacobianA(out Vector3 jacobianX, out Vector3 jacobianY, out Vector3 jacobianZ)
	{
		jacobianX = Toolbox.RightVector;
		jacobianY = Toolbox.UpVector;
		jacobianZ = Toolbox.BackVector;
	}

	/// <summary>
	/// Gets the angular jacobian entry for the second connected entity.
	/// </summary>
	/// <param name="jacobianX">First angular jacobian entry for the second connected entity.</param>
	/// <param name="jacobianY">Second angular jacobian entry for the second connected entity.</param>
	/// <param name="jacobianZ">Third angular jacobian entry for the second connected entity.</param>
	public void GetAngularJacobianB(out Vector3 jacobianX, out Vector3 jacobianY, out Vector3 jacobianZ)
	{
		jacobianX = Toolbox.RightVector;
		jacobianY = Toolbox.UpVector;
		jacobianZ = Toolbox.BackVector;
	}

	/// <summary>
	/// Gets the mass matrix of the constraint.
	/// </summary>
	/// <param name="outputMassMatrix">Constraint's mass matrix.</param>
	public void GetMassMatrix(out Matrix3X3 outputMassMatrix)
	{
		outputMassMatrix = effectiveMassMatrix;
	}

	/// <summary>
	/// Applies the corrective impulses required by the constraint.
	/// </summary>
	public override float SolveIteration()
	{
		Vector3 v = default(Vector3);
		Vector3 angularVelocity = connectionA.angularVelocity;
		Vector3 angularVelocity2 = connectionB.angularVelocity;
		v.X = angularVelocity2.X - angularVelocity.X - biasVelocity.X - usedSoftness * accumulatedImpulse.X;
		v.Y = angularVelocity2.Y - angularVelocity.Y - biasVelocity.Y - usedSoftness * accumulatedImpulse.Y;
		v.Z = angularVelocity2.Z - angularVelocity.Z - biasVelocity.Z - usedSoftness * accumulatedImpulse.Z;
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
		if (connectionA.isDynamic)
		{
			connectionA.ApplyAngularImpulse(ref v);
		}
		if (connectionB.isDynamic)
		{
			Vector3.Negate(ref v, out var result);
			connectionB.ApplyAngularImpulse(ref result);
		}
		return Math.Abs(v.X) + Math.Abs(v.Y) + Math.Abs(v.Z);
	}

	/// <summary>
	/// Initializes the constraint for the current frame.
	/// </summary>
	/// <param name="dt">Time between frames.</param>
	public override void Update(float dt)
	{
		basis.rotationMatrix = connectionA.orientationMatrix;
		basis.ComputeWorldSpaceAxes();
		if (settings.mode == MotorMode.Servomechanism)
		{
			Matrix matrix = Matrix3X3.ToMatrix4X4(basis.WorldTransform);
			Quaternion.CreateFromRotationMatrix(ref matrix, out var result);
			Quaternion.Conjugate(ref connectionB.orientation, out var result2);
			Quaternion.Multiply(ref result, ref result2, out var result3);
			Quaternion.Conjugate(ref result, out var result4);
			Quaternion.Multiply(ref settings.servo.goal, ref result4, out var result5);
			Quaternion.Multiply(ref result, ref result5, out result5);
			Quaternion.Multiply(ref result5, ref result3, out result3);
			settings.servo.springSettings.ComputeErrorReductionAndSoftness(dt, out var errorReduction, out usedSoftness);
			Toolbox.GetAxisAngleFromQuaternion(ref result3, out axis, out angle);
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
				biasVelocity.X = 0f;
				biasVelocity.Y = 0f;
				biasVelocity.Z = 0f;
			}
		}
		else
		{
			usedSoftness = settings.velocityMotor.softness / dt;
			angle = 0f;
			Matrix3X3 matrix2 = basis.WorldTransform;
			Matrix3X3.Transform(ref settings.velocityMotor.goalVelocity, ref matrix2, out biasVelocity);
		}
		Matrix3X3.Add(ref connectionA.inertiaTensorInverse, ref connectionB.inertiaTensorInverse, out effectiveMassMatrix);
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
		if (connectionA.isDynamic)
		{
			connectionA.ApplyAngularImpulse(ref accumulatedImpulse);
		}
		if (connectionB.isDynamic)
		{
			Vector3.Negate(ref accumulatedImpulse, out var result);
			connectionB.ApplyAngularImpulse(ref result);
		}
	}
}
