using Loot.Dungeon;
using Loot.TileSets;
using Loot.Widgets;

namespace Loot.Maps;

public class MapMine : MapRooms
{
	public MapMine(int depth)
		: base(depth, TileSet.Mine)
	{
	}

	protected override void Generate()
	{
		base.Generate();
		randomizeWalls();
		if (DM.Random.Next(2) == 0)
		{
			addSpikeTraps(depth / 10 + 5);
		}
	}

	private void randomizeWalls()
	{
		for (int i = 1; i < base.Rows - 1; i++)
		{
			for (int j = 1; j < base.Cols - 1; j++)
			{
				if (DM.Random.Next(2) != 0)
				{
					Location loc = new Location(i, j);
					if (cells[i, j].Tile.Id == TileId.BorderN)
					{
						SetTile(loc, GetTile(TileId.BorderN2));
					}
					else if (cells[i, j].Tile.Id == TileId.BorderS)
					{
						SetTile(loc, GetTile(TileId.BorderS2));
					}
					else if (cells[i, j].Tile.Id == TileId.BorderE)
					{
						SetTile(loc, GetTile(TileId.BorderE2));
					}
					else if (cells[i, j].Tile.Id == TileId.BorderW)
					{
						SetTile(loc, GetTile(TileId.BorderW2));
					}
				}
			}
		}
	}

	private void addSpikeTraps(int trapOdds)
	{
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				Location loc = new Location(i, j);
				if (IsOpenFloor(loc) && DM.Random.Next(100) < trapOdds)
				{
					Widget widget = new WidgetTrapSpike(loc);
					SetTile(loc, GetTile(TileId.Floor));
					SetWidget(widget);
				}
			}
		}
		if (DM.Player.IsLucky())
		{
			SetWidget(new WidgetFountainXP(FindRandom(base.IsOpenFloor)));
		}
	}
}
