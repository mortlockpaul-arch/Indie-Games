using System.Collections.ObjectModel;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OluXNA;

public class MainMenuComponent : DrawableGameComponent
{
	private Texture2D grayFilter;

	private Texture2D menuSlide;

	private ModelWrapper[] face;

	private ModelWrapper grid;

	private Menu mainMenu;

	private Menu playModeMenu;

	private StretchTex pauseWindow;

	private float faceRotate;

	private float gridRotate;

	private float textFlash;

	private float flashInc;

	private int state;

	private float stateChange;

	private Vector3 offset;

	private Vector3 offset2;

	private Vector3 offsetFinal;

	private Vector3 offsetFinal2;

	private Vector3 curOffset;

	private Vector3 curOffset2;

	private bool tentativeSave;

	private ModelWrapper koko;

	private string dirText = "";

	private bool SaveLocationSelected;

	private bool GlobSaveLocationSelected;

	private bool checkSignIn;

	private Thread chooseLocation;

	private Vector3 particleSource;

	private float partDelay = 0.01f;

	private float curPart;

	private int numPart = 5;

	private float musCountdown;

	private float musMax;

	private int curMusCue;

	private string[] cues;

	private MenuItem currentMenuItem
	{
		get
		{
			if (state != 2)
			{
				if (state != 4)
				{
					if (state != 6)
					{
						if (state != 10)
						{
							return null;
						}
						return BaseGame.Get().optionMenu.ActiveItem;
					}
					return BaseGame.Get().levelMenu.ActiveItem;
				}
				return playModeMenu.ActiveItem;
			}
			return mainMenu.ActiveItem;
		}
	}

	private bool addParticles
	{
		get
		{
			if (state != 2 && state != 4 && state != 6)
			{
				return state == 10;
			}
			return true;
		}
	}

	private float genTime => partDelay / (float)numPart;

	public MainMenuComponent(Game game)
		: this(game, 0)
	{
	}

	public MainMenuComponent(Game game, int _state)
		: base(game)
	{
		face = new ModelWrapper[2];
		faceRotate = (gridRotate = 0f);
		state = _state;
		textFlash = 0f;
		flashInc = 1.2f;
		stateChange = 0f;
		BaseGame.Get().ReloadLevelObj();
		BaseGame.Get().ps = new ParticleSystem();
		if (BaseGame.Get().storageDevice != null && BaseGame.Get().storageDevice.IsConnected)
		{
			SaveLocationSelected = true;
		}
		if (BaseGame.Get().globStorageDevice != null && BaseGame.Get().globStorageDevice.IsConnected)
		{
			GlobSaveLocationSelected = true;
		}
		if (BaseGame.Get().continueWithoutSaving)
		{
			SaveLocationSelected = (GlobSaveLocationSelected = true);
		}
		checkSignIn = false;
		musCountdown = 0f;
		musMax = 10f;
		curMusCue = 1;
		cues = new string[3] { "Menu01", "Menu02", "Menu03" };
	}

	protected override void LoadContent()
	{
		BaseGame.Get().ps.LoadGraphics();
		((DrawableGameComponent)this).LoadContent();
	}

