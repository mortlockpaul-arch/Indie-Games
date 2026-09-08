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
	private const int _3A_0018 = 1;

	private int _3AL;

	private EffectParameter _3A_0019;

	private EffectParameter _3A3;

	private EffectParameter _3A6;

	private EffectParameter _3AD;

	private static Vector4 _3A_0017 = default(Vector4);

	private static Vector4 _3A_0003 = default(Vector4);

	private static Vector4 _3Al = default(Vector4);

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
			_3_0019(value);
			_3A7._3A_0019.AccumulationValue++;
		}
	}

	private void _3_0019(List<BaseLight> P_0)
	{
		if (_3A3 == null || _3A6 == null || _3AD == null || P_0 == null)
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
		_3A_0017 = new Vector4(compositeColorAndIntensity, 0f);
		_3Al = default(Vector4);
		LightTypeCaster lightTypeCaster = OptimizationSystem.LightTypeCasters.Get(_CurrentLight);
		IPointSource pointSource = lightTypeCaster.PointSource;
		if (pointSource != null)
		{
			ISpotSource spotSource = lightTypeCaster.SpotSource;
			if (spotSource != null)
			{
				float value = spotSource.Angle * 0.5f;
				float num = (float)Math.Cos(MathHelper.ToRadians(MathHelper.Clamp(value, 0.01f, 89.99f)));
				float w = 1f / (1f - num);
				_3A_0017.W = w;
				_3Al = new Vector4(spotSource.Direction, num);
				_3A_0003 = new Vector4(spotSource.Position, spotSource.Radius);
			}
			else
			{
				_3A_0003 = new Vector4(pointSource.Position, pointSource.Radius);
			}
		}
		else if (lightTypeCaster.ShadowSource != null)
		{
			_3A_0003 = new Vector4(lightTypeCaster.ShadowSource.ShadowPosition, 1E+09f);
		}
		_3A3.SetValue(_3A_0017);
		_3A6.SetValue(_3A_0003);
		_3AD.SetValue(_3Al);
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
		if (_3A_0019 != null)
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
			if (num != _3AL)
			{
				_3AL = num;
				_3A_0019.SetValue(_3AL);
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
		_0018(graphicsdevice);
	}

	internal LightingEffect(GraphicsDevice P_0, bool P_1)
		: base(P_0, "LightingEffect", P_1)
	{
		_0018(P_0);
	}

	/// <summary>
	/// Creates a new empty effect of the same class type and using the same effect file as this object.
	/// </summary>
	/// <returns></returns>
	protected override Effect Create()
	{
		return new LightingEffect(base.GraphicsDevice);
	}

	private void _0018(GraphicsDevice P_0)
	{
		_3A_0019 = base.Parameters["_PixelShaderIndex"];
		_3A3 = base.Parameters["_DiffuseColor_And_SpotAngleInv"];
		_3A6 = base.Parameters["_Position_And_Radius"];
		_3AD = base.Parameters["_SpotDirection_And_SpotAngle"];
	}
}
