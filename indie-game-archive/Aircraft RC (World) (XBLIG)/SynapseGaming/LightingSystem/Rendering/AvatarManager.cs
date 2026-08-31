using System;
using System.Collections.Generic;
using F;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Effects;
using SynapseGaming.LightingSystem.Effects.Forward;
using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Shadows;
using Z;
using q;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Manages scene avatars and provides support for rendering
/// and finding avatars by bounding volume.
/// </summary>
public class AvatarManager : BaseObjectGraphManager<Avatar, IAvatarManager>, IAvatarManager, IRenderableManager, IManagerService, IShadowRenderer, IUpdatableManager, IManager, IUnloadable, IQuery<Avatar>, ISubmit<Avatar>
{
	private class _0001CB
	{
		public SystemStatistic ObjectsSubmitted = SystemConsole.GetStatistic("SceneGraph_ObjectsSubmitted", SystemStatisticCategory.SceneGraph);

		public SystemStatistic ObjectsRemoved = SystemConsole.GetStatistic("SceneGraph_ObjectsRemoved", SystemStatisticCategory.SceneGraph);

		public SystemStatistic ObjectsRetrieved = SystemConsole.GetStatistic("SceneGraph_ObjectsRetrieved", SystemStatisticCategory.SceneGraph);

		public SystemStatistic AvatarsRendered = SystemConsole.GetStatistic("Renderer_AvatarsRendered", SystemStatisticCategory.Rendering);

		public SystemStatistic AvatarProxiesRendered = SystemConsole.GetStatistic("Renderer_AvatarProxiesRendered", SystemStatisticCategory.Rendering);
	}

	private int HCB = 70;

	private float HC_0002 = 0.55f;

	private float HC_0012 = 1f;

	private ISceneState HCH;

	private List<Avatar> HC7 = new List<Avatar>(16);

	private List<Avatar> HC_0001 = new List<Avatar>(32);

	private FogEffect HCw;

	private List<Avatar> HCZ = new List<Avatar>(16);

	private Z._0012 HC_000F;

	private List<Avatar> HCy = new List<Avatar>(16);

	private _0001CB HC6 = new _0001CB();

	/// <summary>
	/// Gets the manager specific Type used as a unique key for storing and
	/// requesting the manager from the IManagerServiceProvider.
	/// </summary>
	public override Type ManagerType => SceneInterface.AvatarManagerType;

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
	public override int ManagerProcessOrder
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
	/// Controls avatar lighting by blending between approximate directional
	/// and ambient lighting.  A blending value of 0.0f makes avatar lighting
	/// highly directional, while a value of 1.0f makes avatar lighting highly
	/// ambient.
	/// </summary>
	public float AmbientBlend
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
	/// Controls avatar lighting intensity, providing a means to tune avatar
	/// lighting to the rest of the scene. An intensity of 1.0f keeps
	/// avatar lighting the same, a value of 0.5f halves the lighting
	/// intensity, while 2.0f doubles it.
	/// </summary>
	public float LightingIntensity
	{
		get
		{
			return HC_0012;
		}
		set
		{
			HC_0012 = value;
		}
	}

	/// <summary>
	/// Creates a new AvatarManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	public AvatarManager(IManagerServiceProvider sceneinterface)
		: base(sceneinterface)
	{
	}

	/// <summary>
	/// Prepares for shadow map rendering.
	/// </summary>
	/// <param name="shadowgroup"></param>
	public void BeginShadowGroupRendering(ShadowGroup shadowgroup)
	{
		HCy.Clear();
		HCZ.Clear();
		ObjectFilter objectFilter = ObjectFilter.Static;
		if (shadowgroup.ShadowSource.ShadowType == ShadowType.AllObjects)
		{
			objectFilter |= ObjectFilter.Dynamic;
		}
		Find(HCy, shadowgroup.BoundingBox, objectFilter);
		for (int i = 0; i < HCy.Count; i++)
		{
			Avatar avatar = HCy[i];
			if (avatar.CastShadows)
			{
				HCZ.Add(avatar);
			}
		}
	}

	/// <summary>
	/// Finalizes shadow map rendering.
	/// </summary>
	/// <param name="shadowgroup"></param>
	public void EndShadowGroupRendering(ShadowGroup shadowgroup)
	{
	}

	/// <summary>
	/// Performs shadow map rendering.
	/// </summary>
	/// <param name="shadowgroup"></param>
	/// <param name="surface"></param>
	/// <param name="shadoweffect"></param>
	public bool RenderToShadowMapSurface(ShadowGroup shadowgroup, ShadowMapSurface surface, Effect shadoweffect)
	{
		LightingSystemPerformance.Begin("AvatarManager.RenderToShadowMapSurface (filter)");
		HCy.Clear();
		foreach (Avatar item in HCZ)
		{
			if (surface.Frustum.Contains(item.WorldBoundingSphere) != ContainmentType.Disjoint)
			{
				HCy.Add(item);
			}
		}
		if (HCy.Count < 1)
		{
			return false;
		}
		if (!(shadoweffect is IRenderableEffect) || !(shadoweffect is ISkinnedEffect))
		{
			return false;
		}
		LightingSystemPerformance.Begin("AvatarManager.RenderToShadowMapSurface (render)");
		GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
		IRenderableEffect renderableEffect = shadoweffect as IRenderableEffect;
		ISkinnedEffect skinnedEffect = shadoweffect as ISkinnedEffect;
		if (HC_000F == null)
		{
			HC_000F = new Z._0012(graphicsDevice);
		}
		skinnedEffect.Skinned = false;
		EffectHelper.SyncObjectAndShadowEffects(HC_000F.DefaultEffect, shadoweffect);
		bool result = false;
		int num = 1;
		foreach (Avatar item2 in HCy)
		{
			if (item2.CastShadows)
			{
				AvatarRenderer renderer = item2.Renderer;
				graphicsDevice.DepthStencilState = q.B.HCD;
				graphicsDevice.ReferenceStencil = num;
				graphicsDevice.BlendState = q.B.HC6;
				renderer.World = item2.World;
				renderer.View = surface.WorldToSurfaceView;
				renderer.Projection = surface.Projection;
				renderer.Draw(item2.SkinBones, item2.Expression);
				HC6.AvatarsRendered.AccumulationValue++;
				graphicsDevice.DepthStencilState = q.B.HC_0011;
				graphicsDevice.ReferenceStencil = num;
				graphicsDevice.BlendState = BlendState.Opaque;
				graphicsDevice.RasterizerState = RasterizerState.CullNone;
				renderableEffect.World = HC_000F._1(item2.WorldBoundingBoxProxy);
				skinnedEffect.SkinBones = null;
				shadoweffect.CurrentTechnique.Passes[0].Apply();
				HC_000F.b();
				HC6.AvatarProxiesRendered.AccumulationValue++;
				graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
				result = true;
				num++;
			}
		}
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		return result;
	}

