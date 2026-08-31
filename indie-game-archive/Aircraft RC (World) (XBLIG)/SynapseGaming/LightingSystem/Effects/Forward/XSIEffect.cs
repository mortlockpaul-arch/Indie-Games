using System;
using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Effects.Forward;

/// <summary>
/// Provides support for XSI shaders.
/// </summary>
public class XSIEffect : SasLightingEffect
{
	/// <summary>
	/// Sets the effect technique based on its current property values.
	/// </summary>
	protected override void SetTechnique()
	{
		HCy.HCB.AccumulationValue++;
		if (base.Skinned)
		{
			SetTechnique("Skinned");
		}
		else
		{
			SetTechnique("Static");
		}
	}

	/// <summary>
	/// Creates a new XSIEffect instance from an effect containing an XSI shader
	/// (often loaded through the content pipeline or from disk).
	/// </summary>
	/// <param name="graphicsdevice"></param>
	/// <param name="effectbytecode"></param>
	internal XSIEffect(GraphicsDevice P_0, byte[] P_1)
		: base(P_0, P_1, false)
	{
		BindBySasAddress(FindByName("AmbientColor"), BaseSasBindEffect.SASAddress_AmbientLight_Color[0]);
		base.SkinBonesEffectParameter = FindByName("Bones");
		int num = BaseSasBindEffect.SASAddress_PointLight_Position.Length;
		for (int i = 0; i < num; i++)
		{
			BindBySasAddress(FindBySasAddress("Sas.PointLights[" + i + "].Position"), BaseSasBindEffect.SASAddress_PointLight_Position[i]);
			BindBySasAddress(FindBySasAddress("Sas.PointLights[" + i + "].Color"), BaseSasBindEffect.SASAddress_PointLight_Color[i]);
		}
		FindMaxLightCount();
		SetMaxLightCount(Math.Min(base.MaxLightSources, 3));
	}
}
