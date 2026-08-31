using System;
using _0003;
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
	private const int HCB = 6;

	private const int HC_0002 = 8;

	private ShadowMapSurface[] HC_0012 = new ShadowMapSurface[6];

	private Plane[] HCH = new Plane[6];

	private static bool HC7 = false;

	private static Matrix[] HC_0001 = new Matrix[6];

	private static Plane[] HCw = new Plane[6];

	/// <summary>
	/// Array of the cube-map surfaces.
	/// </summary>
	public override ShadowMapSurface[] Surfaces => HC_0012;

	/// <summary>
	/// Unused, this object supports render targets from the ShadowMapCache.
	/// </summary>
	public override RenderTarget2D CustomRenderTarget => null;

	/// <summary>
	/// Creates a new ShadowCubeMap instance.
	/// </summary>
	public BaseShadowCubeMap()
	{
		if (!HC7)
		{
			ref Matrix reference = ref HC_0001[0];
			reference = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitX, Vector3.UnitY);
			ref Matrix reference2 = ref HC_0001[1];
			reference2 = Matrix.CreateLookAt(Vector3.Zero, -Vector3.UnitX, Vector3.UnitY);
			ref Matrix reference3 = ref HC_0001[2];
			reference3 = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitY, Vector3.UnitZ);
			ref Matrix reference4 = ref HC_0001[3];
			reference4 = Matrix.CreateLookAt(Vector3.Zero, -Vector3.UnitY, Vector3.UnitZ);
			ref Matrix reference5 = ref HC_0001[4];
			reference5 = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
			ref Matrix reference6 = ref HC_0001[5];
			reference6 = Matrix.CreateLookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
			for (int i = 0; i < HCw.Length; i++)
			{
				ref Plane reference7 = ref HCw[i];
				reference7 = Plane.Transform(new Plane(0f, 0f, 1f, 1f), Matrix.Invert(HC_0001[i]));
			}
			HC7 = true;
		}
		for (int j = 0; j < HC_0012.Length; j++)
		{
			HC_0012[j] = new ShadowMapSurface();
		}
		HC_0012[0].WorldToSurfaceView = HC_0001[0];
		HC_0012[1].WorldToSurfaceView = HC_0001[1];
		HC_0012[2].WorldToSurfaceView = HC_0001[2];
		HC_0012[3].WorldToSurfaceView = HC_0001[3];
		HC_0012[4].WorldToSurfaceView = HC_0001[4];
		HC_0012[5].WorldToSurfaceView = HC_0001[5];
		for (int k = 0; k < HC_0012.Length; k++)
		{
			ref Plane reference8 = ref HCH[k];
			reference8 = HCw[k];
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
		HC_0012[0]._7U(new Vector3(0f - shadowPosition.Z, 0f - shadowPosition.Y, shadowPosition.X));
		HC_0012[1]._7U(new Vector3(shadowPosition.Z, 0f - shadowPosition.Y, 0f - shadowPosition.X));
		HC_0012[2]._7U(new Vector3(0f - shadowPosition.X, 0f - shadowPosition.Z, shadowPosition.Y));
		HC_0012[3]._7U(new Vector3(shadowPosition.X, 0f - shadowPosition.Z, 0f - shadowPosition.Y));
		HC_0012[4]._7U(new Vector3(shadowPosition.X, 0f - shadowPosition.Y, shadowPosition.Z));
		HC_0012[5]._7U(new Vector3(0f - shadowPosition.X, 0f - shadowPosition.Y, 0f - shadowPosition.Z));
		HCH[0].D = shadowPosition.X + radius;
		HCH[1].D = 0f - shadowPosition.X + radius;
		HCH[2].D = shadowPosition.Y + radius;
		HCH[3].D = 0f - shadowPosition.Y + radius;
		HCH[4].D = shadowPosition.Z + radius;
		HCH[5].D = 0f - shadowPosition.Z + radius;
		shadowPosition = base.SceneState.ViewToWorld.Translation;
		float num = 0f;
		for (int i = 0; i < HC_0012.Length; i++)
		{
			ShadowMapSurface shadowMapSurface = HC_0012[i];
			if (!shadowMapSurface.Enabled)
			{
				shadowMapSurface.LevelOfDetail = 0f;
				continue;
			}
			Plane plane = HCH[i];
			float num2 = plane.DotCoordinate(shadowPosition);
			Vector3 vector = shadowPosition - plane.Normal * num2;
			for (int j = 0; j < HC_0012.Length; j++)
			{
				Plane plane2 = HCH[j];
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
			ShadowMapSurface[] array = HC_0012;
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
		ShadowMapSurface shadowMapSurface = HC_0012[surface];
		shadowMapSurface.RenderTargetLocation = location;
		float num = (float)location.Width * 0.5f;
		float num2 = (float)shadowMapSurface._7_0015(8).Width * 0.5f;
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
		HC_0012[surface].Projection = projection;
	}

	/// <summary>
	/// Determines if the shadow map surface is visible to the provided view frustum.
	/// </summary>
	/// <param name="surface">Shadow map surface index.</param>
	/// <param name="viewfrustum"></param>
	/// <returns></returns>
	public override bool IsSurfaceVisible(int surface, BoundingFrustum viewfrustum)
	{
		ShadowMapSurface shadowMapSurface = HC_0012[surface];
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
			effectTypeCaster.HCB.SetShadowMapAndType(null, _0003.H.Point);
			return;
		}
		IRenderableEffect renderableEffect = effectTypeCaster.RenderableEffect;
		_0003._0012 hCB = effectTypeCaster.HCB;
		IShadowGenerateEffect shadowGenerateEffect = effectTypeCaster.ShadowGenerateEffect;
		hCB.SetShadowMapAndType(shadowmap2, _0003.H.Point);
		renderableEffect?.SetViewAndProjection(base.SceneState.View, base.SceneState.ViewToWorld, base.SceneState.Projection, base.SceneState.ProjectionToView);
		if (shadowGenerateEffect != null)
		{
			shadowGenerateEffect.ShadowPrimaryBias = base.ShadowGroup.ShadowSource.ShadowPrimaryBias;
			shadowGenerateEffect.ShadowSecondaryBias = base.ShadowGroup.ShadowSource.ShadowSecondaryBias;
		}
		hCB.ShadowArea = base.ShadowGroup.BoundingSphereCentered;
		hCB.ShadowMapLocationAndSpan = GetPackedRenderTargetLocationAndSpan(shadowmap2, 8);
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
		ShadowMapSurface shadowMapSurface = HC_0012[surface];
		IRenderableEffect renderableEffect = effectTypeCaster.RenderableEffect;
		_0003._0012 hCB = effectTypeCaster.HCB;
		IShadowGenerateEffect shadowGenerateEffect = effectTypeCaster.ShadowGenerateEffect;
		hCB?.SetShadowMapAndType(null, _0003.H.Point);
		renderableEffect?.SetViewAndProjection(shadowMapSurface.WorldToSurfaceView, Matrix.Identity, shadowMapSurface.Projection, base.SceneState.ProjectionToView);
		if (shadowGenerateEffect != null)
		{
			shadowGenerateEffect.ShadowPrimaryBias = base.ShadowGroup.ShadowSource.ShadowPrimaryBias;
			shadowGenerateEffect.ShadowSecondaryBias = base.ShadowGroup.ShadowSource.ShadowSecondaryBias;
			shadowGenerateEffect.ShadowArea = base.ShadowGroup.BoundingSphereCentered;
			shadowGenerateEffect.SetCameraView(base.SceneState.View, base.SceneState.ViewToWorld);
		}
		else if (hCB != null)
		{
			hCB.ShadowArea = base.ShadowGroup.BoundingSphereCentered;
		}
		base.Device.Viewport = HC_0012[surface].Viewport;
	}

	/// <summary>
	/// Finalizes rendering.
	/// </summary>
	public override void EndSurfaceRendering()
	{
	}
}
