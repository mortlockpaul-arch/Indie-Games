using System;
using System.IO;
using System.Threading;
using EGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace GameEngine;

public class GameEngine : EndGameEngine
{
	private float TrialTime;

	private StorageHelper InitStorageHelper = new StorageHelper();

	private static bool TrySetStorage;

	private static bool NewLoadThreadstarted;

	private static bool NewLoadThreadRunning;

	public static bool AudioUpdateOnLoadRunning;

	protected override void OnExiting(object sender, EventArgs args)
	{
		if (base.GraphicsDevice != null && !base.GraphicsDevice.IsDisposed)
		{
			base.GraphicsDevice.SetRenderTarget(null);
		}
		base.OnExiting(sender, args);
	}

	protected override void LoadContent()
	{
		base.LoadContent();
		base.IsMouseVisible = true;
		EndGameEngine.AudioEng = new AudioEngine(EndGameEngine.GameAssetMgr.RootDirectory + "\\audio\\audio.xgs");
		EndGameEngine.WaveBnk = new WaveBank(EndGameEngine.AudioEng, EndGameEngine.GameAssetMgr.RootDirectory + "\\audio\\Wave Bank.xwb");
		EndGameEngine.WaveBnkStreaming = new WaveBank(EndGameEngine.AudioEng, EndGameEngine.GameAssetMgr.RootDirectory + "\\audio\\Wave Bank Streaming.xwb", 0, 32);
		EndGameEngine.SoundBnk = new SoundBank(EndGameEngine.AudioEng, EndGameEngine.GameAssetMgr.RootDirectory + "\\audio\\Sound Bank.xsb");
		EndGameEngine.AudioEng.Update();
		Thread thread = new Thread(AudioUpdateOnLoadThread);
		thread.Start();
		Thread.Sleep(5);
		EndGameEngine.menuMgr.AddMenu(new MainMenu(GameMenus.MainMenu));
		EndGameEngine.menuMgr.AddMenu(new OtherGamesMenu(GameMenus.OtherGamesMenu));
		EndGameEngine.menuMgr.AddMenu(new SetCharacterMenu(GameMenus.SetCharacterMenu));
		EndGameEngine.menuMgr.AddMenu(new PlayersMenu(GameMenus.PlayersMenu));
		EndGameEngine.menuMgr.AddMenu(new AudioVideoMenu(GameMenus.AudioVideoMenu));
		EndGameEngine.menuMgr.AddMenu(new PurchaseMenu(GameMenus.PurchaseMenu));
		EndGameEngine.menuMgr.AddMenu(new SurvivalGuideMenu(GameMenus.SurvivalGuideMenu));
		EndGameEngine.menuMgr.AddMenu(new UpdateNotesMenu(GameMenus.UpdateNotesMenu));
		EndGameEngine.LevelMgr = new LevelGame(GameMenus.FPSGame);
		StartMenu.BackGroundOverride = null;
		PlayerBase.PreLoad();
	}

	protected override void Update(GameTime gameTime)
	{
		try
		{
			UpdateInternal(gameTime);
		}
		catch (Exception ex)
		{
			File.WriteAllText("ApocZ-update-crash.log", ex.ToString());
			throw;
		}
	}

	private void UpdateInternal(GameTime gameTime)
	{
		LevelBaseMenu.debugSecondHasElapsed = false;
		float num = (float)gameTime.ElapsedGameTime.Milliseconds * 0.001f;
		LevelBaseMenu.debugElapsedSecounds += num;
		if (LevelBaseMenu.debugElapsedSecounds > 1f)
		{
			LevelBaseMenu.debugSecondHasElapsed = true;
			LevelBaseMenu.debugElapsedSecounds--;
			LevelBaseMenu.debugUpdateFPS = LevelBaseMenu.debugUpdateCounter / 32;
			LevelBaseMenu.debugUpdateCounter = 0;
			LevelBaseMenu.debugDrawFPS = LevelBaseMenu.debugDrawCounter;
			LevelBaseMenu.debugDrawCounter = 0;
		}
		TrialTime += num;
		base.Update(gameTime);
		EndGameEngine.currentEleapsedTime = gameTime;
		EndGameEngine.currentTimeStep = (float)gameTime.ElapsedGameTime.Milliseconds * 0.001f;
		if (!EndGameEngine.GameSettings.FixedTimeStep && EndGameEngine.currentTimeStep > 0.05f)
		{
			EndGameEngine.currentTimeStep = 0.05f;
		}
		if (!UpdateVideo())
		{
			if (!NewLoadThreadstarted && UpdateControllingPlayer() && !LevelBaseMenu.UpdateThreadRunning)
			{
				LevelBaseMenu.InputUpdate.BeginUpdate(gameTime);
			}
			if (LevelBaseMenu.LoadState == LevelLoadState.Loading)
			{
				EndGameEngine.LevelMgr.UpdateLoad(gameTime);
				LevelBaseMenu.InputUpdate.BeginUpdate(gameTime);
			}
			if (EndGameEngine.StartInLevelMenuUpdate && !LevelBaseMenu.UpdateThreadRunning && LevelBaseMenu.LoadState == LevelLoadState.Loaded)
			{
				EndGameEngine.StartInLevelMenuUpdate = false;
				EndGameEngine.menuMgr.MakeActive(GameMenus.MainMenu);
				((LevelGame)EndGameEngine.LevelMgr).UpdateThreadRunNew();
			}
			if (LevelBaseMenu.LoadState != LevelLoadState.Loaded)
			{
				StartMenu.PlayThemeMusic(e: true);
				EndGameEngine.menuMgr.Update(EndGameEngine.currentTimeStep);
			}
			ControllerBase.Update(gameTime);
			if (EndGameEngine.ThreadExceptionArgument != null)
			{
				File.WriteAllText("ApocZ-crash-real.log", EndGameEngine.ThreadExceptionArgument.ToString());
				throw new Exception("Real exception logged to ApocZ-crash-real.log", EndGameEngine.ThreadExceptionArgument);
			}
		}
	}

