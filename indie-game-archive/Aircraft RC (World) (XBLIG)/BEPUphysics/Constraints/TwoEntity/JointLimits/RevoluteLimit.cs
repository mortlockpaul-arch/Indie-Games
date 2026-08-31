using System;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.TwoEntity.JointLimits;

/// <summary>
/// Constraint which prevents the connected entities from rotating relative to each other around an axis beyond given limits.
/// </summary>
public class RevoluteLimit : JointLimit, I2DImpulseConstraintWithError, I2DImpulseConstraint, I2DJacobianConstraint
{
	private readonly JointBasis2D basis = new JointBasis2D();

	private Vector2 accumulatedImpulse;

	private Vector2 biasVelocity;

	private Vector3 jacobianMaxA;

	private Vector3 jacobianMaxB;

	private Vector3 jacobianMinA;

	private Vector3 jacobianMinB;

	private bool maxIsActive;

	private bool minIsActive;

	private Vector2 error;

	private Vector3 localTestAxis;

	/// <summary>
	/// Naximum angle that entities can twist.
	/// </summary>
	protected float maximumAngle;

	/// <summary>
	/// Minimum angle that entities can twist.
	/// </summary>
	protected float minimumAngle;

	private Vector3 worldTestAxis;

	private Vector2 velocityToImpulse;

	/// <summary>
	/// Gets the basis attached to entity A.
	/// The primary axis represents the limited axis of rotation.  The 'measurement plane' which the test axis is tested against is based on this primary axis.
	/// The x axis defines the 'base' direction on the measurement plane corresponding to 0 degrees of relative rotation.
	/// </summary>
	public JointBasis2D Basis => basis;

	/// <summary>
	/// Gets or sets the axis attached to entity B in its local space that will be tested against the limits.
	/// </summary>
	public Vector3 LocalTestAxis
	{
		get
		{
			return localTestAxis;
		}
		set
		{
			localTestAxis = Vector3.Normalize(value);
			Matrix3X3.Transform(ref localTestAxis, ref connectionB.orientationMatrix, out worldTestAxis);
		}
	}

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
	/// Gets or sets the axis attached to entity B in world space that will be tested against the limits.
	/// </summary>
	public Vector3 TestAxis
	{
		get
		{
			return worldTestAxis;
		}
		set
		{
			worldTestAxis = Vector3.Normalize(value);
			Matrix3X3.TransformTranspose(ref worldTestAxis, ref connectionB.orientationMatrix, out localTestAxis);
		}
	}

	/// <summary>
	/// Gets the current relative velocity between the connected entities with respect to the constraint.
	/// The revolute limit is special; internally, it is sometimes two constraints.
	/// The X value of the vector is the "minimum" plane of the limit, and the Y value is the "maximum" plane.
	/// If a plane isn't active, its error is zero.
	/// </summary>
	public Vector2 RelativeVelocity
	{
		get
		{
			if (isLimitActive)
			{
				Vector2 zero = Vector2.Zero;
				float result;
				float result2;
				if (minIsActive)
				{
					Vector3.Dot(ref connectionA.angularVelocity, ref jacobianMinA, out result);
					Vector3.Dot(ref connectionB.angularVelocity, ref jacobianMinB, out result2);
					zero.X = result + result2;
				}
				if (maxIsActive)
				{
					Vector3.Dot(ref connectionA.angularVelocity, ref jacobianMaxA, out result);
					Vector3.Dot(ref connectionB.angularVelocity, ref jacobianMaxB, out result2);
					zero.Y = result + result2;
				}
				return zero;
			}
			return default(Vector2);
		}
	}

	/// <summary>
	/// Gets the total impulse applied by this constraint.
	/// The x component corresponds to the minimum plane limit,
	/// while the y component corresponds to the maximum plane limit.
	/// </summary>
	public Vector2 TotalImpulse => accumulatedImpulse;

