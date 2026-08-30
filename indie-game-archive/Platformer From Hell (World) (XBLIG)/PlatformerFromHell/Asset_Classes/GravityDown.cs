using Microsoft.Xna.Framework;

namespace PlatformerFromHell.Asset_Classes;

internal class GravityDown : Gravity
{
	public GravityDown(Level level, Vector2 position, string texturename, int frameCount, Dir newFlip)
		: base(level, position, texturename, frameCount, newFlip)
	{
		gravDir = GravDir.Down;
	}
}
