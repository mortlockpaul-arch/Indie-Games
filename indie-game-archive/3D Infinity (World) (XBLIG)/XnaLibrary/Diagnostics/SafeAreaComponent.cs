using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XnaLibrary.Diagnostics;

public class SafeAreaComponent : DrawableGameComponent
{
	private readonly Color DefaultColor;

	private SpriteBatch batch;

	private Texture2D texture;

	private Rectangle[] regions;

	private Color color;

	private float safeRate;

	public Color Color
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return color;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			color = value;
		}
	}

	public float SafeRate
	{
		get
		{
			return safeRate;
		}
		set
		{
			safeRate = MathHelper.Clamp(value, 0f, 1f);
			regions = GetRegions(safeRate);
		}
	}

	public SafeAreaComponent(Game game)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		DefaultColor = new Color(byte.MaxValue, (byte)0, byte.MaxValue, (byte)64);
		((DrawableGameComponent)this)._002Ector(game);
	}

	public override void Initialize()
	{
		((DrawableGameComponent)this).Initialize();
	}

	protected override void LoadContent()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		batch = new SpriteBatch(((DrawableGameComponent)this).GraphicsDevice);
		texture = new Texture2D(((DrawableGameComponent)this).GraphicsDevice, 1, 1, 1, (TextureUsage)0, (SurfaceFormat)1);
		texture.SetData<Color>((Color[])(object)new Color[1] { Color.White });
		Color = DefaultColor;
		SafeRate = 0.1f;
		((DrawableGameComponent)this).LoadContent();
	}

	protected override void UnloadContent()
	{
		batch.Dispose();
		((GraphicsResource)texture).Dispose();
		((DrawableGameComponent)this).UnloadContent();
	}

	public override void Update(GameTime gameTime)
	{
		((GameComponent)this).Update(gameTime);
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		batch.Begin();
		Rectangle[] array = regions;
		foreach (Rectangle val in array)
		{
			batch.Draw(texture, val, Color);
		}
		batch.End();
		((DrawableGameComponent)this).Draw(gameTime);
	}

	private Rectangle[] GetRegions(float rate)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		Viewport viewport = ((DrawableGameComponent)this).GraphicsDevice.Viewport;
		int width = ((Viewport)(ref viewport)).Width;
		Viewport viewport2 = ((DrawableGameComponent)this).GraphicsDevice.Viewport;
		int height = ((Viewport)(ref viewport2)).Height;
		int num = (int)((float)width * (rate * 0.5f));
		int num2 = (int)((float)height * (rate * 0.5f));
		return (Rectangle[])(object)new Rectangle[4]
		{
			new Rectangle(0, 0, width, num2),
			new Rectangle(0, height - num2, width, num2),
			new Rectangle(0, num2, num, height - num2 * 2),
			new Rectangle(width - num, num2, num, height - num2 * 2)
		};
	}
}