	/// <summary>
	/// Sets up the object prior to rendering.
	/// </summary>
	/// <param name="scenestate"></param>
	public void BeginFrameRendering(ISceneState scenestate)
	{
		HCH = scenestate;
		HC7.Clear();
		Find(HC7, scenestate.ViewFrustum, ObjectFilter.All);
	}

	/// <summary>
	/// Finalizes rendering.
	/// </summary>
	public void EndFrameRendering()
	{
		if (HC7.Count < 1)
		{
			return;
		}
		LightingSystemPerformance.Begin("AvatarManager.EndFrameRendering");
		GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
		ILightManager lightManager = (ILightManager)base.OwnerSceneInterface.GetManager(SceneInterface.LightManagerType, required: false);
		bool fogEnabled = HCH.Environment.FogEnabled;
		if (fogEnabled)
		{
			if (HCw == null)
			{
				HCw = new FogEffect(graphicsDevice);
			}
			if (HC_000F == null)
			{
				HC_000F = new Z._0012(graphicsDevice);
			}
			graphicsDevice.Clear(ClearOptions.Stencil, Color.Black, 0f, 0);
			HCw.Color = HCH.Environment.FogColor;
			HCw.StartDistance = HCH.Environment.FogStartDistance;
			HCw.EndDistance = HCH.Environment.FogEndDistance;
		}
		int num = 1;
		float visibleDistance = HCH.Environment.VisibleDistance;
		float fogStartDistance = HCH.Environment.FogStartDistance;
		Vector3 value = HCH.ViewToWorld.Translation;
		foreach (Avatar item in HC7)
		{
			if (!item.Visible)
			{
				continue;
			}
			BoundingSphere worldBoundingSphere = item.WorldBoundingSphere;
			float num2 = visibleDistance + worldBoundingSphere.Radius;
			Vector3.DistanceSquared(ref value, ref worldBoundingSphere.Center, out var result);
			if (!(result > num2 * num2))
			{
				bool flag = false;
				if (fogEnabled)
				{
					num2 = fogStartDistance - worldBoundingSphere.Radius;
					flag = result > num2 * num2;
				}
				if (fogEnabled)
				{
					graphicsDevice.DepthStencilState = q.B.HCK;
					graphicsDevice.ReferenceStencil = num;
				}
				AvatarRenderer renderer = item.Renderer;
				renderer.World = item.World;
				renderer.View = HCH.View;
				renderer.Projection = HCH.Projection;
				if (lightManager != null)
				{
					CompositeLighting compositeLighting = lightManager.GetCompositeLighting(item.WorldBoundingBox, HC_0002, LightingType.RealTime | LightingType.BakedDown);
					renderer.LightColor = compositeLighting.DiffuseColor * HC_0012;
					renderer.LightDirection = compositeLighting.Direction;
					renderer.AmbientLightColor = compositeLighting.AmbientColor * HC_0012 * 0.25f;
				}
				else
				{
					renderer.LightColor = new Vector3(0f);
					renderer.AmbientLightColor = new Vector3(0.25f);
				}
				renderer.Draw(item.SkinBones, item.Expression);
				HC6.AvatarsRendered.AccumulationValue++;
				if (flag)
				{
					graphicsDevice.DepthStencilState = q.B.HC_0003;
					graphicsDevice.ReferenceStencil = num;
					graphicsDevice.RasterizerState = RasterizerState.CullNone;
					graphicsDevice.BlendState = q.B.HCk;
					HCw.World = HC_000F._1(item.WorldBoundingBoxProxy);
					HCw.View = HCH.View;
					HCw.Projection = HCH.Projection;
					HCw.CurrentTechnique.Passes[0].Apply();
					HC_000F.b();
					HC6.AvatarProxiesRendered.AccumulationValue++;
					graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
				}
				num++;
			}
		}
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		graphicsDevice.BlendState = BlendState.Opaque;
		foreach (Avatar item2 in HC7)
		{
			item2.RenderCustomPass(HCH);
		}
	}

	/// <summary>
	/// Disposes any graphics resource used internally by this object, and removes
	/// scene resources managed by this object. Commonly used during Game.UnloadContent.
	/// </summary>
	public override void Unload()
	{
		base.Unload();
		F.B._7_0004(ref HC_000F);
		F.B._7_0004(ref HCw);
	}
}
