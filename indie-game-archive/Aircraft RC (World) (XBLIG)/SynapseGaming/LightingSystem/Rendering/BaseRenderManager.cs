using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using N;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Shadows;
using Z;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Base class that provides basic render management.  Used by the forward rendering
/// RenderManager and deferred rendering DeferredRenderManager classes.
/// </summary>
public abstract class BaseRenderManager : IRenderManager, IRenderableManager, IManagerService, IManager, IUnloadable
{
	internal enum _0001CB
	{
		Solid,
		TransparentSingleSided,
		TransparentDoubleSided
	}

	internal class _0001C_0002
	{
		internal SystemStatistic HCB = SystemConsole.GetStatistic("Renderer_PolysRendered", SystemStatisticCategory.Rendering);

		internal SystemStatistic HC_0002 = SystemConsole.GetStatistic("Renderer_SceneObjectsRendered", SystemStatisticCategory.Rendering);

		internal SystemStatistic HC_0012 = SystemConsole.GetStatistic("Renderer_MeshesRendered", SystemStatisticCategory.Rendering);

		internal SystemStatistic HCH = SystemConsole.GetStatistic("Renderer_Batches", SystemStatisticCategory.Rendering);

		internal SystemStatistic HC7 = SystemConsole.GetStatistic("Renderer_BatchPasses", SystemStatisticCategory.Rendering);

		internal SystemStatistic HC_0001 = SystemConsole.GetStatistic("Renderer_BatchCullModeChanges", SystemStatisticCategory.Rendering);

		internal SystemStatistic HCw = SystemConsole.GetStatistic("Renderer_BatchEffectCommitChanges", SystemStatisticCategory.Rendering);

		internal SystemStatistic HCZ = SystemConsole.GetStatistic("Light_LightsRendered", SystemStatisticCategory.Lighting);

		internal SystemStatistic HC_000F = SystemConsole.GetStatistic("Light_LightsRenderedAsGroup", SystemStatisticCategory.Lighting);

		internal SystemStatistic HCy = SystemConsole.GetStatistic("Shadow_ShadowGroupsRendered", SystemStatisticCategory.Shadowing);

		internal SystemStatistic HC6 = SystemConsole.GetStatistic("Shadow_ShadowMapPagesProcessed", SystemStatisticCategory.Shadowing);

		internal SystemStatistic HCD = SystemConsole.GetStatistic("Shadow_ShadowMapsProcessed", SystemStatisticCategory.Shadowing);

		internal SystemStatistic HC_0011 = SystemConsole.GetStatistic("Shadow_ShadowMapFacesProcessed", SystemStatisticCategory.Shadowing);

		internal SystemStatistic HCK = SystemConsole.GetStatistic("Shadow_ShadowMapFacesFilled", SystemStatisticCategory.Shadowing);
	}

	/// <summary />
	protected const float _CompositeLightingBlendAmount = 0.25f;

	private int HCB = 60;

	private DetailPreference HC_0002 = DetailPreference.Medium;

	private DetailPreference HC_0012;

	private bool HCH;

	private bool HC7 = true;

	private FillMode HC_0001;

	private ISceneState HCw;

	private int HCZ = 4;

	private TextureFilter HC_000F;

	private IManagerServiceProvider HCy;

	private TransparencyRenderNodeSorter HC6 = new TransparencyRenderNodeSorter();

	private Z.y<RenderableMeshTransparencyRenderNode> HCD = new Z.y<RenderableMeshTransparencyRenderNode>();

	private ShadowRenderTargetGroup HC_0011 = new ShadowRenderTargetGroup();

	private N.B HCK;

	private List<ShadowRenderTargetGroup> HC_0003 = new List<ShadowRenderTargetGroup>();

	private List<BaseLight> HCk = new List<BaseLight>();

	private List<BaseLight> HCs = new List<BaseLight>();

	private AmbientLight HC_0013 = new AmbientLight();

	internal _0001C_0002 HCX = new _0001C_0002();

	/// <summary>
	/// Gets the manager specific Type used as a unique key for storing and
	/// requesting the manager from the IManagerServiceProvider.
	/// </summary>
	public Type ManagerType => SceneInterface.RenderManagerType;

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
	public IManagerServiceProvider OwnerSceneInterface => HCy;

