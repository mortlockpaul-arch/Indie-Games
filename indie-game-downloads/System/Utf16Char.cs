namespace System;

internal readonly struct Utf16Char(char ch) : System.IUtfChar<Utf16Char>, IEquatable<Utf16Char>
{
	private readonly char value = ch;

	public static Utf16Char CastFrom(byte value)
	{
		return new Utf16Char((char)value);
	}

	public static Utf16Char CastFrom(char value)
	{
		return new Utf16Char(value);
	}

	public static Utf16Char CastFrom(int value)
	{
		return new Utf16Char((char)value);
	}

	public static Utf16Char CastFrom(uint value)
	{
		return new Utf16Char((char)value);
	}

	public static Utf16Char CastFrom(ulong value)
	{
		return new Utf16Char((char)value);
	}

	public static uint CastToUInt32(Utf16Char value)
	{
		return value.value;
	}

	public bool Equals(Utf16Char other)
	{
		return value == other.value;
	}
}
