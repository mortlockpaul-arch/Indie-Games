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
		if (fixtureA.CollisionGroup == fixtureB.CollisionGroup && fixtureA.CollisionGroup != 0)
		{
			return fixtureA.CollisionGroup > 0;
		}
		bool flag = (fixtureA.CollidesWith & fixtureB.CollisionCategories) != CollisionCategory.None && (fixtureA.CollisionCategories & fixtureB.CollidesWith) != 0;
		if (flag && (fixtureA.IsFixtureIgnored(fixtureB) || fixtureB.IsFixtureIgnored(fixtureA)))
		{
			return false;
		}
		return flag;
	}
}
