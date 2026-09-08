using System.IO;

namespace Loot.Items.Armors;

public class ArmorHarness : Armor
{
	private const string armorName = "Harness";

	protected override ArmorSprite ArmorSprite => Loot.Items.ArmorSprite.Harness;

	public ArmorHarness()
		: base("Harness")
	{
		Stats.DEF = (int)Tier * 2 + 2;
	}

	public ArmorHarness(EquipmentTier tier)
		: base(tier, "Harness")
	{
		Stats.DEF = (int)Tier * 2 + 2;
	}

	public ArmorHarness(BinaryReader reader)
		: base(reader, "Harness")
	{
	}
}
