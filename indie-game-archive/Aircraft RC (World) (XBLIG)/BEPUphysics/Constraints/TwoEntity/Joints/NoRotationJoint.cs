using System;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.TwoEntity.Joints;

/// <summary>
/// Constrains two entities so that they cannot rotate relative to each other.
/// </summary>
public class NoRotationJoint : Joint, I3DImpulseConstraintWithError, I3DImpulseConstraint, I3DJacobianConstraint
{
	private Vector3 accumulatedImpulse;

	private Vector3 biasVelocity;

	private Matrix3X3 effectiveMassMatrix;

	private Quaternion initialQuaternionConjugateA;

	private Quaternion initialQuaternionConjugateB;

	private Vector3 error;

	/// <summary>
	/// Gets or sets the initial orientation of the first connected entity.
	/// The constraint will try to maintain the relative orientation between the initialOrientationA and initialOrientationB.
	/// </summary>
	public Quaternion InitialOrientationA
	{
		get
		{
			return Quaternion.Conjugate(initialQuaternionConjugateA);
		}
		set
		{
			initialQuaternionConjugateA = Quaternion.Conjugate(value);
		}
	}

	/// <summary>
	/// Gets or sets the initial orientation of the second connected entity.
	/// The constraint will try to maintain the relative orientation between the initialOrientationA and initialOrientationB.
	/// </summary>
	public Quaternion InitialOrientationB
	{
		get
		{
			return Quaternion.Conjugate(initialQuaternionConjugateB);
		}
		set
		{
			initialQuaternionConjugateB = Quaternion.Conjugate(value);
		}
	}

