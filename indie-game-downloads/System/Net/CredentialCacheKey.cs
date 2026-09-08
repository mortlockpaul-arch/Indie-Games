using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace System.Net;

internal sealed class CredentialCacheKey : IEquatable<CredentialCacheKey>
{
	public readonly Uri UriPrefix;

	public readonly int UriPrefixLength = -1;

	public readonly string AuthenticationType;

	internal CredentialCacheKey(Uri uriPrefix, string authenticationType)
	{
		UriPrefix = uriPrefix;
		UriPrefixLength = UriPrefix.AbsolutePath.LastIndexOf('/');
		AuthenticationType = authenticationType;
	}

	internal bool Match(Uri uri, int prefixLen, string authenticationType)
	{
		if (uri == null || authenticationType == null)
		{
			return false;
		}
		if (!string.Equals(authenticationType, AuthenticationType, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		if (NetEventSource.Log.IsEnabled())
		{
			NetEventSource.Info(this, $"Match({UriPrefix} & {uri})", "Match");
		}
		return IsPrefix(uri, prefixLen);
	}

	private bool IsPrefix(Uri uri, int prefixLen)
	{
		Uri uriPrefix = UriPrefix;
		if (uriPrefix.Scheme != uri.Scheme || uriPrefix.Host != uri.Host || uriPrefix.Port != uri.Port)
		{
			return false;
		}
		if (UriPrefixLength > prefixLen)
		{
			return false;
		}
		return string.Compare(uri.AbsolutePath, 0, uriPrefix.AbsolutePath, 0, UriPrefixLength, StringComparison.OrdinalIgnoreCase) == 0;
	}

	public override int GetHashCode()
	{
		return StringComparer.OrdinalIgnoreCase.GetHashCode(AuthenticationType) ^ UriPrefix.GetHashCode();
	}

	public bool Equals([NotNullWhen(true)] CredentialCacheKey other)
	{
		if (other == null)
		{
			return false;
		}
		bool flag = string.Equals(AuthenticationType, other.AuthenticationType, StringComparison.OrdinalIgnoreCase) && UriPrefix.Equals(other.UriPrefix);
		if (NetEventSource.Log.IsEnabled())
		{
			NetEventSource.Info(this, $"Equals({this},{other}) returns {flag}", "Equals");
		}
		return flag;
	}

	public override bool Equals([NotNullWhen(true)] object obj)
	{
		return Equals(obj as CredentialCacheKey);
	}

	public override string ToString()
	{
		IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
		DefaultInterpolatedStringHandler handler = new DefaultInterpolatedStringHandler(4, 3, invariantCulture);
		handler.AppendLiteral("[");
		handler.AppendFormatted(UriPrefixLength);
		handler.AppendLiteral("]:");
		handler.AppendFormatted(UriPrefix);
		handler.AppendLiteral(":");
		handler.AppendFormatted(AuthenticationType);
		return string.Create(invariantCulture, ref handler);
	}
}
