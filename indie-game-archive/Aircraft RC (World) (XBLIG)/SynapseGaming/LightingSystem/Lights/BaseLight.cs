using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;
using Microsoft.Xna.Framework;
using R;
using SynapseGaming.LightingSystem.Components;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Serialization;
using SynapseGaming.LightingSystem.Shadows;
using Z;

namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Abstract class that provides the base properties required for all light types.
/// </summary>
[Serializable]
public abstract class BaseLight : ILight, IMovableObject, IWorldBoundingBoxObject, IComponentObject<ILight>, IEditorCreatedObject<ILight>, IEditorObject, INamedObject, IFullSerializable, ISerializable
{
	private UpdateDelegate HCB;

	private SubmitRemoveManagerDelegate HC_0002;

	private SubmitRemoveManagerDelegate HC_0012;

	private ComponentCollection<ILight> HCH;

	private TypeDictionary<IManagerService> HC7 = new TypeDictionary<IManagerService>();

	private int HC_0001;

	private bool HCw = true;

	private UpdateType HCZ;

	private Vector3 HC_000F = new Vector3(0.7f, 0.6f, 0.5f);

	private float HCy = 1f;

	private string HC6 = "";

	[CompilerGenerated]
	private BoundingBox HCD;

	[CompilerGenerated]
	private BoundingSphere HC_0011;

	[CompilerGenerated]
	private bool HCK;

	/// <summary>
	/// Dictionary of all managers the object is currently contained in (submitted to).
	///
	/// Managers are accessible by their ManagerType and only one manager of a
	/// particular type can be contained in the dictionary at a time.
	/// </summary>
	public TypeDictionary<IManagerService> ContainingManagers => HC7;

	/// <summary>
	/// Turns illumination on and off without removing the light from the scene.
	/// </summary>
	public bool Enabled
	{
		get
		{
			return HCw;
		}
		set
		{
			HCw = value;
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
			return HC_000F;
		}
		set
		{
			HC_000F = value;
		}
	}

