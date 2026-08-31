using System;
using System.Threading;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseSystems;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.DeactivationManagement;
using BEPUphysics.EntityStateManagement;
using BEPUphysics.Materials;
using BEPUphysics.MathExtensions;
using BEPUphysics.OtherSpaceStages;
using BEPUphysics.PositionUpdating;
using BEPUphysics.Settings;
using BEPUphysics.Threading;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Entities;

/// <summary>
///  Superclass of movable rigid bodies.  Contains information for
///  both dynamic and kinematic simulation.
/// </summary>
public class Entity : IBroadPhaseEntryOwner, IDeferredEventCreatorOwner, ISimulationIslandMemberOwner, ICCDPositionUpdateable, IPositionUpdateable, IForceUpdateable, ISpaceObject, IMaterialOwner, ICollisionRulesOwner
{
	internal Vector3 position;

	internal Quaternion orientation = Quaternion.Identity;

	internal Matrix3X3 orientationMatrix = Matrix3X3.Identity;

	internal Vector3 linearVelocity;

	internal Vector3 linearMomentum;

	internal Vector3 angularVelocity;

	internal Vector3 angularMomentum;

	internal bool isDynamic;

	private bool isAffectedByGravity = true;

	internal Matrix3X3 inertiaTensorInverse;

	internal Matrix3X3 inertiaTensor;

	internal Matrix3X3 localInertiaTensor;

	internal Matrix3X3 localInertiaTensorInverse;

	internal float mass;

	internal float inverseMass;

	internal float volume;

	protected EntityCollidable collisionInformation;

	protected internal BEPUphysics.Threading.SpinLock locker = new BEPUphysics.Threading.SpinLock();

	internal Material material;

	private Action<Material> materialChangedDelegate;

	internal SimulationIslandMember activityInformation;

	private Action<CollisionShape> shapeChangedDelegate;

	private ForceUpdater forceUpdater;

	private ISpace space;

	private PositionUpdateMode positionUpdateMode = MotionSettings.DefaultPositionUpdateMode;

	private float linearDampingBoost;

	private float angularDampingBoost;

	private float angularDamping = 0.15f;

	private float linearDamping = 0.03f;

	private static long idCounter;

	private int hashCode;

