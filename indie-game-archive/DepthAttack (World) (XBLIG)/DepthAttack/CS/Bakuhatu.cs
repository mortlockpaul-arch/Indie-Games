using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DepthAttack.CS;

public class Bakuhatu : DrawableGameComponent
{
	public struct srcBakuhatuCore
	{
		public bool pflgEnable;

		public Vector3 pVecIti;

		public Vector3 pVecMovIti;

		public float pfltItiR;

		public float fltScale;

		public enuBakuhatuImgState[] penuImgState;

		public enuBakuhatuType penuType;

		public int intSleepTime;

		public int intBlinkTime;
	}

	public enum enuBakuhatuImgState
	{
		intKieru,
		intBakuhatu02,
		intBakuhatu02_r,
		intBakuhatu03,
		intBakuhatu03_r,
		intBakuhatu04,
		intBakuhatu04_r,
		intBakuhatu05,
		intBakuhatu05_r,
		intKemuri04,
		intKemuri05,
		intKemuri06,
		intKemuri07,
		intHit00,
		intHit01,
		intKaminari00,
		intKaminari01,
		intESPHand00
	}

	public enum enuBakuhatuType
	{
		intBakuhatu00,
		intBakuhatu00_r,
		intBakuhatu01,
		intBakuhatu01_r,
		intKemuri00,
		intKemuri01,
		intKemuri02,
		intHit00,
		intHit01,
		intKaminari00,
		intKaminari01,
		intESPHand00
	}

	private const string cstrBakuhatu00 = "PNG\\Bakuhatu\\Bakuhatu00";

	private const string cstrBakuhatu00_r = "PNG\\Bakuhatu\\Bakuhatu00_r";

	private const string cstrBakuhatu01 = "PNG\\Bakuhatu\\Bakuhatu01";

	private const string cstrBakuhatu01_r = "PNG\\Bakuhatu\\Bakuhatu01_r";

	private const string cstrBakuhatu02 = "PNG\\Bakuhatu\\Bakuhatu02";

	private const string cstrBakuhatu02_r = "PNG\\Bakuhatu\\Bakuhatu02_r";

	private const string cstrBakuhatu03 = "PNG\\Bakuhatu\\Bakuhatu03";

	private const string cstrBakuhatu03_r = "PNG\\Bakuhatu\\Bakuhatu03_r";

	private const string cstrBakuhatu04 = "PNG\\Bakuhatu\\Bakuhatu04";

	private const string cstrBakuhatu04_r = "PNG\\Bakuhatu\\Bakuhatu04_r";

	private const string cstrBakuhatu05 = "PNG\\Bakuhatu\\Bakuhatu05";

	private const string cstrBakuhatu05_r = "PNG\\Bakuhatu\\Bakuhatu05_r";

	private const string cstrKemuri00 = "PNG\\Bakuhatu\\Kemuri00";

	private const string cstrKemuri01 = "PNG\\Bakuhatu\\Kemuri01";

	private const string cstrKemuri02 = "PNG\\Bakuhatu\\Kemuri02";

	private const string cstrKemuri03 = "PNG\\Bakuhatu\\Kemuri03";

	private const string cstrKemuri04 = "PNG\\Bakuhatu\\Kemuri04";

	private const string cstrKemuri05 = "PNG\\Bakuhatu\\Kemuri05";

	private const string cstrKemuri06 = "PNG\\Bakuhatu\\Kemuri08";

	private const string cstrKemuri07 = "PNG\\Bakuhatu\\Kemuri09";

	private const string cstrHit00 = "PNG\\Bakuhatu\\Hit04";

	private const string cstrHit01 = "PNG\\Bakuhatu\\Hit05";

	private const string CstrKaminari00 = "PNG\\Bakuhatu\\Kaminari01";

	private const string CstrKaminari01 = "PNG\\Bakuhatu\\Kaminari03";

	private const string CstrESPHand00 = "PNG\\Character\\CPUBOSS\\ESP_Te00";

	public srcBakuhatuCore[] psrcBakuhatuCore = new srcBakuhatuCore[256];

