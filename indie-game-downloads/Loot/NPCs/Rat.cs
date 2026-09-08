using System.IO;
using Loot.Dungeon;

namespace Loot.NPCs;

public class Rat : NPC
{
	public override string Name => "Rat";

	protected override NPCSprite.Info SpriteInfo => NPCSprite.Rat;

	public Rat(BinaryReader reader)
		: base(reader)
	{
	}

	public Rat(int level, Location loc)
		: base(level, loc)
	{
		MaxHP = base.Level + 5;
		HP = MaxHP;
		Stats.DMG = new DMGRange(5, 5);
		Stats.DEX = base.Level;
		Stats.DEF = base.Level;
		Stats.LCK = 0;
		AdjustDifficulty();
	}
}
