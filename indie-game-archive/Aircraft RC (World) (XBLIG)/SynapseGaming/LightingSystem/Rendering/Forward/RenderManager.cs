using System;
using System.Collections.Generic;
using _000F;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Effects;
using SynapseGaming.LightingSystem.Effects.Forward;
using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Shadows;
using k;
using q;
using u;

namespace SynapseGaming.LightingSystem.Rendering.Forward;

/// <summary>
/// Provides a complete forward renderer.
/// </summary>
public class RenderManager : BaseRenderManager
{
	private bool HCB;

	private bool HC_0002 = true;

	private bool HC_0012;

	private int HCH = 1;

	private FogEffect HC7;

	private List<SceneEntity> HC_0001 = new List<SceneEntity>();

	private List<RenderableMesh> HCw = new List<RenderableMesh>();

	private List<u.w> HCZ = new List<u.w>();

	private List<u.w> HC_000F = new List<u.w>();

	private List<u.w> HCy = new List<u.w>();

	private CompositeLighting HC6 = default(CompositeLighting);

	private List<BaseLight> HCD = new List<BaseLight>();

	private Dictionary<object, CompositeLighting> HC_0011 = new Dictionary<object, CompositeLighting>(32);

	private Dictionary<object, CompositeLighting> HCK = new Dictionary<object, CompositeLighting>(32);

	private u.y HC_0003 = new u.y();

	private u._6 HCk = new u._6();

	private List<BaseLight> HCs = new List<BaseLight>();

	private List<RenderableMesh> HC_0013 = new List<RenderableMesh>();

	private new List<u.w> HCX = new List<u.w>();

	private List<u.w> HCz = new List<u.w>();

	private List<u.w> HCA = new List<u.w>();

	private static u._0001 HCc = new u._0001();

	private static u._7 HCY = new u._7();

	private static u._000F HCV = new u._000F();

	/// <summary>
	/// Cleans up shimmering effects on object edges. Requires a
	/// depth buffer format that supports stencil tests. Improper
	/// depth buffer formats will disable the feature.
	/// </summary>
	public bool MultiPassEdgeCleanupEnabled
	{
		get
		{
			return HC_0002;
		}
		set
		{
			HC_0002 = value;
		}
	}

	/// <summary>
	/// Creates a new RenderManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	public RenderManager(IManagerServiceProvider sceneinterface)
		: base(sceneinterface)
	{
		AmbientLight item = new AmbientLight
		{
			DiffuseColor = Vector3.Zero
		};
		HCD.Add(item);
	}

