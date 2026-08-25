using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DepthAttack.CS;

public class CPUTama : DrawableGameComponent
{
	public struct srcCPUTamaCore
	{
		public bool pflgEnable;

		public Vector3 pVecIti;

		public Vector3 pVecMovIti;

		public enuCPUTamaImgState[] penuImgState;
	}

	public enum enuCPUTamaImgState
	{
		intKieru,
		intNormal00,
		intNormal01,
		intNormal02
	}

	private const string cstrCPUTama00 = "PNG\\Tama\\CPUTama02";

	private const string cstrCPUTama01 = "PNG\\Tama\\CPUTama02";

	private const string cstrCPUTama02 = "PNG\\Tama\\CPUTama02";

	public const float cfltOffSetHaba = 1f / 32f;

	private const float cfltZTani = 1f / 128f;

	public srcCPUTamaCore[] psrcCPUTamaCore = new srcCPUTamaCore[16];

	public Texture2D[] pimgCPUTama = new Texture2D[4];

	public Rectangle[] precTamaOffSet = new Rectangle[1]
	{
		new Rectangle(-69, -69, 118, 118)
	};

	public CPUTama(Game game)
		: base(game)
	{
		for (int i = 0; i < psrcCPUTamaCore.Length; i++)
		{
			psrcCPUTamaCore[i].penuImgState = new enuCPUTamaImgState[30];
		}
	}

	public override void Initialize()
	{
		psrcCPUTamaCoreInit();
		base.Initialize();
	}

	public void psrcCPUTamaCoreInit()
	{
		for (int i = 0; i < psrcCPUTamaCore.Length; i++)
		{
			psrcCPUTamaCore[i].pflgEnable = false;
			psrcCPUTamaCore[i].pVecIti.X = 0f;
			psrcCPUTamaCore[i].pVecIti.Y = 0f;
			psrcCPUTamaCore[i].pVecIti.Z = 0f;
			psrcCPUTamaCore[i].pVecMovIti.X = 0f;
			psrcCPUTamaCore[i].pVecMovIti.Y = 0f;
			psrcCPUTamaCore[i].pVecMovIti.Z = 0f;
			NormalStateSet(i);
		}
	}

	public void psrcCPUTamaCoreEnable(Vector3 avec3Iti, Vector3 avec3MovIti)
	{
		for (int i = 0; i < psrcCPUTamaCore.Length; i++)
		{
			if (!psrcCPUTamaCore[i].pflgEnable)
			{
				psrcCPUTamaCore[i].pflgEnable = true;
				psrcCPUTamaCore[i].pVecIti.X = avec3Iti.X;
				psrcCPUTamaCore[i].pVecIti.Y = avec3Iti.Y;
				psrcCPUTamaCore[i].pVecIti.Z = avec3Iti.Z;
				psrcCPUTamaCore[i].pVecMovIti.X = avec3MovIti.X;
				psrcCPUTamaCore[i].pVecMovIti.Y = avec3MovIti.Y;
				psrcCPUTamaCore[i].pVecMovIti.Z = avec3MovIti.Z;
				NormalStartStateSet(i);
				break;
			}
		}
	}

	public void psrcCPUTamaCorePlayerItiEnable(Vector3 avec3Iti, Vector3 avec3MovIti, Vector3 avec3PlayerIti)
	{
		for (int i = 0; i < psrcCPUTamaCore.Length; i++)
		{
			if (!psrcCPUTamaCore[i].pflgEnable)
			{
				Game1.bGM.pflgSECPUTama[0] = true;
				psrcCPUTamaCore[i].pflgEnable = true;
				psrcCPUTamaCore[i].pVecIti.X = avec3Iti.X;
				psrcCPUTamaCore[i].pVecIti.Y = avec3Iti.Y;
				psrcCPUTamaCore[i].pVecIti.Z = avec3Iti.Z;
				double num = dblVec3Kyori(avec3PlayerIti, avec3Iti);
				psrcCPUTamaCore[i].pVecMovIti.X = (float)((double)(avec3PlayerIti.X - avec3Iti.X) / num * 20.0);
				psrcCPUTamaCore[i].pVecMovIti.Y = (float)((double)(avec3PlayerIti.Y - avec3Iti.Y) / num * 20.0);
				psrcCPUTamaCore[i].pVecMovIti.Z = (float)((double)(avec3PlayerIti.Z - avec3Iti.Z) / num * 20.0);
				NormalStartStateSet(i);
				break;
			}
		}
	}

