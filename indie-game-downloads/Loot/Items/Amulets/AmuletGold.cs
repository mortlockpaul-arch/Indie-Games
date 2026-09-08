using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;

namespace Loot.Items.Amulets;

public class AmuletGold : Amulet
{
	private const string amuletName = "Gold Amulet";

	public override Sprite Sprite => AmuletSprite.Gold;

	public AmuletGold()
		: base("Gold Amulet")
	{
		modifyStats();
	}

	public AmuletGold(EquipmentTier tier)
		: base(tier, "Gold Amulet")
	{
		modifyStats();
	}

	public AmuletGold(BinaryReader reader)
		: base(reader, "Gold Amulet")
	{
	}

	private void modifyStats()
	{
		Stats.Modify(DM.Random.Next(4), 2);
		Stats.Modify(DM.Random.Next(4), 2);
		Stats.Modify(DM.Random.Next(4), 2);
	}
}
