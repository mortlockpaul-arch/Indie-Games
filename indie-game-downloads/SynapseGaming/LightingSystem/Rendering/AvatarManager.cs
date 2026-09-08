using System;
using System.Collections.Generic;
using _0003;
using _0010;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Effects;
using SynapseGaming.LightingSystem.Effects.Forward;
using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Shadows;
using p;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Manages scene avatars and provides support for rendering
/// and finding avatars by bounding volume.
/// </summary>
public class AvatarManager : BaseObjectGraphManager<Avatar, IAvatarManager>, IAvatarManager, IRenderableManager, IManagerService, IShadowRenderer, IUpdatableManager, IManager, IUnloadable, IQuery<Avatar>, ISubmit<Avatar>
{
	private class DA_0018
	{
		public SystemStatistic ObjectsSubmitted = SystemConsole.GetStatistic("SceneGraph_ObjectsSubmitted", SystemStatisticCategory.SceneGraph);

		public SystemStatistic ObjectsRemoved = SystemConsole.GetStatistic("SceneGraph_ObjectsRemoved", SystemStatisticCategory.SceneGraph);

		public SystemStatistic ObjectsRetrieved = SystemConsole.GetStatistic("SceneGraph_ObjectsRetrieved", SystemStatisticCategory.SceneGraph);

		public SystemStatistic AvatarsRendered = SystemConsole.GetStatistic("Renderer_AvatarsRendered", SystemStatisticCategory.Rendering);

		public SystemStatistic AvatarProxiesRendered = SystemConsole.GetStatistic("Renderer_AvatarProxiesRendered", SystemStatisticCategory.Rendering);
	}

	private int _3A_0018 = 70;

	private float _3AL = 0.55f;

	private float _3A_0019 = 1f;

	private ISceneState _3A3;

	private List<Avatar> _3A6 = new List<Avatar>(16);

	private List<Avatar> _3AD = new List<Avatar>(32);

	private FogEffect _3A_0017;

	private List<Avatar> _3A_0003 = new List<Avatar>(16);

	private _0003._0019 _3Al;

	private List<Avatar> _3At = new List<Avatar>(16);

