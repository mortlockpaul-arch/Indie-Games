using System.Globalization;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Net;

internal static class IPAddressParser
{
	public unsafe static bool IsValid<TChar>(ReadOnlySpan<TChar> ipSpan) where TChar : unmanaged, IBinaryInteger<TChar>
	{
		fixed (TChar* reference = &MemoryMarshal.GetReference(ipSpan))
		{
			if (ipSpan.Contains(TChar.CreateTruncating(':')))
			{
				return System.Net.IPv6AddressHelper.IsValidStrict(reference, 0, ipSpan.Length);
			}
			int end = ipSpan.Length;
			if (System.Net.IPv4AddressHelper.ParseNonCanonical(reference, 0, ref end, notImplicitFile: true) != -1)
			{
				return end == ipSpan.Length;
			}
			return false;
		}
	}

	internal static IPAddress Parse<TChar>(ReadOnlySpan<TChar> ipSpan, bool tryParse) where TChar : unmanaged, IBinaryInteger<TChar>
	{
		long address;
		if (ipSpan.Contains(TChar.CreateTruncating(':')))
		{
			Span<ushort> span = stackalloc ushort[8];
			span.Clear();
			if (TryParseIPv6(ipSpan, span, 8, out var scope))
			{
				return new IPAddress(span, scope);
			}
		}
		else if (TryParseIpv4(ipSpan, out address))
		{
			return new IPAddress(address);
		}
		if (tryParse)
		{
			return null;
		}
		throw new FormatException(System.SR.dns_bad_ip_address, new SocketException(SocketError.InvalidArgument));
	}

	private unsafe static bool TryParseIpv4<TChar>(ReadOnlySpan<TChar> ipSpan, out long address) where TChar : unmanaged, IBinaryInteger<TChar>
	{
		int end = ipSpan.Length;
		long num;
		fixed (TChar* reference = &MemoryMarshal.GetReference(ipSpan))
		{
			num = System.Net.IPv4AddressHelper.ParseNonCanonical(reference, 0, ref end, notImplicitFile: true);
		}
		if (num != -1 && end == ipSpan.Length)
		{
			address = (uint)IPAddress.HostToNetworkOrder((int)num);
			return true;
		}
		address = 0L;
		return false;
	}

	private unsafe static bool TryParseIPv6<TChar>(ReadOnlySpan<TChar> ipSpan, Span<ushort> numbers, int numbersLength, out uint scope) where TChar : unmanaged, IBinaryInteger<TChar>
	{
		fixed (TChar* reference = &MemoryMarshal.GetReference(ipSpan))
		{
			if (!System.Net.IPv6AddressHelper.IsValidStrict(reference, 0, ipSpan.Length))
			{
				scope = 0u;
				return false;
			}
		}
		System.Net.IPv6AddressHelper.Parse(ipSpan, numbers, out var scopeId);
		if (scopeId.Length > 1)
		{
			scopeId = scopeId.Slice(1);
			if ((!(typeof(TChar) == typeof(byte))) ? uint.TryParse(MemoryMarshal.Cast<TChar, char>(scopeId), NumberStyles.None, CultureInfo.InvariantCulture, out scope) : uint.TryParse(MemoryMarshal.Cast<TChar, byte>(scopeId), NumberStyles.None, CultureInfo.InvariantCulture, out scope))
			{
				return true;
			}
			uint num = InterfaceInfoPal.InterfaceNameToIndex(scopeId);
			if (num != 0)
			{
				scope = num;
				return true;
			}
		}
		scope = 0u;
		return true;
	}

