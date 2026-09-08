using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using _0003;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Components;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Avatar implementation that provides properties necessary for avatar
/// rendering.
/// </summary>
public class Avatar : IAvatar, ICollisionObject, ISceneEntity, IMovableObject, IWorldBoundingBoxObject, IComponentObject<ISceneEntity>, IEditorCreatedObject<ISceneEntity>, IEditorObject, INamedObject, IEditorRenderableObject, ICollisionMaterial
{
	private const int _3A_0018 = 71;

	private UpdateDelegate _3AL;

	private SubmitRemoveManagerDelegate _3A_0019;

	private SubmitRemoveManagerDelegate _3A3;

	private CollisionReactDelegate _3A6;

	private CollisionTriggerDelegate _3AD;

	private TypeDictionary<IManagerService> _3A_0017 = new TypeDictionary<IManagerService>();

	private int _3A_0003;

	private string _3Al = "";

	private UpdateType _3At;

	private Matrix _3AF;

	private BoundingSphere _3Ac;

	private BoundingBox _3Ag;

	private BoundingBox _3AI;

	private BoundingBox _3A8;

	private BoundingSphere _3AZ;

	private BoundingBox _3Ax;

	private IList<Matrix> _3Aq;

	private AvatarExpression _3Ab;

	private AvatarRenderer _3AT;

	private AvatarDescription _3Ay;

	private ObjectVisibility _3A_0015 = ObjectVisibility.RenderedAndCastShadows;

	private int _3A_0001;

	private bool _3A7;

	private float _3AX;

	private float _3A_0010;

	private float _3A_0016;

	private CollisionType _3Ak;

	private ICollisionMove _3Ap;

	private static List<Matrix> _3Au;

	private ComponentCollection<ISceneEntity> _3AU;

	[CompilerGenerated]
	private Matrix _3Az;

	[CompilerGenerated]
	private bool _3A_0014;

	[CompilerGenerated]
	private ICollisionMaterial _3Av;

	/// <summary>
	/// Dictionary of all managers the object is currently contained in (submitted to).
	///
	/// Managers are accessible by their ManagerType and only one manager of a
	/// particular type can be contained in the dictionary at a time.
	/// </summary>
	public TypeDictionary<IManagerService> ContainingManagers => _3A_0017;

	/// <summary>
	/// Unique id used to identify the object across multiple scene loads / reloads.
	/// </summary>
	public int UniqueId => 0;

	/// <summary>
	/// Indicates the object bounding area spans the entire world and
	/// the object is always visible.
	/// </summary>
	public bool InfiniteBounds => false;

	/// <summary>
	/// Indicates the current move. This value increments each time the object
	/// is moved (when the World transform changes).
	/// </summary>
	public int MoveId => _3A_0003;

	/// <summary>
	/// The object's current name.
	///
	/// Important note: Name can be changed at any time, HOWEVER managers
	/// will only see the change after removing and resubmitting the object.
	/// </summary>
	public string Name
	{
		get
		{
			return _3Al;
		}
		set
		{
			_3Al = value;
		}
	}

	/// <summary>
	/// Determines if objects receive update events from the engine and are tracked
	/// by the scenegraph.
	///
	/// Automatic update events are necessary to be affected by gravity, for
	/// components, and for the scenegraph to track moving objects.  Objects without
	/// Automatic update events can still move, however the containing scenegraph
	/// (ObjectManager or LightManager) must be notified using Manager.Move(object).
	/// </summary>
	public UpdateType UpdateType
	{
		get
		{
			return _3At;
		}
		set
		{
			_3At = value;
		}
	}

	/// <summary>
	/// World space transform of the object.
	/// </summary>
	public Matrix World
	{
		get
		{
			return _3AF;
		}
		set
		{
			if (!_3AF.Equals(value))
			{
				_3AF = value;
				_3A_0003++;
				WorldToObject = Matrix.Invert(value);
				_3h();
			}
		}
	}

