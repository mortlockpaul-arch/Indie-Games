using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Defenders;

internal class Confirmation
{
	public string text;

	public bool Active;

	public GameState gsTarget;

	public float transp;

	public Vector2 position;

	private float transpTarget;

	private float selection;

	private float selectionTarget;

	private bool finish;

	private GameState gs;

	private MouseState currentMouseState;

	private MouseState oldMouseState;

	private KeyboardState currentKeyboardState;

	private KeyboardState oldKeyboardState;

	private GamePadState currentGamePadState;

	private GamePadState oldGamePadState;

	public Confirmation()
	{
		Initialize();
	}

	public void Initialize()
	{
		text = "";
		Active = false;
		transp = 0f;
		transpTarget = 0f;
		finish = false;
		selectionTarget = 0f;
	}

	public void CreateConfirmation(string text, GameState gsTarget, GameState gs)
	{
		this.text = text;
		this.gsTarget = gsTarget;
		this.gs = gs;
		Active = true;
	}

	public GameState Update(PlayerIndex controllingPlayer, Vector2 pos)
	{
		oldMouseState = currentMouseState;
		oldKeyboardState = currentKeyboardState;
		oldGamePadState = currentGamePadState;
		currentMouseState = Mouse.GetState();
		currentKeyboardState = Keyboard.GetState();
		currentGamePadState = GamePad.GetState(controllingPlayer);
		position = pos;
		Rectangle rectangle = ((!(currentMouseState != oldMouseState)) ? new Rectangle(-5, -5, 2, 2) : new Rectangle(currentMouseState.X - 5, currentMouseState.Y - 5, 2, 2));
		Rectangle value = new Rectangle((int)pos.X - 30, (int)pos.Y + 80, 60, 40);
		Rectangle value2 = new Rectangle((int)(pos.X - 30f), (int)pos.Y + 30, 60, 40);
		GameState result = gs;
		if (Active)
		{
			if (finish)
			{
				transpTarget = 0f;
			}
			else
			{
				transpTarget = 1f;
			}
		}
		else
		{
			transpTarget = 0f;
		}
		if (finish && transp < 0.1f)
		{
			if (Active)
			{
				result = gsTarget;
			}
			Initialize();
		}
		if (transp > 0.9f)
		{
			if (currentKeyboardState.IsKeyDown(Keys.Up) || currentKeyboardState.IsKeyDown(Keys.W) || currentGamePadState.DPad.Up == ButtonState.Pressed || currentGamePadState.ThumbSticks.Left.Y > 0.33f || rectangle.Intersects(value2))
			{
				selectionTarget = 1f;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Down) || currentKeyboardState.IsKeyDown(Keys.S) || currentGamePadState.DPad.Down == ButtonState.Pressed || currentGamePadState.ThumbSticks.Left.Y < -0.33f || rectangle.Intersects(value))
			{
				selectionTarget = 0f;
			}
			if ((currentKeyboardState != oldKeyboardState && (currentKeyboardState.IsKeyDown(Keys.Enter) || currentKeyboardState.IsKeyDown(Keys.Space))) || (oldGamePadState.Buttons.A != ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Pressed) || ((rectangle.Intersects(value2) || rectangle.Intersects(value)) && currentMouseState.LeftButton == ButtonState.Pressed))
			{
				if (selectionTarget == 0f)
				{
					Active = false;
					finish = true;
				}
				else
				{
					Active = true;
					finish = true;
				}
			}
			if (currentKeyboardState.IsKeyDown(Keys.Escape) || currentGamePadState.Buttons.B == ButtonState.Pressed || currentGamePadState.Buttons.Back == ButtonState.Pressed)
			{
				Active = false;
				finish = true;
			}
		}
		transp = MathHelper.Lerp(transp, transpTarget, 0.2f);
		selection = MathHelper.Lerp(selection, selectionTarget, 0.5f);
		return result;
	}

	public void Draw(SpriteBatch spriteBatch, SpriteFont font)
	{
		spriteBatch.DrawString(font, text, position, new Color(transp * 0.5f, transp * 0.8f, transp, transp), 0f, new Vector2(font.MeasureString(text).X / 2f, 0f), 1f, SpriteEffects.None, 0f);
		spriteBatch.DrawString(font, "Yes", new Vector2(position.X, position.Y + 50f), new Color((selection * 0.5f + 0.5f) * transp, (selection * 0.5f + 0.5f) * transp, (selection * 0.5f + 0.5f) * transp, (selection * 0.5f + 0.5f) * transp), 0f, new Vector2(font.MeasureString("Yes").X / 2f, 0f), selection * 0.5f + 0.7f, SpriteEffects.None, 0f);
		spriteBatch.DrawString(font, "No", new Vector2(position.X, position.Y + 100f), new Color(((1f - selection) * 0.5f + 0.5f) * transp, ((1f - selection) * 0.5f + 0.5f) * transp, ((1f - selection) * 0.5f + 0.5f) * transp, ((1f - selection) * 0.5f + 0.5f) * transp), 0f, new Vector2(font.MeasureString("No").X / 2f, 0f), (1f - selection) * 0.5f + 0.7f, SpriteEffects.None, 0f);
	}
}
