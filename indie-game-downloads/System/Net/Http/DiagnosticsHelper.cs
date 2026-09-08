using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Threading;

namespace System.Net.Http;

internal static class DiagnosticsHelper
{
	private static object[] s_boxedStatusCodes;

	private static string[] s_statusCodeStrings;

	public static InstrumentAdvice<double> ShortHistogramAdvice { get; } = new InstrumentAdvice<double>
	{
		HistogramBucketBoundaries = new global::_003C_003Ez__ReadOnlyArray<double>(new double[14]
		{
			0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1.0,
			2.5, 5.0, 7.5, 10.0
		})
	};

	internal static KeyValuePair<string, object> GetMethodTag(HttpMethod method, out bool isUnknownMethod)
	{
		HttpMethod knownMethod = HttpMethod.GetKnownMethod(method.Method.AsSpan());
		isUnknownMethod = (object)knownMethod == null;
		return new KeyValuePair<string, object>("http.request.method", isUnknownMethod ? "_OTHER" : knownMethod.Method);
	}

	internal static string GetProtocolVersionString(Version httpVersion)
	{
		int major = httpVersion.Major;
		int minor = httpVersion.Minor;
		switch (major)
		{
		case 1:
			switch (minor)
			{
			case 0:
				return "1.0";
			case 1:
				return "1.1";
			}
			break;
		case 2:
			if (minor != 0)
			{
				break;
			}
			return "2";
		case 3:
			if (minor != 0)
			{
				break;
			}
			return "3";
		}
		return httpVersion.ToString();
	}

	public static string GetServerAddress(HttpRequestMessage request, IWebProxy proxy)
	{
		if ((proxy == null || proxy.IsBypassed(request.RequestUri)) && request.HasHeaders)
		{
			string host = request.Headers.Host;
			if (host != null)
			{
				return HttpUtilities.ParseHostNameFromHeader(host);
			}
		}
		return request.RequestUri.IdnHost;
	}

	public static bool TryGetErrorType(HttpResponseMessage response, Exception exception, out string errorType)
	{
		if (response != null)
		{
			int statusCode = (int)response.StatusCode;
			if (statusCode >= 400 && statusCode <= 599)
			{
				errorType = GetErrorStatusCodeString(statusCode);
				return true;
			}
		}
		if (exception == null)
		{
			errorType = null;
			return false;
		}
		errorType = (exception as HttpRequestException)?.HttpRequestError switch
		{
			HttpRequestError.NameResolutionError => "name_resolution_error", 
			HttpRequestError.ConnectionError => "connection_error", 
			HttpRequestError.SecureConnectionError => "secure_connection_error", 
			HttpRequestError.HttpProtocolError => "http_protocol_error", 
			HttpRequestError.ExtendedConnectNotSupported => "extended_connect_not_supported", 
			HttpRequestError.VersionNegotiationError => "version_negotiation_error", 
			HttpRequestError.UserAuthenticationError => "user_authentication_error", 
			HttpRequestError.ProxyTunnelError => "proxy_tunnel_error", 
			HttpRequestError.InvalidResponse => "invalid_response", 
			HttpRequestError.ResponseEnded => "response_ended", 
			HttpRequestError.ConfigurationLimitExceeded => "configuration_limit_exceeded", 
			_ => exception.GetType().FullName, 
		};
		return true;
	}

	public static object GetBoxedInt32(int value)
	{
		object[] array = LazyInitializer.EnsureInitialized(ref s_boxedStatusCodes, () => new object[512]);
		if ((uint)value >= (uint)array.Length)
		{
			return value;
		}
		object[] array2 = array;
		return array2[value] ?? (array2[value] = value);
	}

	private static string GetErrorStatusCodeString(int statusCode)
	{
		string[] array = LazyInitializer.EnsureInitialized(ref s_statusCodeStrings, () => new string[200]);
		int num = statusCode - 400;
		if ((uint)num >= (uint)array.Length)
		{
			return statusCode.ToString();
		}
		ref string reference = ref array[num];
		return reference ?? (reference = statusCode.ToString());
	}
}
