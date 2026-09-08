using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;

namespace Loot.Items.Rings;

public class RingEmerald : Ring
{
	private const string ringName = "Emerald Ring";

	public override Sprite Sprite => RingSprite.Emerald;

	public RingEmerald()
		: base("Emerald Ring")
	{
		modifyStats();
	}

	public RingEmerald(EquipmentTier tier)
		: base(tier, "Emerald Ring")
	{
		modifyStats();
	}

	public RingEmerald(BinaryReader reader)
		: base(reader, "Emerald Ring")
	{
	}

	private void modifyStats()
	{
		Stats.Modify(DM.Random.Next(4), 2);
		Stats.Modify(DM.Random.Next(4), 1);
	}
}
