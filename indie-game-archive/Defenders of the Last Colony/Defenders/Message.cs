using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

internal class Message
{
	public string text;

	public int counter;

	public float transp = 0f;

	public SpriteFont sf;

	public Vector2 position;

	private Vector2 position0;

	private float scale;

	private Texture2D txBackground;

	private MessageType type;

	public bool Active = true;

	public Message(string text, int counter, SpriteFont spritefont, Texture2D txBackground, Vector2 position, SoundEffect se, float volume, MessageType type)
	{
		Initialize(text, counter, spritefont, txBackground, position, se, volume, type);
	}

	public Message(string text, int counter, SpriteFont spritefont, Texture2D txBackground, Vector2 position, SoundEffect se, float volume)
	{
		Initialize(text, counter, spritefont, txBackground, position, se, volume, MessageType.normal);
	}

	public void Initialize(string text, int counter, SpriteFont spritefont, Texture2D txBackground, Vector2 position, SoundEffect se, float volume, MessageType type)
	{
		Active = true;
		this.text = text;
		this.counter = counter;
		sf = spritefont;
		this.position = position;
		position0 = position;
		this.txBackground = txBackground;
		this.type = type;
		se.Play(volume / 100f, 0f, 0f);
		scale = 0f;
	}

	public void reset()
	{
	}

	public void Update(int n)
	{
		if (type == MessageType.normal)
		{
			if (counter > 0)
			{
				transp = MathHelper.Lerp(transp, 1f, 0.1f);
				scale = MathHelper.Lerp(scale, 1f, 0.1f);
			}
			else
			{
				transp = MathHelper.Lerp(transp, -0.02f, 0.2f);
				scale = MathHelper.Lerp(scale, 6f, 0.1f);
			}
			position.Y = MathHelper.Lerp(position.Y, position0.Y + (float)n * 55f, 0.15f);
			counter--;
			if (counter <= 0 && transp < 0.01f)
			{
				Active = false;
			}
		}
	}

	public void Draw(SpriteBatch sb, float opac)
	{
		if (type == MessageType.normal)
		{
			Vector2 vector = sf.MeasureString(text) * scale * new Vector2(0.11f, 0.2f);
			sb.Draw(txBackground, position, null, new Color(transp, transp, transp, transp) * 0.5f * opac, 0f, new Vector2((float)txBackground.Width / 2f, (float)txBackground.Height / 2f), vector, SpriteEffects.None, 0.1f);
			sb.DrawString(sf, text, position, new Color(transp * 0.5f, transp * 0.75f, transp, transp) * (opac * 0.5f + 0.25f), 0f, new Vector2(sf.MeasureString(text).X / 2f, sf.MeasureString(text).Y / 2f), scale, SpriteEffects.None, 0f);
		}
		else
		{
			Vector2 vector = sf.MeasureString(text) * scale * new Vector2(0.11f, 0.2f);
			sb.Draw(txBackground, position, null, new Color(transp, transp, transp, transp) * 0.5f * opac, 0f, new Vector2((float)txBackground.Width / 2f, (float)txBackground.Height / 2f), vector, SpriteEffects.None, 0.1f);
			sb.DrawString(sf, text, position, new Color(transp * 0.5f, transp * 0.75f, transp, transp) * (opac * 0.5f + 0.25f), 0f, new Vector2(sf.MeasureString(text).X / 2f, sf.MeasureString(text).Y / 2f), scale, SpriteEffects.None, 0f);
		}
	}
}
