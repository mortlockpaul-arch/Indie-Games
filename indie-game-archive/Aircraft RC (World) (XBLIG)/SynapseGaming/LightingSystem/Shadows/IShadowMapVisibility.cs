namespace SynapseGaming.LightingSystem.Shadows;

/// <summary>
/// Interface that provides support for determining shadow visibility and level-of-detail information.
/// </summary>
public interface IShadowMapVisibility
{
	/// <summary>
	/// Determines how far from the camera a directional shadow level-of-detail (lod) will
	/// stretch before transitioning to the next lod.
	///
	/// Index 0 controls the highest level of detail, 1 controls the next highest, and so on.
	///
	/// The range is normalized relative to the environment ShadowFadeEndDistance,
	/// for instance a value of 1.0 transitions at the ShadowFadeEndDistance
	/// whereas a value of 0.25 transitions at (ShadowFadeEndDistance * 0.25).
	/// </summary>
	float[] ShadowLODRangeHints { get; }

	/// <summary>
	/// Determines if a directional shadow level-of-detail (lod) is enabled and will have its
	/// shadow map filled with an image of the scene.  Disabling unneeded lods reduces
	/// the number of rendered objects and draw calls.
	///
	/// Index 0 controls the highest level of detail, 1 controls the next highest, and so on.
	///
	/// Unlike point light shadows, directional light shadows render all of their lods
	/// every frame.  Each lod represents a different area in front of the camera with
	/// the highest lod closest to the viewer.  For some games (such as top-down
	/// perspective games) only a single lod is necessary and the rest can be disabled.
	/// </summary>
	bool[] ShadowLODEnabled { get; }
}