	public Texture2D[] pimgBakuhatu = new Texture2D[20];

	public Bakuhatu(Game game)
		: base(game)
	{
		for (int i = 0; i < psrcBakuhatuCore.Length; i++)
		{
			psrcBakuhatuCore[i].penuImgState = new enuBakuhatuImgState[30];
		}
	}

	public override void Initialize()
	{
		psrcBakuhatuCoreInit();
		base.Initialize();
	}

	public void psrcBakuhatuCoreInit()
	{
		for (int i = 0; i < psrcBakuhatuCore.Length; i++)
		{
			psrcBakuhatuCore[i].pflgEnable = false;
			psrcBakuhatuCore[i].pVecIti.X = 0f;
			psrcBakuhatuCore[i].pVecIti.Y = 0f;
			psrcBakuhatuCore[i].pVecIti.Z = 0f;
			psrcBakuhatuCore[i].pVecMovIti.X = 0f;
			psrcBakuhatuCore[i].pVecMovIti.Y = 0f;
			psrcBakuhatuCore[i].pVecMovIti.Z = 0f;
			psrcBakuhatuCore[i].pfltItiR = 0f;
			psrcBakuhatuCore[i].fltScale = 0f;
			psrcBakuhatuCore[i].penuType = enuBakuhatuType.intBakuhatu00;
			psrcBakuhatuCore[i].intSleepTime = 0;
			psrcBakuhatuCore[i].intBlinkTime = 0;
			NomarlStateSet(i);
		}
	}

	public int psrcBakuhatuCoreEnable(Vector3 avecIti, float afltItiR, enuBakuhatuType aenuBakuhatuType, int aintSleepTime)
	{
		int result = psrcBakuhatuCore.Length;
		for (int i = 0; i < psrcBakuhatuCore.Length; i++)
		{
			if (!psrcBakuhatuCore[i].pflgEnable)
			{
				result = i;
				psrcBakuhatuCore[i].pflgEnable = true;
				psrcBakuhatuCore[i].pVecIti.X = avecIti.X;
				psrcBakuhatuCore[i].pVecIti.Y = avecIti.Y;
				psrcBakuhatuCore[i].pVecIti.Z = avecIti.Z;
				psrcBakuhatuCore[i].pVecMovIti.X = 0f;
				psrcBakuhatuCore[i].pVecMovIti.Y = 0f;
				psrcBakuhatuCore[i].pVecMovIti.Z = 0f;
				psrcBakuhatuCore[i].intBlinkTime = 0;
				psrcBakuhatuCore[i].pfltItiR = afltItiR;
				psrcBakuhatuCore[i].intSleepTime = aintSleepTime;
				psrcBakuhatuCore[i].penuType = aenuBakuhatuType;
				switch (aenuBakuhatuType)
				{
				case enuBakuhatuType.intBakuhatu00:
					Bakuhatu00StateSet(i);
					psrcBakuhatuCore[i].fltScale = 1f / 128f;
					break;
				case enuBakuhatuType.intBakuhatu00_r:
					Bakuhatu00_redStateSet(i);
					psrcBakuhatuCore[i].fltScale = 1f / 128f;
					break;
				case enuBakuhatuType.intBakuhatu01:
					Bakuhatu01StateSet(i);
					psrcBakuhatuCore[i].fltScale = 1f / 128f;
					break;
				case enuBakuhatuType.intBakuhatu01_r:
					Bakuhatu01_redStateSet(i);
					psrcBakuhatuCore[i].fltScale = 1f / 128f;
					break;
				case enuBakuhatuType.intKemuri00:
					Kemuri00StateSet(i);
					psrcBakuhatuCore[i].fltScale = 1f / 128f;
					break;
				case enuBakuhatuType.intKemuri01:
					Kemuri01StateSet(i);
					psrcBakuhatuCore[i].fltScale = 1.6f;
					break;
				case enuBakuhatuType.intKemuri02:
					Kemuri02StateSet(i);
					psrcBakuhatuCore[i].fltScale = 1.6f;
					break;
				case enuBakuhatuType.intHit00:
					Hit00StateSet(i);
					psrcBakuhatuCore[i].fltScale = 1.9f;
					break;
				case enuBakuhatuType.intHit01:
					Hit01StateSet(i);
					psrcBakuhatuCore[i].fltScale = 1.9f;
					break;
				case enuBakuhatuType.intKaminari00:
					Kaminari00StateSet(i);
					psrcBakuhatuCore[i].fltScale = 0.5f;
					psrcBakuhatuCore[i].intBlinkTime = 30;
					break;
				case enuBakuhatuType.intKaminari01:
					Kaminari01StateSet(i);
					psrcBakuhatuCore[i].fltScale = 0.8f;
					psrcBakuhatuCore[i].intBlinkTime = 30;
					break;
				case enuBakuhatuType.intESPHand00:
					ESPHand00StateSet(i);
					psrcBakuhatuCore[i].fltScale = 1f;
					psrcBakuhatuCore[i].intBlinkTime = 20;
					break;
				}
				break;
			}
		}
		return result;
	}

