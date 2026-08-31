using System.Collections.Generic;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Rendering;
using Z;

namespace SynapseGaming.LightingSystem.Shadows;

/// <summary>
/// Provides base scene shadow management support.
/// </summary>
public abstract class BaseShadowManager : IRenderableManager, IManager, IUnloadable
{
	private IManagerServiceProvider HCB;

	private ISceneState HC_0002 = new SceneState();

	private Z.y<ShadowGroup> HC_0012 = new Z.y<ShadowGroup>();

	private static ShadowSource HCH = new ShadowSource();

	private static DirectionalLight HC7 = new DirectionalLight();

	private static ShadowGroup HC_0001 = new ShadowGroup();

	private static ShadowGroup HCw = new ShadowGroup();

	private static Dictionary<IShadowSource, ShadowGroup> HCZ = new Dictionary<IShadowSource, ShadowGroup>(32);

	/// <summary>
	/// Scene interface the manager was created by, or assigned during construction.
	/// </summary>
	public IManagerServiceProvider OwnerSceneInterface => HCB;

	/// <summary>
	/// The current SceneState used by this object.
	/// </summary>
	protected ISceneState SceneState => HC_0002;

	/// <summary>
	/// Creates a new BaseShadowManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	public BaseShadowManager(IManagerServiceProvider sceneinterface)
	{
		HCB = sceneinterface;
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
		SplashScreen._7z();
		HC_0002 = scenestate;
	}

	/// <summary>
	/// Cleans up frame information.
	/// </summary>
	public virtual void EndFrameRendering()
	{
		HC_0012.FreeAllTracked();
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
		HCZ.Clear();
		HCH.ShadowType = ShadowType.None;
		HC_0001.Shadow = null;
		HC_0001.Lights.Clear();
		HCZ.Add(HCH, HC_0001);
		HC7.ShadowType = ShadowType.None;
		HCw.Shadow = null;
		HCw.Lights.Clear();
		HCZ.Add(HC7, HCw);
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
					HC_0001.Lights.Add(baseLight);
				}
				else
				{
					HCw.Lights.Add(baseLight);
				}
				continue;
			}
			if (!HCZ.TryGetValue(shadowSource, out var value))
			{
				value = HC_0012.New();
				value.Shadow = null;
				value.Lights.Clear();
				HCZ.Add(shadowSource, value);
			}
			value.Lights.Add(baseLight);
		}
		if (HCw.Lights.Count <= 0)
		{
			HCZ.Remove(HC7);
		}
		if (HC_0001.Lights.Count <= 0)
		{
			HCZ.Remove(HCH);
		}
		else
		{
			HCH.Position = (HC_0001.Lights[0] as IPointSource).Position;
		}
		foreach (KeyValuePair<IShadowSource, ShadowGroup> item in HCZ)
		{
			item.Value.Build(item.Key, HC_0002);
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
