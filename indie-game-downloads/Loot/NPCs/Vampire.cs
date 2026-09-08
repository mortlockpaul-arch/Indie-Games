using System.IO;
using Loot.Dungeon;
using Loot.Items;
using Loot.Items.Potions;

namespace Loot.NPCs;

public class Vampire : NPC
{
	public override string Name => "Vampire";

	protected override NPCSprite.Info SpriteInfo => NPCSprite.Vampire;

	public override bool CanFly => true;

	public Vampire(BinaryReader reader)
		: base(reader)
	{
	}

	public Vampire(int level, Location loc)
		: base(level, loc)
	{
		MaxHP = base.Level * 3;
		HP = MaxHP;
		Stats.DMG = new DMGRange(base.Level * 3 + 10, base.Level * 3 + 10);
		Stats.DEX = base.Level;
		Stats.DEF = base.Level;
		Stats.LCK = 0;
		AdjustDifficulty();
	}

	protected override Item RareLoot()
	{
		return new PotionHealth();
	}
}
