namespace System.Net.Sockets;

internal static class IPEndPointExtensions
{
	public static IPAddress GetIPAddress(ReadOnlySpan<byte> socketAddressBuffer)
	{
		switch (System.Net.SocketAddressPal.GetAddressFamily(socketAddressBuffer))
		{
		case AddressFamily.InterNetworkV6:
		{
			Span<byte> span = stackalloc byte[16];
			System.Net.SocketAddressPal.GetIPv6Address(socketAddressBuffer, span, out var scope);
			return new IPAddress(span, (span[0] == 254 && (span[1] & 0xC0) == 128) ? scope : 0);
		}
		case AddressFamily.InterNetwork:
			return new IPAddress((long)System.Net.SocketAddressPal.GetIPv4Address(socketAddressBuffer) & 0xFFFFFFFFL);
		default:
			throw new SocketException(10047);
		}
	}

	public static void SetIPAddress(Span<byte> socketAddressBuffer, IPAddress address)
	{
		System.Net.SocketAddressPal.SetAddressFamily(socketAddressBuffer, address.AddressFamily);
		System.Net.SocketAddressPal.SetPort(socketAddressBuffer, 0);
		if (address.AddressFamily == AddressFamily.InterNetwork)
		{
			System.Net.SocketAddressPal.SetIPv4Address(socketAddressBuffer, (uint)address.Address);
			return;
		}
		Span<byte> span = stackalloc byte[16];
		address.TryWriteBytes(span, out var _);
		System.Net.SocketAddressPal.SetIPv6Address(socketAddressBuffer, span, (uint)address.ScopeId);
	}

	public static IPEndPoint CreateIPEndPoint(ReadOnlySpan<byte> socketAddressBuffer)
	{
		return new IPEndPoint(GetIPAddress(socketAddressBuffer), System.Net.SocketAddressPal.GetPort(socketAddressBuffer));
	}

	public static void Serialize(this IPEndPoint endPoint, Span<byte> destination)
	{
		System.Net.SocketAddressPal.SetAddressFamily(destination, endPoint.AddressFamily);
		SetIPAddress(destination, endPoint.Address);
		System.Net.SocketAddressPal.SetPort(destination, (ushort)endPoint.Port);
	}

	public static bool Equals(this IPEndPoint endPoint, ReadOnlySpan<byte> socketAddressBuffer)
	{
		if (socketAddressBuffer.Length >= SocketAddress.GetMaximumAddressSize(endPoint.AddressFamily) && endPoint.AddressFamily == System.Net.SocketAddressPal.GetAddressFamily(socketAddressBuffer) && endPoint.Port == System.Net.SocketAddressPal.GetPort(socketAddressBuffer))
		{
			if (endPoint.AddressFamily == AddressFamily.InterNetwork)
			{
				return endPoint.Address.Address == System.Net.SocketAddressPal.GetIPv4Address(socketAddressBuffer);
			}
			Span<byte> span = stackalloc byte[16];
			Span<byte> span2 = stackalloc byte[16];
			System.Net.SocketAddressPal.GetIPv6Address(socketAddressBuffer, span, out var scope);
			if (endPoint.Address.ScopeId != scope)
			{
				return false;
			}
			endPoint.Address.TryWriteBytes(span2, out var _);
			return ((ReadOnlySpan<byte>)span).SequenceEqual((ReadOnlySpan<byte>)span2);
		}
		return false;
	}
}
