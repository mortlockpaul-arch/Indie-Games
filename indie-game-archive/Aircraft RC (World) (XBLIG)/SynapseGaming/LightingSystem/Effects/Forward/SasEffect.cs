using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Effects.Forward;

/// <summary>
/// Effect class with full non-lighting support for, and binding of, FX Standard Annotations and Semantics (SAS).
/// </summary>
public class SasEffect : BaseSasEffect
{
	internal SasEffect(GraphicsDevice P_0, byte[] P_1, bool P_2)
		: base(P_0, P_1, P_2)
	{
		EffectByteCode = P_1;
	}

	/// <summary>
	/// Creates a new empty effect of the same class type and using the same effect file as this object.
	/// </summary>
	/// <returns></returns>
	protected override Effect Create()
	{
		return new SasEffect(base.GraphicsDevice, EffectByteCode, true);
	}

	internal static SasEffect H_0001(GraphicsDevice P_0, byte[] P_1, bool P_2)
	{
		SasLightingEffect sasLightingEffect = new SasLightingEffect(P_0, P_1, P_2);
		if (sasLightingEffect.MaxLightSources > 0)
		{
			return sasLightingEffect;
		}
		sasLightingEffect.Dispose();
		return new SasEffect(P_0, P_1, P_2);
	}
}
