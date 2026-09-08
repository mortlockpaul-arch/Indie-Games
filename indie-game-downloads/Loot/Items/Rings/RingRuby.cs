using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;

namespace Loot.Items.Rings;

public class RingRuby : Ring
{
	private const string ringName = "Ruby Ring";

	public override Sprite Sprite => RingSprite.Ruby;

	public RingRuby()
		: base("Ruby Ring")
	{
		modifyStats();
	}

	public RingRuby(EquipmentTier tier)
		: base(tier, "Ruby Ring")
	{
		modifyStats();
	}

	public RingRuby(BinaryReader reader)
		: base(reader, "Ruby Ring")
	{
	}

	private void modifyStats()
	{
		Stats.Modify(DM.Random.Next(4), 2);
		Stats.Modify(DM.Random.Next(4), 2);
	}
}
