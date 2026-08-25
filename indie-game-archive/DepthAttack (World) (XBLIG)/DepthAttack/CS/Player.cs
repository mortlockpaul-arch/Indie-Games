using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DepthAttack.CS;

public class Player : DrawableGameComponent
{
	public struct srcPlayerCore
	{
		public bool pflgLogin;

		public Vector3 pVecIti;

		public enuPlayerImgType penuPlayerImgType;

		public enuPlayerImgState[] pPlayerImgState;

		public SpriteEffects pSpriteEffects;

		public bool pflgClear;

		public int pintVulcan;

		public int pintVulcanWait;

		public int pintHp;
	}

	public struct srcGamePad
	{
		public GamePadState pgamePadState;

		public GamePadState pgamePadMaeState;

		public PlayerIndex pplayerIndex;
	}

	public enum enuPlayerImgState
	{
		intKieru,
		intNormal00,
		intNormal01,
		intNormal02,
		intNormal03,
		intNormalYoko00,
		intShot00,
		intDamage00,
		intDamage01
	}

	public enum enuPlayerImgType
	{
		intJK,
		intNurse
	}

	public enum enuPlayerMovState
	{
		intNormal,
		intDamage
	}

	public enum OperationTypeMovXY
	{
		None,
		Mov
	}

	public struct srcMovXY
	{
		public Vector2 pvecMov;
	}

	public enum OperationTypeMovShot
	{
		None,
		A,
		Ap,
		B,
		AB,
		X
	}

	private const int cintVulcanMax = 3;

	private const int cintVulcanWaitMax = 6;

	private const int cintHpMax = 1000;

	public const float cfltOffSetHaba = 5f / 128f;

	private const string cstrPlayer00_00 = "PNG\\Character\\Player\\Hito09";

	private const string cstrPlayer00_01 = "PNG\\Character\\Player\\Hito14";

	private const string cstrPlayer00_02 = "PNG\\Character\\Player\\Hito12";

	private const string cstrPlayer00_03 = "PNG\\Character\\Player\\Hito15";

	private const string cstrPlayer00_04 = "PNG\\Character\\Player\\Hito17";

	private const string cstrPlayer00_05 = "PNG\\Character\\Player\\Hito16";

	private const string cstrPlayer00_06 = "PNG\\Character\\Player\\Hito20";

	private const string cstrPlayer01_00 = "PNG\\Character\\Player\\HitoNurse01";

	private const string cstrPlayer01_01 = "PNG\\Character\\Player\\HitoNurse02";

	private const string cstrPlayer01_02 = "PNG\\Character\\Player\\HitoNurse01";

	private const string cstrPlayer01_03 = "PNG\\Character\\Player\\HitoNurse03";

	private const string cstrPlayer01_04 = "PNG\\Character\\Player\\HitoNurse06";

	private const string cstrPlayer01_05 = "PNG\\Character\\Player\\HitoNurse05";

	private const string cstrPlayer01_06 = "PNG\\Character\\Player\\HitoNurse08";

	private const string cstrLookOnSite00 = "PNG\\Character\\Player\\LookOn00";

	private const int cintMapXMax = 8;

	private const int cintMapYMax = 4;

	private const int cintMapXMin = -8;

	private const int cintMapYMin = -3;

	private const int cintMap1MasuWidth = 96;

	private const int cintMap1MasuHeight = 96;

	public srcPlayerCore psrcPlayerCore;

	public Rectangle[] precPlayerOffSet = new Rectangle[1]
	{
		new Rectangle(-8, -53, 16, 48)
	};

	public Texture2D[,] pimgPlayer = new Texture2D[2, 9];

	public Texture2D pimgLookOnSite;

	public bool pflgLookOnEnable = true;

	public srcGamePad psctGamePad;

	public enuPlayerMovState PlayerMovState;

	public OperationTypeMovXY TypeMovXY;

	public srcMovXY psctMovXY;

	public OperationTypeMovShot TypeMovShot;

	public Vector3 pvec3Scroll;

	private Vector2 vecMaeScroll;

	public Player(Game game)
		: base(game)
	{
		psrcPlayerCore.pPlayerImgState = new enuPlayerImgState[30];
	}

