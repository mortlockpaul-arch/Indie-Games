using System;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.TwoEntity.Joints;

/// <summary>
/// Constrains two entities so that one has a point that stays on a line defined by the other.
/// </summary>
public class PointOnLineJoint : Joint, I2DImpulseConstraintWithError, I2DImpulseConstraint, I2DJacobianConstraint
{
	private Vector2 accumulatedImpulse;

	private Vector3 angularA1;

	private Vector3 angularA2;

	private Vector3 angularB1;

	private Vector3 angularB2;

	private Vector2 biasVelocity;

	private Vector3 localRestrictedAxis1;

	private Vector3 localRestrictedAxis2;

	private Vector2 error;

	private Vector3 localAxisAnchor;

	private Vector3 localLineDirection;

	private Vector3 localPoint;

	private Vector3 worldLineAnchor;

	private Vector3 worldLineDirection;

	private Vector3 worldPoint;

	private Matrix2X2 negativeEffectiveMassMatrix;

	private Vector3 rA;

	private Vector3 rB;

	private Vector3 worldRestrictedAxis1;

	private Vector3 worldRestrictedAxis2;

	/// <summary>
	/// Gets or sets the line anchor in world space.
	/// </summary>
	public Vector3 LineAnchor
	{
		get
		{
			return worldLineAnchor;
		}
		set
		{
			localAxisAnchor = value - connectionA.position;
			Matrix3X3.TransformTranspose(ref localAxisAnchor, ref connectionA.orientationMatrix, out localAxisAnchor);
			worldLineAnchor = value;
		}
	}

	/// <summary>
	/// Gets or sets the line direction in world space.
	/// </summary>
	public Vector3 LineDirection
	{
		get
		{
			return worldLineDirection;
		}
		set
		{
			worldLineDirection = Vector3.Normalize(value);
			Matrix3X3.TransformTranspose(ref worldLineDirection, ref connectionA.orientationMatrix, out localLineDirection);
			UpdateRestrictedAxes();
		}
	}

	/// <summary>
	/// Gets or sets the line anchor in connection A's local space.
	/// </summary>
	public Vector3 LocalLineAnchor
	{
		get
		{
			return localAxisAnchor;
		}
		set
		{
			localAxisAnchor = value;
			Matrix3X3.Transform(ref localAxisAnchor, ref connectionA.orientationMatrix, out worldLineAnchor);
			Vector3.Add(ref worldLineAnchor, ref connectionA.position, out worldLineAnchor);
		}
	}

	/// <summary>
	/// Gets or sets the line direction in connection A's local space.
	/// </summary>
	public Vector3 LocalLineDirection
	{
		get
		{
			return localLineDirection;
		}
		set
		{
			localLineDirection = Vector3.Normalize(value);
			Matrix3X3.Transform(ref localLineDirection, ref connectionA.orientationMatrix, out worldLineDirection);
			UpdateRestrictedAxes();
		}
	}

