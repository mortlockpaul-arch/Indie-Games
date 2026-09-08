using System;
using System.IO;
using Eyehook.Framework;
using Microsoft.Xna.Framework;

namespace Loot.Dungeon;

public struct Location(int r, int c) : BinaryRW
{
	public static Location Zero = new Location(0, 0);

	public int Row = r;

	public int Col = c;

	public Location N => new Location(Row - 1, Col);

	public Location NE => new Location(Row - 1, Col + 1);

	public Location E => new Location(Row, Col + 1);

	public Location SE => new Location(Row + 1, Col + 1);

	public Location S => new Location(Row + 1, Col);

	public Location SW => new Location(Row + 1, Col - 1);

	public Location W => new Location(Row, Col - 1);

	public Location NW => new Location(Row - 1, Col - 1);

	public override string ToString()
	{
		return "[" + Row + "," + Col + "]";
	}

	public static bool operator ==(Location a, Location b)
	{
		return a.Row == b.Row && a.Col == b.Col;
	}

	public static bool operator !=(Location a, Location b)
	{
		return a.Row != b.Row || a.Col != b.Col;
	}

	public override bool Equals(object obj)
	{
		Location location = (Location)obj;
		return location.Row == Row && location.Col == Col;
	}

	public override int GetHashCode()
	{
		return Row + Col;
	}

	public static Location operator +(Location a, Location b)
	{
		return new Location(a.Row + b.Row, a.Col + b.Col);
	}

	public static Location operator -(Location a, Location b)
	{
		return new Location(a.Row - b.Row, a.Col - b.Col);
	}

	public float Distance(Location loc)
	{
		int num = Row - loc.Row;
		int num2 = Col - loc.Col;
		return (float)Math.Sqrt(num * num + num2 * num2);
	}

	public Vector2 ViewOffset(Location loc)
	{
		return new Vector2((Col - loc.Col) * 64, (Row - loc.Row) * 64);
	}

	public float AngleTo(Location loc)
	{
		return (float)Math.Atan2(loc.Row - Row, loc.Col - Col);
	}

	public void Read(BinaryReader reader)
	{
		Row = reader.ReadInt32();
		Col = reader.ReadInt32();
	}

	public void Write(BinaryWriter writer)
	{
		writer.Write(Row);
		writer.Write(Col);
	}
}
