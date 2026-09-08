using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Net;

internal static class CredentialCacheHelper
{
	public static bool TryGetCredential(Dictionary<CredentialCacheKey, NetworkCredential> cache, Uri uriPrefix, string authType, [NotNullWhen(true)] out Uri mostSpecificMatchUri, [NotNullWhen(true)] out NetworkCredential mostSpecificMatch)
	{
		int num = -1;
		mostSpecificMatch = null;
		mostSpecificMatchUri = null;
		if (cache.Count == 0)
		{
			return false;
		}
		int num2 = uriPrefix.AbsolutePath.LastIndexOf('/');
		foreach (var (credentialCacheKey2, networkCredential2) in cache)
		{
			int uriPrefixLength = credentialCacheKey2.UriPrefixLength;
			if (uriPrefixLength > num && credentialCacheKey2.Match(uriPrefix, num2, authType))
			{
				num = uriPrefixLength;
				mostSpecificMatch = networkCredential2;
				mostSpecificMatchUri = credentialCacheKey2.UriPrefix;
				if (num2 == uriPrefixLength)
				{
					break;
				}
			}
		}
		return mostSpecificMatch != null;
	}
}
