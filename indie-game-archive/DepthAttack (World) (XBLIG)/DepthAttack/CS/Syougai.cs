using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DepthAttack.CS;

public class Syougai : DrawableGameComponent
{
	public struct srcSyougaiCore
	{
		public bool pflgEnable;

		public Vector3 pVecIti;

		public Vector3 pVecMovIti;

		public Vector3 pVecMovIti2;

		public int pintMovItiCount;

		public enuSyougaiImgState[] penuImgState;

		public enuSyougaiType penuType;
	}

	public enum enuSyougaiImgState
	{
		intKieru,
		intNormal
	}

	public enum enuSyougaiType
	{
		intHasira,
		intIwa,
		intKusa,
		intKumo00,
		intKumo01,
		intESPIwa00
	}

	private const string cstrSyougai00_00 = "PNG\\Syougai\\Hasira04";

	private const string cstrSyougai01_00 = "PNG\\Syougai\\Iwa02";

	private const string cstrSyougai02_00 = "PNG\\Syougai\\Kusa00";

	private const string cstrSyougai03_00 = "PNG\\Syougai\\Kumo04";

	private const string cstrSyougai04_00 = "PNG\\Syougai\\Kumo03";

	public srcSyougaiCore[] psrcSyougaiCore = new srcSyougaiCore[64];

	public Texture2D[,] pimgSyougai = new Texture2D[6, 2];

	public float[] fltOffSetHaba = new float[6]
	{
		3f / 128f,
		3f / 128f,
		3f / 128f,
		0f,
		0f,
		3f / 128f
	};

	public Rectangle[] precSyougaiOffSet = new Rectangle[6]
	{
		new Rectangle(-48, -176, 96, 352),
		new Rectangle(-98, -98, 196, 196),
		new Rectangle(-98, -98, 196, 196),
		new Rectangle(0, 0, 0, 0),
		new Rectangle(0, 0, 0, 0),
		new Rectangle(-98, -98, 196, 196)
	};

	private int intJikan = 480;

	public Syougai(Game game)
		: base(game)
	{
		for (int i = 0; i < psrcSyougaiCore.Length; i++)
		{
			psrcSyougaiCore[i].penuImgState = new enuSyougaiImgState[30];
		}
	}

	public void psrcSyougaiCoreInit()
	{
		for (int i = 0; i < psrcSyougaiCore.Length; i++)
		{
			psrcSyougaiCore[i].pflgEnable = false;
			psrcSyougaiCore[i].pVecIti.X = 0f;
			psrcSyougaiCore[i].pVecIti.Y = 0f;
			psrcSyougaiCore[i].pVecIti.Z = 0f;
			psrcSyougaiCore[i].pVecMovIti.X = 0f;
			psrcSyougaiCore[i].pVecMovIti.Y = 0f;
			psrcSyougaiCore[i].pVecMovIti.Z = 0f;
			psrcSyougaiCore[i].pVecMovIti2.X = 0f;
			psrcSyougaiCore[i].pVecMovIti2.Y = 0f;
			psrcSyougaiCore[i].pVecMovIti2.Z = 0f;
			psrcSyougaiCore[i].pintMovItiCount = 0;
			psrcSyougaiCore[i].penuType = enuSyougaiType.intKusa;
		}
	}

	public override void Initialize()
	{
		psrcSyougaiCoreInit();
		base.Initialize();
	}

	public void pSyoubaiEnablePort()
	{
		intJikan--;
		if (intJikan < 0)
		{
			intJikan = 480;
			pSyougaiEnable(new Vector3(100f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), enuSyougaiType.intHasira);
		}
		if (intJikan == 60)
		{
			pSyougaiEnable(new Vector3(200f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), enuSyougaiType.intHasira);
		}
		if (intJikan == 120)
		{
			pSyougaiEnable(new Vector3(-300f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), enuSyougaiType.intHasira);
		}
		if (intJikan == 300)
		{
			pSyougaiEnable(new Vector3(-300f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), enuSyougaiType.intIwa);
		}
		if (intJikan == 370)
		{
			pSyougaiEnable(new Vector3(300f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), enuSyougaiType.intIwa);
		}
		if (intJikan == 130)
		{
			pSyougaiEnable(new Vector3(-4000f, -800f, 0.8f), new Vector3(30f, 0f, 0f), enuSyougaiType.intKumo00);
		}
		if (intJikan == 250)
		{
			pSyougaiEnable(new Vector3(-4000f, -1100f, 0.2f), new Vector3(30f, 0f, 0f), enuSyougaiType.intKumo00);
		}
		if (intJikan == 310)
		{
			pSyougaiEnable(new Vector3(-4000f, -1100f, 0.3f), new Vector3(30f, 0f, 0f), enuSyougaiType.intKumo00);
		}
		if (intJikan == 430)
		{
			pSyougaiEnable(new Vector3(-4000f, -700f, 0.5f), new Vector3(30f, 0f, 0f), enuSyougaiType.intKumo00);
		}
		if (intJikan == 370)
		{
			pSyougaiEnable(new Vector3(-4000f, -800f, 0.3f), new Vector3(30f, 0f, 0f), enuSyougaiType.intKumo00);
		}
	}

