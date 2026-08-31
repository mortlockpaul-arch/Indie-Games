using System;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.TwoEntity.Motors;

/// <summary>
/// Tries to rotate two entities so that they reach a specified relative orientation or speed around an axis.
/// </summary>
public class RevoluteMotor : Motor, I1DImpulseConstraintWithError, I1DImpulseConstraint, I1DJacobianConstraint
{
	private readonly JointBasis2D basis = new JointBasis2D();

	private readonly MotorSettings1D settings;

	private float accumulatedImpulse;

	protected float biasVelocity;

	private Vector3 jacobianA;

	private Vector3 jacobianB;

	private float error;

	private Vector3 localTestAxis;

	private Vector3 worldTestAxis;

	private float velocityToImpulse;

	/// <summary>
	/// Gets the basis attached to entity A.
	/// The primary axis represents the limited axis of rotation.  The 'measurement plane' which the test axis is tested against is based on this primary axis.
	/// The x axis defines the 'base' direction on the measurement plane corresponding to 0 degrees of relative rotation.
	/// </summary>
	public JointBasis2D Basis => basis;

	/// <summary>
	/// Gets or sets the axis attached to entity B in its local space.
	/// This axis is projected onto the x and y axes of transformA to determine the hinge angle.
	/// </summary>
	public Vector3 LocalTestAxis
	{
		get
		{
			return localTestAxis;
		}
		set
		{
			localTestAxis = Vector3.Normalize(value);
			Matrix3X3.Transform(ref localTestAxis, ref connectionB.orientationMatrix, out worldTestAxis);
		}
	}

	/// <summary>
	/// Gets the motor's velocity and servo settings.
	/// </summary>
	public MotorSettings1D Settings => settings;

	/// <summary>
	/// Gets or sets the axis attached to entity B in world space.
	/// This axis is projected onto the x and y axes of the Basis attached to entity A to determine the hinge angle.
	/// </summary>
	public Vector3 TestAxis
	{
		get
		{
			return worldTestAxis;
		}
		set
		{
			worldTestAxis = Vector3.Normalize(value);
			Matrix3X3.TransformTranspose(ref worldTestAxis, ref connectionB.orientationMatrix, out localTestAxis);
		}
	}

	/// <summary>
	/// Gets the current relative velocity between the connected entities with respect to the constraint.
	/// </summary>
	public float RelativeVelocity
	{
		get
		{
			Vector3.Dot(ref connectionA.angularVelocity, ref jacobianA, out var result);
			Vector3.Dot(ref connectionB.angularVelocity, ref jacobianB, out var result2);
			return result + result2;
		}
	}

	/// <summary>
	/// Gets the total impulse applied by this constraint.
	/// </summary>
	public float TotalImpulse => accumulatedImpulse;

	/// <summary>
	/// Gets the current constraint error.
	/// If the motor is in velocity only mode, the error is zero.
	/// </summary>
	public float Error => error;

	/// <summary>
	/// Constructs a new constraint tries to rotate two entities so that they reach a specified relative orientation around an axis.
	/// To finish the initialization, specify the connections (ConnectionA and ConnectionB) 
	/// as well as the Basis and TestAxis.
	/// This constructor sets the constraint's IsActive property to false by default.
	/// </summary>
	public RevoluteMotor()
	{
		settings = new MotorSettings1D(this);
		base.IsActive = false;
	}

	/// <summary>
	/// Constructs a new constraint tries to rotate two entities so that they reach a specified relative orientation around an axis.
	/// </summary>
	/// <param name="connectionA">First connection of the pair.</param>
	/// <param name="connectionB">Second connection of the pair.</param>
	/// <param name="motorizedAxis">Rotation axis to control in world space.</param>
	public RevoluteMotor(Entity connectionA, Entity connectionB, Vector3 motorizedAxis)
	{
		base.ConnectionA = connectionA;
		base.ConnectionB = connectionB;
		SetupJointTransforms(motorizedAxis);
		settings = new MotorSettings1D(this);
	}

	/// <summary>
	/// Gets the linear jacobian entry for the first connected entity.
	/// </summary>
	/// <param name="jacobian">Linear jacobian entry for the first connected entity.</param>
	public void GetLinearJacobianA(out Vector3 jacobian)
	{
		jacobian = Toolbox.ZeroVector;
	}

	/// <summary>
	/// Gets the linear jacobian entry for the second connected entity.
	/// </summary>
	/// <param name="jacobian">Linear jacobian entry for the second connected entity.</param>
	public void GetLinearJacobianB(out Vector3 jacobian)
	{
		jacobian = Toolbox.ZeroVector;
	}

	/// <summary>
	/// Gets the angular jacobian entry for the first connected entity.
	/// </summary>
	/// <param name="jacobian">Angular jacobian entry for the first connected entity.</param>
	public void GetAngularJacobianA(out Vector3 jacobian)
	{
		jacobian = jacobianA;
	}

	/// <summary>
	/// Gets the angular jacobian entry for the second connected entity.
	/// </summary>
	/// <param name="jacobian">Angular jacobian entry for the second connected entity.</param>
	public void GetAngularJacobianB(out Vector3 jacobian)
	{
		jacobian = jacobianB;
	}

	/// <summary>
	/// Gets the mass matrix of the constraint.
	/// </summary>
	/// <param name="outputMassMatrix">Constraint's mass matrix.</param>
	public void GetMassMatrix(out float outputMassMatrix)
	{
		outputMassMatrix = velocityToImpulse;
	}

