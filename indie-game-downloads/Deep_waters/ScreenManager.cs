using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace Deep_waters;

public class ScreenManager : DrawableGameComponent
{
	private List<GameScreen> screens = new List<GameScreen>();

	private List<GameScreen> screensToUpdate = new List<GameScreen>();

	public List<SignedInGamer> gamers;

	public StorageManager storageManager;

	public SunBurnCoreSystem lightingSystemManager;

	public SplashScreenGameComponent splashScreenGameComponent;

	public SceneState sceneState;

	public FrameBuffers frameBuffers;

	public SceneInterface sceneInterface;

	public ContentRepository contentRepository;

	public ParticleManager particleManager;

	private InputState input = new InputState();

	public RenderTarget2D rt;

	private SpriteBatch spriteBatch;

	private SpriteFont font;

	private Texture2D blankTexture;

	public GraphicsDeviceManager graphics;

	public AudioManager audioManager;

	public GameSettings default_settings;

	public GameSettings settings;

	private bool isInitialized;

	private bool traceEnabled;

	public int percX = 100;

	public int percY = 100;

	public bool loadsettings;

	public bool finishedonce;

	public bool reachedboss;

	public int currentlevelnumber = 1;

	public int Height;

	public int Width;

	public SpriteFont unselectedFont;

	public SpriteBatch SpriteBatch => spriteBatch;

	public SpriteFont Font => font;

	public bool TraceEnabled
	{
		get
		{
			return traceEnabled;
		}
		set
		{
			traceEnabled = value;
		}
	}

	public void splashsunburn()
	{
		splashScreenGameComponent = new SplashScreenGameComponent(base.Game);
		splashScreenGameComponent.ShowDuringDevelopment = false;
		base.Game.Components.Add(splashScreenGameComponent);
		init_managers();
	}

	public ScreenManager(Game game, GraphicsDeviceManager g)
		: base(game)
	{
		graphics = g;
		game.Components.Add(new GamerServicesComponent(game));
		storageManager = new StorageManager(base.Game);
		game.Components.Add(storageManager);
		audioManager = new AudioManager("Audio", "Sound Bank", "Wave Bank");
		default_settings = new GameSettings();
		settings = new GameSettings();
		gamers = new List<SignedInGamer>(4);
	}

	public void init_sunburn()
	{
		percX = settings.percX;
		percY = settings.percY;
		audioManager.BGMVolume = settings.music / 30;
		audioManager.FXvolume = settings.sfx / 30;
	}

	public override void Initialize()
	{
		base.Initialize();
		isInitialized = true;
	}

	protected override void LoadContent()
	{
		ContentManager content = base.Game.Content;
		rt = new RenderTarget2D(base.Game.GraphicsDevice, base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height, mipMap: true, SurfaceFormat.Dxt5, DepthFormat.Depth24Stencil8, 3, RenderTargetUsage.DiscardContents);
		spriteBatch = new SpriteBatch(rt.GraphicsDevice);
		font = content.Load<SpriteFont>("Font/MenuSelected");
		unselectedFont = content.Load<SpriteFont>("Font/MenuUnselected");
		blankTexture = content.Load<Texture2D>("Background/blank");
		foreach (GameScreen screen in screens)
		{
			screen.LoadContent();
		}
		init_sunburn();
	}

	protected override void UnloadContent()
	{
		foreach (GameScreen screen in screens)
		{
			screen.UnloadContent();
		}
	}

	public void load()
	{
		if (!loadsettings)
		{
			return;
		}
		if (storageManager.filegameexist)
		{
			if (storageManager.settings != null && storageManager.settings != settings)
			{
				settings = storageManager.settings;
				finishedonce = storageManager.settings.gamefinishedonce;
				loadsettings = false;
			}
		}
		else
		{
			storageManager.settings = new GameSettings();
			settings = storageManager.settings;
			finishedonce = storageManager.settings.gamefinishedonce;
			loadsettings = false;
		}
	}

