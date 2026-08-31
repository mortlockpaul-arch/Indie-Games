using SynapseGaming.LightingSystem.Core;
using Z;

namespace SynapseGaming.LightingSystem.Shadows.Forward;

/// <summary>
/// Manages scene shadow maps and provides methods for building and organizing
/// relationships between lights and shadows. Uses a render target cache to
/// minimize memory usage.
/// </summary>
public class ShadowMapManager : BaseShadowMapManager
{
	private Z._6<ShadowCubeMap> HCB = new Z._6<ShadowCubeMap>();

	private Z._6<ShadowDirectionalMap> HC_0002 = new Z._6<ShadowDirectionalMap>();

	/// <summary>
	/// Creates a new ShadowMapManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	/// <param name="pagesize">Size in pixels of each render target (page) in the cache.
	/// For a size of 1024 the actual page dimensions are 1024x1024. Small sizes can reduce
	/// performance by fragmenting the shadow maps, and reduce shadow quality by lowering
	/// the maximum resolution of each shadow map section.</param>
	/// <param name="maxmemoryusage">Maximum amount of memory the cache is allowed to consume.
	/// This is an approximate value and the cache may use more memory in certain instances.</param>
	/// <param name="preferhalffloat">True when smaller half-float format render targets are
	/// preferred. These formats consume less memory and generally perform better, but have
	/// lower accuracy on directional lights.</param>
	public ShadowMapManager(IManagerServiceProvider sceneinterface, int pagesize, int maxmemoryusage, bool preferhalffloat)
		: base(sceneinterface, pagesize, maxmemoryusage, preferhalffloat)
	{
	}

	/// <summary>
	/// Creates a new ShadowMapManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	/// <param name="shadowmapcache"></param>
	public ShadowMapManager(IManagerServiceProvider sceneinterface, ShadowMapCache shadowmapcache)
		: base(sceneinterface, shadowmapcache)
	{
	}

	/// <summary>
	/// Creates a new ShadowMapManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	public ShadowMapManager(IManagerServiceProvider sceneinterface)
		: base(sceneinterface)
	{
	}

	/// <summary>
	/// Creates a new or cached shadow map object for this light type.
	/// </summary>
	/// <param name="shadowsource">Shadow source which uses the newly created or cached shadow map object.
	/// Provides information about how the shadow is used, such as location and the type of objects rendered
	/// to the shadow map.</param>
	/// <returns></returns>
	protected override IShadowMap CreateDirectionalShadowMap(IShadowSource shadowsource)
	{
		return HC_0002.New();
	}

	/// <summary>
	/// Creates a new or cached shadow map object for this light type.
	/// </summary>
	/// <param name="shadowsource">Shadow source which uses the newly created or cached shadow map object.
	/// Provides information about how the shadow is used, such as location and the type of objects rendered
	/// to the shadow map.</param>
	/// <returns></returns>
	protected override IShadowMap CreatePointShadowMap(IShadowSource shadowsource)
	{
		return HCB.New();
	}

	/// <summary>
	/// Creates a new or cached shadow map object for this light type.
	/// </summary>
	/// <param name="shadowsource">Shadow source which uses the newly created or cached shadow map object.
	/// Provides information about how the shadow is used, such as location and the type of objects rendered
	/// to the shadow map.</param>
	/// <returns></returns>
	protected override IShadowMap CreateSpotShadowMap(IShadowSource shadowsource)
	{
		return HCB.New();
	}

	/// <summary>
	/// Finalizes rendering and cleans up frame information including removing all frame lifespan objects.
	/// </summary>
	public override void EndFrameRendering()
	{
		base.EndFrameRendering();
		HCB.FreeAllTracked();
		HC_0002.FreeAllTracked();
	}

	/// <summary>
	/// Unloads all scene and device specific data.  Must be called
	/// when the device is reset (during Game.UnloadGraphicsContent()).
	/// </summary>
	public override void Unload()
	{
		base.Unload();
		HCB.Unload();
		HC_0002.Unload();
	}
}
