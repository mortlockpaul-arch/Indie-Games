using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

internal abstract class SimpleObject
{
	public Texture2D texture;

	public Vector2 position;

	public float angle;

	public Vector2 size;

	public Vector2 origin;

	public Color color;

	public bool Active;

	public int Width => texture.Width;

	public int Height => texture.Height;

	public void Initialize(Texture2D texture, Vector2 position)
	{
		this.texture = texture;
		this.position = position;
		angle = 0f;
		size = Vector2.One;
		origin = new Vector2(0f, 0f);
		color = Color.White;
		Active = true;
	}

	public abstract void Update();

	public virtual void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(texture, position, null, color, angle, origin, size, SpriteEffects.None, 1f);
	}
}
