using Microsoft.Xna.Framework.Audio;
using SynapseGaming.LightingSystem.Components;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Rendering;

namespace SynapseGaming.LightingSystem.Audio;

/// <summary>
/// Interface used by audio sources / emitters stored in and handled by the IAudioManager.
/// </summary>
public interface IAudioSource : ISceneEntity, IMovableObject, IWorldBoundingBoxObject, IComponentObject<ISceneEntity>, IEditorCreatedObject<ISceneEntity>, IEditorObject, INamedObject, IEditorRenderableObject
{
	/// <summary>
	/// Determines if the sound will repeat after completing.
	/// </summary>
	bool Loop { get; set; }

	/// <summary>
	/// Determines how loud the sound is.
	/// </summary>
	float Volume { get; set; }

	/// <summary>
	/// Maximum distance in world space of the source's influence.
	/// </summary>
	float Radius { get; set; }

	/// <summary>
	/// Determines how the sound changes in relationship to the viewer. Ambient sounds
	/// are heard equally from everywhere in the scene, whereas 3D sounds are relative
	/// to the viewer / listener.
	/// </summary>
	AudioType AudioType { get; set; }

	/// <summary>
	/// Determines if the sound is currently playing.
	/// </summary>
	AudioState AudioState { get; set; }

	/// <summary>
	/// The SoundEffect used by the emitter to play sounds. This is either
	/// the sound loaded by the SoundEffectAsset or the sound passed into the constructor
	/// depending on how the object was initialized.
	/// </summary>
	SoundEffect SoundEffect { get; }

	/// <summary>
	/// Provides direct access to the repository name, file name, and sound
	/// the object was created from. Only valid for serialized objects
	/// created via the SunBurn editor.
	/// </summary>
	SoundEffectAsset SoundEffectAsset { get; set; }

	/// <summary>
	/// Starts playing the contained sound from the beginning.
	/// </summary>
	void Play();

	/// <summary>
	/// Stops playing the contained sound.
	/// </summary>
	void Stop();
}
