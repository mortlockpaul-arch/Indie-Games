using Microsoft.Xna.Framework;

namespace PlatformerFromHell.Asset_Classes;

internal class Wall : Asset
{
	public Wall(Level level, Vector2 position, string texturename, int frameCount, Dir newFlip)
		: base(level, position, texturename, frameCount, newFlip)
	{
	}
}
