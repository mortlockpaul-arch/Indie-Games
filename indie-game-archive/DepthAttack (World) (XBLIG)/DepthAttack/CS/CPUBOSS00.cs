using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DepthAttack.CS;

public class CPUBOSS00 : DrawableGameComponent
{
	public struct srcCPUBOSS00Core
	{
		public bool pflgEnable;

		public int pintChildMax;

		public Vector3 pVecIti;

		public float pfltItiR;

		public Vector3 pVecMovIti;

		public Vector3 pVecMovDelta;

		public enuCPUBOSS00Type penuType;

		public float pfltHP;

		public float pfltDF;

		public float pfltDamage;

		public enuCPUBOSS00ImgState[] penuImgState;

		public SpriteEffects pSpriteEffects;

		public enuCPUBOSS00ImgStateHoji penuImgStateHoji;

		public structCPUBOSS00AI[] psrcAI;

		public enuCPUBOSS00MovState enuMovState;
	}

	public enum enuCPUBOSS00ImgState
	{
		intKieru,
		intOSPREY00,
		intOSPREYDamage00,
		intDragonHeadFront,
		intDragonHeadBack,
		intDragonHeadDamage,
		intHatiNoSu,
		intHatiNoSuDamage,
		intQueenBee00,
		intQueenBee01,
		intQueenBeeDamage00,
		intQueenBeeDamage01,
		intArshipTate,
		intArshipTateDamage,
		intArship,
		intArshipDamage,
		intArshipTateX,
		intArshipTateDamageX,
		intArshipX,
		intArshipDamageX,
		intESP,
		intESPDamage,
		intESPMove,
		intESPMoveDamage
	}

	public enum enuCPUBOSS00Type
	{
		intNormal00,
		intOSPREY00,
		intDragon,
		intHatiNosu,
		intQueenBee,
		intArshipTate,
		intArship,
		intESP
	}

	public enum enuCPUBOSS00ImgStateHoji
	{
		intNone,
		intFront,
		intBack,
		intRight,
		intLeft,
		intMove
	}

	public struct srcCPUBOSSChild00Core
	{
		public bool pflgEnable;

		public Vector3 pVecIti;

		public float pfltItiR;

		public Vector3 pVecMovIti;

		public Vector3 pVecMovDelta;

		public enuCPUBOSSChild00Type penuType;

		public float pintHP;

		public float pintDF;

		public enuCPUBOSSChild00ImgState[] penuImgState;

		public SpriteEffects pSpriteEffects;

		public structCPUBOSS00AI[] psrcAI;

		public enuCPUBOSSChild00MovState enuMovState;
	}

	public enum enuCPUBOSSChild00ImgState
	{
		intKieru,
		intOSPREYChild00,
		intOSPREYChildX00,
		intOSPREYChildDamage,
		intDragonBody,
		intDragonBodyDamage
	}

	public enum enuCPUBOSSChild00Type
	{
		intNormal00,
		intOSPREYChild00,
		intOSPREYChildX00,
		intDragonBody
	}

	public struct structCPUBOSS00AI
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
		Attack,
		SPAttack
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

	public enum enuCPUBOSS00MovState
	{
		intNormal,
		intMove,
		intDashMove,
		intAttack,
		intSPAttack,
		intDamage,
		intDead
	}

	public enum enuCPUBOSSChild00MovState
	{
		intNormal,
		intMove,
		intDashMove,
		intAttack,
		intDamage,
		intDead
	}

	private const string cstrCPUBOSS00_OSPREY00 = "PNG\\Character\\CPUBOSS\\OSPREY08";

	private const string cstrCPUBOSS00_OSPREYDamage00 = "PNG\\Character\\CPUBOSS\\OSPREY_Damage00";

	private const string cstrCPUBOSS00_DragonHeadFront = "PNG\\Character\\CPUBOSS\\DragonHead_Bo09";

	private const string cstrCPUBOSS00_DragonHeadBack = "PNG\\Character\\CPUBOSS\\DragonHead_Bo_Back03";

	private const string cstrCPUBOSS00_DragonHeadDamage = "PNG\\Character\\CPUBOSS\\DragonHead_Damage00";

	private const string cstrCPUBOSS00_HatiNosu = "PNG\\Character\\CPUBOSS\\HatiNoSu00";

	private const string cstrCPUBOSS00_HatiNosuDamage = "PNG\\Character\\CPUBOSS\\HatiNoSuDamage00";

	private const string cstrCPUBOSS00_QueenBee00 = "PNG\\Character\\CPUBOSS\\QueenBee04";

	private const string cstrCPUBOSS00_QueenBee00Damage = "PNG\\Character\\CPUBOSS\\QueenBeeDamage02";

	private const string cstrCPUBOSS00_QueenBee01 = "PNG\\Character\\CPUBOSS\\QueenBee05";

	private const string cstrCPUBOSS00_QueenBee01Damage = "PNG\\Character\\CPUBOSS\\QueenBeeDamage03";

	private const string cstrCPUBOSS00_ArshipTate = "PNG\\Character\\CPUBOSS\\ArshipG04";

	private const string cstrCPUBOSS00_ArshipTateDamage = "PNG\\Character\\CPUBOSS\\ArshipG_DM01";

	private const string cstrCPUBOSS00_Arship = "PNG\\Character\\CPUBOSS\\Arship11";

	private const string cstrCPUBOSS00_ArshipDamage = "PNG\\Character\\CPUBOSS\\Arship_DM09";

	private const string cstrCPUBOSS00_ESP = "PNG\\Character\\CPUBOSS\\ESP08";

	private const string cstrCPUBOSS00_ESPDamage = "PNG\\Character\\CPUBOSS\\ESP_DM08";

	private const string cstrCPUBOSS00_ESPMove = "PNG\\Character\\CPUBOSS\\ESP_Move00";

	private const string cstrCPUBOSS00_ESPMoveDamage = "PNG\\Character\\CPUBOSS\\ESP_Move_DM00";

	private const string cstrCPUBOSS00_OSPREYChild00 = "PNG\\Character\\CPUBOSS\\OSPREY_Child01";

	private const string cstrCPUBOSS00_OSPREYChildDamage00 = "PNG\\Character\\CPUBOSS\\OSPREY_Child_Damage00";

	private const string cstrCPUBOSS00_DragonBody = "PNG\\Character\\CPUBOSS\\DragonBody09";

	private const string cstrCPUBOSS00_DragonBodyDamage = "PNG\\Character\\CPUBOSS\\DragonBodyDamage00";

	private Random rnd = new Random();

	public srcCPUBOSS00Core[] psrcCPUBOSS00Core = new srcCPUBOSS00Core[2];

	public Texture2D[] pimgCPUBOSS00 = new Texture2D[24];

	public srcCPUBOSSChild00Core[] psrcCPUBOSSChild00Core = new srcCPUBOSSChild00Core[16];

	public Texture2D[] pimgCPUBOSSChild00 = new Texture2D[6];

	public long[] lngScoreUp = new long[8] { 0L, 50000L, 100000L, 25000L, 100000L, 25000L, 400000L, 1000000L };

	public float[] fltOffSetHaba = new float[8]
	{
		0f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f,
		3f / 128f
	};

	public float[] fltChildOffSetHaba = new float[4]
	{
		0f,
		3f / 128f,
		3f / 128f,
		3f / 128f
	};

