using System.IO;
using Loot.Dungeon;

namespace Loot.NPCs;

public class Wolf : NPC
{
	public override string Name => "Wolf";

	protected override NPCSprite.Info SpriteInfo => NPCSprite.Wolf;

	public Wolf(BinaryReader reader)
		: base(reader)
	{
	}

	public Wolf(int level, Location loc)
		: base(level, loc)
	{
		MaxHP = base.Level + 15;
		HP = MaxHP;
		Stats.DMG = new DMGRange(base.Level + 5, base.Level + 5);
		Stats.DEX = base.Level;
		Stats.DEF = base.Level;
		Stats.LCK = 0;
		AdjustDifficulty();
	}
}