	/// <summary>
	/// Sets up the joint transforms by automatically creating perpendicular vectors to complete the bases.
	/// </summary>
	/// <param name="motorizedAxis">Axis around which the motor acts.</param>
	public void SetupJointTransforms(Vector3 motorizedAxis)
	{
		Vector3.Cross(ref motorizedAxis, ref Toolbox.UpVector, out var result);
		float num = result.LengthSquared();
		if (num < 1E-07f)
		{
			Vector3.Cross(ref motorizedAxis, ref Toolbox.RightVector, out result);
		}
		basis.rotationMatrix = connectionA.orientationMatrix;
		basis.SetWorldAxes(motorizedAxis, result);
		TestAxis = basis.xAxis;
	}

	/// <summary>
	///  Performs the frame's configuration step.
	/// </summary>
	/// <param name="dt">Timestep duration.</param>
	public override void Update(float dt)
	{
		basis.rotationMatrix = connectionA.orientationMatrix;
		basis.ComputeWorldSpaceAxes();
		Matrix3X3.Transform(ref localTestAxis, ref connectionB.orientationMatrix, out worldTestAxis);
		if (settings.mode == MotorMode.Servomechanism)
		{
			Vector3.Cross(ref basis.primaryAxis, ref basis.xAxis, out var result);
			Vector3.Dot(ref worldTestAxis, ref result, out var result2);
			Vector3.Dot(ref worldTestAxis, ref basis.xAxis, out var result3);
			float angle = (float)Math.Atan2(result2, result3);
			error = GetDistanceFromGoal(angle);
			float value = Math.Abs(error / dt);
			settings.servo.springSettings.ComputeErrorReductionAndSoftness(dt, out var errorReduction, out usedSoftness);
			biasVelocity = (float)Math.Sign(error) * MathHelper.Min(settings.servo.baseCorrectiveSpeed, value) + error * errorReduction;
			biasVelocity = MathHelper.Clamp(biasVelocity, 0f - settings.servo.maxCorrectiveVelocity, settings.servo.maxCorrectiveVelocity);
		}
		else
		{
			biasVelocity = settings.velocityMotor.goalVelocity;
			usedSoftness = settings.velocityMotor.softness / dt;
			error = 0f;
		}
		jacobianA = basis.primaryAxis;
		jacobianB.X = 0f - jacobianA.X;
		jacobianB.Y = 0f - jacobianA.Y;
		jacobianB.Z = 0f - jacobianA.Z;
		Vector3 result4;
		float result5;
		if (connectionA.isDynamic)
		{
			Matrix3X3.Transform(ref jacobianA, ref connectionA.inertiaTensorInverse, out result4);
			Vector3.Dot(ref result4, ref jacobianA, out result5);
		}
		else
		{
			result5 = 0f;
		}
		float result6;
		if (connectionB.isDynamic)
		{
			Matrix3X3.Transform(ref jacobianB, ref connectionB.inertiaTensorInverse, out result4);
			Vector3.Dot(ref result4, ref jacobianB, out result6);
		}
		else
		{
			result6 = 0f;
		}
		velocityToImpulse = 1f / (usedSoftness + result5 + result6);
		ComputeMaxForces(settings.maximumForce, dt);
	}

	/// <summary>
	/// Performs any pre-solve iteration work that needs exclusive
	/// access to the members of the solver updateable.
	/// Usually, this is used for applying warmstarting impulses.
	/// </summary>
	public override void ExclusiveUpdate()
	{
		Vector3 result;
		if (connectionA.isDynamic)
		{
			Vector3.Multiply(ref jacobianA, accumulatedImpulse, out result);
			connectionA.ApplyAngularImpulse(ref result);
		}
		if (connectionB.isDynamic)
		{
			Vector3.Multiply(ref jacobianB, accumulatedImpulse, out result);
			connectionB.ApplyAngularImpulse(ref result);
		}
	}

	/// <summary>
	/// Computes one iteration of the constraint to meet the solver updateable's goal.
	/// </summary>
	/// <returns>The rough applied impulse magnitude.</returns>
	public override float SolveIteration()
	{
		Vector3.Dot(ref connectionA.angularVelocity, ref jacobianA, out var result);
		Vector3.Dot(ref connectionB.angularVelocity, ref jacobianB, out var result2);
		float num = 0f - (result + result2) - biasVelocity - usedSoftness * accumulatedImpulse;
		num *= velocityToImpulse;
		float num2 = accumulatedImpulse;
		accumulatedImpulse = MathHelper.Clamp(accumulatedImpulse + num, 0f - maxForceDt, maxForceDt);
		num = accumulatedImpulse - num2;
		Vector3 result3;
		if (connectionA.isDynamic)
		{
			Vector3.Multiply(ref jacobianA, num, out result3);
			connectionA.ApplyAngularImpulse(ref result3);
		}
		if (connectionB.isDynamic)
		{
			Vector3.Multiply(ref jacobianB, num, out result3);
			connectionB.ApplyAngularImpulse(ref result3);
		}
		return Math.Abs(num);
	}

	private float GetDistanceFromGoal(float angle)
	{
		float num = MathHelper.WrapAngle(settings.servo.goal);
		float num2 = ((num > 0f) ? ((angle > num) ? (angle - num) : ((!(angle > 0f)) ? ((float)Math.PI * 2f - num + angle) : ((float)Math.PI * 2f - num + angle))) : ((!(angle < num)) ? (angle - num) : ((float)Math.PI * 2f - num + angle)));
		if (!(num2 > (float)Math.PI))
		{
			return 0f - num2;
		}
		return (float)Math.PI * 2f - num2;
	}
}
