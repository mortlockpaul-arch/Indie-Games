using SynapseGaming.LightingSystem.Lights;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Interface that provides custom effects with static lighting support.
/// </summary>
public interface IStaticLightingEffect
{
	/// <summary>
	/// Sets the static lighting applied to the effect during rendering.
	/// </summary>
	/// <param name="lightingmode">Determines the static lighting
	/// mode used by the effect.</param>
	/// <param name="lightmap">Light map texture containing static
	/// lighting. If the static lighting mode does not use light
	/// mapping this value can be null.</param>
	/// <param name="compositelighting">Composite lighting containing
	/// static lighting. Only used if the static lighting mode specifies
	/// composite lighting.</param>
	void SetStaticLighting(StaticLightingEffectMode lightingmode, LightMap lightmap, ref CompositeLighting compositelighting);

	/// <summary>
	/// Sets the static lighting applied to the effect during rendering.
	/// </summary>
	/// <param name="lightingmode">Determines the static lighting
	/// mode used by the effect.</param>
	/// <param name="lightmap">Light map texture containing static
	/// lighting. If the static lighting mode does not use light
	/// mapping this value can be null.</param>
	void SetStaticLighting(StaticLightingEffectMode lightingmode, LightMap lightmap);
}
