using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class UITimer
{
	private Texture2D texCircle;

	private Texture2D texAiguille;

	private Texture2D texAlphaLayerMoving;

	private Texture2D texAlphaLayerHalfFull;

	private Vector2 texSize;

	private Vector2 origin;

	public void LoadContent(ContentManager content)
	{
		texCircle = Utils.Textures.LoadTex(content, "textures/", Utils.Textures.TexSize.Independant, "timer-circle");
		texAiguille = Utils.Textures.LoadTex(content, "textures/", Utils.Textures.TexSize.Independant, "timer-aiguille");
		texAlphaLayerHalfFull = Utils.Textures.LoadTex(content, "textures/", Utils.Textures.TexSize.Independant, "timer-alphalayer");
		texAlphaLayerMoving = Utils.Textures.LoadTex(content, "textures/", Utils.Textures.TexSize.Independant, "timer-alphalayer-anim");
		texSize = new Vector2(texCircle.Width, texCircle.Height);
		origin = texSize * 0.5f;
	}

	public void render(SpriteBatch sb, Rectangle destRect, Color color, float ratio, int remainingSec, SpriteFont font, float fontScale)
	{
		sb.Draw(texCircle, destRect, color);
		Rectangle destinationRectangle = new Rectangle((int)((float)destRect.X + origin.X), (int)((float)destRect.Y + origin.Y), destRect.Width, destRect.Height);
		float rotation = ratio * 1.01f * (float)Math.PI * 2f * -1f;
		sb.Draw(texAiguille, destinationRectangle, null, color, rotation, origin, SpriteEffects.None, 0f);
		float rotation2 = (float)Math.PI;
		Vector2 vector = new Vector2(0f, origin.Y);
		Rectangle destinationRectangle2 = destRect;
		destinationRectangle2.Width /= 2;
		destinationRectangle2.X += (int)origin.X;
		destinationRectangle2.Y += (int)origin.Y;
		destinationRectangle2.X += destinationRectangle2.Width;
		if (ratio <= 0.5f)
		{
			sb.Draw(texAlphaLayerHalfFull, destRect, null, color, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
			rotation2 = 0f;
			destinationRectangle2.X -= destinationRectangle2.Width;
			vector = origin;
		}
		sb.Draw(texAlphaLayerMoving, destinationRectangle2, SourceRectangleAlphaMoving(ratio), color, rotation2, vector, SpriteEffects.None, 0f);
		if (font != null && remainingSec != -1 && remainingSec + 1 < 10)
		{
			string text = (remainingSec + 1).ToString("0");
			sb.DrawString(font, text, new Vector2(destRect.Center.X, destRect.Center.Y + 5) - font.MeasureString(text) * fontScale / 2f, Utils.ColorWithAlpha(Color.Black, color.A), 0f, Vector2.Zero, fontScale, SpriteEffects.None, 0f);
		}
	}

	private Rectangle SourceRectangleAlphaMoving(float ratio)
	{
		Rectangle result = new Rectangle(0, 0, (int)(texSize.X / 2f), (int)texSize.Y);
		float num = ((ratio <= 0.5f) ? ratio : (ratio - 0.5f));
		if (num <= 0.25f)
		{
			result.X = result.Width * (int)(num * 120f);
			result.Y = 0;
		}
		else
		{
			result.X = result.Width * (int)((num - 0.25f) * 120f);
			result.Y = result.Height;
		}
		return result;
	}
}