	public override void Initialize()
	{
		for (int i = 0; i < psrcPlayerCore.pPlayerImgState.Length; i++)
		{
			psrcPlayerCore.pPlayerImgState[i] = enuPlayerImgState.intKieru;
		}
		pvec3Scroll = new Vector3(0f, 0f, 0f);
		vecMaeScroll = new Vector2(0f, 0f);
		psrcPlayerCore.pVecIti.X = 0f;
		psrcPlayerCore.pVecIti.Y = 0f;
		psrcPlayerCore.pVecIti.Z = 1f;
		base.Initialize();
	}

	public void pPlayerLogin()
	{
		psrcPlayerCore.pVecIti.X = 0f;
		psrcPlayerCore.pVecIti.Y = 0f;
		psrcPlayerCore.pVecIti.Z = 1f;
		NomalStartStateSet();
		pflgLookOnEnable = true;
		psrcPlayerCore.pflgClear = false;
		if (Game1.titleContent.flgCommandSelectEnable)
		{
			psrcPlayerCore.penuPlayerImgType = enuPlayerImgType.intNurse;
		}
		else
		{
			psrcPlayerCore.penuPlayerImgType = enuPlayerImgType.intJK;
		}
		pPleyerHpMax();
	}

	public void pPleyerHpMax()
	{
		psrcPlayerCore.pintHp = 1000;
	}

	private void PlayerOperationUpDate()
	{
		psctGamePad.pgamePadState = GamePad.GetState(psctGamePad.pplayerIndex);
		TypeMovXY = OperationTypeMovXY.None;
		TypeMovShot = OperationTypeMovShot.None;
		psctMovXY.pvecMov.X = 0f;
		psctMovXY.pvecMov.Y = 0f;
		if (psctGamePad.pgamePadState.DPad.Up == ButtonState.Pressed && psctGamePad.pgamePadState.DPad.Right == ButtonState.Released && psctGamePad.pgamePadState.DPad.Left == ButtonState.Released)
		{
			TypeMovXY = OperationTypeMovXY.Mov;
			psctMovXY.pvecMov.Y = -12f;
		}
		if (psctGamePad.pgamePadState.DPad.Down == ButtonState.Pressed && psctGamePad.pgamePadState.DPad.Right == ButtonState.Released && psctGamePad.pgamePadState.DPad.Left == ButtonState.Released)
		{
			TypeMovXY = OperationTypeMovXY.Mov;
			psctMovXY.pvecMov.Y = 12f;
		}
		if (psctGamePad.pgamePadState.DPad.Right == ButtonState.Pressed && psctGamePad.pgamePadState.DPad.Up == ButtonState.Released && psctGamePad.pgamePadState.DPad.Down == ButtonState.Released)
		{
			TypeMovXY = OperationTypeMovXY.Mov;
			psctMovXY.pvecMov.X = 12f;
		}
		if (psctGamePad.pgamePadState.DPad.Left == ButtonState.Pressed && psctGamePad.pgamePadState.DPad.Up == ButtonState.Released && psctGamePad.pgamePadState.DPad.Down == ButtonState.Released)
		{
			TypeMovXY = OperationTypeMovXY.Mov;
			psctMovXY.pvecMov.X = -12f;
		}
		if (psctGamePad.pgamePadState.DPad.Up == ButtonState.Pressed && psctGamePad.pgamePadState.DPad.Right == ButtonState.Pressed)
		{
			TypeMovXY = OperationTypeMovXY.Mov;
			psctMovXY.pvecMov.Y = -6f;
			psctMovXY.pvecMov.X = 6f;
		}
		if (psctGamePad.pgamePadState.DPad.Down == ButtonState.Pressed && psctGamePad.pgamePadState.DPad.Right == ButtonState.Pressed)
		{
			TypeMovXY = OperationTypeMovXY.Mov;
			psctMovXY.pvecMov.Y = 6f;
			psctMovXY.pvecMov.X = 6f;
		}
		if (psctGamePad.pgamePadState.DPad.Down == ButtonState.Pressed && psctGamePad.pgamePadState.DPad.Left == ButtonState.Pressed)
		{
			TypeMovXY = OperationTypeMovXY.Mov;
			psctMovXY.pvecMov.Y = 6f;
			psctMovXY.pvecMov.X = -6f;
		}
		if (psctGamePad.pgamePadState.DPad.Up == ButtonState.Pressed && psctGamePad.pgamePadState.DPad.Left == ButtonState.Pressed)
		{
			TypeMovXY = OperationTypeMovXY.Mov;
			psctMovXY.pvecMov.Y = -6f;
			psctMovXY.pvecMov.X = -6f;
		}
		if (psctGamePad.pgamePadState.ThumbSticks.Left.X > 0.1f || psctGamePad.pgamePadState.ThumbSticks.Left.X < -0.1f || psctGamePad.pgamePadState.ThumbSticks.Left.Y > 0.1f || psctGamePad.pgamePadState.ThumbSticks.Left.Y < -0.1f)
		{
			TypeMovXY = OperationTypeMovXY.Mov;
			psctMovXY.pvecMov.X = (int)(psctGamePad.pgamePadState.ThumbSticks.Left.X * 12f);
			psctMovXY.pvecMov.Y = (int)(psctGamePad.pgamePadState.ThumbSticks.Left.Y * -12f);
		}
		if ((psctGamePad.pgamePadState.Buttons.A == ButtonState.Pressed && psctGamePad.pgamePadMaeState.Buttons.A == ButtonState.Released) || (psctGamePad.pgamePadState.Triggers.Right >= 0.5f && psctGamePad.pgamePadMaeState.Triggers.Right < 0.5f))
		{
			TypeMovShot = OperationTypeMovShot.A;
		}
		if ((psctGamePad.pgamePadState.Buttons.B == ButtonState.Pressed && psctGamePad.pgamePadMaeState.Buttons.B == ButtonState.Released) || (psctGamePad.pgamePadState.Triggers.Left >= 0.5f && psctGamePad.pgamePadMaeState.Triggers.Left < 0.5f))
		{
			TypeMovShot = OperationTypeMovShot.B;
		}
		if (psctGamePad.pgamePadState.Buttons.Y == ButtonState.Pressed && psctGamePad.pgamePadMaeState.Buttons.Y == ButtonState.Released)
		{
			if (!psrcPlayerCore.pflgClear)
			{
				psrcPlayerCore.pflgClear = true;
			}
			else
			{
				psrcPlayerCore.pflgClear = false;
			}
		}
		psctGamePad.pgamePadMaeState = psctGamePad.pgamePadState;
	}

