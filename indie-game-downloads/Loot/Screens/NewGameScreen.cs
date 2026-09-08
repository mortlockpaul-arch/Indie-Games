using Eyehook.Framework;
using Loot.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Loot.Screens;

public class NewGameScreen : DialogScreen
{
	private Color infoColor = Color.White;

	private Vector2 infoPos = new Vector2(520f, 324f);

	private int lineHeight = 44;

	private Color titleColor = new Color(255, 204, 0);

	protected override int CenterX => 288;

	public NewGameScreen()
		: base("New Game", null, null, new DialogOption("Easy", easy), new DialogOption("Normal", normal), new DialogOption("Hard", hard))
	{
		selected = 1;
	}

	public override void update(GameTime gameTime)
	{
		if (MC.GamePadManager.isNewButtonDown(Buttons.B))
		{
			PlaySound.MenuClick();
			MC.ScreenManager.removeAllScreens();
			MC.ScreenManager.addScreen(new MainMenu());
		}
		else
		{
			base.update(gameTime);
		}
	}

	private static void easy()
	{
		GameOptions gameOptions = new GameOptions(Difficulty.Easy);
		MC.ScreenManager.removeAllScreens();
		MC.ScreenManager.addScreen(new ClassScreen(gameOptions));
	}

	private static void normal()
	{
		GameOptions gameOptions = new GameOptions(Difficulty.Normal);
		MC.ScreenManager.removeAllScreens();
		MC.ScreenManager.addScreen(new ClassScreen(gameOptions));
	}

	private static void hard()
	{
		GameOptions gameOptions = new GameOptions(Difficulty.Hard);
		MC.ScreenManager.removeAllScreens();
		MC.ScreenManager.addScreen(new ClassScreen(gameOptions));
	}

	public override void draw(GameTime gameTime)
	{
		base.draw(gameTime);
		base.spriteBatch.Begin();
		if (selected == 0)
		{
			drawEasy(infoPos);
		}
		if (selected == 1)
		{
			drawNormal(infoPos);
		}
		if (selected == 2)
		{
			drawHard(infoPos);
		}
		base.spriteBatch.End();
	}

	private void drawEasy(Vector2 pos)
	{
		Text.Draw(base.spriteBatch, pos, "+ Monsters are Weaker", infoColor);
		pos.Y += lineHeight;
		Text.Draw(base.spriteBatch, pos, "+ Gain Experience Faster", infoColor);
		pos.Y += lineHeight;
		Text.Draw(base.spriteBatch, pos, "+ Regeneration", infoColor);
		pos.Y += lineHeight;
		Text.Draw(base.spriteBatch, pos, "+ Charm of Resurrection", infoColor);
	}

	private void drawNormal(Vector2 pos)
	{
		Text.Draw(base.spriteBatch, pos, "~ The Classic Experience ~", titleColor);
		pos.Y += lineHeight;
		Text.Draw(base.spriteBatch, pos, "Try to survive 50 randomly", infoColor);
		pos.Y += lineHeight;
		Text.Draw(base.spriteBatch, pos, "generated levels, or die", infoColor);
		pos.Y += lineHeight;
		Text.Draw(base.spriteBatch, pos, "trying.", infoColor);
	}

	private void drawHard(Vector2 pos)
	{
		Text.Draw(base.spriteBatch, pos, "- Monsters are Tougher", infoColor);
		pos.Y += lineHeight;
		Text.Draw(base.spriteBatch, pos, "- Gain Experience Slower", infoColor);
		pos.Y += lineHeight;
		Text.Draw(base.spriteBatch, pos, "- Beware the Reaper", infoColor);
		pos.Y += lineHeight;
		Text.Draw(base.spriteBatch, pos, "+ Can Unlock All Awardments", infoColor);
	}
}
