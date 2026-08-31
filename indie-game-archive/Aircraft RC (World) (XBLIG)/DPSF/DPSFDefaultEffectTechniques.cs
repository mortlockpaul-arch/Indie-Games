namespace DPSF;

/// <summary>
/// The Techniques provided by the DPSF Default Effect.
/// </summary>
public enum DPSFDefaultEffectTechniques
{
	/// <summary>
	/// The default technique used to display particles as sprites.
	/// </summary>
	Sprites,
	/// <summary>
	/// The default technique used to display particles as colored quads.
	/// </summary>
	Quads,
	/// <summary>
	/// The default technique used to display particles as textured quads.
	/// </summary>
	TexturedQuads,
	/// <summary>
	/// An experimental technique used to display particles as textured quads, doing the color blending using premultiplied colors.
	/// </summary>
	TexturedQuadsExperimental
}
