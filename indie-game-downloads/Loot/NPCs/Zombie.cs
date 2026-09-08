using System.IO;
using Loot.Dungeon;
using Loot.Items;
using Loot.Items.Potions;

namespace Loot.NPCs;

public class Zombie : NPC
{
	public override string Name => "Zombie";

	protected override NPCSprite.Info SpriteInfo => NPCSprite.Zombie;

	public Zombie(BinaryReader reader)
		: base(reader)
	{
	}

	public Zombie(int level, Location loc)
		: base(level, loc)
	{
		MoveSpeed = 2.5f;
		MaxHP = base.Level * 2 + 30;
		HP = MaxHP;
		Stats.DMG = new DMGRange(base.Level * 2 + 25, base.Level * 2 + 25);
		Stats.DEX = base.Level;
		Stats.DEF = base.Level;
		Stats.LCK = 0;
		AdjustDifficulty();
	}

	protected override Item RareLoot()
	{
		return new PotionDirePoison();
	}
}
