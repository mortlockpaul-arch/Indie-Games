using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarbeamDefenderGame;

public class GameOver
{
	private Texture2D backgroundtexture;

	private SpriteFont gamefont;

	private int keydeley;

	private int flash;

	private Color flashcolor;

	private AudioManager audioeffects;

	public GameOver()
	{
		keydeley = 0;
		flash = 100;
		flashcolor = Color.Red;
	}

	public void Reset()
	{
		keydeley = 1000;
		flash = 1000;
		flashcolor = Color.Red;
	}

	public void LoadContent(ContentManager content, AudioManager audioeffects)
	{
		backgroundtexture = content.Load<Texture2D>("Menus/GameOver");
		gamefont = content.Load<SpriteFont>("Fonts/Main");
		this.audioeffects = audioeffects;
	}

	private bool IsKeyDown(Keys key)
	{
		if (Keyboard.GetState().IsKeyDown(key))
		{
			return true;
		}
		return false;
	}

	private bool IsButtonDown(PlayerIndex controller, Buttons button)
	{
		if (GamePad.GetState(controller).IsButtonDown(button))
		{
			return true;
		}
		return false;
	}

	public int Update(int timems, PlayerIndex playerone)
	{
		if (keydeley > 0)
		{
			keydeley -= timems;
		}
		else
		{
			if (IsButtonDown(playerone, Buttons.A) | IsButtonDown(playerone, Buttons.X) | IsButtonDown(playerone, Buttons.B))
			{
				return 1;
			}
			if (IsKeyDown(Keys.Enter) | IsKeyDown(Keys.Space))
			{
				return 1;
			}
		}
		flash -= timems;
		if (flash < 0)
		{
			flash += 1000;
			if (flashcolor == Color.Red)
			{
				flashcolor = Color.Green;
			}
			else
			{
				flashcolor = Color.Red;
			}
		}
		return 0;
	}

	public void Draw(SpriteBatch spritebatch, Turret[] scores)
	{
		Rectangle value = new Rectangle(0, 0, 1280, 720);
		spritebatch.Draw(destinationRectangle: new Rectangle(0, 0, 1280, 720), texture: backgroundtexture, sourceRectangle: value, color: Color.White);
		if (scores.Count() == 1)
		{
			DrawCenteredText(spritebatch, "Final Score", new Vector2(640f, 450f), Color.Green);
			DrawCenteredText(spritebatch, scores[0].Score.ToString(), new Vector2(640f, 500f), Color.Green);
		}
		else
		{
			DrawCenteredText(spritebatch, "Total Score", new Vector2(640f, 450f), Color.Green);
			DrawCenteredText(spritebatch, FileAndGamerServices.gamer.PlayerNames[(int)scores[0].ControllerIndex], new Vector2(200f, 550f), Color.Green);
			DrawCenteredText(spritebatch, FileAndGamerServices.gamer.PlayerNames[(int)scores[1].ControllerIndex], new Vector2(1100f, 550f), Color.Green);
			DrawCenteredText(spritebatch, (scores[0].Score + scores[1].Score).ToString(), new Vector2(640f, 500f), Color.Green);
			DrawCenteredText(spritebatch, scores[0].Score.ToString(), new Vector2(200f, 600f), Color.Green);
			DrawCenteredText(spritebatch, scores[1].Score.ToString(), new Vector2(1100f, 600f), Color.Green);
		}
		DrawCenteredText(spritebatch, "Press A To Continue", new Vector2(640f, 580f), flashcolor);
	}

	private void DrawCenteredText(SpriteBatch spritebatch, string text, Vector2 position, Color color)
	{
		Vector2 vector = gamefont.MeasureString(text);
		spritebatch.DrawString(gamefont, text, position -= vector / 2f, color);
	}
}
