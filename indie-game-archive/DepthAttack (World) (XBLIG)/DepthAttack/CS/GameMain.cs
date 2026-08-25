using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DepthAttack.CS;

public class GameMain : DrawableGameComponent
{
	public enum enuGameScene
	{
		cintNone,
		cintMainInit00,
		cintMain00,
		cintMainClear00,
		cintMainInit01,
		cintMain01,
		cintMainClear01,
		cintMainInit02,
		cintMain02,
		cintMainClear02,
		cintMainInit03,
		cintMain03,
		cintMainClear03,
		cintMainInit04,
		cintMain04,
		cintMainClear04,
		cintGameClear,
		cintGameOver,
		cintRankIn,
		cintRecord
	}

	private const int cintOneScan = 60;

	private SpriteBatch spritesBatch;

	public PlayerIndex playerIndex;

	private GamePadState gamePadState;

	private GamePadState gamePadMaeState;

	public bool pflgPause = false;

	private SpriteFont font1;

	public int intOneScan;

	public int pintStage = 0;

	private enuGameScene enuGameMainScene = enuGameScene.cintNone;

	public int pGetStage()
	{
		return pintStage;
	}

	public GameMain(Game game)
		: base(game)
	{
	}

	public override void Initialize()
	{
		enuGameMainScene = enuGameScene.cintNone;
		pflgPause = false;
		base.Initialize();
	}

	private void MainInit()
	{
		Game1.bG.Initialize();
		Game1.bG02.Initialize();
		Game1.player.Initialize();
		Game1.playerVulcan.Initialize();
		Game1.playerHoming.Initialize();
		Game1.syougai.Initialize();
		Game1.bakuhatu.Initialize();
		Game1.item.Initialize();
		Game1.cPU00.Initialize();
		Game1.cPUBOSS00.Initialize();
		Game1.cPUPort00.Initialize();
		Game1.cPUAI00.Initialize();
		Game1.cPUTama.Initialize();
		Game1.score.Initialize();
		Game1.hPBar.Initialize();
		Game1.stageChange.Initialize();
		Game1.gameMainClear.Initialize();
	}

	private void MainUpdate()
	{
		if (!pflgPause)
		{
			PlayerOperationUpDate();
			Game1.playerVulcan.pPlayerVulcanUpdate();
			Game1.playerVulcan.pPlayerVulcanHantei();
			Game1.playerHoming.pPlayerHomingUpdate();
			Game1.playerHoming.pPlayerHomingHantei();
			Game1.player.pPlayerUpDate();
			Game1.syougai.pSyougaiUpdate();
			Game1.bakuhatu.pBakuhatuUpdate();
			Game1.bG02.BG02Update();
			Game1.cPUPort00.pPortUpDate();
			Game1.cPU00.pCPU00Update();
			Game1.cPUBOSS00.pCPUBOSSUpdate();
			Game1.cPUTama.pCPUTamaUpdate();
			Game1.cPUTama.pCPUTamaHantei();
			Game1.item.pItemUpdate();
			Game1.player.pMapScrollMov(new Vector2(Game1.player.psrcPlayerCore.pVecIti.X + 640f, Game1.player.psrcPlayerCore.pVecIti.Y + 360f));
		}
		else
		{
			PlayerOperationUpDate();
		}
	}

