using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using ZXBox;
using ZXBox.Hardware.Input;
using ZXBox.Hardware.Input.Joystick;
using ZXBox.Hardware.Output;
using ZXBox.Snapshot;

namespace Manic_Miner_360;

public class Game1 : Game
{
	private ScreenEnum CurrentScreen = ScreenEnum.Splash1;

	private DateTime? SplashTime = null;

	private string GameName = "";

	private GameTypeEnum GameType;

	private GraphicsDeviceManager graphics;

	private Beeper beeper;

	private int bufferCount20ms;

	private byte[] combinedBuffer;

	private int flashcounter;

	private int HelpPosition;

	private int HistoryPosition;

	private Kempston kempston;

	private ZXBox.Hardware.Input.Keyboard keyboard;

	private Screen screen;

	private ZxSpectrum speccy;

	private Texture2D textureTarget;

	private bool GameLoaded;

	private bool flash;

	private int tstates = 69888;

	private DynamicSoundEffectInstance soundEffect = new DynamicSoundEffectInstance(22500, AudioChannels.Mono);

	private int BufferingSoundCounter;

	private bool Buffering = true;

	private bool DoingInstructions;

	private int checksound;

	private bool SoundOn = true;

	private StorageDevice Device;

	private bool GameSaved;

	private bool isLocalAccount;

	private List<byte[]> SoundQueue = new List<byte[]>();

	private ContentManager content;

	private SpriteBatch spriteBatch;

	private float Zoomlevel = 3.35f;

	private Vector2 ScreenPosition = new Vector2(50f, 30f);

	private uint[] screenints;

	private bool emulate = true;

	private SpriteFont MenuSprite;

	private SpriteFont TextSprite;

	private PlayerIndex playerIndex;

	private bool PlayerIndexSelected;

	private Texture2D hud;

	private Texture2D GlowTexture;

	private Texture2D ScannLinesTexture;

	private Texture2D Splash1;

	private Texture2D Splash2;

	private Texture2D ManicTexture;

	private Texture2D MinerTexture;

	private MMSettings Settings;

	private bool keyboardIsShown;

	private StorageContainer loadcontainer;

	private bool DownPressed;

	private bool UpPressed;

	private bool ButtonA;

	private bool ButtonB;

	private bool ButtonBack;

	private bool ButtonY;

	private StorageContainer savecontainer;

	private bool change = true;

	private int MenuItem;

	private int SettingsItem;

	private int GameTypeSelectMenuItem;

	private int GameContinueSelectMenuItem;

	private int startcounter;

	private int counter;

	private bool? SaveGameExists = null;

	private StorageContainer container;

	public int samplesperframe { get; set; }

	public bool IsTrial => Guide.IsTrialMode;

	public Game1()
	{
		graphics = new GraphicsDeviceManager(this);
		base.Content.RootDirectory = "Content";
		graphics.PreferredBackBufferHeight = 720;
		graphics.PreferredBackBufferWidth = 1280;
		base.TargetElapsedTime = TimeSpan.FromMilliseconds(20.0);
		base.IsFixedTimeStep = true;
		base.Components.Add(new GamerServicesComponent(this));
		StorageDevice.DeviceChanged += StorageDevice_DeviceChanged;
		soundEffect.Volume = 0.05f;
	}

	private void StorageDevice_DeviceChanged(object sender, EventArgs e)
	{
		if (Device != null && !Device.IsConnected && !Guide.IsVisible)
		{
			try
			{
				Guide.BeginShowMessageBox(playerIndex, "Chosen storage device removed", "Please re-insert it and select again or saved games will default to the Hard Drive if available", new string[1] { "Ok" }, 0, MessageBoxIcon.Warning, confirmwarning, null);
			}
			catch
			{
			}
			SignInPlayerAndChooseStorageDevice();
		}
	}

	private void confirmwarning(IAsyncResult result)
	{
		Guide.EndShowMessageBox(result);
		while (Guide.IsVisible)
		{
		}
		StorageDevice.BeginShowSelector(playerIndex, delegate(IAsyncResult ar)
		{
			Device = StorageDevice.EndShowSelector(ar);
		}, null);
	}

	protected override void Initialize()
	{
		base.Initialize();
	}

