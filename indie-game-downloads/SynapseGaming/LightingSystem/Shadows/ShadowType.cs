using SynapseGaming.LightingSystem.Editor;

namespace SynapseGaming.LightingSystem.Shadows;

/// <summary>
/// Determines the types of objects that cast shadows from a light source.
/// </summary>
public enum ShadowType
{
	/// <summary>
	/// Disables shadow casting from the light (highest performance).
	/// </summary>
	[EditorEnumDescription("No Shadows")]
	None,
	/// <summary>
	/// Limits shadow casting to objects that are static (provides optimal
	/// balance of quality and performance).
	/// </summary>
	[EditorEnumDescription("Static Objects")]
	StaticObjects,
	/// <summary>
	/// Allows shadows from all shadow casting objects.
	/// </summary>
	[EditorEnumDescription("All Objects")]
	AllObjects
}
