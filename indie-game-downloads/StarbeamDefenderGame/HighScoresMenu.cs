using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarbeamDefenderGame;

public class HighScoresMenu
{
	private SpriteFont menufont;

	private Texture2D backgroundtexture;

	private int keyboarddelay;

	private int scorepage;

	private int changepage;

	public void LoadContent(ContentManager content, string backgroundfile)
	{
		menufont = content.Load<SpriteFont>("Fonts/Highscores");
		backgroundtexture = content.Load<Texture2D>(backgroundfile);
	}

	public void Reset()
	{
		keyboarddelay = 1000;
		scorepage = 0;
		changepage = 5000;
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

	public int DoGameUpdate(int timems, PlayerIndex mastercontroller)
	{
		if (keyboarddelay > 0)
		{
			keyboarddelay -= timems;
			return 0;
		}
		if (IsKeyDown(mastercontroller, Buttons.A, Keys.Enter) | IsKeyDown(mastercontroller, Buttons.B, Keys.Escape) | IsKeyDown(mastercontroller, Buttons.Back, Keys.B))
		{
			return 1;
		}
		changepage -= timems;
		if (changepage < 0)
		{
			changepage = 5000;
			scorepage++;
			if (scorepage > 1)
			{
				scorepage = 0;
			}
		}
		return 0;
	}

	public void Draw(SpriteBatch spritebatch)
	{
		Rectangle value = new Rectangle(0, 0, 1280, 720);
		spritebatch.Draw(destinationRectangle: new Rectangle(0, 0, 1280, 720), texture: backgroundtexture, sourceRectangle: value, color: Color.White);
		if (scorepage == 0)
		{
			DrawSinglePlayerScores(spritebatch);
		}
		else
		{
			DrawMultiplayerScores(spritebatch);
		}
		DrawCenteredText(spritebatch, "Press A For The Main Menu", new Vector2(640f, 580f), Color.Green);
	}

	private void DrawSinglePlayerScores(SpriteBatch spritebatch)
	{
		DrawCenteredText(spritebatch, "Single Player", new Vector2(640f, 180f), Color.Yellow);
		spritebatch.DrawString(menufont, "Position", new Vector2(200f, 200f), Color.Yellow);
		spritebatch.DrawString(menufont, "Name", new Vector2(400f, 200f), Color.Yellow);
		spritebatch.DrawString(menufont, "Final Score", new Vector2(1000f, 200f), Color.Yellow);
		for (int i = 0; i < 10; i++)
		{
			if (FileAndGamerServices.savegames.SinglePlayerScores.saveditems[i].score > 0)
			{
				spritebatch.DrawString(menufont, (i + 1).ToString(), new Vector2(200f, 250 + i * 30), Color.Green);
				spritebatch.DrawString(menufont, FileAndGamerServices.savegames.SinglePlayerScores.saveditems[i].Name, new Vector2(400f, 250 + i * 30), Color.Green);
				spritebatch.DrawString(menufont, FileAndGamerServices.savegames.SinglePlayerScores.saveditems[i].score.ToString(), new Vector2(1000f, 250 + i * 30), Color.Green);
			}
		}
	}

	private void DrawMultiplayerScores(SpriteBatch spritebatch)
	{
		DrawCenteredText(spritebatch, "Multiplayer Leaderboard", new Vector2(640f, 180f), Color.Yellow);
		spritebatch.DrawString(menufont, "Position", new Vector2(200f, 200f), Color.Yellow);
		spritebatch.DrawString(menufont, "Names", new Vector2(400f, 200f), Color.Yellow);
		spritebatch.DrawString(menufont, "Total Score", new Vector2(1000f, 200f), Color.Yellow);
		for (int i = 0; i < 10; i++)
		{
			if (FileAndGamerServices.savegames.MultiplayerScores.saveditems[i].score > 0)
			{
				spritebatch.DrawString(menufont, (i + 1).ToString(), new Vector2(200f, 250 + i * 30), Color.Green);
				spritebatch.DrawString(menufont, FileAndGamerServices.savegames.MultiplayerScores.saveditems[i].Name + " & " + FileAndGamerServices.savegames.MultiplayerScores.saveditems[i].NameTwo, new Vector2(400f, 250 + i * 30), Color.Green);
				spritebatch.DrawString(menufont, FileAndGamerServices.savegames.MultiplayerScores.saveditems[i].score.ToString(), new Vector2(1000f, 250 + i * 30), Color.Green);
			}
		}
	}

	private void DrawCenteredText(SpriteBatch spritebatch, string text, Vector2 position, Color color)
	{
		Vector2 vector = menufont.MeasureString(text);
		spritebatch.DrawString(menufont, text, position -= vector / 2f, color);
	}
}
