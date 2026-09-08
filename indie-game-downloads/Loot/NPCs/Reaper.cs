using System.IO;
using Loot.Awardments;
using Loot.Dungeon;

namespace Loot.NPCs;

public class Reaper : NPC
{
	public override string Name => "Grim Reaper";

	public override int XP => 666;

	protected override NPCSprite.Info SpriteInfo => NPCSprite.Reaper;

	public override bool CanFly => true;

	public Reaper(BinaryReader reader)
		: base(reader)
	{
	}

	public Reaper(int level, Location loc)
		: base(level, loc)
	{
		MaxHP = 9999;
		HP = MaxHP;
		int num = DM.Player.MaxHP / 2;
		Stats.DMG = new DMGRange(num, num);
		Stats.DEX = 999;
		Stats.DEF = 999;
		Stats.LCK = 0;
	}

	public override void Attack(Character defender)
	{
		base.Attack(defender);
		Profile.Awardments.Unlock(Awardment.FearTheReaper);
	}
}
