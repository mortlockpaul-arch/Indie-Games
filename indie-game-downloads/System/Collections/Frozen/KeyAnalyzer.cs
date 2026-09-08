using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Frozen;

internal static class KeyAnalyzer
{
	private delegate ReadOnlySpan<char> GetSpan(string s, int index, int count);

	internal readonly struct AnalysisResults(bool ignoreCase, bool allAsciiIfIgnoreCase, int hashIndex, int hashCount, int minLength, int maxLength)
	{
		public bool IgnoreCase { get; } = ignoreCase;

		public bool AllAsciiIfIgnoreCase { get; } = allAsciiIfIgnoreCase;

		public int HashIndex { get; } = hashIndex;

		public int HashCount { get; } = hashCount;

		public int MinimumLength { get; } = minLength;

		public int MaximumLengthDiff { get; } = maxLength - minLength;

		public bool SubstringHashing => HashCount != 0;

		public bool RightJustifiedSubstring => HashIndex < 0;
	}

	private abstract class SubstringComparer : IEqualityComparer<string>
	{
		public int Index;

		public int Count;

		public bool IsLeft;

		public abstract bool Equals(string x, string y);

		public abstract int GetHashCode(string s);
	}

	private sealed class JustifiedSubstringComparer : SubstringComparer
	{
		public override bool Equals(string x, string y)
		{
			return x.AsSpan(IsLeft ? Index : (x.Length + Index), Count).SequenceEqual(y.AsSpan(IsLeft ? Index : (y.Length + Index), Count));
		}

		public override int GetHashCode(string s)
		{
			return Hashing.GetHashCodeOrdinal(s.AsSpan(IsLeft ? Index : (s.Length + Index), Count));
		}
	}

