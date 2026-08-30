using System;

namespace FarseerPhysics.Dynamics;

[Flags]
public enum WorldFlags
{
	NewFixture = 1,
	Locked = 2,
	ClearForces = 4
}
