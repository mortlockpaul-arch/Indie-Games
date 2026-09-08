using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Interface that provides directional lighting information.
/// </summary>
public interface IDirectionalSource
{
	/// <summary>
	/// Direction in world space of the light's influence.
	/// </summary>
	Vector3 Direction { get; set; }
}
