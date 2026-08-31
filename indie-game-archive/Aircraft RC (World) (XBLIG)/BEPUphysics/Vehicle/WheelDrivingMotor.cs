using BEPUphysics.Constraints;
using BEPUphysics.Entities;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Vehicle;

/// <summary>
/// Handles a wheel's driving force for a vehicle.
/// </summary>
public class WheelDrivingMotor : ISolverSettings
{
	/// <summary>
	/// Default blender used by WheelSlidingFriction constraints.
	/// </summary>
	public static WheelFrictionBlender DefaultGripFrictionBlender;

	internal float accumulatedImpulse;

	internal float angularAX;

	internal float angularAY;

	internal float angularAZ;

	internal float angularBX;

	internal float angularBY;

	internal float angularBZ;

	internal bool isActive = true;

	internal float linearAX;

	internal float linearAY;

	internal float linearAZ;

	private float currentFrictionCoefficient;

	internal Vector3 forceAxis;

	private float gripFriction;

	private WheelFrictionBlender gripFrictionBlender = DefaultGripFrictionBlender;

	private float maxMotorForceDt;

	private float maximumBackwardForce = float.MaxValue;

	private float maximumForwardForce = float.MaxValue;

	internal SolverSettings solverSettings = new SolverSettings();

	private float targetSpeed;

	private Wheel wheel;

	internal int numIterationsAtZeroImpulse;

	private Entity vehicleEntity;

	private Entity supportEntity;

	internal float velocityToImpulse;

	private bool supportIsDynamic;

	/// <summary>
	/// Gets the coefficient of grip friction between the wheel and support.
	/// This coefficient is the blended result of the supporting entity's friction and the wheel's friction.
	/// </summary>
	public float BlendedCoefficient => currentFrictionCoefficient;

	/// <summary>
	/// Gets the axis along which the driving forces are applied.
	/// </summary>
	public Vector3 ForceAxis => ForceAxis;

	/// <summary>
	/// Gets or sets the coefficient of forward-backward gripping friction for this wheel.
	/// This coefficient and the supporting entity's coefficient of friction will be 
	/// taken into account to determine the used coefficient at any given time.
	/// </summary>
	public float GripFriction
	{
		get
		{
			return gripFriction;
		}
		set
		{
			gripFriction = MathHelper.Max(value, 0f);
		}
	}

	/// <summary>
	/// Gets or sets the function used to blend the supporting entity's friction and the wheel's friction.
	/// </summary>
	public WheelFrictionBlender GripFrictionBlender
	{
		get
		{
			return gripFrictionBlender;
		}
		set
		{
			gripFrictionBlender = value;
		}
	}

	/// <summary>
	/// Gets or sets the maximum force that the wheel motor can apply when driving backward (a target speed less than zero).
	/// </summary>
	public float MaximumBackwardForce
	{
		get
		{
			return maximumBackwardForce;
		}
		set
		{
			maximumBackwardForce = value;
		}
	}

	/// <summary>
	/// Gets or sets the maximum force that the wheel motor can apply when driving forward (a target speed greater than zero).
	/// </summary>
	public float MaximumForwardForce
	{
		get
		{
			return maximumForwardForce;
		}
		set
		{
			maximumForwardForce = value;
		}
	}

	/// <summary>
	/// Gets or sets the target speed of this wheel.
	/// </summary>
	public float TargetSpeed
	{
		get
		{
			return targetSpeed;
		}
		set
		{
			targetSpeed = value;
		}
	}

	/// <summary>
	/// Gets the force this wheel's motor is applying.
	/// </summary>
	public float TotalImpulse => accumulatedImpulse;

	/// <summary>
	/// Gets the wheel that this motor applies to.
	/// </summary>
	public Wheel Wheel
	{
		get
		{
			return wheel;
		}
		internal set
		{
			wheel = value;
		}
	}

	/// <summary>
	/// Gets the solver settings used by this wheel constraint.
	/// </summary>
	public SolverSettings SolverSettings => solverSettings;

