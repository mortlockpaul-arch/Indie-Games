using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kobingo.Xna.Library.Graphics;

public static class Extensions
{
	public static void DrawAligned(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Align align, Color color)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		spriteBatch.DrawAligned(texture, position, 0f, 1f, align, color);
	}

	public static void DrawAligned(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, float rotation, float scale, Align align, Color color)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector((float)texture.Width, (float)texture.Height);
		Vector2 val2 = Vector2.Zero;
		switch (align)
		{
		case Align.Center:
			val2 = val / 2f;
			break;
		case Align.Right:
			((Vector2)(ref val2))._002Ector(val.X, 0f);
			break;
		}
		spriteBatch.Draw(texture, position, (Rectangle?)null, color, rotation, val2, scale, (SpriteEffects)0, 0f);
	}

	public static void DrawAligned(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, float rotation, Vector2 scale, Align align, Color color)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector((float)texture.Width, (float)texture.Height);
		Vector2 val2 = Vector2.Zero;
		switch (align)
		{
		case Align.Center:
			val2 = val / 2f;
			break;
		case Align.Right:
			((Vector2)(ref val2))._002Ector(val.X, 0f);
			break;
		}
		spriteBatch.Draw(texture, position, (Rectangle?)null, color, rotation, val2, scale, (SpriteEffects)0, 0f);
	}

	public static void DrawAlignedString(this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Align align, Color color)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = font.MeasureString(text);
		Vector2 val2 = Vector2.Zero;
		switch (align)
		{
		case Align.Center:
			val2 = val / 2f;
			break;
		case Align.Right:
			((Vector2)(ref val2))._002Ector(val.X, 0f);
			break;
		}
		spriteBatch.DrawString(font, text, position, color, 0f, val2, 1f, (SpriteEffects)0, 0f);
	}
}
