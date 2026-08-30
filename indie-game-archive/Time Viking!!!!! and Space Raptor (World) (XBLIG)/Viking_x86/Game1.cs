using System;
using System.Threading;
using IMAK3Z0MB1EGAEM;
using IMAK3Z0MB1EGAEM.audio;
using IMAK3Z0MB1EGAEM.director;
using IMAK3Z0MB1EGAEM.menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86.character;
using Viking_x86.director;
using Viking_x86.loader;
using ZP2K9.store;

namespace Viking_x86;

public class Game1 : Game
{
	private GraphicsDeviceManager graphics;

	public static float frameTime;

	public static Texture2D nullTex;

	public static VikingGame vgame;

	public static TimeMgr tMgr;

	public static Loader loader;

	private bool loadcomplete;

	public static Store store;

	public Game1()
	{
		graphics = new GraphicsDeviceManager(this);
		base.Content.RootDirectory = "Content";
		base.Components.Add(new GamerServicesComponent(this));
	}

	protected override void Initialize()
	{
		Rand.rand = new Random();
		Sound.Init();
		loader = new Loader();
		store = new Store();
		VScroll.screenSize = new Vector2(1280f, 720f);
		graphics.PreferMultiSampling = false;
		graphics.PreferredBackBufferWidth = 1280;
		graphics.PreferredBackBufferHeight = 720;
		graphics.SynchronizeWithVerticalRetrace = true;
		graphics.ApplyChanges();
		base.Initialize();
	}

	protected override void LoadContent()
	{
		SpriteTools.sprite = new SpriteBatch(base.GraphicsDevice);
		nullTex = base.Content.Load<Texture2D>("gfx/1x1");
		Thread thread = new Thread(ThreadedMainLoad);
		thread.Start();
	}

	public void ThreadedMainLoad()
	{
		Text.Init(nullTex);
		HighScores.Init();
		CharDefMgr.Initialize();
		vgame = new VikingGame(base.Content);
		loadcomplete = true;
	}

	protected override void UnloadContent()
	{
	}

	protected override void Update(GameTime gameTime)
	{
		frameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
		if (Menu.needsQuit)
		{
			Exit();
		}
		switch (GameState.state)
		{
		case GameState.State.Loading:
			loader.Update();
			if (loadcomplete)
			{
				GameState.state = GameState.State.VikingMenu;
				vgame.Init();
			}
			break;
		case GameState.State.VikingMenu:
		case GameState.State.VikingPlaying:
			vgame.Update(gameTime);
			break;
		}
		store.Update();
		Sound.Update();
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		base.GraphicsDevice.Clear(Color.Black);
		switch (GameState.state)
		{
		case GameState.State.Loading:
			if (!loadcomplete)
			{
				loader.Draw();
			}
			break;
		case GameState.State.VikingMenu:
		case GameState.State.VikingPlaying:
			vgame.Draw(graphics.GraphicsDevice);
			break;
		}
		base.Draw(gameTime);
	}
}
