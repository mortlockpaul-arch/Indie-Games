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
using Z;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Scene entity implementation used to derive custom entities and scene objects.
/// </summary>
[Serializable]
[EditorCreatedObject]
public class SceneEntity : ISceneEntity, IMovableObject, IWorldBoundingBoxObject, IComponentObject<ISceneEntity>, IEditorCreatedObject<ISceneEntity>, IEditorObject, INamedObject, IEditorRenderableObject, IFullSerializable, ISerializable
{
	private UpdateDelegate HCB;

	private SubmitRemoveManagerDelegate HC_0002;

	private SubmitRemoveManagerDelegate HC_0012;

	private TypeDictionary<IManagerService> HCH = new TypeDictionary<IManagerService>();

	private int HC7;

	private UpdateType HC_0001;

	private Matrix HCw = Matrix.Identity;

	private Matrix HCZ = Matrix.Identity;

	private BoundingBox HC_000F = default(BoundingBox);

	private BoundingSphere HCy = default(BoundingSphere);

	private int HC6;

	private bool HCD;

	private string HC_0011;

	/// <summary />
	protected int _CollisionId;

	private HullType HCK = HullType.Box;

	private BoundingSphere HC_0003;

	private BoundingBox HCk;

	/// <summary />
	protected ComponentCollection<ISceneEntity> _Components;

	[CompilerGenerated]
	private bool HCs;

	/// <summary>
	/// Dictionary of all managers the object is currently contained in (submitted to).
	///
	/// Managers are accessible by their ManagerType and only one manager of a
	/// particular type can be contained in the dictionary at a time.
	/// </summary>
	public TypeDictionary<IManagerService> ContainingManagers => HCH;

	/// <summary>
	/// Unique id used to identify the object across multiple scene loads / reloads.
	/// </summary>
	[EditorProperty(false)]
	public int UniqueId
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
	/// World space transform of the object.
	/// </summary>
	[EditorProperty(false)]
	public Matrix World
	{
		get
		{
			return HCw;
		}
		set
		{
			if (!HCw.Equals(value))
			{
				SetWorldAndWorldToObject(value, Matrix.Invert(value));
			}
		}
	}

	/// <summary>
	/// Inverse world space transform of the object.
	/// </summary>
	public Matrix WorldToObject => HCZ;

	/// <summary>
	/// Indicates the object bounding area spans the entire world and
	/// the object is always visible.
	/// </summary>
	public bool InfiniteBounds => HCD;

