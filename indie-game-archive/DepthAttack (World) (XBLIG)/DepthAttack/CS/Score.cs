using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DepthAttack.CS;

public class Score(Game game) : DrawableGameComponent(game)
{
	private const string cstrTextBG00 = "PNG\\System\\TextBG02";

	private SpriteBatch spritesBatch;

	private SpriteFont font1;

	public long plngHighScore = 0L;

	public long plngScore = 0L;

	private Texture2D imgTextBG00;

	public override void Initialize()
	{
		base.Initialize();
	}

	public void pScoreUp(long alngScoreUp)
	{
		plngScore += alngScoreUp;
		if (plngScore > plngHighScore)
		{
			plngHighScore = plngScore;
		}
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}

	protected override void LoadContent()
	{
		spritesBatch = new SpriteBatch(base.GraphicsDevice);
		font1 = base.Game.Content.Load<SpriteFont>("SpriteFont1");
		imgTextBG00 = base.Game.Content.Load<Texture2D>("PNG\\System\\TextBG02");
		base.LoadContent();
	}

	public void pScoreDraw(SpriteBatch aspritesBatch)
	{
		if (imgTextBG00 == null)
		{
			return;
		}
		for (int i = 0; i < 24; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				aspritesBatch.Draw(imgTextBG00, new Vector2(i * imgTextBG00.Width, j * imgTextBG00.Height - 30), new Color(96, 96, 96, 96));
			}
		}
		for (int i = 0; i < 24; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				aspritesBatch.Draw(imgTextBG00, new Vector2(i * imgTextBG00.Width, j * imgTextBG00.Height + 625), new Color(96, 96, 96, 96));
			}
		}
		aspritesBatch.DrawString(font1, "Stage", new Vector2(1000f, 50f), Color.White);
		aspritesBatch.DrawString(font1, Game1.gameMain.pintStage.ToString(), new Vector2(1100f, 50f), Color.White);
		aspritesBatch.DrawString(font1, "HighScore", new Vector2(70f, 50f), Color.White);
		aspritesBatch.DrawString(font1, plngHighScore.ToString(), new Vector2(270f, 50f), Color.White);
		aspritesBatch.DrawString(font1, "Score", new Vector2(540f, 50f), Color.White);
		aspritesBatch.DrawString(font1, plngScore.ToString(), new Vector2(650f, 50f), Color.White);
	}
}
