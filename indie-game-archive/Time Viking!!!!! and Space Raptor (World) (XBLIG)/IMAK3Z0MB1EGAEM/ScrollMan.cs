using Microsoft.Xna.Framework;

namespace IMAK3Z0MB1EGAEM;

public class ScrollMan
{
	public static float zoom = 1f;

	public static Vector2 screenSize;

	public static Vector2 scroll;

	public static Vector2 camOffset = default(Vector2);

	public static float camOffsetFrame = 0f;

	public static float camZoom = 0f;

	public static float camZoomFrame = 0f;

	public static Vector2 GetRealLoc(Vector2 loc, float layer)
	{
		return (loc - screenSize / 2f) / (zoom * layer) + scroll;
	}

	public static Vector2 GetScreenLoc(Vector2 loc, float layer)
	{
		return (loc - scroll) * zoom * layer + screenSize / 2f;
	}
}
