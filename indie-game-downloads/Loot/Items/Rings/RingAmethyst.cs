using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;

namespace Loot.Items.Rings;

public class RingAmethyst : Ring
{
	private const string ringName = "Amethyst Ring";

	public override Sprite Sprite => RingSprite.Amethyst;

	public RingAmethyst()
		: base("Amethyst Ring")
	{
		modifyStats();
	}

	public RingAmethyst(EquipmentTier tier)
		: base(tier, "Amethyst Ring")
	{
		modifyStats();
	}

	public RingAmethyst(BinaryReader reader)
		: base(reader, "Amethyst Ring")
	{
	}

	private void modifyStats()
	{
		Stats.Modify(DM.Random.Next(4), 1);
	}
}
