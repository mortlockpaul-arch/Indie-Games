using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using SynapseGaming.LightingSystem.Components;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Rendering;
using SynapseGaming.LightingSystem.Serialization;
using Z;

namespace SynapseGaming.LightingSystem.Audio;

/// <summary>
/// Provides an audio emitter which is capable of emitting 3D sound from a specific
/// location, or ambient sound heard equally from everywhere in the scene.
/// </summary>
[Serializable]
[EditorCreatedObject]
public class AudioSource : IAudioSource, ISceneEntity, IMovableObject, IWorldBoundingBoxObject, IComponentObject<ISceneEntity>, IEditorCreatedObject<ISceneEntity>, IEditorObject, INamedObject, IEditorRenderableObject, IPointSource, IFullSerializable, ISerializable
{
	private UpdateDelegate HCB;

	private SubmitRemoveManagerDelegate HC_0002;

	private SubmitRemoveManagerDelegate HC_0012;

	private TypeDictionary<IManagerService> HCH = new TypeDictionary<IManagerService>();

	private bool HC7 = true;

	private bool HC_0001 = true;

	private int HCw;

	private int HCZ;

	private float HC_000F = 1f;

	private float HCy = 1f;

	private string HC6 = string.Empty;

	private UpdateType HCD;

	private AudioType HC_0011;

	private Matrix HCK = Matrix.Identity;

	private BoundingBox HC_0003;

	private BoundingSphere HCk;

	private BoundingBox HCs;

	private BoundingSphere HC_0013;

	private SoundEffectAsset HCX = SoundEffectAsset.Empty;

	private ComponentCollection<ISceneEntity> HCz;

	private static Vector3[] HCA = new Vector3[7];

	private static readonly Vector3[] HCc = new Vector3[7]
	{
		new Vector3(0.577f, 0.577f, 0.577f),
		new Vector3(0.577f, -0.577f, 0.577f),
		new Vector3(0.577f, 0.577f, -0.577f),
		new Vector3(0.577f, -0.577f, -0.577f),
		new Vector3(1f, 0f, 0f),
		new Vector3(0f, 1f, 0f),
		new Vector3(0f, 0f, 1f)
	};

	[CompilerGenerated]
	private AudioState HCY;

	[CompilerGenerated]
	private SoundEffect HCV;

	[CompilerGenerated]
	private bool HCu;

	[CompilerGenerated]
	private bool HCq;

	/// <summary>
	/// Dictionary of all managers the object is currently contained in (submitted to).
	///
	/// Managers are accessible by their ManagerType and only one manager of a
	/// particular type can be contained in the dictionary at a time.
	/// </summary>
	public TypeDictionary<IManagerService> ContainingManagers => HCH;

	/// <summary>
	/// Determines if the sound will repeat after completing.
	/// </summary>
	[EditorProperty(true, Description = "Looping Sound", HorizontalAlignment = true, MajorGrouping = 4, MinorGrouping = 1, ToolTipText = "")]
	public bool Loop
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
	/// Determines how loud the sound is.
	/// </summary>
	[EditorProperty(true, Description = "Volume", HorizontalAlignment = true, MajorGrouping = 5, MinorGrouping = 1, ToolTipText = "")]
	[EditorNumberPadOptions(2, 0.0, 1.0, 0.05)]
	public float Volume
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
	/// Determines how the sound changes in relationship to the viewer. Ambient sounds
	/// are heard equally from everywhere in the scene, whereas 3D sounds are relative
	/// to the viewer / listener.
	/// </summary>
	[EditorProperty(true, Description = "Audio Type", HorizontalAlignment = true, MajorGrouping = 4, MinorGrouping = 3, ToolTipText = "")]
	public AudioType AudioType
	{
		get
		{
			return HC_0011;
		}
		set
		{
			HC_0011 = value;
			UpdateBounds();
		}
	}

