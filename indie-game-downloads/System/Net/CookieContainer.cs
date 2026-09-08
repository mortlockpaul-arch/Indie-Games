using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Net;

[Serializable]
[TypeForwardedFrom("System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
public class CookieContainer
{
	public const int DefaultCookieLimit = 300;

	public const int DefaultPerDomainCookieLimit = 20;

	public const int DefaultCookieLengthLimit = 4096;

	private static readonly HeaderVariantInfo[] s_headerInfo = new HeaderVariantInfo[2]
	{
		new HeaderVariantInfo("Set-Cookie", CookieVariant.Rfc2109),
		new HeaderVariantInfo("Set-Cookie2", CookieVariant.Rfc2965)
	};

	private readonly Hashtable m_domainTable = new Hashtable();

	private int m_maxCookieSize = 4096;

	private int m_maxCookies = 300;

	private int m_maxCookiesPerDomain = 20;

	private int m_count;

	private readonly string m_fqdnMyDomain = string.Empty;

	public int Capacity
	{
		get
		{
			return m_maxCookies;
		}
		set
		{
			if (value <= 0 || (value < m_maxCookiesPerDomain && m_maxCookiesPerDomain != int.MaxValue))
			{
				throw new ArgumentOutOfRangeException("value", System.SR.Format(System.SR.net_cookie_capacity_range, "Capacity", 0, m_maxCookiesPerDomain));
			}
			if (value < m_maxCookies)
			{
				m_maxCookies = value;
				AgeCookies(null);
			}
			m_maxCookies = value;
		}
	}

	public int Count => m_count;

	public int MaxCookieSize
	{
		get
		{
			return m_maxCookieSize;
		}
		set
		{
			ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, "value");
			m_maxCookieSize = value;
		}
	}

	public int PerDomainCapacity
	{
		get
		{
			return m_maxCookiesPerDomain;
		}
		set
		{
			ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, "value");
			if (value != int.MaxValue)
			{
				ArgumentOutOfRangeException.ThrowIfGreaterThan(value, m_maxCookies, "value");
			}
			if (value < m_maxCookiesPerDomain)
			{
				m_maxCookiesPerDomain = value;
				AgeCookies(null);
			}
			m_maxCookiesPerDomain = value;
		}
	}

	public CookieContainer()
	{
	}

	public CookieContainer(int capacity)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity, "capacity");
		m_maxCookies = capacity;
	}

	public CookieContainer(int capacity, int perDomainCapacity, int maxCookieSize)
		: this(capacity)
	{
		if (perDomainCapacity != int.MaxValue && (perDomainCapacity <= 0 || perDomainCapacity > capacity))
		{
			throw new ArgumentOutOfRangeException("perDomainCapacity", System.SR.Format(System.SR.net_cookie_capacity_range, "PerDomainCapacity", 0, capacity));
		}
		m_maxCookiesPerDomain = perDomainCapacity;
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxCookieSize, "maxCookieSize");
		m_maxCookieSize = maxCookieSize;
	}

	public void Add(Cookie cookie)
	{
		ArgumentNullException.ThrowIfNull(cookie, "cookie");
		if (cookie.Domain.Length == 0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.net_emptystringcall, "cookie.Domain"), "cookie");
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(cookie.Secure ? "https" : "http").Append("://");
		if (!cookie.DomainImplicit && cookie.Domain[0] == '.')
		{
			stringBuilder.Append('0');
		}
		stringBuilder.Append(cookie.Domain);
		if (cookie.PortList != null)
		{
			stringBuilder.Append(':').Append(cookie.PortList[0]);
		}
		stringBuilder.Append(cookie.Path);
		if (!Uri.TryCreate(stringBuilder.ToString(), UriKind.Absolute, out Uri result))
		{
			throw new CookieException(System.SR.Format(System.SR.net_cookie_attribute, "Domain", cookie.Domain));
		}
		Cookie cookie2 = cookie.Clone();
		cookie2.VerifyAndSetDefaults(cookie2.Variant, result);
		InternalAdd(cookie2);
	}

	internal void InternalAdd(Cookie cookie)
	{
		if (cookie.Value.Length > m_maxCookieSize)
		{
			throw new CookieException(System.SR.Format(System.SR.net_cookie_size, cookie, m_maxCookieSize));
		}
		try
		{
			PathList pathList;
			lock (m_domainTable.SyncRoot)
			{
				pathList = (PathList)m_domainTable[cookie.DomainKey];
				if (pathList == null)
				{
					pathList = (PathList)(m_domainTable[cookie.DomainKey] = new PathList());
				}
			}
			int cookiesCount = pathList.GetCookiesCount();
			CookieCollection cookieCollection;
			lock (pathList.SyncRoot)
			{
				cookieCollection = (CookieCollection)pathList[cookie.Path];
				if (cookieCollection == null)
				{
					cookieCollection = new CookieCollection();
					pathList[cookie.Path] = cookieCollection;
				}
			}
			if (cookie.Expired)
			{
				lock (cookieCollection)
				{
					int num = cookieCollection.IndexOf(cookie);
					if (num != -1)
					{
						cookieCollection.RemoveAt(num);
						m_count--;
					}
				}
			}
			else
			{
				if ((cookiesCount >= m_maxCookiesPerDomain && !AgeCookies(cookie.DomainKey)) || (m_count >= m_maxCookies && !AgeCookies(null)))
				{
					return;
				}
				lock (cookieCollection)
				{
					m_count += cookieCollection.InternalAdd(cookie, isStrict: true);
				}
			}
			if (m_domainTable.Count > m_count || pathList.Count > m_maxCookiesPerDomain)
			{
				DomainTableCleanup();
			}
		}
		catch (OutOfMemoryException)
		{
			throw;
		}
		catch (Exception innerException)
		{
			throw new CookieException(System.SR.net_container_add_cookie, innerException);
		}
	}

	private bool AgeCookies(string domain)
	{
		int num = 0;
		DateTime dateTime = DateTime.MaxValue;
		CookieCollection cookieCollection = null;
		int num2 = 0;
		float num3 = 1f;
		if (m_count > m_maxCookies)
		{
			num3 = (float)m_maxCookies / (float)m_count;
		}
		lock (m_domainTable.SyncRoot)
		{
			foreach (DictionaryEntry item in m_domainTable)
			{
				PathList pathList;
				if (domain == null)
				{
					string text = (string)item.Key;
					pathList = (PathList)item.Value;
				}
				else
				{
					string text = domain;
					pathList = (PathList)m_domainTable[domain];
				}
				int num4 = 0;
				lock (pathList.SyncRoot)
				{
					foreach (CookieCollection value in pathList.Values)
					{
						num2 = ExpireCollection(value);
						num += num2;
						m_count -= num2;
						num4 += value.Count;
						DateTime dateTime2;
						if (value.Count > 0 && (dateTime2 = value.TimeStamp(CookieCollection.Stamp.Check)) < dateTime)
						{
							cookieCollection = value;
							dateTime = dateTime2;
						}
					}
				}
				int num5 = Math.Min((int)((float)num4 * num3), Math.Min(m_maxCookiesPerDomain, m_maxCookies) - 1);
				if (num4 <= num5)
				{
					continue;
				}
				CookieCollection[] array;
				DateTime[] array2;
				lock (pathList.SyncRoot)
				{
					array = new CookieCollection[pathList.Count];
					array2 = new DateTime[pathList.Count];
					foreach (CookieCollection value2 in pathList.Values)
					{
						array2[num2] = value2.TimeStamp(CookieCollection.Stamp.Check);
						array[num2] = value2;
						num2++;
					}
				}
				Array.Sort(array2, array);
				num2 = 0;
				foreach (CookieCollection cookieCollection4 in array)
				{
					lock (cookieCollection4)
					{
						while (num4 > num5 && cookieCollection4.Count > 0)
						{
							cookieCollection4.RemoveAt(0);
							num4--;
							m_count--;
							num++;
						}
					}
					if (num4 <= num5)
					{
						break;
					}
				}
				if (num4 > num5 && domain != null)
				{
					return false;
				}
			}
		}
		if (domain != null)
		{
			return true;
		}
		if (num != 0)
		{
			return true;
		}
		if (dateTime == DateTime.MaxValue)
		{
			return false;
		}
		lock (cookieCollection)
		{
			while (m_count >= m_maxCookies && cookieCollection.Count > 0)
			{
				cookieCollection.RemoveAt(0);
				m_count--;
			}
		}
		return true;
	}

	private void DomainTableCleanup()
	{
		List<object> list = new List<object>();
		List<string> list2 = new List<string>();
		lock (m_domainTable.SyncRoot)
		{
			IDictionaryEnumerator enumerator = m_domainTable.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string item = (string)enumerator.Key;
				PathList pathList = (PathList)enumerator.Value;
				lock (pathList.SyncRoot)
				{
					IDictionaryEnumerator enumerator2 = pathList.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						if (((CookieCollection)enumerator2.Value).Count == 0)
						{
							list.Add(enumerator2.Key);
						}
					}
					foreach (object item2 in list)
					{
						pathList.Remove(item2);
					}
					list.Clear();
					if (pathList.Count == 0)
					{
						list2.Add(item);
					}
				}
			}
			foreach (string item3 in list2)
			{
				m_domainTable.Remove(item3);
			}
		}
	}

	private static int ExpireCollection(CookieCollection cc)
	{
		lock (cc)
		{
			int count = cc.Count;
			for (int num = count - 1; num >= 0; num--)
			{
				if (cc[num].Expired)
				{
					cc.RemoveAt(num);
				}
			}
			return count - cc.Count;
		}
	}

	public void Add(CookieCollection cookies)
	{
		ArgumentNullException.ThrowIfNull(cookies, "cookies");
		foreach (Cookie item in (IEnumerable<Cookie>)cookies)
		{
			Add(item);
		}
	}

	public void Add(Uri uri, Cookie cookie)
	{
		ArgumentNullException.ThrowIfNull(uri, "uri");
		ArgumentNullException.ThrowIfNull(cookie, "cookie");
		Cookie cookie2 = cookie.Clone();
		cookie2.VerifyAndSetDefaults(cookie2.Variant, uri);
		InternalAdd(cookie2);
	}

	public void Add(Uri uri, CookieCollection cookies)
	{
		ArgumentNullException.ThrowIfNull(uri, "uri");
		ArgumentNullException.ThrowIfNull(cookies, "cookies");
		foreach (Cookie cookie2 in cookies)
		{
			Cookie cookie = cookie2.Clone();
			cookie.VerifyAndSetDefaults(cookie.Variant, uri);
			InternalAdd(cookie);
		}
	}

	internal CookieCollection CookieCutter(Uri uri, string headerName, string setCookieHeader)
	{
		if (NetEventSource.Log.IsEnabled())
		{
			NetEventSource.Info(this, $"uri:{uri} headerName:{headerName} setCookieHeader:{setCookieHeader}", "CookieCutter");
		}
		CookieCollection cookieCollection = new CookieCollection();
		CookieVariant variant = CookieVariant.Unknown;
		if (headerName == null)
		{
			variant = CookieVariant.Rfc2109;
		}
		else
		{
			for (int i = 0; i < s_headerInfo.Length; i++)
			{
				if (string.Equals(headerName, s_headerInfo[i].Name, StringComparison.OrdinalIgnoreCase))
				{
					variant = s_headerInfo[i].Variant;
				}
			}
		}
		try
		{
			CookieParser cookieParser = new CookieParser(setCookieHeader);
			while (true)
			{
				Cookie cookie = cookieParser.Get();
				if (NetEventSource.Log.IsEnabled())
				{
					NetEventSource.Info(this, $"CookieParser returned cookie:{cookie}", "CookieCutter");
				}
				if (cookie == null)
				{
					if (cookieParser.EndofHeader())
					{
						break;
					}
					continue;
				}
				if (string.IsNullOrEmpty(cookie.Name))
				{
					throw new CookieException(System.SR.net_cookie_format);
				}
				cookie.VerifyAndSetDefaults(variant, uri);
				cookieCollection.InternalAdd(cookie, isStrict: true);
			}
		}
		catch (OutOfMemoryException)
		{
			throw;
		}
		catch (Exception innerException)
		{
			throw new CookieException(System.SR.Format(System.SR.net_cookie_parse_header, uri.AbsoluteUri), innerException);
		}
		int count = cookieCollection.Count;
		for (int j = 0; j < count; j++)
		{
			InternalAdd(cookieCollection[j]);
		}
		return cookieCollection;
	}

	public CookieCollection GetCookies(Uri uri)
	{
		ArgumentNullException.ThrowIfNull(uri, "uri");
		return InternalGetCookies(uri) ?? new CookieCollection();
	}

	public CookieCollection GetAllCookies()
	{
		CookieCollection cookieCollection = new CookieCollection();
		lock (m_domainTable.SyncRoot)
		{
			IDictionaryEnumerator enumerator = m_domainTable.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PathList pathList = (PathList)enumerator.Value;
				lock (pathList.SyncRoot)
				{
					IDictionaryEnumerator enumerator2 = pathList.List.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						cookieCollection.Add((CookieCollection)enumerator2.Value);
					}
				}
			}
			return cookieCollection;
		}
	}

	internal CookieCollection InternalGetCookies(Uri uri)
	{
		if (m_count == 0)
		{
			return null;
		}
		bool isSecure = uri.Scheme == "https" || uri.Scheme == "wss";
		int port = uri.Port;
		CookieCollection cookies = null;
		int num = 1;
		List<string> list = new List<string>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<string> span = CollectionsMarshal.AsSpan(list);
		int index = 0;
		span[index] = uri.Host;
		List<string> list2 = list;
		ReadOnlySpan<char> span2 = uri.Host.AsSpan();
		int num2 = span2.LastIndexOf('.');
		while (num2 > 0)
		{
			int num3 = span2.Slice(0, num2).LastIndexOf('.');
			if (num3 > 0)
			{
				index = num3 + 1;
				string item = span2.Slice(index, span2.Length - index).ToString();
				list2.Add(item);
			}
			num2 = num3;
		}
		BuildCookieCollectionFromDomainMatches(uri, isSecure, port, ref cookies, list2);
		return cookies;
	}

	private void BuildCookieCollectionFromDomainMatches(Uri uri, bool isSecure, int port, ref CookieCollection cookies, List<string> matchingDomainKeys)
	{
		for (int i = 0; i < matchingDomainKeys.Count; i++)
		{
			PathList pathList;
			lock (m_domainTable.SyncRoot)
			{
				pathList = (PathList)m_domainTable[matchingDomainKeys[i]];
				if (pathList == null)
				{
					continue;
				}
			}
			lock (pathList.SyncRoot)
			{
				SortedList list = pathList.List;
				int count = list.Count;
				for (int j = 0; j < count; j++)
				{
					string cookiePath = (string)list.GetKey(j);
					if (PathMatch(uri.AbsolutePath, cookiePath))
					{
						CookieCollection cookieCollection = (CookieCollection)list.GetByIndex(j);
						cookieCollection.TimeStamp(CookieCollection.Stamp.Set);
						MergeUpdateCollections(ref cookies, uri.Host, cookieCollection, port, isSecure);
					}
				}
			}
			if (pathList.Count == 0)
			{
				lock (m_domainTable.SyncRoot)
				{
					m_domainTable.Remove(matchingDomainKeys[i]);
				}
			}
		}
	}

	private static bool PathMatch(string requestPath, string cookiePath)
	{
		cookiePath = CookieParser.CheckQuoted(cookiePath);
		if (!requestPath.StartsWith(cookiePath, StringComparison.Ordinal))
		{
			return false;
		}
		if (requestPath.Length != cookiePath.Length && !cookiePath.EndsWith('/'))
		{
			return requestPath[cookiePath.Length] == '/';
		}
		return true;
	}

	private void MergeUpdateCollections(ref CookieCollection destination, string host, CookieCollection source, int port, bool isSecure)
	{
		lock (source)
		{
			for (int i = 0; i < source.Count; i++)
			{
				bool flag = false;
				Cookie cookie = source[i];
				if (cookie.Expired)
				{
					source.RemoveAt(i);
					m_count--;
					i--;
					continue;
				}
				if (cookie.PortList != null)
				{
					int[] portList = cookie.PortList;
					for (int j = 0; j < portList.Length; j++)
					{
						if (portList[j] == port)
						{
							flag = true;
							break;
						}
					}
				}
				else
				{
					flag = true;
				}
				if (cookie.Secure && !isSecure)
				{
					flag = false;
				}
				if (cookie.DomainImplicit && !string.Equals(host, cookie.Domain, StringComparison.OrdinalIgnoreCase))
				{
					flag = false;
				}
				if (flag)
				{
					if (destination == null)
					{
						destination = new CookieCollection();
					}
					destination.InternalAdd(cookie, isStrict: false);
				}
			}
		}
	}

	public string GetCookieHeader(Uri uri)
	{
		ArgumentNullException.ThrowIfNull(uri, "uri");
		string optCookie;
		return GetCookieHeader(uri, out optCookie);
	}

	internal string GetCookieHeader(Uri uri, out string optCookie2)
	{
		CookieCollection cookieCollection = InternalGetCookies(uri);
		if (cookieCollection == null)
		{
			optCookie2 = string.Empty;
			return string.Empty;
		}
		string value = string.Empty;
		StringBuilder stringBuilder = System.Text.StringBuilderCache.Acquire();
		for (int i = 0; i < cookieCollection.Count; i++)
		{
			stringBuilder.Append(value);
			cookieCollection[i].ToString(stringBuilder);
			value = "; ";
		}
		optCookie2 = (cookieCollection.IsOtherVersionSeen ? "$Version=1" : string.Empty);
		return System.Text.StringBuilderCache.GetStringAndRelease(stringBuilder);
	}

	public void SetCookies(Uri uri, string cookieHeader)
	{
		ArgumentNullException.ThrowIfNull(uri, "uri");
		ArgumentNullException.ThrowIfNull(cookieHeader, "cookieHeader");
		CookieCutter(uri, null, cookieHeader);
	}
}
