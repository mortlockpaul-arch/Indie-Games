using System.IO;
using Loot.Awardments;
using Loot.Dungeon;
using Loot.PC;
using Loot.Screens;

namespace Loot.Items.Scrolls;

public class ScrollRemoveCurse : Scroll
{
	public ScrollRemoveCurse()
		: base("Remove Curse Scroll")
	{
	}

	public ScrollRemoveCurse(BinaryReader reader)
		: base(reader, "Remove Curse Scroll")
	{
	}

	public override void onActivate(int id)
	{
		base.onActivate(id);
		int num = 0;
		Player player = DM.Player;
		for (int i = 0; i < player.Inventory.Length; i++)
		{
			Item item = player.Inventory[i];
			if (item != null && item.Identified && item.Cursed)
			{
				player.Inventory[i] = null;
				num++;
			}
		}
		for (int j = 0; j < player.Equipment.Length; j++)
		{
			Item item2 = player.Equipment[j];
			if (item2 != null && item2.Identified && item2.Cursed)
			{
				player.Equipment[j] = null;
				num++;
			}
		}
		player.CalcStats();
		Message.Display(num + " cursed " + ((num == 1) ? "item was" : "items were") + " destroyed.");
		if (num > 0)
		{
			Profile.Awardments.Unlock(Awardment.CurseBreaker);
		}
	}
}
