using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Eyehook.Framework;

public class StillSprite : Sprite
{
	protected readonly Rectangle? srcRect;

	protected readonly Vector2 origin;

	public StillSprite(Texture2D texture)
		: base(texture)
	{
		srcRect = null;
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
	}

	public StillSprite(Texture2D texture, Rectangle? srcRect, Vector2? origin)
		: base(texture)
	{
		this.srcRect = srcRect;
		if (!origin.HasValue)
		{
			if (srcRect.HasValue)
			{
				Rectangle value = srcRect.Value;
				this.origin = new Vector2(value.Width / 2, value.Height / 2);
			}
			else
			{
				this.origin = new Vector2(texture.Width / 2, texture.Height / 2);
			}
		}
		else
		{
			this.origin = origin.Value;
		}
	}

	public override void Update(GameTime gameTime)
	{
	}

	public override void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float scale)
	{
		spriteBatch.Draw(texture, position, srcRect, color, Rotation, origin, scale, SpriteEffects, 0f);
	}
}
