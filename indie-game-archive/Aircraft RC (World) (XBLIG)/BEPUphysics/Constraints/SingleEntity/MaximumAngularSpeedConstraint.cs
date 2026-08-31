using System;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.SingleEntity;

/// <summary>
/// Prevents the target entity from moving faster than the specified speeds.
/// </summary>
public class MaximumAngularSpeedConstraint : SingleEntityConstraint, I3DImpulseConstraint
{
	private Matrix3X3 effectiveMassMatrix;

	private float maxForceDt = float.MaxValue;

	private float maxForceDtSquared = float.MaxValue;

	private Vector3 accumulatedImpulse;

	private float maximumForce = float.MaxValue;

	private float maximumSpeed;

	private float maximumSpeedSquared;

	private float softness = 1E-05f;

	private float usedSoftness;

	/// <summary>
	/// Gets and sets the maximum impulse that the constraint will attempt to apply when satisfying its requirements.
	/// This field can be used to simulate friction in a constraint.
	/// </summary>
	public float MaximumForce
	{
		get
		{
			if (maximumForce > 0f)
			{
				return maximumForce;
			}
			return 0f;
		}
		set
		{
			maximumForce = ((value >= 0f) ? value : 0f);
		}
	}

	/// <summary>
	/// Gets or sets the maximum angular speed that this constraint allows.
	/// </summary>
	public float MaximumSpeed
	{
		get
		{
			return maximumSpeed;
		}
		set
		{
			maximumSpeed = MathHelper.Max(0f, value);
			maximumSpeedSquared = maximumSpeed * maximumSpeed;
		}
	}

	/// <summary>
	/// Gets and sets the softness of this constraint.
	/// Higher values of softness allow the constraint to be violated more.
	/// Must be greater than zero.
	/// Sometimes, if a joint system is unstable, increasing the softness of the involved constraints will make it settle down.
	/// For motors, softness can be used to implement damping.  For a damping constant k, the appropriate softness is 1/k.
	/// </summary>
	public float Softness
	{
		get
		{
			return softness;
		}
		set
		{
			softness = Math.Max(0f, value);
		}
	}

	/// <summary>
	/// Gets the current relative velocity between the connected entities with respect to the constraint.
	/// </summary>
	Vector3 I3DImpulseConstraint.RelativeVelocity => entity.angularVelocity;

	/// <summary>
	/// Gets the total impulse applied by the constraint.
	/// </summary>
	public Vector3 TotalImpulse => accumulatedImpulse;

	/// <summary>
	/// Constructs a maximum speed constraint.
	/// Set its Entity and MaximumSpeed to complete the configuration.
	/// IsActive also starts as false with this constructor.
	/// </summary>
	public MaximumAngularSpeedConstraint()
	{
		base.IsActive = false;
	}

	/// <summary>
	/// Constructs a maximum speed constraint.
	/// </summary>
	/// <param name="e">Affected entity.</param>
	/// <param name="maxSpeed">Maximum angular speed allowed.</param>
	public MaximumAngularSpeedConstraint(Entity e, float maxSpeed)
	{
		Entity = e;
		MaximumSpeed = maxSpeed;
	}

	/// <summary>
	/// Calculates and applies corrective impulses.
	/// Called automatically by space.
	/// </summary>
	public override float SolveIteration()
	{
		float num = entity.angularVelocity.LengthSquared();
		if (num > maximumSpeedSquared)
		{
			num = (float)Math.Sqrt(num);
			Vector3.Multiply(ref entity.angularVelocity, (0f - (num - maximumSpeed)) / num, out var result);
			Vector3.Multiply(ref accumulatedImpulse, usedSoftness, out var result2);
			Vector3.Subtract(ref result, ref result2, out result);
			Matrix3X3.Transform(ref result, ref effectiveMassMatrix, out result);
			Vector3 vector = accumulatedImpulse;
			Vector3.Add(ref accumulatedImpulse, ref result, out accumulatedImpulse);
			float num2 = accumulatedImpulse.LengthSquared();
			if (num2 > maxForceDtSquared)
			{
				float num3 = maxForceDt / (float)Math.Sqrt(num2);
				accumulatedImpulse.X *= num3;
				accumulatedImpulse.Y *= num3;
				accumulatedImpulse.Z *= num3;
				result.X = accumulatedImpulse.X - vector.X;
				result.Y = accumulatedImpulse.Y - vector.Y;
				result.Z = accumulatedImpulse.Z - vector.Z;
			}
			entity.ApplyAngularImpulse(ref result);
			return Math.Abs(result.X) + Math.Abs(result.Y) + Math.Abs(result.Z);
		}
		return 0f;
	}

	/// <summary>
	/// Calculates necessary information for velocity solving.
	/// Called automatically by space.
	/// </summary>
	/// <param name="dt">Time in seconds since the last update.</param>
	public override void Update(float dt)
	{
		usedSoftness = softness / dt;
		effectiveMassMatrix = entity.inertiaTensorInverse;
		effectiveMassMatrix.M11 += usedSoftness;
		effectiveMassMatrix.M22 += usedSoftness;
		effectiveMassMatrix.M33 += usedSoftness;
		Matrix3X3.Invert(ref effectiveMassMatrix, out effectiveMassMatrix);
		if (maximumForce < float.MaxValue)
		{
			maxForceDt = maximumForce * dt;
			maxForceDtSquared = maxForceDt * maxForceDt;
		}
		else
		{
			maxForceDt = float.MaxValue;
			maxForceDtSquared = float.MaxValue;
		}
	}

	public override void ExclusiveUpdate()
	{
		accumulatedImpulse = Toolbox.ZeroVector;
	}
}
