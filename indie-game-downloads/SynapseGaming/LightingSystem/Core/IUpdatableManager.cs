using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Interface used by objects managing resources that are updated based on real or game time.
/// </summary>
public interface IUpdatableManager : IManager, IUnloadable
{
	/// <summary>
	/// Updates the object and its contained resources.
	/// </summary>
	/// <param name="gameTime"></param>
	void Update(GameTime gameTime);
}
