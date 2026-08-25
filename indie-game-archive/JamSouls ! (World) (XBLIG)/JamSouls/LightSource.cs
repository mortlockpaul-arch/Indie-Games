using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JamSouls;

public class LightSource
{
	private Vector2 position;

	private SpriteBatch m_Batch;

	private Vector2 m_Center;

	private float m_Scale;

	private float range;

	private Color color;

	private Texture2D lightTexture;

	public Vector2 Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
		}
	}

	public float Range
	{
		get
		{
			return range;
		}
		set
		{
			range = value;
		}
	}

	public Color Color
	{
		get
		{
			return color;
		}
		set
		{
			color = value;
		}
	}

	public Texture2D LightTexture
	{
		get
		{
			return lightTexture;
		}
		set
		{
			lightTexture = value;
		}
	}

	public LightSource(SpriteBatch spriteBatch, Texture2D texture, Color color, float range, Vector2 position)
	{
		lightTexture = texture;
		this.color = color;
		this.range = range;
		this.position = position;
		m_Batch = spriteBatch;
		m_Center = new Vector2(lightTexture.Width / 2, lightTexture.Height / 2);
		m_Scale = range / ((float)lightTexture.Width / 2f);
	}

	public void Draw()
	{
		m_Batch.Draw(lightTexture, position, null, color, 0f, m_Center, m_Scale, SpriteEffects.None, 1f);
	}
}
