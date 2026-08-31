using System;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.TwoEntity.JointLimits;

/// <summary>
/// Prevents the connected entities from twisting relative to each other beyond given limits.
/// </summary>
public class TwistLimit : JointLimit, I1DImpulseConstraintWithError, I1DImpulseConstraint, I1DJacobianConstraint
{
	private readonly JointBasis3D basisA = new JointBasis3D();

	private readonly JointBasis2D basisB = new JointBasis2D();

	private float accumulatedImpulse;

	private float biasVelocity;

	private Vector3 jacobianA;

	private Vector3 jacobianB;

	private float error;

	/// <summary>
	/// Naximum angle that entities can twist.
	/// </summary>
	protected float maximumAngle;

	/// <summary>
	/// Minimum angle that entities can twist.
	/// </summary>
	protected float minimumAngle;

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
	/// Gets or sets the maximum angle that entities can twist.
	/// </summary>
	public float MaximumAngle
	{
		get
		{
			return maximumAngle;
		}
		set
		{
			maximumAngle = value % ((float)Math.PI * 2f);
			if (minimumAngle > (float)Math.PI)
			{
				minimumAngle -= (float)Math.PI * 2f;
			}
			if (minimumAngle <= -(float)Math.PI)
			{
				minimumAngle += (float)Math.PI * 2f;
			}
		}
	}

	/// <summary>
	/// Gets or sets the minimum angle that entities can twist.
	/// </summary>
	public float MinimumAngle
	{
		get
		{
			return minimumAngle;
		}
		set
		{
			minimumAngle = value % ((float)Math.PI * 2f);
			if (minimumAngle > (float)Math.PI)
			{
				minimumAngle -= (float)Math.PI * 2f;
			}
			if (minimumAngle <= -(float)Math.PI)
			{
				minimumAngle += (float)Math.PI * 2f;
			}
		}
	}

	/// <summary>
	/// Gets the current relative velocity between the connected entities with respect to the constraint.
	/// </summary>
	public float RelativeVelocity
	{
		get
		{
			if (isLimitActive)
			{
				Vector3.Dot(ref connectionA.angularVelocity, ref jacobianA, out var result);
				Vector3.Dot(ref connectionB.angularVelocity, ref jacobianB, out var result2);
				return result + result2;
			}
			return 0f;
		}
	}

	/// <summary>
	/// Gets the total impulse applied by this constraint.
	/// </summary>
	public float TotalImpulse => accumulatedImpulse;

	/// <summary>
	/// Gets the current constraint error.
	/// </summary>
	public float Error => error;

	/// <summary>
	/// Constructs a new constraint which prevents the connected entities from twisting relative to each other beyond given limits.
	/// To finish the initialization, specify the connections (ConnectionA and ConnectionB) 
	/// as well as the BasisA, BasisB and the MinimumAngle and MaximumAngle.
	/// This constructor sets the constraint's IsActive property to false by default.
	/// </summary>
	public TwistLimit()
	{
		base.IsActive = false;
	}

