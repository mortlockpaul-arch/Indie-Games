using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace BunnyOfWar;

public class BunnyOfWarGame : Game
{
	public static bool showLoadingScreen;

	public BunnyOfWarGame()
	{
		try
		{
			GraphicsManager.graphics = new GraphicsDeviceManager(this);
			GraphicsManager.InitGraphicsStuff();
			base.Components.Add(new GamerServicesComponent(this));
		}
		catch (Exception ex)
		{
			_ = ex.Message;
		}
	}

	protected override void Initialize()
	{
		try
		{
			showLoadingScreen = true;
			base.Initialize();
		}
		catch (Exception ex)
		{
			_ = ex.Message;
		}
	}

	protected override void LoadContent()
	{
		try
		{
			base.Content.RootDirectory = Definitions.ContentRootDirectory;
			RandomStaticGlobals.Content = new ContentManager(base.Services);
			RandomStaticGlobals.Content.RootDirectory = Definitions.ContentRootDirectory;
			RandomStaticGlobals.ContentTemporary = new ContentManager(base.Services);
			RandomStaticGlobals.ContentTemporary.RootDirectory = Definitions.ContentRootDirectory;
			GraphicsManager.spriteBatch = new SpriteBatch(base.GraphicsDevice);
			showLoadingScreen = true;
			GraphicsManager.DrawLoadingScreen();
			base.GraphicsDevice.Present();
			ScreenManager.playThemeSong();
			GraphicsManager.LoadContent(base.Content);
			LevelManager.init(base.Content, GraphicsManager.viewportRect);
			FileManager.Select360StorageDevice();
			FileManager.LoadHighScores();
			LevelManager.LoadPreloadData();
			SoundManager.LoadContent(base.Content);
		}
		catch (Exception ex)
		{
			_ = ex.Message;
		}
		ScreenManager.ShowMainMenu();
		showLoadingScreen = false;
	}

	protected override void UnloadContent()
	{
	}

