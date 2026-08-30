using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class TextureWithName
{
	public enum RelativePos
	{
		NameBelowTexture,
		NameLeftOfTexture,
		NameRightOfTexture
	}

	public Texture2D tex;

	public string name;

	private RelativePos relativePos;

	public TextureWithName(Texture2D tex, string name)
		: this(tex, name, RelativePos.NameBelowTexture)
	{
	}

	public TextureWithName(Texture2D tex, string name, RelativePos relativePos)
	{
		this.tex = tex;
		this.name = name;
		this.relativePos = relativePos;
	}

	public Vector2 Size(SpriteFont font)
	{
		Vector2 result = new Vector2(tex.Width, tex.Height);
		switch (relativePos)
		{
		case RelativePos.NameBelowTexture:
			result.Y += font.MeasureString(name).Y;
			result.X = Math.Max(result.X, font.MeasureString(name).X);
			break;
		case RelativePos.NameLeftOfTexture:
		case RelativePos.NameRightOfTexture:
			result.X += font.MeasureString(name).X;
			result.Y = Math.Max(result.Y, font.MeasureString(name).Y);
			break;
		default:
			throw new Exception("relative pos not supported : " + relativePos);
		}
		return result;
	}

	private void ComputeDrawInfo(SpriteFont font, Vector2 pos, bool isLocked, out Vector2 posTex, out Vector2 posName)
	{
		Vector2 vector = font.MeasureString(isLocked ? "Locked " : name);
		switch (relativePos)
		{
		case RelativePos.NameBelowTexture:
			posTex = pos;
			posName = new Vector2(pos.X, pos.Y + (float)tex.Height + (float)font.LineSpacing);
			if (vector.X > (float)tex.Width)
			{
				posTex.X += vector.X / 2f - (float)(tex.Width / 2);
			}
			else
			{
				posName.X += (float)(tex.Width / 2) - vector.X / 2f;
			}
			posName.Y -= font.MeasureString("AA").Y / 2f;
			break;
		case RelativePos.NameLeftOfTexture:
			posName = pos;
			posTex = pos;
			posTex.X += vector.X;
			posName.Y += (float)(tex.Height / 2) - vector.Y / 2f;
			break;
		case RelativePos.NameRightOfTexture:
			posTex = pos;
			posName = pos;
			posName.X += tex.Width;
			posName.Y += (float)(tex.Height / 2) - vector.Y / 2f;
			break;
		default:
			throw new Exception("relative pos not supported : " + relativePos);
		}
	}

	public void Draw(SpriteBatch sb, SpriteFont font, Vector2 pos, Color colorString)
	{
		Vector2 posTex = default(Vector2);
		Vector2 posName = default(Vector2);
		ComputeDrawInfo(font, pos, isLocked: false, out posTex, out posName);
		sb.Draw(tex, posTex, new Color(Color.White.R, Color.White.G, Color.White.B, colorString.A));
		sb.DrawString(font, name, posName, colorString);
	}

	public void DrawWithSelectEffect(Drawing2D draw2D, Vector2 pos, Color colorString, Color secondColor, float scaleMin, float scaleMax, float selectionTransition, Color? customTexColor, string customString, bool isLocked)
	{
		Vector2 posTex = default(Vector2);
		Vector2 posName = default(Vector2);
		ComputeDrawInfo(draw2D.Font, pos, isLocked, out posTex, out posName);
		float num = 0f;
		if (customTexColor.HasValue)
		{
			num = (float)(int)customTexColor.Value.A / 255f;
		}
		Color color = ((!customTexColor.HasValue) ? new Color(Color.White.R, Color.White.G, Color.White.B, colorString.A) : new Color(customTexColor.Value.R, customTexColor.Value.G, customTexColor.Value.B, (byte)((float)(int)colorString.A * num)));
		Vector2 vector = new Vector2(tex.Width, tex.Height) / 2f;
		draw2D.SpriteBatch.Draw(tex, posTex + vector, null, color, 0f, vector, MathHelper.Lerp(scaleMin, scaleMax, selectionTransition), SpriteEffects.None, 0f);
		if (MaximinusGame.Id == MaximinusGame.ID.MissileEscape && isLocked && name == "USA 1")
		{
			posName.X -= 20f;
		}
		draw2D.DrawStringWithSelectEffect(isLocked ? "Locked" : ((customString == "") ? name : customString), posName, colorString, secondColor, scaleMax, selectionTransition);
	}
}
