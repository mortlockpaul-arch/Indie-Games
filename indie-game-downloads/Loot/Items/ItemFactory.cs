using Loot.Dungeon;

namespace Loot.Items;

public class ItemFactory
{
	public static Item Random()
	{
		int num = 5 + DM.Player.Stats.LCK / 10;
		return (DM.Random.Next(100) <= num) ? ItemRegistry.Random(DM.Player.Depth) : null;
	}
}
