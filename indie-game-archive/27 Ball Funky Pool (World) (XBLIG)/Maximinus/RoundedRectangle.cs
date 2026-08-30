using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class RoundedRectangle
{
	private static Texture2D texLines;

	private static Rectangle srcCorner;

	private static Rectangle srcHLine;

	private static Rectangle srcVLine;

	private static int size;

	private static Vector2 CornerOrigin;

	private Rectangle rect;

	private float scale;

	private Color color;

	private Vector2 TopLeft;

	private Vector2 TopRight;

	private Vector2 BottomLeft;

	private Vector2 BottomRight;

	private Rectangle rectTopLine;

	private Rectangle rectBottomLine;

	private Rectangle rectLeftLine;

	private Rectangle rectRightLine;

	public Rectangle Rect
	{
		get
		{
			return rect;
		}
		set
		{
			rect = value;
			Update();
		}
	}

	public int TexWidth
	{
		set
		{
			scale = (float)value / (float)size;
			Update();
		}
	}

	public Color Color
	{
		set
		{
			color = value;
		}
	}

	public static void Initialize(Texture2D texLinesValue)
	{
		texLines = texLinesValue;
		size = texLines.Height;
		srcCorner = new Rectangle(0, 0, size, size);
		srcHLine = new Rectangle(size - 1, 0, size, size);
		srcVLine = new Rectangle(2 * size, 0, size, size);
		CornerOrigin = new Vector2(size / 2, size / 2);
	}

	public RoundedRectangle(Rectangle rect)
	{
		this.rect = rect;
		scale = 1f;
		Update();
	}

	private void Update()
	{
		TopLeft = new Vector2(rect.X, rect.Y);
		TopRight = TopLeft + new Vector2(rect.Width, 0f);
		BottomLeft = TopLeft + new Vector2(0f, rect.Height);
		BottomRight = BottomLeft + new Vector2(rect.Width, 0f);
		int num = (int)((float)size * scale);
		rectTopLine = Rectangle.Empty;
		rectTopLine.Height = num;
		rectTopLine.Width = rect.Width - num;
		rectTopLine.X = rect.X + num / 2;
		rectTopLine.Y = rect.Y - num / 2;
		rectBottomLine = rectTopLine;
		rectBottomLine.Y += rect.Height;
		rectLeftLine = Rectangle.Empty;
		rectLeftLine.Width = num;
		rectLeftLine.Height = rect.Height - num;
		rectLeftLine.X = rect.X - num / 2;
		rectLeftLine.Y = rect.Y + num / 2;
		rectRightLine = rectLeftLine;
		rectRightLine.X += rect.Width;
	}

	public void Draw(SpriteBatch sb)
	{
		Draw(sb, 0f);
	}

	public void Draw(SpriteBatch sb, float rotation)
	{
		if (rect.Width != 0 && rect.Height != 0)
		{
			sb.Draw(texLines, TopLeft, srcCorner, color, 0f + rotation, CornerOrigin, scale, SpriteEffects.None, 0f);
			sb.Draw(texLines, TopRight, srcCorner, color, (float)Math.PI / 2f + rotation, CornerOrigin, scale, SpriteEffects.None, 0f);
			sb.Draw(texLines, BottomRight, srcCorner, color, (float)Math.PI + rotation, CornerOrigin, scale, SpriteEffects.None, 0f);
			sb.Draw(texLines, BottomLeft, srcCorner, color, 4.712389f + rotation, CornerOrigin, scale, SpriteEffects.None, 0f);
			sb.Draw(texLines, rectTopLine, srcHLine, color, 0f + rotation, Vector2.Zero, SpriteEffects.None, 0f);
			sb.Draw(texLines, rectBottomLine, srcHLine, color, 0f + rotation, Vector2.Zero, SpriteEffects.None, 0f);
			sb.Draw(texLines, rectLeftLine, srcVLine, color, 0f + rotation, Vector2.Zero, SpriteEffects.None, 0f);
			sb.Draw(texLines, rectRightLine, srcVLine, color, 0f + rotation, Vector2.Zero, SpriteEffects.None, 0f);
		}
	}
}
