using System;
using _8;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Effects;
using SynapseGaming.LightingSystem.Lights;

namespace SynapseGaming.LightingSystem.Shadows;

/// <summary>
/// Shadow map class that implements cube-mapped shadows with
/// per surface level-of-detail. Used for point based lights.
/// </summary>
public abstract class BaseShadowCubeMap : BaseShadowEffectShadowMap
{
	private const int _3A_0018 = 6;

	private const int _3AL = 8;

	private ShadowMapSurface[] _3A_0019 = new ShadowMapSurface[6];

	private Plane[] _3A3 = new Plane[6];

	private static bool _3A6 = false;

	private static Matrix[] _3AD = new Matrix[6];

	private static Plane[] _3A_0017 = new Plane[6];

	/// <summary>
	/// Array of the cube-map surfaces.
	/// </summary>
	public override ShadowMapSurface[] Surfaces => _3A_0019;

	/// <summary>
	/// Unused, this object supports render targets from the ShadowMapCache.
	/// </summary>
	public override RenderTarget2D CustomRenderTarget => null;

	/// <summary>
	/// Creates a new ShadowCubeMap instance.
	/// </summary>
	public BaseShadowCubeMap()
	{
		if (!_3A6)
		{
			ref Matrix reference = ref _3AD[0];
			reference = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitX, Vector3.UnitY);
			ref Matrix reference2 = ref _3AD[1];
			reference2 = Matrix.CreateLookAt(Vector3.Zero, -Vector3.UnitX, Vector3.UnitY);
			ref Matrix reference3 = ref _3AD[2];
			reference3 = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitY, Vector3.UnitZ);
			ref Matrix reference4 = ref _3AD[3];
			reference4 = Matrix.CreateLookAt(Vector3.Zero, -Vector3.UnitY, Vector3.UnitZ);
			ref Matrix reference5 = ref _3AD[4];
			reference5 = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
			ref Matrix reference6 = ref _3AD[5];
			reference6 = Matrix.CreateLookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
			for (int i = 0; i < _3A_0017.Length; i++)
			{
				ref Plane reference7 = ref _3A_0017[i];
				reference7 = Plane.Transform(new Plane(0f, 0f, 1f, 1f), Matrix.Invert(_3AD[i]));
			}
			_3A6 = true;
		}
		for (int j = 0; j < _3A_0019.Length; j++)
		{
			_3A_0019[j] = new ShadowMapSurface();
		}
		_3A_0019[0].WorldToSurfaceView = _3AD[0];
		_3A_0019[1].WorldToSurfaceView = _3AD[1];
		_3A_0019[2].WorldToSurfaceView = _3AD[2];
		_3A_0019[3].WorldToSurfaceView = _3AD[3];
		_3A_0019[4].WorldToSurfaceView = _3AD[4];
		_3A_0019[5].WorldToSurfaceView = _3AD[5];
		for (int k = 0; k < _3A_0019.Length; k++)
		{
			ref Plane reference8 = ref _3A3[k];
			reference8 = _3A_0017[k];
		}
	}

	/// <summary>
	/// Builds the shadow map information based on the provided scene state and shadow
	/// group, visibility, and quality.
	/// </summary>
	/// <param name="device"></param>
	/// <param name="scenestate"></param>
	/// <param name="shadowgroup">Shadow group used as the source for the shadow map.</param>
	/// <param name="shadowvisibility"></param>
	/// <param name="shadowquality">Shadow quality from 1.0 (highest) to 0.0 (lowest).</param>
	public override void Build(GraphicsDevice device, ISceneState scenestate, ShadowGroup shadowgroup, IShadowMapVisibility shadowvisibility, float shadowquality)
	{
		base.Build(device, scenestate, shadowgroup, shadowvisibility, shadowquality);
		IShadowSource shadowSource = shadowgroup.ShadowSource;
		BoundingSphere boundingSphereCentered = shadowgroup.BoundingSphereCentered;
		Vector3 shadowPosition = shadowgroup.ShadowSource.ShadowPosition;
		float radius = boundingSphereCentered.Radius;
		_3A_0019[0]._6n(new Vector3(0f - shadowPosition.Z, 0f - shadowPosition.Y, shadowPosition.X));
		_3A_0019[1]._6n(new Vector3(shadowPosition.Z, 0f - shadowPosition.Y, 0f - shadowPosition.X));
		_3A_0019[2]._6n(new Vector3(0f - shadowPosition.X, 0f - shadowPosition.Z, shadowPosition.Y));
		_3A_0019[3]._6n(new Vector3(shadowPosition.X, 0f - shadowPosition.Z, 0f - shadowPosition.Y));
		_3A_0019[4]._6n(new Vector3(shadowPosition.X, 0f - shadowPosition.Y, shadowPosition.Z));
		_3A_0019[5]._6n(new Vector3(0f - shadowPosition.X, 0f - shadowPosition.Y, 0f - shadowPosition.Z));
		_3A3[0].D = shadowPosition.X + radius;
		_3A3[1].D = 0f - shadowPosition.X + radius;
		_3A3[2].D = shadowPosition.Y + radius;
		_3A3[3].D = 0f - shadowPosition.Y + radius;
		_3A3[4].D = shadowPosition.Z + radius;
		_3A3[5].D = 0f - shadowPosition.Z + radius;
		shadowPosition = base.SceneState.ViewToWorld.Translation;
		float num = 0f;
		for (int i = 0; i < _3A_0019.Length; i++)
		{
			ShadowMapSurface shadowMapSurface = _3A_0019[i];
			if (!shadowMapSurface.Enabled)
			{
				shadowMapSurface.LevelOfDetail = 0f;
				continue;
			}
			Plane plane = _3A3[i];
			float num2 = plane.DotCoordinate(shadowPosition);
			Vector3 vector = shadowPosition - plane.Normal * num2;
			for (int j = 0; j < _3A_0019.Length; j++)
			{
				Plane plane2 = _3A3[j];
				float num3 = plane2.DotCoordinate(vector);
				if (num3 < 0f)
				{
					vector -= plane2.Normal * num3;
				}
			}
			num2 = (vector - shadowPosition).Length();
			float screenSize = CoreHelper.GetScreenSize(radius, num2, base.SceneState.Projection);
			shadowMapSurface.LevelOfDetail = MathHelper.Clamp(screenSize, 0f, 1f);
			num = Math.Max(num, shadowMapSurface.LevelOfDetail);
		}
		if (!shadowSource.ShadowPerSurfaceLOD)
		{
			ShadowMapSurface[] array = _3A_0019;
			foreach (ShadowMapSurface shadowMapSurface2 in array)
			{
				shadowMapSurface2.LevelOfDetail = num;
			}
		}
		if (ShadowEffect is IRenderableEffect)
		{
			(ShadowEffect as IRenderableEffect).World = Matrix.Identity;
		}
		if (ShadowEffect is IShadowGenerateEffect)
		{
			(ShadowEffect as IShadowGenerateEffect).ShadowArea = shadowgroup.BoundingSphereCentered;
		}
	}

	/// <summary>
	/// Sets the location in the shadow map render target the surface renders to.
	/// </summary>
	/// <param name="surface">Shadow map surface index.</param>
	/// <param name="location">Texel region used by the shadow map surface.</param>
	public override void SetSurfaceRenderTargetLocation(int surface, Rectangle location)
	{
		ShadowMapSurface shadowMapSurface = _3A_0019[surface];
		shadowMapSurface.RenderTargetLocation = location;
		float num = (float)location.Width * 0.5f;
		float num2 = (float)shadowMapSurface._6G(8).Width * 0.5f;
		float fieldOfView = ((!(num2 > 0f)) ? MathHelper.ToRadians(90f) : ((float)Math.Atan(num / num2) * 2f));
		float num3 = 10000f;
		if (base.ShadowGroup.ShadowSource is IPointSource)
		{
			num3 = base.ShadowGroup.BoundingSphereCentered.Radius;
		}
		if (num3 <= 0f)
		{
			num3 = 1E-05f;
		}
		float nearPlaneDistance = num3 * 1E-05f;
		Matrix projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, 1f, nearPlaneDistance, num3);
		projection.M11 *= -1f;
		_3A_0019[surface].Projection = projection;
	}

	/// <summary>
	/// Determines if the shadow map surface is visible to the provided view frustum.
	/// </summary>
	/// <param name="surface">Shadow map surface index.</param>
	/// <param name="viewfrustum"></param>
	/// <returns></returns>
	public override bool IsSurfaceVisible(int surface, BoundingFrustum viewfrustum)
	{
		ShadowMapSurface shadowMapSurface = _3A_0019[surface];
		return shadowMapSurface.Enabled;
	}

	/// <summary>
	/// Sets up the shadow map for rendering shadows to the scene.
	/// </summary>
	/// <param name="shadowmap"></param>
	public override void BeginRendering(Texture shadowmap)
	{
		BeginRendering(shadowmap, ShadowEffect);
	}

	/// <summary>
	/// Sets up the shadow map for rendering shadows to the scene.
	/// </summary>
	/// <param name="shadowmap"></param>
	/// <param name="shadoweffect">Custom shadow effect used in rendering.</param>
	public override void BeginRendering(Texture shadowmap, Effect shadoweffect)
	{
		EffectTypeCaster effectTypeCaster = OptimizationSystem.EffectTypeCasters.Get(shadoweffect);
		if (!(shadowmap is Texture2D shadowmap2))
		{
			effectTypeCaster._3A_0018.SetShadowMapAndType(null, _8._3.Point);
			return;
		}
		IRenderableEffect renderableEffect = effectTypeCaster.RenderableEffect;
		_8._0019 obj = effectTypeCaster._3A_0018;
		IShadowGenerateEffect shadowGenerateEffect = effectTypeCaster.ShadowGenerateEffect;
		obj.SetShadowMapAndType(shadowmap2, _8._3.Point);
		renderableEffect?.SetViewAndProjection(base.SceneState.View, base.SceneState.ViewToWorld, base.SceneState.Projection, base.SceneState.ProjectionToView);
		if (shadowGenerateEffect != null)
		{
			shadowGenerateEffect.ShadowPrimaryBias = base.ShadowGroup.ShadowSource.ShadowPrimaryBias;
			shadowGenerateEffect.ShadowSecondaryBias = base.ShadowGroup.ShadowSource.ShadowSecondaryBias;
		}
		obj.ShadowArea = base.ShadowGroup.BoundingSphereCentered;
		obj.ShadowMapLocationAndSpan = GetPackedRenderTargetLocationAndSpan(shadowmap2, 8);
	}

	/// <summary>
	/// Finalizes rendering.
	/// </summary>
	public override void EndRendering()
	{
	}

	/// <summary>
	/// Sets up the shadow map surface for generating the shadow map depth buffer.
	/// </summary>
	/// <param name="surface">Shadow map surface index.</param>
	public override void BeginSurfaceRendering(int surface)
	{
		BeginSurfaceRendering(surface, ShadowEffect);
	}

	/// <summary>
	/// Sets up the shadow map surface for generating the shadow map depth buffer.
	/// </summary>
	/// <param name="surface">Shadow map surface index.</param>
	/// <param name="shadoweffect">Custom shadow effect used in rendering.</param>
	public override void BeginSurfaceRendering(int surface, Effect shadoweffect)
	{
		EffectTypeCaster effectTypeCaster = OptimizationSystem.EffectTypeCasters.Get(shadoweffect);
		ShadowMapSurface shadowMapSurface = _3A_0019[surface];
		IRenderableEffect renderableEffect = effectTypeCaster.RenderableEffect;
		_8._0019 obj = effectTypeCaster._3A_0018;
		IShadowGenerateEffect shadowGenerateEffect = effectTypeCaster.ShadowGenerateEffect;
		obj?.SetShadowMapAndType(null, _8._3.Point);
		renderableEffect?.SetViewAndProjection(shadowMapSurface.WorldToSurfaceView, Matrix.Identity, shadowMapSurface.Projection, base.SceneState.ProjectionToView);
		if (shadowGenerateEffect != null)
		{
			shadowGenerateEffect.ShadowPrimaryBias = base.ShadowGroup.ShadowSource.ShadowPrimaryBias;
			shadowGenerateEffect.ShadowSecondaryBias = base.ShadowGroup.ShadowSource.ShadowSecondaryBias;
			shadowGenerateEffect.ShadowArea = base.ShadowGroup.BoundingSphereCentered;
			shadowGenerateEffect.SetCameraView(base.SceneState.View, base.SceneState.ViewToWorld);
		}
		else if (obj != null)
		{
			obj.ShadowArea = base.ShadowGroup.BoundingSphereCentered;
		}
		base.Device.Viewport = _3A_0019[surface].Viewport;
	}

	/// <summary>
	/// Finalizes rendering.
	/// </summary>
	public override void EndSurfaceRendering()
	{
	}
}
