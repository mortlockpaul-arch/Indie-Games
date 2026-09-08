using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using _0003;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using SynapseGaming.LightingSystem.Components;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Rendering;
using SynapseGaming.LightingSystem.Serialization;

namespace SynapseGaming.LightingSystem.Audio;

/// <summary>
/// Provides an audio emitter which is capable of emitting 3D sound from a specific
/// location, or ambient sound heard equally from everywhere in the scene.
/// </summary>
[Serializable]
[EditorCreatedObject]
public class AudioSource : IAudioSource, ISceneEntity, IMovableObject, IWorldBoundingBoxObject, IComponentObject<ISceneEntity>, IEditorCreatedObject<ISceneEntity>, IEditorObject, INamedObject, IEditorRenderableObject, IPointSource, IFullSerializable, ISerializable
{
	private UpdateDelegate _3A_0018;

	private SubmitRemoveManagerDelegate _3AL;

	private SubmitRemoveManagerDelegate _3A_0019;

	private TypeDictionary<IManagerService> _3A3 = new TypeDictionary<IManagerService>();

	private bool _3A6 = true;

	private bool _3AD = true;

	private int _3A_0017;

	private int _3A_0003;

	private float _3Al = 1f;

	private float _3At = 1f;

	private float _3AF = 1f;

	private string _3Ac = string.Empty;

	private UpdateType _3Ag;

	private AudioType _3AI;

	private Matrix _3A8 = Matrix.Identity;

	private BoundingBox _3AZ;

	private BoundingSphere _3Ax;

	private BoundingBox _3Aq;

	private BoundingSphere _3Ab;

	private SoundEffectAsset _3AT = SoundEffectAsset.Empty;

	private ComponentCollection<ISceneEntity> _3Ay;

	private static Vector3[] _3A_0015 = new Vector3[7];

	private static readonly Vector3[] _3A_0001 = new Vector3[7]
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
	private AudioState _3A7;

	[CompilerGenerated]
	private SoundEffect _3AX;

	[CompilerGenerated]
	private bool _3A_0010;

	[CompilerGenerated]
	private bool _3A_0016;

	/// <summary>
	/// Dictionary of all managers the object is currently contained in (submitted to).
	///
	/// Managers are accessible by their ManagerType and only one manager of a
	/// particular type can be contained in the dictionary at a time.
	/// </summary>
	public TypeDictionary<IManagerService> ContainingManagers => _3A3;

	/// <summary>
	/// Determines if the sound will repeat after completing.
	/// </summary>
	[EditorProperty(true, Description = "Looping Sound", HorizontalAlignment = true, MajorGrouping = 4, MinorGrouping = 1, ToolTipText = "")]
	public bool Loop
	{
		get
		{
			return _3A6;
		}
		set
		{
			_3A6 = value;
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
			return _3At;
		}
		set
		{
			_3At = value;
		}
	}

	[EditorProperty(true, Description = "Doppler Scale", HorizontalAlignment = true, MajorGrouping = 5, MinorGrouping = 3, ToolTipText = "")]
	public float DopplerScale
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
	/// Determines how the sound changes in relationship to the viewer. Ambient sounds
	/// are heard equally from everywhere in the scene, whereas 3D sounds are relative
	/// to the viewer / listener.
	/// </summary>
	[EditorProperty(true, Description = "Audio Type", HorizontalAlignment = true, MajorGrouping = 4, MinorGrouping = 3, ToolTipText = "")]
	public AudioType AudioType
	{
		get
		{
			return _3AI;
		}
		set
		{
			_3AI = value;
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
			return _3AD;
		}
		set
		{
			_3AD = value;
			if (_3AD)
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
			return _3A7;
		}
		[CompilerGenerated]
		set
		{
			_3A7 = value;
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
			return _3AX;
		}
		[CompilerGenerated]
		private set
		{
			_3AX = soundEffect;
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
			return _3AT;
		}
		set
		{
			if (value != null)
			{
				_3AT = value;
			}
			else
			{
				_3AT = SoundEffectAsset.Empty;
			}
			SoundEffect = _3AT.Asset;
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
	[EditorProperty(true, Description = "Sound Radius", HorizontalAlignment = true, MajorGrouping = 5, MinorGrouping = 2, ToolTipText = "")]
	[EditorNumberPadOptions(2, 0.0, 2147483647.0, 0.25)]
	public float Radius
	{
		get
		{
			return _3Al;
		}
		set
		{
			if (_3Al != value)
			{
				_3Al = value;
				UpdateBounds();
			}
		}
	}

	/// <summary>
	/// Object bounding area of the source's influence.
	/// </summary>
	public BoundingBox ObjectBoundingBox => _3AZ;

	/// <summary>
	/// Object bounding area of the source's influence.
	/// </summary>
	public BoundingSphere ObjectBoundingSphere => _3Ax;

	/// <summary>
	/// World bounding area of the source's influence.
	/// </summary>
	public BoundingBox WorldBoundingBox => _3Aq;

	/// <summary>
	/// World bounding area of the source's influence.
	/// </summary>
	public BoundingSphere WorldBoundingSphere => _3Ab;

	/// <summary>
	/// World space transform of the source.
	/// </summary>
	public Matrix World
	{
		get
		{
			return _3A8;
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
			return _3A_0010;
		}
		[CompilerGenerated]
		private set
		{
			_3A_0010 = flag;
		}
	}

	/// <summary>
	/// Indicates the current move. This value increments each time the object
	/// is moved (when the World transform changes).
	/// </summary>
	[EditorProperty(false)]
	public int MoveId => _3A_0017;

	/// <summary>
	/// Unique id used to identify the object across multiple scene loads / reloads.
	/// </summary>
	[EditorProperty(false)]
	public int UniqueId => _3A_0003;

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
			return _3Ag;
		}
		set
		{
			_3Ag = value;
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
			return _3Ac;
		}
		set
		{
			_3Ac = value;
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
			return _3A_0016;
		}
		[CompilerGenerated]
		set
		{
			_3A_0016 = value;
		}
	}

