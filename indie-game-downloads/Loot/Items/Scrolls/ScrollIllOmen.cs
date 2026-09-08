using System.IO;
using Loot.Dungeon;
using Loot.PC;
using Loot.Screens;

namespace Loot.Items.Scrolls;

public class ScrollIllOmen : Scroll
{
	public override int Value => (!Identified) ? 100 : 10;

	public ScrollIllOmen()
		: base("Scroll of Ill Omen")
	{
	}

	public ScrollIllOmen(BinaryReader reader)
		: base(reader, "Scroll of Ill Omen")
	{
	}

	public override void onActivate(int id)
	{
		base.onActivate(id);
		int num = DM.Random.Next(4);
		Player player = DM.Player;
		switch (num)
		{
		case 0:
			player.Base.DMG = new DMGRange(player.Base.DMG.Min - 3, player.Base.DMG.Max - 3);
			Message.Display("You feel weaker! (-3 DMG)");
			break;
		case 1:
			player.Base.DEF -= 3;
			Message.Display("You feel vulnerable! (-3 DEF)");
			break;
		case 2:
			player.Base.DEX -= 3;
			Message.Display("You feel clumsy! (-3 DEX)");
			break;
		case 3:
			player.Base.LCK -= 3;
			Message.Display("You feel unlucky! (-3 LCK)");
			break;
		}
		player.CalcStats();
	}
}
