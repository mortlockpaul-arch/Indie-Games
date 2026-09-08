using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StarbeamDefenderGame.Menus;

namespace StarbeamDefenderGame;

public class StarbeamDefender : Microsoft.Xna.Framework.Game
{
	private GraphicsDeviceManager graphics;

	private SpriteBatch spriteBatch;

	private int currentscreen;

	private MainGame maingame;

	private AudioManager audiomanager;

	private StartMenu startmenu;

	private LogoScreen logomenu;

	private MainMenu mainmenu;

	private GameOver gameovermenu;

	private SetupMenu setupmenu;

	private HighScoresMenu highscoremenu;

	private ScreenSize sizemenu;

	private int framelimiter;

	private RenderTarget2D backbuffer;

	public StarbeamDefender()
	{
		graphics = new GraphicsDeviceManager(this);
		base.Content.RootDirectory = "Content";
		graphics.PreferredBackBufferHeight = 720;
		graphics.PreferredBackBufferWidth = 1280;
		graphics.ApplyChanges();
	}

	protected override void Initialize()
	{
		base.Initialize();
		FileAndGamerServices.gamer = new GamerServicesManager(this);
		if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back))
		{
			FileAndGamerServices.gamer.EnterTrialMode = true;
		}
	}

	protected override void LoadContent()
	{
		backbuffer = new RenderTarget2D(base.GraphicsDevice, 1280, 720, mipMap: false, base.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
		FileAndGamerServices.savegames = new SavesManager(null);
		spriteBatch = new SpriteBatch(base.GraphicsDevice);
		audiomanager = new AudioManager();
		audiomanager.Initialise(base.Content);
		mainmenu = new MainMenu();
		mainmenu.LoadContent(base.Content, "Menus/MainMenu");
		logomenu = new LogoScreen();
		logomenu.LoadContent(base.Content, "Menus/Logo");
		startmenu = new StartMenu();
		startmenu.LoadContent(base.Content, "Menus/StartMenu");
		gameovermenu = new GameOver();
		gameovermenu.LoadContent(base.Content, audiomanager);
		setupmenu = new SetupMenu();
		setupmenu.LoadContent(base.Content, "Menus/Setup");
		highscoremenu = new HighScoresMenu();
		highscoremenu.LoadContent(base.Content, "Menus/HighScores");
		sizemenu = new ScreenSize();
		sizemenu.LoadContent(base.Content, "Menus/");
		LoadEris();
		currentscreen = 0;
	}

	private void LoadEris()
	{
		maingame = new MainGame();
		maingame.LoadAssets("Eris", base.Content, audiomanager);
		maingame.SetupLevel(588, new Vector2(200f, 100f), new Vector2(256f, 128f), 0, 100);
	}

	private void LoadAmun()
	{
		maingame = new MainGame();
		maingame.LoadAssets("Amun", base.Content, audiomanager);
		maingame.SetupLevel(588, new Vector2(200f, 100f), new Vector2(400f, 200f), 4, 100);
	}

	private void LoadHelios()
	{
		maingame = new MainGame();
		maingame.LoadAssets("Helios", base.Content, audiomanager);
		maingame.SetupLevel(588, new Vector2(200f, 100f), new Vector2(400f, 200f), 4, 400);
	}

	private void LoadBoreas()
	{
		maingame = new MainGame();
		maingame.LoadAssets("Boreas", base.Content, audiomanager);
		maingame.SetupLevel(588, new Vector2(200f, 100f), new Vector2(400f, 200f), 4, 400);
	}

	protected override void UnloadContent()
	{
	}

	protected override void Update(GameTime gameTime)
	{
		framelimiter += gameTime.ElapsedGameTime.Milliseconds;
		while (framelimiter > 15)
		{
			switch (currentscreen)
			{
			case 0:
			{
				int num3 = logomenu.DoGameUpdate(16);
				if (num3 == 1)
				{
					currentscreen = 1;
				}
				break;
			}
			case 1:
			{
				int num2 = startmenu.DoGameUpdate(16);
				if (num2 == 1)
				{
					FileAndGamerServices.savegames = new SavesManager(startmenu.StorageAccessDevice);
					FileAndGamerServices.savegames.LoadFiles();
					if (FileAndGamerServices.savegames.NeedsSetup)
					{
						currentscreen = 12;
						sizemenu.Setup(FileAndGamerServices.savegames.LWCMasterFile.ScreenScale);
						FileAndGamerServices.savegames.LWCMasterFile.ScreenScale = 1f;
						sizemenu.Reset();
					}
					else
					{
						currentscreen = 2;
						mainmenu.Reset();
					}
				}
				break;
			}
			case 2:
				switch (mainmenu.DoGameUpdate(16, startmenu.MasterControllerIndex))
				{
				case 1:
					currentscreen = 3;
					setupmenu.Reset(startmenu.MasterControllerIndex);
					break;
				case 2:
					highscoremenu.Reset();
					currentscreen = 11;
					break;
				case 3:
					sizemenu.Reset();
					sizemenu.Setup(FileAndGamerServices.savegames.LWCMasterFile.ScreenScale);
					FileAndGamerServices.savegames.LWCMasterFile.ScreenScale = 1f;
					currentscreen = 12;
					break;
				case 4:
					Exit();
					break;
				}
				break;
			case 3:
				switch (setupmenu.DoGameUpdate(16))
				{
				case 1:
					currentscreen = 9;
					LoadEris();
					if (setupmenu.PlayerTwoController != -1)
					{
						maingame.Reset(startmenu.MasterControllerIndex, Color.Yellow, (PlayerIndex)setupmenu.PlayerTwoController, Color.Red, setupmenu.Difficulty);
					}
					else
					{
						maingame.Reset(startmenu.MasterControllerIndex, Color.Yellow, setupmenu.Difficulty);
					}
					break;
				case 2:
					currentscreen = 9;
					LoadAmun();
					if (setupmenu.PlayerTwoController != -1)
					{
						maingame.Reset(startmenu.MasterControllerIndex, Color.Brown, (PlayerIndex)setupmenu.PlayerTwoController, Color.Red, setupmenu.Difficulty);
					}
					else
					{
						maingame.Reset(startmenu.MasterControllerIndex, Color.Brown, setupmenu.Difficulty);
					}
					break;
				case 3:
					currentscreen = 9;
					LoadHelios();
					if (setupmenu.PlayerTwoController != -1)
					{
						maingame.Reset(startmenu.MasterControllerIndex, Color.SaddleBrown, (PlayerIndex)setupmenu.PlayerTwoController, Color.Red, setupmenu.Difficulty);
					}
					else
					{
						maingame.Reset(startmenu.MasterControllerIndex, Color.SaddleBrown, setupmenu.Difficulty);
					}
					break;
				case 4:
					currentscreen = 9;
					LoadBoreas();
					if (setupmenu.PlayerTwoController != -1)
					{
						maingame.Reset(startmenu.MasterControllerIndex, Color.Blue, (PlayerIndex)setupmenu.PlayerTwoController, Color.Green, setupmenu.Difficulty);
					}
					else
					{
						maingame.Reset(startmenu.MasterControllerIndex, Color.Blue, setupmenu.Difficulty);
					}
					break;
				case -1:
					mainmenu.Reset();
					currentscreen = 2;
					break;
				}
				break;
			case 9:
				if (!maingame.Update(16))
				{
					currentscreen = 10;
					gameovermenu.Reset();
				}
				break;
			case 10:
				if (gameovermenu.Update(16, startmenu.MasterControllerIndex) == 1)
				{
					mainmenu.Reset();
					currentscreen = 2;
				}
				break;
			case 11:
				if (highscoremenu.DoGameUpdate(16, startmenu.MasterControllerIndex) == 1)
				{
					mainmenu.Reset();
					currentscreen = 2;
				}
				break;
			case 12:
			{
				int num = sizemenu.DoGameUpdate(gameTime.ElapsedGameTime.Milliseconds, startmenu.MasterControllerIndex);
				if (num == 1)
				{
					FileAndGamerServices.savegames.LWCMasterFile.ScreenScale = sizemenu.GetScreenSizeMod;
					FileAndGamerServices.savegames.SaveFiles();
					mainmenu.Reset();
					currentscreen = 2;
				}
				break;
			}
			}
			framelimiter -= 16;
		}
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		base.GraphicsDevice.SetRenderTarget(backbuffer);
		base.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin();
		switch (currentscreen)
		{
		case 0:
			logomenu.Draw(spriteBatch);
			break;
		case 1:
			startmenu.Draw(spriteBatch);
			break;
		case 2:
			mainmenu.Draw(spriteBatch);
			break;
		case 3:
			setupmenu.Draw(spriteBatch);
			break;
		case 9:
			maingame.Draw(spriteBatch);
			break;
		case 10:
			gameovermenu.Draw(spriteBatch, maingame.Players);
			break;
		case 11:
			highscoremenu.Draw(spriteBatch);
			break;
		case 12:
			sizemenu.Draw(spriteBatch);
			break;
		}
		spriteBatch.End();
		base.GraphicsDevice.SetRenderTarget(null);
		base.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin();
		Rectangle value = new Rectangle(0, 0, 1280, 720);
		Rectangle destinationRectangle = new Rectangle((int)(1280f * ((1f - FileAndGamerServices.savegames.LWCMasterFile.ScreenScale) / 2f)), (int)(720f * ((1f - FileAndGamerServices.savegames.LWCMasterFile.ScreenScale) / 2f)), (int)(1280f * FileAndGamerServices.savegames.LWCMasterFile.ScreenScale), (int)(720f * FileAndGamerServices.savegames.LWCMasterFile.ScreenScale));
		spriteBatch.Draw(backbuffer, destinationRectangle, value, Color.White);
		spriteBatch.End();
		base.Draw(gameTime);
	}
}
