using System;
using System.Collections.Generic;
using _0010;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Effects;
using SynapseGaming.LightingSystem.Effects.Forward;
using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Shadows;
using X;
using Z;
using l;

namespace SynapseGaming.LightingSystem.Rendering.Forward;

/// <summary>
/// Provides a complete forward renderer.
/// </summary>
public class RenderManager : BaseRenderManager
{
	private bool _3A_0018;

	private bool _3AL = true;

	private bool _3A_0019;

	private int _3A3 = 1;

	private FogEffect _3A6;

	private List<SceneEntity> _3AD = new List<SceneEntity>();

	private List<RenderableMesh> _3A_0017 = new List<RenderableMesh>();

	private List<X._0017> _3A_0003 = new List<X._0017>();

	private List<X._0017> _3Al = new List<X._0017>();

	private List<X._0017> _3At = new List<X._0017>();

	private CompositeLighting _3AF = default(CompositeLighting);

	private List<BaseLight> _3Ac = new List<BaseLight>();

	private Dictionary<object, CompositeLighting> _3Ag = new Dictionary<object, CompositeLighting>(32);

	private Dictionary<object, CompositeLighting> _3AI = new Dictionary<object, CompositeLighting>(32);

	private X.t _3A8 = new X.t();

	private X.F _3AZ = new X.F();

	private List<BaseLight> _3Ax = new List<BaseLight>();

	private List<RenderableMesh> _3Aq = new List<RenderableMesh>();

	private new List<X._0017> _3Ab = new List<X._0017>();

	private List<X._0017> _3AT = new List<X._0017>();

	private List<X._0017> _3Ay = new List<X._0017>();

	private static X.D _3A_0015 = new X.D();

	private static X._6 _3A_0001 = new X._6();

	private static X.l _3A7 = new X.l();

