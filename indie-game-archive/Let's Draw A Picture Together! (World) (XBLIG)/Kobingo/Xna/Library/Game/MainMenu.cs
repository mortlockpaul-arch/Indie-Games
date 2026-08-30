using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace Kobingo.Xna.Library.Game;

public class MainMenu : MenuScreen
{
	public PlayScreen PlayScreen { get; set; }

	public SettingsScreen SettingsScreen { get; set; }

	private MenuEntry UnlockFullGameEntry { get; set; }

	public MainMenu(ScreenManager screenManager)
		: base(screenManager, "Main Menu")
	{
		PlayScreen = new PlayScreen(screenManager);
		SettingsScreen = new SettingsScreen(screenManager);
		UnlockFullGameEntry = new MenuEntry("Unlock Full Game", string.Empty, delegate
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			Guide.ShowMarketplace(GameManager.ActiveGamer.PlayerIndex);
		});
		base.Entries.Add(new MenuEntry("Play Game", string.Empty, delegate
		{
			GameManager.TitleScreen.MainMenu.PlayScreen.Show();
		}));
		base.Entries.Add(new MenuEntry("Settings", string.Empty, SettingsScreen));
		base.Entries.Add(UnlockFullGameEntry);
		base.Entries.Add(new MenuEntry("Exit Game", string.Empty, delegate
		{
			((GameComponent)base.ScreenManager).Game.Exit();
		}));
	}

	public override void Draw(GameTime gameTime, float transition)
	{
		UnlockFullGameEntry.Enabled = GameManager.ActiveGamer != null && GameManager.ActiveGamer.Privileges.AllowPurchaseContent;
		if (!Guide.IsTrialMode && base.Entries.Contains(UnlockFullGameEntry))
		{
			base.Entries.Remove(UnlockFullGameEntry);
		}
		base.Draw(gameTime, transition);
	}
}