	public void pKumoEnablePort()
	{
		intJikan--;
		if (intJikan < 0)
		{
			intJikan = 720;
		}
		if (intJikan == 130)
		{
			pSyougaiEnable(new Vector3(-4000f, -800f, 0.8f), new Vector3(30f, 0f, 0f), enuSyougaiType.intKumo00);
		}
		if (intJikan == 250)
		{
			pSyougaiEnable(new Vector3(-4000f, -1100f, 0.2f), new Vector3(30f, 0f, 0f), enuSyougaiType.intKumo00);
		}
		if (intJikan == 310)
		{
			pSyougaiEnable(new Vector3(-4000f, -1100f, 0.3f), new Vector3(30f, 0f, 0f), enuSyougaiType.intKumo00);
		}
		if (intJikan == 430)
		{
			pSyougaiEnable(new Vector3(-4000f, -700f, 0.5f), new Vector3(30f, 0f, 0f), enuSyougaiType.intKumo00);
		}
		if (intJikan == 370)
		{
			pSyougaiEnable(new Vector3(-4000f, -800f, 0.3f), new Vector3(30f, 0f, 0f), enuSyougaiType.intKumo00);
		}
	}

	public void pSyougaiEnable(Vector3 avec3Iti, Vector3 avec3MovIti, enuSyougaiType aenuSyougaiType)
	{
		for (int i = 0; i < psrcSyougaiCore.Length; i++)
		{
			if (!psrcSyougaiCore[i].pflgEnable)
			{
				psrcSyougaiCore[i].pflgEnable = true;
				psrcSyougaiCore[i].pVecIti.X = avec3Iti.X;
				psrcSyougaiCore[i].pVecIti.Y = avec3Iti.Y;
				psrcSyougaiCore[i].pVecIti.Z = avec3Iti.Z;
				psrcSyougaiCore[i].pVecMovIti.X = avec3MovIti.X;
				psrcSyougaiCore[i].pVecMovIti.Y = avec3MovIti.Y;
				psrcSyougaiCore[i].pVecMovIti.Z = avec3MovIti.Z;
				psrcSyougaiCore[i].penuType = aenuSyougaiType;
				NomarlStartStateSet(i);
				break;
			}
		}
	}

	public void pSyougaiESPHandEnable(Vector3 avec3Iti, Vector3 avec3MovIti, Vector3 avec3MovIti2, int aintMovItiCount, enuSyougaiType aenuSyougaiType)
	{
		for (int i = 0; i < psrcSyougaiCore.Length; i++)
		{
			if (!psrcSyougaiCore[i].pflgEnable)
			{
				psrcSyougaiCore[i].pflgEnable = true;
				psrcSyougaiCore[i].pVecIti.X = avec3Iti.X;
				psrcSyougaiCore[i].pVecIti.Y = avec3Iti.Y;
				psrcSyougaiCore[i].pVecIti.Z = avec3Iti.Z;
				psrcSyougaiCore[i].pVecMovIti.X = avec3MovIti.X;
				psrcSyougaiCore[i].pVecMovIti.Y = avec3MovIti.Y;
				psrcSyougaiCore[i].pVecMovIti.Z = avec3MovIti.Z;
				psrcSyougaiCore[i].pVecMovIti2.X = avec3MovIti2.X;
				psrcSyougaiCore[i].pVecMovIti2.Y = avec3MovIti2.Y;
				psrcSyougaiCore[i].pVecMovIti2.Z = avec3MovIti2.Z;
				psrcSyougaiCore[i].pintMovItiCount = aintMovItiCount;
				psrcSyougaiCore[i].penuType = aenuSyougaiType;
				NomarlStartStateSet(i);
				break;
			}
		}
	}

