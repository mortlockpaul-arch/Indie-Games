using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Eyehook.Framework;

public abstract class Sprite
{
	public float Rotation;

	public SpriteEffects SpriteEffects;

	protected readonly Texture2D texture;

	public virtual int Width => texture.Width;

	public virtual int Height => texture.Height;

	public Sprite(Texture2D texture)
	{
		this.texture = texture;
	}

	public abstract void Update(GameTime gameTime);

	public void Draw(SpriteBatch spriteBatch, Vector2 position)
	{
		Draw(spriteBatch, position, Color.White, 1f);
	}

	public abstract void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float scale);
}
