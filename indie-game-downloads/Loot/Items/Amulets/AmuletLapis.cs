using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;

namespace Loot.Items.Amulets;

public class AmuletLapis : Amulet
{
	private const string amuletName = "Lapis Amulet";

	public override Sprite Sprite => AmuletSprite.Lapis;

	public AmuletLapis()
		: base("Lapis Amulet")
	{
		modifyStats();
	}

	public AmuletLapis(EquipmentTier tier)
		: base(tier, "Lapis Amulet")
	{
		modifyStats();
	}

	public AmuletLapis(BinaryReader reader)
		: base(reader, "Lapis Amulet")
	{
	}

	private void modifyStats()
	{
		Stats.Modify(DM.Random.Next(4), 2);
		Stats.Modify(DM.Random.Next(4), 2);
	}
}
