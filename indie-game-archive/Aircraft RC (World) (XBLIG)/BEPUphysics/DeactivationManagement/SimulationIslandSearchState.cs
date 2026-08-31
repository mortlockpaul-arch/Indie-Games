namespace BEPUphysics.DeactivationManagement;

/// <summary>
///  Defines the current state of a simulation island member in a split attempt.
/// </summary>
public enum SimulationIslandSearchState
{
	Unclaimed,
	OwnedByFirst,
	OwnedBySecond
}