	public override void Update(GameTime gameTime)
	{
		switch (enuGameMainScene)
		{
		case enuGameScene.cintNone:
			base.Game.Components.Add(Game1.bG02);
			base.Game.Components.Add(Game1.player);
			base.Game.Components.Add(Game1.playerVulcan);
			base.Game.Components.Add(Game1.playerHoming);
			base.Game.Components.Add(Game1.syougai);
			base.Game.Components.Add(Game1.bakuhatu);
			base.Game.Components.Add(Game1.item);
			base.Game.Components.Add(Game1.cPU00);
			base.Game.Components.Add(Game1.cPUBOSS00);
			base.Game.Components.Add(Game1.cPUPort00);
			base.Game.Components.Add(Game1.cPUAI00);
			base.Game.Components.Add(Game1.cPUTama);
			base.Game.Components.Add(Game1.score);
			base.Game.Components.Add(Game1.hPBar);
			base.Game.Components.Add(Game1.stageChange);
			base.Game.Components.Add(Game1.gameMainClear);
			enuGameMainScene = enuGameScene.cintMainInit00;
			if (pintStage == 1)
			{
				enuGameMainScene = enuGameScene.cintMainInit01;
			}
			else if (pintStage == 2)
			{
				enuGameMainScene = enuGameScene.cintMainInit02;
			}
			else if (pintStage == 3)
			{
				enuGameMainScene = enuGameScene.cintMainInit03;
			}
			else if (pintStage == 4)
			{
				enuGameMainScene = enuGameScene.cintMainInit04;
			}
			Game1.player.pPlayerLogin();
			break;
		case enuGameScene.cintMainInit00:
			if (intOneScan < 60)
			{
				intOneScan++;
				break;
			}
			Game1.player.pMapScrollMov(new Vector2(640f, 360f));
			Game1.player.pMapScrollMov(new Vector2(Game1.player.psrcPlayerCore.pVecIti.X + 640f, Game1.player.psrcPlayerCore.pVecIti.Y + 360f));
			Game1.cPUPort00.pStage(0);
			enuGameMainScene = enuGameScene.cintMain00;
			break;
		case enuGameScene.cintMain00:
			MainUpdate();
			break;
		case enuGameScene.cintMainInit01:
			if (intOneScan < 60)
			{
				intOneScan++;
				break;
			}
			MainInit();
			Game1.bG02.pBGTexture(0);
			pintStage = 1;
			Game1.cPUPort00.pStage(pintStage);
			Game1.player.pPleyerHpMax();
			enuGameMainScene = enuGameScene.cintMain01;
			break;
		case enuGameScene.cintMain01:
			MainUpdate();
			if (Game1.cPUPort00.pIsStageClear())
			{
				enuGameMainScene = enuGameScene.cintMainClear01;
				Game1.stageChange.pflgGameClearEnable = true;
			}
			if (Game1.player.psrcPlayerCore.pintHp <= 0)
			{
				enuGameMainScene = enuGameScene.cintGameOver;
				Game1.stageChange.pflgGameOverEnable = true;
			}
			break;
		case enuGameScene.cintMainClear01:
			Game1.stageChange.pStageChangeMov();
			if (Game1.stageChange.pflgStageChangeEnd)
			{
				enuGameMainScene = enuGameScene.cintMainInit02;
				intOneScan = 0;
				pintStage++;
				Game1.continueContent.pintStage = pintStage;
				Game1.continueContent.ContinueCopy(0);
				Game1.continueContent.ContinueSave();
			}
			break;
		case enuGameScene.cintMainInit02:
			if (intOneScan < 60)
			{
				intOneScan++;
				break;
			}
			MainInit();
			Game1.bG02.pBGTexture(1);
			Game1.cPUPort00.pStage(pintStage);
			Game1.player.pPleyerHpMax();
			enuGameMainScene = enuGameScene.cintMain02;
			break;
		case enuGameScene.cintMain02:
			MainUpdate();
			if (Game1.cPUPort00.pIsStageClear())
			{
				enuGameMainScene = enuGameScene.cintMainClear02;
				Game1.stageChange.pflgGameClearEnable = true;
			}
			if (Game1.player.psrcPlayerCore.pintHp <= 0)
			{
				enuGameMainScene = enuGameScene.cintGameOver;
				Game1.stageChange.pflgGameOverEnable = true;
			}
			break;
		case enuGameScene.cintMainClear02:
			Game1.stageChange.pStageChangeMov();
			if (Game1.stageChange.pflgStageChangeEnd)
			{
				enuGameMainScene = enuGameScene.cintMainInit03;
				intOneScan = 0;
				pintStage++;
				Game1.continueContent.pintStage = pintStage;
				Game1.continueContent.ContinueCopy(0);
				Game1.continueContent.ContinueSave();
			}
			break;
		case enuGameScene.cintMainInit03:
			if (intOneScan < 60)
			{
				intOneScan++;
				break;
			}
			MainInit();
			Game1.bG02.pBGTexture(0);
			Game1.cPUPort00.pStage(pintStage);
			Game1.player.pPleyerHpMax();
			enuGameMainScene = enuGameScene.cintMain03;
			break;
		case enuGameScene.cintMain03:
			MainUpdate();
			if (Game1.cPUPort00.pIsStageClear())
			{
				enuGameMainScene = enuGameScene.cintMainClear03;
				Game1.stageChange.pflgGameClearEnable = true;
			}
			if (Game1.player.psrcPlayerCore.pintHp <= 0)
			{
				enuGameMainScene = enuGameScene.cintGameOver;
				Game1.stageChange.pflgGameOverEnable = true;
			}
			break;
		case enuGameScene.cintMainClear03:
			Game1.stageChange.pStageChangeMov();
			if (Game1.stageChange.pflgStageChangeEnd)
			{
				enuGameMainScene = enuGameScene.cintMainInit04;
				intOneScan = 0;
				pintStage++;
				Game1.continueContent.pintStage = pintStage;
				Game1.continueContent.ContinueCopy(0);
				Game1.continueContent.ContinueSave();
			}
			break;
		case enuGameScene.cintMainInit04:
			if (intOneScan < 60)
			{
				intOneScan++;
				break;
			}
			MainInit();
			Game1.bG02.pBGTexture(0);
			Game1.cPUPort00.pStage(pintStage);
			Game1.player.pPleyerHpMax();
			enuGameMainScene = enuGameScene.cintMain04;
			break;
		case enuGameScene.cintMain04:
			MainUpdate();
			if (Game1.cPUPort00.pIsStageClear())
			{
				enuGameMainScene = enuGameScene.cintMainClear04;
				Game1.stageChange.pflgGameClearEnable = true;
			}
			if (Game1.player.psrcPlayerCore.pintHp <= 0)
			{
				enuGameMainScene = enuGameScene.cintGameOver;
				Game1.stageChange.pflgGameOverEnable = true;
			}
			break;
		case enuGameScene.cintMainClear04:
			Game1.stageChange.pStageChangeMov();
			if (Game1.stageChange.pflgStageChangeEnd)
			{
				Game1.gameMainClear.pflgEnable = true;
				enuGameMainScene = enuGameScene.cintGameClear;
				intOneScan = 0;
			}
			break;
		case enuGameScene.cintGameClear:
			Game1.gameMainClear.GameMainClearMov();
			if (Game1.gameMainClear.pflgGameMainClearEnd)
			{
				MainInit();
				base.Game.Components.Remove(Game1.bG02);
				base.Game.Components.Remove(Game1.player);
				base.Game.Components.Remove(Game1.playerVulcan);
				base.Game.Components.Remove(Game1.playerHoming);
				base.Game.Components.Remove(Game1.syougai);
				base.Game.Components.Remove(Game1.bakuhatu);
				base.Game.Components.Remove(Game1.item);
				base.Game.Components.Remove(Game1.cPU00);
				base.Game.Components.Remove(Game1.cPUBOSS00);
				base.Game.Components.Remove(Game1.cPUPort00);
				base.Game.Components.Remove(Game1.cPUAI00);
				base.Game.Components.Remove(Game1.cPUTama);
				base.Game.Components.Remove(Game1.score);
				base.Game.Components.Remove(Game1.hPBar);
				base.Game.Components.Remove(Game1.stageChange);
				base.Game.Components.Remove(Game1.gameMainClear);
				if (Game1.rankIn.intFunRankIn(Game1.score.plngScore) < Game1.recordContent.pscoreRecord.Length)
				{
					Game1.bGM.pflgSERankIn[0] = true;
					Game1.rankIn.plngScore = Game1.score.plngScore;
					Game1.rankIn.playerIndex = playerIndex;
					enuGameMainScene = enuGameScene.cintRankIn;
				}
				else
				{
					base.Game.Components.Remove(Game1.gameMain);
					Game1.titleContent.Initialize();
					base.Game.Components.Add(Game1.titleContent);
				}
			}
			break;
		case enuGameScene.cintGameOver:
			Game1.stageChange.pStageChangeMov();
			if (Game1.stageChange.pflgStageChangeEnd)
			{
				MainInit();
				base.Game.Components.Remove(Game1.bG02);
				base.Game.Components.Remove(Game1.player);
				base.Game.Components.Remove(Game1.playerVulcan);
				base.Game.Components.Remove(Game1.playerHoming);
				base.Game.Components.Remove(Game1.syougai);
				base.Game.Components.Remove(Game1.bakuhatu);
				base.Game.Components.Remove(Game1.item);
				base.Game.Components.Remove(Game1.cPU00);
				base.Game.Components.Remove(Game1.cPUBOSS00);
				base.Game.Components.Remove(Game1.cPUPort00);
				base.Game.Components.Remove(Game1.cPUAI00);
				base.Game.Components.Remove(Game1.cPUTama);
				base.Game.Components.Remove(Game1.score);
				base.Game.Components.Remove(Game1.hPBar);
				base.Game.Components.Remove(Game1.stageChange);
				base.Game.Components.Remove(Game1.gameMainClear);
				if (Game1.rankIn.intFunRankIn(Game1.score.plngScore) < Game1.recordContent.pscoreRecord.Length)
				{
					Game1.rankIn.plngScore = Game1.score.plngScore;
					Game1.rankIn.playerIndex = playerIndex;
					enuGameMainScene = enuGameScene.cintRankIn;
					Game1.bGM.pflgSERankIn[0] = true;
				}
				else
				{
					base.Game.Components.Remove(Game1.gameMain);
					Game1.titleContent.Initialize();
					base.Game.Components.Add(Game1.titleContent);
				}
			}
			break;
		case enuGameScene.cintRankIn:
			Game1.rankIn.RankInMov(gameTime);
			if (Game1.rankIn.pflgRankInEnd)
			{
				enuGameMainScene = enuGameScene.cintRecord;
				Game1.recordContent.playerIndex = playerIndex;
				Game1.recordContent.pflgRecordEnd = false;
			}
			break;
		case enuGameScene.cintRecord:
			Game1.recordContent.pRecordMov(gameTime);
			if (Game1.recordContent.pflgRecordEnd)
			{
				base.Game.Components.Remove(Game1.gameMain);
				Game1.titleContent.Initialize();
				base.Game.Components.Add(Game1.titleContent);
			}
			break;
		}
		base.Update(gameTime);
	}

