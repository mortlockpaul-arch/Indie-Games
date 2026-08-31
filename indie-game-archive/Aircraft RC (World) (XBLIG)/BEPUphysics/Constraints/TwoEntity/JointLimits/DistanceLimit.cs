using System;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.TwoEntity.JointLimits;

/// <summary>
/// A modified distance constraint allowing a range of lengths between two anchor points.
/// </summary>
public class DistanceLimit : JointLimit, I1DImpulseConstraintWithError, I1DImpulseConstraint, I1DJacobianConstraint
{
	private float accumulatedImpulse;

	private Vector3 anchorA;

	private Vector3 anchorB;

	private float biasVelocity;

	private Vector3 jAngularA;

	private Vector3 jAngularB;

	private Vector3 jLinearA;

	private Vector3 jLinearB;

	private float error;

	private Vector3 localAnchorA;

	private Vector3 localAnchorB;

	/// <summary>
	/// Maximum distance allowed between the anchors.
	/// </summary>
	protected float maximumLength;

	/// <summary>
	/// Minimum distance maintained between the anchors.
	/// </summary>
	protected float minimumLength;

	private Vector3 offsetA;

	private Vector3 offsetB;

	private float velocityToImpulse;

	/// <summary>
	/// Gets or sets the first entity's connection point in local space.
	/// </summary>
	public Vector3 LocalAnchorA
	{
		get
		{
			return localAnchorA;
		}
		set
		{
			localAnchorA = value;
			Matrix3X3.Transform(ref localAnchorA, ref connectionA.orientationMatrix, out anchorA);
			anchorA += connectionA.position;
		}
	}

	/// <summary>
	/// Gets or sets the first entity's connection point in local space.
	/// </summary>
	public Vector3 LocalAnchorB
	{
		get
		{
			return localAnchorB;
		}
		set
		{
			localAnchorB = value;
			Matrix3X3.Transform(ref localAnchorB, ref connectionB.orientationMatrix, out anchorB);
			anchorB += connectionB.position;
		}
	}

	/// <summary>
	/// Gets or sets the maximum distance allowed between the anchors.
	/// </summary>
	public float MaximumLength
	{
		get
		{
			return maximumLength;
		}
		set
		{
			maximumLength = Math.Max(0f, value);
			minimumLength = Math.Min(minimumLength, maximumLength);
		}
	}

	/// <summary>
	/// Gets or sets the minimum distance maintained between the anchors.
	/// </summary>
	public float MinimumLength
	{
		get
		{
			return minimumLength;
		}
		set
		{
			minimumLength = Math.Max(0f, value);
			maximumLength = Math.Max(minimumLength, maximumLength);
		}
	}

	/// <summary>
	/// Gets or sets the connection to the distance constraint from the first connected body in world space.
	/// </summary>
	public Vector3 WorldAnchorA
	{
		get
		{
			return anchorA;
		}
		set
		{
			anchorA = value;
			localAnchorA = Vector3.Transform(anchorA - connectionA.position, Quaternion.Conjugate(connectionA.orientation));
		}
	}

