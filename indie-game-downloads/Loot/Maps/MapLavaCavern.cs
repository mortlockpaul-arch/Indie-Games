using Loot.Dungeon;
using Loot.TileSets;
using Loot.Widgets;

namespace Loot.Maps;

public class MapLavaCavern : MapCavern
{
	public MapLavaCavern(int depth)
		: base(depth, TileSet.Lava)
	{
	}

	protected override void Generate()
	{
		base.Generate();
		randomizeWalls();
		addLavaVents();
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

	private void addLavaVents()
	{
		for (int i = 0; i < 3; i++)
		{
			Location src = FindRandom(base.IsOpenFloor);
			int num = 4 + DM.Random.Next(5);
			for (int j = 0; j < num; j++)
			{
				Location location = FindNearest(src, base.IsOpenFloor);
				if (!(location == Location.Zero))
				{
					SetWidget(new WidgetLavaVent(location));
					src = location;
					src.Row += DM.Random.Next(3) - 1;
					src.Col += DM.Random.Next(3) - 1;
					continue;
				}
				break;
			}
		}
	}
}
