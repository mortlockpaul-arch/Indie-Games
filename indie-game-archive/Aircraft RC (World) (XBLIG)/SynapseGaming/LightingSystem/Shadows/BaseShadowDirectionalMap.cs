using System;
using _0003;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Effects;

namespace SynapseGaming.LightingSystem.Shadows;

/// <summary>
/// Shadow map class that implements cascading level-of-detail
/// directional shadows. Used for directional lights.
/// </summary>
public abstract class BaseShadowDirectionalMap : BaseShadowEffectShadowMap
{
	private const int HCB = 3;

	private float[] HC_0002 = new float[4];

	private float HC_0012 = 250f;

	private float HCH = 300f;

	private float HC7 = 300f;

	private Vector4 HC_0001;

	private ShadowMapSurface[] HCw = new ShadowMapSurface[3];

	private BoundingFrustum HCZ = new BoundingFrustum(Matrix.Identity);

	private Vector3[] HC_000F = new Vector3[8];

	/// <summary>
	/// Array of the level-of-detail surfaces.
	/// </summary>
	public override ShadowMapSurface[] Surfaces => HCw;

	/// <summary>
	/// Unused, this object supports render targets from the ShadowMapCache.
	/// </summary>
	public override RenderTarget2D CustomRenderTarget => null;

