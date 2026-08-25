using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DepthAttack.CS;

public class PlayerHoming : DrawableGameComponent
{
	public struct srcPlayerHomingCore
	{
		public bool pflgEnable;

		public Vector3 pVecIti;

		public Vector3 pVecMovIti;

		public enuPlayerHomingImgState[] penuImgState;
	}

	public enum enuPlayerHomingImgState
	{
		intKieru,
		intNormal00,
		intNormal01,
		intNormal02
	}

	private const string cstrPlayerHoming00 = "PNG\\Tama\\TamaHoming00";

	private const string cstrPlayerHoming01 = "PNG\\Tama\\TamaHoming02";

	private const string cstrPlayerHoming02 = "PNG\\Tama\\TamaHoming01";

	public const float cfltOffSetHaba = 3f / 128f;

	private const int cintHomingWait = -1;

	public srcPlayerHomingCore[] psrcPlayerHomingCore = new srcPlayerHomingCore[3];

	public Texture2D[] pimgPlayerHoming = new Texture2D[4];

	public Rectangle[] precHomingOffSet = new Rectangle[1]
	{
		new Rectangle(-101, -69, 182, 118)
	};

	public PlayerHoming(Game game)
		: base(game)
	{
		for (int i = 0; i < psrcPlayerHomingCore.Length; i++)
		{
			psrcPlayerHomingCore[i].penuImgState = new enuPlayerHomingImgState[60];
		}
	}

	public override void Initialize()
	{
		psrcPlayerHomingCoreInit();
		base.Initialize();
	}

	public void psrcPlayerHomingCoreInit()
	{
		for (int i = 0; i < psrcPlayerHomingCore.Length; i++)
		{
			psrcPlayerHomingCore[i].pflgEnable = false;
			psrcPlayerHomingCore[i].pVecIti.X = 0f;
			psrcPlayerHomingCore[i].pVecIti.Y = 0f;
			psrcPlayerHomingCore[i].pVecIti.Z = 0f;
			psrcPlayerHomingCore[i].pVecMovIti.X = 0f;
			psrcPlayerHomingCore[i].pVecMovIti.Y = 0f;
			psrcPlayerHomingCore[i].pVecMovIti.Z = 0f;
			NormalStateSet(i);
		}
	}

	public void psrcPlayerHomingCoreEnable(Vector3 avec3Iti, Vector3 avec3MovIti)
	{
		for (int i = 0; i < psrcPlayerHomingCore.Length; i++)
		{
			if (!psrcPlayerHomingCore[i].pflgEnable)
			{
				psrcPlayerHomingCore[i].pflgEnable = true;
				psrcPlayerHomingCore[i].pVecIti.X = avec3Iti.X;
				psrcPlayerHomingCore[i].pVecIti.Y = avec3Iti.Y;
				psrcPlayerHomingCore[i].pVecIti.Z = avec3Iti.Z;
				psrcPlayerHomingCore[i].pVecMovIti.X = avec3MovIti.X;
				psrcPlayerHomingCore[i].pVecMovIti.Y = avec3MovIti.Y;
				psrcPlayerHomingCore[i].pVecMovIti.Z = avec3MovIti.Z;
				NormalStartStateSet(i);
				break;
			}
		}
	}

	public void pPlayerHomingUpdate()
	{
		for (int i = 0; i < psrcPlayerHomingCore.Length; i++)
		{
			if (psrcPlayerHomingCore[i].pflgEnable)
			{
				psrcPlayerHomingCore[i].pVecIti.Z += psrcPlayerHomingCore[i].pVecMovIti.Z;
				Vector3 vector = new Vector3(0f, 0f, 0f);
				vector = Vec3HomingTarget(psrcPlayerHomingCore[i].pVecIti);
				if (vector.X != psrcPlayerHomingCore[i].pVecIti.X || vector.Y != psrcPlayerHomingCore[i].pVecIti.Y || vector.Z != psrcPlayerHomingCore[i].pVecIti.Z)
				{
					double num = 0.0;
					num = Math.Atan2(psrcPlayerHomingCore[i].pVecIti.Y - vector.Y, psrcPlayerHomingCore[i].pVecIti.X - vector.X);
					double num2 = (int)(num * (180.0 / Math.PI));
					psrcPlayerHomingCore[i].pVecMovIti.X = (float)(Math.Cos(num) * 15.0);
					psrcPlayerHomingCore[i].pVecMovIti.Y = (float)(Math.Sin(num) * 17.0);
					psrcPlayerHomingCore[i].pVecIti.X -= (int)psrcPlayerHomingCore[i].pVecMovIti.X;
					psrcPlayerHomingCore[i].pVecIti.Y -= (int)psrcPlayerHomingCore[i].pVecMovIti.Y;
				}
				NormalStateSet(i);
			}
		}
		pImageStateUpdate();
	}

