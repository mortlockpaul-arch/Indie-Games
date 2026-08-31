using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using _000F;
using SynapseGaming.LightingSystem.Audio;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Serialization;
using Z;
using u;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Base container object used for storing, sharing, and organizing scene entities, objects, and lights.
/// </summary>
[Serializable]
public abstract class BaseScene : IScene, IEditorActiveObject, IEditorObject, INamedObject, IDisposable, Z.w, IFullSerializable, ISerializable
{
	private string HCB = "";

	private string HC_0002 = "";

	private string HC_0012 = "";

	/// <summary />
	protected List<ILightGroup> _LightGroups = new List<ILightGroup>(16);

	/// <summary />
	protected List<ISceneEntityGroup> _EntityGroups = new List<ISceneEntityGroup>(16);

	private bool HCH;

	private u.B<IAudioManager, AudioSource> HC7 = new u.B<IAudioManager, AudioSource>();

	private u.B<IObjectManager, SceneEntity> HC_0001 = new u.B<IObjectManager, SceneEntity>();

	private u.B<ILightManager, BaseLight> HCw = new u.B<ILightManager, BaseLight>();

	[CompilerGenerated]
	private bool HCZ;

	[CompilerGenerated]
	private bool HC_000F;

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
			return HCB;
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
			return HCZ;
		}
		[CompilerGenerated]
		internal set
		{
			HCZ = hCZ;
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
			return HC_000F;
		}
		[CompilerGenerated]
		set
		{
			HC_000F = value;
		}
	}

	internal string FileName
	{
		get
		{
			return HC_0002;
		}
		set
		{
			HC_0002 = text;
		}
	}

	internal string ProjectFile
	{
		get
		{
			return HC_0012;
		}
		set
		{
			HC_0012 = text;
		}
	}

	string Z.w.ProjectFile => HC_0012;

	internal void _0002N(string P_0)
	{
		HCB = P_0;
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
		if (!HCH)
		{
			HCH = true;
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
			HC7.SetManager(manager as IAudioManager);
		}
		else if ((object)typeof(T) == typeof(IObjectManager))
		{
			HC_0001.SetManager(manager as IObjectManager);
		}
		else if ((object)typeof(T) == typeof(ILightManager))
		{
			HCw.SetManager(manager as ILightManager);
		}
		AssetInUse = HC7.ContainingManager != null || HC_0001.ContainingManager != null || HCw.ContainingManager != null;
		SunBurnEditor._0012X(this, _000F._0012.InUse);
	}

	/// <summary>
	/// Removes all objects and groups.
	/// </summary>
	public void Clear()
	{
		_LightGroups.Clear();
		_EntityGroups.Clear();
		HC7.RemoveSubmittedObjects();
		HC_0001.RemoveSubmittedObjects();
		HCw.RemoveSubmittedObjects();
	}

	/// <summary>
	/// Applies changes made to contained objects and groups.  This must be called after
	/// making changes and before rendering the scene.
	/// </summary>
	public void Apply()
	{
		HC7.RemoveSubmittedObjects();
		HC_0001.RemoveSubmittedObjects();
		HCw.RemoveSubmittedObjects();
		foreach (ISceneEntityGroup entityGroup in _EntityGroups)
		{
			for (int i = 0; i < entityGroup.Entities.Count; i++)
			{
				ISceneEntity sceneEntity = entityGroup.Entities[i];
				if (sceneEntity is SceneEntity)
				{
					HC_0001.Submit(sceneEntity as SceneEntity);
				}
				else if (sceneEntity is AudioSource)
				{
					HC7.Submit(sceneEntity as AudioSource);
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
						lightGroup2.Hh(lightGroup);
						lightGroup = lightGroup2;
						num = 0;
					}
					ILight light = lightGroup3.Lights[j];
					if (light is BaseLight obj)
					{
						light.ShadowSource = lightGroup;
						HCw.Submit(obj);
						num++;
					}
				}
				continue;
			}
			for (int k = 0; k < lightGroup3.Lights.Count; k++)
			{
				if (lightGroup3.Lights[k] is BaseLight obj2)
				{
					HCw.Submit(obj2);
				}
			}
		}
		HC7.Optimize();
		HC_0001.Optimize();
		HCw.Optimize();
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
