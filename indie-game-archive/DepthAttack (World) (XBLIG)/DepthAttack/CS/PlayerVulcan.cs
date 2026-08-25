using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DepthAttack.CS;

public class PlayerVulcan : DrawableGameComponent
{
	public struct srcPlayerVulcanCore
	{
		public bool pflgEnable;

		public Vector3 pVecIti;

		public Vector3 pVecMovIti;

		public enuPlayerVulcanImgState[] penuImgState;
	}

	public enum enuPlayerVulcanImgState
	{
		intKieru,
		intNormal00,
		intNormal01,
		intNormal02
	}

	private const string cstrPlayerVulcan00 = "PNG\\Tama\\Tama08";

	private const string cstrPlayerVulcan01 = "PNG\\Tama\\Tama09";

	private const string cstrPlayerVulcan02 = "PNG\\Tama\\Tama10";

	public const float cfltOffSetHaba = 3f / 128f;

	public srcPlayerVulcanCore[] psrcPlayerVulcanCore = new srcPlayerVulcanCore[8];

	public Texture2D[] pimgPlayerVulcan = new Texture2D[4];

	public Rectangle[] precVulcanOffSet = new Rectangle[1]
	{
		new Rectangle(-69, -53, 118, 86)
	};

	public PlayerVulcan(Game game)
		: base(game)
	{
		for (int i = 0; i < psrcPlayerVulcanCore.Length; i++)
		{
			psrcPlayerVulcanCore[i].penuImgState = new enuPlayerVulcanImgState[30];
		}
	}

	public override void Initialize()
	{
		psrcPlayerVulcanCoreInit();
		base.Initialize();
	}

	public void psrcPlayerVulcanCoreInit()
	{
		for (int i = 0; i < psrcPlayerVulcanCore.Length; i++)
		{
			psrcPlayerVulcanCore[i].pflgEnable = false;
			psrcPlayerVulcanCore[i].pVecIti.X = 0f;
			psrcPlayerVulcanCore[i].pVecIti.Y = 0f;
			psrcPlayerVulcanCore[i].pVecIti.Z = 0f;
			psrcPlayerVulcanCore[i].pVecMovIti.X = 0f;
			psrcPlayerVulcanCore[i].pVecMovIti.Y = 0f;
			psrcPlayerVulcanCore[i].pVecMovIti.Z = 0f;
			NormalStateSet(i);
		}
	}

	public void psrcPlayerVulcanCoreEnable(Vector3 avec3Iti, Vector3 avec3MovIti)
	{
		for (int i = 0; i < psrcPlayerVulcanCore.Length; i++)
		{
			if (!psrcPlayerVulcanCore[i].pflgEnable)
			{
				psrcPlayerVulcanCore[i].pflgEnable = true;
				psrcPlayerVulcanCore[i].pVecIti.X = avec3Iti.X;
				psrcPlayerVulcanCore[i].pVecIti.Y = avec3Iti.Y;
				psrcPlayerVulcanCore[i].pVecIti.Z = avec3Iti.Z;
				psrcPlayerVulcanCore[i].pVecMovIti.X = avec3MovIti.X;
				psrcPlayerVulcanCore[i].pVecMovIti.Y = avec3MovIti.Y;
				psrcPlayerVulcanCore[i].pVecMovIti.Z = avec3MovIti.Z;
				NormalStartStateSet(i);
				break;
			}
		}
	}

	public void pPlayerVulcanUpdate()
	{
		for (int i = 0; i < psrcPlayerVulcanCore.Length; i++)
		{
			if (psrcPlayerVulcanCore[i].pflgEnable)
			{
				psrcPlayerVulcanCore[i].pVecIti.X += psrcPlayerVulcanCore[i].pVecMovIti.X;
				psrcPlayerVulcanCore[i].pVecIti.Y += psrcPlayerVulcanCore[i].pVecMovIti.Y;
				psrcPlayerVulcanCore[i].pVecIti.Z += psrcPlayerVulcanCore[i].pVecMovIti.Z;
				NormalStateSet(i);
			}
		}
		pImageStateUpdate();
	}