	/// <summary>
	/// Gets the current constraint error.
	/// The x component corresponds to the minimum plane limit,
	/// while the y component corresponds to the maximum plane limit.
	/// </summary>
	public Vector2 Error => error;

	/// <summary>
	/// Constructs a new constraint which prevents the connected entities from rotating relative to each other around an axis beyond given limits.
	/// To finish the initialization, specify the connections (ConnectionA and ConnectionB) 
	/// as well as the TestAxis (or its entity-local version) and the Basis.
	/// This constructor sets the constraint's IsActive property to false by default.
	/// </summary>
	public RevoluteLimit()
	{
		base.IsActive = false;
	}

	/// <summary>
	/// Constructs a new constraint which prevents the connected entities from rotating relative to each other around an axis beyond given limits.
	/// </summary>
	/// <param name="connectionA">First connection of the pair.</param>
	/// <param name="connectionB">Second connection of the pair.</param>
	/// <param name="limitedAxis">Axis of rotation to be limited.</param>
	/// <param name="testAxis">Axis attached to connectionB that is tested to determine the current angle.
	/// Will also be used as the base rotation axis representing 0 degrees.</param>
	/// <param name="minimumAngle">Minimum twist angle allowed.</param>
	/// <param name="maximumAngle">Maximum twist angle allowed.</param>
	public RevoluteLimit(Entity connectionA, Entity connectionB, Vector3 limitedAxis, Vector3 testAxis, float minimumAngle, float maximumAngle)
	{
		base.ConnectionA = connectionA;
		base.ConnectionB = connectionB;
		basis.rotationMatrix = base.connectionA.orientationMatrix;
		basis.SetWorldAxes(limitedAxis, testAxis);
		TestAxis = basis.xAxis;
		MinimumAngle = minimumAngle;
		MaximumAngle = maximumAngle;
	}

	/// <summary>
	/// Constructs a new constraint which prevents the connected entities from rotating relative to each other around an axis beyond given limits.
	/// Using this constructor will leave the limit uninitialized.  Before using the limit in a simulation, be sure to set the basis axes using
	/// Basis.SetLocalAxes or Basis.SetWorldAxes and the test axis using the LocalTestAxis or TestAxis properties.
	/// </summary>
	/// <param name="connectionA">First connection of the pair.</param>
	/// <param name="connectionB">Second connection of the pair.</param>
	public RevoluteLimit(Entity connectionA, Entity connectionB)
	{
		base.ConnectionA = connectionA;
		base.ConnectionB = connectionB;
	}

	/// <summary>
	/// Gets the linear jacobian entry for the first connected entity.
	/// </summary>
	/// <param name="jacobianX">First linear jacobian entry for the first connected entity.</param>
	/// <param name="jacobianY">Second linear jacobian entry for the first connected entity.</param>
	public void GetLinearJacobianA(out Vector3 jacobianX, out Vector3 jacobianY)
	{
		jacobianX = Toolbox.ZeroVector;
		jacobianY = Toolbox.ZeroVector;
	}

	/// <summary>
	/// Gets the linear jacobian entry for the second connected entity.
	/// </summary>
	/// <param name="jacobianX">First linear jacobian entry for the second connected entity.</param>
	/// <param name="jacobianY">Second linear jacobian entry for the second connected entity.</param>
	public void GetLinearJacobianB(out Vector3 jacobianX, out Vector3 jacobianY)
	{
		jacobianX = Toolbox.ZeroVector;
		jacobianY = Toolbox.ZeroVector;
	}

	/// <summary>
	/// Gets the angular jacobian entry for the first connected entity.
	/// </summary>
	/// <param name="jacobianX">First angular jacobian entry for the first connected entity.</param>
	/// <param name="jacobianY">Second angular jacobian entry for the first connected entity.</param>
	public void GetAngularJacobianA(out Vector3 jacobianX, out Vector3 jacobianY)
	{
		jacobianX = jacobianMinA;
		jacobianY = jacobianMaxA;
	}