	/// <summary>
	/// Gets or sets the point's location in connection B's local space.
	/// The point is the location that is attached to the line.
	/// </summary>
	public Vector3 LocalPoint
	{
		get
		{
			return localPoint;
		}
		set
		{
			localPoint = value;
			Matrix3X3.Transform(ref localPoint, ref connectionB.orientationMatrix, out worldPoint);
			Vector3.Add(ref worldPoint, ref connectionB.position, out worldPoint);
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
	/// Gets or sets the point's location in world space.
	/// The point is the location on connection B that is attached to the line.
	/// </summary>
	public Vector3 Point
	{
		get
		{
			return worldPoint;
		}
		set
		{
			worldPoint = value;
			localPoint = worldPoint - connectionB.position;
			Matrix3X3.TransformTranspose(ref localPoint, ref connectionB.orientationMatrix, out localPoint);
		}
	}

	/// <summary>
	/// Gets the current relative velocity between the connected entities with respect to the constraint.
	/// </summary>
	public Vector2 RelativeVelocity
	{
		get
		{
			Vector2 result = default(Vector2);
			Vector3.Cross(ref connectionA.angularVelocity, ref rA, out var result2);
			Vector3.Add(ref result2, ref connectionA.linearVelocity, out result2);
			Vector3.Cross(ref connectionB.angularVelocity, ref rB, out var result3);
			Vector3.Add(ref result3, ref connectionB.linearVelocity, out result3);
			Vector3.Subtract(ref result2, ref result3, out var result4);
			Vector3.Dot(ref result4, ref worldRestrictedAxis1, out result.X);
			Vector3.Dot(ref result4, ref worldRestrictedAxis2, out result.Y);
			return result;
		}
	}

	/// <summary>
	/// Gets the total impulse applied by this constraint.
	/// </summary>
	public Vector2 TotalImpulse => accumulatedImpulse;

	/// <summary>
	/// Gets the current constraint error.
	/// </summary>
	public Vector2 Error => error;

	/// <summary>
	/// Constructs a joint which constrains a point of one body to be on a line based on the other body.
	/// To finish the initialization, specify the connections (ConnectionA and ConnectionB),
	/// the LineAnchor, the LineDirection, and the Point (or the entity-local versions).
	/// This constructor sets the constraint's IsActive property to false by default.
	/// </summary>
	public PointOnLineJoint()
	{
		base.IsActive = false;
	}

	/// <summary>
	/// Constructs a joint which constrains a point of one body to be on a line based on the other body.
	/// </summary>
	/// <param name="connectionA">First connected entity which defines the line.</param>
	/// <param name="connectionB">Second connected entity which has a point.</param>
	/// <param name="lineAnchor">Location off of which the line is based in world space.</param>
	/// <param name="lineDirection">Direction of the line in world space.</param>
	/// <param name="pointLocation">Location of the point anchored to connectionB in world space.</param>
	public PointOnLineJoint(Entity connectionA, Entity connectionB, Vector3 lineAnchor, Vector3 lineDirection, Vector3 pointLocation)
	{
		base.ConnectionA = connectionA;
		base.ConnectionB = connectionB;
		LineAnchor = lineAnchor;
		LineDirection = lineDirection;
		Point = pointLocation;
	}

	/// <summary>
	/// Gets the linear jacobian entry for the first connected entity.
	/// </summary>
	/// <param name="jacobianX">First linear jacobian entry for the first connected entity.</param>
	/// <param name="jacobianY">Second linear jacobian entry for the first connected entity.</param>
	public void GetLinearJacobianA(out Vector3 jacobianX, out Vector3 jacobianY)
	{
		jacobianX = worldRestrictedAxis1;
		jacobianY = worldRestrictedAxis2;
	}

	/// <summary>
	/// Gets the linear jacobian entry for the second connected entity.
	/// </summary>
	/// <param name="jacobianX">First linear jacobian entry for the second connected entity.</param>
	/// <param name="jacobianY">Second linear jacobian entry for the second connected entity.</param>
	public void GetLinearJacobianB(out Vector3 jacobianX, out Vector3 jacobianY)
	{
		jacobianX = -worldRestrictedAxis1;
		jacobianY = -worldRestrictedAxis2;
	}

	/// <summary>
	/// Gets the angular jacobian entry for the first connected entity.
	/// </summary>
	/// <param name="jacobianX">First angular jacobian entry for the first connected entity.</param>
	/// <param name="jacobianY">Second angular jacobian entry for the first connected entity.</param>
	public void GetAngularJacobianA(out Vector3 jacobianX, out Vector3 jacobianY)
	{
		jacobianX = angularA1;
		jacobianY = angularA2;
	}

	/// <summary>
	/// Gets the angular jacobian entry for the second connected entity.
	/// </summary>
	/// <param name="jacobianX">First angular jacobian entry for the second connected entity.</param>
	/// <param name="jacobianY">Second angular jacobian entry for the second connected entity.</param>
	public void GetAngularJacobianB(out Vector3 jacobianX, out Vector3 jacobianY)
	{
		jacobianX = angularB1;
		jacobianY = angularB2;
	}

	/// <summary>
	/// Gets the mass matrix of the constraint.
	/// </summary>
	/// <param name="massMatrix">Constraint's mass matrix.</param>
	public void GetMassMatrix(out Matrix2X2 massMatrix)
	{
		Matrix2X2.Negate(ref negativeEffectiveMassMatrix, out massMatrix);
	}

	/// <summary>
	/// Calculates and applies corrective impulses.
	/// Called automatically by space.
	/// </summary>
	public override float SolveIteration()
	{
		Vector2 v = default(Vector2);
		Vector3.Cross(ref connectionA.angularVelocity, ref rA, out var result);
		Vector3.Add(ref result, ref connectionA.linearVelocity, out result);
		Vector3.Cross(ref connectionB.angularVelocity, ref rB, out var result2);
		Vector3.Add(ref result2, ref connectionB.linearVelocity, out result2);
		Vector3.Subtract(ref result, ref result2, out var result3);
		Vector3.Dot(ref result3, ref worldRestrictedAxis1, out v.X);
		Vector3.Dot(ref result3, ref worldRestrictedAxis2, out v.Y);
		v.X += biasVelocity.X + softness * accumulatedImpulse.X;
		v.Y += biasVelocity.Y + softness * accumulatedImpulse.Y;
		Matrix2X2.Transform(ref v, ref negativeEffectiveMassMatrix, out v);
		Vector2.Add(ref v, ref accumulatedImpulse, out accumulatedImpulse);
		float x = v.X;
		float y = v.Y;
		Vector3 impulse = default(Vector3);
		Vector3 impulse2 = default(Vector3);
		impulse.X = worldRestrictedAxis1.X * x + worldRestrictedAxis2.X * y;
		impulse.Y = worldRestrictedAxis1.Y * x + worldRestrictedAxis2.Y * y;
		impulse.Z = worldRestrictedAxis1.Z * x + worldRestrictedAxis2.Z * y;
		if (connectionA.isDynamic)
		{
			impulse2.X = x * angularA1.X + y * angularA2.X;
			impulse2.Y = x * angularA1.Y + y * angularA2.Y;
			impulse2.Z = x * angularA1.Z + y * angularA2.Z;
			connectionA.ApplyLinearImpulse(ref impulse);
			connectionA.ApplyAngularImpulse(ref impulse2);
		}
		if (connectionB.isDynamic)
		{
			impulse.X = 0f - impulse.X;
			impulse.Y = 0f - impulse.Y;
			impulse.Z = 0f - impulse.Z;
			impulse2.X = x * angularB1.X + y * angularB2.X;
			impulse2.Y = x * angularB1.Y + y * angularB2.Y;
			impulse2.Z = x * angularB1.Z + y * angularB2.Z;
			connectionB.ApplyLinearImpulse(ref impulse);
			connectionB.ApplyAngularImpulse(ref impulse2);
		}
		return Math.Abs(v.X) + Math.Abs(v.Y);
	}

	/// <summary>
	///  Performs the frame's configuration step.
	/// </summary>
	/// <param name="dt">Timestep duration.</param>
	public override void Update(float dt)
	{
		Matrix3X3.Transform(ref localRestrictedAxis1, ref connectionA.orientationMatrix, out worldRestrictedAxis1);
		Matrix3X3.Transform(ref localRestrictedAxis2, ref connectionA.orientationMatrix, out worldRestrictedAxis2);
		Matrix3X3.Transform(ref localAxisAnchor, ref connectionA.orientationMatrix, out worldLineAnchor);
		Vector3.Add(ref worldLineAnchor, ref connectionA.position, out worldLineAnchor);
		Matrix3X3.Transform(ref localLineDirection, ref connectionA.orientationMatrix, out worldLineDirection);
		Matrix3X3.Transform(ref localPoint, ref connectionB.orientationMatrix, out rB);
		Vector3.Add(ref rB, ref connectionB.position, out worldPoint);
		Vector3.Subtract(ref worldPoint, ref worldLineAnchor, out var result);
		Vector3.Dot(ref result, ref worldLineDirection, out var result2);
		Vector3.Multiply(ref worldLineDirection, result2, out result);
		Vector3.Add(ref worldLineAnchor, ref result, out var result3);
		Vector3.Subtract(ref result3, ref connectionA.position, out rA);
		Vector3.Subtract(ref worldPoint, ref result3, out var result4);
		Vector3.Dot(ref result4, ref worldRestrictedAxis1, out error.X);
		Vector3.Dot(ref result4, ref worldRestrictedAxis2, out error.Y);
		springSettings.ComputeErrorReductionAndSoftness(dt, out var errorReduction, out softness);
		float num = 0f - errorReduction;
		biasVelocity.X = num * error.X;
		biasVelocity.Y = num * error.Y;
		float num2 = biasVelocity.LengthSquared();
		if (num2 > maxCorrectiveVelocitySquared)
		{
			float num3 = maxCorrectiveVelocity / (float)Math.Sqrt(num2);
			biasVelocity.X *= num3;
			biasVelocity.Y *= num3;
		}
		Vector3.Cross(ref rA, ref worldRestrictedAxis1, out angularA1);
		Vector3.Cross(ref worldRestrictedAxis1, ref rB, out angularB1);
		Vector3.Cross(ref rA, ref worldRestrictedAxis2, out angularA2);
		Vector3.Cross(ref worldRestrictedAxis2, ref rB, out angularB2);
		float result5 = 0f;
		float result6 = 0f;
		float result7 = 0f;
		Vector3 result8;
		if (connectionA.isDynamic)
		{
			float inverseMass = connectionA.inverseMass;
			Matrix3X3.Transform(ref angularA1, ref connectionA.inertiaTensorInverse, out result8);
			Vector3.Dot(ref result8, ref angularA1, out result5);
			result5 += inverseMass;
			Vector3.Dot(ref result8, ref angularA2, out result7);
			Matrix3X3.Transform(ref angularA2, ref connectionA.inertiaTensorInverse, out result8);
			Vector3.Dot(ref result8, ref angularA2, out result6);
			result6 += inverseMass;
		}
		if (connectionB.isDynamic)
		{
			float inverseMass = connectionB.inverseMass;
			Matrix3X3.Transform(ref angularB1, ref connectionB.inertiaTensorInverse, out result8);
			Vector3.Dot(ref result8, ref angularB1, out var result9);
			result5 += inverseMass + result9;
			Vector3.Dot(ref result8, ref angularB2, out result9);
			result7 += result9;
			Matrix3X3.Transform(ref angularB2, ref connectionB.inertiaTensorInverse, out result8);
			Vector3.Dot(ref result8, ref angularB2, out result9);
			result6 += inverseMass + result9;
		}
		negativeEffectiveMassMatrix.M11 = result5 + softness;
		negativeEffectiveMassMatrix.M12 = result7;
		negativeEffectiveMassMatrix.M21 = result7;
		negativeEffectiveMassMatrix.M22 = result6 + softness;
		Matrix2X2.Invert(ref negativeEffectiveMassMatrix, out negativeEffectiveMassMatrix);
		Matrix2X2.Negate(ref negativeEffectiveMassMatrix, out negativeEffectiveMassMatrix);
	}

	/// <summary>
	/// Performs any pre-solve iteration work that needs exclusive
	/// access to the members of the solver updateable.
	/// Usually, this is used for applying warmstarting impulses.
	/// </summary>
	public override void ExclusiveUpdate()
	{
		Vector3 impulse = default(Vector3);
		Vector3 impulse2 = default(Vector3);
		float x = accumulatedImpulse.X;
		float y = accumulatedImpulse.Y;
		impulse.X = worldRestrictedAxis1.X * x + worldRestrictedAxis2.X * y;
		impulse.Y = worldRestrictedAxis1.Y * x + worldRestrictedAxis2.Y * y;
		impulse.Z = worldRestrictedAxis1.Z * x + worldRestrictedAxis2.Z * y;
		if (connectionA.isDynamic)
		{
			impulse2.X = x * angularA1.X + y * angularA2.X;
			impulse2.Y = x * angularA1.Y + y * angularA2.Y;
			impulse2.Z = x * angularA1.Z + y * angularA2.Z;
			connectionA.ApplyLinearImpulse(ref impulse);
			connectionA.ApplyAngularImpulse(ref impulse2);
		}
		if (connectionB.isDynamic)
		{
			impulse.X = 0f - impulse.X;
			impulse.Y = 0f - impulse.Y;
			impulse.Z = 0f - impulse.Z;
			impulse2.X = x * angularB1.X + y * angularB2.X;
			impulse2.Y = x * angularB1.Y + y * angularB2.Y;
			impulse2.Z = x * angularB1.Z + y * angularB2.Z;
			connectionB.ApplyLinearImpulse(ref impulse);
			connectionB.ApplyAngularImpulse(ref impulse2);
		}
	}

	private void UpdateRestrictedAxes()
	{
		localRestrictedAxis1 = Vector3.Cross(Vector3.Up, localLineDirection);
		if (localRestrictedAxis1.LengthSquared() < 0.001f)
		{
			localRestrictedAxis1 = Vector3.Cross(Vector3.Right, localLineDirection);
		}
		localRestrictedAxis2 = Vector3.Cross(localLineDirection, localRestrictedAxis1);
		localRestrictedAxis1.Normalize();
		localRestrictedAxis2.Normalize();
	}
}
