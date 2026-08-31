using System;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.TwoEntity.Motors;

/// <summary>
/// Constrains anchors on two entities to move relative to each other on a line.
/// </summary>
public class LinearAxisMotor : Motor, I1DImpulseConstraintWithError, I1DImpulseConstraint, I1DJacobianConstraint
{
	private readonly MotorSettings1D settings;

	private float accumulatedImpulse;

	private float biasVelocity;

	private Vector3 jAngularA;

	private Vector3 jAngularB;

	private Vector3 jLinearA;

	private Vector3 jLinearB;

	private Vector3 localAnchorA;

	private Vector3 localAnchorB;

	private float massMatrix;

	private float error;

	private Vector3 localAxis;

	private Vector3 worldAxis;

	private Vector3 rA;

	private Vector3 worldAnchorA;

	private Vector3 worldAnchorB;

	private Vector3 worldOffsetA;

	private Vector3 worldOffsetB;

	/// <summary>
	/// Gets or sets the anchor point attached to entity A in world space.
	/// </summary>
	public Vector3 AnchorA
	{
		get
		{
			return worldAnchorA;
		}
		set
		{
			worldAnchorA = value;
			worldOffsetA = worldAnchorA - connectionA.position;
			Matrix3X3.TransformTranspose(ref worldOffsetA, ref connectionA.orientationMatrix, out localAnchorA);
		}
	}

	/// <summary>
	/// Gets or sets the anchor point attached to entity A in world space.
	/// </summary>
	public Vector3 AnchorB
	{
		get
		{
			return worldAnchorB;
		}
		set
		{
			worldAnchorB = value;
			worldOffsetB = worldAnchorB - connectionB.position;
			Matrix3X3.TransformTranspose(ref worldOffsetB, ref connectionB.orientationMatrix, out localAnchorB);
		}
	}

	/// <summary>
	/// Gets or sets the motorized axis in world space.
	/// </summary>
	public Vector3 Axis
	{
		get
		{
			return worldAxis;
		}
		set
		{
			worldAxis = Vector3.Normalize(value);
			Matrix3X3.TransformTranspose(ref worldAxis, ref connectionA.orientationMatrix, out localAxis);
		}
	}

	/// <summary>
	/// Gets or sets the limited axis in the local space of connection A.
	/// </summary>
	public Vector3 LocalAxis
	{
		get
		{
			return localAxis;
		}
		set
		{
			localAxis = Vector3.Normalize(value);
			Matrix3X3.Transform(ref localAxis, ref connectionA.orientationMatrix, out worldAxis);
		}
	}

	/// <summary>
	/// Gets or sets the offset from the first entity's center of mass to the anchor point in its local space.
	/// </summary>
	public Vector3 LocalOffsetA
	{
		get
		{
			return localAnchorA;
		}
		set
		{
			localAnchorA = value;
			Matrix3X3.Transform(ref localAnchorA, ref connectionA.orientationMatrix, out worldOffsetA);
			worldAnchorA = connectionA.position + worldOffsetA;
		}
	}

	/// <summary>
	/// Gets or sets the offset from the second entity's center of mass to the anchor point in its local space.
	/// </summary>
	public Vector3 LocalOffsetB
	{
		get
		{
			return localAnchorB;
		}
		set
		{
			localAnchorB = value;
			Matrix3X3.Transform(ref localAnchorB, ref connectionB.orientationMatrix, out worldOffsetB);
			worldAnchorB = connectionB.position + worldOffsetB;
		}
	}

	/// <summary>
	/// Gets or sets the offset from the first entity's center of mass to the anchor point in world space.
	/// </summary>
	public Vector3 OffsetA
	{
		get
		{
			return worldOffsetA;
		}
		set
		{
			worldOffsetA = value;
			worldAnchorA = connectionA.position + worldOffsetA;
			Matrix3X3.TransformTranspose(ref worldOffsetA, ref connectionA.orientationMatrix, out localAnchorA);
		}
	}

