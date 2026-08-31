using System;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.TwoEntity.Joints;

/// <summary>
/// Prevents the connected entities from twisting relative to each other.
/// Acts like the angular part of a universal joint.
/// </summary>
public class TwistJoint : Joint, I1DImpulseConstraintWithError, I1DImpulseConstraint, I1DJacobianConstraint
{
	private Vector3 aLocalAxisY;

	private Vector3 aLocalAxisZ;

	private float accumulatedImpulse;

	private Vector3 bLocalAxisY;

	private float biasVelocity;

	private Vector3 jacobianA;

	private Vector3 jacobianB;

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
			Initialize();
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
			Initialize();
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
			Initialize();
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
			Quaternion.Conjugate(ref connectionA.orientation, out var result);
			Vector3.Transform(ref worldAxisB, ref result, out localAxisB);
			Initialize();
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
	/// </summary>
	public float Error => error;

	/// <summary>
	/// Constructs a new constraint which prevents the connected entities from twisting relative to each other.
	/// To finish the initialization, specify the connections (ConnectionA and ConnectionB) 
	/// as well as the WorldAxisA and WorldAxisB (or their entity-local versions).
	/// This constructor sets the constraint's IsActive property to false by default.
	/// </summary>
	public TwistJoint()
	{
		base.IsActive = false;
	}

	/// <summary>
	/// Constructs a new constraint which prevents the connected entities from twisting relative to each other.
	/// </summary>
	/// <param name="connectionA">First connection of the pair.</param>
	/// <param name="connectionB">Second connection of the pair.</param>
	/// <param name="axisA">Twist axis attached to the first connected entity.</param>
	/// <param name="axisB">Twist axis attached to the second connected entity.</param>
	public TwistJoint(Entity connectionA, Entity connectionB, Vector3 axisA, Vector3 axisB)
	{
		base.ConnectionA = connectionA;
		base.ConnectionB = connectionB;
		WorldAxisA = axisA;
		WorldAxisB = axisB;
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
	/// Solves for velocity.
	/// </summary>
	public override float SolveIteration()
	{
		Vector3.Dot(ref connectionA.angularVelocity, ref jacobianA, out var result);
		Vector3.Dot(ref connectionB.angularVelocity, ref jacobianB, out var result2);
		float num = 0f - (result + result2) + biasVelocity - softness * accumulatedImpulse;
		num *= velocityToImpulse;
		accumulatedImpulse += num;
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
		Matrix3X3.Transform(ref localAxisA, ref connectionA.orientationMatrix, out worldAxisA);
		Matrix3X3.Transform(ref aLocalAxisY, ref connectionA.orientationMatrix, out var result);
		Matrix3X3.Transform(ref aLocalAxisZ, ref connectionA.orientationMatrix, out var result2);
		Matrix3X3.Transform(ref localAxisB, ref connectionB.orientationMatrix, out worldAxisB);
		Matrix3X3.Transform(ref bLocalAxisY, ref connectionB.orientationMatrix, out var result3);
		Toolbox.GetQuaternionBetweenNormalizedVectors(ref worldAxisB, ref worldAxisA, out var q);
		Vector3.Transform(ref result3, ref q, out var result4);
		Vector3.Dot(ref result4, ref result2, out var result5);
		Vector3.Dot(ref result4, ref result, out var result6);
		error = (float)Math.Atan2(result5, result6);
		Vector3.Add(ref worldAxisA, ref worldAxisB, out jacobianB);
		if (jacobianB.LengthSquared() < 1E-07f)
		{
			isActiveInSolver = false;
			return;
		}
		jacobianB.Normalize();
		jacobianA.X = 0f - jacobianB.X;
		jacobianA.Y = 0f - jacobianB.Y;
		jacobianA.Z = 0f - jacobianB.Z;
		springSettings.ComputeErrorReductionAndSoftness(dt, out var errorReduction, out softness);
		biasVelocity = MathHelper.Clamp((0f - error) * errorReduction, 0f - maxCorrectiveVelocity, maxCorrectiveVelocity);
		Vector3 result7;
		float result8;
		if (connectionA.isDynamic)
		{
			Matrix3X3.Transform(ref jacobianA, ref connectionA.inertiaTensorInverse, out result7);
			Vector3.Dot(ref result7, ref jacobianA, out result8);
		}
		else
		{
			result8 = 0f;
		}
		float result9;
		if (connectionB.isDynamic)
		{
			Matrix3X3.Transform(ref jacobianB, ref connectionB.inertiaTensorInverse, out result7);
			Vector3.Dot(ref result7, ref jacobianB, out result9);
		}
		else
		{
			result9 = 0f;
		}
		velocityToImpulse = 1f / (softness + result8 + result9);
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

	private void Initialize()
	{
		Vector3.Cross(ref worldAxisA, ref Toolbox.UpVector, out var result);
		float num = result.LengthSquared();
		if (num < 1E-07f)
		{
			Vector3.Cross(ref worldAxisA, ref Toolbox.RightVector, out result);
		}
		result.Normalize();
		Quaternion.Conjugate(ref connectionA.orientation, out var result2);
		Vector3.Transform(ref result, ref result2, out aLocalAxisY);
		Vector3.Cross(ref localAxisA, ref aLocalAxisY, out aLocalAxisZ);
		Toolbox.GetQuaternionBetweenNormalizedVectors(ref worldAxisA, ref worldAxisB, out var q);
		Vector3.Transform(ref result, ref q, out result);
		Quaternion.Conjugate(ref connectionB.orientation, out result2);
		Vector3.Transform(ref result, ref result2, out bLocalAxisY);
	}
}
