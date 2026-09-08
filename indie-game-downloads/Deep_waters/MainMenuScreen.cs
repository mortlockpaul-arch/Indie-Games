using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Deep_waters;

internal class MainMenuScreen : MenuScreen
{
	private ContentManager content;

	private MenuEntry playGameMenuEntry;

	private MenuEntry Credits;

	private MenuEntry optionsMenuEntry;

	private MenuEntry controlentry;

	private MenuEntry buyMenuEntry;

	private MenuEntry exitMenuEntry;

	private Texture2D Title;

	public MainMenuScreen(string title, Vector2 pos)
		: base(title, new Vector2(640f, 300f))
	{
		playGameMenuEntry = new MenuEntry("New Game");
		Credits = new MenuEntry("Credits");
		controlentry = new MenuEntry("How to play");
		optionsMenuEntry = new MenuEntry("Options");
		buyMenuEntry = new MenuEntry("Buy The Full Game");
		exitMenuEntry = new MenuEntry("Exit");
		playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
		controlentry.Selected += tutorialEntrySelected;
		optionsMenuEntry.Selected += OptionsMenuEntrySelected;
		Credits.Selected += credits;
		buyMenuEntry.Selected += BuyScreenGameMenuEntrySelected;
		exitMenuEntry.Selected += base.OnCancel;
		base.MenuEntries.Add(playGameMenuEntry);
		base.MenuEntries.Add(controlentry);
		base.MenuEntries.Add(optionsMenuEntry);
		base.MenuEntries.Add(Credits);
		if (Guide.IsTrialMode)
		{
			base.MenuEntries.Add(buyMenuEntry);
		}
		base.MenuEntries.Add(exitMenuEntry);
	}

	public override void LoadContent()
	{
		base.LoadContent();
		if (content == null)
		{
			content = new ContentManager(base.ScreenManager.Game.Services, "Content");
		}
		Title = content.Load<Texture2D>("Sprites/Title");
	}

	private void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		LoadingScreen.Load(base.ScreenManager, true, e.PlayerIndex, new GameplayScreen(1, bossalreadyreach: false));
	}

	private void BuyScreenGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		LoadingScreen.Load(base.ScreenManager, true, e.PlayerIndex, new BuyScreen());
	}

	private void tutorialEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		base.ScreenManager.AddScreen(new ControlsScreen(), e.PlayerIndex);
	}

	private void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		base.ScreenManager.AddScreen(new OptionsMenuScreen(new Vector2(base.ScreenManager.GraphicsDevice.Viewport.Width / 2, base.ScreenManager.GraphicsDevice.Viewport.Height / 3)), e.PlayerIndex);
	}

	protected override void OnCancel(PlayerIndex playerIndex)
	{
		MessageBoxScreen messageBoxScreen = new MessageBoxScreen("Are you sure you want to exit?");
		messageBoxScreen.Accepted += ConfirmExitMessageBoxAccepted;
		base.ScreenManager.AddScreen(messageBoxScreen, playerIndex);
	}

	private void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
	{
		base.ScreenManager.Game.Exit();
	}

	private void credits(object sender, PlayerIndexEventArgs e)
	{
		LoadingScreen.Load(base.ScreenManager, true, e.PlayerIndex, new CreditsScreen());
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		if (!otherScreenHasFocus && base.ScreenManager.audioManager != null)
		{
			if (base.ScreenManager.audioManager.BGMCue == null)
			{
				base.ScreenManager.audioManager.PlayMusic("menu");
			}
			else if (base.ScreenManager.audioManager.BGMCue.IsStopped)
			{
				base.ScreenManager.audioManager.PlayMusic("menu");
			}
		}
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
	}

	public override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
		SpriteBatch spriteBatch = new SpriteBatch(base.ScreenManager.GraphicsDevice);
		if (base.IsActive)
		{
			spriteBatch.Begin();
			spriteBatch.Draw(Title, new Vector2(base.ScreenManager.GraphicsDevice.Viewport.Width / 2 - Title.Width / 2, 10f), Color.White);
			spriteBatch.End();
		}
	}
}
