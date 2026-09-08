using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Deep_waters;

internal class PauseMenuScreen : MenuScreen
{
	private Texture2D Sfondo;

	public PauseMenuScreen(Texture2D text, Vector2 pos)
		: base("", pos)
	{
		MenuEntry menuEntry = new MenuEntry("Resume Game");
		MenuEntry menuEntry2 = new MenuEntry("Quit Game");
		menuEntry.Selected += base.OnCancel;
		menuEntry2.Selected += QuitGameMenuEntrySelected;
		base.MenuEntries.Add(menuEntry);
		base.MenuEntries.Add(menuEntry2);
		Sfondo = text;
	}

	public PauseMenuScreen(Vector2 pos)
		: base("Pause", pos)
	{
		MenuEntry menuEntry = new MenuEntry("Resume Game");
		MenuEntry menuEntry2 = new MenuEntry("Quit Game");
		menuEntry.Selected += base.OnCancel;
		menuEntry2.Selected += QuitGameMenuEntrySelected;
		base.MenuEntries.Add(menuEntry);
		base.MenuEntries.Add(menuEntry2);
	}

	private void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		MessageBoxScreen messageBoxScreen = new MessageBoxScreen("Are you sure you want to quit this game?");
		messageBoxScreen.Accepted += ConfirmQuitMessageBoxAccepted;
		base.ScreenManager.AddScreen(messageBoxScreen, base.ControllingPlayer);
	}

	private void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
	{
		currentsettings = base.ScreenManager.settings;
		LoadingScreen.Load(base.ScreenManager, true, null, new BackgroundMenu3D("Menu", ext: true), new MainMenuScreen("", new Vector2(base.ScreenManager.GraphicsDevice.Viewport.Width / 2, base.ScreenManager.GraphicsDevice.Viewport.Height / 2)));
	}

	public override void Draw(GameTime gameTime)
	{
		base.ScreenManager.SpriteBatch.Begin();
		if (Sfondo != null && base.IsActive)
		{
			base.ScreenManager.SpriteBatch.Draw(Sfondo, Vector2.Zero, Color.White);
		}
		base.ScreenManager.SpriteBatch.End();
		base.Draw(gameTime);
	}
}
