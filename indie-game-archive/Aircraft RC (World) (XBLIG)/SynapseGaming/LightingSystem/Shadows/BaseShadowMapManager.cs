using System;
using System.Collections.Generic;
using F;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Lights;
using Z;

namespace SynapseGaming.LightingSystem.Shadows;

/// <summary>
/// Base class that provides shadow map management.  Used by the forward rendering
/// ShadowMapManager and deferred rendering DeferredShadowMapManager classes.
/// </summary>
public abstract class BaseShadowMapManager : BaseShadowManager, IShadowMapVisibility, IShadowMapManager, IRenderableManager, IManagerService, IManager, IUnloadable
{
	private enum _0001CB
	{
		High,
		Normal,
		Low
	}

	private const int HCB = 67108864;

	private const int HC_0002 = 2048;

	private const int HC_0012 = 32;

	private int HCH = 40;

	private float[] HC7 = new float[3] { 0.2f, 0.53f, 1f };

	private bool[] HC_0001 = new bool[3] { true, true, true };

	private bool HCw = true;

	private float HCZ = 1f;

	private _0001CB HC_000F = _0001CB.Normal;

	private float HCy = 1f;

	private Z._0001 HC6;

	private ShadowMapCache HCD;

	private Z._6<ShadowRenderTargetGroup> HC_0011 = new Z._6<ShadowRenderTargetGroup>();

	private RenderTarget2D HCK;

	private static List<ShadowGroup> HC_0003 = new List<ShadowGroup>(128);

	private static List<Rectangle> HCk = new List<Rectangle>();

	private static Dictionary<RenderTarget2D, ShadowRenderTargetGroup> HCs = new Dictionary<RenderTarget2D, ShadowRenderTargetGroup>();

	/// <summary>
	/// Gets the manager specific Type used as a unique key for storing and
	/// requesting the manager from the IManagerServiceProvider.
	/// </summary>
	public Type ManagerType => SceneInterface.ShadowMapManagerType;

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
	/// Determines how far from the camera a directional shadow level-of-detail (lod) will
	/// stretch before transitioning to the next lod.
	///
	/// Index 0 controls the highest level of detail, 1 controls the next highest, and so on.
	///
	/// The range is normalized relative to the environment ShadowFadeEndDistance,
	/// for instance a value of 1.0 transitions at the ShadowFadeEndDistance
	/// whereas a value of 0.25 transitions at (ShadowFadeEndDistance * 0.25).
	/// </summary>
	public float[] ShadowLODRangeHints => HC7;

	/// <summary>
	/// Determines if a directional shadow level-of-detail (lod) is enabled and will have its
	/// shadow map filled with an image of the scene.  Disabling unneeded lods reduces
	/// the number of rendered objects and draw calls.
	///
	/// Index 0 controls the highest level of detail, 1 controls the next highest, and so on.
	///
	/// Unlike point light shadows, directional light shadows render all of their lods
	/// every frame.  Each lod represents a different area in front of the camera with
	/// the highest lod closest to the viewer.  For some games (such as top-down
	/// perspective games) only a single lod is necessary and the rest can be disabled.
	/// </summary>
	public bool[] ShadowLODEnabled => HC_0001;

	/// <summary>
	/// True when smaller half-float format render targets are preferred. These
	/// formats consume less memory and generally perform better, but have lower
	/// accuracy on directional lights.
	/// </summary>
	public bool PreferHalfFloatTextureFormat
	{
		get
		{
			return HCD.PreferHalfFloatTextureFormat;
		}
		set
		{
			if (HCD.PreferHalfFloatTextureFormat != value)
			{
				HCD.Resize(HCD.PageSize, HCD.MaxMemoryUsage, value);
			}
		}
	}

	/// <summary>
	/// Maximum amount of memory the shadow map cache is allowed to consume. This is an
	/// approximate value and the cache may use more memory in certain instances.
	/// </summary>
	public int MaxMemoryUsage
	{
		get
		{
			return HCD.MaxMemoryUsage;
		}
		set
		{
			if (HCD.MaxMemoryUsage != value)
			{
				HCD.Resize(HCD.PageSize, value, HCD.PreferHalfFloatTextureFormat);
			}
		}
	}

