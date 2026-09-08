using System.Collections.Generic;
using Loot.Dungeon;
using Loot.TileSets;

namespace Loot.Maps;

public class MapRooms(int depth, TileSet tileSet) : Map(depth, tileSet, 30, 30)
{
	private struct EndPoint(Location loc, Direction dir)
	{
		public Location Loc = loc;

		public Direction Dir = dir;
	}

	protected struct Boundary(int minWidth, int maxWidth, int minHeight, int maxHeight)
	{
		public int MinWidth = minWidth;

		public int MaxWidth = maxWidth;

		public int MinHeight = minHeight;

		public int MaxHeight = maxHeight;

		public int generateWidth()
		{
			return DM.Random.Next(MaxWidth + 1 - MinWidth) + MinWidth;
		}

		public int generateHeight()
		{
			return DM.Random.Next(MaxHeight + 1 - MinHeight) + MinHeight;
		}
	}

	protected Boundary roomBoundary;

	protected Boundary vertHallBoundary;

	protected Boundary horHallBoundary;

	private List<EndPoint> endPoints = new List<EndPoint>();

	protected override void Generate()
	{
		base.Generate();
		roomBoundary = new Boundary(3, 7, 3, 7);
		vertHallBoundary = new Boundary(1, 1, 3, 5);
		horHallBoundary = new Boundary(3, 5, 1, 1);
		generateRooms();
		removeDeadEnds();
		switch (DM.Random.Next(3))
		{
		case 0:
			addSecretDoors(10);
			break;
		case 1:
			addSecretDoors(10);
			addDoors(50);
			break;
		case 2:
			addSecretDoors(10);
			addDoors(100);
			break;
		}
		randomizeFloor();
		fixBorders();
		AddExits();
	}

	protected void generateRooms()
	{
		EndPoint endPoint = new EndPoint(new Location(1 + base.Rows / 2, base.Cols / 2), Direction.North);
		generateRoom(endPoint, roomBoundary);
		SetTile(endPoint.Loc, GetTile(TileId.Wall));
		while (endPoints.Count > 0)
		{
			EndPoint endPoint2 = endPoints[0];
			if (DM.Random.Next(3) == 0)
			{
				generateRoom(endPoint2, roomBoundary);
			}
			else
			{
				switch (endPoint2.Dir)
				{
				case Direction.North:
					generateRoom(endPoint2, vertHallBoundary);
					break;
				case Direction.South:
					generateRoom(endPoint2, vertHallBoundary);
					break;
				case Direction.East:
					generateRoom(endPoint2, horHallBoundary);
					break;
				case Direction.West:
					generateRoom(endPoint2, horHallBoundary);
					break;
				}
			}
			endPoints.Remove(endPoint2);
		}
	}

	private void generateRoom(EndPoint endPoint, Boundary bounds)
	{
		int num = bounds.generateWidth();
		int num2 = bounds.generateHeight();
		int num3 = endPoint.Loc.Row;
		int num4 = endPoint.Loc.Col;
		int num5 = num2 / 2;
		int num6 = num / 2;
		switch (endPoint.Dir)
		{
		case Direction.North:
			num3 -= num2;
			num4 -= num6;
			break;
		case Direction.South:
			num3++;
			num4 -= num6;
			break;
		case Direction.East:
			num4++;
			num3 -= num5;
			break;
		case Direction.West:
			num4 -= num;
			num3 -= num5;
			break;
		}
		int num7 = num3 + num2;
		int num8 = num4 + num;
		if (num3 <= 1)
		{
			num3 = 2;
		}
		if (num7 >= base.Rows - 2)
		{
			num7 = base.Rows - 3;
		}
		if (num4 <= 1)
		{
			num4 = 2;
		}
		if (num8 >= base.Cols - 2)
		{
			num8 = base.Cols - 3;
		}
		if (num7 - num3 < bounds.MinHeight || num8 - num4 < bounds.MinWidth)
		{
			return;
		}
		int num9 = num7 - num3;
		int num10 = num8 - num4;
		for (int i = num3 - 1; i < num7 + 1; i++)
		{
			for (int j = num4 - 1; j < num8 + 1; j++)
			{
				if (!IsWall(new Location(i, j)))
				{
					return;
				}
			}
		}
		for (int k = num3; k < num7; k++)
		{
			for (int l = num4; l < num8; l++)
			{
				SetTile(new Location(k, l), GetTile(TileId.Floor));
			}
		}
		SetTile(endPoint.Loc, GetTile(TileId.Floor));
		int c = ((num10 <= 2) ? (num4 + DM.Random.Next(num10)) : (num4 + DM.Random.Next(num10 - 2) + 1));
		endPoints.Add(new EndPoint(new Location(num3 - 1, c), Direction.North));
		int c2 = ((num10 <= 2) ? (num4 + DM.Random.Next(num10)) : (num4 + DM.Random.Next(num10 - 2) + 1));
		endPoints.Add(new EndPoint(new Location(num7, c2), Direction.South));
		endPoints.Add(new EndPoint(new Location((num9 <= 2) ? (num3 + DM.Random.Next(num9)) : (num3 + DM.Random.Next(num9 - 2) + 1), num8), Direction.East));
		endPoints.Add(new EndPoint(new Location((num9 <= 2) ? (num3 + DM.Random.Next(num9)) : (num3 + DM.Random.Next(num9 - 2) + 1), num4 - 1), Direction.West));
	}

