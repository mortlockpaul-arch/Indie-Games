using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Lights;

namespace SynapseGaming.LightingSystem.Effects.Forward;

/// <summary>
/// Effect class with complete support for, and binding of, FX Standard Annotations and Semantics (SAS).
/// </summary>
public class SasLightingEffect : SasEffect, ILightingEffect
{
	private int HCB = 1;

	private List<BaseLight> HC_0002 = new List<BaseLight>();

	/// <summary>
	/// Maximum number of light sources the effect supports.
	/// </summary>
	public int MaxLightSources => HCB;

	/// <summary>
	/// Light sources that apply lighting to the effect during rendering.
	/// </summary>
	public List<BaseLight> LightSources
	{
		set
		{
			HC_0002.Clear();
			foreach (BaseLight item in value)
			{
				HC_0002.Add(item);
			}
			SyncLightSourceEffectData();
			HCy.HC_0002.AccumulationValue++;
		}
	}

	/// <summary>
	/// Sets the max light count supported by the effect.
	/// </summary>
	/// <param name="maxlights"></param>
	protected virtual void SetMaxLightCount(int maxlights)
	{
		HCB = maxlights;
	}

	/// <summary>
	/// Finds the max light count supported by the effect's shader.
	/// </summary>
	protected virtual void FindMaxLightCount()
	{
		HCB = 0;
		int num = 0;
		int num2 = 0;
		int num3 = BaseSasBindEffect.SASAddress_AmbientLight_Color.Length;
		for (int i = 0; i < num3; i++)
		{
			if (base.SasAutoBindTable.Find(BaseSasBindEffect.SASAddress_DirectionalLight_Color[i]) != null)
			{
				num = i + 1;
			}
			if (base.SasAutoBindTable.Find(BaseSasBindEffect.SASAddress_PointLight_Color[i]) != null)
			{
				num2 = i + 1;
			}
		}
		if (num2 < 1)
		{
			HCB = num;
		}
		else if (num < 1)
		{
			HCB = num2;
		}
		else
		{
			HCB = Math.Min(num, num2);
		}
	}

	/// <summary>
	/// Applies the current lighting information to the bound effect parameters.
	/// </summary>
	protected virtual void SyncLightSourceEffectData()
	{
		if (HC_0002.Count < 1)
		{
			return;
		}
		bool flag = base.SasAutoBindTable.Find(BaseSasBindEffect.SASAddress_DirectionalLight_Color[0]) != null;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < HC_0002.Count; i++)
		{
			BaseLight baseLight = HC_0002[0];
			LightTypeCaster lightTypeCaster = OptimizationSystem.LightTypeCasters.Get(baseLight);
			if (lightTypeCaster.AmbientSource != null)
			{
				EffectHelper._0012v(base.SasAutoBindTable.Find(BaseSasBindEffect.SASAddress_AmbientLight_Color[num]), new Vector4(baseLight.CompositeColorAndIntensity, 1f));
				num++;
			}
			else if (lightTypeCaster.PointSource != null)
			{
				EffectHelper._0012v(base.SasAutoBindTable.Find(BaseSasBindEffect.SASAddress_PointLight_Color[num3]), new Vector4(baseLight.CompositeColorAndIntensity, 1f));
				EffectHelper._0012v(base.SasAutoBindTable.Find(BaseSasBindEffect.SASAddress_PointLight_Position[num3]), new Vector4(lightTypeCaster.PointSource.Position, 1f));
				EffectHelper._0012v(base.SasAutoBindTable.Find(BaseSasBindEffect.SASAddress_PointLight_Range[num3]), new Vector4(lightTypeCaster.PointSource.Radius));
				num3++;
			}
			else if (lightTypeCaster.DirectionalSource != null)
			{
				if (flag)
				{
					EffectHelper._0012v(base.SasAutoBindTable.Find(BaseSasBindEffect.SASAddress_DirectionalLight_Color[num2]), new Vector4(baseLight.CompositeColorAndIntensity, 1f));
					EffectHelper._0012v(base.SasAutoBindTable.Find(BaseSasBindEffect.SASAddress_DirectionalLight_Direction[num2]), new Vector4(lightTypeCaster.DirectionalSource.Direction, 1f));
					num2++;
				}
				else if (lightTypeCaster.ShadowSource != null)
				{
					EffectHelper._0012v(base.SasAutoBindTable.Find(BaseSasBindEffect.SASAddress_PointLight_Color[num3]), new Vector4(baseLight.CompositeColorAndIntensity, 1f));
					EffectHelper._0012v(base.SasAutoBindTable.Find(BaseSasBindEffect.SASAddress_PointLight_Position[num3]), new Vector4(lightTypeCaster.ShadowSource.ShadowPosition, 1f));
					EffectHelper._0012v(base.SasAutoBindTable.Find(BaseSasBindEffect.SASAddress_PointLight_Range[num3]), new Vector4(1E+09f));
					num3++;
				}
			}
		}
		int num4 = BaseSasBindEffect.SASAddress_AmbientLight_Color.Length;
		for (int j = num; j < num4; j++)
		{
			EffectHelper._0012v(base.SasAutoBindTable.Find(BaseSasBindEffect.SASAddress_AmbientLight_Color[j]), new Vector4(0f));
		}
		for (int k = num2; k < num4; k++)
		{
			EffectHelper._0012v(base.SasAutoBindTable.Find(BaseSasBindEffect.SASAddress_DirectionalLight_Color[k]), new Vector4(0f));
		}
		for (int l = num3; l < num4; l++)
		{
			EffectHelper._0012v(base.SasAutoBindTable.Find(BaseSasBindEffect.SASAddress_PointLight_Color[l]), new Vector4(0f));
		}
		EffectHelper._0012v(base.SasAutoBindTable.Find("Sas.NumAmbientLights"), new Vector4(num));
		EffectHelper._0012v(base.SasAutoBindTable.Find("Sas.NumDirectionalLights"), new Vector4(num2));
		EffectHelper._0012v(base.SasAutoBindTable.Find("Sas.NumPointLights"), new Vector4(num3));
	}

	internal SasLightingEffect(GraphicsDevice P_0, byte[] P_1, bool P_2)
		: base(P_0, P_1, P_2)
	{
		EffectByteCode = P_1;
		FindMaxLightCount();
	}

	/// <summary>
	/// Creates a new empty effect of the same class type and using the same effect file as this object.
	/// </summary>
	/// <returns></returns>
	protected override Effect Create()
	{
		return new SasLightingEffect(base.GraphicsDevice, EffectByteCode, true);
	}
}
