using System;
using BEPUphysics.Entities;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.SingleEntity;

/// <summary>
/// Prevents the target entity from moving faster than the specified speeds.
/// </summary>
public class MaximumLinearSpeedConstraint : SingleEntityConstraint, I3DImpulseConstraint
{
	private float effectiveMassMatrix;

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
	/// Gets or sets the maximum linear speed that this constraint allows.
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
	/// Gets the current relative velocity with respect to the constraint.
	/// For a single entity constraint, this is pretty straightforward as the
	/// velocity of the entity.
	/// </summary>
	Vector3 I3DImpulseConstraint.RelativeVelocity => Entity.LinearVelocity;

	/// <summary>
	/// Gets the total impulse applied by the constraint.
	/// </summary>
	public Vector3 TotalImpulse => accumulatedImpulse;

	/// <summary>
	/// Constructs a maximum speed constraint.
	/// Set its Entity and MaximumSpeed to complete the configuration.
	/// IsActive also starts as false with this constructor.
	/// </summary>
	public MaximumLinearSpeedConstraint()
	{
		base.IsActive = false;
	}

	/// <summary>
	/// Constructs a maximum speed constraint.
	/// </summary>
	/// <param name="e">Affected entity.</param>
	/// <param name="maxSpeed">Maximum linear speed allowed.</param>
	public MaximumLinearSpeedConstraint(Entity e, float maxSpeed)
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
		float num = entity.linearVelocity.LengthSquared();
		if (num > maximumSpeedSquared)
		{
			num = (float)Math.Sqrt(num);
			Vector3.Multiply(ref entity.linearVelocity, (0f - (num - maximumSpeed)) / num, out var result);
			Vector3.Multiply(ref accumulatedImpulse, usedSoftness, out var result2);
			Vector3.Subtract(ref result, ref result2, out result);
			Vector3.Multiply(ref result, effectiveMassMatrix, out result);
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
			entity.ApplyLinearImpulse(ref result);
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
		effectiveMassMatrix = 1f / (entity.inverseMass + usedSoftness);
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

	/// <summary>
	/// Performs any pre-solve iteration work that needs exclusive
	/// access to the members of the solver updateable.
	/// Usually, this is used for applying warmstarting impulses.
	/// </summary>
	public override void ExclusiveUpdate()
	{
		accumulatedImpulse = Toolbox.ZeroVector;
	}
}
