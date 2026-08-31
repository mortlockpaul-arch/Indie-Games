using System;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.TwoEntity.JointLimits;

/// <summary>
/// Keeps the angle between the axes attached to two entities below some maximum value.
/// </summary>
public class SwingLimit : JointLimit, I1DImpulseConstraintWithError, I1DImpulseConstraint, I1DJacobianConstraint
{
	private float accumulatedImpulse;

	private float biasVelocity;

	private Vector3 hingeAxis;

	private float minimumCosine = 1f;

	private float error;

	private Vector3 localAxisA;

	private Vector3 localAxisB;

	private Vector3 worldAxisA;

	private Vector3 worldAxisB;

	private float velocityToImpulse;

	/// <summary>
	/// Gets or sets the axis attached to the first connected entity in its local space.
	/// </summary>
	public Vector3 LocalAxisA
	{
		get
		{
			return localAxisA;
		}
		set
		{
			localAxisA = Vector3.Normalize(value);
			Matrix3X3.Transform(ref localAxisA, ref connectionA.orientationMatrix, out worldAxisA);
		}
	}

	/// <summary>
	/// Gets or sets the axis attached to the first connected entity in its local space.
	/// </summary>
	public Vector3 LocalAxisB
	{
		get
		{
			return localAxisB;
		}
		set
		{
			localAxisB = Vector3.Normalize(value);
			Matrix3X3.Transform(ref localAxisB, ref connectionA.orientationMatrix, out worldAxisB);
		}
	}

	/// <summary>
	/// Maximum angle allowed between the two axes, from 0 to pi.
	/// </summary>
	public float MaximumAngle
	{
		get
		{
			return (float)Math.Acos(minimumCosine);
		}
		set
		{
			minimumCosine = (float)Math.Cos(MathHelper.Clamp(value, 0f, (float)Math.PI));
		}
	}

	/// <summary>
	/// Gets or sets the axis attached to the first connected entity in world space.
	/// </summary>
	public Vector3 WorldAxisA
	{
		get
		{
			return worldAxisA;
		}
		set
		{
			worldAxisA = Vector3.Normalize(value);
			Quaternion.Conjugate(ref connectionA.orientation, out var result);
			Vector3.Transform(ref worldAxisA, ref result, out localAxisA);
		}
	}

