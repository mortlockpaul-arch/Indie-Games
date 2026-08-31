using System;
using F;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Shadows;

/// <summary>
/// Base shadow map class that provides support for the built-in ShadowEffect.
/// </summary>
public abstract class BaseShadowEffectShadowMap : BaseShadowMap
{
	private Effect HCB;

	private Vector4[] HC_0002 = new Vector4[6];

	private Matrix[] HC_0012 = new Matrix[6];

	/// <summary>
	/// Effect used for shadow map rendering.
	/// </summary>
	public override Effect ShadowEffect => HCB;

	/// <summary>
	/// Gets the effect type that performs rendering specific to the shadow
	/// mapping implementation used by this object.
	/// </summary>
	/// <returns></returns>
	protected abstract Type GetEffectType();

	/// <summary>
	/// Creates a new effect that performs rendering specific to the shadow
	/// mapping implementation used by this object.
	/// </summary>
	/// <returns></returns>
	protected abstract Effect CreateEffect();

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
		if (HCB == null)
		{
			HCB = ResourceManager._0002R(GetEffectType(), CreateEffect);
		}
	}

	/// <summary>
	/// Releases resources allocated by this object.
	/// </summary>
	public override void Dispose()
	{
		F.B._7_0004(ref HCB);
		base.Dispose();
	}

	/// <summary>
	/// Creates packed surface information used by the built-in ShadowEffect.
	/// </summary>
	/// <param name="shadowmap"></param>
	/// <param name="padding">Width of pixel padding used to avoid edge artifacts.</param>
	/// <returns></returns>
	protected Vector4[] GetPackedRenderTargetLocationAndSpan(Texture2D shadowmap, int padding)
	{
		Vector4 vector = new Vector4(1f / (float)shadowmap.Width, 1f / (float)shadowmap.Height, 1f / (float)shadowmap.Width, 1f / (float)shadowmap.Height);
		for (int i = 0; i < Surfaces.Length; i++)
		{
			Rectangle rectangle = Surfaces[i]._7_0015(padding);
			ref Vector4 reference = ref HC_0002[i];
			reference = new Vector4(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height) * vector;
		}
		return HC_0002;
	}

	/// <summary>
	/// Creates packed surface transforms used by the built-in ShadowEffect.
	/// </summary>
	/// <returns></returns>
	protected Matrix[] GetPackedSurfaceViewProjection()
	{
		for (int i = 0; i < Surfaces.Length; i++)
		{
			ref Matrix reference = ref HC_0012[i];
			reference = Surfaces[i].WorldToSurfaceView * Surfaces[i].Projection;
		}
		return HC_0012;
	}
}
