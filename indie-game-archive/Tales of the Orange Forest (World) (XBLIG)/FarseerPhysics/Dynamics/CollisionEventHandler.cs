using FarseerPhysics.Dynamics.Contacts;

namespace FarseerPhysics.Dynamics;

public delegate bool CollisionEventHandler(Fixture fixtureA, Fixture fixtureB, Contact manifold);
