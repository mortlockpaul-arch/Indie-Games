using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;

namespace Loot.Items.Rings;

public class RingDiamond : Ring
{
	private const string ringName = "Diamond Ring";

	public override Sprite Sprite => RingSprite.Diamond;

	public RingDiamond()
		: base("Diamond Ring")
	{
		modifyStats();
	}

	public RingDiamond(EquipmentTier tier)
		: base(tier, "Diamond Ring")
	{
		modifyStats();
	}

	public RingDiamond(BinaryReader reader)
		: base(reader, "Diamond Ring")
	{
	}

	private void modifyStats()
	{
		Stats.Modify(DM.Random.Next(4), 2);
		Stats.Modify(DM.Random.Next(4), 2);
		Stats.Modify(DM.Random.Next(4), 1);
	}
}
