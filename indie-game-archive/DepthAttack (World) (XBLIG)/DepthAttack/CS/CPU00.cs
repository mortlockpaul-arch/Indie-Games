using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DepthAttack.CS;

public class CPU00 : DrawableGameComponent
{
	public struct srcCPU00Core
	{
		public bool pflgEnable;

		public Vector3 pVecIti;

		public float pfltItiR;

		public Vector3 pVecMovIti;

		public Vector3 pVecMovDelta;

		public enuCPU00Type penuType;

		public int pintHP;

		public int pintDF;

		public enuCPU00ImgState[] penuImgState;

		public SpriteEffects pSpriteEffects;

		public structCPU00AI[] psrcAI;

		public enuCPU00MovState enuMovState;

		public int pintDashCount;

		public int pintDamageCount;
	}

	public enum enuCPU00ImgState
	{
		intKieru,
		intNormal00,
		intTower00,
		intTower00R90,
		intTower00R270,
		intTowerBack00,
		intTowerBackX00,
		intHoudai00,
		intHoudaiX00,
		intInvader00,
		intInvaderX00,
		intPlane00,
		intBee00,
		intBee01,
		intTowerFront00,
		intTowerFrontX00,
		intTower00_y,
		intTower00_r,
		intTower00R90_y,
		intTower00R90_r,
		intTowerBack00_y,
		intTowerBack00_r,
		intTowerBack00X_y,
		intTowerBack00X_r,
		intTowerFront00_y,
		intTowerFront00_r,
		intTowerFront00X_y,
		intTowerFront00X_r,
		intTower00R270_y,
		intTower00R270_r,
		intBeetle00,
		intBeetle01,
		intTonbo00,
		intTonbo01
	}

	public enum enuCPU00Type
	{
		intNormal00,
		intNormal01,
		intTower00,
		intTower00R90,
		intTower00R270,
		intTowerBack00,
		intTowerBackX00,
		intHoudai00,
		intHoudaiX00,
		intInvader00,
		intInvaderX00,
		intPlane00,
		intBee00,
		intBeeX00,
		intTowerFront00,
		intTowerFrontX00,
		intTower00_y,
		intTower00_r,
		intTower00R90_y,
		intTower00R90_r,
		intTowerBack00_y,
		intTowerBack00_r,
		intTowerBackX00_y,
		intTowerBackX00_r,
		intTowerFront00_r,
		intTowerFront00_y,
		intTowerFrontX00_r,
		intTowerFrontX00_y,
		intTower00R270_y,
		intTower00R270_r,
		intBeetle00,
		intBeetleX00,
		intTonbo00,
		intTonboX00,
		intQueenChildBee00,
		intQueenChildBeeX00
	}

	public struct structCPU00AI
	{
		public int pintLoopCount;

		public OperationTypeMovX penuTypeMovX;

		public OperationTypeMovY penuTypeMovY;

		public OperationTypeMovZ penuTypeMovZ;

		public OperationTypeMovXDash penuTypeMovXDash;

		public OperationTypeMovYDash penuTypeMovYDash;

		public OperationTypeMovZDash penuTypeMovZDash;

		public OperationTypeMovAttack penuTypeMovAttack;

		public OperationTypeMukiX penuTypeMuki;

		public OperationTypeDead penuTypeDead;
	}

	public enum OperationTypeMovX
	{
		None,
		Xp,
		Xm
	}

	public enum OperationTypeMovY
	{
		None,
		Yp,
		Ym
	}

	public enum OperationTypeMovZ
	{
		None,
		Zp,
		Zm
	}

	public enum OperationTypeMovXDash
	{
		None,
		XpDash,
		XmDash,
		DashCancel
	}

	public enum OperationTypeMovYDash
	{
		None,
		YpDash,
		YmDash,
		DashCancel
	}

	public enum OperationTypeMovZDash
	{
		None,
		ZpDash,
		ZmDash,
		DashCancel
	}

	public enum OperationTypeMovAttack
	{
		None,
		Attack
	}

	public enum OperationTypeMukiX
	{
		None,
		X,
		Y
	}

	public enum OperationTypeDead
	{
		None,
		Dead
	}

	public enum enuCPU00MovState
	{
		intNormal,
		intMove,
		intDashMove,
		intAttack,
		intDamage,
		intDead
	}

	private const string cstrCPU00_00 = "PNG\\Character\\CPU\\TekiSize01";

	private const string cstrCPU00Tower00 = "PNG\\Character\\CPU\\Tower01";

	private const string cstrCPU00TowerBack00 = "PNG\\Character\\CPU\\TowerBack02";

	private const string cstrCPU00Houdai00 = "PNG\\Character\\CPU\\Houdai01";

	private const string cstrCPU00Invader00 = "PNG\\Character\\CPU\\Invader01";

	private const string cstrCPU00Invader01 = "PNG\\Character\\CPU\\Invader02";

	private const string cstrCPU00Plane00 = "PNG\\Character\\CPU\\Plane07";

	private const string cstrCPU00Bee00 = "PNG\\Character\\CPU\\Bee09";

	private const string cstrCPU00Bee01 = "PNG\\Character\\CPU\\Bee10";

	private const string cstrCPU00TowerFront00 = "PNG\\Character\\CPU\\TowerFront03";

	private const string cstrCPU00Tower00_y = "PNG\\Character\\CPU\\Tower01_y";

	private const string cstrCPU00Tower00_r = "PNG\\Character\\CPU\\Tower01_r";

	private const string cstrCPU00TowerBack00_y = "PNG\\Character\\CPU\\TowerBack02_y";

	private const string cstrCPU00TowerBack00_r = "PNG\\Character\\CPU\\TowerBack02_r";

	private const string cstrCPU00TowerFront00_y = "PNG\\Character\\CPU\\TowerFront03_y";

	private const string cstrCPU00TowerFront00_r = "PNG\\Character\\CPU\\TowerFront03_r";

	private const string cstrCPU00Beetle00 = "PNG\\Character\\CPU\\Beetle06";

	private const string cstrCPU00Beetle01 = "PNG\\Character\\CPU\\Beetle07";

	private const string cstrCPU00Tonbo00 = "PNG\\Character\\CPU\\Tonbo06";

	private const string cstrCPU00Tonbo01 = "PNG\\Character\\CPU\\Tonbo07";

	public srcCPU00Core[] psrcCPU00Core = new srcCPU00Core[32];

	public Texture2D[] pimgCPU00 = new Texture2D[36];

	public long[] lngScoreUp = new long[36]
	{
		2000L, 2000L, 2000L, 3000L, 3000L, 3000L, 3000L, 2000L, 2000L, 2000L,
		2000L, 1500L, 1500L, 1500L, 2000L, 2000L, 3000L, 5000L, 4000L, 6000L,
		4000L, 6000L, 4000L, 6000L, 3000L, 5000L, 3000L, 5000L, 4000L, 6000L,
		5000L, 5000L, 4000L, 4000L, 1500L, 1500L
	};

	public float[] fltOffSetHaba = new float[36]
	{
		0f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		1f / 32f,
		1f / 32f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f
	};

