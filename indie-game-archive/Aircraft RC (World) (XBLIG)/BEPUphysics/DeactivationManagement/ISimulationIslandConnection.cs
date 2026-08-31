using BEPUphysics.DataStructures;

namespace BEPUphysics.DeactivationManagement;

/// <summary>
///  Defines an object which connects simulation islands together.
/// </summary>
public interface ISimulationIslandConnection
{
	/// <summary>
	///  Gets or sets the deactivation member that owns this connection.
	/// </summary>
	DeactivationManager DeactivationManager { get; set; }

	/// <summary>
	///  Gets the simulation island members associated with this connection.
	/// </summary>
	ReadOnlyList<SimulationIslandMember> ConnectedMembers { get; }

	/// <summary>
	///  Adds references to the connection to all connected members.
	/// </summary>
	void AddReferencesToConnectedMembers();

	/// <summary>
	///  Removes references to the connection from all connected members.
	/// </summary>
	void RemoveReferencesFromConnectedMembers();
}
