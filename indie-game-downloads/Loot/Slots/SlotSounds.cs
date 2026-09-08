using Eyehook.Framework;

namespace Loot.Slots;

public static class SlotSounds
{
	public static void Spin()
	{
		MC.AudioManager.playCue("SlotsSpin");
	}

	public static void Lose()
	{
		MC.AudioManager.playCue("SlotsLose");
	}

	public static void Win()
	{
		MC.AudioManager.playCue("SlotsWin");
	}

	public static void ReelStop()
	{
		MC.AudioManager.playCue("woodBlock");
	}

	public static void ReelHold()
	{
		MC.AudioManager.playCue("woodBlockHigh");
	}
}
