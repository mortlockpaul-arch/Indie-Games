using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;

namespace EGEngine;

public class XBoxLiveMenu(GameMenus id) : Menu(id)
{
	private Vector2 shadowOffset = new Vector2(2f, 2f);

	private bool AvailableSessionListActive;

	private int AvailableSessionIndex;

	private int TextTop;

	private int SelectedNetworkMode;

	private static float JumpServerWaitTimer = 0f;

	private float AvailableNetworkUpdate;

	private float AvailableNetworkRefresh;

	private static int startIndex = 0;

	private static List<string> hostNames = new List<string>();

	private static List<string> playerCounts = new List<string>();

	private static List<string> qualityPer = new List<string>();

	public override void LoadContent()
	{
		base.LoadContent();
		SetupMenuEntries();
	}

	public override void Update(float eTime)
	{
		UpdateTransition(eTime);
		if (state != MenuState.Active)
		{
			return;
		}
		JumpServerWaitTimer -= 0.03f;
		if (!(JumpServerWaitTimer < 0f))
		{
			return;
		}
		if (!MainMenu.SpawningPlayerIntoWorld)
		{
			if (EGENetWorkNext.inviteToGameScheduled)
			{
				MainMenu.SpawningPlayerIntoWorld = true;
				EGENetWorkNext.JoinInvite();
				return;
			}
			for (int i = 0; i < menuEntryList.Count; i++)
			{
				menuEntryList[i].Update(eTime, transitionDelta);
			}
			AvailableNetworkUpdate += 0.03f;
			AvailableNetworkRefresh += 0.03f;
			if (AvailableNetworkUpdate > 10f)
			{
				AvailableNetworkUpdate = 0f;
				EGENetWorkNext.GetAvailableSessionFunc();
			}
			else if (AvailableNetworkRefresh > 1f)
			{
				AvailableNetworkRefresh = 0f;
				EGENetWorkNext.UpdateSessionQualities();
			}
			if (Menu.ActivePlayer == null)
			{
				return;
			}
			if (Menu.ActivePlayer.menuInput == MenuInput.MenuRight)
			{
				if (!AvailableSessionListActive && EGENetWorkNext.GetAvailableSessions().Count > 0)
				{
					AvailableSessionIndex = ((AvailableSessionIndex > EGENetWorkNext.GetAvailableSessions().Count) ? EGENetWorkNext.GetAvailableSessions().Count : AvailableSessionIndex);
					Menu.PlaySelect();
					AvailableSessionListActive = true;
					for (int j = 0; j < menuEntryList.Count; j++)
					{
						menuEntryList[j].isSelected = false;
					}
				}
				else
				{
					Menu.PlayInvalidSelect();
				}
			}
			else if (Menu.ActivePlayer.menuInput == MenuInput.MenuLeft)
			{
				if (!AvailableSessionListActive)
				{
					Menu.PlayInvalidSelect();
				}
				else
				{
					Menu.PlaySelect();
				}
				if (AvailableSessionListActive)
				{
					SelectedEntry = 0;
				}
				menuEntryList[0].isSelected = true;
				AvailableSessionListActive = false;
			}
			else if (Menu.ActivePlayer.menuInput == MenuInput.MenuBack)
			{
				ExitXBoxLiveMenuFunc(null, null);
			}
			if (!AvailableSessionListActive)
			{
				if (Menu.ActivePlayer.menuInput == MenuInput.MenuSelect && menuEntryList[SelectedEntry].strikeOutText == null)
				{
					Menu.ActivePlayer.menuInput = MenuInput.None;
					menuEntryList[SelectedEntry].TrySelected();
				}
				else if (Menu.ActivePlayer.menuInput == MenuInput.MenuUp)
				{
					Menu.PlaySelect();
					SelectedEntry--;
					if (SelectedEntry < 0)
					{
						SelectedEntry = menuEntryList.Count - 1;
					}
				}
				else if (Menu.ActivePlayer.menuInput == MenuInput.MenuDown)
				{
					Menu.PlaySelect();
					SelectedEntry++;
					if (SelectedEntry >= menuEntryList.Count)
					{
						SelectedEntry = 0;
					}
				}
				for (int k = 0; k < menuEntryList.Count; k++)
				{
					if (k == SelectedEntry)
					{
						menuEntryList[k].isSelected = true;
					}
					else
					{
						menuEntryList[k].isSelected = false;
					}
				}
				return;
			}
			List<MyNetworkSessionEntry> availableSessions = EGENetWorkNext.GetAvailableSessions();
			if (availableSessions == null)
			{
				return;
			}
			int count = availableSessions.Count;
			if (Menu.ActivePlayer.menuInput == MenuInput.MenuUp || Menu.ActivePlayer.menuInputContinuos == MenuInput.MenuUp)
			{
				if (AvailableSessionIndex > 0)
				{
					Menu.PlayQuickSelect();
				}
				Menu.ActivePlayer.menuInputContinuos = MenuInput.None;
				AvailableSessionIndex = ((AvailableSessionIndex - 1 > 0) ? (AvailableSessionIndex - 1) : 0);
			}
			else if (Menu.ActivePlayer.menuInput == MenuInput.MenuDown || Menu.ActivePlayer.menuInputContinuos == MenuInput.MenuDown)
			{
				if (AvailableSessionIndex + 1 < count)
				{
					Menu.PlayQuickSelect();
				}
				Menu.ActivePlayer.menuInputContinuos = MenuInput.None;
				AvailableSessionIndex = ((AvailableSessionIndex + 1 < count) ? (AvailableSessionIndex + 1) : (count - 1));
			}
			if (Menu.ActivePlayer.menuInput == MenuInput.MenuSelect)
			{
				JoinSessionFunc(null, null);
			}
		}
		else
		{
			if (SelectedNetworkMode == 0)
			{
				Menu.PlaySelect();
			}
			if (SelectedNetworkMode == 1)
			{
				CreateSessionFunc(null, new MenuEntry());
			}
			else if (SelectedNetworkMode == 2)
			{
				JoinSessionFunc(null, new MenuEntry());
			}
		}
	}