	public Rectangle[,] precCPU00OffSet = new Rectangle[36, 4]
	{
		{
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-98, -98, 196, 196),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-5, -128, 10, 256),
			new Rectangle(-16, -64, 32, 192),
			new Rectangle(-32, 0, 96, 128),
			new Rectangle(-64, 64, 128, 64)
		},
		{
			new Rectangle(-128, -5, 256, 10),
			new Rectangle(-64, -64, 192, 32),
			new Rectangle(0, -96, 128, 64),
			new Rectangle(64, -128, 64, 128)
		},
		{
			new Rectangle(-128, -128, 64, 256),
			new Rectangle(-64, -64, 64, 192),
			new Rectangle(0, 0, 64, 128),
			new Rectangle(64, 64, 64, 64)
		},
		{
			new Rectangle(-5, -128, 10, 256),
			new Rectangle(-20, -113, 40, 238),
			new Rectangle(-46, 98, 90, 143),
			new Rectangle(-60, -54, 114, 87)
		},
		{
			new Rectangle(-5, -128, 10, 256),
			new Rectangle(-20, -113, 40, 238),
			new Rectangle(-46, 98, 90, 143),
			new Rectangle(-60, -54, 114, 87)
		},
		{
			new Rectangle(7, -122, 44, 49),
			new Rectangle(-40, -73, 110, 150),
			new Rectangle(-125, 21, 251, 34),
			new Rectangle(-43, 75, 37, 52)
		},
		{
			new Rectangle(-55, -124, 44, 47),
			new Rectangle(-71, -73, 109, 147),
			new Rectangle(-125, 19, 252, 38),
			new Rectangle(8, 73, 35, 51)
		},
		{
			new Rectangle(-64, -128, 128, 256),
			new Rectangle(-128, -64, 256, 192),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-64, -128, 128, 255),
			new Rectangle(-128, 64, 256, 192),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-31, -64, 103, 63),
			new Rectangle(-68, -48, 125, 68),
			new Rectangle(-104, -60, 36, 65),
			new Rectangle(57, -62, 46, 69)
		},
		{
			new Rectangle(-24, -38, 43, 87),
			new Rectangle(57, 43, 35, 37),
			new Rectangle(22, -39, 35, 37),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-24, -38, 43, 87),
			new Rectangle(-58, -43, 35, 37),
			new Rectangle(22, -43, 35, 37),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-64, -98, 128, 98),
			new Rectangle(-51, -1, 100, 37),
			new Rectangle(-31, 36, 58, 41),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-64, -98, 128, 98),
			new Rectangle(-51, -1, 100, 37),
			new Rectangle(-31, 36, 58, 41),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-5, -128, 10, 256),
			new Rectangle(-16, -64, 32, 192),
			new Rectangle(-32, 0, 96, 128),
			new Rectangle(-64, 64, 128, 64)
		},
		{
			new Rectangle(-5, -128, 10, 256),
			new Rectangle(-16, -64, 32, 192),
			new Rectangle(-32, 0, 96, 128),
			new Rectangle(-64, 64, 128, 64)
		},
		{
			new Rectangle(-128, -5, 256, 10),
			new Rectangle(-64, -64, 192, 32),
			new Rectangle(0, -96, 128, 64),
			new Rectangle(64, -128, 64, 128)
		},
		{
			new Rectangle(-128, -5, 256, 10),
			new Rectangle(-64, -64, 192, 32),
			new Rectangle(0, -96, 128, 64),
			new Rectangle(64, -128, 64, 128)
		},
		{
			new Rectangle(-5, -128, 10, 256),
			new Rectangle(-20, -113, 40, 238),
			new Rectangle(-46, 98, 90, 143),
			new Rectangle(-60, -54, 114, 87)
		},
		{
			new Rectangle(-5, -128, 10, 256),
			new Rectangle(-20, -113, 40, 238),
			new Rectangle(-46, 98, 90, 143),
			new Rectangle(-60, -54, 114, 87)
		},
		{
			new Rectangle(-5, -128, 10, 256),
			new Rectangle(-20, -113, 40, 238),
			new Rectangle(-46, 98, 90, 143),
			new Rectangle(-60, -54, 114, 87)
		},
		{
			new Rectangle(-5, -128, 10, 256),
			new Rectangle(-20, -113, 40, 238),
			new Rectangle(-46, 98, 90, 143),
			new Rectangle(-60, -54, 114, 87)
		},
		{
			new Rectangle(-64, -98, 128, 98),
			new Rectangle(-51, -1, 100, 37),
			new Rectangle(-31, 36, 58, 41),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-64, -98, 128, 98),
			new Rectangle(-51, -1, 100, 37),
			new Rectangle(-31, 36, 58, 41),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-64, -98, 128, 98),
			new Rectangle(-51, -1, 100, 37),
			new Rectangle(-31, 36, 58, 41),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-64, -98, 128, 98),
			new Rectangle(-51, -1, 100, 37),
			new Rectangle(-31, 36, 58, 41),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-128, -128, 64, 256),
			new Rectangle(-64, -64, 64, 192),
			new Rectangle(0, 0, 64, 128),
			new Rectangle(64, 64, 64, 64)
		},
		{
			new Rectangle(-128, -128, 64, 256),
			new Rectangle(-64, -64, 64, 192),
			new Rectangle(0, 0, 64, 128),
			new Rectangle(64, 64, 64, 64)
		},
		{
			new Rectangle(-33, -75, 69, 100),
			new Rectangle(-14, 25, 29, 63),
			new Rectangle(-91, -117, 53, 113),
			new Rectangle(35, -117, 57, 111)
		},
		{
			new Rectangle(-33, -75, 69, 100),
			new Rectangle(14, 25, 23, 63),
			new Rectangle(-91, -117, 53, 113),
			new Rectangle(35, -117, 57, 111)
		},
		{
			new Rectangle(-128, -39, 108, 61),
			new Rectangle(-18, -16, 66, 31),
			new Rectangle(48, -31, 78, 48),
			new Rectangle(-44, 25, 49, 70)
		},
		{
			new Rectangle(-128, -39, 108, 61),
			new Rectangle(-18, -16, 66, 31),
			new Rectangle(48, -31, 78, 48),
			new Rectangle(-45, -126, 52, 96)
		},
		{
			new Rectangle(-24, -38, 43, 87),
			new Rectangle(57, 43, 35, 37),
			new Rectangle(22, -39, 35, 37),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-24, -38, 43, 87),
			new Rectangle(-58, -43, 35, 37),
			new Rectangle(22, -43, 35, 37),
			new Rectangle(0, 0, 0, 0)
		}
	};

	public CPU00(Game game)
		: base(game)
	{
		for (int i = 0; i < psrcCPU00Core.Length; i++)
		{
			psrcCPU00Core[i].penuImgState = new enuCPU00ImgState[30];
			psrcCPU00Core[i].psrcAI = new structCPU00AI[30];
		}
	}

	public override void Initialize()
	{
		pCPU00Init();
		base.Initialize();
	}

	public void pCPU00Init()
	{
		for (int i = 0; i < psrcCPU00Core.Length; i++)
		{
			KakuCPU00Init(i);
		}
	}

	private void KakuCPU00Init(int aintCPUNo)
	{
		psrcCPU00Core[aintCPUNo].pflgEnable = false;
		psrcCPU00Core[aintCPUNo].pSpriteEffects = SpriteEffects.None;
		psrcCPU00Core[aintCPUNo].pintDF = 0;
		psrcCPU00Core[aintCPUNo].pintHP = 0;
		psrcCPU00Core[aintCPUNo].pVecIti.X = 0f;
		psrcCPU00Core[aintCPUNo].pVecIti.Y = 0f;
		psrcCPU00Core[aintCPUNo].pVecIti.Z = 0f;
		psrcCPU00Core[aintCPUNo].pfltItiR = 0f;
		psrcCPU00Core[aintCPUNo].pVecMovIti.X = 0f;
		psrcCPU00Core[aintCPUNo].pVecMovIti.Y = 0f;
		psrcCPU00Core[aintCPUNo].pVecMovIti.Z = 0f;
		psrcCPU00Core[aintCPUNo].pVecMovDelta.X = 0f;
		psrcCPU00Core[aintCPUNo].pVecMovDelta.Y = 0f;
		psrcCPU00Core[aintCPUNo].pVecMovDelta.Z = 0f;
		CPU00AIInit(aintCPUNo);
		psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intNormal;
		psrcCPU00Core[aintCPUNo].pintDashCount = 0;
		psrcCPU00Core[aintCPUNo].pintDamageCount = 0;
		psrcCPU00Core[aintCPUNo].penuType = enuCPU00Type.intNormal00;
		NomarlStartStateSet(aintCPUNo);
	}

	private void CPU00AIInit(int aintCPUNo)
	{
		for (int i = 0; i < psrcCPU00Core[aintCPUNo].psrcAI.Length; i++)
		{
			psrcCPU00Core[aintCPUNo].psrcAI[i].pintLoopCount = 0;
			psrcCPU00Core[aintCPUNo].psrcAI[i].penuTypeMovX = OperationTypeMovX.None;
			psrcCPU00Core[aintCPUNo].psrcAI[i].penuTypeMovY = OperationTypeMovY.None;
			psrcCPU00Core[aintCPUNo].psrcAI[i].penuTypeMovZ = OperationTypeMovZ.None;
			psrcCPU00Core[aintCPUNo].psrcAI[i].penuTypeMovXDash = OperationTypeMovXDash.None;
			psrcCPU00Core[aintCPUNo].psrcAI[i].penuTypeMovYDash = OperationTypeMovYDash.None;
			psrcCPU00Core[aintCPUNo].psrcAI[i].penuTypeMovZDash = OperationTypeMovZDash.None;
			psrcCPU00Core[aintCPUNo].psrcAI[i].penuTypeMovAttack = OperationTypeMovAttack.None;
			psrcCPU00Core[aintCPUNo].psrcAI[i].penuTypeMuki = OperationTypeMukiX.None;
			psrcCPU00Core[aintCPUNo].psrcAI[i].penuTypeDead = OperationTypeDead.None;
		}
	}

	public void pCPU00Enable(Vector3 avec3Iti, enuCPU00Type aenuCPU00Type)
	{
		for (int i = 0; i < psrcCPU00Core.Length; i++)
		{
			if (!psrcCPU00Core[i].pflgEnable)
			{
				KakuCPU00Init(i);
				psrcCPU00Core[i].pflgEnable = true;
				psrcCPU00Core[i].pSpriteEffects = SpriteEffects.None;
				psrcCPU00Core[i].pVecIti.X = avec3Iti.X;
				psrcCPU00Core[i].pVecIti.Y = avec3Iti.Y;
				psrcCPU00Core[i].pVecIti.Z = avec3Iti.Z;
				psrcCPU00Core[i].pVecMovIti.X = 0f;
				psrcCPU00Core[i].pVecMovIti.Y = 0f;
				psrcCPU00Core[i].pVecMovIti.Z = 0f;
				psrcCPU00Core[i].penuType = aenuCPU00Type;
				CPU00AIInit(i);
				NomarlStartStateSet(i);
				break;
			}
		}
	}

	public void pCPU00SPEnable(Vector3 avec3Iti, SpriteEffects aSp, enuCPU00Type aenuCPU00Type)
	{
		for (int i = 0; i < psrcCPU00Core.Length; i++)
		{
			if (!psrcCPU00Core[i].pflgEnable)
			{
				KakuCPU00Init(i);
				psrcCPU00Core[i].pflgEnable = true;
				psrcCPU00Core[i].pSpriteEffects = aSp;
				psrcCPU00Core[i].pVecIti.X = avec3Iti.X;
				psrcCPU00Core[i].pVecIti.Y = avec3Iti.Y;
				psrcCPU00Core[i].pVecIti.Z = avec3Iti.Z;
				psrcCPU00Core[i].pVecMovIti.X = 0f;
				psrcCPU00Core[i].pVecMovIti.Y = 0f;
				psrcCPU00Core[i].pVecMovIti.Z = 0f;
				psrcCPU00Core[i].penuType = aenuCPU00Type;
				CPU00AIInit(i);
				NomarlStartStateSet(i);
				break;
			}
		}
	}

	public void pCPU00ItiREnable(Vector3 avec3Iti, float afltItiR, enuCPU00Type aenuCPU00Type)
	{
		for (int i = 0; i < psrcCPU00Core.Length; i++)
		{
			if (!psrcCPU00Core[i].pflgEnable)
			{
				KakuCPU00Init(i);
				psrcCPU00Core[i].pflgEnable = true;
				psrcCPU00Core[i].pSpriteEffects = SpriteEffects.None;
				psrcCPU00Core[i].pVecIti.X = avec3Iti.X;
				psrcCPU00Core[i].pVecIti.Y = avec3Iti.Y;
				psrcCPU00Core[i].pVecIti.Z = avec3Iti.Z;
				psrcCPU00Core[i].pfltItiR = afltItiR;
				psrcCPU00Core[i].pVecMovIti.X = 0f;
				psrcCPU00Core[i].pVecMovIti.Y = 0f;
				psrcCPU00Core[i].pVecMovIti.Z = 0f;
				psrcCPU00Core[i].penuType = aenuCPU00Type;
				CPU00AIInit(i);
				NomarlStartStateSet(i);
				break;
			}
		}
	}

	public void pCPU00SPItiREnable(Vector3 avec3Iti, SpriteEffects aSp, float afltItiR, enuCPU00Type aenuCPU00Type)
	{
		for (int i = 0; i < psrcCPU00Core.Length; i++)
		{
			if (!psrcCPU00Core[i].pflgEnable)
			{
				KakuCPU00Init(i);
				psrcCPU00Core[i].pflgEnable = true;
				psrcCPU00Core[i].pSpriteEffects = aSp;
				psrcCPU00Core[i].pVecIti.X = avec3Iti.X;
				psrcCPU00Core[i].pVecIti.Y = avec3Iti.Y;
				psrcCPU00Core[i].pVecIti.Z = avec3Iti.Z;
				psrcCPU00Core[i].pfltItiR = afltItiR;
				psrcCPU00Core[i].pVecMovIti.X = 0f;
				psrcCPU00Core[i].pVecMovIti.Y = 0f;
				psrcCPU00Core[i].pVecMovIti.Z = 0f;
				psrcCPU00Core[i].penuType = aenuCPU00Type;
				CPU00AIInit(i);
				NomarlStartStateSet(i);
				break;
			}
		}
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}

	public void pCPU00Update()
	{
		pImageStateUpdate();
		for (int i = 0; i < psrcCPU00Core.Length; i++)
		{
			if (psrcCPU00Core[i].pflgEnable)
			{
				kakuCPU00Update(i);
			}
		}
	}

	private void kakuCPU00Update(int aintCPUNo)
	{
		CPU00AISift(aintCPUNo);
		Game1.cPUAI00.pkakuCPU00(ref psrcCPU00Core[aintCPUNo]);
		switch (psrcCPU00Core[aintCPUNo].penuType)
		{
		case enuCPU00Type.intNormal00:
			CPU00Normal00Update(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intNormal01:
			break;
		case enuCPU00Type.intTower00:
			CPU00Tower00Update(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intTower00R90:
			CPU00Tower00Update(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intTower00R270:
			CPU00Tower00Update(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intTowerBack00:
			CPU00TowerBack00Update(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intTowerBackX00:
			CPU00TowerBack00Update(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intHoudai00:
			CPU00Houdai00Update(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intHoudaiX00:
			CPU00Houdai00Update(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intInvader00:
			CPU00Normal00Update(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intInvaderX00:
			CPU00Normal00Update(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intPlane00:
			CPU00Plane00Update(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intBee00:
			CPU00Normal00Update(aintCPUNo);
			Bee00StateSet(aintCPUNo);
			break;
		case enuCPU00Type.intBeeX00:
			CPU00Normal00Update(aintCPUNo);
			Bee00StateSet(aintCPUNo);
			break;
		case enuCPU00Type.intTowerFront00:
			CPU00TowerFront00Update(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intTowerFrontX00:
			CPU00TowerFront00Update(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intTower00_y:
			CPU00Tower00_yUpdate(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intTower00_r:
			CPU00Tower00Update(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intTower00R90_y:
			CPU00Tower00_yUpdate(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intTower00R90_r:
			CPU00Tower00Update(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intTowerBack00_y:
			CPU00TowerBack00_yUpdate(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intTowerBack00_r:
			CPU00TowerBack00_yUpdate(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intTowerBackX00_y:
			CPU00TowerBack00_yUpdate(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intTowerBackX00_r:
			CPU00TowerBack00_yUpdate(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intTowerFront00_y:
			CPU00TowerFront00_yUpdate(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intTowerFront00_r:
			CPU00TowerFront00_yUpdate(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intTowerFrontX00_y:
			CPU00TowerFront00_yUpdate(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intTowerFrontX00_r:
			CPU00TowerFront00_yUpdate(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intTower00R270_y:
			CPU00Tower00_yUpdate(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intTower00R270_r:
			CPU00Tower00Update(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPU00Type.intBeetle00:
			CPU00Normal00Update(aintCPUNo);
			Beetle00StateSet(aintCPUNo);
			break;
		case enuCPU00Type.intBeetleX00:
			CPU00Normal00Update(aintCPUNo);
			Beetle00StateSet(aintCPUNo);
			break;
		case enuCPU00Type.intTonbo00:
			CPU00Normal00Update(aintCPUNo);
			Tonbo00StateSet(aintCPUNo);
			break;
		case enuCPU00Type.intTonboX00:
			CPU00Normal00Update(aintCPUNo);
			Tonbo00StateSet(aintCPUNo);
			break;
		case enuCPU00Type.intQueenChildBee00:
			CPU00Normal00Update(aintCPUNo);
			Bee00StateSet(aintCPUNo);
			break;
		case enuCPU00Type.intQueenChildBeeX00:
			CPU00Normal00Update(aintCPUNo);
			Bee00StateSet(aintCPUNo);
			break;
		}
	}

	private void CPU00AISift(int aintCPUNo)
	{
		if (psrcCPU00Core[aintCPUNo].psrcAI[0].pintLoopCount > 0)
		{
			psrcCPU00Core[aintCPUNo].psrcAI[0].pintLoopCount--;
			return;
		}
		for (int i = 0; i < psrcCPU00Core[aintCPUNo].psrcAI.Length - 1; i++)
		{
			ref structCPU00AI reference = ref psrcCPU00Core[aintCPUNo].psrcAI[i];
			reference = psrcCPU00Core[aintCPUNo].psrcAI[i + 1];
		}
		int num = psrcCPU00Core[aintCPUNo].psrcAI.Length - 1;
		psrcCPU00Core[aintCPUNo].psrcAI[num].pintLoopCount = 0;
		psrcCPU00Core[aintCPUNo].psrcAI[num].penuTypeMovX = OperationTypeMovX.None;
		psrcCPU00Core[aintCPUNo].psrcAI[num].penuTypeMovY = OperationTypeMovY.None;
		psrcCPU00Core[aintCPUNo].psrcAI[num].penuTypeMovZ = OperationTypeMovZ.None;
		psrcCPU00Core[aintCPUNo].psrcAI[num].penuTypeMovXDash = OperationTypeMovXDash.None;
		psrcCPU00Core[aintCPUNo].psrcAI[num].penuTypeMovYDash = OperationTypeMovYDash.None;
		psrcCPU00Core[aintCPUNo].psrcAI[num].penuTypeMovZDash = OperationTypeMovZDash.None;
		psrcCPU00Core[aintCPUNo].psrcAI[num].penuTypeMovAttack = OperationTypeMovAttack.None;
		psrcCPU00Core[aintCPUNo].psrcAI[num].penuTypeMuki = OperationTypeMukiX.None;
	}

	private void CPU00Normal00OpeUpdate(int aintCPUNo)
	{
		if (psrcCPU00Core[aintCPUNo].psrcAI[0].penuTypeMovZ == OperationTypeMovZ.Zm)
		{
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intMove;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Z = -1f / 128f;
		}
		else if (psrcCPU00Core[aintCPUNo].psrcAI[0].penuTypeMovZ == OperationTypeMovZ.Zp)
		{
			psrcCPU00Core[aintCPUNo].pVecMovIti.Z = 1f / 128f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intMove;
		}
		if (psrcCPU00Core[aintCPUNo].psrcAI[0].penuTypeMovX == OperationTypeMovX.Xm)
		{
			psrcCPU00Core[aintCPUNo].pVecMovIti.X = -15f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intMove;
		}
		else if (psrcCPU00Core[aintCPUNo].psrcAI[0].penuTypeMovX == OperationTypeMovX.Xp)
		{
			psrcCPU00Core[aintCPUNo].pVecMovIti.X = 15f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intMove;
		}
		if (psrcCPU00Core[aintCPUNo].psrcAI[0].penuTypeMovY == OperationTypeMovY.Ym)
		{
			psrcCPU00Core[aintCPUNo].pVecMovIti.Y = -15f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intMove;
		}
		else if (psrcCPU00Core[aintCPUNo].psrcAI[0].penuTypeMovY == OperationTypeMovY.Yp)
		{
			psrcCPU00Core[aintCPUNo].pVecMovIti.Y = 15f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intMove;
		}
		if (psrcCPU00Core[aintCPUNo].psrcAI[0].penuTypeMovZDash == OperationTypeMovZDash.ZmDash)
		{
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intMove;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Z = -1f / 64f;
		}
		else if (psrcCPU00Core[aintCPUNo].psrcAI[0].penuTypeMovZDash == OperationTypeMovZDash.ZpDash)
		{
			psrcCPU00Core[aintCPUNo].pVecMovIti.Z = 1f / 64f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intMove;
		}
		if (psrcCPU00Core[aintCPUNo].psrcAI[0].penuTypeMovXDash == OperationTypeMovXDash.XmDash)
		{
			psrcCPU00Core[aintCPUNo].pVecMovIti.X = -30f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intMove;
		}
		else if (psrcCPU00Core[aintCPUNo].psrcAI[0].penuTypeMovXDash == OperationTypeMovXDash.XpDash)
		{
			psrcCPU00Core[aintCPUNo].pVecMovIti.X = 30f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intMove;
		}
		if (psrcCPU00Core[aintCPUNo].psrcAI[0].penuTypeMovYDash == OperationTypeMovYDash.YmDash)
		{
			psrcCPU00Core[aintCPUNo].pVecMovIti.Y = -30f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intMove;
		}
		else if (psrcCPU00Core[aintCPUNo].psrcAI[0].penuTypeMovYDash == OperationTypeMovYDash.YpDash)
		{
			psrcCPU00Core[aintCPUNo].pVecMovIti.Y = 30f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intMove;
		}
		if (psrcCPU00Core[aintCPUNo].psrcAI[0].penuTypeMuki == OperationTypeMukiX.Y)
		{
			if (psrcCPU00Core[aintCPUNo].pSpriteEffects == SpriteEffects.None)
			{
				psrcCPU00Core[aintCPUNo].pSpriteEffects = SpriteEffects.FlipVertically;
			}
			else
			{
				psrcCPU00Core[aintCPUNo].pSpriteEffects = SpriteEffects.None;
			}
		}
		if (psrcCPU00Core[aintCPUNo].psrcAI[0].penuTypeMovAttack == OperationTypeMovAttack.Attack)
		{
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intAttack;
		}
		if (psrcCPU00Core[aintCPUNo].psrcAI[0].penuTypeDead == OperationTypeDead.Dead)
		{
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intDead;
		}
	}

	private void CPU00Normal00Update(int aintCPUNo)
	{
		CPU00Normal00OpeUpdate(aintCPUNo);
		switch (psrcCPU00Core[aintCPUNo].enuMovState)
		{
		case enuCPU00MovState.intNormal:
			break;
		case enuCPU00MovState.intMove:
			psrcCPU00Core[aintCPUNo].pVecIti.X += psrcCPU00Core[aintCPUNo].pVecMovIti.X;
			psrcCPU00Core[aintCPUNo].pVecIti.Y += psrcCPU00Core[aintCPUNo].pVecMovIti.Y;
			psrcCPU00Core[aintCPUNo].pVecIti.Z += psrcCPU00Core[aintCPUNo].pVecMovIti.Z;
			psrcCPU00Core[aintCPUNo].pVecMovIti.X = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Y = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Z = 0f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intNormal;
			break;
		case enuCPU00MovState.intDashMove:
			break;
		case enuCPU00MovState.intAttack:
			Game1.cPUTama.psrcCPUTamaCorePlayerItiEnable(psrcCPU00Core[aintCPUNo].pVecIti, new Vector3(0f, 0f, 3f / 128f), Game1.player.psrcPlayerCore.pVecIti);
			Game1.bakuhatu.psrcKemuriCoreConboEnable(psrcCPU00Core[aintCPUNo].pVecIti, psrcCPU00Core[aintCPUNo].pfltItiR, 0);
			break;
		case enuCPU00MovState.intDamage:
			break;
		case enuCPU00MovState.intDead:
			KakuCPU00Init(aintCPUNo);
			break;
		}
	}

	private void CPU00Tower00Update(int aintCPUNo)
	{
		CPU00Normal00OpeUpdate(aintCPUNo);
		switch (psrcCPU00Core[aintCPUNo].enuMovState)
		{
		case enuCPU00MovState.intNormal:
			break;
		case enuCPU00MovState.intMove:
			if ((int)psrcCPU00Core[aintCPUNo].pVecIti.Y % 50 == 0 && psrcCPU00Core[aintCPUNo].pVecMovIti.Y != 0f && psrcCPU00Core[aintCPUNo].pfltItiR == 0f)
			{
				Game1.bakuhatu.psrcBakuhatuMov01CoreConboEnable(new Vector3(psrcCPU00Core[aintCPUNo].pVecIti.X, psrcCPU00Core[aintCPUNo].pVecIti.Y + 256f, psrcCPU00Core[aintCPUNo].pVecIti.Z), new Vector3(psrcCPU00Core[aintCPUNo].pVecMovIti.X, psrcCPU00Core[aintCPUNo].pVecMovIti.Y, psrcCPU00Core[aintCPUNo].pVecMovIti.Z), psrcCPU00Core[aintCPUNo].pfltItiR, 0);
			}
			if ((int)psrcCPU00Core[aintCPUNo].pVecIti.X % 50 == 0 && psrcCPU00Core[aintCPUNo].pVecMovIti.X > 0f && psrcCPU00Core[aintCPUNo].pfltItiR != 0f)
			{
				Game1.bakuhatu.psrcBakuhatuMov01CoreConboEnable(new Vector3(psrcCPU00Core[aintCPUNo].pVecIti.X - 256f, psrcCPU00Core[aintCPUNo].pVecIti.Y, psrcCPU00Core[aintCPUNo].pVecIti.Z), new Vector3(psrcCPU00Core[aintCPUNo].pVecMovIti.X, psrcCPU00Core[aintCPUNo].pVecMovIti.Y, psrcCPU00Core[aintCPUNo].pVecMovIti.Z), psrcCPU00Core[aintCPUNo].pfltItiR, 0);
			}
			else if ((int)psrcCPU00Core[aintCPUNo].pVecIti.X % 50 == 0 && psrcCPU00Core[aintCPUNo].pVecMovIti.X < 0f && psrcCPU00Core[aintCPUNo].pfltItiR != 0f)
			{
				Game1.bakuhatu.psrcBakuhatuMov01CoreConboEnable(new Vector3(psrcCPU00Core[aintCPUNo].pVecIti.X + 256f, psrcCPU00Core[aintCPUNo].pVecIti.Y, psrcCPU00Core[aintCPUNo].pVecIti.Z), new Vector3(psrcCPU00Core[aintCPUNo].pVecMovIti.X, psrcCPU00Core[aintCPUNo].pVecMovIti.Y, psrcCPU00Core[aintCPUNo].pVecMovIti.Z), psrcCPU00Core[aintCPUNo].pfltItiR, 0);
			}
			psrcCPU00Core[aintCPUNo].pVecIti.X += psrcCPU00Core[aintCPUNo].pVecMovIti.X;
			psrcCPU00Core[aintCPUNo].pVecIti.Y += psrcCPU00Core[aintCPUNo].pVecMovIti.Y;
			psrcCPU00Core[aintCPUNo].pVecIti.Z += psrcCPU00Core[aintCPUNo].pVecMovIti.Z;
			psrcCPU00Core[aintCPUNo].pVecMovIti.X = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Y = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Z = 0f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intNormal;
			break;
		case enuCPU00MovState.intDashMove:
			if ((int)psrcCPU00Core[aintCPUNo].pVecIti.Y % 50 == 0 && psrcCPU00Core[aintCPUNo].pVecMovIti.Y != 0f)
			{
				Game1.bakuhatu.psrcBakuhatuMov01CoreConboEnable(new Vector3(psrcCPU00Core[aintCPUNo].pVecIti.X, psrcCPU00Core[aintCPUNo].pVecIti.Y + 64f, psrcCPU00Core[aintCPUNo].pVecIti.Z), new Vector3(psrcCPU00Core[aintCPUNo].pVecMovIti.X, psrcCPU00Core[aintCPUNo].pVecMovIti.Y, psrcCPU00Core[aintCPUNo].pVecMovIti.Z), psrcCPU00Core[aintCPUNo].pfltItiR, 0);
			}
			if ((int)psrcCPU00Core[aintCPUNo].pVecIti.X % 50 == 0 && psrcCPU00Core[aintCPUNo].pVecMovIti.X < 0f)
			{
				Game1.bakuhatu.psrcBakuhatuMov01CoreConboEnable(new Vector3(psrcCPU00Core[aintCPUNo].pVecIti.X + 128f, psrcCPU00Core[aintCPUNo].pVecIti.Y, psrcCPU00Core[aintCPUNo].pVecIti.Z), new Vector3(psrcCPU00Core[aintCPUNo].pVecMovIti.X, psrcCPU00Core[aintCPUNo].pVecMovIti.Y, psrcCPU00Core[aintCPUNo].pVecMovIti.Z), psrcCPU00Core[aintCPUNo].pfltItiR, 0);
			}
			else if ((int)psrcCPU00Core[aintCPUNo].pVecIti.X % 50 == 0 && psrcCPU00Core[aintCPUNo].pVecMovIti.X > 0f)
			{
				Game1.bakuhatu.psrcBakuhatuMov01CoreConboEnable(new Vector3(psrcCPU00Core[aintCPUNo].pVecIti.X - 128f, psrcCPU00Core[aintCPUNo].pVecIti.Y, psrcCPU00Core[aintCPUNo].pVecIti.Z), new Vector3(psrcCPU00Core[aintCPUNo].pVecMovIti.X, psrcCPU00Core[aintCPUNo].pVecMovIti.Y, psrcCPU00Core[aintCPUNo].pVecMovIti.Z), psrcCPU00Core[aintCPUNo].pfltItiR, 0);
			}
			psrcCPU00Core[aintCPUNo].pVecIti.X += psrcCPU00Core[aintCPUNo].pVecMovIti.X;
			psrcCPU00Core[aintCPUNo].pVecIti.Y += psrcCPU00Core[aintCPUNo].pVecMovIti.Y;
			psrcCPU00Core[aintCPUNo].pVecIti.Z += psrcCPU00Core[aintCPUNo].pVecMovIti.Z;
			psrcCPU00Core[aintCPUNo].pVecMovIti.X = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Y = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Z = 0f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intNormal;
			break;
		case enuCPU00MovState.intAttack:
			Game1.cPUTama.psrcCPUTamaCorePlayerItiEnable(psrcCPU00Core[aintCPUNo].pVecIti, new Vector3(0f, 0f, 3f / 128f), Game1.player.psrcPlayerCore.pVecIti);
			Game1.bakuhatu.psrcKemuriCoreConboEnable(psrcCPU00Core[aintCPUNo].pVecIti, psrcCPU00Core[aintCPUNo].pfltItiR, 0);
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intNormal;
			break;
		case enuCPU00MovState.intDamage:
			break;
		case enuCPU00MovState.intDead:
			KakuCPU00Init(aintCPUNo);
			break;
		}
	}

	private void CPU00Tower00_yUpdate(int aintCPUNo)
	{
		CPU00Normal00OpeUpdate(aintCPUNo);
		switch (psrcCPU00Core[aintCPUNo].enuMovState)
		{
		case enuCPU00MovState.intNormal:
			break;
		case enuCPU00MovState.intMove:
			psrcCPU00Core[aintCPUNo].pVecIti.X += (float)(int)psrcCPU00Core[aintCPUNo].pVecMovIti.X * 1.5f;
			psrcCPU00Core[aintCPUNo].pVecIti.Y += (float)(int)psrcCPU00Core[aintCPUNo].pVecMovIti.Y * 1.5f;
			psrcCPU00Core[aintCPUNo].pVecIti.Z += psrcCPU00Core[aintCPUNo].pVecMovIti.Z;
			if ((int)psrcCPU00Core[aintCPUNo].pVecIti.Y % 50 == 0 && psrcCPU00Core[aintCPUNo].pVecMovIti.Y != 0f)
			{
				Game1.bakuhatu.psrcBakuhatuMov01CoreConboEnable(new Vector3(psrcCPU00Core[aintCPUNo].pVecIti.X, psrcCPU00Core[aintCPUNo].pVecIti.Y, psrcCPU00Core[aintCPUNo].pVecIti.Z), new Vector3(psrcCPU00Core[aintCPUNo].pVecMovIti.X * 1.5f, psrcCPU00Core[aintCPUNo].pVecMovIti.Y * 1.5f, psrcCPU00Core[aintCPUNo].pVecMovIti.Z), psrcCPU00Core[aintCPUNo].pfltItiR, 0);
			}
			if ((int)psrcCPU00Core[aintCPUNo].pVecIti.X % 50 == 0 && psrcCPU00Core[aintCPUNo].pVecMovIti.X > 0f)
			{
				Game1.bakuhatu.psrcBakuhatuMov01CoreConboEnable(new Vector3(psrcCPU00Core[aintCPUNo].pVecIti.X - 256f, psrcCPU00Core[aintCPUNo].pVecIti.Y, psrcCPU00Core[aintCPUNo].pVecIti.Z), new Vector3(psrcCPU00Core[aintCPUNo].pVecMovIti.X * 1.5f, psrcCPU00Core[aintCPUNo].pVecMovIti.Y * 1.5f, psrcCPU00Core[aintCPUNo].pVecMovIti.Z), psrcCPU00Core[aintCPUNo].pfltItiR, 0);
			}
			else if ((int)psrcCPU00Core[aintCPUNo].pVecIti.X % 50 == 0 && psrcCPU00Core[aintCPUNo].pVecMovIti.X < 0f)
			{
				Game1.bakuhatu.psrcBakuhatuMov01CoreConboEnable(new Vector3(psrcCPU00Core[aintCPUNo].pVecIti.X + 256f, psrcCPU00Core[aintCPUNo].pVecIti.Y, psrcCPU00Core[aintCPUNo].pVecIti.Z), new Vector3(psrcCPU00Core[aintCPUNo].pVecMovIti.X * 1.5f, psrcCPU00Core[aintCPUNo].pVecMovIti.Y * 1.5f, psrcCPU00Core[aintCPUNo].pVecMovIti.Z), psrcCPU00Core[aintCPUNo].pfltItiR, 0);
			}
			psrcCPU00Core[aintCPUNo].pVecMovIti.X = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Y = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Z = 0f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intNormal;
			break;
		case enuCPU00MovState.intDashMove:
			psrcCPU00Core[aintCPUNo].pVecIti.X += (float)(int)psrcCPU00Core[aintCPUNo].pVecMovIti.X * 1.5f;
			psrcCPU00Core[aintCPUNo].pVecIti.Y += (float)(int)psrcCPU00Core[aintCPUNo].pVecMovIti.Y * 1.5f;
			psrcCPU00Core[aintCPUNo].pVecIti.Z += psrcCPU00Core[aintCPUNo].pVecMovIti.Z;
			psrcCPU00Core[aintCPUNo].pVecMovIti.X = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Y = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Z = 0f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intNormal;
			break;
		case enuCPU00MovState.intAttack:
			Game1.cPUTama.psrcCPUTamaCorePlayerItiEnable(psrcCPU00Core[aintCPUNo].pVecIti, new Vector3(0f, 0f, 3f / 128f), Game1.player.psrcPlayerCore.pVecIti);
			Game1.bakuhatu.psrcKemuriCoreConboEnable(psrcCPU00Core[aintCPUNo].pVecIti, psrcCPU00Core[aintCPUNo].pfltItiR, 0);
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intNormal;
			break;
		case enuCPU00MovState.intDamage:
			break;
		case enuCPU00MovState.intDead:
			KakuCPU00Init(aintCPUNo);
			break;
		}
	}

	private void CPU00TowerBack00Update(int aintCPUNo)
	{
		CPU00Normal00OpeUpdate(aintCPUNo);
		switch (psrcCPU00Core[aintCPUNo].enuMovState)
		{
		case enuCPU00MovState.intNormal:
			break;
		case enuCPU00MovState.intMove:
			if ((int)(psrcCPU00Core[aintCPUNo].pVecIti.Z * 100f) % 5 == 0 && psrcCPU00Core[aintCPUNo].pVecMovIti.Z != 0f)
			{
				Game1.bakuhatu.psrcBakuhatuMov01CoreConboEnable(new Vector3(psrcCPU00Core[aintCPUNo].pVecIti.X, psrcCPU00Core[aintCPUNo].pVecIti.Y + 192f, psrcCPU00Core[aintCPUNo].pVecIti.Z), new Vector3(psrcCPU00Core[aintCPUNo].pVecMovIti.X / 4f, psrcCPU00Core[aintCPUNo].pVecMovIti.Y / 8f, psrcCPU00Core[aintCPUNo].pVecMovIti.Z / 2f), 0f, 0);
			}
			psrcCPU00Core[aintCPUNo].pVecIti.X += psrcCPU00Core[aintCPUNo].pVecMovIti.X / 4f;
			psrcCPU00Core[aintCPUNo].pVecIti.Y += psrcCPU00Core[aintCPUNo].pVecMovIti.Y / 8f;
			psrcCPU00Core[aintCPUNo].pVecIti.Z += psrcCPU00Core[aintCPUNo].pVecMovIti.Z / 2f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.X = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Y = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Z = 0f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intNormal;
			break;
		case enuCPU00MovState.intDashMove:
			if ((int)(psrcCPU00Core[aintCPUNo].pVecIti.Z * 100f) % 5 == 0 && psrcCPU00Core[aintCPUNo].pVecMovIti.Z != 0f)
			{
				Game1.bakuhatu.psrcBakuhatuMov01CoreConboEnable(new Vector3(psrcCPU00Core[aintCPUNo].pVecIti.X, psrcCPU00Core[aintCPUNo].pVecIti.Y + 192f, psrcCPU00Core[aintCPUNo].pVecIti.Z), new Vector3(psrcCPU00Core[aintCPUNo].pVecMovIti.X / 4f, psrcCPU00Core[aintCPUNo].pVecMovIti.Y / 8f, psrcCPU00Core[aintCPUNo].pVecMovIti.Z / 2f), 0f, 0);
			}
			psrcCPU00Core[aintCPUNo].pVecIti.X += psrcCPU00Core[aintCPUNo].pVecMovIti.X / 4f;
			psrcCPU00Core[aintCPUNo].pVecIti.Y += psrcCPU00Core[aintCPUNo].pVecMovIti.Y / 8f;
			psrcCPU00Core[aintCPUNo].pVecIti.Z += psrcCPU00Core[aintCPUNo].pVecMovIti.Z / 2f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.X = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Y = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Z = 0f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intNormal;
			break;
		case enuCPU00MovState.intAttack:
			Game1.cPUTama.psrcCPUTamaCorePlayerItiEnable(psrcCPU00Core[aintCPUNo].pVecIti, new Vector3(0f, 0f, 3f / 128f), Game1.player.psrcPlayerCore.pVecIti);
			Game1.bakuhatu.psrcKemuriCoreConboEnable(psrcCPU00Core[aintCPUNo].pVecIti, psrcCPU00Core[aintCPUNo].pfltItiR, 0);
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intNormal;
			break;
		case enuCPU00MovState.intDamage:
			break;
		case enuCPU00MovState.intDead:
			KakuCPU00Init(aintCPUNo);
			break;
		}
	}

	private void CPU00TowerBack00_yUpdate(int aintCPUNo)
	{
		CPU00Normal00OpeUpdate(aintCPUNo);
		switch (psrcCPU00Core[aintCPUNo].enuMovState)
		{
		case enuCPU00MovState.intNormal:
			break;
		case enuCPU00MovState.intMove:
			psrcCPU00Core[aintCPUNo].pVecIti.X += psrcCPU00Core[aintCPUNo].pVecMovIti.X / 4f;
			psrcCPU00Core[aintCPUNo].pVecIti.Y += psrcCPU00Core[aintCPUNo].pVecMovIti.Y / 8f;
			psrcCPU00Core[aintCPUNo].pVecIti.Z += psrcCPU00Core[aintCPUNo].pVecMovIti.Z;
			if ((int)(psrcCPU00Core[aintCPUNo].pVecIti.Z * 100f) % 5 == 0 && psrcCPU00Core[aintCPUNo].pVecMovIti.Z != 0f)
			{
				Game1.bakuhatu.psrcBakuhatuMov01CoreConboEnable(new Vector3(psrcCPU00Core[aintCPUNo].pVecIti.X, psrcCPU00Core[aintCPUNo].pVecIti.Y + 192f, psrcCPU00Core[aintCPUNo].pVecIti.Z), new Vector3(psrcCPU00Core[aintCPUNo].pVecMovIti.X / 4f, psrcCPU00Core[aintCPUNo].pVecMovIti.Y / 8f, psrcCPU00Core[aintCPUNo].pVecMovIti.Z), 0f, 0);
			}
			psrcCPU00Core[aintCPUNo].pVecMovIti.X = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Y = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Z = 0f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intNormal;
			break;
		case enuCPU00MovState.intDashMove:
			psrcCPU00Core[aintCPUNo].pVecIti.X += psrcCPU00Core[aintCPUNo].pVecMovIti.X / 4f;
			psrcCPU00Core[aintCPUNo].pVecIti.Y += psrcCPU00Core[aintCPUNo].pVecMovIti.Y / 8f;
			psrcCPU00Core[aintCPUNo].pVecIti.Z += psrcCPU00Core[aintCPUNo].pVecMovIti.Z;
			if ((int)(psrcCPU00Core[aintCPUNo].pVecIti.Z * 100f) % 5 == 0 && psrcCPU00Core[aintCPUNo].pVecMovIti.Z != 0f)
			{
				Game1.bakuhatu.psrcBakuhatuMov01CoreConboEnable(new Vector3(psrcCPU00Core[aintCPUNo].pVecIti.X, psrcCPU00Core[aintCPUNo].pVecIti.Y + 192f, psrcCPU00Core[aintCPUNo].pVecIti.Z), new Vector3(psrcCPU00Core[aintCPUNo].pVecMovIti.X / 4f, psrcCPU00Core[aintCPUNo].pVecMovIti.Y / 8f, psrcCPU00Core[aintCPUNo].pVecMovIti.Z), 0f, 0);
			}
			psrcCPU00Core[aintCPUNo].pVecMovIti.X = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Y = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Z = 0f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intNormal;
			break;
		case enuCPU00MovState.intAttack:
			Game1.cPUTama.psrcCPUTamaCorePlayerItiEnable(psrcCPU00Core[aintCPUNo].pVecIti, new Vector3(0f, 0f, 3f / 128f), Game1.player.psrcPlayerCore.pVecIti);
			Game1.bakuhatu.psrcKemuriCoreConboEnable(psrcCPU00Core[aintCPUNo].pVecIti, psrcCPU00Core[aintCPUNo].pfltItiR, 0);
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intNormal;
			break;
		case enuCPU00MovState.intDamage:
			break;
		case enuCPU00MovState.intDead:
			KakuCPU00Init(aintCPUNo);
			break;
		}
	}

	private void CPU00Houdai00Update(int aintCPUNo)
	{
		CPU00Normal00OpeUpdate(aintCPUNo);
		switch (psrcCPU00Core[aintCPUNo].enuMovState)
		{
		case enuCPU00MovState.intNormal:
			break;
		case enuCPU00MovState.intMove:
			psrcCPU00Core[aintCPUNo].pVecIti.X += psrcCPU00Core[aintCPUNo].pVecMovIti.X;
			psrcCPU00Core[aintCPUNo].pVecIti.Y += psrcCPU00Core[aintCPUNo].pVecMovIti.Y;
			psrcCPU00Core[aintCPUNo].pVecIti.Z += psrcCPU00Core[aintCPUNo].pVecMovIti.Z;
			psrcCPU00Core[aintCPUNo].pVecMovIti.X = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Y = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Z = 0f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intNormal;
			break;
		case enuCPU00MovState.intDashMove:
			break;
		case enuCPU00MovState.intAttack:
			Game1.cPUTama.psrcCPUTamaCorePlayerItiEnable(new Vector3(psrcCPU00Core[aintCPUNo].pVecIti.X, psrcCPU00Core[aintCPUNo].pVecIti.Y - 120f, psrcCPU00Core[aintCPUNo].pVecIti.Z), new Vector3(0f, 0f, 3f / 128f), Game1.player.psrcPlayerCore.pVecIti);
			Game1.bakuhatu.psrcKemuriCoreConboEnable(new Vector3(psrcCPU00Core[aintCPUNo].pVecIti.X, psrcCPU00Core[aintCPUNo].pVecIti.Y - 120f, psrcCPU00Core[aintCPUNo].pVecIti.Z), psrcCPU00Core[aintCPUNo].pfltItiR, 0);
			break;
		case enuCPU00MovState.intDamage:
			break;
		case enuCPU00MovState.intDead:
			KakuCPU00Init(aintCPUNo);
			break;
		}
	}

	private void CPU00Plane00Update(int aintCPUNo)
	{
		CPU00Normal00OpeUpdate(aintCPUNo);
		switch (psrcCPU00Core[aintCPUNo].enuMovState)
		{
		case enuCPU00MovState.intNormal:
			break;
		case enuCPU00MovState.intMove:
			psrcCPU00Core[aintCPUNo].pVecIti.X += psrcCPU00Core[aintCPUNo].pVecMovIti.X;
			psrcCPU00Core[aintCPUNo].pVecIti.Y += psrcCPU00Core[aintCPUNo].pVecMovIti.Y / 2f;
			psrcCPU00Core[aintCPUNo].pVecIti.Z += psrcCPU00Core[aintCPUNo].pVecMovIti.Z;
			psrcCPU00Core[aintCPUNo].pVecMovIti.X = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Y = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Z = 0f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intNormal;
			break;
		case enuCPU00MovState.intDashMove:
			break;
		case enuCPU00MovState.intAttack:
			Game1.cPUTama.psrcCPUTamaCorePlayerItiEnable(psrcCPU00Core[aintCPUNo].pVecIti, new Vector3(0f, 0f, 3f / 128f), Game1.player.psrcPlayerCore.pVecIti);
			Game1.bakuhatu.psrcKemuriCoreConboEnable(psrcCPU00Core[aintCPUNo].pVecIti, psrcCPU00Core[aintCPUNo].pfltItiR, 0);
			break;
		case enuCPU00MovState.intDamage:
			break;
		case enuCPU00MovState.intDead:
			KakuCPU00Init(aintCPUNo);
			break;
		}
	}

	private void CPU00TowerFront00Update(int aintCPUNo)
	{
		CPU00Normal00OpeUpdate(aintCPUNo);
		switch (psrcCPU00Core[aintCPUNo].enuMovState)
		{
		case enuCPU00MovState.intNormal:
			break;
		case enuCPU00MovState.intMove:
			if ((int)(psrcCPU00Core[aintCPUNo].pVecIti.Z * 100f) % 5 == 0 && psrcCPU00Core[aintCPUNo].pVecMovIti.Z != 0f)
			{
				Game1.bakuhatu.psrcBakuhatuMov01CoreConboEnable(new Vector3(psrcCPU00Core[aintCPUNo].pVecIti.X, psrcCPU00Core[aintCPUNo].pVecIti.Y - 192f, psrcCPU00Core[aintCPUNo].pVecIti.Z - 1f / 64f), new Vector3(psrcCPU00Core[aintCPUNo].pVecMovIti.X / 4f, psrcCPU00Core[aintCPUNo].pVecMovIti.Y / 8f, psrcCPU00Core[aintCPUNo].pVecMovIti.Z / 2f), 180f, 0);
			}
			psrcCPU00Core[aintCPUNo].pVecIti.X += psrcCPU00Core[aintCPUNo].pVecMovIti.X / 4f;
			psrcCPU00Core[aintCPUNo].pVecIti.Y += psrcCPU00Core[aintCPUNo].pVecMovIti.Y / 8f;
			psrcCPU00Core[aintCPUNo].pVecIti.Z += psrcCPU00Core[aintCPUNo].pVecMovIti.Z / 2f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.X = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Y = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Z = 0f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intNormal;
			break;
		case enuCPU00MovState.intDashMove:
			psrcCPU00Core[aintCPUNo].pVecIti.X += psrcCPU00Core[aintCPUNo].pVecMovIti.X / 4f;
			psrcCPU00Core[aintCPUNo].pVecIti.Y += psrcCPU00Core[aintCPUNo].pVecMovIti.Y / 8f;
			psrcCPU00Core[aintCPUNo].pVecIti.Z += psrcCPU00Core[aintCPUNo].pVecMovIti.Z / 2f;
			if ((int)(psrcCPU00Core[aintCPUNo].pVecIti.Z * 100f) % 5 == 0 && psrcCPU00Core[aintCPUNo].pVecMovIti.Z != 0f)
			{
				Game1.bakuhatu.psrcBakuhatuMov01CoreConboEnable(new Vector3(psrcCPU00Core[aintCPUNo].pVecIti.X, psrcCPU00Core[aintCPUNo].pVecIti.Y - 192f, psrcCPU00Core[aintCPUNo].pVecIti.Z - 1f / 64f), new Vector3(psrcCPU00Core[aintCPUNo].pVecMovIti.X / 4f, psrcCPU00Core[aintCPUNo].pVecMovIti.Y / 8f, psrcCPU00Core[aintCPUNo].pVecMovIti.Z / 2f), 180f, 0);
			}
			psrcCPU00Core[aintCPUNo].pVecMovIti.X = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Y = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Z = 0f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intNormal;
			break;
		case enuCPU00MovState.intAttack:
			Game1.cPUTama.psrcCPUTamaCorePlayerItiEnable(psrcCPU00Core[aintCPUNo].pVecIti, new Vector3(0f, 0f, 3f / 128f), Game1.player.psrcPlayerCore.pVecIti);
			Game1.bakuhatu.psrcKemuriCoreConboEnable(psrcCPU00Core[aintCPUNo].pVecIti, psrcCPU00Core[aintCPUNo].pfltItiR, 0);
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intNormal;
			break;
		case enuCPU00MovState.intDamage:
			break;
		case enuCPU00MovState.intDead:
			KakuCPU00Init(aintCPUNo);
			break;
		}
	}

	private void CPU00TowerFront00_yUpdate(int aintCPUNo)
	{
		CPU00Normal00OpeUpdate(aintCPUNo);
		switch (psrcCPU00Core[aintCPUNo].enuMovState)
		{
		case enuCPU00MovState.intNormal:
			break;
		case enuCPU00MovState.intMove:
			if ((int)(psrcCPU00Core[aintCPUNo].pVecIti.Z * 100f) % 5 == 0 && psrcCPU00Core[aintCPUNo].pVecMovIti.Z != 0f)
			{
				Game1.bakuhatu.psrcBakuhatuMov01CoreConboEnable(new Vector3(psrcCPU00Core[aintCPUNo].pVecIti.X, psrcCPU00Core[aintCPUNo].pVecIti.Y - 192f, psrcCPU00Core[aintCPUNo].pVecIti.Z - 1f / 64f), new Vector3(psrcCPU00Core[aintCPUNo].pVecMovIti.X / 4f, psrcCPU00Core[aintCPUNo].pVecMovIti.Y / 8f, psrcCPU00Core[aintCPUNo].pVecMovIti.Z), 180f, 0);
			}
			psrcCPU00Core[aintCPUNo].pVecIti.X += psrcCPU00Core[aintCPUNo].pVecMovIti.X / 4f;
			psrcCPU00Core[aintCPUNo].pVecIti.Y += psrcCPU00Core[aintCPUNo].pVecMovIti.Y / 8f;
			psrcCPU00Core[aintCPUNo].pVecIti.Z += psrcCPU00Core[aintCPUNo].pVecMovIti.Z;
			psrcCPU00Core[aintCPUNo].pVecMovIti.X = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Y = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Z = 0f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intNormal;
			break;
		case enuCPU00MovState.intDashMove:
			psrcCPU00Core[aintCPUNo].pVecIti.X += psrcCPU00Core[aintCPUNo].pVecMovIti.X / 4f;
			psrcCPU00Core[aintCPUNo].pVecIti.Y += psrcCPU00Core[aintCPUNo].pVecMovIti.Y / 8f;
			psrcCPU00Core[aintCPUNo].pVecIti.Z += psrcCPU00Core[aintCPUNo].pVecMovIti.Z;
			psrcCPU00Core[aintCPUNo].pVecMovIti.X = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Y = 0f;
			psrcCPU00Core[aintCPUNo].pVecMovIti.Z = 0f;
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intNormal;
			break;
		case enuCPU00MovState.intAttack:
			Game1.cPUTama.psrcCPUTamaCorePlayerItiEnable(psrcCPU00Core[aintCPUNo].pVecIti, new Vector3(0f, 0f, 3f / 128f), Game1.player.psrcPlayerCore.pVecIti);
			Game1.bakuhatu.psrcKemuriCoreConboEnable(psrcCPU00Core[aintCPUNo].pVecIti, psrcCPU00Core[aintCPUNo].pfltItiR, 0);
			psrcCPU00Core[aintCPUNo].enuMovState = enuCPU00MovState.intNormal;
			break;
		case enuCPU00MovState.intDamage:
			break;
		case enuCPU00MovState.intDead:
			KakuCPU00Init(aintCPUNo);
			break;
		}
	}

	public bool pCPU00TamaHantei(Vector3 avecIti, float afltHabaHantei, Rectangle arecHantei)
	{
		bool result = false;
		Rectangle rectangle = new Rectangle
		{
			X = arecHantei.X + (int)avecIti.X,
			Y = arecHantei.Y + (int)avecIti.Y,
			Width = arecHantei.Width,
			Height = arecHantei.Height
		};
		for (int i = 0; i < psrcCPU00Core.Length; i++)
		{
			if (!psrcCPU00Core[i].pflgEnable || ((!(avecIti.Z <= psrcCPU00Core[i].pVecIti.Z) || !(avecIti.Z + afltHabaHantei >= psrcCPU00Core[i].pVecIti.Z)) && (!(avecIti.Z <= psrcCPU00Core[i].pVecIti.Z + fltOffSetHaba[(int)psrcCPU00Core[i].penuType]) || !(avecIti.Z + afltHabaHantei >= psrcCPU00Core[i].pVecIti.Z + fltOffSetHaba[(int)psrcCPU00Core[i].penuType]))))
			{
				continue;
			}
			for (int j = 0; j < precCPU00OffSet.GetUpperBound(1) && (precCPU00OffSet[(int)psrcCPU00Core[i].penuImgState[0], j].X != 0 || precCPU00OffSet[(int)psrcCPU00Core[i].penuImgState[0], j].Y != 0 || precCPU00OffSet[(int)psrcCPU00Core[i].penuImgState[0], j].Width != 0 || precCPU00OffSet[(int)psrcCPU00Core[i].penuImgState[0], j].Height != 0); j++)
			{
				if (rectangle.Intersects(new Rectangle((int)psrcCPU00Core[i].pVecIti.X + precCPU00OffSet[(int)psrcCPU00Core[i].penuImgState[0], j].X, (int)psrcCPU00Core[i].pVecIti.Y + precCPU00OffSet[(int)psrcCPU00Core[i].penuImgState[0], j].Y, precCPU00OffSet[(int)psrcCPU00Core[i].penuImgState[0], j].Width, precCPU00OffSet[(int)psrcCPU00Core[i].penuImgState[0], j].Height)))
				{
					Game1.bGM.pflgSEBakuhatu[1] = true;
					Game1.score.pScoreUp(lngScoreUp[(int)psrcCPU00Core[i].penuType]);
					if (psrcCPU00Core[i].penuType != enuCPU00Type.intQueenChildBee00 && psrcCPU00Core[i].penuType != enuCPU00Type.intQueenChildBeeX00)
					{
						Game1.cPUPort00.pCpuGekihaCount();
					}
					return flgCPU00Dead(i);
				}
			}
		}
		return result;
	}

	private bool flgCPU00Dead(int aintCPUNo)
	{
		bool result = true;
		Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(psrcCPU00Core[aintCPUNo].pVecIti, psrcCPU00Core[aintCPUNo].pfltItiR, 0);
		KakuCPU00Init(aintCPUNo);
		return result;
	}

	public bool pCPU00PlayerHantei(Vector3 avecIti, float afltHabaHantei, Rectangle arecHantei)
	{
		bool result = false;
		Rectangle rectangle = new Rectangle
		{
			X = arecHantei.X + (int)avecIti.X,
			Y = arecHantei.Y + (int)avecIti.Y,
			Width = arecHantei.Width,
			Height = arecHantei.Height
		};
		for (int i = 0; i < psrcCPU00Core.Length; i++)
		{
			if (!psrcCPU00Core[i].pflgEnable || ((!(avecIti.Z <= psrcCPU00Core[i].pVecIti.Z) || !(avecIti.Z + afltHabaHantei >= psrcCPU00Core[i].pVecIti.Z)) && (!(avecIti.Z <= psrcCPU00Core[i].pVecIti.Z + fltOffSetHaba[(int)psrcCPU00Core[i].penuType]) || !(avecIti.Z + afltHabaHantei >= psrcCPU00Core[i].pVecIti.Z + fltOffSetHaba[(int)psrcCPU00Core[i].penuType]))))
			{
				continue;
			}
			for (int j = 0; j < precCPU00OffSet.GetUpperBound(1) && (precCPU00OffSet[(int)psrcCPU00Core[i].penuImgState[0], j].X != 0 || precCPU00OffSet[(int)psrcCPU00Core[i].penuImgState[0], j].Y != 0 || precCPU00OffSet[(int)psrcCPU00Core[i].penuImgState[0], j].Width != 0 || precCPU00OffSet[(int)psrcCPU00Core[i].penuImgState[0], j].Height != 0); j++)
			{
				if (rectangle.Intersects(new Rectangle((int)psrcCPU00Core[i].pVecIti.X + precCPU00OffSet[(int)psrcCPU00Core[i].penuImgState[0], j].X, (int)psrcCPU00Core[i].pVecIti.Y + precCPU00OffSet[(int)psrcCPU00Core[i].penuImgState[0], j].Y, precCPU00OffSet[(int)psrcCPU00Core[i].penuImgState[0], j].Width, precCPU00OffSet[(int)psrcCPU00Core[i].penuImgState[0], j].Height)))
				{
					result = true;
					Game1.bakuhatu.psrcHitCoreConboEnable(psrcCPU00Core[i].pVecIti, 0f, 0);
					return result;
				}
			}
		}
		return result;
	}

	protected override void LoadContent()
	{
		pimgCPU00[0] = null;
		pimgCPU00[1] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPU\\TekiSize01");
		pimgCPU00[2] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPU\\Tower01");
		pimgCPU00[3] = pimgCPU00[2];
		pimgCPU00[4] = pimgCPU00[2];
		pimgCPU00[5] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPU\\TowerBack02");
		pimgCPU00[6] = pimgCPU00[5];
		pimgCPU00[7] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPU\\Houdai01");
		pimgCPU00[8] = pimgCPU00[7];
		pimgCPU00[9] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPU\\Invader01");
		pimgCPU00[10] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPU\\Invader02");
		pimgCPU00[11] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPU\\Plane07");
		pimgCPU00[12] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPU\\Bee09");
		pimgCPU00[13] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPU\\Bee10");
		pimgCPU00[14] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPU\\TowerFront03");
		pimgCPU00[15] = pimgCPU00[14];
		pimgCPU00[16] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPU\\Tower01_y");
		pimgCPU00[17] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPU\\Tower01_r");
		pimgCPU00[18] = pimgCPU00[16];
		pimgCPU00[19] = pimgCPU00[17];
		pimgCPU00[20] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPU\\TowerBack02_y");
		pimgCPU00[21] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPU\\TowerBack02_r");
		pimgCPU00[22] = pimgCPU00[20];
		pimgCPU00[23] = pimgCPU00[21];
		pimgCPU00[24] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPU\\TowerFront03_y");
		pimgCPU00[25] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPU\\TowerFront03_r");
		pimgCPU00[26] = pimgCPU00[24];
		pimgCPU00[27] = pimgCPU00[25];
		pimgCPU00[28] = pimgCPU00[16];
		pimgCPU00[29] = pimgCPU00[17];
		pimgCPU00[30] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPU\\Beetle06");
		pimgCPU00[31] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPU\\Beetle07");
		pimgCPU00[32] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPU\\Tonbo06");
		pimgCPU00[33] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPU\\Tonbo07");
		base.LoadContent();
	}

	public void pImageStateUpdate()
	{
		for (int i = 0; i < psrcCPU00Core.Length; i++)
		{
			if (psrcCPU00Core[i].pflgEnable)
			{
				for (int j = 0; j < psrcCPU00Core[i].penuImgState.Length - 1; j++)
				{
					psrcCPU00Core[i].penuImgState[j] = psrcCPU00Core[i].penuImgState[j + 1];
				}
			}
		}
	}

	private void NomarlStartStateSet(int aintCPUNo)
	{
		enuCPU00ImgState enuCPU00ImgState2 = psrcCPU00Core[aintCPUNo].penuType switch
		{
			enuCPU00Type.intNormal00 => enuCPU00ImgState.intNormal00, 
			enuCPU00Type.intNormal01 => enuCPU00ImgState.intNormal00, 
			enuCPU00Type.intTower00 => enuCPU00ImgState.intTower00, 
			enuCPU00Type.intTower00R90 => enuCPU00ImgState.intTower00R90, 
			enuCPU00Type.intTower00R270 => enuCPU00ImgState.intTower00R270, 
			enuCPU00Type.intTowerBack00 => enuCPU00ImgState.intTowerBack00, 
			enuCPU00Type.intTowerBackX00 => enuCPU00ImgState.intTowerBackX00, 
			enuCPU00Type.intHoudai00 => enuCPU00ImgState.intHoudai00, 
			enuCPU00Type.intHoudaiX00 => enuCPU00ImgState.intHoudaiX00, 
			enuCPU00Type.intInvader00 => enuCPU00ImgState.intInvader00, 
			enuCPU00Type.intInvaderX00 => enuCPU00ImgState.intInvaderX00, 
			enuCPU00Type.intPlane00 => enuCPU00ImgState.intPlane00, 
			enuCPU00Type.intBee00 => enuCPU00ImgState.intBee00, 
			enuCPU00Type.intBeeX00 => enuCPU00ImgState.intBee01, 
			enuCPU00Type.intTowerFront00 => enuCPU00ImgState.intTowerFront00, 
			enuCPU00Type.intTowerFrontX00 => enuCPU00ImgState.intTowerFrontX00, 
			enuCPU00Type.intTower00_y => enuCPU00ImgState.intTower00_y, 
			enuCPU00Type.intTower00_r => enuCPU00ImgState.intTower00_r, 
			enuCPU00Type.intTower00R90_y => enuCPU00ImgState.intTower00R90_y, 
			enuCPU00Type.intTower00R90_r => enuCPU00ImgState.intTower00R90_r, 
			enuCPU00Type.intTowerBack00_y => enuCPU00ImgState.intTowerBack00_y, 
			enuCPU00Type.intTowerBack00_r => enuCPU00ImgState.intTowerBack00_r, 
			enuCPU00Type.intTowerBackX00_y => enuCPU00ImgState.intTowerBack00X_y, 
			enuCPU00Type.intTowerBackX00_r => enuCPU00ImgState.intTowerBack00X_r, 
			enuCPU00Type.intTowerFront00_y => enuCPU00ImgState.intTowerFront00_y, 
			enuCPU00Type.intTowerFront00_r => enuCPU00ImgState.intTowerFront00_r, 
			enuCPU00Type.intTowerFrontX00_y => enuCPU00ImgState.intTowerFront00X_y, 
			enuCPU00Type.intTowerFrontX00_r => enuCPU00ImgState.intTowerFront00X_r, 
			enuCPU00Type.intTower00R270_y => enuCPU00ImgState.intTower00R270_y, 
			enuCPU00Type.intTower00R270_r => enuCPU00ImgState.intTower00R270_r, 
			enuCPU00Type.intBeetle00 => enuCPU00ImgState.intBeetle00, 
			enuCPU00Type.intBeetleX00 => enuCPU00ImgState.intBeetle00, 
			enuCPU00Type.intTonbo00 => enuCPU00ImgState.intTonbo00, 
			enuCPU00Type.intTonboX00 => enuCPU00ImgState.intTonbo00, 
			_ => enuCPU00ImgState.intNormal00, 
		};
		for (int i = 0; i < psrcCPU00Core[aintCPUNo].penuImgState.Length - 1; i++)
		{
			psrcCPU00Core[aintCPUNo].penuImgState[i] = enuCPU00ImgState2;
		}
	}

	private void NomarlStateSet(int aintCPUNo)
	{
		if (psrcCPU00Core[aintCPUNo].penuImgState[0] == enuCPU00ImgState.intKieru)
		{
			NomarlStartStateSet(aintCPUNo);
		}
	}

	private void Bee00StartStateSet(int aintCPUNo)
	{
		psrcCPU00Core[aintCPUNo].penuImgState[0] = enuCPU00ImgState.intBee00;
		psrcCPU00Core[aintCPUNo].penuImgState[1] = enuCPU00ImgState.intBee00;
		psrcCPU00Core[aintCPUNo].penuImgState[2] = enuCPU00ImgState.intBee00;
		psrcCPU00Core[aintCPUNo].penuImgState[3] = enuCPU00ImgState.intBee00;
		psrcCPU00Core[aintCPUNo].penuImgState[4] = enuCPU00ImgState.intBee00;
		psrcCPU00Core[aintCPUNo].penuImgState[5] = enuCPU00ImgState.intBee00;
		psrcCPU00Core[aintCPUNo].penuImgState[6] = enuCPU00ImgState.intBee00;
		psrcCPU00Core[aintCPUNo].penuImgState[7] = enuCPU00ImgState.intBee00;
		psrcCPU00Core[aintCPUNo].penuImgState[8] = enuCPU00ImgState.intBee01;
		psrcCPU00Core[aintCPUNo].penuImgState[9] = enuCPU00ImgState.intBee01;
		psrcCPU00Core[aintCPUNo].penuImgState[10] = enuCPU00ImgState.intBee01;
		psrcCPU00Core[aintCPUNo].penuImgState[11] = enuCPU00ImgState.intBee01;
		psrcCPU00Core[aintCPUNo].penuImgState[12] = enuCPU00ImgState.intBee01;
		psrcCPU00Core[aintCPUNo].penuImgState[13] = enuCPU00ImgState.intBee01;
		psrcCPU00Core[aintCPUNo].penuImgState[14] = enuCPU00ImgState.intBee01;
		psrcCPU00Core[aintCPUNo].penuImgState[15] = enuCPU00ImgState.intBee01;
	}

	private void Bee00StateSet(int aintCPUNo)
	{
		if (psrcCPU00Core[aintCPUNo].penuImgState[0] == enuCPU00ImgState.intKieru)
		{
			Bee00StartStateSet(aintCPUNo);
		}
	}

	private void Beetle00StartStateSet(int aintCPUNo)
	{
		psrcCPU00Core[aintCPUNo].penuImgState[0] = enuCPU00ImgState.intBeetle00;
		psrcCPU00Core[aintCPUNo].penuImgState[1] = enuCPU00ImgState.intBeetle00;
		psrcCPU00Core[aintCPUNo].penuImgState[2] = enuCPU00ImgState.intBeetle00;
		psrcCPU00Core[aintCPUNo].penuImgState[3] = enuCPU00ImgState.intBeetle00;
		psrcCPU00Core[aintCPUNo].penuImgState[4] = enuCPU00ImgState.intBeetle00;
		psrcCPU00Core[aintCPUNo].penuImgState[5] = enuCPU00ImgState.intBeetle00;
		psrcCPU00Core[aintCPUNo].penuImgState[6] = enuCPU00ImgState.intBeetle01;
		psrcCPU00Core[aintCPUNo].penuImgState[7] = enuCPU00ImgState.intBeetle01;
		psrcCPU00Core[aintCPUNo].penuImgState[8] = enuCPU00ImgState.intBeetle01;
		psrcCPU00Core[aintCPUNo].penuImgState[9] = enuCPU00ImgState.intBeetle01;
		psrcCPU00Core[aintCPUNo].penuImgState[10] = enuCPU00ImgState.intBeetle01;
		psrcCPU00Core[aintCPUNo].penuImgState[11] = enuCPU00ImgState.intBeetle01;
	}

	private void Beetle00StateSet(int aintCPUNo)
	{
		if (psrcCPU00Core[aintCPUNo].penuImgState[0] == enuCPU00ImgState.intKieru)
		{
			Beetle00StartStateSet(aintCPUNo);
		}
	}

	private void Tonbo00StartStateSet(int aintCPUNo)
	{
		psrcCPU00Core[aintCPUNo].penuImgState[0] = enuCPU00ImgState.intTonbo00;
		psrcCPU00Core[aintCPUNo].penuImgState[1] = enuCPU00ImgState.intTonbo00;
		psrcCPU00Core[aintCPUNo].penuImgState[2] = enuCPU00ImgState.intTonbo00;
		psrcCPU00Core[aintCPUNo].penuImgState[3] = enuCPU00ImgState.intTonbo00;
		psrcCPU00Core[aintCPUNo].penuImgState[4] = enuCPU00ImgState.intTonbo00;
		psrcCPU00Core[aintCPUNo].penuImgState[5] = enuCPU00ImgState.intTonbo00;
		psrcCPU00Core[aintCPUNo].penuImgState[6] = enuCPU00ImgState.intTonbo01;
		psrcCPU00Core[aintCPUNo].penuImgState[7] = enuCPU00ImgState.intTonbo01;
		psrcCPU00Core[aintCPUNo].penuImgState[8] = enuCPU00ImgState.intTonbo01;
		psrcCPU00Core[aintCPUNo].penuImgState[9] = enuCPU00ImgState.intTonbo01;
		psrcCPU00Core[aintCPUNo].penuImgState[10] = enuCPU00ImgState.intTonbo01;
		psrcCPU00Core[aintCPUNo].penuImgState[11] = enuCPU00ImgState.intTonbo01;
	}

	private void Tonbo00StateSet(int aintCPUNo)
	{
		if (psrcCPU00Core[aintCPUNo].penuImgState[0] == enuCPU00ImgState.intKieru)
		{
			Tonbo00StartStateSet(aintCPUNo);
		}
	}

	public void pCPU00Draw(SpriteBatch aspritesBatch)
	{
		if (pimgCPU00[1] == null)
		{
			return;
		}
		for (int i = 0; i < psrcCPU00Core.Length; i++)
		{
			if (psrcCPU00Core[i].pflgEnable && psrcCPU00Core[i].penuImgState[0] != enuCPU00ImgState.intKieru)
			{
				int width = pimgCPU00[(int)psrcCPU00Core[i].penuImgState[0]].Width;
				int height = pimgCPU00[(int)psrcCPU00Core[i].penuImgState[0]].Height;
				aspritesBatch.Draw(pimgCPU00[(int)psrcCPU00Core[i].penuImgState[0]], new Vector2(psrcCPU00Core[i].pVecIti.X * psrcCPU00Core[i].pVecIti.Z + 640f, psrcCPU00Core[i].pVecIti.Y * psrcCPU00Core[i].pVecIti.Z + 360f), null, Color.White, MathHelper.ToRadians(psrcCPU00Core[i].pfltItiR), new Vector2(width / 2, height / 2), new Vector2(psrcCPU00Core[i].pVecIti.Z, psrcCPU00Core[i].pVecIti.Z), psrcCPU00Core[i].pSpriteEffects, psrcCPU00Core[i].pVecIti.Z);
			}
		}
	}

	public override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
	}
}
