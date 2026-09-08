using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;

namespace System.Text.RegularExpressions;

internal sealed class RegexCache
{
	internal readonly struct Key(string pattern, string culture, RegexOptions options, TimeSpan matchTimeout) : IEquatable<Key>
	{
		private readonly string _pattern = pattern;

		private readonly string _culture = culture;

		private readonly RegexOptions _options = options;

		private readonly TimeSpan _matchTimeout = matchTimeout;

		public override bool Equals([NotNullWhen(true)] object obj)
		{
			if (obj is Key other)
			{
				return Equals(other);
			}
			return false;
		}

		public bool Equals(Key other)
		{
			if (_pattern.Equals(other._pattern) && _culture.Equals(other._culture) && _options == other._options)
			{
				return _matchTimeout == other._matchTimeout;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return _pattern.GetHashCode() ^ (int)_options;
		}
	}

	private sealed class Node(Key key, Regex regex)
	{
		public readonly Key Key = key;

		public readonly Regex Regex = regex;

		public long LastAccessStamp;
	}

	private static volatile Node s_lastAccessed;

	private static readonly ConcurrentDictionary<Key, Node> s_cacheDictionary = new ConcurrentDictionary<Key, Node>(1, 31);

	private static readonly List<Node> s_cacheList = new List<Node>(15);

	private static readonly Random s_random = new Random();

	private static int s_maxCacheSize = 15;

	private static object SyncObj => s_cacheDictionary;

	public static int MaxCacheSize
	{
		get
		{
			lock (SyncObj)
			{
				return s_maxCacheSize;
			}
		}
		set
		{
			lock (SyncObj)
			{
				s_maxCacheSize = value;
				if (value == 0)
				{
					s_cacheDictionary.Clear();
					s_cacheList.Clear();
					s_lastAccessed = null;
				}
				else if (value < s_cacheList.Count)
				{
					s_lastAccessed = s_cacheList[0];
					for (int i = value; i < s_cacheList.Count; i++)
					{
						s_cacheDictionary.TryRemove(s_cacheList[i].Key, out var _);
					}
					s_cacheList.RemoveRange(value, s_cacheList.Count - value);
				}
			}
		}
	}

	public static Regex GetOrAdd(string pattern)
	{
		Regex.ValidatePattern(pattern);
		CultureInfo currentCulture = CultureInfo.CurrentCulture;
		Key key = new Key(pattern, currentCulture.ToString(), RegexOptions.None, Regex.s_defaultMatchTimeout);
		Regex regex = Get(key);
		if (regex == null)
		{
			regex = new Regex(pattern, currentCulture);
			Add(key, regex);
		}
		return regex;
	}

	public static Regex GetOrAdd(string pattern, RegexOptions options, TimeSpan matchTimeout)
	{
		Regex.ValidatePattern(pattern);
		Regex.ValidateOptions(options);
		Regex.ValidateMatchTimeout(matchTimeout);
		CultureInfo targetCulture = RegexParser.GetTargetCulture(options);
		Key key = new Key(pattern, targetCulture.ToString(), options, matchTimeout);
		Regex regex = Get(key);
		if (regex == null)
		{
			regex = new Regex(pattern, options, matchTimeout, targetCulture);
			Add(key, regex);
		}
		return regex;
	}

	private static Regex Get(Key key)
	{
		long num = 0L;
		Node node = s_lastAccessed;
		if (node != null)
		{
			if (key.Equals(node.Key))
			{
				return node.Regex;
			}
			num = Volatile.Read(in node.LastAccessStamp);
		}
		if (s_maxCacheSize != 0 && s_cacheDictionary.TryGetValue(key, out var value))
		{
			Volatile.Write(ref value.LastAccessStamp, num + 1);
			s_lastAccessed = value;
			return value.Regex;
		}
		return null;
	}

	private static void Add(Key key, Regex regex)
	{
		lock (SyncObj)
		{
			if (s_maxCacheSize == 0 || s_cacheDictionary.TryGetValue(key, out var value))
			{
				return;
			}
			if (s_cacheList.Count == s_maxCacheSize)
			{
				int num;
				bool flag;
				if (s_maxCacheSize <= 30)
				{
					num = s_cacheList.Count;
					flag = false;
				}
				else
				{
					num = 30;
					flag = true;
				}
				int num2 = (flag ? s_random.Next(s_cacheList.Count) : 0);
				long num3 = Volatile.Read(in s_cacheList[num2].LastAccessStamp);
				for (int i = 1; i < num; i++)
				{
					int num4 = (flag ? s_random.Next(s_cacheList.Count) : i);
					long num5 = Volatile.Read(in s_cacheList[num4].LastAccessStamp);
					if (num5 < num3)
					{
						num2 = num4;
						num3 = num5;
					}
				}
				s_cacheDictionary.TryRemove(s_cacheList[num2].Key, out value);
				List<Node> list = s_cacheList;
				int index = num2;
				List<Node> list2 = s_cacheList;
				list[index] = list2[list2.Count - 1];
				s_cacheList.RemoveAt(s_cacheList.Count - 1);
			}
			Node node = new Node(key, regex);
			Node node2 = s_lastAccessed;
			if (node2 != null)
			{
				node.LastAccessStamp = Volatile.Read(in node2.LastAccessStamp) + 1;
			}
			s_lastAccessed = node;
			s_cacheList.Add(node);
			s_cacheDictionary.TryAdd(key, node);
		}
	}
}