	/// <summary>
	/// Builds all object batches, shadow maps, and cached information before rendering.
	/// Any object added to the RenderManager after this call will not be visible during the frame.
	/// </summary>
	/// <param name="scenestate"></param>
	public override void BeginFrameRendering(ISceneState scenestate)
	{
		LightingSystemPerformance.Begin("RenderManager.BeginFrameRendering");
		HC_0003._76();
		HCk._76();
		base.BeginFrameRendering(scenestate);
		_ = scenestate.ViewToWorld.Translation;
		GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
		RasterizerState rasterizerState = graphicsDevice.RasterizerState;
		if (rasterizerState == null)
		{
			rasterizerState = RasterizerState.CullCounterClockwise;
		}
		HC_0012 = false;
		if (HC_0002)
		{
			RenderTargetBinding[] renderTargets = graphicsDevice.GetRenderTargets();
			if (renderTargets.Length > 0)
			{
				if (renderTargets[0].RenderTarget is RenderTarget2D renderTarget2D)
				{
					HC_0012 = renderTarget2D.DepthStencilFormat == DepthFormat.Depth24Stencil8;
				}
			}
			else
			{
				HC_0012 = graphicsDevice.PresentationParameters.DepthStencilFormat == DepthFormat.Depth24Stencil8;
			}
		}
		if (HC7 == null)
		{
			HC7 = new FogEffect(graphicsDevice);
		}
		LightingSystemPerformance.Begin("RenderManager.BeginFrameRendering (find objects)");
		HC_0001.Clear();
		HCw.Clear();
		HC_0011.Clear();
		HCK.Clear();
		IObjectManager objectManager = (IObjectManager)base.ServiceProvider.GetManager(SceneInterface.ObjectManagerType, required: false);
		ILightManager lightmanager = (ILightManager)base.ServiceProvider.GetManager(SceneInterface.LightManagerType, required: false);
		objectManager?.Find(HC_0001, base.SceneState.ViewFrustum, ObjectFilter.DynamicAndStatic);
		LightingSystemPerformance.Begin("RenderManager.BeginFrameRendering (filter objects)");
		ExtractAndFilterRenderableMeshes(HC_0001, lightmanager, HCB, HCw, HC_0011, HCK);
		LightingSystemPerformance.Begin("RenderManager.BeginFrameRendering (sort and batch)");
		HCw.Sort(HCc);
		HCV._7_000F(base.SceneState.View, base.SceneState.ViewToWorld, base.SceneState.Projection, base.SceneState.ProjectionToView, HCZ, HCw, u._0012.LightingEffect | u._0012.BasicEffect_Lighting | u._0012.BasicEffect_NonLighting | u._0012.MiscEffect);
		HCV._7_000F(base.SceneState.View, base.SceneState.ViewToWorld, base.SceneState.Projection, base.SceneState.ProjectionToView, HC_000F, HCw, u._0012.LightingEffect);
		HCV._7_000F(base.SceneState.View, base.SceneState.ViewToWorld, base.SceneState.Projection, base.SceneState.ProjectionToView, HCy, HCw, u._0012.LightingEffect | u._0012.BasicEffect_Lighting);
		LightingSystemPerformance.Begin("RenderManager.BeginFrameRendering (shadows)");
		List<ShadowRenderTargetGroup> frameShadowRenderTargetGroups = base.FrameShadowRenderTargetGroups;
		frameShadowRenderTargetGroups.Clear();
		IShadowMapManager shadowMapManager = (IShadowMapManager)base.ServiceProvider.GetManager(SceneInterface.ShadowMapManagerType, required: false);
		if (shadowMapManager == null)
		{
			GetDefaultShadows(frameShadowRenderTargetGroups, base.FrameLights);
		}
		else
		{
			shadowMapManager.BuildShadows(frameShadowRenderTargetGroups, base.FrameLights, usedefaultgrouping: false);
		}
		if (base.ShadowDetail != DetailPreference.Off && objectManager != null)
		{
			graphicsDevice.BlendState = BlendState.Opaque;
			graphicsDevice.DepthStencilState = DepthStencilState.Default;
			BuildShadowMaps(frameShadowRenderTargetGroups);
		}
		if (base.ClearBackBufferEnabled)
		{
			LightingSystemPerformance.Begin("RenderManager.BeginFrameRendering (clear backbuffer)");
			Color color = new Color(scenestate.Environment.FogColor);
			ClearOptions clearOptions = ClearOptions.Target | ClearOptions.DepthBuffer;
			if (HC_0002 && HC_0012)
			{
				clearOptions |= ClearOptions.Stencil;
			}
			graphicsDevice.Clear(clearOptions, color, 1f, 0);
		}
		graphicsDevice.RasterizerState = rasterizerState;
	}

