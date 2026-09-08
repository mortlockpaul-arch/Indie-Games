using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Audio;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Rendering;
using SynapseGaming.LightingSystem.Rendering.Forward;
using SynapseGaming.LightingSystem.Shadows;
using SynapseGaming.LightingSystem.Shadows.Forward;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Acts as both a service provider and component manager for a scene.
///
/// As a service provider its contained manager services can be requested by type and
/// accessed through common interfaces, making both using the built-in managers and
/// writing custom replacement managers easy.
///
/// As a component manager all contained manager services automatically receive calls
/// to BeginFrameRendering, EndFrameRendering, Update, and more, allowing custom managers
/// to be plugged in and run with out writing any additional code to specifically handle them.
/// </summary>
public class SceneInterface : IManagerServiceProvider
{
	internal class DA_0018 : IComparer<IManagerService>
	{
		public int Compare(IManagerService a, IManagerService b)
		{
			if (a == null)
			{
				return -1;
			}
			if (b == null)
			{
				return 1;
			}
			return a.ManagerProcessOrder - b.ManagerProcessOrder;
		}
	}

	internal class DAL : IComparer<IRenderableManager>
	{
		public int Compare(IRenderableManager a, IRenderableManager b)
		{
			if (a == null)
			{
				return -1;
			}
			if (b == null)
			{
				return 1;
			}
			return (a as IManagerService).ManagerProcessOrder - (b as IManagerService).ManagerProcessOrder;
		}
	}

	internal class DA_0019 : IComparer<IUpdatableManager>
	{
		public int Compare(IUpdatableManager a, IUpdatableManager b)
		{
			if (a == null)
			{
				return -1;
			}
			if (b == null)
			{
				return 1;
			}
			return (a as IManagerService).ManagerProcessOrder - (b as IManagerService).ManagerProcessOrder;
		}
	}

	private ISceneState _3A_0018;

	private Dictionary<Type, IManagerService> _3AL = new Dictionary<Type, IManagerService>(16);

	private List<IManagerService> _3A_0019 = new List<IManagerService>(16);

	private List<IUpdatableManager> _3A3 = new List<IUpdatableManager>(16);

	private List<IRenderableManager> _3A6 = new List<IRenderableManager>(16);

	private static DA_0018 _3AD = new DA_0018();

	private static DAL _3A_0017 = new DAL();

	private static DA_0019 _3A_0003 = new DA_0019();

	[CompilerGenerated]
	private static SceneInterface _3Al;

	[CompilerGenerated]
	private IResourceManager _3At;

	[CompilerGenerated]
	private IObjectManager _3AF;

	[CompilerGenerated]
	private IRenderManager _3Ac;

	[CompilerGenerated]
	private ILightManager _3Ag;

	[CompilerGenerated]
	private IAudioManager _3AI;

	[CompilerGenerated]
	private SunBurnEditor _3A8;

	[CompilerGenerated]
	private IShadowMapManager _3AZ;

	[CompilerGenerated]
	private IAvatarManager _3Ax;

	[CompilerGenerated]
	private ILightMapManager _3Aq;

	[CompilerGenerated]
	private ICollisionManager _3Ab;

	[CompilerGenerated]
	private bool _3AT;

	[CompilerGenerated]
	private bool _3Ay;

	[CompilerGenerated]
	private SystemStatisticCategory _3A_0015;

	/// <summary>
	/// Provides access to the current active SceneInterface. A SceneInterface becomes
	/// active when ApplyPreferences(), Update(), Clear(), Unload(),
	/// BeginFrameRendering(), or EndFrameRendering() is called.
	/// </summary>
	public static SceneInterface ActiveSceneInterface
	{
		[CompilerGenerated]
		get
		{
			return _3Al;
		}
		[CompilerGenerated]
		private set
		{
			_3Al = sceneInterface;
		}
	}

	/// <summary>
	/// Provides convenient access to the ResourceManager manager service contained in the provider.
	///
	/// Note: this property will be null if no manager service of this type is contained in the provider.
	/// </summary>
	public IResourceManager ResourceManager
	{
		[CompilerGenerated]
		get
		{
			return _3At;
		}
		[CompilerGenerated]
		private set
		{
			_3At = resourceManager;
		}
	}