	/// <summary>
	/// Determines if the sound automatically begins playing when the emitter is loaded
	/// as part of a scene. If the sound is not automatically played it will need to be triggered
	/// using the Play() method.
	/// </summary>
	[EditorProperty(true, Description = "Play When Loaded", HorizontalAlignment = true, MajorGrouping = 4, MinorGrouping = 2, ToolTipText = "")]
	public bool PlayWhenLoaded
	{
		get
		{
			return HC_0001;
		}
		set
		{
			HC_0001 = value;
			if (HC_0001)
			{
				AudioState = AudioState.Playing;
			}
			else
			{
				AudioState = AudioState.Stopped;
			}
		}
	}

	/// <summary>
	/// Determines if the sound is currently playing.
	/// </summary>
	[EditorProperty(false)]
	public AudioState AudioState
	{
		[CompilerGenerated]
		get
		{
			return HCY;
		}
		[CompilerGenerated]
		set
		{
			HCY = value;
		}
	}

	/// <summary>
	/// The SoundEffect used by the emitter to play sounds. This is either
	/// the sound loaded by the SoundEffectAsset or the sound passed into the constructor
	/// depending on how the object was initialized.
	/// </summary>
	[EditorProperty(false)]
	public SoundEffect SoundEffect
	{
		[CompilerGenerated]
		get
		{
			return HCV;
		}
		[CompilerGenerated]
		private set
		{
			HCV = hCV;
		}
	}

	/// <summary>
	/// Provides direct access to the repository name, file name, and sound
	/// the object was created from. Only valid for serialized objects
	/// created via the SunBurn editor.
	/// </summary>
	[EditorProperty(true, Description = "Sound Effect", HorizontalAlignment = true, MajorGrouping = 1, MinorGrouping = 2, ToolTipText = "")]
	public SoundEffectAsset SoundEffectAsset
	{
		get
		{
			return HCX;
		}
		set
		{
			if (value != null)
			{
				HCX = value;
			}
			else
			{
				HCX = SoundEffectAsset.Empty;
			}
			SoundEffect = HCX.Asset;
		}
	}

	/// <summary>
	/// Position in world space of the source.
	/// </summary>
	[EditorProperty(false)]
	public Vector3 Position
	{
		get
		{
			return World.Translation;
		}
		set
		{
			Matrix world = World;
			world.Translation = value;
			SetWorldAndWorldToObject(world, Matrix.Identity);
		}
	}

	/// <summary>
	/// Maximum distance in world space of the source's influence.
	/// </summary>
	[EditorNumberPadOptions(2, 0.0, 2147483647.0, 0.25)]
	[EditorProperty(true, Description = "Sound Radius", HorizontalAlignment = true, MajorGrouping = 5, MinorGrouping = 2, ToolTipText = "")]
	public float Radius
	{
		get
		{
			return HC_000F;
		}
		set
		{
			if (HC_000F != value)
			{
				HC_000F = value;
				UpdateBounds();
			}
		}
	}

	/// <summary>
	/// Object bounding area of the source's influence.
	/// </summary>
	public BoundingBox ObjectBoundingBox => HC_0003;

	/// <summary>
	/// Object bounding area of the source's influence.
	/// </summary>
	public BoundingSphere ObjectBoundingSphere => HCk;

	/// <summary>
	/// World bounding area of the source's influence.
	/// </summary>
	public BoundingBox WorldBoundingBox => HCs;

	/// <summary>
	/// World bounding area of the source's influence.
	/// </summary>
	public BoundingSphere WorldBoundingSphere => HC_0013;

	/// <summary>
	/// World space transform of the source.
	/// </summary>
	public Matrix World
	{
		get
		{
			return HCK;
		}
		set
		{
			SetWorldAndWorldToObject(value, Matrix.Identity);
		}
	}

	/// <summary>
	/// Indicates the object bounding area spans the entire world and
	/// the object is always visible.
	/// </summary>
	[EditorProperty(false)]
	public bool InfiniteBounds
	{
		[CompilerGenerated]
		get
		{
			return HCu;
		}
		[CompilerGenerated]
		private set
		{
			HCu = hCu;
		}
	}