	public Matrix WorldToObject
	{
		[CompilerGenerated]
		get
		{
			return _3Az;
		}
		[CompilerGenerated]
		private set
		{
			_3Az = matrix;
		}
	}

	/// <summary>
	/// Array of bone transforms for the skeleton's current pose. The matrix index is the
	/// same as the bone order used by the avatar.
	/// </summary>
	public IList<Matrix> SkinBones
	{
		get
		{
			return _3Aq;
		}
		set
		{
			_3Aq = value;
		}
	}

	/// <summary>
	/// The current avatar facial expression.
	/// </summary>
	public AvatarExpression Expression
	{
		get
		{
			return _3Ab;
		}
		set
		{
			_3Ab = value;
		}
	}

	/// <summary>
	/// Defines how the avatar is rendered.
	///
	/// This enumeration is a Flag, which allows combining multiple values using the
	/// Logical OR operator (example: "ObjectVisibility.Rendered | ObjectVisibility.CastShadows",
	/// both renders the avatar and casts shadows from it).
	/// </summary>
	public ObjectVisibility Visibility
	{
		get
		{
			return _3A_0015;
		}
		set
		{
			_3A_0015 = value;
		}
	}

	/// <summary>
	/// Determines the bounds used in object culling and collision.
	/// </summary>
	public HullType HullType
	{
		get
		{
			return HullType.Box;
		}
		set
		{
		}
	}

	/// <summary>
	/// Object space bounding area of the object.
	/// </summary>
	public BoundingSphere ObjectBoundingSphere => _3Ac;

	/// <summary>
	/// Object space bounding area of the object.
	/// </summary>
	public BoundingBox ObjectBoundingBox => _3Ag;

	/// <summary>
	/// World space bounding area of the object.
	/// </summary>
	public BoundingBox WorldBoundingBox => _3A8;

	/// <summary>
	/// World space bounding area of the object.
	/// </summary>
	public BoundingSphere WorldBoundingSphere => _3AZ;

	/// <summary>
	/// Extended world space bounding area of the object. This area is roughly twice the size
	/// to accommodate avatar animations that fall outside the normal bounds.
	/// </summary>
	public BoundingBox WorldBoundingBoxProxy => _3Ax;

	/// <summary>
	/// AvatarRenderer used to render the avatar.
	/// </summary>
	public AvatarRenderer Renderer => _3AT;

	/// <summary>
	/// Description of the avatar size, clothing, features, and more.
	/// </summary>
	public AvatarDescription Description => _3Ay;

	/// <summary>
	/// Determines if the avatar casts shadows base on the current ObjectVisibility options.
	/// </summary>
	public bool CastShadows => (_3A_0015 & ObjectVisibility.CastShadows) != 0;

	/// <summary>
	/// Determines if the avatar is visible base on the current ObjectVisibility options.
	/// </summary>
	public bool Visible => (_3A_0015 & ObjectVisibility.Rendered) != 0;

	/// <summary>
	/// Notifies the editor that this object is partially controlled via code. The editor
	/// will display information to the user indicating some property values are
	/// overridden in code and changes may not take effect.
	/// </summary>
	public bool AffectedInCode
	{
		[CompilerGenerated]
		get
		{
			return _3A_0014;
		}
		[CompilerGenerated]
		set
		{
			_3A_0014 = value;
		}
	}

	/// <summary>
	/// Container that stores, manages, and updates the object's components.
	/// </summary>
	public ComponentCollection<ISceneEntity> Components => _3AU;

	/// <summary>
	/// Amount material absorbs impact force.
	/// </summary>
	public float Elasticity
	{
		get
		{
			return _3A_0010;
		}
		set
		{
			_3A_0010 = value;
			_3A_0001++;
		}
	}

	/// <summary>
	/// Amount material resists objects moving across its surface.
	/// </summary>
	public float Friction
	{
		get
		{
			return _3AX;
		}
		set
		{
			_3AX = value;
			_3A_0001++;
		}
	}

