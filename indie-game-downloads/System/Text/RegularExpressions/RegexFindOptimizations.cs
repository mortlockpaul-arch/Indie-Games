using System.Buffers;
using System.Collections.Generic;

namespace System.Text.RegularExpressions;

internal sealed class RegexFindOptimizations
{
	public struct FixedDistanceSet(char[] chars, string set, int distance)
	{
		public string Set = set;

		public bool Negated = false;

		public char[] Chars = chars;

		public int Distance = distance;

		public (char LowInclusive, char HighInclusive)? Range = null;
	}

	private readonly uint[][] _asciiLookups;

	public bool IsUseful
	{
		get
		{
			if (FindMode == FindNextStartingPositionMode.NoSearch)
			{
				return LeadingAnchor == RegexNodeKind.Bol;
			}
			return true;
		}
	}

	public FindNextStartingPositionMode FindMode { get; } = FindNextStartingPositionMode.NoSearch;

	public RegexNodeKind LeadingAnchor { get; private set; }

	public RegexNodeKind TrailingAnchor { get; }

	public int MinRequiredLength { get; private set; }

	public int? MaxPossibleLength { get; private set; }

	public string LeadingPrefix { get; } = string.Empty;

	public string[] LeadingPrefixes { get; } = Array.Empty<string>();

	public (char Char, string String, int Distance) FixedDistanceLiteral { get; }

	public List<FixedDistanceSet> FixedDistanceSets { get; }

	public SearchValues<string> LeadingStrings { get; }

	public (RegexNode LoopNode, (char Char, string String, StringComparison StringComparison, char[] Chars) Literal)? LiteralAfterLoop { get; }

	public static RegexFindOptimizations Create(RegexNode root, RegexOptions options)
	{
		RegexFindOptimizations regexFindOptimizations = new RegexFindOptimizations(root, options, isLeadingPartial: false);
		if ((options & RegexOptions.RightToLeft) == 0 && !regexFindOptimizations.IsUseful)
		{
			RegexNode regexNode = RegexPrefixAnalyzer.FindLeadingPositiveLookahead(root);
			if (regexNode != null)
			{
				RegexFindOptimizations regexFindOptimizations2 = new RegexFindOptimizations(regexNode.Child(0), options, isLeadingPartial: true);
				regexFindOptimizations2.MinRequiredLength = Math.Max(regexFindOptimizations.MinRequiredLength, regexFindOptimizations2.MinRequiredLength);
				regexFindOptimizations2.MaxPossibleLength = regexFindOptimizations.MaxPossibleLength;
				regexFindOptimizations = regexFindOptimizations2;
			}
		}
		return regexFindOptimizations;
	}

