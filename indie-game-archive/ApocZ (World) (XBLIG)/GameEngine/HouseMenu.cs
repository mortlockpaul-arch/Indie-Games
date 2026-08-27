using EGEngine;
using Microsoft.Xna.Framework;

namespace GameEngine;

public class HouseMenu : GameMenuScreenCls
{
	public HouseMenu()
	{
		Entry entry = new Entry();
		entry.message = "Post Evidence On Social Media";
		entry.messagesize = Menu.defaultFont.MeasureString(entry.message);
		entry.SelectedFunction += PostEvidenceFunc;
		MenuEntries.Add(entry);
		entry = new Entry();
		entry.message = "Hunt In Early Morning";
		entry.messagesize = Menu.defaultFont.MeasureString(entry.message);
		entry.SelectedFunction += SetMorningFunc;
		MenuEntries.Add(entry);
		entry = new Entry();
		entry.message = "Hunt Middle Of Night";
		entry.messagesize = Menu.defaultFont.MeasureString(entry.message);
		entry.SelectedFunction += SetNightFunc;
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

	private void SetMorningFunc(object obj, Entry e)
	{
		LevelOutside.SetSkyDome(new Vector3(21725f, 11044f, -7782f), new Color(255, 210, 126), new Color(3, 1, 1), 0);
	}

	private void SetNightFunc(object obj, Entry e)
	{
		LevelOutside.SetSkyDome(new Vector3(21725f, 11044f, -7782f), new Color(25, 25, 38), new Color(1, 1, 3), 1);
	}

	private void PostEvidenceFunc(object obj, Entry e)
	{
		LevelObjectives.IssueCallbackFunc(5);
	}

	private void ExitFunc(object obj, Entry e)
	{
		Timer = 1f;
		State = GMSCState.TransitionOff;
	}
}
