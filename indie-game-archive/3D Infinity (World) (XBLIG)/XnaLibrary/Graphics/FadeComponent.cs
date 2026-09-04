using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XnaLibrary.Graphics;

public class FadeComponent : DrawableGameComponent
{
	private readonly TimeSpan DefaultFadeTime;

	private SpriteBatch batch;

	private Texture2D texture;

	private Vector4 fromColor;

	private Vector4 toColor;

	private TimeSpan currentFadeTime;

	private Rectangle screenRect;

	private bool isFading;

	private float beforeAmount;

	[CompilerGenerated]
	private Color _003CColor_003Ek__BackingField;

	[CompilerGenerated]
	private SpriteBlendMode _003CBlendMode_003Ek__BackingField;

	public TimeSpan FadeTime { get; set; }

	public bool IsFading => isFading;

	public Color Color
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CColor_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CColor_003Ek__BackingField = value;
		}
	}

	public SpriteBlendMode BlendMode
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CBlendMode_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CBlendMode_003Ek__BackingField = value;
		}
	}

	public event EventHandler FadeFinished;

	public FadeComponent(Game game)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		DefaultFadeTime = new TimeSpan(0, 0, 1);
		((DrawableGameComponent)this)._002Ector(game);
		Color = Color.Black;
	}

	public override void Initialize()
	{
		FadeTime = DefaultFadeTime;
		BlendMode = (SpriteBlendMode)1;
		((DrawableGameComponent)this).Initialize();
	}

	protected override void LoadContent()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		fromColor = Vector4.Zero;
		toColor = Vector4.Zero;
		isFading = false;
		texture = new Texture2D(((DrawableGameComponent)this).GraphicsDevice, 1, 1, 1, (TextureUsage)0, (SurfaceFormat)1);
		texture.SetData<Color>((Color[])(object)new Color[1] { Color.White });
		Viewport viewport = ((DrawableGameComponent)this).GraphicsDevice.Viewport;
		int width = ((Viewport)(ref viewport)).Width;
		Viewport viewport2 = ((DrawableGameComponent)this).GraphicsDevice.Viewport;
		screenRect = new Rectangle(0, 0, width, ((Viewport)(ref viewport2)).Height);
		batch = new SpriteBatch(((DrawableGameComponent)this).GraphicsDevice);
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
		if (!isFading)
		{
			return;
		}
		currentFadeTime += gameTime.ElapsedGameTime;
		if (isFading && GetAmount() == 1f && beforeAmount == 1f)
		{
			isFading = false;
			if (FadeFinished != null)
			{
				FadeFinished(this, EventArgs.Empty);
			}
		}
		beforeAmount = GetAmount();
		((GameComponent)this).Update(gameTime);
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		batch.Begin(BlendMode, (SpriteSortMode)0, (SaveStateMode)0);
		batch.Draw(texture, screenRect, new Color(GetCurrentColor()));
		batch.End();
		((DrawableGameComponent)this).Draw(gameTime);
	}

	public void FadeIn()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Color color = Color;
		Vector3 val = ((Color)(ref color)).ToVector3();
		Fade(new Vector4(val, 1f), new Vector4(val, 0f));
	}

	public void FadeOut()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Color color = Color;
		Vector3 val = ((Color)(ref color)).ToVector3();
		Fade(new Vector4(val, 0f), new Vector4(val, 1f));
	}

	public void Fade(Vector4 fromColor, Vector4 toColor)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		this.fromColor = fromColor;
		this.toColor = toColor;
		currentFadeTime = TimeSpan.Zero;
		isFading = true;
	}

	public void ClearEvents()
	{
		FadeFinished = null;
	}

	public float GetAmount()
	{
		float num = (float)(currentFadeTime.TotalSeconds / FadeTime.TotalSeconds);
		return MathHelper.Clamp(num, 0f, 1f);
	}

	public Vector4 GetCurrentColor()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return Vector4.Lerp(fromColor, toColor, GetAmount());
	}
}
