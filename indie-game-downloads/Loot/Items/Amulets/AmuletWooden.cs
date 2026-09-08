using System.IO;
using Eyehook.Framework;

namespace Loot.Items.Amulets;

public class AmuletWooden : Amulet
{
	private const string amuletName = "Wooden Amulet";

	public override Sprite Sprite => AmuletSprite.Wooden;

	public AmuletWooden()
		: base("Wooden Amulet")
	{
	}

	public AmuletWooden(EquipmentTier tier)
		: base(tier, "Wooden Amulet")
	{
	}

	public AmuletWooden(BinaryReader reader)
		: base(reader, "Wooden Amulet")
	{
	}
}
