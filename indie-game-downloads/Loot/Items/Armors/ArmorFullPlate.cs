using System.IO;

namespace Loot.Items.Armors;

public class ArmorFullPlate : Armor
{
	private const string armorName = "Full Plate";

	protected override ArmorSprite ArmorSprite => Loot.Items.ArmorSprite.FullPlate;

	public ArmorFullPlate()
		: base("Full Plate")
	{
		Stats.DEF = (int)Tier * 5 + 30;
	}

	public ArmorFullPlate(EquipmentTier tier)
		: base(tier, "Full Plate")
	{
		Stats.DEF = (int)Tier * 5 + 30;
	}

	public ArmorFullPlate(BinaryReader reader)
		: base(reader, "Full Plate")
	{
	}
}
