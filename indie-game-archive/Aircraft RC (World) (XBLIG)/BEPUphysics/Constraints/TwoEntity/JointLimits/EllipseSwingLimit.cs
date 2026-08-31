using System;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.TwoEntity.JointLimits;

/// <summary>
/// Constrains the relative orientation of two entities to within an ellipse.
/// </summary>
public class EllipseSwingLimit : JointLimit, I1DImpulseConstraintWithError, I1DImpulseConstraint, I1DJacobianConstraint
{
	private readonly JointBasis3D basis = new JointBasis3D();

	private float accumulatedImpulse;

	private float biasVelocity;

	private Vector3 jacobianA;

	private Vector3 jacobianB;

	private float error;

	private Vector3 localTwistAxisB;

	private float maximumAngleX;

	private float maximumAngleY;

	private Vector3 worldTwistAxisB;

	private float velocityToImpulse;

	/// <summary>
	/// Gets the basis attached to entity A.
	/// The primary axis is the "twist" axis attached to entity A.
	/// The xAxis is the axis around which the angle will be limited by maximumAngleX.
	/// Similarly, the yAxis is the axis around which the angle will be limited by maximumAngleY.
	/// </summary>
	public JointBasis3D Basis => basis;

	/// <summary>
	/// Gets or sets the twist axis attached to entity B in its local space.
	/// The transformed twist axis will be used to determine the angles around entity A's basis axes.
	/// </summary>
	public Vector3 LocalTwistAxisB
	{
		get
		{
			return localTwistAxisB;
		}
		set
		{
			localTwistAxisB = value;
			Matrix3X3.Transform(ref localTwistAxisB, ref connectionB.orientationMatrix, out worldTwistAxisB);
		}
	}

	/// <summary>
	/// Gets or sets the maximum angle of rotation around the x axis.
	/// This can be thought of as the major radius of the swing limit's ellipse.
	/// </summary>
	public float MaximumAngleX
	{
		get
		{
			return maximumAngleX;
		}
		set
		{
			maximumAngleX = MathHelper.Clamp(value, 1E-05f, (float)Math.PI);
		}
	}

	/// <summary>
	/// Gets or sets the maximum angle of rotation around the y axis.
	/// This can be thought of as the minor radius of the swing limit's ellipse.
	/// </summary>
	public float MaximumAngleY
	{
		get
		{
			return maximumAngleY;
		}
		set
		{
			maximumAngleY = MathHelper.Clamp(value, 1E-05f, (float)Math.PI);
		}
	}

