using System;
using System.Collections.Generic;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Base manager used for generating light maps.
/// </summary>
public abstract class BaseLightMapManager : ILightMapManager, IManagerService, IManager, IUnloadable
{
	private int _3A_0018 = 30;

	private IManagerServiceProvider _3AL;

	private Dictionary<int, LightMap> _3A_0019 = new Dictionary<int, LightMap>();

	private Dictionary<int, LightOcclusionBuffer> _3A3 = new Dictionary<int, LightOcclusionBuffer>();

	/// <summary>
	/// Gets the manager specific Type used as a unique key for storing and
	/// requesting the manager from the IManagerServiceProvider.
	/// </summary>
	public Type ManagerType => typeof(ILightMapManager);

	/// <summary>
	/// Sets the order this manager is processed relative to other managers
	/// in the IManagerServiceProvider. Managers with lower processing order
	/// values are processed first.
	///
	/// In the case of BeginFrameRendering and EndFrameRendering, BeginFrameRendering
	/// is processed in the normal order (lowest order value to highest), however
	/// EndFrameRendering is processed in reverse order (highest to lowest) to ensure
	/// the first manager begun is the last one ended (FILO).
	/// </summary>
	public int ManagerProcessOrder
	{
		get
		{
			return _3A_0018;
		}
		set
		{
			_3A_0018 = value;
		}
	}

	/// <summary>
	/// Scene interface the manager was created by, or assigned during construction.
	/// </summary>
	public IManagerServiceProvider OwnerSceneInterface => _3AL;

	internal Dictionary<int, LightMap> MeshLightMaps => _3A_0019;

	internal Dictionary<int, LightOcclusionBuffer> LightOcclusionBuffers => _3A3;

	/// <summary>
	/// Creates a new BaseLightMapManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	public BaseLightMapManager(IManagerServiceProvider sceneinterface)
	{
		_3AL = sceneinterface;
	}

	/// <summary>
	/// Use to apply user quality and performance preferences to the resources managed by this object.
	/// </summary>
	/// <param name="preferences"></param>
	public void ApplyPreferences(ISystemPreferences preferences)
	{
	}

	/// <summary>
	/// Removes all objects from the container. Commonly used while clearing the scene.
	/// </summary>
	public virtual void Clear()
	{
	}

	/// <summary>
	/// Disposes any graphics resource used internally by this object, and removes
	/// scene resources managed by this object. Commonly used during Game.UnloadContent.
	/// </summary>
	public virtual void Unload()
	{
		foreach (KeyValuePair<int, LightMap> item in _3A_0019)
		{
			item.Value?.Dispose();
		}
		_3A_0019.Clear();
		_3A3.Clear();
	}

	/// <summary>
	/// Set the light map associated with the provided renderable mesh unique id.
	/// </summary>
	/// <param name="meshuniqueid"></param>
	/// <param name="lightmap"></param>
	public abstract void SetLightMap(int meshuniqueid, LightMap lightmap);

	/// <summary>
	/// Gets the light map associated with the provided mesh or a default light map if non exists.
	/// </summary>
	/// <param name="mesh"></param>
	/// <returns></returns>
	public abstract LightMap GetLightMap(RenderableMesh mesh);

	/// <summary>
	/// Gets the light occlusion buffer associated with the provided light or null if non exists.
	/// </summary>
	/// <param name="light"></param>
	/// <returns></returns>
	public abstract LightOcclusionBuffer GetLightOcclusionBuffer(ILight light);
}
