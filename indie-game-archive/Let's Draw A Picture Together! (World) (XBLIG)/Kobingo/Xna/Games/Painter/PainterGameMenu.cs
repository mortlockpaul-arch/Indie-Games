using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Kobingo.Xna.Library.Data;
using Kobingo.Xna.Library.Game;
using Microsoft.Xna.Framework.Net;

namespace Kobingo.Xna.Games.Painter;

internal class PainterGameMenu : GameMenu
{
	public PainterPlayScreen PainterPlayScreen { get; set; }

	public PainterGameMenu(ScreenManager screenManager, PainterPlayScreen painterPlayScreen)
		: base(screenManager)
	{
		base.Title = "Picture Menu";
		base.Entries[0].Text = "Continue";
		base.Entries[2].Text = "Unlock Full Version";
		base.Entries[3].Text = "Quit To Main";
		base.Entries.RemoveAt(1);
		List<MenuEntry> entries = base.Entries;
		EventHandler selectedCallback = delegate
		{
			PainterPlayScreen.Save();
			Close();
		};
		entries.Insert(1, new MenuEntry("Save Picture", "", selectedCallback));
		base.Entries.Insert(2, new MenuEntry("Create New", "", delegate
		{
			PainterPlayScreen.SendPacketDataCreateNew(((ReadOnlyCollection<LocalNetworkGamer>)(object)NetworkManager.Session.LocalGamers)[0]);
			PainterPlayScreen.New();
			Close();
		}));
		PainterPlayScreen = painterPlayScreen;
	}

	public override void Show()
	{
		base.Entries[2].Enabled = false;
		if (NetworkManager.Session != null && NetworkManager.Session.IsHost)
		{
			base.Entries[2].Enabled = true;
		}
		base.Show();
	}
}
