using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace System.Net;

public class IPAddress : ISpanFormattable, IFormattable, ISpanParsable<IPAddress>, IParsable<IPAddress>, IUtf8SpanFormattable, IUtf8SpanParsable<IPAddress>
{
	private sealed class ReadOnlyIPAddress : IPAddress
	{
		public ReadOnlyIPAddress(ReadOnlySpan<byte> newAddress)
			: base(newAddress)
		{
		}

		public ReadOnlyIPAddress(ReadOnlySpan<byte> address, long scopeid)
			: base(address, scopeid)
		{
		}
	}

	public static readonly IPAddress Any = new ReadOnlyIPAddress(new byte[4] { 0, 0, 0, 0 });

	public static readonly IPAddress Loopback = new ReadOnlyIPAddress(new byte[4] { 127, 0, 0, 1 });

	public static readonly IPAddress Broadcast = new ReadOnlyIPAddress(new byte[4] { 255, 255, 255, 255 });

	public static readonly IPAddress None = Broadcast;

	public static readonly IPAddress IPv6Any = new ReadOnlyIPAddress(new byte[16]
	{
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0
	}, 0L);

	public static readonly IPAddress IPv6Loopback = new ReadOnlyIPAddress(new byte[16]
	{
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 1
	}, 0L);

	public static readonly IPAddress IPv6None = IPv6Any;

	private static readonly IPAddress s_loopbackMappedToIPv6 = new ReadOnlyIPAddress(new byte[16]
	{
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		255, 255, 127, 0, 0, 1
	}, 0L);

	private uint _addressOrScopeId;

	private readonly ushort[] _numbers;

	private string _toString;

	private int _hashCode;

	[MemberNotNullWhen(false, "_numbers")]
	private bool IsIPv4
	{
		[MemberNotNullWhen(false, "_numbers")]
		get
		{
			return _numbers == null;
		}
	}

	[MemberNotNullWhen(true, "_numbers")]
	private bool IsIPv6
	{
		[MemberNotNullWhen(true, "_numbers")]
		get
		{
			return _numbers != null;
		}
	}

	internal uint PrivateAddress
	{
		get
		{
			return _addressOrScopeId;
		}
		private set
		{
			_toString = null;
			_hashCode = 0;
			_addressOrScopeId = value;
		}
	}

	internal uint PrivateIPv4Address
	{
		get
		{
			if (IsIPv4)
			{
				return _addressOrScopeId;
			}
			return (uint)HostToNetworkOrder((_numbers[6] << 16) | _numbers[7]);
		}
	}

	private uint PrivateScopeId
	{
		get
		{
			return _addressOrScopeId;
		}
		set
		{
			_toString = null;
			_hashCode = 0;
			_addressOrScopeId = value;
		}
	}

	public AddressFamily AddressFamily
	{
		get
		{
			if (!IsIPv4)
			{
				return AddressFamily.InterNetworkV6;
			}
			return AddressFamily.InterNetwork;
		}
	}

