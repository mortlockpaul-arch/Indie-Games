using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;

namespace Loot.Items.Amulets;

public class AmuletRune : Amulet
{
	private const string amuletName = "Ancient Rune";

	public override Sprite Sprite => AmuletSprite.Rune;

	public AmuletRune()
		: base("Ancient Rune")
	{
		modifyStats();
	}

	public AmuletRune(EquipmentTier tier)
		: base(tier, "Ancient Rune")
	{
		modifyStats();
	}

	public AmuletRune(BinaryReader reader)
		: base(reader, "Ancient Rune")
	{
	}

	private void modifyStats()
	{
		Stats.Modify(DM.Random.Next(4), 2);
	}
}
