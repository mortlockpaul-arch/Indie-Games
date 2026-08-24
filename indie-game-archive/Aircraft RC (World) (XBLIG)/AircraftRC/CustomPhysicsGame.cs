#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using BEPUphysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace AircraftRC;

public class CustomPhysicsGame : Game
{
	public enum GameState
	{
		Ecran,
		pressA,
		Debut,
		Menu,
		Loading,
		Partie
	}

	public enum Hide
	{
		cache,
		vu
	}

	public enum ScoreP
	{
		cache,
		vu
	}

	public enum CreditsV
	{
		cache,
		vu
	}

	public enum GameMode
	{
		M0,
		M1
	}

	public enum HideHUD
	{
		tout,
		Hcommande,
		Hcompteurs,
		compter,
		Hfeul
	}

	public enum ManetteChoix
	{
		M1,
		M2,
		M3,
		M4,
		M5,
		M6
	}

	private enum LoadingState
	{
		NotLoading,
		ReadyToSelectStorageDevice,
		SelectingStorageDevice,
		ReadyToOpenStorageContainer,
		OpeningStorageContainer,
		ReadyToLoad
	}

	private enum SavingState
	{
		NotSaving,
		ReadyToSelectStorageDevice,
		SelectingStorageDevice,
		ReadyToOpenStorageContainer,
		OpeningStorageContainer,
		ReadyToSave
	}

	[Serializable]
	public struct SaveGameData
	{
		public HideHUD hideHUDs;

		public Hide hides;

		public ManetteChoix manetteChoixs;

		public float Vol;

		public float volBoutton;

		public int ScoreRing;

		public int ScoreTimeM;

		public int ScoreTimeS;

		public int crashS;

		public int ScoreTimeHS;

		public int ScoreTimeMS;

		public int ScoreTimeSS;

		public int crashCa;

		public int ScoreTimeHCa;

		public int ScoreTimeMCa;

		public int ScoreTimeSCa;

		public int crashC;

		public int ScoreTimeHC;

		public int ScoreTimeMC;

		public int ScoreTimeSC;

		public int crashAc;

		public int ScoreTimeHAc;

		public int ScoreTimeMAc;

		public int ScoreTimeSAc;

		public int crashD;

		public int ScoreTimeHD;

		public int ScoreTimeMD;

		public int ScoreTimeSD;

		public int crashF;

		public int ScoreTimeHF;

		public int ScoreTimeMF;

		public int ScoreTimeSF;
	}

	private const string userPreferencesFile = "UserPreferences.xml";

	public string string1;

	public bool pause;

	public bool activeSR;

	public bool scorePage;

	public bool SortirGM;

	public ManetteConfig inputStateConfig;

	public GamePadState GamePadState;

	public GamePadState lastGamePadState;

	private SplashScreenGameComponent splashScreenGameComponent;

	public SystemPreferences preferences;

	public GraphicsDeviceManager graphics;

	public GameState gameState;

	public Hide hide = Hide.vu;

	public ScoreP scoreP = ScoreP.vu;

	public CreditsV creditsv;

	public GameMode gamemode;

	public HideHUD hideHUD;

	public ManetteChoix manetteChoix;

	private Texture2D splashdebut;

	public float tempsplash;

	public float achatP;

	public Space space;

	public Camera camera;

	public MenuModel menu;

	private Records records;

	private Ambiance ambience;

	private TextesSpring textesspring;

	private float tempdedemo;

	public SpadVII avion1;

	public Corsair avion2;

	public DH112 avion3;

	public Canadair avion4;

	public AC130 avion5;

	public F22 avion6;

	public TerrainP terrain;

	public Jeux jeux;

	public AudioEngine audioEngine;

	public WaveBank waveBank;

	public SoundBank soundBank;

	public InputState input;

	public float dt;

	private SignedInGamer gamer;

	public CompteurSpad conteurSpad;

	public CompteurCorsair conteurCorsair;

	public CompteurDH112 conteurDH112;

	public CompteurCanadair conteurCanadair;

	public CompteurAC130 conteurac130;

	public CompteurF22 conteurF22;

	public SaveGameData saveGameData;

	private StorageDevice storageDevice;

	private SavingState savingState;

	private LoadingState loadingState;

	private IAsyncResult asyncResult;

	private StorageContainer storageContainer;

	private string filename = "savegame.sav";

	public int pla;

	public CustomPhysicsGame()
	{
		base.Components.Add(new GamerServicesComponent(this));
		graphics = new GraphicsDeviceManager(this);
		base.Content.RootDirectory = "Content";
		graphics.PreferredBackBufferWidth = 1280;
		graphics.PreferredBackBufferHeight = 720;
		graphics.PreferMultiSampling = true;
		graphics.PreparingDeviceSettings += PrepareDeviceSettings;
		graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
		splashScreenGameComponent = new SplashScreenGameComponent(this);
		base.Components.Add(splashScreenGameComponent);
		inputStateConfig = new ManetteConfig(this);
		input = new InputState();
		camera = new Camera(this);
		space = new Space();
		space.ThreadManager.AddThread(delegate
		{
			Thread.CurrentThread.SetProcessorAffinity(new int[1] { 1 });
		}, null);
		space.ThreadManager.AddThread(delegate
		{
			Thread.CurrentThread.SetProcessorAffinity(new int[1] { 3 });
		}, null);
		space.ThreadManager.AddThread(delegate
		{
			Thread.CurrentThread.SetProcessorAffinity(new int[1] { 4 });
		}, null);
		space.ThreadManager.AddThread(delegate
		{
			Thread.CurrentThread.SetProcessorAffinity(new int[1] { 5 });
		}, null);
		space.ForceUpdater.Gravity = new Vector3(0f, -9.81f, 0f);
		textesspring = new TextesSpring(this);
		records = new Records(this);
		menu = new MenuModel(this);
		terrain = new TerrainP(this);
		jeux = new Jeux(this);
		conteurSpad = new CompteurSpad(this);
		conteurCorsair = new CompteurCorsair(this);
		conteurDH112 = new CompteurDH112(this);
		conteurCanadair = new CompteurCanadair(this);
		conteurac130 = new CompteurAC130(this);
		conteurF22 = new CompteurF22(this);
		ambience = new Ambiance(this);
		Strings.Culture = CultureInfo.CurrentCulture;
		string1 = Strings.HowToChange;
		preferences = new SystemPreferences();
		preferences.LightingDetail = DetailPreference.High;
		preferences.EffectDetail = DetailPreference.High;
		preferences.MaxAnisotropy = 4;
		preferences.PostProcessingDetail = DetailPreference.High;
		preferences.ShadowDetail = DetailPreference.High;
		preferences.ShadowQuality = 1f;
		preferences.TextureSampling = SamplingPreference.Anisotropic;
	}

