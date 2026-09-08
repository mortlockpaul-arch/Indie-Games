namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Determines the type of shader output deferred object rendering effects will generate.
/// </summary>
public enum DeferredEffectOutput
{
	/// <summary>
	/// Renders the z-fill optimization pass.
	/// </summary>
	Depth,
	/// <summary>
	/// Renders the g-buffer generation pass.
	/// </summary>
	GBuffer,
	/// <summary>
	/// Renders the shadow map depth.
	/// </summary>
	ShadowDepth,
	/// <summary>
	/// Renders the final image composition pass.
	/// </summary>
	Final
}
