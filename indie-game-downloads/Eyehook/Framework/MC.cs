using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Eyehook.Framework;

public static class MC
{
	public static TimeSpan ElapsedTime = TimeSpan.Zero;

	public static GameState State;

	private static bool guideVisible;

	private static EyehookGame game;

	private static SpriteBatch spriteBatch;

	private static ScreenManager screenManager;

	private static GamerManager gamerManager;

	private static GamePadManager gamePadManager;

	private static StorageManager storageManager;

	private static AudioManager audioManager;

	public static EyehookGame Game => game;

	public static ContentManager Content => game.Content;

	public static SpriteBatch SpriteBatch => spriteBatch;

	public static ScreenManager ScreenManager => screenManager;

	public static GamerManager GamerManager => gamerManager;

	public static GamePadManager GamePadManager => gamePadManager;

	public static StorageManager StorageManager => storageManager;

	public static AudioManager AudioManager => audioManager;

	public static void Initialize(EyehookGame eyehookGame)
	{
		game = eyehookGame;
		spriteBatch = new SpriteBatch(game.GraphicsDevice);
		screenManager = Eyehook.Framework.ScreenManager.instance;
		audioManager = Eyehook.Framework.AudioManager.instance;
		audioManager.Initialize();
	}

	public static void InitializePlayer(PlayerIndex playerIndex)
	{
		gamerManager = new GamerManager(playerIndex);
		gamePadManager = new GamePadManager(playerIndex);
		storageManager = new StorageManager(playerIndex);
		GamerManager.Initialize();
		StorageManager.Initialize();
	}

	public static void LoadContent()
	{
	}

	public static void UnloadContent()
	{
		audioManager.unloadContent();
	}

	public static void Reset()
	{
		storageManager.Dispose();
		storageManager = null;
		gamePadManager.dispose();
		gamePadManager = null;
		gamerManager.dispose();
		gamerManager = null;
		Eyehook.Framework.ScreenManager.ForceReset();
		screenManager = Eyehook.Framework.ScreenManager.instance;
	}

	public static void Update(GameTime gameTime)
	{
		ElapsedTime += gameTime.ElapsedGameTime;
		if (State == GameState.EXIT)
		{
			game.Exit();
		}
		if (guideVisible)
		{
			AudioManager.unMuteMusic();
		}
		guideVisible = false;
		Eyehook.Framework.GamePadManager.beginUpdate();
		ScreenManager.update(gameTime);
		Eyehook.Framework.GamePadManager.endUpdate(gameTime);
		AudioManager.update(gameTime);
	}

	public static void Draw(GameTime gameTime)
	{
		ScreenManager.draw(gameTime);
	}
}