	/// <summary>
	/// Size in pixels of each render target (page) in the cache. For a size of 1024
	/// the actual page dimensions are 1024x1024. Small sizes can reduce performance by
	/// fragmenting the shadow maps, and reduce shadow quality by lowering the maximum
	/// resolution of each shadow map section.
	/// </summary>
	public int PageSize
	{
		get
		{
			return HCD.PageSize;
		}
		set
		{
			if (HCD.PageSize != value)
			{
				HCD.Resize(value, HCD.MaxMemoryUsage, HCD.PreferHalfFloatTextureFormat);
			}
		}
	}

	private int _MaxShadowLOD => HCD.PageSize >> 1;

	/// <summary>
	/// Creates a new or cached shadow map object for this light type.
	/// </summary>
	/// <param name="shadowsource">Shadow source which uses the newly created or cached shadow map object.
	/// Provides information about how the shadow is used, such as location and the type of objects rendered
	/// to the shadow map.</param>
	/// <returns></returns>
	protected abstract IShadowMap CreateDirectionalShadowMap(IShadowSource shadowsource);

	/// <summary>
	/// Creates a new or cached shadow map object for this light type.
	/// </summary>
	/// <param name="shadowsource">Shadow source which uses the newly created or cached shadow map object.
	/// Provides information about how the shadow is used, such as location and the type of objects rendered
	/// to the shadow map.</param>
	/// <returns></returns>
	protected abstract IShadowMap CreatePointShadowMap(IShadowSource shadowsource);

	/// <summary>
	/// Creates a new or cached shadow map object for this light type.
	/// </summary>
	/// <param name="shadowsource">Shadow source which uses the newly created or cached shadow map object.
	/// Provides information about how the shadow is used, such as location and the type of objects rendered
	/// to the shadow map.</param>
	/// <returns></returns>
	protected abstract IShadowMap CreateSpotShadowMap(IShadowSource shadowsource);

	/// <summary>
	/// Creates a new BaseShadowMapManager instance.
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
	public BaseShadowMapManager(IManagerServiceProvider sceneinterface, int pagesize, int maxmemoryusage, bool preferhalffloat)
		: base(sceneinterface)
	{
		HC6 = new Z._0001();
		HCD = new ShadowMapCache(pagesize, maxmemoryusage, preferhalffloat);
	}

	/// <summary>
	/// Creates a new BaseShadowMapManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	/// <param name="shadowmapcache"></param>
	public BaseShadowMapManager(IManagerServiceProvider sceneinterface, ShadowMapCache shadowmapcache)
		: base(sceneinterface)
	{
		HC6 = new Z._0001();
		HCD = shadowmapcache;
	}

	/// <summary>
	/// Creates a new BaseShadowMapManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	public BaseShadowMapManager(IManagerServiceProvider sceneinterface)
		: base(sceneinterface)
	{
		HC6 = new Z._0001();
		HCD = new ShadowMapCache(2048, 67108864, preferhalffloat: false);
	}

	/// <summary>
	/// Use to apply user quality and performance preferences to the resources managed by this object.
	/// </summary>
	/// <param name="preferences"></param>
	public override void ApplyPreferences(ISystemPreferences preferences)
	{
		HCw = preferences.ShadowDetail != DetailPreference.Off;
		HCZ = MathHelper.Clamp(preferences.ShadowQuality, 0.05f, 1f);
	}

