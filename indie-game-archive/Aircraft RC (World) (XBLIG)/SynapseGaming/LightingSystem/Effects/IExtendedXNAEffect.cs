using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Interface that provides basic material properties to XNA built-in effects.
/// </summary>
public interface IExtendedXNAEffect : ICollisionMaterial
{
	/// <summary>
	/// Surfaces rendered with the effect should be visible from both sides.
	/// </summary>
	bool DoubleSided { get; set; }

	/// <summary>
	/// The transparency style used when rendering the effect.
	/// </summary>
	TransparencyMode TransparencyMode { get; set; }
}
