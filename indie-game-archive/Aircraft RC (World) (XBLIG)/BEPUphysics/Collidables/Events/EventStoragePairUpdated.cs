using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace BEPUphysics.Collidables.Events;

internal struct EventStoragePairUpdated
{
	internal NarrowPhasePair pair;

	internal BroadPhaseEntry other;

	internal EventStoragePairUpdated(BroadPhaseEntry other, NarrowPhasePair pair)
	{
		this.other = other;
		this.pair = pair;
	}
}
