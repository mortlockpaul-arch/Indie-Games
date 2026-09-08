using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;

namespace Loot.Items.Amulets;

public class AmuletSilver : Amulet
{
	private const string amuletName = "Silver Amulet";

	public override Sprite Sprite => AmuletSprite.Silver;

	public AmuletSilver()
		: base("Silver Amulet")
	{
		modifyStats();
	}

	public AmuletSilver(EquipmentTier tier)
		: base(tier, "Silver Amulet")
	{
		modifyStats();
	}

	public AmuletSilver(BinaryReader reader)
		: base(reader, "Silver Amulet")
	{
	}

	private void modifyStats()
	{
		Stats.Modify(DM.Random.Next(4), 2);
		Stats.Modify(DM.Random.Next(4), 2);
		Stats.Modify(DM.Random.Next(4), 1);
	}
}
