using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EGEngine;

public class ExploreMapMenu : Menu
{
	public ExploreMapMenu(GameMenus id)
		: base(id)
	{
	}

	public override void LoadContent()
	{
		base.LoadContent();
	}

	public override void Update(float eTime)
	{
		int selectedEntry = SelectedEntry;
		base.Update(eTime);
		if (selectedEntry != SelectedEntry)
		{
			Menu.PlaySelect();
		}
	}

	public override void Draw()
	{
		Viewport viewport = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport;
		EndGameEngine.GraphicMgr.GraphicsDevice.Viewport = EndGameEngine.DefualtViewport;
		Menu.spriteBatch.Begin();
		bgTextureColor.R = (byte)(200f * transitionDelta);
		bgTextureColor.G = (byte)(200f * transitionDelta);
		bgTextureColor.B = (byte)(200f * transitionDelta);
		bgTextureColor.A = (byte)(200f * transitionDelta);
		Menu.spriteBatch.Draw(LevelBaseMenu.texBlack, EndGameEngine.DefualtViewport.TitleSafeArea, bgTextureColor);
		bgTextureColor.R = (byte)(40f * transitionDelta);
		bgTextureColor.G = (byte)(40f * transitionDelta);
		bgTextureColor.B = (byte)(40f * transitionDelta);
		bgTextureColor.A = (byte)(40f * transitionDelta);
		int height = (int)((float)EndGameEngine.DefualtViewport.TitleSafeArea.Width / (float)Menu.titleTexture.Width * (float)Menu.titleTexture.Height);
		Menu.spriteBatch.Draw(d: new Rectangle(EndGameEngine.DefualtViewport.TitleSafeArea.X, EndGameEngine.DefualtViewport.TitleSafeArea.Y, EndGameEngine.DefualtViewport.TitleSafeArea.Width, height), s: new Rectangle(4, 4, Menu.titleTexture.Width - 8, Menu.titleTexture.Height - 8), t: Menu.titleTexture, c: bgTextureColor);
		DrawButtonControl(EndGameEngine.GraphicMgr.GraphicsDevice.Viewport, drawSelect: true, drawBack: true, drawReady: false);
		Menu.spriteBatch.End();
		base.Draw();
		EndGameEngine.GraphicMgr.GraphicsDevice.Viewport = viewport;
	}

	public override void DrawBackground()
	{
	}

	public override void MakeActive(MenuMgr e)
	{
		base.MakeActive(e);
		for (int i = 0; i < 4; i++)
		{
			if (LevelBaseMenu.Players[i].IsValid)
			{
				LevelBaseMenu.Players[i].KillSound();
			}
		}
		SetupMenus();
		Menu.PlaySelect();
		base.Update(0f);
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
		MenuEntry menuEntry5 = new MenuEntry();
		menuEntryList.Clear();
		if (EndGameEngine.GameSettings.GameName.Contains("ApocalypseZ"))
		{
			menuEntryList.Add(menuEntry.Set("Resume", MenuTextJustify.Left, zero, ResumeFunc, EndGameEngine.GameAssetMgr));
			menuEntry.isSelected = true;
			zero.Y += menuEntry.textHeight;
			menuEntryList.Add(menuEntry2.Set("Controller", MenuTextJustify.Left, zero, ControllerFunc, EndGameEngine.GameAssetMgr));
			menuEntry2.isSelected = false;
			zero.Y += menuEntry2.textHeight;
			if (!Guide.IsTrialMode && !LevelBaseMenu.isLocalMode)
			{
				menuEntryList.Add(menuEntry3.Set("Players", MenuTextJustify.Left, zero, PlayersFunc, EndGameEngine.GameAssetMgr));
				menuEntry3.isSelected = false;
				zero.Y += menuEntry3.textHeight;
				menuEntryList.Add(menuEntry4.Set("Invite Friend", MenuTextJustify.Left, zero, RunFriendInvite, EndGameEngine.GameAssetMgr));
				menuEntry4.isSelected = false;
				zero.Y += menuEntry4.textHeight;
			}
			menuEntryList.Add(menuEntry5.Set("Exit The Apocalypse", MenuTextJustify.Left, zero, ExitXBoxLiveFunc, EndGameEngine.GameAssetMgr));
			menuEntry5.isSelected = false;
			zero.Y += menuEntry5.textHeight;
		}
	}

	private void ResumeFunc(object sender, MenuEntry e)
	{
		ExecuteBackDelegate();
		Menu.PlaySelect();
	}

	private void LoadoutFunc(object sender, MenuEntry e)
	{
		if (Menu.BackDelegateEntry == e)
		{
			Manager.MakeActive(GameMenus.ExploreMenu);
			return;
		}
		Manager.SetBackMenuFunction(GameMenus.MatchLoadoutMenu, LoadoutFunc);
		Manager.MakeActive(GameMenus.MatchLoadoutMenu);
	}