	private RegexFindOptimizations(RegexNode root, RegexOptions options, bool isLeadingPartial)
	{
		bool flag = (options & RegexOptions.RightToLeft) != 0;
		MinRequiredLength = root.ComputeMinLength();
		LeadingAnchor = RegexPrefixAnalyzer.FindLeadingAnchor(root);
		if (flag && LeadingAnchor == RegexNodeKind.Bol)
		{
			LeadingAnchor = RegexNodeKind.Unknown;
		}
		RegexNodeKind leadingAnchor = LeadingAnchor;
		if (leadingAnchor - 18 <= RegexNodeKind.Oneloop)
		{
			FindMode = LeadingAnchor switch
			{
				RegexNodeKind.Beginning => flag ? FindNextStartingPositionMode.LeadingAnchor_RightToLeft_Beginning : FindNextStartingPositionMode.LeadingAnchor_LeftToRight_Beginning, 
				RegexNodeKind.Start => (!flag) ? FindNextStartingPositionMode.LeadingAnchor_LeftToRight_Start : FindNextStartingPositionMode.LeadingAnchor_RightToLeft_Start, 
				RegexNodeKind.End => flag ? FindNextStartingPositionMode.LeadingAnchor_RightToLeft_End : FindNextStartingPositionMode.LeadingAnchor_LeftToRight_End, 
				_ => flag ? FindNextStartingPositionMode.LeadingAnchor_RightToLeft_EndZ : FindNextStartingPositionMode.LeadingAnchor_LeftToRight_EndZ, 
			};
			return;
		}
		if (!flag && !isLeadingPartial)
		{
			TrailingAnchor = RegexPrefixAnalyzer.FindTrailingAnchor(root);
			leadingAnchor = TrailingAnchor;
			if (leadingAnchor - 20 <= (RegexNodeKind)1)
			{
				int? num = root.ComputeMaxLength();
				if (num.HasValue)
				{
					int valueOrDefault = num.GetValueOrDefault();
					MaxPossibleLength = valueOrDefault;
					if (MinRequiredLength == valueOrDefault)
					{
						FindMode = ((TrailingAnchor == RegexNodeKind.End) ? FindNextStartingPositionMode.TrailingAnchor_FixedLength_LeftToRight_End : FindNextStartingPositionMode.TrailingAnchor_FixedLength_LeftToRight_EndZ);
						return;
					}
				}
			}
		}
		string text = RegexPrefixAnalyzer.FindPrefix(root);
		if (text.Length > 1)
		{
			LeadingPrefix = text;
			FindMode = (flag ? FindNextStartingPositionMode.LeadingString_RightToLeft : FindNextStartingPositionMode.LeadingString_LeftToRight);
			return;
		}
		bool flag2 = (options & RegexOptions.NonBacktracking) != 0;
		bool flag3 = (options & RegexOptions.Compiled) != RegexOptions.None && !flag2;
		bool flag4 = !flag3 && !flag2;
		if (flag)
		{
			string text2 = RegexPrefixAnalyzer.FindFirstCharClass(root);
			if (text2 != null)
			{
				Span<char> chars = stackalloc char[5];
				char[] array = null;
				int setChars;
				if (!RegexCharClass.IsNegated(text2) && (setChars = RegexCharClass.GetSetChars(text2, chars)) > 0)
				{
					array = chars.Slice(0, setChars).ToArray();
				}
				if (!flag3 && array != null && array.Length == 1)
				{
					FixedDistanceLiteral = (Char: array[0], String: null, Distance: 0);
					FindMode = FindNextStartingPositionMode.LeadingChar_RightToLeft;
					return;
				}
				FixedDistanceSets = new List<FixedDistanceSet>
				{
					new FixedDistanceSet(array, text2, 0)
				};
				FindMode = FindNextStartingPositionMode.LeadingSet_RightToLeft;
				_asciiLookups = new uint[1][];
			}
			return;
		}
		text = RegexPrefixAnalyzer.FindPrefixOrdinalCaseInsensitive(root);
		if (text != null && text.Length > 1)
		{
			LeadingPrefix = text;
			FindMode = FindNextStartingPositionMode.LeadingString_OrdinalIgnoreCase_LeftToRight;
			return;
		}
		if (!flag4)
		{
			string[] array2 = RegexPrefixAnalyzer.FindPrefixes(root, ignoreCase: true);
			if (array2 != null && array2.Length > 1)
			{
				LeadingPrefixes = array2;
				FindMode = FindNextStartingPositionMode.LeadingStrings_OrdinalIgnoreCase_LeftToRight;
				LeadingStrings = SearchValues.Create(LeadingPrefixes, StringComparison.OrdinalIgnoreCase);
				return;
			}
		}
		List<FixedDistanceSet> list = RegexPrefixAnalyzer.FindFixedDistanceSets(root, !flag4);
		if (list != null)
		{
			(string, int)? tuple = FindFixedDistanceString(list);
			if (tuple.HasValue)
			{
				(string, int) valueOrDefault2 = tuple.GetValueOrDefault();
				if (valueOrDefault2.Item1 != null)
				{
					FindMode = FindNextStartingPositionMode.FixedDistanceString_LeftToRight;
					FixedDistanceLiteral = (Char: '\0', String: valueOrDefault2.Item1, Distance: valueOrDefault2.Item2);
					return;
				}
			}
		}
		(RegexNode, (char, string, StringComparison, char[]))? tuple2 = RegexPrefixAnalyzer.FindLiteralFollowingLeadingLoop(root);
		if (list != null)
		{
			RegexPrefixAnalyzer.SortFixedDistanceSetsByQuality(list);
			if (!tuple2.HasValue || (list[0].Chars != null && !list[0].Negated))
			{
				if (!flag3 && list.Count == 1)
				{
					char[] chars2 = list[0].Chars;
					if (chars2 != null && chars2.Length == 1 && !list[0].Negated)
					{
						FixedDistanceLiteral = (Char: list[0].Chars[0], String: null, Distance: list[0].Distance);
						FindMode = FindNextStartingPositionMode.FixedDistanceChar_LeftToRight;
						return;
					}
				}
				if (list.Count > 3)
				{
					list.RemoveRange(3, list.Count - 3);
				}
				FixedDistanceSets = list;
				FindMode = ((list.Count == 1 && list[0].Distance == 0) ? FindNextStartingPositionMode.LeadingSet_LeftToRight : FindNextStartingPositionMode.FixedDistanceSets_LeftToRight);
				_asciiLookups = new uint[list.Count][];
				return;
			}
		}
		if (tuple2.HasValue)
		{
			FindMode = FindNextStartingPositionMode.LiteralAfterLoop_LeftToRight;
			LiteralAfterLoop = tuple2;
			_asciiLookups = new uint[1][];
		}
	}

