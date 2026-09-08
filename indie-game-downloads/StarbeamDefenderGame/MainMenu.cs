using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarbeamDefenderGame;

public class MainMenu
{
	private SpriteFont menufont;

	private Texture2D backgroundtexture;

	private int keyboarddelay;

	private int selecteditem;

	public void LoadContent(ContentManager content, string backgroundfile)
	{
		menufont = content.Load<SpriteFont>("Fonts/Main");
		backgroundtexture = content.Load<Texture2D>(backgroundfile);
	}

	public void Reset()
	{
		keyboarddelay = 700;
		selecteditem = 0;
	}

	private bool IsKeyDown(PlayerIndex controllerindex, Buttons button, Keys key)
	{
		GamePadState state = GamePad.GetState(controllerindex);
		KeyboardState state2 = Keyboard.GetState();
		if (state.IsButtonDown(button))
		{
			return true;
		}
		if (state2.IsKeyDown(key))
		{
			return true;
		}
		return false;
	}

	private int GetInputMove(PlayerIndex controller)
	{
		GamePadState state = GamePad.GetState(controller);
		if (state.ThumbSticks.Left.Y < -0.2f)
		{
			keyboarddelay = 250;
			return 1;
		}
		if (state.ThumbSticks.Left.Y > 0.2f)
		{
			keyboarddelay = 250;
			return -1;
		}
		if (state.ThumbSticks.Right.Y < -0.2f)
		{
			keyboarddelay = 250;
			return 1;
		}
		if (state.ThumbSticks.Right.Y > 0.2f)
		{
			keyboarddelay = 250;
			return -1;
		}
		if (state.IsButtonDown(Buttons.DPadDown))
		{
			keyboarddelay = 250;
			return 1;
		}
		if (state.IsButtonDown(Buttons.DPadUp))
		{
			keyboarddelay = 250;
			return -1;
		}
		if (controller == PlayerIndex.One)
		{
			KeyboardState state2 = Keyboard.GetState();
			if (state2.IsKeyDown(Keys.Up))
			{
				keyboarddelay = 250;
				return -1;
			}
			if (state2.IsKeyDown(Keys.Down))
			{
				keyboarddelay = 250;
				return 1;
			}
		}
		return 0;
	}

	public int DoGameUpdate(int timems, PlayerIndex mastercontroller)
	{
		if (keyboarddelay > 0)
		{
			keyboarddelay -= timems;
			return 0;
		}
		selecteditem += GetInputMove(mastercontroller);
		if (selecteditem < 0)
		{
			selecteditem = 0;
		}
		if (selecteditem > 3)
		{
			selecteditem = 3;
		}
		if (IsKeyDown(mastercontroller, Buttons.A, Keys.Enter))
		{
			return selecteditem + 1;
		}
		return 0;
	}

	public void Draw(SpriteBatch spritebatch)
	{
		Rectangle value = new Rectangle(0, 0, 1280, 720);
		spritebatch.Draw(destinationRectangle: new Rectangle(0, 0, 1280, 720), texture: backgroundtexture, sourceRectangle: value, color: Color.White);
		if (selecteditem == 0)
		{
			DrawCenteredText(spritebatch, "Start Game", new Vector2(640f, 200f), Color.Green);
		}
		else
		{
			DrawCenteredText(spritebatch, "Start Game", new Vector2(640f, 200f), Color.Red);
		}
		if (selecteditem == 1)
		{
			DrawCenteredText(spritebatch, "Highscores", new Vector2(640f, 250f), Color.Green);
		}
		else
		{
			DrawCenteredText(spritebatch, "Highscores", new Vector2(640f, 250f), Color.Red);
		}
		if (selecteditem == 2)
		{
			DrawCenteredText(spritebatch, "Screen Setup", new Vector2(640f, 300f), Color.Green);
		}
		else
		{
			DrawCenteredText(spritebatch, "Screen Setup", new Vector2(640f, 300f), Color.Red);
		}
		if (selecteditem == 3)
		{
			DrawCenteredText(spritebatch, "Exit Game", new Vector2(640f, 350f), Color.Green);
		}
		else
		{
			DrawCenteredText(spritebatch, "Exit Game", new Vector2(640f, 350f), Color.Red);
		}
	}

	private void DrawCenteredText(SpriteBatch spritebatch, string text, Vector2 position, Color color)
	{
		Vector2 vector = menufont.MeasureString(text);
		spritebatch.DrawString(menufont, text, position -= vector / 2f, color);
	}
}
