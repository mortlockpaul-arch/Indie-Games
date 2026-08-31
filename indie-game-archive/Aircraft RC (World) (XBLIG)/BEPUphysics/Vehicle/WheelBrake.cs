using System;
using BEPUphysics.Constraints;
using BEPUphysics.Entities;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Vehicle;

/// <summary>
/// Attempts to resist rolling motion of a vehicle.
/// </summary>
public class WheelBrake : ISolverSettings
{
	/// <summary>
	/// Default blender used by WheelRollingFriction constraints.
	/// </summary>
	public static WheelFrictionBlender DefaultRollingFrictionBlender;

	internal float accumulatedImpulse;

	private float angularAX;

	private float angularAY;

	private float angularAZ;

	private float angularBX;

	private float angularBY;

	private float angularBZ;

	internal bool isActive = true;

	private float linearAX;

	private float linearAY;

	private float linearAZ;

	private float blendedCoefficient;

	private float kineticBrakingFrictionCoefficient;

	private WheelFrictionBlender frictionBlender = DefaultRollingFrictionBlender;

	private bool isBraking;

	private float rollingFrictionCoefficient;

	internal SolverSettings solverSettings = new SolverSettings();

	private float staticBrakingFrictionCoefficient;

	private float staticFrictionVelocityThreshold = 5f;

	private Wheel wheel;

	internal int numIterationsAtZeroImpulse;

	private Entity vehicleEntity;

	private Entity supportEntity;

	private float velocityToImpulse;

	private bool supportIsDynamic;

	/// <summary>
	/// Gets the coefficient of rolling friction between the wheel and support.
	/// This coefficient is the blended result of the supporting entity's friction and the wheel's friction.
	/// </summary>
	public float BlendedCoefficient => blendedCoefficient;

	/// <summary>
	/// Gets or sets the coefficient of braking dynamic friction for this wheel.
	/// This coefficient and the supporting entity's coefficient of friction will be 
	/// taken into account to determine the used coefficient at any given time.
	/// This coefficient is used instead of the rollingFrictionCoefficient when 
	/// isBraking is true.
	/// </summary>
	public float KineticBrakingFrictionCoefficient
	{
		get
		{
			return kineticBrakingFrictionCoefficient;
		}
		set
		{
			kineticBrakingFrictionCoefficient = MathHelper.Max(value, 0f);
		}
	}

	/// <summary>
	/// Gets the axis along which rolling friction is applied.
	/// </summary>
	public Vector3 FrictionAxis => wheel.drivingMotor.ForceAxis;

	/// <summary>
	/// Gets or sets the function used to blend the supporting entity's friction and the wheel's friction.
	/// </summary>
	public WheelFrictionBlender FrictionBlender
	{
		get
		{
			return frictionBlender;
		}
		set
		{
			frictionBlender = value;
		}
	}

	/// <summary>
	/// Gets or sets whether or not the wheel is braking.
	/// When set to true, the brakingFrictionCoefficient is used.
	/// When false, the rollingFrictionCoefficient is used.
	/// </summary>
	public bool IsBraking
	{
		get
		{
			return isBraking;
		}
		set
		{
			isBraking = value;
		}
	}

	/// <summary>
	/// Gets or sets the coefficient of rolling friction for this wheel.
	/// This coefficient and the supporting entity's coefficient of friction will be 
	/// taken into account to determine the used coefficient at any given time.
	/// This coefficient is used instead of the brakingFrictionCoefficient when 
	/// isBraking is false.
	/// </summary>
	public float RollingFrictionCoefficient
	{
		get
		{
			return rollingFrictionCoefficient;
		}
		set
		{
			rollingFrictionCoefficient = MathHelper.Max(value, 0f);
		}
	}

	/// <summary>
	/// Gets or sets the coefficient of static dynamic friction for this wheel.
	/// This coefficient and the supporting entity's coefficient of friction will be 
	/// taken into account to determine the used coefficient at any given time.
	/// This coefficient is used instead of the rollingFrictionCoefficient when 
	/// isBraking is true.
	/// </summary>
	public float StaticBrakingFrictionCoefficient
	{
		get
		{
			return staticBrakingFrictionCoefficient;
		}
		set
		{
			staticBrakingFrictionCoefficient = MathHelper.Max(value, 0f);
		}
	}