	public void pPlayerUpDate()
	{
		pImageStateUpdate();
		PlayerOperationUpDate();
		if (TypeMovXY == OperationTypeMovXY.Mov)
		{
			if (psrcPlayerCore.penuPlayerImgType == enuPlayerImgType.intNurse)
			{
				psctMovXY.pvecMov.Y *= -1f;
			}
			psrcPlayerCore.pVecIti.X += psctMovXY.pvecMov.X / 1f;
			psrcPlayerCore.pVecIti.Y += psctMovXY.pvecMov.Y / 1f;
			NomalMoveStateSet();
			if (psctMovXY.pvecMov.X < -0.2f)
			{
				NomalYokoStartStateSet();
				psrcPlayerCore.pSpriteEffects = SpriteEffects.None;
			}
			else if (psctMovXY.pvecMov.X > 0.2f)
			{
				NomalYokoStartStateSet();
				psrcPlayerCore.pSpriteEffects = SpriteEffects.FlipHorizontally;
			}
		}
		if (TypeMovShot == OperationTypeMovShot.A)
		{
			psrcPlayerCore.pintVulcan = 3;
			psrcPlayerCore.pintVulcanWait = 6;
		}
		if (psrcPlayerCore.pintVulcan > 0)
		{
			if (psrcPlayerCore.pintVulcan == 3)
			{
				Game1.bGM.pflgSEPlayerTama[1] = true;
				Game1.playerVulcan.psrcPlayerVulcanCoreEnable(new Vector3(psrcPlayerCore.pVecIti.X, psrcPlayerCore.pVecIti.Y, 127f / 128f), new Vector3(0f, 0f, -1f / 32f));
				psrcPlayerCore.pintVulcan--;
				NomalShotStateSet();
			}
			if (psrcPlayerCore.pintVulcanWait <= 0)
			{
				Game1.bGM.pflgSEPlayerTama[1] = true;
				Game1.playerVulcan.psrcPlayerVulcanCoreEnable(new Vector3(psrcPlayerCore.pVecIti.X, psrcPlayerCore.pVecIti.Y, 127f / 128f), new Vector3(0f, 0f, -1f / 32f));
				psrcPlayerCore.pintVulcan--;
				psrcPlayerCore.pintVulcanWait = 6;
				NomalShotStateSet();
			}
			else
			{
				psrcPlayerCore.pintVulcanWait--;
			}
		}
		if (TypeMovShot == OperationTypeMovShot.B)
		{
			Game1.bGM.pflgSEPlayerTama[0] = true;
			Game1.playerHoming.psrcPlayerHomingCoreEnable(new Vector3(psrcPlayerCore.pVecIti.X, psrcPlayerCore.pVecIti.Y, 127f / 128f), new Vector3(0f, 0f, -3f / 128f));
			NomalShotStateSet();
		}
		PlayerHantei();
		NomalStateSet();
	}

