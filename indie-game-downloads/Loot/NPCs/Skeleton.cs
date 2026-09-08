using System.IO;
using Loot.Dungeon;
using Loot.Items;
using Loot.Items.Potions;

namespace Loot.NPCs;

public class Skeleton : NPC
{
	public override string Name => "Skeleton";

	protected override NPCSprite.Info SpriteInfo => NPCSprite.Skeleton;

	public Skeleton(BinaryReader reader)
		: base(reader)
	{
	}

	public Skeleton(int level, Location loc)
		: base(level, loc)
	{
		MaxHP = base.Level * 2 + 15;
		HP = MaxHP;
		Stats.DMG = new DMGRange(base.Level * 2 + 10, base.Level * 2 + 10);
		Stats.DEX = base.Level;
		Stats.DEF = base.Level;
		Stats.LCK = 0;
		AdjustDifficulty();
	}

	protected override Item RareLoot()
	{
		return new PotionDrinkMe();
	}
}
