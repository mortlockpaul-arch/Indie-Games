using System;
using Loot.Dungeon;
using Loot.TileSets;
using Loot.Widgets;

namespace Loot.Maps;

public class MapTrap : MapRooms
{
	private int swordOdds = 50;

	private int trapOdds = 5;

	protected MapTrap(int depth, TileSet ts)
		: base(depth, ts)
	{
	}

	protected override void Generate()
	{
		base.Generate();
		if (depth < 9)
		{
			return;
		}
		if (depth < 30)
		{
			if (DM.Random.Next(2) == 0)
			{
				addSpikeTraps();
			}
			else
			{
				addWallSwords();
			}
			if (DM.Player.IsLucky())
			{
				SetWidget(new WidgetFountainHP(FindRandom(base.IsOpenFloor)));
			}
		}
		else
		{
			swordOdds = depth;
			trapOdds = depth / 6;
			addWallSwords();
			addSpikeTraps();
			SetWidget(new WidgetFountainHP(FindRandom(base.IsOpenFloor)));
		}
	}

	private void addWallSwords()
	{
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				Location loc = new Location(i, j);
				if (!IsOpenFloor(loc) || DM.Random.Next(100) >= swordOdds)
				{
					continue;
				}
				TimeSpan offset = TimeSpan.FromMilliseconds((double)DM.Random.Next(1000));
				if (IsWall(loc.N) && IsWall(loc.S))
				{
					SetWidget(new WidgetWallSword(loc, (loc.Col % 2 == 0) ? ((float)Math.PI / 2f) : (-(float)Math.PI / 2f), offset));
					if (loc.Col % 2 == 0)
					{
						SetWidget(new WidgetWallGear(loc.N));
					}
					else
					{
						SetWidget(new WidgetWallGear(loc.S));
					}
				}
				else if (IsWall(loc.E) && IsWall(loc.W))
				{
					SetWidget(new WidgetWallSword(loc, (loc.Row % 2 == 0) ? ((float)Math.PI) : 0f, offset));
					if (loc.Row % 2 == 0)
					{
						SetWidget(new WidgetWallGear(loc.E));
					}
					else
					{
						SetWidget(new WidgetWallGear(loc.W));
					}
				}
			}
		}
	}

	private void addSpikeTraps()
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
	}
}
