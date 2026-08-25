using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DepthAttack.CS;

public class BG02(Game game) : DrawableGameComponent(game)
{
	private const string cstrMainBGSky00 = "PNG\\BackGround\\Taile26";

	private VertexPositionTexture[] VertexData00 = new VertexPositionTexture[4];

	private Texture2D[] imgBG = new Texture2D[2];

	private BasicEffect bEffect;

	private Texture2D imgMainBGSky00;

	private float flt_X_Offset = 0f;

	private float flt_Y_Offset = 0f;

	private float flt_Z_Offset = 0f;

	private float fltViewYOffset = 0f;

	private float fltViewZOffset = 0f;

	public override void Initialize()
	{
		ref VertexPositionTexture reference = ref VertexData00[0];
		reference = new VertexPositionTexture(new Vector3(-2f, 0f, -2f), new Vector2(0f, 0f));
		ref VertexPositionTexture reference2 = ref VertexData00[1];
		reference2 = new VertexPositionTexture(new Vector3(2f, 0f, -2f), new Vector2(1f, 0f));
		ref VertexPositionTexture reference3 = ref VertexData00[2];
		reference3 = new VertexPositionTexture(new Vector3(-2f, 0f, 2f), new Vector2(0f, 1f));
		ref VertexPositionTexture reference4 = ref VertexData00[3];
		reference4 = new VertexPositionTexture(new Vector3(2f, 0f, 2f), new Vector2(1f, 1f));
		pBGTexture(0);
		base.Initialize();
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}

	protected override void LoadContent()
	{
		imgBG[0] = base.Game.Content.Load<Texture2D>("PNG\\BackGround\\BG33");
		imgBG[1] = base.Game.Content.Load<Texture2D>("PNG\\BackGround\\BG27");
		bEffect = new BasicEffect(base.GraphicsDevice);
		bEffect.Texture = imgBG[0];
		bEffect.TextureEnabled = true;
		imgMainBGSky00 = base.Game.Content.Load<Texture2D>("PNG\\BackGround\\Taile26");
		base.LoadContent();
	}

	public void pBGTexture(int imgNo)
	{
		if (bEffect != null)
		{
			bEffect.Texture = imgBG[imgNo];
		}
	}

	public void BG02Update()
	{
		flt_Z_Offset += 0.1f;
		if (flt_Z_Offset > 24f)
		{
			flt_Z_Offset = 0f;
		}
	}

	private void BG_Sky_Draw(SpriteBatch aspritesBatch)
	{
		int num = 170;
		for (int i = -720; i < 250; i += imgMainBGSky00.Height)
		{
			num += 6;
			for (int j = -500; j < 4000; j += imgMainBGSky00.Width)
			{
				aspritesBatch.Draw(imgMainBGSky00, new Vector2(j, i + 70), null, new Color((byte)num, (byte)num, (byte)num, 255), 0f, new Vector2(0f, 0f), new Vector2(1f, 1f), SpriteEffects.None, 0f);
			}
		}
	}

	public void BG02Draw(SpriteBatch aspritesBatch)
	{
		base.GraphicsDevice.BlendState = BlendState.AlphaBlend;
		Matrix identity = Matrix.Identity;
		identity = Matrix.CreateTranslation(new Vector3(10f, 0f, 0f));
		Matrix view = Matrix.CreateLookAt(new Vector3(0f, fltViewYOffset + 0.3f, fltViewZOffset + 6f), new Vector3(Game1.player.pvec3Scroll.X / -200f, fltViewYOffset + 0.4f + Game1.player.pvec3Scroll.Y / 120f, fltViewZOffset - 10f), Vector3.Up);
		Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(22.5f), base.GraphicsDevice.Viewport.Width / base.GraphicsDevice.Viewport.Height, 1f, 100f);
		bEffect.World = identity;
		bEffect.View = view;
		bEffect.Projection = projection;
		for (float num = -80f; num <= 40f; num += 4f)
		{
			for (float num2 = -40f; num2 <= 40f; num2 += 4f)
			{
				identity = Matrix.CreateTranslation(new Vector3(num2 + flt_X_Offset, 0f + flt_Y_Offset, num + flt_Z_Offset));
				bEffect.World = identity;
				foreach (EffectPass pass in bEffect.CurrentTechnique.Passes)
				{
					pass.Apply();
					base.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, VertexData00, 0, 2);
				}
			}
		}
	}

	public override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
	}
}
