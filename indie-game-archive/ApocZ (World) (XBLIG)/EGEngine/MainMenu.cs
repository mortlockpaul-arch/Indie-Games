using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EGEngine;

public class MainMenu(GameMenus id) : Menu(id)
{
	private MenuEntry SurvivalLocal = new MenuEntry();

	private MenuEntry Trial = new MenuEntry();

	private MenuEntry Purchase = new MenuEntry();

	private MenuEntry XBoxLive = new MenuEntry();

	private MenuEntry SetCharacter = new MenuEntry();

	private MenuEntry SurvivalGuide = new MenuEntry();

	private MenuEntry OtherGames = new MenuEntry();

	private MenuEntry UpdateNotes = new MenuEntry();

	public static GraphicsDevice gdLoadScreen;

	public static GameTime loadStartTime;

	public static bool LoadContentEnabled = false;

	public static bool ExitLevel = false;

	public static bool ThreadRunning = false;

	public static bool StartedInTrialMode = false;

	public static bool SpawningPlayerIntoWorld = false;

	public static float SpawningPlayerTimer = 0f;

	public new static Texture2D backgraoundTexture = null;

	public static AudioVideoMenu AudioVideoInstance;

	private static float JumpServerWaitTimer = 0f;

	private Texture2D playeroverlay;

	public override void LoadContent()
	{
		base.LoadContent();
		AudioVideoInstance = new AudioVideoMenu();
		AudioVideoInstance.LoadMusic();
		SetupMenuEntries();
	}