	protected override void LoadContent()
	{
		spritesBatch = new SpriteBatch(base.GraphicsDevice);
		font1 = base.Game.Content.Load<SpriteFont>("SpriteFont1");
		base.LoadContent();
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void MainDraw()
	{
		Matrix identity = Matrix.Identity;
		identity = Matrix.CreateTranslation(Game1.player.pvec3Scroll);
		spritesBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, null, identity);
		Game1.bG02.BG02Draw(spritesBatch);
		Game1.player.pPlayerDraw(spritesBatch);
		Game1.playerVulcan.pPlayerVulcanDraw(spritesBatch);
		Game1.playerHoming.pPlayerHomingDraw(spritesBatch);
		Game1.syougai.pSyougaiDraw(spritesBatch);
		Game1.bakuhatu.pBakuhatuDraw(spritesBatch);
		Game1.cPU00.pCPU00Draw(spritesBatch);
		Game1.cPUBOSS00.pCPUBOSS00Draw(spritesBatch);
		Game1.cPUTama.pCPUTamaDraw(spritesBatch);
		Game1.item.pItemDraw(spritesBatch);
		spritesBatch.End();
		spritesBatch.Begin();
		Game1.score.pScoreDraw(spritesBatch);
		Game1.hPBar.PlayerHpDraw(spritesBatch, Game1.player.psrcPlayerCore.pintHp);
		if (Game1.cPUBOSS00.psrcCPUBOSS00Core[1].pflgEnable)
		{
			Game1.hPBar.CPUBOSSHpDraw(spritesBatch, (int)Game1.cPUBOSS00.psrcCPUBOSS00Core[1].pfltHP);
		}
		else if (Game1.cPUBOSS00.psrcCPUBOSS00Core[0].pflgEnable)
		{
			Game1.hPBar.CPUBOSSHpDraw(spritesBatch, (int)Game1.cPUBOSS00.psrcCPUBOSS00Core[0].pfltHP);
		}
		else
		{
			Game1.hPBar.CPUBOSSHpDraw(spritesBatch, 0);
		}
		if (pflgPause)
		{
			PauseDraw(spritesBatch);
		}
		Game1.stageChange.pStageChangeDraw(spritesBatch);
		Game1.gameMainClear.GameMainClearDraw(spritesBatch);
		spritesBatch.End();
	}

