using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace BEPUphysics.Collidables.Events;

internal struct EventStorageCollisionEnded
{
	internal CollidablePairHandler pair;

	internal Collidable other;

	internal EventStorageCollisionEnded(Collidable other, CollidablePairHandler pair)
	{
		this.other = other;
		this.pair = pair;
	}
}