	public void pPlayerVulcanHantei()
	{
		PlayerVulcanHazureHantei();
		PlayerVulcanSyougaiHantei();
	}

	public void PlayerVulcanHazureHantei()
	{
		for (int i = 0; i < psrcPlayerVulcanCore.Length; i++)
		{
			if (psrcPlayerVulcanCore[i].pflgEnable && psrcPlayerVulcanCore[i].pVecIti.Z < 5f / 64f)
			{
				psrcPlayerVulcanCore[i].pflgEnable = false;
				psrcPlayerVulcanCore[i].pVecIti.X = 0f;
				psrcPlayerVulcanCore[i].pVecIti.Y = 0f;
				psrcPlayerVulcanCore[i].pVecIti.Z = 0f;
				psrcPlayerVulcanCore[i].pVecMovIti.X = 0f;
				psrcPlayerVulcanCore[i].pVecMovIti.Y = 0f;
				psrcPlayerVulcanCore[i].pVecMovIti.Z = 0f;
			}
		}
	}

	public void PlayerVulcanSyougaiHantei()
	{
		for (int i = 0; i < psrcPlayerVulcanCore.Length; i++)
		{
			if (psrcPlayerVulcanCore[i].pflgEnable)
			{
				if (Game1.cPU00.pCPU00TamaHantei(psrcPlayerVulcanCore[i].pVecIti, 3f / 128f, precVulcanOffSet[0]))
				{
					psrcPlayerVulcanCore[i].pflgEnable = false;
					psrcPlayerVulcanCore[i].pVecIti.X = 0f;
					psrcPlayerVulcanCore[i].pVecIti.Y = 0f;
					psrcPlayerVulcanCore[i].pVecIti.Z = 0f;
					psrcPlayerVulcanCore[i].pVecMovIti.X = 0f;
					psrcPlayerVulcanCore[i].pVecMovIti.Y = 0f;
					psrcPlayerVulcanCore[i].pVecMovIti.Z = 0f;
				}
				else if (Game1.syougai.pSyougaiTamaHantei(psrcPlayerVulcanCore[i].pVecIti, 3f / 128f, precVulcanOffSet[0]))
				{
					psrcPlayerVulcanCore[i].pflgEnable = false;
					psrcPlayerVulcanCore[i].pVecIti.X = 0f;
					psrcPlayerVulcanCore[i].pVecIti.Y = 0f;
					psrcPlayerVulcanCore[i].pVecIti.Z = 0f;
					psrcPlayerVulcanCore[i].pVecMovIti.X = 0f;
					psrcPlayerVulcanCore[i].pVecMovIti.Y = 0f;
					psrcPlayerVulcanCore[i].pVecMovIti.Z = 0f;
				}
				else if (Game1.cPUBOSS00.pCPUBOSS00TamaHantei(psrcPlayerVulcanCore[i].pVecIti, 3f / 128f, precVulcanOffSet[0], aflgVulcan: true))
				{
					psrcPlayerVulcanCore[i].pflgEnable = false;
					psrcPlayerVulcanCore[i].pVecIti.X = 0f;
					psrcPlayerVulcanCore[i].pVecIti.Y = 0f;
					psrcPlayerVulcanCore[i].pVecIti.Z = 0f;
					psrcPlayerVulcanCore[i].pVecMovIti.X = 0f;
					psrcPlayerVulcanCore[i].pVecMovIti.Y = 0f;
					psrcPlayerVulcanCore[i].pVecMovIti.Z = 0f;
				}
				else if (Game1.cPUBOSS00.pCPUBOSSChild00TamaHantei(psrcPlayerVulcanCore[i].pVecIti, 3f / 128f, precVulcanOffSet[0], aflgVulcan: true))
				{
					psrcPlayerVulcanCore[i].pflgEnable = false;
					psrcPlayerVulcanCore[i].pVecIti.X = 0f;
					psrcPlayerVulcanCore[i].pVecIti.Y = 0f;
					psrcPlayerVulcanCore[i].pVecIti.Z = 0f;
					psrcPlayerVulcanCore[i].pVecMovIti.X = 0f;
					psrcPlayerVulcanCore[i].pVecMovIti.Y = 0f;
					psrcPlayerVulcanCore[i].pVecMovIti.Z = 0f;
				}
			}
		}
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}

