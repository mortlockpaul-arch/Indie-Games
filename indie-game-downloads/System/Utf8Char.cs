namespace System;

internal readonly struct Utf8Char(byte ch) : System.IUtfChar<Utf8Char>, IEquatable<Utf8Char>
{
	private readonly byte value = ch;

	public static Utf8Char CastFrom(byte value)
	{
		return new Utf8Char(value);
	}

	public static Utf8Char CastFrom(char value)
	{
		return new Utf8Char((byte)value);
	}

	public static Utf8Char CastFrom(int value)
	{
		return new Utf8Char((byte)value);
	}

	public static Utf8Char CastFrom(uint value)
	{
		return new Utf8Char((byte)value);
	}

	public static Utf8Char CastFrom(ulong value)
	{
		return new Utf8Char((byte)value);
	}

	public static uint CastToUInt32(Utf8Char value)
	{
		return value.value;
	}

	public bool Equals(Utf8Char other)
	{
		return value == other.value;
	}
}
