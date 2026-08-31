using BEPUphysics.BroadPhaseEntries;

namespace BEPUphysics.BroadPhaseSystems.SortAndSweep;

internal class Grid2DEntry
{
	internal BroadPhaseEntry item;

	internal Int2 previousMin;

	internal Int2 previousMax;

	internal void Initialize(BroadPhaseEntry entry)
	{
		item = entry;
		Grid2DSortAndSweep.ComputeCell(ref entry.boundingBox.Min, out previousMin);
		Grid2DSortAndSweep.ComputeCell(ref entry.boundingBox.Max, out previousMax);
	}
}