	/// <summary>
	/// Indicates the current move. This value increments each time the object
	/// is moved (when the World transform changes).
	/// </summary>
	[EditorProperty(false)]
	public int MoveId => HCw;

	/// <summary>
	/// Unique id used to identify the object across multiple scene loads / reloads.
	/// </summary>
	[EditorProperty(false)]
	public int UniqueId => HCZ;

	/// <summary>
	/// Determines the bounds used for emitter culling (always returns HullType.Box).
	/// </summary>
	[EditorProperty(false)]
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
	/// Determines if objects receive update events from the engine and are tracked
	/// by the scenegraph.
	///
	/// Automatic update events are necessary to be affected by gravity, for
	/// components, and for the scenegraph to track moving objects.  Objects without
	/// Automatic update events can still move, however the containing scenegraph
	/// (ObjectManager or LightManager) must be notified using Manager.Move(object).
	/// </summary>
	[EditorProperty(true, Description = "Receives Updates", HorizontalAlignment = true, MajorGrouping = 2, MinorGrouping = 1, ControlType = ControlType.CheckBox)]
	[EditorCheckboxOptions(true)]
	public UpdateType UpdateType
	{
		get
		{
			return HCD;
		}
		set
		{
			HCD = value;
		}
	}

	/// <summary>
	/// The object's current name.
	///
	/// Important note: Name can be changed at any time, HOWEVER managers
	/// will only see the change after removing and resubmitting the object.
	/// </summary>
	[EditorProperty(true, Description = "Name", HorizontalAlignment = true, MajorGrouping = 1, MinorGrouping = 1)]
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
	[EditorProperty(false)]
	public bool AffectedInCode
	{
		[CompilerGenerated]
		get
		{
			return HCq;
		}
		[CompilerGenerated]
		set
		{
			HCq = value;
		}
	}

	/// <summary>
	/// Container that stores, manages, and updates the object's components.
	/// </summary>
	public ComponentCollection<ISceneEntity> Components => HCz;

	/// <summary>
	/// Event used to update the source at regular intervals. This and all
	/// events are only called on automatic source.
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
	/// Creates a new AudioSource instance.
	/// </summary>
	public AudioSource()
		: this(null)
	{
		AudioState = AudioState.Playing;
	}

	/// <summary>
	/// Creates a new AudioSource instance.
	/// </summary>
	/// <param name="sound">The SoundEffect used by the emitter to play sounds.</param>
	public AudioSource(SoundEffect sound)
	{
		SoundEffect = sound;
		AudioState = AudioState.Stopped;
		HCz = new ComponentCollection<ISceneEntity>(this);
		if (HCZ == 0)
		{
			HCZ = CoreHelper.GetUniqueId(this);
		}
		UpdateBounds();
	}

