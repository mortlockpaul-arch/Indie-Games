namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Interface used by objects that support Unloading. Unlike disposed objects, unloaded objects
/// can continue to be used and any required internal resources are recreated as needed.
/// </summary>
public interface IUnloadable
{
	/// <summary>
	/// Disposes any graphics resource used internally by this object, and removes
	/// scene resources managed by this object. Commonly used during Game.UnloadContent.
	/// </summary>
	void Unload();
}