	/// <summary>
	/// Determines if XNA rendering exceptions are quietly caught
	/// or allowed to bubble-up to the debugger.
	///
	/// Note: exceptions are always hidden while the editor is
	/// open and when the game is not being debugged.
	/// </summary>
	public bool HideXNARuntimeExceptions
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
	/// Enables clearing the back buffer during rendering.
	/// Disabling allows custom rendering (such as skybox)
	/// prior to calling RenderManager.Render().
	/// </summary>
	public bool ClearBackBufferEnabled
	{
		get
		{
			return HC7;
		}
		set
		{
			HC7 = value;
		}
	}

	/// <summary>
	/// Changes the render fill mode allowing solid and wireframe rendering.
	/// </summary>
	public FillMode RenderFillMode
	{
		get
		{
			return HC_0001;
		}
		set
		{
			HC_0001 = value;
		}
	}

	/// <summary>
	/// Current composite ambient light (combination of all scene ambient lights)
	/// provided by the LightManager (only valid between calls to
	/// BeginFrameRendering and EndFrameRendering).
	/// </summary>
	public AmbientLight FrameAmbientLight => HC_0013;

	/// <summary>
	/// Current scene lights provided by the LightManager (only valid between
	/// calls to BeginFrameRendering and EndFrameRendering).
	/// </summary>
	public List<BaseLight> FrameLights => HCk;

	/// <summary>
	/// Current scene shadow maps provided by the ShadowManager and
	/// filled by this render manager (only valid between calls to
	/// BeginFrameRendering and EndFrameRendering).
	///
	/// See the Custom Renderer project template for an example of
	/// how to use ShadowRenderTargetGroup and the contained
	/// shadow maps to render shadows onto the scene.
	/// </summary>
	public List<ShadowRenderTargetGroup> FrameShadowRenderTargetGroups => HC_0003;

	/// <summary>
	/// Determines the current rendering quality based on the user preferences provided to ApplyPreferences.
	/// </summary>
	protected int MaxAnisotropy => HCZ;

	/// <summary>
	/// Current scene state information provided to BeginFrameRendering (only valid between calls to BeginFrameRendering and EndFrameRendering).
	/// </summary>
	protected ISceneState SceneState => HCw;

	/// <summary>
	/// Determines the current rendering quality based on the user preferences provided to ApplyPreferences.
	/// </summary>
	protected DetailPreference ShadowDetail => HC_0002;

	/// <summary>
	/// Determines the current rendering quality based on the user preferences provided to ApplyPreferences.
	/// </summary>
	protected DetailPreference EffectDetail => HC_0012;

	/// <summary>
	/// Determines the current rendering quality based on the user preferences provided to ApplyPreferences.
	/// </summary>
	protected TextureFilter Filter => HC_000F;

	/// <summary>
	/// Current ambient lights provided by the LightManager (only valid between calls to BeginFrameRendering and EndFrameRendering).
	/// </summary>
	protected List<BaseLight> FrameAmbientLights => HCs;

	/// <summary>
	/// Service provider used to access all other manager services in this scene. Allows querying
	/// objects through the IObjectManager manager interface, querying lights through the ILightManager manager
	/// interface, and more.
	/// </summary>
	protected IManagerServiceProvider ServiceProvider => HCy;

	/// <summary>
	/// Current collection of transparent scene nodes (only valid between
	/// calls to BeginFrameRendering and EndFrameRendering).
	/// </summary>
	protected TransparencyRenderNodeSorter TransparencyRenderNodes => HC6;

	/// <summary>
	/// Creates a new BaseRenderManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	public BaseRenderManager(IManagerServiceProvider sceneinterface)
	{
		HCy = sceneinterface;
		HCK = new N.B(sceneinterface);
	}

