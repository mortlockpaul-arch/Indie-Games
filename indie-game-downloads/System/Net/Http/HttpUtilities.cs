namespace System.Net.Http;

internal static class HttpUtilities
{
	public static string ParseHostNameFromHeader(string hostHeader)
	{
		int num = hostHeader.IndexOf(':');
		if (num >= 0)
		{
			int num2 = hostHeader.IndexOf(']');
			if (num2 == -1)
			{
				return hostHeader.Substring(0, num);
			}
			num = hostHeader.LastIndexOf(':');
			if (num > num2)
			{
				return hostHeader.Substring(0, num);
			}
		}
		return hostHeader;
	}

	internal static bool IsSupportedScheme(string scheme)
	{
		if (!IsSupportedNonSecureScheme(scheme))
		{
			return IsSupportedSecureScheme(scheme);
		}
		return true;
	}

	internal static bool IsSupportedNonSecureScheme(string scheme)
	{
		if (!string.Equals(scheme, "http", StringComparison.OrdinalIgnoreCase))
		{
			return IsNonSecureWebSocketScheme(scheme);
		}
		return true;
	}

	internal static bool IsSupportedSecureScheme(string scheme)
	{
		if (!string.Equals(scheme, "https", StringComparison.OrdinalIgnoreCase))
		{
			return IsSecureWebSocketScheme(scheme);
		}
		return true;
	}

	internal static bool IsNonSecureWebSocketScheme(string scheme)
	{
		return string.Equals(scheme, "ws", StringComparison.OrdinalIgnoreCase);
	}

	internal static bool IsSecureWebSocketScheme(string scheme)
	{
		return string.Equals(scheme, "wss", StringComparison.OrdinalIgnoreCase);
	}

	internal static bool IsSupportedProxyScheme(string scheme)
	{
		if (!string.Equals(scheme, "http", StringComparison.OrdinalIgnoreCase) && !string.Equals(scheme, "https", StringComparison.OrdinalIgnoreCase))
		{
			return IsSocksScheme(scheme);
		}
		return true;
	}

	internal static bool IsSocksScheme(string scheme)
	{
		if (!string.Equals(scheme, "socks5", StringComparison.OrdinalIgnoreCase) && !string.Equals(scheme, "socks4a", StringComparison.OrdinalIgnoreCase))
		{
			return string.Equals(scheme, "socks4", StringComparison.OrdinalIgnoreCase);
		}
		return true;
	}
}