	/// <summary>
	/// Determines if gravity will cause the object to fall. For an object to be affected
	/// by gravity its UpdateType must be Automatic and CollisionType must be Collide.
	/// </summary>
	public bool AffectedByGravity
	{
		get
		{
			return _3A7;
		}
		set
		{
			_3A7 = value;
			_3A_0001++;
			if (!_3A7 && _3Ap != null)
			{
				_3Ap.RemoveForces();
			}
		}
	}

	/// <summary>
	/// Indicates if collision related properties changed. This value increments each time the object
	/// collision properties change.
	/// </summary>
	public int CollisionId
	{
		get
		{
			return _3A_0001;
		}
		set
		{
			_3A_0001 = value;
		}
	}

	/// <summary>
	/// Move helper used by this object to determine its momentum, next location, and sweep volume.
	/// </summary>
	public ICollisionMove CollisionMove
	{
		get
		{
			return _3Ap;
		}
		set
		{
			if (value == _3Ap)
			{
				return;
			}
			if (_3Ap != null)
			{
				foreach (KeyValuePair<Type, IManagerService> item in ContainingManagers.Items)
				{
					_3Ap.OnRemovedFromManager(item.Value);
				}
			}
			_3Ap = value;
			if (value == null)
			{
				return;
			}
			foreach (KeyValuePair<Type, IManagerService> item2 in ContainingManagers.Items)
			{
				value.OnSubmittedToManager(item2.Value);
			}
		}
	}

	/// <summary>
	/// Determines how an object interacts with the scene.
	/// </summary>
	public CollisionType CollisionType
	{
		get
		{
			return _3Ak;
		}
		set
		{
			_3Ak = value;
			_3A_0001++;
		}
	}

	/// <summary>
	/// Default material used when collision surface does not implement material info.
	/// </summary>
	public ICollisionMaterial DefaultCollisionMaterial
	{
		[CompilerGenerated]
		get
		{
			return _3Av;
		}
		[CompilerGenerated]
		set
		{
			_3Av = value;
		}
	}

	/// <summary>
	/// Mass of the object.
	/// </summary>
	public float Mass
	{
		get
		{
			return _3A_0016;
		}
		set
		{
			_3A_0016 = value;
			_3A_0001++;
		}
	}

	/// <summary>
	/// Event used to update the object at regular intervals. This and all
	/// events are only called on dynamic objects.
	/// </summary>
	public event UpdateDelegate UpdateEvent
	{
		add
		{
			UpdateDelegate updateDelegate = _3AL;
			UpdateDelegate updateDelegate2;
			do
			{
				updateDelegate2 = updateDelegate;
				UpdateDelegate value2 = (UpdateDelegate)Delegate.Combine(updateDelegate2, value);
				updateDelegate = Interlocked.CompareExchange(ref _3AL, value2, updateDelegate2);
			}
			while ((object)updateDelegate != updateDelegate2);
		}
		remove
		{
			UpdateDelegate updateDelegate = _3AL;
			UpdateDelegate updateDelegate2;
			do
			{
				updateDelegate2 = updateDelegate;
				UpdateDelegate value2 = (UpdateDelegate)Delegate.Remove(updateDelegate2, value);
				updateDelegate = Interlocked.CompareExchange(ref _3AL, value2, updateDelegate2);
			}
			while ((object)updateDelegate != updateDelegate2);
		}
	}

	/// <summary>
	/// Event used to determine when the object is submitted to a manager.
	/// </summary>
	public event SubmitRemoveManagerDelegate SubmittedToManagerEvent
	{
		add
		{
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate = _3A_0019;
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate2;
			do
			{
				submitRemoveManagerDelegate2 = submitRemoveManagerDelegate;
				SubmitRemoveManagerDelegate value2 = (SubmitRemoveManagerDelegate)Delegate.Combine(submitRemoveManagerDelegate2, value);
				submitRemoveManagerDelegate = Interlocked.CompareExchange(ref _3A_0019, value2, submitRemoveManagerDelegate2);
			}
			while ((object)submitRemoveManagerDelegate != submitRemoveManagerDelegate2);
		}
		remove
		{
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate = _3A_0019;
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate2;
			do
			{
				submitRemoveManagerDelegate2 = submitRemoveManagerDelegate;
				SubmitRemoveManagerDelegate value2 = (SubmitRemoveManagerDelegate)Delegate.Remove(submitRemoveManagerDelegate2, value);
				submitRemoveManagerDelegate = Interlocked.CompareExchange(ref _3A_0019, value2, submitRemoveManagerDelegate2);
			}
			while ((object)submitRemoveManagerDelegate != submitRemoveManagerDelegate2);
		}
	}