	protected void removeDeadEnds()
	{
		for (int i = 1; i < rows - 1; i++)
		{
			for (int j = 1; j < cols - 1; j++)
			{
				removeDeadEnd(new Location(i, j));
			}
		}
	}

	private void removeDeadEnd(Location loc)
	{
		while (isDeadEnd(loc))
		{
			SetTile(loc, GetTile(TileId.Wall));
			loc = ((!IsFloor(loc.N)) ? ((!IsFloor(loc.S)) ? ((!IsFloor(loc.E)) ? loc.W : loc.E) : loc.S) : loc.N);
		}
	}

	private bool isDeadEnd(Location loc)
	{
		if (!IsFloor(loc))
		{
			return false;
		}
		int num = 0;
		if (IsWall(loc.N))
		{
			num++;
		}
		if (IsWall(loc.S))
		{
			num++;
		}
		if (IsWall(loc.E))
		{
			num++;
		}
		if (IsWall(loc.W))
		{
			num++;
		}
		return num == 3;
	}

	protected void addDoors(int doorOdds)
	{
		for (int i = 1; i < rows - 1; i++)
		{
			for (int j = 1; j < cols - 1; j++)
			{
				Location location = new Location(i, j);
				if (IsFloor(location) && needsDoor(location) && DM.Random.Next(100) < doorOdds)
				{
					SetClosedDoor(location);
				}
			}
		}
	}

	protected void addSecretDoors(int doorOdds)
	{
		for (int i = 1; i < rows - 1; i++)
		{
			for (int j = 1; j < cols - 1; j++)
			{
				Location location = new Location(i, j);
				if (IsFloor(location) && needsDoor(location) && DM.Random.Next(100) < doorOdds)
				{
					SetSecretDoor(location);
				}
			}
		}
	}

	private bool needsDoor(Location loc)
	{
		if ((!IsWall(new Location(loc.Row, loc.Col - 1)) || !IsWall(new Location(loc.Row, loc.Col + 1))) && (!IsWall(new Location(loc.Row - 1, loc.Col)) || !IsWall(new Location(loc.Row + 1, loc.Col))))
		{
			return false;
		}
		int num = 0;
		for (int i = loc.Row - 1; i <= loc.Row + 1; i++)
		{
			for (int j = loc.Col - 1; j <= loc.Col + 1; j++)
			{
				if (IsFloor(new Location(i, j)))
				{
					num++;
				}
			}
		}
		return num >= 5;
	}

	private void SetClosedDoor(Location loc)
	{
		if (cells[loc.Row + 1, loc.Col].Tile.IsFloor())
		{
			SetTile(loc, GetTile(TileId.ClosedDoorNS));
		}
		else
		{
			SetTile(loc, GetTile(TileId.ClosedDoorEW));
		}
	}

	private void SetSecretDoor(Location loc)
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
}
