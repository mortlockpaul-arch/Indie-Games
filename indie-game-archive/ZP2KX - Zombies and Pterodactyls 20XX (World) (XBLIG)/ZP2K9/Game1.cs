using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using SceneEdit.scene;
using xCharEdit;
using xCharEdit.Character;
using yMapEdit.map;
using yMapEdit.map.postglow;
using yMapEdit.segdef;
using ZP2K9.ai;
using ZP2K9.characters;
using ZP2K9.characters.weapons;
using ZP2K9.debug;
using ZP2K9.hud;
using ZP2K9.map;
using ZP2K9.menu;
using ZP2K9.menu.levels;
using ZP2K9.net;
using ZP2K9.particles;
using ZP2K9.store;

namespace ZP2K9;

public class Game1 : Game
{
	public const int DEF_HUMAN = 0;

	public const int DEF_PTERO = 1;

	public const int DEF_FISH = 2;

	private GraphicsDeviceManager graphics;

	private SpriteBatch spriteBatch;

	private RenderTarget2D rTarg;

	private RenderTarget2D backTarg;

	private RenderTarget2D liteTarg;

	private Effect blurEffect;

	private Effect mainEffect;

	private Effect mainliteEffect;

	public static FlashLight flashLight;

	public static PostGlowManager postGlowMgr;

	public static ZProfile zProfile;

	public static PerkDescriptions perkDescriptions;

	public static BodyCatalog bodyCatalog;

	public static CharTexture[] charTex;

	public static CharTexture[] weapTex;

	public static CharTexture[] pteroTex;

	public static Texture2D jetpacks;

	public static Texture2D zp2kxTex;

	public static Texture2D[] mapTex;

	public static Texture2D iconsTex;

	public static Texture2D nullTex;

	public static Texture2D spritesTex;

	public static Texture2D[] backTex;

	public static Texture2D[] foreBackTex;

	public static Texture2D logoTex;

	public static Texture2D skaLogoTex;

	public static Texture2D controlsTex;

	public static NodeMgr nodeMgr;

	public static Texture2D perksTex;

	public static float frameTime = 0f;

	public static CharDef[] charDef;

	public static Character[] character = new Character[32];

	public static Pterodactyl[] pterodactyl = new Pterodactyl[64];

	public static Fish[] fish = new Fish[32];

	public static Loader loader;

	public static Character rosterChar;

	public static GameMap gameMap;

	public static HUD hud;

	public static InterfaceKeys[] iKeys = new InterfaceKeys[4];

	public static SegDefManager segDefMgr;

	public static ParticleManager pMan;

	public static SceneMgr sceneMgr;

	public static Text text;

	public static SpriteFont impact;

	public static Texture2D badgesTex;

	public static Ticker ticker;

	public static Settings settings;

	public static Store store;

	public static Menu menu;

	public static float gravity = 1500f;

	public static NetSession netSession;

	public static bool needsExit;

	public static int mainPlayerIndex = -1;

	private MainMenu mainMenu;

	private Thread loaderThread;

	public static bool handlingInvite = false;

	public static InviteAcceptedEventArgs ie;

	public static bool inviteHandled = false;

	public static BotBag botBag;

