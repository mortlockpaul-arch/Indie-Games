using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AircraftRC;

public class Records
{
	public SpriteBatch spriteBatch;

	private SpriteFont police;

	private Texture2D Score;

	private Texture2D Scores;

	private Texture2D Credits;

	public Records(CustomPhysicsGame game)
	{
	}

	public void Initialize()
	{
	}

	public void LoadContent(CustomPhysicsGame game)
	{
		spriteBatch = new SpriteBatch(game.GraphicsDevice);
		police = game.Content.Load<SpriteFont>("Font/DataFont2");
		Score = game.Content.Load<Texture2D>("Textures/score");
		Scores = game.LoadLocalizedAsset<Texture2D>("Textures/scores");
		Credits = game.LoadLocalizedAsset<Texture2D>("Textures/credits");
	}

	public void Update()
	{
	}

	public void Draw(CustomPhysicsGame game, GameTime gameTime)
	{
		spriteBatch.Begin();
		spriteBatch.Draw(Score, new Vector2(135f, 620f), Color.White);
		spriteBatch.End();
		if (game.scoreP == CustomPhysicsGame.ScoreP.cache && game.creditsv == CustomPhysicsGame.CreditsV.cache)
		{
			spriteBatch.Begin();
			spriteBatch.Draw(Scores, new Vector2(430f, 75f), Color.White);
			spriteBatch.End();
			if (game.conteurSpad.timecounterHA1 < 10)
			{
				string text = $"0{game.conteurSpad.timecounterHA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text, new Vector2(650f, 145f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text2 = $"{game.conteurSpad.timecounterHA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text2, new Vector2(650f, 145f), Color.Green);
				spriteBatch.End();
			}
			if (game.conteurSpad.timecounterMA1 < 10)
			{
				string text3 = $"0{game.conteurSpad.timecounterMA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text3, new Vector2(705f, 145f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text4 = $"{game.conteurSpad.timecounterMA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text4, new Vector2(705f, 145f), Color.Green);
				spriteBatch.End();
			}
			if (game.conteurSpad.timecounterSA1 < 10)
			{
				string text5 = $"0{game.conteurSpad.timecounterSA1}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text5, new Vector2(760f, 145f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text6 = $"{game.conteurSpad.timecounterSA1}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text6, new Vector2(760f, 145f), Color.Green);
				spriteBatch.End();
			}
			if (game.conteurSpad.totalCrash < 10)
			{
				string text7 = $"0{game.conteurSpad.totalCrash}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text7, new Vector2(642f, 175f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text8 = $"{game.conteurSpad.totalCrash}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text8, new Vector2(642f, 175f), Color.Green);
				spriteBatch.End();
			}
			if (game.conteurCanadair.timecounterHA1 < 10)
			{
				string text9 = $"0{game.conteurCanadair.timecounterHA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text9, new Vector2(650f, 208f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text10 = $"{game.conteurCanadair.timecounterHA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text10, new Vector2(650f, 208f), Color.Green);
				spriteBatch.End();
			}
			if (game.conteurCanadair.timecounterMA1 < 10)
			{
				string text11 = $"0{game.conteurCanadair.timecounterMA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text11, new Vector2(705f, 208f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text12 = $"{game.conteurCanadair.timecounterMA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text12, new Vector2(705f, 208f), Color.Green);
				spriteBatch.End();
			}
			if (game.conteurCanadair.timecounterSA1 < 10)
			{
				string text13 = $"0{game.conteurCanadair.timecounterSA1}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text13, new Vector2(760f, 208f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text14 = $"{game.conteurCanadair.timecounterSA1}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text14, new Vector2(760f, 208f), Color.Green);
				spriteBatch.End();
			}
			if (game.conteurCanadair.totalCrash < 10)
			{
				string text15 = $"0{game.conteurCanadair.totalCrash}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text15, new Vector2(642f, 238f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text16 = $"{game.conteurCanadair.totalCrash}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text16, new Vector2(642f, 238f), Color.Green);
				spriteBatch.End();
			}
			if (game.conteurCorsair.timecounterHA1 < 10)
			{
				string text17 = $"0{game.conteurCorsair.timecounterHA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text17, new Vector2(650f, 271f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text18 = $"{game.conteurCorsair.timecounterHA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text18, new Vector2(650f, 271f), Color.Green);
				spriteBatch.End();
			}
			if (game.conteurCorsair.timecounterMA1 < 10)
			{
				string text19 = $"0{game.conteurCorsair.timecounterMA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text19, new Vector2(705f, 271f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text20 = $"{game.conteurCorsair.timecounterMA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text20, new Vector2(705f, 271f), Color.Green);
				spriteBatch.End();
			}
			if (game.conteurCorsair.timecounterSA1 < 10)
			{
				string text21 = $"0{game.conteurCorsair.timecounterSA1}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text21, new Vector2(760f, 271f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text22 = $"{game.conteurCorsair.timecounterSA1}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text22, new Vector2(760f, 271f), Color.Green);
				spriteBatch.End();
			}
			if (game.conteurCorsair.totalCrash < 10)
			{
				string text23 = $"0{game.conteurCorsair.totalCrash}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text23, new Vector2(642f, 301f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text24 = $"{game.conteurCorsair.totalCrash}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text24, new Vector2(642f, 301f), Color.Green);
				spriteBatch.End();
			}
			if (game.conteurac130.timecounterHA1 < 10)
			{
				string text25 = $"0{game.conteurac130.timecounterHA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text25, new Vector2(650f, 334f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text26 = $"{game.conteurac130.timecounterHA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text26, new Vector2(650f, 334f), Color.Green);
				spriteBatch.End();
			}
			if (game.conteurac130.timecounterMA1 < 10)
			{
				string text27 = $"0{game.conteurac130.timecounterMA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text27, new Vector2(705f, 334f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text28 = $"{game.conteurac130.timecounterMA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text28, new Vector2(705f, 334f), Color.Green);
				spriteBatch.End();
			}
			if (game.conteurac130.timecounterSA1 < 10)
			{
				string text29 = $"0{game.conteurac130.timecounterSA1}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text29, new Vector2(760f, 334f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text30 = $"{game.conteurac130.timecounterSA1}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text30, new Vector2(760f, 334f), Color.Green);
				spriteBatch.End();
			}
			if (game.conteurac130.totalCrash < 10)
			{
				string text31 = $"0{game.conteurac130.totalCrash}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text31, new Vector2(642f, 364f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text32 = $"{game.conteurac130.totalCrash}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text32, new Vector2(642f, 364f), Color.Green);
				spriteBatch.End();
			}
			if (game.conteurDH112.timecounterHA1 < 10)
			{
				string text33 = $"0{game.conteurDH112.timecounterHA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text33, new Vector2(650f, 397f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text34 = $"{game.conteurDH112.timecounterHA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text34, new Vector2(650f, 397f), Color.Green);
				spriteBatch.End();
			}
			if (game.conteurDH112.timecounterMA1 < 10)
			{
				string text35 = $"0{game.conteurDH112.timecounterMA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text35, new Vector2(705f, 397f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text36 = $"{game.conteurDH112.timecounterMA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text36, new Vector2(705f, 397f), Color.Green);
				spriteBatch.End();
			}
			if (game.conteurDH112.timecounterSA1 < 10)
			{
				string text37 = $"0{game.conteurDH112.timecounterSA1}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text37, new Vector2(760f, 397f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text38 = $"{game.conteurDH112.timecounterSA1}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text38, new Vector2(760f, 397f), Color.Green);
				spriteBatch.End();
			}
			if (game.conteurDH112.totalCrash < 10)
			{
				string text39 = $"0{game.conteurDH112.totalCrash}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text39, new Vector2(642f, 427f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text40 = $"{game.conteurDH112.totalCrash}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text40, new Vector2(642f, 427f), Color.Green);
				spriteBatch.End();
			}
			if (game.conteurF22.timecounterHA1 < 10)
			{
				string text41 = $"0{game.conteurF22.timecounterHA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text41, new Vector2(650f, 460f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text42 = $"{game.conteurF22.timecounterHA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text42, new Vector2(650f, 460f), Color.Green);
				spriteBatch.End();
			}
			if (game.conteurF22.timecounterMA1 < 10)
			{
				string text43 = $"0{game.conteurF22.timecounterMA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text43, new Vector2(705f, 460f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text44 = $"{game.conteurF22.timecounterMA1}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text44, new Vector2(705f, 460f), Color.Green);
				spriteBatch.End();
			}
			if (game.conteurF22.timecounterSA1 < 10)
			{
				string text45 = $"0{game.conteurF22.timecounterSA1}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text45, new Vector2(760f, 460f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text46 = $"{game.conteurF22.timecounterSA1}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text46, new Vector2(760f, 460f), Color.Green);
				spriteBatch.End();
			}
			if (game.conteurF22.totalCrash < 10)
			{
				string text47 = $"0{game.conteurF22.totalCrash}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text47, new Vector2(642f, 490f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text48 = $"{game.conteurF22.totalCrash}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text48, new Vector2(642f, 490f), Color.Green);
				spriteBatch.End();
			}
			if (game.jeux.timecounter1M < 10)
			{
				string text49 = $"0{game.jeux.timecounter1M}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text49, new Vector2(640f, 535f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text50 = $"{game.jeux.timecounter1M}:";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text50, new Vector2(640f, 535f), Color.Green);
				spriteBatch.End();
			}
			if (game.jeux.timecounter1S < 10)
			{
				string text51 = $"0{game.jeux.timecounter1S}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text51, new Vector2(695f, 535f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text52 = $"{game.jeux.timecounter1S}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text52, new Vector2(695f, 535f), Color.Green);
				spriteBatch.End();
			}
			if (game.jeux.ReA < 10)
			{
				string text53 = $"0{game.jeux.ReA}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text53, new Vector2(632f, 565f), Color.Green);
				spriteBatch.End();
			}
			else
			{
				string text54 = $"{game.jeux.ReA}";
				spriteBatch.Begin();
				spriteBatch.DrawString(police, text54, new Vector2(632f, 565f), Color.Green);
				spriteBatch.End();
			}
		}
		if (game.scoreP == CustomPhysicsGame.ScoreP.cache && game.creditsv == CustomPhysicsGame.CreditsV.vu)
		{
			spriteBatch.Begin();
			spriteBatch.Draw(Credits, new Vector2(430f, 75f), Color.White);
			spriteBatch.End();
		}
	}
}
