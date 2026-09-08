using System.IO;
using Loot.Awardments;
using Loot.Dungeon;
using Loot.PC;
using Loot.Screens;

namespace Loot.Items.Scrolls;

public class ScrollEnchantArmor : Scroll
{
	public ScrollEnchantArmor()
		: base("Enchant Armor Scroll")
	{
	}

	public ScrollEnchantArmor(BinaryReader reader)
		: base(reader, "Enchant Armor Scroll")
	{
	}

	public override void onActivate(int id)
	{
		base.onActivate(id);
		Player player = DM.Player;
		Armor armor = player.Armor;
		if (armor != null)
		{
			Message.Display("Your armor glows!");
			ref Stats stats = ref armor.Stats;
			int dEF = stats.DEF + 1;
			stats.DEF = dEF;
			player.CalcStats();
			Profile.Awardments.Unlock(Awardment.SwordAndSworcery);
		}
		else
		{
			Message.Display("Nothing happens!");
		}
	}
}
