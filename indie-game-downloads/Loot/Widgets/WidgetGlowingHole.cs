using System;
using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Platforms;
using Loot.Screens;

namespace Loot.Widgets;

public class WidgetGlowingHole : WidgetAction
{
	protected override Sprite Sprite => ((double)DungeonView.Camera.Zoom > 0.5) ? WidgetSprite.GlowingHole : WidgetSprite.ArrowDown;

	public WidgetGlowingHole(Location loc)
		: base(loc)
	{
	}

	public WidgetGlowingHole(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		MC.ScreenManager.addScreen(new FadeOutScreen(TimeSpan.FromMilliseconds(500.0), MC.ScreenManager.BackgroundColor, enterHole));
	}

	private void enterHole()
	{
		DM.Map.SetWidget(new WidgetHole(Location));
		MC.ScreenManager.removeAllScreens();
		MC.ScreenManager.addScreen(new PlatformerScreen());
		MC.ScreenManager.addScreen(new FadeInScreen(TimeSpan.FromMilliseconds(500.0), MC.ScreenManager.BackgroundColor, null));
	}
}