	/// <summary>
	/// Use to apply user quality and performance preferences to the resources managed by this object.
	/// </summary>
	/// <param name="preferences"></param>
	public virtual void ApplyPreferences(ISystemPreferences preferences)
	{
		switch (preferences.TextureSampling)
		{
		case SamplingPreference.Point:
			HC_000F = TextureFilter.PointMipLinear;
			break;
		case SamplingPreference.Bilinear:
			HC_000F = TextureFilter.LinearMipPoint;
			break;
		case SamplingPreference.Trilinear:
			HC_000F = TextureFilter.Linear;
			break;
		case SamplingPreference.Anisotropic:
			HC_000F = TextureFilter.Anisotropic;
			break;
		}
		HCZ = preferences.MaxAnisotropy;
		HC_0002 = preferences.ShadowDetail;
		HC_0012 = preferences.EffectDetail;
		HCK.ApplyPreferences(preferences);
	}

	internal abstract void _0017dO_0016P(List<RenderableMesh> P_0, bool P_1);

	/// <summary>
	/// Determines if the render manager allows transparent scene
	/// nodes of the same type and effect to be rendered together.
	///
	/// If not each transparent scene node will be rendered individually.
	/// </summary>
	/// <param name="scenestate"></param>
	/// <returns></returns>
	protected abstract bool CanBatchTransparencies(ISceneState scenestate);

	/// <summary>
	/// Extracts all renderable meshes and composite lighting (if necessary) from a list of
	/// scene entities.
	///
	/// It's assumed the entity list is already frustum-culled to only visible entities. Additionally
	/// this method view-culls entities based on distance from the camera and the scene state visible distance.
	/// </summary>
	/// <param name="entities">List of entities to extract renderable meshes and composite lighting from.</param>
	/// <param name="lightmanager">Light manager used to calculate composite lighting. If null composite lighting is not generated.</param>
	/// <param name="editorisopen">Indicates the editor is currently open.</param>
	/// <param name="rendermeshes">Destination list for extracted renderable meshes.</param>
	/// <param name="renderingcompositelightingcache">Destination dictionary for entity composite lighting that is applied to opaque meshes.</param>
	/// <param name="transparentcompositelightingcache">Destination dictionary for entity composite lighting that is applied to transparent meshes.</param>
	protected virtual void ExtractAndFilterRenderableMeshes(List<SceneEntity> entities, ILightManager lightmanager, bool editorisopen, List<RenderableMesh> rendermeshes, Dictionary<object, CompositeLighting> renderingcompositelightingcache, Dictionary<object, CompositeLighting> transparentcompositelightingcache)
	{
		LightingSystemPerformance.Begin("BaseRenderManager.ExtractAndFilterRenderableMeshes");
		bool flag = lightmanager != null;
		Vector3 value = HCw.ViewToWorld.Translation;
		for (int i = 0; i < entities.Count; i++)
		{
			SceneEntity sceneEntity = entities[i];
			if (sceneEntity == null)
			{
				continue;
			}
			SceneEntityTypeCaster sceneEntityTypeCaster = OptimizationSystem.SceneEntityTypeCasters.Get(sceneEntity);
			ISceneObject sceneObject = sceneEntityTypeCaster.SceneObject;
			if (sceneObject == null || (!sceneObject.Visible && (!sceneObject.VisibleInEditor || !editorisopen)))
			{
				continue;
			}
			BoundingSphere worldBoundingSphere = sceneObject.WorldBoundingSphere;
			float num = HCw.Environment.VisibleDistance + worldBoundingSphere.Radius;
			float num2 = num * num;
			Vector3.DistanceSquared(ref value, ref worldBoundingSphere.Center, out var result);
			if (result > num2)
			{
				continue;
			}
			ushort num3 = 0;
			if (num2 > 0f)
			{
				num3 = (ushort)(result / num2 * 65535f);
			}
			HCX.HC_0002.AccumulationValue++;
			bool flag2 = false;
			bool flag3 = false;
			for (int j = 0; j < sceneObject.RenderableMeshes.Count; j++)
			{
				RenderableMesh renderableMesh = sceneObject.RenderableMeshes[j];
				if (renderableMesh.HC6 != null)
				{
					EffectTypeCaster effectTypeCaster = OptimizationSystem.EffectTypeCasters.Get(renderableMesh.HC6);
					if (effectTypeCaster.TransparentEffect != null && (effectTypeCaster.TransparentEffect.TransparencyMode == TransparencyMode.Blend || effectTypeCaster.TransparentEffect.TransparencyMode == TransparencyMode.Additive))
					{
						RenderableMeshTransparencyRenderNode renderableMeshTransparencyRenderNode = HCD.New();
						renderableMeshTransparencyRenderNode.Build(this, HCw, renderableMesh);
						HC6.Add(renderableMeshTransparencyRenderNode);
						flag3 = true;
					}
					else
					{
						ushort num4 = (ushort)(renderableMesh.HCu >> 16);
						ushort num5 = (ushort)renderableMesh.HCu;
						int num6 = (num5 ^ 1) + (num4 ^ 2) << 16;
						renderableMesh.HCq = num3 | num6;
						rendermeshes.Add(renderableMesh);
						flag2 = true;
					}
				}
			}
			if (!flag)
			{
				continue;
			}
			StaticLightingType staticLightingType = sceneObject.StaticLightingType;
			if (flag2 && (staticLightingType == StaticLightingType.Composite || staticLightingType == StaticLightingType.Custom) && !renderingcompositelightingcache.ContainsKey(sceneObject))
			{
				CompositeLighting compositelighting = default(CompositeLighting);
				BoundingBox worldbounds = sceneObject.WorldBoundingBox;
				lightmanager.GetCompositeLighting(ref worldbounds, 0.25f, LightingType.BakedDown, out compositelighting);
				if (staticLightingType == StaticLightingType.Custom)
				{
					float num7 = MathHelper.Clamp(0.25f, 0f, 1f);
					float num8 = 1f - num7;
					Vector3 vector = sceneObject.CustomStaticLightingColor * 2f;
					compositelighting.AmbientColor = vector * num7;
					compositelighting.DiffuseColor = vector * num8;
				}
				renderingcompositelightingcache.Add(sceneObject, compositelighting);
			}
			if (flag3 && !transparentcompositelightingcache.ContainsKey(sceneObject))
			{
				CompositeLighting compositelighting2 = default(CompositeLighting);
				StaticLightingType staticLightingType2 = staticLightingType;
				LightingType lightingtype = ((staticLightingType2 == StaticLightingType.BakedDown) ? LightingType.RealTime : (LightingType.RealTime | LightingType.BakedDown));
				BoundingBox worldbounds2 = sceneObject.WorldBoundingBox;
				lightmanager.GetCompositeLighting(ref worldbounds2, 0.25f, lightingtype, out compositelighting2);
				if (staticLightingType == StaticLightingType.Custom)
				{
					float num9 = MathHelper.Clamp(0.25f, 0f, 1f);
					float num10 = 1f - num9;
					Vector3 vector2 = sceneObject.CustomStaticLightingColor * 2f;
					compositelighting2.AmbientColor = vector2 * num9;
					compositelighting2.DiffuseColor = vector2 * num10;
				}
				transparentcompositelightingcache.Add(sceneObject, compositelighting2);
			}
		}
	}

