using Loot.Dungeon;
using Loot.TileSets;
using Loot.Widgets;

namespace Loot.Maps;

public class MapPoisonCavern : MapCavern
{
	public MapPoisonCavern(int depth)
		: base(depth, TileSet.Poison)
	{
	}

	protected override void Generate()
	{
		base.Generate();
		randomizeWalls();
		addPoisonVents();
		addMushrooms();
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

	private void addPoisonVents()
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
					SetWidget(new WidgetPoisonVent(location));
					src = location;
					src.Row += DM.Random.Next(3) - 1;
					src.Col += DM.Random.Next(3) - 1;
					continue;
				}
				break;
			}
		}
	}

	private void addMushrooms()
	{
		for (int i = 1; i < base.Rows - 1; i++)
		{
			for (int j = 1; j < base.Cols - 1; j++)
			{
				Location loc = new Location(i, j);
				if (IsFloor(loc) && GetWidget(loc) == null && (isVent(loc.N) || isVent(loc.S) || isVent(loc.E) || isVent(loc.W)))
				{
					switch (DM.Random.Next(8))
					{
					case 0:
						SetTile(loc, GetTile(TileId.Floor4));
						break;
					case 1:
						SetTile(loc, GetTile(TileId.Floor5));
						break;
					case 2:
						SetTile(loc, GetTile(TileId.Floor6));
						break;
					case 3:
						SetTile(loc, GetTile(TileId.Floor7));
						break;
					}
				}
			}
		}
	}

	private bool isVent(Location loc)
	{
		Widget widget = GetWidget(loc);
		return widget != null && widget is WidgetPoisonVent;
	}
}
