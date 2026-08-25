using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DepthAttack.CS;

public class TitleContent : DrawableGameComponent
{
	private enum selectMode
	{
		GameStart,
		Continue,
		Record,
		Help_Credit,
		Option,
		Exit,
		Game_Full_Mode
	}

	private enum Option
	{
		BGM,
		SE,
		OK,
		Back_To_Title
	}

	private const int cintSEVolumeMax = 10;

	private const int cintBGMVolumeMax = 10;

	private const int cintCoursolPointMax = 6;

	private const int cintCoursolPointGameStart = 0;

	private const int cintCoursolPointExitGame = 5;

	private const int cintCoursolOptionPointMax = 3;

	private SpriteBatch spritesBatch;

	private Texture2D imgTitle;

	private Texture2D imgCoursor;

	private Texture2D imgGarageSoft;

	private SpriteFont font1;

	private bool flgHelpEnable;

	private Texture2D imgHelp;

	public bool flgCommandSelectEnable = false;

	private bool flgOptionEnable;

	private int intSEVolume = 0;

	private int intBGMVolume = 0;

	private bool flgRecord = false;

	private bool flgStart;

	private int intCoursolItiY;

	private double dblCoursolItiOmega;

	public int intOneScan;

	private int intCoursolPoint;

	private int intCoursolOptionPoint;

	private PlayerIndex playerIndex;

	private GamePadState gamePadMaeState;

	private int ingPadUpNagaOsi;

	private int ingPadDownNagaOsi;

	public TitleContent(Game game)
		: base(game)
	{
		base.Enabled = true;
		flgStart = false;
		intCoursolPoint = 0;
		dblCoursolItiOmega = 0.0;
		intCoursolItiY = 0;
		playerIndex = PlayerIndex.One;
		ingPadUpNagaOsi = 0;
		ingPadDownNagaOsi = 0;
		flgOptionEnable = false;
	}

	public override void Initialize()
	{
		flgStart = false;
		intCoursolPoint = 0;
		intCoursolItiY = 0;
		ingPadUpNagaOsi = 0;
		ingPadDownNagaOsi = 0;
		intOneScan = 0;
		playerIndex = PlayerIndex.One;
		flgOptionEnable = false;
		flgRecord = false;
		Game1.bGM.pflgBGMOFF();
		base.Initialize();
	}

	protected override void LoadContent()
	{
		spritesBatch = new SpriteBatch(base.GraphicsDevice);
		imgTitle = base.Game.Content.Load<Texture2D>("PNG\\System\\Comment\\Title13");
		imgCoursor = base.Game.Content.Load<Texture2D>("PNG\\System\\Cursor01");
		font1 = base.Game.Content.Load<SpriteFont>("SpriteFont1");
		imgGarageSoft = base.Game.Content.Load<Texture2D>("PNG\\System\\Comment\\GarageSoft01");
		imgHelp = base.Game.Content.Load<Texture2D>("PNG\\System\\Pause25");
		base.LoadContent();
	}

