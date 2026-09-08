using System.IO;

namespace Loot.Items.Armors;

public class ArmorPlateMail : Armor
{
	private const string armorName = "Plate Mail";

	protected override ArmorSprite ArmorSprite => Loot.Items.ArmorSprite.PlateMail;

	public ArmorPlateMail()
		: base("Plate Mail")
	{
		Stats.DEF = (int)Tier * 4 + 25;
	}

	public ArmorPlateMail(EquipmentTier tier)
		: base(tier, "Plate Mail")
	{
		Stats.DEF = (int)Tier * 4 + 25;
	}

	public ArmorPlateMail(BinaryReader reader)
		: base(reader, "Plate Mail")
	{
	}
}
