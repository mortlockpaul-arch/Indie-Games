using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;

namespace Loot.Items.Amulets;

public class AmuletBone : Amulet
{
	private const string amuletName = "Bone Necklace";

	public override Sprite Sprite => AmuletSprite.Bone;

	public AmuletBone()
		: base("Bone Necklace")
	{
		modifyStats();
	}

	public AmuletBone(EquipmentTier tier)
		: base(tier, "Bone Necklace")
	{
		modifyStats();
	}

	public AmuletBone(BinaryReader reader)
		: base(reader, "Bone Necklace")
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
