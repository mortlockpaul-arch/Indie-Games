using _0003;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Effects;

namespace SynapseGaming.LightingSystem.Core;

/// <summary />
public class EffectTypeCaster : ITypeCaster<Effect>
{
	/// <summary />
	public IRenderableEffect RenderableEffect;

	/// <summary />
	public ILightingEffect LightingEffect;

	/// <summary />
	public ISkinnedEffect SkinnedEffect;

	/// <summary />
	public IStaticLightingEffect StaticLightingEffect;

	/// <summary />
	public ITransparentEffect TransparentEffect;

	/// <summary />
	public ITerrainEffect TerrainEffect;

	/// <summary />
	public IAddressableEffect AddressableEffect;

	internal _0003._0012 HCB;

	/// <summary />
	public IShadowGenerateEffect ShadowGenerateEffect;

	/// <summary />
	public BaseRenderableEffect BaseRenderableEffect;

	/// <summary />
	public BaseMaterialEffect BaseMaterialEffect;

	/// <summary />
	public BaseSasEffect BaseSasEffect;

	/// <summary />
	public Effect Effect;

	/// <summary />
	public IEffectFog EffectFog;

	/// <summary />
	public IEffectLights EffectLights;

	/// <summary />
	public IEffectMatrices EffectMatrices;

	/// <summary />
	public void Set(Effect effect)
	{
		RenderableEffect = effect as IRenderableEffect;
		LightingEffect = effect as ILightingEffect;
		SkinnedEffect = effect as ISkinnedEffect;
		StaticLightingEffect = effect as IStaticLightingEffect;
		TransparentEffect = effect as ITransparentEffect;
		TerrainEffect = effect as ITerrainEffect;
		AddressableEffect = effect as IAddressableEffect;
		HCB = effect as _0003._0012;
		ShadowGenerateEffect = effect as IShadowGenerateEffect;
		BaseRenderableEffect = effect as BaseRenderableEffect;
		BaseMaterialEffect = effect as BaseMaterialEffect;
		BaseSasEffect = effect as BaseSasEffect;
		Effect = effect;
		EffectFog = effect as IEffectFog;
		EffectLights = effect as IEffectLights;
		EffectMatrices = effect as IEffectMatrices;
	}
}
