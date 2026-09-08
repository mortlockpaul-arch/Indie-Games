using Microsoft.Xna.Framework;

namespace JamSouls;

public static class PlayerConfig
{
	public enum SBIRE_DEF
	{
		Pas_Sbire,
		Mal_Sbire,
		Vic_Sbire,
		Fol_Sbire,
		Mis_Sbire,
		Fam_Sbire,
		Mor_Sbire,
		Gue_Sbire,
		Tro_Sbire,
		Esp_Sbire,
		NONE
	}

	public const int MAX_PLAYERS = 4;

	public const string SPAWN_FX = "Fx/Particle/SpawnBeam";

	public const string GIB_FX = "Fx/Particle/PlayerGibs";

	public const string FIRE_FX = "Fx/Particle/Burning";

	public const string BLEED_FX = "Fx/Particle/BulletGibs";

	public const string BUBBLE_FX = "Fx/Particle/Bubble";

	public const float SizeX = 34f;

	public const float SizeY = 70f;

	public const float FROG_SIZE_SCALE = 2.5f;

	public static string[] CHARACTER_NAME = new string[11]
	{
		"Passion", "Maladie", "Vice", "Folie", "Misere", "Famine", "Mort", "Guerre", "Tromperie", "Esperance",
		"Frog"
	};

	public static Color[] CHARACTER_COLOR = new Color[11]
	{
		new Color(185, 0, 38),
		Color.Green,
		new Color(74, 84, 102),
		new Color(65, 54, 27),
		new Color(181, 55, 0),
		Color.Orange,
		Color.Purple,
		Color.Red,
		new Color(234, 255, 0),
		Color.White,
		Color.GreenYellow
	};

	public static Vector2[] SCORE_POSITION = new Vector2[4]
	{
		new Vector2(80f, 10f),
		new Vector2(380f, 10f),
		new Vector2(680f, 10f),
		new Vector2(980f, 10f)
	};

	public static int[] SoulPokeX = new int[6] { 20, -20, 30, -30, 40, -40 };

	public static int[] SoulPokeY = new int[6] { -30, -40, -50, -60, -70, -80 };

	public static Color BLUE_TEAM_COLOR = new Color(9, 174, 255);

	public static Color RED_TEAM_COLOR = Color.Red;

	public static AudioClip JumpSound = null;
}
