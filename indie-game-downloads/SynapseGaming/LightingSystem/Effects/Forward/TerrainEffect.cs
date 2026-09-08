using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Lights;

namespace SynapseGaming.LightingSystem.Effects.Forward;

/// <summary>
/// Provides SunBurn's built-in forward terrain rendering.
/// </summary>
public class TerrainEffect : BaseTerrainEffect, ILightingEffect
{
	private const int _3A_0018 = 1;

	private BaseLight _3AL = new AmbientLight();

	private EffectParameter _3A_0019;

	private EffectParameter _3A3;

	private EffectParameter _3A6;

	private EffectParameter _3AD;

	private static Vector4 _3A_0017;

	private static Vector4 _3A_0003;

	private static Vector4 _3Al;

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
			if (value.Count != 1)
			{
				throw new ArgumentException("TerrainEffect only supports a single light per-pass at this time.");
			}
			_3AL = value[0];
			_3_0019();
			_3A7._3A_0019.AccumulationValue++;
		}
	}

	private void _3_0019()
	{
		if (_3A3 == null || _3A6 == null || _3AD == null || _3AL == null)
		{
			return;
		}
		_3A_0017 = new Vector4(_3AL.CompositeColorAndIntensity, 0f);
		_3Al = default(Vector4);
		LightTypeCaster lightTypeCaster = OptimizationSystem.LightTypeCasters.Get(_3AL);
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
		SetTechnique();
	}

	/// <summary>
	/// Sets the effect technique based on its current property values.
	/// </summary>
	protected override void SetTechnique()
	{
		if (_3AL is IAmbientSource)
		{
			base.CurrentTechnique = base.Techniques["Terrain_Ambient_Technique"];
		}
		else
		{
			base.CurrentTechnique = base.Techniques["Terrain_Technique"];
		}
	}

	/// <summary>
	/// Creates a new TerrainEffect instance.
	/// </summary>
	/// <param name="graphicsdevice"></param>
	public TerrainEffect(GraphicsDevice graphicsdevice)
		: base(graphicsdevice, "TerrainEffect")
	{
		_0018(graphicsdevice);
	}

	internal TerrainEffect(GraphicsDevice P_0, bool P_1)
		: base(P_0, "TerrainEffect", P_1)
	{
		_0018(P_0);
	}

	private void _0018(GraphicsDevice P_0)
	{
		_3A_0019 = base.Parameters["_LightingTexture"];
		_3A3 = base.Parameters["_DiffuseColor_And_SpotAngleInv"];
		_3A6 = base.Parameters["_Position_And_Radius"];
		_3AD = base.Parameters["_SpotDirection_And_SpotAngle"];
	}

	/// <summary>
	/// Creates a new empty effect of the same class type and using the same effect file as this object.
	/// </summary>
	/// <returns></returns>
	protected override Effect Create()
	{
		return new TerrainEffect(base.GraphicsDevice);
	}
}