	protected override void LoadContent()
	{
		pimgPlayerVulcan[0] = null;
		pimgPlayerVulcan[1] = base.Game.Content.Load<Texture2D>("PNG\\Tama\\Tama08");
		pimgPlayerVulcan[2] = base.Game.Content.Load<Texture2D>("PNG\\Tama\\Tama09");
		pimgPlayerVulcan[3] = base.Game.Content.Load<Texture2D>("PNG\\Tama\\Tama10");
		base.LoadContent();
	}

	public void pImageStateUpdate()
	{
		for (int i = 0; i < psrcPlayerVulcanCore.Length; i++)
		{
			if (psrcPlayerVulcanCore[i].pflgEnable)
			{
				for (int j = 0; j < psrcPlayerVulcanCore[i].penuImgState.Length - 1; j++)
				{
					psrcPlayerVulcanCore[i].penuImgState[j] = psrcPlayerVulcanCore[i].penuImgState[j + 1];
				}
			}
		}
	}

	private void NormalStartStateSet(int aintVulcanNo)
	{
		for (int i = 0; i < psrcPlayerVulcanCore[aintVulcanNo].penuImgState.Length - 29; i += 29)
		{
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i] = enuPlayerVulcanImgState.intNormal00;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 1] = enuPlayerVulcanImgState.intNormal00;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 2] = enuPlayerVulcanImgState.intNormal00;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 3] = enuPlayerVulcanImgState.intNormal00;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 4] = enuPlayerVulcanImgState.intNormal00;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 5] = enuPlayerVulcanImgState.intNormal00;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 6] = enuPlayerVulcanImgState.intNormal00;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 7] = enuPlayerVulcanImgState.intNormal00;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 8] = enuPlayerVulcanImgState.intNormal00;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 9] = enuPlayerVulcanImgState.intNormal00;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 10] = enuPlayerVulcanImgState.intNormal01;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 11] = enuPlayerVulcanImgState.intNormal01;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 12] = enuPlayerVulcanImgState.intNormal01;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 13] = enuPlayerVulcanImgState.intNormal01;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 14] = enuPlayerVulcanImgState.intNormal01;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 15] = enuPlayerVulcanImgState.intNormal01;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 16] = enuPlayerVulcanImgState.intNormal01;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 17] = enuPlayerVulcanImgState.intNormal01;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 18] = enuPlayerVulcanImgState.intNormal01;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 19] = enuPlayerVulcanImgState.intNormal01;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 20] = enuPlayerVulcanImgState.intNormal02;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 21] = enuPlayerVulcanImgState.intNormal02;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 22] = enuPlayerVulcanImgState.intNormal02;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 23] = enuPlayerVulcanImgState.intNormal02;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 24] = enuPlayerVulcanImgState.intNormal02;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 25] = enuPlayerVulcanImgState.intNormal02;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 26] = enuPlayerVulcanImgState.intNormal02;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 27] = enuPlayerVulcanImgState.intNormal02;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 28] = enuPlayerVulcanImgState.intNormal02;
			psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 29] = enuPlayerVulcanImgState.intNormal02;
		}
	}

	private void NormalStateSet(int aintVulcanNo)
	{
		if (psrcPlayerVulcanCore[aintVulcanNo].penuImgState[0] == enuPlayerVulcanImgState.intKieru)
		{
			for (int i = 0; i < psrcPlayerVulcanCore[aintVulcanNo].penuImgState.Length - 29; i += 29)
			{
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i] = enuPlayerVulcanImgState.intNormal00;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 1] = enuPlayerVulcanImgState.intNormal00;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 2] = enuPlayerVulcanImgState.intNormal00;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 3] = enuPlayerVulcanImgState.intNormal00;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 4] = enuPlayerVulcanImgState.intNormal00;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 5] = enuPlayerVulcanImgState.intNormal00;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 6] = enuPlayerVulcanImgState.intNormal00;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 7] = enuPlayerVulcanImgState.intNormal00;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 8] = enuPlayerVulcanImgState.intNormal00;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 9] = enuPlayerVulcanImgState.intNormal00;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 10] = enuPlayerVulcanImgState.intNormal01;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 11] = enuPlayerVulcanImgState.intNormal01;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 12] = enuPlayerVulcanImgState.intNormal01;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 13] = enuPlayerVulcanImgState.intNormal01;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 14] = enuPlayerVulcanImgState.intNormal01;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 15] = enuPlayerVulcanImgState.intNormal01;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 16] = enuPlayerVulcanImgState.intNormal01;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 17] = enuPlayerVulcanImgState.intNormal01;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 18] = enuPlayerVulcanImgState.intNormal01;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 19] = enuPlayerVulcanImgState.intNormal01;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 20] = enuPlayerVulcanImgState.intNormal02;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 21] = enuPlayerVulcanImgState.intNormal02;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 22] = enuPlayerVulcanImgState.intNormal02;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 23] = enuPlayerVulcanImgState.intNormal02;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 24] = enuPlayerVulcanImgState.intNormal02;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 25] = enuPlayerVulcanImgState.intNormal02;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 26] = enuPlayerVulcanImgState.intNormal02;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 27] = enuPlayerVulcanImgState.intNormal02;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 28] = enuPlayerVulcanImgState.intNormal02;
				psrcPlayerVulcanCore[aintVulcanNo].penuImgState[i + 29] = enuPlayerVulcanImgState.intNormal02;
			}
		}
	}

	public void pPlayerVulcanDraw(SpriteBatch aspritesBatch)
	{
		if (pimgPlayerVulcan[1] == null)
		{
			return;
		}
		for (int i = 0; i < psrcPlayerVulcanCore.Length; i++)
		{
			if (psrcPlayerVulcanCore[i].pflgEnable && psrcPlayerVulcanCore[i].penuImgState[0] != enuPlayerVulcanImgState.intKieru)
			{
				int width = pimgPlayerVulcan[(int)psrcPlayerVulcanCore[i].penuImgState[0]].Width;
				int height = pimgPlayerVulcan[(int)psrcPlayerVulcanCore[i].penuImgState[0]].Height;
				aspritesBatch.Draw(pimgPlayerVulcan[(int)psrcPlayerVulcanCore[i].penuImgState[0]], new Vector2(psrcPlayerVulcanCore[i].pVecIti.X * psrcPlayerVulcanCore[i].pVecIti.Z + (640f + 0f * psrcPlayerVulcanCore[i].pVecIti.Z), psrcPlayerVulcanCore[i].pVecIti.Y * psrcPlayerVulcanCore[i].pVecIti.Z + (360f + 0f * psrcPlayerVulcanCore[i].pVecIti.Z)), null, new Color(200, 200, 200, 200), MathHelper.ToRadians(0f), new Vector2(width / 2, height / 2), new Vector2(psrcPlayerVulcanCore[i].pVecIti.Z, psrcPlayerVulcanCore[i].pVecIti.Z), SpriteEffects.None, psrcPlayerVulcanCore[i].pVecIti.Z);
			}
		}
	}

	public override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
	}
}
