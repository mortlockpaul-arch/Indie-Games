using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace System.Net;

internal static class IPv4AddressHelper
{
	internal unsafe static string ParseCanonicalName(string str, int start, int end, ref bool isLoopback)
	{
		int end2 = end;
		long num;
		fixed (char* name = str)
		{
			num = ParseNonCanonical(name, start, ref end2, notImplicitFile: true);
		}
		global::_003C_003Ey__InlineArray4<byte> buffer = default(global::_003C_003Ey__InlineArray4<byte>);
		buffer[0] = (byte)(num >> 24);
		buffer[1] = (byte)(num >> 16);
		buffer[2] = (byte)(num >> 8);
		buffer[3] = (byte)num;
		Span<byte> span = buffer;
		isLoopback = span[0] == 127;
		Span<char> span2 = stackalloc char[15];
		int num2 = 0;
		int charsWritten;
		for (int i = 0; i < 3; i++)
		{
			span[i].TryFormat(span2.Slice(num2), out charsWritten);
			int num3 = num2 + charsWritten;
			span2[num3] = '.';
			num2 = num3 + 1;
		}
		span[3].TryFormat(span2.Slice(num2), out charsWritten);
		return new string(span2.Slice(0, num2 + charsWritten));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ushort ToUShort<TChar>(TChar value) where TChar : unmanaged, IBinaryInteger<TChar>
	{
		if (!(typeof(TChar) == typeof(char)))
		{
			return (byte)(object)value;
		}
		return (char)(object)value;
	}

	internal static int ParseHostNumber<TChar>(ReadOnlySpan<TChar> str, int start, int end) where TChar : unmanaged, IBinaryInteger<TChar>
	{
		Span<byte> span = stackalloc byte[4];
		for (int i = 0; i < span.Length; i++)
		{
			int num = 0;
			int num2;
			while (start < end && (num2 = ToUShort(str[start])) != 46 && num2 != 58)
			{
				num = num * 10 + num2 - 48;
				start++;
			}
			span[i] = (byte)num;
			start++;
		}
		return BinaryPrimitives.ReadInt32BigEndian(span);
	}

	internal unsafe static bool IsValid<TChar>(TChar* name, int start, ref int end, bool allowIPv6, bool notImplicitFile, bool unknownScheme) where TChar : unmanaged, IBinaryInteger<TChar>
	{
		if (allowIPv6 | unknownScheme)
		{
			return IsValidCanonical(name, start, ref end, allowIPv6, notImplicitFile);
		}
		return ParseNonCanonical(name, start, ref end, notImplicitFile) != -1;
	}

	internal unsafe static bool IsValidCanonical<TChar>(TChar* name, int start, ref int end, bool allowIPv6, bool notImplicitFile) where TChar : unmanaged, IBinaryInteger<TChar>
	{
		int num = 0;
		long num2 = 0L;
		bool flag = false;
		bool flag2 = false;
		while (start < end)
		{
			int num3 = ToUShort(name[start]);
			if (allowIPv6)
			{
				if (num3 == 93 || num3 == 47 || num3 == 37)
				{
					break;
				}
			}
			else if (num3 == 47 || num3 == 92 || (notImplicitFile && (num3 == 58 || num3 == 63 || num3 == 35)))
			{
				break;
			}
			uint num4 = (uint)(num3 - 48);
			if (num4 < 10)
			{
				if (!flag && num4 == 0)
				{
					if (start + 1 < end && name[start + 1] == TChar.CreateTruncating('0'))
					{
						return false;
					}
					flag2 = true;
				}
				flag = true;
				num2 = num2 * 10 + num4;
				if (num2 > 255)
				{
					return false;
				}
			}
			else
			{
				if (num3 != 46)
				{
					return false;
				}
				if (!flag || ((num2 > 0) & flag2))
				{
					return false;
				}
				num++;
				flag = false;
				num2 = 0L;
				flag2 = false;
			}
			start++;
		}
		bool num5 = (num == 3) & flag;
		if (num5)
		{
			end = start;
		}
		return num5;
	}

	internal unsafe static long ParseNonCanonical<TChar>(TChar* name, int start, ref int end, bool notImplicitFile) where TChar : unmanaged, IBinaryInteger<TChar>
	{
		int num = 10;
		int num2 = 0;
		Span<long> span = stackalloc long[3];
		long num3 = 0L;
		bool flag = false;
		int num4 = 0;
		int i;
		for (i = start; i < end; i++)
		{
			num2 = ToUShort(name[i]);
			num3 = 0L;
			num = 10;
			if (num2 == 48)
			{
				i++;
				flag = true;
				if (i < end)
				{
					num2 = ToUShort(name[i]);
					if (num2 == 120 || num2 == 88)
					{
						num = 16;
						i++;
						flag = false;
					}
					else
					{
						num = 8;
					}
				}
			}
			for (; i < end; i++)
			{
				num2 = ToUShort(name[i]);
				int num5 = System.HexConverter.FromChar(num2);
				if (num5 >= num)
				{
					break;
				}
				num3 = num3 * num + num5;
				if (num3 > uint.MaxValue)
				{
					return -1L;
				}
				flag = true;
			}
			if (i >= end || num2 != 46)
			{
				break;
			}
			if (num4 >= 3 || !flag || num3 > 255)
			{
				return -1L;
			}
			span[num4] = num3;
			num4++;
			flag = false;
		}
		if (!flag)
		{
			return -1L;
		}
		if (i < end)
		{
			if (num2 != 47 && num2 != 92 && (!notImplicitFile || (num2 != 58 && num2 != 63 && num2 != 35)))
			{
				return -1L;
			}
			end = i;
		}
		switch (num4)
		{
		case 0:
			return num3;
		case 1:
			if (num3 > 16777215)
			{
				return -1L;
			}
			return (span[0] << 24) | num3;
		case 2:
			if (num3 > 65535)
			{
				return -1L;
			}
			return (span[0] << 24) | (span[1] << 16) | num3;
		case 3:
			if (num3 > 255)
			{
				return -1L;
			}
			return (span[0] << 24) | (span[1] << 16) | (span[2] << 8) | num3;
		default:
			return -1L;
		}
	}
}
