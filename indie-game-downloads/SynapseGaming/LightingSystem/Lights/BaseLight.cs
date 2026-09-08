using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;
using _0003;
using _0016;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Components;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Serialization;
using SynapseGaming.LightingSystem.Shadows;

namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Abstract class that provides the base properties required for all light types.
/// </summary>
[Serializable]
public abstract class BaseLight : ILight, IMovableObject, IWorldBoundingBoxObject, IComponentObject<ILight>, IEditorCreatedObject<ILight>, IEditorObject, INamedObject, IFullSerializable, ISerializable
{
	private UpdateDelegate _3A_0018;

	private SubmitRemoveManagerDelegate _3AL;

	private SubmitRemoveManagerDelegate _3A_0019;

	private ComponentCollection<ILight> _3A3;

	private TypeDictionary<IManagerService> _3A6 = new TypeDictionary<IManagerService>();

	private int _3AD;

	private bool _3A_0017 = true;

	private UpdateType _3A_0003;

	private Vector3 _3Al = new Vector3(0.7f, 0.6f, 0.5f);

	private float _3At = 1f;

	private string _3AF = "";

	[CompilerGenerated]
	private BoundingBox _3Ac;

	[CompilerGenerated]
	private BoundingSphere _3Ag;

	[CompilerGenerated]
	private bool _3AI;

	/// <summary>
	/// Dictionary of all managers the object is currently contained in (submitted to).
	///
	/// Managers are accessible by their ManagerType and only one manager of a
	/// particular type can be contained in the dictionary at a time.
	/// </summary>
	public TypeDictionary<IManagerService> ContainingManagers => _3A6;

	/// <summary>
	/// Turns illumination on and off without removing the light from the scene.
	/// </summary>
	public bool Enabled
	{
		get
		{
			return _3A_0017;
		}
		set
		{
			_3A_0017 = value;
		}
	}

	/// <summary>
	/// Determines if the lighting is real-time or bake-down.
	/// </summary>
	public abstract LightingType LightingType { get; set; }

	/// <summary>
	/// Direct lighting color given off by the light.
	/// </summary>
	public Vector3 DiffuseColor
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
	/// Intensity of the light.
	/// </summary>
	public float Intensity
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
	/// Provides softer indirect-like illumination without "hot-spots".
	/// </summary>
	public abstract bool FillLight { get; set; }

	/// <summary>
	/// Controls how quickly lighting falls off over distance (only available in deferred rendering).
	/// Value ranges from 0.0f to 1.0f.
	/// </summary>
	public abstract float FalloffStrength { get; set; }

	/// <summary>
	/// The combined light color and intensity (provided for convenience).
	/// </summary>
	public Vector3 CompositeColorAndIntensity => _3Al * _3At;

	/// <summary>
	/// Bounding area of the light's influence.
	/// </summary>
	public BoundingBox WorldBoundingBox
	{
		[CompilerGenerated]
		get
		{
			return _3Ac;
		}
		[CompilerGenerated]
		protected set
		{
			_3Ac = value;
		}
	}

	/// <summary>
	/// Bounding area of the light's influence.
	/// </summary>
	public BoundingSphere WorldBoundingSphere
	{
		[CompilerGenerated]
		get
		{
			return _3Ag;
		}
		[CompilerGenerated]
		protected set
		{
			_3Ag = value;
		}
	}

	/// <summary>
	/// Shadow source the light's shadows are generated from.
	/// Allows sharing shadows between point light sources.
	/// </summary>
	public abstract IShadowSource ShadowSource { get; set; }

	/// <summary>
	/// World space transform of the light.
	/// </summary>
	public abstract Matrix World { get; set; }

	/// <summary>
	/// Unique id used to identify the object across multiple scene loads / reloads.
	/// </summary>
	public int UniqueId => _3AD;

	/// <summary>
	/// Indicates the object bounding area spans the entire world and
	/// the object is always visible.
	/// </summary>
	public abstract bool InfiniteBounds { get; }

