using Microsoft.Xna.Framework;

namespace MapEdit;

public class ScrollManager
{
	public static float zoom = 1f;

	public static Vector2 screenSize;

	public static Vector2 scroll;

	public static Vector2 GetRealLoc(Vector2 loc, float layer)
	{
		return (loc - screenSize / 2f) / (zoom * layer) + scroll;
	}

	public static Vector2 GetScreenLoc(Vector2 loc, float layer)
	{
		return (loc - scroll) * zoom * layer + screenSize / 2f;
	}

	public static Vector2 GetScreenLoc(Vector3 loc, float layer)
	{
		return (new Vector2(loc.X, loc.Y - loc.Z) - scroll) * zoom * layer + screenSize / 2f;
	}
}
