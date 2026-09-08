using System.Collections.Generic;
using Loot.Dungeon;
using Loot.TileSets;

namespace Loot.Maps;

public class MapBasicMaze(int depth) : Map(depth, TileSet.Cave, 20, 20)
{
	private List<Location> ends = new List<Location>();

	protected override void Generate()
	{
		base.Generate();
		ends.Add(new Location(rows / 2, cols / 2));
		while (ends.Count > 0)
		{
			int index = DM.Random.Next(ends.Count);
			Location loc = ends[index];
			ends.RemoveAt(index);
			SetTile(loc, GetTile(TileId.Floor));
			if (loc.Row > 2 && loc.Row < rows - 2 && loc.Col > 2 && loc.Col < cols - 2)
			{
				if (isFree(loc.N) && isFree(loc.N.N) && isFree(loc.NW) && isFree(loc.NE) && isFree(loc.N.NW) && isFree(loc.N.NE))
				{
					ends.Add(loc.N);
				}
				if (isFree(loc.S) && isFree(loc.S.S) && isFree(loc.SW) && isFree(loc.SE) && isFree(loc.S.SW) && isFree(loc.S.SE))
				{
					ends.Add(loc.S);
				}
				if (isFree(loc.E) && isFree(loc.E.E) && isFree(loc.NE) && isFree(loc.SE) && isFree(loc.E.NE) && isFree(loc.E.SE))
				{
					ends.Add(loc.E);
				}
				if (isFree(loc.W) && isFree(loc.W.W) && isFree(loc.NW) && isFree(loc.SW) && isFree(loc.W.NW) && isFree(loc.W.SW))
				{
					ends.Add(loc.W);
				}
			}
		}
		randomizeFloor();
		fixBorders();
		AddExits();
	}

	private bool isFree(Location l)
	{
		return IsWall(l) && !ends.Contains(l);
	}
}