	/// <summary>
	/// Gets the relative velocity between the ground and wheel.
	/// </summary>
	/// <returns>Relative velocity between the ground and wheel.</returns>
	public float RelativeVelocity
	{
		get
		{
			float num = 0f;
			if (vehicleEntity != null)
			{
				num += vehicleEntity.linearVelocity.X * linearAX + vehicleEntity.linearVelocity.Y * linearAY + vehicleEntity.linearVelocity.Z * linearAZ + vehicleEntity.angularVelocity.X * angularAX + vehicleEntity.angularVelocity.Y * angularAY + vehicleEntity.angularVelocity.Z * angularAZ;
			}
			if (supportEntity != null)
			{
				num += (0f - supportEntity.linearVelocity.X) * linearAX - supportEntity.linearVelocity.Y * linearAY - supportEntity.linearVelocity.Z * linearAZ + supportEntity.angularVelocity.X * angularBX + supportEntity.angularVelocity.Y * angularBY + supportEntity.angularVelocity.Z * angularBZ;
			}
			return num;
		}
	}

	static WheelDrivingMotor()
	{
		DefaultGripFrictionBlender = BlendFriction;
	}

	/// <summary>
	/// Function which takes the friction values from a wheel and a supporting material and computes the blended friction.
	/// </summary>
	/// <param name="wheelFriction">Friction coefficient associated with the wheel.</param>
	/// <param name="materialFriction">Friction coefficient associated with the support material.</param>
	/// <param name="usingKineticFriction">True if the friction coefficients passed into the blender are kinetic coefficients, false otherwise.</param>
	/// <param name="wheel">Wheel being blended.</param>
	/// <returns>Blended friction coefficient.</returns>
	public static float BlendFriction(float wheelFriction, float materialFriction, bool usingKinematicFriction, Wheel wheel)
	{
		return wheelFriction * materialFriction;
	}

	/// <summary>
	/// Constructs a new wheel motor.
	/// </summary>
	/// <param name="gripFriction">Friction coefficient of the wheel.  Blended with the ground's friction coefficient and normal force to determine a maximum force.</param>
	/// <param name="maximumForwardForce">Maximum force that the wheel motor can apply when driving forward (a target speed greater than zero).</param>
	/// <param name="maximumBackwardForce">Maximum force that the wheel motor can apply when driving backward (a target speed less than zero).</param>
	public WheelDrivingMotor(float gripFriction, float maximumForwardForce, float maximumBackwardForce)
	{
		GripFriction = gripFriction;
		MaximumForwardForce = maximumForwardForce;
		MaximumBackwardForce = maximumBackwardForce;
	}

	internal WheelDrivingMotor(Wheel wheel)
	{
		Wheel = wheel;
	}

	internal float ApplyImpulse()
	{
		float num = (RelativeVelocity - targetSpeed) * velocityToImpulse;
		float num2 = accumulatedImpulse;
		accumulatedImpulse += num;
		if (targetSpeed > 0f)
		{
			accumulatedImpulse = MathHelper.Clamp(accumulatedImpulse, 0f, maxMotorForceDt);
		}
		else if (targetSpeed < 0f)
		{
			accumulatedImpulse = MathHelper.Clamp(accumulatedImpulse, maxMotorForceDt, 0f);
		}
		else
		{
			accumulatedImpulse = 0f;
		}
		float num3 = currentFrictionCoefficient * wheel.suspension.accumulatedImpulse;
		accumulatedImpulse = MathHelper.Clamp(accumulatedImpulse, num3, 0f - num3);
		num = accumulatedImpulse - num2;
		Vector3 impulse = default(Vector3);
		Vector3 impulse2 = default(Vector3);
		impulse.X = num * linearAX;
		impulse.Y = num * linearAY;
		impulse.Z = num * linearAZ;
		if (vehicleEntity.isDynamic)
		{
			impulse2.X = num * angularAX;
			impulse2.Y = num * angularAY;
			impulse2.Z = num * angularAZ;
			vehicleEntity.ApplyLinearImpulse(ref impulse);
			vehicleEntity.ApplyAngularImpulse(ref impulse2);
		}
		if (supportIsDynamic)
		{
			impulse.X = 0f - impulse.X;
			impulse.Y = 0f - impulse.Y;
			impulse.Z = 0f - impulse.Z;
			impulse2.X = num * angularBX;
			impulse2.Y = num * angularBY;
			impulse2.Z = num * angularBZ;
			supportEntity.ApplyLinearImpulse(ref impulse);
			supportEntity.ApplyAngularImpulse(ref impulse2);
		}
		return num;
	}