	/// <summary>
	/// Gets or sets the axis attached to the first connected entity in world space.
	/// </summary>
	public Vector3 WorldAxisB
	{
		get
		{
			return worldAxisB;
		}
		set
		{
			worldAxisB = Vector3.Normalize(value);
			Quaternion.Conjugate(ref connectionB.orientation, out var result);
			Vector3.Transform(ref worldAxisB, ref result, out localAxisB);
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
				Vector3.Subtract(ref connectionA.angularVelocity, ref connectionB.angularVelocity, out var result);
				Vector3.Dot(ref result, ref hingeAxis, out var result2);
				return result2;
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
	/// Constructs a new constraint which attempts to restrict the maximum relative angle of two entities to some value.
	/// To finish the initialization, specify the connections (ConnectionA and ConnectionB) 
	/// as well as the WorldAxisA, WorldAxisB (or their entity-local versions) and the MaximumAngle.
	/// This constructor sets the constraint's IsActive property to false by default.
	/// </summary>
	public SwingLimit()
	{
		base.IsActive = false;
	}

	/// <summary>
	/// Constructs a new constraint which attempts to restrict the maximum relative angle of two entities to some value.
	/// </summary>
	/// <param name="connectionA">First connection of the pair.</param>
	/// <param name="connectionB">Second connection of the pair.</param>
	/// <param name="axisA">Axis attached to the first connected entity.</param>
	/// <param name="axisB">Axis attached to the second connected entity.</param>
	/// <param name="maximumAngle">Maximum angle between the axes allowed.</param>
	public SwingLimit(Entity connectionA, Entity connectionB, Vector3 axisA, Vector3 axisB, float maximumAngle)
	{
		base.ConnectionA = connectionA;
		base.ConnectionB = connectionB;
		WorldAxisA = axisA;
		WorldAxisB = axisB;
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
		jacobian = hingeAxis;
	}

	/// <summary>
	/// Gets the angular jacobian entry for the second connected entity.
	/// </summary>
	/// <param name="jacobian">Angular jacobian entry for the second connected entity.</param>
	public void GetAngularJacobianB(out Vector3 jacobian)
	{
		jacobian = -hingeAxis;
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
	/// Applies the sequential impulse.
	/// </summary>
	public override float SolveIteration()
	{
		Vector3.Subtract(ref connectionA.angularVelocity, ref connectionB.angularVelocity, out var result);
		Vector3.Dot(ref result, ref hingeAxis, out var result2);
		result2 = 0f - result2 + biasVelocity - softness * accumulatedImpulse;
		result2 *= velocityToImpulse;
		float num = accumulatedImpulse;
		accumulatedImpulse = MathHelper.Max(accumulatedImpulse + result2, 0f);
		result2 = accumulatedImpulse - num;
		Vector3.Multiply(ref hingeAxis, result2, out var result3);
		if (connectionA.isDynamic)
		{
			connectionA.ApplyAngularImpulse(ref result3);
		}
		if (connectionB.isDynamic)
		{
			Vector3.Negate(ref result3, out result3);
			connectionB.ApplyAngularImpulse(ref result3);
		}
		return Math.Abs(result2);
	}

	/// <summary>
	/// Initializes the constraint for this frame.
	/// </summary>
	/// <param name="dt">Time since the last frame.</param>
	public override void Update(float dt)
	{
		Matrix3X3.Transform(ref localAxisA, ref connectionA.orientationMatrix, out worldAxisA);
		Matrix3X3.Transform(ref localAxisB, ref connectionB.orientationMatrix, out worldAxisB);
		Vector3.Dot(ref worldAxisA, ref worldAxisB, out var result);
		if (result > minimumCosine)
		{
			isActiveInSolver = false;
			error = 0f;
			accumulatedImpulse = 0f;
			isLimitActive = false;
			return;
		}
		isLimitActive = true;
		Vector3.Cross(ref worldAxisA, ref worldAxisB, out hingeAxis);
		float num = hingeAxis.LengthSquared();
		if (!(num > 1E-07f))
		{
			Vector3.Cross(ref worldAxisA, ref Toolbox.UpVector, out hingeAxis);
			num = hingeAxis.LengthSquared();
			if (!(num > 1E-07f))
			{
				Vector3.Cross(ref worldAxisA, ref Toolbox.RightVector, out hingeAxis);
			}
		}
		springSettings.ComputeErrorReductionAndSoftness(dt, out var errorReduction, out softness);
		error = Math.Max(0f, minimumCosine - result - margin);
		biasVelocity = MathHelper.Clamp(errorReduction * error, 0f - maxCorrectiveVelocity, maxCorrectiveVelocity);
		if (bounciness > 0f)
		{
			Vector3.Subtract(ref connectionA.angularVelocity, ref connectionB.angularVelocity, out var result2);
			Vector3.Dot(ref result2, ref hingeAxis, out var result3);
			if (result3 < 0f - bounceVelocityThreshold)
			{
				biasVelocity = Math.Max(biasVelocity, bounciness * result3);
			}
		}
		Vector3 result4;
		float result5;
		if (connectionA.isDynamic)
		{
			Matrix3X3.Transform(ref hingeAxis, ref connectionA.inertiaTensorInverse, out result4);
			Vector3.Dot(ref result4, ref hingeAxis, out result5);
		}
		else
		{
			result5 = 0f;
		}
		float result6;
		if (connectionB.isDynamic)
		{
			Matrix3X3.Transform(ref hingeAxis, ref connectionB.inertiaTensorInverse, out result4);
			Vector3.Dot(ref result4, ref hingeAxis, out result6);
		}
		else
		{
			result6 = 0f;
		}
		velocityToImpulse = 1f / (softness + result5 + result6);
	}

	public override void ExclusiveUpdate()
	{
		Vector3.Multiply(ref hingeAxis, accumulatedImpulse, out var result);
		if (connectionA.isDynamic)
		{
			connectionA.ApplyAngularImpulse(ref result);
		}
		if (connectionB.isDynamic)
		{
			Vector3.Negate(ref result, out result);
			connectionB.ApplyAngularImpulse(ref result);
		}
	}
}