	/// <summary>
	/// Generates shadow maps for the provided shadow render groups. Override this
	/// method to customize shadow map generation.
	/// </summary>
	/// <param name="shadowrendertargetgroups">Shadow render groups to generate shadow maps for.</param>
	protected override void BuildShadowMaps(List<ShadowRenderTargetGroup> shadowrendertargetgroups)
	{
		GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
		IObjectManager objectManager = (IObjectManager)base.ServiceProvider.GetManager(SceneInterface.ObjectManagerType, required: false);
		IAvatarManager avatarManager = (IAvatarManager)base.ServiceProvider.GetManager(SceneInterface.AvatarManagerType, required: false);
		foreach (ShadowRenderTargetGroup shadowrendertargetgroup in shadowrendertargetgroups)
		{
			if (!shadowrendertargetgroup.HasShadows() || shadowrendertargetgroup.ContentsAreValid)
			{
				continue;
			}
			base.HCX.HC6.AccumulationValue++;
			shadowrendertargetgroup.Begin();
			foreach (ShadowGroup shadowGroup in shadowrendertargetgroup.ShadowGroups)
			{
				IShadowMap shadow = shadowGroup.Shadow;
				if (shadow == null)
				{
					continue;
				}
				base.HCX.HCD.AccumulationValue++;
				HC_0013.Clear();
				objectManager.Find(objectfilter: (shadowGroup.ShadowSource.ShadowType != ShadowType.AllObjects) ? ObjectFilter.Static : ObjectFilter.DynamicAndStatic, foundobjects: HC_0013, worldbounds: shadowGroup.BoundingBox);
				HC_0013.Sort(HCY);
				HCV._7y(HCX, HC_0013, false, HCB);
				avatarManager?.BeginShadowGroupRendering(shadowGroup);
				_ = shadowGroup.ShadowSource.ShadowPosition;
				for (int i = 0; i < shadow.Surfaces.Length; i++)
				{
					ShadowMapSurface shadowMapSurface = shadow.Surfaces[i];
					base.HCX.HC_0011.AccumulationValue++;
					if (!shadow.IsSurfaceVisible(i, base.SceneState.ViewFrustum))
					{
						continue;
					}
					shadow.BeginSurfaceRendering(i);
					LightingSystemPerformance.Begin("RenderManager.BuildShadowMaps (object filter loop)");
					u.Z._7Z(HC_0013, shadowMapSurface.Frustum);
					foreach (u.w item in HCX)
					{
						if (item.HCB)
						{
							EffectTypeCaster effectTypeCaster = OptimizationSystem.EffectTypeCasters.Get(item.HC_0001);
							EffectTypeCaster effectTypeCaster2 = OptimizationSystem.EffectTypeCasters.Get(shadow.ShadowEffect);
							EffectHelper._0012C(effectTypeCaster, effectTypeCaster2);
							if (effectTypeCaster2.SkinnedEffect != null)
							{
								effectTypeCaster2.SkinnedEffect.Skinned = item.HC_0002;
							}
							_7_0012(graphicsDevice, item.Objects, effectTypeCaster2, null, true, true, true, _0001CB.Solid, FillMode.Solid, false, false, effectTypeCaster.TerrainEffect != null);
						}
					}
					if (avatarManager != null && avatarManager.RenderToShadowMapSurface(shadowGroup, shadowMapSurface, shadow.ShadowEffect))
					{
						HCk._76();
						HC_0003._76();
					}
					shadow.EndSurfaceRendering();
					base.HCX.HCK.AccumulationValue++;
				}
				avatarManager?.EndShadowGroupRendering(shadowGroup);
				shadow.ContentsAreValid = true;
			}
			shadowrendertargetgroup.End();
		}
	}

