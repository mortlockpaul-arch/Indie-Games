using xCharEdit.Character;

namespace Viking_x86.character;

public class CharDefMgr
{
	public const int DEF_VIKING = 0;

	public const int DEF_RAPTOR = 1;

	public const int DEF_ROBOT = 2;

	public const int DEF_INVADER = 3;

	public const int DEF_BOMB = 4;

	public const int DEF_NEKO = 5;

	public const int DEF_GALAGA = 6;

	public const int DEF_BLARTARD = 7;

	public const int DEF_MEGA_BLARTARD = 8;

	public static CharDef[] charDef;

	public static void Initialize()
	{
		charDef = new CharDef[3];
		for (int i = 0; i < charDef.Length; i++)
		{
			charDef[i] = new CharDef();
		}
		ReadCharDef(0, "viking");
		ReadCharDef(1, "raptor");
		ReadCharDef(2, "robot");
	}

	private static void ReadCharDef(int ID, string path)
	{
		charDef[ID].path = "chardef/" + path + ".zsx";
		charDef[ID].ReadShort(abs: true);
	}
}
