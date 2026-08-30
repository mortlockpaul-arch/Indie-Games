using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Defenders;

internal class Selection
{
	public int id;

	public bool Active;

	public bool selected;

	public bool ready = false;

	public int direction;

	public int oldDireccion;

	public int menuCounter;

	private Rectangle card = Rectangle.Empty;

	public PlayerIndex index;

	private MouseState currentMouseState;

	private MouseState oldMouseState;

	private KeyboardState currentKeyboardState;

	private KeyboardState oldKeyboardState;

	private GamePadState currentGamePadState;

	private GamePadState oldGamePadState;

	public Selection(int id, PlayerIndex index)
	{
		this.id = id;
		this.index = index;
	}

	public void UpdateSelection(bool useKeyboardControls, Vector2 mousePos)
	{
		oldMouseState = currentMouseState;
		oldKeyboardState = currentKeyboardState;
		oldGamePadState = currentGamePadState;
		currentMouseState = Mouse.GetState();
		currentKeyboardState = Keyboard.GetState();
		currentGamePadState = GamePad.GetState(index);
		direction = 0;
		menuCounter--;
		if (menuCounter < 0)
		{
			menuCounter = 0;
		}
		if (isMouseOn(mousePos) && currentMouseState.LeftButton != oldMouseState.LeftButton && currentMouseState.LeftButton == ButtonState.Pressed)
		{
			select(sel: true);
		}
		if (isMouseOn(mousePos) && currentMouseState.RightButton != oldMouseState.RightButton && currentMouseState.RightButton == ButtonState.Pressed)
		{
			select(sel: false);
		}
		if (currentGamePadState != oldGamePadState && currentGamePadState.Buttons.A == ButtonState.Pressed)
		{
			select(sel: true);
		}
		if (currentGamePadState != oldGamePadState && currentGamePadState.Buttons.B == ButtonState.Pressed)
		{
			select(sel: false);
		}
		if (menuCounter == 0 && currentGamePadState != oldGamePadState && (GamePad.GetState(index).ThumbSticks.Left.X > 0.75f || GamePad.GetState(index).DPad.Right == ButtonState.Pressed))
		{
			direction = 1;
			menuCounter = 10;
		}
		if (menuCounter == 0 && currentGamePadState != oldGamePadState && (GamePad.GetState(index).ThumbSticks.Left.X < -0.75f || GamePad.GetState(index).DPad.Left == ButtonState.Pressed))
		{
			direction = -1;
			menuCounter = 10;
		}
		if (useKeyboardControls)
		{
			if (currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Enter))
			{
				select(sel: true);
			}
			if (currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Escape))
			{
				select(sel: false);
			}
			if (menuCounter == 0 && currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Right))
			{
				direction = 1;
				menuCounter = 10;
			}
			if (menuCounter == 0 && currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Left))
			{
				direction = -1;
				menuCounter = 10;
			}
		}
		if (selected)
		{
			direction = 0;
		}
		if (direction != oldDireccion && direction == 1 && Active)
		{
			id++;
		}
		if (direction != oldDireccion && direction == -1 && Active)
		{
			id--;
		}
		if (id > 3)
		{
			id = 0;
		}
		if (id < 0)
		{
			id = 3;
		}
		oldDireccion = direction;
	}

	private void select(bool sel)
	{
		if (sel)
		{
			if (Active)
			{
				if (selected)
				{
					ready = true;
				}
				else
				{
					selected = true;
				}
			}
			else
			{
				Active = true;
			}
		}
		else if (selected)
		{
			if (ready)
			{
				ready = false;
			}
			else
			{
				selected = false;
			}
		}
		else
		{
			Active = false;
		}
	}

	public bool isMouseOn(Vector2 mousePos)
	{
		return new Rectangle((int)mousePos.X - 1, (int)mousePos.Y - 1, 2, 2).Intersects(card);
	}
}