	/// <summary>
	/// Constructs a new constraint which prevents the connected entities from twisting relative to each other beyond given limits.
	/// </summary>
	/// <param name="connectionA">First connection of the pair.</param>
	/// <param name="connectionB">Second connection of the pair.</param>
	/// <param name="axisA">Twist axis attached to the first connected entity.</param>
	/// <param name="axisB">Twist axis attached to the second connected entity.</param>
	/// <param name="minimumAngle">Minimum twist angle allowed.</param>
	/// <param name="maximumAngle">Maximum twist angle allowed.</param>
	public TwistLimit(Entity connectionA, Entity connectionB, Vector3 axisA, Vector3 axisB, float minimumAngle, float maximumAngle)
	{
		base.ConnectionA = connectionA;
		base.ConnectionB = connectionB;
		SetupJointTransforms(axisA, axisB);
		MinimumAngle = minimumAngle;
		MaximumAngle = maximumAngle;
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
		float num = 0f - (result + result2) + biasVelocity - softness * accumulatedImpulse;
		num *= velocityToImpulse;
		float num2 = accumulatedImpulse;
		accumulatedImpulse = MathHelper.Max(accumulatedImpulse + num, 0f);
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
		Toolbox.GetQuaternionBetweenNormalizedVectors(ref basisB.primaryAxis, ref basisA.primaryAxis, out var q);
		Vector3.Transform(ref basisB.xAxis, ref q, out var result);
		Vector3.Dot(ref result, ref basisA.yAxis, out var result2);
		Vector3.Dot(ref result, ref basisA.xAxis, out var result3);
		float currentAngle = (float)Math.Atan2(result2, result3);
		if (IsAngleValid(currentAngle, out var distanceFromCurrent, out var distanceFromMaximum))
		{
			isActiveInSolver = false;
			accumulatedImpulse = 0f;
			error = 0f;
			isLimitActive = false;
			return;
		}
		isLimitActive = true;
		if (error > 0f)
		{
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
		}
		else
		{
			Vector3.Add(ref basisA.primaryAxis, ref basisB.primaryAxis, out jacobianA);
			if (jacobianA.LengthSquared() < 1E-07f)
			{
				isActiveInSolver = false;
				return;
			}
			jacobianA.Normalize();
			jacobianB.X = 0f - jacobianA.X;
			jacobianB.Y = 0f - jacobianA.Y;
			jacobianB.Z = 0f - jacobianA.Z;
		}
		error = ComputeAngleError(distanceFromCurrent, distanceFromMaximum);
		springSettings.ComputeErrorReductionAndSoftness(dt, out var errorReduction, out softness);
		biasVelocity = MathHelper.Min(MathHelper.Max(0f, Math.Abs(error) - margin) * errorReduction, maxCorrectiveVelocity);
		if (bounciness > 0f)
		{
			Vector3.Dot(ref connectionA.angularVelocity, ref jacobianA, out var result4);
			Vector3.Dot(ref connectionB.angularVelocity, ref jacobianB, out var result5);
			result4 += result5;
			if (0f - result4 > bounceVelocityThreshold)
			{
				biasVelocity = MathHelper.Max(biasVelocity, (0f - bounciness) * result4);
			}
		}
		Vector3 result6;
		float result7;
		if (connectionA.isDynamic)
		{
			Matrix3X3.Transform(ref jacobianA, ref connectionA.inertiaTensorInverse, out result6);
			Vector3.Dot(ref result6, ref jacobianA, out result7);
		}
		else
		{
			result7 = 0f;
		}
		float result8;
		if (connectionB.isDynamic)
		{
			Matrix3X3.Transform(ref jacobianB, ref connectionB.inertiaTensorInverse, out result6);
			Vector3.Dot(ref result6, ref jacobianB, out result8);
		}
		else
		{
			result8 = 0f;
		}
		velocityToImpulse = 1f / (softness + result7 + result8);
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

	private static float ComputeAngleError(float distanceFromCurrent, float distanceFromMaximum)
	{
		float num = (float)Math.PI * 2f - distanceFromCurrent;
		float num2 = distanceFromCurrent - distanceFromMaximum;
		if (!(num2 > num))
		{
			return 0f - num2;
		}
		return num;
	}

	private float GetDistanceFromMinimum(float angle)
	{
		if (minimumAngle > 0f)
		{
			if (angle >= minimumAngle)
			{
				return angle - minimumAngle;
			}
			if (angle > 0f)
			{
				return (float)Math.PI * 2f - minimumAngle + angle;
			}
			return (float)Math.PI * 2f - minimumAngle + angle;
		}
		if (angle < minimumAngle)
		{
			return (float)Math.PI * 2f - minimumAngle + angle;
		}
		return angle - minimumAngle;
	}

	private bool IsAngleValid(float currentAngle, out float distanceFromCurrent, out float distanceFromMaximum)
	{
		distanceFromCurrent = GetDistanceFromMinimum(currentAngle);
		distanceFromMaximum = GetDistanceFromMinimum(maximumAngle);
		return distanceFromCurrent < distanceFromMaximum;
	}
}
