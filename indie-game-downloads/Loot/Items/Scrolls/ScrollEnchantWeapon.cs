using System.IO;
using Loot.Awardments;
using Loot.Dungeon;
using Loot.PC;
using Loot.Screens;

namespace Loot.Items.Scrolls;

public class ScrollEnchantWeapon : Scroll
{
	public ScrollEnchantWeapon()
		: base("Enchant Weapon Scroll")
	{
	}

	public ScrollEnchantWeapon(BinaryReader reader)
		: base(reader, "Enchant Weapon Scroll")
	{
	}

	public override void onActivate(int id)
	{
		base.onActivate(id);
		Player player = DM.Player;
		Weapon weapon = player.Weapon;
		if (weapon != null)
		{
			DMGRange dMG = weapon.Stats.DMG;
			Message.Display("Your weapon glows!");
			weapon.Stats.DMG = new DMGRange(dMG.Min + 1, dMG.Max + 1);
			player.CalcStats();
			Profile.Awardments.Unlock(Awardment.SwordAndSworcery);
		}
		else
		{
			player.RemoveItem(id);
			Message.Display("Nothing happens!");
		}
	}
}
