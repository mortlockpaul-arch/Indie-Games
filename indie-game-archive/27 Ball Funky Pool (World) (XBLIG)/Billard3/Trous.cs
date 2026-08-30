using Microsoft.Xna.Framework;

namespace Billard3;

public class Trous
{
	public static Trou trouXMZP = new Trou(0, new Vector2(-30.833f, 30.833f), 1.873f, "trou_CORNER_XM_ZP");

	public static Trou trouXMZM = new Trou(1, new Vector2(-30.833f, -30.833f), 1.873f, "trou_CORNER_XM_XM");

	public static Trou trouX0ZP = new Trou(2, new Vector2(0f, 30.833f), 0.885f, "trou_CENTRAL_ZP");

	public static Trou trouX0ZM = new Trou(3, new Vector2(0f, -30.833f), 0.885f, "trou_CENTRAL_ZM");

	public static Trou trouXPZP = new Trou(4, new Vector2(30.833f, 30.833f), 1.873f, "trou_CORNER_XP_ZP");

	public static Trou trouXPZM = new Trou(5, new Vector2(30.833f, -30.833f), 1.873f, "trou_CORNER_XP_ZM");
}