	public void pPlayerHomingHantei()
	{
		PlayerHomingHazureHantei();
		PlayerHomingSyougaiHantei();
	}

	public void PlayerHomingHazureHantei()
	{
		for (int i = 0; i < psrcPlayerHomingCore.Length; i++)
		{
			if (psrcPlayerHomingCore[i].pflgEnable && psrcPlayerHomingCore[i].pVecIti.Z < 3f / 64f)
			{
				psrcPlayerHomingCore[i].pflgEnable = false;
				psrcPlayerHomingCore[i].pVecIti.X = 0f;
				psrcPlayerHomingCore[i].pVecIti.Y = 0f;
				psrcPlayerHomingCore[i].pVecIti.Z = 0f;
				psrcPlayerHomingCore[i].pVecMovIti.X = 0f;
				psrcPlayerHomingCore[i].pVecMovIti.Y = 0f;
				psrcPlayerHomingCore[i].pVecMovIti.Z = 0f;
			}
		}
	}

	public void PlayerHomingSyougaiHantei()
	{
		for (int i = 0; i < psrcPlayerHomingCore.Length; i++)
		{
			if (psrcPlayerHomingCore[i].pflgEnable)
			{
				if (Game1.cPU00.pCPU00TamaHantei(psrcPlayerHomingCore[i].pVecIti, 3f / 128f, precHomingOffSet[0]))
				{
					psrcPlayerHomingCore[i].pflgEnable = false;
					psrcPlayerHomingCore[i].pVecIti.X = 0f;
					psrcPlayerHomingCore[i].pVecIti.Y = 0f;
					psrcPlayerHomingCore[i].pVecIti.Z = 0f;
					psrcPlayerHomingCore[i].pVecMovIti.X = 0f;
					psrcPlayerHomingCore[i].pVecMovIti.Y = 0f;
					psrcPlayerHomingCore[i].pVecMovIti.Z = 0f;
				}
				else if (Game1.syougai.pSyougaiTamaHantei(psrcPlayerHomingCore[i].pVecIti, 3f / 128f, precHomingOffSet[0]))
				{
					psrcPlayerHomingCore[i].pflgEnable = false;
					psrcPlayerHomingCore[i].pVecIti.X = 0f;
					psrcPlayerHomingCore[i].pVecIti.Y = 0f;
					psrcPlayerHomingCore[i].pVecIti.Z = 0f;
					psrcPlayerHomingCore[i].pVecMovIti.X = 0f;
					psrcPlayerHomingCore[i].pVecMovIti.Y = 0f;
					psrcPlayerHomingCore[i].pVecMovIti.Z = 0f;
				}
				else if (Game1.cPUBOSS00.pCPUBOSS00TamaHantei(psrcPlayerHomingCore[i].pVecIti, 3f / 128f, precHomingOffSet[0], aflgVulcan: false))
				{
					psrcPlayerHomingCore[i].pflgEnable = false;
					psrcPlayerHomingCore[i].pVecIti.X = 0f;
					psrcPlayerHomingCore[i].pVecIti.Y = 0f;
					psrcPlayerHomingCore[i].pVecIti.Z = 0f;
					psrcPlayerHomingCore[i].pVecMovIti.X = 0f;
					psrcPlayerHomingCore[i].pVecMovIti.Y = 0f;
					psrcPlayerHomingCore[i].pVecMovIti.Z = 0f;
				}
				else if (Game1.cPUBOSS00.pCPUBOSSChild00TamaHantei(psrcPlayerHomingCore[i].pVecIti, 3f / 128f, precHomingOffSet[0], aflgVulcan: true))
				{
					psrcPlayerHomingCore[i].pflgEnable = false;
					psrcPlayerHomingCore[i].pVecIti.X = 0f;
					psrcPlayerHomingCore[i].pVecIti.Y = 0f;
					psrcPlayerHomingCore[i].pVecIti.Z = 0f;
					psrcPlayerHomingCore[i].pVecMovIti.X = 0f;
					psrcPlayerHomingCore[i].pVecMovIti.Y = 0f;
					psrcPlayerHomingCore[i].pVecMovIti.Z = 0f;
				}
			}
		}
	}

