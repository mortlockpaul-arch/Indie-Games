using BEPUphysics.BroadPhaseEntries;

namespace BEPUphysics.Collidables.Events;

internal struct EventStoragePairRemoved
{
	internal BroadPhaseEntry other;

	internal EventStoragePairRemoved(BroadPhaseEntry other)
	{
		this.other = other;
	}
}