	/// <summary>
	/// Updates the object using the provided game time.
	/// </summary>
	/// <param name="gametime"></param>
	public virtual void Update(GameTime gametime)
	{
		HCz.OnUpdate(gametime);
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
		HCz.OnSubmittedToManager(manager);
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
		HCz.OnRemovedFromManager(manager);
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
	public virtual void SetWorldAndWorldToObject(Matrix world, Matrix worldtoobj)
	{
		if (!HCK.Equals(world))
		{
			HCK = world;
			UpdateBounds();
		}
	}

	/// <summary>
	/// Recalculates the emitter bounds based on the audio type, position, and radius.
	/// </summary>
	protected virtual void UpdateBounds()
	{
		if (InfiniteBounds = HC_0011 == AudioType.Ambient)
		{
			float num = 3.4028235E+37f;
			HC_0003 = new BoundingBox(new Vector3(0f - num), new Vector3(num));
			HCk = new BoundingSphere(Vector3.Zero, num);
			HCs = HC_0003;
			HC_0013 = HCk;
		}
		else
		{
			Vector3 vector = new Vector3(HC_000F, HC_000F, HC_000F);
			Vector3 translation = HCK.Translation;
			HC_0003 = new BoundingBox(-vector, vector);
			HCk = new BoundingSphere(Vector3.Zero, HC_000F);
			HCs = new BoundingBox(translation - vector, translation + vector);
			HC_0013 = new BoundingSphere(HCK.Translation, HC_000F);
		}
		HCw++;
	}

	/// <summary>
	/// Starts playing the contained sound from the beginning.
	/// </summary>
	public virtual void Play()
	{
		AudioState = AudioState.Playing;
	}

	/// <summary>
	/// Stops playing the contained sound.
	/// </summary>
	public virtual void Stop()
	{
		AudioState = AudioState.Stopped;
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
	public virtual ISceneEntity Clone()
	{
		ISceneEntity sceneEntity = Create();
		Z._7._0002w(this, sceneEntity);
		foreach (IComponent<ISceneEntity> component in HCz.Components)
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

	/// <summary>
	/// Deserializes object data from the provided SerializationInfo.
	/// </summary>
	/// <param name="info">Contains the serialized object data.</param>
	/// <param name="context"></param>
	public virtual void SetObjectData(SerializationInfo info, StreamingContext context)
	{
		SerializationHelper.DeserializeField(ref HCZ, info, "UniqueId", usedefault: false);
		SerializationHelper.DeserializeField(ref HC6, info, "Name", usedefault: true);
		SerializationHelper.DeserializeField(ref HC7, info, "Loop", usedefault: true);
		SerializationHelper.DeserializeField(ref HC_0001, info, "PlayWhenLoaded", usedefault: true);
		SerializationHelper.DeserializeField(ref HCy, info, "Volume", usedefault: true);
		SerializationHelper.DeserializeField(ref HC_000F, info, "Radius", usedefault: true);
		SerializationHelper.DeserializeEnum(ref HC_0011, info, "AudioType", isflag: false);
		SerializationHelper.DeserializeEnum(ref HCD, info, "UpdateType", isflag: false);
		SerializationHelper.DeserializeField(ref HCK, info, "World", usedefault: true);
		string field = string.Empty;
		string field2 = string.Empty;
		SerializationHelper.DeserializeField(ref field, info, "ContentRepositoryName", usedefault: true);
		SerializationHelper.DeserializeField(ref field2, info, "SoundEffectFile", usedefault: true);
		SoundEffectAsset = new SoundEffectAsset(field, field2);
		UpdateBounds();
		HCz.SetObjectData(info, context);
		if (HC_0001)
		{
			AudioState = AudioState.Playing;
		}
	}

	/// <summary>
	/// Serializes object data to the provided SerializationInfo.
	/// </summary>
	/// <param name="info">SerializationInfo to store the serialized data.</param>
	/// <param name="context"></param>
	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		SerializationHelper.SerializeFieldOrEnum(ref HCZ, info, "UniqueId");
		SerializationHelper.SerializeFieldOrEnum(ref HC6, info, "Name");
		SerializationHelper.SerializeFieldOrEnum(ref HC7, info, "Loop");
		SerializationHelper.SerializeFieldOrEnum(ref HC_0001, info, "PlayWhenLoaded");
		SerializationHelper.SerializeFieldOrEnum(ref HCy, info, "Volume");
		SerializationHelper.SerializeFieldOrEnum(ref HC_000F, info, "Radius");
		SerializationHelper.SerializeFieldOrEnum(ref HC_0011, info, "AudioType");
		SerializationHelper.SerializeFieldOrEnum(ref HCD, info, "UpdateType");
		SerializationHelper.SerializeFieldOrEnum(ref HCK, info, "World");
		string field = HCX.ContentRepositoryName;
		string field2 = HCX.SourceAssetFilePath;
		SerializationHelper.SerializeFieldOrEnum(ref field, info, "ContentRepositoryName");
		SerializationHelper.SerializeFieldOrEnum(ref field2, info, "SoundEffectFile");
		HCz.GetObjectData(info, context);
	}
}
