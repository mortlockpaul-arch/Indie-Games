using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SgMotion.Effects;

public class Material : IMaterial
{
	private EffectParameter emissiveColorParam;

	private EffectParameter diffuseColorParam;

	private EffectParameter specularColorParam;

	private EffectParameter specularPowerParam;

	public Vector3 EmissiveColor
	{
		get
		{
			return emissiveColorParam.GetValueVector3();
		}
		set
		{
			emissiveColorParam.SetValue(value);
		}
	}

	public Vector3 DiffuseColor
	{
		get
		{
			return diffuseColorParam.GetValueVector3();
		}
		set
		{
			diffuseColorParam.SetValue(value);
		}
	}

	public Vector3 SpecularColor
	{
		get
		{
			return specularColorParam.GetValueVector3();
		}
		set
		{
			specularColorParam.SetValue(value);
		}
	}

	public float SpecularPower
	{
		get
		{
			return specularPowerParam.GetValueSingle();
		}
		set
		{
			specularPowerParam.SetValue(value);
		}
	}

	internal Material(EffectParameter materialStructParameter)
	{
		CacheEffectParams(materialStructParameter);
	}

	private void CacheEffectParams(EffectParameter materialStructParameter)
	{
		emissiveColorParam = materialStructParameter.StructureMembers["emissiveColor"];
		diffuseColorParam = materialStructParameter.StructureMembers["diffuseColor"];
		specularColorParam = materialStructParameter.StructureMembers["specularColor"];
		specularPowerParam = materialStructParameter.StructureMembers["specularPower"];
	}
}
