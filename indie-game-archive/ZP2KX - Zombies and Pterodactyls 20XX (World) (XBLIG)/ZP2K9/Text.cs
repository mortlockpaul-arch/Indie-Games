using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9;

public class Text
{
	public const int ALIGN_LEFT = 0;

	public const int ALIGN_CENTER = 1;

	public const int ALIGN_RIGHT = 2;

	public float size;

	public Color color;

	public void DrawString(Vector2 loc, string s, int align, float maxLen, SpriteFont font, SpriteBatch sprite)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		float num = font.MeasureString(s).X;
		float num2 = 1f;
		if (maxLen > -1f && num > maxLen)
		{
			num2 = maxLen / num;
			num = maxLen;
		}
		switch (align)
		{
		case 1:
			loc.X -= num * 0.5f * size;
			break;
		case 2:
			loc.X -= num * size;
			break;
		}
		sprite.DrawString(font, s, loc, color, 0f, default(Vector2), new Vector2(size * num2, size), (SpriteEffects)0, 1f);
	}

	public float GetStringLength(StringBuilder s, SpriteFont font)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return font.MeasureString(s).X * size;
	}

	public void DrawString(Vector2 loc, StringBuilder s, int align, float maxLen, SpriteFont font, SpriteBatch sprite)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		try
		{
			num = font.MeasureString(s).X * size;
		}
		catch (Exception)
		{
		}
		float num2 = 1f;
		if (maxLen > -1f && num > maxLen)
		{
			num2 = maxLen / num;
			num = maxLen;
		}
		switch (align)
		{
		case 1:
			loc.X -= num * 0.5f;
			break;
		case 2:
			loc.X -= num;
			break;
		}
		try
		{
			sprite.DrawString(font, s, loc, color, 0f, default(Vector2), new Vector2(size * num2, size), (SpriteEffects)0, 1f);
		}
		catch (Exception)
		{
		}
	}
}
