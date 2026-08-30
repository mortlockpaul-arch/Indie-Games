using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Defenders;

internal class PlayableCharacter
{
	public Player player;

	public int id;

	public PlayerIndex index;

	public bool Active;

	public bool ready;

	public bool selected = false;

	public Color shootColor;

	public int direction;

	public int oldDireccion;

	public int menuCounter;

	private Rectangle card = Rectangle.Empty;

	private MouseState currentMouseState;

	private MouseState oldMouseState;

	private KeyboardState currentKeyboardState;

	private KeyboardState oldKeyboardState;

	private GamePadState currentGamePadState;

	private GamePadState oldGamePadState;

	public PlayableCharacter(Player player, int id, PlayerIndex index)
	{
		this.id = id;
		this.index = index;
		UpdatePlayer(player);
		switch (index)
		{
		case PlayerIndex.One:
			shootColor = Color.LightCyan;
			break;
		case PlayerIndex.Two:
			shootColor = new Color(1f, 0.37f, 0f);
			break;
		case PlayerIndex.Three:
			shootColor = new Color(0f, 0.86f, 0.01f);
			break;
		case PlayerIndex.Four:
			shootColor = new Color(0.86f, 0f, 0.84f);
			break;
		}
	}

	public void UpdatePlayer(Player p)
	{
		player = new Player(id);
		player = p;
	}

	public void UpdateSelection(bool useKeyboardControls, Vector2 mousePos)
	{
		oldMouseState = currentMouseState;
		oldKeyboardState = currentKeyboardState;
		oldGamePadState = currentGamePadState;
		currentMouseState = Mouse.GetState();
		currentKeyboardState = Keyboard.GetState();
		currentGamePadState = GamePad.GetState(index);
		player.Active = Active;
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
		player.Active = Active;
		if (player.Active)
		{
			player.Health = player.maximunHealth;
		}
		else
		{
			player.Health = 0f;
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
