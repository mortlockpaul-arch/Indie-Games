using System.IO;
using Loot.Dungeon;
using Loot.Items;
using Loot.Items.Armors;

namespace Loot.NPCs;

public class Orc : NPC
{
	public override string Name => "Orc";

	protected override NPCSprite.Info SpriteInfo => NPCSprite.Orc;

	public Orc(BinaryReader reader)
		: base(reader)
	{
	}

	public Orc(int level, Location loc)
		: base(level, loc)
	{
		MaxHP = base.Level + 40;
		HP = MaxHP;
		Stats.DMG = new DMGRange(base.Level * 2 + 5, base.Level * 2 + 5);
		Stats.DEX = base.Level;
		Stats.DEF = base.Level;
		Stats.LCK = 0;
		AdjustDifficulty();
	}

	protected override Item RareLoot()
	{
		return new ArmorChainMail();
	}
}
