using System;
using Loot.Dungeon;
using Loot.TileSets;
using Loot.Widgets;

namespace Loot.Maps;

public class MapLast : Map
{
	private const int trapOdds = 8;

	private Location center;

	public MapLast(int depth)
		: base(depth, TileSet.Poison, 35, 35)
	{
	}

	protected override void Generate()
	{
		center = new Location(rows / 2, cols / 2);
		base.Generate();
		createSpiral();
		randomizeFloor();
		fixBorders();
		SetExitUp(center);
		Location location = FindFarthest(center, base.MatchNorthWall);
		SetTile(location, GetTile(TileId.Floor));
		SetExitOut(location);
		addSpikeTraps();
		addPoisonVents();
	}

	private void createSpiral()
	{
		float num = 0f;
		Location loc = center;
		while (add2x2floor(loc))
		{
			num += (float)Math.PI / 32f;
			float num2 = (float)((double)num / 6.2831854820251465 * 5.0);
			loc = center + new Location((int)((0.0 - Math.Sin(num)) * (double)num2), (int)(Math.Cos(num) * (double)num2));
		}
	}

	private bool add2x2floor(Location loc)
	{
		Tile tile = GetTile(TileId.Floor);
		for (int i = loc.Row; i <= loc.Row + 1; i++)
		{
			for (int j = loc.Col; j <= loc.Col + 1; j++)
			{
				Location loc2 = new Location(i, j);
				if (loc2.Row <= 1 || loc2.Row >= rows - 2 || loc2.Col <= 1 || loc2.Col >= cols - 2)
				{
					return false;
				}
				SetTile(loc2, tile);
			}
		}
		return true;
	}

	private void addSpikeTraps()
	{
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				Location loc = new Location(i, j);
				if (IsOpenFloor(loc) && DM.Random.Next(100) < 8)
				{
					Widget widget = new WidgetTrapSpike(loc);
					SetTile(loc, GetTile(TileId.Floor));
					SetWidget(widget);
				}
			}
		}
	}

	private void addPoisonVents()
	{
		for (int i = 0; i < 6; i++)
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
}
