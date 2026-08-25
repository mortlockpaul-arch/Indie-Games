using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DepthAttack.CS;

public class RankIn(Game game) : DrawableGameComponent(game)
{
	private SpriteFont font1;

	private SpriteBatch spritesBatch;

	public bool pflgRankInEnd = false;

	public PlayerIndex playerIndex;

	private GamePadState gamePadState;

	private GamePadState gamePadMaeState;

	public long plngScore;

	private string strName;

	private char[] chrName = new char[5];

	private int intNameIti = 0;

	public int pintRank = 0;

	private int ingPadUpNagaOsi;

	private int ingPadDownNagaOsi;

	public int intOneScan;

	public override void Initialize()
	{
		intOneScan = 0;
		for (int i = 0; i < chrName.Length; i++)
		{
			chrName[i] = 'A';
		}
		intNameIti = 0;
		pflgRankInEnd = false;
		base.Initialize();
	}

	protected override void LoadContent()
	{
		spritesBatch = new SpriteBatch(base.GraphicsDevice);
		font1 = base.Game.Content.Load<SpriteFont>("SpriteFont1");
		base.LoadContent();
	}

	public int intFunRankIn(long lngScore)
	{
		int result = Game1.recordContent.pscoreRecord.Length + 1;
		for (int i = 0; i < Game1.recordContent.pscoreRecord.Length; i++)
		{
			if (Game1.recordContent.pscoreRecord[i].lngScore < lngScore)
			{
				return i;
			}
		}
		return result;
	}

	public void RankInSort(string astrName, long alngScore)
	{
		int num = Game1.recordContent.pscoreRecord.Length - 1;
		if (Game1.recordContent.pscoreRecord[num].lngScore < alngScore)
		{
			Game1.recordContent.pscoreRecord[num].lngScore = alngScore;
			Game1.recordContent.pscoreRecord[num].strName = astrName;
			int num2 = num - 1;
			while (num2 >= 0 && Game1.recordContent.pscoreRecord[num2].lngScore < Game1.recordContent.pscoreRecord[num2 + 1].lngScore)
			{
				long lngScore = Game1.recordContent.pscoreRecord[num2].lngScore;
				string text = Game1.recordContent.pscoreRecord[num2].strName;
				Game1.recordContent.pscoreRecord[num2].lngScore = Game1.recordContent.pscoreRecord[num2 + 1].lngScore;
				Game1.recordContent.pscoreRecord[num2].strName = Game1.recordContent.pscoreRecord[num2 + 1].strName;
				Game1.recordContent.pscoreRecord[num2 + 1].lngScore = lngScore;
				Game1.recordContent.pscoreRecord[num2 + 1].strName = text;
				num2--;
			}
		}
	}