	private static (string String, int Distance)? FindFixedDistanceString(List<FixedDistanceSet> fixedDistanceSets)
	{
		(string, int)? result = null;
		if (fixedDistanceSets.Count >= 2)
		{
			fixedDistanceSets.Sort((FixedDistanceSet s1, FixedDistanceSet s2) => s1.Distance.CompareTo(s2.Distance));
			Span<char> initialBuffer = stackalloc char[64];
			System.Text.ValueStringBuilder valueStringBuilder = new System.Text.ValueStringBuilder(initialBuffer);
			int num = -1;
			for (int num2 = 0; num2 < fixedDistanceSets.Count + 1; num2++)
			{
				char[] array = ((num2 < fixedDistanceSets.Count) ? fixedDistanceSets[num2].Chars : null);
				bool flag = array == null || array.Length != 1 || fixedDistanceSets[num2].Negated;
				if (flag || (num2 > 0 && fixedDistanceSets[num2].Distance != fixedDistanceSets[num2 - 1].Distance + 1))
				{
					if (num != -1 && num2 - num >= ((!result.HasValue) ? 2 : result.Value.Item1.Length))
					{
						result = (valueStringBuilder.ToString(), fixedDistanceSets[num].Distance);
					}
					valueStringBuilder = new System.Text.ValueStringBuilder(initialBuffer);
					num = -1;
					if (flag)
					{
						continue;
					}
				}
				if (num == -1)
				{
					num = num2;
				}
				valueStringBuilder.Append(array[0]);
			}
			valueStringBuilder.Dispose();
		}
		return result;
	}

	public bool TryFindNextStartingPositionRightToLeft(ReadOnlySpan<char> textSpan, ref int pos, int start)
	{
		if (pos < MinRequiredLength)
		{
			pos = 0;
			return false;
		}
		switch (FindMode)
		{
		case FindNextStartingPositionMode.LeadingAnchor_RightToLeft_Beginning:
			if (pos != 0)
			{
				pos = 0;
			}
			return true;
		case FindNextStartingPositionMode.LeadingAnchor_RightToLeft_Start:
			if (pos != start)
			{
				pos = 0;
				return false;
			}
			return true;
		case FindNextStartingPositionMode.LeadingAnchor_RightToLeft_EndZ:
			if (pos < textSpan.Length - 1 || ((uint)pos < (uint)textSpan.Length && textSpan[pos] != '\n'))
			{
				pos = 0;
				return false;
			}
			return true;
		case FindNextStartingPositionMode.LeadingAnchor_RightToLeft_End:
			if (pos < textSpan.Length)
			{
				pos = 0;
				return false;
			}
			return true;
		case FindNextStartingPositionMode.LeadingString_RightToLeft:
		{
			int num3 = textSpan.Slice(0, pos).LastIndexOf(LeadingPrefix.AsSpan());
			if (num3 >= 0)
			{
				pos = num3 + LeadingPrefix.Length;
				return true;
			}
			pos = 0;
			return false;
		}
		case FindNextStartingPositionMode.LeadingChar_RightToLeft:
		{
			int num2 = textSpan.Slice(0, pos).LastIndexOf(FixedDistanceLiteral.Char);
			if (num2 >= 0)
			{
				pos = num2 + 1;
				return true;
			}
			pos = 0;
			return false;
		}
		case FindNextStartingPositionMode.LeadingSet_RightToLeft:
		{
			ref uint[] asciiLazyCache = ref _asciiLookups[0];
			string set = FixedDistanceSets[0].Set;
			ReadOnlySpan<char> readOnlySpan = textSpan.Slice(0, pos);
			for (int num = readOnlySpan.Length - 1; num >= 0; num--)
			{
				if (RegexCharClass.CharInClass(readOnlySpan[num], set, ref asciiLazyCache))
				{
					pos = num + 1;
					return true;
				}
			}
			pos = 0;
			return false;
		}
		default:
			return true;
		}
	}