	private void PrepareDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
	{
		e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PlatformContents;
	}

	public void LoadAvion1()
	{
		if (avion2 != null)
		{
			avion2.Remove(this);
		}
		if (avion3 != null)
		{
			avion3.Remove(this);
		}
		if (avion4 != null)
		{
			avion4.Remove(this);
		}
		if (avion5 != null)
		{
			avion5.Remove(this);
		}
		if (avion6 != null)
		{
			avion6.Remove(this);
		}
		if (avion1 == null)
		{
			avion1 = new SpadVII(this);
			avion1.Load(this);
			avion1.compteCrash = 0;
			conteurSpad.timecounterSA = 0;
			conteurSpad.timecounterMA = 0;
			conteurSpad.timecounterHA = 0;
			if (gamemode == GameMode.M1 && !activeSR)
			{
				UnloadJeux();
			}
			gamemode = GameMode.M0;
		}
		if (avion1 != null)
		{
			avion1.Moteur();
		}
	}

	public void LoadAvion2()
	{
		if (avion1 != null)
		{
			avion1.Remove(this);
		}
		if (avion3 != null)
		{
			avion3.Remove(this);
		}
		if (avion4 != null)
		{
			avion4.Remove(this);
		}
		if (avion5 != null)
		{
			avion5.Remove(this);
		}
		if (avion6 != null)
		{
			avion6.Remove(this);
		}
		if (avion2 == null)
		{
			avion2 = new Corsair(this);
			avion2.Load(this);
			avion2.compteCrash = 0;
			conteurCorsair.timecounterSA = 0;
			conteurCorsair.timecounterMA = 0;
			conteurCorsair.timecounterHA = 0;
			if (gamemode == GameMode.M1 && !activeSR)
			{
				UnloadJeux();
			}
			gamemode = GameMode.M0;
		}
		if (avion2 != null)
		{
			avion2.Moteur();
		}
	}

	public void LoadAvion3()
	{
		if (avion1 != null)
		{
			avion1.Remove(this);
		}
		if (avion2 != null)
		{
			avion2.Remove(this);
		}
		if (avion4 != null)
		{
			avion4.Remove(this);
		}
		if (avion5 != null)
		{
			avion5.Remove(this);
		}
		if (avion6 != null)
		{
			avion6.Remove(this);
		}
		if (avion3 == null)
		{
			avion3 = new DH112(this);
			avion3.Load(this);
			avion3.compteCrash = 0;
			conteurDH112.timecounterSA = 0;
			conteurDH112.timecounterMA = 0;
			conteurDH112.timecounterHA = 0;
			if (gamemode == GameMode.M1 && !activeSR)
			{
				UnloadJeux();
			}
			gamemode = GameMode.M0;
		}
		if (avion3 != null)
		{
			avion3.Moteur();
		}
	}

	public void LoadAvion4()
	{
		if (avion1 != null)
		{
			avion1.Remove(this);
		}
		if (avion2 != null)
		{
			avion2.Remove(this);
		}
		if (avion3 != null)
		{
			avion3.Remove(this);
		}
		if (avion5 != null)
		{
			avion5.Remove(this);
		}
		if (avion6 != null)
		{
			avion6.Remove(this);
		}
		if (avion4 == null)
		{
			avion4 = new Canadair(this);
			avion4.Load(this);
			avion4.compteCrash = 0;
			conteurCanadair.timecounterSA = 0;
			conteurCanadair.timecounterMA = 0;
			conteurCanadair.timecounterHA = 0;
			if (gamemode == GameMode.M1 && !activeSR)
			{
				UnloadJeux();
			}
			gamemode = GameMode.M0;
		}
		if (avion4 != null)
		{
			avion4.Moteur();
		}
	}

	public void LoadAvion5()
	{
		if (avion1 != null)
		{
			avion1.Remove(this);
		}
		if (avion2 != null)
		{
			avion2.Remove(this);
		}
		if (avion3 != null)
		{
			avion3.Remove(this);
		}
		if (avion4 != null)
		{
			avion4.Remove(this);
		}
		if (avion6 != null)
		{
			avion6.Remove(this);
		}
		if (avion5 == null)
		{
			avion5 = new AC130(this);
			avion5.Load(this);
			avion5.compteCrash = 0;
			conteurac130.timecounterSA = 0;
			conteurac130.timecounterMA = 0;
			conteurac130.timecounterHA = 0;
			if (gamemode == GameMode.M1 && !activeSR)
			{
				UnloadJeux();
			}
			gamemode = GameMode.M0;
		}
		if (avion5 != null)
		{
			avion5.Moteur();
		}
	}

	public void LoadAvion6()
	{
		if (avion1 != null)
		{
			avion1.Remove(this);
		}
		if (avion2 != null)
		{
			avion2.Remove(this);
		}
		if (avion3 != null)
		{
			avion3.Remove(this);
		}
		if (avion4 != null)
		{
			avion4.Remove(this);
		}
		if (avion5 != null)
		{
			avion5.Remove(this);
		}
		if (avion6 == null)
		{
			avion6 = new F22(this);
			avion6.Load(this);
			avion6.compteCrash = 0;
			conteurF22.timecounterSA = 0;
			conteurF22.timecounterMA = 0;
			conteurF22.timecounterHA = 0;
			if (gamemode == GameMode.M1 && !activeSR)
			{
				UnloadJeux();
			}
			gamemode = GameMode.M0;
		}
		if (avion6 != null)
		{
			avion6.Moteur();
		}
	}