	public void RankInMov(GameTime gameTime)
	{
		gamePadState = GamePad.GetState(playerIndex);
		if (intOneScan < 30)
		{
			intOneScan++;
			gamePadMaeState = gamePadState;
			base.Update(gameTime);
			return;
		}
		if (intNameIti < 5 && (((double)gamePadState.ThumbSticks.Left.Y > 0.5 && (double)gamePadMaeState.ThumbSticks.Left.Y <= 0.5) || ((double)gamePadState.ThumbSticks.Right.Y > 0.5 && (double)gamePadMaeState.ThumbSticks.Right.Y <= 0.5) || (gamePadState.DPad.Up == ButtonState.Pressed && gamePadMaeState.DPad.Up == ButtonState.Released)))
		{
			if (chrName[intNameIti] > ' ')
			{
				chrName[intNameIti] -= '\u0001';
			}
			else
			{
				chrName[intNameIti] = '~';
			}
		}
		if (intNameIti < 5 && (((double)gamePadState.ThumbSticks.Left.Y < -0.5 && (double)gamePadMaeState.ThumbSticks.Left.Y >= -0.5) || ((double)gamePadState.ThumbSticks.Right.Y < -0.5 && (double)gamePadMaeState.ThumbSticks.Right.Y >= -0.5) || (gamePadState.DPad.Down == ButtonState.Pressed && gamePadMaeState.DPad.Down == ButtonState.Released)))
		{
			if (chrName[intNameIti] < '~')
			{
				chrName[intNameIti] += '\u0001';
			}
			else
			{
				chrName[intNameIti] = ' ';
			}
		}
		if (intNameIti < 5 && ((double)gamePadState.ThumbSticks.Left.Y >= 0.5 || gamePadState.DPad.Up == ButtonState.Pressed))
		{
			if (ingPadUpNagaOsi < 16)
			{
				ingPadUpNagaOsi++;
			}
			else if (gameTime.TotalGameTime.Milliseconds % 6 == 0)
			{
				if (chrName[intNameIti] > ' ')
				{
					chrName[intNameIti] -= '\u0001';
				}
				else
				{
					chrName[intNameIti] = '~';
				}
			}
		}
		else
		{
			ingPadUpNagaOsi = 0;
		}
		if (intNameIti < 5 && ((double)gamePadState.ThumbSticks.Left.Y < -0.5 || gamePadState.DPad.Down == ButtonState.Pressed))
		{
			if (ingPadDownNagaOsi < 16)
			{
				ingPadDownNagaOsi++;
			}
			else if (gameTime.TotalGameTime.Milliseconds % 6 == 0)
			{
				chrName[intNameIti] += '\u0001';
				if (chrName[intNameIti] < '~')
				{
					chrName[intNameIti] += '\u0001';
				}
				else
				{
					chrName[intNameIti] = ' ';
				}
			}
		}
		else
		{
			ingPadDownNagaOsi = 0;
		}
		if (((gamePadState.Buttons.B == ButtonState.Pressed && gamePadMaeState.Buttons.B == ButtonState.Released) || (gamePadState.DPad.Left == ButtonState.Pressed && gamePadMaeState.DPad.Left == ButtonState.Released) || ((double)gamePadState.ThumbSticks.Left.X < -0.5 && (double)gamePadMaeState.ThumbSticks.Left.X >= -0.5)) && intNameIti > 0)
		{
			intNameIti--;
		}
		if ((intNameIti < 5 && gamePadState.Buttons.A == ButtonState.Pressed && gamePadMaeState.Buttons.A == ButtonState.Released) || (gamePadState.Buttons.Start == ButtonState.Pressed && gamePadMaeState.Buttons.Start == ButtonState.Released) || (gamePadState.DPad.Right == ButtonState.Pressed && gamePadMaeState.DPad.Right == ButtonState.Released) || ((double)gamePadState.ThumbSticks.Left.X > 0.5 && (double)gamePadMaeState.ThumbSticks.Left.X <= 0.5))
		{
			intNameIti++;
		}
		else if ((intNameIti == 5 && gamePadState.Buttons.A == ButtonState.Pressed && gamePadMaeState.Buttons.A == ButtonState.Released) || (gamePadState.Buttons.Start == ButtonState.Pressed && gamePadMaeState.Buttons.Start == ButtonState.Released))
		{
			strName = chrName[0].ToString() + chrName[1] + chrName[2] + chrName[3] + chrName[4];
			RankInSort(strName, plngScore);
			if (!Guide.IsTrialMode)
			{
				Game1.recordContent.recordSave();
			}
			pflgRankInEnd = true;
		}
		gamePadMaeState = gamePadState;
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}

	public void pRankInDraw(GameTime gameTime, SpriteBatch aspriteBatch)
	{
		aspriteBatch.DrawString(font1, "RANK IN", new Vector2(640f, 320f), Color.White);
		aspriteBatch.DrawString(font1, "SCORE", new Vector2(640f, 220f), Color.White);
		aspriteBatch.DrawString(font1, plngScore.ToString(), new Vector2(640f, 260f), Color.White);
		aspriteBatch.DrawString(font1, chrName[0].ToString() + chrName[1] + chrName[2] + chrName[3] + chrName[4], new Vector2(640f, 360f), Color.White);
		if (intNameIti == 5)
		{
			aspriteBatch.DrawString(font1, "end", new Vector2(640 + intNameIti * 17 + 5, 360f), Color.White);
			aspriteBatch.DrawString(font1, "_", new Vector2(640 + intNameIti * 17 + 5, 360f), Color.White);
		}
		else if (intNameIti < 5)
		{
			aspriteBatch.DrawString(font1, "_", new Vector2(640 + intNameIti * 17, 360f), Color.White);
		}
		if (Game1.pStorageDevice != null)
		{
			if (!Game1.pStorageDevice.IsConnected)
			{
				aspriteBatch.DrawString(font1, "cannot save.", new Vector2(128f, 72f), Color.White);
			}
		}
		else
		{
			aspriteBatch.DrawString(font1, "cannot save.", new Vector2(128f, 72f), Color.White);
		}
	}

	public override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
	}
}
