using System;
using DepthAttack.CS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace DepthAttack;

public class Game1 : Game
{
	private const int TargetFrameRate = 60;

	public const int BackBufferWidth = 1280;

	public const int BackBufferHeight = 720;

	public const float pcfltZTani = 1f / 128f;

	public const Buttons ContinueButton = Buttons.A;

	private GraphicsDeviceManager graphics;

	private SpriteBatch spriteBatch;

	public static TitleContent titleContent;

	public static GameMain gameMain;

	public static Record recordContent;

	public static RankIn rankIn;

	public static Continue continueContent;

	public static BGM bGM;

	public static BG bG;

	public static BG02 bG02;

	public static Player player;

	public static PlayerVulcan playerVulcan;

	public static PlayerHoming playerHoming;

	public static CPU00 cPU00;

	public static CPUBOSS00 cPUBOSS00;

	public static CPUPort00 cPUPort00;

	public static CPU_AI00 cPUAI00;

	public static CPUTama cPUTama;

	public static StageChange stageChange;

	public static GameMainClear gameMainClear;

	public static Syougai syougai;

	public static Item item;

	public static Bakuhatu bakuhatu;

	public static Score score;

	public static HPBar hPBar;

	private bool flgStorage = false;

	public static StorageDevice pStorageDevice;

	public Game1()
	{
		graphics = new GraphicsDeviceManager(this);
		base.Content.RootDirectory = "Content";
		titleContent = new TitleContent(this);
		base.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0);
		graphics.PreferredBackBufferWidth = 1280;
		graphics.PreferredBackBufferHeight = 720;
		bGM = new BGM(this);
		bG = new BG(this);
		gameMain = new GameMain(this);
		recordContent = new Record(this);
		rankIn = new RankIn(this);
		bG02 = new BG02(this);
		stageChange = new StageChange(this);
		gameMainClear = new GameMainClear(this);
		player = new Player(this);
		playerVulcan = new PlayerVulcan(this);
		playerHoming = new PlayerHoming(this);
		cPU00 = new CPU00(this);
		cPUBOSS00 = new CPUBOSS00(this);
		cPUPort00 = new CPUPort00(this);
		cPUAI00 = new CPU_AI00(this);
		cPUTama = new CPUTama(this);
		syougai = new Syougai(this);
		bakuhatu = new Bakuhatu(this);
		item = new Item(this);
		score = new Score(this);
		hPBar = new HPBar(this);
		continueContent = new Continue(this);
		recordContent.recordInit();
		base.Components.Add(bGM);
		base.Components.Add(bG);
		base.Components.Add(bG02);
		base.Components.Add(titleContent);
		base.Components.Add(rankIn);
		base.Components.Add(recordContent);
		base.Components.Add(new GamerServicesComponent(this));
	}

	private void CallbackStorageDeviceSelector(IAsyncResult ar)
	{
		pStorageDevice = StorageDevice.EndShowSelector(ar);
		if (pStorageDevice != null)
		{
			recordContent.recordRead();
			continueContent.ContinueRead();
			bGM.volumeRead();
		}
	}

	protected override void Initialize()
	{
		base.Initialize();
	}

	protected override void LoadContent()
	{
		spriteBatch = new SpriteBatch(base.GraphicsDevice);
	}

	protected override void UnloadContent()
	{
	}

	protected override void Update(GameTime gameTime)
	{
		SignedInGamer signedInGamer = Gamer.SignedInGamers[PlayerIndex.One];
		SignedInGamer signedInGamer2 = Gamer.SignedInGamers[PlayerIndex.Two];
		SignedInGamer signedInGamer3 = Gamer.SignedInGamers[PlayerIndex.Three];
		SignedInGamer signedInGamer4 = Gamer.SignedInGamers[PlayerIndex.Four];
		if ((signedInGamer == null || signedInGamer.IsGuest) && (signedInGamer2 == null || signedInGamer2.IsGuest) && (signedInGamer3 == null || signedInGamer3.IsGuest) && (signedInGamer4 == null || signedInGamer4.IsGuest))
		{
			GamePadState state = GamePad.GetState(PlayerIndex.One);
			if (!Guide.IsVisible && state.Buttons.A == ButtonState.Pressed)
			{
				Guide.ShowSignIn(4, onlineOnly: true);
				return;
			}
		}
		if (!flgStorage && (!Guide.IsTrialMode & !Guide.IsVisible))
		{
			StorageDevice.BeginShowSelector(CallbackStorageDeviceSelector, null);
			flgStorage = true;
		}
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		base.GraphicsDevice.Clear(Color.CornflowerBlue);
		base.Draw(gameTime);
	}
}
