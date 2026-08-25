using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace RenegadeEngine;

public static class Global
{
	public const int DefaultScreenWidth = 1024;

	public const int DefaultScreenHeight = 768;

	public const float DefaultAspectRatio = 1f;

	public static int TitleSafeWidth = 1200;

	public static int TitleSafeHeight = 100;

	public static int TitleSafeX = 40;

	public static int TitleSafeY = 40;

	private static List<DisplayMode> displayModes;

	private static int screenWidth = 1024;

	private static int screenHeight = 768;

	private static float aspectRatio = 1f;

	public static bool BloomEffect { get; set; }

	public static bool VSync { get; set; }

	public static bool FullScreen { get; set; }

	public static int ScreenWidth => screenWidth;

	public static int ScreenHeight => screenHeight;

	public static float AspectRatio => aspectRatio;

	public static event EventHandler ResolutionChanged;

	public static void SetDisplayModes(List<DisplayMode> modes)
	{
		displayModes = modes;
	}

	public static void GetResolutionList(out List<string> resList)
	{
		resList = new List<string>();
		foreach (DisplayMode displayMode in displayModes)
		{
			resList.Add(displayMode.Width + "x" + displayMode.Height);
		}
	}

	public static void SetScreenDimensions(int width, int height)
	{
		screenWidth = width;
		screenHeight = height;
		aspectRatio = (float)width / (float)height;
		TitleSafeWidth = (int)((float)screenWidth * 0.9f);
		TitleSafeX = (screenWidth - TitleSafeWidth) / 2;
		On_ResolutionChanged(new object(), new EventArgs());
	}

	public static void SetScreenDimensions(int index)
	{
		screenWidth = displayModes[index].Width;
		screenHeight = displayModes[index].Height;
		aspectRatio = displayModes[index].AspectRatio;
		On_ResolutionChanged(new object(), new EventArgs());
	}

	public static void ResetToDefaults()
	{
		screenWidth = 1024;
		screenHeight = 768;
		aspectRatio = 1f;
		TitleSafeWidth = 1200;
		TitleSafeHeight = 100;
		TitleSafeX = 40;
		TitleSafeY = 40;
		BloomEffect = true;
		VSync = true;
		FullScreen = true;
		On_ResolutionChanged(new object(), new EventArgs());
	}

	internal static void On_ResolutionChanged(object sender, EventArgs e)
	{
		if (ResolutionChanged != null)
		{
			ResolutionChanged(sender, e);
		}
	}
}
