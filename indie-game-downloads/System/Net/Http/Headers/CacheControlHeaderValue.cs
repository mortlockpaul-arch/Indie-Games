using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace System.Net.Http.Headers;

/// <summary>Represents the value of the Cache-Control header.</summary>
public class CacheControlHeaderValue : ICloneable
{
	[Flags]
	private enum Flags
	{
		None = 0,
		MaxAgeHasValue = 1,
		SharedMaxAgeHasValue = 2,
		MaxStaleLimitHasValue = 4,
		MinFreshHasValue = 8,
		NoCache = 0x10,
		NoStore = 0x20,
		MaxStale = 0x40,
		NoTransform = 0x80,
		OnlyIfCached = 0x100,
		Public = 0x200,
		Private = 0x400,
		MustRevalidate = 0x800,
		ProxyRevalidate = 0x1000
	}

	private sealed class TokenObjectCollection : ObjectCollection<string>
	{
		public override void Validate(string item)
		{
			HeaderUtilities.CheckValidToken(item, "item");
		}

		public int GetHashCode(StringComparer comparer)
		{
			int num = 0;
			using Enumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				num ^= comparer.GetHashCode(current);
			}
			return num;
		}
	}

	private static readonly GenericHeaderParser s_nameValueListParser = GenericHeaderParser.MultipleValueNameValueParser;

	private Flags _flags;

	private TokenObjectCollection _noCacheHeaders;

	private TimeSpan _maxAge;

	private TimeSpan _sharedMaxAge;

	private TimeSpan _maxStaleLimit;

	private TimeSpan _minFresh;

	private TokenObjectCollection _privateHeaders;

	private UnvalidatedObjectCollection<NameValueHeaderValue> _extensions;

	/// <summary>Whether an HTTP client is willing to accept a cached response.</summary>
	/// <returns>
	///   <see langword="true" /> if the HTTP client is willing to accept a cached response; otherwise, <see langword="false" />.</returns>
	public bool NoCache
	{
		get
		{
			return (_flags & Flags.NoCache) != 0;
		}
		set
		{
			SetFlag(Flags.NoCache, value);
		}
	}

	/// <summary>A collection of fieldnames in the "no-cache" directive in a cache-control header field on an HTTP response.</summary>
	/// <returns>A collection of fieldnames.</returns>
	public ICollection<string> NoCacheHeaders => _noCacheHeaders ?? (_noCacheHeaders = new TokenObjectCollection());

	/// <summary>Whether a cache must not store any part of either the HTTP request mressage or any response.</summary>
	/// <returns>
	///   <see langword="true" /> if a cache must not store any part of either the HTTP request mressage or any response; otherwise, <see langword="false" />.</returns>
	public bool NoStore
	{
		get
		{
			return (_flags & Flags.NoStore) != 0;
		}
		set
		{
			SetFlag(Flags.NoStore, value);
		}
	}

	/// <summary>The maximum age, specified in seconds, that the HTTP client is willing to accept a response.</summary>
	/// <returns>The time in seconds.</returns>
	public TimeSpan? MaxAge
	{
		get
		{
			if ((_flags & Flags.MaxAgeHasValue) != Flags.None)
			{
				return _maxAge;
			}
			return null;
		}
		set
		{
			SetTimeSpan(ref _maxAge, Flags.MaxAgeHasValue, value);
		}
	}

	/// <summary>The shared maximum age, specified in seconds, in an HTTP response that overrides the "max-age" directive in a cache-control header or an Expires header for a shared cache.</summary>
	/// <returns>The time in seconds.</returns>
	public TimeSpan? SharedMaxAge
	{
		get
		{
			if ((_flags & Flags.SharedMaxAgeHasValue) != Flags.None)
			{
				return _sharedMaxAge;
			}
			return null;
		}
		set
		{
			SetTimeSpan(ref _sharedMaxAge, Flags.SharedMaxAgeHasValue, value);
		}
	}

	/// <summary>Whether an HTTP client is willing to accept a response that has exceeded its expiration time.</summary>
	/// <returns>
	///   <see langword="true" /> if the HTTP client is willing to accept a response that has exceed the expiration time; otherwise, <see langword="false" />.</returns>
	public bool MaxStale
	{
		get
		{
			return (_flags & Flags.MaxStale) != 0;
		}
		set
		{
			SetFlag(Flags.MaxStale, value);
		}
	}

	/// <summary>The maximum time, in seconds, an HTTP client is willing to accept a response that has exceeded its expiration time.</summary>
	/// <returns>The time in seconds.</returns>
	public TimeSpan? MaxStaleLimit
	{
		get
		{
			if ((_flags & Flags.MaxStaleLimitHasValue) != Flags.None)
			{
				return _maxStaleLimit;
			}
			return null;
		}
		set
		{
			SetTimeSpan(ref _maxStaleLimit, Flags.MaxStaleLimitHasValue, value);
		}
	}

	/// <summary>The freshness lifetime, in seconds, that an HTTP client is willing to accept a response.</summary>
	/// <returns>The time in seconds.</returns>
	public TimeSpan? MinFresh
	{
		get
		{
			if ((_flags & Flags.MinFreshHasValue) != Flags.None)
			{
				return _minFresh;
			}
			return null;
		}
		set
		{
			SetTimeSpan(ref _minFresh, Flags.MinFreshHasValue, value);
		}
	}

	/// <summary>Whether a cache or proxy must not change any aspect of the entity-body.</summary>
	/// <returns>
	///   <see langword="true" /> if a cache or proxy must not change any aspect of the entity-body; otherwise, <see langword="false" />.</returns>
	public bool NoTransform
	{
		get
		{
			return (_flags & Flags.NoTransform) != 0;
		}
		set
		{
			SetFlag(Flags.NoTransform, value);
		}
	}

	/// <summary>Whether a cache should either respond using a cached entry that is consistent with the other constraints of the HTTP request, or respond with a 504 (Gateway Timeout) status.</summary>
	/// <returns>
	///   <see langword="true" /> if a cache should either respond using a cached entry that is consistent with the other constraints of the HTTP request, or respond with a 504 (Gateway Timeout) status; otherwise, <see langword="false" />.</returns>
	public bool OnlyIfCached
	{
		get
		{
			return (_flags & Flags.OnlyIfCached) != 0;
		}
		set
		{
			SetFlag(Flags.OnlyIfCached, value);
		}
	}

	/// <summary>Whether an HTTP response may be cached by any cache, even if it would normally be non-cacheable or cacheable only within a non- shared cache.</summary>
	/// <returns>
	///   <see langword="true" /> if the HTTP response may be cached by any cache, even if it would normally be non-cacheable or cacheable only within a non- shared cache; otherwise, <see langword="false" />.</returns>
	public bool Public
	{
		get
		{
			return (_flags & Flags.Public) != 0;
		}
		set
		{
			SetFlag(Flags.Public, value);
		}
	}

	/// <summary>Whether all or part of the HTTP response message is intended for a single user and must not be cached by a shared cache.</summary>
	/// <returns>
	///   <see langword="true" /> if the HTTP response message is intended for a single user and must not be cached by a shared cache; otherwise, <see langword="false" />.</returns>
	public bool Private
	{
		get
		{
			return (_flags & Flags.Private) != 0;
		}
		set
		{
			SetFlag(Flags.Private, value);
		}
	}

	/// <summary>A collection fieldnames in the "private" directive in a cache-control header field on an HTTP response.</summary>
	/// <returns>A collection of fieldnames.</returns>
	public ICollection<string> PrivateHeaders => _privateHeaders ?? (_privateHeaders = new TokenObjectCollection());

	/// <summary>Whether the origin server require revalidation of a cache entry on any subsequent use when the cache entry becomes stale.</summary>
	/// <returns>
	///   <see langword="true" /> if the origin server requires revalidation of a cache entry on any subsequent use when the entry becomes stale; otherwise, <see langword="false" />.</returns>
	public bool MustRevalidate
	{
		get
		{
			return (_flags & Flags.MustRevalidate) != 0;
		}
		set
		{
			SetFlag(Flags.MustRevalidate, value);
		}
	}

	/// <summary>Whether the origin server require revalidation of a cache entry on any subsequent use when the cache entry becomes stale for shared user agent caches.</summary>
	/// <returns>
	///   <see langword="true" /> if the origin server requires revalidation of a cache entry on any subsequent use when the entry becomes stale for shared user agent caches; otherwise, <see langword="false" />.</returns>
	public bool ProxyRevalidate
	{
		get
		{
			return (_flags & Flags.ProxyRevalidate) != 0;
		}
		set
		{
			SetFlag(Flags.ProxyRevalidate, value);
		}
	}

	/// <summary>Cache-extension tokens, each with an optional assigned value.</summary>
	/// <returns>A collection of cache-extension tokens each with an optional assigned value.</returns>
	public ICollection<NameValueHeaderValue> Extensions => _extensions ?? (_extensions = new UnvalidatedObjectCollection<NameValueHeaderValue>());

	private void SetTimeSpan(ref TimeSpan fieldRef, Flags flag, TimeSpan? value)
	{
		fieldRef = value.GetValueOrDefault();
		SetFlag(flag, value.HasValue);
	}

	private void SetFlag(Flags flag, bool value)
	{
		if (value)
		{
			Interlocked.Or(ref Unsafe.As<Flags, int>(ref _flags), (int)flag);
		}
		else
		{
			Interlocked.And(ref Unsafe.As<Flags, int>(ref _flags), (int)(~flag));
		}
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.CacheControlHeaderValue" /> class.</summary>
	public CacheControlHeaderValue()
	{
	}

	private CacheControlHeaderValue(CacheControlHeaderValue source)
	{
		_flags = source._flags;
		_maxAge = source._maxAge;
		_sharedMaxAge = source._sharedMaxAge;
		_maxStaleLimit = source._maxStaleLimit;
		_minFresh = source._minFresh;
		if (source._noCacheHeaders != null)
		{
			foreach (string noCacheHeader in source._noCacheHeaders)
			{
				NoCacheHeaders.Add(noCacheHeader);
			}
		}
		if (source._privateHeaders != null)
		{
			foreach (string privateHeader in source._privateHeaders)
			{
				PrivateHeaders.Add(privateHeader);
			}
		}
		_extensions = source._extensions.Clone();
	}

	/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.CacheControlHeaderValue" /> object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override string ToString()
	{
		StringBuilder stringBuilder = System.Text.StringBuilderCache.Acquire();
		AppendValueIfRequired(stringBuilder, NoStore, "no-store");
		AppendValueIfRequired(stringBuilder, NoTransform, "no-transform");
		AppendValueIfRequired(stringBuilder, OnlyIfCached, "only-if-cached");
		AppendValueIfRequired(stringBuilder, Public, "public");
		AppendValueIfRequired(stringBuilder, MustRevalidate, "must-revalidate");
		AppendValueIfRequired(stringBuilder, ProxyRevalidate, "proxy-revalidate");
		if (NoCache)
		{
			AppendValueWithSeparatorIfRequired(stringBuilder, "no-cache");
			if (_noCacheHeaders != null && _noCacheHeaders.Count > 0)
			{
				stringBuilder.Append("=\"");
				AppendValues(stringBuilder, _noCacheHeaders);
				stringBuilder.Append('"');
			}
		}
		if ((_flags & Flags.MaxAgeHasValue) != Flags.None)
		{
			AppendValueWithSeparatorIfRequired(stringBuilder, "max-age");
			stringBuilder.Append('=');
			int num = (int)_maxAge.TotalSeconds;
			if (num >= 0)
			{
				stringBuilder.Append(num);
			}
			else
			{
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder3 = stringBuilder2;
				IFormatProvider invariantInfo = NumberFormatInfo.InvariantInfo;
				IFormatProvider provider = invariantInfo;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(0, 1, stringBuilder2, invariantInfo);
				handler.AppendFormatted(num);
				stringBuilder3.Append(provider, ref handler);
			}
		}
		if ((_flags & Flags.SharedMaxAgeHasValue) != Flags.None)
		{
			AppendValueWithSeparatorIfRequired(stringBuilder, "s-maxage");
			stringBuilder.Append('=');
			int num2 = (int)_sharedMaxAge.TotalSeconds;
			if (num2 >= 0)
			{
				stringBuilder.Append(num2);
			}
			else
			{
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder4 = stringBuilder2;
				IFormatProvider invariantInfo = NumberFormatInfo.InvariantInfo;
				IFormatProvider provider2 = invariantInfo;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(0, 1, stringBuilder2, invariantInfo);
				handler.AppendFormatted(num2);
				stringBuilder4.Append(provider2, ref handler);
			}
		}
		if (MaxStale)
		{
			AppendValueWithSeparatorIfRequired(stringBuilder, "max-stale");
			if ((_flags & Flags.MaxStaleLimitHasValue) != Flags.None)
			{
				stringBuilder.Append('=');
				int num3 = (int)_maxStaleLimit.TotalSeconds;
				if (num3 >= 0)
				{
					stringBuilder.Append(num3);
				}
				else
				{
					StringBuilder stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder5 = stringBuilder2;
					IFormatProvider invariantInfo = NumberFormatInfo.InvariantInfo;
					IFormatProvider provider3 = invariantInfo;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(0, 1, stringBuilder2, invariantInfo);
					handler.AppendFormatted(num3);
					stringBuilder5.Append(provider3, ref handler);
				}
			}
		}
		if ((_flags & Flags.MinFreshHasValue) != Flags.None)
		{
			AppendValueWithSeparatorIfRequired(stringBuilder, "min-fresh");
			stringBuilder.Append('=');
			int num4 = (int)_minFresh.TotalSeconds;
			if (num4 >= 0)
			{
				stringBuilder.Append(num4);
			}
			else
			{
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder6 = stringBuilder2;
				IFormatProvider invariantInfo = NumberFormatInfo.InvariantInfo;
				IFormatProvider provider4 = invariantInfo;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(0, 1, stringBuilder2, invariantInfo);
				handler.AppendFormatted(num4);
				stringBuilder6.Append(provider4, ref handler);
			}
		}
		if (Private)
		{
			AppendValueWithSeparatorIfRequired(stringBuilder, "private");
			if (_privateHeaders != null && _privateHeaders.Count > 0)
			{
				stringBuilder.Append("=\"");
				AppendValues(stringBuilder, _privateHeaders);
				stringBuilder.Append('"');
			}
		}
		NameValueHeaderValue.ToString(_extensions, ',', leadingSeparator: false, stringBuilder);
		return System.Text.StringBuilderCache.GetStringAndRelease(stringBuilder);
	}

	/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.CacheControlHeaderValue" /> object.</summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>
	///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is CacheControlHeaderValue cacheControlHeaderValue && _flags == cacheControlHeaderValue._flags && _maxAge == cacheControlHeaderValue._maxAge && _sharedMaxAge == cacheControlHeaderValue._sharedMaxAge && _maxStaleLimit == cacheControlHeaderValue._maxStaleLimit && _minFresh == cacheControlHeaderValue._minFresh && HeaderUtilities.AreEqualCollections(_noCacheHeaders, cacheControlHeaderValue._noCacheHeaders, StringComparer.OrdinalIgnoreCase) && HeaderUtilities.AreEqualCollections(_privateHeaders, cacheControlHeaderValue._privateHeaders, StringComparer.OrdinalIgnoreCase))
		{
			return HeaderUtilities.AreEqualCollections(_extensions, cacheControlHeaderValue._extensions);
		}
		return false;
	}

	/// <summary>Serves as a hash function for a  <see cref="T:System.Net.Http.Headers.CacheControlHeaderValue" /> object.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override int GetHashCode()
	{
		return HashCode.Combine(_flags, _maxAge, _sharedMaxAge, _maxStaleLimit, _minFresh, (_noCacheHeaders != null) ? _noCacheHeaders.GetHashCode(StringComparer.OrdinalIgnoreCase) : 0, (_privateHeaders != null) ? _privateHeaders.GetHashCode(StringComparer.OrdinalIgnoreCase) : 0, NameValueHeaderValue.GetHashCode(_extensions));
	}

	/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.CacheControlHeaderValue" /> instance.</summary>
	/// <param name="input">A string that represents cache-control header value information.</param>
	/// <returns>A <see cref="T:System.Net.Http.Headers.CacheControlHeaderValue" /> instance.</returns>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
	/// <exception cref="T:System.FormatException">
	///   <paramref name="input" /> is not valid cache-control header value information.</exception>
	public static CacheControlHeaderValue Parse(string? input)
	{
		int index = 0;
		return ((CacheControlHeaderValue)CacheControlHeaderParser.Parser.ParseValue(input, null, ref index)) ?? new CacheControlHeaderValue();
	}

	/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.CacheControlHeaderValue" /> information.</summary>
	/// <param name="input">The string to validate.</param>
	/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.CacheControlHeaderValue" /> version of the string.</param>
	/// <returns>
	///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.CacheControlHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
	public static bool TryParse(string? input, [NotNullWhen(true)] out CacheControlHeaderValue? parsedValue)
	{
		int index = 0;
		parsedValue = null;
		if (CacheControlHeaderParser.Parser.TryParseValue(input, null, ref index, out var parsedValue2))
		{
			parsedValue = ((CacheControlHeaderValue)parsedValue2) ?? new CacheControlHeaderValue();
			return true;
		}
		return false;
	}

	internal static int GetCacheControlLength(string input, int startIndex, CacheControlHeaderValue storeValue, out CacheControlHeaderValue parsedValue)
	{
		parsedValue = null;
		if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
		{
			return 0;
		}
		int index = startIndex;
		List<NameValueHeaderValue> list = new List<NameValueHeaderValue>();
		while (index < input.Length)
		{
			if (!s_nameValueListParser.TryParseValue(input, null, ref index, out var parsedValue2))
			{
				return 0;
			}
			list.Add((NameValueHeaderValue)parsedValue2);
		}
		CacheControlHeaderValue cacheControlHeaderValue = storeValue ?? new CacheControlHeaderValue();
		if (!TrySetCacheControlValues(cacheControlHeaderValue, list))
		{
			return 0;
		}
		if (storeValue == null)
		{
			parsedValue = cacheControlHeaderValue;
		}
		return input.Length - startIndex;
	}

	private static bool TrySetCacheControlValues(CacheControlHeaderValue cc, List<NameValueHeaderValue> nameValueList)
	{
		foreach (NameValueHeaderValue nameValue in nameValueList)
		{
			string text = nameValue.Name.ToLowerInvariant();
			string value = nameValue.Value;
			Flags flags = Flags.None;
			bool flag = value == null;
			switch (text)
			{
			case "no-cache":
				flags = Flags.NoCache;
				flag = TrySetOptionalTokenList(nameValue, ref cc._noCacheHeaders);
				break;
			case "no-store":
				flags = Flags.NoStore;
				break;
			case "max-age":
				flags = Flags.MaxAgeHasValue;
				flag = TrySetTimeSpan(value, ref cc._maxAge);
				break;
			case "max-stale":
				flags = Flags.MaxStale;
				if (TrySetTimeSpan(value, ref cc._maxStaleLimit))
				{
					flag = true;
					flags = Flags.MaxStaleLimitHasValue | Flags.MaxStale;
				}
				break;
			case "min-fresh":
				flags = Flags.MinFreshHasValue;
				flag = TrySetTimeSpan(value, ref cc._minFresh);
				break;
			case "no-transform":
				flags = Flags.NoTransform;
				break;
			case "only-if-cached":
				flags = Flags.OnlyIfCached;
				break;
			case "public":
				flags = Flags.Public;
				break;
			case "private":
				flags = Flags.Private;
				flag = TrySetOptionalTokenList(nameValue, ref cc._privateHeaders);
				break;
			case "must-revalidate":
				flags = Flags.MustRevalidate;
				break;
			case "proxy-revalidate":
				flags = Flags.ProxyRevalidate;
				break;
			case "s-maxage":
				flags = Flags.SharedMaxAgeHasValue;
				flag = TrySetTimeSpan(value, ref cc._sharedMaxAge);
				break;
			default:
				flag = true;
				cc.Extensions.Add(nameValue);
				break;
			}
			if (flag)
			{
				cc._flags |= flags;
				continue;
			}
			return false;
		}
		return true;
	}

	private static bool TrySetOptionalTokenList(NameValueHeaderValue nameValue, ref TokenObjectCollection destination)
	{
		if (nameValue.Value == null)
		{
			return true;
		}
		string value = nameValue.Value;
		if (value.Length < 3 || !value.StartsWith('"') || !value.EndsWith('"'))
		{
			return false;
		}
		int num = 1;
		int num2 = value.Length - 1;
		int num3 = ((destination != null) ? destination.Count : 0);
		while (num < num2)
		{
			num = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(value, num, skipEmptyValues: true, out var _);
			if (num == num2)
			{
				break;
			}
			int tokenLength = HttpRuleParser.GetTokenLength(value, num);
			if (tokenLength == 0)
			{
				return false;
			}
			if (destination == null)
			{
				destination = new TokenObjectCollection();
			}
			destination.Add(value.Substring(num, tokenLength));
			num += tokenLength;
		}
		if (destination != null && destination.Count > num3)
		{
			return true;
		}
		return false;
	}

	private static bool TrySetTimeSpan(string value, ref TimeSpan timeSpan)
	{
		if (value == null || !HeaderUtilities.TryParseInt32(value, out var result))
		{
			return false;
		}
		timeSpan = new TimeSpan(0, 0, result);
		return true;
	}

	private static void AppendValueIfRequired(StringBuilder sb, bool appendValue, string value)
	{
		if (appendValue)
		{
			AppendValueWithSeparatorIfRequired(sb, value);
		}
	}

	private static void AppendValueWithSeparatorIfRequired(StringBuilder sb, string value)
	{
		if (sb.Length > 0)
		{
			sb.Append(", ");
		}
		sb.Append(value);
	}

	private static void AppendValues(StringBuilder sb, TokenObjectCollection values)
	{
		bool flag = true;
		foreach (string value in values)
		{
			if (flag)
			{
				flag = false;
			}
			else
			{
				sb.Append(", ");
			}
			sb.Append(value);
		}
	}

	/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.CacheControlHeaderValue" /> instance.</summary>
	/// <returns>A copy of the current instance.</returns>
	object ICloneable.Clone()
	{
		return new CacheControlHeaderValue(this);
	}
}
