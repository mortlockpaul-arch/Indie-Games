using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;

namespace Loot.Items.Rings;

public class RingSignet : Ring
{
	private const string ringName = "Signet Ring";

	public override Sprite Sprite => RingSprite.Signet;

	public RingSignet()
		: base("Signet Ring")
	{
		modifyStats();
	}

	public RingSignet(EquipmentTier tier)
		: base(tier, "Signet Ring")
	{
		modifyStats();
	}

	public RingSignet(BinaryReader reader)
		: base(reader, "Signet Ring")
	{
	}

	private void modifyStats()
	{
		Stats.Modify(DM.Random.Next(4), 2);
		Stats.Modify(DM.Random.Next(4), 2);
		Stats.Modify(DM.Random.Next(4), 2);
	}
}