	public void PauseAvion1Live()
	{
		if (avion1 != null && !avion1.Avioncasse && menu.avionChoix == MenuModel.AvionChoix.A1)
		{
			avion2 = null;
			avion3 = null;
			avion4 = null;
			avion5 = null;
			avion6 = null;
			if (avion2 != null)
			{
				avion2.Remove(this);
			}
			if (avion3 != null)
			{
				avion3.Remove(this);
			}
			if (avion4 != null)
			{
				avion4.Remove(this);
			}
			if (avion5 != null)
			{
				avion5.Remove(this);
			}
			if (avion6 != null)
			{
				avion6.Remove(this);
			}
			if (!avion1.Avioncasse)
			{
				gameState = GameState.Menu;
			}
			pause = true;
			Enregistrer();
			avion1.Pause();
		}
	}

	public void PauseAvion1()
	{
		if (avion1 != null && !avion1.Avioncasse && (inputStateConfig.StartP || !GamePadState.IsConnected) && menu.avionChoix == MenuModel.AvionChoix.A1)
		{
			avion2 = null;
			avion3 = null;
			avion4 = null;
			avion5 = null;
			avion6 = null;
			if (avion2 != null)
			{
				avion2.Remove(this);
			}
			if (avion3 != null)
			{
				avion3.Remove(this);
			}
			if (avion4 != null)
			{
				avion4.Remove(this);
			}
			if (avion5 != null)
			{
				avion5.Remove(this);
			}
			if (avion6 != null)
			{
				avion6.Remove(this);
			}
			if (!avion1.Avioncasse)
			{
				gameState = GameState.Menu;
			}
			pause = true;
			Enregistrer();
			avion1.Pause();
		}
	}

	public void PauseAvion2()
	{
		if (avion2 != null && !avion2.Avioncasse && (inputStateConfig.StartP || !GamePadState.IsConnected) && menu.avionChoix == MenuModel.AvionChoix.A3)
		{
			avion1 = null;
			avion3 = null;
			avion4 = null;
			avion5 = null;
			avion6 = null;
			if (avion1 != null)
			{
				avion1.Remove(this);
			}
			if (avion3 != null)
			{
				avion3.Remove(this);
			}
			if (avion4 != null)
			{
				avion4.Remove(this);
			}
			if (avion5 != null)
			{
				avion5.Remove(this);
			}
			if (avion6 != null)
			{
				avion6.Remove(this);
			}
			if (!avion2.Avioncasse)
			{
				gameState = GameState.Menu;
			}
			pause = true;
			Enregistrer();
			avion2.Pause();
		}
	}

	public void PauseAvion3()
	{
		if (avion3 != null && !avion3.Avioncasse && (inputStateConfig.StartP || !GamePadState.IsConnected) && menu.avionChoix == MenuModel.AvionChoix.A5)
		{
			avion1 = null;
			avion2 = null;
			avion4 = null;
			avion5 = null;
			avion6 = null;
			if (avion1 != null)
			{
				avion1.Remove(this);
			}
			if (avion2 != null)
			{
				avion2.Remove(this);
			}
			if (avion4 != null)
			{
				avion4.Remove(this);
			}
			if (avion5 != null)
			{
				avion5.Remove(this);
			}
			if (avion6 != null)
			{
				avion6.Remove(this);
			}
			if (!avion3.Avioncasse)
			{
				gameState = GameState.Menu;
			}
			pause = true;
			Enregistrer();
			avion3.Pause();
		}
	}

	public void PauseAvion4()
	{
		if (avion4 != null && !avion4.Avioncasse && (inputStateConfig.StartP || !GamePadState.IsConnected) && menu.avionChoix == MenuModel.AvionChoix.A2)
		{
			avion1 = null;
			avion2 = null;
			avion3 = null;
			avion5 = null;
			avion6 = null;
			if (avion1 != null)
			{
				avion1.Remove(this);
			}
			if (avion2 != null)
			{
				avion2.Remove(this);
			}
			if (avion3 != null)
			{
				avion3.Remove(this);
			}
			if (avion5 != null)
			{
				avion5.Remove(this);
			}
			if (avion6 != null)
			{
				avion6.Remove(this);
			}
			if (!avion4.Avioncasse)
			{
				gameState = GameState.Menu;
			}
			pause = true;
			Enregistrer();
			avion4.Pause();
		}
	}

	public void PauseAvion5()
	{
		if (avion5 != null && !avion5.Avioncasse && (inputStateConfig.StartP || !GamePadState.IsConnected) && menu.avionChoix == MenuModel.AvionChoix.A4)
		{
			avion1 = null;
			avion2 = null;
			avion3 = null;
			avion4 = null;
			avion6 = null;
			if (avion1 != null)
			{
				avion1.Remove(this);
			}
			if (avion2 != null)
			{
				avion2.Remove(this);
			}
			if (avion3 != null)
			{
				avion3.Remove(this);
			}
			if (avion4 != null)
			{
				avion4.Remove(this);
			}
			if (avion6 != null)
			{
				avion6.Remove(this);
			}
			if (!avion5.Avioncasse)
			{
				gameState = GameState.Menu;
			}
			pause = true;
			Enregistrer();
			avion5.Pause();
		}
	}

	public void PauseAvion6()
	{
		if (avion6 != null && !avion6.Avioncasse && (inputStateConfig.StartP || !GamePadState.IsConnected) && menu.avionChoix == MenuModel.AvionChoix.A6)
		{
			avion1 = null;
			avion2 = null;
			avion3 = null;
			avion4 = null;
			avion5 = null;
			if (avion1 != null)
			{
				avion1.Remove(this);
			}
			if (avion2 != null)
			{
				avion2.Remove(this);
			}
			if (avion3 != null)
			{
				avion3.Remove(this);
			}
			if (avion4 != null)
			{
				avion4.Remove(this);
			}
			if (avion5 != null)
			{
				avion5.Remove(this);
			}
			if (!avion6.Avioncasse)
			{
				gameState = GameState.Menu;
			}
			pause = true;
			Enregistrer();
			avion6.Pause();
		}
	}

