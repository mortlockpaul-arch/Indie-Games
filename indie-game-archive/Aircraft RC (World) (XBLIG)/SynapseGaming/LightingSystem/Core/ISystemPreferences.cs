namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Interface that provides a base for lighting system user preferences.
/// </summary>
public interface ISystemPreferences
{
	/// <summary>
	/// Sets the user preferred balance of texture sampling quality and performance.
	/// </summary>
	SamplingPreference TextureSampling { get; }

	/// <summary>
	/// Sets the maximum anisotropy level when TextureSampling is set to Anisotropic.
	/// </summary>
	int MaxAnisotropy { get; }

	/// <summary>
	/// Sets the user preferred balance of shadow filtering quality and performance.
	/// </summary>
	DetailPreference ShadowDetail { get; }

	/// <summary>
	/// Sets the user preferred balance of shadow resolution and performance.
	/// </summary>
	float ShadowQuality { get; }

	/// <summary>
	/// Sets the user preferred balance of LightingEffect detail and performance.
	/// </summary>
	DetailPreference EffectDetail { get; }

	/// <summary>
	/// Sets the user preferred balance of lighting detail and performance.
	/// </summary>
	DetailPreference LightingDetail { get; }

	/// <summary>
	/// Sets the user preferred balance of post-processing effect detail and performance.
	/// </summary>
	DetailPreference PostProcessingDetail { get; }
}
