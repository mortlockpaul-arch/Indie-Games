using EGEngine;

namespace GameEngine;

public class GasStationMenu : GameMenuScreenCls
{
	public GasStationMenu()
	{
		Entry entry = new Entry();
		entry.message = "$40 Fill Up With Gas";
		entry.messagesize = Menu.defaultFont.MeasureString(entry.message);
		MenuEntries.Add(entry);
		entry = new Entry();
		entry.message = "Exit";
		entry.messagesize = Menu.defaultFont.MeasureString(entry.message);
		entry.SelectedFunction += ExitFunc;
		MenuEntries.Add(entry);
	}

	public void Update(PlayerBase playerRef, int qIndex, bool canAccessMenu)
	{
		if (!canAccessMenu)
		{
			base.Update(playerRef, qIndex);
		}
	}

	public void DrawPost(PlayerBase playerRef, int qIndex, bool canAccessMenu)
	{
		if (!canAccessMenu)
		{
			base.DrawPost(playerRef, qIndex);
		}
	}

	private void ExitFunc(object obj, Entry e)
	{
		Timer = 1f;
		State = GMSCState.TransitionOff;
	}
}
