namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Defines the transparency mode when rendering materials and effects.
/// </summary>
public enum TransparencyMode
{
	/// <summary>
	/// No transparency - the material is solid.
	/// </summary>
	None,
	/// <summary>
	/// Clipped transparency - the material is only rendered where its diffuse
	/// map alpha value is greater than the reference value.
	/// </summary>
	Clip,
	/// <summary>
	/// Blended transparency - the material is alpha blended with the scene.
	/// This creates glass, plastic, and similar effects
	/// </summary>
	Blend,
	/// <summary>
	/// Additive transparency - the material is added to the scene. This creates
	/// glow, illumination, and lighting effects.
	/// </summary>
	Additive
}