	private sealed class JustifiedCaseInsensitiveSubstringComparer : SubstringComparer
	{
		public override bool Equals(string x, string y)
		{
			return x.AsSpan(IsLeft ? Index : (x.Length + Index), Count).Equals(y.AsSpan(IsLeft ? Index : (y.Length + Index), Count), StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode(string s)
		{
			return Hashing.GetHashCodeOrdinalIgnoreCase(s.AsSpan(IsLeft ? Index : (s.Length + Index), Count));
		}
	}

	private sealed class JustifiedCaseInsensitiveAsciiSubstringComparer : SubstringComparer
	{
		public override bool Equals(string x, string y)
		{
			return x.AsSpan(IsLeft ? Index : (x.Length + Index), Count).Equals(y.AsSpan(IsLeft ? Index : (y.Length + Index), Count), StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode(string s)
		{
			return Hashing.GetHashCodeOrdinalIgnoreCaseAscii(s.AsSpan(IsLeft ? Index : (s.Length + Index), Count));
		}
	}

	private static readonly SearchValues<char> s_asciiLetters = SearchValues.Create("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".AsSpan());

	public static AnalysisResults Analyze(ReadOnlySpan<string> uniqueStrings, bool ignoreCase, int minLength, int maxLength)
	{
		bool allUniqueStringsAreConfirmedAscii = ignoreCase && AreAllAscii(uniqueStrings);
		if (minLength == 0 || !TryUseSubstring(uniqueStrings, allUniqueStringsAreConfirmedAscii, ignoreCase, minLength, maxLength, out var results))
		{
			return CreateAnalysisResults(uniqueStrings, allUniqueStringsAreConfirmedAscii, ignoreCase, minLength, maxLength, 0, 0, (string s, int _, int _) => s.AsSpan());
		}
		return results;
	}

	private static bool TryUseSubstring(ReadOnlySpan<string> uniqueStrings, bool allUniqueStringsAreConfirmedAscii, bool ignoreCase, int minLength, int maxLength, out AnalysisResults results)
	{
		int acceptableNonUniqueCount = uniqueStrings.Length / 20;
		SubstringComparer substringComparer = ((!ignoreCase) ? new JustifiedSubstringComparer() : (allUniqueStringsAreConfirmedAscii ? ((SubstringComparer)new JustifiedCaseInsensitiveAsciiSubstringComparer()) : ((SubstringComparer)new JustifiedCaseInsensitiveSubstringComparer())));
		HashSet<string> set = new HashSet<string>(uniqueStrings.Length, substringComparer);
		int num = Math.Min(minLength, 8);
		for (int i = 1; i <= num; i++)
		{
			substringComparer.IsLeft = true;
			substringComparer.Count = i;
			for (int j = 0; j <= minLength - i; j++)
			{
				substringComparer.Index = j;
				if (HasSufficientUniquenessFactor(set, uniqueStrings, acceptableNonUniqueCount))
				{
					results = CreateAnalysisResults(uniqueStrings, allUniqueStringsAreConfirmedAscii, ignoreCase, minLength, maxLength, j, i, (string s, int index, int count) => s.AsSpan(index, count));
					return true;
				}
			}
			if (minLength == maxLength)
			{
				continue;
			}
			substringComparer.IsLeft = false;
			for (int num2 = 0; num2 <= minLength - i; num2++)
			{
				substringComparer.Index = -num2 - i;
				if (HasSufficientUniquenessFactor(set, uniqueStrings, acceptableNonUniqueCount))
				{
					results = CreateAnalysisResults(uniqueStrings, allUniqueStringsAreConfirmedAscii, ignoreCase, minLength, maxLength, substringComparer.Index, i, (string s, int index, int count) => s.AsSpan(s.Length + index, count));
					return true;
				}
			}
		}
		results = default(AnalysisResults);
		return false;
	}

	private static AnalysisResults CreateAnalysisResults(ReadOnlySpan<string> uniqueStrings, bool allUniqueStringsAreConfirmedAscii, bool ignoreCase, int minLength, int maxLength, int index, int count, GetSpan getHashString)
	{
		bool allAsciiIfIgnoreCase = true;
		if (ignoreCase)
		{
			bool flag = true;
			ReadOnlySpan<string> readOnlySpan = uniqueStrings;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				string text = readOnlySpan[i];
				if (!allUniqueStringsAreConfirmedAscii && !IsAllAscii(getHashString(text, index, count)))
				{
					allAsciiIfIgnoreCase = false;
					flag = false;
					break;
				}
				if (flag && ((count > 0 && !allUniqueStringsAreConfirmedAscii && !IsAllAscii(text.AsSpan())) || ContainsAnyAsciiLetters(text.AsSpan())))
				{
					flag = false;
					if (allUniqueStringsAreConfirmedAscii)
					{
						break;
					}
				}
			}
			if (flag)
			{
				ignoreCase = false;
			}
		}
		return new AnalysisResults(ignoreCase, allAsciiIfIgnoreCase, index, count, minLength, maxLength);
	}

	private static bool AreAllAscii(ReadOnlySpan<string> strings)
	{
		ReadOnlySpan<string> readOnlySpan = strings;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			if (!IsAllAscii(readOnlySpan[i].AsSpan()))
			{
				return false;
			}
		}
		return true;
	}

	internal static bool IsAllAscii(ReadOnlySpan<char> s)
	{
		return Ascii.IsValid(s);
	}

	internal static bool ContainsAnyAsciiLetters(ReadOnlySpan<char> s)
	{
		return s.ContainsAny(s_asciiLetters);
	}

	internal static bool HasSufficientUniquenessFactor(HashSet<string> set, ReadOnlySpan<string> uniqueStrings, int acceptableNonUniqueCount)
	{
		set.Clear();
		ReadOnlySpan<string> readOnlySpan = uniqueStrings;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			string item = readOnlySpan[i];
			if (!set.Add(item) && --acceptableNonUniqueCount < 0)
			{
				return false;
			}
		}
		return true;
	}
}
