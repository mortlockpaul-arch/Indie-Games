using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DepthAttack.CS;

public class PlayerTamaVulcan : DrawableGameComponent
{
	public struct srcPlayerTamaVulcanCore
	{
		public bool pflgEnable;

		public Vector3 pVecIti;

		public Vector3 pVecMovIti;

		public enuPlayerTamaVulcanImgState[] penuImgState;
	}

	public enum enuPlayerTamaVulcanImgState
	{
		intKieru,
		intNormal00,
		intNormal01,
		intNormal02
	}

	private const string cstrPlayerTamaVulcan00 = "PNG\\Tama\\Tama08";

	private const string cstrPlayerTamaVulcan01 = "PNG\\Tama\\Tama09";

	private const string cstrPlayerTamaVulcan02 = "PNG\\Tama\\Tama10";

	public const float cfltOffSetHaba = 3f / 128f;

	public srcPlayerTamaVulcanCore[] psrcPlayerTamaVulcanCore = new srcPlayerTamaVulcanCore[8];

	public Texture2D[] pimgPlayerTamaVulcan = new Texture2D[4];

	public Rectangle[] precVulcanOffSet = new Rectangle[1]
	{
		new Rectangle(-69, -53, 118, 86)
	};

	public PlayerTamaVulcan(Game game)
		: base(game)
	{
		for (int i = 0; i < psrcPlayerTamaVulcanCore.Length; i++)
		{
			psrcPlayerTamaVulcanCore[i].penuImgState = new enuPlayerTamaVulcanImgState[30];
		}
	}

	public override void Initialize()
	{
		psrcPlayerTamaVulcanCoreInit();
		base.Initialize();
	}

	public void psrcPlayerTamaVulcanCoreInit()
	{
		for (int i = 0; i < psrcPlayerTamaVulcanCore.Length; i++)
		{
			psrcPlayerTamaVulcanCore[i].pflgEnable = false;
			psrcPlayerTamaVulcanCore[i].pVecIti.X = 0f;
			psrcPlayerTamaVulcanCore[i].pVecIti.Y = 0f;
			psrcPlayerTamaVulcanCore[i].pVecIti.Z = 0f;
			psrcPlayerTamaVulcanCore[i].pVecMovIti.X = 0f;
			psrcPlayerTamaVulcanCore[i].pVecMovIti.Y = 0f;
			psrcPlayerTamaVulcanCore[i].pVecMovIti.Z = 0f;
			NomarlStateSet(i);
		}
	}

	public void psrcPlayerTamaVulcanCoreEnable(Vector3 avec3Iti, Vector3 avec3MovIti)
	{
		for (int i = 0; i < psrcPlayerTamaVulcanCore.Length; i++)
		{
			if (!psrcPlayerTamaVulcanCore[i].pflgEnable)
			{
				psrcPlayerTamaVulcanCore[i].pflgEnable = true;
				psrcPlayerTamaVulcanCore[i].pVecIti.X = avec3Iti.X;
				psrcPlayerTamaVulcanCore[i].pVecIti.Y = avec3Iti.Y;
				psrcPlayerTamaVulcanCore[i].pVecIti.Z = avec3Iti.Z;
				psrcPlayerTamaVulcanCore[i].pVecMovIti.X = avec3MovIti.X;
				psrcPlayerTamaVulcanCore[i].pVecMovIti.Y = avec3MovIti.Y;
				psrcPlayerTamaVulcanCore[i].pVecMovIti.Z = avec3MovIti.Z;
				NomarlStartStateSet(i);
				break;
			}
		}
	}

	public void pPlayerTamaVulcanUpdate()
	{
		for (int i = 0; i < psrcPlayerTamaVulcanCore.Length; i++)
		{
			if (psrcPlayerTamaVulcanCore[i].pflgEnable)
			{
				psrcPlayerTamaVulcanCore[i].pVecIti.X += psrcPlayerTamaVulcanCore[i].pVecMovIti.X;
				psrcPlayerTamaVulcanCore[i].pVecIti.Y += psrcPlayerTamaVulcanCore[i].pVecMovIti.Y;
				psrcPlayerTamaVulcanCore[i].pVecIti.Z += psrcPlayerTamaVulcanCore[i].pVecMovIti.Z;
				NomarlStateSet(i);
			}
		}
		pImageStateUpdate();
	}

	public void pPlayerTamaVulcanHantei()
	{
		PlayerTamaVulcanHazureHantei();
		PlayerTamaVulcanSyougaiHantei();
	}

