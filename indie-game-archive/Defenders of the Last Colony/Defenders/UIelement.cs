using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Defenders;

internal class UIelement
{
	public bool Active;

	public bool usable;

	public bool selected;

	public bool clicked;

	public bool inScreen;

	public bool keepSelected;

	public Vector2 position;

	public string text;

	public int ID;

	private SpriteFont font;

	private Texture2D tx;

	private Rectangle rec;

	private float width;

	private float height;

	private float scale;

	private Vector2 alignement;

	private string message;

	private MouseState currentMouseState;

	private MouseState oldMouseState;

	public UIelement(int ID, bool usable, bool keepSelected, string text, Vector2 position, SpriteFont font, Texture2D background, float scale, Vector2 alignement)
	{
		this.ID = ID;
		this.keepSelected = keepSelected;
		this.usable = usable;
		this.text = text;
		this.position = position;
		this.font = font;
		this.scale = scale;
		this.alignement = alignement;
		tx = background;
		width = font.MeasureString(text).X * scale;
		height = font.MeasureString(text).Y * scale;
		rec = new Rectangle((int)(position.X - width / 2f * alignement.X), (int)(position.Y - height / 2f * alignement.X), (int)width, (int)height);
	}

	public int Update()
	{
		currentMouseState = Mouse.GetState();
		message = "NULL";
		int result = -1;
		Rectangle rectangle = new Rectangle(currentMouseState.X - 1, currentMouseState.Y - 1, 2, 2);
		Rectangle value = new Rectangle(65, 77, 1150, 643);
		Active = false;
		clicked = false;
		inScreen = false;
		if (rectangle.Intersects(rec))
		{
			Active = true;
			if (currentMouseState.LeftButton == ButtonState.Pressed || currentMouseState.RightButton == ButtonState.Pressed)
			{
				clicked = true;
				result = ID;
				message = text;
			}
		}
		if (rectangle.Intersects(value))
		{
			inScreen = true;
		}
		if (selected)
		{
			message = text;
		}
		oldMouseState = currentMouseState;
		if (!usable)
		{
			result = -1;
		}
		return result;
	}

	public void Draw(SpriteBatch sb)
	{
		if (Active && usable)
		{
			if (clicked)
			{
				sb.Draw(tx, position, null, Color.Red, 0f, new Vector2(tx.Width, tx.Height) * alignement, font.MeasureString(text) / 80f * scale, SpriteEffects.None, 0.6f);
				sb.DrawString(font, text, position, Color.LightCyan, 0f, font.MeasureString(text) * alignement, scale, SpriteEffects.None, 0.5f);
			}
			else
			{
				sb.Draw(tx, position, null, Color.White, 0f, new Vector2(tx.Width, tx.Height) * alignement, font.MeasureString(text) / 80f * scale, SpriteEffects.None, 0.6f);
				sb.DrawString(font, text, position, Color.White, 0f, font.MeasureString(text) * alignement, scale, SpriteEffects.None, 0.5f);
			}
		}
		else
		{
			sb.Draw(tx, position, null, Color.Black * 0.5f, 0f, new Vector2(tx.Width, tx.Height) * alignement, font.MeasureString(text) / 80f * scale, SpriteEffects.None, 0.6f);
			sb.DrawString(font, text, position, Color.DarkGray, 0f, font.MeasureString(text) * alignement, scale, SpriteEffects.None, 0.5f);
		}
		if (selected && usable)
		{
			sb.DrawString(font, "___", position, Color.White * 0.75f, 0f, font.MeasureString("___") * (alignement * 1f), new Vector2(font.MeasureString(text).X / 65f, 1f) * scale, SpriteEffects.None, 0.5f);
			sb.DrawString(font, "___", position, Color.White * 0.75f, 0f, font.MeasureString("___") * (alignement * 1f), new Vector2(font.MeasureString(text).X / 65f, 1f) * scale, SpriteEffects.FlipVertically, 0.5f);
		}
	}
}