	public void psrcBakuhatuMovCoreEnable(Vector3 avecIti, Vector3 avecMovIti, float afltItiR, enuBakuhatuType aenuBakuhatuType, int aintSleepTime)
	{
		int num = psrcBakuhatuCore.Length;
		num = psrcBakuhatuCoreEnable(avecIti, afltItiR, aenuBakuhatuType, aintSleepTime);
		if (num != psrcBakuhatuCore.Length)
		{
			psrcBakuhatuCore[num].pVecMovIti.X = avecMovIti.X;
			psrcBakuhatuCore[num].pVecMovIti.Y = avecMovIti.Y;
			psrcBakuhatuCore[num].pVecMovIti.Z = avecMovIti.Z;
		}
	}

	public void psrcBakuhatu00CoreConboEnable(Vector3 avecIti, float afltItiR, int aintSleepTimeOffset)
	{
		psrcBakuhatuCoreEnable(new Vector3(avecIti.X, avecIti.Y, avecIti.Z), afltItiR, enuBakuhatuType.intBakuhatu00, aintSleepTimeOffset);
		psrcBakuhatuCoreEnable(new Vector3(avecIti.X + 50f, avecIti.Y + 50f, avecIti.Z), afltItiR, enuBakuhatuType.intBakuhatu00_r, aintSleepTimeOffset);
		psrcBakuhatuCoreEnable(new Vector3(avecIti.X, avecIti.Y + 100f, avecIti.Z - 1f / 128f), afltItiR, enuBakuhatuType.intKemuri00, aintSleepTimeOffset);
		psrcBakuhatuCoreEnable(new Vector3(avecIti.X + 100f, avecIti.Y + 50f, avecIti.Z + 1f / 64f), afltItiR, enuBakuhatuType.intKemuri00, aintSleepTimeOffset);
		psrcBakuhatuCoreEnable(new Vector3(avecIti.X + 20f, avecIti.Y + 130f, avecIti.Z - 1f / 128f), afltItiR, enuBakuhatuType.intKemuri00, 13 + aintSleepTimeOffset);
		psrcBakuhatuCoreEnable(new Vector3(avecIti.X + 130f, avecIti.Y + 50f, avecIti.Z + 1f / 64f), afltItiR, enuBakuhatuType.intKemuri00, 16 + aintSleepTimeOffset);
	}

	public void psrcBakuhatu02CoreConboEnable(Vector3 avecIti, float afltItiR, int aintSleepTimeOffset)
	{
		psrcBakuhatuCoreEnable(new Vector3(avecIti.X, avecIti.Y, avecIti.Z - 1f / 128f), afltItiR, enuBakuhatuType.intKemuri00, aintSleepTimeOffset);
	}

