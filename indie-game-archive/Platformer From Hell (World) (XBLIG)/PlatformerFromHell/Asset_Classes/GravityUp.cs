using Microsoft.Xna.Framework;

namespace PlatformerFromHell.Asset_Classes;

internal class GravityUp : Gravity
{
	public GravityUp(Level level, Vector2 position, string texturename, int frameCount, Dir flip)
		: base(level, position, texturename, frameCount, flip)
	{
		gravDir = GravDir.Up;
	}
}