	/// <summary>
	/// Gets the current relative velocity between the connected entities with respect to the constraint.
	/// </summary>
	public Vector3 RelativeVelocity
	{
		get
		{
			Vector3.Subtract(ref connectionB.angularVelocity, ref connectionA.angularVelocity, out var result);
			return result;
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
	/// Constructs a new constraint which prevents relative angular motion between the two connected bodies.
	/// To finish the initialization, specify the connections (ConnectionA and ConnectionB) and the initial orientations
	/// (InitialOrientationA, InitialOrientationB).
	/// This constructor sets the constraint's IsActive property to false by default.
	/// </summary>
	public NoRotationJoint()
	{
		base.IsActive = false;
	}

	/// <summary>
	/// Constructs a new constraint which prevents relative angular motion between the two connected bodies.
	/// </summary>
	/// <param name="connectionA">First connection of the pair.</param>
	/// <param name="connectionB">Second connection of the pair.</param>
	public NoRotationJoint(Entity connectionA, Entity connectionB)
	{
		base.ConnectionA = connectionA;
		base.ConnectionB = connectionB;
		initialQuaternionConjugateA = Quaternion.Conjugate(base.ConnectionA.orientation);
		initialQuaternionConjugateB = Quaternion.Conjugate(base.ConnectionB.orientation);
	}

	/// <summary>
	/// Gets the linear jacobian entry for the first connected entity.
	/// </summary>
	/// <param name="jacobianX">First linear jacobian entry for the first connected entity.</param>
	/// <param name="jacobianY">Second linear jacobian entry for the first connected entity.</param>
	/// <param name="jacobianZ">Third linear jacobian entry for the first connected entity.</param>
	public void GetLinearJacobianA(out Vector3 jacobianX, out Vector3 jacobianY, out Vector3 jacobianZ)
	{
		jacobianX = Toolbox.ZeroVector;
		jacobianY = Toolbox.ZeroVector;
		jacobianZ = Toolbox.ZeroVector;
	}

	/// <summary>
	/// Gets the linear jacobian entry for the second connected entity.
	/// </summary>
	/// <param name="jacobianX">First linear jacobian entry for the second connected entity.</param>
	/// <param name="jacobianY">Second linear jacobian entry for the second connected entity.</param>
	/// <param name="jacobianZ">Third linear jacobian entry for the second connected entity.</param>
	public void GetLinearJacobianB(out Vector3 jacobianX, out Vector3 jacobianY, out Vector3 jacobianZ)
	{
		jacobianX = Toolbox.ZeroVector;
		jacobianY = Toolbox.ZeroVector;
		jacobianZ = Toolbox.ZeroVector;
	}

	/// <summary>
	/// Gets the angular jacobian entry for the first connected entity.
	/// </summary>
	/// <param name="jacobianX">First angular jacobian entry for the first connected entity.</param>
	/// <param name="jacobianY">Second angular jacobian entry for the first connected entity.</param>
	/// <param name="jacobianZ">Third angular jacobian entry for the first connected entity.</param>
	public void GetAngularJacobianA(out Vector3 jacobianX, out Vector3 jacobianY, out Vector3 jacobianZ)
	{
		jacobianX = Toolbox.RightVector;
		jacobianY = Toolbox.UpVector;
		jacobianZ = Toolbox.BackVector;
	}

	/// <summary>
	/// Gets the angular jacobian entry for the second connected entity.
	/// </summary>
	/// <param name="jacobianX">First angular jacobian entry for the second connected entity.</param>
	/// <param name="jacobianY">Second angular jacobian entry for the second connected entity.</param>
	/// <param name="jacobianZ">Third angular jacobian entry for the second connected entity.</param>
	public void GetAngularJacobianB(out Vector3 jacobianX, out Vector3 jacobianY, out Vector3 jacobianZ)
	{
		jacobianX = Toolbox.RightVector;
		jacobianY = Toolbox.UpVector;
		jacobianZ = Toolbox.BackVector;
	}

	/// <summary>
	/// Gets the mass matrix of the constraint.
	/// </summary>
	/// <param name="outputMassMatrix">Constraint's mass matrix.</param>
	public void GetMassMatrix(out Matrix3X3 outputMassMatrix)
	{
		outputMassMatrix = effectiveMassMatrix;
	}

	/// <summary>
	/// Applies the corrective impulses required by the constraint.
	/// </summary>
	public override float SolveIteration()
	{
		Vector3.Subtract(ref connectionB.angularVelocity, ref connectionA.angularVelocity, out var result);
		Vector3.Multiply(ref accumulatedImpulse, softness, out var result2);
		Vector3.Add(ref result, ref biasVelocity, out var result3);
		Vector3.Subtract(ref result3, ref result2, out result3);
		Matrix3X3.Transform(ref result3, ref effectiveMassMatrix, out result3);
		Vector3.Add(ref result3, ref accumulatedImpulse, out accumulatedImpulse);
		if (connectionA.isDynamic)
		{
			connectionA.ApplyAngularImpulse(ref result3);
		}
		if (connectionB.isDynamic)
		{
			Vector3.Negate(ref result3, out var result4);
			connectionB.ApplyAngularImpulse(ref result4);
		}
		return Math.Abs(result3.X) + Math.Abs(result3.Y) + Math.Abs(result3.Z);
	}

	/// <summary>
	/// Initializes the constraint for the current frame.
	/// </summary>
	/// <param name="dt">Time between frames.</param>
	public override void Update(float dt)
	{
		Quaternion.Multiply(ref connectionA.orientation, ref initialQuaternionConjugateA, out var result);
		Quaternion.Multiply(ref connectionB.orientation, ref initialQuaternionConjugateB, out var result2);
		Quaternion.Conjugate(ref result2, out result2);
		Quaternion.Multiply(ref result, ref result2, out var result3);
		Toolbox.GetAxisAngleFromQuaternion(ref result3, out var axis, out var angle);
		error.X = axis.X * angle;
		error.Y = axis.Y * angle;
		error.Z = axis.Z * angle;
		springSettings.ComputeErrorReductionAndSoftness(dt, out var errorReduction, out softness);
		errorReduction = 0f - errorReduction;
		biasVelocity.X = errorReduction * error.X;
		biasVelocity.Y = errorReduction * error.Y;
		biasVelocity.Z = errorReduction * error.Z;
		float num = biasVelocity.LengthSquared();
		if (num > maxCorrectiveVelocitySquared)
		{
			float num2 = maxCorrectiveVelocity / (float)Math.Sqrt(num);
			biasVelocity.X *= num2;
			biasVelocity.Y *= num2;
			biasVelocity.Z *= num2;
		}
		Matrix3X3.Add(ref connectionA.inertiaTensorInverse, ref connectionB.inertiaTensorInverse, out effectiveMassMatrix);
		effectiveMassMatrix.M11 += softness;
		effectiveMassMatrix.M22 += softness;
		effectiveMassMatrix.M33 += softness;
		Matrix3X3.Invert(ref effectiveMassMatrix, out effectiveMassMatrix);
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
			connectionA.ApplyAngularImpulse(ref accumulatedImpulse);
		}
		if (connectionB.isDynamic)
		{
			Vector3.Negate(ref accumulatedImpulse, out var result);
			connectionB.ApplyAngularImpulse(ref result);
		}
	}
}
