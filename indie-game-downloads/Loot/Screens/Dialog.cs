using Eyehook.Framework;

namespace Loot.Screens;

public static class Dialog
{
	public static void Display(string title, string text, DialogCallback callback)
	{
		MC.ScreenManager.addScreen(new DialogScreen(title, text, callback, (DialogOption[])null));
	}

	public static void Display(string title, string text, params DialogOption[] options)
	{
		MC.ScreenManager.addScreen(new DialogScreen(title, text, null, options));
	}
}
