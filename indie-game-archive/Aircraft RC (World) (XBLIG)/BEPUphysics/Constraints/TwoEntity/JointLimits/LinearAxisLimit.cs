using System;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.TwoEntity.JointLimits;

/// <summary>
/// Constrains the distance along an axis between anchor points attached to two entities.
/// </summary>
public class LinearAxisLimit : JointLimit, I1DImpulseConstraintWithError, I1DImpulseConstraint, I1DJacobianConstraint
{
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

	private float maximum;

	private float minimum;

	private Vector3 worldAxis;

	private Vector3 rA;

	private float unadjustedError;

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
	/// Gets or sets the limited axis in world space.
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
	/// Gets or sets the maximum allowed distance along the axis.
	/// </summary>
	public float Maximum
	{
		get
		{
			return maximum;
		}
		set
		{
			maximum = value;
			minimum = MathHelper.Min(minimum, maximum);
		}
	}

	/// <summary>
	/// Gets or sets the minimum allowed distance along the axis.
	/// </summary>
	public float Minimum
	{
		get
		{
			return minimum;
		}
		set
		{
			minimum = value;
			maximum = MathHelper.Max(minimum, maximum);
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
	/// Gets the current relative velocity between the connected entities with respect to the constraint.
	/// </summary>
	public float RelativeVelocity
	{
		get
		{
			if (isLimitActive)
			{
				Vector3.Dot(ref jLinearA, ref connectionA.linearVelocity, out var result);
				Vector3.Dot(ref jAngularA, ref connectionA.angularVelocity, out var result2);
				result += result2;
				Vector3.Dot(ref jLinearB, ref connectionB.linearVelocity, out result2);
				result += result2;
				Vector3.Dot(ref jAngularB, ref connectionB.angularVelocity, out result2);
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
	/// Constructs a constraint which tries to keep anchors on two entities within a certain distance of each other along an axis.
	/// To finish the initialization, specify the connections (ConnectionA and ConnectionB) 
	/// as well as the AnchorA, AnchorB, and Axis (or their entity-local versions),
	/// and the Minimum and Maximum.
	/// This constructor sets the constraint's IsActive property to false by default.
	/// </summary>
	public LinearAxisLimit()
	{
		base.IsActive = false;
	}

	/// <summary>
	/// Constructs a constraint which tries to keep anchors on two entities within a certain distance of each other along an axis.
	/// </summary>
	/// <param name="connectionA">First connection of the pair.</param>
	/// <param name="connectionB">Second connection of the pair.</param>
	/// <param name="anchorA">World space point to attach to connection A that will be constrained.</param>
	/// <param name="anchorB">World space point to attach to connection B that will be constrained.</param>
	/// <param name="axis">Limited axis in world space to attach to connection A.</param>
	/// <param name="minimum">Minimum allowed position along the axis.</param>
	/// <param name="maximum">Maximum allowed position along the axis.</param>
	public LinearAxisLimit(Entity connectionA, Entity connectionB, Vector3 anchorA, Vector3 anchorB, Vector3 axis, float minimum, float maximum)
	{
		base.ConnectionA = connectionA;
		base.ConnectionB = connectionB;
		AnchorA = anchorA;
		AnchorB = anchorB;
		Axis = axis;
		Minimum = minimum;
		Maximum = maximum;
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
		result = 0f - result + biasVelocity - softness * accumulatedImpulse;
		result *= massMatrix;
		float num = accumulatedImpulse;
		if (unadjustedError < 0f)
		{
			accumulatedImpulse = MathHelper.Min(accumulatedImpulse + result, 0f);
		}
		else
		{
			accumulatedImpulse = MathHelper.Max(accumulatedImpulse + result, 0f);
		}
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

	/// <summary>
	///  Performs the frame's configuration step.
	/// </summary>
	/// <param name="dt">Timestep duration.</param>
	public override void Update(float dt)
	{
		Matrix3X3.Transform(ref localAnchorA, ref connectionA.orientationMatrix, out worldOffsetA);
		Matrix3X3.Transform(ref localAnchorB, ref connectionB.orientationMatrix, out worldOffsetB);
		Vector3.Add(ref worldOffsetA, ref connectionA.position, out worldAnchorA);
		Vector3.Add(ref worldOffsetB, ref connectionB.position, out worldAnchorB);
		Vector3.Subtract(ref worldAnchorB, ref connectionA.position, out rA);
		Matrix3X3.Transform(ref localAxis, ref connectionA.orientationMatrix, out worldAxis);
		Vector3 vector = new Vector3
		{
			X = worldAnchorB.X - worldAnchorA.X,
			Y = worldAnchorB.Y - worldAnchorA.Y,
			Z = worldAnchorB.Z - worldAnchorA.Z
		};
		Vector3.Dot(ref vector, ref worldAxis, out unadjustedError);
		if (unadjustedError < minimum)
		{
			unadjustedError = minimum - unadjustedError;
		}
		else
		{
			if (!(unadjustedError > maximum))
			{
				unadjustedError = 0f;
				isActiveInSolver = false;
				accumulatedImpulse = 0f;
				isLimitActive = false;
				return;
			}
			unadjustedError = maximum - unadjustedError;
		}
		isLimitActive = true;
		unadjustedError = 0f - unadjustedError;
		if (unadjustedError > 0f)
		{
			error = MathHelper.Max(0f, unadjustedError - margin);
		}
		else if (unadjustedError < 0f)
		{
			error = MathHelper.Min(0f, unadjustedError + margin);
		}
		jLinearA = worldAxis;
		jLinearB.X = 0f - jLinearA.X;
		jLinearB.Y = 0f - jLinearA.Y;
		jLinearB.Z = 0f - jLinearA.Z;
		Vector3.Cross(ref rA, ref jLinearA, out jAngularA);
		Vector3.Cross(ref worldOffsetB, ref jLinearB, out jAngularB);
		springSettings.ComputeErrorReductionAndSoftness(dt, out var errorReduction, out softness);
		biasVelocity = MathHelper.Clamp(errorReduction * error, 0f - maxCorrectiveVelocity, maxCorrectiveVelocity);
		if (bounciness > 0f)
		{
			Vector3.Dot(ref jLinearA, ref connectionA.linearVelocity, out var result);
			Vector3.Dot(ref jAngularA, ref connectionA.angularVelocity, out var result2);
			result += result2;
			Vector3.Dot(ref jLinearB, ref connectionB.linearVelocity, out result2);
			result += result2;
			Vector3.Dot(ref jAngularB, ref connectionB.angularVelocity, out result2);
			result += result2;
			if (unadjustedError > 0f && 0f - result > bounceVelocityThreshold)
			{
				biasVelocity = Math.Max(biasVelocity, (0f - result) * bounciness);
			}
			else if (unadjustedError < 0f && result > bounceVelocityThreshold)
			{
				biasVelocity = Math.Min(biasVelocity, (0f - result) * bounciness);
			}
		}
		Vector3 result3;
		float result4;
		if (connectionA.isDynamic)
		{
			Matrix3X3.Transform(ref jAngularA, ref connectionA.inertiaTensorInverse, out result3);
			Vector3.Dot(ref result3, ref jAngularA, out result4);
			result4 += connectionA.inverseMass;
		}
		else
		{
			result4 = 0f;
		}
		float result5;
		if (connectionB.isDynamic)
		{
			Matrix3X3.Transform(ref jAngularB, ref connectionB.inertiaTensorInverse, out result3);
			Vector3.Dot(ref result3, ref jAngularB, out result5);
			result5 += connectionB.inverseMass;
		}
		else
		{
			result5 = 0f;
		}
		massMatrix = 1f / (result4 + result5 + softness);
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
