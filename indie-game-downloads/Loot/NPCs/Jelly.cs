using System.IO;
using Loot.Dungeon;

namespace Loot.NPCs;

public class Jelly : NPC
{
	public override string Name => "Jelly";

	protected override NPCSprite.Info SpriteInfo => NPCSprite.Jelly;

	public Jelly(BinaryReader reader)
		: base(reader)
	{
	}

	public Jelly(int level, Location loc)
		: base(level, loc)
	{
		MaxHP = (int)((double)base.Level * 2.5);
		HP = MaxHP;
		Stats.DMG = new DMGRange(base.Level * 2 + 10, base.Level * 2 + 10);
		Stats.DEX = base.Level;
		Stats.DEF = base.Level;
		Stats.LCK = 0;
		AdjustDifficulty();
	}
}
