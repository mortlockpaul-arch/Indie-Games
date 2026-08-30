using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlatformerFromHell.Asset_Classes;

internal class StartAsset : Asset
{
	public StartAsset(Level level, Vector2 position, string texturename, int frameCount, Dir flip)
		: base(level, position, texturename, frameCount, flip)
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
