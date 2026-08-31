using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Lights;

namespace SynapseGaming.LightingSystem.Effects.Forward;

/// <summary>
/// Effect provides SunBurn's built-in lighting and material support.
///
/// Including:
/// -Diffuse mapping
/// -Bump mapping
/// -Specular mapping (with specular intensity mapping)
/// -Point, spot, directional, and ambient lighting
/// </summary>
public class LightingEffect : BaseMaterialEffect, ILightingEffect
{
	private const int HCB = 1;

	private int HC_0002;

	private EffectParameter HC_0012;

	private EffectParameter HCH;

	private EffectParameter HC7;

	private EffectParameter HC_0001;

	private static Vector4 HCw = default(Vector4);

	private static Vector4 HCZ = default(Vector4);

	private static Vector4 HC_000F = default(Vector4);

	/// <summary>
	/// Maximum number of light sources the effect supports.
	/// </summary>
	public int MaxLightSources => 1;

	/// <summary>
	/// Light sources that apply lighting to the effect during rendering.
	/// </summary>
	public List<BaseLight> LightSources
	{
		set
		{
			HH(value);
			HCV.HC_0012.AccumulationValue++;
		}
	}

	private void HH(List<BaseLight> P_0)
	{
		if (HCH == null || HC7 == null || HC_0001 == null || P_0 == null)
		{
			return;
		}
		if (P_0.Count != 1)
		{
			throw new ArgumentException("LightingEffect only supports a single light per-pass at this time.");
		}
		BaseLight baseLight = P_0[0];
		bool flag = baseLight == _CurrentLight;
		_CurrentLight = baseLight;
		Vector3 compositeColorAndIntensity = _CurrentLight.CompositeColorAndIntensity;
		HCw = new Vector4(compositeColorAndIntensity, 0f);
		HC_000F = default(Vector4);
		LightTypeCaster lightTypeCaster = OptimizationSystem.LightTypeCasters.Get(_CurrentLight);
		IPointSource pointSource = lightTypeCaster.PointSource;
		if (pointSource != null)
		{
			ISpotSource spotSource = lightTypeCaster.SpotSource;
			if (spotSource != null)
			{
				float value = spotSource.Angle * 0.5f;
				float num = (float)Math.Cos(MathHelper.ToRadians(MathHelper.Clamp(value, 0.01f, 89.99f)));
				float num2 = 1f / (1f - num);
				HCw.W = num2;
				HC_000F = new Vector4(spotSource.Direction, num);
				HCZ = new Vector4(spotSource.Position, spotSource.Radius);
			}
			else
			{
				HCZ = new Vector4(pointSource.Position, pointSource.Radius);
			}
		}
		else if (lightTypeCaster.ShadowSource != null)
		{
			HCZ = new Vector4(lightTypeCaster.ShadowSource.ShadowPosition, 1E+09f);
		}
		HCH.SetValue(HCw);
		HC7.SetValue(HCZ);
		HC_0001.SetValue(HC_000F);
		if (!flag)
		{
			SetTechnique();
		}
	}

	/// <summary>
	/// Sets the EffectParameter(s) associated with the index into the current technique's
	/// shader array. This method cannot change the current technique, instead use SetTechnique().
	/// </summary>
	protected override void SetTechniqueShaderArrayIndices()
	{
		if (HC_0012 != null)
		{
			int num = 0;
			if (base.TransparencyMap != null)
			{
				_ = TransparencyMode;
			}
			if (base.EffectDetail <= DetailPreference.Medium)
			{
				_ = TransparencyMode;
			}
			num = ((_NormalMapTexture == null) ? (num | ((int)_CurrentStaticLightingEffectMode << 1)) : (num | ((int)_CurrentStaticLightingEffectMode << 2)));
			if (num != HC_0002)
			{
				HC_0002 = num;
				HC_0012.SetValue(HC_0002);
				_UpdatedByBatch = true;
			}
		}
	}

	/// <summary>
	/// Creates a new LightingEffect instance.
	/// </summary>
	/// <param name="graphicsdevice"></param>
	public LightingEffect(GraphicsDevice graphicsdevice)
		: base(graphicsdevice, "LightingEffect")
	{
		B(graphicsdevice);
	}

	internal LightingEffect(GraphicsDevice P_0, bool P_1)
		: base(P_0, "LightingEffect", P_1)
	{
		B(P_0);
	}

	/// <summary>
	/// Creates a new empty effect of the same class type and using the same effect file as this object.
	/// </summary>
	/// <returns></returns>
	protected override Effect Create()
	{
		return new LightingEffect(base.GraphicsDevice);
	}

	private void B(GraphicsDevice P_0)
	{
		HC_0012 = base.Parameters["_PixelShaderIndex"];
		HCH = base.Parameters["_DiffuseColor_And_SpotAngleInv"];
		HC7 = base.Parameters["_Position_And_Radius"];
		HC_0001 = base.Parameters["_SpotDirection_And_SpotAngle"];
	}
}
