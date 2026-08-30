using System.Collections.ObjectModel;
using Kobingo.Xna.Library.Data;
using Kobingo.Xna.Library.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Kobingo.Xna.Library.Game;

public class GameManager
{
	private const string FILENAME_SETTINGS = "settings.bin";

	private const string FILENAME_HIGHSCORES = "highscores.bin";

	public static string Title { get; set; }

	public static ScreenManager ScreenManager { get; private set; }

	public static TitleScreen TitleScreen { get; set; }

	public static SettingsManager Settings { get; private set; }

	public static HighscoreManager Highscores { get; private set; }

	public static SignedInGamer ActiveGamer { get; set; }

	public static SpriteFont Font { get; set; }

	public static void Initialize(Game game, string title)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		((Collection<IGameComponent>)(object)game.Components).Add((IGameComponent)new GamerServicesComponent(game));
		SignedInGamer.SignedOut += delegate(object sender, SignedOutEventArgs e)
		{
			if (e.Gamer == ActiveGamer)
			{
				ReturnToTitle();
			}
		};
		ScreenManager = new ScreenManager(game);
		((Collection<IGameComponent>)(object)game.Components).Add((IGameComponent)(object)ScreenManager);
		TitleScreen = new TitleScreen(ScreenManager);
		TitleScreen.Show();
		Settings = new SettingsManager("settings.bin");
		Highscores = new HighscoreManager("highscores.bin");
		Title = title;
		StorageManager.TitleName = title;
		GamepadManager.Initialize(game);
		KeyboardManager.Initialize(game);
	}

	public static void ReturnToTitle()
	{
		ReturnToTitle(showMainMenu: false);
	}

	public static void ReturnToTitle(bool showMainMenu)
	{
		ScreenManager.CloseAll();
		TitleScreen.Show();
		if (showMainMenu)
		{
			TitleScreen.MainMenu.Show();
			return;
		}
		ActiveGamer = null;
		StorageManager.Reset();
	}
}