	/// <summary>
	/// Event used to determine when the object is removed from a manager.
	/// </summary>
	public event SubmitRemoveManagerDelegate RemovedFromManagerEvent
	{
		add
		{
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate = _3A3;
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate2;
			do
			{
				submitRemoveManagerDelegate2 = submitRemoveManagerDelegate;
				SubmitRemoveManagerDelegate value2 = (SubmitRemoveManagerDelegate)Delegate.Combine(submitRemoveManagerDelegate2, value);
				submitRemoveManagerDelegate = Interlocked.CompareExchange(ref _3A3, value2, submitRemoveManagerDelegate2);
			}
			while ((object)submitRemoveManagerDelegate != submitRemoveManagerDelegate2);
		}
		remove
		{
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate = _3A3;
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate2;
			do
			{
				submitRemoveManagerDelegate2 = submitRemoveManagerDelegate;
				SubmitRemoveManagerDelegate value2 = (SubmitRemoveManagerDelegate)Delegate.Remove(submitRemoveManagerDelegate2, value);
				submitRemoveManagerDelegate = Interlocked.CompareExchange(ref _3A3, value2, submitRemoveManagerDelegate2);
			}
			while ((object)submitRemoveManagerDelegate != submitRemoveManagerDelegate2);
		}
	}

	/// <summary>
	/// Event used to detect when the object collides with another object, or to
	/// override the default reaction behavior between objects.
	/// </summary>
	public event CollisionReactDelegate CollisionReactEvent
	{
		add
		{
			CollisionReactDelegate collisionReactDelegate = _3A6;
			CollisionReactDelegate collisionReactDelegate2;
			do
			{
				collisionReactDelegate2 = collisionReactDelegate;
				CollisionReactDelegate value2 = (CollisionReactDelegate)Delegate.Combine(collisionReactDelegate2, value);
				collisionReactDelegate = Interlocked.CompareExchange(ref _3A6, value2, collisionReactDelegate2);
			}
			while ((object)collisionReactDelegate != collisionReactDelegate2);
		}
		remove
		{
			CollisionReactDelegate collisionReactDelegate = _3A6;
			CollisionReactDelegate collisionReactDelegate2;
			do
			{
				collisionReactDelegate2 = collisionReactDelegate;
				CollisionReactDelegate value2 = (CollisionReactDelegate)Delegate.Remove(collisionReactDelegate2, value);
				collisionReactDelegate = Interlocked.CompareExchange(ref _3A6, value2, collisionReactDelegate2);
			}
			while ((object)collisionReactDelegate != collisionReactDelegate2);
		}
	}

	/// <summary>
	/// Event used to detect when another object collides with this object, but only
	/// when this object's CollisionType is set to Trigger.
	///
	/// The event handler can then apply custom trigger code like damage, apply force, and more.
	/// </summary>
	public event CollisionTriggerDelegate CollisionTriggerEvent
	{
		add
		{
			CollisionTriggerDelegate collisionTriggerDelegate = _3AD;
			CollisionTriggerDelegate collisionTriggerDelegate2;
			do
			{
				collisionTriggerDelegate2 = collisionTriggerDelegate;
				CollisionTriggerDelegate value2 = (CollisionTriggerDelegate)Delegate.Combine(collisionTriggerDelegate2, value);
				collisionTriggerDelegate = Interlocked.CompareExchange(ref _3AD, value2, collisionTriggerDelegate2);
			}
			while ((object)collisionTriggerDelegate != collisionTriggerDelegate2);
		}
		remove
		{
			CollisionTriggerDelegate collisionTriggerDelegate = _3AD;
			CollisionTriggerDelegate collisionTriggerDelegate2;
			do
			{
				collisionTriggerDelegate2 = collisionTriggerDelegate;
				CollisionTriggerDelegate value2 = (CollisionTriggerDelegate)Delegate.Remove(collisionTriggerDelegate2, value);
				collisionTriggerDelegate = Interlocked.CompareExchange(ref _3AD, value2, collisionTriggerDelegate2);
			}
			while ((object)collisionTriggerDelegate != collisionTriggerDelegate2);
		}
	}

