using System;

namespace XnaToFna.ProxyDrawing;

[Serializable]
public struct Point
{
	public static readonly Point Empty;

	private int x;

	private int y;

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

	public Point(int dw)
		: this(dw & 0xFFFF, dw >> 16)
	{
	}

	public Point(int x, int y)
	{
		this.x = x;
		this.y = y;
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

	public static bool operator !=(Point left, Point right)
	{
		return !(left == right);
	}

	public static bool operator ==(Point left, Point right)
	{
		if (left.x == right.x)
		{
			return left.y == right.y;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is Point)
		{
			return this == (Point)obj;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return x ^ y;
	}

	public override string ToString()
	{
		return $"{{X={x},Y={y}}}";
	}
}