	/// <summary>
	/// Gets the angular jacobian entry for the second connected entity.
	/// </summary>
	/// <param name="jacobianX">First angular jacobian entry for the second connected entity.</param>
	/// <param name="jacobianY">Second angular jacobian entry for the second connected entity.</param>
	public void GetAngularJacobianB(out Vector3 jacobianX, out Vector3 jacobianY)
	{
		jacobianX = jacobianMinB;
		jacobianY = jacobianMaxB;
	}

	/// <summary>
	/// Gets the mass matrix of the revolute limit.
	/// The revolute limit is special; in terms of solving, it is
	/// actually sometimes TWO constraints; a minimum plane, and a
	/// maximum plane.  The M11 field represents the minimum plane mass matrix
	/// and the M22 field represents the maximum plane mass matrix.
	/// </summary>
	/// <param name="massMatrix">Mass matrix of the constraint.</param>
	public void GetMassMatrix(out Matrix2X2 massMatrix)
	{
		massMatrix.M11 = velocityToImpulse.X;
		massMatrix.M22 = velocityToImpulse.Y;
		massMatrix.M12 = 0f;
		massMatrix.M21 = 0f;
	}

	/// <summary>
	/// Computes one iteration of the constraint to meet the solver updateable's goal.
	/// </summary>
	/// <returns>The rough applied impulse magnitude.</returns>
	public override float SolveIteration()
	{
		float num = 0f;
		float result;
		float result2;
		if (minIsActive)
		{
			Vector3.Dot(ref connectionA.angularVelocity, ref jacobianMinA, out result);
			Vector3.Dot(ref connectionB.angularVelocity, ref jacobianMinB, out result2);
			float num2 = 0f - (result + result2) + biasVelocity.X - softness * accumulatedImpulse.X;
			num2 *= velocityToImpulse.X;
			float x = accumulatedImpulse.X;
			accumulatedImpulse.X = MathHelper.Max(accumulatedImpulse.X + num2, 0f);
			num2 = accumulatedImpulse.X - x;
			Vector3 result3;
			if (connectionA.isDynamic)
			{
				Vector3.Multiply(ref jacobianMinA, num2, out result3);
				connectionA.ApplyAngularImpulse(ref result3);
			}
			if (connectionB.isDynamic)
			{
				Vector3.Multiply(ref jacobianMinB, num2, out result3);
				connectionB.ApplyAngularImpulse(ref result3);
			}
			num += Math.Abs(num2);
		}
		if (maxIsActive)
		{
			Vector3.Dot(ref connectionA.angularVelocity, ref jacobianMaxA, out result);
			Vector3.Dot(ref connectionB.angularVelocity, ref jacobianMaxB, out result2);
			float num2 = 0f - (result + result2) + biasVelocity.Y - softness * accumulatedImpulse.Y;
			num2 *= velocityToImpulse.Y;
			float x = accumulatedImpulse.Y;
			accumulatedImpulse.Y = MathHelper.Max(accumulatedImpulse.Y + num2, 0f);
			num2 = accumulatedImpulse.Y - x;
			Vector3 result4;
			if (connectionA.isDynamic)
			{
				Vector3.Multiply(ref jacobianMaxA, num2, out result4);
				connectionA.ApplyAngularImpulse(ref result4);
			}
			if (connectionB.isDynamic)
			{
				Vector3.Multiply(ref jacobianMaxB, num2, out result4);
				connectionB.ApplyAngularImpulse(ref result4);
			}
			num += Math.Abs(num2);
		}
		return num;
	}