	private void LoadingDraw(SpriteBatch aspritesBatch)
	{
		if (font1 != null)
		{
			aspritesBatch.DrawString(font1, "Now Loading", new Vector2(540f, 260f), Color.White);
		}
	}

	private void PauseDraw(SpriteBatch aspritesBatch)
	{
		if (font1 != null)
		{
			aspritesBatch.DrawString(font1, "PAUSE", new Vector2(590f, 360f), Color.White);
		}
	}

	public override void Draw(GameTime gameTime)
	{
		switch (enuGameMainScene)
		{
		case enuGameScene.cintNone:
			spritesBatch.Begin();
			LoadingDraw(spritesBatch);
			spritesBatch.End();
			break;
		case enuGameScene.cintMainInit00:
			spritesBatch.Begin();
			LoadingDraw(spritesBatch);
			spritesBatch.End();
			break;
		case enuGameScene.cintMain00:
			MainDraw();
			break;
		case enuGameScene.cintMainClear00:
			MainDraw();
			break;
		case enuGameScene.cintMainInit01:
			spritesBatch.Begin();
			LoadingDraw(spritesBatch);
			spritesBatch.End();
			break;
		case enuGameScene.cintMain01:
			MainDraw();
			break;
		case enuGameScene.cintMainClear01:
			MainDraw();
			break;
		case enuGameScene.cintMainInit02:
			spritesBatch.Begin();
			LoadingDraw(spritesBatch);
			spritesBatch.End();
			break;
		case enuGameScene.cintMain02:
			MainDraw();
			break;
		case enuGameScene.cintMainClear02:
			MainDraw();
			break;
		case enuGameScene.cintMainInit03:
			spritesBatch.Begin();
			LoadingDraw(spritesBatch);
			spritesBatch.End();
			break;
		case enuGameScene.cintMain03:
			MainDraw();
			break;
		case enuGameScene.cintMainClear03:
			MainDraw();
			break;
		case enuGameScene.cintMainInit04:
			spritesBatch.Begin();
			LoadingDraw(spritesBatch);
			spritesBatch.End();
			break;
		case enuGameScene.cintMain04:
			MainDraw();
			break;
		case enuGameScene.cintMainClear04:
			MainDraw();
			break;
		case enuGameScene.cintGameClear:
			MainDraw();
			break;
		case enuGameScene.cintGameOver:
			MainDraw();
			break;
		case enuGameScene.cintRankIn:
			spritesBatch.Begin();
			Game1.rankIn.pRankInDraw(gameTime, spritesBatch);
			spritesBatch.End();
			break;
		case enuGameScene.cintRecord:
			spritesBatch.Begin();
			Game1.recordContent.pRecordDraw(spritesBatch);
			spritesBatch.End();
			break;
		}
		base.Draw(gameTime);
	}

	private void PlayerOperationUpDate()
	{
		gamePadState = GamePad.GetState(playerIndex);
		if (gamePadState.Buttons.Start == ButtonState.Pressed && gamePadMaeState.Buttons.Start == ButtonState.Released)
		{
			if (!pflgPause)
			{
				pflgPause = true;
			}
			else
			{
				pflgPause = false;
			}
		}
		gamePadMaeState = gamePadState;
	}
}
