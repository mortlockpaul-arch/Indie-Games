namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Determines if objects receive update events from the engine and are tracked
/// by the scenegraph.
///
/// Automatic update events are necessary to be affected by gravity, for
/// components, and for the scenegraph to track moving objects.  Objects without
/// Automatic update events can still move, however the containing scenegraph
/// (ObjectManager or LightManager) must be notified using Manager.Move(object).
/// </summary>
public enum UpdateType
{
	/// <summary>
	/// Object does not receive update events and is not tracked by the scenegraph.
	/// The object can still move, however the containing scenegraph
	/// (ObjectManager or LightManager) must be notified using Manager.Move(object).
	/// </summary>
	None,
	/// <summary>
	/// Object receives update events from the engine and is automatically tracked
	/// by the scenegraph allowing it to move simply by setting the World transform.
	///
	/// Automatic update events are necessary to be affected by gravity, for
	/// components, and for the scenegraph to track moving objects.
	/// </summary>
	Automatic
}