	/// <summary>
	/// Container that stores, manages, and updates the object's components.
	/// </summary>
	public ComponentCollection<ISceneEntity> Components => _3Ay;

	/// <summary>
	/// Event used to update the source at regular intervals. This and all
	/// events are only called on automatic source.
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
		_3Ay = new ComponentCollection<ISceneEntity>(this);
		if (_3A_0003 == 0)
		{
			_3A_0003 = CoreHelper.GetUniqueId(this);
		}
		UpdateBounds();
	}

	/// <summary>
	/// Updates the object using the provided game time.
	/// </summary>
	/// <param name="gametime"></param>
	public virtual void Update(GameTime gametime)
	{
		_3Ay.OnUpdate(gametime);
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
		_3A3.Add(manager.ManagerType, manager);
		_3Ay.OnSubmittedToManager(manager);
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
		_3Ay.OnRemovedFromManager(manager);
		if (_3A_0019 != null)
		{
			_3A_0019(manager);
		}
		_3A3.Remove(manager.ManagerType);
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
		if (!_3A8.Equals(world))
		{
			_3A8 = world;
			UpdateBounds();
		}
	}

	/// <summary>
	/// Recalculates the emitter bounds based on the audio type, position, and radius.
	/// </summary>
	protected virtual void UpdateBounds()
	{
		if (InfiniteBounds = _3AI == AudioType.Ambient)
		{
			float num = 3.4028235E+37f;
			_3AZ = new BoundingBox(new Vector3(0f - num), new Vector3(num));
			_3Ax = new BoundingSphere(Vector3.Zero, num);
			_3Aq = _3AZ;
			_3Ab = _3Ax;
		}
		else
		{
			Vector3 vector = new Vector3(_3Al, _3Al, _3Al);
			Vector3 translation = _3A8.Translation;
			_3AZ = new BoundingBox(-vector, vector);
			_3Ax = new BoundingSphere(Vector3.Zero, _3Al);
			_3Aq = new BoundingBox(translation - vector, translation + vector);
			_3Ab = new BoundingSphere(_3A8.Translation, _3Al);
		}
		_3A_0017++;
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
		_0003._6.L_0003(this, sceneEntity);
		foreach (IComponent<ISceneEntity> component in _3Ay.Components)
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
		SerializationHelper.DeserializeField(ref _3A_0003, info, "UniqueId", usedefault: false);
		SerializationHelper.DeserializeField(ref _3Ac, info, "Name", usedefault: true);
		SerializationHelper.DeserializeField(ref _3A6, info, "Loop", usedefault: true);
		SerializationHelper.DeserializeField(ref _3AD, info, "PlayWhenLoaded", usedefault: true);
		SerializationHelper.DeserializeField(ref _3At, info, "Volume", usedefault: true);
		SerializationHelper.DeserializeField(ref _3Al, info, "Radius", usedefault: true);
		SerializationHelper.DeserializeField(ref _3AF, info, "DopplerScale", usedefault: true);
		SerializationHelper.DeserializeEnum(ref _3AI, info, "AudioType", isflag: false);
		SerializationHelper.DeserializeEnum(ref _3Ag, info, "UpdateType", isflag: false);
		SerializationHelper.DeserializeField(ref _3A8, info, "World", usedefault: true);
		string field = string.Empty;
		string field2 = string.Empty;
		SerializationHelper.DeserializeField(ref field, info, "ContentRepositoryName", usedefault: true);
		SerializationHelper.DeserializeField(ref field2, info, "SoundEffectFile", usedefault: true);
		SoundEffectAsset = new SoundEffectAsset(field, field2);
		UpdateBounds();
		_3Ay.SetObjectData(info, context);
		if (_3AD)
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
		SerializationHelper.SerializeFieldOrEnum(ref _3A_0003, info, "UniqueId");
		SerializationHelper.SerializeFieldOrEnum(ref _3Ac, info, "Name");
		SerializationHelper.SerializeFieldOrEnum(ref _3A6, info, "Loop");
		SerializationHelper.SerializeFieldOrEnum(ref _3AD, info, "PlayWhenLoaded");
		SerializationHelper.SerializeFieldOrEnum(ref _3At, info, "Volume");
		SerializationHelper.SerializeFieldOrEnum(ref _3Al, info, "Radius");
		SerializationHelper.SerializeFieldOrEnum(ref _3AF, info, "DopplerScale");
		SerializationHelper.SerializeFieldOrEnum(ref _3AI, info, "AudioType");
		SerializationHelper.SerializeFieldOrEnum(ref _3Ag, info, "UpdateType");
		SerializationHelper.SerializeFieldOrEnum(ref _3A8, info, "World");
		string field = _3AT.ContentRepositoryName;
		string field2 = _3AT.SourceAssetFilePath;
		SerializationHelper.SerializeFieldOrEnum(ref field, info, "ContentRepositoryName");
		SerializationHelper.SerializeFieldOrEnum(ref field2, info, "SoundEffectFile");
		_3Ay.GetObjectData(info, context);
	}
}
