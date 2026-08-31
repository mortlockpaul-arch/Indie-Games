using System;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.TwoEntity.Joints;

/// <summary>
/// Constrains two bodies so that they can rotate relative to each other like a modified door hinge.
/// Instead of removing two degrees of freedom, only one is removed so that the second connection to the constraint can twist.
/// </summary>
public class SwivelHingeAngularJoint : Joint, I1DImpulseConstraintWithError, I1DImpulseConstraint, I1DJacobianConstraint
{
	private float accumulatedImpulse;

	private float biasVelocity;

	private Vector3 jacobianA;

	private Vector3 jacobianB;

	private float error;

	private Vector3 localHingeAxis;

	private Vector3 localTwistAxis;

	private Vector3 worldHingeAxis;

	private Vector3 worldTwistAxis;

	private float velocityToImpulse;

	/// <summary>
	/// Gets or sets the hinge axis attached to entity A in its local space.
	/// </summary>
	public Vector3 LocalHingeAxis
	{
		get
		{
			return localHingeAxis;
		}
		set
		{
			localHingeAxis = Vector3.Normalize(value);
			Matrix3X3.Transform(ref localHingeAxis, ref connectionA.orientationMatrix, out worldHingeAxis);
		}
	}

	/// <summary>
	/// Gets or sets the twist axis attached to entity B in its local space.
	/// </summary>
	public Vector3 LocalTwistAxis
	{
		get
		{
			return localTwistAxis;
		}
		set
		{
			localTwistAxis = Vector3.Normalize(value);
			Matrix3X3.Transform(ref localTwistAxis, ref connectionB.orientationMatrix, out worldTwistAxis);
		}
	}

	/// <summary>
	/// Gets or sets the hinge axis attached to entity A in world space.
	/// </summary>
	public Vector3 WorldHingeAxis
	{
		get
		{
			return worldHingeAxis;
		}
		set
		{
			worldHingeAxis = Vector3.Normalize(value);
			Quaternion.Conjugate(ref connectionA.orientation, out var result);
			Vector3.Transform(ref worldHingeAxis, ref result, out localHingeAxis);
		}
	}

	/// <summary>
	/// Gets or sets the axis attached to the first connected entity in world space.
	/// </summary>
	public Vector3 WorldTwistAxis
	{
		get
		{
			return worldTwistAxis;
		}
		set
		{
			worldTwistAxis = Vector3.Normalize(value);
			Quaternion.Conjugate(ref connectionB.orientation, out var result);
			Vector3.Transform(ref worldTwistAxis, ref result, out localTwistAxis);
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
	/// Constructs a new constraint which allows relative angular motion around a hinge axis and a twist axis.
	/// To finish the initialization, specify the connections (ConnectionA and ConnectionB) 
	/// as well as the WorldHingeAxis and WorldTwistAxis (or their entity-local versions).
	/// This constructor sets the constraint's IsActive property to false by default.
	/// </summary>
	public SwivelHingeAngularJoint()
	{
		base.IsActive = false;
	}

	/// <summary>
	/// Constructs a new constraint which allows relative angular motion around a hinge axis and a twist axis.
	/// </summary>
	/// <param name="connectionA">First connection of the pair.</param>
	/// <param name="connectionB">Second connection of the pair.</param>
	/// <param name="worldHingeAxis">Hinge axis attached to connectionA.
	/// The connected entities will be able to rotate around this axis relative to each other.</param>
	/// <param name="worldTwistAxis">Twist axis attached to connectionB.
	/// The connected entities will be able to rotate around this axis relative to each other.</param>
	public SwivelHingeAngularJoint(Entity connectionA, Entity connectionB, Vector3 worldHingeAxis, Vector3 worldTwistAxis)
	{
		base.ConnectionA = connectionA;
		base.ConnectionB = connectionB;
		WorldHingeAxis = worldHingeAxis;
		WorldTwistAxis = worldTwistAxis;
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
		float num = 0f - (result + result2) - biasVelocity - softness * accumulatedImpulse;
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
		Matrix3X3.Transform(ref localHingeAxis, ref connectionA.orientationMatrix, out worldHingeAxis);
		Matrix3X3.Transform(ref localTwistAxis, ref connectionB.orientationMatrix, out worldTwistAxis);
		Vector3.Dot(ref worldHingeAxis, ref worldTwistAxis, out error);
		springSettings.ComputeErrorReductionAndSoftness(dt, out var errorReduction, out softness);
		biasVelocity = MathHelper.Clamp(error * errorReduction, 0f - maxCorrectiveVelocity, maxCorrectiveVelocity);
		Vector3.Cross(ref worldHingeAxis, ref worldTwistAxis, out jacobianA);
		float num = jacobianA.LengthSquared();
		if (num > 1E-07f)
		{
			Vector3.Divide(ref jacobianA, (float)Math.Sqrt(num), out jacobianA);
		}
		else
		{
			jacobianA = default(Vector3);
		}
		jacobianB.X = 0f - jacobianA.X;
		jacobianB.Y = 0f - jacobianA.Y;
		jacobianB.Z = 0f - jacobianA.Z;
		Vector3 result;
		float result2;
		if (connectionA.isDynamic)
		{
			Matrix3X3.Transform(ref jacobianA, ref connectionA.inertiaTensorInverse, out result);
			Vector3.Dot(ref result, ref jacobianA, out result2);
		}
		else
		{
			result2 = 0f;
		}
		float result3;
		if (connectionB.isDynamic)
		{
			Matrix3X3.Transform(ref jacobianB, ref connectionB.inertiaTensorInverse, out result);
			Vector3.Dot(ref result, ref jacobianB, out result3);
		}
		else
		{
			result3 = 0f;
		}
		velocityToImpulse = 1f / (softness + result2 + result3);
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
}
