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
	private const int HCB = 1;

	private BaseLight HC_0002 = new AmbientLight();

	private EffectParameter HC_0012;

	private EffectParameter HCH;

	private EffectParameter HC7;

	private EffectParameter HC_0001;

	private static Vector4 HCw;

	private static Vector4 HCZ;

	private static Vector4 HC_000F;

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
			HC_0002 = value[0];
			HH();
			HCV.HC_0012.AccumulationValue++;
		}
	}

	private void HH()
	{
		if (HCH == null || HC7 == null || HC_0001 == null || HC_0002 == null)
		{
			return;
		}
		HCw = new Vector4(HC_0002.CompositeColorAndIntensity, 0f);
		HC_000F = default(Vector4);
		LightTypeCaster lightTypeCaster = OptimizationSystem.LightTypeCasters.Get(HC_0002);
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
		SetTechnique();
	}

	/// <summary>
	/// Sets the effect technique based on its current property values.
	/// </summary>
	protected override void SetTechnique()
	{
		if (HC_0002 is IAmbientSource)
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
		B(graphicsdevice);
	}

	internal TerrainEffect(GraphicsDevice P_0, bool P_1)
		: base(P_0, "TerrainEffect", P_1)
	{
		B(P_0);
	}

	private void B(GraphicsDevice P_0)
	{
		HC_0012 = base.Parameters["_LightingTexture"];
		HCH = base.Parameters["_DiffuseColor_And_SpotAngleInv"];
		HC7 = base.Parameters["_Position_And_Radius"];
		HC_0001 = base.Parameters["_SpotDirection_And_SpotAngle"];
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