	/// <summary>
	/// Renders the scene.
	/// </summary>
	public override void Render()
	{
		if (base.SceneState == null)
		{
			return;
		}
		LightingSystemPerformance.Begin("RenderManager.Render");
		GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
		ILightMapManager lightMapManager = (ILightMapManager)base.ServiceProvider.GetManager(SceneInterface.LightMapManagerType, required: false);
		HC_0003._76();
		HCk._76();
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		RasterizerState rasterizerState = graphicsDevice.RasterizerState;
		if (rasterizerState == null)
		{
			rasterizerState = RasterizerState.CullCounterClockwise;
		}
		ISceneState sceneState = base.SceneState;
		foreach (u.w item in HCZ)
		{
			EffectTypeCaster effectTypeCaster = OptimizationSystem.EffectTypeCasters.Get(item.HC_0001);
			if (effectTypeCaster.RenderableEffect != null)
			{
				effectTypeCaster.RenderableEffect.SetViewAndProjection(sceneState.View, sceneState.ViewToWorld, sceneState.Projection, sceneState.ProjectionToView);
			}
			else if (effectTypeCaster.EffectMatrices != null)
			{
				effectTypeCaster.EffectMatrices.View = sceneState.View;
				effectTypeCaster.EffectMatrices.Projection = sceneState.Projection;
			}
		}
		LightingSystemPerformance.Begin("RenderManager.Render (ambient)");
		_7_0002(graphicsDevice, HCZ, base.FrameAmbientLights, lightMapManager, false, base.RenderFillMode, false, false);
		graphicsDevice.DepthStencilState = q.B.HC_0001;
		LightingSystemPerformance.Begin("RenderManager.Render (lighting loop)");
		List<ShadowRenderTargetGroup> frameShadowRenderTargetGroups = base.FrameShadowRenderTargetGroups;
		foreach (ShadowRenderTargetGroup item2 in frameShadowRenderTargetGroups)
		{
			foreach (ShadowGroup shadowGroup in item2.ShadowGroups)
			{
				HC(graphicsDevice, item2, shadowGroup);
			}
		}
		if (base.SceneState.Environment.FogEnabled)
		{
			LightingSystemPerformance.Begin("RenderManager.Render (fog)");
			EffectTypeCaster effectTypeCaster2 = OptimizationSystem.EffectTypeCasters.Get(HC7);
			HC7.SetViewAndProjection(base.SceneState.View, base.SceneState.ViewToWorld, base.SceneState.Projection, base.SceneState.ProjectionToView);
			graphicsDevice.BlendState = BlendState.NonPremultiplied;
			graphicsDevice.DepthStencilState = q.B.HC_0001;
			HC7.StartDistance = base.SceneState.Environment.FogStartDistance;
			HC7.EndDistance = base.SceneState.Environment.FogEndDistance;
			HC7.Color = base.SceneState.Environment.FogColor;
			HCV._7y(HCA, HCw, false, HCB);
			foreach (u.w item3 in HCA)
			{
				EffectTypeCaster effectTypeCaster3 = OptimizationSystem.EffectTypeCasters.Get(item3.HC_0001);
				EffectHelper._0012C(effectTypeCaster3, effectTypeCaster2);
				HC7.Skinned = item3.HC_0002;
				_7_0012(graphicsDevice, item3.Objects, effectTypeCaster2, null, false, true, false, _0001CB.Solid, base.RenderFillMode, false, false, effectTypeCaster3.TerrainEffect != null);
			}
		}
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		graphicsDevice.RasterizerState = rasterizerState;
	}

	/// <summary>
	/// Finalizes rendering and cleans up frame information including removing all frame lifespan objects.
	/// </summary>
	public override void EndFrameRendering()
	{
		LightingSystemPerformance.Begin("RenderManager.EndFrameRendering");
		ISceneState sceneState = base.SceneState;
		foreach (SceneEntity item in HC_0001)
		{
			item.RenderCustomPass(sceneState);
		}
		base.EndFrameRendering();
		HCV.G();
	}

	/// <summary>
	/// Unloads all scene and device specific data.  Must be called
	/// when the device is reset (during Game.UnloadGraphicsContent()).
	/// </summary>
	public override void Unload()
	{
		if (HC7 != null)
		{
			HC7.Dispose();
			HC7 = null;
		}
		base.Unload();
	}

	/// <summary>
	/// Determines if the render manager allows transparent scene
	/// nodes of the same type and effect to be rendered together.
	///
	/// If not each transparent scene node will be rendered individually.
	/// </summary>
	/// <param name="scenestate"></param>
	/// <returns></returns>
	protected override bool CanBatchTransparencies(ISceneState scenestate)
	{
		if (scenestate.Environment.FogEnabled)
		{
			return false;
		}
		return true;
	}