	public bool TryFindNextStartingPositionLeftToRight(ReadOnlySpan<char> textSpan, ref int pos, int start)
	{
		if (pos > textSpan.Length - MinRequiredLength)
		{
			pos = textSpan.Length;
			return false;
		}
		if (LeadingAnchor == RegexNodeKind.Bol)
		{
			int num = pos - 1;
			if ((uint)num < (uint)textSpan.Length && textSpan[num] != '\n')
			{
				int num2 = textSpan.Slice(pos).IndexOf('\n');
				if ((uint)num2 > textSpan.Length - 1 - pos)
				{
					pos = textSpan.Length;
					return false;
				}
				pos = num2 + 1 + pos;
				if (pos > textSpan.Length - MinRequiredLength)
				{
					pos = textSpan.Length;
					return false;
				}
			}
		}
		switch (FindMode)
		{
		case FindNextStartingPositionMode.LeadingAnchor_LeftToRight_Beginning:
			if (pos == 0)
			{
				return true;
			}
			pos = textSpan.Length;
			return false;
		case FindNextStartingPositionMode.LeadingAnchor_LeftToRight_Start:
			if (pos == start)
			{
				return true;
			}
			pos = textSpan.Length;
			return false;
		case FindNextStartingPositionMode.LeadingAnchor_LeftToRight_EndZ:
			if (pos < textSpan.Length - 1)
			{
				pos = textSpan.Length - 1;
			}
			return true;
		case FindNextStartingPositionMode.LeadingAnchor_LeftToRight_End:
			if (pos < textSpan.Length)
			{
				pos = textSpan.Length;
			}
			return true;
		case FindNextStartingPositionMode.TrailingAnchor_FixedLength_LeftToRight_EndZ:
			if (pos < textSpan.Length - MinRequiredLength - 1)
			{
				pos = textSpan.Length - MinRequiredLength - 1;
			}
			return true;
		case FindNextStartingPositionMode.TrailingAnchor_FixedLength_LeftToRight_End:
			if (pos < textSpan.Length - MinRequiredLength)
			{
				pos = textSpan.Length - MinRequiredLength;
			}
			return true;
		case FindNextStartingPositionMode.LeadingString_LeftToRight:
		{
			int num15 = textSpan.Slice(pos).IndexOf(LeadingPrefix.AsSpan());
			if (num15 >= 0)
			{
				pos += num15;
				return true;
			}
			pos = textSpan.Length;
			return false;
		}
		case FindNextStartingPositionMode.LeadingString_OrdinalIgnoreCase_LeftToRight:
		{
			int num6 = textSpan.Slice(pos).IndexOf(LeadingPrefix.AsSpan(), StringComparison.OrdinalIgnoreCase);
			if (num6 >= 0)
			{
				pos += num6;
				return true;
			}
			pos = textSpan.Length;
			return false;
		}
		case FindNextStartingPositionMode.LeadingSet_LeftToRight:
		{
			FixedDistanceSet fixedDistanceSet4 = FixedDistanceSets[0];
			ReadOnlySpan<char> span3 = textSpan.Slice(pos);
			char[] chars2 = fixedDistanceSet4.Chars;
			if (chars2 != null && chars2.Length <= 5)
			{
				int num13 = (fixedDistanceSet4.Negated ? span3.IndexOfAnyExcept(chars2) : span3.IndexOfAny(chars2));
				if (num13 >= 0)
				{
					pos += num13;
					return true;
				}
			}
			else
			{
				(char, char)? range = fixedDistanceSet4.Range;
				if (range.HasValue)
				{
					(char LowInclusive, char HighInclusive) valueOrDefault2 = fixedDistanceSet4.Range.GetValueOrDefault();
					char item3 = valueOrDefault2.LowInclusive;
					char item4 = valueOrDefault2.HighInclusive;
					int num14 = (fixedDistanceSet4.Negated ? span3.IndexOfAnyExceptInRange(item3, item4) : span3.IndexOfAnyInRange(item3, item4));
					if (num14 >= 0)
					{
						pos += num14;
						return true;
					}
				}
				else
				{
					ref uint[] asciiLazyCache2 = ref _asciiLookups[0];
					for (int j = 0; j < span3.Length; j++)
					{
						if (RegexCharClass.CharInClass(span3[j], fixedDistanceSet4.Set, ref asciiLazyCache2))
						{
							pos += j;
							return true;
						}
					}
				}
			}
			pos = textSpan.Length;
			return false;
		}
		case FindNextStartingPositionMode.FixedDistanceChar_LeftToRight:
		{
			int num16 = textSpan.Slice(pos + FixedDistanceLiteral.Distance).IndexOf(FixedDistanceLiteral.Char);
			if (num16 >= 0)
			{
				pos += num16;
				return true;
			}
			pos = textSpan.Length;
			return false;
		}
		case FindNextStartingPositionMode.FixedDistanceString_LeftToRight:
		{
			int num18 = textSpan.Slice(pos + FixedDistanceLiteral.Distance).IndexOf(FixedDistanceLiteral.String.AsSpan());
			if (num18 >= 0)
			{
				pos += num18;
				return true;
			}
			pos = textSpan.Length;
			return false;
		}
		case FindNextStartingPositionMode.LeadingStrings_LeftToRight:
		case FindNextStartingPositionMode.LeadingStrings_OrdinalIgnoreCase_LeftToRight:
		{
			int num17 = textSpan.Slice(pos).IndexOfAny(LeadingStrings);
			if (num17 >= 0)
			{
				pos += num17;
				return true;
			}
			pos = textSpan.Length;
			return false;
		}
		case FindNextStartingPositionMode.FixedDistanceSets_LeftToRight:
		{
			List<FixedDistanceSet> fixedDistanceSets = FixedDistanceSets;
			FixedDistanceSet fixedDistanceSet = fixedDistanceSets[0];
			int num7 = textSpan.Length - Math.Max(1, MinRequiredLength);
			char[] chars = fixedDistanceSet.Chars;
			if (chars != null && chars.Length <= 5)
			{
				int num8;
				for (num8 = pos; num8 <= num7; num8++)
				{
					int num9 = num8 + fixedDistanceSet.Distance;
					ReadOnlySpan<char> span2 = textSpan.Slice(num9);
					int num10 = (fixedDistanceSet.Negated ? span2.IndexOfAnyExcept(fixedDistanceSet.Chars) : span2.IndexOfAny(fixedDistanceSet.Chars));
					if (num10 < 0)
					{
						break;
					}
					num10 += num9;
					num8 = num10 - fixedDistanceSet.Distance;
					if (num8 > num7)
					{
						break;
					}
					int num11 = 1;
					while (true)
					{
						if (num11 < fixedDistanceSets.Count)
						{
							FixedDistanceSet fixedDistanceSet2 = fixedDistanceSets[num11];
							if (!RegexCharClass.CharInClass(textSpan[num8 + fixedDistanceSet2.Distance], fixedDistanceSet2.Set, ref _asciiLookups[num11]))
							{
								break;
							}
							num11++;
							continue;
						}
						pos = num8;
						return true;
					}
				}
			}
			else
			{
				ref uint[] asciiLazyCache = ref _asciiLookups[0];
				for (int i = pos; i <= num7; i++)
				{
					if (!RegexCharClass.CharInClass(textSpan[i + fixedDistanceSet.Distance], fixedDistanceSet.Set, ref asciiLazyCache))
					{
						continue;
					}
					int num12 = 1;
					while (true)
					{
						if (num12 < fixedDistanceSets.Count)
						{
							FixedDistanceSet fixedDistanceSet3 = fixedDistanceSets[num12];
							if (!RegexCharClass.CharInClass(textSpan[i + fixedDistanceSet3.Distance], fixedDistanceSet3.Set, ref _asciiLookups[num12]))
							{
								break;
							}
							num12++;
							continue;
						}
						pos = i;
						return true;
					}
				}
			}
			pos = textSpan.Length;
			return false;
		}
		case FindNextStartingPositionMode.LiteralAfterLoop_LeftToRight:
		{
			(RegexNode LoopNode, (char Char, string String, StringComparison StringComparison, char[] Chars) Literal) valueOrDefault = LiteralAfterLoop.GetValueOrDefault();
			RegexNode item = valueOrDefault.LoopNode;
			(char, string, StringComparison, char[]) item2 = valueOrDefault.Literal;
			int num3 = pos;
			while (true)
			{
				ReadOnlySpan<char> span = textSpan.Slice(num3);
				int num4 = ((item2.Item2 != null) ? span.IndexOf(item2.Item2.AsSpan(), item2.Item3) : ((item2.Item4 != null) ? span.IndexOfAny(item2.Item4.AsSpan()) : span.IndexOf(item2.Item1)));
				if (num4 < 0)
				{
					break;
				}
				int num5 = num4;
				while ((uint)(--num5) < (uint)span.Length && RegexCharClass.CharInClass(span[num5], item.Str, ref _asciiLookups[0]))
				{
				}
				if (num4 - num5 - 1 < item.M)
				{
					num3 += num4 + 1;
					continue;
				}
				pos = num3 + num5 + 1;
				return true;
			}
			pos = textSpan.Length;
			return false;
		}
		default:
			return true;
		}
	}
}