	public void LoadJeux()
	{
		jeux.Load(this);
	}

	public void UnloadJeux()
	{
		jeux.Unload(this);
	}

	protected override void Initialize()
	{
		base.Initialize();
	}

	protected override void LoadContent()
	{
		splashdebut = base.Content.Load<Texture2D>("Textures/bepulogo");
		audioEngine = new AudioEngine("Content/Audio/avion.xgs");
		waveBank = new WaveBank(audioEngine, "Content/Audio/Wave Bank.xwb");
		soundBank = new SoundBank(audioEngine, "Content/Audio/Sound Bank.xsb");
		textesspring.LoadContent(this);
		records.LoadContent(this);
		menu.Load(this);
		terrain.Load(this);
		if (string1 == "0")
		{
			menu.MMenuF(this);
		}
		if (string1 == "2")
		{
			menu.MMenuE(this);
		}
		if (string1 == "1")
		{
			menu.MMenu(this);
		}
	}

	public T LoadLocalizedAsset<T>(string assetName)
	{
		string[] array = new string[2]
		{
			CultureInfo.CurrentCulture.Name,
			CultureInfo.CurrentCulture.TwoLetterISOLanguageName
		};
		string[] array2 = array;
		foreach (string text in array2)
		{
			string assetName2 = assetName + '.' + text;
			try
			{
				return base.Content.Load<T>(assetName2);
			}
			catch (ContentLoadException)
			{
			}
		}
		return base.Content.Load<T>(assetName);
	}

	protected override void UnloadContent()
	{
	}

