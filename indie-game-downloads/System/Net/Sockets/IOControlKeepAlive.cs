using System.Runtime.CompilerServices;

namespace System.Net.Sockets;

internal sealed class IOControlKeepAlive
{
	private static readonly bool s_supportsKeepAliveViaSocketOption = SupportsKeepAliveViaSocketOption();

	private static readonly ConditionalWeakTable<SafeSocketHandle, IOControlKeepAlive> s_socketKeepAliveTable = new ConditionalWeakTable<SafeSocketHandle, IOControlKeepAlive>();

	[ThreadStatic]
	private static byte[] s_keepAliveValuesBuffer;

	private uint _timeMs = 7200000u;

	private uint _intervalMs = 1000u;

	public static bool IsNeeded => !s_supportsKeepAliveViaSocketOption;

	public static SocketError Get(SafeSocketHandle handle, SocketOptionName optionName, byte[] optionValueSeconds, ref int optionLength)
	{
		if (optionValueSeconds == null || !BitConverter.TryWriteBytes(optionValueSeconds.AsSpan(), Get(handle, optionName)))
		{
			return SocketError.Fault;
		}
		optionLength = optionValueSeconds.Length;
		return SocketError.Success;
	}

	public static int Get(SafeSocketHandle handle, SocketOptionName optionName)
	{
		if (s_socketKeepAliveTable.TryGetValue(handle, out var value))
		{
			if (optionName != SocketOptionName.TypeOfService)
			{
				return MillisecondsToSeconds(value._intervalMs);
			}
			return MillisecondsToSeconds(value._timeMs);
		}
		if (optionName != SocketOptionName.TypeOfService)
		{
			return MillisecondsToSeconds(1000u);
		}
		return MillisecondsToSeconds(7200000u);
	}

	public static SocketError Set(SafeSocketHandle handle, SocketOptionName optionName, byte[] optionValueSeconds)
	{
		if (optionValueSeconds == null || optionValueSeconds.Length < 4)
		{
			return SocketError.Fault;
		}
		return Set(handle, optionName, BitConverter.ToInt32(optionValueSeconds, 0));
	}

	public static SocketError Set(SafeSocketHandle handle, SocketOptionName optionName, int optionValueSeconds)
	{
		IOControlKeepAlive orAdd = s_socketKeepAliveTable.GetOrAdd(handle, (SafeSocketHandle safeSocketHandle) => new IOControlKeepAlive());
		if (optionName == SocketOptionName.TypeOfService)
		{
			orAdd._timeMs = SecondsToMilliseconds(optionValueSeconds);
		}
		else
		{
			orAdd._intervalMs = SecondsToMilliseconds(optionValueSeconds);
		}
		byte[] array = s_keepAliveValuesBuffer ?? (s_keepAliveValuesBuffer = new byte[12]);
		orAdd.Fill(array);
		int optionLength = 0;
		return SocketPal.WindowsIoctl(handle, -1744830460, array, null, out optionLength);
	}

	private static bool SupportsKeepAliveViaSocketOption()
	{
		using Socket socket = new Socket(Socket.OSSupportsIPv4 ? AddressFamily.InterNetwork : AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
		int optionValue = MillisecondsToSeconds(7200000u);
		SocketError num = global::Interop.Winsock.setsockopt(socket.SafeHandle, SocketOptionLevel.Tcp, SocketOptionName.TypeOfService, ref optionValue, 4);
		int optionValue2 = MillisecondsToSeconds(1000u);
		SocketError socketError = global::Interop.Winsock.setsockopt(socket.SafeHandle, SocketOptionLevel.Tcp, SocketOptionName.BlockSource, ref optionValue2, 4);
		return num == SocketError.Success && socketError == SocketError.Success;
	}

	private static int MillisecondsToSeconds(uint milliseconds)
	{
		return (int)(milliseconds / 1000);
	}

	private static uint SecondsToMilliseconds(int seconds)
	{
		return (uint)(seconds * 1000);
	}

	private void Fill(byte[] buffer)
	{
		BitConverter.TryWriteBytes(buffer.AsSpan(), 1u);
		BitConverter.TryWriteBytes(buffer.AsSpan(4), _timeMs);
		BitConverter.TryWriteBytes(buffer.AsSpan(8), _intervalMs);
	}
}
