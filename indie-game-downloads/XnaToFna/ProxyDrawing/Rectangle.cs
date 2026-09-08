using System;

namespace XnaToFna.ProxyDrawing;

[Serializable]
public struct Rectangle
{
	public static readonly Rectangle Empty;

	private int x;

	private int y;

	private int width;

	private int height;

	public bool IsEmpty => this == Empty;

	public int X
	{
		get
		{
			return x;
		}
		set
		{
			x = value;
		}
	}

	public int Y
	{
		get
		{
			return y;
		}
		set
		{
			y = value;
		}
	}

	public int Width
	{
		get
		{
			return width;
		}
		set
		{
			width = value;
		}
	}

	public int Height
	{
		get
		{
			return height;
		}
		set
		{
			height = value;
		}
	}

	public int Left => X;

	public int Top => y;

	public int Right => X + Width;

	public int Bottom => y + height;

	public Rectangle(int x, int y, int width, int height)
	{
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
	}

	public void Inflate(int width, int height)
	{
		x -= width;
		y -= height;
		this.width += width * 2;
		this.height += height * 2;
	}

	public void Intersect(Rectangle rect)
	{
		if (!IntersectsWithInclusive(rect))
		{
			x = (y = (width = (height = 0)));
			return;
		}
		x = Math.Max(Left, rect.Left);
		y = Math.Max(Top, rect.Top);
		width = Math.Min(Right, rect.Right) - x;
		height = Math.Min(Bottom, rect.Bottom) - y;
	}

	public bool Contains(Point p)
	{
		return Contains(p.X, p.Y);
	}

	public bool Contains(int x, int y)
	{
		if (x >= Left && x < Right && y >= Top)
		{
			return y < Bottom;
		}
		return false;
	}

	public bool Contains(Rectangle rect)
	{
		return rect == Intersect(this, rect);
	}

	public bool IntersectsWith(Rectangle rect)
	{
		if (Left < rect.Right && Right > rect.Left && Top < rect.Bottom)
		{
			return Bottom > rect.Top;
		}
		return false;
	}

	private bool IntersectsWithInclusive(Rectangle rect)
	{
		if (Left <= rect.Right && Right >= rect.Left && Top <= rect.Bottom)
		{
			return Bottom >= rect.Top;
		}
		return false;
	}

	public void Offset(Point p)
	{
		Offset(p.X, p.Y);
	}

	public void Offset(int x, int y)
	{
		this.x += x;
		this.y += y;
	}

	public static Rectangle FromLTRB(int left, int top, int right, int bottom)
	{
		return new Rectangle(left, top, right - left, bottom - top);
	}

	public static Rectangle Inflate(Rectangle rect, int x, int y)
	{
		Rectangle result = new Rectangle(rect.x, rect.y, rect.width, rect.height);
		result.Inflate(x, y);
		return result;
	}

	public static Rectangle Intersect(Rectangle a, Rectangle b)
	{
		a = new Rectangle(a.x, a.y, a.width, a.height);
		a.Intersect(b);
		return a;
	}

	public static Rectangle Union(Rectangle a, Rectangle b)
	{
		return FromLTRB(Math.Min(a.Left, b.Left), Math.Min(a.Top, b.Top), Math.Max(a.Right, b.Right), Math.Max(a.Bottom, b.Bottom));
	}

	public static bool operator !=(Rectangle left, Rectangle right)
	{
		return !(left == right);
	}

	public static bool operator ==(Rectangle left, Rectangle right)
	{
		if (left.x == right.x && left.y == right.y && left.width == right.width)
		{
			return left.height == right.height;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is Rectangle)
		{
			return this == (Rectangle)obj;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (height + width) ^ (x + y);
	}

	public override string ToString()
	{
		return $"{{X={x},Y={y},Width={width},Height={height}}}";
	}
}