	/// <summary />
	/// <param name="e"></param>
	/// <param name="contextmessage"></param>
	protected virtual void OnXNARuntimeException(Exception e, string contextmessage)
	{
		if (HCH || !Debugger.IsAttached || SunBurnEditor.EditorAttachedStatic)
		{
			return;
		}
		string text = $"XNA raised the following runtime exception: \"{e.Message}\". To ignore future errors disable HideXNARuntimeExceptions on the RenderManager.";
		if (!string.IsNullOrEmpty(contextmessage))
		{
			text = $"{contextmessage} {text}";
		}
		throw new Exception(text, e);
	}

	/// <summary>
	/// Provides a default set of shadow groups when no IShadowMapManager manager service is available.
	/// </summary>
	/// <param name="rendertargetgroups">Returned render target groups.</param>
	/// <param name="lights">Source lights to create groups for.</param>
	protected void GetDefaultShadows(List<ShadowRenderTargetGroup> rendertargetgroups, List<BaseLight> lights)
	{
		HC_0011.ShadowGroups.Clear();
		HCK.BuildShadowGroups(HC_0011.ShadowGroups, lights, usedefaultgrouping: true);
		HC_0011.Build(null);
		rendertargetgroups.Add(HC_0011);
	}

	/// <summary>
	/// Generates shadow maps for the provided shadow render groups. Override this
	/// method to customize shadow map generation.
	/// </summary>
	/// <param name="shadowrendertargetgroups">Shadow render groups to generate shadow maps for.</param>
	protected abstract void BuildShadowMaps(List<ShadowRenderTargetGroup> shadowrendertargetgroups);