	private void PlayerHpDown(int intDamage)
	{
		psrcPlayerCore.pintHp -= intDamage;
		if (psrcPlayerCore.pintHp <= 0)
		{
			psrcPlayerCore.pintHp = 0;
		}
	}

	public void pPlayerHpUp(int intRecover)
	{
		psrcPlayerCore.pintHp += intRecover;
		if (psrcPlayerCore.pintHp >= 1000)
		{
			psrcPlayerCore.pintHp = 1000;
		}
	}

	private void PlayerHantei()
	{
		PlayerDamageHantei();
		playerGamenHantei();
		Game1.item.pItemPlayerHantei(psrcPlayerCore.pVecIti, 5f / 128f, precPlayerOffSet[0]);
	}

	private void PlayerDamageHantei()
	{
		if (psrcPlayerCore.pPlayerImgState[0] != enuPlayerImgState.intDamage00 && psrcPlayerCore.pPlayerImgState[0] != enuPlayerImgState.intDamage01)
		{
			if (Game1.cPUTama.pCPUTamaAtariHantei(psrcPlayerCore.pVecIti, 5f / 128f, precPlayerOffSet[0]))
			{
				Game1.bGM.pflgSECancelStart[0] = true;
				pDamageStateSet();
				PlayerHpDown(100);
			}
			else if (Game1.syougai.pSyougaiPlayerHantei(psrcPlayerCore.pVecIti, 5f / 128f, precPlayerOffSet[0]))
			{
				Game1.bGM.pflgSECancelStart[0] = true;
				pDamageStateSet();
				PlayerHpDown(50);
			}
			else if (Game1.cPU00.pCPU00PlayerHantei(psrcPlayerCore.pVecIti, 5f / 128f, precPlayerOffSet[0]))
			{
				Game1.bGM.pflgSECancelStart[0] = true;
				pDamageStateSet();
				PlayerHpDown(150);
			}
		}
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}

	protected override void LoadContent()
	{
		pimgPlayer[0, 0] = null;
		pimgPlayer[0, 1] = base.Game.Content.Load<Texture2D>("PNG\\Character\\Player\\Hito09");
		pimgPlayer[0, 2] = base.Game.Content.Load<Texture2D>("PNG\\Character\\Player\\Hito14");
		pimgPlayer[0, 3] = base.Game.Content.Load<Texture2D>("PNG\\Character\\Player\\Hito12");
		pimgPlayer[0, 4] = base.Game.Content.Load<Texture2D>("PNG\\Character\\Player\\Hito15");
		pimgPlayer[0, 5] = base.Game.Content.Load<Texture2D>("PNG\\Character\\Player\\Hito17");
		pimgPlayer[0, 6] = base.Game.Content.Load<Texture2D>("PNG\\Character\\Player\\Hito16");
		pimgPlayer[0, 7] = base.Game.Content.Load<Texture2D>("PNG\\Character\\Player\\Hito20");
		pimgPlayer[0, 8] = null;
		pimgPlayer[1, 0] = null;
		pimgPlayer[1, 1] = base.Game.Content.Load<Texture2D>("PNG\\Character\\Player\\HitoNurse01");
		pimgPlayer[1, 2] = base.Game.Content.Load<Texture2D>("PNG\\Character\\Player\\HitoNurse02");
		pimgPlayer[1, 3] = base.Game.Content.Load<Texture2D>("PNG\\Character\\Player\\HitoNurse01");
		pimgPlayer[1, 4] = base.Game.Content.Load<Texture2D>("PNG\\Character\\Player\\HitoNurse03");
		pimgPlayer[1, 5] = base.Game.Content.Load<Texture2D>("PNG\\Character\\Player\\HitoNurse06");
		pimgPlayer[1, 6] = base.Game.Content.Load<Texture2D>("PNG\\Character\\Player\\HitoNurse05");
		pimgPlayer[1, 7] = base.Game.Content.Load<Texture2D>("PNG\\Character\\Player\\HitoNurse08");
		pimgPlayer[1, 8] = null;
		pimgLookOnSite = base.Game.Content.Load<Texture2D>("PNG\\Character\\Player\\LookOn00");
		base.LoadContent();
	}