	private void LeaderboardFunc(object sender, MenuEntry e)
	{
		if (Menu.BackDelegateEntry == e)
		{
			Manager.MakeActive(GameMenus.ExploreMenu);
			return;
		}
		Manager.SetBackMenuFunction(GameMenus.EODLeaderboard, LeaderboardFunc);
		Manager.MakeActive(GameMenus.EODLeaderboard);
	}

	private void ControllerFunc(object sender, MenuEntry e)
	{
		if (Menu.BackDelegateEntry == e)
		{
			Manager.MakeActive(GameMenus.ExploreMenu);
			return;
		}
		Manager.SetBackMenuFunction(GameMenus.FPSControllerMenu, ControllerFunc);
		Manager.MakeActive(GameMenus.FPSControllerMenu);
	}

	private void AudioVideoFunc(object sender, MenuEntry e)
	{
		if (Menu.BackDelegateEntry == e)
		{
			Manager.MakeActive(GameMenus.ExploreMenu);
			return;
		}
		Manager.SetBackMenuFunction(GameMenus.AudioVideoMenu, AudioVideoFunc);
		Manager.MakeActive(GameMenus.AudioVideoMenu);
	}

	private void PlayersFunc(object sender, MenuEntry e)
	{
		if (Menu.BackDelegateEntry == e)
		{
			Manager.MakeActive(GameMenus.ExploreMenu);
			return;
		}
		Manager.SetBackMenuFunction(GameMenus.PlayersMenu, PlayersFunc);
		Manager.MakeActive(GameMenus.PlayersMenu);
	}

	private void RunFriendInvite(object sender, MenuEntry e)
	{
		if (!Guide.IsVisible && !ErrorMessage.valid)
		{
			Menu.ActivePlayer.menuInput = MenuInput.None;
			Menu.ActivePlayer.menuInputContinuos = MenuInput.None;
			Menu.ActivePlayer.menuInputRightStick = MenuInput.None;
			LevelBaseMenu.InputUpdate.menuInput = MenuInput.None;
			LevelBaseMenu.InputUpdate.menuInputContinuos = MenuInput.None;
			LevelBaseMenu.InputUpdate.menuInputRightStick = MenuInput.None;
			GamePad.GetState(EndGameEngine.controllingPlayer.Value);
			SignedInGamer signedInGamer = Gamer.SignedInGamers[EndGameEngine.controllingPlayer.Value];
			if (signedInGamer == null || !signedInGamer.IsSignedInToLive || !signedInGamer.Privileges.AllowOnlineSessions)
			{
				ErrorMessage.AddMessage("The current gamer profile does not have suitable privileges to perform this operation. You may require a LIVE Gold account, or need to change your parental control settings.", backToMain: true, atemptSignIn: true);
				Menu.PlayInvalidSelect();
			}
			else
			{
				Guide.ShowGameInvite(EndGameEngine.controllingPlayer.Value, null);
			}
		}
	}

	private void ExitXBoxLiveFunc(object sender, MenuEntry e)
	{
		if (e.text == "ConfirmMessage")
		{
			FPSGameMenu.Close();
			AIBase.BlackFadeTimer = 5f;
			Storage.SavePlayerStatus();
			EGENetWorkNext.ExitSession();
			EndGameEngine.LevelMgr.UpdateMenuReset();
			EndGameEngine.menuMgr.MakeActive(GameMenus.MainMenu);
			for (int i = 0; i < 4; i++)
			{
				LevelBaseMenu.Players[i].Spawned = false;
				if (LevelBaseMenu.Players[i].playerIndex != EndGameEngine.controllingPlayer)
				{
					LevelBaseMenu.Players[i].IsValid = false;
				}
			}
			transitionTime = 0f;
		}
		else
		{
			ConfirmMessage.AddMessage("Exit The Apocalypse?", ExitXBoxLiveFunc);
		}
	}

	private void ExitExploreFunc(object sender, MenuEntry e)
	{
		if (e.text == "ConfirmMessage")
		{
			EndGameEngine.LevelMgr.UpdateMenuReset();
			EndGameEngine.menuMgr.MakeActive(GameMenus.MainMenu);
			for (int i = 0; i < 4; i++)
			{
				LevelBaseMenu.Players[i].Spawned = false;
				if (LevelBaseMenu.Players[i].playerIndex != EndGameEngine.controllingPlayer)
				{
					LevelBaseMenu.Players[i].IsValid = false;
				}
			}
			transitionTime = 0f;
		}
		else if (PlayerBase.ToyPlane_Hack)
		{
			ConfirmMessage.AddMessage("Exit To Lobby?", ExitExploreFunc);
		}
		else
		{
			ConfirmMessage.AddMessage("Exit To Main Menu?", ExitExploreFunc);
		}
	}
}
