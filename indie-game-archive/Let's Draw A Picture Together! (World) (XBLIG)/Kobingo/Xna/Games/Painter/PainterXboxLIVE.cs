using System;
using System.Collections.Generic;
using Kobingo.Xna.Library.Game;

namespace Kobingo.Xna.Games.Painter;

internal class PainterXboxLIVE : MenuScreen
{
	public PainterPlayScreen PainterPlayScreen { get; set; }

	public PainterXboxLIVE(ScreenManager screenManager)
		: base(screenManager, "Xbox LIVE")
	{
		List<MenuEntry> entries = base.Entries;
		EventHandler selectedCallback = delegate
		{
			PainterPlayScreen.Show(PainterSessionType.Public, null);
		};
		entries.Insert(0, new MenuEntry("Quick Match", "", selectedCallback));
		base.Entries.Insert(1, new MenuEntry("Private Session", "", delegate
		{
			PainterPlayScreen.Show(PainterSessionType.Private, null);
		}));
		base.Entries.Insert(2, new MenuEntry("Back", "", delegate
		{
			Close();
		}));
	}

	public override void Show()
	{
		PainterPlayScreen = (PainterPlayScreen)GameManager.TitleScreen.MainMenu.PlayScreen;
		base.Show();
	}
}
