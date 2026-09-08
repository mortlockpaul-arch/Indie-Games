using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace SynapseGaming.LightingSystem.Audio;

/// <summary>
/// Interface that provides access to the scene's audio manager. The audio manager
/// provides methods for storing and querying audio emitters.
/// </summary>
public interface IAudioManager : IUpdatableManager, IManagerService, IQuery<AudioSource>, ISubmit<AudioSource>, ISubmit<IScene>, IWorldRenderableManager, IRenderableManager, IManager, IUnloadable
{
	/// <summary>
	/// Removes all objects from the container. Commonly used while clearing the scene.
	/// </summary>
	new void Clear();
}