	private DA_0018 _3AF = new DA_0018();

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
			return _3A_0018;
		}
		set
		{
			_3A_0018 = value;
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
			return _3AL;
		}
		set
		{
			_3AL = value;
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
			return _3A_0019;
		}
		set
		{
			_3A_0019 = value;
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
		_3At.Clear();
		_3A_0003.Clear();
		ObjectFilter objectFilter = ObjectFilter.Static;
		if (shadowgroup.ShadowSource.ShadowType == ShadowType.AllObjects)
		{
			objectFilter |= ObjectFilter.Dynamic;
		}
		Find(_3At, shadowgroup.BoundingBox, objectFilter);
		for (int i = 0; i < _3At.Count; i++)
		{
			Avatar avatar = _3At[i];
			if (avatar.CastShadows)
			{
				_3A_0003.Add(avatar);
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
		_3At.Clear();
		foreach (Avatar item in _3A_0003)
		{
			if (surface.Frustum.Contains(item.WorldBoundingSphere) != ContainmentType.Disjoint)
			{
				_3At.Add(item);
			}
		}
		if (_3At.Count < 1)
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
		if (_3Al == null)
		{
			_3Al = new _0003._0019(graphicsDevice);
		}
		skinnedEffect.Skinned = false;
		EffectHelper.SyncObjectAndShadowEffects(_3Al.DefaultEffect, shadoweffect);
		bool result = false;
		int num = 1;
		foreach (Avatar item2 in _3At)
		{
			if (item2.CastShadows)
			{
				AvatarRenderer renderer = item2.Renderer;
				graphicsDevice.DepthStencilState = _0010._0018._3Ac;
				graphicsDevice.ReferenceStencil = num;
				graphicsDevice.BlendState = _0010._0018._3AF;
				renderer.World = item2.World;
				renderer.View = surface.WorldToSurfaceView;
				renderer.Projection = surface.Projection;
				renderer.Draw(item2.SkinBones, item2.Expression);
				_3AF.AvatarsRendered.AccumulationValue++;
				graphicsDevice.DepthStencilState = _0010._0018._3Ag;
				graphicsDevice.ReferenceStencil = num;
				graphicsDevice.BlendState = BlendState.Opaque;
				graphicsDevice.RasterizerState = RasterizerState.CullNone;
				renderableEffect.World = _3Al.w(item2.WorldBoundingBoxProxy);
				skinnedEffect.SkinBones = null;
				shadoweffect.CurrentTechnique.Passes[0].Apply();
				_3Al.e();
				_3AF.AvatarProxiesRendered.AccumulationValue++;
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
		_3A3 = scenestate;
		_3A6.Clear();
		Find(_3A6, scenestate.ViewFrustum, ObjectFilter.All);
	}

	/// <summary>
	/// Finalizes rendering.
	/// </summary>
	public void EndFrameRendering()
	{
		if (_3A6.Count < 1)
		{
			return;
		}
		LightingSystemPerformance.Begin("AvatarManager.EndFrameRendering");
		GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
		ILightManager lightManager = (ILightManager)base.OwnerSceneInterface.GetManager(SceneInterface.LightManagerType, required: false);
		bool fogEnabled = _3A3.Environment.FogEnabled;
		if (fogEnabled)
		{
			if (_3A_0017 == null)
			{
				_3A_0017 = new FogEffect(graphicsDevice);
			}
			if (_3Al == null)
			{
				_3Al = new _0003._0019(graphicsDevice);
			}
			graphicsDevice.Clear(ClearOptions.Stencil, Color.Black, 0f, 0);
			_3A_0017.Color = _3A3.Environment.FogColor;
			_3A_0017.StartDistance = _3A3.Environment.FogStartDistance;
			_3A_0017.EndDistance = _3A3.Environment.FogEndDistance;
		}
		int num = 1;
		float visibleDistance = _3A3.Environment.VisibleDistance;
		float fogStartDistance = _3A3.Environment.FogStartDistance;
		Vector3 value = _3A3.ViewToWorld.Translation;
		foreach (Avatar item in _3A6)
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
					graphicsDevice.DepthStencilState = _0010._0018._3AI;
					graphicsDevice.ReferenceStencil = num;
				}
				AvatarRenderer renderer = item.Renderer;
				renderer.World = item.World;
				renderer.View = _3A3.View;
				renderer.Projection = _3A3.Projection;
				if (lightManager != null)
				{
					CompositeLighting compositeLighting = lightManager.GetCompositeLighting(item.WorldBoundingBox, _3AL, LightingType.RealTime | LightingType.BakedDown);
					renderer.LightColor = compositeLighting.DiffuseColor * _3A_0019;
					renderer.LightDirection = compositeLighting.Direction;
					renderer.AmbientLightColor = compositeLighting.AmbientColor * _3A_0019 * 0.25f;
				}
				else
				{
					renderer.LightColor = new Vector3(0f);
					renderer.AmbientLightColor = new Vector3(0.25f);
				}
				renderer.Draw(item.SkinBones, item.Expression);
				_3AF.AvatarsRendered.AccumulationValue++;
				if (flag)
				{
					graphicsDevice.DepthStencilState = _0010._0018._3A8;
					graphicsDevice.ReferenceStencil = num;
					graphicsDevice.RasterizerState = RasterizerState.CullNone;
					graphicsDevice.BlendState = _0010._0018._3AZ;
					_3A_0017.World = _3Al.w(item.WorldBoundingBoxProxy);
					_3A_0017.View = _3A3.View;
					_3A_0017.Projection = _3A3.Projection;
					_3A_0017.CurrentTechnique.Passes[0].Apply();
					_3Al.e();
					_3AF.AvatarProxiesRendered.AccumulationValue++;
					graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
				}
				num++;
			}
		}
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		graphicsDevice.BlendState = BlendState.Opaque;
		foreach (Avatar item2 in _3A6)
		{
			item2.RenderCustomPass(_3A3);
		}
	}

	/// <summary>
	/// Disposes any graphics resource used internally by this object, and removes
	/// scene resources managed by this object. Commonly used during Game.UnloadContent.
	/// </summary>
	public override void Unload()
	{
		base.Unload();
		p._0018._6_0006(ref _3Al);
		p._0018._6_0006(ref _3A_0017);
	}
}
