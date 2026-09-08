using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Net.Http;

internal sealed class PreAuthCredentialCache
{
	private Dictionary<System.Net.CredentialCacheKey, NetworkCredential> _cache;

	public void Add(Uri uriPrefix, string authType, NetworkCredential cred)
	{
		System.Net.CredentialCacheKey credentialCacheKey = new System.Net.CredentialCacheKey(uriPrefix, authType);
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(this, $"Adding key:[{credentialCacheKey}], cred:[{cred.Domain}],[{cred.UserName}]", "Add");
		}
		if (_cache == null)
		{
			_cache = new Dictionary<System.Net.CredentialCacheKey, NetworkCredential>();
		}
		_cache.Add(credentialCacheKey, cred);
	}

	public void Remove(Uri uriPrefix, string authType)
	{
		if (_cache != null)
		{
			System.Net.CredentialCacheKey credentialCacheKey = new System.Net.CredentialCacheKey(uriPrefix, authType);
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(this, $"Removing key:[{credentialCacheKey}]", "Remove");
			}
			_cache.Remove(credentialCacheKey);
		}
	}

	public (Uri uriPrefix, NetworkCredential credential)? GetCredential(Uri uriPrefix, string authType)
	{
		if (_cache == null)
		{
			return null;
		}
		System.Net.CredentialCacheHelper.TryGetCredential(_cache, uriPrefix, authType, out var mostSpecificMatchUri, out var mostSpecificMatch);
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(this, FormattableStringFactory.Create("Returning {0}", (mostSpecificMatch == null) ? "null" : ("(" + mostSpecificMatch.UserName + ":" + mostSpecificMatch.Domain + ")")), "GetCredential");
		}
		if (mostSpecificMatch != null)
		{
			return (mostSpecificMatchUri, mostSpecificMatch);
		}
		return null;
	}
}
