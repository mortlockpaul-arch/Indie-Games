using System;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Interface that provides access to the scene's resource manager. The resource manager
/// tracks disposable and unloadable resources, freeing them when the scene is unloaded.
/// </summary>
public interface IResourceManager : IManagerService, IUpdatableManager, IManager, IUnloadable
{
	/// <summary>
	/// Assigns ownership of the resource to the resource manager, this means the manager
	/// will handle disposing and removing (IDisposable), or unloading (IUnloadable) the
	/// resource when the scene is unloaded (when [manager].Unload() is called).
	/// </summary>
	/// <param name="resource"></param>
	void AssignOwnership(IDisposable resource);

	/// <summary>
	/// Assigns ownership of the resource to the resource manager, this means the manager
	/// will handle disposing and removing (IDisposable), or unloading (IUnloadable) the
	/// resource when the scene is unloaded (when [manager].Unload() is called).
	/// </summary>
	/// <param name="resource"></param>
	void AssignOwnership(IUnloadable resource);

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
	void LinkOwnership(IDisposable resource, object linkedobject);
}
