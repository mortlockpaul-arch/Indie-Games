using System;
using System.Collections.Generic;
using Kobingo.Xna.Library.Game;
using Microsoft.Xna.Framework;

namespace Kobingo.Xna.Games.Painter;

internal class PainterMainMenu : MainMenu
{
	public PainterXboxLIVE PainterXboxLIVE { get; set; }

	public PainterPictureGallery PainterPictureGallery { get; set; }

	public PainterMainMenu(ScreenManager screenManager)
		: base(screenManager)
	{
		PainterXboxLIVE = new PainterXboxLIVE(screenManager);
		PainterPictureGallery = new PainterPictureGallery(screenManager);
		base.Entries[2].Text = "Unlock Full Version";
		base.Entries[3].Text = "Exit";
		base.Entries.RemoveAt(1);
		base.Entries.RemoveAt(0);
		List<MenuEntry> entries = base.Entries;
		EventHandler selectedCallback = delegate
		{
			((PainterPlayScreen)base.PlayScreen).Show(PainterSessionType.Local, null);
		};
		entries.Insert(0, new MenuEntry("New Picture", "", selectedCallback));
		base.Entries.Insert(1, new MenuEntry("Picture Gallery", "", delegate
		{
			PainterPictureGallery.Show();
		}));
		base.Entries.Insert(2, new MenuEntry("Xbox LIVE", "", delegate
		{
			PainterXboxLIVE.Show();
		}));
	}

	public override void Update(GameTime gameTime, bool active)
	{
		base.Entries[2].Enabled = GameManager.ActiveGamer.Privileges.AllowOnlineSessions;
		base.Update(gameTime, active);
	}
}
