using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;

namespace Loot.Items.Amulets;

public class AmuletIron : Amulet
{
	private const string amuletName = "Iron Amulet";

	public override Sprite Sprite => AmuletSprite.Iron;

	public AmuletIron()
		: base("Iron Amulet")
	{
		modifyStats();
	}

	public AmuletIron(EquipmentTier tier)
		: base(tier, "Iron Amulet")
	{
		modifyStats();
	}

	public AmuletIron(BinaryReader reader)
		: base(reader, "Iron Amulet")
	{
	}

	private void modifyStats()
	{
		Stats.Modify(DM.Random.Next(4), 2);
		Stats.Modify(DM.Random.Next(4), 1);
	}
}
