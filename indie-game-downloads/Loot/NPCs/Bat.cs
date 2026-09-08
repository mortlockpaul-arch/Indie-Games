using System.IO;
using Loot.Dungeon;
using Loot.Items;
using Loot.Items.Potions;

namespace Loot.NPCs;

public class Bat : NPC
{
	public override string Name => "Bat";

	protected override NPCSprite.Info SpriteInfo => NPCSprite.Bat;

	public override bool CanFly => true;

	public Bat(BinaryReader reader)
		: base(reader)
	{
	}

	public Bat(int level, Location loc)
		: base(level, loc)
	{
		MaxHP = base.Level + 10;
		HP = MaxHP;
		Stats.DMG = new DMGRange(8, 8);
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