	public override void Update(GameTime gameTime)
	{
		GamePadState state = GamePad.GetState(this.playerIndex);
		if (intOneScan < 30)
		{
			intOneScan++;
			gamePadMaeState = state;
			base.Update(gameTime);
			return;
		}
		if (flgStart && !flgHelpEnable && !flgOptionEnable && !flgRecord)
		{
			if (((double)state.ThumbSticks.Left.Y >= 0.5 && (double)gamePadMaeState.ThumbSticks.Left.Y < 0.5) || (state.DPad.Up == ButtonState.Pressed && gamePadMaeState.DPad.Up == ButtonState.Released))
			{
				intCoursolPoint--;
				if (Guide.IsTrialMode)
				{
					if (intCoursolPoint < 0)
					{
						intCoursolPoint = 6;
					}
				}
				else if (intCoursolPoint < 0)
				{
					intCoursolPoint = 5;
				}
			}
			if (((double)state.ThumbSticks.Left.Y <= -0.5 && (double)gamePadMaeState.ThumbSticks.Left.Y > -0.5) || (state.DPad.Down == ButtonState.Pressed && gamePadMaeState.DPad.Down == ButtonState.Released))
			{
				intCoursolPoint++;
				if (Guide.IsTrialMode)
				{
					if (intCoursolPoint > 6)
					{
						intCoursolPoint = 0;
					}
				}
				else if (intCoursolPoint > 5)
				{
					intCoursolPoint = 0;
				}
			}
			if ((double)state.ThumbSticks.Left.Y >= 0.5 || state.DPad.Up == ButtonState.Pressed)
			{
				if (ingPadUpNagaOsi < 16)
				{
					ingPadUpNagaOsi++;
				}
				else if (gameTime.TotalGameTime.Milliseconds % 4 == 0)
				{
					intCoursolPoint--;
					if (Guide.IsTrialMode)
					{
						if (intCoursolPoint < 0)
						{
							intCoursolPoint = 6;
						}
					}
					else if (intCoursolPoint < 0)
					{
						intCoursolPoint = 5;
					}
				}
			}
			else
			{
				ingPadUpNagaOsi = 0;
			}
			if ((double)state.ThumbSticks.Left.Y <= -0.5 || state.DPad.Down == ButtonState.Pressed)
			{
				if (ingPadDownNagaOsi < 16)
				{
					ingPadDownNagaOsi++;
				}
				else if (gameTime.TotalGameTime.TotalMilliseconds % 4.0 == 0.0)
				{
					intCoursolPoint++;
					if (Guide.IsTrialMode)
					{
						if (intCoursolPoint > 6)
						{
							intCoursolPoint = 0;
						}
					}
					else if (intCoursolPoint > 5)
					{
						intCoursolPoint = 0;
					}
				}
			}
			else
			{
				ingPadDownNagaOsi = 0;
			}
			if ((state.Buttons.A == ButtonState.Pressed && gamePadMaeState.Buttons.A == ButtonState.Released) || (state.Buttons.Start == ButtonState.Pressed && gamePadMaeState.Buttons.Start == ButtonState.Released))
			{
				switch (intCoursolPoint)
				{
				case 0:
					Game1.bGM.pflgSEKetteiStart[0] = true;
					Game1.gameMain.playerIndex = this.playerIndex;
					Game1.player.psctGamePad.pplayerIndex = this.playerIndex;
					Game1.stageChange.playerIndex = this.playerIndex;
					Game1.gameMainClear.playerIndex = this.playerIndex;
					Game1.score.plngHighScore = Game1.recordContent.pscoreRecord[0].lngScore;
					Game1.gameMain.Initialize();
					Game1.bG.penuBGSelect = BG.enuBGScene.Main01;
					base.Game.Components.Add(Game1.gameMain);
					base.Game.Components.Remove(Game1.bG02);
					base.Game.Components.Remove(Game1.titleContent);
					Game1.score.plngScore = 0L;
					Game1.gameMain.pintStage = 1;
					break;
				case 1:
					if (Game1.continueContent.pflgRead)
					{
						Game1.bGM.pflgSEKetteiStart[0] = true;
						Game1.gameMain.playerIndex = this.playerIndex;
						Game1.stageChange.playerIndex = this.playerIndex;
						Game1.gameMainClear.playerIndex = this.playerIndex;
						Game1.score.plngHighScore = Game1.recordContent.pscoreRecord[0].lngScore;
						Game1.gameMain.Initialize();
						base.Game.Components.Add(Game1.gameMain);
						base.Game.Components.Remove(Game1.bG02);
						Game1.continueContent.PlayerCopy(0);
						Game1.gameMain.pintStage = Game1.continueContent.pintStage;
						base.Game.Components.Remove(Game1.titleContent);
					}
					else
					{
						Game1.bGM.pflgSECancelStart[0] = true;
					}
					break;
				case 2:
					Game1.recordContent.playerIndex = this.playerIndex;
					flgRecord = true;
					Game1.recordContent.pflgRecordEnd = false;
					break;
				case 3:
					flgHelpEnable = true;
					break;
				case 4:
				{
					flgOptionEnable = true;
					float num = Game1.bGM.fltBGMVolume * 10f;
					intBGMVolume = (int)num;
					num = Game1.bGM.fltSEVolume * 10f;
					intSEVolume = (int)num;
					break;
				}
				case 5:
					base.Game.Components.Remove(this);
					UnloadContent();
					base.Game.Exit();
					break;
				case 6:
				{
					if (!Guide.IsTrialMode)
					{
						break;
					}
					SignedInGamer signedInGamer = Gamer.SignedInGamers[this.playerIndex];
					if (signedInGamer != null && !signedInGamer.IsGuest)
					{
						string gamertag = signedInGamer.Gamertag;
						Guide.ShowMarketplace(this.playerIndex);
						Game1.titleContent.Initialize();
						break;
					}
					GamePadState state2 = GamePad.GetState(this.playerIndex);
					if (!Guide.IsVisible && state2.Buttons.A == ButtonState.Pressed)
					{
						Guide.ShowSignIn(4, onlineOnly: true);
						Game1.titleContent.Initialize();
						return;
					}
					break;
				}
				default:
					base.Game.Components.Remove(this);
					UnloadContent();
					break;
				}
			}
			if (state.Buttons.Back == ButtonState.Pressed && gamePadMaeState.Buttons.Back == ButtonState.Released)
			{
				flgStart = false;
				intCoursolPoint = 0;
				Game1.bGM.pflgBGMOFF();
			}
		}
		else if (!flgStart && !flgHelpEnable && !flgOptionEnable && !flgRecord)
		{
			this.playerIndex = PlayerIndex.One;
			for (PlayerIndex playerIndex = PlayerIndex.One; playerIndex <= PlayerIndex.Four; playerIndex++)
			{
				if (GamePad.GetState(playerIndex).Buttons.A == ButtonState.Pressed || GamePad.GetState(playerIndex).Buttons.Start == ButtonState.Pressed)
				{
					this.playerIndex = playerIndex;
					flgStart = true;
					state = GamePad.GetState(this.playerIndex);
					Game1.bGM.pflgBGMON(0);
					Game1.bGM.BGMSetVolume();
					break;
				}
			}
		}
		else if (flgStart && flgHelpEnable && !flgOptionEnable && !flgRecord)
		{
			if ((state.Buttons.A == ButtonState.Pressed && gamePadMaeState.Buttons.A == ButtonState.Released) || (state.Buttons.Start == ButtonState.Pressed && gamePadMaeState.Buttons.Start == ButtonState.Released))
			{
				flgHelpEnable = false;
				intCoursolPoint = 3;
			}
		}
		else if (flgStart && !flgHelpEnable && flgOptionEnable && !flgRecord)
		{
			if (((double)state.ThumbSticks.Left.Y >= 0.5 && (double)gamePadMaeState.ThumbSticks.Left.Y < 0.5) || (state.DPad.Up == ButtonState.Pressed && gamePadMaeState.DPad.Up == ButtonState.Released))
			{
				intCoursolOptionPoint--;
				if (intCoursolOptionPoint < 0)
				{
					intCoursolOptionPoint = 3;
				}
			}
			if (((double)state.ThumbSticks.Left.Y <= -0.5 && (double)gamePadMaeState.ThumbSticks.Left.Y > -0.5) || (state.DPad.Down == ButtonState.Pressed && gamePadMaeState.DPad.Down == ButtonState.Released))
			{
				intCoursolOptionPoint++;
				if (intCoursolOptionPoint > 3)
				{
					intCoursolOptionPoint = 0;
				}
			}
			if ((double)state.ThumbSticks.Left.Y >= 0.5 || state.DPad.Up == ButtonState.Pressed)
			{
				if (ingPadUpNagaOsi < 16)
				{
					ingPadUpNagaOsi++;
				}
				else if (gameTime.TotalGameTime.Milliseconds % 4 == 0)
				{
					intCoursolOptionPoint--;
					if (intCoursolOptionPoint < 0)
					{
						intCoursolOptionPoint = 3;
					}
				}
			}
			else
			{
				ingPadUpNagaOsi = 0;
			}
			if ((double)state.ThumbSticks.Left.Y <= -0.5 || state.DPad.Down == ButtonState.Pressed)
			{
				if (ingPadDownNagaOsi < 16)
				{
					ingPadDownNagaOsi++;
				}
				else if (gameTime.TotalGameTime.TotalMilliseconds % 4.0 == 0.0)
				{
					intCoursolOptionPoint++;
					if (intCoursolOptionPoint > 3)
					{
						intCoursolOptionPoint = 0;
					}
				}
			}
			else
			{
				ingPadDownNagaOsi = 0;
			}
			if ((state.Buttons.A == ButtonState.Pressed && gamePadMaeState.Buttons.A == ButtonState.Released) || (state.Buttons.Start == ButtonState.Pressed && gamePadMaeState.Buttons.Start == ButtonState.Released))
			{
				switch (intCoursolOptionPoint)
				{
				case 2:
					Game1.bGM.fltBGMVolume = (float)intBGMVolume / 10f;
					Game1.bGM.fltSEVolume = (float)intSEVolume / 10f;
					Game1.bGM.BGMSetVolume();
					if (!Guide.IsTrialMode)
					{
						Game1.bGM.volumeSave();
					}
					Game1.bGM.pflgSEKetteiStart[0] = true;
					break;
				case 3:
					flgOptionEnable = false;
					break;
				}
			}
			else if (state.Buttons.B == ButtonState.Pressed && gamePadMaeState.Buttons.B == ButtonState.Released)
			{
				flgOptionEnable = false;
			}
			if ((intCoursolOptionPoint == 0 && state.DPad.Right == ButtonState.Pressed && gamePadMaeState.DPad.Right == ButtonState.Released) || ((double)state.ThumbSticks.Left.X >= 0.5 && (double)gamePadMaeState.ThumbSticks.Left.X <= 0.5))
			{
				intBGMVolume++;
				if (intBGMVolume > 10)
				{
					intBGMVolume = 0;
				}
			}
			else if ((intCoursolOptionPoint == 0 && state.DPad.Left == ButtonState.Pressed && gamePadMaeState.DPad.Left == ButtonState.Released) || ((double)state.ThumbSticks.Left.X <= -0.5 && (double)gamePadMaeState.ThumbSticks.Left.X > -0.5))
			{
				intBGMVolume--;
				if (intBGMVolume < 0)
				{
					intBGMVolume = 10;
				}
			}
			if ((intCoursolOptionPoint == 1 && state.DPad.Right == ButtonState.Pressed && gamePadMaeState.DPad.Right == ButtonState.Released) || ((double)state.ThumbSticks.Left.X >= 0.5 && (double)gamePadMaeState.ThumbSticks.Left.X <= 0.5))
			{
				intSEVolume++;
				if (intSEVolume > 10)
				{
					intSEVolume = 0;
				}
			}
			else if ((intCoursolOptionPoint == 1 && state.DPad.Left == ButtonState.Pressed && gamePadMaeState.DPad.Left == ButtonState.Released) || ((double)state.ThumbSticks.Left.X <= -0.5 && (double)gamePadMaeState.ThumbSticks.Left.X > -0.5))
			{
				intSEVolume--;
				if (intSEVolume < 0)
				{
					intSEVolume = 10;
				}
			}
		}
		else if (flgStart && !flgHelpEnable && !flgOptionEnable && flgRecord)
		{
			Game1.recordContent.pRecordMov(gameTime);
			if (Game1.recordContent.pflgRecordEnd)
			{
				flgStart = true;
				flgHelpEnable = false;
				flgOptionEnable = false;
				flgRecord = false;
			}
		}
		if ((double)state.Triggers.Right >= 0.5 && (double)gamePadMaeState.Triggers.Right < 0.5)
		{
			if (flgCommandSelectEnable)
			{
				Game1.bGM.pflgSECancelStart[0] = true;
				flgCommandSelectEnable = false;
			}
			else
			{
				Game1.bGM.pflgSEKetteiStart[0] = true;
				flgCommandSelectEnable = true;
			}
		}
		subCoursolIti();
		gamePadMaeState = state;
		base.Update(gameTime);
	}