	public void psrcBakuhatuMov00CoreConboEnable(Vector3 avecIti, Vector3 avecMovIti, float afltItiR, int aintSleepTimeOffset)
	{
		psrcBakuhatuMovCoreEnable(new Vector3(avecIti.X, avecIti.Y, avecIti.Z), avecMovIti, afltItiR, enuBakuhatuType.intBakuhatu00, aintSleepTimeOffset);
		psrcBakuhatuMovCoreEnable(new Vector3(avecIti.X + 50f, avecIti.Y + 50f, avecIti.Z), avecMovIti, afltItiR, enuBakuhatuType.intBakuhatu00_r, aintSleepTimeOffset);
		psrcBakuhatuMovCoreEnable(new Vector3(avecIti.X, avecIti.Y + 100f, avecIti.Z - 1f / 128f), avecMovIti, afltItiR, enuBakuhatuType.intKemuri00, aintSleepTimeOffset);
		psrcBakuhatuMovCoreEnable(new Vector3(avecIti.X + 100f, avecIti.Y + 50f, avecIti.Z + 1f / 64f), avecMovIti, afltItiR, enuBakuhatuType.intKemuri00, aintSleepTimeOffset);
		psrcBakuhatuMovCoreEnable(new Vector3(avecIti.X + 20f, avecIti.Y + 130f, avecIti.Z - 1f / 128f), avecMovIti, afltItiR, enuBakuhatuType.intKemuri00, 13 + aintSleepTimeOffset);
		psrcBakuhatuMovCoreEnable(new Vector3(avecIti.X + 130f, avecIti.Y + 50f, avecIti.Z + 1f / 64f), avecMovIti, afltItiR, enuBakuhatuType.intKemuri00, 16 + aintSleepTimeOffset);
	}

	public void psrcBakuhatu01CoreConboEnable(Vector3 avecIti, float afltItiR, int aintSleepTimeOffset)
	{
		Game1.bGM.pflgSEBakuhatu[0] = true;
		psrcBakuhatuCoreEnable(new Vector3(avecIti.X, avecIti.Y, avecIti.Z), afltItiR, enuBakuhatuType.intBakuhatu01, aintSleepTimeOffset);
		psrcBakuhatuCoreEnable(new Vector3(avecIti.X, avecIti.Y, avecIti.Z), afltItiR, enuBakuhatuType.intBakuhatu01_r, aintSleepTimeOffset);
		psrcBakuhatuCoreEnable(new Vector3(avecIti.X, avecIti.Y + 100f, avecIti.Z - 1f / 128f), afltItiR, enuBakuhatuType.intKemuri00, aintSleepTimeOffset);
		psrcBakuhatuCoreEnable(new Vector3(avecIti.X + 100f, avecIti.Y + 50f, avecIti.Z + 1f / 64f), afltItiR, enuBakuhatuType.intKemuri00, aintSleepTimeOffset);
	}

	public void psrcBakuhatuMov01CoreConboEnable(Vector3 avecIti, Vector3 avecMovIti, float afltItiR, int aintSleepTimeOffset)
	{
		Game1.bGM.pflgSEBakuhatu[0] = true;
		psrcBakuhatuMovCoreEnable(new Vector3(avecIti.X, avecIti.Y, avecIti.Z), avecMovIti, afltItiR, enuBakuhatuType.intBakuhatu01, aintSleepTimeOffset);
		psrcBakuhatuMovCoreEnable(new Vector3(avecIti.X, avecIti.Y, avecIti.Z), avecMovIti, afltItiR, enuBakuhatuType.intBakuhatu01_r, aintSleepTimeOffset);
		psrcBakuhatuMovCoreEnable(new Vector3(avecIti.X, avecIti.Y + 100f, avecIti.Z - 1f / 128f), avecMovIti, afltItiR, enuBakuhatuType.intKemuri00, aintSleepTimeOffset);
		psrcBakuhatuMovCoreEnable(new Vector3(avecIti.X + 100f, avecIti.Y + 50f, avecIti.Z + 1f / 64f), avecMovIti, afltItiR, enuBakuhatuType.intKemuri00, aintSleepTimeOffset);
	}