	/// <summary>
	/// Provides convenient access to the ObjectManager manager service contained in the provider.
	///
	/// Note: this property will be null if no manager service of this type is contained in the provider.
	/// </summary>
	public IObjectManager ObjectManager
	{
		[CompilerGenerated]
		get
		{
			return _3AF;
		}
		[CompilerGenerated]
		private set
		{
			_3AF = objectManager;
		}
	}

	/// <summary>
	/// Provides convenient access to the RenderManager manager service contained in the provider.
	///
	/// Note: this property will be null if no manager service of this type is contained in the provider.
	/// </summary>
	public IRenderManager RenderManager
	{
		[CompilerGenerated]
		get
		{
			return _3Ac;
		}
		[CompilerGenerated]
		private set
		{
			_3Ac = renderManager;
		}
	}

	/// <summary>
	/// Provides convenient access to the LightManager manager service contained in the provider.
	///
	/// Note: this property will be null if no manager service of this type is contained in the provider.
	/// </summary>
	public ILightManager LightManager
	{
		[CompilerGenerated]
		get
		{
			return _3Ag;
		}
		[CompilerGenerated]
		private set
		{
			_3Ag = lightManager;
		}
	}

	/// <summary>
	/// Provides convenient access to the AudioManager manager service contained in the provider.
	///
	/// Note: this property will be null if no manager service of this type is contained in the provider.
	/// </summary>
	public IAudioManager AudioManager
	{
		[CompilerGenerated]
		get
		{
			return _3AI;
		}
		[CompilerGenerated]
		private set
		{
			_3AI = audioManager;
		}
	}

	/// <summary>
	/// Provides convenient access to the LightingSystemEditor service contained in the provider.
	///
	/// Note: this property will be null if no manager service of this type is contained in the provider.
	/// </summary>
	public SunBurnEditor Editor
	{
		[CompilerGenerated]
		get
		{
			return _3A8;
		}
		[CompilerGenerated]
		private set
		{
			_3A8 = sunBurnEditor;
		}
	}

	/// <summary>
	/// Provides convenient access to the ShadowMapManager manager service contained in the provider.
	///
	/// Note: this property will be null if no manager service of this type is contained in the provider.
	/// </summary>
	public IShadowMapManager ShadowMapManager
	{
		[CompilerGenerated]
		get
		{
			return _3AZ;
		}
		[CompilerGenerated]
		private set
		{
			_3AZ = shadowMapManager;
		}
	}

	/// <summary>
	/// Provides convenient access to the AvatarManager manager service contained in the provider.
	///
	/// Note: this property will be null if no manager service of this type is contained in the provider.
	/// </summary>
	public IAvatarManager AvatarManager
	{
		[CompilerGenerated]
		get
		{
			return _3Ax;
		}
		[CompilerGenerated]
		private set
		{
			_3Ax = avatarManager;
		}
	}

	/// <summary>
	/// Provides convenient access to the LightMapManager manager service contained in the provider.
	///
	/// Note: this property will be null if no manager service of this type is contained in the provider.
	/// </summary>
	public ILightMapManager LightMapManager
	{
		[CompilerGenerated]
		get
		{
			return _3Aq;
		}
		[CompilerGenerated]
		private set
		{
			_3Aq = lightMapManager;
		}
	}

	/// <summary>
	/// Provides convenient access to the CollisionManager manager service contained in the provider.
	///
	/// Note: this property will be null if no manager service of this type is contained in the provider.
	/// </summary>
	public ICollisionManager CollisionManager
	{
		[CompilerGenerated]
		get
		{
			return _3Ab;
		}
		[CompilerGenerated]
		private set
		{
			_3Ab = collisionManager;
		}
	}

	/// <summary>
	/// Manager type used to retrieve the IResourceManager manager service.  Use this
	/// type when creating a custom manager that replaces the built-in manager.
	/// </summary>
	public static Type ResourceManagerType => typeof(IResourceManager);

	/// <summary>
	/// Manager type used to retrieve the IObjectManager manager service.  Use this
	/// type when creating a custom manager that replaces the built-in manager.
	/// </summary>
	public static Type ObjectManagerType => typeof(IObjectManager);

