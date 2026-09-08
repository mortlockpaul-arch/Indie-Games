using System.IO;

namespace Loot.Items.Armors;

public class ArmorCloak : Armor
{
	private const string armorName = "Cloak";

	protected override ArmorSprite ArmorSprite => Loot.Items.ArmorSprite.Cloak;

	public ArmorCloak()
		: base("Cloak")
	{
		Stats.DEF = (int)Tier * 2 + 2;
	}

	public ArmorCloak(EquipmentTier tier)
		: base(tier, "Cloak")
	{
		Stats.DEF = (int)Tier * 2 + 2;
	}

	public ArmorCloak(BinaryReader reader)
		: base(reader, "Cloak")
	{
	}
}
