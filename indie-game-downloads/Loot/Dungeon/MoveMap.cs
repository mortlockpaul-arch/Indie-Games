using System.Collections.Generic;
using Loot.Maps;

namespace Loot.Dungeon;

public class MoveMap
{
	private struct MoveMapCell
	{
		public byte isSet;

		public byte dist;
	}

	private const byte nsewCost = 10;

	private const byte diagCost = 14;

	private readonly byte maxCost;

	private byte trueValue = 1;

	private List<Location> ends = new List<Location>();

	private MoveMapCell[,] moveMap;

	private Map map;

	public MoveMap(Map map, Location target)
	{
		this.map = map;
		maxCost = 200;
		moveMap = new MoveMapCell[map.Rows, map.Cols];
		initialize();
		Generate(target);
	}

	public MoveMap(Map map, Location target, byte maxCost)
	{
		this.map = map;
		this.maxCost = maxCost;
		moveMap = new MoveMapCell[map.Rows, map.Cols];
		initialize();
		Generate(target);
	}

	private void initialize()
	{
		for (int i = 0; i < map.Rows; i++)
		{
			for (int j = 0; j < map.Cols; j++)
			{
				moveMap[i, j].isSet = 0;
				moveMap[i, j].dist = byte.MaxValue;
			}
		}
	}

	public void Generate(Location target)
	{
		trueValue++;
		if (trueValue == 0)
		{
			trueValue = 1;
		}
		addEnd(target, 0);
		while (ends.Count > 0)
		{
			Location location = ends[0];
			ends.RemoveAt(0);
			byte b = (byte)(moveMap[location.Row, location.Col].dist + 10);
			if (b <= maxCost)
			{
				addEnd(location.N, b);
				addEnd(location.W, b);
				addEnd(location.E, b);
				addEnd(location.S, b);
				byte b2 = (byte)(moveMap[location.Row, location.Col].dist + 14);
				if (b2 <= maxCost)
				{
					addEnd(location.NW, b2);
					addEnd(location.NE, b2);
					addEnd(location.SW, b2);
					addEnd(location.SE, b2);
				}
			}
		}
	}

	private void addEnd(Location l, byte d)
	{
		if (map.IsFloor(l))
		{
			if (moveMap[l.Row, l.Col].isSet != trueValue)
			{
				moveMap[l.Row, l.Col].dist = d;
				moveMap[l.Row, l.Col].isSet = trueValue;
				ends.Add(l);
			}
			else if (moveMap[l.Row, l.Col].dist > d)
			{
				moveMap[l.Row, l.Col].dist = d;
			}
		}
	}

	public Location PathToTarget(Location src)
	{
		Location result = src;
		byte b = TargetDistance(src);
		for (int i = src.Row - 1; i <= src.Row + 1; i++)
		{
			for (int j = src.Col - 1; j <= src.Col + 1; j++)
			{
				Location location = new Location(i, j);
				if (map.GetNPC(location) == null)
				{
					byte b2 = TargetDistance(location);
					if (b2 < b)
					{
						result = location;
						b = b2;
					}
				}
			}
		}
		return result;
	}

	public byte TargetDistance(Location l)
	{
		return (moveMap[l.Row, l.Col].isSet != trueValue) ? byte.MaxValue : moveMap[l.Row, l.Col].dist;
	}
}
