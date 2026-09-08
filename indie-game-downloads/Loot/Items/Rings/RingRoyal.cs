using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;

namespace Loot.Items.Rings;

public class RingRoyal : Ring
{
	private const string ringName = "Royal Ring";

	public override Sprite Sprite => RingSprite.Royal;

	public RingRoyal()
		: base("Royal Ring")
	{
		modifyStats();
	}

	public RingRoyal(EquipmentTier tier)
		: base(tier, "Royal Ring")
	{
		modifyStats();
	}

	public RingRoyal(BinaryReader reader)
		: base(reader, "Royal Ring")
	{
	}

	private void modifyStats()
	{
		Stats.Modify(DM.Random.Next(4), 2);
		Stats.Modify(DM.Random.Next(4), 2);
		Stats.Modify(DM.Random.Next(4), 2);
		Stats.Modify(DM.Random.Next(4), 2);
	}
}
