using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace EGEngine;

public class MatchLobbyMenu : Menu
{
	public MatchLobbyMenu(GameMenus id)
		: base(id)
	{
	}

	public override void LoadContent()
	{
		base.LoadContent();
		SetupMenus();
	}

	public override void Update(float eTime)
	{
		base.Update(eTime);
	}

	public override void Draw()
	{
		Menu.spriteBatch.Begin();
		Menu.spriteBatch.Draw(Menu.texGradientVertical, Menu.menuGradientRec, bgTextureColor);
		DrawButtonControl(EndGameEngine.GraphicMgr.GraphicsDevice.Viewport, drawSelect: true, drawBack: true, drawReady: false);
		Menu.spriteBatch.End();
		base.Draw();
	}

	public override void DrawBackground()
	{
	}

	public override void MakeActive(MenuMgr e)
	{
		base.MakeActive(e);
	}

	private void SetupMenus()
	{
		Vector2 zero = Vector2.Zero;
		zero.X = (float)Menu.titleSafeArea.X + (float)Menu.titleSafeArea.Width * 0.12f;
		zero.Y = (float)Menu.titleSafeArea.Y + (float)Menu.titleSafeArea.Height * 0.2f;
		MenuEntry menuEntry = new MenuEntry();
		MenuEntry menuEntry2 = new MenuEntry();
		MenuEntry menuEntry3 = new MenuEntry();
		MenuEntry menuEntry4 = new MenuEntry();
		menuEntryList.Add(menuEntry.Set("Invite Friend", MenuTextJustify.Left, zero, RunFriendInvite, EndGameEngine.GameAssetMgr));
		menuEntry.isSelected = true;
		zero.Y += menuEntry.textHeight;
		menuEntryList.Add(menuEntry2.Set("Loadout", MenuTextJustify.Left, zero, LoadoutFunc, EndGameEngine.GameAssetMgr));
		menuEntry2.isSelected = true;
		zero.Y += menuEntry2.textHeight;
		menuEntryList.Add(menuEntry3.Set("EOD Leader", MenuTextJustify.Left, zero, LeaderboardFunc, EndGameEngine.GameAssetMgr));
		menuEntry3.isSelected = true;
		zero.Y += menuEntry3.textHeight;
		menuEntryList.Add(menuEntry4.Set("Controller", MenuTextJustify.Left, zero, ControllerFunc, EndGameEngine.GameAssetMgr));
		menuEntry4.isSelected = true;
		zero.Y += menuEntry4.textHeight;
	}

	private void RunFriendInvite(object sender, MenuEntry e)
	{
		if (!Guide.IsVisible)
		{
			Guide.ShowGameInvite(EndGameEngine.controllingPlayer.Value, null);
		}
	}

	private void LoadoutFunc(object sender, MenuEntry e)
	{
		if (Menu.BackDelegateEntry == e)
		{
			Manager.MakeActive(GameMenus.MatchLobbyMenu);
			return;
		}
		Manager.SetBackMenuFunction(GameMenus.MatchLoadoutMenu, LoadoutFunc);
		Manager.MakeActive(GameMenus.MatchLoadoutMenu);
	}

	private void LeaderboardFunc(object sender, MenuEntry e)
	{
		if (Menu.BackDelegateEntry == e)
		{
			Manager.MakeActive(GameMenus.MatchLobbyMenu);
			return;
		}
		Manager.SetBackMenuFunction(GameMenus.EODLeaderboard, LeaderboardFunc);
		Manager.MakeActive(GameMenus.EODLeaderboard);
	}

	private void ControllerFunc(object sender, MenuEntry e)
	{
		if (Menu.BackDelegateEntry == e)
		{
			Manager.MakeActive(GameMenus.MatchLobbyMenu);
			return;
		}
		Manager.SetBackMenuFunction(GameMenus.FPSControllerMenu, LeaderboardFunc);
		Manager.MakeActive(GameMenus.FPSControllerMenu);
	}
}