	protected override void Update(GameTime gameTime)
	{
		UpdateLoading();
		UpdateSaving();
		lastGamePadState = GamePadState;
		checked
		{
			for (int i = 0; i < 4; i++)
			{
				GamePadState = GamePad.GetState(menu.player);
				if (GamePadState.IsConnected)
				{
					break;
				}
			}
			if (menu.player == PlayerIndex.One)
			{
				pla = 0;
			}
			if (menu.player == PlayerIndex.Two)
			{
				pla = 1;
			}
			if (menu.player == PlayerIndex.Three)
			{
				pla = 2;
			}
			if (menu.player == PlayerIndex.Four)
			{
				pla = 3;
			}
			gamer = Gamer.SignedInGamers[menu.player];
			if ((gameState == GameState.Debut || gameState == GameState.Menu || gameState == GameState.Partie) && gamer == null && !Guide.IsVisible)
			{
				Guide.ShowSignIn(1, onlineOnly: false);
			}
			input.Update();
			inputStateConfig.Update(this);
			dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
			Guide.SimulateTrialMode = false;
			if (gameState == GameState.pressA)
			{
				menu.Update(gameTime, this);
			}
			if (gameState == GameState.Debut || gameState == GameState.Menu)
			{
				if (inputStateConfig.Xpress)
				{
					scoreP++;
				}
				if (scoreP == ScoreP.vu)
				{
					scorePage = true;
				}
				else
				{
					scorePage = false;
				}
				if (scoreP > ScoreP.vu)
				{
					scoreP = ScoreP.cache;
				}
				if (scoreP < ScoreP.cache)
				{
					scoreP = ScoreP.vu;
				}
				if (inputStateConfig.Ypress)
				{
					creditsv++;
				}
				if (creditsv > CreditsV.vu)
				{
					creditsv = CreditsV.cache;
				}
				if (creditsv < CreditsV.cache)
				{
					creditsv = CreditsV.vu;
				}
				textesspring.Update(this, gameTime);
				menu.Update(gameTime, this);
			}
			if (gameState == GameState.Loading)
			{
				menu.Update(gameTime, this);
			}
			if (gameState == GameState.Partie && !pause)
			{
				tempdedemo += 0.1f;
				if (inputStateConfig.Bpress && !SortirGM)
				{
					hide++;
				}
				if (hide > Hide.vu)
				{
					hide = Hide.cache;
				}
				if (hide < Hide.cache)
				{
					hide = Hide.vu;
				}
				if (inputStateConfig.BHPlus)
				{
					hideHUD++;
				}
				if (inputStateConfig.BHMoins)
				{
					hideHUD--;
				}
				if (hideHUD < HideHUD.tout)
				{
					hideHUD = HideHUD.Hfeul;
				}
				if (hideHUD > HideHUD.Hfeul)
				{
					hideHUD = HideHUD.tout;
				}
				if (gamemode == GameMode.M1)
				{
					if (hideHUD < HideHUD.tout)
					{
						hideHUD = HideHUD.Hcommande;
					}
					if (hideHUD > HideHUD.Hcommande)
					{
						hideHUD = HideHUD.tout;
					}
				}
				terrain.sceneInterfaceScene.Update(gameTime);
				if (menu.avionChoix == MenuModel.AvionChoix.A1)
				{
					avion1.Update(dt, this, gameTime);
					conteurSpad.UpdateTime(this, gameTime);
				}
				if (menu.avionChoix == MenuModel.AvionChoix.A3)
				{
					avion2.Update(dt, this, gameTime);
					conteurCorsair.UpdateTime(this, gameTime);
				}
				if (menu.avionChoix == MenuModel.AvionChoix.A5)
				{
					avion3.Update(dt, this, gameTime);
					conteurDH112.UpdateTime(this, gameTime);
				}
				if (menu.avionChoix == MenuModel.AvionChoix.A2)
				{
					avion4.Update(dt, this, gameTime);
					conteurCanadair.UpdateTime(this, gameTime);
				}
				if (menu.avionChoix == MenuModel.AvionChoix.A4)
				{
					avion5.Update(dt, this, gameTime);
					conteurac130.UpdateTime(this, gameTime);
				}
				if (menu.avionChoix == MenuModel.AvionChoix.A6)
				{
					avion6.Update(dt, this, gameTime);
					conteurF22.UpdateTime(this, gameTime);
				}
				if (menu.avionChoix == MenuModel.AvionChoix.A1)
				{
					avion2 = null;
					avion3 = null;
					avion4 = null;
					avion5 = null;
					avion6 = null;
					if (avion1 != null && !avion1.Avioncasse)
					{
						if (inputStateConfig.Xpress && gamemode == GameMode.M0 && !jeux.finAficheR && !jeux.finAficheT)
						{
							gamemode = GameMode.M1;
							activeSR = true;
						}
						if (inputStateConfig.ApressBis && gamemode == GameMode.M1 && activeSR)
						{
							activeSR = false;
							LoadJeux();
							avion1.moteur.Stop(AudioStopOptions.Immediate);
							avion1.Restart(this);
						}
						if (inputStateConfig.Xpress && gamemode == GameMode.M1 && !activeSR && !jeux.finAficheR && !jeux.finAficheT)
						{
							SortirGM = true;
						}
						if (inputStateConfig.ApressBis && SortirGM)
						{
							UnloadJeux();
							Enregistrer();
							SortirGM = false;
							conteurSpad.timecounterSA = 0;
							conteurSpad.timecounterMA = 0;
							conteurSpad.timecounterHA = 0;
							gamemode = GameMode.M0;
						}
						if (inputStateConfig.Bpress && SortirGM)
						{
							SortirGM = false;
						}
						if ((jeux.A >= 10 || jeux.timecounterM >= 15) && (jeux.finAficheR || jeux.finAficheT) && menu.avionChoix == MenuModel.AvionChoix.A1)
						{
							avion2 = null;
							avion3 = null;
							avion4 = null;
							avion5 = null;
							avion6 = null;
							if (avion1 != null && !avion1.Avioncasse && inputStateConfig.ApressBis)
							{
								jeux.finAficheR = false;
								jeux.Unload(this);
								jeux.Load(this);
								avion1.moteur.Stop(AudioStopOptions.Immediate);
								avion1.Restart(this);
							}
						}
					}
				}
				if (menu.avionChoix == MenuModel.AvionChoix.A3)
				{
					avion1 = null;
					avion3 = null;
					avion4 = null;
					avion5 = null;
					avion6 = null;
					if (avion2 != null && !avion2.Avioncasse)
					{
						if (inputStateConfig.Xpress && gamemode == GameMode.M0 && !jeux.finAficheR && !jeux.finAficheT)
						{
							gamemode = GameMode.M1;
							activeSR = true;
						}
						if (inputStateConfig.ApressBis && gamemode == GameMode.M1 && activeSR)
						{
							activeSR = false;
							LoadJeux();
							avion2.moteur.Stop(AudioStopOptions.Immediate);
							avion2.Restart(this);
						}
						if (inputStateConfig.Xpress && gamemode == GameMode.M1 && !activeSR && !jeux.finAficheR && !jeux.finAficheT)
						{
							SortirGM = true;
						}
						if (inputStateConfig.ApressBis && SortirGM)
						{
							UnloadJeux();
							Enregistrer();
							SortirGM = false;
							conteurCorsair.timecounterSA = 0;
							conteurCorsair.timecounterMA = 0;
							conteurCorsair.timecounterHA = 0;
							gamemode = GameMode.M0;
						}
						if (inputStateConfig.Bpress && SortirGM)
						{
							SortirGM = false;
						}
						if ((jeux.A >= 10 || jeux.timecounterM >= 15) && (jeux.finAficheR || jeux.finAficheT) && menu.avionChoix == MenuModel.AvionChoix.A3)
						{
							avion1 = null;
							avion3 = null;
							avion4 = null;
							avion5 = null;
							avion6 = null;
							if (avion2 != null && !avion2.Avioncasse && inputStateConfig.ApressBis)
							{
								jeux.finAficheR = false;
								jeux.Unload(this);
								jeux.Load(this);
								Enregistrer();
								avion2.moteur.Stop(AudioStopOptions.Immediate);
								avion2.Restart(this);
							}
						}
					}
				}
				if (menu.avionChoix == MenuModel.AvionChoix.A5)
				{
					avion1 = null;
					avion2 = null;
					avion4 = null;
					avion5 = null;
					avion6 = null;
					if (avion3 != null && !avion3.Avioncasse)
					{
						if (inputStateConfig.Xpress && gamemode == GameMode.M0 && !jeux.finAficheR && !jeux.finAficheT)
						{
							gamemode = GameMode.M1;
							activeSR = true;
						}
						if (inputStateConfig.ApressBis && gamemode == GameMode.M1 && activeSR)
						{
							activeSR = false;
							LoadJeux();
							avion3.reacteur.Stop(AudioStopOptions.Immediate);
							avion3.reacteur2.Stop(AudioStopOptions.Immediate);
							avion3.Restart(this);
						}
						if (inputStateConfig.Xpress && gamemode == GameMode.M1 && !activeSR && !jeux.finAficheR && !jeux.finAficheT)
						{
							SortirGM = true;
						}
						if (inputStateConfig.ApressBis && SortirGM)
						{
							UnloadJeux();
							Enregistrer();
							SortirGM = false;
							conteurDH112.timecounterSA = 0;
							conteurDH112.timecounterMA = 0;
							conteurDH112.timecounterHA = 0;
							gamemode = GameMode.M0;
						}
						if (inputStateConfig.Bpress && SortirGM)
						{
							SortirGM = false;
						}
						if ((jeux.A >= 10 || jeux.timecounterM >= 15) && (jeux.finAficheR || jeux.finAficheT) && menu.avionChoix == MenuModel.AvionChoix.A5)
						{
							avion1 = null;
							avion2 = null;
							avion4 = null;
							avion5 = null;
							avion6 = null;
							if (avion3 != null && !avion3.Avioncasse && inputStateConfig.ApressBis)
							{
								jeux.finAficheR = false;
								jeux.Unload(this);
								jeux.Load(this);
								Enregistrer();
								avion3.reacteur.Stop(AudioStopOptions.Immediate);
								avion3.reacteur2.Stop(AudioStopOptions.Immediate);
								avion3.Restart(this);
							}
						}
					}
				}
				if (menu.avionChoix == MenuModel.AvionChoix.A2)
				{
					avion1 = null;
					avion2 = null;
					avion3 = null;
					avion5 = null;
					avion6 = null;
					if (avion4 != null && !avion4.Avioncasse)
					{
						if (inputStateConfig.Xpress && gamemode == GameMode.M0 && !jeux.finAficheR && !jeux.finAficheT)
						{
							gamemode = GameMode.M1;
							activeSR = true;
						}
						if (inputStateConfig.ApressBis && gamemode == GameMode.M1 && activeSR)
						{
							activeSR = false;
							LoadJeux();
							avion4.moteur.Stop(AudioStopOptions.Immediate);
							avion4.Restart(this);
						}
						if (inputStateConfig.Xpress && gamemode == GameMode.M1 && !activeSR && !jeux.finAficheR && !jeux.finAficheT)
						{
							SortirGM = true;
						}
						if (inputStateConfig.ApressBis && SortirGM)
						{
							UnloadJeux();
							Enregistrer();
							SortirGM = false;
							conteurCanadair.timecounterSA = 0;
							conteurCanadair.timecounterMA = 0;
							conteurCanadair.timecounterHA = 0;
							gamemode = GameMode.M0;
						}
						if (inputStateConfig.Bpress && SortirGM)
						{
							SortirGM = false;
						}
						if ((jeux.A >= 10 || jeux.timecounterM >= 15) && (jeux.finAficheR || jeux.finAficheT) && menu.avionChoix == MenuModel.AvionChoix.A2)
						{
							avion1 = null;
							avion2 = null;
							avion3 = null;
							avion5 = null;
							avion6 = null;
							if (avion4 != null && !avion4.Avioncasse && inputStateConfig.ApressBis)
							{
								jeux.finAficheR = false;
								jeux.Unload(this);
								jeux.Load(this);
								Enregistrer();
								avion4.moteur.Stop(AudioStopOptions.Immediate);
								avion4.Restart(this);
							}
						}
					}
				}
				if (menu.avionChoix == MenuModel.AvionChoix.A4)
				{
					avion1 = null;
					avion2 = null;
					avion3 = null;
					avion4 = null;
					avion6 = null;
					if (avion5 != null && !avion5.Avioncasse)
					{
						if (inputStateConfig.Xpress && gamemode == GameMode.M0 && !jeux.finAficheR && !jeux.finAficheT)
						{
							gamemode = GameMode.M1;
							activeSR = true;
						}
						if (inputStateConfig.ApressBis && gamemode == GameMode.M1 && activeSR)
						{
							activeSR = false;
							LoadJeux();
							avion5.moteur.Stop(AudioStopOptions.Immediate);
							avion5.Restart(this);
						}
						if (inputStateConfig.Xpress && gamemode == GameMode.M1 && !activeSR && !jeux.finAficheR && !jeux.finAficheT)
						{
							SortirGM = true;
						}
						if (inputStateConfig.ApressBis && SortirGM)
						{
							UnloadJeux();
							Enregistrer();
							SortirGM = false;
							conteurac130.timecounterSA = 0;
							conteurac130.timecounterMA = 0;
							conteurac130.timecounterHA = 0;
							gamemode = GameMode.M0;
						}
						if (inputStateConfig.Bpress && SortirGM)
						{
							SortirGM = false;
						}
						if ((jeux.A >= 10 || jeux.timecounterM >= 15) && (jeux.finAficheR || jeux.finAficheT) && menu.avionChoix == MenuModel.AvionChoix.A4)
						{
							avion1 = null;
							avion2 = null;
							avion3 = null;
							avion4 = null;
							avion6 = null;
							if (avion5 != null && !avion5.Avioncasse && inputStateConfig.ApressBis)
							{
								jeux.finAficheR = false;
								jeux.Unload(this);
								jeux.Load(this);
								Enregistrer();
								avion5.moteur.Stop(AudioStopOptions.Immediate);
								avion5.Restart(this);
							}
						}
					}
				}
				if (menu.avionChoix == MenuModel.AvionChoix.A6)
				{
					avion1 = null;
					avion2 = null;
					avion3 = null;
					avion4 = null;
					avion5 = null;
					if (avion6 != null && !avion6.Avioncasse)
					{
						if (inputStateConfig.Xpress && gamemode == GameMode.M0 && !jeux.finAficheR && !jeux.finAficheT)
						{
							gamemode = GameMode.M1;
							activeSR = true;
						}
						if (inputStateConfig.ApressBis && gamemode == GameMode.M1 && activeSR)
						{
							activeSR = false;
							LoadJeux();
							avion6.reacteur.Stop(AudioStopOptions.Immediate);
							avion6.reacteur2.Stop(AudioStopOptions.Immediate);
							avion6.Restart(this);
						}
						if (inputStateConfig.Xpress && gamemode == GameMode.M1 && !activeSR && !jeux.finAficheR && !jeux.finAficheT)
						{
							SortirGM = true;
						}
						if (inputStateConfig.ApressBis && SortirGM)
						{
							UnloadJeux();
							Enregistrer();
							SortirGM = false;
							conteurF22.timecounterSA = 0;
							conteurF22.timecounterMA = 0;
							conteurF22.timecounterHA = 0;
							gamemode = GameMode.M0;
						}
						if (inputStateConfig.Bpress && SortirGM)
						{
							SortirGM = false;
						}
						if ((jeux.A >= 10 || jeux.timecounterM >= 15) && (jeux.finAficheR || jeux.finAficheT) && menu.avionChoix == MenuModel.AvionChoix.A6)
						{
							avion1 = null;
							avion2 = null;
							avion3 = null;
							avion4 = null;
							avion5 = null;
							if (avion6 != null && !avion6.Avioncasse && inputStateConfig.ApressBis)
							{
								jeux.finAficheR = false;
								jeux.Unload(this);
								jeux.Load(this);
								Enregistrer();
								avion6.reacteur.Stop(AudioStopOptions.Immediate);
								avion6.reacteur2.Stop(AudioStopOptions.Immediate);
								avion6.Restart(this);
							}
						}
					}
				}
			}
			if (gameState == GameState.Partie)
			{
				ambience.UpdateSons(this);
				PauseAvion1();
				PauseAvion2();
				PauseAvion3();
				PauseAvion4();
				PauseAvion5();
				PauseAvion6();
			}
			if ((gameState == GameState.Debut || gameState == GameState.Menu) && Guide.IsTrialMode)
			{
				gamer = Gamer.SignedInGamers[menu.player];
				if (gamer != null && gamer.IsSignedInToLive && inputStateConfig.Ypress && !Guide.IsVisible && scoreP == ScoreP.vu)
				{
					Guide.ShowMarketplace(menu.player);
				}
			}
			if ((gameState == GameState.Debut || gameState == GameState.Menu) && Guide.IsTrialMode)
			{
				gamer = Gamer.SignedInGamers[menu.player];
				if (gamer != null && !gamer.IsSignedInToLive && inputStateConfig.Ypress && !Guide.IsVisible && scoreP == ScoreP.vu)
				{
					List<string> list = new List<string>();
					list.Add("OK");
					string title = "                       Connexion";
					string text = "";
					if (string1 == "0")
					{
						text = "Il vous faut un compte Xbox Live pour pouvoir acheter ce jeu, merci...";
					}
					if (string1 == "2")
					{
						text = "Necesita una cuenta Xbox Live para comprar este juego, gracias ...";
					}
					if (string1 == "1")
					{
						text = "You need an Xbox Live account to buy this game, thank you ...";
					}
					Guide.BeginShowMessageBox(menu.player, title, text, list, 0, MessageBoxIcon.Warning, null, null);
				}
			}
			base.Update(gameTime);
		}
	}

