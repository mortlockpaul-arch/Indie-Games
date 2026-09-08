using System.IO;
using Loot.Dungeon;

namespace Loot.Items;

public abstract class Equipment : Item
{
	protected string name;

	protected EquipmentTier Tier;

	public Stats Stats;

	protected static readonly string[] tierNames = new string[5] { "", "Orcish ", "Dwarven ", "Elven ", "Epic " };

	public bool IsEpic => Tier == EquipmentTier.Epic;

	public override int Value
	{
		get
		{
			int num = 0;
			return base.Cursed ? num : (num + 50 * (Stats.DMG.Min + Stats.DMG.Max) / 2 + 50 * Stats.DEF + 50 * Stats.DEX + 50 * Stats.LCK);
		}
	}

	public Equipment(EquipmentTier tier, string name)
	{
		Tier = tier;
		if (Tier == EquipmentTier.Basic)
		{
			Identify();
		}
		this.name = TierName(tier) + name;
	}

	public Equipment(BinaryReader reader, string name)
		: base(reader)
	{
		this.name = TierName(Tier) + name;
	}

	public static string TierName(EquipmentTier tier)
	{
		return tierNames[(uint)tier];
	}

	public override void Curse()
	{
		base.Curse();
		Stats.DMG = DMGRange.Zero;
		Stats.DEF = 0;
		Stats.DEX = 0;
		Stats.LCK = 0;
	}

	public bool IsJunk()
	{
		if (!Identified)
		{
			return false;
		}
		Stats cmp = DM.Player.GetEquipment(GetType())?.Stats ?? Stats.Zero;
		for (int i = 0; i < 4; i++)
		{
			if (Stats.CompareTo(i, cmp) > 0)
			{
				return false;
			}
		}
		return true;
	}

	public override void onActivate(int id)
	{
		DM.Player.Equip(id);
	}

	protected static EquipmentTier RandomTier()
	{
		int num = DM.Random.Next(100);
		if (num < 1)
		{
			return EquipmentTier.Epic;
		}
		if (num < 5)
		{
			return EquipmentTier.Elven;
		}
		if (num < 15)
		{
			return EquipmentTier.Dwarven;
		}
		return (num < 35) ? EquipmentTier.Orcish : EquipmentTier.Basic;
	}

	protected void RandomStats()
	{
		for (int i = 0; (int)(byte)i <= (int)Tier; i++)
		{
			Stats.Modify(DM.Random.Next(4), 2);
		}
	}

	public override void Read(BinaryReader reader)
	{
		base.Read(reader);
		Tier = (EquipmentTier)reader.ReadByte();
		Stats.Read(reader);
	}

	public override void Write(BinaryWriter writer)
	{
		base.Write(writer);
		writer.Write((byte)Tier);
		Stats.Write(writer);
	}
}
