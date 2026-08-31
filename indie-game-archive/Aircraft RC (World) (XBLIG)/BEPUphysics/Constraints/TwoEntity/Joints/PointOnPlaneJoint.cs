using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.TwoEntity.Joints;

/// <summary>
/// Constrains a point on one body to be on a plane defined by another body.
/// </summary>
public class PointOnPlaneJoint : Joint, I1DImpulseConstraintWithError, I1DImpulseConstraint, I1DJacobianConstraint
{
	private float accumulatedImpulse;

	private float biasVelocity;

	private float error;

	private Vector3 localPlaneAnchor;

	private Vector3 localPlaneNormal;

	private Vector3 localPointAnchor;

	private Vector3 worldPlaneAnchor;

	private Vector3 worldPlaneNormal;

	private Vector3 worldPointAnchor;

	private float negativeEffectiveMass;

	private Vector3 rA;

	private Vector3 rAcrossN;

	private Vector3 rB;

	private Vector3 rBcrossN;

	/// <summary>
	/// Gets or sets the plane's anchor in entity A's local space.
	/// </summary>
	public Vector3 LocalPlaneAnchor
	{
		get
		{
			return localPlaneAnchor;
		}
		set
		{
			localPlaneAnchor = value;
			Matrix3X3.Transform(ref localPlaneAnchor, ref connectionA.orientationMatrix, out worldPlaneAnchor);
			Vector3.Add(ref connectionA.position, ref worldPlaneAnchor, out worldPlaneAnchor);
		}
	}

	/// <summary>
	/// Gets or sets the plane's normal in entity A's local space.
	/// </summary>
	public Vector3 LocalPlaneNormal
	{
		get
		{
			return localPlaneNormal;
		}
		set
		{
			localPlaneNormal = Vector3.Normalize(value);
			Matrix3X3.Transform(ref localPlaneNormal, ref connectionA.orientationMatrix, out worldPlaneNormal);
		}
	}

	/// <summary>
	/// Gets or sets the point anchor in entity B's local space.
	/// </summary>
	public Vector3 LocalPointAnchor
	{
		get
		{
			return localPointAnchor;
		}
		set
		{
			localPointAnchor = value;
			Matrix3X3.Transform(ref localPointAnchor, ref connectionB.orientationMatrix, out worldPointAnchor);
			Vector3.Add(ref worldPointAnchor, ref connectionB.position, out worldPointAnchor);
		}
	}

	/// <summary>
	/// Gets the offset from A to the connection point between the entities.
	/// </summary>
	public Vector3 OffsetA => rA;

	/// <summary>
	/// Gets the offset from B to the connection point between the entities.
	/// </summary>
	public Vector3 OffsetB => rB;

	/// <summary>
	/// Gets or sets the plane anchor in world space.
	/// </summary>
	public Vector3 PlaneAnchor
	{
		get
		{
			return worldPlaneAnchor;
		}
		set
		{
			worldPlaneAnchor = value;
			localPlaneAnchor = value - connectionA.position;
			Matrix3X3.TransformTranspose(ref localPlaneAnchor, ref connectionA.orientationMatrix, out localPlaneAnchor);
		}
	}

	/// <summary>
	/// Gets or sets the plane's normal in world space.
	/// </summary>
	public Vector3 PlaneNormal
	{
		get
		{
			return worldPlaneNormal;
		}
		set
		{
			worldPlaneNormal = Vector3.Normalize(value);
			Matrix3X3.TransformTranspose(ref worldPlaneNormal, ref connectionA.orientationMatrix, out localPlaneNormal);
		}
	}

	/// <summary>
	/// Gets or sets the point anchor in world space.
	/// </summary>
	public Vector3 PointAnchor
	{
		get
		{
			return worldPointAnchor;
		}
		set
		{
			worldPointAnchor = value;
			localPointAnchor = value - connectionB.position;
			Matrix3X3.TransformTranspose(ref localPointAnchor, ref connectionB.orientationMatrix, out localPointAnchor);
		}
	}

