using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlatformerFromHell.Asset_Classes;

internal class ExitAsset : Asset
{
	public ExitAsset(Level level, Vector2 position, string texturename, int frameCount, Dir newFlip)
		: base(level, position, texturename, frameCount, newFlip)
	{
	}

	public override void LoadContent()
	{
		base.LoadContent();
	}

	public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
	{
		base.Draw(gameTime, spriteBatch);
	}
}
