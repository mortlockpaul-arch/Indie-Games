using System;
using Eyehook.Framework;
using Loot.Core;
using Loot.Dungeon;
using Loot.PC;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Loot.Screens;

public class MainMenu : Screen
{
	private Sprite mainMenu;

	private Texture2D quill;

	private bool saveExists;

	private int option;

	private string[] options = new string[8] { "Continue", "New Game", "High Scores", "Awardments", "Controls", "Options", "Help", "Credits" };

	private Vector2 mainMenuPos = new Vector2(380f, 360f);

	private Vector2 menuItemPos = new Vector2(178f, 274f);

	private Color optionColor = new Color(0, 0, 0);

	private Color selectedColor = new Color(176, 0, 0);

	private Color disabledColor = new Color(0, 0, 0) * 0.35f;

	private Color versionColor = new Color(0, 0, 0) * 0.25f;

	private Vector2 quillOrigin = new Vector2(64f, 64f);

	private Vector2 quillOffset = new Vector2(36f, -24f);

	public MainMenu()
		: base(modal: false)
	{
		saveExists = DM.SaveFileExists();
		if (!saveExists)
		{
			option = 1;
		}
		DM.Demo();
	}

	public override void loadContent(ContentManager content)
	{
		mainMenu = new StillSprite(content.Load<Texture2D>("Sprites\\UI\\MainMenu"));
		quill = content.Load<Texture2D>("Sprites\\UI\\Quill");
		MC.ScreenManager.insertBefore(new DemoView(), this);
	}

	public override void transitionOn()
	{
		BgMusic.TitleMusic();
	}

	public override void update(GameTime gameTime)
	{
		if (((DemoPlayer)DM.Player).IsIdle)
		{
			DM.Demo();
		}
		if (MC.GamePadManager.isNewButtonDown(Buttons.B))
		{
			PlaySound.MenuClick();
			MessageBox.Display("Quit Cursed Loot", "Are you sure you want to exit?", delegate
			{
				MC.State = GameState.EXIT;
			}, delegate
			{
			});
			return;
		}
		if (MC.GamePadManager.isNewDirDown())
		{
			PlaySound.MenuMove();
			option++;
			if (option > options.Length - 1)
			{
				option = ((!saveExists) ? 1 : 0);
			}
		}
		if (MC.GamePadManager.isNewDirUp())
		{
			PlaySound.MenuMove();
			option--;
			if (option < 0 || (!saveExists && option == 0))
			{
				option = options.Length - 1;
			}
		}
		if (!MC.GamePadManager.isNewButtonDown(Buttons.A))
		{
			return;
		}
		PlaySound.MenuClick();
		switch (option)
		{
		case 0:
			MC.ScreenManager.addScreen(new FadeOutScreen(TimeSpan.FromMilliseconds(500.0), MC.ScreenManager.BackgroundColor, loadGame));
			break;
		case 1:
			if (saveExists)
			{
				MessageBox.Display("New Game", new FormattedText("Your current game will be lost.\nDo you want to continue?", 744), delegate
				{
					fadeToNewGame();
				}, delegate
				{
				});
			}
			else
			{
				fadeToNewGame();
			}
			break;
		case 2:
			MC.ScreenManager.addScreen(new HighScoreScreen());
			break;
		case 3:
			MC.ScreenManager.addScreen(new AwardmentScreen());
			break;
		case 4:
			MC.ScreenManager.addScreen(new ControlsScreen());
			break;
		case 5:
			MC.ScreenManager.addScreen(new FadeOutScreen(TimeSpan.FromMilliseconds(250.0), MC.ScreenManager.BackgroundColor, showOptions));
			break;
		case 6:
			HelpScreen.Display();
			break;
		case 7:
			MC.ScreenManager.addScreen(new FadeOutScreen(TimeSpan.FromMilliseconds(500.0), MC.ScreenManager.BackgroundColor, showCredits));
			break;
		default:
			Message.Display("Not implemented...");
			break;
		}
	}

	private void loadGame()
	{
		DM.Load();
		MC.Game.ResetElapsedTime();
		BgMusic.Next();
		MC.ScreenManager.removeAllScreens();
		MC.ScreenManager.addScreen(new DungeonView());
		MC.ScreenManager.addScreen(new FadeInScreen(TimeSpan.FromMilliseconds(500.0), MC.ScreenManager.BackgroundColor, null));
	}

	private void fadeToNewGame()
	{
		MC.ScreenManager.addScreen(new FadeOutScreen(TimeSpan.FromMilliseconds(500.0), MC.ScreenManager.BackgroundColor, newGame));
	}

	private void newGame()
	{
		MC.ScreenManager.removeAllScreens();
		MC.ScreenManager.addScreen(new NewGameScreen());
		MC.ScreenManager.addScreen(new FadeInScreen(TimeSpan.FromMilliseconds(500.0), MC.ScreenManager.BackgroundColor, null));
	}

	private void showOptions()
	{
		MC.ScreenManager.addScreen(new FadeInScreen(TimeSpan.FromMilliseconds(500.0), MC.ScreenManager.BackgroundColor, null));
		MC.ScreenManager.addScreen(new OptionsScreen());
		MC.ScreenManager.addScreen(new FadeInScreen(TimeSpan.FromMilliseconds(250.0), MC.ScreenManager.BackgroundColor, null));
	}

	private void showCredits()
	{
		MC.ScreenManager.removeAllScreens();
		MC.ScreenManager.addScreen(new CreditScreen());
	}

	public override void draw(GameTime gameTime)
	{
		base.spriteBatch.Begin();
		mainMenu.Draw(base.spriteBatch, mainMenuPos);
		for (int i = 0; i < options.Length; i++)
		{
			Vector2 v = menuItemPos + new Vector2(0f, 40 * i);
			Color color = ((option == i) ? selectedColor : optionColor);
			if (i == 0 && !saveExists)
			{
				color = disabledColor;
			}
			Text.Draw(base.spriteBatch, ref v, options[i], color);
			if (option == i)
			{
				base.spriteBatch.Draw(quill, v + quillOffset, null, Color.White, 0f, quillOrigin, 1f, SpriteEffects.None, 0f);
			}
		}
		Vector2 v2 = new Vector2(536f, 561f);
		Text.Draw(base.spriteBatch, ref v2, "2.1.0", versionColor, 0.5f);
		base.spriteBatch.End();
	}
}
