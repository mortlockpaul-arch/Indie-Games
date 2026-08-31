namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Base interface for objects managing scene resources.
/// </summary>
public interface IManager : IUnloadable
{
	/// <summary>
	/// Scene interface the manager was created by, or assigned during construction.
	/// </summary>
	IManagerServiceProvider OwnerSceneInterface { get; }

	/// <summary>
	/// Use to apply user quality and performance preferences to the resources managed by this object.
	/// </summary>
	/// <param name="preferences"></param>
	void ApplyPreferences(ISystemPreferences preferences);

	/// <summary>
	/// Removes resources managed by this object. Commonly used while clearing the scene.
	/// </summary>
	void Clear();
}