	protected override void Draw(GameTime gameTime)
	{
		if (!SplashScreenGameComponent.DisplayComplete)
		{
			gameState = GameState.Ecran;
			base.Draw(gameTime);
			return;
		}
		if (gameState == GameState.Ecran)
		{
			tempsplash++;
			if (tempsplash >= 170f)
			{
				tempsplash = 171f;
			}
			if (tempsplash <= 170f)
			{
				textesspring.spriteBatch.Begin();
				textesspring.spriteBatch.Draw(splashdebut, new Vector2(0f, 0f), Color.White);
				textesspring.spriteBatch.End();
			}
		}
		if (tempsplash >= 170f)
		{
			tempsplash = 0f;
			gameState = GameState.pressA;
		}
		if (gameState == GameState.pressA)
		{
			menu.Draw(this, gameTime);
			textesspring.Draw(this, gameTime);
		}
		if (gameState == GameState.Debut || gameState == GameState.Menu)
		{
			menu.Draw(this, gameTime);
			textesspring.Draw(this, gameTime);
			records.Draw(this, gameTime);
		}
		if (gameState == GameState.Loading)
		{
			menu.Draw(this, gameTime);
			textesspring.Draw(this, gameTime);
		}
		if (gameState == GameState.Partie)
		{
			if (gamemode == GameMode.M1)
			{
				jeux.Draw(this, gameTime);
			}
			terrain.Draw(this, gameTime);
			textesspring.Draw(this, gameTime);
			if (menu.avionChoix == MenuModel.AvionChoix.A1)
			{
				avion1.Draw();
			}
			if (menu.avionChoix == MenuModel.AvionChoix.A3)
			{
				avion2.Draw();
			}
			if (menu.avionChoix == MenuModel.AvionChoix.A5)
			{
				avion3.Draw();
			}
			if (menu.avionChoix == MenuModel.AvionChoix.A2)
			{
				avion4.Draw();
			}
			if (menu.avionChoix == MenuModel.AvionChoix.A4)
			{
				avion5.Draw();
			}
			if (menu.avionChoix == MenuModel.AvionChoix.A6)
			{
				avion6.Draw();
			}
		}
		base.Draw(gameTime);
	}

