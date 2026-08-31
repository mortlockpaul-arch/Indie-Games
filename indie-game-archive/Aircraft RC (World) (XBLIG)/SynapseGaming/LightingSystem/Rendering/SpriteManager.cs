using System;
using SynapseGaming.LightingSystem.Core;
using Z;
using u;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Acts as a resource manager for arrays and buffers used during sprite creation.
/// </summary>
public class SpriteManager : IManagerService, IManager, IUnloadable
{
	private int HCB = 100;

	private IManagerServiceProvider HC_0002;

	private Z.y<RenderableMesh> HC_0012 = new Z.y<RenderableMesh>();

	private Z._6<u._0011> HCH = new Z._6<u._0011>();

	/// <summary>
	/// Gets the manager specific Type used as a unique key for storing and
	/// requesting the manager from the IManagerServiceProvider.
	/// </summary>
	public Type ManagerType => typeof(SpriteManager);

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
			return HCB;
		}
		set
		{
			HCB = value;
		}
	}

	/// <summary>
	/// Scene interface the manager was created by, or assigned during construction.
	/// </summary>
	public IManagerServiceProvider OwnerSceneInterface => HC_0002;

	/// <summary>
	/// Creates a new SpriteManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	public SpriteManager(IManagerServiceProvider sceneinterface)
	{
		HC_0002 = sceneinterface;
	}

	/// <summary>
	/// Creates a new SpriteContainer instance for storing and rendering 2D sprites.
	/// </summary>
	/// <returns></returns>
	public SpriteContainer CreateSpriteContainer()
	{
		SpriteContainer spriteContainer = new SpriteContainer(SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice, HC_0012, HCH);
		spriteContainer.UpdateType = UpdateType.Automatic;
		return spriteContainer;
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
	public void Clear()
	{
		HC_0012.FreeAllTracked();
		HCH.FreeAllTracked();
	}

	/// <summary>
	/// Disposes any graphics resource used internally by this object, and removes
	/// scene resources managed by this object. Commonly used during Game.UnloadContent.
	/// </summary>
	public void Unload()
	{
		HC_0012.Clear();
		HCH.Unload();
	}
}