	/// <summary>
	/// Gets or sets the offset from the second entity's center of mass to the anchor point in world space.
	/// </summary>
	public Vector3 OffsetB
	{
		get
		{
			return worldOffsetB;
		}
		set
		{
			worldOffsetB = value;
			worldAnchorB = connectionB.position + worldOffsetB;
			Matrix3X3.TransformTranspose(ref worldOffsetB, ref connectionB.orientationMatrix, out localAnchorB);
		}
	}

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
			Vector3.Dot(ref jLinearA, ref connectionA.linearVelocity, out var result);
			Vector3.Dot(ref jAngularA, ref connectionA.angularVelocity, out var result2);
			result += result2;
			Vector3.Dot(ref jLinearB, ref connectionB.linearVelocity, out result2);
			result += result2;
			Vector3.Dot(ref jAngularB, ref connectionB.angularVelocity, out result2);
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
	/// Constrains anchors on two entities to move relative to each other on a line.
	/// To finish the initialization, specify the connections (ConnectionA and ConnectionB) 
	/// as well as the AnchorA, AnchorB and the Axis (or their entity-local versions).
	/// This constructor sets the constraint's IsActive property to false by default.
	/// </summary>
	public LinearAxisMotor()
	{
		settings = new MotorSettings1D(this);
		base.IsActive = false;
	}

	/// <summary>
	/// Constrains anchors on two entities to move relative to each other on a line.
	/// </summary>
	/// <param name="connectionA">First connection of the pair.</param>
	/// <param name="connectionB">Second connection of the pair.</param>
	/// <param name="anchorA">World space point to attach to connection A that will be constrained.</param>
	/// <param name="anchorB">World space point to attach to connection B that will be constrained.</param>
	/// <param name="axis">Limited axis in world space to attach to connection A.</param>
	public LinearAxisMotor(Entity connectionA, Entity connectionB, Vector3 anchorA, Vector3 anchorB, Vector3 axis)
	{
		base.ConnectionA = connectionA;
		base.ConnectionB = connectionB;
		AnchorA = anchorA;
		AnchorB = anchorB;
		Axis = axis;
		settings = new MotorSettings1D(this);
	}

	/// <summary>
	/// Gets the linear jacobian entry for the first connected entity.
	/// </summary>
	/// <param name="jacobian">Linear jacobian entry for the first connected entity.</param>
	public void GetLinearJacobianA(out Vector3 jacobian)
	{
		jacobian = jLinearA;
	}

	/// <summary>
	/// Gets the linear jacobian entry for the second connected entity.
	/// </summary>
	/// <param name="jacobian">Linear jacobian entry for the second connected entity.</param>
	public void GetLinearJacobianB(out Vector3 jacobian)
	{
		jacobian = jLinearB;
	}

	/// <summary>
	/// Gets the angular jacobian entry for the first connected entity.
	/// </summary>
	/// <param name="jacobian">Angular jacobian entry for the first connected entity.</param>
	public void GetAngularJacobianA(out Vector3 jacobian)
	{
		jacobian = jAngularA;
	}

	/// <summary>
	/// Gets the angular jacobian entry for the second connected entity.
	/// </summary>
	/// <param name="jacobian">Angular jacobian entry for the second connected entity.</param>
	public void GetAngularJacobianB(out Vector3 jacobian)
	{
		jacobian = jAngularB;
	}

	/// <summary>
	/// Gets the mass matrix of the constraint.
	/// </summary>
	/// <param name="outputMassMatrix">Constraint's mass matrix.</param>
	public void GetMassMatrix(out float outputMassMatrix)
	{
		outputMassMatrix = massMatrix;
	}

	/// <summary>
	/// Computes one iteration of the constraint to meet the solver updateable's goal.
	/// </summary>
	/// <returns>The rough applied impulse magnitude.</returns>
	public override float SolveIteration()
	{
		Vector3.Dot(ref jLinearA, ref connectionA.linearVelocity, out var result);
		Vector3.Dot(ref jAngularA, ref connectionA.angularVelocity, out var result2);
		result += result2;
		Vector3.Dot(ref jLinearB, ref connectionB.linearVelocity, out result2);
		result += result2;
		Vector3.Dot(ref jAngularB, ref connectionB.angularVelocity, out result2);
		result += result2;
		result = 0f - result + biasVelocity - usedSoftness * accumulatedImpulse;
		result *= massMatrix;
		float num = accumulatedImpulse;
		accumulatedImpulse = MathHelper.Clamp(accumulatedImpulse + result, 0f - maxForceDt, maxForceDt);
		result = accumulatedImpulse - num;
		Vector3 result3;
		if (connectionA.isDynamic)
		{
			Vector3.Multiply(ref jLinearA, result, out result3);
			connectionA.ApplyLinearImpulse(ref result3);
			Vector3.Multiply(ref jAngularA, result, out result3);
			connectionA.ApplyAngularImpulse(ref result3);
		}
		if (connectionB.isDynamic)
		{
			Vector3.Multiply(ref jLinearB, result, out result3);
			connectionB.ApplyLinearImpulse(ref result3);
			Vector3.Multiply(ref jAngularB, result, out result3);
			connectionB.ApplyAngularImpulse(ref result3);
		}
		return Math.Abs(result);
	}

