using Microsoft.Xna.Framework.Audio;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Audio;

/// <summary>
/// Provides an asset wrapper for XNA SoundEffects containing the source repository name, file name, and direct access
/// to the loaded asset. When visible in the SunBurn editor properties of this type automatically support drag
/// and drop of repository sounds into the property.
///
/// The content repository provided must be loaded before creating an instance of
/// this class otherwise the asset will fail to load.
/// </summary>
public class SoundEffectAsset : ContentRepositoryAsset<SoundEffect>
{
	/// <summary>
	/// Provides an empty SoundEffectAsset which can be used to initialize properties of this type.
	/// </summary>
	public static readonly SoundEffectAsset Empty = new SoundEffectAsset();

	private SoundEffectAsset()
	{
	}

	/// <summary>
	/// Creates a new SoundEffectAsset instance and loads the provided sound.
	/// </summary>
	/// <param name="repositoryname">Name of the content repository, which contains the asset.</param>
	/// <param name="sourceassetfilepath">Relative path to the file the asset is loaded from.</param>
	public SoundEffectAsset(string repositoryname, string sourceassetfilepath)
		: base(repositoryname, sourceassetfilepath)
	{
	}
}