	/// <summary>
	/// Gets or sets the twist axis attached to entity B in world space.
	/// The transformed twist axis will be used to determine the angles around entity A's basis axes.
	/// </summary>
	public Vector3 TwistAxisB
	{
		get
		{
			return worldTwistAxisB;
		}
		set
		{
			worldTwistAxisB = value;
			Matrix3X3.TransformTranspose(ref worldTwistAxisB, ref connectionB.orientationMatrix, out localTwistAxisB);
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
				Vector3.Dot(ref connectionA.angularVelocity, ref jacobianA, out var result);
				Vector3.Dot(ref connectionB.angularVelocity, ref jacobianB, out var result2);
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
	/// Constructs a new swing limit.
	/// To finish the initialization, specify the connections (ConnectionA and ConnectionB) 
	/// as well as the TwistAxis (or its entity-local version),
	/// the MaximumAngleX and MaximumAngleY,
	/// and the Basis.
	/// This constructor sets the constraint's IsActive property to false by default.
	/// </summary>
	public EllipseSwingLimit()
	{
		base.SpringSettings.StiffnessConstant /= 5f;
		base.SpringSettings.Advanced.ErrorReductionFactor /= 5f;
		base.Margin = 0.05f;
		base.IsActive = false;
	}

	/// <summary>
	/// Constructs a new swing limit.
	/// </summary>
	/// <param name="connectionA">First entity connected by the constraint.</param>
	/// <param name="connectionB">Second entity connected by the constraint.</param>
	/// <param name="twistAxis">Axis in world space to use as the initial unrestricted twist direction.
	/// This direction will be transformed to entity A's local space to form the basis's primary axis
	/// and to entity B's local space to form its twist axis.
	/// The basis's x and y axis are automatically created from the twist axis.</param>
	/// <param name="maximumAngleX">Maximum angle of rotation around the basis X axis.</param>
	/// <param name="maximumAngleY">Maximum angle of rotation around the basis Y axis.</param>
	public EllipseSwingLimit(Entity connectionA, Entity connectionB, Vector3 twistAxis, float maximumAngleX, float maximumAngleY)
	{
		base.ConnectionA = connectionA;
		base.ConnectionB = connectionB;
		SetupJointTransforms(twistAxis);
		MaximumAngleX = maximumAngleX;
		MaximumAngleY = maximumAngleY;
		base.SpringSettings.StiffnessConstant /= 5f;
		base.SpringSettings.Advanced.ErrorReductionFactor /= 5f;
		base.Margin = 0.05f;
	}

	/// <summary>
	/// Constructs a new swing limit.
	/// Using this constructor will leave the limit uninitialized.  Before using the limit in a simulation, be sure to set the basis axes using
	/// limit.basis.setLocalAxes or limit.basis.setWorldAxes and b's twist axis using the localTwistAxisB or twistAxisB properties.
	/// </summary>
	/// <param name="connectionA">First entity connected by the constraint.</param>
	/// <param name="connectionB">Second entity connected by the constraint.</param>
	public EllipseSwingLimit(Entity connectionA, Entity connectionB)
	{
		base.ConnectionA = connectionA;
		base.ConnectionB = connectionB;
		base.SpringSettings.StiffnessConstant /= 5f;
		base.SpringSettings.Advanced.ErrorReductionFactor /= 5f;
		base.Margin = 0.05f;
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
	/// Sets up the joint transforms by automatically creating perpendicular vectors to complete the bases.
	/// </summary>
	/// <param name="twistAxis">Axis around which rotation is allowed.</param>
	public void SetupJointTransforms(Vector3 twistAxis)
	{
		Vector3.Cross(ref twistAxis, ref Toolbox.UpVector, out var result);
		float num = result.LengthSquared();
		if (num < 1E-07f)
		{
			Vector3.Cross(ref twistAxis, ref Toolbox.RightVector, out result);
		}
		Vector3.Cross(ref twistAxis, ref result, out var result2);
		basis.rotationMatrix = connectionA.orientationMatrix;
		basis.SetWorldAxes(twistAxis, result, result2);
		TwistAxisB = twistAxis;
	}

	/// <summary>
	/// Computes one iteration of the constraint to meet the solver updateable's goal.
	/// </summary>
	/// <returns>The rough applied impulse magnitude.</returns>
	public override float SolveIteration()
	{
		Vector3.Dot(ref connectionA.angularVelocity, ref jacobianA, out var result);
		Vector3.Dot(ref connectionB.angularVelocity, ref jacobianB, out var result2);
		float num = 0f - result - result2 - biasVelocity - softness * accumulatedImpulse;
		num *= velocityToImpulse;
		float num2 = accumulatedImpulse;
		accumulatedImpulse = MathHelper.Min(accumulatedImpulse + num, 0f);
		num = accumulatedImpulse - num2;
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
	///  Performs the frame's configuration step.
	/// </summary>
	/// <param name="dt">Timestep duration.</param>
	public override void Update(float dt)
	{
		basis.rotationMatrix = connectionA.orientationMatrix;
		basis.ComputeWorldSpaceAxes();
		Matrix3X3.Transform(ref localTwistAxisB, ref connectionB.orientationMatrix, out worldTwistAxisB);
		Toolbox.GetQuaternionBetweenNormalizedVectors(ref worldTwistAxisB, ref basis.primaryAxis, out var q);
		Toolbox.GetAxisAngleFromQuaternion(ref q, out var axis, out var angle);
		Vector3 vector = new Vector3
		{
			X = axis.X * angle,
			Y = axis.Y * angle,
			Z = axis.Z * angle
		};
		Vector3.Dot(ref vector, ref basis.xAxis, out var result);
		Vector3.Dot(ref vector, ref basis.yAxis, out var result2);
		float num = maximumAngleX * maximumAngleX;
		float num2 = maximumAngleY * maximumAngleY;
		error = result * result * num2 + result2 * result2 * num - num * num2;
		if (error <= 0f)
		{
			isActiveInSolver = false;
			error = 0f;
			accumulatedImpulse = 0f;
			isLimitActive = false;
			return;
		}
		isLimitActive = true;
		Vector2 vector2 = default(Vector2);
		vector2.X = result / num;
		vector2.Y = result2 / num2;
		vector2.Normalize();
		Quaternion.Conjugate(ref q, out q);
		Vector3.Transform(ref basis.xAxis, ref q, out var result3);
		Vector3.Transform(ref basis.yAxis, ref q, out var result4);
		Vector3.Multiply(ref result3, vector2.X, out jacobianA);
		Vector3.Multiply(ref result4, vector2.Y, out jacobianB);
		Vector3.Add(ref jacobianA, ref jacobianB, out jacobianA);
		jacobianB.X = 0f - jacobianA.X;
		jacobianB.Y = 0f - jacobianA.Y;
		jacobianB.Z = 0f - jacobianA.Z;
		springSettings.ComputeErrorReductionAndSoftness(dt, out var errorReduction, out softness);
		error -= margin;
		biasVelocity = MathHelper.Min(Math.Max(error, 0f) * errorReduction, maxCorrectiveVelocity);
		if (bounciness > 0f)
		{
			Vector3.Dot(ref connectionA.angularVelocity, ref jacobianA, out var result5);
			Vector3.Dot(ref connectionB.angularVelocity, ref jacobianB, out var result6);
			result5 += result6;
			result5 /= 5f;
			if (result5 > bounceVelocityThreshold)
			{
				biasVelocity = MathHelper.Max(biasVelocity, bounciness * result5);
			}
		}
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
}
