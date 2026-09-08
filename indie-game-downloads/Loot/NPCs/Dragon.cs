using System.IO;
using Loot.Dungeon;
using Loot.Items;
using Loot.Items.Junks;

namespace Loot.NPCs;

public class Dragon : NPC
{
	public override string Name => "Dragon";

	protected override NPCSprite.Info SpriteInfo => NPCSprite.Dragon;

	public override bool CanFly => true;

	public Dragon(BinaryReader reader)
		: base(reader)
	{
	}

	public Dragon(int level, Location loc)
		: base(level, loc)
	{
		MaxHP = base.Level * 4;
		HP = MaxHP;
		Stats.DMG = new DMGRange(base.Level * 4 + 15, base.Level * 4 + 15);
		Stats.DEX = base.Level;
		Stats.DEF = base.Level;
		Stats.LCK = 0;
		AdjustDifficulty();
	}

	protected override Item RareLoot()
	{
		int num = DM.Random.Next(100);
		if (num < 50)
		{
			return new Emerald();
		}
		return (num < 80) ? ((Junk)new Sapphire()) : ((Junk)new Ruby());
	}
}
