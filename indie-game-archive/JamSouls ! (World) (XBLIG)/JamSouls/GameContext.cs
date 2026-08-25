using Microsoft.Xna.Framework.GamerServices;

namespace JamSouls;

internal static class GameContext
{
	public const int ResolutionX = 1280;

	public const int ResolutionY = 720;

	public const float TVSCREEN_SCALE = 1f;

	public const int ViewportWidth = 1280;

	public const int ViewportHeight = 720;

	public const float ScreenScale = 1f;

	public const float GRAVITY = 150f;

	public const float WORLDSCALE = 10f;

	public const int LEVEL_COUNT = 13;

	public const int HUD_A = 0;

	public const int HUD_B = 1;

	public const int HUD_Y = 2;

	public const int HUD_X = 3;

	public const int HUD_CROSS = 0;

	public const int HUD_RT = 1;

	public const int HUD_RB = 2;

	public const int HUD_RL = 3;

	public const int START_LEVEL = 9;

	public static GAME_MODE GameMode = GAME_MODE.NONE;

	public static int PointLimit;

	public static float TimeLimit;

	public static int BotNumber;

	public static int DifficultyLevel;

	public static string SelectedLevel;

	public static PlayerDef[] Pinfo = new PlayerDef[4];

	public static bool m_bSuddentDeath = true;

	public static int TileSafeTop = 0;

	public static int TileSafeBottom = 0;

	public static int TileSafeLeft = 0;

	public static int TileSafeRight = 0;

	public static float PLAYER_Z = 0.3f;

	public static float BALL_Z = 0.4f;

	public static float POWERUP_Z = 0.35f;

	public static string[] PAD_BUTTON_HUD = new string[4] { "bulle_A", "bulle_B", "bulle_Y", "bulle_X" };

	public static string[] PAD_BUTTON_HUD_SOFT = new string[4] { "bt_A", "bt_B", "bt_Y", "bt_X" };

	public static string[] PAD_BUTTON_HUD_TEX = new string[4] { "ICO_Pad", "bt_RT", "bt_RB", "bt_RB" };

	public static string[] BACKGROUND_MUSIC = new string[13]
	{
		"Sound/Bgm/Tromperie", "Sound/Bgm/Passion", "Sound/Bgm/Mort", "Sound/Bgm/Passion", "Sound/Bgm/Famine", "Sound/Bgm/Maladie", "Sound/Bgm/Guerre", "Sound/Bgm/Mort", "Sound/Bgm/Misere", "Sound/Bgm/Tromperie",
		"Sound/Bgm/Passion", "Sound/Bgm/Vice", "Sound/Bgm/Folie"
	};

	public static string[] SELECTABLE_LEVEL = new string[13]
	{
		"DarkForest", "Water", "Fire", "Wind", "Famine", "Maladie", "Guerre", "Mort", "Misere", "Tromperie",
		"Passion", "Vice", "Folie"
	};

	public static string[] BALL_LEVEL = new string[3] { "Guerre", "Tromperie", "Passion" };

	public static bool[] LOCKED_CHAR = new bool[11]
	{
		false, false, false, false, true, true, true, true, false, true,
		false
	};

	public static bool[] LOCKED_LEVEL = new bool[13]
	{
		true, true, true, true, true, true, true, true, true, false,
		true, true, true
	};

	public static int CurrentMusic = 0;

	private static bool SimulateTrialMode = false;

	public static void SetSimulateTrialMode(bool bTrial)
	{
		Guide.SimulateTrialMode = bTrial;
	}

	public static bool IsTrialMode()
	{
		return Guide.IsTrialMode;
	}
}
