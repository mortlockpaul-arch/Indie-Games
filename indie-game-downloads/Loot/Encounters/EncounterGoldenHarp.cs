using System.IO;
using Loot.Dungeon;
using Loot.NPCs;
using Loot.Screens;

namespace Loot.Encounters;

public class EncounterGoldenHarp : Encounter
{
	public EncounterGoldenHarp(Location loc)
		: base(loc)
	{
	}

	public EncounterGoldenHarp(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		Dialog.Display(null, "Resting on the ground before you is large golden harp.\n\nUnfortuantely it is too large and too heavy to take with you, but it is a marvel to behold.", new DialogOption("Play it.", play), new DialogOption("Leave it alone."));
	}

	private void play()
	{
		if (base.goodOutcome)
		{
			playWell();
		}
		else
		{
			playBadly();
		}
	}

	private void playWell()
	{
		Dialog.Display(null, "You run your fingers along the strings, and the harp seems to almost play itself.\n\nThe echoing chamber is filled with beautiful music, and your spirits soar! (+1 LCK)", new DialogOption("That was nice."));
		ref Stats reference = ref DM.Player.Base;
		int lCK = reference.LCK + 1;
		reference.LCK = lCK;
		DM.Player.CalcStats();
	}

	private void playBadly()
	{
		Dialog.Display(null, "You pluck out a few notes, but to be honest, you were never very good at this sort of thing.\n\nFurrowing your brow, you try again, only to snap one of the strings with a horrible TWANG!\n\nIn the distance you hear a hoard of creatures awakening...", new DialogOption("Uh oh.", monsters));
	}

	private void monsters()
	{
		for (int i = 0; i < DM.Map.Rows; i++)
		{
			for (int j = 0; j < DM.Map.Cols; j++)
			{
				Location loc = new Location(i, j);
				if (DM.Map.IsOpenFloor(loc) && DM.Random.Next(4) == 0)
				{
					DM.AddNPC(NPCRegistry.Random(base.depth, loc));
				}
			}
		}
	}
}
