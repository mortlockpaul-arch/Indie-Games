using System.IO;
using Loot.Dungeon;
using Loot.Items;
using Loot.Items.Weapons;

namespace Loot.NPCs;

public class Goblin : NPC
{
	public override string Name => "Goblin";

	protected override NPCSprite.Info SpriteInfo => NPCSprite.Goblin;

	public Goblin(BinaryReader reader)
		: base(reader)
	{
	}

	public Goblin(int level, Location loc)
		: base(level, loc)
	{
		MaxHP = base.Level + 20;
		HP = MaxHP;
		Stats.DMG = new DMGRange(base.Level * 2, base.Level * 2);
		Stats.DEX = base.Level;
		Stats.DEF = base.Level;
		Stats.LCK = 0;
		AdjustDifficulty();
	}

	protected override Item RareLoot()
	{
		return new WeaponShortsword();
	}
}
