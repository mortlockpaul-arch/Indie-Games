using System;
using System.IO;
using Loot.Dungeon;
using Loot.Items.Junks;
using Loot.Screens;
using Loot.Statuses;

namespace Loot.Encounters;

public class EncounterHangedMan : Encounter
{
	public EncounterHangedMan(Location loc)
		: base(loc)
	{
	}

	public EncounterHangedMan(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		Dialog.Display(null, "The blackened husk of a man hangs upside down before you.  He is tied up by one ankle with his other leg dangling wildly askew.\n\nYou spy something glinting behind his rotten teeth... Interesting.", new DialogOption("I should investigate.", investigate), new DialogOption("No thanks.  That's creepy."));
	}

	private void investigate()
	{
		if (base.goodOutcome)
		{
			gem();
		}
		else
		{
			poison();
		}
	}

	private void gem()
	{
		Dialog.Display(null, "You boldly walk up to the hanging man, and pry his jaws open.\n\nWith a brittle crack, his mouth splits apart and a large emerald tumbles to the floor.", new DialogOption("Lucky me!"));
		DM.Player.AddItemOrDrop(new Emerald());
	}

	private void poison()
	{
		Dialog.Display(null, "You grasp the hanging man by his teeth and force his jaws open.\n\nThe corpse sighs, exhaling a cloud of green gas in your face as a single gold piece falls from its lips.\n\nCoughing, you back away.", new DialogOption("I have been poisoned!", poison2));
		DM.Player.Gold++;
	}

	private void poison2()
	{
		DM.Player.Status.Add(new StatusPoison(DM.Player.MaxHP / 20, 10, TimeSpan.FromSeconds(1.0)));
	}
}
