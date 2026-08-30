using System;

namespace FarseerPhysics.Dynamics;

public sealed class DefaultContactFilter
{
	public DefaultContactFilter(World world)
	{
		ContactManager contactManager = world.ContactManager;
		contactManager.ContactFilter = (CollisionFilterDelegate)Delegate.Combine(contactManager.ContactFilter, new CollisionFilterDelegate(ShouldCollide));
	}

	private static bool ShouldCollide(Fixture fixtureA, Fixture fixtureB)
	{
		if (fixtureA.CollisionGroup == fixtureB.CollisionGroup && fixtureA.CollisionGroup != 0 && fixtureB.CollisionGroup != 0)
		{
			return false;
		}
		if (((fixtureA.CollisionCategories & fixtureB.CollidesWith) == 0) & ((fixtureB.CollisionCategories & fixtureA.CollidesWith) == 0))
		{
			return false;
		}
		if (fixtureA.IsFixtureIgnored(fixtureB) || fixtureB.IsFixtureIgnored(fixtureA))
		{
			return false;
		}
		return true;
	}
}
