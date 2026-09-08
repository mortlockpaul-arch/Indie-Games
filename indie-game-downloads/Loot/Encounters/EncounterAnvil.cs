using System.IO;
using Loot.Awardments;
using Loot.Dungeon;
using Loot.Screens;

namespace Loot.Encounters;

public class EncounterAnvil : Encounter
{
	public EncounterAnvil(Location loc)
		: base(loc)
	{
	}

	public EncounterAnvil(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		Dialog.Display(null, "Before you lies an enormous anvil, and it's surface glitters with the remnants of some ancient magic.", new DialogOption("Place your weapon on the anvil.", use), new DialogOption("Leave it.  It could be cursed!", end));
	}

	private void use()
	{
		if (DM.Player.Weapon == null)
		{
			Dialog.Display(null, "Alas, you do not have a weapon equipped.\n\nYou walk away wondering what might have been...", new DialogOption("Oh, well.", end));
		}
		else if (DM.Player.Weapon.Cursed)
		{
			string name = DM.Player.Weapon.Name;
			DM.Player.Equipment[1] = null;
			Dialog.Display(null, "As you place your " + name + " upon the anvil, it bursts into a shower of multicolored sparks, leaving nothing behind.\n\nYour cursed weapon has been destroyed!", new DialogOption("Okay.", end));
		}
		else if (base.goodOutcome)
		{
			enchant();
		}
		else
		{
			curse();
		}
	}

	private void enchant()
	{
		int num = DM.Player.Depth / 10 + 2;
		DM.Player.Weapon.Stats.DMG = new DMGRange(DM.Player.Weapon.Stats.DMG.Min + num, DM.Player.Weapon.Stats.DMG.Max + num);
		DM.Player.CalcStats();
		Profile.Awardments.Unlock(Awardment.SwordAndSworcery);
		Profile.Awardments.Unlock(Awardment.Anvil);
		Dialog.Display(null, "You place your " + DM.Player.Weapon.Name + " upon the anvil, and it begins to emit a pale blue glow.\n\nYour weapon has been enchanted! (+" + num + ")", new DialogOption("All the better to kill with!", end));
	}

	private void curse()
	{
		DM.Player.Weapon.Curse();
		DM.Player.Weapon.Identify();
		DM.Player.CalcStats();
		Dialog.Display(null, "As you place your " + DM.Player.Weapon.Name + " upon the anvil, you feel a mystical force bind you and drain your very will to fight!\n\nYour weapon has been cursed!", new DialogOption("Noooooooo!", end));
	}

	private void end()
	{
	}
}