	protected override void Update(GameTime gameTime)
	{
		if (showLoadingScreen)
		{
			base.Update(gameTime);
			return;
		}
		try
		{
			if (RandomStaticGlobals.UpdateNetworkAfterThisTime < DateTime.Now)
			{
				Networking.UpdateNetworkSession();
				NetworkGameplayManager.ReadPackets();
				RandomStaticGlobals.UpdateNetworkAfterThisTime = DateTime.Now.AddMilliseconds(1000 / Definitions.NetworkUpdatesPerSecond);
			}
			if (ScreenManager.CurrentScreen == ScreenManager.screens.Blank)
			{
				if (RandomStaticGlobals.UpdateAfterThisTime <= DateTime.Now)
				{
					RandomStaticGlobals.UpdateAfterThisTime = DateTime.Now.AddMilliseconds(1000 / Definitions.UpdatesPerSecond);
					List<FighterObject> computerPlayers = FighterManager.getComputerPlayers(onlyLiving: true, canBeDying: false);
					foreach (FighterObject humanPlayer in FighterManager.humanPlayers)
					{
						if (humanPlayer.PROPERTIES.isLocal)
						{
							if (RandomStaticGlobals.GameMode == Definitions.GameMode.none || RandomStaticGlobals.GameMode == Definitions.GameMode.brawler)
							{
								RandomStaticGlobals.InputManagerInstance.processBrawlerInput(humanPlayer, computerPlayers);
							}
							else if (RandomStaticGlobals.GameMode == Definitions.GameMode.runner)
							{
								RandomStaticGlobals.InputManagerInstance.processRunnerInput(humanPlayer, computerPlayers);
							}
							else if (RandomStaticGlobals.GameMode == Definitions.GameMode.swimmer)
							{
								RandomStaticGlobals.InputManagerInstance.processSwimmerInput(humanPlayer, computerPlayers);
							}
							else if (RandomStaticGlobals.GameMode == Definitions.GameMode.redbaron)
							{
								RandomStaticGlobals.InputManagerInstance.processRedBaronInput(humanPlayer, computerPlayers);
							}
							else if (RandomStaticGlobals.GameMode == Definitions.GameMode.cutsceneORqte)
							{
								RandomStaticGlobals.InputManagerInstance.processCutsceneOrQTEInput(humanPlayer);
							}
							else if (RandomStaticGlobals.GameMode == Definitions.GameMode.helicopter)
							{
								RandomStaticGlobals.InputManagerInstance.processHelicopterInput(humanPlayer, computerPlayers);
							}
							else if (RandomStaticGlobals.GameMode == Definitions.GameMode.flappy || RandomStaticGlobals.GameMode == Definitions.GameMode.flappychase)
							{
								RandomStaticGlobals.InputManagerInstance.processFlappyInput(humanPlayer, computerPlayers);
							}
							else if (RandomStaticGlobals.GameMode == Definitions.GameMode.shooter)
							{
								RandomStaticGlobals.InputManagerInstance.processShooterInput(humanPlayer, computerPlayers);
							}
							else if (RandomStaticGlobals.GameMode == Definitions.GameMode.space)
							{
								RandomStaticGlobals.InputManagerInstance.processSpaceInput(humanPlayer, computerPlayers);
							}
							else if (RandomStaticGlobals.GameMode == Definitions.GameMode.gunsmoke)
							{
								RandomStaticGlobals.InputManagerInstance.processGunSmokeInput(humanPlayer, computerPlayers);
							}
							else if (RandomStaticGlobals.GameMode == Definitions.GameMode.trampoline)
							{
								RandomStaticGlobals.InputManagerInstance.processTrampolineInput(humanPlayer, computerPlayers);
							}
							else if (RandomStaticGlobals.GameMode == Definitions.GameMode.brawlerX)
							{
								RandomStaticGlobals.InputManagerInstance.processBrawlerXInput(humanPlayer, computerPlayers);
							}
						}
					}
					foreach (FighterObject item in computerPlayers)
					{
						RandomStaticGlobals.InputManagerInstance.processInput(item, FighterManager.getHumanPlayers(onlyLiving: true, canBeDying: false));
					}
					TriggerManager.checkTimerTriggers();
					if (RandomStaticGlobals.GameMode != Definitions.GameMode.cutsceneORqte && !QuickTimeEventsManager.hasQTE && (GraphicsManager.messages == null || GraphicsManager.messages.Count == 0))
					{
						ProjectileManager.ProcessProjectiles();
						SceneryManager.ProcessScenery();
						FighterManager.ProcessFighters();
						ObstacleManager.ProcessObstacles();
						WaveManager.Update();
					}
					if (RandomStaticGlobals.isPvPEnabled && FighterManager.getHumanPlayers(onlyLiving: true, canBeDying: false).Count <= 1)
					{
						ScreenManager.GameOver();
					}
				}
			}
			else if (RandomStaticGlobals.UpdateAfterThisTime < DateTime.Now)
			{
				RandomStaticGlobals.UpdateAfterThisTime = DateTime.Now.AddMilliseconds(1000 / Definitions.UpdatesPerSecond);
				ScreenManager.UpdateAndProcessInput();
			}
			base.Update(gameTime);
		}
		catch (Exception ex)
		{
			_ = ex.Message;
		}
	}

	protected override void Draw(GameTime gameTime)
	{
		if (showLoadingScreen)
		{
			GraphicsManager.DrawLoadingScreen();
			base.Draw(gameTime);
			return;
		}
		if (RandomStaticGlobals.GameMode == Definitions.GameMode.cutsceneORqte)
		{
			base.GraphicsDevice.Clear(Color.Transparent);
			GraphicsManager.DrawCutscene(gameTime);
		}
		else
		{
			base.GraphicsDevice.Clear(Color.Black);
			GraphicsManager.Draw(gameTime);
		}
		base.Draw(gameTime);
	}
}
