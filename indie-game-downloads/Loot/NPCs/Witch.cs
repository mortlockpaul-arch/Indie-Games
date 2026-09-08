using System.IO;
using Loot.Dungeon;
using Loot.Items;
using Loot.Items.Scrolls;

namespace Loot.NPCs;

public class Witch : NPC
{
	public override string Name => "Witch";

	protected override NPCSprite.Info SpriteInfo => NPCSprite.Witch;

	public Witch(BinaryReader reader)
		: base(reader)
	{
	}

	public Witch(int level, Location loc)
		: base(level, loc)
	{
		MaxHP = base.Level + 15;
		HP = MaxHP;
		Stats.DMG = new DMGRange(10, 10);
		Stats.DEX = base.Level;
		Stats.DEF = base.Level;
		Stats.LCK = 0;
		AdjustDifficulty();
	}

	protected override Item RareLoot()
	{
		int num = DM.Random.Next(100);
		if (num < 33)
		{
			return new ScrollEnchantArmor();
		}
		return (num < 66) ? ((Scroll)new ScrollEnchantWeapon()) : ((Scroll)new ScrollProtect());
	}
}
