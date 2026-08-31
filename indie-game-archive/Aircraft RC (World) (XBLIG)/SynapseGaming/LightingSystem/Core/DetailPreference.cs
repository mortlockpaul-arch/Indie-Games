using System;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Provides enumerated values for applying user detail
/// and performance preferences.
/// </summary>
[Serializable]
public enum DetailPreference
{
	/// <summary>
	/// Highest detail and lowest performance setting.
	/// </summary>
	High,
	/// <summary>
	/// Medium detail and medium performance setting.
	/// </summary>
	Medium,
	/// <summary>
	/// Low detail and high performance setting.
	/// </summary>
	Low,
	/// <summary>
	/// Disable feature and highest performance setting.
	/// </summary>
	Off
}