	/// <summary>
	/// Manager type used to retrieve the IRenderManager manager service.  Use this
	/// type when creating a custom manager that replaces the built-in manager.
	/// </summary>
	public static Type RenderManagerType => typeof(IRenderManager);

	/// <summary>
	/// Manager type used to retrieve the ILightManager manager service.  Use this
	/// type when creating a custom manager that replaces the built-in manager.
	/// </summary>
	public static Type LightManagerType => typeof(ILightManager);

	/// <summary>
	/// Manager type used to retrieve the IAudioManager manager service.  Use this
	/// type when creating a custom manager that replaces the built-in manager.
	/// </summary>
	public static Type AudioManagerType => typeof(IAudioManager);

	/// <summary>
	/// Manager type used to retrieve the IShadowMapManager manager service.  Use this
	/// type when creating a custom manager that replaces the built-in manager.
	/// </summary>
	public static Type ShadowMapManagerType => typeof(IShadowMapManager);

	/// <summary>
	/// Manager type used to retrieve the LightingSystemEditor service.
	/// </summary>
	public static Type EditorType => typeof(SunBurnEditor);

	/// <summary>
	/// Manager type used to retrieve the IAvatarManager manager service.  Use this
	/// type when creating a custom manager that replaces the built-in manager.
	/// </summary>
	public static Type AvatarManagerType => typeof(IAvatarManager);

	/// <summary>
	/// Manager type used to retrieve the ILightMapManager manager service.  Use this
	/// type when creating a custom manager that replaces the built-in manager.
	/// </summary>
	public static Type LightMapManagerType => typeof(ILightMapManager);

	/// <summary>
	/// Manager type used to retrieve the ICollisionManager manager service.  Use this
	/// type when creating a custom manager that replaces the built-in manager.
	/// </summary>
	public static Type CollisionManagerType => typeof(ICollisionManager);

	/// <summary>
	/// Enables on-screen statistics.
	/// </summary>
	public bool ShowStatistics
	{
		[CompilerGenerated]
		get
		{
			return _3AT;
		}
		[CompilerGenerated]
		set
		{
			_3AT = value;
		}
	}

	/// <summary>
	/// Enables on-screen console messages. The console is not displayed if no messages exist.
	/// </summary>
	public bool ShowConsole
	{
		[CompilerGenerated]
		get
		{
			return _3Ay;
		}
		[CompilerGenerated]
		set
		{
			_3Ay = value;
		}
	}

	/// <summary>
	/// Categories used when rendering on-screen statistics.
	/// </summary>
	public SystemStatisticCategory StatisticCategories
	{
		[CompilerGenerated]
		get
		{
			return _3A_0015;
		}
		[CompilerGenerated]
		set
		{
			_3A_0015 = value;
		}
	}

	/// <summary>
	/// Creates a new SceneInterface instance.
	/// </summary>
	public SceneInterface()
	{
	}

	/// <summary>
	/// Creates and adds a default set of manager services. This makes
	/// initializing the SceneInterface easier.
	///
	/// Depending on the creation options provided the following manager
	/// services will be created:
	///
	/// Always
	///     -ResourceManager
	///     -ObjectManager
	///     -LightManager
	///     -AvatarManager
	///     -PostProcessManager
	///     -LightMapManager
	///     -LightingSystemEditor
	///
	/// Forward rendering
	///     -RenderManager
	///     -LightManager
	///
	/// Deferred rednering
	///     -DeferredRenderManager
	///     -DeferredShadowMapManager
	/// </summary>
	/// <param name="renderingsystemtype">Determines if deferred or forward rendering should be used.</param>
	/// <param name="includeautoloadedplugins">Determines if 3rd party plugins should automatically be loaded.</param>
	public void CreateDefaultManagers(RenderingSystemType renderingsystemtype, bool includeautoloadedplugins)
	{
		Unload();
		AddManager(new ResourceManager(this));
		AddManager(new ObjectManager(this));
		AddManager(new LightManager(this));
		AddManager(new LightMapManager(this));
		AddManager(new AudioManager(this));
		if (SunBurnCoreSystem.Instance.GraphicsDeviceManager is GraphicsDeviceManager)
		{
			AddManager(new SunBurnEditor(this));
		}
		if (renderingsystemtype == RenderingSystemType.Deferred)
		{
			throw new Exception("Deferred rendering only available in SunBurn Pro and Studio editions.");
		}
		AddManager(new RenderManager(this));
		AddManager(new ShadowMapManager(this));
		AddManager(new AvatarManager(this));
		SunBurnCoreSystem.Instance.InitializeSceneInterfaceWithLoadedPlugins(this, includeautoloadedplugins);
	}