	/// <summary>
	/// Gets or sets the connection to the distance constraint from the second connected body in world space.
	/// </summary>
	public Vector3 WorldAnchorB
	{
		get
		{
			return anchorB;
		}
		set
		{
			anchorB = value;
			localAnchorB = Vector3.Transform(anchorB - connectionB.position, Quaternion.Conjugate(connectionB.orientation));
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
	/// Constructs a distance limit joint.
	/// To finish the initialization, specify the connections (ConnectionA and ConnectionB) 
	/// as well as the WorldAnchorA and WorldAnchorB (or their entity-local versions)
	/// and the MinimumLength and MaximumLength.
	/// This constructor sets the constraint's IsActive property to false by default.
	/// </summary>
	public DistanceLimit()
	{
		base.IsActive = false;
	}

	/// <summary>
	/// Constructs a distance limit joint.
	/// </summary>
	/// <param name="connectionA">First body connected to the distance limit.</param>
	/// <param name="connectionB">Second body connected to the distance limit.</param>
	/// <param name="anchorA">Connection to the spring from the first connected body in world space.</param>
	/// <param name="anchorB"> Connection to the spring from the second connected body in world space.</param>
	/// <param name="minimumLength">Minimum distance maintained between the anchors.</param>
	/// <param name="maximumLength">Maximum distance allowed between the anchors.</param>
	public DistanceLimit(Entity connectionA, Entity connectionB, Vector3 anchorA, Vector3 anchorB, float minimumLength, float maximumLength)
	{
		base.ConnectionA = connectionA;
		base.ConnectionB = connectionB;
		MinimumLength = minimumLength;
		MaximumLength = maximumLength;
		WorldAnchorA = anchorA;
		WorldAnchorB = anchorB;
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
		outputMassMatrix = velocityToImpulse;
	}

	/// <summary>
	/// Calculates and applies corrective impulses.
	/// Called automatically by space.
	/// </summary>
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
		result *= velocityToImpulse;
		float num = accumulatedImpulse;
		accumulatedImpulse = MathHelper.Max(accumulatedImpulse + result, 0f);
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
	/// Calculates necessary information for velocity solving.
	/// </summary>
	/// <param name="dt">Time in seconds since the last update.</param>
	public override void Update(float dt)
	{
		Matrix3X3.Transform(ref localAnchorA, ref connectionA.orientationMatrix, out offsetA);
		Matrix3X3.Transform(ref localAnchorB, ref connectionB.orientationMatrix, out offsetB);
		Vector3.Add(ref connectionA.position, ref offsetA, out anchorA);
		Vector3.Add(ref connectionB.position, ref offsetB, out anchorB);
		Vector3.Subtract(ref anchorB, ref anchorA, out var result);
		float num = result.Length();
		if (num < maximumLength && num > minimumLength)
		{
			isActiveInSolver = false;
			accumulatedImpulse = 0f;
			error = 0f;
			isLimitActive = false;
			return;
		}
		isLimitActive = true;
		if (num > maximumLength)
		{
			if (num > 1E-07f)
			{
				jLinearA.X = result.X / num;
				jLinearA.Y = result.Y / num;
				jLinearA.Z = result.Z / num;
			}
			else
			{
				jLinearB = Toolbox.ZeroVector;
			}
			jLinearB.X = 0f - jLinearA.X;
			jLinearB.Y = 0f - jLinearA.Y;
			jLinearB.Z = 0f - jLinearA.Z;
			Vector3.Cross(ref jLinearA, ref offsetA, out jAngularA);
			Vector3.Cross(ref jLinearA, ref offsetB, out jAngularB);
		}
		else
		{
			if (num > 1E-07f)
			{
				jLinearB.X = result.X / num;
				jLinearB.Y = result.Y / num;
				jLinearB.Z = result.Z / num;
			}
			else
			{
				jLinearB = Toolbox.ZeroVector;
			}
			jLinearA.X = 0f - jLinearB.X;
			jLinearA.Y = 0f - jLinearB.Y;
			jLinearA.Z = 0f - jLinearB.Z;
			Vector3.Cross(ref offsetA, ref jLinearB, out jAngularA);
			Vector3.Cross(ref offsetB, ref jLinearB, out jAngularB);
		}
		if (connectionA.isDynamic && connectionB.isDynamic)
		{
			Matrix3X3.Transform(ref jAngularA, ref connectionA.localInertiaTensorInverse, out var result2);
			Vector3.Cross(ref result2, ref offsetA, out result2);
			Matrix3X3.Transform(ref jAngularB, ref connectionB.localInertiaTensorInverse, out var result3);
			Vector3.Cross(ref result3, ref offsetB, out result3);
			Vector3.Add(ref result2, ref result3, out result2);
			Vector3.Dot(ref result2, ref jLinearB, out velocityToImpulse);
			velocityToImpulse += connectionA.inverseMass + connectionB.inverseMass;
		}
		else if (connectionA.isDynamic)
		{
			Matrix3X3.Transform(ref jAngularA, ref connectionA.localInertiaTensorInverse, out var result4);
			Vector3.Cross(ref result4, ref offsetA, out result4);
			Vector3.Dot(ref result4, ref jLinearB, out velocityToImpulse);
			velocityToImpulse += connectionA.inverseMass;
		}
		else
		{
			if (!connectionB.isDynamic)
			{
				isActiveInSolver = false;
				accumulatedImpulse = 0f;
				return;
			}
			Matrix3X3.Transform(ref jAngularB, ref connectionB.localInertiaTensorInverse, out var result5);
			Vector3.Cross(ref result5, ref offsetB, out result5);
			Vector3.Dot(ref result5, ref jLinearB, out velocityToImpulse);
			velocityToImpulse += connectionB.inverseMass;
		}
		springSettings.ComputeErrorReductionAndSoftness(dt, out var errorReduction, out softness);
		velocityToImpulse = 1f / (softness + velocityToImpulse);
		jAngularA.X = 0f - jAngularA.X;
		jAngularA.Y = 0f - jAngularA.Y;
		jAngularA.Z = 0f - jAngularA.Z;
		if (num > maximumLength)
		{
			error = Math.Max(0f, num - maximumLength - base.Margin);
		}
		else
		{
			error = Math.Max(0f, minimumLength - base.Margin - num);
		}
		biasVelocity = Math.Min(errorReduction * error, maxCorrectiveVelocity);
		if (bounciness > 0f)
		{
			Vector3.Dot(ref jLinearA, ref connectionA.linearVelocity, out var result6);
			Vector3.Dot(ref jAngularA, ref connectionA.angularVelocity, out var result7);
			result6 += result7;
			Vector3.Dot(ref jLinearB, ref connectionB.linearVelocity, out result7);
			result6 += result7;
			Vector3.Dot(ref jAngularB, ref connectionB.angularVelocity, out result7);
			result6 += result7;
			if (0f - result6 > bounceVelocityThreshold)
			{
				biasVelocity = Math.Max(biasVelocity, (0f - result6) * bounciness);
			}
		}
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
