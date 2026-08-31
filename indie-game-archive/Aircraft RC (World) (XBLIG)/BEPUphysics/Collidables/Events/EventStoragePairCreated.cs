using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace BEPUphysics.Collidables.Events;

internal struct EventStoragePairCreated
{
	internal NarrowPhasePair pair;

	internal BroadPhaseEntry other;

	internal EventStoragePairCreated(BroadPhaseEntry other, NarrowPhasePair pair)
	{
		this.other = other;
		this.pair = pair;
	}
}