	public void pImageStateUpdate()
	{
		for (int i = 0; i < psrcPlayerCore.pPlayerImgState.Length - 1; i++)
		{
			psrcPlayerCore.pPlayerImgState[i] = psrcPlayerCore.pPlayerImgState[i + 1];
		}
	}

	private void NomalStartStateSet()
	{
		psrcPlayerCore.pPlayerImgState[0] = enuPlayerImgState.intNormal01;
		psrcPlayerCore.pPlayerImgState[1] = enuPlayerImgState.intNormal01;
		psrcPlayerCore.pPlayerImgState[2] = enuPlayerImgState.intNormal01;
		psrcPlayerCore.pPlayerImgState[3] = enuPlayerImgState.intNormal01;
		psrcPlayerCore.pPlayerImgState[4] = enuPlayerImgState.intNormal00;
		psrcPlayerCore.pPlayerImgState[5] = enuPlayerImgState.intNormal00;
		psrcPlayerCore.pPlayerImgState[6] = enuPlayerImgState.intNormal00;
		psrcPlayerCore.pPlayerImgState[7] = enuPlayerImgState.intNormal00;
		psrcPlayerCore.pPlayerImgState[8] = enuPlayerImgState.intNormal03;
		psrcPlayerCore.pPlayerImgState[9] = enuPlayerImgState.intNormal03;
		psrcPlayerCore.pPlayerImgState[10] = enuPlayerImgState.intNormal03;
		psrcPlayerCore.pPlayerImgState[11] = enuPlayerImgState.intNormal03;
	}

	private void NomalYokoStartStateSet()
	{
		if (psrcPlayerCore.pPlayerImgState[0] != enuPlayerImgState.intDamage00 && psrcPlayerCore.pPlayerImgState[0] != enuPlayerImgState.intDamage01)
		{
			for (int i = 0; i < 2; i++)
			{
				psrcPlayerCore.pPlayerImgState[i] = enuPlayerImgState.intNormalYoko00;
			}
		}
	}

	private void NomalStateSet()
	{
		if (psrcPlayerCore.pPlayerImgState[0] == enuPlayerImgState.intKieru)
		{
			psrcPlayerCore.pPlayerImgState[0] = enuPlayerImgState.intNormal01;
			psrcPlayerCore.pPlayerImgState[1] = enuPlayerImgState.intNormal01;
			psrcPlayerCore.pPlayerImgState[2] = enuPlayerImgState.intNormal01;
			psrcPlayerCore.pPlayerImgState[3] = enuPlayerImgState.intNormal01;
			psrcPlayerCore.pPlayerImgState[4] = enuPlayerImgState.intNormal00;
			psrcPlayerCore.pPlayerImgState[5] = enuPlayerImgState.intNormal00;
			psrcPlayerCore.pPlayerImgState[6] = enuPlayerImgState.intNormal00;
			psrcPlayerCore.pPlayerImgState[7] = enuPlayerImgState.intNormal00;
			psrcPlayerCore.pPlayerImgState[8] = enuPlayerImgState.intNormal03;
			psrcPlayerCore.pPlayerImgState[9] = enuPlayerImgState.intNormal03;
			psrcPlayerCore.pPlayerImgState[10] = enuPlayerImgState.intNormal03;
			psrcPlayerCore.pPlayerImgState[11] = enuPlayerImgState.intNormal03;
		}
	}