	internal void PreStep(float dt)
	{
		vehicleEntity = wheel.Vehicle.Body;
		supportEntity = wheel.SupportingEntity;
		supportIsDynamic = supportEntity != null && supportEntity.isDynamic;
		Vector3.Cross(ref wheel.normal, ref wheel.slidingFriction.slidingFrictionAxis, out forceAxis);
		forceAxis.Normalize();
		linearAX = forceAxis.X;
		linearAY = forceAxis.Y;
		linearAZ = forceAxis.Z;
		angularAX = wheel.ra.Y * linearAZ - wheel.ra.Z * linearAY;
		angularAY = wheel.ra.Z * linearAX - wheel.ra.X * linearAZ;
		angularAZ = wheel.ra.X * linearAY - wheel.ra.Y * linearAX;
		angularBX = linearAY * wheel.rb.Z - linearAZ * wheel.rb.Y;
		angularBY = linearAZ * wheel.rb.X - linearAX * wheel.rb.Z;
		angularBZ = linearAX * wheel.rb.Y - linearAY * wheel.rb.X;
		float num4;
		if (vehicleEntity.isDynamic)
		{
			float num = angularAX * vehicleEntity.inertiaTensorInverse.M11 + angularAY * vehicleEntity.inertiaTensorInverse.M21 + angularAZ * vehicleEntity.inertiaTensorInverse.M31;
			float num2 = angularAX * vehicleEntity.inertiaTensorInverse.M12 + angularAY * vehicleEntity.inertiaTensorInverse.M22 + angularAZ * vehicleEntity.inertiaTensorInverse.M32;
			float num3 = angularAX * vehicleEntity.inertiaTensorInverse.M13 + angularAY * vehicleEntity.inertiaTensorInverse.M23 + angularAZ * vehicleEntity.inertiaTensorInverse.M33;
			num4 = num * angularAX + num2 * angularAY + num3 * angularAZ + vehicleEntity.inverseMass;
		}
		else
		{
			num4 = 0f;
		}
		float num5;
		if (supportIsDynamic)
		{
			float num = angularBX * supportEntity.inertiaTensorInverse.M11 + angularBY * supportEntity.inertiaTensorInverse.M21 + angularBZ * supportEntity.inertiaTensorInverse.M31;
			float num2 = angularBX * supportEntity.inertiaTensorInverse.M12 + angularBY * supportEntity.inertiaTensorInverse.M22 + angularBZ * supportEntity.inertiaTensorInverse.M32;
			float num3 = angularBX * supportEntity.inertiaTensorInverse.M13 + angularBY * supportEntity.inertiaTensorInverse.M23 + angularBZ * supportEntity.inertiaTensorInverse.M33;
			num5 = num * angularBX + num2 * angularBY + num3 * angularBZ + supportEntity.inverseMass;
		}
		else
		{
			num5 = 0f;
		}
		velocityToImpulse = -1f / (num4 + num5);
		currentFrictionCoefficient = gripFrictionBlender(gripFriction, wheel.supportMaterial.kineticFriction, usingKineticFriction: true, wheel);
		if (targetSpeed > 0f)
		{
			maxMotorForceDt = maximumForwardForce * dt;
		}
		else
		{
			maxMotorForceDt = (0f - maximumBackwardForce) * dt;
		}
	}

	internal void ExclusiveUpdate()
	{
		Vector3 impulse = default(Vector3);
		Vector3 impulse2 = default(Vector3);
		impulse.X = accumulatedImpulse * linearAX;
		impulse.Y = accumulatedImpulse * linearAY;
		impulse.Z = accumulatedImpulse * linearAZ;
		if (vehicleEntity.isDynamic)
		{
			impulse2.X = accumulatedImpulse * angularAX;
			impulse2.Y = accumulatedImpulse * angularAY;
			impulse2.Z = accumulatedImpulse * angularAZ;
			vehicleEntity.ApplyLinearImpulse(ref impulse);
			vehicleEntity.ApplyAngularImpulse(ref impulse2);
		}
		if (supportIsDynamic)
		{
			impulse.X = 0f - impulse.X;
			impulse.Y = 0f - impulse.Y;
			impulse.Z = 0f - impulse.Z;
			impulse2.X = accumulatedImpulse * angularBX;
			impulse2.Y = accumulatedImpulse * angularBY;
			impulse2.Z = accumulatedImpulse * angularBZ;
			supportEntity.ApplyLinearImpulse(ref impulse);
			supportEntity.ApplyAngularImpulse(ref impulse2);
		}
	}
}
