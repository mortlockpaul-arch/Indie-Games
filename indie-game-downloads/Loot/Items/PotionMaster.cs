using System.IO;
using Eyehook.Framework;
using Loot.Items.Potions;

namespace Loot.Items;

public class PotionMaster : GenericMaster
{
	protected override Sprite[] MiscSprites => PotionSprite.Misc;

	public PotionMaster(BinaryReader reader)
		: base(reader)
	{
	}

	public PotionMaster()
	{
		Identify(typeof(PotionHealth));
		Identify(typeof(PotionOil));
		Identify(typeof(PotionDrinkMe));
	}
}
