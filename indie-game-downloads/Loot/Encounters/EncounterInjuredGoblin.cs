using System.IO;
using Loot.Dungeon;
using Loot.Items.Potions;
using Loot.Screens;

namespace Loot.Encounters;

public class EncounterInjuredGoblin : Encounter
{
	public EncounterInjuredGoblin(Location loc)
		: base(loc)
	{
	}

	public EncounterInjuredGoblin(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		Dialog.Display(null, "An old goblin is lying in a pool of his own black blood.\n\nHe reaches out to you and feebly pleads: \"Help me...\"", new DialogOption("Walk away...", abandon), new DialogOption("Kill him.", kill), new DialogOption("Use a health potion.", heal));
	}

	private void abandon()
	{
		Dialog.Display(null, "Having no interest in the wretched creature's troubles, you walk away.\n\nSoon he will be a snack for the spiders and wolves.", new DialogOption("Aw, he's better off dead."));
	}

	private void kill()
	{
		Dialog.Display(null, "You raise your weapon high and bring it down upon the goblin's head.\n\nBut, with his last, shuddering breath, the goblin utters a curse:\n\n\"You will suffer the same fate, " + DM.Player.ClassName + ".  I'll see you in hell!\"", new DialogOption("Pfft.  Not likely."));
	}

	private void heal()
	{
		if (DM.Player.HealthPotions > 0)
		{
			for (int i = 0; i < DM.Player.Inventory.Length; i++)
			{
				if (DM.Player.Inventory[i] is PotionHealth)
				{
					DM.Player.RemoveItem(i);
					break;
				}
			}
			DM.Player.ShopBonus = 0.1f;
			Dialog.Display(null, "You pour the potion between his shriveled lips, and his wounds begin to quickly mend.\n\n\"Thank you, " + DM.Player.ClassName + ".  I am but a lowly shopkeeper, but I shall tell my brothers  of your deed.  It is my hope that they show you the same kindness that you have shown me.\"", new DialogOption("You're welcome."));
		}
		else
		{
			Dialog.Display(null, "If only you had a health potion... You try your best to comfort the old goblin, but his wounds are too great.\n\nWith a nod of appreciation, the goblin's eyes close for the last time.", new DialogOption("Farewell, my green friend."));
		}
	}
}
