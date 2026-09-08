using Loot.TileSets;

namespace Loot.Maps;

public class MapTrapWooden : MapTrap
{
	public MapTrapWooden(int depth)
		: base(depth, TileSet.Wooden)
	{
	}
}
