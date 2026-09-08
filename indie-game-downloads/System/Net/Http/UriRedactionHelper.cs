using System.Diagnostics.CodeAnalysis;

namespace System.Net.Http;

internal static class UriRedactionHelper
{
	public static bool IsDisabled { get; } = GetDisableUriRedactionSettingValue();

	private static bool GetDisableUriRedactionSettingValue()
	{
		if (AppContext.TryGetSwitch("System.Net.Http.DisableUriRedaction", out var isEnabled))
		{
			return isEnabled;
		}
		string environmentVariable = Environment.GetEnvironmentVariable("DOTNET_SYSTEM_NET_HTTP_DISABLEURIREDACTION");
		if (bool.TryParse(environmentVariable, out isEnabled))
		{
			return isEnabled;
		}
		if (uint.TryParse(environmentVariable, out var result))
		{
			return result != 0;
		}
		return false;
	}

	public static string GetRedactedPathAndQuery(string pathAndQuery)
	{
		if (!IsDisabled)
		{
			int num = pathAndQuery.IndexOf('?');
			if (num >= 0 && num < pathAndQuery.Length - 1)
			{
				pathAndQuery = $"{Slice(pathAndQuery, 0, num + 1)}*";
			}
		}
		return pathAndQuery;
	}

	[return: NotNullIfNotNull("uri")]
	public static string GetRedactedUriString(Uri uri)
	{
		if ((object)uri == null)
		{
			return null;
		}
		if (IsDisabled)
		{
			if (!uri.IsAbsoluteUri)
			{
				return uri.ToString();
			}
			return uri.AbsoluteUri;
		}
		if (!uri.IsAbsoluteUri)
		{
			return "*";
		}
		string pathAndQuery = uri.PathAndQuery;
		int num = pathAndQuery.IndexOf('?');
		bool num2 = num >= 0 && num < pathAndQuery.Length - 1;
		bool isDefaultPort = uri.IsDefaultPort;
		if (num2)
		{
			if (isDefaultPort)
			{
				return $"{uri.Scheme}://{uri.Host}{Slice(pathAndQuery, 0, num + 1)}*";
			}
			return $"{uri.Scheme}://{uri.Host}:{uri.Port}{Slice(pathAndQuery, 0, num + 1)}*";
		}
		if (!isDefaultPort)
		{
			return $"{uri.Scheme}://{uri.Host}:{uri.Port}{pathAndQuery}";
		}
		return uri.Scheme + "://" + uri.Host + pathAndQuery;
	}

	private static ReadOnlySpan<char> Slice(string text, int startIndex, int length)
	{
		return text.AsSpan(startIndex, length);
	}
}
