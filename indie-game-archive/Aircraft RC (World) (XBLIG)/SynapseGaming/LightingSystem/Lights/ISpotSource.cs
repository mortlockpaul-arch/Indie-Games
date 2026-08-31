namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Interface that provides spotlight lighting information.
/// </summary>
public interface ISpotSource : IPointSource, IDirectionalSource
{
	/// <summary>
	/// Angle in degrees of the light's influence.
	/// </summary>
	float Angle { get; set; }

	/// <summary>
	/// Intensity of the light's 3D light beam.
	/// </summary>
	float Volume { get; set; }
}
