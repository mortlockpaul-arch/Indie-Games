using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace BEPUphysics.Collidables.Events;

internal struct EventStorageInitialCollisionDetected
{
	internal CollidablePairHandler pair;

	internal Collidable other;

	internal EventStorageInitialCollisionDetected(Collidable other, CollidablePairHandler pair)
	{
		this.pair = pair;
		this.other = other;
	}
}
