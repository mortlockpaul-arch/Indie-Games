using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Interface used by game objects that implement moving while contained
/// within a manager / container object.
/// </summary>
public interface IMovableObject : IWorldBoundingBoxObject, INamedObject
{
	/// <summary>
	/// Unique id used to identify the object across multiple scene loads / reloads.
	/// </summary>
	int UniqueId { get; }

	/// <summary>
	/// Indicates the object bounding area spans the entire world and
	/// the object is always visible.
	/// </summary>
	bool InfiniteBounds { get; }

	/// <summary>
	/// Indicates the current move. This value increments each time the object
	/// is moved (when the World transform changes).
	/// </summary>
	int MoveId { get; }

	/// <summary>
	/// Determines if objects receive update events from the engine and are tracked
	/// by the scenegraph.
	///
	/// Automatic update events are necessary to be affected by gravity, for
	/// components, and for the scenegraph to track moving objects.  Objects without
	/// Automatic update events can still move, however the containing scenegraph
	/// (ObjectManager or LightManager) must be notified using Manager.Move(object).
	/// </summary>
	UpdateType UpdateType { get; set; }

	/// <summary>
	/// World space transform of the object.
	/// </summary>
	Matrix World { get; set; }

	/// <summary>
	/// Dictionary of all managers the object is currently contained in (submitted to).
	///
	/// Managers are accessible by their ManagerType and only one manager of a
	/// particular type can be contained in the dictionary at a time.
	/// </summary>
	TypeDictionary<IManagerService> ContainingManagers { get; }

	/// <summary>
	/// Event used to update the object at regular intervals. This and all
	/// events are only called on dynamic objects.
	/// </summary>
	event UpdateDelegate UpdateEvent;

	/// <summary>
	/// Event used to determine when the object is submitted to a manager.
	/// </summary>
	event SubmitRemoveManagerDelegate SubmittedToManagerEvent;

	/// <summary>
	/// Event used to determine when the object is removed from a manager.
	/// </summary>
	event SubmitRemoveManagerDelegate RemovedFromManagerEvent;

	/// <summary>
	/// Called when the object is submitted to a manager.
	/// </summary>
	/// <param name="manager"></param>
	void OnSubmittedToManager(IManagerService manager);

	/// <summary>
	/// Called when the object is removed from a manager.
	/// </summary>
	/// <param name="manager"></param>
	void OnRemovedFromManager(IManagerService manager);

	/// <summary>
	/// Updates the object using the provided game time.
	/// </summary>
	/// <param name="gametime"></param>
	void Update(GameTime gametime);
}