	internal override void _0017dO_0016P(List<RenderableMesh> P_0, bool P_1)
	{
		if (P_0.Count <= 0)
		{
			return;
		}
		ISceneState sceneState = base.SceneState;
		_ = sceneState.Environment;
		ILightMapManager lightMapManager = (ILightMapManager)base.ServiceProvider.GetManager(SceneInterface.LightMapManagerType, required: false);
		GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
		EffectTypeCaster effectTypeCaster = OptimizationSystem.EffectTypeCasters.Get(P_0[0].Effect);
		_0001CB obj = _0001CB.TransparentDoubleSided;
		if (effectTypeCaster.EffectMatrices != null)
		{
			effectTypeCaster.EffectMatrices.View = sceneState.View;
			effectTypeCaster.EffectMatrices.Projection = sceneState.Projection;
			graphicsDevice.BlendState = BlendState.NonPremultiplied;
		}
		if (effectTypeCaster.RenderableEffect != null)
		{
			effectTypeCaster.RenderableEffect.SetViewAndProjection(sceneState.View, sceneState.ViewToWorld, sceneState.Projection, sceneState.ProjectionToView);
			obj = ((effectTypeCaster.RenderableEffect == null || !effectTypeCaster.RenderableEffect.DoubleSided) ? _0001CB.TransparentSingleSided : _0001CB.TransparentDoubleSided);
			if (effectTypeCaster.TransparentEffect != null && effectTypeCaster.TransparentEffect.TransparencyMode == TransparencyMode.Additive)
			{
				graphicsDevice.BlendState = BlendState.Additive;
			}
			else
			{
				graphicsDevice.BlendState = BlendState.NonPremultiplied;
			}
		}
		if (effectTypeCaster.LightingEffect != null)
		{
			effectTypeCaster.LightingEffect.LightSources = HCD;
		}
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		if (P_1)
		{
			HC_0003._76();
			HCk._76();
		}
		_7_0012(graphicsDevice, P_0, effectTypeCaster, lightMapManager, false, true, false, obj, base.RenderFillMode, false, false, false);
		if (base.SceneState.Environment.FogEnabled)
		{
			EffectTypeCaster effectTypeCaster2 = OptimizationSystem.EffectTypeCasters.Get(HC7);
			HC7.SetViewAndProjection(base.SceneState.View, base.SceneState.ViewToWorld, base.SceneState.Projection, base.SceneState.ProjectionToView);
			graphicsDevice.BlendState = BlendState.NonPremultiplied;
			graphicsDevice.DepthStencilState = q.B.HC_0001;
			HC7.StartDistance = base.SceneState.Environment.FogStartDistance;
			HC7.EndDistance = base.SceneState.Environment.FogEndDistance;
			HC7.Color = base.SceneState.Environment.FogColor;
			EffectHelper._0012C(effectTypeCaster, effectTypeCaster2);
			if (effectTypeCaster.SkinnedEffect != null)
			{
				HC7.Skinned = effectTypeCaster.SkinnedEffect.Skinned;
			}
			else
			{
				HC7.Skinned = false;
			}
			_7_0012(graphicsDevice, P_0, effectTypeCaster2, null, false, true, false, obj, base.RenderFillMode, false, false, effectTypeCaster.TerrainEffect != null);
		}
	}

	private void HC(GraphicsDevice P_0, ShadowRenderTargetGroup P_1, ShadowGroup P_2)
	{
		if (P_2.Lights.Count < 1 || HCw.Count < 1)
		{
			return;
		}
		base.HCX.HCy.AccumulationValue++;
		base.HCX.HCZ.AccumulationValue += P_2.Lights.Count;
		bool flag = P_2.ShadowSourceTypes.PointSource != null;
		List<u.w> list = (flag ? HC_000F : HCy);
		LightingSystemPerformance.Begin("RenderManager.RenderShadowGroup (object filter loop)");
		foreach (u.w item in list)
		{
			if (flag)
			{
				item.HCB = u.Z._7w(item.Objects, P_2.BoundingBox);
				continue;
			}
			item.HCB = true;
			foreach (RenderableMesh item2 in item.Objects)
			{
				item2.HCY = true;
			}
		}
		if (flag)
		{
			Rectangle screenArea = CoreHelper.GetScreenArea(P_2.BoundingBox, P_0.Viewport, base.SceneState.ViewProjection, base.SceneState.ViewToWorld);
			if ((float)screenArea.Width <= 0f || (float)screenArea.Height <= 0f)
			{
				return;
			}
			P_0.ScissorRectangle = screenArea;
		}
		if (base.ShadowDetail != DetailPreference.Off)
		{
			IShadowMap shadow = P_2.Shadow;
			if (shadow != null && shadow.ShadowEffect is IRenderableEffect)
			{
				HCz.Clear();
				HCV._7y(HCz, HCw, true, HCB);
				P_0.BlendState = q.B.HC_0012;
				P_0.DepthStencilState = q.B.HC_0001;
				foreach (u.w item3 in HCz)
				{
					if (item3.HCB)
					{
						EffectTypeCaster effectTypeCaster = OptimizationSystem.EffectTypeCasters.Get(item3.HC_0001);
						EffectTypeCaster effectTypeCaster2 = OptimizationSystem.EffectTypeCasters.Get(shadow.ShadowEffect);
						EffectHelper._0012C(effectTypeCaster, effectTypeCaster2);
						_7B(P_0, P_1, P_2, shadow, effectTypeCaster2, item3, flag, effectTypeCaster.TerrainEffect != null);
					}
				}
				P_0.BlendState = q.B.HCH;
			}
			else
			{
				P_0.BlendState = q.B.HCB;
			}
		}
		else
		{
			P_0.BlendState = q.B.HCB;
		}
		if (P_2.Lights.Count == 1 && HC_0002 && HC_0012)
		{
			P_0.DepthStencilState = q.B.HCy;
			P_0.ReferenceStencil = HCH;
			HCH++;
			if (HCH > 250)
			{
				HCH = 1;
			}
		}
		else
		{
			P_0.DepthStencilState = q.B.HC_0001;
		}
		_7_0002(P_0, list, P_2.Lights, null, true, base.RenderFillMode, flag, false);
	}