	private Vector3 Vec3HomingTarget(Vector3 Vec3Jibun)
	{
		Vector3 result = new Vector3(0f, 0f, 0f);
		double num = 0.0;
		for (int i = 0; i < Game1.cPUBOSS00.psrcCPUBOSS00Core.Length; i++)
		{
			double num2 = 0.0;
			num2 = dblVec3Kyori(Vec3Jibun, Game1.cPUBOSS00.psrcCPUBOSS00Core[i].pVecIti);
			if (Game1.cPUBOSS00.psrcCPUBOSS00Core[i].pflgEnable && Vec3Jibun.Z > Game1.cPUBOSS00.psrcCPUBOSS00Core[i].pVecIti.Z && num2 < 1500.0)
			{
				result.X = Game1.cPUBOSS00.psrcCPUBOSS00Core[i].pVecIti.X;
				result.Y = Game1.cPUBOSS00.psrcCPUBOSS00Core[i].pVecIti.Y;
				result.Z = Game1.cPUBOSS00.psrcCPUBOSS00Core[i].pVecIti.Z;
				num = dblVec3Kyori(Vec3Jibun, Game1.cPUBOSS00.psrcCPUBOSS00Core[i].pVecIti);
				break;
			}
		}
		if (result.X == 0f && result.Y == 0f && result.Z == 0f)
		{
			for (int i = 0; i < Game1.cPU00.psrcCPU00Core.Length; i++)
			{
				double num2 = 0.0;
				num2 = dblVec3Kyori(Vec3Jibun, Game1.cPU00.psrcCPU00Core[i].pVecIti);
				if (Game1.cPU00.psrcCPU00Core[i].pflgEnable && Vec3Jibun.Z > Game1.cPU00.psrcCPU00Core[i].pVecIti.Z && num2 < 1500.0)
				{
					result.X = Game1.cPU00.psrcCPU00Core[i].pVecIti.X;
					result.Y = Game1.cPU00.psrcCPU00Core[i].pVecIti.Y;
					result.Z = Game1.cPU00.psrcCPU00Core[i].pVecIti.Z;
					num = dblVec3Kyori(Vec3Jibun, Game1.cPU00.psrcCPU00Core[i].pVecIti);
					break;
				}
			}
		}
		if (result.X == 0f && result.Y == 0f && result.Z == 0f)
		{
			return Vec3Jibun;
		}
		for (int i = 0; i < Game1.cPUBOSS00.psrcCPUBOSS00Core.Length; i++)
		{
			if (Game1.cPUBOSS00.psrcCPUBOSS00Core[i].pflgEnable && Vec3Jibun.Z > Game1.cPUBOSS00.psrcCPUBOSS00Core[i].pVecIti.Z)
			{
				double num2 = 0.0;
				num2 = dblVec3Kyori(Vec3Jibun, Game1.cPUBOSS00.psrcCPUBOSS00Core[i].pVecIti);
				if (num2 < num)
				{
					num = num2;
					result.X = Game1.cPUBOSS00.psrcCPUBOSS00Core[i].pVecIti.X;
					result.Y = Game1.cPUBOSS00.psrcCPUBOSS00Core[i].pVecIti.Y;
					result.Z = Game1.cPUBOSS00.psrcCPUBOSS00Core[i].pVecIti.Z;
				}
			}
		}
		for (int i = 0; i < Game1.cPU00.psrcCPU00Core.Length; i++)
		{
			if (Game1.cPU00.psrcCPU00Core[i].pflgEnable && Vec3Jibun.Z > Game1.cPU00.psrcCPU00Core[i].pVecIti.Z)
			{
				double num2 = 0.0;
				num2 = dblVec3Kyori(Vec3Jibun, Game1.cPU00.psrcCPU00Core[i].pVecIti);
				if (num2 < num)
				{
					num = num2;
					result.X = Game1.cPU00.psrcCPU00Core[i].pVecIti.X;
					result.Y = Game1.cPU00.psrcCPU00Core[i].pVecIti.Y;
					result.Z = Game1.cPU00.psrcCPU00Core[i].pVecIti.Z;
				}
			}
		}
		if (result.X == 0f && result.Y == 0f && result.Z == 0f)
		{
			return Vec3Jibun;
		}
		return result;
	}

