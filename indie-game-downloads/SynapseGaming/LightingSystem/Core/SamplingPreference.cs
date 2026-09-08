using System;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Provides enumerated values for applying user sampling
/// and performance preferences.
/// </summary>
[Serializable]
public enum SamplingPreference
{
	/// <summary>
	/// Provides classic filtering mode similar to old software rendered games.
	/// </summary>
	Point,
	/// <summary>
	/// Lowest sampling quality and highest performance setting.
	/// </summary>
	Bilinear,
	/// <summary>
	/// Medium sampling quality and medium performance setting.
	/// </summary>
	Trilinear,
	/// <summary>
	/// Highest sampling quality and lowest performance setting.
	/// </summary>
	Anisotropic
}
