using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BunnyOfWar.Screens;

public class MainMenu
{
	private bool showMoreFromAwesomeEnterprises;

	private DateTime showLogoSplashUntil = DateTime.MinValue;

	private DateTime lastMonkeyBlink = DateTime.MinValue;

	private string[] menuChoices = new string[4] { "Play", "Credits", "Exit", "Buy Me!!" };

	private int currentSelection;

	public Texture2D background => GraphicsManager.LoadTexture("screens/mainscreen.png", cacheResult: true);

	public Texture2D cursor => GraphicsManager.LoadTexture("screens/cursor", cacheResult: true);

	public Texture2D buyMe => GraphicsManager.LoadTexture("screens/BuyMeText", cacheResult: true);

	public MainMenu()
	{
		Load(RandomStaticGlobals.Content);
	}

	public void Draw()
	{
		if (lastMonkeyBlink.AddMilliseconds(3000.0) < DateTime.Now)
		{
			GraphicsManager.Draw(GraphicsManager.LoadTexture("screens/mainscreenblink", cacheResult: true), new Rectangle(0, 0, 1920, 1080), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Definitions.LayerDepthForGround);
			if (lastMonkeyBlink.AddMilliseconds(4000.0) < DateTime.Now)
			{
				lastMonkeyBlink = DateTime.Now;
			}
		}
		if (RandomStaticGlobals.ScoreCurrent > 0)
		{
			GraphicsManager.DrawHighScoresForMainScreen(RandomStaticGlobals.ScoreCurrent.ToString(), RandomStaticGlobals.ScoreAllTimeHigh.ToString());
		}
		if (showMoreFromAwesomeEnterprises)
		{
			GraphicsManager.Draw(GraphicsManager.LoadTexture("screens/UnPermanent/MoreFromAwesomeEnterprises", cacheResult: true), new Rectangle(0, 0, 1920, 1080), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Definitions.LayerDepthForSky);
		}
		GraphicsManager.Draw(background, new Rectangle(0, 0, 1920, 1080), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Definitions.LayerDepthForSky);
	}

	public void ProcessInput()
	{
		InputFromAnywhere inputFromAnywhere = null;
		if (GamePad.GetState(PlayerIndex.One).IsConnected)
		{
			inputFromAnywhere = InputManager.GetPlayerInput(PlayerIndex.One, ref InputManager.gamePad1previous, ref InputManager.nullKeyboard);
			FigureOutInput(inputFromAnywhere, PlayerIndex.One);
		}
		if (GamePad.GetState(PlayerIndex.Two).IsConnected)
		{
			inputFromAnywhere = InputManager.GetPlayerInput(PlayerIndex.Two, ref InputManager.gamePad2previous, ref InputManager.nullKeyboard);
			FigureOutInput(inputFromAnywhere, PlayerIndex.Two);
		}
		if (GamePad.GetState(PlayerIndex.Three).IsConnected)
		{
			inputFromAnywhere = InputManager.GetPlayerInput(PlayerIndex.Three, ref InputManager.gamePad3previous, ref InputManager.nullKeyboard);
			FigureOutInput(inputFromAnywhere, PlayerIndex.Three);
		}
		if (GamePad.GetState(PlayerIndex.Four).IsConnected)
		{
			inputFromAnywhere = InputManager.GetPlayerInput(PlayerIndex.Four, ref InputManager.gamePad4previous, ref InputManager.nullKeyboard);
			FigureOutInput(inputFromAnywhere, PlayerIndex.Four);
		}
	}

	private void FigureOutInput(InputFromAnywhere anywhereInput, PlayerIndex pi)
	{
		if (anywhereInput == null)
		{
			return;
		}
		if (GraphicsManager.messages != null && GraphicsManager.messages.Count > 0)
		{
			if (anywhereInput.B_pressed)
			{
				GraphicsManager.messages.Clear();
			}
			return;
		}
		if (anywhereInput.A_pressed || anywhereInput.B_pressed || anywhereInput.X_pressed || anywhereInput.Y_pressed || anywhereInput.START_pressed || anywhereInput.SELECT_pressed)
		{
			selectedSomething(pi);
		}
		if (anywhereInput.RIGHT_SHOULDER_held)
		{
			_ = anywhereInput.LEFT_SHOULDER_held;
		}
	}

	private void moveUp()
	{
		currentSelection--;
		if (currentSelection < 0)
		{
			currentSelection = menuChoices.Length - 1;
		}
		if (!RandomStaticGlobals.IsTrial() && menuChoices[currentSelection] == "Buy Me!!")
		{
			currentSelection--;
		}
	}

	private void moveDown()
	{
		currentSelection++;
		if (currentSelection >= menuChoices.Length)
		{
			currentSelection = 0;
		}
		if (!RandomStaticGlobals.IsTrial() && menuChoices[currentSelection] == "Buy Me!!")
		{
			currentSelection = 0;
		}
	}

	private void selectedSomething(PlayerIndex pi)
	{
		SoundManager.PlayMenuClick();
		switch (menuChoices[currentSelection])
		{
		case "Play":
			InputManager.ClearPreviousInputs();
			FighterManager.humanPlayers.Clear();
			_ = Gamer.SignedInGamers[pi];
			try
			{
				if (Gamer.SignedInGamers[pi].IsSignedInToLive)
				{
					FighterManager.addNewHumanPlayer(pi, isNetworkPlayer: false, "XBOX360", 1f);
				}
				else
				{
					FighterManager.addNewHumanPlayer(pi, isNetworkPlayer: false, "GUEST", 1f);
				}
			}
			catch (Exception)
			{
				FighterManager.humanPlayers.Clear();
				FighterManager.addNewHumanPlayer(pi, isNetworkPlayer: false, "GUEST", 1f);
			}
			ScreenManager.ShowWorldMap(broadcastOverLive: true);
			break;
		case "Credits":
			ScreenManager.ShowCredits();
			break;
		case "Settings":
			ScreenManager.ShowOptionsFromMenus();
			break;
		case "Exit":
			Program.game.Exit();
			break;
		case "Buy Me!!":
			currentSelection = 0;
			RandomStaticGlobals.BuyMe(pi);
			break;
		case "Register":
		case "Tutorial":
			break;
		}
	}

	private void guideButton()
	{
		Guide.BeginShowMessageBox(PlayerIndex.One, "Byaaaaa Title", "Texty texty long time!!", new string[1] { "Ok" }, 0, MessageBoxIcon.Alert, AsyncCallback(0), object.Equals(0, 0));
	}

	private AsyncCallback AsyncCallback(object p)
	{
		throw new Exception("The method or operation is not implemented.");
	}

	public void Load(ContentManager Content)
	{
	}

	public void Clear()
	{
	}
}
