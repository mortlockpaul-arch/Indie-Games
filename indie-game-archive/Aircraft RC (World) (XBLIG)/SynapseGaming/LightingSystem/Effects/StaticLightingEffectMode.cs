namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Determines the static lighting used by effects that implement IStaticLightingEffect.
/// </summary>
public enum StaticLightingEffectMode
{
	/// <summary>
	/// No static lighting is used and ambient lighting is applied dynamically.
	/// </summary>
	Ambient,
	/// <summary>
	/// Static and ambient lighting is applied using composite (approximate) lighting.
	/// </summary>
	Composite,
	/// <summary>
	/// Static and ambient lighting is applied using baked-down (light mapped) lighting.
	/// </summary>
	BakedDown,
	/// <summary>
	/// Static and ambient lighting is applied using baked-down (light mapped) lighting. Additional lighting
	/// can be applied using composite (approximate) lighting.
	///
	/// This is often used to apply illumination from dynamic light sources in a single faster approximate
	/// pass than full dynamic lighting.
	/// </summary>
	BakedDownAndComposite
}
