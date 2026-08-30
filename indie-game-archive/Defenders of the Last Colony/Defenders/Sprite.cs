using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

internal class Sprite
{
	public Texture2D texture;

	public Vector2 position;

	public float angle;

	public Vector2 size;

	public Vector2 origin = new Vector2(0f, 0f);

	public Color color = Color.White;

	public float transparency = 1f;

	public float depth = 0.75f;

	public float scroll = 0f;

	public int Width => texture.Width;

	public int Height => texture.Height;

	public void Initialize(Texture2D texture, Vector2 position, Vector2 size, float angle)
	{
		this.texture = texture;
		this.position = position;
		this.angle = angle;
		this.size = size;
	}

	public void Update()
	{
		color = new Color(transparency, transparency, transparency, transparency);
	}

	public void UpdateScrollY()
	{
		if (scroll > (float)Height)
		{
			scroll -= Height;
		}
		if (scroll < (float)(-Height))
		{
			scroll += Height;
		}
	}

	public void UpdateScrollX()
	{
		if (scroll > (float)Width)
		{
			scroll -= Width;
		}
		if (scroll < (float)(-Width))
		{
			scroll += Width;
		}
	}

	public void Draw(SpriteBatch spriteBatch, float transp, float depthLayer)
	{
		try
		{
			spriteBatch.Draw(texture, position, null, new Color(transp, transp, transp, transp), angle, origin, size, SpriteEffects.None, depthLayer);
		}
		catch
		{
		}
	}

	public void Draw(SpriteBatch spriteBatch, float transp)
	{
		spriteBatch.Draw(texture, position, null, Color.White * transp, angle, origin, size, SpriteEffects.None, depth);
	}

	public void DrawScrollY(SpriteBatch spriteBatch, float transp)
	{
		spriteBatch.Draw(texture, position - Vector2.UnitY * Height, null, new Color(transp, transp, transp, transp), angle, origin, size, SpriteEffects.None, depth);
		spriteBatch.Draw(texture, position, null, new Color(transp, transp, transp, transp), angle, origin, size, SpriteEffects.None, depth);
		spriteBatch.Draw(texture, position + Vector2.UnitY * Height, null, new Color(transp, transp, transp, transp), angle, origin, size, SpriteEffects.None, depth);
	}

	public void DrawScrollX(SpriteBatch spriteBatch, float transp)
	{
		spriteBatch.Draw(texture, position - Vector2.UnitX * Width, null, new Color(transp, transp, transp, transp), angle, origin, size, SpriteEffects.None, depth);
		spriteBatch.Draw(texture, position, null, new Color(transp, transp, transp, transp), angle, origin, size, SpriteEffects.None, depth);
		spriteBatch.Draw(texture, position + Vector2.UnitX * Width, null, new Color(transp, transp, transp, transp), angle, origin, size, SpriteEffects.None, depth);
	}
}
