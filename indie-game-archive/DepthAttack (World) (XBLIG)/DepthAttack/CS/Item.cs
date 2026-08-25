using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DepthAttack.CS;

public class Item : DrawableGameComponent
{
	public struct srcItemCore
	{
		public bool pflgEnable;

		public Vector3 pVecIti;

		public Vector3 pVecMovIti;

		public enuItemImgState[] penuImgState;

		public enuItemType penuType;
	}

	public enum enuItemImgState
	{
		intKieru,
		intNormal
	}

	public enum enuItemType
	{
		intLifeRecover00
	}

	private const string cstrItem00_00 = "PNG\\Item\\Beaker04";

	public srcItemCore[] psrcItemCore = new srcItemCore[16];

	public Texture2D[,] pimgItem = new Texture2D[1, 2];

	public float[] fltOffSetHaba = new float[1] { 1f / 32f };

	public Rectangle[,] precItemOffSet = new Rectangle[1, 4] { 
	{
		new Rectangle(-40, -128, 80, 256),
		new Rectangle(-128, 0, 128, 128),
		new Rectangle(0, 0, 0, 0),
		new Rectangle(0, 0, 0, 0)
	} };

	public Item(Game game)
		: base(game)
	{
		for (int i = 0; i < psrcItemCore.Length; i++)
		{
			psrcItemCore[i].penuImgState = new enuItemImgState[30];
		}
	}

	public void psrcItemCoreInit()
	{
		for (int i = 0; i < psrcItemCore.Length; i++)
		{
			psrcKakuItemCoreInit(i);
		}
	}

	public void psrcKakuItemCoreInit(int aintItem)
	{
		psrcItemCore[aintItem].pflgEnable = false;
		psrcItemCore[aintItem].pVecIti.X = 0f;
		psrcItemCore[aintItem].pVecIti.Y = 0f;
		psrcItemCore[aintItem].pVecIti.Z = 0f;
		psrcItemCore[aintItem].pVecMovIti.X = 0f;
		psrcItemCore[aintItem].pVecMovIti.Y = 0f;
		psrcItemCore[aintItem].pVecMovIti.Z = 0f;
		psrcItemCore[aintItem].penuType = enuItemType.intLifeRecover00;
	}

	public void pItemEnable(Vector3 avec3Iti, Vector3 avec3MovIti, enuItemType aenuItemType)
	{
		for (int i = 0; i < psrcItemCore.Length; i++)
		{
			if (!psrcItemCore[i].pflgEnable)
			{
				psrcItemCore[i].pflgEnable = true;
				psrcItemCore[i].pVecIti.X = avec3Iti.X;
				psrcItemCore[i].pVecIti.Y = avec3Iti.Y;
				psrcItemCore[i].pVecIti.Z = avec3Iti.Z;
				psrcItemCore[i].pVecMovIti.X = avec3MovIti.X;
				psrcItemCore[i].pVecMovIti.Y = avec3MovIti.Y;
				psrcItemCore[i].pVecMovIti.Z = avec3MovIti.Z;
				psrcItemCore[i].penuType = aenuItemType;
				NomarlStartStateSet(i);
				break;
			}
		}
	}

	public void pItemUpdate()
	{
		pItemHantei();
		for (int i = 0; i < psrcItemCore.Length; i++)
		{
			if (psrcItemCore[i].pflgEnable)
			{
				ItemUpDate(i);
			}
		}
		pImageStateUpdate();
	}

	private void ItemUpDate(int aintItemNo)
	{
		psrcItemCore[aintItemNo].pVecIti.X += psrcItemCore[aintItemNo].pVecMovIti.X;
		psrcItemCore[aintItemNo].pVecIti.Y += psrcItemCore[aintItemNo].pVecMovIti.Y;
		psrcItemCore[aintItemNo].pVecIti.Z += psrcItemCore[aintItemNo].pVecMovIti.Z;
		NomarlStateSet(aintItemNo);
	}

	public void pItemHantei()
	{
		ItemHazureHantei();
	}

	private void ItemHazureHantei()
	{
		for (int i = 0; i < psrcItemCore.Length; i++)
		{
			if (psrcItemCore[i].pflgEnable && ((double)psrcItemCore[i].pVecIti.Z > 63.0 / 64.0 || psrcItemCore[i].pVecIti.X < -5000f || psrcItemCore[i].pVecIti.X > 5000f))
			{
				psrcItemCore[i].pflgEnable = false;
				psrcItemCore[i].pVecIti.X = 0f;
				psrcItemCore[i].pVecIti.Y = 0f;
				psrcItemCore[i].pVecIti.Z = 0f;
				psrcItemCore[i].pVecMovIti.X = 0f;
				psrcItemCore[i].pVecMovIti.Y = 0f;
				psrcItemCore[i].pVecMovIti.Z = 0f;
			}
		}
	}

