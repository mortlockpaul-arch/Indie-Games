using System.Threading;

namespace System.Net.WebSockets;

internal static class WebSocketDefaults
{
	public static readonly TimeSpan DefaultKeepAliveInterval = TimeSpan.Zero;

	public static readonly TimeSpan DefaultClientKeepAliveInterval = TimeSpan.FromSeconds(30L);

	public static readonly TimeSpan DefaultKeepAliveTimeout = Timeout.InfiniteTimeSpan;
}
