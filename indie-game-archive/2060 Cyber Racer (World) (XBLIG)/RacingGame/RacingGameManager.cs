using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.GameLogic;
using RacingGame.GameScreens;
using RacingGame.Graphics;
using RacingGame.Landscapes;
using RacingGame.Properties;
using RacingGame.Shaders;
using RacingGame.Sounds;

namespace RacingGame;

public class RacingGameManager : BaseGame
{
	public enum Level
	{
		Beginner,
		Advanced,
		Expert
	}

	private static Stack<IGameScreen> gameScreens;

	private static Player player;

	private static Model carModel;

	private static Model carSelectionPlate;

	private static Texture[] carTextures;

	public static int currentCarNumber;

	public static int currentCarColor;

	public static Texture colorSelectionTexture;

	private static Material brakeTrackMaterial;

	public static List<Color> CarColors;

	private static Landscape landscape;

	public static bool InMenu
	{
		get
		{
			if (gameScreens.Count > 0)
			{
				return (object)gameScreens.Peek().GetType() != typeof(GameScreen);
			}
			return false;
		}
	}

	public static bool InGame
	{
		get
		{
			if (gameScreens.Count > 0)
			{
				return (object)gameScreens.Peek().GetType() == typeof(GameScreen);
			}
			return false;
		}
	}

	public static bool ShowMouseCursor
	{
		get
		{
			if (gameScreens.Count > 0 && (object)gameScreens.Peek().GetType() != typeof(GameScreen))
			{
				return (object)gameScreens.Peek().GetType() != typeof(SplashScreen);
			}
			return false;
		}
	}

	public static bool InCarSelectionScreen
	{
		get
		{
			if (gameScreens.Count > 0)
			{
				return (object)gameScreens.Peek().GetType() == typeof(CarSelection);
			}
			return false;
		}
	}

	public static Player Player => player;

	public static Model CarModel => carModel;

	public static Color CarColor
	{
		get
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			return CarColors[currentCarColor % CarColors.Count];
		}
	}

	public static int NumberOfCarColors => CarColors.Count;

	public static int NumberOfCarTextureTypes => carTextures.Length;

	public static Material BrakeTrackMaterial => brakeTrackMaterial;

	public static Model CarSelectionPlate => carSelectionPlate;

	public static Landscape Landscape => landscape;

	public static void LoadLevel(Level setNewLevel)
	{
		landscape.ReloadLevel(setNewLevel);
	}

	public static Texture CarTexture(int carNumber)
	{
		return carTextures[carNumber % carTextures.Length];
	}

	public RacingGameManager()
		: base("RacingGame")
	{
		Sound.Play(Sound.Sounds.MenuMusic);
		gameScreens.Push(new MainMenu());
		gameScreens.Push(new SplashScreen());
	}

	public RacingGameManager(string unitTestName)
		: base(unitTestName)
	{
	}

	protected override void Initialize()
	{
		base.Initialize();
		carModel = new Model("Car");
		carSelectionPlate = new Model("CarSelectionPlate");
		landscape = new Landscape(Level.Beginner);
		carTextures = new Texture[3];
		carTextures[0] = new Texture("RacerCar");
		carTextures[1] = new Texture("RacerCar2");
		carTextures[2] = new Texture("RacerCar3");
		colorSelectionTexture = new Texture("ColorSelection");
		brakeTrackMaterial = new Material("track");
	}

	public static void AddGameScreen(IGameScreen gameScreen)
	{
		Sound.Play(Sound.Sounds.ScreenClick);
		gameScreens.Push(gameScreen);
	}

	protected override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
		player.Update();
	}

	protected override void Render()
	{
		if (gameScreens.Count == 0)
		{
			Sound.PlayCrashSound(totalCrash: true);
			Sound.StopMusic();
			((Game)this).Exit();
		}
		else if (gameScreens.Peek().Render())
		{
			if ((object)gameScreens.Peek().GetType() == typeof(Options) && (BaseGame.Width != GameSettings.Default.ResolutionWidth || BaseGame.Height != GameSettings.Default.ResolutionHeight || BaseGame.Fullscreen != GameSettings.Default.Fullscreen))
			{
				BaseGame.ApplyResolutionChange();
			}
			Sound.Play(Sound.Sounds.ScreenBack);
			gameScreens.Pop();
		}
	}

	protected override void PostUIRender()
	{
		BaseGame.Device.RenderState.DepthBufferEnable = true;
		if (gameScreens.Count > 0 && (object)gameScreens.Peek().GetType() == typeof(CarSelection))
		{
			((CarSelection)gameScreens.Peek()).PostUIRender();
		}
		if (InMenu && PostScreenMenu.Started)
		{
			BaseGame.UI.PostScreenMenuShader.Show();
		}
	}

	static RacingGameManager()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		gameScreens = new Stack<IGameScreen>();
		player = new Player(new Vector3(0f, 0f, 0f));
		carModel = null;
		carSelectionPlate = null;
		carTextures = null;
		currentCarNumber = 0;
		colorSelectionTexture = null;
		brakeTrackMaterial = null;
		CarColors = new List<Color>((IEnumerable<Color>)(object)new Color[11]
		{
			Color.White,
			Color.Yellow,
			Color.Blue,
			Color.Purple,
			Color.Red,
			Color.Green,
			Color.Teal,
			Color.Gray,
			Color.Chocolate,
			Color.Orange,
			Color.SeaGreen
		});
		landscape = null;
	}
}
