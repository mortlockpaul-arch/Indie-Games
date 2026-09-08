using System.IO;

namespace Loot.Items.Armors;

public class ArmorStudded : Armor
{
	private const string armorName = "Studded Armor";

	protected override ArmorSprite ArmorSprite => Loot.Items.ArmorSprite.Studded;

	public ArmorStudded()
		: base("Studded Armor")
	{
		Stats.DEF = (int)Tier * 2 + 10;
	}

	public ArmorStudded(EquipmentTier tier)
		: base(tier, "Studded Armor")
	{
		Stats.DEF = (int)Tier * 2 + 10;
	}

	public ArmorStudded(BinaryReader reader)
		: base(reader, "Studded Armor")
	{
	}
}
