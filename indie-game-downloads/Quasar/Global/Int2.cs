using System;
using Microsoft.Xna.Framework;

namespace Quasar.Global;

public struct Int2 : IEquatable<Int2>
{
	public int X;

	public int Y;

	public int SumAbsCoords => Math.Abs(X) + Math.Abs(Y);

	public int MaxAbsCoord => Math.Max(Math.Abs(X), Math.Abs(Y));

	public static Int2 Zero => new Int2(0);

	public static Int2 One => new Int2(1);

	public static Int2 Down => new Int2(0, -1);

	public static Int2 Up => new Int2(0, 1);

	public static Int2 Left => new Int2(-1, 0);

	public static Int2 Right => new Int2(1, 0);

	public static Int2 UnitX => new Int2(1, 0);

	public static Int2 UnitY => new Int2(0, 1);

	public Int2(int value)
	{
		X = (Y = value);
	}

	public Int2(int x, int y)
	{
		X = x;
		Y = y;
	}

	public static Int2 RotateCW(Int2 value)
	{
		return new Int2(value.Y, -value.X);
	}

	public static Int2 RotateCCW(Int2 value)
	{
		return new Int2(-value.Y, value.X);
	}

	public float Length()
	{
		return (float)Math.Sqrt(X * X + Y * Y);
	}

	public static Vector2 operator *(Int2 v1, float value)
	{
		return new Vector2((float)v1.X * value, (float)v1.Y * value);
	}

	public static Int2 operator *(Int2 v1, int value)
	{
		return new Int2(v1.X * value, v1.Y * value);
	}

	public static Int2 operator /(Int2 v1, int value)
	{
		return new Int2(v1.X / value, v1.Y / value);
	}

	public static Vector2 operator /(Int2 v1, float value)
	{
		return new Vector2((float)v1.X / value, (float)v1.Y / value);
	}

	public static Int2 operator -(Int2 value)
	{
		return new Int2(-value.X, -value.Y);
	}

	public static Int2 operator -(Int2 v1, Int2 v2)
	{
		return new Int2(v1.X - v2.X, v1.Y - v2.Y);
	}

	public static Int2 operator +(Int2 v1, Int2 v2)
	{
		return new Int2(v1.X + v2.X, v1.Y + v2.Y);
	}

	public static Int2 operator *(Int2 v1, Int2 v2)
	{
		return new Int2(v1.X * v2.X, v1.Y * v2.Y);
	}

	public static bool operator ==(Int2 v1, Int2 v2)
	{
		if (v1.X == v2.X)
		{
			return v1.Y == v2.Y;
		}
		return false;
	}

	public static bool operator !=(Int2 v1, Int2 v2)
	{
		if (v1.X == v2.X)
		{
			return v1.Y != v2.Y;
		}
		return true;
	}

	public override bool Equals(object obj)
	{
		if ((object)obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((Int2)obj);
	}

	public override int GetHashCode()
	{
		return (X + Y).GetHashCode();
	}

	public bool Equals(Int2 other)
	{
		if (other.X == X)
		{
			return other.Y == Y;
		}
		return false;
	}

	public static Int2 Clamp(Int2 min, Int2 max, Int2 value)
	{
		return new Int2((value.X < min.X) ? min.X : ((value.X > max.X) ? max.X : value.X), (value.Y < min.Y) ? min.Y : ((value.Y > max.Y) ? max.Y : value.Y));
	}

	public static Int2 Max(Int2 v1, Int2 v2)
	{
		return new Int2((v1.X > v2.X) ? v1.X : v2.X, (v1.Y > v2.Y) ? v1.Y : v2.Y);
	}

	public static Int2 Min(Int2 v1, Int2 v2)
	{
		return new Int2((v1.X < v2.X) ? v1.X : v2.X, (v1.Y < v2.Y) ? v1.Y : v2.Y);
	}

	public override string ToString()
	{
		return "X: " + X + ", Y: " + Y;
	}

	public static implicit operator Vector2(Int2 value)
	{
		return new Vector2(value.X, value.Y);
	}
}
