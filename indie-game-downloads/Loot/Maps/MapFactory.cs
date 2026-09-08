namespace Loot.Maps;

public static class MapFactory
{
	public static Map GetMap(int depth)
	{
		if (depth >= 50)
		{
			return new MapLast(depth);
		}
		return ((depth - 1) % 6) switch
		{
			0 => CaveMap(depth), 
			1 => PoisonMap(depth), 
			2 => WoodenMap(depth), 
			3 => MineMap(depth), 
			4 => LavaMap(depth), 
			_ => DungeonMap(depth), 
		};
	}

	private static Map CaveMap(int depth)
	{
		return new MapCavern(depth);
	}

	private static Map LavaMap(int depth)
	{
		return new MapLavaCavern(depth);
	}

	private static Map WoodenMap(int depth)
	{
		return new MapTrapWooden(depth);
	}

	private static Map MineMap(int depth)
	{
		return new MapMine(depth);
	}

	private static Map PoisonMap(int depth)
	{
		return new MapPoisonCavern(depth);
	}

	private static Map DungeonMap(int depth)
	{
		return new MapTrapDungeon(depth);
	}
}