	/// <summary>
	/// Builds all object batches, shadow maps, and cached information before rendering.
	/// Any object added to the RenderManager after this call will not be visible during the frame.
	/// </summary>
	/// <param name="scenestate"></param>
	public virtual void BeginFrameRendering(ISceneState scenestate)
	{
		LightingSystemPerformance.Begin("BaseRenderManager.BeginFrameRendering");
		SplashScreen._7z();
		HCw = scenestate;
		GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
		SunBurnCoreSystem instance = SunBurnCoreSystem.Instance;
		instance._0002_0019(scenestate.FrameBuffers);
		GraphicsDeviceSupport graphicsDeviceSupport = instance.GetGraphicsDeviceSupport();
		HCZ = Math.Max(Math.Min(HCZ, graphicsDeviceSupport.MaxAnisotropy), 1);
		for (int i = 0; i < 8; i++)
		{
			graphicsDevice.SamplerStates[i] = SamplerState.AnisotropicWrap;
		}
		HCK.BeginFrameRendering(scenestate);
		HCk.Clear();
		HCs.Clear();
		ILightManager lightManager = (ILightManager)HCy.GetManager(SceneInterface.LightManagerType, required: false);
		if (lightManager == null)
		{
			HC_0013.DiffuseColor = new Vector3(1f, 0.9f, 0.8f);
			HC_0013.Intensity = 0.25f;
		}
		else
		{
			lightManager.Find(HCk, HCw.ViewFrustum, ObjectFilter.EnabledDynamicAndStatic);
			HC_0013.DiffuseColor = Vector3.Zero;
			HC_0013.Intensity = 1f;
			int num = 0;
			float num2 = 0f;
			Matrix viewToWorld = scenestate.ViewToWorld;
			for (int j = 0; j < HCk.Count; j++)
			{
				BaseLight baseLight = HCk[j];
				if (baseLight == null)
				{
					continue;
				}
				if (baseLight.LightingType != LightingType.RealTime)
				{
					HCk[j] = null;
					continue;
				}
				LightTypeCaster lightTypeCaster = OptimizationSystem.LightTypeCasters.Get(baseLight);
				if (lightTypeCaster.AmbientSource != null)
				{
					num2 += lightTypeCaster.AmbientSource.Depth;
					num++;
					HC_0013.DiffuseColor += baseLight.CompositeColorAndIntensity;
					HCk[j] = null;
				}
				else if (lightTypeCaster.PointSource != null)
				{
					IPointSource pointSource = lightTypeCaster.PointSource;
					float num3 = Vector3.DistanceSquared(viewToWorld.Translation, pointSource.Position);
					float num4 = SceneState.Environment.VisibleDistance + pointSource.Radius;
					if (num3 > num4 * num4)
					{
						HCk[j] = null;
					}
				}
			}
			if (num > 0)
			{
				HC_0013.Depth = num2 / (float)num;
			}
			else
			{
				HC_0013.Depth = 0.1f;
			}
		}
		HCs.Add(HC_0013);
	}

	/// <summary>
	/// Renders the scene.
	/// </summary>
	public abstract void Render();

	/// <summary>
	/// Finalizes rendering and cleans up frame information including removing all frame lifespan objects.
	/// </summary>
	public virtual void EndFrameRendering()
	{
		LightingSystemPerformance.Begin("BaseRenderManager.EndFrameRendering");
		GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
		LightingSystemPerformance.Begin("BaseRenderManager.EndFrameRendering (transparency nodes)");
		HC6.RenderBatches(HCw, CanBatchTransparencies(HCw));
		HC6.Clear();
		HCD.FreeAllTracked();
		HCK.EndFrameRendering();
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
	}

	/// <summary>
	/// Removes all scene objects and cleans up scene information.
	/// </summary>
	public virtual void Clear()
	{
		HCK.Clear();
		OptimizationSystem.Clear();
	}

	/// <summary>
	/// Unloads all scene and device specific data.  Must be called
	/// when the device is reset (during Game.UnloadGraphicsContent()).
	/// </summary>
	public virtual void Unload()
	{
		Clear();
		HCK.Unload();
	}
}
