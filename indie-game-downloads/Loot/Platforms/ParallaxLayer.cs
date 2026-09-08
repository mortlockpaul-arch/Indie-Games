using Eyehook.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Platforms;

public class ParallaxLayer
{
	private Texture2D texture;

	private float speed;

	private float offset;

	private int width;

	public ParallaxLayer(Texture2D texture, float speed)
	{
		this.texture = texture;
		this.speed = speed;
		width = texture.Width;
		offset = 0f;
	}

	public void Reset()
	{
		offset = 0f;
	}

	public void Update(GameTime gameTime)
	{
		offset -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
		if (!((double)offset >= (double)(-width)))
		{
			offset += width;
		}
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Begin(SpriteSortMode.Immediate, null, GraphicUtil.Wrap, null, null);
		spriteBatch.Draw(texture, new Vector2(offset, 0f), new Rectangle(0, 0, 1280 + width, 720), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		spriteBatch.End();
	}
}
