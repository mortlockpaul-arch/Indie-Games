using System.Collections.Generic;
using _0003;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Rendering;

namespace SynapseGaming.LightingSystem.Shadows;

/// <summary>
/// Provides base scene shadow management support.
/// </summary>
public abstract class BaseShadowManager : IRenderableManager, IManager, IUnloadable
{
	private IManagerServiceProvider _3A_0018;

	private ISceneState _3AL = new SceneState();

	private _0003.t<ShadowGroup> _3A_0019 = new _0003.t<ShadowGroup>();

	private static ShadowSource _3A3 = new ShadowSource();

	private static DirectionalLight _3A6 = new DirectionalLight();

	private static ShadowGroup _3AD = new ShadowGroup();

	private static ShadowGroup _3A_0017 = new ShadowGroup();

	private static Dictionary<IShadowSource, ShadowGroup> _3A_0003 = new Dictionary<IShadowSource, ShadowGroup>(32);

	/// <summary>
	/// Scene interface the manager was created by, or assigned during construction.
	/// </summary>
	public IManagerServiceProvider OwnerSceneInterface => _3A_0018;

	/// <summary>
	/// The current SceneState used by this object.
	/// </summary>
	protected ISceneState SceneState => _3AL;

	/// <summary>
	/// Creates a new BaseShadowManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	public BaseShadowManager(IManagerServiceProvider sceneinterface)
	{
		_3A_0018 = sceneinterface;
	}

	/// <summary>
	/// Use to apply user quality and performance preferences to the resources managed by this object.
	/// </summary>
	/// <param name="preferences"></param>
	public virtual void ApplyPreferences(ISystemPreferences preferences)
	{
	}

	/// <summary>
	/// Sets up frame information necessary for scene shadowing.
	/// </summary>
	public virtual void BeginFrameRendering(ISceneState scenestate)
	{
		SplashScreen._6b();
		_3AL = scenestate;
	}

	/// <summary>
	/// Cleans up frame information.
	/// </summary>
	public virtual void EndFrameRendering()
	{
		_3A_0019.FreeAllTracked();
	}

	/// <summary>
	/// Builds a list of shadow groups based on the provided light list.  Shadow
	/// groups contain a list of all lights that share a common shadow source.
	/// </summary>
	/// <param name="shadowgroups">Destination shadow group list.</param>
	/// <param name="lights">Source light list.</param>
	/// <param name="usedefaultgrouping">Determines if ungrouped lights should be placed in a
	/// single default group (recommended: true for deferred rendering and false for forward).</param>
	protected void BuildShadowGroups(List<ShadowGroup> shadowgroups, List<BaseLight> lights, bool usedefaultgrouping)
	{
		_3A_0003.Clear();
		_3A3.ShadowType = ShadowType.None;
		_3AD.Shadow = null;
		_3AD.Lights.Clear();
		_3A_0003.Add(_3A3, _3AD);
		_3A6.ShadowType = ShadowType.None;
		_3A_0017.Shadow = null;
		_3A_0017.Lights.Clear();
		_3A_0003.Add(_3A6, _3A_0017);
		int count = lights.Count;
		for (int i = 0; i < count; i++)
		{
			BaseLight baseLight = lights[i];
			if (baseLight == null)
			{
				continue;
			}
			IShadowSource shadowSource = baseLight.ShadowSource;
			if (usedefaultgrouping && (shadowSource == null || (baseLight == shadowSource && shadowSource.ShadowType == ShadowType.None)))
			{
				LightTypeCaster lightTypeCaster = OptimizationSystem.LightTypeCasters.Get(baseLight);
				if (lightTypeCaster.PointSource != null)
				{
					_3AD.Lights.Add(baseLight);
				}
				else
				{
					_3A_0017.Lights.Add(baseLight);
				}
				continue;
			}
			if (!_3A_0003.TryGetValue(shadowSource, out var value))
			{
				value = _3A_0019.New();
				value.Shadow = null;
				value.Lights.Clear();
				_3A_0003.Add(shadowSource, value);
			}
			value.Lights.Add(baseLight);
		}
		if (_3A_0017.Lights.Count <= 0)
		{
			_3A_0003.Remove(_3A6);
		}
		if (_3AD.Lights.Count <= 0)
		{
			_3A_0003.Remove(_3A3);
		}
		else
		{
			_3A3.Position = (_3AD.Lights[0] as IPointSource).Position;
		}
		foreach (KeyValuePair<IShadowSource, ShadowGroup> item in _3A_0003)
		{
			item.Value.Build(item.Key, _3AL);
			shadowgroups.Add(item.Value);
		}
	}

	/// <summary>
	/// Removes resources managed by this object. Commonly used while clearing the scene.
	/// </summary>
	public abstract void Clear();

	/// <summary>
	/// Disposes any graphics resource used internally by this object, and removes
	/// scene resources managed by this object. Commonly used during Game.UnloadContent.
	/// </summary>
	public abstract void Unload();
}