	/// <summary>
	/// Gets or sets the velocity under which the coefficient of static friction will be used instead of the dynamic one.
	/// </summary>
	public float StaticFrictionVelocityThreshold
	{
		get
		{
			return staticFrictionVelocityThreshold;
		}
		set
		{
			staticFrictionVelocityThreshold = Math.Abs(value);
		}
	}

	/// <summary>
	/// Gets the force 
	/// </summary>
	public float TotalImpulse => accumulatedImpulse;

	/// <summary>
	/// Gets the wheel that this rolling friction applies to.
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
	///  Gets the relative velocity along the braking direction at the wheel contact.
	/// </summary>
	public float RelativeVelocity
	{
		get
		{
			float num = vehicleEntity.linearVelocity.X * linearAX + vehicleEntity.linearVelocity.Y * linearAY + vehicleEntity.linearVelocity.Z * linearAZ + vehicleEntity.angularVelocity.X * angularAX + vehicleEntity.angularVelocity.Y * angularAY + vehicleEntity.angularVelocity.Z * angularAZ;
			if (supportEntity != null)
			{
				num += (0f - supportEntity.linearVelocity.X) * linearAX - supportEntity.linearVelocity.Y * linearAY - supportEntity.linearVelocity.Z * linearAZ + supportEntity.angularVelocity.X * angularBX + supportEntity.angularVelocity.Y * angularBY + supportEntity.angularVelocity.Z * angularBZ;
			}
			return num;
		}
	}

	static WheelBrake()
	{
		DefaultRollingFrictionBlender = BlendFriction;
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
	/// Constructs a new rolling friction object for a wheel.
	/// </summary>
	/// <param name="dynamicBrakingFrictionCoefficient">Coefficient of dynamic friction of the wheel for friction when the brake is active.</param>
	/// <param name="staticBrakingFrictionCoefficient">Coefficient of static friction of the wheel for friction when the brake is active.</param>
	/// <param name="rollingFrictionCoefficient">Coefficient of friction of the wheel for rolling friction when the brake isn't active.</param>
	public WheelBrake(float dynamicBrakingFrictionCoefficient, float staticBrakingFrictionCoefficient, float rollingFrictionCoefficient)
	{
		KineticBrakingFrictionCoefficient = dynamicBrakingFrictionCoefficient;
		StaticBrakingFrictionCoefficient = staticBrakingFrictionCoefficient;
		RollingFrictionCoefficient = rollingFrictionCoefficient;
	}

	internal WheelBrake(Wheel wheel)
	{
		Wheel = wheel;
	}

	internal float ApplyImpulse()
	{
		float num = RelativeVelocity * velocityToImpulse;
		float num2 = accumulatedImpulse;
		float num3 = (0f - blendedCoefficient) * wheel.suspension.accumulatedImpulse;
		accumulatedImpulse = MathHelper.Clamp(accumulatedImpulse + num, 0f - num3, num3);
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
		linearAX = wheel.drivingMotor.linearAX;
		linearAY = wheel.drivingMotor.linearAY;
		linearAZ = wheel.drivingMotor.linearAZ;
		angularAX = wheel.drivingMotor.angularAX;
		angularAY = wheel.drivingMotor.angularAY;
		angularAZ = wheel.drivingMotor.angularAZ;
		angularBX = wheel.drivingMotor.angularBX;
		angularBY = wheel.drivingMotor.angularBY;
		angularBZ = wheel.drivingMotor.angularBZ;
		velocityToImpulse = wheel.drivingMotor.velocityToImpulse;
		if (isBraking)
		{
			if (Math.Abs(RelativeVelocity) < staticFrictionVelocityThreshold)
			{
				blendedCoefficient = frictionBlender(staticBrakingFrictionCoefficient, wheel.supportMaterial.staticFriction, usingKineticFriction: false, wheel);
			}
			else
			{
				blendedCoefficient = frictionBlender(kineticBrakingFrictionCoefficient, wheel.supportMaterial.kineticFriction, usingKineticFriction: true, wheel);
			}
		}
		else
		{
			blendedCoefficient = rollingFrictionCoefficient;
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