	private void SyougaiUpDate(int aintSyougaiNo)
	{
		if (psrcSyougaiCore[aintSyougaiNo].penuType == enuSyougaiType.intESPIwa00)
		{
			if (psrcSyougaiCore[aintSyougaiNo].pintMovItiCount > 0)
			{
				psrcSyougaiCore[aintSyougaiNo].pintMovItiCount--;
				psrcSyougaiCore[aintSyougaiNo].pVecIti.X += psrcSyougaiCore[aintSyougaiNo].pVecMovIti2.X;
				psrcSyougaiCore[aintSyougaiNo].pVecIti.Y += psrcSyougaiCore[aintSyougaiNo].pVecMovIti2.Y;
				psrcSyougaiCore[aintSyougaiNo].pVecIti.Z += psrcSyougaiCore[aintSyougaiNo].pVecMovIti2.Z;
			}
			else
			{
				psrcSyougaiCore[aintSyougaiNo].pVecIti.X += psrcSyougaiCore[aintSyougaiNo].pVecMovIti.X;
				psrcSyougaiCore[aintSyougaiNo].pVecIti.Y += psrcSyougaiCore[aintSyougaiNo].pVecMovIti.Y;
				psrcSyougaiCore[aintSyougaiNo].pVecIti.Z += psrcSyougaiCore[aintSyougaiNo].pVecMovIti.Z;
			}
		}
		else
		{
			psrcSyougaiCore[aintSyougaiNo].pVecIti.X += psrcSyougaiCore[aintSyougaiNo].pVecMovIti.X;
			psrcSyougaiCore[aintSyougaiNo].pVecIti.Y += psrcSyougaiCore[aintSyougaiNo].pVecMovIti.Y;
			psrcSyougaiCore[aintSyougaiNo].pVecIti.Z += psrcSyougaiCore[aintSyougaiNo].pVecMovIti.Z;
		}
		NomarlStateSet(aintSyougaiNo);
	}

	public void pSyougaiUpdate()
	{
		pSyougaiHantei();
		pKumoEnablePort();
		for (int i = 0; i < psrcSyougaiCore.Length; i++)
		{
			if (psrcSyougaiCore[i].pflgEnable)
			{
				SyougaiUpDate(i);
			}
		}
		pImageStateUpdate();
	}

	public void pSyougaiHantei()
	{
		SyougaiHazureHantei();
	}

	private void SyougaiHazureHantei()
	{
		for (int i = 0; i < psrcSyougaiCore.Length; i++)
		{
			if (psrcSyougaiCore[i].pflgEnable && ((double)psrcSyougaiCore[i].pVecIti.Z > 63.0 / 64.0 || psrcSyougaiCore[i].pVecIti.X < -5000f || psrcSyougaiCore[i].pVecIti.X > 5000f))
			{
				psrcSyougaiCore[i].pflgEnable = false;
				psrcSyougaiCore[i].pVecIti.X = 0f;
				psrcSyougaiCore[i].pVecIti.Y = 0f;
				psrcSyougaiCore[i].pVecIti.Z = 0f;
				psrcSyougaiCore[i].pVecMovIti.X = 0f;
				psrcSyougaiCore[i].pVecMovIti.Y = 0f;
				psrcSyougaiCore[i].pVecMovIti.Z = 0f;
			}
		}
	}