	/// <summary>
	/// Indicates the current move. This value increments each time the object
	/// is moved (when the World transform changes).
	/// </summary>
	[EditorProperty(false)]
	public int MoveId
	{
		get
		{
			return HC7;
		}
		set
		{
			HC7 = value;
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
	[EditorCheckboxOptions(true)]
	[EditorProperty(true, Description = "Receives Updates", HorizontalAlignment = true, MajorGrouping = 2, MinorGrouping = 1, ToolTipText = "", ControlType = ControlType.CheckBox)]
	public UpdateType UpdateType
	{
		get
		{
			return HC_0001;
		}
		set
		{
			HC_0001 = value;
		}
	}

	/// <summary>
	/// Object space bounding area of the object.
	/// </summary>
	public BoundingSphere ObjectBoundingSphere => HC_0003;

	/// <summary>
	/// Object space bounding area of the object.
	/// </summary>
	public BoundingBox ObjectBoundingBox => HCk;

	/// <summary>
	/// World space bounding area of the object.
	/// </summary>
	public BoundingBox WorldBoundingBox => HC_000F;

	/// <summary>
	/// World space bounding area of the object.
	/// </summary>
	public BoundingSphere WorldBoundingSphere => HCy;

	/// <summary>
	/// The object's current name.
	///
	/// Important note: Name can be changed at any time, HOWEVER managers
	/// will only see the change after removing and resubmitting the object.
	/// </summary>
	[EditorProperty(true, Description = "Name", HorizontalAlignment = true, MajorGrouping = 1, MinorGrouping = 1, ToolTipText = "")]
	public string Name
	{
		get
		{
			return HC_0011;
		}
		set
		{
			HC_0011 = value;
		}
	}

	/// <summary>
	/// Determines the bounds used in object culling and collision.
	/// </summary>
	[EditorProperty(true, Description = "Hull Type", HorizontalAlignment = true, MajorGrouping = 5, MinorGrouping = 2, ToolTipText = "")]
	public HullType HullType
	{
		get
		{
			return HCK;
		}
		set
		{
			if (HCK != value)
			{
				HCK = value;
				_CollisionId++;
				CalculateWorldBounds(ref HC_000F, ref HCy, alreadymoved: false);
			}
		}
	}

	/// <summary>
	/// Notifies the editor that this object is partially controlled via code. The editor
	/// will display information to the user indicating some property values are
	/// overridden in code and changes may not take effect.
	/// </summary>
	[EditorProperty(false)]
	public bool AffectedInCode
	{
		[CompilerGenerated]
		get
		{
			return HCs;
		}
		[CompilerGenerated]
		set
		{
			HCs = value;
		}
	}

	/// <summary>
	/// Container that stores, manages, and updates the object's components.
	/// </summary>
	public ComponentCollection<ISceneEntity> Components => _Components;

	/// <summary>
	/// Event used to update the object at regular intervals. This and all
	/// events are only called on dynamic objects.
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
	/// Creates a new SceneEntity instance.
	/// </summary>
	/// <param name="name">Custom name for the object.</param>
	/// <param name="infinitebounds">Indicates the object bounding area spans the entire world and
	/// the object is always visible.</param>
	public SceneEntity(string name, bool infinitebounds)
	{
		Init(name, infinitebounds);
	}

	/// <summary>
	/// Creates a new SceneEntity instance.
	/// </summary>
	public SceneEntity()
	{
		Init("", infinitebounds: false);
	}

	/// <summary>
	/// Initializes the object to default values.
	/// </summary>
	/// <param name="name">Custom name for the object.</param>
	/// <param name="infinitebounds">Indicates the object bounding area spans the entire world and
	/// the object is always visible.</param>
	protected virtual void Init(string name, bool infinitebounds)
	{
		_Components = new ComponentCollection<ISceneEntity>(this);
		if (HC6 == 0)
		{
			HC6 = CoreHelper.GetUniqueId(this);
		}
		HCD = infinitebounds;
		if (!string.IsNullOrEmpty(name))
		{
			HC_0011 = name;
		}
		else
		{
			HC_0011 = string.Empty;
		}
		CalculateBounds();
	}

	/// <summary>
	/// Deep clones the object including any contained sub-objects and components.
	/// </summary>
	/// <returns></returns>
	public virtual ISceneEntity Clone()
	{
		ISceneEntity sceneEntity = Create();
		if (HCD && sceneEntity is SceneEntity sceneEntity2)
		{
			sceneEntity2.HCD = HCD;
			sceneEntity2.CalculateBounds();
		}
		Z._7._0002w(this, sceneEntity);
		foreach (IComponent<ISceneEntity> component in _Components.Components)
		{
			sceneEntity.Components.Add(component.Clone());
		}
		return sceneEntity;
	}

	/// <summary>
	/// Creates a new instance of the object type. This method assumes the type has a
	/// default constructor. If the type does not have a default constructor this method
	/// can be overridden to manually create the type.
	/// </summary>
	/// <returns></returns>
	protected virtual ISceneEntity Create()
	{
		return (ISceneEntity)Activator.CreateInstance(GetType());
	}

	/// <summary>
	/// Updates the object using the provided game time.
	/// </summary>
	/// <param name="gametime"></param>
	public virtual void Update(GameTime gametime)
	{
		_Components.OnUpdate(gametime);
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
		HCH.Add(manager.ManagerType, manager);
		_Components.OnSubmittedToManager(manager);
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
		_Components.OnRemovedFromManager(manager);
		if (HC_0012 != null)
		{
			HC_0012(manager);
		}
		HCH.Remove(manager.ManagerType);
	}

	/// <summary>
	/// Called when the object is created in the SunBurn editor.
	/// </summary>
	public virtual void OnCreatedInEditor()
	{
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
		if (!HCw.Equals(world))
		{
			UpdateWorldAndWorldToObject(ref world, ref worldtoobj);
		}
	}

	/// <summary>
	/// Updates the object world space and inverse world space transforms.
	/// Override to perform custom code when the world transform changes.
	/// </summary>
	/// <param name="world">World space transform.</param>
	/// <param name="worldtoobj">Inverse world space transform.</param>
	protected virtual void UpdateWorldAndWorldToObject(ref Matrix world, ref Matrix worldtoobj)
	{
		HCw = world;
		HCZ = worldtoobj;
		HC7++;
		CalculateWorldBounds(ref HC_000F, ref HCy, alreadymoved: true);
	}

	/// <summary>
	/// Recalculates the object bounding area based on all contained meshes.
	///
	/// Calling this method may become necessary if a mesh bounding area is
	/// altered after being added to the object.
	/// </summary>
	public void CalculateBounds()
	{
		CalculateObjectBounds(ref HCk, ref HC_0003);
		CalculateWorldBounds(ref HC_000F, ref HCy, alreadymoved: false);
	}

	/// <summary>
	/// Calculates the object bounds.
	/// </summary>
	/// <param name="objectboundingbox">Object bounds to update.</param>
	/// <param name="objectboundingsphere">Object bounds to update.</param>
	protected virtual void CalculateObjectBounds(ref BoundingBox objectboundingbox, ref BoundingSphere objectboundingsphere)
	{
		if (HCD)
		{
			float num = 3.4028235E+37f;
			objectboundingbox = new BoundingBox(new Vector3(0f - num), new Vector3(num));
			objectboundingsphere = new BoundingSphere(Vector3.Zero, num);
		}
		else
		{
			objectboundingbox = new BoundingBox(-Vector3.One, Vector3.One);
			objectboundingsphere = new BoundingSphere(Vector3.Zero, 1f);
		}
	}

	/// <summary>
	/// Updates the object world bounds based on the current world transform and object space bounds.
	///
	/// NOTE: when implementing custom bounds ensure the hull type (box or sphere) is completely
	/// enclosed by the other bounds type. For instance if the hull type is Box then the bounding
	/// sphere should completely contain the bounding box, and vice-versa. This is critical for
	/// correct collision.
	/// </summary>
	/// <param name="worldboundingbox">World bounds to update.</param>
	/// <param name="worldboundingsphere">World bounds to update.</param>
	/// <param name="alreadymoved">Indicates the object move id is already updated.</param>
	protected virtual void CalculateWorldBounds(ref BoundingBox worldboundingbox, ref BoundingSphere worldboundingsphere, bool alreadymoved)
	{
		if (HCD)
		{
			float num = 3.4028235E+37f;
			worldboundingbox = new BoundingBox(new Vector3(0f - num), new Vector3(num));
			worldboundingsphere = new BoundingSphere(Vector3.Zero, num);
			return;
		}
		BoundingBox boundingBox = worldboundingbox;
		if (HCK == HullType.Box)
		{
			worldboundingbox = CoreHelper.TransformBoundingBox(HCk, HCw);
			BoundingSphere.CreateFromBoundingBox(ref worldboundingbox, out worldboundingsphere);
		}
		else if (HCK == HullType.Sphere)
		{
			CoreHelper.TransformBoundingSphere(ref HC_0003, ref HCw, out worldboundingsphere);
			BoundingBox.CreateFromSphere(ref worldboundingsphere, out worldboundingbox);
		}
		else
		{
			CoreHelper.TransformBoundingSphere(ref HC_0003, ref HCw, out worldboundingsphere);
			worldboundingbox = CoreHelper.TransformBoundingBox(HCk, HCw);
		}
		if (!alreadymoved && !boundingBox.Equals(worldboundingbox))
		{
			HC7++;
		}
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

	internal void _7k(SerializationInfo P_0)
	{
		SerializationHelper.DeserializeField(ref HCD, P_0, "InfiniteBounds", usedefault: true);
		SerializationHelper.DeserializeField(ref HC_0011, P_0, "Name", usedefault: true);
		SerializationHelper.DeserializeField(ref HC6, P_0, "UniqueId", usedefault: false);
		SerializationHelper.DeserializeEnum(ref HCK, P_0, "HullType", isflag: false);
		HC_0001 = R._7._7o(P_0);
		Matrix field = default(Matrix);
		SerializationHelper.DeserializeField(ref field, P_0, "World", usedefault: true);
		Init(HC_0011, HCD);
		World = field;
	}

	/// <summary>
	/// Deserializes object data from the provided SerializationInfo.
	/// </summary>
	/// <param name="info">Contains the serialized object data.</param>
	/// <param name="context"></param>
	public virtual void SetObjectData(SerializationInfo info, StreamingContext context)
	{
		_7k(info);
		_Components.SetObjectData(info, context);
	}

	/// <summary>
	/// Serializes object data to the provided SerializationInfo.
	/// </summary>
	/// <param name="info">SerializationInfo to store the serialized data.</param>
	/// <param name="context"></param>
	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("InfiniteBounds", HCD);
		info.AddValue("Name", HC_0011);
		info.AddValue("UpdateType", HC_0001);
		info.AddValue("UniqueId", HC6);
		info.AddValue("World", HCw);
		info.AddValue("HullType", HCK);
		_Components.GetObjectData(info, context);
	}
}
