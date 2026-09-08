using System;
using Eyehook.Framework;
using Loot.Core;
using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Loot.Screens;

public class GameOverScreen : Screen
{
	private bool escaped;

	private string gamertag;

	private int rank;

	private string levelText;

	private string gpText;

	private string depthText;

	private string scrollTitle;

	private FormattedText scrollText;

	private static Texture2D tombstone;

	private static Texture2D escape;

	private static Sprite rankSprite;

	private Vector2 scrollTitlePos = new Vector2(872f, 154f);

	private Vector2 scrollTextPos = new Vector2(656f, 214f) + new Vector2(12f, 20f);

	private Vector2 playerPos = new Vector2(338f, 540f);

	private float playerScale = 1f;

	private Color rankColor = new Color(152, 101, 0);

	private Vector2 v;

	private Vector2 tombPos = new Vector2(340f, 236f);

	private Vector2 lineHeight = new Vector2(0f, 40f);

	private string s1 = "Here Lies";

	private string s2 = "Who Died On";

	private string s3 = "Dungeon Level";

	private string s4 = "With";

	private string s5 = "R.I.P.";

	private Vector2 shadowOffset = new Vector2(0f, 4f);

	private Color textColor = new Color(204, 204, 204);

	private Color shadowColor = new Color(96, 96, 96);

	public GameOverScreen(int rank)
		: base(modal: true)
	{
		this.rank = rank;
		escaped = DM.Player.Depth == 51;
		DM.Player.Facing = Direction.South;
		DM.Player.Sprite.Reset();
		gamertag = DM.Player.Name;
		levelText = "A Level " + DM.Player.Level;
		gpText = DM.Player.Gold + " GP";
		depthText = string.Concat(DM.Player.Depth);
		if (escaped)
		{
			TimeSpan playTime = DM.Player.PlayTime;
			scrollTitle = "Congratulations!";
			scrollText = new FormattedText("You escaped the dungeon in\n\n" + ((playTime.Hours < 10) ? "0" : "") + playTime.Hours + " Hours\n" + ((playTime.Minutes < 10) ? "0" : "") + playTime.Minutes + " Minutes\n" + ((playTime.Seconds < 10) ? "0" : "") + playTime.Seconds + " Seconds\n\n\u0080\u0081 Main Menu\n\u0082\u0083 High Scores\n", 432);
		}
		else
		{
			TimeSpan playTime2 = DM.Player.PlayTime;
			scrollTitle = "Game Over";
			scrollText = new FormattedText("Your adventure has ended.\n\n" + ((playTime2.Hours < 10) ? "0" : "") + playTime2.Hours + " Hours\n" + ((playTime2.Minutes < 10) ? "0" : "") + playTime2.Minutes + " Minutes\n" + ((playTime2.Seconds < 10) ? "0" : "") + playTime2.Seconds + " Seconds\n\n\u0080\u0081 Main Menu\n\u0082\u0083 High Scores\n", 432);
		}
	}

	public override void transitionOn()
	{
		BgMusic.GameOver();
	}

	public static void Load(ContentManager content)
	{
		tombstone = content.Load<Texture2D>("Sprites\\GameOver\\Tombstone");
		escape = content.Load<Texture2D>("Sprites\\GameOver\\Escape");
		rankSprite = new StillSprite(content.Load<Texture2D>("Sprites\\GameOver\\Rank"));
	}

	public override void update(GameTime gameTime)
	{
		if (MC.GamePadManager.isNewButtonDown(Buttons.A))
		{
			MC.ScreenManager.addScreen(new FadeOutScreen(TimeSpan.FromMilliseconds(500.0), MC.ScreenManager.BackgroundColor, showMainMenu));
		}
		else if (MC.GamePadManager.isNewButtonDown(Buttons.B))
		{
			MC.ScreenManager.addScreen(new FadeOutScreen(TimeSpan.FromMilliseconds(500.0), MC.ScreenManager.BackgroundColor, showHighScores));
		}
	}

	private void showMainMenu()
	{
		MC.ScreenManager.removeScreen(this);
		MC.ScreenManager.addScreen(new MainMenu());
		MC.ScreenManager.addScreen(new FadeInScreen(TimeSpan.FromMilliseconds(500.0), MC.ScreenManager.BackgroundColor, null));
	}

	private void showHighScores()
	{
		Difficulty difficulty = DM.GameOptions.Difficulty;
		MC.ScreenManager.removeScreen(this);
		MC.ScreenManager.addScreen(new MainMenu());
		MC.ScreenManager.addScreen(new FadeInScreen(TimeSpan.FromMilliseconds(500.0), MC.ScreenManager.BackgroundColor, null));
		MC.ScreenManager.addScreen(new HighScoreScreen(difficulty));
		MC.Game.ResetElapsedTime();
	}

	public override void draw(GameTime gameTime)
	{
		base.spriteBatch.Begin(SpriteSortMode.Immediate, null, GraphicUtil.PointFilter, null, null);
		if (escaped)
		{
			drawEscaped();
		}
		else
		{
			drawTombstone();
		}
		drawRank();
		base.spriteBatch.End();
	}

	private void drawEscaped()
	{
		base.spriteBatch.Draw(escape, base.viewportRect, Color.White);
		Text.DrawCentered(base.spriteBatch, scrollTitlePos, scrollTitle, Color.Black);
		Text.Draw(base.spriteBatch, scrollTextPos, scrollText, Color.Black);
		DM.Player.Sprite.Draw(base.spriteBatch, playerPos, playerScale);
		if (DM.Player.Armor != null)
		{
			DM.Player.Armor.DrawOnPlayer(base.spriteBatch, playerPos, playerScale);
		}
	}

	private void drawRank()
	{
		if (rank != 0)
		{
			rankSprite.Draw(base.spriteBatch, new Vector2(1007f, 394f));
			Text.DrawCentered(base.spriteBatch, new Vector2(1007f, 374f), "RANK", rankColor);
			Text.DrawCentered(base.spriteBatch, new Vector2(1007f, 414f), rank, rankColor);
		}
	}

	private void drawTombstone()
	{
		base.spriteBatch.Draw(tombstone, base.viewportRect, Color.White);
		Text.DrawCentered(base.spriteBatch, scrollTitlePos, scrollTitle, Color.Black);
		Text.Draw(base.spriteBatch, scrollTextPos, scrollText, Color.Black);
		v = tombPos;
		drawLine(s1);
		drawLine(gamertag);
		drawLine(levelText);
		drawLine(DM.Player.ClassName);
		drawLine(s2);
		drawLine(s3);
		drawLine(depthText);
		drawLine(s4);
		drawLine(gpText);
		v += lineHeight;
		drawLine(s5);
	}

	private void drawLine(string s)
	{
		Text.DrawCentered(base.spriteBatch, v + shadowOffset, s, shadowColor);
		Text.DrawCentered(base.spriteBatch, v, s, textColor);
		v += lineHeight;
	}
}