	private void _7B(GraphicsDevice P_0, ShadowRenderTargetGroup P_1, ShadowGroup P_2, IShadowMap P_3, EffectTypeCaster P_4, u.w P_5, bool P_6, bool P_7)
	{
		if (P_5.Objects.Count >= 1 && P_2.Lights.Count >= 1 && P_3 != null)
		{
			ISkinnedEffect skinnedEffect = P_4.SkinnedEffect;
			if (P_4.Effect is k.B b)
			{
				b.EffectDetail = base.ShadowDetail;
			}
			P_3.BeginRendering(P_1.RenderTarget);
			if (skinnedEffect != null)
			{
				skinnedEffect.Skinned = P_5.HC_0002;
			}
			_7_0012(P_0, P_5.Objects, P_4, null, false, true, false, _0001CB.Solid, base.RenderFillMode, P_6, true, P_7);
			P_3.EndRendering();
		}
	}

	private void _7_0002(GraphicsDevice P_0, List<u.w> P_1, List<BaseLight> P_2, ILightMapManager P_3, bool P_4, FillMode P_5, bool P_6, bool P_7)
	{
		LightingSystemPerformance.Begin("RenderManager.RenderObjectBatches");
		foreach (u.w item in P_1)
		{
			if (P_4 && !item.HCB)
			{
				continue;
			}
			EffectTypeCaster effectTypeCaster = OptimizationSystem.EffectTypeCasters.Get(item.HC_0001);
			bool flag = effectTypeCaster.TerrainEffect != null;
			if (effectTypeCaster.RenderableEffect != null)
			{
				effectTypeCaster.RenderableEffect.EffectDetail = base.EffectDetail;
			}
			else
			{
				IEffectLights effectLights = effectTypeCaster.EffectLights;
				if (effectLights != null && effectLights.LightingEnabled && P_2.Count > 0)
				{
					BaseLight baseLight = P_2[0];
					LightTypeCaster lightTypeCaster = OptimizationSystem.LightTypeCasters.Get(baseLight);
					if (lightTypeCaster.DirectionalSource != null && lightTypeCaster.SpotSource == null)
					{
						effectLights.AmbientLightColor = default(Vector3);
						effectLights.DirectionalLight0.Enabled = true;
						effectLights.DirectionalLight0.DiffuseColor = baseLight.CompositeColorAndIntensity;
						effectLights.DirectionalLight0.Direction = lightTypeCaster.DirectionalSource.Direction;
						effectLights.DirectionalLight1.Enabled = false;
						effectLights.DirectionalLight2.Enabled = false;
					}
					else
					{
						if (lightTypeCaster.AmbientSource == null)
						{
							throw new ArgumentException("BasicEffect / IEffectLights can only render directional lights.");
						}
						effectLights.AmbientLightColor = baseLight.CompositeColorAndIntensity;
						effectLights.DirectionalLight0.Enabled = false;
						effectLights.DirectionalLight1.Enabled = false;
						effectLights.DirectionalLight2.Enabled = false;
					}
				}
			}
			_ = item.HC_0001;
			ILightingEffect lightingEffect = effectTypeCaster.LightingEffect;
			if (P_2.Count > 0 && lightingEffect != null)
			{
				int maxLightSources = lightingEffect.MaxLightSources;
				if (P_2.Count > maxLightSources)
				{
					HCs.Clear();
					for (int i = 0; i < P_2.Count; i++)
					{
						HCs.Add(P_2[i]);
						if (HCs.Count >= maxLightSources || i + 1 >= P_2.Count)
						{
							lightingEffect.LightSources = HCs;
							_7_0012(P_0, item.Objects, effectTypeCaster, P_3, P_4, true, false, _0001CB.Solid, P_5, P_6, P_7, flag);
							HCs.Clear();
						}
					}
				}
				else
				{
					lightingEffect.LightSources = P_2;
					_7_0012(P_0, item.Objects, effectTypeCaster, P_3, P_4, true, false, _0001CB.Solid, P_5, P_6, P_7, flag);
				}
			}
			else
			{
				_7_0012(P_0, item.Objects, effectTypeCaster, P_3, P_4, true, false, _0001CB.Solid, P_5, P_6, P_7, flag);
			}
		}
	}