	private void subCoursolIti()
	{
		dblCoursolItiOmega += 0.2;
		if (dblCoursolItiOmega > 360.0)
		{
			dblCoursolItiOmega = 0.0;
		}
	}

	private void subSinIti(ref double dblItiOmega, double dblKakudo)
	{
		dblItiOmega += dblKakudo;
		if (dblItiOmega > 360.0)
		{
			dblItiOmega -= 360.0;
		}
	}

	protected override void UnloadContent()
	{
		spritesBatch.Dispose();
		imgCoursor.Dispose();
		base.UnloadContent();
	}

	public override void Draw(GameTime gameTime)
	{
		spritesBatch.Begin();
		Game1.bG02.BG02Draw(spritesBatch);
		if (!flgRecord)
		{
			spritesBatch.Draw(imgTitle, new Vector2(640 - imgTitle.Width / 2, 180 - imgTitle.Height / 2), null, Color.White);
		}
		spritesBatch.Draw(imgGarageSoft, new Vector2(1280 - imgGarageSoft.Width - 20, 720 - imgGarageSoft.Height - 20), Color.White);
		if (flgStart && !flgHelpEnable && !flgOptionEnable && !flgRecord)
		{
			spritesBatch.Draw(imgCoursor, new Vector2(520f, 410 + intCoursolPoint * 40 - 5 + (int)((double)intCoursolItiY + Math.Sin(dblCoursolItiOmega) * 10.0)), Color.White);
			spritesBatch.DrawString(font1, "New Game", new Vector2(560f, 410f), Color.White);
			spritesBatch.DrawString(font1, "Continue", new Vector2(560f, 450f), Color.White);
			spritesBatch.DrawString(font1, "Record", new Vector2(560f, 490f), Color.White);
			spritesBatch.DrawString(font1, "Help & Credit", new Vector2(560f, 530f), Color.White);
			spritesBatch.DrawString(font1, "Option", new Vector2(560f, 570f), Color.White);
			spritesBatch.DrawString(font1, "Exit Game", new Vector2(560f, 610f), Color.White);
			if (Guide.IsTrialMode)
			{
				spritesBatch.DrawString(font1, "Unlock Full Game", new Vector2(560f, 650f), Color.White);
			}
		}
		else if (!flgStart && !flgHelpEnable && !flgOptionEnable && !flgRecord)
		{
			spritesBatch.DrawString(font1, "Push Start Button", new Vector2(490f, 540f), Color.White);
			spritesBatch.Draw(imgCoursor, new Vector2(445f, 535 + (int)((double)intCoursolItiY + Math.Sin(dblCoursolItiOmega) * 10.0)), Color.White);
		}
		else if (flgStart && !flgHelpEnable && !flgOptionEnable && flgRecord)
		{
			Game1.recordContent.pRecordDraw(spritesBatch);
		}
		if (flgStart && flgHelpEnable && !flgOptionEnable)
		{
			spritesBatch.Draw(imgHelp, new Vector2(0f, 0f), Color.White);
			spritesBatch.Draw(imgCoursor, new Vector2(510f, 655 + (int)(Math.Sin(dblCoursolItiOmega) * 15.0)), Color.White);
			spritesBatch.DrawString(font1, "Back to Title", new Vector2(560f, 650f), Color.White);
		}
		if (flgStart && !flgHelpEnable && flgOptionEnable)
		{
			spritesBatch.Draw(imgCoursor, new Vector2(520f, 530 + intCoursolOptionPoint * 40 - 5 + (int)((double)intCoursolItiY + Math.Sin(dblCoursolItiOmega) * 10.0)), Color.White);
			spritesBatch.DrawString(font1, "Volume", new Vector2(640f, 490f), Color.White);
			spritesBatch.DrawString(font1, "BGM", new Vector2(560f, 530f), Color.White);
			spritesBatch.DrawString(font1, "SE", new Vector2(560f, 570f), Color.White);
			spritesBatch.DrawString(font1, "OK", new Vector2(560f, 610f), Color.White);
			spritesBatch.DrawString(font1, "Back to Title", new Vector2(560f, 650f), Color.White);
			spritesBatch.DrawString(font1, intBGMVolume.ToString(), new Vector2(670f, 530f), Color.White);
			spritesBatch.DrawString(font1, intSEVolume.ToString(), new Vector2(670f, 570f), Color.White);
		}
		spritesBatch.End();
		base.Draw(gameTime);
	}
}