	public bool pItemPlayerHantei(Vector3 avecIti, float afltHabaHantei, Rectangle arecHantei)
	{
		bool result = false;
		Rectangle rectangle = new Rectangle
		{
			X = arecHantei.X + (int)avecIti.X,
			Y = arecHantei.Y + (int)avecIti.Y,
			Width = arecHantei.Width,
			Height = arecHantei.Height
		};
		for (int i = 0; i < psrcItemCore.Length; i++)
		{
			if (!psrcItemCore[i].pflgEnable || ((!(avecIti.Z <= psrcItemCore[i].pVecIti.Z) || !(avecIti.Z + afltHabaHantei >= psrcItemCore[i].pVecIti.Z)) && (!(avecIti.Z <= psrcItemCore[i].pVecIti.Z + fltOffSetHaba[(int)psrcItemCore[i].penuType]) || !(avecIti.Z + afltHabaHantei >= psrcItemCore[i].pVecIti.Z + fltOffSetHaba[(int)psrcItemCore[i].penuType]))))
			{
				continue;
			}
			for (int j = 0; j < precItemOffSet.GetUpperBound(1) && (precItemOffSet[(int)psrcItemCore[i].penuType, j].X != 0 || precItemOffSet[(int)psrcItemCore[i].penuType, j].Y != 0 || precItemOffSet[(int)psrcItemCore[i].penuType, j].Width != 0 || precItemOffSet[(int)psrcItemCore[i].penuType, j].Height != 0); j++)
			{
				if (rectangle.Intersects(new Rectangle((int)psrcItemCore[i].pVecIti.X + precItemOffSet[(int)psrcItemCore[i].penuType, j].X, (int)psrcItemCore[i].pVecIti.Y + precItemOffSet[(int)psrcItemCore[i].penuType, j].Y, precItemOffSet[(int)psrcItemCore[i].penuType, j].Width, precItemOffSet[(int)psrcItemCore[i].penuType, j].Height)))
				{
					result = true;
					psrcKakuItemCoreInit(i);
					if (psrcItemCore[i].penuType == enuItemType.intLifeRecover00)
					{
						Game1.bGM.pflgSEKetteiStart[0] = true;
						Game1.player.pPlayerHpUp(100);
						Game1.score.pScoreUp(2000L);
					}
					return result;
				}
			}
		}
		return result;
	}

	public override void Initialize()
	{
		psrcItemCoreInit();
		base.Initialize();
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}

	protected override void LoadContent()
	{
		pimgItem[0, 0] = null;
		pimgItem[0, 1] = base.Game.Content.Load<Texture2D>("PNG\\Item\\Beaker04");
		base.LoadContent();
	}

	public void pImageStateUpdate()
	{
		for (int i = 0; i < psrcItemCore.Length; i++)
		{
			if (psrcItemCore[i].pflgEnable)
			{
				for (int j = 0; j < psrcItemCore[i].penuImgState.Length - 1; j++)
				{
					psrcItemCore[i].penuImgState[j] = psrcItemCore[i].penuImgState[j + 1];
				}
			}
		}
	}

	private void NomarlStartStateSet(int aintItemNo)
	{
		for (int i = 0; i < psrcItemCore[aintItemNo].penuImgState.Length; i++)
		{
			psrcItemCore[aintItemNo].penuImgState[i] = enuItemImgState.intNormal;
		}
	}

	private void NomarlStateSet(int aintItemNo)
	{
		if (psrcItemCore[aintItemNo].penuImgState[1] == enuItemImgState.intKieru)
		{
			for (int i = 0; i < psrcItemCore[aintItemNo].penuImgState.Length; i++)
			{
				psrcItemCore[aintItemNo].penuImgState[i] = enuItemImgState.intNormal;
			}
		}
	}

	public void pItemDraw(SpriteBatch aspritesBatch)
	{
		if (pimgItem[0, 1] == null)
		{
			return;
		}
		for (int i = 0; i < psrcItemCore.Length; i++)
		{
			if (psrcItemCore[i].pflgEnable && psrcItemCore[i].penuImgState[0] != enuItemImgState.intKieru)
			{
				int width = pimgItem[(int)psrcItemCore[i].penuType, (int)psrcItemCore[i].penuImgState[0]].Width;
				int height = pimgItem[(int)psrcItemCore[i].penuType, (int)psrcItemCore[i].penuImgState[0]].Height;
				aspritesBatch.Draw(pimgItem[(int)psrcItemCore[i].penuType, (int)psrcItemCore[i].penuImgState[0]], new Vector2(psrcItemCore[i].pVecIti.X * psrcItemCore[i].pVecIti.Z + (640f + 0f * psrcItemCore[i].pVecIti.Z), psrcItemCore[i].pVecIti.Y * psrcItemCore[i].pVecIti.Z + (360f + 0f * psrcItemCore[i].pVecIti.Z)), null, new Color(256, 256, 256, 256), MathHelper.ToRadians(0f), new Vector2(width / 2, height / 2), new Vector2(psrcItemCore[i].pVecIti.Z, psrcItemCore[i].pVecIti.Z), SpriteEffects.None, psrcItemCore[i].pVecIti.Z);
			}
		}
	}

	public override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
	}
}
