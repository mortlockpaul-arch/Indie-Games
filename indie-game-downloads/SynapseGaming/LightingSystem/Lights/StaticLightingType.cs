using SynapseGaming.LightingSystem.Editor;

namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Determines if an object uses light mapping, approximate lighting, or no lighting
/// to receive illumination from BakedDown light sources.
/// </summary>
public enum StaticLightingType
{
	/// <summary>
	/// Object does not receive illumination from BakedDown light sources.
	/// </summary>
	[EditorEnumDescription("None")]
	None,
	/// <summary>
	/// Object receives illumination from an approximate combination of all BakedDown light sources.
	/// </summary>
	[EditorEnumDescription("Approximate")]
	Composite,
	/// <summary>
	/// Object receives illumination from BakedDown light sources using light maps (textures) generated in-editor.
	/// </summary>
	[EditorEnumDescription("Light Mapped")]
	BakedDown,
	/// <summary>
	/// Object does not receive illumination from BakedDown light sources and instead is applied with a user defined color.
	/// </summary>
	[EditorEnumDescription("Custom")]
	Custom
}
