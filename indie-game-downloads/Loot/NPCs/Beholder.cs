using System.IO;
using Loot.Dungeon;
using Loot.Items;
using Loot.Items.Potions;

namespace Loot.NPCs;

public class Beholder : NPC
{
	public override string Name => "Beholder";

	protected override NPCSprite.Info SpriteInfo => NPCSprite.Beholder;

	public override bool CanFly => true;

	public Beholder(BinaryReader reader)
		: base(reader)
	{
	}

	public Beholder(int level, Location loc)
		: base(level, loc)
	{
		MaxHP = (int)((double)base.Level * 3.5);
		HP = MaxHP;
		Stats.DMG = new DMGRange(base.Level * 3 + 20, base.Level * 3 + 20);
		Stats.DEX = base.Level;
		Stats.DEF = base.Level;
		Stats.LCK = 0;
		AdjustDifficulty();
	}

	protected override Item RareLoot()
	{
		return new PotionLevitate();
	}
}