	public override void Update(float eTime)
	{
		if (!Guide.IsTrialMode && StartedInTrialMode)
		{
			SetupMenuEntries();
		}
		UpdateTransition(eTime);
		if (LevelBaseMenu.LoadState != LevelLoadState.Loaded)
		{
			return;
		}
		JumpServerWaitTimer -= 0.03f;
		if (!(JumpServerWaitTimer < 0f))
		{
			return;
		}
		if (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].gamerTag.Contains("LX3 Livegamer X"))
		{
			if ((Menu.ActivePlayer.menuInput == MenuInput.MenuBack || Menu.ActivePlayer.menuInput == MenuInput.MenuSelect) && !Guide.IsVisible)
			{
				Guide.ShowSignIn(1, onlineOnly: false);
			}
		}
		else if (!SpawningPlayerIntoWorld)
		{
			if (EGENetWorkNext.inviteToGameScheduled)
			{
				SpawningPlayerIntoWorld = true;
				EGENetWorkNext.JoinInvite();
				return;
			}
			int num = ((menuListCountOverride > 0) ? menuListCountOverride : menuEntryList.Count);
			for (int i = 0; i < num; i++)
			{
				menuEntryList[i].Update(eTime, transitionDelta);
			}
			int selectedEntry = SelectedEntry;
			HandleInput();
			if (selectedEntry != SelectedEntry)
			{
				Menu.PlaySelect();
			}
		}
		else if (state == MenuState.Active)
		{
			if (Guide.IsTrialMode)
			{
				TrialSurvivalLiveFunc(null, new MenuEntry());
			}
			else
			{
				LocalSurvivalFunc(null, new MenuEntry());
			}
		}
	}

	public override void Draw()
	{
		if (LevelBaseMenu.LoadState != LevelLoadState.Loaded)
		{
			return;
		}
		EndGameEngine.GraphicMgr.GraphicsDevice.Viewport = EndGameEngine.DefualtViewport;
		bgTextureColor.R = (byte)(255f * transitionDelta);
		bgTextureColor.G = (byte)(255f * transitionDelta);
		bgTextureColor.B = (byte)(255f * transitionDelta);
		bgTextureColor.A = (byte)(255f * transitionDelta);
		if (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].gamerTag.Contains("LX3 Livegamer X"))
		{
			if (playeroverlay == null)
			{
				playeroverlay = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\PlayOverlay");
			}
			Rectangle a = EndGameEngine.DefualtViewport.TitleSafeArea;
			a.Y = EndGameEngine.DefualtViewport.TitleSafeArea.Top;
			float num = (float)a.Height / 750f;
			a.Width = (int)(684f * num);
			a.X = EndGameEngine.DefualtViewport.TitleSafeArea.Center.X - a.Width / 2;
			Menu.spriteBatch.Begin();
			Menu.spriteBatch.Draw(playeroverlay, a, bgTextureColor);
			Menu.spriteBatch.End();
			return;
		}
		Menu.spriteBatch.Begin();
		Menu.spriteBatch.Draw(Menu.titleTexture, EndGameEngine.DefualtViewport.Bounds, bgTextureColor);
		Menu.spriteBatch.End();
		if (!SpawningPlayerIntoWorld)
		{
			base.Draw();
		}
		else
		{
			Color c = TransitionThisColor(Color.LightGray);
			Vector2 zero = Vector2.Zero;
			zero.X = Menu.titleSafeArea.Center.X - (int)(Menu.defaultFont.MeasureString("Spawning Into World...").X * 0.5f);
			zero.Y = Menu.titleSafeArea.Bottom - 120;
			Menu.spriteBatch.Begin();
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Spawning Into World...", zero, c);
			Menu.spriteBatch.End();
		}
		string b = "www.ApocZ.com";
		Vector2 zero2 = Vector2.Zero;
		Color d = TransitionThisColor(Color.LightGray);
		Menu.spriteBatch.Begin();
		zero2.X = Menu.titleSafeArea.Left;
		zero2.Y = Menu.titleSafeArea.Bottom - 24;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b, zero2, d, 0f, Vector2.Zero, 0.85f, SpriteEffects.None, 0);
		Menu.spriteBatch.End();
		if (JumpServerWaitTimer > 0f)
		{
			Vector2 e = new Vector2(50f, 50f);
			Rectangle empty = Rectangle.Empty;
			empty.X = Menu.titleSafeArea.Center.X;
			empty.Y = Menu.titleSafeArea.Center.Y;
			empty.Width = 60;
			empty.Height = 60;
			Rectangle empty2 = Rectangle.Empty;
			empty2.Width = 100;
			empty2.Height = 100;
			OtherGamesMenu.BusyIconAngle += 0.2f;
			Menu.spriteBatch.Begin();
			Menu.spriteBatch.Draw(OtherGamesMenu.BusyIcon, empty, empty2, Color.White, OtherGamesMenu.BusyIconAngle, e, SpriteEffects.None, 0);
			Menu.spriteBatch.End();
		}
	}

	public override void DrawBackground()
	{
	}

	public override void MakeActive(MenuMgr e)
	{
		if (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].gamerTag.Contains("LX3 Livegamer X"))
		{
			menuEntryList.Clear();
		}
		Menu.HackSetViewPort = true;
		base.MakeActive(e);
		MessagePump.Flush();
		LevelBaseMenu.FPSCameraActive = false;
		LevelBaseMenu.gameMode = GameMode.Menu;
		LevelBaseMenu.isLocalMode = false;
		LevelBaseMenu.isTrialMode = false;
		EndGameEngine.UpdatePresence(GamerPresenceMode.AtMenu);
		if (WorldItemsCls.OceanSound != null)
		{
			WorldItemsCls.OceanSound.SetVariable("Distance", 20000f);
		}
		if (WorldItemsCls.ForestDay != null)
		{
			WorldItemsCls.ForestDay.SetVariable("Distance", 20000f);
		}
		if (WorldItemsCls.FireLoop != null)
		{
			WorldItemsCls.FireLoop.SetVariable("Distance", 20000f);
		}
		if (WorldItemsCls.Radiation != null)
		{
			WorldItemsCls.Radiation.SetVariable("Distance", 20000f);
		}
		if (ZombiePositionGrid.ZombieHordeSound != null)
		{
			ZombiePositionGrid.ZombieHordeSound.SetVariable("Distance", 20000f);
		}
		AIBase.DestroyVehicles();
		StartMenu.ApocThemeMusicRampUp = true;
		StartMenu.PlayThemeMusic(e: true);
		SpawningPlayerIntoWorld = false;
		SpawningPlayerTimer = 0f;
		JumpServerWaitTimer = 4f;
		if (LevelBaseMenu.LoadState == LevelLoadState.Loaded)
		{
			Menu.PlaySelect();
		}
	}

	private void SetupMenuEntries()
	{
		MenuEntry.MenuColor = Color.White;
		Vector2 zero = Vector2.Zero;
		menuEntryList.Clear();
		zero.X = Menu.titleSafeArea.Center.X;
		zero.Y = Menu.titleSafeArea.Bottom - 220;
		if (Guide.IsTrialMode)
		{
			StartedInTrialMode = true;
			zero.Y = Menu.titleSafeArea.Bottom - 260;
			menuEntryList.Add(Trial.Set("Survival Trial", MenuTextJustify.Center, zero, TrialSurvivalLiveFunc, EndGameEngine.GameAssetMgr));
			Trial.isSelected = false;
			zero.Y += Trial.textHeight * 0.875f;
			menuEntryList.Add(Purchase.Set("Unlock Full Version", MenuTextJustify.Center, zero, UnlockFullVersionFunc, EndGameEngine.GameAssetMgr));
			Purchase.isSelected = false;
			zero.Y += Purchase.textHeight * 0.875f;
			menuEntryList.Add(XBoxLive.Set("XBox Live - Available In Full Game", MenuTextJustify.Center, zero, XBoxLiveFunc, EndGameEngine.GameAssetMgr));
			XBoxLive.isSelected = false;
			zero.Y += XBoxLive.textHeight * 0.875f;
		}
		else
		{
			StartedInTrialMode = false;
			menuEntryList.Add(XBoxLive.Set("XBox Live", MenuTextJustify.Center, zero, XBoxLiveFunc, EndGameEngine.GameAssetMgr));
			XBoxLive.isSelected = false;
			zero.Y += XBoxLive.textHeight * 0.875f;
			menuEntryList.Add(Trial.Set("Local Survival", MenuTextJustify.Center, zero, LocalSurvivalFunc, EndGameEngine.GameAssetMgr));
			Trial.isSelected = false;
			zero.Y += Trial.textHeight * 0.875f;
		}
		menuEntryList.Add(SetCharacter.Set("Set Character", MenuTextJustify.Center, zero, SetCharacterMenuFunc, EndGameEngine.GameAssetMgr));
		SetCharacter.isSelected = false;
		zero.Y += SetCharacter.textHeight * 0.875f;
		menuEntryList.Add(SurvivalGuide.Set("Survival Guide", MenuTextJustify.Center, zero, SurvivalGuideMenuFunc, EndGameEngine.GameAssetMgr));
		SurvivalGuide.isSelected = false;
		zero.Y += SurvivalGuide.textHeight * 0.875f;
		menuEntryList.Add(OtherGames.Set("Other Games", MenuTextJustify.Center, zero, OtherGamesFunc, EndGameEngine.GameAssetMgr));
		OtherGames.isSelected = false;
		zero.Y += OtherGames.textHeight * 0.875f;
		menuEntryList.Add(UpdateNotes.Set("Update Notes", MenuTextJustify.Center, zero, UpdateNotesFunc, EndGameEngine.GameAssetMgr));
		UpdateNotes.isSelected = false;
		zero.Y += UpdateNotes.textHeight * 0.875f;
	}

	private void UnlockFullVersionFunc(object sender, MenuEntry e)
	{
		Manager.MakeActive(GameMenus.PurchaseMenu);
	}

	private void LocalSurvivalFunc(object sender, MenuEntry e)
	{
		StartMenu.ApocThemeMusicRampUp = false;
		LevelBaseMenu.isLocalMode = true;
		LevelBaseMenu.isTrialMode = false;
		SpawningPlayerIntoWorld = true;
		SpawningPlayerTimer += 0.0334f;
		if (!(SpawningPlayerTimer < 1f))
		{
			SpawningPlayerTimer = float.MinValue;
			string gamerTag = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].gamerTag;
			Storage.PlayerCharacterFilename = gamerTag + "_Character";
			Storage.PlayerStatisFilename = gamerTag + "_LocalStatis";
			Storage.PlayerInventoryFilename = gamerTag + "_LocalInventory";
			Storage.PlayerTentsFilename = gamerTag + "_LocalTents";
			GC.Collect();
			AIBase.ScheduledWorldDownloads.Clear();
			AIBase.AllWorldItems.Reset();
			AIBase.ResetZombies();
			AIBase.ResetVehicles();
			ZombiePositionGrid.Reset();
			LevelBaseMenu.PrepareLoadLevel();
			ApocZSaveDataCls.SyncingToServer = false;
			FPSGameMenu.Close();
			AIBase.BlackFadeTimer = 8f;
			PlayerBase.ApocalypseZ_Hack = true;
			LevelBaseMenu.gameMode = GameMode.SurvivorLocal;
			Manager.MakeActive(GameMenus.FPSGame);
			for (int i = 0; i < 4; i++)
			{
				FPSGameMenu.TrialTime = 90f;
				LevelBaseMenu.Players[i].TargetPraticeMessage = false;
				LevelBaseMenu.Players[i].AvRStartMessage = false;
				LevelBaseMenu.Players[i].DeathTimer = -1f;
				LevelBaseMenu.Players[i].ToggledRespawn = true;
				LevelBaseMenu.Players[i].CurrentBulletsHitCount = 0;
				LevelBaseMenu.Players[i].CurrentBulletsFiredCount = 0;
				LevelBaseMenu.Players[i].CurrentTargetScore = 0;
				LevelBaseMenu.Players[i].CurrentRatioScore = 0;
				LevelBaseMenu.Players[i].CurrentTimeScore = 0;
				LevelBaseMenu.Players[i].IsSplitScreen = false;
			}
			LevelOutside.Reset();
			LevelBaseMenu.AvRai.ResetWave();
			TriggerData.TargetsActive = true;
			_ = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
			try
			{
				AIBase.HostCreateWorld(null);
			}
			catch (Exception ex)
			{
				MessagePump.AddMessage(ex.Message);
			}
			StartMenu.PlayThemeMusic(e: false);
			LevelOutside.SunAngle = 8f;
			AIBase.LocalOffLineMessage = 12f;
			EndGameEngine.UpdatePresence(GamerPresenceMode.SinglePlayer);
		}
	}

	private void TrialSurvivalLiveFunc(object sender, MenuEntry e)
	{
		StartMenu.ApocThemeMusicRampUp = false;
		LevelBaseMenu.isTrialMode = true;
		LevelBaseMenu.isLocalMode = false;
		SpawningPlayerIntoWorld = true;
		SpawningPlayerTimer += 0.0334f;
		if (!(SpawningPlayerTimer < 1f))
		{
			SpawningPlayerTimer = float.MinValue;
			string gamerTag = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].gamerTag;
			Storage.PlayerCharacterFilename = gamerTag + "_Character";
			Storage.PlayerStatisFilename = gamerTag + "_TrialStatis";
			Storage.PlayerInventoryFilename = gamerTag + "_TrialInventory";
			Storage.PlayerTentsFilename = gamerTag + "_TrialTents";
			GC.Collect();
			AIBase.ScheduledWorldDownloads.Clear();
			AIBase.AllWorldItems.Reset();
			AIBase.ResetZombies();
			AIBase.ResetVehicles();
			ZombiePositionGrid.Reset();
			LevelBaseMenu.PrepareLoadLevel();
			ApocZSaveDataCls.SyncingToServer = false;
			FPSGameMenu.Close();
			AIBase.BlackFadeTimer = 8f;
			PlayerBase.ApocalypseZ_Hack = true;
			LevelBaseMenu.gameMode = GameMode.SurvivorLocal;
			Manager.MakeActive(GameMenus.FPSGame);
			for (int i = 0; i < 4; i++)
			{
				FPSGameMenu.TrialTime = 90f;
				LevelBaseMenu.Players[i].TargetPraticeMessage = false;
				LevelBaseMenu.Players[i].AvRStartMessage = false;
				LevelBaseMenu.Players[i].DeathTimer = -1f;
				LevelBaseMenu.Players[i].ToggledRespawn = true;
				LevelBaseMenu.Players[i].CurrentBulletsHitCount = 0;
				LevelBaseMenu.Players[i].CurrentBulletsFiredCount = 0;
				LevelBaseMenu.Players[i].CurrentTargetScore = 0;
				LevelBaseMenu.Players[i].CurrentRatioScore = 0;
				LevelBaseMenu.Players[i].CurrentTimeScore = 0;
				LevelBaseMenu.Players[i].IsSplitScreen = false;
			}
			LevelOutside.Reset();
			LevelBaseMenu.AvRai.ResetWave();
			TriggerData.TargetsActive = true;
			_ = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
			try
			{
				AIBase.HostCreateWorld(null);
			}
			catch (Exception ex)
			{
				MessagePump.AddMessage(ex.Message);
			}
			StartMenu.PlayThemeMusic(e: false);
			LevelOutside.SunAngle = 9.9f;
			AIBase.LocalOffLineMessage = 12f;
		}
	}

	private void XBoxLiveFunc(object sender, MenuEntry e)
	{
		if (!Guide.IsTrialMode)
		{
			AIBase.LocalOffLineMessage = -1f;
			if (!ErrorMessage.valid)
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
					LevelBaseMenu.isTrialMode = false;
					Manager.MakeActive(GameMenus.XBoxLiveMenu);
				}
			}
		}
		else
		{
			Menu.PlayInvalidSelect();
		}
	}

	private void SetCharacterMenuFunc(object sender, MenuEntry e)
	{
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].SetCharacter();
		EndGameEngine.menuMgr.MakeActive(GameMenus.SetCharacterMenu);
	}

	private void SurvivalGuideMenuFunc(object sender, MenuEntry e)
	{
		if (Menu.BackDelegateEntry == e)
		{
			Manager.MakeActive(GameMenus.MainMenu);
			return;
		}
		Manager.SetBackMenuFunction(GameMenus.SurvivalGuideMenu, SurvivalGuideMenuFunc);
		Manager.MakeActive(GameMenus.SurvivalGuideMenu);
	}

	private void OtherGamesFunc(object sender, MenuEntry e)
	{
		Manager.MakeActive(GameMenus.OtherGamesMenu);
	}

	private void UpdateNotesFunc(object sender, MenuEntry e)
	{
		Manager.MakeActive(GameMenus.UpdateNotesMenu);
	}
}