	private double dblVec3Kyori(Vector3 vec3Jibun, Vector3 Vec3Aite)
	{
		Vector3 vector = default(Vector3);
		vector.X = vec3Jibun.X - Vec3Aite.X;
		vector.Y = vec3Jibun.Y - Vec3Aite.Y;
		vector.Z = (vec3Jibun.Z - Vec3Aite.Z) / (1f / 128f);
		return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}

	public void pImageStateUpdate()
	{
		for (int i = 0; i < psrcPlayerHomingCore.Length; i++)
		{
			if (psrcPlayerHomingCore[i].pflgEnable)
			{
				for (int j = 0; j < psrcPlayerHomingCore[i].penuImgState.Length - 1; j++)
				{
					psrcPlayerHomingCore[i].penuImgState[j] = psrcPlayerHomingCore[i].penuImgState[j + 1];
				}
			}
		}
	}

	protected override void LoadContent()
	{
		pimgPlayerHoming[0] = null;
		pimgPlayerHoming[1] = base.Game.Content.Load<Texture2D>("PNG\\Tama\\TamaHoming00");
		pimgPlayerHoming[2] = base.Game.Content.Load<Texture2D>("PNG\\Tama\\TamaHoming02");
		pimgPlayerHoming[3] = base.Game.Content.Load<Texture2D>("PNG\\Tama\\TamaHoming01");
		base.LoadContent();
	}

	private void NormalStartStateSet(int aintHomingNo)
	{
		for (int i = 0; i < 20; i++)
		{
			psrcPlayerHomingCore[aintHomingNo].penuImgState[i] = enuPlayerHomingImgState.intNormal00;
		}
		for (int i = 0; i < 20; i++)
		{
			psrcPlayerHomingCore[aintHomingNo].penuImgState[i + 20] = enuPlayerHomingImgState.intNormal01;
		}
		for (int i = 0; i < 20; i++)
		{
			psrcPlayerHomingCore[aintHomingNo].penuImgState[i + 40] = enuPlayerHomingImgState.intNormal02;
		}
	}

	private void NormalStateSet(int aintHomingNo)
	{
		if (psrcPlayerHomingCore[aintHomingNo].penuImgState[0] == enuPlayerHomingImgState.intKieru)
		{
			NormalStartStateSet(aintHomingNo);
		}
	}

	public void pPlayerHomingDraw(SpriteBatch aspritesBatch)
	{
		if (pimgPlayerHoming[1] == null)
		{
			return;
		}
		for (int i = 0; i < psrcPlayerHomingCore.Length; i++)
		{
			if (psrcPlayerHomingCore[i].pflgEnable && psrcPlayerHomingCore[i].penuImgState[0] != enuPlayerHomingImgState.intKieru)
			{
				int width = pimgPlayerHoming[(int)psrcPlayerHomingCore[i].penuImgState[0]].Width;
				int height = pimgPlayerHoming[(int)psrcPlayerHomingCore[i].penuImgState[0]].Height;
				aspritesBatch.Draw(pimgPlayerHoming[(int)psrcPlayerHomingCore[i].penuImgState[0]], new Vector2(psrcPlayerHomingCore[i].pVecIti.X * psrcPlayerHomingCore[i].pVecIti.Z + (640f + 0f * psrcPlayerHomingCore[i].pVecIti.Z), psrcPlayerHomingCore[i].pVecIti.Y * psrcPlayerHomingCore[i].pVecIti.Z + (360f + 0f * psrcPlayerHomingCore[i].pVecIti.Z)), null, new Color(128, 128, 128, 128), MathHelper.ToRadians(0f), new Vector2(width / 2, height / 2), new Vector2(psrcPlayerHomingCore[i].pVecIti.Z, psrcPlayerHomingCore[i].pVecIti.Z), SpriteEffects.None, psrcPlayerHomingCore[i].pVecIti.Z);
			}
		}
	}
}