	public override void Update(float dt)
	{
		Matrix3X3.Transform(ref localAnchorA, ref connectionA.orientationMatrix, out worldOffsetA);
		Matrix3X3.Transform(ref localAnchorB, ref connectionB.orientationMatrix, out worldOffsetB);
		Vector3.Add(ref worldOffsetA, ref connectionA.position, out worldAnchorA);
		Vector3.Add(ref worldOffsetB, ref connectionB.position, out worldAnchorB);
		Vector3.Subtract(ref worldAnchorB, ref connectionA.position, out rA);
		Matrix3X3.Transform(ref localAxis, ref connectionA.orientationMatrix, out worldAxis);
		if (settings.mode == MotorMode.Servomechanism)
		{
			Vector3 vector = new Vector3
			{
				X = worldAnchorB.X - worldAnchorA.X,
				Y = worldAnchorB.Y - worldAnchorA.Y,
				Z = worldAnchorB.Z - worldAnchorA.Z
			};
			Vector3.Dot(ref vector, ref worldAxis, out error);
			error -= settings.servo.goal;
			float value = Math.Abs(error / dt);
			settings.servo.springSettings.ComputeErrorReductionAndSoftness(dt, out var errorReduction, out usedSoftness);
			biasVelocity = (float)Math.Sign(error) * MathHelper.Min(settings.servo.baseCorrectiveSpeed, value) + error * errorReduction;
			biasVelocity = MathHelper.Clamp(biasVelocity, 0f - settings.servo.maxCorrectiveVelocity, settings.servo.maxCorrectiveVelocity);
		}
		else
		{
			biasVelocity = 0f - settings.velocityMotor.goalVelocity;
			usedSoftness = settings.velocityMotor.softness / dt;
			error = 0f;
		}
		jLinearA = worldAxis;
		jLinearB.X = 0f - jLinearA.X;
		jLinearB.Y = 0f - jLinearA.Y;
		jLinearB.Z = 0f - jLinearA.Z;
		Vector3.Cross(ref rA, ref jLinearA, out jAngularA);
		Vector3.Cross(ref worldOffsetB, ref jLinearB, out jAngularB);
		Vector3 result;
		float result2;
		if (connectionA.isDynamic)
		{
			Matrix3X3.Transform(ref jAngularA, ref connectionA.inertiaTensorInverse, out result);
			Vector3.Dot(ref result, ref jAngularA, out result2);
			result2 += connectionA.inverseMass;
		}
		else
		{
			result2 = 0f;
		}
		float result3;
		if (connectionB.isDynamic)
		{
			Matrix3X3.Transform(ref jAngularB, ref connectionB.inertiaTensorInverse, out result);
			Vector3.Dot(ref result, ref jAngularB, out result3);
			result3 += connectionB.inverseMass;
		}
		else
		{
			result3 = 0f;
		}
		massMatrix = 1f / (result2 + result3 + usedSoftness);
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
			Vector3.Multiply(ref jLinearA, accumulatedImpulse, out result);
			connectionA.ApplyLinearImpulse(ref result);
			Vector3.Multiply(ref jAngularA, accumulatedImpulse, out result);
			connectionA.ApplyAngularImpulse(ref result);
		}
		if (connectionB.isDynamic)
		{
			Vector3.Multiply(ref jLinearB, accumulatedImpulse, out result);
			connectionB.ApplyLinearImpulse(ref result);
			Vector3.Multiply(ref jAngularB, accumulatedImpulse, out result);
			connectionB.ApplyAngularImpulse(ref result);
		}
	}
}
