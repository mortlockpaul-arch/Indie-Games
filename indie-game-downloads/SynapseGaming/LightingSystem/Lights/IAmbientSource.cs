namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Interface that provides ambient lighting information.
/// </summary>
public interface IAmbientSource
{
	/// <summary>
	/// Increases the detail of normal mapped surfaces during the ambient lighting pass (deferred rendering only).
	/// </summary>
	float Depth { get; }
}