	protected override void Draw(GameTime gameTime)
	{
		try
		{
			DrawInternal(gameTime);
		}
		catch (Exception ex)
		{
			File.WriteAllText("ApocZ-draw-crash.log", ex.ToString());
			throw;
		}
	}

	private void DrawInternal(GameTime gameTime)
	{
		if (!DrawVideo())
		{
			if (LevelBaseMenu.LoadState == LevelLoadState.Loading)
			{
				while (MyContentManager.LoadingTexture)
				{
				}
				MyContentManager.CanLoadTexture = false;
				EndGameEngine.GraphicMgr.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
				EndGameEngine.LevelMgr.DrawLoadingMessage(gameTime, 1);
				Thread.Sleep(48);
			}
			if (LevelBaseMenu.UpdateThreadRunning)
			{
				if (LevelBaseMenu.LoadState == LevelLoadState.Loaded)
				{
					((LevelGame)EndGameEngine.LevelMgr).DrawLevel();
				}
			}
			else
			{
				EndGameEngine.menuMgr.Draw();
			}
		}
		ConfirmMessage.Draw();
		MessagePump.Draw();
		ErrorMessage.Draw();
		base.Draw(gameTime);
		MyContentManager.CanLoadTexture = true;
	}

	private bool DrawVideo()
	{
		if (!EndGameEngine.LogoDonePlayed && EndGameEngine.videoPlayer.State == MediaState.Playing)
		{
			EndGameEngine.GraphicMgr.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
			EndGameEngine.videoTexture = EndGameEngine.videoPlayer.GetTexture();
			if (EndGameEngine.videoTexture != null)
			{
				Viewport viewport = base.GraphicsDevice.Viewport;
				Viewport viewport2 = base.GraphicsDevice.Viewport;
				viewport2.X = 0;
				viewport2.Y = 0;
				viewport2.Width = EndGameEngine.GameSettings.BackBufferSizeX;
				viewport2.Height = EndGameEngine.GameSettings.BackBufferSizeY;
				base.GraphicsDevice.Viewport = viewport2;
				Menu.spriteBatch.Begin();
				Menu.spriteBatch.Draw(EndGameEngine.videoTexture, EndGameEngine.LogoVideoDstRectangle, EndGameEngine.LogoVideoSrcRectangle, Color.White);
				Menu.spriteBatch.End();
				base.GraphicsDevice.Viewport = viewport;
			}
			return true;
		}
		return false;
	}

	private bool UpdateVideo()
	{
		EndGameEngine.LogoPlayed = true;
		EndGameEngine.LogoDonePlayed = true;
		if (EndGameEngine.LogoPlayed && EndGameEngine.LogoDonePlayed)
		{
			return false;
		}
		return true;
	}

	private bool UpdateControllingPlayer()
	{
		if (!EndGameEngine.controllingPlayer.HasValue)
		{
			if (EndGameEngine.videoPlayer.State == MediaState.Stopped)
			{
				GetControllingPlayer();
				EndGameEngine.StartInLevelMenuUpdate = true;
			}
			return false;
		}
		if (EndGameEngine.GamerSigningIn || Guide.IsVisible)
		{
			SignedInGamer signedInGamer = Gamer.SignedInGamers[EndGameEngine.controllingPlayer.Value];
			if (signedInGamer != null)
			{
				EndGameEngine.GamerSigningIn = false;
			}
			else if (!Guide.IsVisible)
			{
				EndGameEngine.GamerSigningIn = false;
			}
			return false;
		}
		if (!TrySetStorage)
		{
			TrySetStorage = true;
			Storage.SetStorageDevice(EndGameEngine.controllingPlayer.Value, EndGameEngine.GameSettings.GameName);
		}
		if (!NewLoadThreadRunning && Storage.DoneStorageDeviceSelect && EndGameEngine.menuMgr.MenuList[0].state == MenuState.Active)
		{
			StartMenu.StartMessage.scale = 1.15f;
			StartMenu.StartMessage.text = "Initializing...";
			StartMenu.StartMessage.position.X = 640f - Menu.defaultFont.MeasureString(StartMenu.StartMessage.text).X * 0.5f * 1.15f;
			StartMenu.StartMessage.diffuse = Color.White;
			StartMenu.StartMessage.shadow = Color.Black;
			NewLoadThreadRunning = true;
			NewLoadThreadstarted = true;
			File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "ApocZ-init-trace.log"), "Triggered Initializing...\n");
			Thread thread = new Thread(NewLoadThread);
			thread.Start();
			Thread.Sleep(5);
		}
		return true;
	}

	private void NewLoadThread()
	{
		try
		{
			((LevelGame)EndGameEngine.LevelMgr).NewPreLoadContent();
			((LevelGame)EndGameEngine.LevelMgr).UpdateMenuReset();
		}
		catch (Exception ex)
		{
			File.WriteAllText("ApocZ-crash.log", ex.ToString());
		}
		finally
		{
			NewLoadThreadRunning = false;
		}
	}

	public override void HandleInput()
	{
		base.HandleInput();
	}

	private void AudioUpdateOnLoadThread()
	{
		AudioUpdateOnLoadRunning = true;
		while (AudioUpdateOnLoadRunning)
		{
			EndGameEngine.AudioEng.Update();
			Thread.Sleep(256);
		}
	}
}