	/// <summary>
	/// Adds a manager service to the provider.
	/// </summary>
	/// <param name="manager"></param>
	public virtual void AddManager(IManagerService manager)
	{
		RemoveManager(manager);
		if (!_3AL.ContainsKey(manager.ManagerType))
		{
			_3AL.Add(manager.ManagerType, manager);
			ResortServices();
			Lf();
		}
	}

	/// <summary>
	/// Removes a manager service from the provider.
	/// </summary>
	/// <param name="manager"></param>
	public virtual void RemoveManager(IManagerService manager)
	{
		if (_3AL.ContainsKey(manager.ManagerType))
		{
			_3AL.Remove(manager.ManagerType);
			ResortServices();
			Lf();
		}
	}

	private void Lf()
	{
		ResourceManager = GetManager(ResourceManagerType, required: false) as IResourceManager;
		ObjectManager = GetManager(ObjectManagerType, required: false) as IObjectManager;
		RenderManager = GetManager(RenderManagerType, required: false) as IRenderManager;
		LightManager = GetManager(LightManagerType, required: false) as ILightManager;
		AudioManager = GetManager(AudioManagerType, required: false) as IAudioManager;
		ShadowMapManager = GetManager(ShadowMapManagerType, required: false) as IShadowMapManager;
		AvatarManager = GetManager(AvatarManagerType, required: false) as IAvatarManager;
		LightMapManager = GetManager(LightMapManagerType, required: false) as ILightMapManager;
		CollisionManager = GetManager(CollisionManagerType, required: false) as ICollisionManager;
		Editor = GetManager(EditorType, required: false) as SunBurnEditor;
	}

	/// <summary>
	/// Resorts the contained manager services.
	///
	/// Providers should automatically resort when manager services
	/// are added and removed, however manual resorting is necessary
	/// if a manager service's ManagerProcessOrder property changes
	/// after being added to the provider.
	/// </summary>
	public virtual void ResortServices()
	{
		_3A_0019.Clear();
		_3A6.Clear();
		_3A3.Clear();
		foreach (KeyValuePair<Type, IManagerService> item in _3AL)
		{
			IManagerService value = item.Value;
			_3A_0019.Add(value);
			if (value is IRenderableManager)
			{
				_3A6.Add(value as IRenderableManager);
			}
			if (value is IUpdatableManager)
			{
				_3A3.Add(value as IUpdatableManager);
			}
		}
		_3A_0019.Sort(_3AD);
		_3A6.Sort(_3A_0017);
		_3A3.Sort(_3A_0003);
	}

	/// <summary>
	/// Retrieves a manager service by type from the provider.
	/// </summary>
	/// <typeparam name="T">Type used by the manager as a unique
	/// identifying key (IManagerService.ManagerType).</typeparam>
	/// <param name="required">Determines whether an exception should
	/// be thrown if the manager is not found.</param>
	/// <returns></returns>
	public virtual T GetManager<T>(bool required) where T : class
	{
		Type typeFromHandle = typeof(T);
		T val = GetManager(typeFromHandle, required) as T;
		if (val == null && required)
		{
			throw new Exception("Service manager does not contain a service assigned to the '" + typeFromHandle.Name + "' type.");
		}
		return val;
	}

	/// <summary>
	/// Retrieves a manager service by type from the provider.
	/// </summary>
	/// <param name="managertype">Type used by the manager as a unique
	/// identifying key (IManagerService.ManagerType).</param>
	/// <param name="required">Determines whether an exception should
	/// be thrown if the manager is not found.</param>
	/// <returns></returns>
	public IManagerService GetManager(Type managertype, bool required)
	{
		_3AL.TryGetValue(managertype, out var value);
		if (value == null && required)
		{
			throw new Exception("Service manager does not contain a service assigned to the '" + managertype.Name + "' type.");
		}
		return value;
	}

