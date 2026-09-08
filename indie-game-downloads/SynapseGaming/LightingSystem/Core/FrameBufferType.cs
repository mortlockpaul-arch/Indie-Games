namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Provides a list of both deferred and auxiliary buffer used for rendering.
/// </summary>
public enum FrameBufferType
{
	/// <summary>
	/// Stores the scene's linear depth and specular power information during deferred rendering.
	/// </summary>
	DeferredDepthAndSpecularPower,
	/// <summary>
	/// Stores the scene's view-space normal map and specular amount information during deferred rendering.
	/// </summary>
	DeferredNormalViewSpaceAndSpecular,
	/// <summary>
	/// Stores the scene's accumulated lighting during deferred rendering.
	/// </summary>
	DeferredLightingDiffuse,
	/// <summary>
	/// Stores the scene's accumulated specular during deferred rendering.
	/// </summary>
	DeferredLightingSpecular,
	/// <summary>
	/// Auxiliary / temporary buffer used during post processing.
	/// </summary>
	PostProcessing1,
	/// <summary>
	/// Auxiliary / temporary buffer used during post processing.
	/// </summary>
	PostProcessing2
}
