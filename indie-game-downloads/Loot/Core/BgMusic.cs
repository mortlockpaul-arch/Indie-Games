using System;
using Eyehook.Framework;

namespace Loot.Core;

public static class BgMusic
{
	private static string currentCue = "";

	private static int id = 0;

	private static TimeSpan beginTime;

	private static readonly TimeSpan nextTime = TimeSpan.FromSeconds(60.0);

	private static string platformerCue = "Platforms";

	private static string shopCue = "Shop";

	private static string slotsCue = "SlotsMusic";

	private static string gameOverCue = "GameOver";

	private static string[] bgCues = new string[5] { "Bg1", "Bg2", "Bg3", "Bg4", "Bg5" };

	private static void PlayCue(string cueName)
	{
		if (!currentCue.Equals(cueName))
		{
			currentCue = cueName;
			MC.AudioManager.playCue(currentCue);
		}
	}

	public static void TitleMusic()
	{
		id = 0;
		PlayCue(bgCues[id]);
	}

	public static void PlatformerOpen()
	{
		PlayCue(platformerCue);
	}

	public static void PlatformerClose()
	{
		Next();
	}

	public static void ShopOpen()
	{
		PlayCue(shopCue);
	}

	public static void ShopClose()
	{
		PlayCue(bgCues[id]);
	}

	public static void SlotsOpen()
	{
		PlayCue(slotsCue);
	}

	public static void SlotsClose()
	{
		PlayCue(bgCues[id]);
	}

	public static void GameOver()
	{
		PlayCue(gameOverCue);
	}

	public static void Next()
	{
		id++;
		if (id >= bgCues.Length)
		{
			id = 0;
		}
		beginTime = MC.ElapsedTime;
		PlayCue(bgCues[id]);
	}

	public static void NextIfTime()
	{
		if (MC.ElapsedTime - beginTime >= nextTime)
		{
			Next();
		}
	}
}