	public void psrcKemuriCoreConboEnable(Vector3 avecIti, float afltItiR, int aintSleepTimeOffset)
	{
		psrcBakuhatuCoreEnable(new Vector3(avecIti.X, avecIti.Y, avecIti.Z + 0.0625f), afltItiR, enuBakuhatuType.intKemuri02, aintSleepTimeOffset);
		psrcBakuhatuCoreEnable(new Vector3(avecIti.X, avecIti.Y, avecIti.Z + 3f / 64f), afltItiR, enuBakuhatuType.intKemuri02, aintSleepTimeOffset);
		psrcBakuhatuCoreEnable(new Vector3(avecIti.X, avecIti.Y, avecIti.Z + 0.0625f), afltItiR, enuBakuhatuType.intKemuri02, aintSleepTimeOffset + 2);
		psrcBakuhatuCoreEnable(new Vector3(avecIti.X, avecIti.Y, avecIti.Z + 3f / 64f), afltItiR, enuBakuhatuType.intKemuri02, aintSleepTimeOffset + 2);
	}

	public void psrcHitCoreConboEnable(Vector3 avecIti, float afltItiR, int aintSleepTimeOffset)
	{
		psrcBakuhatuCoreEnable(new Vector3(avecIti.X, avecIti.Y, avecIti.Z), afltItiR, enuBakuhatuType.intHit00, aintSleepTimeOffset);
		psrcBakuhatuCoreEnable(new Vector3(avecIti.X + 15f, avecIti.Y, avecIti.Z), afltItiR, enuBakuhatuType.intHit01, 2 + aintSleepTimeOffset);
		psrcBakuhatuCoreEnable(new Vector3(avecIti.X + 15f, avecIti.Y + 15f, avecIti.Z), afltItiR, enuBakuhatuType.intHit00, 5 + aintSleepTimeOffset);
	}

	private void BakuhatuUpdate()
	{
		for (int i = 0; i < psrcBakuhatuCore.Length; i++)
		{
			if (!psrcBakuhatuCore[i].pflgEnable)
			{
				continue;
			}
			psrcBakuhatuCore[i].pVecIti.X += psrcBakuhatuCore[i].pVecMovIti.X;
			psrcBakuhatuCore[i].pVecIti.Y += psrcBakuhatuCore[i].pVecMovIti.Y;
			psrcBakuhatuCore[i].pVecIti.Z += psrcBakuhatuCore[i].pVecMovIti.Z;
			if (psrcBakuhatuCore[i].intSleepTime > 0)
			{
				psrcBakuhatuCore[i].intSleepTime--;
				continue;
			}
			switch (psrcBakuhatuCore[i].penuType)
			{
			case enuBakuhatuType.intBakuhatu00:
				psrcBakuhatuCore[i].fltScale = psrcBakuhatuCore[i].fltScale * 1.5f;
				break;
			case enuBakuhatuType.intBakuhatu00_r:
				psrcBakuhatuCore[i].fltScale = psrcBakuhatuCore[i].fltScale * 1.5f;
				break;
			case enuBakuhatuType.intBakuhatu01:
				psrcBakuhatuCore[i].fltScale = psrcBakuhatuCore[i].fltScale * 1.5f;
				break;
			case enuBakuhatuType.intBakuhatu01_r:
				psrcBakuhatuCore[i].fltScale = psrcBakuhatuCore[i].fltScale * 1.5f;
				break;
			case enuBakuhatuType.intKemuri00:
				psrcBakuhatuCore[i].fltScale = psrcBakuhatuCore[i].fltScale * 1.5f;
				break;
			case enuBakuhatuType.intKemuri01:
				psrcBakuhatuCore[i].fltScale = psrcBakuhatuCore[i].fltScale / 1.25f;
				break;
			case enuBakuhatuType.intKemuri02:
				psrcBakuhatuCore[i].fltScale = psrcBakuhatuCore[i].fltScale / 1.25f;
				break;
			case enuBakuhatuType.intHit00:
				psrcBakuhatuCore[i].fltScale = psrcBakuhatuCore[i].fltScale / 1.25f;
				break;
			case enuBakuhatuType.intHit01:
				psrcBakuhatuCore[i].fltScale = psrcBakuhatuCore[i].fltScale / 1.25f;
				break;
			case enuBakuhatuType.intKaminari00:
				psrcBakuhatuCore[i].intBlinkTime--;
				break;
			case enuBakuhatuType.intKaminari01:
				psrcBakuhatuCore[i].intBlinkTime--;
				break;
			case enuBakuhatuType.intESPHand00:
				psrcBakuhatuCore[i].intBlinkTime--;
				break;
			}
		}
	}

