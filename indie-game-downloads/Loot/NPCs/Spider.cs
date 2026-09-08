using System.IO;
using Loot.Dungeon;

namespace Loot.NPCs;

public class Spider : NPC
{
	public override string Name => "Spider";

	protected override NPCSprite.Info SpriteInfo => NPCSprite.Spider;

	public Spider(BinaryReader reader)
		: base(reader)
	{
	}

	public Spider(int level, Location loc)
		: base(level, loc)
	{
		MaxHP = level + 10;
		HP = MaxHP;
		Stats.DMG = new DMGRange(7, 7);
		Stats.DEX = base.Level;
		Stats.DEF = base.Level;
		Stats.LCK = 0;
		AdjustDifficulty();
	}
}