	public override void Draw()
	{
		List<MyNetworkSessionEntry> availableSessions = EGENetWorkNext.GetAvailableSessions();
		string text = "";
		Vector2 zero = Vector2.Zero;
		zero.X = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Right - 400;
		zero.Y = TextTop;
		Menu.spriteBatch.Begin();
		bgTextureColor.R = (byte)(255f * transitionDelta);
		bgTextureColor.G = (byte)(255f * transitionDelta);
		bgTextureColor.B = (byte)(255f * transitionDelta);
		bgTextureColor.A = (byte)(255f * transitionDelta);
		Color c = bgTextureColor;
		Rectangle rec = default(Rectangle);
		Menu.spriteBatch.Draw(Menu.multiplayTexture, EndGameEngine.DefualtViewport.Bounds, c);
		Menu.spriteBatch.End();
		if (!MainMenu.SpawningPlayerIntoWorld)
		{
			Menu.spriteBatch.Begin();
			int num = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Right - 680;
			int num2 = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Right - 260;
			int num3 = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Right - 140;
			float g = 0.825f;
			Color d = TransitionThisColor(Color.Gray);
			Color color = TransitionThisColor(Color.Red);
			Color color2 = TransitionThisColor(Color.Yellow);
			if (AvailableSessionListActive)
			{
				d = TransitionThisColor(Color.LightGray);
			}
			zero.Y = TextTop;
			zero.X = num;
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Host", zero, d, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
			zero.X = num2;
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Players", zero, d, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
			zero.X = num3;
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Ping", zero, d, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
			int num4 = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Height - TextTop;
			int num5 = num4 / 28;
			if (AvailableSessionIndex - startIndex >= num5)
			{
				startIndex = AvailableSessionIndex - num5;
			}
			else if (startIndex > AvailableSessionIndex)
			{
				startIndex = AvailableSessionIndex;
			}
			_ = startIndex;
			zero.Y = TextTop + 28;
			if (availableSessions != null)
			{
				int num6 = startIndex;
				int count = availableSessions.Count;
				for (int i = 0; i <= num5 && i < count; i++)
				{
					if (num6 >= count)
					{
						break;
					}
					g = 0.95f;
					MyNetworkSessionEntry myNetworkSessionEntry = availableSessions[num6];
					if (num6 == AvailableSessionIndex)
					{
						if (AvailableSessionListActive)
						{
							g = 1f;
						}
						d = TransitionThisColor(Color.LightGray);
					}
					else
					{
						d = Color.Gray;
						color.R = 211;
						color2.R = 211;
						color2.G = 211;
						d = TransitionThisColor(d);
						color = TransitionThisColor(color);
						color2 = TransitionThisColor(color2);
					}
					zero.X = num;
					Menu.spriteBatch.DrawString(Menu.defaultFont, myNetworkSessionEntry.HostGamertag, zero, d, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
					text = ((myNetworkSessionEntry.CurrentGamerCount < 10) ? (" " + myNetworkSessionEntry.CurrentGamerCount) : myNetworkSessionEntry.CurrentGamerCount.ToString());
					text = text + "/" + (myNetworkSessionEntry.CurrentGamerCount + myNetworkSessionEntry.OpenPublicGamerSlots);
					zero.X = num2;
					if (myNetworkSessionEntry.OpenPublicGamerSlots < 1)
					{
						Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero, color, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
					}
					else
					{
						Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero, d, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
					}
					text = myNetworkSessionEntry.Ping.ToString();
					zero.X = num3;
					if (myNetworkSessionEntry.Ping > 200)
					{
						Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero, color, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
					}
					else if (myNetworkSessionEntry.Ping > 100)
					{
						Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero, color2, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
					}
					else
					{
						Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero, d, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
					}
					num6++;
					zero.Y += 28f;
				}
			}
			else
			{
				Viewport viewport = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport;
				Vector2 e = new Vector2(50f, 50f);
				Rectangle empty = Rectangle.Empty;
				empty.X = viewport.TitleSafeArea.Center.X;
				empty.Y = viewport.TitleSafeArea.Center.Y;
				empty.Width = 60;
				empty.Height = 60;
				Rectangle empty2 = Rectangle.Empty;
				empty2.Width = 100;
				empty2.Height = 100;
				OtherGamesMenu.BusyIconAngle += 0.2f;
				Menu.spriteBatch.Draw(OtherGamesMenu.BusyIcon, empty, empty2, Color.White, OtherGamesMenu.BusyIconAngle, e, SpriteEffects.None, 0);
			}
			Vector2 zero2 = Vector2.Zero;
			rec.X = EndGameEngine.DefualtViewport.TitleSafeArea.X;
			rec.Y = EndGameEngine.DefualtViewport.TitleSafeArea.Bottom - 80;
			rec.Width = 36;
			rec.Height = 36;
			Menu.DrawButton(rec, Buttons.A, bgTextureColor);
			if (AvailableSessionListActive)
			{
				zero2.X = rec.X + 48;
				zero2.Y = rec.Y - 4;
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Join", zero2, bgTextureColor, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0);
			}
			else
			{
				zero2.X = rec.X + 48;
				zero2.Y = rec.Y - 4;
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Select", zero2, bgTextureColor, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0);
			}
			rec.X = EndGameEngine.DefualtViewport.TitleSafeArea.X + 160;
			rec.Y = EndGameEngine.DefualtViewport.TitleSafeArea.Bottom - 80;
			rec.Width = 36;
			rec.Height = 36;
			Menu.DrawButton(rec, Buttons.B, bgTextureColor);
			zero2.X = rec.X + 48;
			zero2.Y = rec.Y - 4;
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Back", zero2, bgTextureColor, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0);
			Menu.spriteBatch.End();
			base.Draw();
		}
		else
		{
			Color c2 = TransitionThisColor(Color.LightGray);
			zero = Vector2.Zero;
			zero.X = Menu.titleSafeArea.Center.X - (int)(Menu.defaultFont.MeasureString("Spawning Into World...").X * 0.5f);
			zero.Y = Menu.titleSafeArea.Bottom - 120;
			Menu.spriteBatch.Begin();
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Spawning Into World...", zero, c2);
			Menu.spriteBatch.End();
		}
		string b = "www.ApocZ.com";
		Vector2 zero3 = Vector2.Zero;
		Color d2 = TransitionThisColor(Color.LightGray);
		Menu.spriteBatch.Begin();
		zero3.X = Menu.titleSafeArea.Left;
		zero3.Y = Menu.titleSafeArea.Bottom - 24;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b, zero3, d2, 0f, Vector2.Zero, 0.85f, SpriteEffects.None, 0);
		Menu.spriteBatch.End();
		if (JumpServerWaitTimer > 0f)
		{
			Vector2 e2 = new Vector2(50f, 50f);
			Rectangle empty3 = Rectangle.Empty;
			empty3.X = Menu.titleSafeArea.Center.X;
			empty3.Y = Menu.titleSafeArea.Center.Y;
			empty3.Width = 60;
			empty3.Height = 60;
			Rectangle empty4 = Rectangle.Empty;
			empty4.Width = 100;
			empty4.Height = 100;
			OtherGamesMenu.BusyIconAngle += 0.2f;
			Menu.spriteBatch.Begin();
			Menu.spriteBatch.Draw(OtherGamesMenu.BusyIcon, empty3, empty4, Color.White, OtherGamesMenu.BusyIconAngle, e2, SpriteEffects.None, 0);
			Menu.spriteBatch.End();
		}
	}