	internal static int FormatIPv4Address<TChar>(uint address, Span<TChar> addressString) where TChar : unmanaged, IBinaryInteger<TChar>
	{
		address = (uint)IPAddress.NetworkToHostOrder((int)address);
		int num = FormatByte(address >> 24, addressString);
		addressString[num++] = TChar.CreateTruncating('.');
		num += FormatByte(address >> 16, addressString.Slice(num));
		addressString[num++] = TChar.CreateTruncating('.');
		num += FormatByte(address >> 8, addressString.Slice(num));
		addressString[num++] = TChar.CreateTruncating('.');
		return num + FormatByte(address, addressString.Slice(num));
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static int FormatByte(uint number, Span<TChar> span)
		{
			number &= 0xFF;
			switch (number)
			{
			default:
			{
				uint left;
				(left, number) = Math.DivRem(number, 10u);
				var (num3, num2) = Math.DivRem(left, 10u);
				span[2] = TChar.CreateTruncating(48 + number);
				span[1] = TChar.CreateTruncating(48 + num2);
				span[0] = TChar.CreateTruncating(48 + num3);
				return 3;
			}
			case 10u:
			case 11u:
			case 12u:
			case 13u:
			case 14u:
			case 15u:
			case 16u:
			case 17u:
			case 18u:
			case 19u:
			case 20u:
			case 21u:
			case 22u:
			case 23u:
			case 24u:
			case 25u:
			case 26u:
			case 27u:
			case 28u:
			case 29u:
			case 30u:
			case 31u:
			case 32u:
			case 33u:
			case 34u:
			case 35u:
			case 36u:
			case 37u:
			case 38u:
			case 39u:
			case 40u:
			case 41u:
			case 42u:
			case 43u:
			case 44u:
			case 45u:
			case 46u:
			case 47u:
			case 48u:
			case 49u:
			case 50u:
			case 51u:
			case 52u:
			case 53u:
			case 54u:
			case 55u:
			case 56u:
			case 57u:
			case 58u:
			case 59u:
			case 60u:
			case 61u:
			case 62u:
			case 63u:
			case 64u:
			case 65u:
			case 66u:
			case 67u:
			case 68u:
			case 69u:
			case 70u:
			case 71u:
			case 72u:
			case 73u:
			case 74u:
			case 75u:
			case 76u:
			case 77u:
			case 78u:
			case 79u:
			case 80u:
			case 81u:
			case 82u:
			case 83u:
			case 84u:
			case 85u:
			case 86u:
			case 87u:
			case 88u:
			case 89u:
			case 90u:
			case 91u:
			case 92u:
			case 93u:
			case 94u:
			case 95u:
			case 96u:
			case 97u:
			case 98u:
			case 99u:
			{
				uint num2;
				(num2, number) = Math.DivRem(number, 10u);
				span[1] = TChar.CreateTruncating(48 + number);
				span[0] = TChar.CreateTruncating(48 + num2);
				return 2;
			}
			case 0u:
			case 1u:
			case 2u:
			case 3u:
			case 4u:
			case 5u:
			case 6u:
			case 7u:
			case 8u:
			case 9u:
				span[0] = TChar.CreateTruncating(48 + number);
				return 1;
			}
		}
	}

	internal static int FormatIPv6Address<TChar>(ushort[] address, uint scopeId, Span<TChar> destination) where TChar : unmanaged, IBinaryInteger<TChar>
	{
		int offset = 0;
		if (System.Net.IPv6AddressHelper.ShouldHaveIpv4Embedded(address))
		{
			AppendSections(address.AsSpan(0, 6), destination, ref offset);
			if (destination[offset - 1] != TChar.CreateTruncating(':'))
			{
				destination[offset++] = TChar.CreateTruncating(':');
			}
			offset += FormatIPv4Address(ExtractIPv4Address(address), destination.Slice(offset));
		}
		else
		{
			AppendSections(address.AsSpan(0, 8), destination, ref offset);
		}
		if (scopeId != 0)
		{
			destination[offset++] = TChar.CreateTruncating('%');
			int bytesWritten;
			if (!(typeof(TChar) == typeof(byte)))
			{
				scopeId.TryFormat(MemoryMarshal.Cast<TChar, char>(destination).Slice(offset), out bytesWritten);
			}
			else
			{
				scopeId.TryFormat(MemoryMarshal.Cast<TChar, byte>(destination).Slice(offset), out bytesWritten);
			}
			offset += bytesWritten;
		}
		return offset;
		static void AppendHex(ushort value, Span<TChar> span, ref int reference)
		{
			if ((value & 0xFFF0) != 0)
			{
				if ((value & 0xFF00) != 0)
				{
					if ((value & 0xF000) != 0)
					{
						span[reference++] = TChar.CreateTruncating(System.HexConverter.ToCharLower(value >> 12));
					}
					span[reference++] = TChar.CreateTruncating(System.HexConverter.ToCharLower(value >> 8));
				}
				span[reference++] = TChar.CreateTruncating(System.HexConverter.ToCharLower(value >> 4));
			}
			span[reference++] = TChar.CreateTruncating(System.HexConverter.ToCharLower(value));
		}
		static void AppendSections(ReadOnlySpan<ushort> numbers, Span<TChar> destination2, ref int reference)
		{
			(int longestSequenceStart, int longestSequenceLength) tuple = System.Net.IPv6AddressHelper.FindCompressionRange(numbers);
			int item = tuple.longestSequenceStart;
			int item2 = tuple.longestSequenceLength;
			bool flag = false;
			if (item >= 0)
			{
				for (int i = 0; i < item; i++)
				{
					if (flag)
					{
						destination2[reference++] = TChar.CreateTruncating(':');
					}
					flag = true;
					AppendHex(numbers[i], destination2, ref reference);
				}
				destination2[reference++] = TChar.CreateTruncating(':');
				destination2[reference++] = TChar.CreateTruncating(':');
				flag = false;
			}
			for (int j = item2; j < numbers.Length; j++)
			{
				if (flag)
				{
					destination2[reference++] = TChar.CreateTruncating(':');
				}
				flag = true;
				AppendHex(numbers[j], destination2, ref reference);
			}
		}
	}

	private static uint ExtractIPv4Address(ushort[] address)
	{
		return (uint)IPAddress.HostToNetworkOrder((address[6] << 16) | address[7]);
	}
}
