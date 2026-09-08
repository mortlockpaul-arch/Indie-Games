using System;
using System.IO;
using Eyehook.Framework;

namespace Loot.Dungeon;

public struct DMGRange : BinaryRW
{
	public static DMGRange Zero = new DMGRange(0, 0);

	public int Min;

	public int Max;

	private string stringValue;

	public int Damage => DM.Random.Next(Max + 1 - Min) + Min;

	public DMGRange(int val1, int val2)
	{
		if (val1 <= val2)
		{
			Min = val1;
			Max = val2;
		}
		else
		{
			Min = val2;
			Max = val1;
		}
		if (Min < 0)
		{
			Min = 0;
		}
		if (Max < 0)
		{
			Max = 0;
		}
		stringValue = "";
		setStringValue();
	}

	public int CompareTo(DMGRange a)
	{
		return (int)Math.Round((double)((Min + Max - (a.Min + a.Max)) / 2));
	}

	public static DMGRange operator +(DMGRange a, DMGRange b)
	{
		return new DMGRange(a.Min + b.Min, a.Max + b.Max);
	}

	public static DMGRange operator -(DMGRange a, DMGRange b)
	{
		return new DMGRange(a.Min - b.Min, a.Max - b.Max);
	}

	public static bool operator ==(DMGRange a, DMGRange b)
	{
		return a.Min == b.Min && a.Max == b.Max;
	}

	public static bool operator !=(DMGRange a, DMGRange b)
	{
		return a.Min != b.Min || a.Max != b.Max;
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return Min + Max;
	}

	private void setStringValue()
	{
		if (Max > 99)
		{
			stringValue = (Min + Max) / 2 + "+";
		}
		else
		{
			stringValue = Min + "-" + Max;
		}
	}

	public override string ToString()
	{
		if (stringValue == null)
		{
			setStringValue();
		}
		return stringValue;
	}

	public void Read(BinaryReader reader)
	{
		Min = reader.ReadInt32();
		Max = reader.ReadInt32();
		setStringValue();
	}

	public void Write(BinaryWriter writer)
	{
		writer.Write(Min);
		writer.Write(Max);
	}
}
