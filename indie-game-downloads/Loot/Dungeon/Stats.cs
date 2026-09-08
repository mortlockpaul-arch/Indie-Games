using System;
using System.IO;
using Eyehook.Framework;

namespace Loot.Dungeon;

public struct Stats(DMGRange dmgStat, int defStat, int dexStat, int lckStat) : BinaryRW
{
	private struct IntStat
	{
		public int Value;

		private string StringValue;

		public IntStat(int value)
		{
			if (value < 0)
			{
				value = 0;
			}
			Value = value;
			StringValue = value.ToString();
		}

		public override string ToString()
		{
			if (StringValue == null)
			{
				StringValue = Value.ToString();
			}
			return StringValue;
		}
	}

	public const int Length = 4;

	public static Stats Zero = new Stats(DMGRange.Zero, 0, 0, 0);

	public static readonly string[] Name = new string[4] { "DMG", "DEF", "DEX", "LCK" };

	public static readonly string[] LongName = new string[4] { "Damage", "Defense", "Dexterity", "Luck" };

	public static readonly string[] Desc = new string[4] { "Hit Harder", "Live Longer", "Fight Better", "Get Richer" };

	public DMGRange DMG = dmgStat;

	private IntStat def = new IntStat(defStat);

	private IntStat dex = new IntStat(dexStat);

	private IntStat lck = new IntStat(lckStat);

	public int DEF
	{
		get
		{
			return def.Value;
		}
		set
		{
			def = new IntStat(value);
		}
	}

	public int DEX
	{
		get
		{
			return dex.Value;
		}
		set
		{
			dex = new IntStat(value);
		}
	}

	public int LCK
	{
		get
		{
			return lck.Value;
		}
		set
		{
			lck = new IntStat(value);
		}
	}

	public void Modify(int stat, int value)
	{
		switch (stat)
		{
		case 0:
			DMG = new DMGRange(DMG.Min + value, DMG.Max + value);
			break;
		case 1:
			DEF += value;
			break;
		case 2:
			DEX += value;
			break;
		case 3:
			LCK += value;
			break;
		}
	}

	public int CompareTo(int stat, Stats cmp)
	{
		return stat switch
		{
			0 => DMG.CompareTo(cmp.DMG), 
			1 => DEF - cmp.DEF, 
			2 => DEX - cmp.DEX, 
			3 => LCK - cmp.LCK, 
			_ => throw new Exception("Unknown stat: " + stat), 
		};
	}

	public string ToString(int stat)
	{
		return stat switch
		{
			0 => DMG.ToString(), 
			1 => def.ToString(), 
			2 => dex.ToString(), 
			3 => lck.ToString(), 
			_ => throw new Exception("Unknown stat: " + stat), 
		};
	}

	public static Stats operator +(Stats a, Stats b)
	{
		Stats result = default(Stats);
		result.DMG = a.DMG + b.DMG;
		result.def = new IntStat(a.DEF + b.DEF);
		result.dex = new IntStat(a.DEX + b.DEX);
		result.lck = new IntStat(a.LCK + b.LCK);
		return result;
	}

	public void Read(BinaryReader reader)
	{
		DMG.Read(reader);
		DEF = reader.ReadInt32();
		DEX = reader.ReadInt32();
		LCK = reader.ReadInt32();
	}

	public void Write(BinaryWriter writer)
	{
		DMG.Write(writer);
		writer.Write(DEF);
		writer.Write(DEX);
		writer.Write(LCK);
	}
}
