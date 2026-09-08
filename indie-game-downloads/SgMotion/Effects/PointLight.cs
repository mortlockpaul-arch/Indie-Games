using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SgMotion.Effects;

public class PointLight : IPointLight, ILight
{
	private EffectParameter positionParam;

	private EffectParameter colorParam;

	public Vector3 Position
	{
		get
		{
			return positionParam.GetValueVector3();
		}
		set
		{
			positionParam.SetValue(value);
		}
	}

	public Vector3 Color
	{
		get
		{
			return colorParam.GetValueVector3();
		}
		set
		{
			colorParam.SetValue(value);
		}
	}

	internal PointLight(EffectParameter lightStuctParameter)
	{
		CacheEffectParams(lightStuctParameter);
	}

	private void CacheEffectParams(EffectParameter lightStuctParameter)
	{
		positionParam = lightStuctParameter.StructureMembers["position"];
		colorParam = lightStuctParameter.StructureMembers["color"];
	}
}