	static Avatar()
	{
		_3Au = new List<Matrix>();
		for (int i = 0; i < 71; i++)
		{
			_3Au.Add(Matrix.Identity);
		}
	}

	/// <summary>
	/// Creates a new Avatar instance.
	/// </summary>
	/// <param name="avatarrenderer">AvatarRenderer used to render the avatar.</param>
	/// <param name="description">Description of the avatar size, clothing, features, and more.</param>
	public Avatar(AvatarRenderer avatarrenderer, AvatarDescription description)
	{
		_3AU = new ComponentCollection<ISceneEntity>(this);
		_3Aq = _3Au;
		_3AF = Matrix.Identity;
		DefaultCollisionMaterial = this;
		Elasticity = 0.5f;
		Friction = 0.25f;
		SetRendererAndDescription(avatarrenderer, description);
		_3h();
	}

	/// <summary>
	/// Changes both the renderer and description used by the avatar.
	/// </summary>
	/// <param name="avatarrenderer">AvatarRenderer used to render the avatar.</param>
	/// <param name="description">Description of the avatar size, clothing, features, and more.</param>
	public void SetRendererAndDescription(AvatarRenderer avatarrenderer, AvatarDescription description)
	{
		_3AT = avatarrenderer;
		_3Ay = description;
		_3Ag = new BoundingBox(new Vector3(-0.5f, 0f, -0.5f), new Vector3(0.5f, _3Ay.Height, 0.5f));
		_3Ac = BoundingSphere.CreateFromBoundingBox(_3Ag);
		_3AI = CoreHelper.TransformBoundingBox(_3Ag, Matrix.CreateScale(2f));
		_3h();
	}

	/// <summary>
	/// Deep clones the object including any contained sub-objects and components.
	/// </summary>
	/// <returns></returns>
	public virtual ISceneEntity Clone()
	{
		Avatar avatar = new Avatar(_3AT, _3Ay);
		_0003._6.L_0003(this, avatar);
		foreach (IComponent<ISceneEntity> component in _3AU.Components)
		{
			avatar.Components.Add(component.Clone());
		}
		return avatar;
	}

	/// <summary>
	/// Updates the object using the provided game time.
	/// </summary>
	/// <param name="gametime"></param>
	public virtual void Update(GameTime gametime)
	{
		_3AU.OnUpdate(gametime);
		if (_3AL != null)
		{
			_3AL(this, gametime);
		}
	}

	/// <summary>
	/// Called when the object is submitted to a manager.
	/// </summary>
	/// <param name="manager"></param>
	public virtual void OnSubmittedToManager(IManagerService manager)
	{
		_3A_0017.Add(manager.ManagerType, manager);
		_3AU.OnSubmittedToManager(manager);
		if (_3Ap != null)
		{
			_3Ap.OnSubmittedToManager(manager);
		}
		if (_3A_0019 != null)
		{
			_3A_0019(manager);
		}
	}

	/// <summary>
	/// Called when the object is removed from a manager.
	/// </summary>
	/// <param name="manager"></param>
	public virtual void OnRemovedFromManager(IManagerService manager)
	{
		_3AU.OnRemovedFromManager(manager);
		if (_3Ap != null)
		{
			_3Ap.OnRemovedFromManager(manager);
		}
		if (_3A3 != null)
		{
			_3A3(manager);
		}
		_3A_0017.Remove(manager.ManagerType);
	}

