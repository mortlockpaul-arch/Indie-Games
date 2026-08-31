using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Rendering;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Base manager class that implements ObjectGraph and includes support for
/// submitting scenes, which may contain objects of type the manager stores.
/// </summary>
/// <typeparam name="T">Type of object the manager contains.</typeparam>
/// <typeparam name="TManagerServiceType">The derived manager class.</typeparam>
public abstract class BaseObjectGraphManager<T, TManagerServiceType> : ObjectGraph<T>, IManagerService, IManager, IUnloadable, ISubmit<IScene> where T : IMovableObject
{
	private IManagerServiceProvider HCB;

	private List<IScene> HC_0002 = new List<IScene>(16);

	/// <summary>
	/// Gets the manager specific Type used as a unique key for storing and
	/// requesting the manager from the IManagerServiceProvider.
	/// </summary>
	public abstract Type ManagerType { get; }

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
	public abstract int ManagerProcessOrder { get; set; }

	/// <summary>
	/// Scene interface the manager was created by, or assigned during construction.
	/// </summary>
	public IManagerServiceProvider OwnerSceneInterface => HCB;

	/// <summary>
	/// Creates a new BaseObjectGraphManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	/// <param name="worldboundingbox">The smallest bounding area that completely
	/// contains the scene.  Helps the RenderManager build an optimal scene tree.</param>
	/// <param name="worldtreemaxdepth"></param>
	public BaseObjectGraphManager(IManagerServiceProvider sceneinterface, BoundingBox worldboundingbox, int worldtreemaxdepth)
		: base(worldboundingbox, worldtreemaxdepth)
	{
		HCB = sceneinterface;
	}

	/// <summary>
	/// Creates a new BaseObjectGraphManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	public BaseObjectGraphManager(IManagerServiceProvider sceneinterface)
	{
		HCB = sceneinterface;
	}

	/// <summary>
	/// Called when the game code sets the manager or SceneInterface preferences.
	/// </summary>
	/// <param name="preferences"></param>
	public virtual void ApplyPreferences(ISystemPreferences preferences)
	{
	}

	/// <summary>
	/// Adds an object to the container. This does not transfer ownership, disposable
	/// objects should be maintained and disposed separately.
	/// </summary>
	/// <param name="scene"></param>
	public virtual void Submit(IScene scene)
	{
		HC_0002.Add(scene);
		scene.SetContainingManager<TManagerServiceType>(this);
		scene.Apply();
	}

	/// <summary>
	/// Moves an object within the container. This method is used when the container
	/// implements a tree or graph, and relocates an object within that structure
	/// often due to a change in object world position.
	/// </summary>
	/// <param name="scene"></param>
	public virtual void Move(IScene scene)
	{
	}

	/// <summary>
	/// Removes an object from the container.
	/// </summary>
	/// <param name="scene"></param>
	public virtual void Remove(IScene scene)
	{
		if (HC_0002.Remove(scene))
		{
			scene.SetContainingManager<TManagerServiceType>(null);
		}
	}

	/// <summary>
	/// Adds an object to the container. This does not transfer ownership, disposable
	/// objects should be maintained and disposed separately.
	/// </summary>
	/// <param name="obj"></param>
	public override void Submit(T obj)
	{
		base.Submit(obj);
		obj.OnSubmittedToManager(this);
	}

	/// <summary>
	/// Removes an object from the container.
	/// </summary>
	/// <param name="obj"></param>
	public override void Remove(T obj)
	{
		base.Remove(obj);
		obj.OnRemovedFromManager(this);
	}

	/// <summary>
	/// Called when the game clears the engine of objects (generally when
	/// clearing the current level / scene and before loading the next one).
	/// </summary>
	public override void Clear()
	{
		while (HC_0002.Count > 0)
		{
			Remove(HC_0002[0]);
		}
		HC_0002.Clear();
		foreach (KeyValuePair<T, int> allObject in base.ObjectIndex.AllObjects)
		{
			allObject.Key.OnRemovedFromManager(this);
		}
		base.Clear();
	}

	/// <summary>
	/// Called when the game's graphics and disposable resources are no longer
	/// used or are invalid (due to exiting the game or the graphics device
	/// resetting).  All resources should be disposed before exiting this method.
	/// </summary>
	public virtual void Unload()
	{
		Clear();
	}
}
