using System;

namespace FarseerPhysics.Dynamics;

[Flags]
public enum BodyFlags
{
	None = 0,
	Island = 1,
	Awake = 2,
	AutoSleep = 4,
	Bullet = 8,
	FixedRotation = 0x10,
	Active = 0x20,
	Toi = 0x40,
	IgnoreGravity = 0x80
}
