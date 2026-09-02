using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.Helpers;

namespace RacingGame.Graphics;

public class Texture : IDisposable
{
	public static SpriteBatch alphaSprite;

	public static SpriteBatch additiveSprite;

	protected string texFilename;

	protected int texWidth;

	protected int texHeight;

	private Vector2 precaledHalfPixelSize;

	protected Texture2D internalXnaTexture;

	protected bool loaded;

	protected string error;

	protected bool hasAlpha;

	public string Filename => texFilename;

	public int Width => texWidth;

	public int Height => texHeight;

	public Rectangle GfxRectangle
	{
		get
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			return new Rectangle(0, 0, texWidth, texHeight);
		}
	}

	public Vector2 HalfPixelSize
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return precaledHalfPixelSize;
		}
	}

	public virtual Texture2D XnaTexture => internalXnaTexture;

	public virtual bool Valid
	{
		get
		{
			if (loaded)
			{
				return internalXnaTexture != null;
			}
			return false;
		}
	}

	public bool HasAlphaPixels => hasAlpha;

	protected void CalcHalfPixelSize()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		precaledHalfPixelSize = new Vector2(1f / (float)texWidth / 2f, 1f / (float)texHeight / 2f);
	}

	public Texture(string setFilename)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Invalid comparison between Unknown and I4
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Invalid comparison between Unknown and I4
		texFilename = "";
		precaledHalfPixelSize = Vector2.Zero;
		loaded = true;
		error = "";
		base._002Ector();
		if (alphaSprite == null)
		{
			alphaSprite = new SpriteBatch(BaseGame.Device);
		}
		if (additiveSprite == null)
		{
			additiveSprite = new SpriteBatch(BaseGame.Device);
		}
		if (string.IsNullOrEmpty(setFilename))
		{
			throw new ArgumentNullException("setFilename", "Unable to create texture without valid filename.");
		}
		texFilename = Path.GetFileNameWithoutExtension(setFilename);
		string text = Path.Combine(Directories.ContentDirectory + "\\textures", texFilename);
		internalXnaTexture = BaseGame.Content.Load<Texture2D>(text);
		texWidth = internalXnaTexture.Width;
		texHeight = internalXnaTexture.Height;
		hasAlpha = (int)internalXnaTexture.Format == 32 || (int)internalXnaTexture.Format == 30;
		loaded = true;
		CalcHalfPixelSize();
	}

	protected Texture()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		texFilename = "";
		precaledHalfPixelSize = Vector2.Zero;
		loaded = true;
		error = "";
		base._002Ector();
	}

	public Texture(Texture2D tex)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Invalid comparison between Unknown and I4
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Invalid comparison between Unknown and I4
		texFilename = "";
		precaledHalfPixelSize = Vector2.Zero;
		loaded = true;
		error = "";
		base._002Ector();
		if (alphaSprite == null)
		{
			alphaSprite = new SpriteBatch(BaseGame.Device);
		}
		if (additiveSprite == null)
		{
			additiveSprite = new SpriteBatch(BaseGame.Device);
		}
		if (tex == null)
		{
			throw new ArgumentNullException("tex");
		}
		internalXnaTexture = tex;
		texWidth = internalXnaTexture.Width;
		texHeight = internalXnaTexture.Height;
		loaded = true;
		hasAlpha = (int)internalXnaTexture.Format == 32 || (int)internalXnaTexture.Format == 30;
		CalcHalfPixelSize();
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (internalXnaTexture != null)
			{
				((GraphicsResource)internalXnaTexture).Dispose();
			}
			internalXnaTexture = null;
		}
		loaded = false;
	}

	public void RenderOnScreen(Rectangle rect, Rectangle pixelRect)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		alphaSprite.Draw(internalXnaTexture, rect, (Rectangle?)pixelRect, Color.White);
	}

	public void RenderOnScreen(Rectangle rect, int pixelX, int pixelY, int pixelWidth, int pixelHeight)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		alphaSprite.Draw(internalXnaTexture, rect, (Rectangle?)new Rectangle(pixelX, pixelY, pixelWidth, pixelHeight), Color.White);
	}

	public void RenderOnScreen(Point pos)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		alphaSprite.Draw(internalXnaTexture, new Rectangle(pos.X, pos.Y, texWidth, texHeight), (Rectangle?)new Rectangle(0, 0, texWidth, texHeight), Color.White);
	}

	public void RenderOnScreen(Rectangle renderRect)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		alphaSprite.Draw(internalXnaTexture, renderRect, (Rectangle?)GfxRectangle, Color.White);
	}

	public void RenderOnScreenRelative16To9(int relX, int relY, Rectangle pixelRect)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		alphaSprite.Draw(internalXnaTexture, BaseGame.CalcRectangle(relX, relY, pixelRect.Width, pixelRect.Height), (Rectangle?)pixelRect, Color.White);
	}

	public void RenderOnScreenRelative4To3(int relX, int relY, Rectangle pixelRect)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		alphaSprite.Draw(internalXnaTexture, BaseGame.CalcRectangleKeep4To3(relX, relY, pixelRect.Width, pixelRect.Height), (Rectangle?)pixelRect, Color.White);
	}

	public void RenderOnScreenRelative1600(int relX, int relY, Rectangle pixelRect)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		alphaSprite.Draw(internalXnaTexture, BaseGame.CalcRectangle1600(relX, relY, pixelRect.Width, pixelRect.Height), (Rectangle?)pixelRect, Color.White);
	}

	public void RenderOnScreen(Rectangle rect, Rectangle pixelRect, Color color)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		alphaSprite.Draw(internalXnaTexture, rect, (Rectangle?)pixelRect, color);
	}

	public void RenderOnScreen(Rectangle rect, Rectangle pixelRect, Color color, SpriteBlendMode blendMode)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if ((int)blendMode == 2)
		{
			additiveSprite.Draw(internalXnaTexture, rect, (Rectangle?)pixelRect, color);
		}
		else
		{
			alphaSprite.Draw(internalXnaTexture, rect, (Rectangle?)pixelRect, color);
		}
	}

	public void RenderOnScreenWithRotation(Rectangle rect, Rectangle pixelRect, float rotation, Vector2 rotationPoint)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		alphaSprite.Draw(internalXnaTexture, rect, (Rectangle?)pixelRect, Color.White, rotation, rotationPoint, (SpriteEffects)0, 0f);
	}

	public override string ToString()
	{
		return "Texture(filename=" + texFilename + ", width=" + texWidth + ", height=" + texHeight + ", xnaTexture=" + ((internalXnaTexture != null) ? "valid" : "null") + ")";
	}
}
