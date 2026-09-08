namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Determines the rendering style to use when drawing the scene.
/// </summary>
public enum RenderingSystemType
{
	/// <summary>
	/// Draw the scene using separate rendering passes for each dynamic light and the base static lighting pass.
	/// </summary>
	Forward,
	/// <summary>
	/// Draw the scene using only two passes and calculate the lighting in screen space.
	/// </summary>
	Deferred
}