	/// <summary>
	/// Called when the object is created in the SunBurn editor.
	/// </summary>
	public virtual void OnCreatedInEditor()
	{
	}

	/// <summary>
	/// Used to trigger the CollisionReactEvent event when two objects collide.
	/// </summary>
	/// <param name="collider">The moving object.</param>
	/// <param name="collidee">The object hit by the moving object.</param>
	/// <param name="worldcollisionpoint">Contains information about the closest collision point to the collider.</param>
	/// <param name="collisionhandled">Determines if the collision was handled by a prior event hander.
	/// If this value is true do NOT process any collision reaction code. If the event handler processes
	/// collision reaction code set this value to true to avoid another handler or SunBurn's built-in
	/// reaction code from processing.</param>
	public void OnCollisionReact(IMovableObject collider, IMovableObject collidee, CollisionPoint worldcollisionpoint, ref bool collisionhandled)
	{
		if (_3A6 != null)
		{
			_3A6(collider, collidee, worldcollisionpoint, ref collisionhandled);
		}
		Components.OnCollisionReact(collider, collidee, worldcollisionpoint, ref collisionhandled);
	}

	/// <summary>
	/// Used to trigger the CollisionTriggerEvent event when an object passes through or overlaps a trigger.
	/// </summary>
	/// <param name="collider">The moving object.</param>
	/// <param name="trigger">The trigger hit by the moving object.</param>
	public void OnCollisionTrigger(IMovableObject collider, IMovableObject trigger)
	{
		if (_3AD != null)
		{
			_3AD(collider, trigger);
		}
		Components.OnCollisionTrigger(collider, trigger);
	}

	/// <summary>
	/// Sets both the avatar bone transforms and expression using an AvatarAnimation object.
	/// </summary>
	/// <param name="animation"></param>
	public void ApplyAnimation(IAvatarAnimation animation)
	{
		_3Aq = animation.BoneTransforms;
		_3Ab = animation.Expression;
	}

	/// <summary>
	/// Sets both the world and inverse world matrices.  Used to improve
	/// performance when the world matrix is set, by providing a cached
	/// or precalculated inverse matrix with the world matrix.
	/// </summary>
	/// <param name="world">World space transform of the object.</param>
	/// <param name="worldtoobj">Inverse world space transform of the object.</param>
	public void SetWorldAndWorldToObject(Matrix world, Matrix worldtoobj)
	{
		World = world;
	}

	private void _3h()
	{
		_3A8 = CoreHelper.TransformBoundingBox(_3Ag, _3AF);
		_3AZ = BoundingSphere.CreateFromBoundingBox(_3A8);
		_3Ax = CoreHelper.TransformBoundingBox(_3AI, _3AF);
	}

	/// <summary>
	/// Implements a custom rendering pass. The pass occurs after scene rendering completes, but before post processing.
	/// </summary>
	/// <param name="scenestate">Current state used to render the scene.</param>
	public virtual void RenderCustomPass(ISceneState scenestate)
	{
	}

	/// <summary>
	/// Implements rendering of in-editor icons and helpers.
	///
	/// This method is called twice per-frame: once with scene depth clipping enable, and once with it disabled.
	/// </summary>
	/// <param name="scenestate">Current state used to render the scene.</param>
	/// <param name="renderhelper">Helper used to draw lines associated with the object. Only calling Submit() is
	/// supported in this method, using other methods may affect rendering of lines drawn by other objects.</param>
	/// <param name="highlighted">Indicates if the object is currently highlighted by the editor.</param>
	/// <param name="selected">Indicates if the object is currently selected by the editor.</param>
	/// <param name="sceneoccludedpass">Indicates if the current rendering pass depth clips with the scene.
	/// If so rendered icons and helpers are occluded by scene objects.</param>
	public virtual void RenderEditorIcon(ISceneState scenestate, BoundingBoxRenderHelper renderhelper, bool highlighted, bool selected, bool sceneoccludedpass)
	{
	}
}