	private void BakuhatuHantei()
	{
		for (int i = 0; i < psrcBakuhatuCore.Length; i++)
		{
			if (psrcBakuhatuCore[i].pflgEnable && (psrcBakuhatuCore[i].fltScale >= 2f || psrcBakuhatuCore[i].fltScale <= 6.1035156E-05f || psrcBakuhatuCore[i].intBlinkTime < 0))
			{
				psrcBakuhatuCore[i].pflgEnable = false;
				psrcBakuhatuCore[i].pVecIti.X = 0f;
				psrcBakuhatuCore[i].pVecIti.Y = 0f;
				psrcBakuhatuCore[i].pVecIti.Z = 0f;
				psrcBakuhatuCore[i].fltScale = 0f;
				psrcBakuhatuCore[i].penuType = enuBakuhatuType.intBakuhatu00;
				psrcBakuhatuCore[i].intBlinkTime = 0;
				NomarlStateSet(i);
			}
		}
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}

	public void pBakuhatuUpdate()
	{
		BakuhatuUpdate();
		BakuhatuHantei();
		ImageStateUpdate();
	}

	protected override void LoadContent()
	{
		pimgBakuhatu[0] = null;
		pimgBakuhatu[1] = base.Game.Content.Load<Texture2D>("PNG\\Bakuhatu\\Bakuhatu02");
		pimgBakuhatu[2] = base.Game.Content.Load<Texture2D>("PNG\\Bakuhatu\\Bakuhatu02_r");
		pimgBakuhatu[3] = base.Game.Content.Load<Texture2D>("PNG\\Bakuhatu\\Bakuhatu03");
		pimgBakuhatu[4] = base.Game.Content.Load<Texture2D>("PNG\\Bakuhatu\\Bakuhatu03_r");
		pimgBakuhatu[5] = base.Game.Content.Load<Texture2D>("PNG\\Bakuhatu\\Bakuhatu04");
		pimgBakuhatu[6] = base.Game.Content.Load<Texture2D>("PNG\\Bakuhatu\\Bakuhatu04_r");
		pimgBakuhatu[7] = base.Game.Content.Load<Texture2D>("PNG\\Bakuhatu\\Bakuhatu05");
		pimgBakuhatu[8] = base.Game.Content.Load<Texture2D>("PNG\\Bakuhatu\\Bakuhatu05_r");
		pimgBakuhatu[9] = base.Game.Content.Load<Texture2D>("PNG\\Bakuhatu\\Kemuri04");
		pimgBakuhatu[10] = base.Game.Content.Load<Texture2D>("PNG\\Bakuhatu\\Kemuri05");
		pimgBakuhatu[11] = base.Game.Content.Load<Texture2D>("PNG\\Bakuhatu\\Kemuri08");
		pimgBakuhatu[12] = base.Game.Content.Load<Texture2D>("PNG\\Bakuhatu\\Kemuri09");
		pimgBakuhatu[13] = base.Game.Content.Load<Texture2D>("PNG\\Bakuhatu\\Hit04");
		pimgBakuhatu[14] = base.Game.Content.Load<Texture2D>("PNG\\Bakuhatu\\Hit05");
		pimgBakuhatu[15] = base.Game.Content.Load<Texture2D>("PNG\\Bakuhatu\\Kaminari01");
		pimgBakuhatu[16] = base.Game.Content.Load<Texture2D>("PNG\\Bakuhatu\\Kaminari03");
		pimgBakuhatu[17] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\ESP_Te00");
		base.LoadContent();
	}

	private void ImageStateUpdate()
	{
		for (int i = 0; i < psrcBakuhatuCore.Length; i++)
		{
			if (psrcBakuhatuCore[i].pflgEnable)
			{
				for (int j = 0; j < psrcBakuhatuCore[i].penuImgState.Length - 1; j++)
				{
					psrcBakuhatuCore[i].penuImgState[j] = psrcBakuhatuCore[i].penuImgState[j + 1];
				}
			}
		}
	}

