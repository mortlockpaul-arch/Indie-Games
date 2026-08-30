using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlatformerFromHell.Asset_Classes;

internal class Switch : Asset
{
	private readonly char switchCharacterID;

	public Switch(Level level, Vector2 position, string texturename, int frameCount, Dir flip, char switchID)
		: base(level, position, texturename, frameCount, flip)
	{
		switchCharacterID = switchID;
	}

	public override void LoadContent()
	{
		base.LoadContent();
	}

	public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
	{
		base.Draw(gameTime, spriteBatch);
	}

	public char GetSwitchChar()
	{
		return switchCharacterID;
	}
}