	public void PlayerTamaVulcanHazureHantei()
	{
		for (int i = 0; i < psrcPlayerTamaVulcanCore.Length; i++)
		{
			if (psrcPlayerTamaVulcanCore[i].pflgEnable && psrcPlayerTamaVulcanCore[i].pVecIti.Z < 5f / 64f)
			{
				psrcPlayerTamaVulcanCore[i].pflgEnable = false;
				psrcPlayerTamaVulcanCore[i].pVecIti.X = 0f;
				psrcPlayerTamaVulcanCore[i].pVecIti.Y = 0f;
				psrcPlayerTamaVulcanCore[i].pVecIti.Z = 0f;
				psrcPlayerTamaVulcanCore[i].pVecMovIti.X = 0f;
				psrcPlayerTamaVulcanCore[i].pVecMovIti.Y = 0f;
				psrcPlayerTamaVulcanCore[i].pVecMovIti.Z = 0f;
			}
		}
	}

	public void PlayerTamaVulcanSyougaiHantei()
	{
		for (int i = 0; i < psrcPlayerTamaVulcanCore.Length; i++)
		{
			if (psrcPlayerTamaVulcanCore[i].pflgEnable)
			{
				if (Game1.cPU00.pCPU00TamaHantei(psrcPlayerTamaVulcanCore[i].pVecIti, 3f / 128f, precVulcanOffSet[0]))
				{
					psrcPlayerTamaVulcanCore[i].pflgEnable = false;
					psrcPlayerTamaVulcanCore[i].pVecIti.X = 0f;
					psrcPlayerTamaVulcanCore[i].pVecIti.Y = 0f;
					psrcPlayerTamaVulcanCore[i].pVecIti.Z = 0f;
					psrcPlayerTamaVulcanCore[i].pVecMovIti.X = 0f;
					psrcPlayerTamaVulcanCore[i].pVecMovIti.Y = 0f;
					psrcPlayerTamaVulcanCore[i].pVecMovIti.Z = 0f;
				}
				else if (Game1.syougai.pSyougaiTamaHantei(psrcPlayerTamaVulcanCore[i].pVecIti, 3f / 128f, precVulcanOffSet[0]))
				{
					psrcPlayerTamaVulcanCore[i].pflgEnable = false;
					psrcPlayerTamaVulcanCore[i].pVecIti.X = 0f;
					psrcPlayerTamaVulcanCore[i].pVecIti.Y = 0f;
					psrcPlayerTamaVulcanCore[i].pVecIti.Z = 0f;
					psrcPlayerTamaVulcanCore[i].pVecMovIti.X = 0f;
					psrcPlayerTamaVulcanCore[i].pVecMovIti.Y = 0f;
					psrcPlayerTamaVulcanCore[i].pVecMovIti.Z = 0f;
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
		pimgPlayerTamaVulcan[0] = null;
		pimgPlayerTamaVulcan[1] = base.Game.Content.Load<Texture2D>("PNG\\Tama\\Tama08");
		pimgPlayerTamaVulcan[2] = base.Game.Content.Load<Texture2D>("PNG\\Tama\\Tama09");
		pimgPlayerTamaVulcan[3] = base.Game.Content.Load<Texture2D>("PNG\\Tama\\Tama10");
		base.LoadContent();
	}

	public void pImageStateUpdate()
	{
		for (int i = 0; i < psrcPlayerTamaVulcanCore.Length; i++)
		{
			if (psrcPlayerTamaVulcanCore[i].pflgEnable)
			{
				for (int j = 0; j < psrcPlayerTamaVulcanCore[i].penuImgState.Length - 1; j++)
				{
					psrcPlayerTamaVulcanCore[i].penuImgState[j] = psrcPlayerTamaVulcanCore[i].penuImgState[j + 1];
				}
			}
		}
	}

	private void NomarlStartStateSet(int aintTamaVulcanNo)
	{
		for (int i = 0; i < psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState.Length - 29; i += 29)
		{
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i] = enuPlayerTamaVulcanImgState.intNormal00;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 1] = enuPlayerTamaVulcanImgState.intNormal00;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 2] = enuPlayerTamaVulcanImgState.intNormal00;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 3] = enuPlayerTamaVulcanImgState.intNormal00;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 4] = enuPlayerTamaVulcanImgState.intNormal00;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 5] = enuPlayerTamaVulcanImgState.intNormal00;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 6] = enuPlayerTamaVulcanImgState.intNormal00;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 7] = enuPlayerTamaVulcanImgState.intNormal00;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 8] = enuPlayerTamaVulcanImgState.intNormal00;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 9] = enuPlayerTamaVulcanImgState.intNormal00;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 10] = enuPlayerTamaVulcanImgState.intNormal01;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 11] = enuPlayerTamaVulcanImgState.intNormal01;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 12] = enuPlayerTamaVulcanImgState.intNormal01;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 13] = enuPlayerTamaVulcanImgState.intNormal01;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 14] = enuPlayerTamaVulcanImgState.intNormal01;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 15] = enuPlayerTamaVulcanImgState.intNormal01;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 16] = enuPlayerTamaVulcanImgState.intNormal01;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 17] = enuPlayerTamaVulcanImgState.intNormal01;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 18] = enuPlayerTamaVulcanImgState.intNormal01;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 19] = enuPlayerTamaVulcanImgState.intNormal01;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 20] = enuPlayerTamaVulcanImgState.intNormal02;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 21] = enuPlayerTamaVulcanImgState.intNormal02;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 22] = enuPlayerTamaVulcanImgState.intNormal02;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 23] = enuPlayerTamaVulcanImgState.intNormal02;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 24] = enuPlayerTamaVulcanImgState.intNormal02;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 25] = enuPlayerTamaVulcanImgState.intNormal02;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 26] = enuPlayerTamaVulcanImgState.intNormal02;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 27] = enuPlayerTamaVulcanImgState.intNormal02;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 28] = enuPlayerTamaVulcanImgState.intNormal02;
			psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 29] = enuPlayerTamaVulcanImgState.intNormal02;
		}
	}

	private void NomarlStateSet(int aintTamaVulcanNo)
	{
		if (psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[0] == enuPlayerTamaVulcanImgState.intKieru)
		{
			for (int i = 0; i < psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState.Length - 29; i += 29)
			{
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i] = enuPlayerTamaVulcanImgState.intNormal00;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 1] = enuPlayerTamaVulcanImgState.intNormal00;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 2] = enuPlayerTamaVulcanImgState.intNormal00;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 3] = enuPlayerTamaVulcanImgState.intNormal00;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 4] = enuPlayerTamaVulcanImgState.intNormal00;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 5] = enuPlayerTamaVulcanImgState.intNormal00;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 6] = enuPlayerTamaVulcanImgState.intNormal00;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 7] = enuPlayerTamaVulcanImgState.intNormal00;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 8] = enuPlayerTamaVulcanImgState.intNormal00;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 9] = enuPlayerTamaVulcanImgState.intNormal00;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 10] = enuPlayerTamaVulcanImgState.intNormal01;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 11] = enuPlayerTamaVulcanImgState.intNormal01;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 12] = enuPlayerTamaVulcanImgState.intNormal01;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 13] = enuPlayerTamaVulcanImgState.intNormal01;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 14] = enuPlayerTamaVulcanImgState.intNormal01;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 15] = enuPlayerTamaVulcanImgState.intNormal01;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 16] = enuPlayerTamaVulcanImgState.intNormal01;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 17] = enuPlayerTamaVulcanImgState.intNormal01;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 18] = enuPlayerTamaVulcanImgState.intNormal01;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 19] = enuPlayerTamaVulcanImgState.intNormal01;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 20] = enuPlayerTamaVulcanImgState.intNormal02;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 21] = enuPlayerTamaVulcanImgState.intNormal02;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 22] = enuPlayerTamaVulcanImgState.intNormal02;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 23] = enuPlayerTamaVulcanImgState.intNormal02;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 24] = enuPlayerTamaVulcanImgState.intNormal02;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 25] = enuPlayerTamaVulcanImgState.intNormal02;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 26] = enuPlayerTamaVulcanImgState.intNormal02;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 27] = enuPlayerTamaVulcanImgState.intNormal02;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 28] = enuPlayerTamaVulcanImgState.intNormal02;
				psrcPlayerTamaVulcanCore[aintTamaVulcanNo].penuImgState[i + 29] = enuPlayerTamaVulcanImgState.intNormal02;
			}
		}
	}

	public void pPlayerTamaVulcanDraw(SpriteBatch aspritesBatch)
	{
		if (pimgPlayerTamaVulcan[1] == null)
		{
			return;
		}
		for (int i = 0; i < psrcPlayerTamaVulcanCore.Length; i++)
		{
			if (psrcPlayerTamaVulcanCore[i].pflgEnable && psrcPlayerTamaVulcanCore[i].penuImgState[0] != enuPlayerTamaVulcanImgState.intKieru)
			{
				int width = pimgPlayerTamaVulcan[(int)psrcPlayerTamaVulcanCore[i].penuImgState[0]].Width;
				int height = pimgPlayerTamaVulcan[(int)psrcPlayerTamaVulcanCore[i].penuImgState[0]].Height;
				aspritesBatch.Draw(pimgPlayerTamaVulcan[(int)psrcPlayerTamaVulcanCore[i].penuImgState[0]], new Vector2(psrcPlayerTamaVulcanCore[i].pVecIti.X * psrcPlayerTamaVulcanCore[i].pVecIti.Z + (640f + 0f * psrcPlayerTamaVulcanCore[i].pVecIti.Z), psrcPlayerTamaVulcanCore[i].pVecIti.Y * psrcPlayerTamaVulcanCore[i].pVecIti.Z + (360f + 0f * psrcPlayerTamaVulcanCore[i].pVecIti.Z)), null, new Color(200, 200, 200, 200), MathHelper.ToRadians(0f), new Vector2(width / 2, height / 2), new Vector2(psrcPlayerTamaVulcanCore[i].pVecIti.Z, psrcPlayerTamaVulcanCore[i].pVecIti.Z), SpriteEffects.None, psrcPlayerTamaVulcanCore[i].pVecIti.Z);
			}
		}
	}

	public override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
	}
}