	/// <summary>
	///  Gets or sets the position of the Entity.  This Position acts
	///  as the center of mass for dynamic entities.
	/// </summary>
	public Vector3 Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
			activityInformation.Activate();
		}
	}

	/// <summary>
	///  Gets or sets the orientation quaternion of the entity.
	/// </summary>
	public Quaternion Orientation
	{
		get
		{
			return orientation;
		}
		set
		{
			Quaternion.Normalize(ref value, out orientation);
			Matrix3X3.CreateFromQuaternion(ref orientation, out orientationMatrix);
			Matrix3X3.MultiplyTransposed(ref orientationMatrix, ref localInertiaTensorInverse, out var result);
			Matrix3X3.Multiply(ref result, ref orientationMatrix, out inertiaTensorInverse);
			Matrix3X3.MultiplyTransposed(ref orientationMatrix, ref localInertiaTensor, out result);
			Matrix3X3.Multiply(ref result, ref orientationMatrix, out inertiaTensor);
			activityInformation.Activate();
		}
	}

	/// <summary>
	/// Gets or sets the orientation matrix of the entity.
	/// </summary>
	public Matrix3X3 OrientationMatrix
	{
		get
		{
			return orientationMatrix;
		}
		set
		{
			Matrix3X3.CreateQuaternion(ref value, out orientation);
			Orientation = orientation;
		}
	}

	/// <summary>
	///  Gets or sets the world transform of the entity.
	///  The upper left 3x3 part is the Orientation, and the translation is the Position.
	///  When setting this property, ensure that the rotation matrix component does not include
	///  any scaling or shearing.
	/// </summary>
	public Matrix WorldTransform
	{
		get
		{
			Matrix3X3.ToMatrix4X4(ref orientationMatrix, out var b);
			b.Translation = position;
			return b;
		}
		set
		{
			Quaternion.CreateFromRotationMatrix(ref value, out orientation);
			Orientation = orientation;
			position = value.Translation;
			activityInformation.Activate();
		}
	}

	/// <summary>
	/// Gets or sets the angular velocity of the entity.
	/// </summary>
	public Vector3 AngularVelocity
	{
		get
		{
			return angularVelocity;
		}
		set
		{
			angularVelocity = value;
			Matrix3X3.Transform(ref value, ref inertiaTensor, out angularMomentum);
			activityInformation.Activate();
		}
	}

	/// <summary>
	/// Gets or sets the angular momentum of the entity.
	/// </summary>
	public Vector3 AngularMomentum
	{
		get
		{
			if (MotionSettings.ConserveAngularMomentum)
			{
				return angularMomentum;
			}
			Matrix3X3.Transform(ref angularVelocity, ref inertiaTensor, out var result);
			return result;
		}
		set
		{
			angularMomentum = value;
			Matrix3X3.Transform(ref value, ref inertiaTensorInverse, out angularVelocity);
			activityInformation.Activate();
		}
	}

	/// <summary>
	/// Gets or sets the linear velocity of the entity.
	/// </summary>
	public Vector3 LinearVelocity
	{
		get
		{
			return linearVelocity;
		}
		set
		{
			linearVelocity = value;
			Vector3.Multiply(ref linearVelocity, mass, out linearMomentum);
			activityInformation.Activate();
		}
	}

	/// <summary>
	/// Gets or sets the linear momentum of the entity.
	/// </summary>
	public Vector3 LinearMomentum
	{
		get
		{
			return linearMomentum;
		}
		set
		{
			linearMomentum = value;
			Vector3.Multiply(ref linearMomentum, inverseMass, out linearVelocity);
			activityInformation.Activate();
		}
	}

	/// <summary>
	/// Gets or sets the position, orientation, linear velocity, and angular velocity of the entity.
	/// </summary>
	public MotionState MotionState
	{
		get
		{
			MotionState result = default(MotionState);
			result.Position = position;
			result.Orientation = orientation;
			result.LinearVelocity = linearVelocity;
			result.AngularVelocity = angularVelocity;
			return result;
		}
		set
		{
			Position = value.Position;
			Orientation = value.Orientation;
			LinearVelocity = value.LinearVelocity;
			AngularVelocity = value.AngularVelocity;
		}
	}

	/// <summary>
	/// Gets whether or not the entity is dynamic.
	/// Dynamic entities have finite mass and respond
	/// to collisions.  Kinematic (non-dynamic) entities
	/// have infinite mass and inertia and will plow through anything.
	/// </summary>
	public bool IsDynamic => isDynamic;

	/// <summary>
	///  Gets or sets whether or not the entity can be affected by gravity applied by the ForceUpdater.
	/// </summary>
	public bool IsAffectedByGravity
	{
		get
		{
			return isAffectedByGravity;
		}
		set
		{
			isAffectedByGravity = value;
		}
	}

	/// <summary>
	///  Gets the buffered states of the entity.  If the Space.BufferedStates manager is enabled,
	///  this property provides access to the buffered and interpolated states of the entity.
	///  Buffered states are the most recent completed update values, while interpolated states are the previous values blended
	///  with the current frame's values.  Interpolated states are helpful when updating the engine with internal time stepping, 
	///  giving entity motion a smooth appearance even when updates aren't occurring consistently every frame.  
	///  Both are buffered for asynchronous access.
	/// </summary>
	public EntityBufferedStates BufferedStates { get; private set; }

	/// <summary>
	///  Gets the world space inertia tensor inverse of the entity.
	/// </summary>
	public Matrix3X3 InertiaTensorInverse => inertiaTensorInverse;

	/// <summary>
	///  Gets the world space inertia tensor of the entity.
	/// </summary>
	public Matrix3X3 InertiaTensor => inertiaTensor;

	/// <summary>
	///  Gets or sets the local inertia tensor of the entity.
	/// </summary>
	public Matrix3X3 LocalInertiaTensor
	{
		get
		{
			return localInertiaTensor;
		}
		set
		{
			localInertiaTensor = value;
			Matrix3X3.AdaptiveInvert(ref localInertiaTensor, out localInertiaTensorInverse);
			Matrix3X3.MultiplyTransposed(ref orientationMatrix, ref localInertiaTensorInverse, out var result);
			Matrix3X3.Multiply(ref result, ref orientationMatrix, out inertiaTensorInverse);
			Matrix3X3.MultiplyTransposed(ref orientationMatrix, ref localInertiaTensor, out result);
			Matrix3X3.Multiply(ref result, ref orientationMatrix, out inertiaTensor);
		}
	}

	/// <summary>
	/// Gets or sets the local inertia tensor inverse of the entity.
	/// </summary>
	public Matrix3X3 LocalInertiaTensorInverse
	{
		get
		{
			return localInertiaTensorInverse;
		}
		set
		{
			localInertiaTensorInverse = value;
			Matrix3X3.AdaptiveInvert(ref localInertiaTensorInverse, out localInertiaTensor);
			Matrix3X3.MultiplyTransposed(ref orientationMatrix, ref localInertiaTensorInverse, out var result);
			Matrix3X3.Multiply(ref result, ref orientationMatrix, out inertiaTensorInverse);
			Matrix3X3.MultiplyTransposed(ref orientationMatrix, ref localInertiaTensor, out result);
			Matrix3X3.Multiply(ref result, ref orientationMatrix, out inertiaTensor);
		}
	}

	/// <summary>
	///  Gets or sets the mass of the entity.  Setting this to an invalid value, such as a non-positive number, NaN, or infinity, makes the entity kinematic.
	///  Setting it to a valid positive number will also scale the inertia tensor if it was already dynamic, or force the calculation of a new inertia tensor
	///  if it was previously kinematic.
	/// </summary>
	public float Mass
	{
		get
		{
			return mass;
		}
		set
		{
			if (value <= 0f || float.IsNaN(value) || float.IsInfinity(value))
			{
				BecomeKinematic();
			}
			else if (isDynamic)
			{
				Matrix3X3.Multiply(ref localInertiaTensor, value * inverseMass, out var result);
				BecomeDynamic(value, result);
			}
			else
			{
				BecomeDynamic(value);
			}
		}
	}

	/// <summary>
	/// Gets or sets the inverse mass of the entity.
	/// </summary>
	public float InverseMass
	{
		get
		{
			return inverseMass;
		}
		set
		{
			if (value > 0f)
			{
				Mass = 1f / value;
			}
			else
			{
				Mass = 0f;
			}
		}
	}

	/// <summary>
	/// Gets or sets the volume of the entity.
	/// This is computed along with other physical properties at initialization,
	/// but it's only used for auxiliary systems like the FluidVolume.
	/// Changing this can tune behavior of those systems.
	/// </summary>
	public float Volume
	{
		get
		{
			return volume;
		}
		set
		{
			volume = value;
		}
	}

	/// <summary>
	///  Gets the collidable used by the entity.
	/// </summary>
	public EntityCollidable CollisionInformation
	{
		get
		{
			return collisionInformation;
		}
		protected set
		{
			if (collisionInformation != null)
			{
				collisionInformation.Shape.ShapeChanged -= shapeChangedDelegate;
			}
			collisionInformation = value;
			if (collisionInformation != null)
			{
				collisionInformation.Shape.ShapeChanged += shapeChangedDelegate;
			}
		}
	}

	/// <summary>
	///  Gets the synchronization object used by systems that need
	///  exclusive access to the entity's properties.
	/// </summary>
	public BEPUphysics.Threading.SpinLock Locker => locker;

	/// <summary>
	///  Gets or sets the material used by the entity.
	/// </summary>
	public Material Material
	{
		get
		{
			return material;
		}
		set
		{
			if (material != null)
			{
				material.MaterialChanged -= materialChangedDelegate;
			}
			material = value;
			if (material != null)
			{
				material.MaterialChanged += materialChangedDelegate;
			}
			OnMaterialChanged(material);
		}
	}

	/// <summary>
	///  Gets all the EntitySolverUpdateables associated with this entity.
	/// </summary>
	public EntitySolverUpdateableCollection SolverUpdateables => new EntitySolverUpdateableCollection(activityInformation.connections);

	/// <summary>
	///  Gets the two-entity constraints associated with this entity (a subset of the solver updateables).
	/// </summary>
	public EntityConstraintCollection Constraints => new EntityConstraintCollection(activityInformation.connections);

	IDeferredEventCreator IDeferredEventCreatorOwner.EventCreator => CollisionInformation.Events;

	public SimulationIslandMember ActivityInformation => activityInformation;

	bool IForceUpdateable.IsActive => activityInformation.IsActive;

	bool IPositionUpdateable.IsActive => activityInformation.IsActive;

	/// <summary>
	/// Gets or sets whether or not to ignore shape changes.  When true, changing the entity's collision shape will not update the volume, density, or inertia tensor. 
	/// </summary>
	public bool IgnoreShapeChanges { get; set; }

	ForceUpdater IForceUpdateable.ForceUpdater
	{
		get
		{
			return forceUpdater;
		}
		set
		{
			forceUpdater = value;
		}
	}

	ISpace ISpaceObject.Space
	{
		get
		{
			return space;
		}
		set
		{
			space = value;
		}
	}

	/// <summary>
	///  Gets the space that owns the entity.
	/// </summary>
	public ISpace Space => space;

	PositionUpdater IPositionUpdateable.PositionUpdater { get; set; }

	/// <summary>
	///  Gets the position update mode of the entity.
	/// </summary>
	public PositionUpdateMode PositionUpdateMode
	{
		get
		{
			return positionUpdateMode;
		}
		set
		{
			PositionUpdateMode positionUpdateMode = this.positionUpdateMode;
			this.positionUpdateMode = value;
			if (this.positionUpdateMode != positionUpdateMode && ((IPositionUpdateable)this).PositionUpdater != null && ((IPositionUpdateable)this).PositionUpdater is ContinuousPositionUpdater)
			{
				(((IPositionUpdateable)this).PositionUpdater as ContinuousPositionUpdater).UpdateableModeChanged(this, positionUpdateMode);
			}
		}
	}

	/// <summary>
	///  Gets or sets the angular damping of the entity.
	///  Values range from 0 to 1, corresponding to a fraction of angular momentum removed
	///  from the entity over a unit of time.
	/// </summary>
	public float AngularDamping
	{
		get
		{
			return angularDamping;
		}
		set
		{
			angularDamping = MathHelper.Clamp(value, 0f, 1f);
		}
	}

	/// <summary>
	///  Gets or sets the linear damping of the entity.
	///  Values range from 0 to 1, correspondong to a fraction of linear momentum removed
	///  from the entity over a unit of time.
	/// </summary>
	public float LinearDamping
	{
		get
		{
			return linearDamping;
		}
		set
		{
			linearDamping = MathHelper.Clamp(value, 0f, 1f);
		}
	}

	/// <summary>
	/// Gets or sets the user data associated with the entity.
	/// This is separate from the entity's collidable's tag.
	/// If a tag needs to be accessed from within the collision
	/// detection pipeline, consider using the entity.CollisionInformation.Tag.
	/// </summary>
	public object Tag { get; set; }

	CollisionRules ICollisionRulesOwner.CollisionRules
	{
		get
		{
			return collisionInformation.collisionRules;
		}
		set
		{
			collisionInformation.CollisionRules = value;
		}
	}

	BroadPhaseEntry IBroadPhaseEntryOwner.Entry => collisionInformation;

	/// <summary>
	/// Gets the entity's unique instance id.
	/// </summary>
	public long InstanceId { get; private set; }

	/// <summary>
	///  Fires when the entity's position and orientation is updated.
	/// </summary>
	public event Action<Entity> PositionUpdated;

	private void OnMaterialChanged(Material newMaterial)
	{
		for (int i = 0; i < collisionInformation.pairs.Count; i++)
		{
			collisionInformation.pairs[i].UpdateMaterialProperties();
		}
	}

	protected Entity()
	{
		InitializeId();
		BufferedStates = new EntityBufferedStates(this);
		material = new Material();
		materialChangedDelegate = OnMaterialChanged;
		material.MaterialChanged += materialChangedDelegate;
		shapeChangedDelegate = OnShapeChanged;
		activityInformation = new SimulationIslandMember(this);
	}

	/// <summary>
	///  Constructs a new kinematic entity.
	/// </summary>
	/// <param name="collisionInformation">Collidable to use with the entity.</param>
	public Entity(EntityCollidable collisionInformation)
		: this()
	{
		Initialize(collisionInformation);
	}

	/// <summary>
	///  Constructs a new dynamic entity.
	/// </summary>
	/// <param name="collisionInformation">Collidable to use with the entity.</param>
	/// <param name="mass">Mass of the entity.</param>
	public Entity(EntityCollidable collisionInformation, float mass)
		: this()
	{
		Initialize(collisionInformation, mass);
	}

	/// <summary>
	///  Constructs a new dynamic entity.
	/// </summary>
	/// <param name="collisionInformation">Collidable to use with the entity.</param>
	/// <param name="mass">Mass of the entity.</param>
	///  <param name="inertiaTensor">Inertia tensor of the entity.</param>
	public Entity(EntityCollidable collisionInformation, float mass, Matrix3X3 inertiaTensor)
		: this()
	{
		Initialize(collisionInformation, mass, inertiaTensor);
	}

	/// <summary>
	///  Constructs a new dynamic entity.
	/// </summary>
	/// <param name="collisionInformation">Collidable to use with the entity.</param>
	/// <param name="mass">Mass of the entity.</param>
	///  <param name="inertiaTensor">Inertia tensor of the entity.</param>
	///  <param name="volume">Volume of the entity.</param>
	public Entity(EntityCollidable collisionInformation, float mass, Matrix3X3 inertiaTensor, float volume)
		: this()
	{
		Initialize(collisionInformation, mass, inertiaTensor, volume);
	}

	/// <summary>
	///  Constructs a new kinematic entity.
	/// </summary>
	/// <param name="shape">Shape to use with the entity.</param>
	public Entity(EntityShape shape)
		: this()
	{
		Initialize(shape.GetCollidableInstance());
	}

	/// <summary>
	///  Constructs a new dynamic entity.
	/// </summary>
	/// <param name="shape">Shape to use with the entity.</param>
	/// <param name="mass">Mass of the entity.</param>
	public Entity(EntityShape shape, float mass)
		: this()
	{
		Initialize(shape.GetCollidableInstance(), mass);
	}

	/// <summary>
	///  Constructs a new dynamic entity.
	/// </summary>
	/// <param name="shape">Shape to use with the entity.</param>
	/// <param name="mass">Mass of the entity.</param>
	///  <param name="inertiaTensor">Inertia tensor of the entity.</param>
	public Entity(EntityShape shape, float mass, Matrix3X3 inertiaTensor)
		: this()
	{
		Initialize(shape.GetCollidableInstance(), mass, inertiaTensor);
	}

	/// <summary>
	///  Constructs a new dynamic entity.
	/// </summary>
	/// <param name="shape">Shape to use with the entity.</param>
	/// <param name="mass">Mass of the entity.</param>
	///  <param name="inertiaTensor">Inertia tensor of the entity.</param>
	///  <param name="volume">Volume of the entity.</param>
	public Entity(EntityShape shape, float mass, Matrix3X3 inertiaTensor, float volume)
		: this()
	{
		Initialize(shape.GetCollidableInstance(), mass, inertiaTensor, volume);
	}

	protected internal void Initialize(EntityCollidable collisionInformation)
	{
		CollisionInformation = collisionInformation;
		BecomeKinematic();
		collisionInformation.Entity = this;
	}

	protected internal void Initialize(EntityCollidable collisionInformation, float mass)
	{
		CollisionInformation = collisionInformation;
		collisionInformation.Shape.ComputeDistributionInformation(out var shapeInfo);
		Matrix3X3.Multiply(ref shapeInfo.VolumeDistribution, mass * InertiaHelper.InertiaTensorScale, out shapeInfo.VolumeDistribution);
		volume = shapeInfo.Volume;
		BecomeDynamic(mass, shapeInfo.VolumeDistribution);
		collisionInformation.Entity = this;
	}

	protected internal void Initialize(EntityCollidable collisionInformation, float mass, Matrix3X3 inertiaTensor)
	{
		CollisionInformation = collisionInformation;
		volume = collisionInformation.Shape.ComputeVolume();
		BecomeDynamic(mass, inertiaTensor);
		collisionInformation.Entity = this;
	}

	protected internal void Initialize(EntityCollidable collisionInformation, float mass, Matrix3X3 inertiaTensor, float volume)
	{
		CollisionInformation = collisionInformation;
		this.volume = volume;
		BecomeDynamic(mass, inertiaTensor);
		collisionInformation.Entity = this;
	}

	/// <summary>
	///  Applies an impulse to the entity.
	/// </summary>
	/// <param name="location">Location to apply the impulse.</param>
	/// <param name="impulse">Impulse to apply.</param>
	public void ApplyImpulse(Vector3 location, Vector3 impulse)
	{
		ApplyImpulse(ref location, ref impulse);
	}

	/// <summary>
	///  Applies an impulse to the entity.
	/// </summary>
	/// <param name="location">Location to apply the impulse.</param>
	/// <param name="impulse">Impulse to apply.</param>
	public void ApplyImpulse(ref Vector3 location, ref Vector3 impulse)
	{
		if (isDynamic)
		{
			ApplyLinearImpulse(ref impulse);
			Vector3 vector = new Vector3
			{
				X = location.X - position.X,
				Y = location.Y - position.Y,
				Z = location.Z - position.Z
			};
			Vector3.Cross(ref vector, ref impulse, out var result);
			ApplyAngularImpulse(ref result);
			activityInformation.Activate();
		}
	}

	/// <summary>
	/// Applies a linear velocity change to the entity using the given impulse.
	/// This method does not wake up the object or perform any other nonessential operation;
	/// it is meant to be used for performance-sensitive constraint solving.
	/// Consider equivalently adding to the LinearMomentum property for convenience instead.
	/// </summary>
	/// <param name="impulse">Impulse to apply.</param>
	public void ApplyLinearImpulse(ref Vector3 impulse)
	{
		linearMomentum.X += impulse.X;
		linearMomentum.Y += impulse.Y;
		linearMomentum.Z += impulse.Z;
		linearVelocity.X = linearMomentum.X * inverseMass;
		linearVelocity.Y = linearMomentum.Y * inverseMass;
		linearVelocity.Z = linearMomentum.Z * inverseMass;
	}

	/// <summary>
	/// Applies an angular velocity change to the entity using the given impulse.
	/// This method does not wake up the object or perform any other nonessential operation;
	/// it is meant to be used for performance-sensitive constraint solving.
	/// Consider equivalently adding to the AngularMomentum property for convenience instead.
	/// </summary>
	/// <param name="impulse">Impulse to apply.</param>
	public void ApplyAngularImpulse(ref Vector3 impulse)
	{
		angularMomentum.X += impulse.X;
		angularMomentum.Y += impulse.Y;
		angularMomentum.Z += impulse.Z;
		if (MotionSettings.ConserveAngularMomentum)
		{
			angularVelocity.X = angularMomentum.X * inertiaTensorInverse.M11 + angularMomentum.Y * inertiaTensorInverse.M21 + angularMomentum.Z * inertiaTensorInverse.M31;
			angularVelocity.Y = angularMomentum.X * inertiaTensorInverse.M12 + angularMomentum.Y * inertiaTensorInverse.M22 + angularMomentum.Z * inertiaTensorInverse.M32;
			angularVelocity.Z = angularMomentum.X * inertiaTensorInverse.M13 + angularMomentum.Y * inertiaTensorInverse.M23 + angularMomentum.Z * inertiaTensorInverse.M33;
		}
		else
		{
			angularVelocity.X += impulse.X * inertiaTensorInverse.M11 + impulse.Y * inertiaTensorInverse.M21 + impulse.Z * inertiaTensorInverse.M31;
			angularVelocity.Y += impulse.X * inertiaTensorInverse.M12 + impulse.Y * inertiaTensorInverse.M22 + impulse.Z * inertiaTensorInverse.M32;
			angularVelocity.Z += impulse.X * inertiaTensorInverse.M13 + impulse.Y * inertiaTensorInverse.M23 + impulse.Z * inertiaTensorInverse.M33;
		}
	}

	protected void OnShapeChanged(CollisionShape shape)
	{
		if (!IgnoreShapeChanges)
		{
			collisionInformation.Shape.ComputeDistributionInformation(out var shapeInfo);
			volume = shapeInfo.Volume;
			if (isDynamic)
			{
				Matrix3X3.Multiply(ref shapeInfo.VolumeDistribution, InertiaHelper.InertiaTensorScale * mass, out shapeInfo.VolumeDistribution);
				LocalInertiaTensor = shapeInfo.VolumeDistribution;
			}
			else
			{
				LocalInertiaTensorInverse = default(Matrix3X3);
			}
		}
	}

	/// <summary>
	///  Forces the entity to become kinematic.  Kinematic entities have infinite mass and inertia.
	/// </summary>
	public void BecomeKinematic()
	{
		bool flag = isDynamic;
		isDynamic = false;
		LocalInertiaTensorInverse = default(Matrix3X3);
		mass = 0f;
		inverseMass = 0f;
		if (flag)
		{
			if (activityInformation.DeactivationManager != null)
			{
				activityInformation.DeactivationManager.RemoveSimulationIslandFromMember(activityInformation);
			}
			if (((IForceUpdateable)this).ForceUpdater != null)
			{
				((IForceUpdateable)this).ForceUpdater.ForceUpdateableBecomingKinematic(this);
			}
		}
		if (collisionInformation.CollisionRules.Group == CollisionRules.DefaultDynamicCollisionGroup || collisionInformation.CollisionRules.Group == null)
		{
			collisionInformation.CollisionRules.Group = CollisionRules.DefaultKinematicCollisionGroup;
		}
		activityInformation.Activate();
		LinearVelocity = linearVelocity;
		AngularVelocity = angularVelocity;
	}

	/// <summary>
	///  Forces the entity to become dynamic.  Dynamic entities respond to collisions and have finite mass and inertia.
	/// </summary>
	/// <param name="mass">Mass to use for the entity.</param>
	public void BecomeDynamic(float mass)
	{
		Matrix3X3 matrix = collisionInformation.Shape.ComputeVolumeDistribution();
		Matrix3X3.Multiply(ref matrix, mass * InertiaHelper.InertiaTensorScale, out matrix);
		BecomeDynamic(mass, matrix);
	}

	/// <summary>
	///  Forces the entity to become dynamic.  Dynamic entities respond to collisions and have finite mass and inertia.
	/// </summary>
	/// <param name="mass">Mass to use for the entity.</param>
	///  <param name="localInertiaTensor">Inertia tensor to use for the entity.</param>
	public void BecomeDynamic(float mass, Matrix3X3 localInertiaTensor)
	{
		if (mass <= 0f || float.IsInfinity(mass) || float.IsNaN(mass))
		{
			throw new InvalidOperationException("Cannot use a mass of " + mass + " for a dynamic entity.  Consider using a kinematic entity instead.");
		}
		bool flag = isDynamic;
		isDynamic = true;
		LocalInertiaTensor = localInertiaTensor;
		this.mass = mass;
		inverseMass = 1f / mass;
		if (!flag)
		{
			if (activityInformation.DeactivationManager != null)
			{
				activityInformation.DeactivationManager.AddSimulationIslandToMember(activityInformation);
			}
			if (((IForceUpdateable)this).ForceUpdater != null)
			{
				((IForceUpdateable)this).ForceUpdater.ForceUpdateableBecomingDynamic(this);
			}
		}
		if (collisionInformation.CollisionRules.Group == CollisionRules.DefaultKinematicCollisionGroup || collisionInformation.CollisionRules.Group == null)
		{
			collisionInformation.CollisionRules.Group = CollisionRules.DefaultDynamicCollisionGroup;
		}
		activityInformation.Activate();
		LinearVelocity = linearVelocity;
		AngularVelocity = angularVelocity;
	}

	void IForceUpdateable.UpdateForForces(float dt)
	{
		if (IsAffectedByGravity)
		{
			Vector3.Add(ref forceUpdater.gravityDt, ref linearVelocity, out linearVelocity);
		}
		if (activityInformation.DeactivationManager.useStabilization && activityInformation.allowStabilization && (activityInformation.isSlowing || activityInformation.velocityTimeBelowLimit > activityInformation.DeactivationManager.lowVelocityTimeMinimum))
		{
			float num = linearVelocity.LengthSquared() + angularVelocity.LengthSquared();
			if (num < activityInformation.DeactivationManager.velocityLowerLimitSquared)
			{
				float damping = 1f - num / (2f * activityInformation.DeactivationManager.velocityLowerLimitSquared);
				ModifyAngularDamping(damping);
				ModifyLinearDamping(damping);
			}
		}
		float num2 = LinearDamping + linearDampingBoost;
		if (num2 > 0f)
		{
			Vector3.Multiply(ref linearVelocity, (float)Math.Pow(MathHelper.Clamp(1f - num2, 0f, 1f), dt), out linearVelocity);
		}
		float num3 = AngularDamping + angularDampingBoost;
		if (num3 > 0f && MotionSettings.ConserveAngularMomentum)
		{
			Vector3.Multiply(ref angularMomentum, (float)Math.Pow(MathHelper.Clamp(1f - num3, 0f, 1f), dt), out angularMomentum);
		}
		else if (num3 > 0f)
		{
			Vector3.Multiply(ref angularVelocity, (float)Math.Pow(MathHelper.Clamp(1f - num3, 0f, 1f), dt), out angularVelocity);
		}
		linearDampingBoost = 0f;
		angularDampingBoost = 0f;
		Vector3.Multiply(ref linearVelocity, mass, out linearMomentum);
		Matrix3X3.MultiplyTransposed(ref orientationMatrix, ref localInertiaTensorInverse, out var result);
		Matrix3X3.Multiply(ref result, ref orientationMatrix, out inertiaTensorInverse);
		Matrix3X3.MultiplyTransposed(ref orientationMatrix, ref localInertiaTensor, out result);
		Matrix3X3.Multiply(ref result, ref orientationMatrix, out inertiaTensor);
		if (MotionSettings.ConserveAngularMomentum)
		{
			Matrix3X3.Transform(ref angularMomentum, ref inertiaTensorInverse, out angularVelocity);
		}
		else
		{
			Matrix3X3.Transform(ref angularVelocity, ref inertiaTensor, out angularMomentum);
		}
	}

	void ISpaceObject.OnAdditionToSpace(ISpace newSpace)
	{
		OnAdditionToSpace(newSpace);
	}

	protected virtual void OnAdditionToSpace(ISpace newSpace)
	{
	}

	void ISpaceObject.OnRemovalFromSpace(ISpace oldSpace)
	{
		OnRemovalFromSpace(oldSpace);
	}

	protected virtual void OnRemovalFromSpace(ISpace oldSpace)
	{
	}

	void ICCDPositionUpdateable.UpdateTimeOfImpacts(float dt)
	{
		for (int i = 0; i < collisionInformation.pairs.count; i++)
		{
			if (MotionSettings.PairAllowsCCD(this, collisionInformation.pairs.Elements[i]))
			{
				collisionInformation.pairs.Elements[i].UpdateTimeOfImpact(collisionInformation, dt);
			}
		}
	}

	void ICCDPositionUpdateable.UpdatePositionContinuously(float dt)
	{
		float num = 1f;
		for (int i = 0; i < collisionInformation.pairs.Count; i++)
		{
			if (collisionInformation.pairs.Elements[i].timeOfImpact < num)
			{
				num = collisionInformation.pairs.Elements[i].timeOfImpact;
			}
		}
		Vector3.Multiply(ref linearVelocity, dt * num, out var result);
		Vector3.Add(ref position, ref result, out position);
		collisionInformation.UpdateWorldTransform(ref position, ref orientation);
		if (PositionUpdated != null)
		{
			PositionUpdated(this);
		}
	}

	void IPositionUpdateable.PreUpdatePosition(float dt)
	{
		Vector3 result;
		if (MotionSettings.UseRk4AngularIntegration && isDynamic)
		{
			Toolbox.UpdateOrientationRK4(ref orientation, ref localInertiaTensorInverse, ref angularMomentum, dt, out orientation);
		}
		else
		{
			Vector3.Multiply(ref angularVelocity, dt * 0.5f, out result);
			Quaternion quaternion = new Quaternion(result.X, result.Y, result.Z, 0f);
			Quaternion.Multiply(ref quaternion, ref orientation, out quaternion);
			Quaternion.Add(ref orientation, ref quaternion, out orientation);
			orientation.Normalize();
		}
		Matrix3X3.CreateFromQuaternion(ref orientation, out orientationMatrix);
		if (PositionUpdateMode == PositionUpdateMode.Discrete)
		{
			Vector3.Multiply(ref linearVelocity, dt, out result);
			Vector3.Add(ref position, ref result, out position);
			collisionInformation.UpdateWorldTransform(ref position, ref orientation);
			if (PositionUpdated != null)
			{
				PositionUpdated(this);
			}
		}
		collisionInformation.UpdateWorldTransform(ref position, ref orientation);
	}

	/// <summary>
	/// Temporarily adjusts the linear damping by an amount.  After the value is used, the
	/// damping returns to the base value.
	/// </summary>
	/// <param name="damping">Damping to add.</param>
	public void ModifyLinearDamping(float damping)
	{
		float num = LinearDamping + linearDampingBoost;
		float num2 = 1f - num;
		linearDampingBoost += damping * num2;
	}

	/// <summary>
	/// Temporarily adjusts the angular damping by an amount.  After the value is used, the
	/// damping returns to the base value.
	/// </summary>
	/// <param name="damping">Damping to add.</param>
	public void ModifyAngularDamping(float damping)
	{
		float num = AngularDamping + angularDampingBoost;
		float num2 = 1f - num;
		angularDampingBoost += damping * num2;
	}

	public override string ToString()
	{
		if (Tag == null)
		{
			return base.ToString();
		}
		return base.ToString() + ", " + Tag;
	}

	private void InitializeId()
	{
		InstanceId = Interlocked.Increment(ref idCounter);
		hashCode = (int)((ulong)(InstanceId * 4294967311L) % 4294967296uL);
	}

	public override int GetHashCode()
	{
		return hashCode;
	}
}
/// <summary>
///  Superclass of all entities which have a defined collidable type.
///  After construction, the collidable on this sort of Entity cannot be changed.
///  It can be constructed directly, or one of its prefab children (Box, Sphere, etc.) can be used.
/// </summary>
///  <remarks>If the collidable needs to be changed after construction, consider using the MorphableEntity.</remarks>
/// <typeparam name="T">Type of EntityCollidable to use for the entity.</typeparam>
public class Entity<T> : Entity where T : EntityCollidable
{
	/// <summary>
	///  Gets the collidable used by the entity.
	/// </summary>
	public new T CollisionInformation => (T)collisionInformation;