	private void NomalMoveStateSet()
	{
		if (psrcPlayerCore.pPlayerImgState[0] == enuPlayerImgState.intKieru)
		{
			psrcPlayerCore.pPlayerImgState[0] = enuPlayerImgState.intNormal01;
			psrcPlayerCore.pPlayerImgState[1] = enuPlayerImgState.intNormal01;
			psrcPlayerCore.pPlayerImgState[2] = enuPlayerImgState.intNormal01;
			psrcPlayerCore.pPlayerImgState[3] = enuPlayerImgState.intNormal01;
			psrcPlayerCore.pPlayerImgState[4] = enuPlayerImgState.intNormal00;
			psrcPlayerCore.pPlayerImgState[5] = enuPlayerImgState.intNormal00;
			psrcPlayerCore.pPlayerImgState[6] = enuPlayerImgState.intNormal00;
			psrcPlayerCore.pPlayerImgState[7] = enuPlayerImgState.intNormal00;
			psrcPlayerCore.pPlayerImgState[8] = enuPlayerImgState.intNormal03;
			psrcPlayerCore.pPlayerImgState[9] = enuPlayerImgState.intNormal03;
			psrcPlayerCore.pPlayerImgState[10] = enuPlayerImgState.intNormal03;
			psrcPlayerCore.pPlayerImgState[11] = enuPlayerImgState.intNormal03;
		}
	}

	private void NomalShotStateSet()
	{
		psrcPlayerCore.pPlayerImgState[0] = enuPlayerImgState.intShot00;
		psrcPlayerCore.pPlayerImgState[1] = enuPlayerImgState.intShot00;
		psrcPlayerCore.pPlayerImgState[2] = enuPlayerImgState.intShot00;
		psrcPlayerCore.pPlayerImgState[3] = enuPlayerImgState.intShot00;
	}

	public void pDamageStateSet()
	{
		psrcPlayerCore.pPlayerImgState[0] = enuPlayerImgState.intDamage00;
		psrcPlayerCore.pPlayerImgState[1] = enuPlayerImgState.intDamage00;
		psrcPlayerCore.pPlayerImgState[2] = enuPlayerImgState.intDamage00;
		psrcPlayerCore.pPlayerImgState[3] = enuPlayerImgState.intDamage00;
		psrcPlayerCore.pPlayerImgState[4] = enuPlayerImgState.intDamage00;
		psrcPlayerCore.pPlayerImgState[5] = enuPlayerImgState.intDamage00;
		psrcPlayerCore.pPlayerImgState[6] = enuPlayerImgState.intDamage01;
		psrcPlayerCore.pPlayerImgState[7] = enuPlayerImgState.intDamage01;
		psrcPlayerCore.pPlayerImgState[8] = enuPlayerImgState.intDamage01;
		psrcPlayerCore.pPlayerImgState[9] = enuPlayerImgState.intDamage01;
		psrcPlayerCore.pPlayerImgState[10] = enuPlayerImgState.intDamage01;
		psrcPlayerCore.pPlayerImgState[11] = enuPlayerImgState.intDamage01;
		psrcPlayerCore.pPlayerImgState[12] = enuPlayerImgState.intDamage00;
		psrcPlayerCore.pPlayerImgState[13] = enuPlayerImgState.intDamage00;
		psrcPlayerCore.pPlayerImgState[14] = enuPlayerImgState.intDamage00;
		psrcPlayerCore.pPlayerImgState[15] = enuPlayerImgState.intDamage00;
		psrcPlayerCore.pPlayerImgState[16] = enuPlayerImgState.intDamage00;
		psrcPlayerCore.pPlayerImgState[17] = enuPlayerImgState.intDamage00;
		psrcPlayerCore.pPlayerImgState[18] = enuPlayerImgState.intDamage01;
		psrcPlayerCore.pPlayerImgState[19] = enuPlayerImgState.intDamage01;
		psrcPlayerCore.pPlayerImgState[20] = enuPlayerImgState.intDamage01;
		psrcPlayerCore.pPlayerImgState[21] = enuPlayerImgState.intDamage01;
		psrcPlayerCore.pPlayerImgState[22] = enuPlayerImgState.intDamage01;
		psrcPlayerCore.pPlayerImgState[23] = enuPlayerImgState.intDamage01;
		psrcPlayerCore.pPlayerImgState[24] = enuPlayerImgState.intDamage00;
		psrcPlayerCore.pPlayerImgState[25] = enuPlayerImgState.intDamage00;
		psrcPlayerCore.pPlayerImgState[26] = enuPlayerImgState.intDamage00;
		psrcPlayerCore.pPlayerImgState[27] = enuPlayerImgState.intDamage00;
		psrcPlayerCore.pPlayerImgState[28] = enuPlayerImgState.intDamage00;
	}

