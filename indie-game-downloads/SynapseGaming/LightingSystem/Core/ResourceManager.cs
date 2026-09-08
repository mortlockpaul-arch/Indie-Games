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
	internal delegate Effect DA_0018();

	private static bool _3A_0018 = true;

	private static Dictionary<string, Effect> _3AL = new Dictionary<string, Effect>();

	private static Dictionary<Type, Effect> _3A_0019 = new Dictionary<Type, Effect>();

	private int _3A3 = 500;

	private IManagerServiceProvider _3A6;

	private Dictionary<IDisposable, WeakReference> _3AD = new Dictionary<IDisposable, WeakReference>(32);

	private Dictionary<IUnloadable, int> _3A_0017 = new Dictionary<IUnloadable, int>(32);

	private List<IDisposable> _3A_0003 = new List<IDisposable>(32);

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
			return _3A_0018;
		}
		set
		{
			_3A_0018 = value;
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
			return _3A3;
		}
		set
		{
			_3A3 = value;
		}
	}

	/// <summary>
	/// Scene interface the manager was created by, or assigned during construction.
	/// </summary>
	public IManagerServiceProvider OwnerSceneInterface => _3A6;

	internal static Effect L_0016(Type P_0, DA_0018 P_1)
	{
		if (_3A_0019.TryGetValue(P_0, out var value) && !value.IsDisposed)
		{
			return value;
		}
		value = P_1();
		_3A_0019[P_0] = value;
		return value;
	}

	internal static Effect L_0016(string P_0, DA_0018 P_1)
	{
		if (!_3A_0018)
		{
			return P_1();
		}
		if (_3AL.TryGetValue(P_0, out var value) && !value.IsDisposed)
		{
			return value;
		}
		value = P_1();
		_3AL[P_0] = value;
		return value;
	}

	/// <summary>
	/// Creates a new ResourceManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	public ResourceManager(IManagerServiceProvider sceneinterface)
	{
		_3A6 = sceneinterface;
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
		_3AD[resource] = null;
	}

	/// <summary>
	/// Assigns ownership of the resource to the resource manager, this means the manager
	/// will handle disposing and removing (IDisposable), or unloading (IUnloadable) the
	/// resource when the scene is unloaded (when [manager].Unload() is called).
	/// </summary>
	/// <param name="resource"></param>
	public void AssignOwnership(IUnloadable resource)
	{
		_3A_0017[resource] = 0;
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
		_3AD[resource] = new WeakReference(linkedobject);
	}

	/// <summary>
	/// Identifies and cleans up any linked resources that need disposed.
	/// </summary>
	/// <param name="gametime"></param>
	public void Update(GameTime gametime)
	{
		_3A_0003.Clear();
		foreach (KeyValuePair<IDisposable, WeakReference> item in _3AD)
		{
			WeakReference value = item.Value;
			if (value != null && value.Target == null)
			{
				_3A_0003.Add(item.Key);
			}
		}
		foreach (IDisposable item2 in _3A_0003)
		{
			item2.Dispose();
			_3AD.Remove(item2);
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
		foreach (KeyValuePair<IDisposable, WeakReference> item in _3AD)
		{
			item.Key.Dispose();
		}
		foreach (KeyValuePair<IUnloadable, int> item2 in _3A_0017)
		{
			item2.Key.Unload();
		}
		_3AD.Clear();
		_3AL.Clear();
		_3A_0019.Clear();
	}
}
