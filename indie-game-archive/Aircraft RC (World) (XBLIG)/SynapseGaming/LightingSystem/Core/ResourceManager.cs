using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Can be assigned ownership of disposable and unloadable resources, automatically
/// freeing them when the scene is unloaded.
/// </summary>
public class ResourceManager : IResourceManager, IManagerService, IUpdatableManager, IManager, IUnloadable
{
	internal delegate Effect _0001CB();

	private static bool HCB = true;

	private static Dictionary<string, Effect> HC_0002 = new Dictionary<string, Effect>();

	private static Dictionary<Type, Effect> HC_0012 = new Dictionary<Type, Effect>();

	private int HCH = 500;

	private IManagerServiceProvider HC7;

	private Dictionary<IDisposable, WeakReference> HC_0001 = new Dictionary<IDisposable, WeakReference>(32);

	private Dictionary<IUnloadable, int> HCw = new Dictionary<IUnloadable, int>(32);

	private List<IDisposable> HCZ = new List<IDisposable>(32);

	/// <summary>
	/// Determines if material effects loaded from the content pipeline are shared
	/// based on the source material file.
	///
	/// As an example models and effects that load the material "Materials\Rock.mat"
	/// will all share a single reference to the same "Materials\Rock.mat" material effect.
	/// Modifying properties on the effect will change the material properties for
	/// all models and effects referencing it.
	///
	/// Disabling shared materials emulates the behavior of SunBurn prior to version 2.0.13
	/// and standard XNA, where loading multiple models will create multiple unique copies
	/// of the same material effect.
	/// </summary>
	public static bool ShareMaterialsBetweenModels
	{
		get
		{
			return HCB;
		}
		set
		{
			HCB = value;
		}
	}

	/// <summary>
	/// Gets the manager specific Type used as a unique key for storing and
	/// requesting the manager from the IManagerServiceProvider.
	/// </summary>
	public Type ManagerType => SceneInterface.ResourceManagerType;

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
			return HCH;
		}
		set
		{
			HCH = value;
		}
	}

	/// <summary>
	/// Scene interface the manager was created by, or assigned during construction.
	/// </summary>
	public IManagerServiceProvider OwnerSceneInterface => HC7;

	internal static Effect _0002R(Type P_0, _0001CB P_1)
	{
		if (HC_0012.TryGetValue(P_0, out var value) && !value.IsDisposed)
		{
			return value;
		}
		value = P_1();
		HC_0012[P_0] = value;
		return value;
	}

	internal static Effect _0002R(string P_0, _0001CB P_1)
	{
		if (!HCB)
		{
			return P_1();
		}
		if (HC_0002.TryGetValue(P_0, out var value) && !value.IsDisposed)
		{
			return value;
		}
		value = P_1();
		HC_0002[P_0] = value;
		return value;
	}

	/// <summary>
	/// Creates a new ResourceManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	public ResourceManager(IManagerServiceProvider sceneinterface)
	{
		HC7 = sceneinterface;
	}

	/// <summary>
	/// Unused.
	/// </summary>
	/// <param name="preferences"></param>
	public void ApplyPreferences(ISystemPreferences preferences)
	{
	}

	/// <summary>
	/// Assigns ownership of the resource to the resource manager, this means the manager
	/// will handle disposing and removing (IDisposable), or unloading (IUnloadable) the
	/// resource when the scene is unloaded (when [manager].Unload() is called).
	/// </summary>
	/// <param name="resource"></param>
	public void AssignOwnership(IDisposable resource)
	{
		HC_0001[resource] = null;
	}

	/// <summary>
	/// Assigns ownership of the resource to the resource manager, this means the manager
	/// will handle disposing and removing (IDisposable), or unloading (IUnloadable) the
	/// resource when the scene is unloaded (when [manager].Unload() is called).
	/// </summary>
	/// <param name="resource"></param>
	public void AssignOwnership(IUnloadable resource)
	{
		HCw[resource] = 0;
	}

	/// <summary>
	/// Assigns ownership of the resource to the resource manager and links the resource
	/// lifespan to that of the "linked" object.  When the object is destroyed (garbage collected)
	/// the resource is automatically disposed.
	///
	/// This allows scene and game objects to continue to be non-disposable even when containing
	/// disposable resources, as the resource manager will handle all resource cleanup.
	/// </summary>
	/// <param name="resource"></param>
	/// <param name="linkedobject"></param>
	public void LinkOwnership(IDisposable resource, object linkedobject)
	{
		HC_0001[resource] = new WeakReference(linkedobject);
	}

	/// <summary>
	/// Identifies and cleans up any linked resources that need disposed.
	/// </summary>
	/// <param name="gametime"></param>
	public void Update(GameTime gametime)
	{
		HCZ.Clear();
		foreach (KeyValuePair<IDisposable, WeakReference> item in HC_0001)
		{
			WeakReference value = item.Value;
			if (value != null && value.Target == null)
			{
				HCZ.Add(item.Key);
			}
		}
		foreach (IDisposable item2 in HCZ)
		{
			item2.Dispose();
			HC_0001.Remove(item2);
		}
	}

	/// <summary>
	/// Unused. Resources assigned to the manager are not removed until
	/// they are disposed (during the Unload method).
	/// </summary>
	public void Clear()
	{
	}

	/// <summary>
	/// Disposes and removes all IDisposable resources. Unloads but
	/// continues tracking IUnloadable resources.
	///
	/// Commonly used during Game.UnloadContent.
	/// </summary>
	public void Unload()
	{
		foreach (KeyValuePair<IDisposable, WeakReference> item in HC_0001)
		{
			item.Key.Dispose();
		}
		foreach (KeyValuePair<IUnloadable, int> item2 in HCw)
		{
			item2.Key.Unload();
		}
		HC_0001.Clear();
		HC_0002.Clear();
		HC_0012.Clear();
	}
}