	public bool pSyougaiTamaHantei(Vector3 avecIti, float afltHabaHantei, Rectangle arecHantei)
	{
		bool result = false;
		Rectangle rectangle = new Rectangle
		{
			X = arecHantei.X + (int)avecIti.X,
			Y = arecHantei.Y + (int)avecIti.Y,
			Width = arecHantei.Width,
			Height = arecHantei.Height
		};
		for (int i = 0; i < psrcSyougaiCore.Length; i++)
		{
			if (psrcSyougaiCore[i].pflgEnable && ((avecIti.Z <= psrcSyougaiCore[i].pVecIti.Z && avecIti.Z + afltHabaHantei >= psrcSyougaiCore[i].pVecIti.Z) || (avecIti.Z <= psrcSyougaiCore[i].pVecIti.Z + fltOffSetHaba[(int)psrcSyougaiCore[i].penuType] && avecIti.Z + afltHabaHantei >= psrcSyougaiCore[i].pVecIti.Z + fltOffSetHaba[(int)psrcSyougaiCore[i].penuType])) && rectangle.Intersects(new Rectangle((int)psrcSyougaiCore[i].pVecIti.X + precSyougaiOffSet[(int)psrcSyougaiCore[i].penuType].X, (int)psrcSyougaiCore[i].pVecIti.Y + precSyougaiOffSet[(int)psrcSyougaiCore[i].penuType].Y, precSyougaiOffSet[(int)psrcSyougaiCore[i].penuType].Width, precSyougaiOffSet[(int)psrcSyougaiCore[i].penuType].Height)))
			{
				result = true;
				Game1.bGM.pflgSEBakuhatu[1] = true;
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(psrcSyougaiCore[i].pVecIti, 0f, 0);
				psrcSyougaiCore[i].pflgEnable = false;
				psrcSyougaiCore[i].pVecIti.X = 0f;
				psrcSyougaiCore[i].pVecIti.Y = 0f;
				psrcSyougaiCore[i].pVecIti.Z = 0f;
				psrcSyougaiCore[i].pVecMovIti.X = 0f;
				psrcSyougaiCore[i].pVecMovIti.Y = 0f;
				psrcSyougaiCore[i].pVecMovIti.Z = 0f;
				Game1.score.pScoreUp(1000L);
				return result;
			}
		}
		return result;
	}

	public bool pSyougaiPlayerHantei(Vector3 avecIti, float afltHabaHantei, Rectangle arecHantei)
	{
		bool result = false;
		Rectangle rectangle = new Rectangle
		{
			X = arecHantei.X + (int)avecIti.X,
			Y = arecHantei.Y + (int)avecIti.Y,
			Width = arecHantei.Width,
			Height = arecHantei.Height
		};
		for (int i = 0; i < psrcSyougaiCore.Length; i++)
		{
			if (psrcSyougaiCore[i].pflgEnable && ((avecIti.Z <= psrcSyougaiCore[i].pVecIti.Z && avecIti.Z + afltHabaHantei >= psrcSyougaiCore[i].pVecIti.Z) || (avecIti.Z <= psrcSyougaiCore[i].pVecIti.Z + fltOffSetHaba[(int)psrcSyougaiCore[i].penuType] && avecIti.Z + afltHabaHantei >= psrcSyougaiCore[i].pVecIti.Z + fltOffSetHaba[(int)psrcSyougaiCore[i].penuType])) && rectangle.Intersects(new Rectangle((int)psrcSyougaiCore[i].pVecIti.X + precSyougaiOffSet[(int)psrcSyougaiCore[i].penuType].X, (int)psrcSyougaiCore[i].pVecIti.Y + precSyougaiOffSet[(int)psrcSyougaiCore[i].penuType].Y, precSyougaiOffSet[(int)psrcSyougaiCore[i].penuType].Width, precSyougaiOffSet[(int)psrcSyougaiCore[i].penuType].Height)))
			{
				result = true;
				Game1.bakuhatu.psrcHitCoreConboEnable(psrcSyougaiCore[i].pVecIti, 0f, 0);
				psrcSyougaiCore[i].pflgEnable = false;
				psrcSyougaiCore[i].pVecIti.X = 0f;
				psrcSyougaiCore[i].pVecIti.Y = 0f;
				psrcSyougaiCore[i].pVecIti.Z = 0f;
				psrcSyougaiCore[i].pVecMovIti.X = 0f;
				psrcSyougaiCore[i].pVecMovIti.Y = 0f;
				psrcSyougaiCore[i].pVecMovIti.Z = 0f;
				return result;
			}
		}
		return result;
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}

	protected override void LoadContent()
	{
		pimgSyougai[0, 0] = null;
		pimgSyougai[0, 1] = base.Game.Content.Load<Texture2D>("PNG\\Syougai\\Hasira04");
		pimgSyougai[1, 0] = null;
		pimgSyougai[1, 1] = base.Game.Content.Load<Texture2D>("PNG\\Syougai\\Iwa02");
		pimgSyougai[2, 0] = null;
		pimgSyougai[2, 1] = base.Game.Content.Load<Texture2D>("PNG\\Syougai\\Kusa00");
		pimgSyougai[3, 0] = null;
		pimgSyougai[3, 1] = base.Game.Content.Load<Texture2D>("PNG\\Syougai\\Kumo04");
		pimgSyougai[4, 0] = null;
		pimgSyougai[4, 1] = base.Game.Content.Load<Texture2D>("PNG\\Syougai\\Kumo03");
		pimgSyougai[5, 0] = null;
		pimgSyougai[5, 1] = pimgSyougai[1, 1];
		base.LoadContent();
	}

