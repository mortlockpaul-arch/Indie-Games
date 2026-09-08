using System;
using System.Collections.Generic;
using Loot.Dungeon;
using Loot.Items;
using Loot.TileSets;

namespace Loot.Maps;

public class MapCavern : Map
{
	private Location secretLocation;

	public MapCavern(int depth)
		: base(depth, TileSet.Cave, 26, 26)
	{
	}

	protected MapCavern(int depth, TileSet tileSet)
		: base(depth, tileSet, 26, 26)
	{
	}

	protected override void Generate()
	{
		base.Generate();
		generateCavern();
		cleanDiags();
		if (DM.Player.IsLucky())
		{
			addSecretChamber(secretLocation);
		}
		randomizeFloor();
		fixBorders();
		AddExits();
	}

	private void generateCavern()
	{
		int num = base.Rows / 4;
		Location center = new Location(base.Rows / 2, base.Cols / 2);
		List<Location> list = new List<Location>();
		tileCircle(center, num, GetTile(TileId.Floor));
		for (int i = 0; i < 4; i++)
		{
			num = base.Rows / 4;
			double num2 = DM.Random.NextDouble() * 2.0 * Math.PI;
			int r = center.Row + (int)((double)num * Math.Sin(num2));
			int c = center.Col + (int)((double)num * Math.Cos(num2));
			tileCircle(new Location(r, c), num, GetTile(TileId.Floor));
			list.Add(new Location(r, c));
		}
		for (int j = 0; j < list.Count; j++)
		{
			tileCircle(list[j], num / 2, GetTile(TileId.Wall));
		}
		secretLocation = list[DM.Random.Next(list.Count)];
	}

	private void addSecretChamber(Location loc)
	{
		Tile tile = GetTile(TileId.Floor);
		List<Location> list = new List<Location>();
		List<Location> list2 = new List<Location>();
		list.Add(loc);
		while (list.Count > 0)
		{
			Location location = list[0];
			list.RemoveAt(0);
			if ((double)loc.Distance(location) <= 3.0 && (IsWall(location.N) || list2.Contains(location.N)) && (IsWall(location.S) || list2.Contains(location.S)) && (IsWall(location.E) || list2.Contains(location.E)) && (IsWall(location.W) || list2.Contains(location.W)) && (IsWall(location.NE) || list2.Contains(location.NE)) && (IsWall(location.SE) || list2.Contains(location.SE)) && (IsWall(location.NW) || list2.Contains(location.NW)) && (IsWall(location.SW) || list2.Contains(location.SW)))
			{
				SetTile(location, tile);
				int value = (int)Math.Ceiling((double)(DM.Random.Next(DM.Player.Depth) + 1) / 2.0);
				SetItem(location, new Gold(value));
				list2.Add(location);
				if (!list2.Contains(location.N))
				{
					list.Add(location.N);
				}
				if (!list2.Contains(location.S))
				{
					list.Add(location.S);
				}
				if (!list2.Contains(location.E))
				{
					list.Add(location.E);
				}
				if (!list2.Contains(location.W))
				{
					list.Add(location.W);
				}
			}
		}
		int num = DM.Random.Next(4);
		for (int i = 0; i < 4; i++)
		{
			int num2;
			int num3;
			switch ((num + i) % 4)
			{
			case 0:
				num2 = -1;
				num3 = 0;
				break;
			case 1:
				num2 = 1;
				num3 = 0;
				break;
			case 2:
				num2 = 0;
				num3 = 1;
				break;
			default:
				num2 = 0;
				num3 = -1;
				break;
			}
			int num4 = 1;
			Location location2 = loc + new Location(num2 * num4, num3 * num4);
			while (!IsWall(location2))
			{
				num4++;
				location2 = loc + new Location(num2 * num4, num3 * num4);
			}
			if (IsFloor(location2 + new Location(num2, num3)))
			{
				addSecretDoor(location2);
				break;
			}
		}
	}

	private void addSecretDoor(Location loc)
	{
		if (cells[loc.Row + 1, loc.Col].Tile.IsFloor())
		{
			SetTile(loc, GetTile(TileId.SecretDoorNS));
		}
		else
		{
			SetTile(loc, GetTile(TileId.SecretDoorEW));
		}
	}

	private void tileCircle(Location center, int radius, Tile tile)
	{
		for (int i = center.Row - radius; i <= center.Row + radius; i++)
		{
			for (int j = center.Col - radius; j <= center.Col + radius; j++)
			{
				Location loc = new Location(i, j);
				if ((double)center.Distance(loc) < (double)radius)
				{
					SetTile(loc, tile);
				}
			}
		}
	}

	private void cleanDiags()
	{
		for (int i = 0; i < base.Rows - 1; i++)
		{
			for (int j = 0; j < base.Cols - 1; j++)
			{
				Location loc = new Location(i, j);
				if (IsWall(loc) && IsFloor(loc.E) && IsFloor(loc.S) && IsWall(loc.SE))
				{
					SetTile(loc, GetTile(TileId.Floor));
					SetTile(loc.SE, GetTile(TileId.Floor));
				}
				else if (IsFloor(loc) && IsWall(loc.E) && IsWall(loc.S) && IsFloor(loc.SE))
				{
					SetTile(loc.E, GetTile(TileId.Floor));
					SetTile(loc.S, GetTile(TileId.Floor));
				}
			}
		}
	}
}