	public Game1()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		((Game)this)._002Ector();
		graphics = new GraphicsDeviceManager((Game)(object)this);
		((Game)this).Content.RootDirectory = "Content";
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)new GamerServicesComponent((Game)(object)this));
		((Game)this).IsFixedTimeStep = false;
	}

	protected override void Initialize()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Rand.rand = new Random();
		ScrollManager.screenSize = new Vector2(1280f, 720f);
		loader = new Loader();
		Special.InitNames();
		WeaponCatalog.Initialize();
		flashLight = new FlashLight();
		postGlowMgr = new PostGlowManager();
		perkDescriptions = new PerkDescriptions();
		graphics.PreferMultiSampling = false;
		graphics.PreferredBackBufferWidth = 1280;
		graphics.PreferredBackBufferHeight = 720;
		graphics.SynchronizeWithVerticalRetrace = true;
		graphics.ApplyChanges();
		bodyCatalog = new BodyCatalog();
		pterodactyl = new Pterodactyl[64];
		for (int i = 0; i < pterodactyl.Length; i++)
		{
			pterodactyl[i] = new Pterodactyl();
		}
		fish = new Fish[32];
		for (int j = 0; j < fish.Length; j++)
		{
			fish[j] = new Fish();
		}
		Numbers.Init();
		Leveling.Init();
		zProfile = new ZProfile();
		zProfile.unlocks.LockAll();
		zProfile.unlocks.UpdateUnlocks();
		botBag = new BotBag();
		((Game)this).Initialize();
	}

	protected override void LoadContent()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		spriteBatch = new SpriteBatch(((Game)this).GraphicsDevice);
		skaLogoTex = ((Game)this).Content.Load<Texture2D>("gfx/skalogo");
		rTarg = new RenderTarget2D(graphics.GraphicsDevice, 1280, 720, 1, (SurfaceFormat)1);
		liteTarg = new RenderTarget2D(graphics.GraphicsDevice, 1280, 720, 1, (SurfaceFormat)1);
		mainMenu = new MainMenu(((Game)this).GraphicsDevice, ((Game)this).Content);
		text = new Text();
		impact = ((Game)this).Content.Load<SpriteFont>("Segoe");
		nullTex = ((Game)this).Content.Load<Texture2D>("gfx/1x1");
		spritesTex = ((Game)this).Content.Load<Texture2D>("gfx/sprites");
		zp2kxTex = ((Game)this).Content.Load<Texture2D>("gfx/zp2kx");
		ticker = new Ticker();
	}

	private void Load()
	{
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 5 });
		charDef = new CharDef[16];
		segDefMgr = new SegDefManager();
		for (int i = 0; i < 3; i++)
		{
			charDef[i] = new CharDef();
		}
		nodeMgr = new NodeMgr();
		settings = new Settings();
		netSession = new NetSession();
		charDef[0].path = "charDef/civ";
		charDef[0].Read();
		charDef[1].path = "charDef/ptero";
		charDef[1].Read();
		charDef[2].path = "charDef/fish";
		charDef[2].Read();
		for (int j = 0; j < iKeys.Length; j++)
		{
			iKeys[j] = new InterfaceKeys();
		}
		Sound.Initialize();
		Music.Init();
		hud = new HUD();
		gameMap = new GameMap(segDefMgr);
		pMan = new ParticleManager();
		store = new Store();
		menu = new Menu();
		menu.menuLevel[13].active = true;
		GameState.mode = 2;
		backTarg = new RenderTarget2D(graphics.GraphicsDevice, 640, 360, 1, (SurfaceFormat)1);
		blurEffect = ((Game)this).Content.Load<Effect>("fx/blur");
		mainEffect = ((Game)this).Content.Load<Effect>("fx/main");
		mainliteEffect = ((Game)this).Content.Load<Effect>("fx/mainlite");
		controlsTex = ((Game)this).Content.Load<Texture2D>("gfx/controls");
		int num = 48;
		charTex = new CharTexture[num * 2];
		weapTex = new CharTexture[5];
		pteroTex = new CharTexture[2];
		badgesTex = ((Game)this).Content.Load<Texture2D>("gfx/badges");
		sceneMgr = new SceneMgr(((Game)this).Content);
		sceneMgr.Read("data/scenes/main.zcx");
		perksTex = ((Game)this).Content.Load<Texture2D>("gfx/perks");
		logoTex = ((Game)this).Content.Load<Texture2D>("gfx/logo");
		for (int k = 0; k < num; k++)
		{
			charTex[k * 2] = new CharTexture("human", k, 0, ((Game)this).Content, game: true);
			charTex[k * 2 + 1] = new CharTexture("zombie", k, 0, ((Game)this).Content, game: true);
		}
		for (int l = 0; l < weapTex.Length; l++)
		{
			weapTex[l] = new CharTexture("weap", l, 0, ((Game)this).Content, game: true);
		}
		for (int m = 0; m < pteroTex.Length; m++)
		{
			pteroTex[m] = new CharTexture("ptero", m, m, ((Game)this).Content, game: true);
		}
		backTex = (Texture2D[])(object)new Texture2D[10];
		backTex[0] = ((Game)this).Content.Load<Texture2D>("gfx/maps/back0");
		backTex[1] = ((Game)this).Content.Load<Texture2D>("gfx/maps/foreback");
		backTex[2] = ((Game)this).Content.Load<Texture2D>("gfx/maps/cityback");
		backTex[3] = ((Game)this).Content.Load<Texture2D>("gfx/maps/cityback2");
		backTex[4] = ((Game)this).Content.Load<Texture2D>("gfx/maps/mtnback");
		backTex[5] = ((Game)this).Content.Load<Texture2D>("gfx/maps/mtnback2");
		backTex[6] = ((Game)this).Content.Load<Texture2D>("gfx/maps/pinkback");
		backTex[7] = ((Game)this).Content.Load<Texture2D>("gfx/maps/pinkback2");
		backTex[8] = ((Game)this).Content.Load<Texture2D>("gfx/maps/lemon");
		backTex[9] = ((Game)this).Content.Load<Texture2D>("gfx/maps/lemon2");
		jetpacks = ((Game)this).Content.Load<Texture2D>("gfx/chars/jetpacks");
		segDefMgr.Read("map/data/segdef.zdx");
		int num2 = 5;
		mapTex = (Texture2D[])(object)new Texture2D[num2];
		for (int n = 0; n < num2; n++)
		{
			mapTex[n] = ((Game)this).Content.Load<Texture2D>("gfx/maps/maps" + (n + 1));
		}
		iconsTex = ((Game)this).Content.Load<Texture2D>("gfx/icons");
		MapList.Init();
		StartServer startServer = (StartServer)menu.menuLevel[11];
		startServer.SetMapList();
		gameMap.Read(new BinaryReader(File.Open("map/data/" + MapList.mapCatalog[MapList.maplist[0]].path + ".zkx", FileMode.Open, FileAccess.Read)));
		nodeMgr.Refresh(gameMap);
		rosterChar = new Character(0, 0, default(Vector2));
		loader.loadComplete = true;
	}

	protected override void UnloadContent()
	{
	}

	protected override void Update(GameTime gameTime)
	{
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Invalid comparison between Unknown and I4
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Invalid comparison between Unknown and I4
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		frameTime = (float)gameTime.ElapsedGameTime.Milliseconds / 1000f;
		postGlowMgr.Update();
		if (handlingInvite && !Guide.IsVisible)
		{
			FinishHandleInvite();
		}
		if (loader.splashFrame > 1f && !loader.loadBegin)
		{
			loader.loadBegin = true;
			loaderThread = new Thread(Load);
			loaderThread.Start();
		}
		if (loader.IsDone() && !inviteHandled)
		{
			NetworkSession.InviteAccepted += HandleInvite;
			inviteHandled = true;
		}
		if (!loader.IsDone())
		{
			loader.Update();
		}
		else
		{
			store.Update();
			netSession.Update(character);
			if (mainPlayerIndex > -1 && Gamer.SignedInGamers[(PlayerIndex)mainPlayerIndex] == null)
			{
				mainPlayerIndex = -1;
				if (netSession.netSession != null)
				{
					netSession.netSession.Dispose();
					while (!netSession.netSession.IsDisposed)
					{
					}
					netSession.netSession = null;
				}
				GameState.mode = 2;
				menu.Close();
				menu.menuLevel[13].active = true;
			}
			if (mainPlayerIndex < 0)
			{
				if (!Guide.IsVisible)
				{
					for (int i = 0; i < 4; i++)
					{
						GamePadState state = GamePad.GetState((PlayerIndex)i);
						GamePadButtons buttons = ((GamePadState)(ref state)).Buttons;
						if ((int)((GamePadButtons)(ref buttons)).A != 1)
						{
							GamePadState state2 = GamePad.GetState((PlayerIndex)i);
							GamePadButtons buttons2 = ((GamePadState)(ref state2)).Buttons;
							if ((int)((GamePadButtons)(ref buttons2)).Start != 1)
							{
								continue;
							}
						}
						if (Gamer.SignedInGamers[(PlayerIndex)i] != null)
						{
							mainPlayerIndex = i;
							store.GetDevice();
							menu.menuLevel[13].active = false;
							menu.menuLevel[0].active = true;
							try
							{
								Guide.NotificationPosition = (NotificationPosition)8;
							}
							catch
							{
							}
						}
						else
						{
							Guide.ShowSignIn(1, false);
						}
					}
				}
			}
			else
			{
				GamePad.GetState((PlayerIndex)mainPlayerIndex, (GamePadDeadZone)0);
				iKeys[0].Update(GamePad.GetState((PlayerIndex)mainPlayerIndex));
				if (GameState.mode == 1)
				{
					hud.Update(iKeys[0], character[netSession.GetPlayerOne()]);
				}
			}
			menu.Update(iKeys[0]);
			if (GameState.mode == 2)
			{
				ticker.Update();
			}
			Sound.Update();
			Music.Update();
			zProfile.second += frameTime;
			if (zProfile.second > 1f)
			{
				zProfile.second--;
				zProfile.time++;
			}
			if (GameState.mode == 0)
			{
				if (iKeys[0].keySelect)
				{
					netSession.netType = 1;
					nodeMgr.Refresh(gameMap);
					GameState.mode = 1;
					for (int j = 0; j < 8; j++)
					{
						character[j] = new Character(j, (j != 0) ? (-1) : 0, new Vector2(300f, 300f));
						gameMap.GetSpawn(0, character[j]);
					}
				}
			}
			else if (GameState.mode == 1)
			{
				if (netSession.netType == 1 && iKeys[0].keySelect)
				{
					GameState.mode = 0;
				}
				if (iKeys[0].keyStart)
				{
					if (menu.IsActive())
					{
						menu.Close();
					}
					else
					{
						menu.menuLevel[9].active = true;
					}
				}
			}
			if (GameState.mode == 1)
			{
				if (!Music.playing)
				{
					Music.playing = true;
					Music.Reset();
				}
				int playerOne = netSession.GetPlayerOne();
				flashLight.active = false;
				if (playerOne < character.Length && character[playerOne] != null)
				{
					bool flag = false;
					if (character[playerOne].hp < 0)
					{
						hud.red = 1f;
						if (character[playerOne].lastHitBy > -1 && character[playerOne].lastHitBy < character.Length && character[character[playerOne].lastHitBy] != null && character[playerOne].dyingFrame > 2f)
						{
							character[playerOne].loc = character[character[playerOne].lastHitBy].loc;
							flag = true;
						}
					}
					else
					{
						hud.red = 0f;
						if (character[playerOne].hp < 100)
						{
							hud.red = (float)(100 - character[playerOne].hp) / 100f;
						}
					}
					Vector2 val = character[playerOne].loc;
					if (character[playerOne].spawnFrame > 0f)
					{
						float num = character[playerOne].spawnFrame / 2f;
						val += new Vector2(num * 300f * ((character[playerOne].face == 1) ? (-1f) : 1f), (float)Math.Pow(num * 10f, 2.0) * -1f);
					}
					if (hud.IsPopupActive())
					{
						val.Y -= 70f;
					}
					Vector2 val2 = Scroll.scroll - (val + character[playerOne].traj * 0.05f + character[playerOne].charKeys.shootVec * 50f);
					if (((Vector2)(ref val2)).LengthSquared() > 100f)
					{
						Scroll.scroll -= val2 * frameTime * 10f;
					}
					float num2 = ((Vector2)(ref character[playerOne].traj)).Length() / 500f;
					if (num2 > 1f)
					{
						num2 = 1f;
					}
					float num3 = 1f - num2 / 6f;
					num3 *= 1.1f;
					if (character[playerOne].weapon[character[playerOne].curWeap] > -1)
					{
						try
						{
							if (WeaponCatalog.weapons[character[playerOne].weapon[character[playerOne].curWeap]].type == 5)
							{
								num3 *= 1.25f;
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.StackTrace);
						}
					}
					if (flag)
					{
						num3 = 2f;
					}
					if (GameState.gameType == 4 && character[playerOne].team == 0 && character[playerOne].dyingFrame <= 0f)
					{
						flashLight.active = true;
					}
					if (character[playerOne].shrink > 0f)
					{
						num3 = 5f;
					}
					Scroll.zoom += (num3 - Scroll.zoom) * frameTime * ((num3 > Scroll.zoom) ? 0.5f : 3f);
					if (Scroll.scroll.Y > 7040f)
					{
						Scroll.scroll.Y = 7040f;
					}
					if (Scroll.scroll.Y < 256f)
					{
						Scroll.scroll.Y = 256f;
					}
					if (Scroll.scroll.X < 512f)
					{
						Scroll.scroll.X = 512f;
					}
					if (Scroll.scroll.X > 15872f)
					{
						Scroll.scroll.X = 15872f;
					}
				}
			}
			else
			{
				Music.playing = false;
			}
			mainMenu.active = GameState.mode == 2;
			mainMenu.Update();
			rosterChar.respawnFrame = 0f;
			rosterChar.bodySec[0].Update(rosterChar, frameTime);
			if (GameState.mode == 1)
			{
				pMan.Update(gameMap, character);
				for (int k = 0; k < character.Length; k++)
				{
					if (character[k] != null)
					{
						character[k].Update(gameMap, character, frameTime);
					}
				}
				if (DebugManager.jumpToNullMe && character[0] != null && character[0].charKeys.keyJump)
				{
					character[0] = null;
				}
				for (int l = 0; l < pterodactyl.Length; l++)
				{
					if (pterodactyl[l].exists)
					{
						pterodactyl[l].Update();
					}
				}
				for (int m = 0; m < fish.Length; m++)
				{
					if (fish[m].exists)
					{
						fish[m].Update(character[m]);
					}
				}
				if (flashLight.active)
				{
					flashLight.Update();
				}
				pMan.NetCleanup(netSession.GetPlayerOne());
				pMan.ResetChronos();
				gameMap.Update();
				Quake.UpdateScroll();
			}
			Quake.UpdateQuake();
			if (needsExit)
			{
				((Game)this).Exit();
			}
		}
		((Game)this).Update(gameTime);
	}

	public void HandleInvite(object sender, EventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		handlingInvite = true;
		ie = (InviteAcceptedEventArgs)e;
	}

	internal static void DestroyChar(int i)
	{
		character[i] = null;
	}

	public void FinishHandleInvite()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected I4, but got Unknown
		int num = mainPlayerIndex;
		mainPlayerIndex = (int)ie.Gamer.PlayerIndex;
		if (mainPlayerIndex != num)
		{
			store.GetDevice();
		}
		GameState.mode = 2;
		menu.Close();
		menu.menuLevel[4].active = true;
		netSession.JoinInvite(ie);
		menu.menuLevel[4] = new Lobby(host: false);
		menu.menuLevel[4].active = true;
		handlingInvite = false;
	}

	protected override void Draw(GameTime gameTime)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_08be: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		postGlowMgr.Reset();
		((Game)this).GraphicsDevice.Clear(Color.Black);
		if (!loader.IsDone())
		{
			loader.Draw(spriteBatch);
		}
		else
		{
			mainMenu.Prepare(spriteBatch, graphics.GraphicsDevice);
			if (!mainMenu.IsSolid())
			{
				graphics.GraphicsDevice.SetRenderTarget(0, backTarg);
				graphics.GraphicsDevice.Clear(Color.Black);
				spriteBatch.Begin((SpriteBlendMode)1);
				gameMap.Draw(spriteBatch, 0, 2, nullTex, mapTex, backTex, 0.5f);
				spriteBatch.End();
				graphics.GraphicsDevice.SetRenderTarget(0, rTarg);
				graphics.GraphicsDevice.Clear(Color.Black);
				blurEffect.Parameters["v"].SetValue(0.005f);
				blurEffect.Parameters["briteGradientR"].SetValue(0.2f);
				blurEffect.Parameters["briteGradientR"].SetValue(0.15f);
				blurEffect.Parameters["briteGradientG"].SetValue(0.1f);
				blurEffect.Begin();
				spriteBatch.Begin((SpriteBlendMode)2, (SpriteSortMode)0, (SaveStateMode)1);
				blurEffect.CurrentTechnique.Passes[0].Begin();
				spriteBatch.Draw(backTarg.GetTexture(), new Rectangle(0, 0, 1280, 720), Color.White);
				blurEffect.CurrentTechnique.Passes[0].End();
				spriteBatch.End();
				blurEffect.End();
				spriteBatch.Begin((SpriteBlendMode)1);
				gameMap.Draw(spriteBatch, 2, 3, nullTex, mapTex, backTex, 1f);
				if (GameState.mode == 1)
				{
					for (int i = 0; i < character.Length; i++)
					{
						if (character[i] == null)
						{
							continue;
						}
						try
						{
							if (character[i].spawnFrame <= 0f)
							{
								character[i].Draw(spriteBatch);
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.StackTrace);
						}
					}
				}
				pMan.Draw(spriteBatch, alpha: false);
				gameMap.DrawEntities(spriteBatch, spritesTex);
				spriteBatch.End();
				spriteBatch.Begin((SpriteBlendMode)2);
				pMan.Draw(spriteBatch, alpha: true);
				if (DebugManager.showAIDest)
				{
					gameMap.DrawAIPaths(character, spriteBatch);
				}
				spriteBatch.End();
				spriteBatch.Begin((SpriteBlendMode)1);
				gameMap.Draw(spriteBatch, 3, 5, nullTex, mapTex, backTex, 1f);
				for (int j = 0; j < character.Length; j++)
				{
					if (character[j] != null && character[j].spawnFrame > 0f)
					{
						character[j].Draw(spriteBatch);
					}
				}
				for (int k = 0; k < pterodactyl.Length; k++)
				{
					if (pterodactyl[k].exists)
					{
						pterodactyl[k].Draw(spriteBatch);
					}
				}
				for (int l = 0; l < fish.Length; l++)
				{
					if (fish[l].exists)
					{
						fish[l].Draw(spriteBatch);
					}
				}
				spriteBatch.End();
				spriteBatch.Begin((SpriteBlendMode)2);
				if (DebugManager.showAIPaths)
				{
					for (int m = 0; m < character.Length; m++)
					{
						if (character[m] != null)
						{
							character[m].DrawPaths(spriteBatch);
						}
					}
				}
				postGlowMgr.Draw(spriteBatch, spritesTex);
				spriteBatch.End();
				float num = hud.red * 0.8f;
				if (character[netSession.GetPlayerOne()] != null && character[netSession.GetPlayerOne()].hp < 0)
				{
					num = 1f;
				}
				if (flashLight.active)
				{
					graphics.GraphicsDevice.SetRenderTarget(0, liteTarg);
					graphics.GraphicsDevice.Clear(Color.Black);
					flashLight.Draw(spriteBatch);
					graphics.GraphicsDevice.SetRenderTarget(0, (RenderTarget2D)null);
					graphics.GraphicsDevice.Clear(Color.Black);
					graphics.GraphicsDevice.Textures[1] = (Texture)(object)liteTarg.GetTexture();
					if (num < 0.7f)
					{
						num = 0.7f;
					}
					mainliteEffect.Parameters["red"].SetValue(hud.red * 3f);
					mainliteEffect.Parameters["gray"].SetValue(num);
					mainliteEffect.Begin();
					spriteBatch.Begin((SpriteBlendMode)2, (SpriteSortMode)0, (SaveStateMode)1);
					mainliteEffect.CurrentTechnique.Passes[0].Begin();
					spriteBatch.Draw(rTarg.GetTexture(), new Rectangle(0, 0, 1280, 720), Color.White);
					mainliteEffect.CurrentTechnique.Passes[0].End();
					spriteBatch.End();
					mainliteEffect.End();
					graphics.GraphicsDevice.Textures[1] = null;
				}
				else
				{
					graphics.GraphicsDevice.SetRenderTarget(0, (RenderTarget2D)null);
					graphics.GraphicsDevice.Clear(Color.Black);
					mainEffect.Parameters["red"].SetValue(hud.red * 3f);
					mainEffect.Parameters["gray"].SetValue(num);
					if (character[netSession.GetPlayerOne()] != null)
					{
						if (GameState.gameType == 4 && character[netSession.GetPlayerOne()].team == 1)
						{
							mainEffect.Parameters["tR"].SetValue(1f);
							mainEffect.Parameters["tG"].SetValue(0.5f);
							mainEffect.Parameters["tB"].SetValue(0.5f);
							mainEffect.Parameters["bR"].SetValue(0.6f);
							mainEffect.Parameters["bG"].SetValue(0.6f);
							mainEffect.Parameters["bB"].SetValue(0.6f);
						}
						else
						{
							mainEffect.Parameters["tR"].SetValue(gameMap.tR);
							mainEffect.Parameters["tG"].SetValue(gameMap.tG);
							mainEffect.Parameters["tB"].SetValue(gameMap.tB);
							mainEffect.Parameters["bR"].SetValue(gameMap.bR);
							mainEffect.Parameters["bG"].SetValue(gameMap.bG);
							mainEffect.Parameters["bB"].SetValue(gameMap.bB);
						}
					}
					mainEffect.Begin();
					spriteBatch.Begin((SpriteBlendMode)2, (SpriteSortMode)0, (SaveStateMode)1);
					mainEffect.CurrentTechnique.Passes[0].Begin();
					spriteBatch.Draw(rTarg.GetTexture(), new Rectangle(0, 0, 1280, 720), Color.White);
					mainEffect.CurrentTechnique.Passes[0].End();
					spriteBatch.End();
					mainEffect.End();
				}
				if (GameState.mode == 1)
				{
					spriteBatch.Begin((SpriteBlendMode)1);
					hud.Draw(character[netSession.GetPlayerOne()], spriteBatch);
					spriteBatch.End();
				}
			}
			mainMenu.Draw(spriteBatch);
			menu.Draw(spriteBatch);
		}
		((Game)this).Draw(gameTime);
	}
}
