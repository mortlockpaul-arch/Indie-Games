using BEPUphysics.Constraints;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Vehicle;

/// <summary>
/// Allows the connected wheel and vehicle to smoothly absorb bumps.
/// </summary>
public class WheelSuspension : ISpringSettings, ISolverSettings
{
	private readonly SpringSettings springSettings = new SpringSettings();

	internal float accumulatedImpulse;

	private float angularAX;

	private float angularAY;

	private float angularAZ;

	private float angularBX;

	private float angularBY;

	private float angularBZ;

	private float bias;

	internal bool isActive = true;

	private float linearAX;

	private float linearAY;

	private float linearAZ;

	private float allowedCompression = 0.01f;

	internal float currentLength;

	internal Vector3 localAttachmentPoint;

	internal Vector3 localDirection;

	private float maximumSpringCorrectionSpeed = float.MaxValue;

	private float maximumSpringForce = float.MaxValue;

	internal float restLength;

	internal SolverSettings solverSettings = new SolverSettings();

	private Wheel wheel;

	internal Vector3 worldAttachmentPoint;

	internal Vector3 worldDirection;

	internal int numIterationsAtZeroImpulse;

	private Entity vehicleEntity;

	private Entity supportEntity;

	private float softness;

	private float velocityToImpulse;

	private bool supportIsDynamic;

	/// <summary>
	/// Gets or sets the allowed compression of the suspension before suspension forces take effect.
	/// Usually a very small number.  Used to prevent 'jitter' where the wheel leaves the ground due to spring forces repeatedly.
	/// </summary>
	public float AllowedCompression
	{
		get
		{
			return allowedCompression;
		}
		set
		{
			allowedCompression = MathHelper.Max(0f, value);
		}
	}

	/// <summary>
	/// Gets the the current length of the suspension.
	/// This will be less than the restLength if the suspension is compressed.
	/// </summary>
	public float CurrentLength => currentLength;

	/// <summary>
	/// Gets or sets the attachment point of the suspension to the vehicle body in the body's local space.
	/// </summary>
	public Vector3 LocalAttachmentPoint
	{
		get
		{
			return localAttachmentPoint;
		}
		set
		{
			localAttachmentPoint = value;
			if (wheel != null && wheel.vehicle != null)
			{
				RigidTransform.Transform(ref localAttachmentPoint, ref wheel.vehicle.Body.CollisionInformation.worldTransform, out worldAttachmentPoint);
			}
			else
			{
				worldAttachmentPoint = localAttachmentPoint;
			}
		}
	}

	/// <summary>
	/// Gets or sets the direction of the wheel suspension in the local space of the vehicle body.
	/// A normal, straight suspension would be (0,-1,0).
	/// </summary>
	public Vector3 LocalDirection
	{
		get
		{
			return localDirection;
		}
		set
		{
			localDirection = Vector3.Normalize(value);
			if (wheel != null && wheel.vehicle != null)
			{
				Matrix3X3.Transform(ref localDirection, ref wheel.vehicle.Body.orientationMatrix, out worldDirection);
			}
			else
			{
				worldDirection = localDirection;
			}
		}
	}

	/// <summary>
	/// Gets or sets the maximum speed at which the suspension will try to return the suspension to rest length.
	/// </summary>
	public float MaximumSpringCorrectionSpeed
	{
		get
		{
			return maximumSpringCorrectionSpeed;
		}
		set
		{
			maximumSpringCorrectionSpeed = MathHelper.Max(0f, value);
		}
	}

	/// <summary>
	/// Gets or sets the maximum force that can be applied by this suspension.
	/// </summary>
	public float MaximumSpringForce
	{
		get
		{
			return maximumSpringForce;
		}
		set
		{
			maximumSpringForce = MathHelper.Max(0f, value);
		}
	}

	/// <summary>
	/// Gets or sets the length of the uncompressed suspension.
	/// </summary>
	public float RestLength
	{
		get
		{
			return restLength;
		}
		set
		{
			restLength = value;
		}
	}

	/// <summary>
	/// Gets the force that the suspension is applying to support the vehicle.
	/// </summary>
	public float TotalImpulse => 0f - accumulatedImpulse;

