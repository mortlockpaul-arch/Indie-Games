using System.IO;
using Loot.Awardments;
using Loot.Dungeon;
using Loot.Items;
using Loot.Items.Potions;
using Loot.NPCs;
using Loot.Screens;

namespace Loot.Encounters;

public class EncounterWitchesBrew : Encounter
{
	public EncounterWitchesBrew(Location loc)
		: base(loc)
	{
	}

	public EncounterWitchesBrew(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		Dialog.Display(null, "You see the silhouette of an old hag bent over a glowing green cauldron.\n\nAs you approach, you hear her murmuring, \"Curses! I need more Toe of " + DM.Player.ClassName + "!\"", new DialogOption("Attack!", attack), new DialogOption("\"Excuse me, madam.  I have toes.\"", approach), new DialogOption("I like my toes.  Slink away."));
	}

	private void attack()
	{
		Witch witch = new Witch(base.depth, NearestOpenFloor());
		witch.Elite();
		PoofNPC(witch);
		for (int i = 0; i < 2; i++)
		{
			PoofNPC(new Bat(base.depth, NearestOpenFloor()));
		}
	}

	private void approach()
	{
		Dialog.Display(null, "The old hag cackles.  \"Indeed you do, my pretty.  Might I have one for my brew?\"\n\nYou shift uncomfortably under her covetous gaze.", new DialogOption("\"Would Hair of " + DM.Player.ClassName + " work instead?\"", hair), new DialogOption("Fine.  But only one!", toe), new DialogOption("This is insane.  Attack!", attack));
	}

	private void hair()
	{
		Dialog.Display(null, "The witch considers your offer for a moment.\n\n\"Fine, fine.  But it won't be nearly as potent.  Now come closer so Gran can clip your hair.\"", new DialogOption("Okay.", hair2), new DialogOption("Oh, go ahead and take my toe.", toe), new DialogOption("Last chance... Attack!", attack));
	}

	private void hair2()
	{
		Dialog.Display(null, "The witch whips out a fiendishly sharp blade, and, in a flash, she slices off a few strands of your hair.\n\nShe then tosses them into the cauldron, and POOF! her potion is complete.", new DialogOption("...", hair3));
	}

	private void hair3()
	{
		Item item = new PotionHealth();
		Dialog.Display(null, "The witch then dips a flask into the mixture which has now turned from green to red.\n\n\"Here, sonny, take a drop for your troubles.\"\n\nYou gladly accept the " + item.Name + ", and stuff it in you sack.", new DialogOption("What a nice lady.  Goodbye!"));
		DM.Player.AddItemOrDrop(item);
	}

	private void toe()
	{
		ref Stats reference = ref DM.Player.Base;
		int dEX = reference.DEX - 1;
		reference.DEX = dEX;
		DM.Player.CalcStats();
		Dialog.Display(null, "\"My hero!\"  She cries, batting her eyelashes.  \"Now, just sit still for a moment...\"\n\nAnd, with a nimble flick of a blade, she cuts off your poor little piggy!  (-1 DEX)", new DialogOption("Ouch!", toe2));
	}

	private void toe2()
	{
		Item item = new PotionSkill();
		Item item2 = new PotionSkill();
		item.Identify();
		Dialog.Display(null, "She tosses your toe into the cauldron, and POOF! her potion is complete.\n\n\"Here,\" she says, handing you two flasks of the new brew, \"for your troubles.\"\n\nYou accept the " + item.Name + " with a pained smile and hobble away.", new DialogOption("Ow, ow, ow..."));
		DM.Player.AddItemOrDrop(item);
		DM.Player.AddItemOrDrop(item2);
		Profile.Awardments.Unlock(Awardment.Toeless);
	}
}