	protected override void LoadContent()
	{
		Splash1 = base.Content.Load<Texture2D>("Splash1");
		Splash2 = base.Content.Load<Texture2D>("Splash2");
		Settings = new MMSettings();
		Settings.PixelFiltering = false;
		Settings.ScanLines = true;
		Settings.TVSurround = true;
		Settings.WinterMode = false;
		Settings.FullScreen = false;
		MenuSprite = base.Content.Load<SpriteFont>("DisposableDroid");
		TextSprite = base.Content.Load<SpriteFont>("DisposableDroidSmall");
		hud = base.Content.Load<Texture2D>("xbox_hud");
		ScannLinesTexture = base.Content.Load<Texture2D>("Scanlines");
		GlowTexture = base.Content.Load<Texture2D>("glow");
		ManicTexture = base.Content.Load<Texture2D>("Manic");
		MinerTexture = base.Content.Load<Texture2D>("Miner");
		spriteBatch = new SpriteBatch(base.GraphicsDevice);
		soundEffect.BufferNeeded += soundEffect_BufferNeeded;
		if (speccy == null)
		{
			speccy = new ZxSpectrum();
			Stream stream = TitleContainer.OpenStream("48.rom");
			int num = 0;
			int num2;
			while ((num2 = stream.ReadByte()) != -1)
			{
				speccy.Memory[num++] = num2;
			}
			stream.Close();
			stream.Dispose();
			kempston = new Kempston();
			keyboard = new ZXBox.Hardware.Input.Keyboard();
			speccy.InputHardware.Add(kempston);
			speccy.InputHardware.Add(keyboard);
			screen = new Screen(speccy, renderBorder: true, switchColors: false);
			speccy.OutputHardware.Add(screen);
			textureTarget = new Texture2D(base.GraphicsDevice, screen.Width, screen.Height, mipMap: false, SurfaceFormat.Color);
			speccy.Reset();
			flashcounter = 8;
			bufferCount20ms = soundEffect.GetSampleSizeInBytes(new TimeSpan(0, 0, 0, 0, 20));
			samplesperframe = tstates / (bufferCount20ms / 2);
			beeper = new Beeper(samplesperframe, bufferCount20ms / 2);
			combinedBuffer = new byte[bufferCount20ms];
			speccy.OutputHardware.Add(beeper);
		}
	}

	private void soundEffect_BufferNeeded(object sender, EventArgs e)
	{
	}

	protected override void UnloadContent()
	{
	}