	/// <summary>
	/// Retrieves all manager services from the provider.
	/// </summary>
	/// <param name="managers">List used to store manager services.</param>
	public void GetManagers(List<IManagerService> managers)
	{
		foreach (KeyValuePair<Type, IManagerService> item in _3AL)
		{
			managers.Add(item.Value);
		}
	}

	/// <summary>
	/// Use to apply user quality and performance preferences to the
	/// contained manager services.
	/// </summary>
	/// <param name="preferences"></param>
	public virtual void ApplyPreferences(ISystemPreferences preferences)
	{
		ActiveSceneInterface = this;
		foreach (IManagerService item in _3A_0019)
		{
			item.ApplyPreferences(preferences);
		}
	}

	/// <summary>
	/// Submits a scene to the contained manager services.
	/// </summary>
	/// <param name="scene"></param>
	public virtual void Submit(IScene scene)
	{
		ActiveSceneInterface = this;
		foreach (IManagerService item in _3A_0019)
		{
			if (item is ISubmit<IScene> submit)
			{
				submit.Submit(scene);
			}
		}
	}

	/// <summary>
	/// Removes a scene from the contained manager services.
	/// </summary>
	/// <param name="scene"></param>
	public virtual void Remove(IScene scene)
	{
		ActiveSceneInterface = this;
		foreach (IManagerService item in _3A_0019)
		{
			if (item is ISubmit<IScene> submit)
			{
				submit.Remove(scene);
			}
		}
	}

	/// <summary>
	/// Removes resources managed by the contained manager services.
	/// Commonly used while clearing the scene.
	///
	/// Note: this does not remove the contained manager services.
	/// </summary>
	public virtual void Clear()
	{
		ActiveSceneInterface = this;
		foreach (IManagerService item in _3A_0019)
		{
			item.Clear();
		}
		OptimizationSystem.Clear();
		GeometryExtractionHelper.Clear();
	}

	/// <summary>
	/// Disposes any graphics resources used internally by the
	/// contained manager services, and removes scene resources
	/// managed by them. Commonly used during Game.UnloadContent.
	///
	/// Note: this does not remove the contained manager services.
	/// </summary>
	public virtual void Unload()
	{
		ActiveSceneInterface = this;
		foreach (IManagerService item in _3A_0019)
		{
			item.Unload();
		}
		OptimizationSystem.Clear();
		GeometryExtractionHelper.Clear();
	}

	/// <summary>
	/// Updates the contained manager services and their managed resources.
	/// </summary>
	/// <param name="gameTime"></param>
	public virtual void Update(GameTime gameTime)
	{
		ActiveSceneInterface = this;
		LightingSystemPerformance.Begin("SceneInterface.Update");
		foreach (IUpdatableManager item in _3A3)
		{
			item.Update(gameTime);
		}
	}

	/// <summary>
	/// Sets up the contained manager services prior to rendering (used for forward rendering).
	/// </summary>
	/// <param name="scenestate"></param>
	public virtual void BeginFrameRendering(ISceneState scenestate)
	{
		ActiveSceneInterface = this;
		LightingSystemPerformance.Begin("SceneInterface.BeginFrameRendering");
		_3A_0018 = scenestate;
		foreach (IRenderableManager item in _3A6)
		{
			item.BeginFrameRendering(scenestate);
		}
	}

	/// <summary>
	/// Finalizes rendering on the contained manager services.
	/// </summary>
	public virtual void EndFrameRendering()
	{
		ActiveSceneInterface = this;
		LightingSystemPerformance.Begin("SceneInterface.EndFrameRendering");
		for (int num = _3A6.Count - 1; num >= 0; num--)
		{
			_3A6[num].EndFrameRendering();
		}
		SystemConsole.Apply();
		LightingSystemPerformance.Begin("SceneInterface.EndFrameRendering (stats)");
		if ((ShowConsole || ShowStatistics) && _3A_0018 != null && _3A_0018.RenderingToScreen)
		{
			SystemConsole.Render(StatisticCategories, ShowStatistics, ShowConsole, new Vector2(20f), Vector2.One, Color.White, _3A_0018.GameTime);
		}
	}
}