	private void NomarlStateSet(int aintBakuhatuNo)
	{
		for (int i = 0; i < psrcBakuhatuCore[aintBakuhatuNo].penuImgState.Length; i++)
		{
			psrcBakuhatuCore[aintBakuhatuNo].penuImgState[i] = enuBakuhatuImgState.intKieru;
		}
	}

	private void Bakuhatu00StateSet(int aintBakuhatuNo)
	{
		for (int i = 0; i < psrcBakuhatuCore[aintBakuhatuNo].penuImgState.Length; i++)
		{
			psrcBakuhatuCore[aintBakuhatuNo].penuImgState[i] = enuBakuhatuImgState.intBakuhatu02;
		}
	}

	private void Bakuhatu00_redStateSet(int aintBakuhatuNo)
	{
		for (int i = 0; i < psrcBakuhatuCore[aintBakuhatuNo].penuImgState.Length; i++)
		{
			psrcBakuhatuCore[aintBakuhatuNo].penuImgState[i] = enuBakuhatuImgState.intBakuhatu03_r;
		}
	}

	private void Bakuhatu01StateSet(int aintBakuhatuNo)
	{
		for (int i = 0; i < psrcBakuhatuCore[aintBakuhatuNo].penuImgState.Length; i++)
		{
			psrcBakuhatuCore[aintBakuhatuNo].penuImgState[i] = enuBakuhatuImgState.intBakuhatu04;
		}
	}

	private void Bakuhatu01_redStateSet(int aintBakuhatuNo)
	{
		for (int i = 0; i < psrcBakuhatuCore[aintBakuhatuNo].penuImgState.Length; i++)
		{
			psrcBakuhatuCore[aintBakuhatuNo].penuImgState[i] = enuBakuhatuImgState.intBakuhatu05_r;
		}
	}

	private void Kemuri00StateSet(int aintBakuhatuNo)
	{
		for (int i = 0; i < psrcBakuhatuCore[aintBakuhatuNo].penuImgState.Length; i++)
		{
			psrcBakuhatuCore[aintBakuhatuNo].penuImgState[i] = enuBakuhatuImgState.intKemuri04;
		}
	}

	private void Kemuri01StateSet(int aintBakuhatuNo)
	{
		for (int i = 0; i < psrcBakuhatuCore[aintBakuhatuNo].penuImgState.Length; i++)
		{
			psrcBakuhatuCore[aintBakuhatuNo].penuImgState[i] = enuBakuhatuImgState.intKemuri06;
		}
	}

	private void Kemuri02StateSet(int aintBakuhatuNo)
	{
		for (int i = 0; i < psrcBakuhatuCore[aintBakuhatuNo].penuImgState.Length; i++)
		{
			psrcBakuhatuCore[aintBakuhatuNo].penuImgState[i] = enuBakuhatuImgState.intKemuri07;
		}
	}

	private void Hit00StateSet(int aintBakuhatuNo)
	{
		for (int i = 0; i < psrcBakuhatuCore[aintBakuhatuNo].penuImgState.Length; i++)
		{
			psrcBakuhatuCore[aintBakuhatuNo].penuImgState[i] = enuBakuhatuImgState.intHit00;
		}
	}

	private void Hit01StateSet(int aintBakuhatuNo)
	{
		for (int i = 0; i < psrcBakuhatuCore[aintBakuhatuNo].penuImgState.Length; i++)
		{
			psrcBakuhatuCore[aintBakuhatuNo].penuImgState[i] = enuBakuhatuImgState.intHit01;
		}
	}

