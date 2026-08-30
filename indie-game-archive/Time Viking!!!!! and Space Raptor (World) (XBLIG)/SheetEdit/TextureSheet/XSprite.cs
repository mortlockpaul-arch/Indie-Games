using Microsoft.Xna.Framework;

namespace SheetEdit.TextureSheet;

public class XSprite
{
	public const int FLAG_NONE = 0;

	public const int FLAG_LEGS = 1;

	public const int FLAG_TORSO = 2;

	public const int FLAG_TORSO_ANCHOR = 3;

	public const int FLAG_LAYERBUMP = 4;

	public const int FLAG_GLOW = 5;

	public const int FLAG_SWAY = 6;

	public const int FLAG_GRASS = 7;

	public const int FLAG_SPACE = 8;

	public const int FLAG_GRID = 9;

	public const int FLAG_FOG = 10;

	public const int FLAG_TOP_DOOR = 11;

	public const int FLAG_LEFT_DOOR = 12;

	public const int FLAG_RIGHT_DOOR = 13;

	public const int FLAG_BOTTOM_DOOR = 14;

	public Rectangle srcRect;

	public Vector2 origin;

	public string name;

	public int flags;
}
