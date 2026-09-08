using System.IO;
using Eyehook.Framework;

namespace Loot.Items.Rings;

public class RingGold : Ring
{
	private const string ringName = "Gold Ring";

	public override Sprite Sprite => RingSprite.Gold;

	public RingGold()
		: base("Gold Ring")
	{
	}

	public RingGold(EquipmentTier tier)
		: base(tier, "Gold Ring")
	{
	}

	public RingGold(BinaryReader reader)
		: base(reader, "Gold Ring")
	{
	}
}
