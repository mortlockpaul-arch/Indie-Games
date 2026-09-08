using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;

namespace Loot.Items.Rings;

public class RingSapphire : Ring
{
	private const string ringName = "Sapphire Ring";

	public override Sprite Sprite => RingSprite.Sapphire;

	public RingSapphire()
		: base("Sapphire Ring")
	{
		modifyStats();
	}

	public RingSapphire(EquipmentTier tier)
		: base(tier, "Sapphire Ring")
	{
		modifyStats();
	}

	public RingSapphire(BinaryReader reader)
		: base(reader, "Sapphire Ring")
	{
	}

	private void modifyStats()
	{
		Stats.Modify(DM.Random.Next(4), 2);
	}
}
