using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarbeamDefenderGame.Menus;

internal class SetupMenu
{
	private SpriteFont menufont;

	private int keyboarddelay;

	private int playertwoindex;

	private PlayerIndex mastercontroller;

	private Texture2D[] worldstexture;

	private int selecteditem;

	private int difficulty;

	private int world;

	public int PlayerTwoController => playertwoindex;

	public int Difficulty => difficulty;

	private string DifficultyName => difficulty switch
	{
		0 => "Easy", 
		1 => "Medium", 
		2 => "Hard", 
		_ => "Unknown", 
	};

	private string WorldName => world switch
	{
		0 => "Eris", 
		1 => "Amun", 
		2 => "Helios", 
		3 => "Boreas", 
		_ => "Unknown", 
	};

	public void LoadContent(ContentManager content, string backgroundfile)
	{
		menufont = content.Load<SpriteFont>("Fonts/Main");
		worldstexture = new Texture2D[4];
		worldstexture[0] = content.Load<Texture2D>("Menus/LevelsImages/SetupEris");
		worldstexture[1] = content.Load<Texture2D>("Menus/LevelsImages/SetupAmun");
		worldstexture[2] = content.Load<Texture2D>("Menus/LevelsImages/SetupHelios");
		worldstexture[3] = content.Load<Texture2D>("Menus/LevelsImages/SetupBoreas");
	}

	public void Reset(PlayerIndex mastercontroller)
	{
		this.mastercontroller = mastercontroller;
		keyboarddelay = 700;
		selecteditem = 0;
		playertwoindex = -1;
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

	private bool IsKeyDown(PlayerIndex controllerindex, Buttons button, Keys key, Buttons alternate)
	{
		GamePadState state = GamePad.GetState(controllerindex);
		KeyboardState state2 = Keyboard.GetState();
		if (state.IsButtonDown(button))
		{
			return true;
		}
		if (state.IsButtonDown(alternate))
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
		if (state.ThumbSticks.Left.X < -0.2f)
		{
			keyboarddelay = 250;
			return -1;
		}
		if (state.ThumbSticks.Left.X > 0.2f)
		{
			keyboarddelay = 250;
			return 1;
		}
		if (state.ThumbSticks.Right.X < -0.2f)
		{
			keyboarddelay = 250;
			return -1;
		}
		if (state.ThumbSticks.Right.X > 0.2f)
		{
			keyboarddelay = 250;
			return 1;
		}
		if (state.IsButtonDown(Buttons.DPadRight))
		{
			keyboarddelay = 250;
			return 1;
		}
		if (state.IsButtonDown(Buttons.DPadLeft))
		{
			keyboarddelay = 250;
			return -1;
		}
		if (controller == PlayerIndex.One)
		{
			KeyboardState state2 = Keyboard.GetState();
			if (state2.IsKeyDown(Keys.Left))
			{
				keyboarddelay = 250;
				return -1;
			}
			if (state2.IsKeyDown(Keys.Right))
			{
				keyboarddelay = 250;
				return 1;
			}
		}
		return 0;
	}

	private int GetInputMoveLeftRight(PlayerIndex controller)
	{
		GamePadState state = GamePad.GetState(controller);
		if (state.ThumbSticks.Left.X > 0.2f)
		{
			keyboarddelay = 250;
			return 1;
		}
		if (state.ThumbSticks.Left.X < -0.2f)
		{
			keyboarddelay = 250;
			return -1;
		}
		if (state.ThumbSticks.Right.X > 0.2f)
		{
			keyboarddelay = 250;
			return 1;
		}
		if (state.ThumbSticks.Right.X < -0.2f)
		{
			keyboarddelay = 250;
			return -1;
		}
		if (state.IsButtonDown(Buttons.DPadLeft))
		{
			keyboarddelay = 250;
			return 1;
		}
		if (state.IsButtonDown(Buttons.DPadRight))
		{
			keyboarddelay = 250;
			return -1;
		}
		if (controller == PlayerIndex.One)
		{
			KeyboardState state2 = Keyboard.GetState();
			if (state2.IsKeyDown(Keys.Left))
			{
				keyboarddelay = 250;
				return -1;
			}
			if (state2.IsKeyDown(Keys.Right))
			{
				keyboarddelay = 250;
				return 1;
			}
		}
		return 0;
	}

	public int DoGameUpdate(int timems)
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
		if (selecteditem > 2)
		{
			selecteditem = 2;
		}
		if (IsKeyDown(mastercontroller, Buttons.A, Keys.Enter))
		{
			switch (selecteditem)
			{
			case 0:
				if (!FileAndGamerServices.gamer.IsTrialMode)
				{
					return world + 1;
				}
				if (world == 0)
				{
					return world + 1;
				}
				FileAndGamerServices.gamer.ShowPurchase(mastercontroller);
				break;
			case 1:
				world++;
				if (world > 3)
				{
					world = 0;
				}
				keyboarddelay = 250;
				break;
			case 2:
				difficulty++;
				if (difficulty > 2)
				{
					difficulty = 0;
				}
				keyboarddelay = 250;
				break;
			}
		}
		if (IsKeyDown(mastercontroller, Buttons.Back, Keys.Escape))
		{
			return -1;
		}
		if (FileAndGamerServices.gamer.IsTrialMode)
		{
			for (int i = 0; i < 4; i++)
			{
				if (IsKeyDown((PlayerIndex)i, Buttons.Y, Keys.P))
				{
					FileAndGamerServices.gamer.ShowPurchase((PlayerIndex)i);
					return 0;
				}
			}
		}
		if (playertwoindex != -1)
		{
			if (IsKeyDown((PlayerIndex)playertwoindex, Buttons.Back, Keys.Escape, Buttons.B))
			{
				playertwoindex = -1;
			}
		}
		else if (!FileAndGamerServices.gamer.IsTrialMode)
		{
			for (int j = 0; j < 4; j++)
			{
				if (j != (int)mastercontroller)
				{
					GamePad.GetState((PlayerIndex)j);
					if (IsKeyDown((PlayerIndex)j, Buttons.A, Keys.D2, Buttons.Start))
					{
						playertwoindex = j;
						break;
					}
				}
			}
		}
		return 0;
	}