	public override void Initialize()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		Color green = Color.Green;
		Color white = Color.White;
		BaseGame.Get().StopAndClearBGCues();
		grayFilter = new Texture2D(BaseGame.Get().graphics.GraphicsDevice, 1, 1, 1, (TextureUsage)0, (SurfaceFormat)1);
		grayFilter.SetData<Color>((Color[])(object)new Color[1]
		{
			new Color(new Vector4(0.2f, 0.2f, 0.2f, 0.25f))
		});
		menuSlide = BaseGame.Get().content.Load<Texture2D>("Content\\menuSlide");
		pauseWindow = new StretchTex();
		pauseWindow.Initialize(9, 12, 9, 12, "Content\\WindowTex");
		face[0] = BaseGame.Get().models.GetModel("Content\\Olu\\Olu");
		BaseGame.SetAllEPCs(face[0].epc, "xEnableLighting", false);
		BaseGame.SetAllEPCs(face[0].epc, "DirLight0Direction", Vector3.Normalize(new Vector3(0.5f, 0.5f, -1f)));
		BaseGame.Get().LinkEffect(face[0].model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		face[1] = BaseGame.Get().models.GetModel("Content\\Olu\\OluBack");
		BaseGame.SetAllEPCs(face[1].epc, "xEnableLighting", false);
		BaseGame.Get().LinkEffect(face[1].model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.Get().fogEffect);
		koko = BaseGame.Get().models.GetModel("Content\\Kokopelli_graphic\\Kokopelli_Flat");
		BaseGame.SetAllEPCs(koko.epc, "xEnableLighting", false);
		BaseGame.Get().LinkEffect(koko.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.Get().fogEffect);
		grid = BaseGame.Get().models.GetModel("Content\\Level01Background01x");
		BaseGame.SetAllEPCs(grid.epc, "xEnableLighting", false);
		BaseGame.Get().LinkEffect(grid.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.Get().fogEffect);
		mainMenu = new Menu();
		mainMenu.Add("[- Play Game -]", new Vector2((float)(BaseGame.WIDTH / 8), (float)(3 * BaseGame.HEIGHT / 8)), green, white, "play");
		mainMenu.Add("[- Options -]", new Vector2((float)(BaseGame.WIDTH / 8), (float)(4 * BaseGame.HEIGHT / 8)), green, white, "option");
		mainMenu.Add("[- Exit -]", new Vector2((float)(BaseGame.WIDTH / 8), (float)(5 * BaseGame.HEIGHT / 8)), green, white, "exit");
		playModeMenu = new Menu();
		playModeMenu.Add("[- Simple -]", new Vector2((float)(BaseGame.WIDTH / 8), (float)(2 * BaseGame.HEIGHT / 9)), green, white, "simple");
		playModeMenu.Add("[- Complex -]", new Vector2((float)(7 * BaseGame.WIDTH / 8), (float)(7 * BaseGame.HEIGHT / 9)) - BaseGame.Get().hud.HUDfont.MeasureString("[- Complex -]"), green, white, "complex");
		Guide.SimulateTrialMode = false;
		BaseGame.Get().TrialModeSettings(Guide.IsTrialMode);
		curOffset = (offset = new Vector3(11f, 0f, 14f));
		curOffset2 = (offset2 = new Vector3(-7f, 0f, 0f));
		offsetFinal = new Vector3(4f, 0f, 14f);
		offsetFinal2 = new Vector3(-2f, 0f, 0f);
		tentativeSave = false;
		((DrawableGameComponent)this).Initialize();
	}

