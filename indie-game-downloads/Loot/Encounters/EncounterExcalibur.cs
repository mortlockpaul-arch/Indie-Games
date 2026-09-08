using System.IO;
using Loot.Awardments;
using Loot.Dungeon;
using Loot.Items;
using Loot.Items.Weapons;
using Loot.Screens;

namespace Loot.Encounters;

public class EncounterExcalibur : Encounter
{
	public EncounterExcalibur(Location loc)
		: base(loc)
	{
	}

	public EncounterExcalibur(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		Dialog.Display(null, "A strange black pool stretches out before you.\n\nAs you approach, you hear a faint singing voice--or, maybe, it's just your imagination...", new DialogOption("How odd.  I should investigate...", investigate), new DialogOption("Some things are best left alone."));
	}

	private void investigate()
	{
		if (base.goodOutcome)
		{
			sword();
		}
		else
		{
			tentacle();
		}
	}

	private void sword()
	{
		Dialog.Display(null, "As you approach, you see the point of a blade slowly emerge from the murky depths.\n\nThe singing builds to a rapturous chorus, and, at last, a dripping hand, clutching the weapon's hilt, breaks the water's black surface.", new DialogOption("How can this be?", sword2));
	}

	private void sword2()
	{
		int num = DM.Player.Depth;
		EquipmentTier tier = EquipmentTier.Elven;
		Weapon weapon = ((num < 10) ? new WeaponDagger(tier) : ((num < 25) ? new WeaponShortsword(tier) : ((num >= 40) ? ((Weapon)new WeaponClaymore(tier)) : ((Weapon)new WeaponLongsword(tier)))));
		weapon.Identify();
		DM.Player.AddItemOrDrop(weapon);
		Dialog.Display(null, "You stand there, dumbstruck, as the hand flings an " + weapon.Name + " betwixt your leaden feet.\n\nTruly, you have been blessed by the elves!", new DialogOption("I feel all tingly inside!"));
		Profile.Awardments.Unlock(Awardment.BlessedByElves);
	}

	private void tentacle()
	{
		Dialog.Display(null, "As you approach, you glimpse something upon the pond's inky surface.\n\nIn horror, you realize it is a mouth with glistening teeth perched atop an oily black tentacle... But, it is too late!", new DialogOption("Fight for your life!", tentacle2));
	}

	private void tentacle2()
	{
		DM.Player.HP = 1;
		Dialog.Display(null, "A dozen more tentacles burst from the waters and grab your legs, pulling you into the abysmal depths.  With your last gurgling breath, you beat them back and crawl to the shore.\n\nYou are alive... barely.", new DialogOption("That was close!"));
	}
}