	private void Kaminari00StateSet(int aintKaminariNo)
	{
		for (int i = 0; i < psrcBakuhatuCore[aintKaminariNo].penuImgState.Length; i++)
		{
			psrcBakuhatuCore[aintKaminariNo].penuImgState[i] = enuBakuhatuImgState.intKaminari00;
		}
		for (int i = 0; i < psrcBakuhatuCore[aintKaminariNo].penuImgState.Length - 4; i += 8)
		{
			psrcBakuhatuCore[aintKaminariNo].penuImgState[i] = enuBakuhatuImgState.intKieru;
			psrcBakuhatuCore[aintKaminariNo].penuImgState[i + 1] = enuBakuhatuImgState.intKieru;
			psrcBakuhatuCore[aintKaminariNo].penuImgState[i + 2] = enuBakuhatuImgState.intKieru;
			psrcBakuhatuCore[aintKaminariNo].penuImgState[i + 3] = enuBakuhatuImgState.intKieru;
		}
	}

	private void Kaminari01StateSet(int aintKaminariNo)
	{
		for (int i = 0; i < psrcBakuhatuCore[aintKaminariNo].penuImgState.Length; i++)
		{
			psrcBakuhatuCore[aintKaminariNo].penuImgState[i] = enuBakuhatuImgState.intKaminari01;
		}
		for (int i = 0; i < psrcBakuhatuCore[aintKaminariNo].penuImgState.Length - 4; i += 8)
		{
			psrcBakuhatuCore[aintKaminariNo].penuImgState[i] = enuBakuhatuImgState.intKieru;
			psrcBakuhatuCore[aintKaminariNo].penuImgState[i + 1] = enuBakuhatuImgState.intKieru;
			psrcBakuhatuCore[aintKaminariNo].penuImgState[i + 2] = enuBakuhatuImgState.intKieru;
			psrcBakuhatuCore[aintKaminariNo].penuImgState[i + 3] = enuBakuhatuImgState.intKieru;
		}
	}

	private void ESPHand00StateSet(int aintESPNo)
	{
		for (int i = 0; i < psrcBakuhatuCore[aintESPNo].penuImgState.Length; i++)
		{
			psrcBakuhatuCore[aintESPNo].penuImgState[i] = enuBakuhatuImgState.intESPHand00;
		}
		for (int i = 0; i < psrcBakuhatuCore[aintESPNo].penuImgState.Length - 4; i += 8)
		{
			psrcBakuhatuCore[aintESPNo].penuImgState[i] = enuBakuhatuImgState.intKieru;
			psrcBakuhatuCore[aintESPNo].penuImgState[i + 1] = enuBakuhatuImgState.intKieru;
			psrcBakuhatuCore[aintESPNo].penuImgState[i + 2] = enuBakuhatuImgState.intKieru;
			psrcBakuhatuCore[aintESPNo].penuImgState[i + 3] = enuBakuhatuImgState.intKieru;
		}
	}

	public void pBakuhatuDraw(SpriteBatch aspritesBatch)
	{
		if (pimgBakuhatu[1] == null)
		{
			return;
		}
		for (int i = 0; i < psrcBakuhatuCore.Length; i++)
		{
			if (psrcBakuhatuCore[i].pflgEnable && psrcBakuhatuCore[i].penuImgState[0] != enuBakuhatuImgState.intKieru)
			{
				int width = pimgBakuhatu[(int)psrcBakuhatuCore[i].penuImgState[0]].Width;
				int height = pimgBakuhatu[(int)psrcBakuhatuCore[i].penuImgState[0]].Height;
				aspritesBatch.Draw(pimgBakuhatu[(int)psrcBakuhatuCore[i].penuImgState[0]], new Vector2(psrcBakuhatuCore[i].pVecIti.X * psrcBakuhatuCore[i].pVecIti.Z + 640f, psrcBakuhatuCore[i].pVecIti.Y * psrcBakuhatuCore[i].pVecIti.Z + 360f), null, new Color(160, 160, 160, 160), MathHelper.ToRadians(psrcBakuhatuCore[i].pfltItiR), new Vector2(width / 2, height / 2), new Vector2(psrcBakuhatuCore[i].pVecIti.Z * psrcBakuhatuCore[i].fltScale, psrcBakuhatuCore[i].pVecIti.Z * psrcBakuhatuCore[i].fltScale), SpriteEffects.None, psrcBakuhatuCore[i].pVecIti.Z);
			}
		}
	}

	public override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
	}
}