	/// <summary>
	///  Performs the frame's configuration step.
	/// </summary>
	/// <param name="dt">Timestep duration.</param>
	public override void Update(float dt)
	{
		basis.rotationMatrix = connectionA.orientationMatrix;
		basis.ComputeWorldSpaceAxes();
		Matrix3X3.Transform(ref localTestAxis, ref connectionB.orientationMatrix, out worldTestAxis);
		Matrix.CreateFromAxisAngle(ref basis.primaryAxis, minimumAngle + (float)Math.PI / 2f, out var result);
		Vector3.TransformNormal(ref basis.xAxis, ref result, out var result2);
		Matrix.CreateFromAxisAngle(ref basis.primaryAxis, maximumAngle - (float)Math.PI / 2f, out result);
		Vector3.TransformNormal(ref basis.xAxis, ref result, out var result3);
		Vector3.Dot(ref result2, ref worldTestAxis, out var result4);
		Vector3.Dot(ref result3, ref worldTestAxis, out var result5);
		float distanceFromMinimum = GetDistanceFromMinimum(maximumAngle);
		if (distanceFromMinimum >= (float)Math.PI)
		{
			if (result5 > 0f || result4 > 0f)
			{
				isActiveInSolver = false;
				minIsActive = false;
				maxIsActive = false;
				error = Vector2.Zero;
				accumulatedImpulse = Vector2.Zero;
				isLimitActive = false;
				return;
			}
			if (result5 > result4)
			{
				error.X = 0f;
				error.Y = 0f - result5;
				accumulatedImpulse.X = 0f;
				minIsActive = false;
				maxIsActive = true;
			}
			else
			{
				error.X = 0f - result4;
				error.Y = 0f;
				accumulatedImpulse.Y = 0f;
				minIsActive = true;
				maxIsActive = false;
			}
		}
		else
		{
			if (result5 > 0f && result4 > 0f)
			{
				isActiveInSolver = false;
				minIsActive = false;
				maxIsActive = false;
				error = Vector2.Zero;
				accumulatedImpulse = Vector2.Zero;
				isLimitActive = false;
				return;
			}
			if (result4 <= 0f && result5 <= 0f)
			{
				error.X = 0f - result4;
				error.Y = 0f - result5;
				minIsActive = true;
				maxIsActive = true;
			}
			else if (result4 <= 0f)
			{
				error.X = 0f - result4;
				error.Y = 0f;
				accumulatedImpulse.Y = 0f;
				minIsActive = true;
				maxIsActive = false;
			}
			else
			{
				error.X = 0f;
				error.Y = 0f - result5;
				accumulatedImpulse.X = 0f;
				minIsActive = false;
				maxIsActive = true;
			}
		}
		isLimitActive = true;
		springSettings.ComputeErrorReductionAndSoftness(dt, out var errorReduction, out softness);
		if (minIsActive)
		{
			Vector3.Cross(ref result2, ref worldTestAxis, out jacobianMinA);
			if (jacobianMinA.LengthSquared() < 1E-07f)
			{
				jacobianMinA = basis.primaryAxis;
			}
			jacobianMinA.Normalize();
			jacobianMinB.X = 0f - jacobianMinA.X;
			jacobianMinB.Y = 0f - jacobianMinA.Y;
			jacobianMinB.Z = 0f - jacobianMinA.Z;
		}
		if (maxIsActive)
		{
			Vector3.Cross(ref result3, ref worldTestAxis, out jacobianMaxA);
			if (jacobianMaxA.LengthSquared() < 1E-07f)
			{
				jacobianMaxA = basis.primaryAxis;
			}
			jacobianMaxA.Normalize();
			jacobianMaxB.X = 0f - jacobianMaxA.X;
			jacobianMaxB.Y = 0f - jacobianMaxA.Y;
			jacobianMaxB.Z = 0f - jacobianMaxA.Z;
		}
		if (minIsActive)
		{
			biasVelocity.X = MathHelper.Min(MathHelper.Max(0f, error.X - margin) * errorReduction, maxCorrectiveVelocity);
			if (bounciness > 0f)
			{
				Vector3.Dot(ref connectionA.angularVelocity, ref jacobianMinA, out var result6);
				Vector3.Dot(ref connectionB.angularVelocity, ref jacobianMinB, out var result7);
				result6 += result7;
				if (0f - result6 > bounceVelocityThreshold)
				{
					biasVelocity.X = MathHelper.Max(biasVelocity.X, (0f - bounciness) * result6);
				}
			}
		}
		if (maxIsActive)
		{
			biasVelocity.Y = MathHelper.Min(MathHelper.Max(0f, error.Y - margin) * errorReduction, maxCorrectiveVelocity);
			if (bounciness > 0f && maxIsActive)
			{
				Vector3.Dot(ref connectionA.angularVelocity, ref jacobianMaxA, out var result8);
				Vector3.Dot(ref connectionB.angularVelocity, ref jacobianMaxB, out var result9);
				result8 += result9;
				if (0f - result8 > bounceVelocityThreshold)
				{
					biasVelocity.Y = MathHelper.Max(biasVelocity.Y, (0f - bounciness) * result8);
				}
			}
		}
		Vector3 result10;
		float result11;
		float result12;
		if (connectionA.isDynamic)
		{
			if (minIsActive)
			{
				Matrix3X3.Transform(ref jacobianMinA, ref connectionA.inertiaTensorInverse, out result10);
				Vector3.Dot(ref result10, ref jacobianMinA, out result11);
			}
			else
			{
				result11 = 0f;
			}
			if (maxIsActive)
			{
				Matrix3X3.Transform(ref jacobianMaxA, ref connectionA.inertiaTensorInverse, out result10);
				Vector3.Dot(ref result10, ref jacobianMaxA, out result12);
			}
			else
			{
				result12 = 0f;
			}
		}
		else
		{
			result11 = 0f;
			result12 = 0f;
		}
		float result13;
		float result14;
		if (connectionB.isDynamic)
		{
			if (minIsActive)
			{
				Matrix3X3.Transform(ref jacobianMinB, ref connectionB.inertiaTensorInverse, out result10);
				Vector3.Dot(ref result10, ref jacobianMinB, out result13);
			}
			else
			{
				result13 = 0f;
			}
			if (maxIsActive)
			{
				Matrix3X3.Transform(ref jacobianMaxB, ref connectionB.inertiaTensorInverse, out result10);
				Vector3.Dot(ref result10, ref jacobianMaxB, out result14);
			}
			else
			{
				result14 = 0f;
			}
		}
		else
		{
			result13 = 0f;
			result14 = 0f;
		}
		velocityToImpulse.X = 1f / (softness + result11 + result13);
		velocityToImpulse.Y = 1f / (softness + result12 + result14);
	}

	/// <summary>
	/// Performs any pre-solve iteration work that needs exclusive
	/// access to the members of the solver updateable.
	/// Usually, this is used for applying warmstarting impulses.
	/// </summary>
	public override void ExclusiveUpdate()
	{
		if (connectionA.isDynamic)
		{
			Vector3 result = default(Vector3);
			if (minIsActive)
			{
				Vector3.Multiply(ref jacobianMinA, accumulatedImpulse.X, out result);
			}
			if (maxIsActive)
			{
				Vector3.Multiply(ref jacobianMaxA, accumulatedImpulse.Y, out var result2);
				Vector3.Add(ref result, ref result2, out result);
			}
			connectionA.ApplyAngularImpulse(ref result);
		}
		if (connectionB.isDynamic)
		{
			Vector3 result3 = default(Vector3);
			if (minIsActive)
			{
				Vector3.Multiply(ref jacobianMinB, accumulatedImpulse.X, out result3);
			}
			if (maxIsActive)
			{
				Vector3.Multiply(ref jacobianMaxB, accumulatedImpulse.Y, out var result4);
				Vector3.Add(ref result3, ref result4, out result3);
			}
			connectionB.ApplyAngularImpulse(ref result3);
		}
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
}
