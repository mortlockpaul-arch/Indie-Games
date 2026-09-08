namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Used to indicate the rendering type supported by custom model processors.
/// </summary>
public enum ProcessorRenderingType
{
	/// <summary>
	/// Does not support any SunBurn rendering.
	/// </summary>
	None,
	/// <summary>
	/// Supports forward rendering.
	/// </summary>
	Forward,
	/// <summary>
	/// Supports deferred rendering.
	/// </summary>
	Deferred
}