	protected internal Entity()
	{
	}

	/// <summary>
	///  Constructs a kinematic Entity.
	/// </summary>
	/// <param name="collisionInformation">Collidable for the entity.</param>
	public Entity(T collisionInformation)
	{
		Initialize(collisionInformation);
	}

	/// <summary>
	///  Constructs a kinematic Entity.
	/// </summary>
	/// <param name="collisionInformation">Collidable for the entity.</param>
	public Entity(T collisionInformation, bool computeVolume)
	{
		Initialize(collisionInformation);
	}

	/// <summary>
	///  Constructs a dynamic Entity.
	/// </summary>
	/// <param name="collisionInformation">Collidable for the entity.</param>
	///  <param name="mass">Mass of the entity.</param>
	public Entity(T collisionInformation, float mass)
	{
		Initialize(collisionInformation, mass);
	}

	/// <summary>
	///  Constructs a dynamic Entity.
	/// </summary>
	/// <param name="collisionInformation">Collidable for the entity.</param>
	///  <param name="mass">Mass of the entity.</param>
	///  <param name="inertiaTensor">Inertia of the entity.</param>
	public Entity(T collisionInformation, float mass, Matrix3X3 inertiaTensor)
	{
		Initialize(collisionInformation, mass, inertiaTensor);
	}

	/// <summary>
	///  Constructs a dynamic Entity.
	/// </summary>
	/// <param name="collisionInformation">Collidable for the entity.</param>
	///  <param name="mass">Mass of the entity.</param>
	///  <param name="inertiaTensor">Inertia of the entity.</param>
	///  <param name="volume">Volume of the entity.</param>
	public Entity(T collisionInformation, float mass, Matrix3X3 inertiaTensor, float volume)
	{
		Initialize(collisionInformation, mass, inertiaTensor, volume);
	}
}
