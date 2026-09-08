using Eyehook.Framework;

namespace Loot.Screens;

public static class Message
{
	public static void Display(string msg)
	{
		Display(msg.ToCharArray());
	}

	public static void Display(char[] chars)
	{
		MC.ScreenManager.addScreen(new MessageScreen(chars));
	}
}