	private void MenuNavigation(ref int CurrentMenuItem, int max)
	{
		GamePadState gs = GamePad.GetState(playerIndex);
		GamePadDPad thumbstickDirection = GetThumbstickDirection(gs, leftStick: true);
		if (thumbstickDirection.Down != ButtonState.Released || thumbstickDirection.Up != ButtonState.Released || thumbstickDirection.Left != ButtonState.Released || thumbstickDirection.Right != ButtonState.Released)
		{
			gs = new GamePadState(gs.ThumbSticks, gs.Triggers, gs.Buttons, thumbstickDirection);
		}
		if (!DownPressed && !UpPressed)
		{
			if (gs.DPad.Down == ButtonState.Pressed)
			{
				DownPressed = true;
				CurrentMenuItem++;
				if (CurrentMenuItem > max)
				{
					CurrentMenuItem = max;
				}
			}
			if (gs.DPad.Up == ButtonState.Pressed)
			{
				UpPressed = true;
				CurrentMenuItem--;
				if (CurrentMenuItem < 0)
				{
					CurrentMenuItem = 0;
				}
			}
		}
		else
		{
			DownPressed = gs.DPad.Down == ButtonState.Pressed;
			UpPressed = gs.DPad.Up == ButtonState.Pressed;
		}
		if (gs.Buttons.A == ButtonState.Pressed && !ButtonA)
		{
			if (CurrentScreen == ScreenEnum.MainMenu)
			{
				switch (CurrentMenuItem)
				{
				case 0:
					CurrentScreen = ScreenEnum.GameTypeSelect;
					break;
				case 1:
					CurrentScreen = ScreenEnum.Settings;
					break;
				case 2:
					CurrentScreen = ScreenEnum.Help;
					break;
				case 3:
					CurrentScreen = ScreenEnum.About;
					break;
				case 4:
					CurrentScreen = ScreenEnum.History;
					break;
				case 5:
					Exit();
					break;
				case 6:
					Guide.ShowMarketplace(playerIndex);
					break;
				}
			}
			else if (CurrentScreen == ScreenEnum.GameTypeSelect)
			{
				if (GameTypeSelectMenuItem == 0)
				{
					GameType = GameTypeEnum.Normal;
				}
				else
				{
					GameType = GameTypeEnum.Cheat;
				}
				switch (GameType)
				{
				case GameTypeEnum.Normal:
					GameName = "Manic_normal.z80";
					break;
				case GameTypeEnum.Cheat:
					GameName = "Manic_cheat.z80";
					break;
				case GameTypeEnum.Trial:
					GameName = "Manic_trial.z80";
					break;
				}
				GameContinueSelectMenuItem = 0;
				SaveGameExists = null;
				CurrentScreen = ScreenEnum.GameContinueSelect;
			}
			else if (CurrentScreen == ScreenEnum.GameContinueSelect)
			{
				screen.SwitchColors(Settings.WinterMode);
				speccy.Reset();
				if (GameContinueSelectMenuItem == 0)
				{
					ISnapshot snapShotHandler = FileFormatFactory.GetSnapShotHandler(GameName);
					Stream stream = TitleContainer.OpenStream(GameName);
					byte[] array = new byte[stream.Length];
					stream.Read(array, 0, Convert.ToInt32(stream.Length));
					snapShotHandler.LoadSnapshot(array, speccy);
					CurrentScreen = ScreenEnum.GamePlay;
				}
				else
				{
					SignInPlayerAndChooseStorageDevice();
					try
					{
						IAsyncResult asyncResult = Device.BeginOpenContainer("Manic Miner Saves", null, null);
						asyncResult.AsyncWaitHandle.WaitOne();
						using (loadcontainer = Device.EndOpenContainer(asyncResult))
						{
							speccy.Reset();
							Stream stream2 = loadcontainer.OpenFile(GameName + ".saved", FileMode.Open, FileAccess.Read);
							byte[] array2 = new byte[stream2.Length];
							stream2.Read(array2, 0, Convert.ToInt32(stream2.Length));
							FileFormatFactory.GetSnapShotHandler("saved.sna").LoadSnapshot(array2, speccy);
							stream2.Close();
						}
						CurrentScreen = ScreenEnum.GamePlay;
					}
					catch (Exception)
					{
						if (loadcontainer != null)
						{
							loadcontainer.Dispose();
						}
					}
				}
			}
			else if (CurrentScreen == ScreenEnum.Settings)
			{
				switch (SettingsItem)
				{
				case 0:
					Settings.PixelFiltering = !Settings.PixelFiltering;
					break;
				case 1:
					Settings.ScanLines = !Settings.ScanLines;
					break;
				case 2:
					Settings.WinterMode = !Settings.WinterMode;
					break;
				case 3:
					Settings.FullScreen = !Settings.FullScreen;
					break;
				}
			}
			ButtonA = true;
		}
		else
		{
			ButtonA = gs.Buttons.A == ButtonState.Pressed;
		}
		if (gs.Buttons.B == ButtonState.Pressed && !ButtonB)
		{
			CurrentScreen = ScreenEnum.MainMenu;
			ButtonB = true;
		}
		else
		{
			ButtonB = gs.Buttons.B == ButtonState.Pressed;
		}
	}

