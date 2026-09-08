using System.IO;

namespace Loot.Items.Armors;

public class ArmorRags : Armor
{
	private const string armorName = "Rags";

	protected override ArmorSprite ArmorSprite => Loot.Items.ArmorSprite.Rags;

	public ArmorRags()
		: base("Rags")
	{
		Stats.DEF = (int)Tier * 2 + 2;
	}

	public ArmorRags(EquipmentTier tier)
		: base(tier, "Rags")
	{
		Stats.DEF = (int)Tier * 2 + 2;
	}

	public ArmorRags(BinaryReader reader)
		: base(reader, "Rags")
	{
	}
}