	private void _7_0012(GraphicsDevice P_0, List<RenderableMesh> P_1, EffectTypeCaster P_2, ILightMapManager P_3, bool P_4, bool P_5, bool P_6, _0001CB P_7, FillMode P_8, bool P_9, bool P_10, bool P_11)
	{
		if (P_1.Count < 1)
		{
			return;
		}
		LightingSystemPerformance.Begin("RenderManager.RenderObjectBatch");
		if (P_4)
		{
			bool flag = false;
			foreach (RenderableMesh item in P_1)
			{
				if (item.HCY && item.HC_0002.Valid)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
		}
		bool flag2 = false;
		CullMode cullMode = CullMode.CullCounterClockwiseFace;
		if (base.SceneState.InvertedWindings)
		{
			P_6 = !P_6;
		}
		bool flag3 = P_7 == _0001CB.TransparentDoubleSided;
		if (flag3)
		{
			q.B.Hm(P_0, P_8, cullMode, !P_6, false, false);
		}
		else
		{
			if (P_2.RenderableEffect != null && P_7 == _0001CB.Solid)
			{
				flag2 = P_2.RenderableEffect.DoubleSided;
			}
			q.B.Hm(P_0, P_8, cullMode, P_6, flag2, false);
		}
		SamplerState samplerState = null;
		if (P_5 && !P_10)
		{
			if (P_2.AddressableEffect != null)
			{
				IAddressableEffect addressableEffect = P_2.AddressableEffect;
				samplerState = HC_0003._7K(P_0, addressableEffect.AddressModeU, addressableEffect.AddressModeV, addressableEffect.AddressModeW, base.Filter, base.MaxAnisotropy);
			}
			else
			{
				samplerState = HC_0003._7K(P_0, TextureAddressMode.Wrap, TextureAddressMode.Wrap, TextureAddressMode.Wrap, base.Filter, base.MaxAnisotropy);
			}
		}
		else
		{
			HC_0003._7_0011(P_0, SamplerState.PointClamp);
		}
		EffectPassCollection passes = P_2.Effect.CurrentTechnique.Passes;
		base.HCX.HCH.AccumulationValue++;
		base.HCX.HC7.AccumulationValue += passes.Count;
		IEffectMatrices effectMatrices = P_2.EffectMatrices;
		IEffectLights effectLights = P_2.EffectLights;
		ISkinnedEffect skinnedEffect = P_2.SkinnedEffect;
		IRenderableEffect renderableEffect = P_2.RenderableEffect;
		BaseRenderableEffect baseRenderableEffect = P_2.BaseRenderableEffect;
		IStaticLightingEffect staticLightingEffect = P_2.StaticLightingEffect;
		if (P_2.BaseSasEffect != null || P_11)
		{
			HC_0003._76();
		}
		bool flag4 = true;
		bool flag5 = (staticLightingEffect != null || effectLights != null) && P_3 != null;
		Dictionary<object, CompositeLighting> dictionary = ((P_7 == _0001CB.Solid) ? HC_0011 : HCK);
		for (int i = 0; i < passes.Count; i++)
		{
			EffectPass effectPass = passes[i];
			foreach (RenderableMesh item2 in P_1)
			{
				if (item2 == null || (P_4 && !item2.HCY) || !item2.HC_0002.Valid)
				{
					continue;
				}
				bool flag6 = false;
				if (flag5)
				{
					CompositeLighting value;
					if (staticLightingEffect != null)
					{
						StaticLightingType staticLightingType = item2.HC_0002.StaticLightingType;
						if (staticLightingType == StaticLightingType.Composite || staticLightingType == StaticLightingType.Custom || P_7 != _0001CB.Solid)
						{
							if (!dictionary.TryGetValue(item2.HC_0002, out value))
							{
								value = HC6;
							}
							if (staticLightingType == StaticLightingType.BakedDown)
							{
								staticLightingEffect.SetStaticLighting(StaticLightingEffectMode.BakedDownAndComposite, P_3.GetLightMap(item2), ref value);
							}
							else
							{
								staticLightingEffect.SetStaticLighting(StaticLightingEffectMode.Composite, null, ref value);
							}
						}
						else if (staticLightingType == StaticLightingType.BakedDown)
						{
							staticLightingEffect.SetStaticLighting(StaticLightingEffectMode.BakedDown, P_3.GetLightMap(item2));
						}
						else
						{
							staticLightingEffect.SetStaticLighting(StaticLightingEffectMode.Ambient, null);
						}
					}
					else if (effectLights != null)
					{
						if (!dictionary.TryGetValue(item2.HC_0002, out value))
						{
							value = HC6;
						}
						effectLights.AmbientLightColor = value.AmbientColor;
						effectLights.DirectionalLight0.DiffuseColor = value.DiffuseColor;
						effectLights.DirectionalLight0.Direction = value.Direction;
					}
					flag6 = true;
				}
				if (effectMatrices != null)
				{
					effectMatrices.World = item2.HCD;
					flag6 = true;
				}
				else
				{
					if (skinnedEffect != null)
					{
						skinnedEffect.SkinBones = item2.HC_0002.SkinBones;
						flag6 = true;
					}
					if (renderableEffect != null)
					{
						renderableEffect.SetWorldAndWorldToObject(ref item2.HCD, ref item2.HC_0011);
						flag6 = true;
					}
					if (baseRenderableEffect != null)
					{
						flag6 = baseRenderableEffect.UpdatedByBatch;
						baseRenderableEffect.UpdatedByBatch = false;
					}
				}
				if (flag6 || flag4)
				{
					effectPass.Apply();
					base.HCX.HCw.AccumulationValue++;
				}
				if (!flag2 && cullMode != item2.HCc)
				{
					cullMode = item2.HCc;
					if (flag3)
					{
						q.B.Hm(P_0, P_8, cullMode, !P_6, false, false);
					}
					else
					{
						q.B.Hm(P_0, P_8, cullMode, P_6, flag2, P_9);
					}
					base.HCX.HC_0001.AccumulationValue++;
				}
				HCk.R(P_0, item2);
				try
				{
					if (flag4 && samplerState != null)
					{
						HC_0003._7_0011(P_0, samplerState);
					}
					if (item2.HCK == null)
					{
						P_0.DrawPrimitives(item2.HCz, item2.HCX, item2.HCA);
					}
					else
					{
						P_0.DrawIndexedPrimitives(item2.HCz, item2.HCs, 0, item2.HC_0013, item2.HCX, item2.HCA);
					}
					if (flag3)
					{
						q.B.Hm(P_0, P_8, cullMode, P_6, false, false);
						cullMode = CullMode.None;
						if (item2.HCK == null)
						{
							P_0.DrawPrimitives(item2.HCz, item2.HCX, item2.HCA);
						}
						else
						{
							P_0.DrawIndexedPrimitives(item2.HCz, item2.HCs, 0, item2.HC_0013, item2.HCX, item2.HCA);
						}
					}
				}
				catch (Exception ex)
				{
					item2.HC_0002.Valid = false;
					item2.HC_0002.RenderingErrors = ex.Message;
					SunBurnEditor._0012X(item2.HC_0002, _000F._0012.Error);
					OnXNARuntimeException(ex, $"Unable to render object '{item2.HC_0002.Name}'.");
				}
				flag4 = false;
				base.HCX.HC_0012.AccumulationValue++;
				base.HCX.HCB.AccumulationValue += item2.HCA;
			}
		}
		if (P_2.BaseSasEffect != null || P_11)
		{
			HC_0003._76();
		}
	}
}
