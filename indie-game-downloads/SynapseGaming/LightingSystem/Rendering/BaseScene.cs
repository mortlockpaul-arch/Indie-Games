using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using _0003;
using SynapseGaming.LightingSystem.Audio;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Serialization;
using X;
using l;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Base container object used for storing, sharing, and organizing scene entities, objects, and lights.
/// </summary>
[Serializable]
public abstract class BaseScene : IScene, IEditorActiveObject, IEditorObject, INamedObject, IDisposable, _0003._0017, IFullSerializable, ISerializable
{
	private string _3A_0018 = "";

	private string _3AL = "";

	private string _3A_0019 = "";

	/// <summary />
	protected List<ILightGroup> _LightGroups = new List<ILightGroup>(16);

	/// <summary />
	protected List<ISceneEntityGroup> _EntityGroups = new List<ISceneEntityGroup>(16);

	private bool _3A3;

	private X._0018<IAudioManager, AudioSource> _3A6 = new X._0018<IAudioManager, AudioSource>();

	private X._0018<IObjectManager, SceneEntity> _3AD = new X._0018<IObjectManager, SceneEntity>();

	private X._0018<ILightManager, BaseLight> _3A_0017 = new X._0018<ILightManager, BaseLight>();

	[CompilerGenerated]
	private bool _3A_0003;

	[CompilerGenerated]
	private bool _3Al;

	/// <summary>
	/// Light groups contained by the scene.
	/// </summary>
	public List<ILightGroup> LightGroups => _LightGroups;

	/// <summary>
	/// Scene object groups contained by the scene.
	/// </summary>
	public List<ISceneEntityGroup> EntityGroups => _EntityGroups;

	/// <summary>
	/// The object's current name.
	/// </summary>
	public string Name
	{
		get
		{
			return _3A_0018;
		}
		set
		{
		}
	}

	/// <summary>
	/// Notifies the editor that the object is currently used for rendering. The editor
	/// will display unused / inactive objects as grayed-out.
	/// </summary>
	public bool AssetInUse
	{
		[CompilerGenerated]
		get
		{
			return _3A_0003;
		}
		[CompilerGenerated]
		internal set
		{
			_3A_0003 = flag;
		}
	}

	/// <summary>
	/// Notifies the editor that the object is partially controlled via code. The editor
	/// will display information to the user indicating some property values are
	/// overridden in code and changes may not take effect.
	/// </summary>
	public bool AffectedInCode
	{
		[CompilerGenerated]
		get
		{
			return _3Al;
		}
		[CompilerGenerated]
		set
		{
			_3Al = value;
		}
	}

	internal string FileName
	{
		get
		{
			return _3AL;
		}
		set
		{
			_3AL = text;
		}
	}

	internal string ProjectFile
	{
		get
		{
			return _3A_0019;
		}
		set
		{
			_3A_0019 = text;
		}
	}

	string _0003._0017.ProjectFile => _3A_0019;

	internal void Lk(string P_0)
	{
		_3A_0018 = P_0;
	}

	/// <summary>
	/// Creates a BaseScene instance.
	/// </summary>
	public BaseScene()
		: this(true)
	{
	}

	/// <summary>
	/// Creates a BaseScene instance.
	/// </summary>
	protected BaseScene(bool P_0)
	{
		if (P_0)
		{
			SunBurnEditor.OnCreateResource(this);
		}
	}

	/// <summary>
	/// Releases resources allocated by this object.
	/// </summary>
	public void Dispose()
	{
		Clear();
		if (!_3A3)
		{
			_3A3 = true;
			SunBurnEditor.OnDisposeResource(this);
		}
	}

	/// <summary>
	/// Sets the single manager of the specified type that contains this scene.
	///
	/// Scenes can only be contained by a single manager of a specific type.
	/// </summary>
	/// <typeparam name="T">Type of the specified manager. This is often the
	/// manager interface type, not the class type.</typeparam>
	/// <param name="manager">Manager object that contains the scene.</param>
	public void SetContainingManager<T>(IManager manager)
	{
		if ((object)typeof(T) == typeof(IAudioManager))
		{
			_3A6.SetManager(manager as IAudioManager);
		}
		else if ((object)typeof(T) == typeof(IObjectManager))
		{
			_3AD.SetManager(manager as IObjectManager);
		}
		else if ((object)typeof(T) == typeof(ILightManager))
		{
			_3A_0017.SetManager(manager as ILightManager);
		}
		AssetInUse = _3A6.ContainingManager != null || _3AD.ContainingManager != null || _3A_0017.ContainingManager != null;
		SunBurnEditor._0019q(this, l._0019.InUse);
	}

	/// <summary>
	/// Removes all objects and groups.
	/// </summary>
	public void Clear()
	{
		_LightGroups.Clear();
		_EntityGroups.Clear();
		_3A6.RemoveSubmittedObjects();
		_3AD.RemoveSubmittedObjects();
		_3A_0017.RemoveSubmittedObjects();
	}

	/// <summary>
	/// Applies changes made to contained objects and groups.  This must be called after
	/// making changes and before rendering the scene.
	/// </summary>
	public void Apply()
	{
		_3A6.RemoveSubmittedObjects();
		_3AD.RemoveSubmittedObjects();
		_3A_0017.RemoveSubmittedObjects();
		foreach (ISceneEntityGroup entityGroup in _EntityGroups)
		{
			for (int i = 0; i < entityGroup.Entities.Count; i++)
			{
				ISceneEntity sceneEntity = entityGroup.Entities[i];
				if (sceneEntity is SceneEntity)
				{
					_3AD.Submit(sceneEntity as SceneEntity);
				}
				else if (sceneEntity is AudioSource)
				{
					_3A6.Submit(sceneEntity as AudioSource);
				}
			}
		}
		foreach (ILightGroup lightGroup3 in _LightGroups)
		{
			if (lightGroup3.ShadowGroup && lightGroup3.ShadowRenderLightsTogether)
			{
				int num = 0;
				ILightGroup lightGroup = lightGroup3;
				for (int j = 0; j < lightGroup3.Lights.Count; j++)
				{
					if (num >= SunBurnCoreSystem.MaxLightsPerGroup)
					{
						LightGroup lightGroup2 = new LightGroup();
						lightGroup2._3v(lightGroup);
						lightGroup = lightGroup2;
						num = 0;
					}
					ILight light = lightGroup3.Lights[j];
					if (light is BaseLight obj)
					{
						light.ShadowSource = lightGroup;
						_3A_0017.Submit(obj);
						num++;
					}
				}
				continue;
			}
			for (int k = 0; k < lightGroup3.Lights.Count; k++)
			{
				if (lightGroup3.Lights[k] is BaseLight obj2)
				{
					_3A_0017.Submit(obj2);
				}
			}
		}
		_3A6.Optimize();
		_3AD.Optimize();
		_3A_0017.Optimize();
	}

	/// <summary>
	/// Deserializes object data from the provided SerializationInfo.
	/// </summary>
	/// <param name="info">Contains the serialized object data.</param>
	/// <param name="context"></param>
	public abstract void SetObjectData(SerializationInfo info, StreamingContext context);

	/// <summary>
	/// Serializes object data to the provided SerializationInfo.
	/// </summary>
	/// <param name="info">SerializationInfo to store the serialized data.</param>
	/// <param name="context"></param>
	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public abstract void GetObjectData(SerializationInfo info, StreamingContext context);
}
