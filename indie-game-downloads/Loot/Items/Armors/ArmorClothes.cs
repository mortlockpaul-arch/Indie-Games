using System.IO;

namespace Loot.Items.Armors;

public class ArmorClothes : Armor
{
	private const string armorName = "Clothes";

	protected override ArmorSprite ArmorSprite => Loot.Items.ArmorSprite.Clothes;

	public ArmorClothes()
		: base("Clothes")
	{
		Stats.DEF = (int)Tier * 2 + 2;
	}

	public ArmorClothes(EquipmentTier tier)
		: base(tier, "Clothes")
	{
		Stats.DEF = (int)Tier * 2 + 2;
	}

	public ArmorClothes(BinaryReader reader)
		: base(reader, "Clothes")
	{
	}
}