	public void Draw(SpriteBatch spritebatch)
	{
		Rectangle value = new Rectangle(0, 0, 1280, 720);
		spritebatch.Draw(destinationRectangle: new Rectangle(0, 0, 1280, 720), texture: worldstexture[world], sourceRectangle: value, color: Color.White);
		if (selecteditem == 0)
		{
			DrawCenteredText(spritebatch, "Start Game", new Vector2(225f, 525f), Color.Yellow);
		}
		else
		{
			DrawCenteredText(spritebatch, "Start Game", new Vector2(225f, 525f), Color.Red);
		}
		string text = string.Empty;
		if (FileAndGamerServices.gamer.IsTrialMode & (world != 0))
		{
			text = "(Full Game Only)";
		}
		if (selecteditem == 1)
		{
			DrawCenteredText(spritebatch, "World: " + WorldName + text, new Vector2(640f, 525f), Color.Yellow);
		}
		else
		{
			DrawCenteredText(spritebatch, "World: " + WorldName + text, new Vector2(640f, 525f), Color.Red);
		}
		if (selecteditem == 2)
		{
			DrawCenteredText(spritebatch, "Difficulty: " + DifficultyName, new Vector2(1050f, 525f), Color.Yellow);
		}
		else
		{
			DrawCenteredText(spritebatch, "Difficulty: " + DifficultyName, new Vector2(1050f, 525f), Color.Red);
		}
		DrawCenteredText(spritebatch, FileAndGamerServices.gamer.PlayerNames[(int)mastercontroller], new Vector2(200f, 300f), Color.Yellow);
		DrawCenteredText(spritebatch, "Controller " + mastercontroller, new Vector2(200f, 350f), Color.Yellow);
		if (playertwoindex == -1)
		{
			if (FileAndGamerServices.gamer.IsTrialMode)
			{
				DrawCenteredText(spritebatch, "Player Two", new Vector2(1080f, 275f), Color.Green);
				DrawCenteredText(spritebatch, "Not Available In Trial", new Vector2(1080f, 325f), Color.Green);
				DrawCenteredText(spritebatch, "Press Y To Purchase", new Vector2(1080f, 375f), Color.Green);
			}
			else
			{
				DrawCenteredText(spritebatch, "Player Two", new Vector2(1080f, 300f), Color.Green);
				DrawCenteredText(spritebatch, "Press A To Join", new Vector2(1080f, 350f), Color.Green);
			}
		}
		else
		{
			DrawCenteredText(spritebatch, FileAndGamerServices.gamer.PlayerNames[playertwoindex], new Vector2(1080f, 275f), Color.Green);
			DrawCenteredText(spritebatch, "Controller " + (PlayerIndex)playertwoindex, new Vector2(1080f, 325f), Color.Green);
			DrawCenteredText(spritebatch, "Press B To Cancel", new Vector2(1080f, 375f), Color.Green);
		}
	}

	private void DrawCenteredText(SpriteBatch spritebatch, string text, Vector2 position, Color color)
	{
		Vector2 vector = menufont.MeasureString(text);
		spritebatch.DrawString(menufont, text, position -= vector / 2f, color);
	}
}