	private double dblVec3Kyori(Vector3 vec3Jibun, Vector3 Vec3Aite)
	{
		Vector3 vector = default(Vector3);
		vector.X = vec3Jibun.X - Vec3Aite.X;
		vector.Y = vec3Jibun.Y - Vec3Aite.Y;
		vector.Z = (vec3Jibun.Z - Vec3Aite.Z) / (1f / 128f) * 8f;
		return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
	}

	private double dblVec3KyoriX(Vector3 vec3Jibun, Vector3 Vec3Aite)
	{
		Vector3 vector = default(Vector3);
		vector.X = vec3Jibun.X - Vec3Aite.X;
		vector.Y = 0f;
		vector.Z = (vec3Jibun.Z - Vec3Aite.Z) / (1f / 128f);
		return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
	}

	private double dblVec3KyoriY(Vector3 vec3Jibun, Vector3 Vec3Aite)
	{
		Vector3 vector = default(Vector3);
		vector.X = 0f;
		vector.Y = vec3Jibun.Y - Vec3Aite.Y;
		vector.Z = (vec3Jibun.Z - Vec3Aite.Z) / (1f / 128f);
		return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
	}

	public void pCPUTamaUpdate()
	{
		for (int i = 0; i < psrcCPUTamaCore.Length; i++)
		{
			if (psrcCPUTamaCore[i].pflgEnable)
			{
				psrcCPUTamaCore[i].pVecIti.X += psrcCPUTamaCore[i].pVecMovIti.X;
				psrcCPUTamaCore[i].pVecIti.Y += psrcCPUTamaCore[i].pVecMovIti.Y;
				psrcCPUTamaCore[i].pVecIti.Z += psrcCPUTamaCore[i].pVecMovIti.Z;
				NormalStateSet(i);
			}
		}
		pImageStateUpdate();
	}

	public void pCPUTamaHantei()
	{
		CPUTamaHazureHantei();
	}

	private void CPUTamaHazureHantei()
	{
		for (int i = 0; i < psrcCPUTamaCore.Length; i++)
		{
			if (psrcCPUTamaCore[i].pflgEnable && (double)psrcCPUTamaCore[i].pVecIti.Z > 127.0 / 128.0)
			{
				psrcCPUTamaCore[i].pflgEnable = false;
				psrcCPUTamaCore[i].pVecIti.X = 0f;
				psrcCPUTamaCore[i].pVecIti.Y = 0f;
				psrcCPUTamaCore[i].pVecIti.Z = 0f;
				psrcCPUTamaCore[i].pVecMovIti.X = 0f;
				psrcCPUTamaCore[i].pVecMovIti.Y = 0f;
				psrcCPUTamaCore[i].pVecMovIti.Z = 0f;
			}
		}
	}

