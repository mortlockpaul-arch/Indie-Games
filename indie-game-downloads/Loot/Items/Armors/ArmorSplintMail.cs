using System.IO;

namespace Loot.Items.Armors;

public class ArmorSplintMail : Armor
{
	private const string armorName = "Splint Mail";

	protected override ArmorSprite ArmorSprite => Loot.Items.ArmorSprite.SplintMail;

	public ArmorSplintMail()
		: base("Splint Mail")
	{
		Stats.DEF = (int)Tier * 4 + 20;
	}

	public ArmorSplintMail(EquipmentTier tier)
		: base(tier, "Splint Mail")
	{
		Stats.DEF = (int)Tier * 4 + 20;
	}

	public ArmorSplintMail(BinaryReader reader)
		: base(reader, "Splint Mail")
	{
	}
}