	public Rectangle[,] precCPUBOSS00OffSet = new Rectangle[24, 4]
	{
		{
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-120, -30, 239, 135),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-120, -30, 239, 135),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-74, -121, 161, 160),
			new Rectangle(-93, -32, 189, 151),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-74, -121, 161, 160),
			new Rectangle(-93, -32, 189, 151),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-74, -121, 161, 160),
			new Rectangle(-93, -32, 189, 151),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-243, -188, 493, 264),
			new Rectangle(-199, 79, 413, 135),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-243, -188, 493, 264),
			new Rectangle(-199, 79, 413, 135),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-56, -108, 114, 226),
			new Rectangle(-122, -106, 243, 102),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-56, -108, 114, 226),
			new Rectangle(-122, -32, 240, 114),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-56, -108, 114, 226),
			new Rectangle(-122, -106, 243, 102),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-56, -108, 114, 226),
			new Rectangle(-122, -32, 240, 114),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-252, -84, 500, 179),
			new Rectangle(-95, 97, 159, 25),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-252, -84, 500, 179),
			new Rectangle(-95, 97, 159, 25),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-155, -77, 317, 125),
			new Rectangle(-39, 52, 99, 12),
			new Rectangle(162, -60, 25, 82),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-155, -77, 317, 125),
			new Rectangle(-39, 52, 99, 12),
			new Rectangle(162, -60, 25, 82),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-248, -83, 500, 178),
			new Rectangle(-65, 94, 158, 28),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-248, -83, 500, 178),
			new Rectangle(-65, 94, 158, 28),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-163, -74, 318, 123),
			new Rectangle(-58, 51, 88, 13),
			new Rectangle(-186, -59, 23, 81),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-163, -74, 318, 123),
			new Rectangle(-58, 51, 88, 13),
			new Rectangle(-186, -59, 23, 81),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-45, -151, 87, 266),
			new Rectangle(-76, -70, 153, 183),
			new Rectangle(-101, -8, 216, 122),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-45, -151, 87, 266),
			new Rectangle(-76, -70, 153, 183),
			new Rectangle(-101, -8, 216, 122),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-28, -26, 59, 58),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-28, -26, 59, 58),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		}
	};

	public Rectangle[,] precCPUBOSS00NoneOffSet = new Rectangle[12, 4]
	{
		{
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-178, -31, 57, 41),
			new Rectangle(118, -31, 51, 40),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-178, -31, 57, 41),
			new Rectangle(118, -31, 51, 40),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		}
	};

	public Rectangle[,] precCPUBOSSChild00NoneOffSet = new Rectangle[6, 4]
	{
		{
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-48, -106, 96, 175),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-48, -106, 96, 175),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-48, -106, 96, 175),
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
			new Rectangle(-98, -98, 196, 196),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		}
	};

	public Rectangle[,] precCPUBOSSChild00R15NoneOffSet = new Rectangle[6, 4]
	{
		{
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-48, -106, 96, 175),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-48, -106, 96, 175),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-48, -106, 96, 175),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		}
	};

	public Rectangle[,] precCPUBOSSChild00R345NoneOffSet = new Rectangle[6, 4]
	{
		{
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-48, -106, 96, 175),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-48, -106, 96, 175),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(-48, -106, 96, 175),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		},
		{
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0),
			new Rectangle(0, 0, 0, 0)
		}
	};

	public CPUBOSS00(Game game)
		: base(game)
	{
		for (int i = 0; i < psrcCPUBOSS00Core.Length; i++)
		{
			psrcCPUBOSS00Core[i].penuImgState = new enuCPUBOSS00ImgState[60];
			psrcCPUBOSS00Core[i].psrcAI = new structCPUBOSS00AI[60];
		}
		for (int i = 0; i < psrcCPUBOSSChild00Core.Length; i++)
		{
			psrcCPUBOSSChild00Core[i].penuImgState = new enuCPUBOSSChild00ImgState[60];
			psrcCPUBOSSChild00Core[i].psrcAI = new structCPUBOSS00AI[60];
		}
	}

	public override void Initialize()
	{
		pCPUBOSS00Init();
		pCPUBOSSChild00Init();
		base.Initialize();
	}

	public void pCPUBOSS00Init()
	{
		for (int i = 0; i < psrcCPUBOSS00Core.Length; i++)
		{
			KakuCPUBOSS00Init(i);
		}
	}

	private void KakuCPUBOSS00Init(int aintCPUNo)
	{
		psrcCPUBOSS00Core[aintCPUNo].pflgEnable = false;
		psrcCPUBOSS00Core[aintCPUNo].pintChildMax = 0;
		psrcCPUBOSS00Core[aintCPUNo].pSpriteEffects = SpriteEffects.None;
		psrcCPUBOSS00Core[aintCPUNo].pfltDF = 0f;
		psrcCPUBOSS00Core[aintCPUNo].pfltHP = 0f;
		psrcCPUBOSS00Core[aintCPUNo].pfltDamage = 0f;
		psrcCPUBOSS00Core[aintCPUNo].pVecIti.X = 0f;
		psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y = 0f;
		psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z = 0f;
		psrcCPUBOSS00Core[aintCPUNo].pfltItiR = 0f;
		psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X = 0f;
		psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y = 0f;
		psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z = 0f;
		psrcCPUBOSS00Core[aintCPUNo].pVecMovDelta.X = 0f;
		psrcCPUBOSS00Core[aintCPUNo].pVecMovDelta.Y = 0f;
		psrcCPUBOSS00Core[aintCPUNo].pVecMovDelta.Z = 0f;
		psrcCPUBOSS00Core[aintCPUNo].penuImgStateHoji = enuCPUBOSS00ImgStateHoji.intNone;
		CPUBOSS00AIInit(aintCPUNo);
		psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intNormal;
		psrcCPUBOSS00Core[aintCPUNo].penuType = enuCPUBOSS00Type.intNormal00;
		NomarlStartStateSet(aintCPUNo);
	}

	private void CPUBOSS00AIInit(int aintCPUNo)
	{
		for (int i = 0; i < psrcCPUBOSS00Core[aintCPUNo].psrcAI.Length; i++)
		{
			psrcCPUBOSS00Core[aintCPUNo].psrcAI[i].pintLoopCount = 0;
			psrcCPUBOSS00Core[aintCPUNo].psrcAI[i].penuTypeMovX = OperationTypeMovX.None;
			psrcCPUBOSS00Core[aintCPUNo].psrcAI[i].penuTypeMovY = OperationTypeMovY.None;
			psrcCPUBOSS00Core[aintCPUNo].psrcAI[i].penuTypeMovZ = OperationTypeMovZ.None;
			psrcCPUBOSS00Core[aintCPUNo].psrcAI[i].penuTypeMovXDash = OperationTypeMovXDash.None;
			psrcCPUBOSS00Core[aintCPUNo].psrcAI[i].penuTypeMovYDash = OperationTypeMovYDash.None;
			psrcCPUBOSS00Core[aintCPUNo].psrcAI[i].penuTypeMovZDash = OperationTypeMovZDash.None;
			psrcCPUBOSS00Core[aintCPUNo].psrcAI[i].penuTypeMovAttack = OperationTypeMovAttack.None;
			psrcCPUBOSS00Core[aintCPUNo].psrcAI[i].penuTypeMuki = OperationTypeMukiX.None;
			psrcCPUBOSS00Core[aintCPUNo].psrcAI[i].penuTypeDead = OperationTypeDead.None;
		}
	}

	public void pCPUBOSS00Enable(Vector3 avec3Iti, enuCPUBOSS00Type aenuCPU00Type, float afltHp, float afltDF)
	{
		for (int i = 0; i < psrcCPUBOSS00Core.Length; i++)
		{
			if (psrcCPUBOSS00Core[i].pflgEnable)
			{
				continue;
			}
			KakuCPUBOSS00Init(i);
			psrcCPUBOSS00Core[i].pflgEnable = true;
			psrcCPUBOSS00Core[i].pSpriteEffects = SpriteEffects.None;
			psrcCPUBOSS00Core[i].pVecIti.X = avec3Iti.X;
			psrcCPUBOSS00Core[i].pVecIti.Y = avec3Iti.Y;
			psrcCPUBOSS00Core[i].pVecIti.Z = avec3Iti.Z;
			psrcCPUBOSS00Core[i].pVecMovIti.X = 0f;
			psrcCPUBOSS00Core[i].pVecMovIti.Y = 0f;
			psrcCPUBOSS00Core[i].pVecMovIti.Z = 0f;
			psrcCPUBOSS00Core[i].penuType = aenuCPU00Type;
			CPUBOSS00AIInit(i);
			psrcCPUBOSS00Core[i].pfltHP = afltHp;
			psrcCPUBOSS00Core[i].pfltDF = afltDF;
			switch (aenuCPU00Type)
			{
			case enuCPUBOSS00Type.intOSPREY00:
				OSPREY00StartAI(ref psrcCPUBOSS00Core[i]);
				pCPUBOSSChild00Enable(new Vector3(avec3Iti.X - 256f + 64f, avec3Iti.Y - 10f, avec3Iti.Z + 0.00390625f), SpriteEffects.None, enuCPUBOSSChild00Type.intOSPREYChild00);
				pCPUBOSSChild00Enable(new Vector3(avec3Iti.X + 256f - 64f, avec3Iti.Y - 10f, avec3Iti.Z + 0.00390625f), SpriteEffects.FlipHorizontally, enuCPUBOSSChild00Type.intOSPREYChildX00);
				psrcCPUBOSS00Core[i].penuImgStateHoji = enuCPUBOSS00ImgStateHoji.intNone;
				psrcCPUBOSS00Core[i].pintChildMax = 2;
				NomarlStartStateSet(i);
				break;
			case enuCPUBOSS00Type.intDragon:
			{
				psrcCPUBOSS00Core[i].pintChildMax = 11;
				Dragon00StartAI(ref psrcCPUBOSS00Core[i]);
				for (int j = 0; j < psrcCPUBOSS00Core[i].pintChildMax; j++)
				{
					pCPUBOSSChild00Enable(new Vector3(avec3Iti.X - (float)((j + 1) * 10), avec3Iti.Y + 32f, avec3Iti.Z), SpriteEffects.None, enuCPUBOSSChild00Type.intDragonBody);
					DragonChild00StartAI(ref psrcCPUBOSSChild00Core[j], (j + 1) * 9);
				}
				NomarlStartStateSet(i);
				break;
			}
			case enuCPUBOSS00Type.intQueenBee:
				QueenBee00StartAI(ref psrcCPUBOSS00Core[i]);
				psrcCPUBOSS00Core[i].pintChildMax = 0;
				psrcCPUBOSS00Core[i].penuImgStateHoji = enuCPUBOSS00ImgStateHoji.intNone;
				QueenBee00StartStateSet(i);
				break;
			case enuCPUBOSS00Type.intHatiNosu:
				HatiNoSu00StartAI(ref psrcCPUBOSS00Core[i]);
				psrcCPUBOSS00Core[i].pintChildMax = 0;
				psrcCPUBOSS00Core[i].penuImgStateHoji = enuCPUBOSS00ImgStateHoji.intNone;
				NomarlStartStateSet(i);
				break;
			case enuCPUBOSS00Type.intArship:
				Arship00StartAI(ref psrcCPUBOSS00Core[i]);
				psrcCPUBOSS00Core[i].pintChildMax = 0;
				psrcCPUBOSS00Core[i].penuImgStateHoji = enuCPUBOSS00ImgStateHoji.intLeft;
				NomarlStartStateSet(i);
				break;
			case enuCPUBOSS00Type.intArshipTate:
				psrcCPUBOSS00Core[i].pintChildMax = 0;
				psrcCPUBOSS00Core[i].penuImgStateHoji = enuCPUBOSS00ImgStateHoji.intLeft;
				NomarlStartStateSet(i);
				break;
			case enuCPUBOSS00Type.intESP:
				ESP00StartAI(ref psrcCPUBOSS00Core[i]);
				psrcCPUBOSS00Core[i].pintChildMax = 0;
				psrcCPUBOSS00Core[i].penuImgStateHoji = enuCPUBOSS00ImgStateHoji.intMove;
				NomarlStartStateSet(i);
				break;
			}
			break;
		}
	}

	private bool isCPUBOSS00EnableSearch(enuCPUBOSS00Type aenuCPU00Type)
	{
		for (int i = 0; i < psrcCPUBOSS00Core.Length; i++)
		{
			if (psrcCPUBOSS00Core[i].pflgEnable && psrcCPUBOSS00Core[i].penuType == aenuCPU00Type)
			{
				return true;
			}
		}
		return false;
	}

	private int intCPUBOSS00EnableSearch(enuCPUBOSS00Type aenuCPU00Type)
	{
		for (int i = 0; i < psrcCPUBOSS00Core.Length; i++)
		{
			if (psrcCPUBOSS00Core[i].pflgEnable && psrcCPUBOSS00Core[i].penuType == aenuCPU00Type)
			{
				return i;
			}
		}
		return psrcCPUBOSS00Core.Length;
	}

	public void pCPUBOSSChild00Init()
	{
		for (int i = 0; i < psrcCPUBOSSChild00Core.Length; i++)
		{
			KakuCPUBOSSChild00Init(i);
		}
	}

	private void KakuCPUBOSSChild00Init(int aintCPUNo)
	{
		psrcCPUBOSSChild00Core[aintCPUNo].pflgEnable = false;
		psrcCPUBOSSChild00Core[aintCPUNo].pSpriteEffects = SpriteEffects.None;
		psrcCPUBOSSChild00Core[aintCPUNo].pintDF = 0f;
		psrcCPUBOSSChild00Core[aintCPUNo].pintHP = 0f;
		psrcCPUBOSSChild00Core[aintCPUNo].pVecIti.X = 0f;
		psrcCPUBOSSChild00Core[aintCPUNo].pVecIti.Y = 0f;
		psrcCPUBOSSChild00Core[aintCPUNo].pVecIti.Z = 0f;
		psrcCPUBOSSChild00Core[aintCPUNo].pfltItiR = 0f;
		psrcCPUBOSSChild00Core[aintCPUNo].pVecMovIti.X = 0f;
		psrcCPUBOSSChild00Core[aintCPUNo].pVecMovIti.Y = 0f;
		psrcCPUBOSSChild00Core[aintCPUNo].pVecMovIti.Z = 0f;
		psrcCPUBOSSChild00Core[aintCPUNo].pVecMovDelta.X = 0f;
		psrcCPUBOSSChild00Core[aintCPUNo].pVecMovDelta.Y = 0f;
		psrcCPUBOSSChild00Core[aintCPUNo].pVecMovDelta.Z = 0f;
		CPUBOSSChild00AIInit(aintCPUNo);
		psrcCPUBOSSChild00Core[aintCPUNo].enuMovState = enuCPUBOSSChild00MovState.intNormal;
		psrcCPUBOSSChild00Core[aintCPUNo].penuType = enuCPUBOSSChild00Type.intNormal00;
		NomarlChildStartStateSet(aintCPUNo);
	}

	private void CPUBOSSChild00AIInit(int aintCPUNo)
	{
		for (int i = 0; i < psrcCPUBOSSChild00Core[aintCPUNo].psrcAI.Length; i++)
		{
			psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[i].pintLoopCount = 0;
			psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[i].penuTypeMovX = OperationTypeMovX.None;
			psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[i].penuTypeMovY = OperationTypeMovY.None;
			psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[i].penuTypeMovZ = OperationTypeMovZ.None;
			psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[i].penuTypeMovXDash = OperationTypeMovXDash.None;
			psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[i].penuTypeMovYDash = OperationTypeMovYDash.None;
			psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[i].penuTypeMovZDash = OperationTypeMovZDash.None;
			psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[i].penuTypeMovAttack = OperationTypeMovAttack.None;
			psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[i].penuTypeMuki = OperationTypeMukiX.None;
			psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[i].penuTypeDead = OperationTypeDead.None;
		}
	}

	public void pCPUBOSSChild00Enable(Vector3 avec3Iti, SpriteEffects aSEF, enuCPUBOSSChild00Type aenuCPU00Type)
	{
		for (int i = 0; i < psrcCPUBOSSChild00Core.Length; i++)
		{
			if (!psrcCPUBOSSChild00Core[i].pflgEnable)
			{
				KakuCPUBOSSChild00Init(i);
				psrcCPUBOSSChild00Core[i].pflgEnable = true;
				psrcCPUBOSSChild00Core[i].pSpriteEffects = aSEF;
				psrcCPUBOSSChild00Core[i].pVecIti.X = avec3Iti.X;
				psrcCPUBOSSChild00Core[i].pVecIti.Y = avec3Iti.Y;
				psrcCPUBOSSChild00Core[i].pVecIti.Z = avec3Iti.Z;
				psrcCPUBOSSChild00Core[i].pVecMovIti.X = 0f;
				psrcCPUBOSSChild00Core[i].pVecMovIti.Y = 0f;
				psrcCPUBOSSChild00Core[i].pVecMovIti.Z = 0f;
				psrcCPUBOSSChild00Core[i].penuType = aenuCPU00Type;
				CPUBOSSChild00AIInit(i);
				NomarlChildStartStateSet(i);
				break;
			}
		}
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}

	public void pCPUBOSSUpdate()
	{
		pImageStateUpdate();
		for (int i = 0; i < psrcCPUBOSS00Core.Length; i++)
		{
			if (psrcCPUBOSS00Core[i].pflgEnable)
			{
				kakuCPUBOSS00Update(i);
				NomarlStateSet(i);
				CPUBOSSDamageUpdate(i);
			}
		}
		pImageChildStateUpdate();
		for (int i = 0; i < psrcCPUBOSSChild00Core.Length; i++)
		{
			if (psrcCPUBOSSChild00Core[i].pflgEnable)
			{
				kakuCPUBOSSChild00Update(i);
			}
		}
	}

	private void CPUBOSSDamageUpdate(int aintCPUNo)
	{
		if (!(psrcCPUBOSS00Core[aintCPUNo].pfltDamage >= 0f))
		{
			return;
		}
		if (psrcCPUBOSS00Core[aintCPUNo].pfltDamage <= 1f)
		{
			psrcCPUBOSS00Core[aintCPUNo].pfltDamage = 0f;
			return;
		}
		if (psrcCPUBOSS00Core[aintCPUNo].pfltDamage >= 25f)
		{
			psrcCPUBOSS00Core[aintCPUNo].pfltDamage = 25f;
		}
		else
		{
			psrcCPUBOSS00Core[aintCPUNo].pfltDamage *= 0.8f;
		}
		psrcCPUBOSS00Core[aintCPUNo].pfltHP -= psrcCPUBOSS00Core[aintCPUNo].pfltDamage;
	}

	private void kakuCPUBOSS00Update(int aintCPUNo)
	{
		CPUBOSS00AISift(aintCPUNo);
		switch (psrcCPUBOSS00Core[aintCPUNo].penuType)
		{
		case enuCPUBOSS00Type.intOSPREY00:
			OSPREY00AI(ref psrcCPUBOSS00Core[aintCPUNo]);
			CPUBOSS00OSPREYUpdate(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPUBOSS00Type.intDragon:
			Dragon00AI(ref psrcCPUBOSS00Core[aintCPUNo]);
			CPUBOSSDragon00Update(aintCPUNo);
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPUBOSS00Type.intQueenBee:
			QueenBee00AI(ref psrcCPUBOSS00Core[aintCPUNo]);
			QueenBee00StateSet(aintCPUNo);
			CPUBOSSQueeenBeeUpdate(aintCPUNo);
			break;
		case enuCPUBOSS00Type.intHatiNosu:
			NomarlStateSet(aintCPUNo);
			CPUBOSSDragon00Update(aintCPUNo);
			break;
		case enuCPUBOSS00Type.intArship:
			Arship00AI(ref psrcCPUBOSS00Core[aintCPUNo]);
			NomarlStateSet(aintCPUNo);
			CPUBOSSArshipUpdate(aintCPUNo);
			break;
		case enuCPUBOSS00Type.intArshipTate:
			NomarlStateSet(aintCPUNo);
			break;
		case enuCPUBOSS00Type.intESP:
			ESP00AI(ref psrcCPUBOSS00Core[aintCPUNo]);
			NomarlStateSet(aintCPUNo);
			CPUBOSSESPUpdate(aintCPUNo);
			break;
		}
	}

	private void CPUBOSS00AISift(int aintCPUNo)
	{
		if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].pintLoopCount > 0)
		{
			psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].pintLoopCount--;
			return;
		}
		for (int i = 0; i < psrcCPUBOSS00Core[aintCPUNo].psrcAI.Length - 1; i++)
		{
			ref structCPUBOSS00AI reference = ref psrcCPUBOSS00Core[aintCPUNo].psrcAI[i];
			reference = psrcCPUBOSS00Core[aintCPUNo].psrcAI[i + 1];
		}
		int num = psrcCPUBOSS00Core[aintCPUNo].psrcAI.Length - 1;
		psrcCPUBOSS00Core[aintCPUNo].psrcAI[num].pintLoopCount = 0;
		psrcCPUBOSS00Core[aintCPUNo].psrcAI[num].penuTypeMovX = OperationTypeMovX.None;
		psrcCPUBOSS00Core[aintCPUNo].psrcAI[num].penuTypeMovY = OperationTypeMovY.None;
		psrcCPUBOSS00Core[aintCPUNo].psrcAI[num].penuTypeMovZ = OperationTypeMovZ.None;
		psrcCPUBOSS00Core[aintCPUNo].psrcAI[num].penuTypeMovXDash = OperationTypeMovXDash.None;
		psrcCPUBOSS00Core[aintCPUNo].psrcAI[num].penuTypeMovYDash = OperationTypeMovYDash.None;
		psrcCPUBOSS00Core[aintCPUNo].psrcAI[num].penuTypeMovZDash = OperationTypeMovZDash.None;
		psrcCPUBOSS00Core[aintCPUNo].psrcAI[num].penuTypeMovAttack = OperationTypeMovAttack.None;
		psrcCPUBOSS00Core[aintCPUNo].psrcAI[num].penuTypeMuki = OperationTypeMukiX.None;
	}

	private void OSPREY00AI(ref srcCPUBOSS00Core asrcCPUBOSS00Core)
	{
		if (BOSSAIEndHantei(asrcCPUBOSS00Core.psrcAI))
		{
			for (int i = 0; i < 45; i += 15)
			{
				asrcCPUBOSS00Core.psrcAI[i].pintLoopCount = 40;
				asrcCPUBOSS00Core.psrcAI[i].penuTypeMovX = OperationTypeMovX.Xp;
				asrcCPUBOSS00Core.psrcAI[i].penuTypeMovZ = OperationTypeMovZ.Zm;
				asrcCPUBOSS00Core.psrcAI[i + 1].pintLoopCount = 40;
				asrcCPUBOSS00Core.psrcAI[i + 1].penuTypeMovX = OperationTypeMovX.Xm;
				asrcCPUBOSS00Core.psrcAI[i + 1].penuTypeMovZ = OperationTypeMovZ.Zm;
				asrcCPUBOSS00Core.psrcAI[i + 2].pintLoopCount = 0;
				asrcCPUBOSS00Core.psrcAI[i + 2].penuTypeMovAttack = OperationTypeMovAttack.Attack;
				asrcCPUBOSS00Core.psrcAI[i + 3].pintLoopCount = 40;
				asrcCPUBOSS00Core.psrcAI[i + 3].penuTypeMovX = OperationTypeMovX.Xp;
				asrcCPUBOSS00Core.psrcAI[i + 3].penuTypeMovZ = OperationTypeMovZ.Zp;
				asrcCPUBOSS00Core.psrcAI[i + 4].pintLoopCount = 40;
				asrcCPUBOSS00Core.psrcAI[i + 4].penuTypeMovX = OperationTypeMovX.Xm;
				asrcCPUBOSS00Core.psrcAI[i + 4].penuTypeMovZ = OperationTypeMovZ.Zp;
				asrcCPUBOSS00Core.psrcAI[i + 5].pintLoopCount = 0;
				asrcCPUBOSS00Core.psrcAI[i + 5].penuTypeMovAttack = OperationTypeMovAttack.Attack;
				asrcCPUBOSS00Core.psrcAI[i + 6].pintLoopCount = 40;
				asrcCPUBOSS00Core.psrcAI[i + 6].penuTypeMovX = OperationTypeMovX.Xm;
				asrcCPUBOSS00Core.psrcAI[i + 6].penuTypeMovZ = OperationTypeMovZ.Zm;
				asrcCPUBOSS00Core.psrcAI[i + 7].pintLoopCount = 40;
				asrcCPUBOSS00Core.psrcAI[i + 7].penuTypeMovX = OperationTypeMovX.Xp;
				asrcCPUBOSS00Core.psrcAI[i + 7].penuTypeMovZ = OperationTypeMovZ.Zm;
				asrcCPUBOSS00Core.psrcAI[i + 8].pintLoopCount = 0;
				asrcCPUBOSS00Core.psrcAI[i + 8].penuTypeMovAttack = OperationTypeMovAttack.Attack;
				asrcCPUBOSS00Core.psrcAI[i + 9].pintLoopCount = 40;
				asrcCPUBOSS00Core.psrcAI[i + 9].penuTypeMovX = OperationTypeMovX.Xp;
				asrcCPUBOSS00Core.psrcAI[i + 9].penuTypeMovZ = OperationTypeMovZ.Zp;
				asrcCPUBOSS00Core.psrcAI[i + 10].pintLoopCount = 40;
				asrcCPUBOSS00Core.psrcAI[i + 10].penuTypeMovX = OperationTypeMovX.Xm;
				asrcCPUBOSS00Core.psrcAI[i + 10].penuTypeMovZ = OperationTypeMovZ.Zp;
				asrcCPUBOSS00Core.psrcAI[i + 11].pintLoopCount = 0;
				asrcCPUBOSS00Core.psrcAI[i + 11].penuTypeMovAttack = OperationTypeMovAttack.Attack;
				asrcCPUBOSS00Core.psrcAI[i + 12].pintLoopCount = 30;
				asrcCPUBOSS00Core.psrcAI[i + 12].penuTypeMovY = OperationTypeMovY.Ym;
				asrcCPUBOSS00Core.psrcAI[i + 13].pintLoopCount = 0;
				asrcCPUBOSS00Core.psrcAI[i + 13].penuTypeMovAttack = OperationTypeMovAttack.Attack;
				asrcCPUBOSS00Core.psrcAI[i + 14].pintLoopCount = 30;
				asrcCPUBOSS00Core.psrcAI[i + 14].penuTypeMovY = OperationTypeMovY.Yp;
			}
			asrcCPUBOSS00Core.psrcAI[45].pintLoopCount = 120;
			asrcCPUBOSS00Core.psrcAI[45].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[46].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[46].penuTypeDead = OperationTypeDead.Dead;
		}
	}

	private void OSPREY00StartAI(ref srcCPUBOSS00Core asrcCPUBOSS00Core)
	{
		if (BOSSAIEndHantei(asrcCPUBOSS00Core.psrcAI))
		{
			asrcCPUBOSS00Core.psrcAI[0].pintLoopCount = 20;
			asrcCPUBOSS00Core.psrcAI[0].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[0].penuTypeMovZ = OperationTypeMovZ.Zp;
			asrcCPUBOSS00Core.psrcAI[1].pintLoopCount = 50;
			asrcCPUBOSS00Core.psrcAI[1].penuTypeMovX = OperationTypeMovX.Xp;
			asrcCPUBOSS00Core.psrcAI[1].penuTypeMovZ = OperationTypeMovZ.Zp;
			asrcCPUBOSS00Core.psrcAI[2].pintLoopCount = 40;
			asrcCPUBOSS00Core.psrcAI[2].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[2].penuTypeMovZ = OperationTypeMovZ.Zp;
			asrcCPUBOSS00Core.psrcAI[3].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[3].penuTypeMovY = OperationTypeMovY.Ym;
			asrcCPUBOSS00Core.psrcAI[4].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[4].penuTypeMovY = OperationTypeMovY.Yp;
			asrcCPUBOSS00Core.psrcAI[5].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[5].penuTypeMovAttack = OperationTypeMovAttack.Attack;
		}
	}

	private void Dragon00StartAI(ref srcCPUBOSS00Core asrcCPUBOSS00Core)
	{
		if (BOSSAIEndHantei(asrcCPUBOSS00Core.psrcAI))
		{
			asrcCPUBOSS00Core.psrcAI[0].pintLoopCount = 33;
			asrcCPUBOSS00Core.psrcAI[0].penuTypeMovZ = OperationTypeMovZ.Zp;
			for (int i = 1; i < 23; i += 11)
			{
				asrcCPUBOSS00Core.psrcAI[i].pintLoopCount = 15;
				asrcCPUBOSS00Core.psrcAI[i].penuTypeMovZ = OperationTypeMovZ.Zp;
				asrcCPUBOSS00Core.psrcAI[i + 1].pintLoopCount = 50;
				asrcCPUBOSS00Core.psrcAI[i + 1].penuTypeMovX = OperationTypeMovX.Xm;
				asrcCPUBOSS00Core.psrcAI[i + 1].penuTypeMovZ = OperationTypeMovZ.Zp;
				asrcCPUBOSS00Core.psrcAI[i + 2].pintLoopCount = 0;
				asrcCPUBOSS00Core.psrcAI[i + 2].penuTypeMovAttack = OperationTypeMovAttack.Attack;
				asrcCPUBOSS00Core.psrcAI[i + 3].pintLoopCount = 65;
				asrcCPUBOSS00Core.psrcAI[i + 3].penuTypeMovZ = OperationTypeMovZ.Zm;
				asrcCPUBOSS00Core.psrcAI[i + 4].pintLoopCount = 50;
				asrcCPUBOSS00Core.psrcAI[i + 4].penuTypeMovX = OperationTypeMovX.Xp;
				asrcCPUBOSS00Core.psrcAI[i + 5].pintLoopCount = 55;
				asrcCPUBOSS00Core.psrcAI[i + 5].penuTypeMovZ = OperationTypeMovZ.Zp;
				asrcCPUBOSS00Core.psrcAI[i + 6].pintLoopCount = 0;
				asrcCPUBOSS00Core.psrcAI[i + 6].penuTypeMovAttack = OperationTypeMovAttack.Attack;
				asrcCPUBOSS00Core.psrcAI[i + 7].pintLoopCount = 25;
				asrcCPUBOSS00Core.psrcAI[i + 7].penuTypeMovX = OperationTypeMovX.Xm;
				asrcCPUBOSS00Core.psrcAI[i + 7].penuTypeMovY = OperationTypeMovY.Ym;
				asrcCPUBOSS00Core.psrcAI[i + 8].pintLoopCount = 0;
				asrcCPUBOSS00Core.psrcAI[i + 8].penuTypeMovAttack = OperationTypeMovAttack.Attack;
				asrcCPUBOSS00Core.psrcAI[i + 9].pintLoopCount = 55;
				asrcCPUBOSS00Core.psrcAI[i + 9].penuTypeMovZ = OperationTypeMovZ.Zm;
				asrcCPUBOSS00Core.psrcAI[i + 10].pintLoopCount = 25;
				asrcCPUBOSS00Core.psrcAI[i + 10].penuTypeMovX = OperationTypeMovX.Xp;
				asrcCPUBOSS00Core.psrcAI[i + 10].penuTypeMovY = OperationTypeMovY.Yp;
			}
			asrcCPUBOSS00Core.psrcAI[24].pintLoopCount = 40;
			asrcCPUBOSS00Core.psrcAI[24].penuTypeMovX = OperationTypeMovX.Xm;
		}
	}

	private void Dragon00AI(ref srcCPUBOSS00Core asrcCPUBOSS00Core)
	{
		if (BOSSAIEndHantei(asrcCPUBOSS00Core.psrcAI))
		{
			asrcCPUBOSS00Core.psrcAI[0].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[0].penuTypeMovZ = OperationTypeMovZ.Zp;
			asrcCPUBOSS00Core.psrcAI[0].penuTypeMovX = OperationTypeMovX.Xp;
			asrcCPUBOSS00Core.psrcAI[1].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[1].penuTypeMovZ = OperationTypeMovZ.Zp;
			asrcCPUBOSS00Core.psrcAI[1].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[2].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[2].penuTypeMovAttack = OperationTypeMovAttack.Attack;
			asrcCPUBOSS00Core.psrcAI[3].pintLoopCount = 25;
			asrcCPUBOSS00Core.psrcAI[3].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[3].penuTypeMovY = OperationTypeMovY.Ym;
			asrcCPUBOSS00Core.psrcAI[4].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[4].penuTypeMovX = OperationTypeMovX.Xp;
			asrcCPUBOSS00Core.psrcAI[5].pintLoopCount = 25;
			asrcCPUBOSS00Core.psrcAI[5].penuTypeMovX = OperationTypeMovX.Xp;
			asrcCPUBOSS00Core.psrcAI[5].penuTypeMovY = OperationTypeMovY.Yp;
			asrcCPUBOSS00Core.psrcAI[6].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[6].penuTypeMovAttack = OperationTypeMovAttack.Attack;
			asrcCPUBOSS00Core.psrcAI[7].pintLoopCount = 60;
			asrcCPUBOSS00Core.psrcAI[7].penuTypeMovZ = OperationTypeMovZ.Zm;
			asrcCPUBOSS00Core.psrcAI[8].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[8].penuTypeMovX = OperationTypeMovX.Xm;
		}
	}

	private void DragonChild00StartAI(ref srcCPUBOSSChild00Core asrcCPUBOSSChild00Core, int aintWait)
	{
		if (BOSSAIEndHantei(asrcCPUBOSSChild00Core.psrcAI))
		{
			asrcCPUBOSSChild00Core.psrcAI[0].pintLoopCount = aintWait;
			asrcCPUBOSSChild00Core.psrcAI[1].pintLoopCount = 33;
			asrcCPUBOSSChild00Core.psrcAI[1].penuTypeMovZ = OperationTypeMovZ.Zp;
			for (int i = 2; i < 24; i += 11)
			{
				asrcCPUBOSSChild00Core.psrcAI[i].pintLoopCount = 15;
				asrcCPUBOSSChild00Core.psrcAI[i].penuTypeMovZ = OperationTypeMovZ.Zp;
				asrcCPUBOSSChild00Core.psrcAI[i + 1].pintLoopCount = 50;
				asrcCPUBOSSChild00Core.psrcAI[i + 1].penuTypeMovX = OperationTypeMovX.Xm;
				asrcCPUBOSSChild00Core.psrcAI[i + 1].penuTypeMovZ = OperationTypeMovZ.Zp;
				asrcCPUBOSSChild00Core.psrcAI[i + 2].pintLoopCount = 0;
				asrcCPUBOSSChild00Core.psrcAI[i + 3].pintLoopCount = 65;
				asrcCPUBOSSChild00Core.psrcAI[i + 3].penuTypeMovZ = OperationTypeMovZ.Zm;
				asrcCPUBOSSChild00Core.psrcAI[i + 4].pintLoopCount = 50;
				asrcCPUBOSSChild00Core.psrcAI[i + 4].penuTypeMovX = OperationTypeMovX.Xp;
				asrcCPUBOSSChild00Core.psrcAI[i + 5].pintLoopCount = 55;
				asrcCPUBOSSChild00Core.psrcAI[i + 5].penuTypeMovZ = OperationTypeMovZ.Zp;
				asrcCPUBOSSChild00Core.psrcAI[i + 6].pintLoopCount = 0;
				asrcCPUBOSSChild00Core.psrcAI[i + 7].pintLoopCount = 25;
				asrcCPUBOSSChild00Core.psrcAI[i + 7].penuTypeMovX = OperationTypeMovX.Xm;
				asrcCPUBOSSChild00Core.psrcAI[i + 7].penuTypeMovY = OperationTypeMovY.Ym;
				asrcCPUBOSSChild00Core.psrcAI[i + 8].pintLoopCount = 0;
				asrcCPUBOSSChild00Core.psrcAI[i + 9].pintLoopCount = 55;
				asrcCPUBOSSChild00Core.psrcAI[i + 9].penuTypeMovZ = OperationTypeMovZ.Zm;
				asrcCPUBOSSChild00Core.psrcAI[i + 10].pintLoopCount = 25;
				asrcCPUBOSSChild00Core.psrcAI[i + 10].penuTypeMovX = OperationTypeMovX.Xp;
				asrcCPUBOSSChild00Core.psrcAI[i + 10].penuTypeMovY = OperationTypeMovY.Yp;
			}
			asrcCPUBOSSChild00Core.psrcAI[25].pintLoopCount = 40;
			asrcCPUBOSSChild00Core.psrcAI[25].penuTypeMovX = OperationTypeMovX.Xm;
		}
	}

	private void DragonChild00AI(ref srcCPUBOSSChild00Core asrcCPUBOSSChild00Core)
	{
		if (BOSSAIEndHantei(asrcCPUBOSSChild00Core.psrcAI))
		{
			asrcCPUBOSSChild00Core.psrcAI[0].pintLoopCount = 30;
			asrcCPUBOSSChild00Core.psrcAI[0].penuTypeMovZ = OperationTypeMovZ.Zp;
			asrcCPUBOSSChild00Core.psrcAI[0].penuTypeMovX = OperationTypeMovX.Xp;
			asrcCPUBOSSChild00Core.psrcAI[1].pintLoopCount = 30;
			asrcCPUBOSSChild00Core.psrcAI[1].penuTypeMovZ = OperationTypeMovZ.Zp;
			asrcCPUBOSSChild00Core.psrcAI[1].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSSChild00Core.psrcAI[2].pintLoopCount = 0;
			asrcCPUBOSSChild00Core.psrcAI[3].pintLoopCount = 25;
			asrcCPUBOSSChild00Core.psrcAI[3].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSSChild00Core.psrcAI[3].penuTypeMovY = OperationTypeMovY.Ym;
			asrcCPUBOSSChild00Core.psrcAI[4].pintLoopCount = 30;
			asrcCPUBOSSChild00Core.psrcAI[4].penuTypeMovX = OperationTypeMovX.Xp;
			asrcCPUBOSSChild00Core.psrcAI[5].pintLoopCount = 25;
			asrcCPUBOSSChild00Core.psrcAI[5].penuTypeMovX = OperationTypeMovX.Xp;
			asrcCPUBOSSChild00Core.psrcAI[5].penuTypeMovY = OperationTypeMovY.Yp;
			asrcCPUBOSSChild00Core.psrcAI[6].pintLoopCount = 0;
			asrcCPUBOSSChild00Core.psrcAI[7].pintLoopCount = 60;
			asrcCPUBOSSChild00Core.psrcAI[7].penuTypeMovZ = OperationTypeMovZ.Zm;
			asrcCPUBOSSChild00Core.psrcAI[8].pintLoopCount = 30;
			asrcCPUBOSSChild00Core.psrcAI[8].penuTypeMovX = OperationTypeMovX.Xm;
		}
	}

	private void QueenBee00StartAI(ref srcCPUBOSS00Core asrcCPUBOSS00Core)
	{
		if (BOSSAIEndHantei(asrcCPUBOSS00Core.psrcAI))
		{
			asrcCPUBOSS00Core.psrcAI[0].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[1].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[1].penuTypeMovAttack = OperationTypeMovAttack.SPAttack;
			asrcCPUBOSS00Core.psrcAI[2].pintLoopCount = 150;
			asrcCPUBOSS00Core.psrcAI[2].penuTypeMovY = OperationTypeMovY.Yp;
		}
	}

	private void QueenBee00AI(ref srcCPUBOSS00Core asrcCPUBOSS00Core)
	{
		if (!BOSSAIEndHantei(asrcCPUBOSS00Core.psrcAI))
		{
			return;
		}
		if (isCPUBOSS00EnableSearch(enuCPUBOSS00Type.intHatiNosu))
		{
			asrcCPUBOSS00Core.psrcAI[0].pintLoopCount = 9;
			asrcCPUBOSS00Core.psrcAI[0].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[1].pintLoopCount = 20;
			asrcCPUBOSS00Core.psrcAI[1].penuTypeMovX = OperationTypeMovX.Xp;
			asrcCPUBOSS00Core.psrcAI[2].pintLoopCount = 10;
			asrcCPUBOSS00Core.psrcAI[2].penuTypeMovX = OperationTypeMovX.Xm;
			return;
		}
		for (int i = 0; i < 6; i += 3)
		{
			asrcCPUBOSS00Core.psrcAI[i].pintLoopCount = 50;
			asrcCPUBOSS00Core.psrcAI[i].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[i + 1].pintLoopCount = 100;
			asrcCPUBOSS00Core.psrcAI[i + 1].penuTypeMovX = OperationTypeMovX.Xp;
			asrcCPUBOSS00Core.psrcAI[i + 2].pintLoopCount = 50;
			asrcCPUBOSS00Core.psrcAI[i + 2].penuTypeMovX = OperationTypeMovX.Xm;
		}
		asrcCPUBOSS00Core.psrcAI[6].pintLoopCount = 150;
		asrcCPUBOSS00Core.psrcAI[6].penuTypeMovY = OperationTypeMovY.Ym;
		asrcCPUBOSS00Core.psrcAI[7].pintLoopCount = 0;
		asrcCPUBOSS00Core.psrcAI[7].penuTypeMovAttack = OperationTypeMovAttack.SPAttack;
		asrcCPUBOSS00Core.psrcAI[8].pintLoopCount = 150;
		asrcCPUBOSS00Core.psrcAI[8].penuTypeMovY = OperationTypeMovY.Yp;
	}

	private void HatiNoSu00StartAI(ref srcCPUBOSS00Core asrcCPUBOSS00Core)
	{
		if (BOSSAIEndHantei(asrcCPUBOSS00Core.psrcAI))
		{
			asrcCPUBOSS00Core.psrcAI[0].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[1].pintLoopCount = 150;
			asrcCPUBOSS00Core.psrcAI[1].penuTypeMovY = OperationTypeMovY.Yp;
		}
	}

	private void Arship00StartAI(ref srcCPUBOSS00Core asrcCPUBOSS00Core)
	{
		if (BOSSAIEndHantei(asrcCPUBOSS00Core.psrcAI))
		{
			asrcCPUBOSS00Core.psrcAI[0].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[1].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[1].penuTypeMovAttack = OperationTypeMovAttack.SPAttack;
			asrcCPUBOSS00Core.psrcAI[2].pintLoopCount = 50;
			asrcCPUBOSS00Core.psrcAI[2].penuTypeMovZ = OperationTypeMovZ.Zp;
			asrcCPUBOSS00Core.psrcAI[2].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[3].pintLoopCount = 80;
			asrcCPUBOSS00Core.psrcAI[3].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[4].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[4].penuTypeMovAttack = OperationTypeMovAttack.Attack;
			asrcCPUBOSS00Core.psrcAI[5].pintLoopCount = 10;
			asrcCPUBOSS00Core.psrcAI[5].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[6].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[6].penuTypeMovAttack = OperationTypeMovAttack.Attack;
			asrcCPUBOSS00Core.psrcAI[7].pintLoopCount = 10;
			asrcCPUBOSS00Core.psrcAI[7].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[8].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[8].penuTypeMovAttack = OperationTypeMovAttack.Attack;
			asrcCPUBOSS00Core.psrcAI[9].pintLoopCount = 80;
			asrcCPUBOSS00Core.psrcAI[9].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[10].pintLoopCount = 10;
			asrcCPUBOSS00Core.psrcAI[10].penuTypeMovZ = OperationTypeMovZ.Zm;
			asrcCPUBOSS00Core.psrcAI[10].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[11].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[11].penuTypeMuki = OperationTypeMukiX.X;
			asrcCPUBOSS00Core.psrcAI[12].pintLoopCount = 10;
			asrcCPUBOSS00Core.psrcAI[12].penuTypeMovZ = OperationTypeMovZ.Zm;
			asrcCPUBOSS00Core.psrcAI[12].penuTypeMovX = OperationTypeMovX.Xp;
			asrcCPUBOSS00Core.psrcAI[13].pintLoopCount = 90;
			asrcCPUBOSS00Core.psrcAI[13].penuTypeMovX = OperationTypeMovX.Xp;
			asrcCPUBOSS00Core.psrcAI[14].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[14].penuTypeMovAttack = OperationTypeMovAttack.Attack;
			asrcCPUBOSS00Core.psrcAI[15].pintLoopCount = 90;
			asrcCPUBOSS00Core.psrcAI[15].penuTypeMovX = OperationTypeMovX.Xp;
			asrcCPUBOSS00Core.psrcAI[16].pintLoopCount = 10;
			asrcCPUBOSS00Core.psrcAI[16].penuTypeMovZ = OperationTypeMovZ.Zp;
			asrcCPUBOSS00Core.psrcAI[16].penuTypeMovX = OperationTypeMovX.Xp;
		}
	}

	private void Arship00AI(ref srcCPUBOSS00Core asrcCPUBOSS00Core)
	{
		if (BOSSAIEndHantei(asrcCPUBOSS00Core.psrcAI))
		{
			asrcCPUBOSS00Core.psrcAI[0].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[0].penuTypeMuki = OperationTypeMukiX.X;
			asrcCPUBOSS00Core.psrcAI[1].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[1].penuTypeMovAttack = OperationTypeMovAttack.SPAttack;
			asrcCPUBOSS00Core.psrcAI[2].pintLoopCount = 10;
			asrcCPUBOSS00Core.psrcAI[2].penuTypeMovZ = OperationTypeMovZ.Zp;
			asrcCPUBOSS00Core.psrcAI[2].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[3].pintLoopCount = 80;
			asrcCPUBOSS00Core.psrcAI[3].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[4].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[4].penuTypeMovAttack = OperationTypeMovAttack.Attack;
			asrcCPUBOSS00Core.psrcAI[5].pintLoopCount = 10;
			asrcCPUBOSS00Core.psrcAI[5].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[6].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[6].penuTypeMovAttack = OperationTypeMovAttack.Attack;
			asrcCPUBOSS00Core.psrcAI[7].pintLoopCount = 10;
			asrcCPUBOSS00Core.psrcAI[7].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[8].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[8].penuTypeMovAttack = OperationTypeMovAttack.Attack;
			asrcCPUBOSS00Core.psrcAI[9].pintLoopCount = 80;
			asrcCPUBOSS00Core.psrcAI[9].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[10].pintLoopCount = 10;
			asrcCPUBOSS00Core.psrcAI[10].penuTypeMovZ = OperationTypeMovZ.Zm;
			asrcCPUBOSS00Core.psrcAI[10].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[11].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[11].penuTypeMuki = OperationTypeMukiX.X;
			asrcCPUBOSS00Core.psrcAI[12].pintLoopCount = 10;
			asrcCPUBOSS00Core.psrcAI[12].penuTypeMovZ = OperationTypeMovZ.Zm;
			asrcCPUBOSS00Core.psrcAI[12].penuTypeMovX = OperationTypeMovX.Xp;
			asrcCPUBOSS00Core.psrcAI[13].pintLoopCount = 90;
			asrcCPUBOSS00Core.psrcAI[13].penuTypeMovX = OperationTypeMovX.Xp;
			asrcCPUBOSS00Core.psrcAI[14].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[14].penuTypeMovAttack = OperationTypeMovAttack.Attack;
			asrcCPUBOSS00Core.psrcAI[15].pintLoopCount = 90;
			asrcCPUBOSS00Core.psrcAI[15].penuTypeMovX = OperationTypeMovX.Xp;
			asrcCPUBOSS00Core.psrcAI[16].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[16].penuTypeMovAttack = OperationTypeMovAttack.Attack;
			asrcCPUBOSS00Core.psrcAI[17].pintLoopCount = 10;
			asrcCPUBOSS00Core.psrcAI[17].penuTypeMovZ = OperationTypeMovZ.Zp;
			asrcCPUBOSS00Core.psrcAI[17].penuTypeMovX = OperationTypeMovX.Xp;
		}
	}

	private void ESP00StartAI(ref srcCPUBOSS00Core asrcCPUBOSS00Core)
	{
		if (BOSSAIEndHantei(asrcCPUBOSS00Core.psrcAI))
		{
			asrcCPUBOSS00Core.psrcAI[0].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[1].pintLoopCount = 60;
			asrcCPUBOSS00Core.psrcAI[1].penuTypeMovY = OperationTypeMovY.Yp;
			asrcCPUBOSS00Core.psrcAI[1].penuTypeMovZ = OperationTypeMovZ.Zp;
			for (int i = 0; i < 6; i += 2)
			{
				asrcCPUBOSS00Core.psrcAI[2 + i].pintLoopCount = 0;
				asrcCPUBOSS00Core.psrcAI[2 + i].penuTypeMovAttack = OperationTypeMovAttack.Attack;
				asrcCPUBOSS00Core.psrcAI[2 + i + 1].pintLoopCount = 30;
			}
			asrcCPUBOSS00Core.psrcAI[8].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[8].penuTypeMovAttack = OperationTypeMovAttack.SPAttack;
			asrcCPUBOSS00Core.psrcAI[9].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[10].pintLoopCount = 55;
			asrcCPUBOSS00Core.psrcAI[10].penuTypeMovX = OperationTypeMovX.Xp;
			asrcCPUBOSS00Core.psrcAI[11].pintLoopCount = 20;
			asrcCPUBOSS00Core.psrcAI[11].penuTypeMovY = OperationTypeMovY.Yp;
			asrcCPUBOSS00Core.psrcAI[12].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[12].penuTypeMovAttack = OperationTypeMovAttack.Attack;
			asrcCPUBOSS00Core.psrcAI[13].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[14].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[14].penuTypeMovAttack = OperationTypeMovAttack.SPAttack;
			asrcCPUBOSS00Core.psrcAI[15].pintLoopCount = 20;
			asrcCPUBOSS00Core.psrcAI[16].pintLoopCount = 55;
			asrcCPUBOSS00Core.psrcAI[16].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[17].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[17].penuTypeMovAttack = OperationTypeMovAttack.Attack;
			asrcCPUBOSS00Core.psrcAI[18].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[19].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[19].penuTypeMovAttack = OperationTypeMovAttack.Attack;
			asrcCPUBOSS00Core.psrcAI[20].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[21].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[22].penuTypeMovAttack = OperationTypeMovAttack.SPAttack;
			asrcCPUBOSS00Core.psrcAI[23].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[24].pintLoopCount = 55;
			asrcCPUBOSS00Core.psrcAI[24].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[25].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[25].penuTypeMovAttack = OperationTypeMovAttack.Attack;
			asrcCPUBOSS00Core.psrcAI[26].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[27].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[27].penuTypeMovAttack = OperationTypeMovAttack.SPAttack;
			asrcCPUBOSS00Core.psrcAI[28].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[29].pintLoopCount = 20;
			asrcCPUBOSS00Core.psrcAI[29].penuTypeMovY = OperationTypeMovY.Ym;
			asrcCPUBOSS00Core.psrcAI[30].pintLoopCount = 55;
			asrcCPUBOSS00Core.psrcAI[30].penuTypeMovX = OperationTypeMovX.Xp;
			for (int i = 0; i < 4; i += 2)
			{
				asrcCPUBOSS00Core.psrcAI[31 + i].pintLoopCount = 0;
				asrcCPUBOSS00Core.psrcAI[31 + i].penuTypeMovAttack = OperationTypeMovAttack.Attack;
				asrcCPUBOSS00Core.psrcAI[31 + i + 1].pintLoopCount = 30;
			}
			asrcCPUBOSS00Core.psrcAI[35].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[35].penuTypeMovAttack = OperationTypeMovAttack.SPAttack;
			asrcCPUBOSS00Core.psrcAI[36].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[37].pintLoopCount = 45;
			asrcCPUBOSS00Core.psrcAI[37].penuTypeMovX = OperationTypeMovX.Xp;
			for (int i = 0; i < 4; i += 2)
			{
				asrcCPUBOSS00Core.psrcAI[38 + i].pintLoopCount = 0;
				asrcCPUBOSS00Core.psrcAI[38 + i].penuTypeMovAttack = OperationTypeMovAttack.Attack;
				asrcCPUBOSS00Core.psrcAI[38 + i + 1].pintLoopCount = 30;
			}
			asrcCPUBOSS00Core.psrcAI[44].pintLoopCount = 45;
			asrcCPUBOSS00Core.psrcAI[44].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[45].pintLoopCount = 20;
			asrcCPUBOSS00Core.psrcAI[45].penuTypeMovY = OperationTypeMovY.Yp;
			for (int i = 0; i < 4; i += 2)
			{
				asrcCPUBOSS00Core.psrcAI[46 + i].pintLoopCount = 0;
				asrcCPUBOSS00Core.psrcAI[46 + i].penuTypeMovAttack = OperationTypeMovAttack.Attack;
				asrcCPUBOSS00Core.psrcAI[46 + i + 1].pintLoopCount = 30;
			}
			asrcCPUBOSS00Core.psrcAI[50].pintLoopCount = 1;
			asrcCPUBOSS00Core.psrcAI[50].penuTypeMovAttack = OperationTypeMovAttack.SPAttack;
			asrcCPUBOSS00Core.psrcAI[51].pintLoopCount = 35;
			asrcCPUBOSS00Core.psrcAI[52].pintLoopCount = 20;
			asrcCPUBOSS00Core.psrcAI[52].penuTypeMovY = OperationTypeMovY.Ym;
			asrcCPUBOSS00Core.psrcAI[53].pintLoopCount = 45;
			asrcCPUBOSS00Core.psrcAI[53].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[54].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[54].penuTypeMovAttack = OperationTypeMovAttack.Attack;
			asrcCPUBOSS00Core.psrcAI[55].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[56].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[56].penuTypeMovAttack = OperationTypeMovAttack.SPAttack;
			asrcCPUBOSS00Core.psrcAI[57].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[58].pintLoopCount = 45;
			asrcCPUBOSS00Core.psrcAI[58].penuTypeMovX = OperationTypeMovX.Xp;
		}
	}

	private void ESP00AI(ref srcCPUBOSS00Core asrcCPUBOSS00Core)
	{
		if (BOSSAIEndHantei(asrcCPUBOSS00Core.psrcAI))
		{
			for (int i = 0; i < 6; i += 2)
			{
				asrcCPUBOSS00Core.psrcAI[i].pintLoopCount = 0;
				asrcCPUBOSS00Core.psrcAI[i].penuTypeMovAttack = OperationTypeMovAttack.Attack;
				asrcCPUBOSS00Core.psrcAI[i + 1].pintLoopCount = 30;
			}
			asrcCPUBOSS00Core.psrcAI[6].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[6].penuTypeMovAttack = OperationTypeMovAttack.SPAttack;
			asrcCPUBOSS00Core.psrcAI[7].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[8].pintLoopCount = 55;
			asrcCPUBOSS00Core.psrcAI[8].penuTypeMovX = OperationTypeMovX.Xp;
			asrcCPUBOSS00Core.psrcAI[9].pintLoopCount = 20;
			asrcCPUBOSS00Core.psrcAI[9].penuTypeMovY = OperationTypeMovY.Yp;
			asrcCPUBOSS00Core.psrcAI[10].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[10].penuTypeMovAttack = OperationTypeMovAttack.Attack;
			asrcCPUBOSS00Core.psrcAI[11].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[12].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[12].penuTypeMovAttack = OperationTypeMovAttack.SPAttack;
			asrcCPUBOSS00Core.psrcAI[13].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[14].pintLoopCount = 55;
			asrcCPUBOSS00Core.psrcAI[14].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[16].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[16].penuTypeMovAttack = OperationTypeMovAttack.Attack;
			asrcCPUBOSS00Core.psrcAI[17].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[18].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[18].penuTypeMovAttack = OperationTypeMovAttack.Attack;
			asrcCPUBOSS00Core.psrcAI[19].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[20].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[20].penuTypeMovAttack = OperationTypeMovAttack.SPAttack;
			asrcCPUBOSS00Core.psrcAI[21].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[22].pintLoopCount = 55;
			asrcCPUBOSS00Core.psrcAI[22].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[23].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[23].penuTypeMovAttack = OperationTypeMovAttack.Attack;
			asrcCPUBOSS00Core.psrcAI[24].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[25].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[25].penuTypeMovAttack = OperationTypeMovAttack.SPAttack;
			asrcCPUBOSS00Core.psrcAI[26].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[27].pintLoopCount = 20;
			asrcCPUBOSS00Core.psrcAI[27].penuTypeMovY = OperationTypeMovY.Ym;
			asrcCPUBOSS00Core.psrcAI[28].pintLoopCount = 55;
			asrcCPUBOSS00Core.psrcAI[28].penuTypeMovX = OperationTypeMovX.Xp;
			for (int i = 0; i < 4; i += 2)
			{
				asrcCPUBOSS00Core.psrcAI[29 + i].pintLoopCount = 0;
				asrcCPUBOSS00Core.psrcAI[29 + i].penuTypeMovAttack = OperationTypeMovAttack.Attack;
				asrcCPUBOSS00Core.psrcAI[29 + i + 1].pintLoopCount = 30;
			}
			asrcCPUBOSS00Core.psrcAI[33].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[33].penuTypeMovAttack = OperationTypeMovAttack.SPAttack;
			asrcCPUBOSS00Core.psrcAI[34].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[35].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[35].penuTypeMovX = OperationTypeMovX.Xp;
			asrcCPUBOSS00Core.psrcAI[36].pintLoopCount = 20;
			asrcCPUBOSS00Core.psrcAI[36].penuTypeMovY = OperationTypeMovY.Yp;
			asrcCPUBOSS00Core.psrcAI[37].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[37].penuTypeMovX = OperationTypeMovX.Xp;
			asrcCPUBOSS00Core.psrcAI[38].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[38].penuTypeMovAttack = OperationTypeMovAttack.Attack;
			asrcCPUBOSS00Core.psrcAI[39].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[40].pintLoopCount = 0;
			asrcCPUBOSS00Core.psrcAI[40].penuTypeMovAttack = OperationTypeMovAttack.SPAttack;
			asrcCPUBOSS00Core.psrcAI[41].pintLoopCount = 30;
			asrcCPUBOSS00Core.psrcAI[42].pintLoopCount = 60;
			asrcCPUBOSS00Core.psrcAI[42].penuTypeMovX = OperationTypeMovX.Xm;
			asrcCPUBOSS00Core.psrcAI[43].pintLoopCount = 20;
			asrcCPUBOSS00Core.psrcAI[43].penuTypeMovY = OperationTypeMovY.Ym;
		}
	}

	private void kakuCPUBOSSChild00Update(int aintCPUNo)
	{
		CPUBOSSChild00AISift(aintCPUNo);
		switch (psrcCPUBOSSChild00Core[aintCPUNo].penuType)
		{
		case enuCPUBOSSChild00Type.intOSPREYChild00:
			NomarlChildStateSet(aintCPUNo);
			break;
		case enuCPUBOSSChild00Type.intOSPREYChildX00:
			NomarlChildStateSet(aintCPUNo);
			break;
		case enuCPUBOSSChild00Type.intDragonBody:
			DragonChild00AI(ref psrcCPUBOSSChild00Core[aintCPUNo]);
			CPUBOSSDragonChild00Update(aintCPUNo);
			NomarlChildStateSet(aintCPUNo);
			break;
		}
	}

	private void CPUBOSSChild00AISift(int aintCPUNo)
	{
		if (psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[0].pintLoopCount > 0)
		{
			psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[0].pintLoopCount--;
			return;
		}
		for (int i = 0; i < psrcCPUBOSSChild00Core[aintCPUNo].psrcAI.Length - 1; i++)
		{
			ref structCPUBOSS00AI reference = ref psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[i];
			reference = psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[i + 1];
		}
		int num = psrcCPUBOSSChild00Core[aintCPUNo].psrcAI.Length - 1;
		psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[num].pintLoopCount = 0;
		psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[num].penuTypeMovX = OperationTypeMovX.None;
		psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[num].penuTypeMovY = OperationTypeMovY.None;
		psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[num].penuTypeMovZ = OperationTypeMovZ.None;
		psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[num].penuTypeMovXDash = OperationTypeMovXDash.None;
		psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[num].penuTypeMovYDash = OperationTypeMovYDash.None;
		psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[num].penuTypeMovZDash = OperationTypeMovZDash.None;
		psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[num].penuTypeMovAttack = OperationTypeMovAttack.None;
		psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[num].penuTypeMuki = OperationTypeMukiX.None;
	}

	private void BOSSChildMov(Vector3 avecMov, float afltR, enuCPUBOSSChild00Type aenuType)
	{
		for (int i = 0; i < psrcCPUBOSSChild00Core.Length; i++)
		{
			if (psrcCPUBOSSChild00Core[i].pflgEnable && psrcCPUBOSSChild00Core[i].penuType == aenuType)
			{
				psrcCPUBOSSChild00Core[i].pVecIti.X += avecMov.X;
				psrcCPUBOSSChild00Core[i].pVecIti.Y += avecMov.Y;
				psrcCPUBOSSChild00Core[i].pVecIti.Z += avecMov.Z;
				psrcCPUBOSSChild00Core[i].pfltItiR = afltR;
			}
		}
	}

	private bool BOSSAIEndHantei(structCPUBOSS00AI[] asrcCPUBOSS00AI)
	{
		for (int i = 0; i < asrcCPUBOSS00AI.Length; i++)
		{
			if (asrcCPUBOSS00AI[i].pintLoopCount != 0 || asrcCPUBOSS00AI[i].penuTypeMovX != OperationTypeMovX.None || asrcCPUBOSS00AI[i].penuTypeMovY != OperationTypeMovY.None || asrcCPUBOSS00AI[i].penuTypeMovZ != OperationTypeMovZ.None || asrcCPUBOSS00AI[i].penuTypeMovXDash != OperationTypeMovXDash.None || asrcCPUBOSS00AI[i].penuTypeMovYDash != OperationTypeMovYDash.None || asrcCPUBOSS00AI[i].penuTypeMovZDash != OperationTypeMovZDash.None || asrcCPUBOSS00AI[i].penuTypeMovAttack != OperationTypeMovAttack.None || asrcCPUBOSS00AI[i].penuTypeMuki != OperationTypeMukiX.None)
			{
				return false;
			}
		}
		return true;
	}

	private void CPUBOSS00OSPREYUpdate(int aintCPUNo)
	{
		CPUBOSS00Normal00OpeUpdate(aintCPUNo);
		switch (psrcCPUBOSS00Core[aintCPUNo].enuMovState)
		{
		case enuCPUBOSS00MovState.intNormal:
			break;
		case enuCPUBOSS00MovState.intMove:
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X *= 1f;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y *= 1f;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y += rnd.Next(1, 6) - 3;
			psrcCPUBOSS00Core[aintCPUNo].pVecIti.X += psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X;
			psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y += psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y;
			psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z += psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z;
			if (psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X > 0f)
			{
				BOSSChildMov(psrcCPUBOSS00Core[aintCPUNo].pVecMovIti, 15f, enuCPUBOSSChild00Type.intOSPREYChild00);
				BOSSChildMov(psrcCPUBOSS00Core[aintCPUNo].pVecMovIti, 15f, enuCPUBOSSChild00Type.intOSPREYChildX00);
			}
			else if (psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X < 0f)
			{
				BOSSChildMov(psrcCPUBOSS00Core[aintCPUNo].pVecMovIti, 345f, enuCPUBOSSChild00Type.intOSPREYChildX00);
				BOSSChildMov(psrcCPUBOSS00Core[aintCPUNo].pVecMovIti, 345f, enuCPUBOSSChild00Type.intOSPREYChild00);
			}
			else if (psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y != 0f)
			{
				BOSSChildMov(psrcCPUBOSS00Core[aintCPUNo].pVecMovIti, 345f, enuCPUBOSSChild00Type.intOSPREYChildX00);
				BOSSChildMov(psrcCPUBOSS00Core[aintCPUNo].pVecMovIti, 15f, enuCPUBOSSChild00Type.intOSPREYChild00);
			}
			if ((int)psrcCPUBOSS00Core[aintCPUNo].pVecIti.X % 40 == 0 && psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X > 0f)
			{
				Game1.bakuhatu.psrcBakuhatu01CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintCPUNo].pVecIti.X + 256f - 64f + 64f, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y + 196f, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z - 1f / 128f), 15f, 0);
				Game1.bakuhatu.psrcBakuhatu01CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintCPUNo].pVecIti.X - 256f + 64f + 64f, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y + 196f, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z - 1f / 128f), 15f, 0);
			}
			else if ((int)psrcCPUBOSS00Core[aintCPUNo].pVecIti.X % 40 == 0 && psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X < 0f)
			{
				Game1.bakuhatu.psrcBakuhatu01CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintCPUNo].pVecIti.X + 256f - 64f, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y + 196f, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z - 1f / 128f), 345f, 0);
				Game1.bakuhatu.psrcBakuhatu01CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintCPUNo].pVecIti.X - 256f + 64f, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y + 196f, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z - 1f / 128f), 345f, 0);
			}
			else if ((int)psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y % 40 == 0 && psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y != 0f)
			{
				Game1.bakuhatu.psrcBakuhatu01CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintCPUNo].pVecIti.X + 256f - 64f, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y + 196f, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z - 1f / 128f), 345f, 0);
				Game1.bakuhatu.psrcBakuhatu01CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintCPUNo].pVecIti.X - 256f + 64f, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y + 196f, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z - 1f / 128f), 15f, 0);
			}
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X = 0f;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y = 0f;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z = 0f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intNormal;
			break;
		case enuCPUBOSS00MovState.intDashMove:
			break;
		case enuCPUBOSS00MovState.intAttack:
			Game1.cPUTama.psrcCPUTamaCorePlayerItiEnable(psrcCPUBOSS00Core[aintCPUNo].pVecIti, new Vector3(0f, 0f, 3f / 128f), Game1.player.psrcPlayerCore.pVecIti);
			Game1.bakuhatu.psrcKemuriCoreConboEnable(psrcCPUBOSS00Core[aintCPUNo].pVecIti, psrcCPUBOSS00Core[aintCPUNo].pfltItiR, 0);
			break;
		case enuCPUBOSS00MovState.intDamage:
			break;
		case enuCPUBOSS00MovState.intDead:
			Game1.cPUPort00.pSubBossTaosita();
			KakuCPUBOSS00Init(aintCPUNo);
			pCPUBOSSChild00Init();
			break;
		case enuCPUBOSS00MovState.intSPAttack:
			break;
		}
	}

	private void CPUBOSSDragon00Update(int aintCPUNo)
	{
		CPUBOSS00Normal00OpeUpdate(aintCPUNo);
		switch (psrcCPUBOSS00Core[aintCPUNo].enuMovState)
		{
		case enuCPUBOSS00MovState.intNormal:
			break;
		case enuCPUBOSS00MovState.intMove:
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X *= 1f;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y *= 1f;
			if (psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z > 0f)
			{
				psrcCPUBOSS00Core[aintCPUNo].penuImgStateHoji = enuCPUBOSS00ImgStateHoji.intFront;
			}
			else if (psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z < 0f)
			{
				psrcCPUBOSS00Core[aintCPUNo].penuImgStateHoji = enuCPUBOSS00ImgStateHoji.intBack;
			}
			psrcCPUBOSS00Core[aintCPUNo].pVecIti.X += psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X;
			psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y += psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y;
			psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z += psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X = 0f;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y = 0f;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z = 0f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intNormal;
			break;
		case enuCPUBOSS00MovState.intDashMove:
			break;
		case enuCPUBOSS00MovState.intAttack:
			Game1.cPUTama.psrcCPUTamaCorePlayerItiEnable(psrcCPUBOSS00Core[aintCPUNo].pVecIti, new Vector3(0f, 0f, 3f / 128f), Game1.player.psrcPlayerCore.pVecIti);
			Game1.bakuhatu.psrcKemuriCoreConboEnable(psrcCPUBOSS00Core[aintCPUNo].pVecIti, psrcCPUBOSS00Core[aintCPUNo].pfltItiR, 0);
			break;
		case enuCPUBOSS00MovState.intDamage:
			break;
		case enuCPUBOSS00MovState.intDead:
			Game1.cPUPort00.pSubBossTaosita();
			KakuCPUBOSS00Init(aintCPUNo);
			pCPUBOSSChild00Init();
			break;
		case enuCPUBOSS00MovState.intSPAttack:
			break;
		}
	}

	private void CPUBOSSArshipUpdate(int aintCPUNo)
	{
		CPUBOSS00Arship00OpeUpdate(aintCPUNo);
		switch (psrcCPUBOSS00Core[aintCPUNo].enuMovState)
		{
		case enuCPUBOSS00MovState.intNormal:
			break;
		case enuCPUBOSS00MovState.intMove:
		{
			int num = 0;
			num = intCPUBOSS00EnableSearch(enuCPUBOSS00Type.intArshipTate);
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X *= 0.9f;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y *= 1f;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z *= 0.5f;
			if (num != psrcCPUBOSS00Core.Length)
			{
				if (psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X > 0f)
				{
					psrcCPUBOSS00Core[num].penuImgStateHoji = enuCPUBOSS00ImgStateHoji.intRight;
				}
				else if (psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X < 0f)
				{
					psrcCPUBOSS00Core[num].penuImgStateHoji = enuCPUBOSS00ImgStateHoji.intLeft;
				}
				psrcCPUBOSS00Core[num].pVecIti.X += psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X;
				psrcCPUBOSS00Core[num].pVecIti.Y += psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y;
				psrcCPUBOSS00Core[num].pVecIti.Z += psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z;
				psrcCPUBOSS00Core[num].pSpriteEffects = psrcCPUBOSS00Core[aintCPUNo].pSpriteEffects;
			}
			if (psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X > 0f)
			{
				psrcCPUBOSS00Core[aintCPUNo].penuImgStateHoji = enuCPUBOSS00ImgStateHoji.intRight;
			}
			else if (psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X < 0f)
			{
				psrcCPUBOSS00Core[aintCPUNo].penuImgStateHoji = enuCPUBOSS00ImgStateHoji.intLeft;
			}
			psrcCPUBOSS00Core[aintCPUNo].pVecIti.X += psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X;
			psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y += psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y;
			psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z += psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X = 0f;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y = 0f;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z = 0f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intNormal;
			break;
		}
		case enuCPUBOSS00MovState.intDashMove:
			break;
		case enuCPUBOSS00MovState.intAttack:
			if (isCPUBOSS00EnableSearch(enuCPUBOSS00Type.intArshipTate))
			{
				Game1.cPUTama.psrcCPUTamaCorePlayerItiEnable(psrcCPUBOSS00Core[aintCPUNo].pVecIti, new Vector3(0f, 0f, 3f / 128f), Game1.player.psrcPlayerCore.pVecIti);
				Game1.bakuhatu.psrcKemuriCoreConboEnable(psrcCPUBOSS00Core[aintCPUNo].pVecIti, psrcCPUBOSS00Core[aintCPUNo].pfltItiR, 0);
				Game1.cPUTama.psrcCPUTamaCorePlayerItiEnable(new Vector3(psrcCPUBOSS00Core[aintCPUNo].pVecIti.X + 64f, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z), new Vector3(0f, 0f, 3f / 128f), Game1.player.psrcPlayerCore.pVecIti);
				Game1.bakuhatu.psrcKemuriCoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintCPUNo].pVecIti.X + 64f, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z), psrcCPUBOSS00Core[aintCPUNo].pfltItiR, 0);
				Game1.cPUTama.psrcCPUTamaCorePlayerItiEnable(new Vector3(psrcCPUBOSS00Core[aintCPUNo].pVecIti.X - 64f, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z), new Vector3(0f, 0f, 3f / 128f), Game1.player.psrcPlayerCore.pVecIti);
				Game1.bakuhatu.psrcKemuriCoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintCPUNo].pVecIti.X - 64f, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z), psrcCPUBOSS00Core[aintCPUNo].pfltItiR, 0);
			}
			else
			{
				Game1.cPUTama.psrcCPUTamaCorePlayerItiEnable(psrcCPUBOSS00Core[aintCPUNo].pVecIti, new Vector3(0f, 0f, 3f / 128f), Game1.player.psrcPlayerCore.pVecIti);
				Game1.bakuhatu.psrcKemuriCoreConboEnable(psrcCPUBOSS00Core[aintCPUNo].pVecIti, psrcCPUBOSS00Core[aintCPUNo].pfltItiR, 0);
				for (int i = 0; i < 7; i++)
				{
					Game1.cPUTama.psrcCPUTamaCorePlayerItiEnable(new Vector3(psrcCPUBOSS00Core[aintCPUNo].pVecIti.X - 256f + (float)(i * 64), psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z), new Vector3(0f, 0f, 3f / 128f), Game1.player.psrcPlayerCore.pVecIti);
					Game1.bakuhatu.psrcKemuriCoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintCPUNo].pVecIti.X - 256f + (float)(i * 64), psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z), psrcCPUBOSS00Core[aintCPUNo].pfltItiR, 0);
				}
			}
			break;
		case enuCPUBOSS00MovState.intSPAttack:
			pCPUBOSS00Enable(new Vector3(psrcCPUBOSS00Core[aintCPUNo].pVecIti.X, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z + 1f / 32f), enuCPUBOSS00Type.intArshipTate, 400f, 10f);
			break;
		case enuCPUBOSS00MovState.intDamage:
			break;
		case enuCPUBOSS00MovState.intDead:
			KakuCPUBOSS00Init(aintCPUNo);
			pCPUBOSSChild00Init();
			break;
		}
	}

	private void CPUBOSSQueeenBeeUpdate(int aintCPUNo)
	{
		CPUBOSS00Normal00OpeUpdate(aintCPUNo);
		switch (psrcCPUBOSS00Core[aintCPUNo].enuMovState)
		{
		case enuCPUBOSS00MovState.intNormal:
			break;
		case enuCPUBOSS00MovState.intMove:
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X *= 1f;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y *= 1f;
			psrcCPUBOSS00Core[aintCPUNo].pVecIti.X += psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X;
			psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y += psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y;
			psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z += psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X = 0f;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y = 0f;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z = 0f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intNormal;
			break;
		case enuCPUBOSS00MovState.intDashMove:
			break;
		case enuCPUBOSS00MovState.intAttack:
			Game1.cPUTama.psrcCPUTamaCorePlayerItiEnable(psrcCPUBOSS00Core[aintCPUNo].pVecIti, new Vector3(0f, 0f, 3f / 128f), Game1.player.psrcPlayerCore.pVecIti);
			Game1.bakuhatu.psrcKemuriCoreConboEnable(psrcCPUBOSS00Core[aintCPUNo].pVecIti, psrcCPUBOSS00Core[aintCPUNo].pfltItiR, 0);
			break;
		case enuCPUBOSS00MovState.intSPAttack:
			pCPUBOSS00Enable(new Vector3(psrcCPUBOSS00Core[aintCPUNo].pVecIti.X, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z + 3f / 128f), enuCPUBOSS00Type.intHatiNosu, 500f, 10f);
			break;
		case enuCPUBOSS00MovState.intDamage:
			break;
		case enuCPUBOSS00MovState.intDead:
			KakuCPUBOSS00Init(aintCPUNo);
			pCPUBOSSChild00Init();
			break;
		}
	}

	private void CPUBOSSESPUpdate(int aintCPUNo)
	{
		CPUBOSS00Normal00OpeUpdate(aintCPUNo);
		switch (psrcCPUBOSS00Core[aintCPUNo].enuMovState)
		{
		case enuCPUBOSS00MovState.intNormal:
			psrcCPUBOSS00Core[aintCPUNo].penuImgStateHoji = enuCPUBOSS00ImgStateHoji.intNone;
			break;
		case enuCPUBOSS00MovState.intMove:
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X *= 1f;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y *= 1f;
			if (psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X != 0f || psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y != 0f || psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z != 0f)
			{
				psrcCPUBOSS00Core[aintCPUNo].penuImgStateHoji = enuCPUBOSS00ImgStateHoji.intMove;
			}
			else
			{
				psrcCPUBOSS00Core[aintCPUNo].penuImgStateHoji = enuCPUBOSS00ImgStateHoji.intNone;
			}
			psrcCPUBOSS00Core[aintCPUNo].pVecIti.X += psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X;
			psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y += psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y;
			psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z += psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X = 0f;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y = 0f;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z = 0f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intNormal;
			break;
		case enuCPUBOSS00MovState.intDashMove:
			break;
		case enuCPUBOSS00MovState.intAttack:
			Game1.cPUTama.psrcCPUTamaCorePlayerItiEnable(psrcCPUBOSS00Core[aintCPUNo].pVecIti, new Vector3(0f, 0f, 3f / 128f), Game1.player.psrcPlayerCore.pVecIti);
			Game1.bakuhatu.psrcKemuriCoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintCPUNo].pVecIti.X, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z + 1f / 128f), psrcCPUBOSS00Core[aintCPUNo].pfltItiR, 0);
			psrcCPUBOSS00Core[aintCPUNo].penuImgStateHoji = enuCPUBOSS00ImgStateHoji.intNone;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intNormal;
			break;
		case enuCPUBOSS00MovState.intSPAttack:
			Game1.bakuhatu.psrcBakuhatuCoreEnable(new Vector3(psrcCPUBOSS00Core[aintCPUNo].pVecIti.X, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z + 1f / 128f), 0f, Bakuhatu.enuBakuhatuType.intESPHand00, 0);
			if (psrcCPUBOSS00Core[aintCPUNo].pfltHP < 250f)
			{
				for (int i = 0; i < 12; i++)
				{
					Game1.syougai.pSyougaiESPHandEnable(new Vector3(psrcCPUBOSS00Core[aintCPUNo].pVecIti.X, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z - 1f / 128f), new Vector3(0f, 0f, 1f / 128f), new Vector3(15 * (2 - rnd.Next(0, 4)), 15 * (2 - rnd.Next(0, 4)), 0f), rnd.Next(1, 35), Syougai.enuSyougaiType.intESPIwa00);
				}
			}
			else if (psrcCPUBOSS00Core[aintCPUNo].pfltHP < 500f)
			{
				for (int i = 0; i < 6; i++)
				{
					Game1.syougai.pSyougaiESPHandEnable(new Vector3(psrcCPUBOSS00Core[aintCPUNo].pVecIti.X, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z - 1f / 128f), new Vector3(0f, 0f, 1f / 128f), new Vector3(15 * (2 - rnd.Next(0, 4)), 15 * (2 - rnd.Next(0, 4)), 0f), rnd.Next(1, 18) + 5, Syougai.enuSyougaiType.intESPIwa00);
				}
			}
			else if (psrcCPUBOSS00Core[aintCPUNo].pfltHP < 750f)
			{
				for (int i = 0; i < 4; i++)
				{
					Game1.syougai.pSyougaiESPHandEnable(new Vector3(psrcCPUBOSS00Core[aintCPUNo].pVecIti.X, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z - 1f / 128f), new Vector3(0f, 0f, 1f / 128f), new Vector3(15 * (2 - rnd.Next(0, 4)), 15 * (2 - rnd.Next(0, 4)), 0f), rnd.Next(1, 10) + 8, Syougai.enuSyougaiType.intESPIwa00);
				}
			}
			else
			{
				for (int i = 0; i < 2; i++)
				{
					Game1.syougai.pSyougaiESPHandEnable(new Vector3(psrcCPUBOSS00Core[aintCPUNo].pVecIti.X, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Y, psrcCPUBOSS00Core[aintCPUNo].pVecIti.Z - 1f / 128f), new Vector3(0f, 0f, 1f / 128f), new Vector3(15 * (2 - rnd.Next(0, 4)), 15 * (2 - rnd.Next(0, 4)), 0f), rnd.Next(1, 10) + 8, Syougai.enuSyougaiType.intESPIwa00);
				}
			}
			psrcCPUBOSS00Core[aintCPUNo].penuImgStateHoji = enuCPUBOSS00ImgStateHoji.intNone;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intNormal;
			break;
		case enuCPUBOSS00MovState.intDamage:
			break;
		case enuCPUBOSS00MovState.intDead:
			KakuCPUBOSS00Init(aintCPUNo);
			pCPUBOSSChild00Init();
			break;
		}
	}

	private void CPUBOSS00Normal00OpeUpdate(int aintCPUNo)
	{
		if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovZ == OperationTypeMovZ.Zm)
		{
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z = -1f / 128f;
		}
		else if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovZ == OperationTypeMovZ.Zp)
		{
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z = 1f / 128f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
		}
		if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovX == OperationTypeMovX.Xm)
		{
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X = -15f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
		}
		else if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovX == OperationTypeMovX.Xp)
		{
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X = 15f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
		}
		if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovY == OperationTypeMovY.Ym)
		{
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y = -15f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
		}
		else if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovY == OperationTypeMovY.Yp)
		{
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y = 15f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
		}
		if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovZDash == OperationTypeMovZDash.ZmDash)
		{
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z = -1f / 64f;
		}
		else if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovZDash == OperationTypeMovZDash.ZpDash)
		{
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z = 1f / 64f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
		}
		if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovXDash == OperationTypeMovXDash.XmDash)
		{
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X = -30f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
		}
		else if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovXDash == OperationTypeMovXDash.XpDash)
		{
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X = 30f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
		}
		if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovYDash == OperationTypeMovYDash.YmDash)
		{
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y = -30f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
		}
		else if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovYDash == OperationTypeMovYDash.YpDash)
		{
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y = 30f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
		}
		if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMuki == OperationTypeMukiX.Y)
		{
			if (psrcCPUBOSS00Core[aintCPUNo].pSpriteEffects == SpriteEffects.None)
			{
				psrcCPUBOSS00Core[aintCPUNo].pSpriteEffects = SpriteEffects.FlipVertically;
			}
			else
			{
				psrcCPUBOSS00Core[aintCPUNo].pSpriteEffects = SpriteEffects.None;
			}
		}
		else if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMuki == OperationTypeMukiX.X)
		{
			if (psrcCPUBOSS00Core[aintCPUNo].pSpriteEffects == SpriteEffects.None)
			{
				psrcCPUBOSS00Core[aintCPUNo].pSpriteEffects = SpriteEffects.FlipHorizontally;
			}
			else
			{
				psrcCPUBOSS00Core[aintCPUNo].pSpriteEffects = SpriteEffects.None;
			}
		}
		if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovAttack == OperationTypeMovAttack.Attack)
		{
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intAttack;
		}
		else if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovAttack == OperationTypeMovAttack.SPAttack)
		{
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intSPAttack;
		}
		if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeDead == OperationTypeDead.Dead)
		{
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intDead;
		}
	}

	private void CPUBOSS00Arship00OpeUpdate(int aintCPUNo)
	{
		if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovZ == OperationTypeMovZ.Zm)
		{
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z = -1f / 128f;
		}
		else if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovZ == OperationTypeMovZ.Zp)
		{
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z = 1f / 128f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
		}
		if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovX == OperationTypeMovX.Xm)
		{
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X = -15f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
		}
		else if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovX == OperationTypeMovX.Xp)
		{
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X = 15f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
		}
		if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovY == OperationTypeMovY.Ym)
		{
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y = -15f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
		}
		else if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovY == OperationTypeMovY.Yp)
		{
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y = 15f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
		}
		if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovZDash == OperationTypeMovZDash.ZmDash)
		{
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z = -1f / 64f;
		}
		else if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovZDash == OperationTypeMovZDash.ZpDash)
		{
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Z = 1f / 64f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
		}
		if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovXDash == OperationTypeMovXDash.XmDash)
		{
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X = -30f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
		}
		else if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovXDash == OperationTypeMovXDash.XpDash)
		{
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.X = 30f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
		}
		if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovYDash == OperationTypeMovYDash.YmDash)
		{
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y = -30f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
		}
		else if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovYDash == OperationTypeMovYDash.YpDash)
		{
			psrcCPUBOSS00Core[aintCPUNo].pVecMovIti.Y = 30f;
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intMove;
		}
		if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMuki == OperationTypeMukiX.Y)
		{
			if (psrcCPUBOSS00Core[aintCPUNo].pSpriteEffects == SpriteEffects.None)
			{
				psrcCPUBOSS00Core[aintCPUNo].pSpriteEffects = SpriteEffects.FlipVertically;
			}
			else
			{
				psrcCPUBOSS00Core[aintCPUNo].pSpriteEffects = SpriteEffects.None;
			}
		}
		else if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMuki == OperationTypeMukiX.X)
		{
			if (psrcCPUBOSS00Core[aintCPUNo].pSpriteEffects == SpriteEffects.None)
			{
				psrcCPUBOSS00Core[aintCPUNo].pSpriteEffects = SpriteEffects.FlipHorizontally;
			}
			else
			{
				psrcCPUBOSS00Core[aintCPUNo].pSpriteEffects = SpriteEffects.None;
			}
			int num = 0;
			NomarlStartStateSet(aintCPUNo);
			num = intCPUBOSS00EnableSearch(enuCPUBOSS00Type.intArshipTate);
			if (num != psrcCPUBOSS00Core.Length)
			{
				NomarlStartStateSet(num);
			}
		}
		if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovAttack == OperationTypeMovAttack.Attack)
		{
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intAttack;
		}
		else if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeMovAttack == OperationTypeMovAttack.SPAttack)
		{
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intSPAttack;
		}
		if (psrcCPUBOSS00Core[aintCPUNo].psrcAI[0].penuTypeDead == OperationTypeDead.Dead)
		{
			psrcCPUBOSS00Core[aintCPUNo].enuMovState = enuCPUBOSS00MovState.intDead;
		}
	}

	private void CPUBOSSChild00Normal00OpeUpdate(int aintCPUNo)
	{
		if (psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[0].penuTypeMovZ == OperationTypeMovZ.Zm)
		{
			psrcCPUBOSSChild00Core[aintCPUNo].enuMovState = enuCPUBOSSChild00MovState.intMove;
			psrcCPUBOSSChild00Core[aintCPUNo].pVecMovIti.Z = -1f / 128f;
		}
		else if (psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[0].penuTypeMovZ == OperationTypeMovZ.Zp)
		{
			psrcCPUBOSSChild00Core[aintCPUNo].pVecMovIti.Z = 1f / 128f;
			psrcCPUBOSSChild00Core[aintCPUNo].enuMovState = enuCPUBOSSChild00MovState.intMove;
		}
		if (psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[0].penuTypeMovX == OperationTypeMovX.Xm)
		{
			psrcCPUBOSSChild00Core[aintCPUNo].pVecMovIti.X = -15f;
			psrcCPUBOSSChild00Core[aintCPUNo].enuMovState = enuCPUBOSSChild00MovState.intMove;
		}
		else if (psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[0].penuTypeMovX == OperationTypeMovX.Xp)
		{
			psrcCPUBOSSChild00Core[aintCPUNo].pVecMovIti.X = 15f;
			psrcCPUBOSSChild00Core[aintCPUNo].enuMovState = enuCPUBOSSChild00MovState.intMove;
		}
		if (psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[0].penuTypeMovY == OperationTypeMovY.Ym)
		{
			psrcCPUBOSSChild00Core[aintCPUNo].pVecMovIti.Y = -15f;
			psrcCPUBOSSChild00Core[aintCPUNo].enuMovState = enuCPUBOSSChild00MovState.intMove;
		}
		else if (psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[0].penuTypeMovY == OperationTypeMovY.Yp)
		{
			psrcCPUBOSSChild00Core[aintCPUNo].pVecMovIti.Y = 15f;
			psrcCPUBOSSChild00Core[aintCPUNo].enuMovState = enuCPUBOSSChild00MovState.intMove;
		}
		if (psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[0].penuTypeMovZDash == OperationTypeMovZDash.ZmDash)
		{
			psrcCPUBOSSChild00Core[aintCPUNo].enuMovState = enuCPUBOSSChild00MovState.intMove;
			psrcCPUBOSSChild00Core[aintCPUNo].pVecMovIti.Z = -1f / 64f;
		}
		else if (psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[0].penuTypeMovZDash == OperationTypeMovZDash.ZpDash)
		{
			psrcCPUBOSSChild00Core[aintCPUNo].pVecMovIti.Z = 1f / 64f;
			psrcCPUBOSSChild00Core[aintCPUNo].enuMovState = enuCPUBOSSChild00MovState.intMove;
		}
		if (psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[0].penuTypeMovXDash == OperationTypeMovXDash.XmDash)
		{
			psrcCPUBOSSChild00Core[aintCPUNo].pVecMovIti.X = -30f;
			psrcCPUBOSSChild00Core[aintCPUNo].enuMovState = enuCPUBOSSChild00MovState.intMove;
		}
		else if (psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[0].penuTypeMovXDash == OperationTypeMovXDash.XpDash)
		{
			psrcCPUBOSSChild00Core[aintCPUNo].pVecMovIti.X = 30f;
			psrcCPUBOSSChild00Core[aintCPUNo].enuMovState = enuCPUBOSSChild00MovState.intMove;
		}
		if (psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[0].penuTypeMovYDash == OperationTypeMovYDash.YmDash)
		{
			psrcCPUBOSSChild00Core[aintCPUNo].pVecMovIti.Y = -30f;
			psrcCPUBOSSChild00Core[aintCPUNo].enuMovState = enuCPUBOSSChild00MovState.intMove;
		}
		else if (psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[0].penuTypeMovYDash == OperationTypeMovYDash.YpDash)
		{
			psrcCPUBOSSChild00Core[aintCPUNo].pVecMovIti.Y = 30f;
			psrcCPUBOSSChild00Core[aintCPUNo].enuMovState = enuCPUBOSSChild00MovState.intMove;
		}
		if (psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[0].penuTypeMuki == OperationTypeMukiX.Y)
		{
			if (psrcCPUBOSSChild00Core[aintCPUNo].pSpriteEffects == SpriteEffects.None)
			{
				psrcCPUBOSSChild00Core[aintCPUNo].pSpriteEffects = SpriteEffects.FlipVertically;
			}
			else
			{
				psrcCPUBOSSChild00Core[aintCPUNo].pSpriteEffects = SpriteEffects.None;
			}
		}
		if (psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[0].penuTypeMovAttack == OperationTypeMovAttack.Attack)
		{
			psrcCPUBOSSChild00Core[aintCPUNo].enuMovState = enuCPUBOSSChild00MovState.intAttack;
		}
		if (psrcCPUBOSSChild00Core[aintCPUNo].psrcAI[0].penuTypeDead == OperationTypeDead.Dead)
		{
			psrcCPUBOSSChild00Core[aintCPUNo].enuMovState = enuCPUBOSSChild00MovState.intDead;
		}
	}

	private void CPUBOSSDragonChild00Update(int aintCPUNo)
	{
		CPUBOSSChild00Normal00OpeUpdate(aintCPUNo);
		switch (psrcCPUBOSSChild00Core[aintCPUNo].enuMovState)
		{
		case enuCPUBOSSChild00MovState.intNormal:
			break;
		case enuCPUBOSSChild00MovState.intMove:
			psrcCPUBOSSChild00Core[aintCPUNo].pVecMovIti.X *= 1f;
			psrcCPUBOSSChild00Core[aintCPUNo].pVecMovIti.Y *= 1f;
			psrcCPUBOSSChild00Core[aintCPUNo].pVecIti.X += psrcCPUBOSSChild00Core[aintCPUNo].pVecMovIti.X;
			psrcCPUBOSSChild00Core[aintCPUNo].pVecIti.Y += psrcCPUBOSSChild00Core[aintCPUNo].pVecMovIti.Y;
			psrcCPUBOSSChild00Core[aintCPUNo].pVecIti.Z += psrcCPUBOSSChild00Core[aintCPUNo].pVecMovIti.Z;
			psrcCPUBOSSChild00Core[aintCPUNo].pVecMovIti.X = 0f;
			psrcCPUBOSSChild00Core[aintCPUNo].pVecMovIti.Y = 0f;
			psrcCPUBOSSChild00Core[aintCPUNo].pVecMovIti.Z = 0f;
			psrcCPUBOSSChild00Core[aintCPUNo].enuMovState = enuCPUBOSSChild00MovState.intNormal;
			break;
		case enuCPUBOSSChild00MovState.intDashMove:
			break;
		case enuCPUBOSSChild00MovState.intAttack:
			Game1.cPUTama.psrcCPUTamaCorePlayerItiEnable(psrcCPUBOSSChild00Core[aintCPUNo].pVecIti, new Vector3(0f, 0f, 3f / 128f), Game1.player.psrcPlayerCore.pVecIti);
			Game1.bakuhatu.psrcKemuriCoreConboEnable(psrcCPUBOSSChild00Core[aintCPUNo].pVecIti, psrcCPUBOSSChild00Core[aintCPUNo].pfltItiR, 0);
			break;
		case enuCPUBOSSChild00MovState.intDamage:
			break;
		case enuCPUBOSSChild00MovState.intDead:
			KakuCPUBOSSChild00Init(aintCPUNo);
			break;
		}
	}

	public bool pCPUBOSS00TamaHantei(Vector3 avecIti, float afltHabaHantei, Rectangle arecHantei, bool aflgVulcan)
	{
		bool result = false;
		Rectangle arecHantei2 = new Rectangle
		{
			X = arecHantei.X + (int)avecIti.X,
			Y = arecHantei.Y + (int)avecIti.Y,
			Width = arecHantei.Width,
			Height = arecHantei.Height
		};
		for (int i = 0; i < psrcCPUBOSS00Core.Length; i++)
		{
			if (!psrcCPUBOSS00Core[i].pflgEnable || ((!(avecIti.Z <= psrcCPUBOSS00Core[i].pVecIti.Z) || !(avecIti.Z + afltHabaHantei >= psrcCPUBOSS00Core[i].pVecIti.Z)) && (!(avecIti.Z <= psrcCPUBOSS00Core[i].pVecIti.Z + fltOffSetHaba[(int)psrcCPUBOSS00Core[i].penuType]) || !(avecIti.Z + afltHabaHantei >= psrcCPUBOSS00Core[i].pVecIti.Z + fltOffSetHaba[(int)psrcCPUBOSS00Core[i].penuType]))))
			{
				continue;
			}
			switch (psrcCPUBOSS00Core[i].penuType)
			{
			case enuCPUBOSS00Type.intOSPREY00:
				if (OSPREYTamaHantei(i, arecHantei2, aflgVulcan))
				{
					return true;
				}
				if (OSPREYNoDamageTamaHantei(i, arecHantei2))
				{
					return true;
				}
				break;
			case enuCPUBOSS00Type.intDragon:
				if (DragonTamaHantei(i, arecHantei2, aflgVulcan))
				{
					return true;
				}
				break;
			case enuCPUBOSS00Type.intQueenBee:
				if (QueenBeeTamaHantei(i, arecHantei2, aflgVulcan))
				{
					return true;
				}
				break;
			case enuCPUBOSS00Type.intHatiNosu:
				if (HatiNoSuTamaHantei(i, arecHantei2, aflgVulcan))
				{
					return true;
				}
				break;
			case enuCPUBOSS00Type.intArshipTate:
				if (ArshipTateTamaHantei(i, arecHantei2, aflgVulcan))
				{
					return true;
				}
				break;
			case enuCPUBOSS00Type.intArship:
				if (ArshipTamaHantei(i, arecHantei2, aflgVulcan))
				{
					return true;
				}
				break;
			case enuCPUBOSS00Type.intESP:
				if (ESPTamaHantei(i, arecHantei2, aflgVulcan))
				{
					return true;
				}
				break;
			}
		}
		return result;
	}

	private bool OSPREYTamaHantei(int aintBOSSNo, Rectangle arecHantei, bool aflgVulcan)
	{
		for (int i = 0; i < precCPUBOSS00OffSet.GetUpperBound(1) && (precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].X != 0 || precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Y != 0 || precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Width != 0 || precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Height != 0); i++)
		{
			if (!arecHantei.Intersects(new Rectangle((int)psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].X, (int)psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y + precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Y, precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Width, precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Height)))
			{
				continue;
			}
			DamageStartStateSet(aintBOSSNo);
			for (int j = 0; j < psrcCPUBOSS00Core[aintBOSSNo].pintChildMax; j++)
			{
				DamageChildStartStateSet(j);
			}
			if (aflgVulcan)
			{
				if (psrcCPUBOSS00Core[aintBOSSNo].pfltDamage <= 2f)
				{
					psrcCPUBOSS00Core[aintBOSSNo].pfltDamage = 5f;
				}
				else
				{
					psrcCPUBOSS00Core[aintBOSSNo].pfltDamage *= 1.4f;
				}
			}
			else
			{
				psrcCPUBOSS00Core[aintBOSSNo].pfltHP -= 10f;
			}
			Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(arecHantei.X, arecHantei.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
			Game1.bGM.pflgSEBakuhatu[1] = true;
			if (psrcCPUBOSS00Core[aintBOSSNo].pfltHP <= 0f)
			{
				Game1.score.pScoreUp(lngScoreUp[(int)psrcCPUBOSS00Core[aintBOSSNo].penuType]);
				Game1.cPUPort00.pSubBossTaosita();
				Game1.bGM.pflgSEBakuhatu[2] = true;
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(psrcCPUBOSS00Core[aintBOSSNo].pVecIti, psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X - 256f + 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + 256f - 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X - 128f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 20);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + 128f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 20);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X - 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 40);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 40);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(psrcCPUBOSS00Core[aintBOSSNo].pVecIti, psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 60);
				KakuCPUBOSS00Init(aintBOSSNo);
				pCPUBOSSChild00Init();
			}
			return true;
		}
		return false;
	}

	private bool ArshipTamaHantei(int aintBOSSNo, Rectangle arecHantei, bool aflgVulcan)
	{
		for (int i = 0; i < precCPUBOSS00OffSet.GetUpperBound(1) && (precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].X != 0 || precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Y != 0 || precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Width != 0 || precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Height != 0); i++)
		{
			if (!arecHantei.Intersects(new Rectangle((int)psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].X, (int)psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y + precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Y, precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Width, precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Height)))
			{
				continue;
			}
			DamageStartStateSet(aintBOSSNo);
			for (int j = 0; j < psrcCPUBOSS00Core[aintBOSSNo].pintChildMax; j++)
			{
				DamageChildStartStateSet(j);
			}
			if (aflgVulcan)
			{
				if (psrcCPUBOSS00Core[aintBOSSNo].pfltDamage <= 1f)
				{
					psrcCPUBOSS00Core[aintBOSSNo].pfltDamage = 3f;
				}
				else
				{
					psrcCPUBOSS00Core[aintBOSSNo].pfltDamage *= 1.05f;
				}
			}
			else
			{
				psrcCPUBOSS00Core[aintBOSSNo].pfltHP -= 8f;
			}
			Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(arecHantei.X, arecHantei.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
			Game1.bGM.pflgSEBakuhatu[1] = true;
			if (psrcCPUBOSS00Core[aintBOSSNo].pfltHP <= 0f)
			{
				Game1.bGM.pflgSEBakuhatu[2] = true;
				Game1.score.pScoreUp(lngScoreUp[(int)psrcCPUBOSS00Core[aintBOSSNo].penuType]);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(psrcCPUBOSS00Core[aintBOSSNo].pVecIti, psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X - 256f + 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + 256f - 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X - 128f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 20);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + 128f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 20);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X - 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 40);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 40);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(psrcCPUBOSS00Core[aintBOSSNo].pVecIti, psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 60);
				KakuCPUBOSS00Init(aintBOSSNo);
				pCPUBOSSChild00Init();
			}
			return true;
		}
		return false;
	}

	private bool ArshipTateTamaHantei(int aintBOSSNo, Rectangle arecHantei, bool aflgVulcan)
	{
		for (int i = 0; i < precCPUBOSS00OffSet.GetUpperBound(1) && (precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].X != 0 || precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Y != 0 || precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Width != 0 || precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Height != 0); i++)
		{
			if (!arecHantei.Intersects(new Rectangle((int)psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].X, (int)psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y + precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Y, precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Width, precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Height)))
			{
				continue;
			}
			DamageStartStateSet(aintBOSSNo);
			for (int j = 0; j < psrcCPUBOSS00Core[aintBOSSNo].pintChildMax; j++)
			{
				DamageChildStartStateSet(j);
			}
			if (aflgVulcan)
			{
				if (psrcCPUBOSS00Core[aintBOSSNo].pfltDamage <= 2f)
				{
					psrcCPUBOSS00Core[aintBOSSNo].pfltDamage = 5f;
				}
				else
				{
					psrcCPUBOSS00Core[aintBOSSNo].pfltDamage *= 1.4f;
				}
			}
			else
			{
				psrcCPUBOSS00Core[aintBOSSNo].pfltHP -= 15f;
			}
			Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(arecHantei.X, arecHantei.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
			Game1.bGM.pflgSEBakuhatu[1] = true;
			if (psrcCPUBOSS00Core[aintBOSSNo].pfltHP <= 0f)
			{
				Game1.bGM.pflgSEBakuhatu[2] = true;
				Game1.score.pScoreUp(lngScoreUp[(int)psrcCPUBOSS00Core[aintBOSSNo].penuType]);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(psrcCPUBOSS00Core[aintBOSSNo].pVecIti, psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X - 256f + 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + 256f - 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X - 128f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 20);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + 128f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 20);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X - 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 40);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 40);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(psrcCPUBOSS00Core[aintBOSSNo].pVecIti, psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 60);
				KakuCPUBOSS00Init(aintBOSSNo);
				pCPUBOSSChild00Init();
			}
			return true;
		}
		return false;
	}

	private bool ESPTamaHantei(int aintBOSSNo, Rectangle arecHantei, bool aflgVulcan)
	{
		for (int i = 0; i < precCPUBOSS00OffSet.GetUpperBound(1) && (precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].X != 0 || precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Y != 0 || precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Width != 0 || precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Height != 0); i++)
		{
			if (!arecHantei.Intersects(new Rectangle((int)psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].X, (int)psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y + precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Y, precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Width, precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Height)))
			{
				continue;
			}
			DamageStartStateSet(aintBOSSNo);
			for (int j = 0; j < psrcCPUBOSS00Core[aintBOSSNo].pintChildMax; j++)
			{
				DamageChildStartStateSet(j);
			}
			if (aflgVulcan)
			{
				if (psrcCPUBOSS00Core[aintBOSSNo].pfltDamage <= 0.5f)
				{
					psrcCPUBOSS00Core[aintBOSSNo].pfltDamage = 2f;
				}
				else
				{
					psrcCPUBOSS00Core[aintBOSSNo].pfltDamage *= 1.1f;
				}
			}
			else
			{
				psrcCPUBOSS00Core[aintBOSSNo].pfltHP -= 5f;
			}
			Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(arecHantei.X, arecHantei.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
			Game1.bGM.pflgSEBakuhatu[1] = true;
			if (psrcCPUBOSS00Core[aintBOSSNo].pfltHP <= 0f)
			{
				Game1.bGM.pflgSEBakuhatu[2] = true;
				Game1.score.pScoreUp(lngScoreUp[(int)psrcCPUBOSS00Core[aintBOSSNo].penuType]);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(psrcCPUBOSS00Core[aintBOSSNo].pVecIti, psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X - 256f + 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + 256f - 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X - 128f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 20);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + 128f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 20);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X - 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 40);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 40);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(psrcCPUBOSS00Core[aintBOSSNo].pVecIti, psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 60);
				KakuCPUBOSS00Init(aintBOSSNo);
				pCPUBOSSChild00Init();
			}
			return true;
		}
		return false;
	}

	private bool OSPREYNoDamageTamaHantei(int aintBOSSNo, Rectangle arecHantei)
	{
		for (int i = 0; i < precCPUBOSS00NoneOffSet.GetUpperBound(1) && (precCPUBOSS00NoneOffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].X != 0 || precCPUBOSS00NoneOffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Y != 0 || precCPUBOSS00NoneOffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Width != 0 || precCPUBOSS00NoneOffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Height != 0); i++)
		{
			if (arecHantei.Intersects(new Rectangle((int)psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + precCPUBOSS00NoneOffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].X, (int)psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y + precCPUBOSS00NoneOffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Y, precCPUBOSS00NoneOffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Width, precCPUBOSS00NoneOffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Height)))
			{
				Game1.bakuhatu.psrcBakuhatu02CoreConboEnable(new Vector3(arecHantei.X, arecHantei.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
				return true;
			}
		}
		return false;
	}

	private bool DragonTamaHantei(int aintBOSSNo, Rectangle arecHantei, bool aflgVulcan)
	{
		for (int i = 0; i < precCPUBOSS00OffSet.GetUpperBound(1) && (precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].X != 0 || precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Y != 0 || precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Width != 0 || precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Height != 0); i++)
		{
			if (!arecHantei.Intersects(new Rectangle((int)psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].X, (int)psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y + precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Y, precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Width, precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Height)))
			{
				continue;
			}
			DamageStartStateSet(aintBOSSNo);
			for (int j = 0; j < psrcCPUBOSS00Core[aintBOSSNo].pintChildMax; j++)
			{
				DamageChildStartStateSet(j);
			}
			if (aflgVulcan)
			{
				if (psrcCPUBOSS00Core[aintBOSSNo].pfltDamage <= 2f)
				{
					psrcCPUBOSS00Core[aintBOSSNo].pfltDamage = 5f;
				}
				else
				{
					psrcCPUBOSS00Core[aintBOSSNo].pfltDamage *= 1.24f;
				}
			}
			else
			{
				psrcCPUBOSS00Core[aintBOSSNo].pfltHP -= 9f;
			}
			Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(arecHantei.X, arecHantei.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
			Game1.bGM.pflgSEBakuhatu[1] = true;
			if (psrcCPUBOSS00Core[aintBOSSNo].pfltHP <= 0f)
			{
				Game1.bGM.pflgSEBakuhatu[2] = true;
				Game1.cPUPort00.pSubBossTaosita();
				Game1.score.pScoreUp(lngScoreUp[(int)psrcCPUBOSS00Core[aintBOSSNo].penuType]);
				for (int j = 0; j < psrcCPUBOSS00Core[aintBOSSNo].pintChildMax; j++)
				{
					Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(psrcCPUBOSSChild00Core[j].pVecIti, psrcCPUBOSSChild00Core[j].pfltItiR, 0);
					Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSSChild00Core[j].pVecIti.X, psrcCPUBOSSChild00Core[j].pVecIti.Y, psrcCPUBOSSChild00Core[j].pVecIti.Z), psrcCPUBOSSChild00Core[j].pfltItiR, 15);
					Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSSChild00Core[j].pVecIti.X + 300f, psrcCPUBOSSChild00Core[j].pVecIti.Y, psrcCPUBOSSChild00Core[j].pVecIti.Z), psrcCPUBOSSChild00Core[j].pfltItiR, 20);
					Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSSChild00Core[j].pVecIti.X - 150f, psrcCPUBOSSChild00Core[j].pVecIti.Y - 150f, psrcCPUBOSSChild00Core[j].pVecIti.Z), psrcCPUBOSSChild00Core[j].pfltItiR, 35);
					Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSSChild00Core[j].pVecIti.X - 200f, psrcCPUBOSSChild00Core[j].pVecIti.Y, psrcCPUBOSSChild00Core[j].pVecIti.Z), psrcCPUBOSSChild00Core[j].pfltItiR, 40);
				}
				KakuCPUBOSS00Init(aintBOSSNo);
				pCPUBOSSChild00Init();
			}
			return true;
		}
		return false;
	}

	private bool QueenBeeTamaHantei(int aintBOSSNo, Rectangle arecHantei, bool aflgVulcan)
	{
		for (int i = 0; i < precCPUBOSS00OffSet.GetUpperBound(1) && (precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].X != 0 || precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Y != 0 || precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Width != 0 || precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Height != 0); i++)
		{
			if (!arecHantei.Intersects(new Rectangle((int)psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].X, (int)psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y + precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Y, precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Width, precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Height)))
			{
				continue;
			}
			QueenBeeDamage00StartStateSet(aintBOSSNo);
			if (aflgVulcan)
			{
				if (psrcCPUBOSS00Core[aintBOSSNo].pfltDamage <= 2f)
				{
					psrcCPUBOSS00Core[aintBOSSNo].pfltDamage = 5f;
				}
				else
				{
					psrcCPUBOSS00Core[aintBOSSNo].pfltDamage *= 1.3f;
				}
			}
			else
			{
				psrcCPUBOSS00Core[aintBOSSNo].pfltHP -= 15f;
			}
			Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(arecHantei.X, arecHantei.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
			Game1.bGM.pflgSEBakuhatu[1] = true;
			if (psrcCPUBOSS00Core[aintBOSSNo].pfltHP <= 0f)
			{
				Game1.score.pScoreUp(lngScoreUp[(int)psrcCPUBOSS00Core[aintBOSSNo].penuType]);
				Game1.bGM.pflgSEBakuhatu[2] = true;
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(psrcCPUBOSS00Core[aintBOSSNo].pVecIti, psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X - 256f + 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + 256f - 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X - 128f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 20);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + 128f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 20);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X - 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 40);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 40);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(psrcCPUBOSS00Core[aintBOSSNo].pVecIti, psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 60);
				KakuCPUBOSS00Init(aintBOSSNo);
				pCPUBOSSChild00Init();
			}
			return true;
		}
		return false;
	}

	private bool HatiNoSuTamaHantei(int aintBOSSNo, Rectangle arecHantei, bool aflgVulcan)
	{
		for (int i = 0; i < precCPUBOSS00OffSet.GetUpperBound(1) && (precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].X != 0 || precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Y != 0 || precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Width != 0 || precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Height != 0); i++)
		{
			if (!arecHantei.Intersects(new Rectangle((int)psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].X, (int)psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y + precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Y, precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Width, precCPUBOSS00OffSet[(int)psrcCPUBOSS00Core[aintBOSSNo].penuImgState[0], i].Height)))
			{
				continue;
			}
			DamageStartStateSet(aintBOSSNo);
			if (rnd.Next(1, 500) < 250)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y + (float)rnd.Next(1, 500) - 250f, 0f), CPU00.enuCPU00Type.intQueenChildBee00);
			}
			else
			{
				Game1.cPU00.pCPU00Enable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y + (float)rnd.Next(1, 500) - 250f, 0f), CPU00.enuCPU00Type.intQueenChildBeeX00);
			}
			if (aflgVulcan)
			{
				if (psrcCPUBOSS00Core[aintBOSSNo].pfltDamage <= 2f)
				{
					psrcCPUBOSS00Core[aintBOSSNo].pfltDamage = 5f;
				}
				else
				{
					psrcCPUBOSS00Core[aintBOSSNo].pfltDamage *= 1.4f;
				}
			}
			else
			{
				psrcCPUBOSS00Core[aintBOSSNo].pfltHP -= 18f;
			}
			Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(arecHantei.X, arecHantei.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
			Game1.bGM.pflgSEBakuhatu[1] = true;
			if (psrcCPUBOSS00Core[aintBOSSNo].pfltHP <= 0f)
			{
				Game1.bGM.pflgSEBakuhatu[2] = true;
				Game1.cPUPort00.pSubBossTaosita();
				if (rnd.Next(1, 500) < 250)
				{
					for (int j = 0; j < 3; j++)
					{
						for (int k = 0; k < 3; k++)
						{
							Game1.cPU00.pCPU00Enable(new Vector3((float)(j * 150) + psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X, (float)(k * 150) + psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y - 300f, 0f), CPU00.enuCPU00Type.intQueenChildBee00);
						}
					}
				}
				else
				{
					for (int j = 0; j < 3; j++)
					{
						for (int k = 0; k < 3; k++)
						{
							Game1.cPU00.pCPU00Enable(new Vector3((float)(j * 150) + psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X, (float)(k * 150) + psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y - 300f, 0f), CPU00.enuCPU00Type.intQueenChildBeeX00);
						}
					}
				}
				Game1.score.pScoreUp(lngScoreUp[(int)psrcCPUBOSS00Core[aintBOSSNo].penuType]);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(psrcCPUBOSS00Core[aintBOSSNo].pVecIti, psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X - 256f + 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + 256f - 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 0);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X - 128f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 20);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + 128f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 20);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X - 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 40);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(new Vector3(psrcCPUBOSS00Core[aintBOSSNo].pVecIti.X + 64f, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Y, psrcCPUBOSS00Core[aintBOSSNo].pVecIti.Z), psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 40);
				Game1.bakuhatu.psrcBakuhatu00CoreConboEnable(psrcCPUBOSS00Core[aintBOSSNo].pVecIti, psrcCPUBOSS00Core[aintBOSSNo].pfltItiR, 60);
				KakuCPUBOSS00Init(aintBOSSNo);
				pCPUBOSSChild00Init();
			}
			return true;
		}
		return false;
	}

	public bool pCPUBOSSChild00TamaHantei(Vector3 avecIti, float afltHabaHantei, Rectangle arecHantei, bool aflgVulcan)
	{
		bool result = false;
		Rectangle arecHantei2 = new Rectangle
		{
			X = arecHantei.X + (int)avecIti.X,
			Y = arecHantei.Y + (int)avecIti.Y,
			Width = arecHantei.Width,
			Height = arecHantei.Height
		};
		for (int i = 0; i < psrcCPUBOSSChild00Core.Length; i++)
		{
			if (!psrcCPUBOSSChild00Core[i].pflgEnable || ((!(avecIti.Z <= psrcCPUBOSSChild00Core[i].pVecIti.Z) || !(avecIti.Z + afltHabaHantei >= psrcCPUBOSSChild00Core[i].pVecIti.Z)) && (!(avecIti.Z <= psrcCPUBOSSChild00Core[i].pVecIti.Z + fltChildOffSetHaba[(int)psrcCPUBOSSChild00Core[i].penuType]) || !(avecIti.Z + afltHabaHantei >= psrcCPUBOSSChild00Core[i].pVecIti.Z + fltChildOffSetHaba[(int)psrcCPUBOSSChild00Core[i].penuType]))))
			{
				continue;
			}
			switch (psrcCPUBOSSChild00Core[i].penuType)
			{
			case enuCPUBOSSChild00Type.intOSPREYChild00:
				if (OSPREYChildNoDamageTamaHantei(i, arecHantei2))
				{
					return true;
				}
				break;
			case enuCPUBOSSChild00Type.intOSPREYChildX00:
				if (OSPREYChildNoDamageTamaHantei(i, arecHantei2))
				{
					return true;
				}
				break;
			case enuCPUBOSSChild00Type.intDragonBody:
				if (DragonChildNoDamageTamaHantei(i, arecHantei2))
				{
					return true;
				}
				break;
			}
		}
		return result;
	}

	private bool OSPREYChildNoDamageTamaHantei(int aintBOSSChildNo, Rectangle arecHantei)
	{
		if (psrcCPUBOSSChild00Core[aintBOSSChildNo].pfltItiR == 0f)
		{
			return ChildNoDamageTamaHantei(arecHantei, precCPUBOSSChild00NoneOffSet, psrcCPUBOSSChild00Core[aintBOSSChildNo].penuImgState[0], aintBOSSChildNo);
		}
		if (psrcCPUBOSSChild00Core[aintBOSSChildNo].pfltItiR == 15f)
		{
			return ChildNoDamageTamaHantei(arecHantei, precCPUBOSSChild00R15NoneOffSet, psrcCPUBOSSChild00Core[aintBOSSChildNo].penuImgState[0], aintBOSSChildNo);
		}
		if (psrcCPUBOSSChild00Core[aintBOSSChildNo].pfltItiR == 345f)
		{
			return ChildNoDamageTamaHantei(arecHantei, precCPUBOSSChild00R345NoneOffSet, psrcCPUBOSSChild00Core[aintBOSSChildNo].penuImgState[0], aintBOSSChildNo);
		}
		return false;
	}

	private bool DragonChildNoDamageTamaHantei(int aintBOSSChildNo, Rectangle arecHantei)
	{
		return ChildNoDamageTamaHantei(arecHantei, precCPUBOSSChild00NoneOffSet, psrcCPUBOSSChild00Core[aintBOSSChildNo].penuImgState[0], aintBOSSChildNo);
	}

	private bool ChildNoDamageTamaHantei(Rectangle arecHantei, Rectangle[,] arecOffset, enuCPUBOSSChild00ImgState aenuImgState, int aintBOSSChildNo)
	{
		for (int i = 0; i < arecOffset.GetUpperBound(1) && (arecOffset[(int)aenuImgState, i].Width != 0 || arecOffset[(int)aenuImgState, i].Height != 0); i++)
		{
			if (arecHantei.Intersects(new Rectangle((int)psrcCPUBOSSChild00Core[aintBOSSChildNo].pVecIti.X + arecOffset[(int)aenuImgState, i].X, (int)psrcCPUBOSSChild00Core[aintBOSSChildNo].pVecIti.Y + arecOffset[(int)aenuImgState, i].Y, arecOffset[(int)aenuImgState, i].Width, arecOffset[(int)aenuImgState, i].Height)))
			{
				Game1.bakuhatu.psrcBakuhatu02CoreConboEnable(new Vector3(arecHantei.X, arecHantei.Y, psrcCPUBOSSChild00Core[aintBOSSChildNo].pVecIti.Z), psrcCPUBOSSChild00Core[aintBOSSChildNo].pfltItiR, 0);
				return true;
			}
		}
		return false;
	}

	protected override void LoadContent()
	{
		pimgCPUBOSS00[0] = null;
		pimgCPUBOSS00[1] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\OSPREY08");
		pimgCPUBOSS00[2] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\OSPREY_Damage00");
		pimgCPUBOSS00[3] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\DragonHead_Bo09");
		pimgCPUBOSS00[4] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\DragonHead_Bo_Back03");
		pimgCPUBOSS00[5] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\DragonHead_Damage00");
		pimgCPUBOSS00[6] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\HatiNoSu00");
		pimgCPUBOSS00[7] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\HatiNoSuDamage00");
		pimgCPUBOSS00[8] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\QueenBee04");
		pimgCPUBOSS00[9] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\QueenBee05");
		pimgCPUBOSS00[10] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\QueenBeeDamage02");
		pimgCPUBOSS00[11] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\QueenBeeDamage03");
		pimgCPUBOSS00[12] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\ArshipG04");
		pimgCPUBOSS00[13] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\ArshipG_DM01");
		pimgCPUBOSS00[14] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\Arship11");
		pimgCPUBOSS00[15] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\Arship_DM09");
		pimgCPUBOSS00[16] = pimgCPUBOSS00[12];
		pimgCPUBOSS00[17] = pimgCPUBOSS00[13];
		pimgCPUBOSS00[18] = pimgCPUBOSS00[14];
		pimgCPUBOSS00[19] = pimgCPUBOSS00[15];
		pimgCPUBOSS00[20] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\ESP08");
		pimgCPUBOSS00[21] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\ESP_DM08");
		pimgCPUBOSS00[22] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\ESP_Move00");
		pimgCPUBOSS00[23] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\ESP_Move_DM00");
		pimgCPUBOSSChild00[0] = null;
		pimgCPUBOSSChild00[1] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\OSPREY_Child01");
		pimgCPUBOSSChild00[2] = pimgCPUBOSSChild00[1];
		pimgCPUBOSSChild00[3] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\OSPREY_Child_Damage00");
		pimgCPUBOSSChild00[4] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\DragonBody09");
		pimgCPUBOSSChild00[5] = base.Game.Content.Load<Texture2D>("PNG\\Character\\CPUBOSS\\DragonBodyDamage00");
		base.LoadContent();
	}

	public void pImageStateUpdate()
	{
		for (int i = 0; i < psrcCPUBOSS00Core.Length; i++)
		{
			if (psrcCPUBOSS00Core[i].pflgEnable)
			{
				for (int j = 0; j < psrcCPUBOSS00Core[i].penuImgState.Length - 1; j++)
				{
					psrcCPUBOSS00Core[i].penuImgState[j] = psrcCPUBOSS00Core[i].penuImgState[j + 1];
				}
			}
		}
	}

	private void NomarlStartStateSet(int aintCPUNo)
	{
		enuCPUBOSS00ImgState enuCPUBOSS00ImgState2 = psrcCPUBOSS00Core[aintCPUNo].penuType switch
		{
			enuCPUBOSS00Type.intNormal00 => enuCPUBOSS00ImgState.intKieru, 
			enuCPUBOSS00Type.intOSPREY00 => enuCPUBOSS00ImgState.intOSPREY00, 
			enuCPUBOSS00Type.intDragon => (psrcCPUBOSS00Core[aintCPUNo].penuImgStateHoji != enuCPUBOSS00ImgStateHoji.intFront) ? enuCPUBOSS00ImgState.intDragonHeadBack : enuCPUBOSS00ImgState.intDragonHeadFront, 
			enuCPUBOSS00Type.intHatiNosu => enuCPUBOSS00ImgState.intHatiNoSu, 
			enuCPUBOSS00Type.intQueenBee => enuCPUBOSS00ImgState.intQueenBee00, 
			enuCPUBOSS00Type.intArshipTate => (psrcCPUBOSS00Core[aintCPUNo].penuImgStateHoji != enuCPUBOSS00ImgStateHoji.intLeft) ? enuCPUBOSS00ImgState.intArshipTateX : enuCPUBOSS00ImgState.intArshipTate, 
			enuCPUBOSS00Type.intArship => (psrcCPUBOSS00Core[aintCPUNo].penuImgStateHoji != enuCPUBOSS00ImgStateHoji.intLeft) ? enuCPUBOSS00ImgState.intArshipX : enuCPUBOSS00ImgState.intArship, 
			enuCPUBOSS00Type.intESP => (psrcCPUBOSS00Core[aintCPUNo].penuImgStateHoji != enuCPUBOSS00ImgStateHoji.intMove) ? enuCPUBOSS00ImgState.intESP : enuCPUBOSS00ImgState.intESPMove, 
			_ => enuCPUBOSS00ImgState.intOSPREY00, 
		};
		for (int i = 0; i < 25; i++)
		{
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i] = enuCPUBOSS00ImgState2;
		}
	}

	private void DamageStartStateSet(int aintCPUNo)
	{
		enuCPUBOSS00ImgState enuCPUBOSS00ImgState2;
		enuCPUBOSS00ImgState enuCPUBOSS00ImgState3;
		switch (psrcCPUBOSS00Core[aintCPUNo].penuType)
		{
		case enuCPUBOSS00Type.intNormal00:
			enuCPUBOSS00ImgState2 = enuCPUBOSS00ImgState.intKieru;
			enuCPUBOSS00ImgState3 = enuCPUBOSS00ImgState.intKieru;
			break;
		case enuCPUBOSS00Type.intOSPREY00:
			enuCPUBOSS00ImgState2 = enuCPUBOSS00ImgState.intOSPREY00;
			enuCPUBOSS00ImgState3 = enuCPUBOSS00ImgState.intOSPREYDamage00;
			break;
		case enuCPUBOSS00Type.intDragon:
			enuCPUBOSS00ImgState2 = ((psrcCPUBOSS00Core[aintCPUNo].penuImgStateHoji != enuCPUBOSS00ImgStateHoji.intFront) ? enuCPUBOSS00ImgState.intDragonHeadBack : enuCPUBOSS00ImgState.intDragonHeadFront);
			enuCPUBOSS00ImgState3 = enuCPUBOSS00ImgState.intDragonHeadDamage;
			break;
		case enuCPUBOSS00Type.intHatiNosu:
			enuCPUBOSS00ImgState2 = enuCPUBOSS00ImgState.intHatiNoSu;
			enuCPUBOSS00ImgState3 = enuCPUBOSS00ImgState.intHatiNoSuDamage;
			break;
		case enuCPUBOSS00Type.intQueenBee:
			enuCPUBOSS00ImgState2 = enuCPUBOSS00ImgState.intQueenBee00;
			enuCPUBOSS00ImgState3 = enuCPUBOSS00ImgState.intQueenBeeDamage00;
			break;
		case enuCPUBOSS00Type.intArshipTate:
			if (psrcCPUBOSS00Core[aintCPUNo].penuImgStateHoji == enuCPUBOSS00ImgStateHoji.intLeft)
			{
				enuCPUBOSS00ImgState2 = enuCPUBOSS00ImgState.intArshipTate;
				enuCPUBOSS00ImgState3 = enuCPUBOSS00ImgState.intArshipTateDamage;
			}
			else
			{
				enuCPUBOSS00ImgState2 = enuCPUBOSS00ImgState.intArshipTateX;
				enuCPUBOSS00ImgState3 = enuCPUBOSS00ImgState.intArshipTateDamageX;
			}
			break;
		case enuCPUBOSS00Type.intArship:
			if (psrcCPUBOSS00Core[aintCPUNo].penuImgStateHoji == enuCPUBOSS00ImgStateHoji.intLeft)
			{
				enuCPUBOSS00ImgState2 = enuCPUBOSS00ImgState.intArship;
				enuCPUBOSS00ImgState3 = enuCPUBOSS00ImgState.intArshipDamage;
			}
			else
			{
				enuCPUBOSS00ImgState2 = enuCPUBOSS00ImgState.intArshipX;
				enuCPUBOSS00ImgState3 = enuCPUBOSS00ImgState.intArshipDamageX;
			}
			break;
		case enuCPUBOSS00Type.intESP:
			if (psrcCPUBOSS00Core[aintCPUNo].penuImgStateHoji == enuCPUBOSS00ImgStateHoji.intMove)
			{
				enuCPUBOSS00ImgState2 = enuCPUBOSS00ImgState.intESPMove;
				enuCPUBOSS00ImgState3 = enuCPUBOSS00ImgState.intESPMoveDamage;
			}
			else
			{
				enuCPUBOSS00ImgState2 = enuCPUBOSS00ImgState.intESP;
				enuCPUBOSS00ImgState3 = enuCPUBOSS00ImgState.intESPDamage;
			}
			break;
		default:
			enuCPUBOSS00ImgState2 = enuCPUBOSS00ImgState.intOSPREY00;
			enuCPUBOSS00ImgState3 = enuCPUBOSS00ImgState.intOSPREYDamage00;
			break;
		}
		for (int i = 0; i < 30; i += 10)
		{
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i] = enuCPUBOSS00ImgState3;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 1] = enuCPUBOSS00ImgState3;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 2] = enuCPUBOSS00ImgState3;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 3] = enuCPUBOSS00ImgState3;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 4] = enuCPUBOSS00ImgState3;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 5] = enuCPUBOSS00ImgState2;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 6] = enuCPUBOSS00ImgState2;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 7] = enuCPUBOSS00ImgState2;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 8] = enuCPUBOSS00ImgState2;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 9] = enuCPUBOSS00ImgState2;
		}
	}

	private void NomarlStateSet(int aintCPUNo)
	{
		if (psrcCPUBOSS00Core[aintCPUNo].penuImgState[0] == enuCPUBOSS00ImgState.intKieru)
		{
			NomarlStartStateSet(aintCPUNo);
		}
	}

	private void QueenBee00StartStateSet(int aintCPUNo)
	{
		psrcCPUBOSS00Core[aintCPUNo].penuImgState[0] = enuCPUBOSS00ImgState.intQueenBee00;
		psrcCPUBOSS00Core[aintCPUNo].penuImgState[1] = enuCPUBOSS00ImgState.intQueenBee00;
		psrcCPUBOSS00Core[aintCPUNo].penuImgState[2] = enuCPUBOSS00ImgState.intQueenBee00;
		psrcCPUBOSS00Core[aintCPUNo].penuImgState[3] = enuCPUBOSS00ImgState.intQueenBee00;
		psrcCPUBOSS00Core[aintCPUNo].penuImgState[4] = enuCPUBOSS00ImgState.intQueenBee00;
		psrcCPUBOSS00Core[aintCPUNo].penuImgState[5] = enuCPUBOSS00ImgState.intQueenBee00;
		psrcCPUBOSS00Core[aintCPUNo].penuImgState[6] = enuCPUBOSS00ImgState.intQueenBee00;
		psrcCPUBOSS00Core[aintCPUNo].penuImgState[7] = enuCPUBOSS00ImgState.intQueenBee00;
		psrcCPUBOSS00Core[aintCPUNo].penuImgState[8] = enuCPUBOSS00ImgState.intQueenBee01;
		psrcCPUBOSS00Core[aintCPUNo].penuImgState[9] = enuCPUBOSS00ImgState.intQueenBee01;
		psrcCPUBOSS00Core[aintCPUNo].penuImgState[10] = enuCPUBOSS00ImgState.intQueenBee01;
		psrcCPUBOSS00Core[aintCPUNo].penuImgState[11] = enuCPUBOSS00ImgState.intQueenBee01;
		psrcCPUBOSS00Core[aintCPUNo].penuImgState[12] = enuCPUBOSS00ImgState.intQueenBee01;
		psrcCPUBOSS00Core[aintCPUNo].penuImgState[13] = enuCPUBOSS00ImgState.intQueenBee01;
		psrcCPUBOSS00Core[aintCPUNo].penuImgState[14] = enuCPUBOSS00ImgState.intQueenBee01;
		psrcCPUBOSS00Core[aintCPUNo].penuImgState[15] = enuCPUBOSS00ImgState.intQueenBee01;
	}

	private void QueenBee00StateSet(int aintCPUNo)
	{
		if (psrcCPUBOSS00Core[aintCPUNo].penuImgState[0] == enuCPUBOSS00ImgState.intKieru)
		{
			QueenBee00StartStateSet(aintCPUNo);
		}
	}

	private void QueenBeeDamage00StartStateSet(int aintCPUNo)
	{
		for (int i = 0; i < 32; i += 16)
		{
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i] = enuCPUBOSS00ImgState.intQueenBeeDamage00;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 1] = enuCPUBOSS00ImgState.intQueenBeeDamage00;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 2] = enuCPUBOSS00ImgState.intQueenBeeDamage00;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 3] = enuCPUBOSS00ImgState.intQueenBeeDamage00;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 4] = enuCPUBOSS00ImgState.intQueenBeeDamage00;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 5] = enuCPUBOSS00ImgState.intQueenBeeDamage00;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 6] = enuCPUBOSS00ImgState.intQueenBeeDamage00;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 7] = enuCPUBOSS00ImgState.intQueenBeeDamage00;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 8] = enuCPUBOSS00ImgState.intQueenBee01;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 9] = enuCPUBOSS00ImgState.intQueenBee01;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 10] = enuCPUBOSS00ImgState.intQueenBee01;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 11] = enuCPUBOSS00ImgState.intQueenBee01;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 12] = enuCPUBOSS00ImgState.intQueenBee01;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 13] = enuCPUBOSS00ImgState.intQueenBee01;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 14] = enuCPUBOSS00ImgState.intQueenBee01;
			psrcCPUBOSS00Core[aintCPUNo].penuImgState[i + 15] = enuCPUBOSS00ImgState.intQueenBee01;
		}
	}

	public void pImageChildStateUpdate()
	{
		for (int i = 0; i < psrcCPUBOSSChild00Core.Length; i++)
		{
			if (psrcCPUBOSSChild00Core[i].pflgEnable)
			{
				for (int j = 0; j < psrcCPUBOSSChild00Core[i].penuImgState.Length - 1; j++)
				{
					psrcCPUBOSSChild00Core[i].penuImgState[j] = psrcCPUBOSSChild00Core[i].penuImgState[j + 1];
				}
			}
		}
	}

	private void NomarlChildStartStateSet(int aintCPUNo)
	{
		enuCPUBOSSChild00ImgState enuCPUBOSSChild00ImgState2 = psrcCPUBOSSChild00Core[aintCPUNo].penuType switch
		{
			enuCPUBOSSChild00Type.intNormal00 => enuCPUBOSSChild00ImgState.intKieru, 
			enuCPUBOSSChild00Type.intOSPREYChild00 => enuCPUBOSSChild00ImgState.intOSPREYChild00, 
			enuCPUBOSSChild00Type.intOSPREYChildX00 => enuCPUBOSSChild00ImgState.intOSPREYChildX00, 
			enuCPUBOSSChild00Type.intDragonBody => enuCPUBOSSChild00ImgState.intDragonBody, 
			_ => enuCPUBOSSChild00ImgState.intOSPREYChild00, 
		};
		for (int i = 0; i < psrcCPUBOSSChild00Core[aintCPUNo].penuImgState.Length - 1; i++)
		{
			psrcCPUBOSSChild00Core[aintCPUNo].penuImgState[i] = enuCPUBOSSChild00ImgState2;
		}
	}

	private void DamageChildStartStateSet(int aintCPUNo)
	{
		enuCPUBOSSChild00ImgState enuCPUBOSSChild00ImgState2;
		enuCPUBOSSChild00ImgState enuCPUBOSSChild00ImgState3;
		switch (psrcCPUBOSSChild00Core[aintCPUNo].penuType)
		{
		case enuCPUBOSSChild00Type.intNormal00:
			enuCPUBOSSChild00ImgState2 = enuCPUBOSSChild00ImgState.intKieru;
			enuCPUBOSSChild00ImgState3 = enuCPUBOSSChild00ImgState.intOSPREYChildDamage;
			break;
		case enuCPUBOSSChild00Type.intOSPREYChild00:
			enuCPUBOSSChild00ImgState2 = enuCPUBOSSChild00ImgState.intOSPREYChild00;
			enuCPUBOSSChild00ImgState3 = enuCPUBOSSChild00ImgState.intOSPREYChildDamage;
			break;
		case enuCPUBOSSChild00Type.intOSPREYChildX00:
			enuCPUBOSSChild00ImgState2 = enuCPUBOSSChild00ImgState.intOSPREYChildX00;
			enuCPUBOSSChild00ImgState3 = enuCPUBOSSChild00ImgState.intOSPREYChildDamage;
			break;
		case enuCPUBOSSChild00Type.intDragonBody:
			enuCPUBOSSChild00ImgState2 = enuCPUBOSSChild00ImgState.intDragonBody;
			enuCPUBOSSChild00ImgState3 = enuCPUBOSSChild00ImgState.intDragonBodyDamage;
			break;
		default:
			enuCPUBOSSChild00ImgState2 = enuCPUBOSSChild00ImgState.intOSPREYChild00;
			enuCPUBOSSChild00ImgState3 = enuCPUBOSSChild00ImgState.intOSPREYChildDamage;
			break;
		}
		for (int i = 0; i < 30; i += 10)
		{
			psrcCPUBOSSChild00Core[aintCPUNo].penuImgState[i] = enuCPUBOSSChild00ImgState3;
			psrcCPUBOSSChild00Core[aintCPUNo].penuImgState[i + 1] = enuCPUBOSSChild00ImgState3;
			psrcCPUBOSSChild00Core[aintCPUNo].penuImgState[i + 2] = enuCPUBOSSChild00ImgState3;
			psrcCPUBOSSChild00Core[aintCPUNo].penuImgState[i + 3] = enuCPUBOSSChild00ImgState3;
			psrcCPUBOSSChild00Core[aintCPUNo].penuImgState[i + 4] = enuCPUBOSSChild00ImgState3;
			psrcCPUBOSSChild00Core[aintCPUNo].penuImgState[i + 5] = enuCPUBOSSChild00ImgState2;
			psrcCPUBOSSChild00Core[aintCPUNo].penuImgState[i + 6] = enuCPUBOSSChild00ImgState2;
			psrcCPUBOSSChild00Core[aintCPUNo].penuImgState[i + 7] = enuCPUBOSSChild00ImgState2;
			psrcCPUBOSSChild00Core[aintCPUNo].penuImgState[i + 8] = enuCPUBOSSChild00ImgState2;
			psrcCPUBOSSChild00Core[aintCPUNo].penuImgState[i + 9] = enuCPUBOSSChild00ImgState2;
		}
	}

	private void NomarlChildStateSet(int aintCPUNo)
	{
		if (psrcCPUBOSSChild00Core[aintCPUNo].penuImgState[0] == enuCPUBOSSChild00ImgState.intKieru)
		{
			NomarlChildStartStateSet(aintCPUNo);
		}
	}

	public void pCPUBOSS00Draw(SpriteBatch aspritesBatch)
	{
		if (pimgCPUBOSS00[2] == null)
		{
			return;
		}
		for (int i = 0; i < psrcCPUBOSS00Core.Length; i++)
		{
			if (psrcCPUBOSS00Core[i].pflgEnable && psrcCPUBOSS00Core[i].penuImgState[0] != enuCPUBOSS00ImgState.intKieru)
			{
				int width = pimgCPUBOSS00[(int)psrcCPUBOSS00Core[i].penuImgState[0]].Width;
				int height = pimgCPUBOSS00[(int)psrcCPUBOSS00Core[i].penuImgState[0]].Height;
				aspritesBatch.Draw(pimgCPUBOSS00[(int)psrcCPUBOSS00Core[i].penuImgState[0]], new Vector2(psrcCPUBOSS00Core[i].pVecIti.X * psrcCPUBOSS00Core[i].pVecIti.Z + 640f, psrcCPUBOSS00Core[i].pVecIti.Y * psrcCPUBOSS00Core[i].pVecIti.Z + 360f), null, Color.White, MathHelper.ToRadians(psrcCPUBOSS00Core[i].pfltItiR), new Vector2(width / 2, height / 2), new Vector2(psrcCPUBOSS00Core[i].pVecIti.Z, psrcCPUBOSS00Core[i].pVecIti.Z), psrcCPUBOSS00Core[i].pSpriteEffects, psrcCPUBOSS00Core[i].pVecIti.Z);
			}
		}
		for (int i = 0; i < psrcCPUBOSSChild00Core.Length; i++)
		{
			if (psrcCPUBOSSChild00Core[i].pflgEnable && psrcCPUBOSSChild00Core[i].penuImgState[0] != enuCPUBOSSChild00ImgState.intKieru)
			{
				int width = pimgCPUBOSSChild00[(int)psrcCPUBOSSChild00Core[i].penuImgState[0]].Width;
				int height = pimgCPUBOSSChild00[(int)psrcCPUBOSSChild00Core[i].penuImgState[0]].Height;
				aspritesBatch.Draw(pimgCPUBOSSChild00[(int)psrcCPUBOSSChild00Core[i].penuImgState[0]], new Vector2(psrcCPUBOSSChild00Core[i].pVecIti.X * psrcCPUBOSSChild00Core[i].pVecIti.Z + 640f, psrcCPUBOSSChild00Core[i].pVecIti.Y * psrcCPUBOSSChild00Core[i].pVecIti.Z + 360f), null, Color.White, MathHelper.ToRadians(psrcCPUBOSSChild00Core[i].pfltItiR), new Vector2(width / 2, height / 2), new Vector2(psrcCPUBOSSChild00Core[i].pVecIti.Z, psrcCPUBOSSChild00Core[i].pVecIti.Z), psrcCPUBOSSChild00Core[i].pSpriteEffects, psrcCPUBOSSChild00Core[i].pVecIti.Z);
			}
		}
	}

	public override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
	}
}