	protected override void Update(GameTime gameTime)
	{
		GamePadState state = GamePad.GetState(this.playerIndex);
		if ((PlayerIndexSelected && !state.IsConnected) || (CurrentScreen != ScreenEnum.Splash1 && CurrentScreen != ScreenEnum.Splash2 && Gamer.SignedInGamers[this.playerIndex] == null))
		{
			CurrentScreen = ScreenEnum.Splash2;
		}
		if (checksound-- < 0)
		{
			SoundOn = MediaPlayer.GameHasControl;
			checksound = 25;
		}
		if (state.Buttons.Back == ButtonState.Pressed && !ButtonBack)
		{
			ButtonBack = true;
			if (CurrentScreen == ScreenEnum.GamePlay)
			{
				if (CheckPlayerIsSignedInAndStorageDeviceIsAvailible())
				{
					try
					{
						IAsyncResult asyncResult = Device.BeginOpenContainer("Manic Miner Saves", null, null);
						asyncResult.AsyncWaitHandle.WaitOne();
						using (savecontainer = Device.EndOpenContainer(asyncResult))
						{
							Stream stream = savecontainer.OpenFile(GameName + ".saved", FileMode.OpenOrCreate, FileAccess.Write);
							byte[] array = FileFormatFactory.GetSnapShotHandler("save.sna").SaveSnapshot(speccy);
							stream.Write(array, 0, array.Length);
							stream.Close();
							savecontainer.Dispose();
						}
						CurrentScreen = ScreenEnum.MainMenu;
					}
					catch (Exception)
					{
						if (savecontainer != null && !savecontainer.IsDisposed)
						{
							savecontainer.Dispose();
						}
					}
				}
			}
			else if (CurrentScreen != ScreenEnum.MainMenu)
			{
				CurrentScreen = ScreenEnum.MainMenu;
			}
		}
		if (state.Buttons.Back == ButtonState.Released)
		{
			ButtonBack = false;
		}
		switch (CurrentScreen)
		{
		case ScreenEnum.Splash1:
			if (!SplashTime.HasValue)
			{
				SplashTime = DateTime.Now;
			}
			if ((DateTime.Now - SplashTime.Value).Seconds >= 2)
			{
				CurrentScreen = ScreenEnum.Splash2;
				SplashTime = DateTime.Now;
			}
			break;
		case ScreenEnum.Splash2:
		{
			for (PlayerIndex playerIndex = PlayerIndex.One; playerIndex <= PlayerIndex.Four; playerIndex++)
			{
				GamePadState state2 = GamePad.GetState(playerIndex);
				if (state2.IsConnected && (state2.Buttons.Start == ButtonState.Pressed || state2.Buttons.A == ButtonState.Pressed))
				{
					this.playerIndex = playerIndex;
					PlayerIndexSelected = true;
					if (CheckPlayerIsSignedInAndStorageDeviceIsAvailible())
					{
						CurrentScreen = ScreenEnum.MainMenu;
					}
					else
					{
						SignInPlayerAndChooseStorageDevice();
					}
				}
			}
			if (PlayerIndexSelected && CheckPlayerIsSignedInAndStorageDeviceIsAvailible())
			{
				CurrentScreen = ScreenEnum.MainMenu;
			}
			break;
		}
		case ScreenEnum.MainMenu:
			MenuNavigation(ref MenuItem, (IsTrial && Gamer.SignedInGamers[this.playerIndex] != null && Gamer.SignedInGamers[this.playerIndex].IsSignedInToLive) ? 6 : 5);
			break;
		case ScreenEnum.Settings:
			MenuNavigation(ref SettingsItem, 4);
			break;
		case ScreenEnum.Help:
			MenuNavigation(ref HelpPosition, 14);
			break;
		case ScreenEnum.History:
			MenuNavigation(ref HistoryPosition, 6);
			break;
		case ScreenEnum.About:
		{
			int CurrentMenuItem = 0;
			MenuNavigation(ref CurrentMenuItem, 0);
			break;
		}
		case ScreenEnum.GameTypeSelect:
			MenuNavigation(ref GameTypeSelectMenuItem, 1);
			break;
		case ScreenEnum.GameContinueSelect:
			MenuNavigation(ref GameContinueSelectMenuItem, (!IsTrial && GameSaved) ? 1 : 0);
			if (GameContinueSelectMenuItem == 1 && (IsTrial || !GameSaved))
			{
				GameContinueSelectMenuItem = 0;
			}
			break;
		case ScreenEnum.GamePlay:
		{
			Stopwatch stopwatch = new Stopwatch();
			if (emulate && !Guide.IsVisible)
			{
				GamePadState gamePadState = GamePad.GetState(this.playerIndex);
				GamePadDPad thumbstickDirection = GetThumbstickDirection(gamePadState, leftStick: true);
				if (thumbstickDirection.Down != ButtonState.Released || thumbstickDirection.Up != ButtonState.Released || thumbstickDirection.Left != ButtonState.Released || thumbstickDirection.Right != ButtonState.Released)
				{
					gamePadState = new GamePadState(gamePadState.ThumbSticks, gamePadState.Triggers, gamePadState.Buttons, thumbstickDirection);
				}
				kempston.UpdateState(gamePadState);
				if (state.Buttons.Y == ButtonState.Pressed && !ButtonY)
				{
					ButtonY = true;
					List<Keys> list = new List<Keys>();
					list.Add(Keys.Enter);
					keyboard.SetKeystate(new KeyboardState(list.ToArray()));
				}
				else if (state.Buttons.Y == ButtonState.Released)
				{
					keyboard.SetKeystate(default(KeyboardState));
					ButtonY = false;
				}
				DoingInstructions = true;
				stopwatch.Start();
				speccy.DoIntructions(tstates);
				stopwatch.Stop();
				if (SoundOn)
				{
					beeper.AddSoundBuffer(tstates);
				}
				DoingInstructions = false;
				if (SoundOn)
				{
					ushort[] soundBuffer = beeper.GetSoundBuffer();
					Buffer.BlockCopy(soundBuffer, 0, combinedBuffer, 0, bufferCount20ms);
					try
					{
						if (soundEffect.PendingBufferCount <= 3)
						{
							soundEffect.SubmitBuffer(combinedBuffer);
						}
					}
					catch
					{
					}
					soundEffect.Play();
				}
			}
			FrameworkDispatcher.Update();
			break;
		}
		}
		base.Update(gameTime);
	}