	/// <summary>
	/// Cleans up shimmering effects on object edges. Requires a
	/// depth buffer format that supports stencil tests. Improper
	/// depth buffer formats will disable the feature.
	/// </summary>
	public bool MultiPassEdgeCleanupEnabled
	{
		get
		{
			return _3AL;
		}
		set
		{
			_3AL = value;
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
		_3Ac.Add(item);
	}

	/// <summary>
	/// Builds all object batches, shadow maps, and cached information before rendering.
	/// Any object added to the RenderManager after this call will not be visible during the frame.
	/// </summary>
	/// <param name="scenestate"></param>
	public override void BeginFrameRendering(ISceneState scenestate)
	{
		LightingSystemPerformance.Begin("RenderManager.BeginFrameRendering");
		_3A8._6F();
		_3AZ._6F();
		base.BeginFrameRendering(scenestate);
		_ = scenestate.ViewToWorld.Translation;
		GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
		RasterizerState rasterizerState = graphicsDevice.RasterizerState;
		if (rasterizerState == null)
		{
			rasterizerState = RasterizerState.CullCounterClockwise;
		}
		_3A_0019 = false;
		if (_3AL)
		{
			RenderTargetBinding[] renderTargets = graphicsDevice.GetRenderTargets();
			if (renderTargets.Length > 0)
			{
				if (renderTargets[0].RenderTarget is RenderTarget2D renderTarget2D)
				{
					_3A_0019 = renderTarget2D.DepthStencilFormat == DepthFormat.Depth24Stencil8;
				}
			}
			else
			{
				_3A_0019 = graphicsDevice.PresentationParameters.DepthStencilFormat == DepthFormat.Depth24Stencil8;
			}
		}
		if (_3A6 == null)
		{
			_3A6 = new FogEffect(graphicsDevice);
		}
		LightingSystemPerformance.Begin("RenderManager.BeginFrameRendering (find objects)");
		_3AD.Clear();
		_3A_0017.Clear();
		_3Ag.Clear();
		_3AI.Clear();
		IObjectManager objectManager = (IObjectManager)base.ServiceProvider.GetManager(SceneInterface.ObjectManagerType, required: false);
		ILightManager lightmanager = (ILightManager)base.ServiceProvider.GetManager(SceneInterface.LightManagerType, required: false);
		objectManager?.Find(_3AD, base.SceneState.ViewFrustum, ObjectFilter.DynamicAndStatic);
		LightingSystemPerformance.Begin("RenderManager.BeginFrameRendering (filter objects)");
		ExtractAndFilterRenderableMeshes(_3AD, lightmanager, _3A_0018, _3A_0017, _3Ag, _3AI);
		LightingSystemPerformance.Begin("RenderManager.BeginFrameRendering (sort and batch)");
		_3A_0017.Sort(_3A_0015);
		_3A7._6l(base.SceneState.View, base.SceneState.ViewToWorld, base.SceneState.Projection, base.SceneState.ProjectionToView, _3A_0003, _3A_0017, X._0019.LightingEffect | X._0019.BasicEffect_Lighting | X._0019.BasicEffect_NonLighting | X._0019.MiscEffect);
		_3A7._6l(base.SceneState.View, base.SceneState.ViewToWorld, base.SceneState.Projection, base.SceneState.ProjectionToView, _3Al, _3A_0017, X._0019.LightingEffect);
		_3A7._6l(base.SceneState.View, base.SceneState.ViewToWorld, base.SceneState.Projection, base.SceneState.ProjectionToView, _3At, _3A_0017, X._0019.LightingEffect | X._0019.BasicEffect_Lighting);
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
			if (_3AL && _3A_0019)
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
			base._3Ab._3AF.AccumulationValue++;
			shadowrendertargetgroup.Begin();
			foreach (ShadowGroup shadowGroup in shadowrendertargetgroup.ShadowGroups)
			{
				IShadowMap shadow = shadowGroup.Shadow;
				if (shadow == null)
				{
					continue;
				}
				base._3Ab._3Ac.AccumulationValue++;
				_3Aq.Clear();
				objectManager.Find(objectfilter: (shadowGroup.ShadowSource.ShadowType != ShadowType.AllObjects) ? ObjectFilter.Static : ObjectFilter.DynamicAndStatic, foundobjects: _3Aq, worldbounds: shadowGroup.BoundingBox);
				_3Aq.Sort(_3A_0001);
				_3A7._6t(_3Ab, _3Aq, false, _3A_0018);
				avatarManager?.BeginShadowGroupRendering(shadowGroup);
				_ = shadowGroup.ShadowSource.ShadowPosition;
				for (int i = 0; i < shadow.Surfaces.Length; i++)
				{
					ShadowMapSurface shadowMapSurface = shadow.Surfaces[i];
					base._3Ab._3Ag.AccumulationValue++;
					if (!shadow.IsSurfaceVisible(i, base.SceneState.ViewFrustum))
					{
						continue;
					}
					shadow.BeginSurfaceRendering(i);
					LightingSystemPerformance.Begin("RenderManager.BuildShadowMaps (object filter loop)");
					X._0003._6_0003(_3Aq, shadowMapSurface.Frustum);
					foreach (X._0017 item in _3Ab)
					{
						if (item._3A_0018)
						{
							EffectTypeCaster effectTypeCaster = OptimizationSystem.EffectTypeCasters.Get(item._3AD);
							EffectTypeCaster effectTypeCaster2 = OptimizationSystem.EffectTypeCasters.Get(shadow.ShadowEffect);
							EffectHelper._0019j(effectTypeCaster, effectTypeCaster2);
							if (effectTypeCaster2.SkinnedEffect != null)
							{
								effectTypeCaster2.SkinnedEffect.Skinned = item._3AL;
							}
							_6_0019(graphicsDevice, item.Objects, effectTypeCaster2, null, true, true, true, DA_0018.Solid, FillMode.Solid, false, false, effectTypeCaster.TerrainEffect != null);
						}
					}
					if (avatarManager != null && avatarManager.RenderToShadowMapSurface(shadowGroup, shadowMapSurface, shadow.ShadowEffect))
					{
						_3AZ._6F();
						_3A8._6F();
					}
					shadow.EndSurfaceRendering();
					base._3Ab._3AI.AccumulationValue++;
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
		_3A8._6F();
		_3AZ._6F();
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		RasterizerState rasterizerState = graphicsDevice.RasterizerState;
		if (rasterizerState == null)
		{
			rasterizerState = RasterizerState.CullCounterClockwise;
		}
		ISceneState sceneState = base.SceneState;
		foreach (X._0017 item in _3A_0003)
		{
			EffectTypeCaster effectTypeCaster = OptimizationSystem.EffectTypeCasters.Get(item._3AD);
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
		_6L(graphicsDevice, _3A_0003, base.FrameAmbientLights, lightMapManager, false, base.RenderFillMode, false, false);
		graphicsDevice.DepthStencilState = _0010._0018._3AD;
		LightingSystemPerformance.Begin("RenderManager.Render (lighting loop)");
		List<ShadowRenderTargetGroup> frameShadowRenderTargetGroups = base.FrameShadowRenderTargetGroups;
		foreach (ShadowRenderTargetGroup item2 in frameShadowRenderTargetGroups)
		{
			foreach (ShadowGroup shadowGroup in item2.ShadowGroups)
			{
				_3A(graphicsDevice, item2, shadowGroup);
			}
		}
		if (base.SceneState.Environment.FogEnabled)
		{
			LightingSystemPerformance.Begin("RenderManager.Render (fog)");
			EffectTypeCaster effectTypeCaster2 = OptimizationSystem.EffectTypeCasters.Get(_3A6);
			_3A6.SetViewAndProjection(base.SceneState.View, base.SceneState.ViewToWorld, base.SceneState.Projection, base.SceneState.ProjectionToView);
			graphicsDevice.BlendState = BlendState.NonPremultiplied;
			graphicsDevice.DepthStencilState = _0010._0018._3AD;
			_3A6.StartDistance = base.SceneState.Environment.FogStartDistance;
			_3A6.EndDistance = base.SceneState.Environment.FogEndDistance;
			_3A6.Color = base.SceneState.Environment.FogColor;
			_3A7._6t(_3Ay, _3A_0017, false, _3A_0018);
			foreach (X._0017 item3 in _3Ay)
			{
				EffectTypeCaster effectTypeCaster3 = OptimizationSystem.EffectTypeCasters.Get(item3._3AD);
				EffectHelper._0019j(effectTypeCaster3, effectTypeCaster2);
				_3A6.Skinned = item3._3AL;
				_6_0019(graphicsDevice, item3.Objects, effectTypeCaster2, null, false, true, false, DA_0018.Solid, base.RenderFillMode, false, false, effectTypeCaster3.TerrainEffect != null);
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
		foreach (SceneEntity item in _3AD)
		{
			item.RenderCustomPass(sceneState);
		}
		base.EndFrameRendering();
		_3A7.U();
	}

	/// <summary>
	/// Unloads all scene and device specific data.  Must be called
	/// when the device is reset (during Game.UnloadGraphicsContent()).
	/// </summary>
	public override void Unload()
	{
		if (_3A6 != null)
		{
			_3A6.Dispose();
			_3A6 = null;
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

	internal override void _000FS_0005_0004Y(List<RenderableMesh> P_0, bool P_1)
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
		DA_0018 obj = DA_0018.TransparentDoubleSided;
		if (effectTypeCaster.EffectMatrices != null)
		{
			effectTypeCaster.EffectMatrices.View = sceneState.View;
			effectTypeCaster.EffectMatrices.Projection = sceneState.Projection;
			graphicsDevice.BlendState = BlendState.NonPremultiplied;
		}
		if (effectTypeCaster.RenderableEffect != null)
		{
			effectTypeCaster.RenderableEffect.SetViewAndProjection(sceneState.View, sceneState.ViewToWorld, sceneState.Projection, sceneState.ProjectionToView);
			obj = ((effectTypeCaster.RenderableEffect == null || !effectTypeCaster.RenderableEffect.DoubleSided) ? DA_0018.TransparentSingleSided : DA_0018.TransparentDoubleSided);
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
			effectTypeCaster.LightingEffect.LightSources = _3Ac;
		}
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		if (P_1)
		{
			_3A8._6F();
			_3AZ._6F();
		}
		_6_0019(graphicsDevice, P_0, effectTypeCaster, lightMapManager, false, true, false, obj, base.RenderFillMode, false, false, false);
		if (base.SceneState.Environment.FogEnabled)
		{
			EffectTypeCaster effectTypeCaster2 = OptimizationSystem.EffectTypeCasters.Get(_3A6);
			_3A6.SetViewAndProjection(base.SceneState.View, base.SceneState.ViewToWorld, base.SceneState.Projection, base.SceneState.ProjectionToView);
			graphicsDevice.BlendState = BlendState.NonPremultiplied;
			graphicsDevice.DepthStencilState = _0010._0018._3AD;
			_3A6.StartDistance = base.SceneState.Environment.FogStartDistance;
			_3A6.EndDistance = base.SceneState.Environment.FogEndDistance;
			_3A6.Color = base.SceneState.Environment.FogColor;
			EffectHelper._0019j(effectTypeCaster, effectTypeCaster2);
			if (effectTypeCaster.SkinnedEffect != null)
			{
				_3A6.Skinned = effectTypeCaster.SkinnedEffect.Skinned;
			}
			else
			{
				_3A6.Skinned = false;
			}
			_6_0019(graphicsDevice, P_0, effectTypeCaster2, null, false, true, false, obj, base.RenderFillMode, false, false, effectTypeCaster.TerrainEffect != null);
		}
	}

	private void _3A(GraphicsDevice P_0, ShadowRenderTargetGroup P_1, ShadowGroup P_2)
	{
		if (P_2.Lights.Count < 1 || _3A_0017.Count < 1)
		{
			return;
		}
		base._3Ab._3At.AccumulationValue++;
		base._3Ab._3A_0003.AccumulationValue += P_2.Lights.Count;
		bool flag = P_2.ShadowSourceTypes.PointSource != null;
		List<X._0017> list = (flag ? _3Al : _3At);
		LightingSystemPerformance.Begin("RenderManager.RenderShadowGroup (object filter loop)");
		foreach (X._0017 item in list)
		{
			if (flag)
			{
				item._3A_0018 = X._0003._6_0017(item.Objects, P_2.BoundingBox);
				continue;
			}
			item._3A_0018 = true;
			foreach (RenderableMesh item2 in item.Objects)
			{
				item2._3Ax = true;
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
				_3AT.Clear();
				_3A7._6t(_3AT, _3A_0017, true, _3A_0018);
				P_0.BlendState = _0010._0018._3A_0019;
				P_0.DepthStencilState = _0010._0018._3AD;
				foreach (X._0017 item3 in _3AT)
				{
					if (item3._3A_0018)
					{
						EffectTypeCaster effectTypeCaster = OptimizationSystem.EffectTypeCasters.Get(item3._3AD);
						EffectTypeCaster effectTypeCaster2 = OptimizationSystem.EffectTypeCasters.Get(shadow.ShadowEffect);
						EffectHelper._0019j(effectTypeCaster, effectTypeCaster2);
						_6_0018(P_0, P_1, P_2, shadow, effectTypeCaster2, item3, flag, effectTypeCaster.TerrainEffect != null);
					}
				}
				P_0.BlendState = _0010._0018._3A3;
			}
			else
			{
				P_0.BlendState = _0010._0018._3A_0018;
			}
		}
		else
		{
			P_0.BlendState = _0010._0018._3A_0018;
		}
		if (P_2.Lights.Count == 1 && _3AL && _3A_0019)
		{
			P_0.DepthStencilState = _0010._0018._3At;
			P_0.ReferenceStencil = _3A3;
			_3A3++;
			if (_3A3 > 250)
			{
				_3A3 = 1;
			}
		}
		else
		{
			P_0.DepthStencilState = _0010._0018._3AD;
		}
		_6L(P_0, list, P_2.Lights, null, true, base.RenderFillMode, flag, false);
	}

	private void _6_0018(GraphicsDevice P_0, ShadowRenderTargetGroup P_1, ShadowGroup P_2, IShadowMap P_3, EffectTypeCaster P_4, X._0017 P_5, bool P_6, bool P_7)
	{
		if (P_5.Objects.Count >= 1 && P_2.Lights.Count >= 1 && P_3 != null)
		{
			ISkinnedEffect skinnedEffect = P_4.SkinnedEffect;
			if (P_4.Effect is Z._0018 obj)
			{
				obj.EffectDetail = base.ShadowDetail;
			}
			P_3.BeginRendering(P_1.RenderTarget);
			if (skinnedEffect != null)
			{
				skinnedEffect.Skinned = P_5._3AL;
			}
			_6_0019(P_0, P_5.Objects, P_4, null, false, true, false, DA_0018.Solid, base.RenderFillMode, P_6, true, P_7);
			P_3.EndRendering();
		}
	}

	private void _6L(GraphicsDevice P_0, List<X._0017> P_1, List<BaseLight> P_2, ILightMapManager P_3, bool P_4, FillMode P_5, bool P_6, bool P_7)
	{
		LightingSystemPerformance.Begin("RenderManager.RenderObjectBatches");
		foreach (X._0017 item in P_1)
		{
			if (P_4 && !item._3A_0018)
			{
				continue;
			}
			EffectTypeCaster effectTypeCaster = OptimizationSystem.EffectTypeCasters.Get(item._3AD);
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
			_ = item._3AD;
			ILightingEffect lightingEffect = effectTypeCaster.LightingEffect;
			if (P_2.Count > 0 && lightingEffect != null)
			{
				int maxLightSources = lightingEffect.MaxLightSources;
				if (P_2.Count > maxLightSources)
				{
					_3Ax.Clear();
					for (int i = 0; i < P_2.Count; i++)
					{
						_3Ax.Add(P_2[i]);
						if (_3Ax.Count >= maxLightSources || i + 1 >= P_2.Count)
						{
							lightingEffect.LightSources = _3Ax;
							_6_0019(P_0, item.Objects, effectTypeCaster, P_3, P_4, true, false, DA_0018.Solid, P_5, P_6, P_7, flag);
							_3Ax.Clear();
						}
					}
				}
				else
				{
					lightingEffect.LightSources = P_2;
					_6_0019(P_0, item.Objects, effectTypeCaster, P_3, P_4, true, false, DA_0018.Solid, P_5, P_6, P_7, flag);
				}
			}
			else
			{
				_6_0019(P_0, item.Objects, effectTypeCaster, P_3, P_4, true, false, DA_0018.Solid, P_5, P_6, P_7, flag);
			}
		}
	}

	private void _6_0019(GraphicsDevice P_0, List<RenderableMesh> P_1, EffectTypeCaster P_2, ILightMapManager P_3, bool P_4, bool P_5, bool P_6, DA_0018 P_7, FillMode P_8, bool P_9, bool P_10, bool P_11)
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
				if (item._3Ax && item._3AL.Valid)
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
		bool flag3 = P_7 == DA_0018.TransparentDoubleSided;
		if (flag3)
		{
			_0010._0018._3j(P_0, P_8, cullMode, !P_6, false, false);
		}
		else
		{
			if (P_2.RenderableEffect != null && P_7 == DA_0018.Solid)
			{
				flag2 = P_2.RenderableEffect.DoubleSided;
			}
			_0010._0018._3j(P_0, P_8, cullMode, P_6, flag2, false);
		}
		SamplerState samplerState = null;
		if (P_5 && !P_10)
		{
			if (P_2.AddressableEffect != null)
			{
				IAddressableEffect addressableEffect = P_2.AddressableEffect;
				samplerState = _3A8._6I(P_0, addressableEffect.AddressModeU, addressableEffect.AddressModeV, addressableEffect.AddressModeW, base.Filter, base.MaxAnisotropy);
			}
			else
			{
				samplerState = _3A8._6I(P_0, TextureAddressMode.Wrap, TextureAddressMode.Wrap, TextureAddressMode.Wrap, base.Filter, base.MaxAnisotropy);
			}
		}
		else
		{
			_3A8._6g(P_0, SamplerState.PointClamp);
		}
		EffectPassCollection passes = P_2.Effect.CurrentTechnique.Passes;
		base._3Ab._3A3.AccumulationValue++;
		base._3Ab._3A6.AccumulationValue += passes.Count;
		IEffectMatrices effectMatrices = P_2.EffectMatrices;
		IEffectLights effectLights = P_2.EffectLights;
		ISkinnedEffect skinnedEffect = P_2.SkinnedEffect;
		IRenderableEffect renderableEffect = P_2.RenderableEffect;
		BaseRenderableEffect baseRenderableEffect = P_2.BaseRenderableEffect;
		IStaticLightingEffect staticLightingEffect = P_2.StaticLightingEffect;
		if (P_2.BaseSasEffect != null || P_11)
		{
			_3A8._6F();
		}
		bool flag4 = true;
		bool flag5 = staticLightingEffect != null || effectLights != null;
		Dictionary<object, CompositeLighting> dictionary = ((P_7 == DA_0018.Solid) ? _3Ag : _3AI);
		for (int i = 0; i < passes.Count; i++)
		{
			EffectPass effectPass = passes[i];
			foreach (RenderableMesh item2 in P_1)
			{
				if (item2 == null || (P_4 && !item2._3Ax) || !item2._3AL.Valid)
				{
					continue;
				}
				bool flag6 = false;
				if (flag5)
				{
					CompositeLighting value;
					if (staticLightingEffect != null)
					{
						StaticLightingType staticLightingType = item2._3AL.StaticLightingType;
						if (staticLightingType == StaticLightingType.Composite || staticLightingType == StaticLightingType.Custom || P_7 != DA_0018.Solid)
						{
							if (!dictionary.TryGetValue(item2._3AL, out value))
							{
								value = _3AF;
							}
							if (staticLightingType == StaticLightingType.BakedDown && P_3 != null)
							{
								staticLightingEffect.SetStaticLighting(StaticLightingEffectMode.BakedDownAndComposite, P_3.GetLightMap(item2), ref value);
							}
							else
							{
								staticLightingEffect.SetStaticLighting(StaticLightingEffectMode.Composite, null, ref value);
							}
						}
						else if (staticLightingType == StaticLightingType.BakedDown && P_3 != null)
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
						if (!dictionary.TryGetValue(item2._3AL, out value))
						{
							value = _3AF;
						}
						effectLights.AmbientLightColor = value.AmbientColor;
						effectLights.DirectionalLight0.DiffuseColor = value.DiffuseColor;
						effectLights.DirectionalLight0.Direction = value.Direction;
					}
					flag6 = true;
				}
				if (effectMatrices != null)
				{
					effectMatrices.World = item2._3Ac;
					flag6 = true;
				}
				else
				{
					if (skinnedEffect != null)
					{
						skinnedEffect.SkinBones = item2._3AL.SkinBones;
						flag6 = true;
					}
					if (renderableEffect != null)
					{
						renderableEffect.SetWorldAndWorldToObject(ref item2._3Ac, ref item2._3Ag);
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
					base._3Ab._3A_0017.AccumulationValue++;
				}
				if (!flag2 && cullMode != item2._3AZ)
				{
					cullMode = item2._3AZ;
					if (flag3)
					{
						_0010._0018._3j(P_0, P_8, cullMode, !P_6, false, false);
					}
					else
					{
						_0010._0018._3j(P_0, P_8, cullMode, P_6, flag2, P_9);
					}
					base._3Ab._3AD.AccumulationValue++;
				}
				_3AZ._0016(P_0, item2);
				try
				{
					if (flag4 && samplerState != null)
					{
						_3A8._6g(P_0, samplerState);
					}
					if (item2.Index.BufferIndex._3A_0018 == null)
					{
						P_0.DrawPrimitives(item2._3A8, item2.Index._3AL, item2.Index._3A_0018);
					}
					else
					{
						P_0.DrawIndexedPrimitives(item2._3A8, item2.Index._3A_0019, 0, item2._3AI, item2.Index._3AL, item2.Index._3A_0018);
					}
					if (flag3)
					{
						_0010._0018._3j(P_0, P_8, cullMode, P_6, false, false);
						cullMode = CullMode.None;
						if (item2.Index.BufferIndex._3A_0018 == null)
						{
							P_0.DrawPrimitives(item2._3A8, item2.Index._3AL, item2.Index._3A_0018);
						}
						else
						{
							P_0.DrawIndexedPrimitives(item2._3A8, item2.Index._3A_0019, 0, item2._3AI, item2.Index._3AL, item2.Index._3A_0018);
						}
					}
				}
				catch (Exception ex)
				{
					item2._3AL.Valid = false;
					item2._3AL.RenderingErrors = ex.Message;
					SunBurnEditor._0019q(item2._3AL, l._0019.Error);
					OnXNARuntimeException(ex, $"Unable to render object '{item2._3AL.Name}'.");
				}
				flag4 = false;
				base._3Ab._3A_0019.AccumulationValue++;
				base._3Ab._3A_0018.AccumulationValue += item2.Index._3A_0018;
			}
		}
		if (P_2.BaseSasEffect != null || P_11)
		{
			_3A8._6F();
		}
	}
}
