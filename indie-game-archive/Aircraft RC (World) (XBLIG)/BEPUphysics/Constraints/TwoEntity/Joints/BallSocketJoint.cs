using System;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.TwoEntity.Joints;

/// <summary>
/// Connects two entities with a spherical joint.  Acts like an unrestricted shoulder joint.
/// </summary>
public class BallSocketJoint : Joint, I3DImpulseConstraintWithError, I3DImpulseConstraint, I3DJacobianConstraint
{
	private Vector3 accumulatedImpulse;

	private Vector3 biasVelocity;

	private Vector3 localAnchorA;

	private Vector3 localAnchorB;

	private Matrix3X3 massMatrix;

	private Vector3 error;

	private Matrix3X3 rACrossProduct;

	private Matrix3X3 rBCrossProduct;

	private Vector3 worldOffsetA;

	private Vector3 worldOffsetB;

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
			Matrix3X3.TransformTranspose(ref worldOffsetB, ref connectionB.orientationMatrix, out localAnchorB);
		}
	}

	/// <summary>
	/// Gets the current relative velocity between the connected entities with respect to the constraint.
	/// </summary>
	public Vector3 RelativeVelocity
	{
		get
		{
			Vector3.Cross(ref connectionA.angularVelocity, ref worldOffsetA, out var result);
			Vector3.Add(ref connectionA.linearVelocity, ref result, out var result2);
			Vector3.Cross(ref connectionB.angularVelocity, ref worldOffsetB, out result);
			Vector3.Add(ref connectionB.linearVelocity, ref result, out var result3);
			return result2 - result3;
		}
	}

	/// <summary>
	/// Gets the total impulse applied by this constraint.
	/// </summary>
	public Vector3 TotalImpulse => accumulatedImpulse;

	/// <summary>
	/// Gets the current constraint error.
	/// </summary>
	public Vector3 Error => error;

	/// <summary>
	/// Constructs a spherical joint.
	/// To finish the initialization, specify the connections (ConnectionA and ConnectionB) 
	/// as well as the offsets (OffsetA, OffsetB or LocalOffsetA, LocalOffsetB).
	/// This constructor sets the constraint's IsActive property to false by default.
	/// </summary>
	public BallSocketJoint()
	{
		base.IsActive = false;
	}

	/// <summary>
	/// Constructs a spherical joint.
	/// </summary>
	/// <param name="connectionA">First connected entity.</param>
	/// <param name="connectionB">Second connected entity.</param>
	/// <param name="anchorLocation">Location of the socket.</param>
	public BallSocketJoint(Entity connectionA, Entity connectionB, Vector3 anchorLocation)
	{
		base.ConnectionA = connectionA;
		base.ConnectionB = connectionB;
		OffsetA = anchorLocation - base.ConnectionA.position;
		OffsetB = anchorLocation - base.ConnectionB.position;
	}

	/// <summary>
	/// Gets the linear jacobian entry for the first connected entity.
	/// </summary>
	/// <param name="jacobianX">First linear jacobian entry for the first connected entity.</param>
	/// <param name="jacobianY">Second linear jacobian entry for the first connected entity.</param>
	/// <param name="jacobianZ">Third linear jacobian entry for the first connected entity.</param>
	public void GetLinearJacobianA(out Vector3 jacobianX, out Vector3 jacobianY, out Vector3 jacobianZ)
	{
		jacobianX = Toolbox.RightVector;
		jacobianY = Toolbox.UpVector;
		jacobianZ = Toolbox.BackVector;
	}

	/// <summary>
	/// Gets the linear jacobian entry for the second connected entity.
	/// </summary>
	/// <param name="jacobianX">First linear jacobian entry for the second connected entity.</param>
	/// <param name="jacobianY">Second linear jacobian entry for the second connected entity.</param>
	/// <param name="jacobianZ">Third linear jacobian entry for the second connected entity.</param>
	public void GetLinearJacobianB(out Vector3 jacobianX, out Vector3 jacobianY, out Vector3 jacobianZ)
	{
		jacobianX = Toolbox.RightVector;
		jacobianY = Toolbox.UpVector;
		jacobianZ = Toolbox.BackVector;
	}

	/// <summary>
	/// Gets the angular jacobian entry for the first connected entity.
	/// </summary>
	/// <param name="jacobianX">First angular jacobian entry for the first connected entity.</param>
	/// <param name="jacobianY">Second angular jacobian entry for the first connected entity.</param>
	/// <param name="jacobianZ">Third angular jacobian entry for the first connected entity.</param>
	public void GetAngularJacobianA(out Vector3 jacobianX, out Vector3 jacobianY, out Vector3 jacobianZ)
	{
		jacobianX = rACrossProduct.Right;
		jacobianY = rACrossProduct.Up;
		jacobianZ = rACrossProduct.Forward;
	}

	/// <summary>
	/// Gets the angular jacobian entry for the second connected entity.
	/// </summary>
	/// <param name="jacobianX">First angular jacobian entry for the second connected entity.</param>
	/// <param name="jacobianY">Second angular jacobian entry for the second connected entity.</param>
	/// <param name="jacobianZ">Third angular jacobian entry for the second connected entity.</param>
	public void GetAngularJacobianB(out Vector3 jacobianX, out Vector3 jacobianY, out Vector3 jacobianZ)
	{
		jacobianX = rBCrossProduct.Right;
		jacobianY = rBCrossProduct.Up;
		jacobianZ = rBCrossProduct.Forward;
	}

	/// <summary>
	/// Gets the mass matrix of the constraint.
	/// </summary>
	/// <param name="outputMassMatrix">Constraint's mass matrix.</param>
	public void GetMassMatrix(out Matrix3X3 outputMassMatrix)
	{
		outputMassMatrix = massMatrix;
	}

	/// <summary>
	/// Calculates necessary information for velocity solving.
	/// Called by preStep(float dt)
	/// </summary>
	/// <param name="dt">Time in seconds since the last update.</param>
	public override void Update(float dt)
	{
		Matrix3X3.Transform(ref localAnchorA, ref connectionA.orientationMatrix, out worldOffsetA);
		Matrix3X3.Transform(ref localAnchorB, ref connectionB.orientationMatrix, out worldOffsetB);
		springSettings.ComputeErrorReductionAndSoftness(dt, out var errorReduction, out softness);
		Matrix3X3.CreateCrossProduct(ref worldOffsetA, out rACrossProduct);
		Matrix3X3.CreateCrossProduct(ref worldOffsetB, out rBCrossProduct);
		Matrix3X3 matrix;
		Matrix3X3 result3;
		if (connectionA.isDynamic && connectionB.isDynamic)
		{
			Matrix3X3.CreateScale(connectionA.inverseMass + connectionB.inverseMass, out matrix);
			Matrix3X3.Multiply(ref rACrossProduct, ref connectionA.inertiaTensorInverse, out var result);
			Matrix3X3.Multiply(ref rBCrossProduct, ref connectionB.inertiaTensorInverse, out var result2);
			Matrix3X3.Multiply(ref result, ref rACrossProduct, out result);
			Matrix3X3.Multiply(ref result2, ref rBCrossProduct, out result2);
			Matrix3X3.Subtract(ref matrix, ref result, out result3);
			Matrix3X3.Subtract(ref result3, ref result2, out result3);
		}
		else if (connectionA.isDynamic && !connectionB.isDynamic)
		{
			Matrix3X3.CreateScale(connectionA.inverseMass, out matrix);
			Matrix3X3.Multiply(ref rACrossProduct, ref connectionA.inertiaTensorInverse, out var result4);
			Matrix3X3.Multiply(ref result4, ref rACrossProduct, out result4);
			Matrix3X3.Subtract(ref matrix, ref result4, out result3);
		}
		else
		{
			if (connectionA.isDynamic || !connectionB.isDynamic)
			{
				throw new InvalidOperationException("Cannot constrain two kinematic bodies.");
			}
			Matrix3X3.CreateScale(connectionB.inverseMass, out matrix);
			Matrix3X3.Multiply(ref rBCrossProduct, ref connectionB.inertiaTensorInverse, out var result5);
			Matrix3X3.Multiply(ref result5, ref rBCrossProduct, out result5);
			Matrix3X3.Subtract(ref matrix, ref result5, out result3);
		}
		result3.M11 += softness;
		result3.M22 += softness;
		result3.M33 += softness;
		Matrix3X3.Invert(ref result3, out massMatrix);
		Vector3.Add(ref connectionB.position, ref worldOffsetB, out error);
		Vector3.Subtract(ref error, ref connectionA.position, out error);
		Vector3.Subtract(ref error, ref worldOffsetA, out error);
		Vector3.Multiply(ref error, 0f - errorReduction, out biasVelocity);
		float num = biasVelocity.LengthSquared();
		if (num > maxCorrectiveVelocitySquared)
		{
			float num2 = maxCorrectiveVelocity / (float)Math.Sqrt(num);
			biasVelocity.X *= num2;
			biasVelocity.Y *= num2;
			biasVelocity.Z *= num2;
		}
	}

	/// <summary>
	/// Performs any pre-solve iteration work that needs exclusive
	/// access to the members of the solver updateable.
	/// Usually, this is used for applying warmstarting impulses.
	/// </summary>
	public override void ExclusiveUpdate()
	{
		Vector3 impulse = default(Vector3);
		if (connectionA.isDynamic)
		{
			impulse.X = 0f - accumulatedImpulse.X;
			impulse.Y = 0f - accumulatedImpulse.Y;
			impulse.Z = 0f - accumulatedImpulse.Z;
			connectionA.ApplyLinearImpulse(ref impulse);
			Vector3.Cross(ref worldOffsetA, ref impulse, out var result);
			connectionA.ApplyAngularImpulse(ref result);
		}
		if (connectionB.isDynamic)
		{
			connectionB.ApplyLinearImpulse(ref accumulatedImpulse);
			Vector3.Cross(ref worldOffsetB, ref accumulatedImpulse, out var result2);
			connectionB.ApplyAngularImpulse(ref result2);
		}
	}

	/// <summary>
	/// Calculates and applies corrective impulses.
	/// Called automatically by space.
	/// </summary>
	public override float SolveIteration()
	{
		Vector3 v = default(Vector3);
		Vector3.Cross(ref connectionA.angularVelocity, ref worldOffsetA, out var result);
		Vector3.Add(ref connectionA.linearVelocity, ref result, out var result2);
		Vector3.Cross(ref connectionB.angularVelocity, ref worldOffsetB, out result);
		Vector3.Add(ref connectionB.linearVelocity, ref result, out var result3);
		v.X = result2.X - result3.X + biasVelocity.X - softness * accumulatedImpulse.X;
		v.Y = result2.Y - result3.Y + biasVelocity.Y - softness * accumulatedImpulse.Y;
		v.Z = result2.Z - result3.Z + biasVelocity.Z - softness * accumulatedImpulse.Z;
		Matrix3X3.Transform(ref v, ref massMatrix, out v);
		Vector3.Add(ref accumulatedImpulse, ref v, out accumulatedImpulse);
		Vector3 impulse = default(Vector3);
		if (connectionA.isDynamic)
		{
			impulse.X = 0f - v.X;
			impulse.Y = 0f - v.Y;
			impulse.Z = 0f - v.Z;
			connectionA.ApplyLinearImpulse(ref impulse);
			Vector3.Cross(ref worldOffsetA, ref impulse, out var result4);
			connectionA.ApplyAngularImpulse(ref result4);
		}
		if (connectionB.isDynamic)
		{
			connectionB.ApplyLinearImpulse(ref v);
			Vector3.Cross(ref worldOffsetB, ref v, out var result5);
			connectionB.ApplyAngularImpulse(ref result5);
		}
		return Math.Abs(v.X) + Math.Abs(v.Y) + Math.Abs(v.Z);
	}
}
