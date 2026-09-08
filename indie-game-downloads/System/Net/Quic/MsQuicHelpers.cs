using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Quic;

namespace System.Net.Quic;

internal static class MsQuicHelpers
{
	internal static bool TryParse(this EndPoint endPoint, out string host, out IPAddress address, out int port)
	{
		if (endPoint is DnsEndPoint dnsEndPoint)
		{
			host = (IPAddress.TryParse(dnsEndPoint.Host, out address) ? null : dnsEndPoint.Host);
			port = dnsEndPoint.Port;
			return true;
		}
		if (endPoint is IPEndPoint iPEndPoint)
		{
			host = null;
			address = iPEndPoint.Address;
			port = iPEndPoint.Port;
			return true;
		}
		host = null;
		address = null;
		port = 0;
		return false;
	}

	internal unsafe static IPEndPoint QuicAddrToIPEndPoint(QuicAddr* quicAddress, AddressFamily? addressFamilyOverride = null)
	{
		Span<byte> span = new Span<byte>(quicAddress, 28);
		if (addressFamilyOverride.HasValue)
		{
			System.Net.SocketAddressPal.SetAddressFamily(span, addressFamilyOverride.Value);
		}
		return System.Net.Sockets.IPEndPointExtensions.CreateIPEndPoint(span);
	}

	internal static QuicAddr ToQuicAddr(this IPEndPoint ipEndPoint)
	{
		QuicAddr reference = default(QuicAddr);
		Span<byte> destination = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref reference, 1));
		ipEndPoint.Serialize(destination);
		return reference;
	}

	internal unsafe static T GetMsQuicParameter<T>(MsQuicSafeHandle handle, uint parameter) where T : unmanaged
	{
		Unsafe.SkipInit(out T result);
		GetMsQuicParameter(handle, parameter, (uint)sizeof(T), (byte*)(&result));
		return result;
	}

	internal unsafe static void GetMsQuicParameter(MsQuicSafeHandle handle, uint parameter, uint length, byte* value)
	{
		int param = MsQuicApi.Api.GetParam(handle, parameter, &length, value);
		if (MsQuic.StatusFailed(param))
		{
			ThrowHelper.ThrowMsQuicException(param, $"GetParam({handle}, {parameter}) failed");
		}
	}

	internal unsafe static void SetMsQuicParameter<T>(MsQuicSafeHandle handle, uint parameter, T value) where T : unmanaged
	{
		SetMsQuicParameter(handle, parameter, (uint)sizeof(T), (byte*)(&value));
	}

	internal unsafe static void SetMsQuicParameter(MsQuicSafeHandle handle, uint parameter, uint length, byte* value)
	{
		int status = MsQuicApi.Api.SetParam(handle, parameter, length, value);
		if (MsQuic.StatusFailed(status))
		{
			ThrowHelper.ThrowMsQuicException(status, $"SetParam({handle}, {parameter}) failed");
		}
	}
}
