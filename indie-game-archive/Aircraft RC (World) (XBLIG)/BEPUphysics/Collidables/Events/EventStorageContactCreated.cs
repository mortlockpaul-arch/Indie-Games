using BEPUphysics.CollisionTests;
using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace BEPUphysics.Collidables.Events;

internal struct EventStorageContactCreated
{
	internal CollidablePairHandler pair;

	internal ContactData contactData;

	internal Collidable other;

	internal EventStorageContactCreated(Collidable other, CollidablePairHandler pair, ref ContactData contactData)
	{
		this.other = other;
		this.pair = pair;
		this.contactData = contactData;
	}
}
