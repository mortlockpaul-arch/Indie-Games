using System.IO;

namespace Loot.Items.Armors;

public class ArmorChainMail : Armor
{
	private const string armorName = "Chain Mail";

	protected override ArmorSprite ArmorSprite => Loot.Items.ArmorSprite.ChainMail;

	public ArmorChainMail()
		: base("Chain Mail")
	{
		Stats.DEF = (int)Tier * 3 + 15;
	}

	public ArmorChainMail(EquipmentTier tier)
		: base(tier, "Chain Mail")
	{
		Stats.DEF = (int)Tier * 3 + 15;
	}

	public ArmorChainMail(BinaryReader reader)
		: base(reader, "Chain Mail")
	{
	}
}