	/// <summary>
	/// Creates a new ShadowDirectionalMap instance.
	/// </summary>
	public BaseShadowDirectionalMap()
	{
		for (int i = 0; i < HCw.Length; i++)
		{
			HCw[i] = new ShadowMapSurface();
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
		HC_0012 = scenestate.Environment.ShadowFadeStartDistance;
		HCH = scenestate.Environment.ShadowFadeEndDistance;
		HC7 = scenestate.Environment.ShadowCasterDistance;
		HC_0001.W = scenestate.Environment.ShadowFadeStartDistance;
		int num = Math.Min(HC_0002.Length - 1, shadowvisibility.ShadowLODRangeHints.Length);
		for (int i = 0; i < num; i++)
		{
			HC_0002[i + 1] = shadowvisibility.ShadowLODRangeHints[i];
		}
		bool[] shadowLODEnabled = shadowvisibility.ShadowLODEnabled;
		for (int j = 0; j < HCw.Length; j++)
		{
			ShadowMapSurface shadowMapSurface = HCw[j];
			if (shadowLODEnabled.Length > j && !shadowLODEnabled[j])
			{
				shadowMapSurface.Enabled = false;
				shadowMapSurface.LevelOfDetail = 0f;
			}
			else
			{
				shadowMapSurface.Enabled = true;
				shadowMapSurface.LevelOfDetail = 1f;
			}
		}
	}

	private BoundingBox _78(Vector3[] P_0, Matrix P_1)
	{
		if (P_0.Length < 1)
		{
			return default(BoundingBox);
		}
		Vector3 vector = Vector3.Transform(P_0[0], P_1);
		BoundingBox result = new BoundingBox(vector, vector);
		for (int i = 1; i < P_0.Length; i++)
		{
			vector = Vector3.Transform(P_0[i], P_1);
			result.Min = Vector3.Min(result.Min, vector);
			result.Max = Vector3.Max(result.Max, vector);
		}
		return result;
	}

	/// <summary>
	/// Sets the location in the shadow map render target the surface renders to.
	/// </summary>
	/// <param name="surface">Shadow map surface index.</param>
	/// <param name="location">Texel region used by the shadow map surface.</param>
	public override void SetSurfaceRenderTargetLocation(int surface, Rectangle location)
	{
		IShadowSource shadowSource = base.ShadowGroup.ShadowSource;
		ShadowMapSurface shadowMapSurface = HCw[surface];
		shadowMapSurface.RenderTargetLocation = location;
		float d = HCH * HC_0002[surface];
		float num = HCH * HC_0002[surface + 1];
		switch (surface)
		{
		case 0:
			HC_0001.X = num;
			break;
		case 1:
			HC_0001.Y = num;
			break;
		default:
			HC_0001.Z = num;
			break;
		}
		HCZ.Matrix = base.SceneState.ProjectionNonOblique;
		HCZ.GetCorners(HC_000F);
		Plane plane = new Plane(0f, 0f, 1f, d);
		Plane plane2 = new Plane(0f, 0f, 1f, num);
		Vector3 intersectionpoint = default(Vector3);
		for (int i = 0; i < 4; i++)
		{
			Vector3 start = HC_000F[i];
			Vector3 end = HC_000F[i + 4];
			if (CoreHelper.Intersects(start, end, plane, ref intersectionpoint))
			{
				HC_000F[i] = intersectionpoint;
			}
			if (CoreHelper.Intersects(start, end, plane2, ref intersectionpoint))
			{
				HC_000F[i + 4] = intersectionpoint;
			}
		}
		Vector3 position = HC_000F[0];
		for (int j = 1; j < 8; j++)
		{
			position += HC_000F[j];
		}
		position /= 8f;
		Matrix viewToWorld = base.SceneState.ViewToWorld;
		Vector3 vector = Vector3.Transform(position, viewToWorld);
		float hC = HC7;
		Vector3 vector2 = vector - shadowSource.World.Forward * hC;
		Matrix matrix = Matrix.CreateTranslation(vector2);
		Matrix matrix2 = Matrix.Invert(matrix) * Matrix.Invert(shadowSource.World);
		Matrix matrix3 = viewToWorld * matrix2;
		for (int k = 0; k < 8; k++)
		{
			ref Vector3 reference = ref HC_000F[k];
			reference = Vector3.Transform(HC_000F[k], matrix3);
		}
		float num2 = Math.Max(Vector3.Distance(HC_000F[0], HC_000F[2]), Vector3.Distance(HC_000F[0], HC_000F[6]));
		CoreHelper.CreateBoundingBoxFromPoints(HC_000F);
		shadowMapSurface.WorldToSurfaceView = matrix2;
		shadowMapSurface.Projection = Matrix.CreateOrthographic(num2, num2, hC * 0.25f, hC * 1.75f) * Matrix.CreateScale(-1f, 1f, 1f);
		int width = location.Width;
		Vector4 vector3 = Vector4.Transform(new Vector4(vector2, 1f), shadowMapSurface.Frustum.Matrix);
		vector3 = (vector3 + Vector4.One) * 0.5f;
		vector3 *= new Vector4(width);
		Vector4 vector4 = Vector4.Transform(new Vector4(Vector3.Zero, 1f), shadowMapSurface.Frustum.Matrix);
		vector4 = (vector4 + Vector4.One) * 0.5f;
		vector4 *= new Vector4(width);
		vector3.X += vector4.X % 1f;
		vector3.Y += vector4.Y % 1f;
		vector3 /= new Vector4(width);
		vector3 = vector3 * 2f - Vector4.One;
		vector3 = Vector4.Transform(vector3, Matrix.Invert(shadowMapSurface.Frustum.Matrix));
		Matrix matrix4 = Matrix.Invert(matrix2);
		matrix4.Translation = new Vector3(vector3.X, vector3.Y, vector3.Z);
		shadowMapSurface.WorldToSurfaceView = Matrix.Invert(matrix4);
	}

	/// <summary>
	/// Determines if the shadow map surface is visible to the provided view frustum.
	/// </summary>
	/// <param name="surface">Shadow map surface index.</param>
	/// <param name="viewfrustum"></param>
	/// <returns></returns>
	public override bool IsSurfaceVisible(int surface, BoundingFrustum viewfrustum)
	{
		return HCw[surface].Enabled;
	}

	private int _7j(float P_0, int P_1, int P_2)
	{
		float num = P_0 * 0.5f + 0.5f;
		return (int)MathHelper.Clamp(num * (float)P_1 + (float)P_2, 0f, P_1);
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
			effectTypeCaster.HCB.SetShadowMapAndType(null, _0003.H.Directional);
			return;
		}
		IRenderableEffect renderableEffect = effectTypeCaster.RenderableEffect;
		_0003._0012 hCB = effectTypeCaster.HCB;
		IShadowGenerateEffect shadowGenerateEffect = effectTypeCaster.ShadowGenerateEffect;
		hCB.SetShadowMapAndType(shadowmap2, _0003.H.Directional);
		hCB.ShadowViewDistance = HC_0001;
		renderableEffect?.SetViewAndProjection(base.SceneState.View, base.SceneState.ViewToWorld, base.SceneState.Projection, base.SceneState.ProjectionToView);
		if (shadowGenerateEffect != null)
		{
			shadowGenerateEffect.ShadowPrimaryBias = base.ShadowGroup.ShadowSource.ShadowPrimaryBias;
			shadowGenerateEffect.ShadowSecondaryBias = base.ShadowGroup.ShadowSource.ShadowSecondaryBias;
		}
		hCB.ShadowArea = base.ShadowGroup.BoundingSphereCentered;
		hCB.ShadowMapLocationAndSpan = GetPackedRenderTargetLocationAndSpan(shadowmap2, 0);
		hCB.ShadowViewProjection = GetPackedSurfaceViewProjection();
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
		ShadowMapSurface shadowMapSurface = HCw[surface];
		IRenderableEffect renderableEffect = effectTypeCaster.RenderableEffect;
		_0003._0012 hCB = effectTypeCaster.HCB;
		IShadowGenerateEffect shadowGenerateEffect = effectTypeCaster.ShadowGenerateEffect;
		hCB?.SetShadowMapAndType(null, _0003.H.Directional);
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
		base.Device.Viewport = HCw[surface].Viewport;
	}

	/// <summary>
	/// Finalizes rendering.
	/// </summary>
	public override void EndSurfaceRendering()
	{
	}
}
