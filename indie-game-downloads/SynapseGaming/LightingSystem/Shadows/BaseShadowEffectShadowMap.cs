using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using p;

namespace SynapseGaming.LightingSystem.Shadows;

/// <summary>
/// Base shadow map class that provides support for the built-in ShadowEffect.
/// </summary>
public abstract class BaseShadowEffectShadowMap : BaseShadowMap
{
	private Effect _3A_0018;

	private Vector4[] _3AL = new Vector4[6];

	private Matrix[] _3A_0019 = new Matrix[6];

	/// <summary>
	/// Effect used for shadow map rendering.
	/// </summary>
	public override Effect ShadowEffect => _3A_0018;

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
		if (_3A_0018 == null)
		{
			_3A_0018 = ResourceManager.L_0016(GetEffectType(), CreateEffect);
		}
	}

	/// <summary>
	/// Releases resources allocated by this object.
	/// </summary>
	public override void Dispose()
	{
		p._0018._6_0006(ref _3A_0018);
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
			Rectangle rectangle = Surfaces[i]._6G(padding);
			ref Vector4 reference = ref _3AL[i];
			reference = new Vector4(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height) * vector;
		}
		return _3AL;
	}

	/// <summary>
	/// Creates packed surface transforms used by the built-in ShadowEffect.
	/// </summary>
	/// <returns></returns>
	protected Matrix[] GetPackedSurfaceViewProjection()
	{
		for (int i = 0; i < Surfaces.Length; i++)
		{
			ref Matrix reference = ref _3A_0019[i];
			reference = Surfaces[i].WorldToSurfaceView * Surfaces[i].Projection;
		}
		return _3A_0019;
	}
}