	private void UpdateSaving()
	{
		switch (savingState)
		{
		case SavingState.ReadyToSelectStorageDevice:
			if (!Guide.IsVisible)
			{
				asyncResult = StorageDevice.BeginShowSelector(menu.player, null, null);
				savingState = SavingState.SelectingStorageDevice;
			}
			break;
		case SavingState.SelectingStorageDevice:
			if (asyncResult.IsCompleted)
			{
				storageDevice = StorageDevice.EndShowSelector(asyncResult);
				savingState = SavingState.ReadyToOpenStorageContainer;
			}
			break;
		case SavingState.ReadyToOpenStorageContainer:
			if (storageDevice == null || !storageDevice.IsConnected)
			{
				savingState = SavingState.ReadyToSelectStorageDevice;
				break;
			}
			asyncResult = storageDevice.BeginOpenContainer("Save AircraftRC", null, null);
			savingState = SavingState.OpeningStorageContainer;
			break;
		case SavingState.OpeningStorageContainer:
			if (asyncResult.IsCompleted)
			{
				storageContainer = storageDevice.EndOpenContainer(asyncResult);
				savingState = SavingState.ReadyToSave;
			}
			break;
		case SavingState.ReadyToSave:
			if (storageContainer == null)
			{
				savingState = SavingState.ReadyToOpenStorageContainer;
				break;
			}
			try
			{
				DeleteExisting();
				Save();
				break;
			}
			catch (IOException ex)
			{
				Debug.WriteLine(ex.Message);
				break;
			}
			finally
			{
				storageContainer.Dispose();
				storageContainer = null;
				savingState = SavingState.NotSaving;
			}
		}
	}