	/// <summary>
	/// Gets the wheel that this suspension applies to.
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
	/// Gets or sets the attachment point of the suspension to the vehicle body in world space.
	/// </summary>
	public Vector3 WorldAttachmentPoint
	{
		get
		{
			return worldAttachmentPoint;
		}
		set
		{
			worldAttachmentPoint = value;
			if (wheel != null && wheel.vehicle != null)
			{
				RigidTransform.TransformByInverse(ref worldAttachmentPoint, ref wheel.vehicle.Body.CollisionInformation.worldTransform, out localAttachmentPoint);
			}
			else
			{
				localAttachmentPoint = worldAttachmentPoint;
			}
		}
	}

	/// <summary>
	/// Gets or sets the direction of the wheel suspension in the world space of the vehicle body.
	/// </summary>
	public Vector3 WorldDirection
	{
		get
		{
			return worldDirection;
		}
		set
		{
			worldDirection = Vector3.Normalize(value);
			if (wheel != null && wheel.vehicle != null)
			{
				Matrix3X3.TransformTranspose(ref worldDirection, ref wheel.Vehicle.Body.orientationMatrix, out localDirection);
			}
			else
			{
				localDirection = worldDirection;
			}
		}
	}

	/// <summary>
	/// Gets the solver settings used by this wheel constraint.
	/// </summary>
	public SolverSettings SolverSettings => solverSettings;

	/// <summary>
	/// Gets the spring settings that define the behavior of the suspension.
	/// </summary>
	public SpringSettings SpringSettings => springSettings;

	/// <summary>
	///  Gets the relative velocity along the support normal at the contact point.
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

	/// <summary>
	/// Constructs a new suspension for a wheel.
	/// </summary>
	/// <param name="stiffnessConstant">Strength of the spring.  Higher values resist compression more.</param>
	/// <param name="dampingConstant">Damping constant of the spring.  Higher values remove more momentum.</param>
	/// <param name="localDirection">Direction of the suspension in the vehicle's local space.  For a normal, straight down suspension, this would be (0, -1, 0).</param>
	/// <param name="restLength">Length of the suspension when uncompressed.</param>
	/// <param name="localAttachmentPoint">Place where the suspension hooks up to the body of the vehicle.</param>
	public WheelSuspension(float stiffnessConstant, float dampingConstant, Vector3 localDirection, float restLength, Vector3 localAttachmentPoint)
	{
		SpringSettings.StiffnessConstant = stiffnessConstant;
		SpringSettings.DampingConstant = dampingConstant;
		LocalDirection = localDirection;
		RestLength = restLength;
		LocalAttachmentPoint = localAttachmentPoint;
	}

	internal WheelSuspension(Wheel wheel)
	{
		Wheel = wheel;
	}

	internal float ApplyImpulse()
	{
		float num = (RelativeVelocity + bias + softness * accumulatedImpulse) * velocityToImpulse;
		float num2 = accumulatedImpulse;
		accumulatedImpulse = MathHelper.Clamp(accumulatedImpulse + num, 0f - maximumSpringForce, 0f);
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

	internal void ComputeWorldSpaceData()
	{
		RigidTransform.Transform(ref localAttachmentPoint, ref wheel.vehicle.Body.CollisionInformation.worldTransform, out worldAttachmentPoint);
		Matrix3X3.Transform(ref localDirection, ref wheel.vehicle.Body.orientationMatrix, out worldDirection);
	}

	internal void OnAdditionToVehicle()
	{
		LocalDirection = LocalDirection;
		LocalAttachmentPoint = LocalAttachmentPoint;
	}

	internal void PreStep(float dt)
	{
		vehicleEntity = wheel.vehicle.Body;
		supportEntity = wheel.supportingEntity;
		supportIsDynamic = supportEntity != null && supportEntity.isDynamic;
		linearAX = 0f - wheel.normal.X;
		linearAY = 0f - wheel.normal.Y;
		linearAZ = 0f - wheel.normal.Z;
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
		springSettings.ComputeErrorReductionAndSoftness(dt, out var errorReduction, out softness);
		velocityToImpulse = -1f / (num4 + num5 + softness);
		bias = MathHelper.Min(MathHelper.Max(0f, restLength - currentLength - allowedCompression) * errorReduction, maximumSpringCorrectionSpeed);
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