	private void DrawManicMiner()
	{
		if (counter++ >= 25)
		{
			spriteBatch.Draw(MinerTexture, new Vector2(247f, 100f), Color.White);
			if (counter >= 50)
			{
				counter = 0;
			}
		}
		else
		{
			spriteBatch.Draw(ManicTexture, new Vector2(247f, 100f), Color.White);
		}
	}

	protected override void Draw(GameTime gameTime)
	{
		if (!Settings.FullScreen || CurrentScreen != ScreenEnum.GamePlay)
		{
			base.GraphicsDevice.Clear(Color.Black);
		}
		else
		{
			base.GraphicsDevice.Clear(screen.GetBackgroundColor());
		}
		base.GraphicsDevice.Textures[0] = null;
		if (Settings.PixelFiltering)
		{
			base.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			base.GraphicsDevice.BlendState = BlendState.Opaque;
			base.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
			spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);
		}
		else
		{
			base.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			base.GraphicsDevice.BlendState = BlendState.AlphaBlend;
			base.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Matrix.Identity);
		}
		switch (CurrentScreen)
		{
		case ScreenEnum.Splash1:
			spriteBatch.Draw(Splash1, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
			break;
		case ScreenEnum.Splash2:
			spriteBatch.Draw(Splash2, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
			if (startcounter++ < 16)
			{
				spriteBatch.DrawString(MenuSprite, "Press start", new Vector2(950f, 620f), Color.White);
			}
			if (startcounter >= 32)
			{
				startcounter = 0;
			}
			break;
		case ScreenEnum.MainMenu:
			DrawManicMiner();
			spriteBatch.DrawString(MenuSprite, "PLAY", new Vector2(400f, 250f), (MenuItem == 0) ? Color.Red : Color.White);
			spriteBatch.DrawString(MenuSprite, "SETTINGS", new Vector2(400f, 300f), (MenuItem == 1) ? Color.Red : Color.White);
			spriteBatch.DrawString(MenuSprite, "HELP", new Vector2(400f, 350f), (MenuItem == 2) ? Color.Red : Color.White);
			spriteBatch.DrawString(MenuSprite, "ABOUT", new Vector2(400f, 400f), (MenuItem == 3) ? Color.Red : Color.White);
			spriteBatch.DrawString(MenuSprite, "HISTORY", new Vector2(400f, 450f), (MenuItem == 4) ? Color.Red : Color.White);
			spriteBatch.DrawString(MenuSprite, "EXIT", new Vector2(400f, 500f), (MenuItem == 5) ? Color.Red : Color.White);
			if (IsTrial && Gamer.SignedInGamers[playerIndex] != null && Gamer.SignedInGamers[playerIndex].IsSignedInToLive)
			{
				spriteBatch.DrawString(MenuSprite, "PURCHASE FULL GAME", new Vector2(260f, 560f), (MenuItem == 6) ? Color.Red : Color.White);
			}
			spriteBatch.DrawString(TextSprite, "Use 'A' to select or 'BACK' to return ", new Vector2(250f, 620f), Color.White);
			spriteBatch.DrawString(TextSprite, "Scroll menu text with the d-pad", new Vector2(250f, 650f), Color.White);
			break;
		case ScreenEnum.GameTypeSelect:
			DrawManicMiner();
			spriteBatch.DrawString(MenuSprite, "NORMAL", new Vector2(400f, 350f), (GameTypeSelectMenuItem == 0) ? Color.Red : Color.White);
			spriteBatch.DrawString(MenuSprite, "CHEAT", new Vector2(400f, 400f), (GameTypeSelectMenuItem == 1) ? Color.Red : Color.White);
			break;
		case ScreenEnum.GameContinueSelect:
			DrawManicMiner();
			if (!CheckPlayerIsSignedInAndStorageDeviceIsAvailible())
			{
				SignInPlayerAndChooseStorageDevice();
			}
			if (Device.IsConnected)
			{
				try
				{
					if (!SaveGameExists.HasValue)
					{
						IAsyncResult asyncResult = Device.BeginOpenContainer("Manic Miner Saves", null, null);
						asyncResult.AsyncWaitHandle.WaitOne();
						container = Device.EndOpenContainer(asyncResult);
						SaveGameExists = (GameSaved = container.FileExists(GameName + ".saved"));
						container.Dispose();
					}
					spriteBatch.DrawString(MenuSprite, "NEW", new Vector2(400f, 350f), (GameContinueSelectMenuItem == 0) ? Color.Red : Color.White);
					if (SaveGameExists.HasValue && SaveGameExists.Value)
					{
						if (!IsTrial)
						{
							spriteBatch.DrawString(MenuSprite, "CONTINUE", new Vector2(400f, 400f), (GameContinueSelectMenuItem == 1) ? Color.Red : Color.White);
						}
						else
						{
							spriteBatch.DrawString(MenuSprite, "CONTINUE", new Vector2(400f, 400f), Color.Gray);
							spriteBatch.DrawString(MenuSprite, "Disabled in trial", new Vector2(400f, 450f), Color.Gray);
						}
					}
				}
				catch (Exception)
				{
					if (container != null)
					{
						container.Dispose();
					}
				}
				finally
				{
					if (container != null)
					{
						container.Dispose();
					}
				}
			}
			spriteBatch.DrawString(TextSprite, "Press 'A' to start game when loaded", new Vector2(250f, 620f), Color.White);
			break;
		case ScreenEnum.GamePlay:
			flashcounter--;
			if (flashcounter < 0)
			{
				flashcounter = 8;
				flash = !flash;
			}
			screenints = screen.drawScreen(flash);
			textureTarget.SetData(screenints);
			if (Settings.FullScreen)
			{
				spriteBatch.Draw(textureTarget, new Rectangle(173, 20, 934, 680), Color.White);
			}
			else
			{
				spriteBatch.Draw(textureTarget, new Rectangle(50, 31, 890, 658), Color.White);
			}
			break;
		case ScreenEnum.Settings:
		{
			int num5 = 247;
			DrawManicMiner();
			spriteBatch.DrawString(MenuSprite, "PIXEL FILTERING", new Vector2(num5, 300f), (SettingsItem == 0) ? Color.Red : Color.White);
			spriteBatch.DrawString(MenuSprite, Settings.PixelFiltering ? "ON" : "OFF", new Vector2(650f, 300f), Color.White);
			spriteBatch.DrawString(MenuSprite, "SCAN LINES", new Vector2(num5, 350f), (SettingsItem == 1) ? Color.Red : Color.White);
			spriteBatch.DrawString(MenuSprite, Settings.ScanLines ? "ON" : "OFF", new Vector2(650f, 350f), Color.White);
			spriteBatch.DrawString(MenuSprite, "WINTER MODE", new Vector2(num5, 400f), (SettingsItem == 2) ? Color.Red : Color.White);
			spriteBatch.DrawString(MenuSprite, Settings.WinterMode ? "ON" : "OFF", new Vector2(650f, 400f), Color.White);
			spriteBatch.DrawString(MenuSprite, "FULL SCREEN", new Vector2(num5, 450f), (SettingsItem == 3) ? Color.Red : Color.White);
			spriteBatch.DrawString(MenuSprite, Settings.FullScreen ? "ON" : "OFF", new Vector2(650f, 450f), Color.White);
			break;
		}
		case ScreenEnum.Help:
		{
			string text2 = "CONTROLS:\nLEFT - Walk Left \nRIGHT - Walk Right \nA - Jump / Start game\nY - Turn Music Off/On\nBACK - Exit game or Menu  \n\nABOUT:\nMiner Willy, while prospecting down Surbiton way, stumbles upon an ancient, long forgotten mine-shaft. On further exploration, he finds evidence of a lost civilisation far superior to our own, which used automatons to dig deep into the Earth's core to supply the essential raw materials for their advanced industry. After centuries of peace and prosperity, the civilisation was torn apart by war, and lapsed into a long dark age, abandoning their industry and machines. Nobody, however, thought to tell the mine robots to stop working, and through countless aeons they had steadily accumulated a hugh stockpile of valuable metals and minerals, and Miner Willy realises that he now has the opportunity to make his fortune by finding the underground store. Can YOU take the challenge and guide Willy through the underground caverns to the surface and riches. In order to move to the next chamber, you must collect all the flashing keys in the room while avoiding nasties like POISONOUS PANSIES and SPIDERS and SLIME and worst of all, MANIC MINING ROBOTS. When you have all the keys, you can enter the portal which will now be flashing. The game ends when you have been 'got' or fallen heavily three times.";
			string text3 = "The ZX Spectrum version of Manic Miner contains the following 20 rooms:\n1.Central Cavern\n2.The Cold Room\n3.The Menagerie\n4.Abandoned Uranium Workings\n5.Eugene's Lair\n6.Processing Plant\n7.The Vat\n8.Miner Willy meets the Kong Beast\n9.Wacky Amoebatrons\n10.The Endorian Forest\n11.Attack of the Mutant Telephones\n12.Return of the Alien Kong Beast\n13.Ore Refinery\n14.Skylab Landing Bay\n15.The Bank\n16.The Sixteenth Cavern\n17.The Warehouse\n18.Amoebatrons' Revenge\n19.Solar Power Generator\n20.The Final Barrier\n";
			spriteBatch.DrawString(TextSprite, WrapText(TextSprite, text2 + "\n" + text3, 700f), new Vector2(145f, -(HelpPosition * 100) + 150), Color.White);
			break;
		}
		case ScreenEnum.History:
		{
			string text = "Manic Miner is a platform game originally written for the ZX Spectrum by Matthew Smith and released by Bug-Byte in 1983 (later re-released by Software Projects). It is the first game in the Miner Willy series and among the pioneers of the platform game genre. The game itself was inspired by the Atari 800 game Miner 2049er. It has since been ported to numerous home computers and video game consoles.\nAt the time, its stand-out features included in-game music and sound effects, excellent playability, and colourful graphics, which were well designed for the graphical limitations of the ZX Spectrum. The Spectrum's video display allowed the background and foreground colors to be exchanged automatically without software attention and the \"animated\" load screen appears to swap the words Manic and Miner through clever manipulation of this feature.\nOn the Spectrum this was the first game with in-game music, the playing of which required constant CPU attention and was thought impossible. It was cleverly achieved by constantly alternating CPU time between the music and the game (which accounts for the music's stuttery rhythm). The in-game music is In the Hall of the Mountain King from Edvard Grieg's music to Henrik Ibsen's play Peer Gynt. The music that plays during the title screen is an arrangement of An der schonen blauen Donau (popularly known as The Blue Danube), a waltz by Johann Strauss.";
			spriteBatch.DrawString(TextSprite, WrapText(TextSprite, text, 700f), new Vector2(145f, -(HistoryPosition * 100) + 150), Color.White);
			break;
		}
		case ScreenEnum.About:
		{
			DrawManicMiner();
			int num = 300;
			int num2 = 50;
			int num3 = 30;
			int num4 = 50;
			spriteBatch.DrawCenteredString(TextSprite, "TM & © 1983-2012", new Vector2(num2, num), 930f);
			num += num3;
			spriteBatch.DrawCenteredString(TextSprite, "Matthew Smith", new Vector2(num2, num), 930f);
			num += num4;
			spriteBatch.DrawCenteredString(TextSprite, "ZX:EC TM & © 2012", new Vector2(num2, num), 930f);
			num += num3;
			spriteBatch.DrawCenteredString(TextSprite, "Elite Systems Group LTD", new Vector2(num2, num), 930f);
			num += num4;
			spriteBatch.DrawCenteredString(TextSprite, "Sinclair & ZX Spectrum", new Vector2(num2, num), 930f);
			num += num3;
			spriteBatch.DrawCenteredString(TextSprite, "TM & © 2012 Amstrad LTD", new Vector2(num2, num), 930f);
			num += num3;
			spriteBatch.DrawCenteredString(TextSprite, "All Rights Reserved", new Vector2(num2, num), 930f);
			num += num3;
			spriteBatch.DrawCenteredString(TextSprite, "Elite® is a registered trademark (142270) ", new Vector2(50f, num), 930f);
			num += num3 + 10;
			spriteBatch.DrawCenteredString(TextSprite, "Programming: Jimmy Engstrom", new Vector2(50f, num), 930f);
			num += num3;
			spriteBatch.DrawCenteredString(TextSprite, "Production: Matthew Hyden", new Vector2(50f, num), 930f);
			break;
		}
		}
		if (CurrentScreen != ScreenEnum.Splash1 && CurrentScreen != ScreenEnum.Splash2 && (CurrentScreen != ScreenEnum.GamePlay || !Settings.FullScreen))
		{
			spriteBatch.Draw(hud, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
			if (Settings.ScanLines && !Settings.FullScreen)
			{
				spriteBatch.Draw(ScannLinesTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
				spriteBatch.Draw(GlowTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
			}
		}
		spriteBatch.End();
		base.Draw(gameTime);
	}

	public string WrapText(SpriteFont spriteFont, string text, float maxLineWidth)
	{
		string[] array = text.Split('\n');
		StringBuilder stringBuilder = new StringBuilder();
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			string[] array3 = text2.Split(' ');
			float num = 0f;
			float x = spriteFont.MeasureString(" ").X;
			string[] array4 = array3;
			foreach (string text3 in array4)
			{
				Vector2 vector = spriteFont.MeasureString(text3);
				if (num + vector.X < maxLineWidth)
				{
					stringBuilder.Append(text3 + " ");
					num += vector.X + x;
				}
				else
				{
					stringBuilder.Append("\n" + text3 + " ");
					num = vector.X + x;
				}
			}
			stringBuilder.Append("\n");
		}
		return stringBuilder.ToString();
	}

	private static GamePadDPad GetThumbstickDirection(GamePadState gs, bool leftStick)
	{
		float num = 0.35f;
		Vector2 vector = (leftStick ? gs.ThumbSticks.Left : gs.ThumbSticks.Right);
		ButtonState rightValue = ButtonState.Released;
		ButtonState leftValue = ButtonState.Released;
		ButtonState upValue = ButtonState.Released;
		ButtonState downValue = ButtonState.Released;
		float num2 = Math.Abs(vector.X);
		float num3 = Math.Abs(vector.Y);
		if (num2 > num3 && num2 > num)
		{
			if (vector.X > 0f)
			{
				rightValue = ButtonState.Pressed;
			}
			else
			{
				leftValue = ButtonState.Pressed;
			}
		}
		else if (num2 < num3 && num3 > num)
		{
			if (vector.Y > 0f)
			{
				upValue = ButtonState.Pressed;
			}
			else
			{
				downValue = ButtonState.Pressed;
			}
		}
		return new GamePadDPad(upValue, downValue, leftValue, rightValue);
	}

	private void SignInPlayerAndChooseStorageDevice()
	{
		if (CheckPlayerIsSignedInAndStorageDeviceIsAvailible())
		{
			return;
		}
		if (Gamer.SignedInGamers[playerIndex] == null)
		{
			try
			{
				while (Guide.IsVisible)
				{
				}
				Guide.ShowSignIn(1, onlineOnly: false);
			}
			catch
			{
			}
		}
		if (Gamer.SignedInGamers[playerIndex] == null)
		{
			return;
		}
		try
		{
			if (!Guide.IsVisible)
			{
				StorageDevice.BeginShowSelector(playerIndex, delegate(IAsyncResult ar)
				{
					Device = StorageDevice.EndShowSelector(ar);
					SaveGameExists = null;
				}, null);
			}
		}
		catch
		{
		}
	}

	public bool CheckPlayerIsSignedInAndStorageDeviceIsAvailible()
	{
		if (Device != null && Device.IsConnected)
		{
			return Gamer.SignedInGamers[playerIndex] != null;
		}
		return false;
	}
}