	private void UpdateLoading()
	{
		switch (loadingState)
		{
		case LoadingState.ReadyToSelectStorageDevice:
			if (!Guide.IsVisible)
			{
				asyncResult = StorageDevice.BeginShowSelector(menu.player, null, null);
				loadingState = LoadingState.SelectingStorageDevice;
			}
			break;
		case LoadingState.SelectingStorageDevice:
			if (asyncResult.IsCompleted)
			{
				storageDevice = StorageDevice.EndShowSelector(asyncResult);
				loadingState = LoadingState.ReadyToOpenStorageContainer;
			}
			break;
		case LoadingState.ReadyToOpenStorageContainer:
			if (storageDevice == null || !storageDevice.IsConnected)
			{
				loadingState = LoadingState.ReadyToSelectStorageDevice;
				break;
			}
			asyncResult = storageDevice.BeginOpenContainer("Save AircraftRC", null, null);
			loadingState = LoadingState.OpeningStorageContainer;
			break;
		case LoadingState.OpeningStorageContainer:
			if (asyncResult.IsCompleted)
			{
				storageContainer = storageDevice.EndOpenContainer(asyncResult);
				loadingState = LoadingState.ReadyToLoad;
			}
			break;
		case LoadingState.ReadyToLoad:
			if (storageContainer == null)
			{
				loadingState = LoadingState.ReadyToOpenStorageContainer;
				break;
			}
			try
			{
				if (storageContainer.FileExists(filename))
				{
					Load();
					hideHUD = saveGameData.hideHUDs;
					hide = saveGameData.hides;
					manetteChoix = saveGameData.manetteChoixs;
					menu.volume = saveGameData.Vol;
					menu.VolumePosition2 = saveGameData.volBoutton;
					jeux.ReA = saveGameData.ScoreRing;
					jeux.timecounter1M = saveGameData.ScoreTimeM;
					jeux.timecounter1S = saveGameData.ScoreTimeS;
					conteurSpad.totalCrash = saveGameData.crashS;
					conteurSpad.timecounterHA1 = saveGameData.ScoreTimeHS;
					conteurSpad.timecounterMA1 = saveGameData.ScoreTimeMS;
					conteurSpad.timecounterSA1 = saveGameData.ScoreTimeSS;
					conteurCanadair.totalCrash = saveGameData.crashCa;
					conteurCanadair.timecounterHA1 = saveGameData.ScoreTimeHCa;
					conteurCanadair.timecounterMA1 = saveGameData.ScoreTimeMCa;
					conteurCanadair.timecounterSA1 = saveGameData.ScoreTimeSCa;
					conteurCorsair.totalCrash = saveGameData.crashD;
					conteurCorsair.timecounterHA1 = saveGameData.ScoreTimeHC;
					conteurCorsair.timecounterMA1 = saveGameData.ScoreTimeMC;
					conteurCorsair.timecounterSA1 = saveGameData.ScoreTimeSC;
					conteurac130.totalCrash = saveGameData.crashD;
					conteurac130.timecounterHA1 = saveGameData.ScoreTimeHAc;
					conteurac130.timecounterMA1 = saveGameData.ScoreTimeMAc;
					conteurac130.timecounterSA1 = saveGameData.ScoreTimeSAc;
					conteurDH112.totalCrash = saveGameData.crashD;
					conteurDH112.timecounterHA1 = saveGameData.ScoreTimeHD;
					conteurDH112.timecounterMA1 = saveGameData.ScoreTimeMD;
					conteurDH112.timecounterSA1 = saveGameData.ScoreTimeSD;
					conteurF22.totalCrash = saveGameData.crashF;
					conteurF22.timecounterHA1 = saveGameData.ScoreTimeHF;
					conteurF22.timecounterMA1 = saveGameData.ScoreTimeMF;
					conteurF22.timecounterSA1 = saveGameData.ScoreTimeSF;
				}
				break;
			}
			catch (IOException ex)
			{
				Debug.WriteLine(ex.Message);
				break;
			}
			finally
			{
				storageContainer.Dispose();
				storageContainer = null;
				loadingState = LoadingState.NotLoading;
			}
		}
	}

	private void DeleteExisting()
	{
		if (storageContainer.FileExists(filename))
		{
			storageContainer.DeleteFile(filename);
		}
	}

	private void Save()
	{
		using Stream stream = storageContainer.CreateFile(filename);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(SaveGameData));
		xmlSerializer.Serialize(stream, saveGameData);
	}

	private void Load()
	{
		using Stream stream = storageContainer.OpenFile(filename, FileMode.Open);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(SaveGameData));
		saveGameData = (SaveGameData)xmlSerializer.Deserialize(stream);
	}

	public void Enregistrer()
	{
		saveGameData = new SaveGameData
		{
			hideHUDs = hideHUD,
			hides = hide,
			manetteChoixs = manetteChoix,
			Vol = menu.volume,
			volBoutton = menu.VolumePosition2,
			ScoreRing = jeux.ReA,
			ScoreTimeM = jeux.timecounter1M,
			ScoreTimeS = jeux.timecounter1S,
			crashS = conteurSpad.totalCrash,
			ScoreTimeHS = conteurSpad.timecounterHA1,
			ScoreTimeMS = conteurSpad.timecounterMA1,
			ScoreTimeSS = conteurSpad.timecounterSA1,
			crashCa = conteurCanadair.totalCrash,
			ScoreTimeHCa = conteurCanadair.timecounterHA1,
			ScoreTimeMCa = conteurCanadair.timecounterMA1,
			ScoreTimeSCa = conteurCanadair.timecounterSA1,
			crashC = conteurCorsair.totalCrash,
			ScoreTimeHC = conteurCorsair.timecounterHA1,
			ScoreTimeMC = conteurCorsair.timecounterMA1,
			ScoreTimeSC = conteurCorsair.timecounterSA1,
			crashAc = conteurac130.totalCrash,
			ScoreTimeHAc = conteurac130.timecounterHA1,
			ScoreTimeMAc = conteurac130.timecounterMA1,
			ScoreTimeSAc = conteurac130.timecounterSA1,
			crashD = conteurDH112.totalCrash,
			ScoreTimeHD = conteurDH112.timecounterHA1,
			ScoreTimeMD = conteurDH112.timecounterMA1,
			ScoreTimeSD = conteurDH112.timecounterSA1,
			crashF = conteurF22.totalCrash,
			ScoreTimeHF = conteurF22.timecounterHA1,
			ScoreTimeMF = conteurF22.timecounterMA1,
			ScoreTimeSF = conteurF22.timecounterSA1
		};
		if (savingState == SavingState.NotSaving)
		{
			savingState = SavingState.ReadyToOpenStorageContainer;
		}
	}

	public void Lire()
	{
		if (loadingState == LoadingState.NotLoading)
		{
			loadingState = LoadingState.ReadyToOpenStorageContainer;
		}
	}
}
