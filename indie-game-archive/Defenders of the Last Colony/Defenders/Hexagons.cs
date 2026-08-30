using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

internal class Hexagons
{
	public Texture2D texture;

	public Vector2 position;

	public float angle;

	public Vector2 size;

	public float distance;

	private Color color;

	private bool active;

	public int type;

	public int Width => texture.Width;

	public int Height => texture.Height;

	public void Initialize(ContentManager Content, int type, Vector2 position, Vector2 size, float angle)
	{
		this.type = type;
		switch (type)
		{
		case 0:
			texture = Content.Load<Texture2D>("Graphics/Levels/hexagon");
			break;
		case 1:
			texture = Content.Load<Texture2D>("Graphics/Levels/hexagonWall");
			break;
		}
		this.position = position;
		this.angle = angle;
		this.size = size;
		active = true;
	}

	public void Update(Vector2 playerPosition)
	{
		distance = MathHelper.Lerp(distance, Vector2.Distance(position, playerPosition), 0.2f);
		switch (type)
		{
		case 0:
			if (distance < 200f)
			{
				color = new Color((200f - distance) / 200f + 0f, (200f - distance) / 200f + 0f, (200f - distance) / 200f + 0f, (200f - distance) / 200f + 0f);
				size = new Vector2((200f - distance) / 200f + 0f, (200f - distance) / 200f + 0f);
			}
			else
			{
				color = new Color(0, 0, 0, 0);
				size = new Vector2(0f, 0f);
			}
			break;
		case 1:
			if (distance < 500f)
			{
				color = new Color((300f - distance) / 600f + 0.5f, (300f - distance) / 600f + 0.5f, (300f - distance) / 600f + 0.5f, (300f - distance) / 600f + 0.5f);
				size = new Vector2((300f - distance) / 600f + 0.5f, (300f - distance) / 600f + 0.5f);
			}
			else
			{
				color = new Color(0, 0, 0, 0);
				size = new Vector2(0f, 0f);
			}
			break;
		}
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		if (active)
		{
			spriteBatch.Draw(texture, position, null, color, angle, new Vector2((float)Width / 2f, (float)Height / 2f), size, SpriteEffects.None, 1f);
		}
	}
}
