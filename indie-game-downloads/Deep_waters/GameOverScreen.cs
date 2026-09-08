using Microsoft.Xna.Framework;

namespace Deep_waters;

internal class GameOverScreen : MenuScreen
{
	private bool musichasbeenplayed;

	public GameOverScreen()
		: base("", new Vector2(640f, 360f))
	{
		MenuEntry menuEntry = new MenuEntry("yes");
		MenuEntry menuEntry2 = new MenuEntry("no");
		menuEntry.Selected += retry;
		menuEntry2.Selected += returntomain;
		isscreenexitable = false;
		base.MenuEntries.Add(menuEntry);
		base.MenuEntries.Add(menuEntry2);
	}

	private void retry(object sender, PlayerIndexEventArgs e)
	{
		LoadingScreen.Load(base.ScreenManager, true, e.PlayerIndex, new GameplayScreen(base.ScreenManager.currentlevelnumber, base.ScreenManager.reachedboss));
	}

	private void returntomain(object sender, PlayerIndexEventArgs e)
	{
		LoadingScreen.Load(base.ScreenManager, true, null, new BackgroundMenu3D("Menu", ext: true), new MainMenuScreen("", new Vector2(base.ScreenManager.GraphicsDevice.Viewport.Width / 2, base.ScreenManager.GraphicsDevice.Viewport.Height / 2)));
	}

	public override void LoadContent()
	{
		base.LoadContent();
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		if (!musichasbeenplayed)
		{
			if (base.ScreenManager.audioManager.BGMCue.Name != "gameover" && base.ScreenManager.audioManager.BGMCue.IsPlaying)
			{
				base.ScreenManager.audioManager.stopMusic();
			}
			base.ScreenManager.audioManager.PlayMusic("gameover");
			musichasbeenplayed = true;
		}
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
	}
}
