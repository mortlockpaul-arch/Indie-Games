using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

internal class Selectable
{
	public Vector2 position;

	public Rectangle rec;

	public float angle;

	public string text;

	public string desc;

	public Color color;

	public bool unlock = false;

	private SpriteFont font;

	private Color colorBase;

	public Selectable(SpriteFont font, string text, Vector2 position, Color color)
	{
		Initialize(font, text, position, color, unlock: false);
	}

	public Selectable(SpriteFont font, string text, Vector2 position, Color color, bool unlock)
	{
		Initialize(font, text, position, color, unlock);
	}

	public void Initialize(SpriteFont font, string text, Vector2 position, Color color, bool unlock)
	{
		this.font = font;
		this.text = text;
		this.position = position;
		this.color = color;
		colorBase = color;
		this.unlock = unlock;
		rec = new Rectangle((int)position.X, (int)position.Y, (int)font.MeasureString(text).X, (int)font.MeasureString(text).Y);
	}

	public bool IsMouseOn(Rectangle mouseRec)
	{
		rec = new Rectangle((int)position.X, (int)position.Y, (int)font.MeasureString(text).X, (int)font.MeasureString(text).Y);
		bool flag = rec.Intersects(mouseRec);
		if (flag)
		{
			color = Color.White;
		}
		else
		{
			color = colorBase;
		}
		return flag;
	}

	public void Draw(SpriteBatch sb, bool unlockable, Color col)
	{
		string text = this.text;
		if (!unlock && unlockable)
		{
			text = "LOCKED";
		}
		Color color = this.color;
		if (this.color == colorBase)
		{
			color = col;
		}
		sb.DrawString(font, text, position, color);
	}
}