	public override void Update(GameTime gameTime)
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Expected O, but got Unknown
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Expected O, but got Unknown
		//IL_09ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a05: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a32: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a43: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a48: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aaa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0add: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b04: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d88: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d92: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().input.Update();
		BaseGame.Get().ps.Update(gameTime);
		BaseGame.Get().CheckAndResetRumble();
		faceRotate += (float)gameTime.ElapsedGameTime.TotalSeconds;
		gridRotate += (float)gameTime.ElapsedGameTime.TotalSeconds;
		textFlash += flashInc * (float)gameTime.ElapsedGameTime.TotalSeconds;
		if (textFlash >= 1f)
		{
			flashInc = -1.2f;
		}
		else if (textFlash <= 0f)
		{
			flashInc = 1.2f;
		}
		if (addParticles)
		{
			particleSource = Vector3.Lerp(particleSource, new Vector3(currentMenuItem.position + new Vector2(-60f, 16f), 0f), 4f * (float)gameTime.ElapsedGameTime.TotalSeconds);
			curPart -= (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (curPart < 0f)
			{
				curPart += partDelay;
				BaseGame.Get().ps.AddParticlesFlat(particleSource, new Vector3(50f, 0f, 0f), 0.2f, 180f, Vector3.Zero, 0f, 1.2f, 0.2f, 0f, new Vector4(1f, 1f, 1f, 1f), numPart, genTime, 1f);
			}
		}
		musCountdown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
		if (musCountdown < 0f)
		{
			int num = BaseGame.Get().r.Next(2);
			if (num >= curMusCue)
			{
				num++;
			}
			BaseGame.Get().PlayCue(cues[num]);
			curMusCue = num;
			musCountdown += musMax;
		}
		if (((GameComponent)this).Game.IsActive)
		{
			if (BaseGame.Get().input.PadPressed((Buttons)32) || BaseGame.Get().input.KeyPressed((Keys)27))
			{
				((GameComponent)this).Game.Exit();
			}
			if (BaseGame.Get().input.PadDown((Buttons)32768) && BaseGame.Get().input.PadDown((Buttons)8192) && BaseGame.Get().input.PadDown((Buttons)512) && BaseGame.Get().input.PadPressed((Buttons)256))
			{
				BaseGame.Get().debug = !BaseGame.Get().debug;
			}
			if (state == 0)
			{
				if (BaseGame.Get().input.KeyPressed((Keys)13) || BaseGame.Get().input.SetPlayerIndex() || (checkSignIn && (Gamer.SignedInGamers[BaseGame.Get().input.playerIndex] != null || BaseGame.Get().continueWithoutSaving)))
				{
					if (Gamer.SignedInGamers[BaseGame.Get().input.playerIndex] != null || BaseGame.Get().continueWithoutSaving)
					{
						if (!Guide.IsVisible || BaseGame.Get().continueWithoutSaving)
						{
							if (BaseGame.Get().continueWithoutSaving)
							{
								SaveLocationSelected = true;
								GlobSaveLocationSelected = true;
							}
							if (!SaveLocationSelected)
							{
								state = 111;
								chooseLocation = new Thread((ThreadStart)delegate
								{
									//IL_0019: Unknown result type (might be due to invalid IL or missing references)
									//IL_001f: Expected O, but got Unknown
									foreach (GameComponent item in (Collection<IGameComponent>)(object)((GameComponent)this).Game.Components)
									{
										GameComponent val3 = item;
										if (!(val3 is GamerServicesComponent))
										{
											val3.Enabled = false;
										}
									}
									((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Add((IGameComponent)(object)new VerifySaveLocationComponent(((GameComponent)this).Game, "Choose a storage device or continue without saving", IOModes.LoadPlayer));
								});
								chooseLocation.Start();
							}
							else if (!GlobSaveLocationSelected)
							{
								state = 112;
								chooseLocation = new Thread((ThreadStart)delegate
								{
									//IL_0019: Unknown result type (might be due to invalid IL or missing references)
									//IL_001f: Expected O, but got Unknown
									foreach (GameComponent item2 in (Collection<IGameComponent>)(object)((GameComponent)this).Game.Components)
									{
										GameComponent val3 = item2;
										if (!(val3 is GamerServicesComponent))
										{
											val3.Enabled = false;
										}
									}
									((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Add((IGameComponent)(object)new VerifySaveLocationComponent(((GameComponent)this).Game, "Choose a storage device for high scores or continue without saving", IOModes.LoadHS));
								});
								chooseLocation.Start();
							}
							else
							{
								state = 1;
								stateChange = 0f;
							}
						}
					}
					else
					{
						foreach (GameComponent item3 in (Collection<IGameComponent>)(object)((GameComponent)this).Game.Components)
						{
							GameComponent val = item3;
							if (!(val is GamerServicesComponent))
							{
								val.Enabled = false;
							}
						}
						((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Add((IGameComponent)(object)new SignBackInComponent(((GameComponent)this).Game, "", "Sign in or continue without saving"));
						checkSignIn = true;
					}
				}
			}
			else if (state == 111)
			{
				if (!SaveLocationSelected && (BaseGame.Get().PlayerLoad || BaseGame.Get().continueWithoutSaving))
				{
					SaveLocationSelected = true;
				}
				if (((GameComponent)this).Game.IsActive && SaveLocationSelected)
				{
					foreach (GameComponent item4 in (Collection<IGameComponent>)(object)((GameComponent)this).Game.Components)
					{
						GameComponent val2 = item4;
						if (!(val2 is GamerServicesComponent))
						{
							val2.Enabled = false;
						}
					}
					((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Add((IGameComponent)(object)new VerifySaveLocationComponent(((GameComponent)this).Game, "Choose a storage device for high scores or continue without saving", IOModes.LoadHS));
					state = 112;
				}
			}
			else if (state == 112)
			{
				if (!GlobSaveLocationSelected && (BaseGame.Get().HSLoad || BaseGame.Get().continueWithoutSaving))
				{
					GlobSaveLocationSelected = true;
				}
				if (SaveLocationSelected && GlobSaveLocationSelected)
				{
					state = 1;
					stateChange = 0f;
					BaseGame.Get().PlayCue("clap_2");
				}
			}
			else if (state == 1)
			{
				if (SaveLocationSelected && GlobSaveLocationSelected)
				{
					faceRotate += 9f * (float)gameTime.ElapsedGameTime.TotalSeconds;
					gridRotate += 9f * (float)gameTime.ElapsedGameTime.TotalSeconds;
					stateChange += (float)gameTime.ElapsedGameTime.TotalSeconds;
					if (stateChange > 1f && SaveLocationSelected && GlobSaveLocationSelected)
					{
						state = 2;
					}
				}
			}
			else if (state == 2)
			{
				curOffset = offset;
				curOffset2 = offset2;
				if (BaseGame.Get().input.DirectionUp())
				{
					mainMenu.MoveUp();
				}
				if (BaseGame.Get().input.DirectionDown())
				{
					mainMenu.MoveDown();
				}
				if (BaseGame.Get().input.Select())
				{
					if (mainMenu.ActiveItem.command == "play")
					{
						if (!BaseGame.demo)
						{
							state = 3;
							BaseGame.Get().PlayCue("clap_2");
						}
						if (BaseGame.demo)
						{
							state = 7;
							BaseGame.Get().PlayCue("clap_2");
							BaseGame.Get().loadThread = new Thread((ThreadStart)delegate
							{
								BaseGame.Get().ReloadLevelGraphics("Levels//LevelOne.xml");
							});
							BaseGame.Get().loadThread.Start();
						}
						stateChange = 0f;
					}
					else if (mainMenu.ActiveItem.command == "option")
					{
						state = 9;
						stateChange = 0f;
						BaseGame.Get().PlayCue("clap_2");
					}
					else if (mainMenu.ActiveItem.command == "exit")
					{
						BaseGame.Get().PlayCue("hat_4");
						((GameComponent)this).Game.Exit();
					}
				}
			}
			else if (state == 3)
			{
				state = 4;
			}
			else if (state == 35)
			{
				state = 2;
			}
			else if (state == 4)
			{
				if (playModeMenu.ActiveItem.command == "simple")
				{
					dirText = "Player is invincible\nNo score";
				}
				else if (playModeMenu.ActiveItem.command == "complex")
				{
					dirText = "Player can take damage\nAwarded score";
				}
				if (BaseGame.Get().input.DirectionUp())
				{
					playModeMenu.MoveUp();
				}
				if (BaseGame.Get().input.DirectionDown())
				{
					playModeMenu.MoveDown();
				}
				if (BaseGame.Get().input.Select())
				{
					if (playModeMenu.ActiveItem.command == "exit")
					{
						state = 35;
						stateChange = 1f;
						BaseGame.Get().PlayCue("hat_4");
					}
					else
					{
						if (playModeMenu.ActiveItem.command == "simple")
						{
							BaseGame.Get().EasyMode = true;
						}
						else
						{
							BaseGame.Get().EasyMode = false;
						}
						state = 5;
						BaseGame.Get().PlayCue("clap_2");
						stateChange = 0f;
					}
				}
				if (BaseGame.Get().input.PadPressed((Buttons)8192))
				{
					state = 35;
					stateChange = 1f;
					BaseGame.Get().PlayCue("hat_4");
				}
			}
			else if (state == 5)
			{
				curOffset = offset * (1f - stateChange) + offsetFinal * stateChange;
				curOffset2 = offset2 * (1f - stateChange) + offsetFinal2 * stateChange;
				stateChange += (float)gameTime.ElapsedGameTime.TotalSeconds;
				if (stateChange > 1f)
				{
					BaseGame.Get().levelMenu.activeItem = 0;
					state = 6;
				}
			}
			else if (state == 55)
			{
				curOffset = offset * (1f - stateChange) + offsetFinal * stateChange;
				curOffset2 = offset2 * (1f - stateChange) + offsetFinal2 * stateChange;
				stateChange -= (float)gameTime.ElapsedGameTime.TotalSeconds;
				if (stateChange < 0f)
				{
					state = 2;
				}
			}
			else if (state == 6)
			{
				if (BaseGame.Get().input.DirectionUp())
				{
					BaseGame.Get().levelMenu.MoveUp();
				}
				if (BaseGame.Get().input.DirectionDown())
				{
					BaseGame.Get().levelMenu.MoveDown();
				}
				if (BaseGame.Get().input.Select())
				{
					if (BaseGame.Get().levelMenu.ActiveItem.command == "exit")
					{
						state = 55;
						stateChange = 1f;
						BaseGame.Get().PlayCue("hat_4");
					}
					else
					{
						state = 7;
						BaseGame.Get().PlayCue("clap_2");
						stateChange = 0f;
						BaseGame.Get().loadThread = new Thread((ThreadStart)delegate
						{
							string[] array = BaseGame.Get().levelMenu.ActiveItem.command.Split(' ');
							if (array.Length > 1)
							{
								BaseGame.Get().ReloadLevelGraphics(array[0], int.Parse(array[1]));
								GameplayChange enem = new GameplayChange("unfade", 3f);
								BaseGame.Get().eQueue.PushAtFront(new EnemyQueuePart(enem, 0f));
							}
							else
							{
								BaseGame.Get().ReloadLevelGraphics(array[0]);
							}
						});
						BaseGame.Get().loadThread.Start();
					}
				}
				if (BaseGame.Get().input.PadPressed((Buttons)8192))
				{
					state = 55;
					stateChange = 1f;
					BaseGame.Get().PlayCue("hat_4");
				}
			}
			else if (state == 7)
			{
				stateChange += (float)gameTime.ElapsedGameTime.TotalSeconds;
				if (stateChange > 2f && BaseGame.Get().levelLoaded)
				{
					state = 8;
				}
			}
			else if (state == 8)
			{
				BaseGame.Get().StopAndClearAllCues();
				((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Add((IGameComponent)(object)new BaseComponent(((GameComponent)this).Game));
				((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Remove((IGameComponent)(object)this);
			}
			else if (state == 9)
			{
				tentativeSave = false;
				curOffset = offset * (1f - stateChange) + offsetFinal * stateChange;
				curOffset2 = offset2 * (1f - stateChange) + offsetFinal2 * stateChange;
				stateChange += (float)gameTime.ElapsedGameTime.TotalSeconds;
				if (stateChange > 1f)
				{
					state = 10;
				}
			}
			else if (state == 10)
			{
				if (BaseGame.Get().input.DirectionUp())
				{
					BaseGame.Get().optionMenu.MoveUp();
				}
				if (BaseGame.Get().input.DirectionDown())
				{
					BaseGame.Get().optionMenu.MoveDown();
				}
				if (BaseGame.Get().input.Select() && BaseGame.Get().optionMenu.ActiveItem.command == "exit")
				{
					BaseGame.Get().BeginSavePlayer();
					tentativeSave = true;
				}
				if (BaseGame.Get().input.PadPressed((Buttons)8192))
				{
					BaseGame.Get().BeginSavePlayer();
					tentativeSave = true;
				}
				if (tentativeSave && BaseGame.Get().PlayerSaved)
				{
					BaseGame.Get().PlayCue("hat_4");
					state = 55;
					stateChange = 1f;
				}
				BaseGame.Get().optionMenu.Update(gameTime);
			}
		}
		((GameComponent)this).Update(gameTime);
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Unknown result type (might be due to invalid IL or missing references)
		//IL_0692: Unknown result type (might be due to invalid IL or missing references)
		//IL_0697: Unknown result type (might be due to invalid IL or missing references)
		//IL_06da: Unknown result type (might be due to invalid IL or missing references)
		//IL_06df: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_072e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0748: Unknown result type (might be due to invalid IL or missing references)
		//IL_074d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0757: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_081c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0821: Unknown result type (might be due to invalid IL or missing references)
		//IL_084a: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0928: Unknown result type (might be due to invalid IL or missing references)
		//IL_092d: Unknown result type (might be due to invalid IL or missing references)
		//IL_097a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0994: Unknown result type (might be due to invalid IL or missing references)
		//IL_099e: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a8: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilEnable = true;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilFunction = (CompareFunction)8;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.ReferenceStencil = 1;
		BaseGame.Get().graphics.GraphicsDevice.Clear((ClearOptions)4, Color.Black, 0f, 0);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.ReferenceStencil = 1;
		BaseGame.Get().combineEffect.Parameters["ColorTint"].SetValue(new Vector4(1f, 1f, 1f, 1f));
		BaseGame.Get().graphics.GraphicsDevice.SetRenderTarget(0, BaseGame.Get().worldTarget);
		_ = BaseGame.bloom_on;
		BaseGame.Get().graphics.GraphicsDevice.Clear(new Color(new Vector4(0f, 0f, 0f, 0f)));
		BaseGame.Get().DrawFullscreenQuad(BaseGame.Get().backTex, BaseGame.WIDTH, BaseGame.HEIGHT, null, new Color(new Vector4(0.05f, 0.2f, 0.05f, 1f)));
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
		BaseGame.Get().fogEffect.Parameters["xView"].SetValue(Matrix.Identity);
		BaseGame.Get().fogEffect.Parameters["xVProj"].SetValue(BaseGame.Get().flatEffect.Projection);
		BaseGame.Get().fogEffect.Parameters["xProjection"].SetValue(BaseGame.Get().flatEffect.Projection);
		BaseGame.Get().fogEffect.Parameters["xWorld"].SetValue(BaseGame.Get().world);
		BaseGame.Get().fogEffect.Parameters["xDoubleSided"].SetValue(true);
		BaseGame.Get().GraphicsSettings();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().fogEffect.Parameters["xFogEnable"].SetValue(false);
		BaseGame.Get().ps.Draw(gameTime);
		BaseGame.Get().fogEffect.Parameters["xFogEnable"].SetValue(true);
		BaseGame.Get().fogEffect.Parameters["xView"].SetValue(BaseGame.Get().viewMatrix);
		BaseGame.Get().fogEffect.Parameters["xVProj"].SetValue(BaseGame.Get().viewMatrix * BaseGame.Get().projectionMatrix);
		BaseGame.Get().LineUpCamera();
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateRotationX(MathHelper.ToRadians(-90f)) * Matrix.CreateScale(20.2f, 20.2f, -20.2f) * Matrix.CreateTranslation(curOffset) * Matrix.CreateRotationZ(MathHelper.ToRadians(10f * faceRotate)) * Matrix.CreateTranslation(curOffset2));
		if (BaseGame.Get().curUserData.levelsCleared == 4)
		{
			BaseGame.Get().DrawModel(ref koko, clearEpc: false, disableAnim: true);
		}
		else
		{
			BaseGame.Get().DrawModel(ref face[0], clearEpc: false, disableAnim: true);
		}
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		BaseGame.Get().DrawModel(ref face[1], clearEpc: false, disableAnim: true);
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)2;
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateRotationX(MathHelper.ToRadians(-90f)) * Matrix.CreateScale(2.2f) * Matrix.CreateTranslation(new Vector3(1.9f, 0f, -1f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-2f * gridRotate)) * Matrix.CreateTranslation(new Vector3(-1.2f, 0f, 0f)));
		BaseGame.Get().DrawModel(ref grid);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().graphics.GraphicsDevice.SetRenderTarget(0, (RenderTarget2D)null);
		if (BaseGame.bloom_on)
		{
			BaseGame.Get().DrawGlow();
		}
		else
		{
			BaseGame.Get().DrawFullscreenQuad(BaseGame.Get().worldTarget.GetTexture(), BaseGame.WIDTH, BaseGame.HEIGHT, null, Color.White);
		}
		((Effect)BaseGame.Get().flatEffect).Begin();
		((Effect)BaseGame.Get().flatEffect).CurrentTechnique.Passes[0].Begin();
		BaseGame.Get().spriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
		BaseGame.Get().spriteBatch.Draw(grayFilter, new Rectangle(0, 0, BaseGame.WIDTH, BaseGame.HEIGHT), Color.White);
		if (BaseGame.Get().debug)
		{
			BaseGame.Get().spriteBatch.DrawString(BaseGame.Get().hud.HUDfont, "D", new Vector2(100f, 100f), Color.Red);
		}
		if (state == 0)
		{
			BaseGame.Get().spriteBatch.DrawString(BaseGame.Get().hud.HUDfont, "Olu", new Vector2((float)(BaseGame.WIDTH / 4), (float)(BaseGame.HEIGHT / 4)), Color.White, 0f, Vector2.Zero, 5f, (SpriteEffects)0, 0f);
			BaseGame.Get().spriteBatch.DrawString(BaseGame.Get().hud.HUDfont, "[ press start ]", new Vector2((float)(BaseGame.WIDTH / 4), (float)(5 * BaseGame.HEIGHT / 8)), new Color(new Vector4(1f, 1f, 1f, textFlash)), 0f, Vector2.Zero, 1f, (SpriteEffects)0, 0f);
		}
		else if (state == 1)
		{
			BaseGame.Get().spriteBatch.DrawString(BaseGame.Get().hud.HUDfont, "Olu", new Vector2((float)(3 * BaseGame.WIDTH / 4), (float)(BaseGame.HEIGHT / 4)), Color.White, MathHelper.ToRadians(90f * stateChange), new Vector2((float)(BaseGame.WIDTH / 2 / 5), 0f), 5f, (SpriteEffects)0, 0f);
			BaseGame.Get().spriteBatch.DrawString(BaseGame.Get().hud.HUDfont, "[ press start ]", new Vector2((float)(3 * BaseGame.WIDTH / 4), (float)(BaseGame.HEIGHT / 4)), Color.White, MathHelper.ToRadians(90f * stateChange), new Vector2((float)(BaseGame.WIDTH / 2), (float)(-3 * BaseGame.HEIGHT / 8)), 1f, (SpriteEffects)0, 0f);
		}
		else if (state == 2)
		{
			mainMenu.Draw(gameTime);
		}
		else if (state == 4)
		{
			BaseGame.Get().spriteBatch.Draw(menuSlide, new Rectangle(0, (int)((float)(-BaseGame.HEIGHT) / 4f + (float)(4 * BaseGame.HEIGHT / 9) - particleSource.Y * 2f), BaseGame.WIDTH, BaseGame.HEIGHT), Color.White);
			BaseGame.Get().spriteBatch.Draw(menuSlide, new Rectangle(0, (int)((float)(5 * BaseGame.HEIGHT) / 4f + (float)(4 * BaseGame.HEIGHT / 9) - particleSource.Y * 2f), BaseGame.WIDTH, BaseGame.HEIGHT), Color.White);
			playModeMenu.Draw(gameTime);
			BaseGame.Get().spriteBatch.DrawString(BaseGame.Get().hud.HUDfont, dirText, new Vector2((float)BaseGame.WIDTH / 2f, (float)BaseGame.HEIGHT / 2f) - BaseGame.Get().hud.HUDfont.MeasureString(dirText) / 2f, Color.White);
		}
		else if (state == 6)
		{
			BaseGame.Get().levelMenu.Draw(gameTime);
		}
		else if (state == 10)
		{
			BaseGame.Get().optionMenu.Draw(gameTime);
		}
		BaseGame.Get().spriteBatch.End();
		((Effect)BaseGame.Get().flatEffect).CurrentTechnique.Passes[0].End();
		((Effect)BaseGame.Get().flatEffect).End();
		((DrawableGameComponent)this).Draw(gameTime);
	}
}
