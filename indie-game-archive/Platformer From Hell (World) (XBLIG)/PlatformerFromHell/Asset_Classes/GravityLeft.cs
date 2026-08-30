using Microsoft.Xna.Framework;

namespace PlatformerFromHell.Asset_Classes;

internal class GravityLeft : Gravity
{
	public GravityLeft(Level level, Vector2 position, string texturename, int frameCount, Dir flip)
		: base(level, position, texturename, frameCount, flip)
	{
		gravDir = GravDir.Left;
	}
}