	/// <summary>
	/// Organizes the provided lights into shadow and render target groups.
	/// </summary>
	/// <param name="rendertargetgroups">Returned render target groups.</param>
	/// <param name="lights">Lights to organize.</param>
	/// <param name="usedefaultgrouping">Determines if ungrouped lights should be placed in a
	/// single default group (recommended: true for deferred rendering and false for forward).</param>
	public void BuildShadows(List<ShadowRenderTargetGroup> rendertargetgroups, List<BaseLight> lights, bool usedefaultgrouping)
	{
		GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
		if (HCK == null)
		{
			HCK = new RenderTarget2D(graphicsDevice, 16, 16, mipMap: true, SurfaceFormat.Color, DepthFormat.None);
		}
		rendertargetgroups.Clear();
		HC_0003.Clear();
		HCs.Clear();
		BuildShadowGroups(HC_0003, lights, usedefaultgrouping);
		if (HC_0003.Count < 1)
		{
			return;
		}
		float num = (float)_MaxShadowLOD * HCZ * HCy;
		foreach (ShadowGroup item2 in HC_0003)
		{
			RenderTarget2D renderTarget2D = HCK;
			if (HCw && item2.ShadowSource.ShadowType != ShadowType.None)
			{
				ShadowSourceTypeCaster shadowSourceTypeCaster = item2.ShadowSourceTypes;
				IShadowMap shadowMap;
				if (shadowSourceTypeCaster.SpotSource != null)
				{
					shadowMap = CreateSpotShadowMap(item2.ShadowSource);
				}
				else if (shadowSourceTypeCaster.PointSource != null)
				{
					shadowMap = CreatePointShadowMap(item2.ShadowSource);
				}
				else
				{
					if (shadowSourceTypeCaster.DirectionalSource == null)
					{
						continue;
					}
					shadowMap = CreateDirectionalShadowMap(item2.ShadowSource);
				}
				shadowMap.Build(graphicsDevice, base.SceneState, item2, this, HCZ);
				if (shadowMap.CustomRenderTarget != null)
				{
					renderTarget2D = shadowMap.CustomRenderTarget;
				}
				else
				{
					float num2 = 1f;
					float num3 = num * item2.ShadowSource.ShadowQuality;
					Rectangle item = default(Rectangle);
					do
					{
						bool flag = true;
						HCk.Clear();
						ShadowMapSurface[] surfaces = shadowMap.Surfaces;
						foreach (ShadowMapSurface shadowMapSurface in surfaces)
						{
							float a = num3 * num2 * shadowMapSurface.LevelOfDetail;
							a = CoreHelper.Log2(a);
							a = (float)Math.Floor(a * 2f) * 0.5f;
							a = (float)Math.Pow(2.0, a);
							int num4 = (item.Height = (item.Width = (int)MathHelper.Clamp(a, 32f, _MaxShadowLOD)));
							HCk.Add(item);
							if (num4 > 32)
							{
								flag = false;
							}
						}
						renderTarget2D = HCD.ReserveSections(HCk);
						if (renderTarget2D == null)
						{
							HC_000F = _0001CB.High;
						}
						if (flag)
						{
							break;
						}
						num2 *= 0.5f;
					}
					while (renderTarget2D == null);
					if (renderTarget2D == null)
					{
						continue;
					}
					for (int j = 0; j < shadowMap.Surfaces.Length; j++)
					{
						shadowMap.SetSurfaceRenderTargetLocation(j, HCk[j]);
					}
				}
				item2.Shadow = shadowMap;
			}
			if (!HCs.ContainsKey(renderTarget2D))
			{
				ShadowRenderTargetGroup shadowRenderTargetGroup = HC_0011.New();
				shadowRenderTargetGroup.ShadowGroups.Clear();
				shadowRenderTargetGroup.ShadowGroups.Add(item2);
				HCs.Add(renderTarget2D, shadowRenderTargetGroup);
			}
			else
			{
				HCs[renderTarget2D].ShadowGroups.Add(item2);
			}
		}
		foreach (KeyValuePair<RenderTarget2D, ShadowRenderTargetGroup> hC in HCs)
		{
			ShadowRenderTargetGroup value = hC.Value;
			RenderTarget2D key = hC.Key;
			if (key == HCK)
			{
				value.Build(null);
			}
			else
			{
				value.Build(key);
			}
			rendertargetgroups.Add(value);
		}
	}

	/// <summary>
	/// Sets up frame information necessary for scene shadowing.
	/// </summary>
	public override void BeginFrameRendering(ISceneState scenestate)
	{
		if (HC6.Changed)
		{
			Unload();
		}
		base.BeginFrameRendering(scenestate);
	}

	/// <summary>
	/// Cleans up frame information including removing all reserved shadow maps.
	/// </summary>
	public override void EndFrameRendering()
	{
		if (HC_000F == _0001CB.High)
		{
			HCy *= 0.75f;
			HC_000F = _0001CB.Normal;
		}
		else if (HCy < 1f && (HC_000F == _0001CB.Low || HCD._73() < 0.33f))
		{
			HCy = Math.Min(HCy * 1.33f, 1f);
			HC_000F = _0001CB.Low;
		}
		HCD.ClearReserves();
		HC_0011.FreeAllTracked();
		base.EndFrameRendering();
	}

	/// <summary>
	/// Cleans up scene information.
	/// </summary>
	public override void Clear()
	{
	}

	/// <summary>
	/// Disposes any graphics resource used internally by this object, and removes
	/// scene resources managed by this object. Commonly used during Game.UnloadContent.
	/// </summary>
	public override void Unload()
	{
		HCD.Unload();
		HC_0011.Unload();
		F.B._7_0004(ref HCK);
	}
}
