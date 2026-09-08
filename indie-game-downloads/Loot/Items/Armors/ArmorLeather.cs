using System.IO;

namespace Loot.Items.Armors;

public class ArmorLeather : Armor
{
	private const string armorName = "Leather Armor";

	protected override ArmorSprite ArmorSprite => Loot.Items.ArmorSprite.Leather;

	public ArmorLeather()
		: base("Leather Armor")
	{
		Stats.DEF = (int)Tier * 2 + 6;
	}

	public ArmorLeather(EquipmentTier tier)
		: base(tier, "Leather Armor")
	{
		Stats.DEF = (int)Tier * 2 + 6;
	}

	public ArmorLeather(BinaryReader reader)
		: base(reader, "Leather Armor")
	{
	}
}
