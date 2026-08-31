using System;
using BEPUphysics.Constraints;
using BEPUphysics.Entities;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Vehicle;

/// <summary>
/// Attempts to resist sliding motion of a vehicle.
/// </summary>
public class WheelSlidingFriction : ISolverSettings
{
	/// <summary>
	/// Default blender used by WheelSlidingFriction constraints.
	/// </summary>
	public static WheelFrictionBlender DefaultSlidingFrictionBlender;

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

	private float kineticCoefficient;

	private WheelFrictionBlender frictionBlender = DefaultSlidingFrictionBlender;

	internal Vector3 slidingFrictionAxis;

	internal SolverSettings solverSettings = new SolverSettings();

	private float staticCoefficient;

	private float staticFrictionVelocityThreshold = 5f;

	private Wheel wheel;

	internal int numIterationsAtZeroImpulse;

	private Entity vehicleEntity;

	private Entity supportEntity;

	private float velocityToImpulse;

	private bool supportIsDynamic;

	/// <summary>
	/// Gets the coefficient of sliding friction between the wheel and support.
	/// This coefficient is the blended result of the supporting entity's friction and the wheel's friction.
	/// </summary>
	public float BlendedCoefficient => blendedCoefficient;

	/// <summary>
	/// Gets or sets the coefficient of dynamic horizontal sliding friction for this wheel.
	/// This coefficient and the supporting entity's coefficient of friction will be 
	/// taken into account to determine the used coefficient at any given time.
	/// </summary>
	public float KineticCoefficient
	{
		get
		{
			return kineticCoefficient;
		}
		set
		{
			kineticCoefficient = MathHelper.Max(value, 0f);
		}
	}

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
	/// Gets the axis along which sliding friction is applied.
	/// </summary>
	public Vector3 SlidingFrictionAxis => slidingFrictionAxis;

	/// <summary>
	/// Gets or sets the coefficient of static horizontal sliding friction for this wheel.
	/// This coefficient and the supporting entity's coefficient of friction will be 
	/// taken into account to determine the used coefficient at any given time.
	/// </summary>
	public float StaticCoefficient
	{
		get
		{
			return staticCoefficient;
		}
		set
		{
			staticCoefficient = MathHelper.Max(value, 0f);
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
	/// Gets the wheel that this sliding friction applies to.
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
	///  Gets the relative velocity along the sliding direction at the wheel contact.
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

	static WheelSlidingFriction()
	{
		DefaultSlidingFrictionBlender = BlendFriction;
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
	/// Constructs a new sliding friction object for a wheel.
	/// </summary>
	/// <param name="dynamicCoefficient">Coefficient of dynamic sliding friction to be blended with the supporting entity's friction.</param>
	/// <param name="staticCoefficient">Coefficient of static sliding friction to be blended with the supporting entity's friction.</param>
	public WheelSlidingFriction(float dynamicCoefficient, float staticCoefficient)
	{
		KineticCoefficient = dynamicCoefficient;
		StaticCoefficient = staticCoefficient;
	}

	internal WheelSlidingFriction(Wheel wheel)
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
		Vector3.Cross(ref wheel.worldForwardDirection, ref wheel.normal, out slidingFrictionAxis);
		float num = slidingFrictionAxis.LengthSquared();
		if (num < 1E-05f)
		{
			Vector3.Cross(ref wheel.worldForwardDirection, ref Toolbox.UpVector, out slidingFrictionAxis);
			num = slidingFrictionAxis.LengthSquared();
			if (num < 1E-05f)
			{
				Vector3.Cross(ref wheel.worldForwardDirection, ref Toolbox.RightVector, out slidingFrictionAxis);
			}
		}
		slidingFrictionAxis.Normalize();
		linearAX = slidingFrictionAxis.X;
		linearAY = slidingFrictionAxis.Y;
		linearAZ = slidingFrictionAxis.Z;
		angularAX = wheel.ra.Y * linearAZ - wheel.ra.Z * linearAY;
		angularAY = wheel.ra.Z * linearAX - wheel.ra.X * linearAZ;
		angularAZ = wheel.ra.X * linearAY - wheel.ra.Y * linearAX;
		angularBX = linearAY * wheel.rb.Z - linearAZ * wheel.rb.Y;
		angularBY = linearAZ * wheel.rb.X - linearAX * wheel.rb.Z;
		angularBZ = linearAX * wheel.rb.Y - linearAY * wheel.rb.X;
		float num5;
		if (vehicleEntity.isDynamic)
		{
			float num2 = angularAX * vehicleEntity.inertiaTensorInverse.M11 + angularAY * vehicleEntity.inertiaTensorInverse.M21 + angularAZ * vehicleEntity.inertiaTensorInverse.M31;
			float num3 = angularAX * vehicleEntity.inertiaTensorInverse.M12 + angularAY * vehicleEntity.inertiaTensorInverse.M22 + angularAZ * vehicleEntity.inertiaTensorInverse.M32;
			float num4 = angularAX * vehicleEntity.inertiaTensorInverse.M13 + angularAY * vehicleEntity.inertiaTensorInverse.M23 + angularAZ * vehicleEntity.inertiaTensorInverse.M33;
			num5 = num2 * angularAX + num3 * angularAY + num4 * angularAZ + vehicleEntity.inverseMass;
		}
		else
		{
			num5 = 0f;
		}
		float num6;
		if (supportIsDynamic)
		{
			float num2 = angularBX * supportEntity.inertiaTensorInverse.M11 + angularBY * supportEntity.inertiaTensorInverse.M21 + angularBZ * supportEntity.inertiaTensorInverse.M31;
			float num3 = angularBX * supportEntity.inertiaTensorInverse.M12 + angularBY * supportEntity.inertiaTensorInverse.M22 + angularBZ * supportEntity.inertiaTensorInverse.M32;
			float num4 = angularBX * supportEntity.inertiaTensorInverse.M13 + angularBY * supportEntity.inertiaTensorInverse.M23 + angularBZ * supportEntity.inertiaTensorInverse.M33;
			num6 = num2 * angularBX + num3 * angularBY + num4 * angularBZ + supportEntity.inverseMass;
		}
		else
		{
			num6 = 0f;
		}
		velocityToImpulse = -1f / (num5 + num6);
		if (Math.Abs(RelativeVelocity) < staticFrictionVelocityThreshold)
		{
			blendedCoefficient = frictionBlender(staticCoefficient, wheel.supportMaterial.staticFriction, usingKineticFriction: false, wheel);
		}
		else
		{
			blendedCoefficient = frictionBlender(kineticCoefficient, wheel.supportMaterial.kineticFriction, usingKineticFriction: true, wheel);
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
