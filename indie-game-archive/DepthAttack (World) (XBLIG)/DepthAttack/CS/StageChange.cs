using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DepthAttack.CS;

public class StageChange : DrawableGameComponent
{
	private const string cstrGameOver00 = "PNG\\System\\Comment\\GameOver";

	private const string cstrGameClear00 = "PNG\\System\\Comment\\GameClear00";

	private const string cstrTextBG00 = "PNG\\System\\TextBG02";

	private SpriteFont font1;

	public PlayerIndex playerIndex;

	private GamePadState gamePadState;

	private GamePadState gamePadMaeState;

	public bool pflgGameOverEnable;

	private Texture2D imgGameOver;

	public bool pflgGameClearEnable;

	private Texture2D imgGameClear;

	private Texture2D imgTextBG00;

	public bool pflgStageChangeEnd;

	public int pintStage = 0;

	public int intOneScan;

	public StageChange(Game game)
		: base(game)
	{
		pflgGameClearEnable = false;
		pflgGameOverEnable = false;
	}

	public override void Initialize()
	{
		pflgGameOverEnable = false;
		pflgGameClearEnable = false;
		pflgStageChangeEnd = false;
		intOneScan = 0;
		base.Initialize();
	}

	public void pGameOverEnable()
	{
		pflgGameOverEnable = true;
		pflgGameClearEnable = false;
		pflgStageChangeEnd = false;
		intOneScan = 0;
	}

	public void pGameClearEnable()
	{
		pflgGameOverEnable = false;
		pflgGameClearEnable = true;
		pflgStageChangeEnd = false;
		intOneScan = 0;
	}

	public void pStageChangeEnd()
	{
		pflgGameClearEnable = false;
		pflgGameOverEnable = false;
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}

	public void pStageChangeMov()
	{
		gamePadState = GamePad.GetState(playerIndex);
		if (intOneScan < 30)
		{
			intOneScan++;
			gamePadMaeState = gamePadState;
			return;
		}
		if (pflgGameOverEnable)
		{
			if (gamePadState.IsButtonDown(Buttons.Start) && !gamePadMaeState.IsButtonDown(Buttons.Start))
			{
				pStageChangeEnd();
				pflgStageChangeEnd = true;
			}
		}
		else if (pflgGameClearEnable && gamePadState.IsButtonDown(Buttons.Start) && !gamePadMaeState.IsButtonDown(Buttons.Start))
		{
			pStageChangeEnd();
			int num = intDestroyPercent(Game1.cPUPort00.pGetCpuCount(), Game1.cPUPort00.pGetGekihaCount());
			Game1.score.pScoreUp(lngDestroyPercentBonus(num, Game1.gameMain.pGetStage()));
			Game1.score.pScoreUp(lngTimeBonus(Game1.gameMain.pGetStage(), Game1.cPUPort00.pGetTime()));
			pflgStageChangeEnd = true;
		}
		gamePadMaeState = gamePadState;
	}

	protected override void LoadContent()
	{
		imgGameClear = base.Game.Content.Load<Texture2D>("PNG\\System\\Comment\\GameClear00");
		imgGameOver = base.Game.Content.Load<Texture2D>("PNG\\System\\Comment\\GameOver");
		font1 = base.Game.Content.Load<SpriteFont>("SpriteFont1");
		imgTextBG00 = base.Game.Content.Load<Texture2D>("PNG\\System\\TextBG02");
		base.LoadContent();
	}

	public void pStageChangeDraw(SpriteBatch aspritesBatch)
	{
		if (imgGameClear == null)
		{
			return;
		}
		if (pflgGameOverEnable)
		{
			aspritesBatch.Draw(imgGameOver, new Vector2(640 - imgGameOver.Width / 2, 360f), null, Color.White, 0f, new Vector2(0f, 0f), new Vector2(1f, 1f), SpriteEffects.None, 1f);
			aspritesBatch.DrawString(font1, "Push Start Button", new Vector2(490f, 510f), Color.White);
			for (int i = 0; i < 24; i++)
			{
				for (int j = 0; j < 24; j++)
				{
					aspritesBatch.Draw(imgTextBG00, new Vector2(i * imgTextBG00.Width, j * imgTextBG00.Height), new Color(96, 96, 96, 96));
				}
			}
		}
		if (!pflgGameClearEnable)
		{
			return;
		}
		aspritesBatch.Draw(imgGameClear, new Vector2(640 - imgGameClear.Width / 2, 310f), null, Color.White, 0f, new Vector2(0f, 0f), new Vector2(1f, 1f), SpriteEffects.None, 1f);
		int num = intDestroyPercent(Game1.cPUPort00.pGetCpuCount(), Game1.cPUPort00.pGetGekihaCount());
		aspritesBatch.DrawString(font1, "DestroyPercent " + num + " Bonus " + lngDestroyPercentBonus(num, Game1.gameMain.pGetStage()), new Vector2(440f, 410f), Color.White);
		aspritesBatch.DrawString(font1, "Time Bonus " + lngTimeBonus(Game1.gameMain.pGetStage(), Game1.cPUPort00.pGetTime()), new Vector2(490f, 460f), Color.White);
		aspritesBatch.DrawString(font1, "Push Start Button", new Vector2(490f, 510f), Color.White);
		for (int i = 0; i < 24; i++)
		{
			for (int j = 0; j < 24; j++)
			{
				aspritesBatch.Draw(imgTextBG00, new Vector2(i * imgTextBG00.Width, j * imgTextBG00.Height), new Color(64, 64, 64, 64));
			}
		}
	}

	private int intDestroyPercent(int intCpuCount, int intGekihaCount)
	{
		return (int)((float)intGekihaCount / (float)intCpuCount * 100f);
	}

	private long lngDestroyPercentBonus(int intDestroyPercent, int intStage)
	{
		long num = intDestroyPercent * intStage * 400;
		if (intDestroyPercent >= 99)
		{
			num += intStage * 20000;
		}
		else if (intDestroyPercent >= 95)
		{
			num += intStage * 10000;
		}
		else if (intDestroyPercent >= 85)
		{
			num += intStage * 2500;
		}
		return num;
	}

	private long lngTimeBonus(int intStage, int intTime)
	{
		switch (intStage)
		{
		case 1:
			if (intTime < 13560)
			{
				return intStage * 40000;
			}
			return 0L;
		case 2:
			if (intTime < 14400)
			{
				return intStage * 40000;
			}
			return 0L;
		case 3:
			if (intTime < 13800)
			{
				return intStage * 40000;
			}
			return 0L;
		case 4:
			if (intTime < 13560)
			{
				return intStage * 40000;
			}
			return 0L;
		default:
			return 0L;
		}
	}

	public override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
	}
}