	public override void MakeActive(MenuMgr e)
	{
		SelectedNetworkMode = 0;
		base.MakeActive(e);
		LevelBaseMenu.isLocalMode = false;
		LevelBaseMenu.isTrialMode = false;
		MainMenu.SpawningPlayerIntoWorld = false;
		string gamerTag = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].gamerTag;
		Storage.PlayerCharacterFilename = gamerTag + "_Character";
		Storage.PlayerStatisFilename = gamerTag + "_OnlineStatis";
		Storage.PlayerInventoryFilename = gamerTag + "_OnlineInventory";
		Storage.PlayerTentsFilename = gamerTag + "_OnlineTents";
		EGENetWorkNext.GetAvailableSessionFunc();
		Menu.PlaySelect();
		SelectedEntry = 0;
		AvailableSessionIndex = 0;
		AvailableSessionListActive = false;
		JumpServerWaitTimer = 4f;
		EndGameEngine.UpdatePresence(GamerPresenceMode.SettingUpMatch);
	}

	private void SetupMenuEntries()
	{
		MenuEntry.MenuColor = Color.White;
		Vector2 zero = Vector2.Zero;
		MenuEntry menuEntry = new MenuEntry();
		MenuEntry menuEntry2 = new MenuEntry();
		zero.X = Menu.titleSafeArea.Left + 40;
		TextTop = Menu.titleSafeArea.Top + 32;
		zero.Y = TextTop;
		menuEntryList.Add(menuEntry.Set("Create Session", MenuTextJustify.Left, zero, CreateSessionFunc, EndGameEngine.GameAssetMgr));
		menuEntry.isSelected = false;
		zero.Y += menuEntry.textHeight * 0.9f;
		menuEntryList.Add(menuEntry2.Set("Exit", MenuTextJustify.Left, zero, ExitXBoxLiveMenuFunc, EndGameEngine.GameAssetMgr));
		menuEntry2.isSelected = false;
		zero.Y += menuEntry2.textHeight * 0.9f;
	}

	private void CreateSessionFunc(object sender, MenuEntry e)
	{
		SelectedNetworkMode = 1;
		StartMenu.ApocThemeMusicRampUp = false;
		MainMenu.SpawningPlayerIntoWorld = true;
		MainMenu.SpawningPlayerTimer += 0.0334f;
		if (!(MainMenu.SpawningPlayerTimer < 1f))
		{
			MainMenu.SpawningPlayerTimer = float.MinValue;
			GC.Collect();
			AIBase.ScheduledWorldDownloads.Clear();
			AIBase.AllWorldItems.Reset();
			AIBase.ResetZombies();
			AIBase.ResetVehicles();
			ZombiePositionGrid.Reset();
			LevelBaseMenu.PrepareLoadLevel();
			ApocZSaveDataCls.SyncingToServer = false;
			if (EGENetWorkNext.CreateSessionFunc(NetworkSessionType.PlayerMatch))
			{
				LoadSurvivorLocal();
			}
		}
	}

	private void JoinSessionFunc(object sender, MenuEntry e)
	{
		SelectedNetworkMode = 2;
		StartMenu.ApocThemeMusicRampUp = false;
		MainMenu.SpawningPlayerIntoWorld = true;
		MainMenu.SpawningPlayerTimer += 0.0334f;
		NetSessionRetCodes netSessionRetCodes = EGENetWorkNext.JoinSessionFunc(AvailableSessionIndex);
		if (netSessionRetCodes != NetSessionRetCodes.Error)
		{
			if (netSessionRetCodes != NetSessionRetCodes.Begin)
			{
				AIBase.ScheduledWorldDownloads.Clear();
				AIBase.AllWorldItems.Reset();
				AIBase.ResetZombies();
				AIBase.ResetVehicles();
				AIBase.AllWeapons.Load("");
				ZombiePositionGrid.Reset();
				LevelBaseMenu.PrepareLoadLevel();
			}
			netSessionRetCodes = EGENetWorkNext.JoinSessionFuncComplete();
			if (netSessionRetCodes == NetSessionRetCodes.Complete)
			{
				MainMenu.SpawningPlayerTimer = float.MinValue;
				ApocZSaveDataCls.SyncingToServer = true;
				LoadSurvivorLocal();
			}
		}
		if (netSessionRetCodes == NetSessionRetCodes.Error)
		{
			SelectedNetworkMode = 0;
			MainMenu.SpawningPlayerIntoWorld = false;
			MainMenu.SpawningPlayerTimer = 0f;
			StartMenu.ApocThemeMusicRampUp = true;
			StartMenu.PlayThemeMusic(e: true);
			MessagePump.AddMessage("Error: Could Not Join Online Session");
		}
	}

	private void LoadSurvivorLocal()
	{
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
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].NetGamerId = EGENetWorkNext.networkSession.LocalGamers[0].Id;
		LevelOutside.Reset();
		LevelBaseMenu.AvRai.ResetWave();
		TriggerData.TargetsActive = true;
		StartMenu.PlayThemeMusic(e: false);
		EndGameEngine.UpdatePresence(GamerPresenceMode.Multiplayer);
	}

	private void ExitXBoxLiveMenuFunc(object sender, MenuEntry e)
	{
		EGENetWorkNext.ExitSession();
		EndGameEngine.menuMgr.MakeActive(GameMenus.MainMenu);
	}
}
