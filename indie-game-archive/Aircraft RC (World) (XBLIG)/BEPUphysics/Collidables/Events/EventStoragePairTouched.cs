using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace BEPUphysics.Collidables.Events;

internal struct EventStoragePairTouched
{
	internal CollidablePairHandler pair;

	internal Collidable other;

	internal EventStoragePairTouched(Collidable other, CollidablePairHandler pair)
	{
		this.other = other;
		this.pair = pair;
	}
}
