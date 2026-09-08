using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;

namespace Loot.Items.Amulets;

public class AmuletLeather : Amulet
{
	private const string amuletName = "Leather Amulet";

	public override Sprite Sprite => AmuletSprite.Leather;

	public AmuletLeather()
		: base("Leather Amulet")
	{
		modifyStats();
	}

	public AmuletLeather(EquipmentTier tier)
		: base(tier, "Leather Amulet")
	{
		modifyStats();
	}

	public AmuletLeather(BinaryReader reader)
		: base(reader, "Leather Amulet")
	{
	}

	private void modifyStats()
	{
		Stats.Modify(DM.Random.Next(4), 1);
	}
}