	public void pImageStateUpdate()
	{
		for (int i = 0; i < psrcSyougaiCore.Length; i++)
		{
			if (psrcSyougaiCore[i].pflgEnable)
			{
				for (int j = 0; j < psrcSyougaiCore[i].penuImgState.Length - 1; j++)
				{
					psrcSyougaiCore[i].penuImgState[j] = psrcSyougaiCore[i].penuImgState[j + 1];
				}
			}
		}
	}

	private void NomarlStartStateSet(int aintSyougaiNo)
	{
		for (int i = 0; i < psrcSyougaiCore[aintSyougaiNo].penuImgState.Length; i++)
		{
			psrcSyougaiCore[aintSyougaiNo].penuImgState[i] = enuSyougaiImgState.intNormal;
		}
	}

	private void NomarlStateSet(int aintSyougaiNo)
	{
		if (psrcSyougaiCore[aintSyougaiNo].penuImgState[1] == enuSyougaiImgState.intKieru)
		{
			for (int i = 0; i < psrcSyougaiCore[aintSyougaiNo].penuImgState.Length; i++)
			{
				psrcSyougaiCore[aintSyougaiNo].penuImgState[i] = enuSyougaiImgState.intNormal;
			}
		}
	}

	public void pSyougaiDraw(SpriteBatch aspritesBatch)
	{
		if (pimgSyougai[0, 1] == null)
		{
			return;
		}
		for (int i = 0; i < psrcSyougaiCore.Length; i++)
		{
			if (psrcSyougaiCore[i].pflgEnable && psrcSyougaiCore[i].penuImgState[0] != enuSyougaiImgState.intKieru)
			{
				if (psrcSyougaiCore[i].penuType != enuSyougaiType.intKumo00 && psrcSyougaiCore[i].penuType != enuSyougaiType.intKumo01)
				{
					int width = pimgSyougai[(int)psrcSyougaiCore[i].penuType, (int)psrcSyougaiCore[i].penuImgState[0]].Width;
					int height = pimgSyougai[(int)psrcSyougaiCore[i].penuType, (int)psrcSyougaiCore[i].penuImgState[0]].Height;
					aspritesBatch.Draw(pimgSyougai[(int)psrcSyougaiCore[i].penuType, (int)psrcSyougaiCore[i].penuImgState[0]], new Vector2(psrcSyougaiCore[i].pVecIti.X * psrcSyougaiCore[i].pVecIti.Z + (640f + 0f * psrcSyougaiCore[i].pVecIti.Z), psrcSyougaiCore[i].pVecIti.Y * psrcSyougaiCore[i].pVecIti.Z + (360f + 0f * psrcSyougaiCore[i].pVecIti.Z)), null, Color.White, MathHelper.ToRadians(0f), new Vector2(width / 2, height / 2), new Vector2(psrcSyougaiCore[i].pVecIti.Z, psrcSyougaiCore[i].pVecIti.Z), SpriteEffects.None, psrcSyougaiCore[i].pVecIti.Z);
				}
				else
				{
					int width = pimgSyougai[(int)psrcSyougaiCore[i].penuType, (int)psrcSyougaiCore[i].penuImgState[0]].Width;
					int height = pimgSyougai[(int)psrcSyougaiCore[i].penuType, (int)psrcSyougaiCore[i].penuImgState[0]].Height;
					aspritesBatch.Draw(pimgSyougai[(int)psrcSyougaiCore[i].penuType, (int)psrcSyougaiCore[i].penuImgState[0]], new Vector2(psrcSyougaiCore[i].pVecIti.X * psrcSyougaiCore[i].pVecIti.Z + (640f + 0f * psrcSyougaiCore[i].pVecIti.Z), psrcSyougaiCore[i].pVecIti.Y * psrcSyougaiCore[i].pVecIti.Z + (360f + 0f * psrcSyougaiCore[i].pVecIti.Z)), null, new Color(96, 96, 96, 96), MathHelper.ToRadians(0f), new Vector2(width / 2, height / 2), new Vector2(psrcSyougaiCore[i].pVecIti.Z, psrcSyougaiCore[i].pVecIti.Z), SpriteEffects.None, psrcSyougaiCore[i].pVecIti.Z);
				}
			}
		}
	}

	public override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
	}
}