	public override void Update(GameTime gameTime)
	{
		load();
		percX = settings.percX;
		percY = settings.percY;
		audioManager.BGMVolume = (float)settings.music / 30f;
		audioManager.FXvolume = (float)settings.sfx / 30f;
		settings.percX = (int)MathHelper.Clamp(settings.percX, 50f, 100f);
		settings.percY = (int)MathHelper.Clamp(settings.percY, 50f, 100f);
		settings.music = (int)MathHelper.Clamp(settings.music, 0f, 100f);
		settings.sfx = (int)MathHelper.Clamp(settings.sfx, 0f, 100f);
		settings.gameDifficulty = (GameDiff)MathHelper.Clamp((float)settings.gameDifficulty, 1f, 3f);
		input.Update();
		audioManager.update();
		screensToUpdate.Clear();
		foreach (GameScreen screen in screens)
		{
			screensToUpdate.Add(screen);
		}
		bool flag = !base.Game.IsActive;
		bool coveredByOtherScreen = false;
		while (screensToUpdate.Count > 0)
		{
			GameScreen gameScreen = screensToUpdate[screensToUpdate.Count - 1];
			screensToUpdate.RemoveAt(screensToUpdate.Count - 1);
			gameScreen.Update(gameTime, flag, coveredByOtherScreen);
			if (gameScreen.ScreenState == ScreenState.TransitionOn || gameScreen.ScreenState == ScreenState.Active)
			{
				if (!flag)
				{
					gameScreen.HandleInput(input);
					flag = true;
				}
				if (!gameScreen.IsPopup)
				{
					coveredByOtherScreen = true;
				}
			}
		}
		if (traceEnabled)
		{
			TraceScreens();
		}
	}

	private void TraceScreens()
	{
		List<string> list = new List<string>();
		foreach (GameScreen screen in screens)
		{
			list.Add(screen.GetType().Name);
		}
	}

	public override void Draw(GameTime gameTime)
	{
		base.GraphicsDevice.BlendState = BlendState.AlphaBlend;
		base.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
		base.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
		base.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
		base.GraphicsDevice.SetRenderTarget(rt);
		foreach (GameScreen screen in screens)
		{
			if (screen.ScreenState != ScreenState.Hidden)
			{
				screen.Draw(gameTime);
			}
		}
		base.GraphicsDevice.SetRenderTarget(null);
		base.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0f, 0);
		Width = base.GraphicsDevice.Viewport.Width * percX / 100;
		Height = base.GraphicsDevice.Viewport.Height * percY / 100;
		spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
		spriteBatch.Draw(rt, new Rectangle(base.GraphicsDevice.Viewport.Width / 2 - Width / 2, base.GraphicsDevice.Viewport.Height / 2 - Height / 2, Width, Height), Color.White);
		spriteBatch.End();
	}

	public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
	{
		screen.ControllingPlayer = controllingPlayer;
		screen.ScreenManager = this;
		screen.IsExiting = false;
		if (isInitialized)
		{
			screen.LoadContent();
		}
		screens.Add(screen);
	}

	public void RemoveScreen(GameScreen screen)
	{
		if (isInitialized)
		{
			screen.UnloadContent();
		}
		screens.Remove(screen);
		screensToUpdate.Remove(screen);
		_ = screens.Count;
		_ = 0;
	}

	public GameScreen[] GetScreens()
	{
		return screens.ToArray();
	}

	public void FadeBackBufferToBlack(float alpha)
	{
		Viewport viewport = base.GraphicsDevice.Viewport;
		spriteBatch.Begin();
		spriteBatch.Draw(blankTexture, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.Black * alpha);
		spriteBatch.End();
	}

	public void init_managers()
	{
		lightingSystemManager = new SunBurnCoreSystem(base.Game.Services, base.Game.Content);
		sceneState = new SceneState();
		sceneInterface = new SceneInterface();
		sceneInterface.CreateDefaultManagers(RenderingSystemType.Forward, CollisionSystemType.Physics, autoloadpluginmanagers: true);
		frameBuffers = new FrameBuffers(DetailPreference.High, DetailPreference.Medium);
		sceneInterface.ResourceManager.AssignOwnership(frameBuffers);
		particleManager = new ParticleManager(graphics);
		particleManager.ManagerProcessOrder = 0;
		sceneInterface.AddManager(particleManager);
		sceneInterface.ShowConsole = true;
	}
}
