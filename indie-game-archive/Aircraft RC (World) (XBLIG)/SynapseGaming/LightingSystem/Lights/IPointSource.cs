using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Interface that provides point light information.
/// </summary>
public interface IPointSource
{
	/// <summary>
	/// Position in world space of the light.
	/// </summary>
	Vector3 Position { get; set; }

	/// <summary>
	/// Maximum distance in world space of the light's influence.
	/// </summary>
	float Radius { get; set; }
}
