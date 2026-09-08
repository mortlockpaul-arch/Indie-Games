using System.IO;
using Loot.Dungeon;
using Loot.PC;
using Loot.Screens;

namespace Loot.Items.Potions;

public class PotionSkill : Potion
{
	protected override int RealValue => 200;

	public PotionSkill()
		: base("Liquid Experience")
	{
	}

	public PotionSkill(BinaryReader reader)
		: base(reader, "Liquid Experience")
	{
	}

	public override void onActivate(int id)
	{
		base.onActivate(id);
		Message.Display("You feel more experienced!");
		Player player = DM.Player;
		int xp = (player.NextXP - player.PrevXP) / 4;
		player.AddXP(xp);
	}
}
