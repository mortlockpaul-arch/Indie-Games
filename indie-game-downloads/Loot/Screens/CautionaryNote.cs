using Loot.Awardments;

namespace Loot.Screens;

public static class CautionaryNote
{
	private static bool hasDisplayed;

	public static void Display()
	{
		if (!hasDisplayed)
		{
			Dialog.Display("A Cautionary Note", "To those that dare brave the depths of this dungeon, beware!\n\nDeath is permanent and... rather unfortunate.  Keep a good stock of health potions and your finger on the right trigger \u0096\u0097.  Only the strong shall survive, but you have the skills and cunning necessary to forge a path to victory!\n\n~The Evil and Unforgiving Management", unlock);
		}
	}

	private static void unlock()
	{
		Profile.Awardments.Unlock(Awardment.IntoTheUnknown);
		hasDisplayed = true;
	}
}