	/// <summary>
	/// Indicates the current move. This value increments each time the object
	/// is moved (when the World transform changes).
	/// </summary>
	public abstract int MoveId { get; }

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
			return _3A_0003;
		}
		set
		{
			_3A_0003 = value;
		}
	}

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
			return _3AF;
		}
		set
		{
			_3AF = value;
		}
	}

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
			return _3AI;
		}
		[CompilerGenerated]
		set
		{
			_3AI = value;
		}
	}

	/// <summary>
	/// Container that stores, manages, and updates the object's components.
	/// </summary>
	public ComponentCollection<ILight> Components => _3A3;

	/// <summary>
	/// Event used to update the light at regular intervals. This and all
	/// events are only called on dynamic lights.
	/// </summary>
	public event UpdateDelegate UpdateEvent
	{
		add
		{
			UpdateDelegate updateDelegate = _3A_0018;
			UpdateDelegate updateDelegate2;
			do
			{
				updateDelegate2 = updateDelegate;
				UpdateDelegate value2 = (UpdateDelegate)Delegate.Combine(updateDelegate2, value);
				updateDelegate = Interlocked.CompareExchange(ref _3A_0018, value2, updateDelegate2);
			}
			while ((object)updateDelegate != updateDelegate2);
		}
		remove
		{
			UpdateDelegate updateDelegate = _3A_0018;
			UpdateDelegate updateDelegate2;
			do
			{
				updateDelegate2 = updateDelegate;
				UpdateDelegate value2 = (UpdateDelegate)Delegate.Remove(updateDelegate2, value);
				updateDelegate = Interlocked.CompareExchange(ref _3A_0018, value2, updateDelegate2);
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
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate = _3AL;
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate2;
			do
			{
				submitRemoveManagerDelegate2 = submitRemoveManagerDelegate;
				SubmitRemoveManagerDelegate value2 = (SubmitRemoveManagerDelegate)Delegate.Combine(submitRemoveManagerDelegate2, value);
				submitRemoveManagerDelegate = Interlocked.CompareExchange(ref _3AL, value2, submitRemoveManagerDelegate2);
			}
			while ((object)submitRemoveManagerDelegate != submitRemoveManagerDelegate2);
		}
		remove
		{
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate = _3AL;
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate2;
			do
			{
				submitRemoveManagerDelegate2 = submitRemoveManagerDelegate;
				SubmitRemoveManagerDelegate value2 = (SubmitRemoveManagerDelegate)Delegate.Remove(submitRemoveManagerDelegate2, value);
				submitRemoveManagerDelegate = Interlocked.CompareExchange(ref _3AL, value2, submitRemoveManagerDelegate2);
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
	/// Creates a new BaseLight instance.
	/// </summary>
	public BaseLight()
	{
		_3A3 = new ComponentCollection<ILight>(this);
		if (_3AD == 0)
		{
			_3AD = CoreHelper.GetUniqueId(this);
		}
	}

	/// <summary>
	/// Updates the object using the provided game time.
	/// </summary>
	/// <param name="gametime"></param>
	public virtual void Update(GameTime gametime)
	{
		_3A3.OnUpdate(gametime);
		if (_3A_0018 != null)
		{
			_3A_0018(this, gametime);
		}
	}

	/// <summary>
	/// Called when the object is submitted to a manager.
	/// </summary>
	/// <param name="manager"></param>
	public virtual void OnSubmittedToManager(IManagerService manager)
	{
		_3A6.Add(manager.ManagerType, manager);
		_3A3.OnSubmittedToManager(manager);
		if (_3AL != null)
		{
			_3AL(manager);
		}
	}

	/// <summary>
	/// Called when the object is removed from a manager.
	/// </summary>
	/// <param name="manager"></param>
	public virtual void OnRemovedFromManager(IManagerService manager)
	{
		_3A3.OnRemovedFromManager(manager);
		if (_3A_0019 != null)
		{
			_3A_0019(manager);
		}
		_3A6.Remove(manager.ManagerType);
	}

	/// <summary>
	/// Called when the object is created in the SunBurn editor.
	/// </summary>
	public virtual void OnCreatedInEditor()
	{
	}

	/// <summary>
	/// Returns a String that represents the current Object.
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		return CoreHelper.GetDisplayName(this);
	}

	/// <summary>
	/// Deep clones the object including any contained sub-objects and components.
	/// </summary>
	/// <returns></returns>
	public virtual ILight Clone()
	{
		ILight light = Create();
		_0003._6.L_0003(this, light);
		foreach (IComponent<ILight> component in _3A3.Components)
		{
			light.Components.Add(component.Clone());
		}
		return light;
	}

	/// <summary>
	/// Creates a new instance of the object type. This method assumes the type has a
	/// default constructor. If the type does not have a default constructor this method
	/// can be overridden to manually create the type.
	/// </summary>
	/// <returns></returns>
	protected virtual ILight Create()
	{
		return (ILight)Activator.CreateInstance(GetType());
	}

	/// <summary>
	/// Deserializes object data from the provided SerializationInfo.
	/// </summary>
	/// <param name="info">Contains the serialized object data.</param>
	/// <param name="context"></param>
	public virtual void SetObjectData(SerializationInfo info, StreamingContext context)
	{
		SerializationHelper.DeserializeField(ref _3AD, info, "UniqueId", usedefault: false);
		SerializationHelper.DeserializeField(ref _3A_0017, info, "Enabled", usedefault: true);
		SerializationHelper.DeserializeField(ref _3Al, info, "DiffuseColor", usedefault: true);
		SerializationHelper.DeserializeField(ref _3At, info, "Intensity", usedefault: true);
		SerializationHelper.DeserializeField(ref _3AF, info, "Name", usedefault: true);
		_3A_0003 = _0016._6._6f(info);
		LightingType field = LightingType;
		SerializationHelper.DeserializeEnum(ref field, info, "LightingType", isflag: true);
		LightingType = field;
		FillLight = SerializationHelper.DeserializeField<bool>(info, "FillLight");
		FalloffStrength = SerializationHelper.DeserializeField<float>(info, "FalloffStrength");
		_3A3.SetObjectData(info, context);
	}

	/// <summary>
	/// Serializes object data to the provided SerializationInfo.
	/// </summary>
	/// <param name="info">SerializationInfo to store the serialized data.</param>
	/// <param name="context"></param>
	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("UniqueId", UniqueId);
		info.AddValue("Name", Name);
		info.AddValue("UpdateType", UpdateType);
		info.AddValue("Enabled", Enabled);
		info.AddValue("LightingType", LightingType);
		info.AddValue("DiffuseColor", DiffuseColor);
		info.AddValue("Intensity", Intensity);
		info.AddValue("FillLight", FillLight);
		info.AddValue("FalloffStrength", FalloffStrength);
		_3A3.GetObjectData(info, context);
	}
}
