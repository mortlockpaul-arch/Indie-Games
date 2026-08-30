using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace Kobingo.Xna.Library.Game;

public class GameMenu : MenuScreen
{
	public SettingsScreen SettingsScreen { get; set; }

	private MenuEntry UnlockFullGameEntry { get; set; }

	public GameMenu(ScreenManager screenManager)
		: base(screenManager, "Game Menu")
	{
		base.IsPopup = true;
		SettingsScreen = new SettingsScreen(screenManager);
		UnlockFullGameEntry = new MenuEntry("Unlock Full Game", string.Empty, delegate
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			Guide.ShowMarketplace(GameManager.ActiveGamer.PlayerIndex);
		});
		base.Entries.Add(new MenuEntry("Resume Game", string.Empty, delegate
		{
			Close();
		}));
		base.Entries.Add(new MenuEntry("Settings", string.Empty, SettingsScreen));
		base.Entries.Add(UnlockFullGameEntry);
		base.Entries.Add(new MenuEntry("Quit To Main Menu", string.Empty, delegate
		{
			GameManager.ReturnToTitle(showMainMenu: true);
		}));
	}

	public override void HandleInput()
	{
		if (ScreenInput.Start)
		{
			Close();
		}
		base.HandleInput();
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
