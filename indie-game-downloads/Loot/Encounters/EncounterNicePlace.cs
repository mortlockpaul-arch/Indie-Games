using System.IO;
using Loot.Dungeon;
using Loot.Screens;

namespace Loot.Encounters;

public class EncounterNicePlace : Encounter
{
	public EncounterNicePlace(Location loc)
		: base(loc)
	{
	}

	public EncounterNicePlace(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		Dialog.Display(null, "You discover a strange grotto filled with glowing mushrooms.\n\nA burbling brook flows across one side, and along its banks grows soft blue grass.", new DialogOption("What a nice place to take a nap.", rest), new DialogOption("I'll rest when I'm dead!"));
	}

	private void rest()
	{
		Dialog.Display(null, "You stretch out upon the grass and slowly drift off to sleep...", new DialogOption("ZZZzzz...", rest2));
	}

	private void rest2()
	{
		if (base.goodOutcome)
		{
			DM.Player.HP = DM.Player.MaxHP;
			ref Stats reference = ref DM.Player.Base;
			int lCK = reference.LCK + 1;
			reference.LCK = lCK;
			DM.Player.CalcStats();
			Dialog.Display(null, "You awake from your slumber feeling thoroughly refreshed.\n\nYou jump to your feet, knowing that today will be your lucky day! (+1 LCK)");
			return;
		}
		if (DM.Player.Base.DEX > 0)
		{
			ref Stats reference2 = ref DM.Player.Base;
			int lCK = reference2.DEX - 1;
			reference2.DEX = lCK;
			DM.Player.CalcStats();
		}
		Dialog.Display(null, "You awake from your slumber with a mighty yawn.  It feels like you've slept for years...\n\nThe cursed grotto seems have drained the very life from you! (-1 DEX)\n\nYou lift your old, weary bones and head back into the dungeon.");
	}
}
