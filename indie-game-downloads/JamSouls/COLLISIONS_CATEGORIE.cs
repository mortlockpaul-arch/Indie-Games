using FarseerPhysics.Dynamics;

namespace JamSouls;

public static class COLLISIONS_CATEGORIE
{
	public const CollisionCategory NONE = CollisionCategory.None;

	public const CollisionCategory ALL = CollisionCategory.All;

	public const CollisionCategory PLAYER = CollisionCategory.Cat1;

	public const CollisionCategory FLOOR = CollisionCategory.Cat2;

	public const CollisionCategory KILLING_FLOOR = CollisionCategory.Cat8;

	public const CollisionCategory REACHABLE_FLOOR = CollisionCategory.Cat3;

	public const CollisionCategory SOUL = CollisionCategory.Cat4;

	public const CollisionCategory BOUNDS = CollisionCategory.Cat5;

	public const CollisionCategory FLAG = CollisionCategory.Cat6;

	public const CollisionCategory BULLET = CollisionCategory.Cat7;

	public const CollisionCategory SPRING = CollisionCategory.Cat9;

	public const CollisionCategory BURNING_FLOOR = CollisionCategory.Cat10;

	public const CollisionCategory SLOW_FLOOR = CollisionCategory.Cat11;

	public const CollisionCategory BALL = CollisionCategory.Cat12;

	public const CollisionCategory GOAL = CollisionCategory.Cat13;
}
