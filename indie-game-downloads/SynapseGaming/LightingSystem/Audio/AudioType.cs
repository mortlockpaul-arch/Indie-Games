namespace SynapseGaming.LightingSystem.Audio;

/// <summary>
/// Determines how the sound changes in relationship to the viewer. Ambient sounds
/// are heard equally from everywhere in the scene, whereas 3D sounds are relative
/// to the viewer / listener.
/// </summary>
public enum AudioType
{
	/// <summary>
	/// The sound is heard equally from everywhere in the scene.
	/// </summary>
	Ambient,
	/// <summary>
	/// The sound is 3D and relative to the viewer / listener.
	/// </summary>
	Point
}