	public bool pCPUTamaAtariHantei(Vector3 avecIti, float afltHabaHantei, Rectangle arecHantei)
	{
		bool result = false;
		Rectangle rectangle = new Rectangle
		{
			X = arecHantei.X + (int)avecIti.X,
			Y = arecHantei.Y + (int)avecIti.Y,
			Width = arecHantei.Width,
			Height = arecHantei.Height
		};
		for (int i = 0; i < psrcCPUTamaCore.Length; i++)
		{
			if (psrcCPUTamaCore[i].pflgEnable && ((avecIti.Z <= psrcCPUTamaCore[i].pVecIti.Z && avecIti.Z + afltHabaHantei >= psrcCPUTamaCore[i].pVecIti.Z) || (avecIti.Z <= psrcCPUTamaCore[i].pVecIti.Z + 1f / 32f && avecIti.Z + afltHabaHantei >= psrcCPUTamaCore[i].pVecIti.Z + 1f / 32f)) && rectangle.Intersects(new Rectangle((int)psrcCPUTamaCore[i].pVecIti.X + precTamaOffSet[0].X, (int)psrcCPUTamaCore[i].pVecIti.Y + precTamaOffSet[0].Y, precTamaOffSet[0].Width, precTamaOffSet[0].Height)))
			{
				Game1.bakuhatu.psrcHitCoreConboEnable(psrcCPUTamaCore[i].pVecIti, 0f, 0);
				return true;
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
		pimgCPUTama[0] = null;
		pimgCPUTama[1] = base.Game.Content.Load<Texture2D>("PNG\\Tama\\CPUTama02");
		pimgCPUTama[2] = base.Game.Content.Load<Texture2D>("PNG\\Tama\\CPUTama02");
		pimgCPUTama[3] = base.Game.Content.Load<Texture2D>("PNG\\Tama\\CPUTama02");
		base.LoadContent();
	}

	public void pImageStateUpdate()
	{
		for (int i = 0; i < psrcCPUTamaCore.Length; i++)
		{
			if (psrcCPUTamaCore[i].pflgEnable)
			{
				for (int j = 0; j < psrcCPUTamaCore[i].penuImgState.Length - 1; j++)
				{
					psrcCPUTamaCore[i].penuImgState[j] = psrcCPUTamaCore[i].penuImgState[j + 1];
				}
			}
		}
	}

	private void NormalStartStateSet(int aintVulcanNo)
	{
		for (int i = 0; i < psrcCPUTamaCore[aintVulcanNo].penuImgState.Length - 29; i += 29)
		{
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i] = enuCPUTamaImgState.intNormal00;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 1] = enuCPUTamaImgState.intNormal00;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 2] = enuCPUTamaImgState.intNormal00;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 3] = enuCPUTamaImgState.intNormal00;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 4] = enuCPUTamaImgState.intNormal00;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 5] = enuCPUTamaImgState.intNormal00;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 6] = enuCPUTamaImgState.intNormal00;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 7] = enuCPUTamaImgState.intNormal00;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 8] = enuCPUTamaImgState.intNormal00;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 9] = enuCPUTamaImgState.intNormal00;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 10] = enuCPUTamaImgState.intNormal01;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 11] = enuCPUTamaImgState.intNormal01;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 12] = enuCPUTamaImgState.intNormal01;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 13] = enuCPUTamaImgState.intNormal01;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 14] = enuCPUTamaImgState.intNormal01;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 15] = enuCPUTamaImgState.intNormal01;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 16] = enuCPUTamaImgState.intNormal01;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 17] = enuCPUTamaImgState.intNormal01;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 18] = enuCPUTamaImgState.intNormal01;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 19] = enuCPUTamaImgState.intNormal01;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 20] = enuCPUTamaImgState.intNormal02;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 21] = enuCPUTamaImgState.intNormal02;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 22] = enuCPUTamaImgState.intNormal02;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 23] = enuCPUTamaImgState.intNormal02;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 24] = enuCPUTamaImgState.intNormal02;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 25] = enuCPUTamaImgState.intNormal02;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 26] = enuCPUTamaImgState.intNormal02;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 27] = enuCPUTamaImgState.intNormal02;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 28] = enuCPUTamaImgState.intNormal02;
			psrcCPUTamaCore[aintVulcanNo].penuImgState[i + 29] = enuCPUTamaImgState.intNormal02;
		}
	}

	private void NormalStateSet(int aintVulcanNo)
	{
		if (psrcCPUTamaCore[aintVulcanNo].penuImgState[0] == enuCPUTamaImgState.intKieru)
		{
			NormalStartStateSet(aintVulcanNo);
		}
	}

	public void pCPUTamaDraw(SpriteBatch aspritesBatch)
	{
		if (pimgCPUTama[1] == null)
		{
			return;
		}
		for (int i = 0; i < psrcCPUTamaCore.Length; i++)
		{
			if (psrcCPUTamaCore[i].pflgEnable && psrcCPUTamaCore[i].penuImgState[0] != enuCPUTamaImgState.intKieru)
			{
				int width = pimgCPUTama[(int)psrcCPUTamaCore[i].penuImgState[0]].Width;
				int height = pimgCPUTama[(int)psrcCPUTamaCore[i].penuImgState[0]].Height;
				aspritesBatch.Draw(pimgCPUTama[(int)psrcCPUTamaCore[i].penuImgState[0]], new Vector2(psrcCPUTamaCore[i].pVecIti.X * psrcCPUTamaCore[i].pVecIti.Z + (640f + 0f * psrcCPUTamaCore[i].pVecIti.Z), psrcCPUTamaCore[i].pVecIti.Y * psrcCPUTamaCore[i].pVecIti.Z + (360f + 0f * psrcCPUTamaCore[i].pVecIti.Z)), null, new Color(200, 200, 200, 200), MathHelper.ToRadians(0f), new Vector2(width / 2, height / 2), new Vector2(psrcCPUTamaCore[i].pVecIti.Z, psrcCPUTamaCore[i].pVecIti.Z), SpriteEffects.None, psrcCPUTamaCore[i].pVecIti.Z);
			}
		}
	}

	public override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
	}
}
