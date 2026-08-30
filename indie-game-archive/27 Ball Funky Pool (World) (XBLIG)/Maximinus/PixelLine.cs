using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class PixelLine
{
	private List<Point> data = new List<Point>();

	public Color color;

	public List<Point> Points => data;

	public PixelLine(Point p0, Point p1)
		: this(p0, p1, Color.White)
	{
	}

	public PixelLine(Point p0, Point p1, Color c)
	{
		Reset(p0, p1);
		color = c;
	}

	public void Reset(Point p0, Point p1)
	{
		int num = p0.X;
		int x = p1.X;
		int num2 = p0.Y;
		int y = p1.Y;
		int num3 = Math.Abs(x - num);
		int num4 = Math.Abs(y - num2);
		int num5 = ((num < x) ? 1 : (-1));
		int num6 = ((num2 < y) ? 1 : (-1));
		int num7 = num3 - num4;
		data.Clear();
		while (true)
		{
			data.Add(new Point(num, num2));
			if (num == x && num2 == y)
			{
				break;
			}
			int num8 = num7 * 2;
			if (num8 > -num4)
			{
				num7 -= num4;
				num += num5;
			}
			if (num8 < num3)
			{
				num7 += num3;
				num2 += num6;
			}
		}
	}

	public void Draw(Drawing2D draw2D, float depth)
	{
		Draw(draw2D, depth, color);
	}

	public void Draw(Drawing2D draw2D)
	{
		Draw(draw2D, 0f);
	}

	public void Draw(Drawing2D draw2D, float depth, Color color)
	{
		foreach (Point datum in data)
		{
			Vector2 position = new Vector2(datum.X, datum.Y);
			draw2D.SpriteBatch.Draw(draw2D.BlankTex, position, null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth);
		}
	}
}
