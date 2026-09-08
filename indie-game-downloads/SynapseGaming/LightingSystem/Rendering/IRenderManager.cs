using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Interface that provides access to the scene's render manager. The render manager
/// provides methods for controlling scene rendering.
/// </summary>
public interface IRenderManager : IRenderableManager, IManagerService, IManager, IUnloadable
{
	/// <summary>
	/// Renders the scene.
	/// </summary>
	void Render();
}