	public void pPlayerDraw(SpriteBatch aspritesBatch)
	{
		if (pimgPlayer[0, 1] != null)
		{
			pLookOnSiteDraw(aspritesBatch, psrcPlayerCore.pVecIti);
			if (psrcPlayerCore.pPlayerImgState[0] != enuPlayerImgState.intKieru && psrcPlayerCore.pPlayerImgState[0] != enuPlayerImgState.intDamage01)
			{
				Color color = ((!psrcPlayerCore.pflgClear) ? new Color(256, 256, 256, 256) : new Color(128, 128, 128, 128));
				int width = pimgPlayer[(int)psrcPlayerCore.penuPlayerImgType, (int)psrcPlayerCore.pPlayerImgState[0]].Width;
				int height = pimgPlayer[(int)psrcPlayerCore.penuPlayerImgType, (int)psrcPlayerCore.pPlayerImgState[0]].Height;
				aspritesBatch.Draw(pimgPlayer[(int)psrcPlayerCore.penuPlayerImgType, (int)psrcPlayerCore.pPlayerImgState[0]], new Vector2(psrcPlayerCore.pVecIti.X + 640f, psrcPlayerCore.pVecIti.Y + 360f), null, color, MathHelper.ToRadians(0f), new Vector2(width / 2, height / 2), new Vector2(1f, 1f), psrcPlayerCore.pSpriteEffects, 1f);
			}
		}
	}

	public void pLookOnSiteDraw(SpriteBatch aspritesBatch, Vector3 aVecIti)
	{
		if (pimgLookOnSite != null && pflgLookOnEnable)
		{
			int width = pimgLookOnSite.Width;
			int height = pimgLookOnSite.Height;
			float num = 0.8f;
			aspritesBatch.Draw(pimgLookOnSite, new Vector2(aVecIti.X * num + 640f, aVecIti.Y * num + 360f), null, Color.White, MathHelper.ToRadians(0f), new Vector2(width / 2, height / 2), new Vector2(num, num), psrcPlayerCore.pSpriteEffects, num);
			num = 0.35f;
			aspritesBatch.Draw(pimgLookOnSite, new Vector2(aVecIti.X * num + 640f, aVecIti.Y * num + 360f), null, Color.White, MathHelper.ToRadians(0f), new Vector2(width / 2, height / 2), new Vector2(num, num), psrcPlayerCore.pSpriteEffects, num);
			num = 0.1f;
			aspritesBatch.Draw(pimgLookOnSite, new Vector2(aVecIti.X * num + 640f, aVecIti.Y * num + 360f), null, Color.White, MathHelper.ToRadians(0f), new Vector2(width / 2, height / 2), new Vector2(num, num), psrcPlayerCore.pSpriteEffects, num);
		}
	}

	public override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
	}

	private void playerGamenHantei()
	{
		if (psrcPlayerCore.pVecIti.X < -768f)
		{
			psrcPlayerCore.pVecIti.X = -768f;
		}
		else if (psrcPlayerCore.pVecIti.X > 768f)
		{
			psrcPlayerCore.pVecIti.X = 768f;
		}
		if (psrcPlayerCore.pVecIti.Y < -288f)
		{
			psrcPlayerCore.pVecIti.Y = -288f;
		}
		else if (psrcPlayerCore.pVecIti.Y > 384f)
		{
			psrcPlayerCore.pVecIti.Y = 384f;
		}
	}

	public void pMapScrollMov(Vector2 vecIti)
	{
		if (vecIti.X - vecMaeScroll.X > 0f && vecIti.X + pvec3Scroll.X > 1120f)
		{
			pvec3Scroll.X -= vecIti.X - vecMaeScroll.X;
		}
		if (vecIti.X - vecMaeScroll.X < 0f && vecIti.X + pvec3Scroll.X < 160f)
		{
			pvec3Scroll.X -= vecIti.X - vecMaeScroll.X;
		}
		if (vecIti.Y + pvec3Scroll.Y > 580f && vecIti.Y - vecMaeScroll.Y > 0f)
		{
			pvec3Scroll.Y -= vecIti.Y - vecMaeScroll.Y;
		}
		if (vecIti.Y + pvec3Scroll.Y < 150f && vecIti.Y - vecMaeScroll.Y < 0f)
		{
			pvec3Scroll.Y -= vecIti.Y - vecMaeScroll.Y;
		}
		vecMaeScroll = vecIti;
	}
}
