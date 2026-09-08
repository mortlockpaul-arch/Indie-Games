namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Interface used by game objects that expose a string name.
/// </summary>
public interface INamedObject
{
	/// <summary>
	/// The object's current name.
	/// </summary>
	string Name { get; set; }
}