	public long ScopeId
	{
		get
		{
			if (IsIPv4)
			{
				ThrowSocketOperationNotSupported();
			}
			return PrivateScopeId;
		}
		set
		{
			if (IsIPv4)
			{
				ThrowSocketOperationNotSupported();
			}
			if (this is ReadOnlyIPAddress)
			{
				ThrowSocketOperationNotSupported();
			}
			ArgumentOutOfRangeException.ThrowIfNegative(value, "value");
			ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 4294967295L, "value");
			PrivateScopeId = (uint)value;
		}
	}

	public bool IsIPv6Multicast
	{
		get
		{
			if (IsIPv6)
			{
				return (_numbers[0] & 0xFF00) == 65280;
			}
			return false;
		}
	}

	public bool IsIPv6LinkLocal
	{
		get
		{
			if (IsIPv6)
			{
				return (_numbers[0] & 0xFFC0) == 65152;
			}
			return false;
		}
	}

	public bool IsIPv6SiteLocal
	{
		get
		{
			if (IsIPv6)
			{
				return (_numbers[0] & 0xFFC0) == 65216;
			}
			return false;
		}
	}

	public bool IsIPv6Teredo
	{
		get
		{
			if (IsIPv6 && _numbers[0] == 8193)
			{
				return _numbers[1] == 0;
			}
			return false;
		}
	}

	public bool IsIPv6UniqueLocal
	{
		get
		{
			if (IsIPv6)
			{
				return (_numbers[0] & 0xFE00) == 64512;
			}
			return false;
		}
	}

	public bool IsIPv4MappedToIPv6
	{
		get
		{
			if (!IsIPv4)
			{
				return ((ReadOnlySpan<ushort>)_numbers.AsSpan(0, 6)).SequenceEqual((ReadOnlySpan<ushort>)new ushort[6] { 0, 0, 0, 0, 0, 65535 });
			}
			return false;
		}
	}

	[Obsolete("IPAddress.Address is address family dependent and has been deprecated. Use IPAddress.Equals to perform comparisons instead.")]
	public long Address
	{
		get
		{
			if (AddressFamily == AddressFamily.InterNetworkV6)
			{
				ThrowSocketOperationNotSupported();
			}
			return PrivateAddress;
		}
		set
		{
			if (AddressFamily == AddressFamily.InterNetworkV6)
			{
				ThrowSocketOperationNotSupported();
			}
			if (PrivateAddress != value)
			{
				if (this is ReadOnlyIPAddress)
				{
					ThrowSocketOperationNotSupported();
				}
				PrivateAddress = (uint)value;
			}
		}
	}

	public IPAddress(long newAddress)
	{
		ArgumentOutOfRangeException.ThrowIfGreaterThan((ulong)newAddress, 4294967295uL, "newAddress");
		PrivateAddress = (uint)newAddress;
	}

	public IPAddress(byte[] address, long scopeid)
		: this(new ReadOnlySpan<byte>(address ?? ThrowAddressNullException()), scopeid)
	{
	}

	public IPAddress(ReadOnlySpan<byte> address, long scopeid)
	{
		if (address.Length != 16)
		{
			throw new ArgumentException(System.SR.dns_bad_ip_address, "address");
		}
		ArgumentOutOfRangeException.ThrowIfGreaterThan((ulong)scopeid, 4294967295uL, "scopeid");
		_numbers = ReadUInt16NumbersFromBytes(address);
		PrivateScopeId = (uint)scopeid;
	}

	internal IPAddress(ReadOnlySpan<ushort> numbers, uint scopeid)
	{
		_numbers = numbers.ToArray();
		PrivateScopeId = scopeid;
	}

	private IPAddress(ushort[] numbers, uint scopeid)
	{
		_numbers = numbers;
		PrivateScopeId = scopeid;
	}

	public IPAddress(byte[] address)
		: this(new ReadOnlySpan<byte>(address ?? ThrowAddressNullException()))
	{
	}

	public IPAddress(ReadOnlySpan<byte> address)
	{
		if (address.Length == 4)
		{
			PrivateAddress = MemoryMarshal.Read<uint>(address);
			return;
		}
		if (address.Length == 16)
		{
			_numbers = ReadUInt16NumbersFromBytes(address);
			return;
		}
		throw new ArgumentException(System.SR.dns_bad_ip_address, "address");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ushort[] ReadUInt16NumbersFromBytes(ReadOnlySpan<byte> address)
	{
		ushort[] array = new ushort[8];
		if (Vector128.IsHardwareAccelerated && BitConverter.IsLittleEndian)
		{
			Vector128<ushort> vector = Vector128.Create(address).AsUInt16();
			vector = (vector << 8) | (vector >> 8);
			vector.CopyTo(array);
		}
		else
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = BinaryPrimitives.ReadUInt16BigEndian(address.Slice(i * 2));
			}
		}
		return array;
	}

	public static bool IsValid(ReadOnlySpan<char> ipSpan)
	{
		return IPAddressParser.IsValid(ipSpan);
	}

	public static bool IsValidUtf8(ReadOnlySpan<byte> utf8Text)
	{
		return IPAddressParser.IsValid(utf8Text);
	}

	public static bool TryParse([NotNullWhen(true)] string? ipString, [NotNullWhen(true)] out IPAddress? address)
	{
		if (ipString == null)
		{
			address = null;
			return false;
		}
		address = IPAddressParser.Parse(ipString.AsSpan(), tryParse: true);
		return address != null;
	}

	public static bool TryParse(ReadOnlySpan<byte> utf8Text, [NotNullWhen(true)] out IPAddress? result)
	{
		result = IPAddressParser.Parse(utf8Text, tryParse: true);
		return result != null;
	}

	public static bool TryParse(ReadOnlySpan<char> ipSpan, [NotNullWhen(true)] out IPAddress? address)
	{
		address = IPAddressParser.Parse(ipSpan, tryParse: true);
		return address != null;
	}

	static bool IUtf8SpanParsable<IPAddress>.TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider provider, [NotNullWhen(true)] out IPAddress result)
	{
		return TryParse(utf8Text, out result);
	}

	static bool IParsable<IPAddress>.TryParse([NotNullWhen(true)] string s, IFormatProvider provider, [NotNullWhen(true)] out IPAddress result)
	{
		return TryParse(s, out result);
	}

	static bool ISpanParsable<IPAddress>.TryParse(ReadOnlySpan<char> s, IFormatProvider provider, [NotNullWhen(true)] out IPAddress result)
	{
		return TryParse(s, out result);
	}

	public static IPAddress Parse(string ipString)
	{
		ArgumentNullException.ThrowIfNull(ipString, "ipString");
		return IPAddressParser.Parse(ipString.AsSpan(), tryParse: false);
	}

	public static IPAddress Parse(ReadOnlySpan<byte> utf8Text)
	{
		return IPAddressParser.Parse(utf8Text, tryParse: false);
	}

	public static IPAddress Parse(ReadOnlySpan<char> ipSpan)
	{
		return IPAddressParser.Parse(ipSpan, tryParse: false);
	}

	static IPAddress IUtf8SpanParsable<IPAddress>.Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider provider)
	{
		return Parse(utf8Text);
	}

	static IPAddress ISpanParsable<IPAddress>.Parse(ReadOnlySpan<char> s, IFormatProvider provider)
	{
		return Parse(s);
	}

	static IPAddress IParsable<IPAddress>.Parse(string s, IFormatProvider provider)
	{
		return Parse(s);
	}

	public bool TryWriteBytes(Span<byte> destination, out int bytesWritten)
	{
		if (IsIPv6)
		{
			if (destination.Length < 16)
			{
				bytesWritten = 0;
				return false;
			}
			WriteIPv6Bytes(destination);
			bytesWritten = 16;
		}
		else
		{
			if (destination.Length < 4)
			{
				bytesWritten = 0;
				return false;
			}
			WriteIPv4Bytes(destination);
			bytesWritten = 4;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void WriteIPv6Bytes(Span<byte> destination)
	{
		ushort[] numbers = _numbers;
		if (BitConverter.IsLittleEndian)
		{
			if (Vector128.IsHardwareAccelerated)
			{
				Vector128<ushort> vector = Vector128.Create(numbers).AsUInt16();
				vector = (vector << 8) | (vector >> 8);
				vector.AsByte().CopyTo(destination);
			}
			else
			{
				for (int i = 0; i < numbers.Length; i++)
				{
					BinaryPrimitives.WriteUInt16BigEndian(destination.Slice(i * 2), numbers[i]);
				}
			}
		}
		else
		{
			MemoryMarshal.AsBytes(numbers).CopyTo(destination);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void WriteIPv4Bytes(Span<byte> destination)
	{
		MemoryMarshal.Write<uint>(destination, PrivateAddress);
	}

	public byte[] GetAddressBytes()
	{
		if (IsIPv6)
		{
			byte[] array = new byte[16];
			WriteIPv6Bytes(array);
			return array;
		}
		byte[] array2 = new byte[4];
		WriteIPv4Bytes(array2);
		return array2;
	}

	public override string ToString()
	{
		string text = _toString;
		if (text == null)
		{
			Span<char> span = stackalloc char[65];
			text = (_toString = new string(span[..(IsIPv4 ? IPAddressParser.FormatIPv4Address(_addressOrScopeId, span) : IPAddressParser.FormatIPv6Address(_numbers, _addressOrScopeId, span))]));
		}
		return text;
	}

	string IFormattable.ToString(string format, IFormatProvider formatProvider)
	{
		return ToString();
	}

	public bool TryFormat(Span<char> destination, out int charsWritten)
	{
		return TryFormatCore(destination, out charsWritten);
	}

	public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten)
	{
		return TryFormatCore(utf8Destination, out bytesWritten);
	}

	bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider provider)
	{
		return TryFormatCore(destination, out charsWritten);
	}

	bool IUtf8SpanFormattable.TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider provider)
	{
		return TryFormatCore(utf8Destination, out bytesWritten);
	}

	private bool TryFormatCore<TChar>(Span<TChar> destination, out int charsWritten) where TChar : unmanaged, IBinaryInteger<TChar>
	{
		if (IsIPv4)
		{
			if (destination.Length >= 15)
			{
				charsWritten = IPAddressParser.FormatIPv4Address(_addressOrScopeId, destination);
				return true;
			}
		}
		else if (destination.Length >= 65)
		{
			charsWritten = IPAddressParser.FormatIPv6Address(_numbers, _addressOrScopeId, destination);
			return true;
		}
		Span<TChar> span = stackalloc TChar[65];
		int num = (IsIPv4 ? IPAddressParser.FormatIPv4Address(PrivateAddress, span) : IPAddressParser.FormatIPv6Address(_numbers, PrivateScopeId, span));
		if (span.Slice(0, num).TryCopyTo(destination))
		{
			charsWritten = num;
			return true;
		}
		charsWritten = 0;
		return false;
	}

	public static long HostToNetworkOrder(long host)
	{
		if (!BitConverter.IsLittleEndian)
		{
			return host;
		}
		return BinaryPrimitives.ReverseEndianness(host);
	}

	public static int HostToNetworkOrder(int host)
	{
		if (!BitConverter.IsLittleEndian)
		{
			return host;
		}
		return BinaryPrimitives.ReverseEndianness(host);
	}

	public static short HostToNetworkOrder(short host)
	{
		if (!BitConverter.IsLittleEndian)
		{
			return host;
		}
		return BinaryPrimitives.ReverseEndianness(host);
	}

	public static long NetworkToHostOrder(long network)
	{
		return HostToNetworkOrder(network);
	}

	public static int NetworkToHostOrder(int network)
	{
		return HostToNetworkOrder(network);
	}

	public static short NetworkToHostOrder(short network)
	{
		return HostToNetworkOrder(network);
	}

	public static bool IsLoopback(IPAddress address)
	{
		ArgumentNullException.ThrowIfNull(address, "address");
		if (address.IsIPv6)
		{
			if (!address.Equals(IPv6Loopback))
			{
				return address.Equals(s_loopbackMappedToIPv6);
			}
			return true;
		}
		long num = (uint)HostToNetworkOrder(-16777216);
		return (address.PrivateAddress & num) == (Loopback.PrivateAddress & num);
	}

	public override bool Equals([NotNullWhen(true)] object? comparand)
	{
		if (comparand is IPAddress comparand2)
		{
			return Equals(comparand2);
		}
		return false;
	}

	internal bool Equals(IPAddress comparand)
	{
		if (AddressFamily != comparand.AddressFamily)
		{
			return false;
		}
		if (IsIPv6)
		{
			ReadOnlySpan<ushort> span = _numbers.AsSpan(0, 8);
			ReadOnlySpan<ushort> other = comparand._numbers.AsSpan(0, 8);
			if (span.SequenceEqual(other))
			{
				return PrivateScopeId == comparand.PrivateScopeId;
			}
			return false;
		}
		return comparand.PrivateAddress == PrivateAddress;
	}

	public override int GetHashCode()
	{
		if (_hashCode == 0)
		{
			if (IsIPv6)
			{
				ReadOnlySpan<byte> source = MemoryMarshal.AsBytes(_numbers);
				_hashCode = HashCode.Combine(MemoryMarshal.Read<uint>(source), MemoryMarshal.Read<uint>(source.Slice(4)), MemoryMarshal.Read<uint>(source.Slice(8)), MemoryMarshal.Read<uint>(source.Slice(12)), _addressOrScopeId);
			}
			else
			{
				_hashCode = HashCode.Combine(_addressOrScopeId);
			}
		}
		return _hashCode;
	}

	public IPAddress MapToIPv6()
	{
		if (IsIPv6)
		{
			return this;
		}
		uint num = (uint)NetworkToHostOrder((int)PrivateAddress);
		return new IPAddress(new ushort[8]
		{
			0,
			0,
			0,
			0,
			0,
			65535,
			(ushort)(num >> 16),
			(ushort)num
		}, 0u);
	}

	public IPAddress MapToIPv4()
	{
		if (IsIPv4)
		{
			return this;
		}
		return new IPAddress((uint)HostToNetworkOrder((_numbers[6] << 16) | _numbers[7]));
	}

	[DoesNotReturn]
	private static byte[] ThrowAddressNullException()
	{
		throw new ArgumentNullException("address");
	}

	[DoesNotReturn]
	private static void ThrowSocketOperationNotSupported()
	{
		throw new SocketException(SocketError.OperationNotSupported);
	}
}