	/// <summary>
	/// Gets the current relative velocity between the connected entities with respect to the constraint.
	/// </summary>
	public float RelativeVelocity
	{
		get
		{
			Vector3.Cross(ref connectionA.angularVelocity, ref rA, out var result);
			Vector3.Add(ref result, ref connectionA.linearVelocity, out result);
			Vector3.Cross(ref connectionB.angularVelocity, ref rB, out var result2);
			Vector3.Add(ref result2, ref connectionB.linearVelocity, out result2);
			Vector3.Subtract(ref result, ref result2, out var result3);
			Vector3.Dot(ref result3, ref worldPlaneNormal, out var result4);
			return result4;
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
	/// Constructs a new point on plane constraint.
	/// To finish the initialization, specify the connections (ConnectionA and ConnectionB) 
	/// as well as the PlaneAnchor, PlaneNormal, and PointAnchor (or their entity-local versions).
	/// This constructor sets the constraint's IsActive property to false by default.
	/// </summary>
	public PointOnPlaneJoint()
	{
		base.IsActive = false;
	}

	/// <summary>
	/// Constructs a new point on plane constraint.
	/// </summary>
	/// <param name="connectionA">Entity to which the constraint's plane is attached.</param>
	/// <param name="connectionB">Entity to which the constraint's point is attached.</param>
	/// <param name="planeAnchor">A point on the plane.</param>
	/// <param name="normal">Direction, attached to the first connected entity, defining the plane's normal</param>
	/// <param name="pointAnchor">The point to constrain to the plane, attached to the second connected object.</param>
	public PointOnPlaneJoint(Entity connectionA, Entity connectionB, Vector3 planeAnchor, Vector3 normal, Vector3 pointAnchor)
	{
		base.ConnectionA = connectionA;
		base.ConnectionB = connectionB;
		PointAnchor = pointAnchor;
		PlaneAnchor = planeAnchor;
		PlaneNormal = normal;
	}

	/// <summary>
	/// Gets the linear jacobian entry for the first connected entity.
	/// </summary>
	/// <param name="jacobian">Linear jacobian entry for the first connected entity.</param>
	public void GetLinearJacobianA(out Vector3 jacobian)
	{
		jacobian = worldPlaneNormal;
	}

	/// <summary>
	/// Gets the linear jacobian entry for the second connected entity.
	/// </summary>
	/// <param name="jacobian">Linear jacobian entry for the second connected entity.</param>
	public void GetLinearJacobianB(out Vector3 jacobian)
	{
		jacobian = -worldPlaneNormal;
	}

	/// <summary>
	/// Gets the angular jacobian entry for the first connected entity.
	/// </summary>
	/// <param name="jacobian">Angular jacobian entry for the first connected entity.</param>
	public void GetAngularJacobianA(out Vector3 jacobian)
	{
		jacobian = rAcrossN;
	}

	/// <summary>
	/// Gets the angular jacobian entry for the second connected entity.
	/// </summary>
	/// <param name="jacobian">Angular jacobian entry for the second connected entity.</param>
	public void GetAngularJacobianB(out Vector3 jacobian)
	{
		jacobian = -rBcrossN;
	}

	/// <summary>
	/// Gets the mass matrix of the constraint.
	/// </summary>
	/// <param name="outputMassMatrix">Constraint's mass matrix.</param>
	public void GetMassMatrix(out float outputMassMatrix)
	{
		outputMassMatrix = 0f - negativeEffectiveMass;
	}

	/// <summary>
	/// Computes one iteration of the constraint to meet the solver updateable's goal.
	/// </summary>
	/// <returns>The rough applied impulse magnitude.</returns>
	public override float SolveIteration()
	{
		Vector3.Cross(ref connectionA.angularVelocity, ref rA, out var result);
		Vector3.Add(ref result, ref connectionA.linearVelocity, out result);
		Vector3.Cross(ref connectionB.angularVelocity, ref rB, out var result2);
		Vector3.Add(ref result2, ref connectionB.linearVelocity, out result2);
		Vector3.Subtract(ref result, ref result2, out var result3);
		Vector3.Dot(ref result3, ref worldPlaneNormal, out var result4);
		float num = negativeEffectiveMass * (result4 + biasVelocity + softness * accumulatedImpulse);
		accumulatedImpulse += num;
		Vector3.Multiply(ref worldPlaneNormal, num, out var result5);
		Vector3 result6;
		if (connectionA.isDynamic)
		{
			Vector3.Multiply(ref rAcrossN, num, out result6);
			connectionA.ApplyLinearImpulse(ref result5);
			connectionA.ApplyAngularImpulse(ref result6);
		}
		if (connectionB.isDynamic)
		{
			Vector3.Negate(ref result5, out result5);
			Vector3.Multiply(ref rBcrossN, num, out result6);
			connectionB.ApplyLinearImpulse(ref result5);
			connectionB.ApplyAngularImpulse(ref result6);
		}
		return num;
	}

	/// <summary>
	///  Performs the frame's configuration step.
	/// </summary>
	/// <param name="dt">Timestep duration.</param>
	public override void Update(float dt)
	{
		Matrix3X3.Transform(ref localPlaneNormal, ref connectionA.orientationMatrix, out worldPlaneNormal);
		Matrix3X3.Transform(ref localPlaneAnchor, ref connectionA.orientationMatrix, out worldPlaneAnchor);
		Vector3.Add(ref worldPlaneAnchor, ref connectionA.position, out worldPlaneAnchor);
		Matrix3X3.Transform(ref localPointAnchor, ref connectionB.orientationMatrix, out rB);
		Vector3.Add(ref rB, ref connectionB.position, out worldPointAnchor);
		Vector3.Dot(ref worldPointAnchor, ref worldPlaneNormal, out var result);
		Vector3.Dot(ref worldPlaneAnchor, ref worldPlaneNormal, out var result2);
		float scaleFactor = result2 - result;
		Vector3.Multiply(ref worldPlaneNormal, scaleFactor, out var result3);
		Vector3.Add(ref result3, ref worldPointAnchor, out result3);
		Vector3.Subtract(ref result3, ref connectionA.position, out rA);
		Vector3.Cross(ref rA, ref worldPlaneNormal, out rAcrossN);
		Vector3.Cross(ref rB, ref worldPlaneNormal, out rBcrossN);
		Vector3.Negate(ref rBcrossN, out rBcrossN);
		Vector3.Subtract(ref worldPointAnchor, ref result3, out var result4);
		Vector3.Dot(ref result4, ref worldPlaneNormal, out error);
		springSettings.ComputeErrorReductionAndSoftness(dt, out var errorReduction, out softness);
		biasVelocity = MathHelper.Clamp((0f - errorReduction) * error, 0f - maxCorrectiveVelocity, maxCorrectiveVelocity);
		if (connectionA.IsDynamic && connectionB.IsDynamic)
		{
			Matrix3X3.Transform(ref rAcrossN, ref connectionA.inertiaTensorInverse, out var result5);
			Matrix3X3.Transform(ref rBcrossN, ref connectionB.inertiaTensorInverse, out var result6);
			Vector3.Dot(ref rAcrossN, ref result5, out var result7);
			Vector3.Dot(ref rBcrossN, ref result6, out var result8);
			negativeEffectiveMass = connectionA.inverseMass + connectionB.inverseMass + result7 + result8;
			negativeEffectiveMass = -1f / (negativeEffectiveMass + softness);
		}
		else if (connectionA.IsDynamic && !connectionB.IsDynamic)
		{
			Matrix3X3.Transform(ref rAcrossN, ref connectionA.inertiaTensorInverse, out var result9);
			Vector3.Dot(ref rAcrossN, ref result9, out var result10);
			negativeEffectiveMass = connectionA.inverseMass + result10;
			negativeEffectiveMass = -1f / (negativeEffectiveMass + softness);
		}
		else if (!connectionA.IsDynamic && connectionB.IsDynamic)
		{
			Matrix3X3.Transform(ref rBcrossN, ref connectionB.inertiaTensorInverse, out var result11);
			Vector3.Dot(ref rBcrossN, ref result11, out var result12);
			negativeEffectiveMass = connectionB.inverseMass + result12;
			negativeEffectiveMass = -1f / (negativeEffectiveMass + softness);
		}
		else
		{
			negativeEffectiveMass = 0f;
		}
	}

	/// <summary>
	/// Performs any pre-solve iteration work that needs exclusive
	/// access to the members of the solver updateable.
	/// Usually, this is used for applying warmstarting impulses.
	/// </summary>
	public override void ExclusiveUpdate()
	{
		Vector3.Multiply(ref worldPlaneNormal, accumulatedImpulse, out var result);
		Vector3 result2;
		if (connectionA.isDynamic)
		{
			Vector3.Multiply(ref rAcrossN, accumulatedImpulse, out result2);
			connectionA.ApplyLinearImpulse(ref result);
			connectionA.ApplyAngularImpulse(ref result2);
		}
		if (connectionB.isDynamic)
		{
			Vector3.Negate(ref result, out result);
			Vector3.Multiply(ref rBcrossN, accumulatedImpulse, out result2);
			connectionB.ApplyLinearImpulse(ref result);
			connectionB.ApplyAngularImpulse(ref result2);
		}
	}
}
