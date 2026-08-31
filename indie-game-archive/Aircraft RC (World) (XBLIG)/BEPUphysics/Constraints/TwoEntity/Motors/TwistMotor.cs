using System;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.TwoEntity.Motors;

/// <summary>
/// Attempts to achieve some defined relative twist angle between the entities.
/// </summary>
public class TwistMotor : Motor, I1DImpulseConstraintWithError, I1DImpulseConstraint, I1DJacobianConstraint
{
	private readonly JointBasis3D basisA = new JointBasis3D();

	private readonly JointBasis2D basisB = new JointBasis2D();

	private readonly MotorSettings1D settings;

	private float accumulatedImpulse;

	/// <summary>
	/// Velocity needed to get closer to the goal.
	/// </summary>
	protected float biasVelocity;

	private Vector3 jacobianA;

	private Vector3 jacobianB;

	private float error;

	private float velocityToImpulse;

	/// <summary>
	/// Gets the basis attached to entity A.
	/// The primary axis represents the twist axis attached to entity A.
	/// The x axis and y axis represent a plane against which entity B's attached x axis is projected to determine the twist angle.
	/// </summary>
	public JointBasis3D BasisA => basisA;

	/// <summary>
	/// Gets the basis attached to entity B.
	/// The primary axis represents the twist axis attached to entity A.
	/// The x axis is projected onto the plane defined by localTransformA's x and y axes
	/// to get the twist angle.
	/// </summary>
	public JointBasis2D BasisB => basisB;

	/// <summary>
	/// Gets the motor's velocity and servo settings.
	/// </summary>
	public MotorSettings1D Settings => settings;

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
	/// If the motor is in velocity only mode, the error will be zero.
	/// </summary>
	public float Error => error;

	/// <summary>
	/// Constructs a new constraint which prevents the connected entities from twisting relative to each other.
	/// To finish the initialization, specify the connections (ConnectionA and ConnectionB) 
	/// as well as the BasisA and BasisB.
	/// This constructor sets the constraint's IsActive property to false by default.
	/// </summary>
	public TwistMotor()
	{
		base.IsActive = false;
		settings = new MotorSettings1D(this);
	}

	/// <summary>
	/// Constructs a new constraint which prevents the connected entities from twisting relative to each other.
	/// </summary>
	/// <param name="connectionA">First connection of the pair.</param>
	/// <param name="connectionB">Second connection of the pair.</param>
	/// <param name="axisA">Twist axis attached to the first connected entity.</param>
	/// <param name="axisB">Twist axis attached to the second connected entity.</param>
	public TwistMotor(Entity connectionA, Entity connectionB, Vector3 axisA, Vector3 axisB)
	{
		base.ConnectionA = connectionA;
		base.ConnectionB = connectionB;
		SetupJointTransforms(axisA, axisB);
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
	/// <param name="worldTwistAxisA">Twist axis in world space to attach to entity A.</param>
	/// <param name="worldTwistAxisB">Twist axis in world space to attach to entity B.</param>
	public void SetupJointTransforms(Vector3 worldTwistAxisA, Vector3 worldTwistAxisB)
	{
		worldTwistAxisA.Normalize();
		worldTwistAxisB.Normalize();
		Vector3.Cross(ref worldTwistAxisA, ref Toolbox.UpVector, out var result);
		float num = result.LengthSquared();
		if (num < 1E-07f)
		{
			Vector3.Cross(ref worldTwistAxisA, ref Toolbox.RightVector, out result);
		}
		result.Normalize();
		Vector3.Cross(ref worldTwistAxisA, ref result, out var result2);
		basisA.rotationMatrix = connectionA.orientationMatrix;
		basisA.SetWorldAxes(worldTwistAxisA, result, result2);
		Toolbox.GetQuaternionBetweenNormalizedVectors(ref worldTwistAxisA, ref worldTwistAxisB, out var q);
		Vector3.Transform(ref result, ref q, out result);
		basisB.rotationMatrix = connectionB.orientationMatrix;
		basisB.SetWorldAxes(worldTwistAxisB, result);
	}

	/// <summary>
	/// Solves for velocity.
	/// </summary>
	public override float SolveIteration()
	{
		Vector3.Dot(ref connectionA.angularVelocity, ref jacobianA, out var result);
		Vector3.Dot(ref connectionB.angularVelocity, ref jacobianB, out var result2);
		float num = 0f - (result + result2) + biasVelocity - usedSoftness * accumulatedImpulse;
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

	/// <summary>
	/// Do any necessary computations to prepare the constraint for this frame.
	/// </summary>
	/// <param name="dt">Simulation step length.</param>
	public override void Update(float dt)
	{
		basisA.rotationMatrix = connectionA.orientationMatrix;
		basisB.rotationMatrix = connectionB.orientationMatrix;
		basisA.ComputeWorldSpaceAxes();
		basisB.ComputeWorldSpaceAxes();
		if (settings.mode == MotorMode.Servomechanism)
		{
			Toolbox.GetQuaternionBetweenNormalizedVectors(ref basisB.primaryAxis, ref basisA.primaryAxis, out var q);
			Vector3.Transform(ref basisB.xAxis, ref q, out var result);
			Vector3.Dot(ref result, ref basisA.yAxis, out var result2);
			Vector3.Dot(ref result, ref basisA.xAxis, out var result3);
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
		Vector3.Add(ref basisA.primaryAxis, ref basisB.primaryAxis, out jacobianB);
		if (jacobianB.LengthSquared() < 1E-07f)
		{
			isActiveInSolver = false;
			return;
		}
		jacobianB.Normalize();
		jacobianA.X = 0f - jacobianB.X;
		jacobianA.Y = 0f - jacobianB.Y;
		jacobianA.Z = 0f - jacobianB.Z;
		ComputeMaxForces(settings.maximumForce, dt);
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