	/// <summary>
	/// Intensity of the light.
	/// </summary>
	public float Intensity
	{
		get
		{
			return HCy;
		}
		set
		{
			HCy = value;
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
	public Vector3 CompositeColorAndIntensity => HC_000F * HCy;

	/// <summary>
	/// Bounding area of the light's influence.
	/// </summary>
	public BoundingBox WorldBoundingBox
	{
		[CompilerGenerated]
		get
		{
			return HCD;
		}
		[CompilerGenerated]
		protected set
		{
			HCD = value;
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
			return HC_0011;
		}
		[CompilerGenerated]
		protected set
		{
			HC_0011 = value;
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
	public int UniqueId => HC_0001;

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
			return HCZ;
		}
		set
		{
			HCZ = value;
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
			return HC6;
		}
		set
		{
			HC6 = value;
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
			return HCK;
		}
		[CompilerGenerated]
		set
		{
			HCK = value;
		}
	}

	/// <summary>
	/// Container that stores, manages, and updates the object's components.
	/// </summary>
	public ComponentCollection<ILight> Components => HCH;

	/// <summary>
	/// Event used to update the light at regular intervals. This and all
	/// events are only called on dynamic lights.
	/// </summary>
	public event UpdateDelegate UpdateEvent
	{
		add
		{
			UpdateDelegate updateDelegate = HCB;
			UpdateDelegate updateDelegate2;
			do
			{
				updateDelegate2 = updateDelegate;
				UpdateDelegate value2 = (UpdateDelegate)Delegate.Combine(updateDelegate2, value);
				updateDelegate = Interlocked.CompareExchange(ref HCB, value2, updateDelegate2);
			}
			while ((object)updateDelegate != updateDelegate2);
		}
		remove
		{
			UpdateDelegate updateDelegate = HCB;
			UpdateDelegate updateDelegate2;
			do
			{
				updateDelegate2 = updateDelegate;
				UpdateDelegate value2 = (UpdateDelegate)Delegate.Remove(updateDelegate2, value);
				updateDelegate = Interlocked.CompareExchange(ref HCB, value2, updateDelegate2);
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
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate = HC_0002;
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate2;
			do
			{
				submitRemoveManagerDelegate2 = submitRemoveManagerDelegate;
				SubmitRemoveManagerDelegate value2 = (SubmitRemoveManagerDelegate)Delegate.Combine(submitRemoveManagerDelegate2, value);
				submitRemoveManagerDelegate = Interlocked.CompareExchange(ref HC_0002, value2, submitRemoveManagerDelegate2);
			}
			while ((object)submitRemoveManagerDelegate != submitRemoveManagerDelegate2);
		}
		remove
		{
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate = HC_0002;
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate2;
			do
			{
				submitRemoveManagerDelegate2 = submitRemoveManagerDelegate;
				SubmitRemoveManagerDelegate value2 = (SubmitRemoveManagerDelegate)Delegate.Remove(submitRemoveManagerDelegate2, value);
				submitRemoveManagerDelegate = Interlocked.CompareExchange(ref HC_0002, value2, submitRemoveManagerDelegate2);
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
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate = HC_0012;
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate2;
			do
			{
				submitRemoveManagerDelegate2 = submitRemoveManagerDelegate;
				SubmitRemoveManagerDelegate value2 = (SubmitRemoveManagerDelegate)Delegate.Combine(submitRemoveManagerDelegate2, value);
				submitRemoveManagerDelegate = Interlocked.CompareExchange(ref HC_0012, value2, submitRemoveManagerDelegate2);
			}
			while ((object)submitRemoveManagerDelegate != submitRemoveManagerDelegate2);
		}
		remove
		{
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate = HC_0012;
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate2;
			do
			{
				submitRemoveManagerDelegate2 = submitRemoveManagerDelegate;
				SubmitRemoveManagerDelegate value2 = (SubmitRemoveManagerDelegate)Delegate.Remove(submitRemoveManagerDelegate2, value);
				submitRemoveManagerDelegate = Interlocked.CompareExchange(ref HC_0012, value2, submitRemoveManagerDelegate2);
			}
			while ((object)submitRemoveManagerDelegate != submitRemoveManagerDelegate2);
		}
	}

	/// <summary>
	/// Creates a new BaseLight instance.
	/// </summary>
	public BaseLight()
	{
		HCH = new ComponentCollection<ILight>(this);
		if (HC_0001 == 0)
		{
			HC_0001 = CoreHelper.GetUniqueId(this);
		}
	}

	/// <summary>
	/// Updates the object using the provided game time.
	/// </summary>
	/// <param name="gametime"></param>
	public virtual void Update(GameTime gametime)
	{
		HCH.OnUpdate(gametime);
		if (HCB != null)
		{
			HCB(this, gametime);
		}
	}

	/// <summary>
	/// Called when the object is submitted to a manager.
	/// </summary>
	/// <param name="manager"></param>
	public virtual void OnSubmittedToManager(IManagerService manager)
	{
		HC7.Add(manager.ManagerType, manager);
		HCH.OnSubmittedToManager(manager);
		if (HC_0002 != null)
		{
			HC_0002(manager);
		}
	}

	/// <summary>
	/// Called when the object is removed from a manager.
	/// </summary>
	/// <param name="manager"></param>
	public virtual void OnRemovedFromManager(IManagerService manager)
	{
		HCH.OnRemovedFromManager(manager);
		if (HC_0012 != null)
		{
			HC_0012(manager);
		}
		HC7.Remove(manager.ManagerType);
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
		Z._7._0002w(this, light);
		foreach (IComponent<ILight> component in HCH.Components)
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
		SerializationHelper.DeserializeField(ref HC_0001, info, "UniqueId", usedefault: false);
		SerializationHelper.DeserializeField(ref HCw, info, "Enabled", usedefault: true);
		SerializationHelper.DeserializeField(ref HC_000F, info, "DiffuseColor", usedefault: true);
		SerializationHelper.DeserializeField(ref HCy, info, "Intensity", usedefault: true);
		SerializationHelper.DeserializeField(ref HC6, info, "Name", usedefault: true);
		HCZ = R._7._7o(info);
		LightingType field = LightingType;
		SerializationHelper.DeserializeEnum(ref field, info, "LightingType", isflag: true);
		LightingType = field;
		FillLight = SerializationHelper.DeserializeField<bool>(info, "FillLight");
		FalloffStrength = SerializationHelper.DeserializeField<float>(info, "FalloffStrength");
		HCH.SetObjectData(info, context);
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
		HCH.GetObjectData(info, context);
	}
}
