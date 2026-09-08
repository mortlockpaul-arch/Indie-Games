using Loot.TileSets;

namespace Loot.Maps;

public class MapTrapDungeon : MapTrap
{
	public MapTrapDungeon(int depth)
		: base(depth, TileSet.Dungeon)
	{
	}
}
