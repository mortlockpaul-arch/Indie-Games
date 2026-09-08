using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Text.RegularExpressions;

internal static class RegexPrefixAnalyzer
{
	private static ReadOnlySpan<float> Frequency => new float[128]
	{
		0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0.001f,
		0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
		0.003f, 0f, 0f, 0f, 0f, 0.004f, 0f, 0f, 0.006f, 0.006f,
		0f, 0f, 8.952f, 0.065f, 0.42f, 0.01f, 0.011f, 0.005f, 0.07f, 0.05f,
		3.911f, 3.91f, 0.356f, 2.775f, 1.411f, 0.173f, 2.054f, 0.677f, 1.199f, 0.87f,
		0.729f, 0.491f, 0.335f, 0.269f, 0.435f, 0.24f, 0.234f, 0.196f, 0.144f, 0.983f,
		0.357f, 0.661f, 0.371f, 0.088f, 0.007f, 0.763f, 0.229f, 0.551f, 0.306f, 0.449f,
		0.337f, 0.162f, 0.131f, 0.489f, 0.031f, 0.035f, 0.301f, 0.205f, 0.253f, 0.228f,
		0.288f, 0.034f, 0.38f, 0.73f, 0.675f, 0.265f, 0.309f, 0.137f, 0.084f, 0.023f,
		0.023f, 0.591f, 0.085f, 0.59f, 0.013f, 0.797f, 0.001f, 4.596f, 1.296f, 2.081f,
		2.005f, 6.903f, 1.494f, 1.019f, 1.024f, 3.75f, 0.286f, 0.439f, 2.913f, 1.459f,
		3.908f, 3.23f, 1.444f, 0.231f, 4.22f, 3.924f, 5.312f, 2.112f, 0.737f, 0.573f,
		0.992f, 1.067f, 0.181f, 0.391f, 0.056f, 0.391f, 0.002f, 0f
	};

	public static string[] FindPrefixes(RegexNode node, bool ignoreCase)
	{
		int num = 1;
		List<StringBuilder> list = new List<StringBuilder>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<StringBuilder> span = CollectionsMarshal.AsSpan(list);
		int index = 0;
		span[index] = new StringBuilder();
		List<StringBuilder> list2 = list;
		FindPrefixesCore(node, list2, ignoreCase);
		if (list2.Count > 16 || !list2.TrueForAll((StringBuilder sb) => sb.Length >= 2))
		{
			return null;
		}
		string[] array = new string[list2.Count];
		for (int num2 = 0; num2 < list2.Count; num2++)
		{
			array[num2] = list2[num2].ToString();
		}
		return array;
		static bool FindPrefixesCore(RegexNode regexNode, List<StringBuilder> results, bool flag)
		{
			if (!StackHelper.TryEnsureSufficientExecutionStack() || !results.TrueForAll((StringBuilder sb) => sb.Length < 8) || (regexNode.Options & RegexOptions.RightToLeft) != RegexOptions.None || results.Count > 16)
			{
				return false;
			}
			Span<char> span2 = stackalloc char[16];
			while (true)
			{
				switch (regexNode.Kind)
				{
				case RegexNodeKind.Capture:
				case RegexNodeKind.Atomic:
					goto IL_0116;
				case RegexNodeKind.Bol:
				case RegexNodeKind.Eol:
				case RegexNodeKind.Boundary:
				case RegexNodeKind.NonBoundary:
				case RegexNodeKind.Beginning:
				case RegexNodeKind.Start:
				case RegexNodeKind.EndZ:
				case RegexNodeKind.End:
				case RegexNodeKind.Empty:
				case RegexNodeKind.PositiveLookaround:
				case RegexNodeKind.NegativeLookaround:
				case RegexNodeKind.ECMABoundary:
				case RegexNodeKind.NonECMABoundary:
				case RegexNodeKind.UpdateBumpalong:
					return true;
				case RegexNodeKind.Oneloop:
				case RegexNodeKind.Onelazy:
				case RegexNodeKind.One:
				case RegexNodeKind.Oneloopatomic:
					if (!flag || !RegexCharClass.ParticipatesInCaseConversion(regexNode.Ch))
					{
						int num13 = ((regexNode.Kind == RegexNodeKind.One) ? 1 : Math.Min(regexNode.M, 8));
						foreach (StringBuilder result in results)
						{
							result.Append(regexNode.Ch, num13);
						}
						if (regexNode.Kind != RegexNodeKind.One)
						{
							return num13 == regexNode.N;
						}
						return true;
					}
					break;
				case RegexNodeKind.Multi:
					if (!flag)
					{
						foreach (StringBuilder result2 in results)
						{
							result2.Append(regexNode.Str);
						}
					}
					else
					{
						string str = regexNode.Str;
						foreach (char c in str)
						{
							if (RegexCharClass.ParticipatesInCaseConversion(c))
							{
								return false;
							}
							foreach (StringBuilder result3 in results)
							{
								result3.Append(c);
							}
						}
					}
					return true;
				case RegexNodeKind.Setloop:
				case RegexNodeKind.Setlazy:
				case RegexNodeKind.Set:
				case RegexNodeKind.Setloopatomic:
					if (!RegexCharClass.IsNegated(regexNode.Str))
					{
						int setChars = RegexCharClass.GetSetChars(regexNode.Str, span2);
						if (setChars == 0)
						{
							return false;
						}
						int num14 = ((regexNode.Kind == RegexNodeKind.Set) ? 1 : Math.Min(regexNode.M, 8));
						if (!flag)
						{
							for (int num15 = 0; num15 < num14; num15++)
							{
								int count2 = results.Count;
								if (count2 * setChars > 16)
								{
									return false;
								}
								Span<char> span5 = span2.Slice(1, setChars - 1);
								for (int num4 = 0; num4 < span5.Length; num4++)
								{
									char value2 = span5[num4];
									for (int num16 = 0; num16 < count2; num16++)
									{
										StringBuilder stringBuilder2 = new StringBuilder().Append(results[num16]);
										stringBuilder2.Append(value2);
										results.Add(stringBuilder2);
									}
								}
								for (int num17 = 0; num17 < count2; num17++)
								{
									results[num17].Append(span2[0]);
								}
							}
						}
						else
						{
							if (!RegexCharClass.SetContainsAsciiOrdinalIgnoreCaseCharacter(regexNode.Str, span2))
							{
								return false;
							}
							foreach (StringBuilder result4 in results)
							{
								result4.Append(span2[1], num14);
							}
						}
						if (regexNode.Kind != RegexNodeKind.Set)
						{
							return num14 == regexNode.N;
						}
						return true;
					}
					break;
				case RegexNodeKind.Concatenate:
				{
					int num11 = regexNode.ChildCount();
					for (int num12 = 0; num12 < num11; num12++)
					{
						if (!FindPrefixesCore(regexNode.Child(num12), results, flag))
						{
							return false;
						}
					}
					return true;
				}
				case RegexNodeKind.Loop:
				case RegexNodeKind.Lazyloop:
					if (regexNode.M > 0)
					{
						int num9 = Math.Min(regexNode.M, 8);
						for (int num10 = 0; num10 < num9; num10++)
						{
							if (!FindPrefixesCore(regexNode.Child(0), results, flag))
							{
								return false;
							}
						}
						return num9 == regexNode.N;
					}
					break;
				case RegexNodeKind.Alternate:
				{
					int num3 = regexNode.ChildCount();
					if (num3 > 16)
					{
						return false;
					}
					List<StringBuilder> list3 = null;
					int num4 = 1;
					List<StringBuilder> list4 = new List<StringBuilder>(num4);
					CollectionsMarshal.SetCount(list4, num4);
					Span<StringBuilder> span3 = CollectionsMarshal.AsSpan(list4);
					int index2 = 0;
					span3[index2] = new StringBuilder();
					List<StringBuilder> list5 = list4;
					for (int num5 = 0; num5 < num3; num5++)
					{
						FindPrefixesCore(regexNode.Child(num5), list5, flag);
						if ((list3?.Count ?? 0) + list5.Count > 16)
						{
							return false;
						}
						foreach (StringBuilder item in list5)
						{
							if (item.Length == 0)
							{
								return false;
							}
						}
						if (list3 == null)
						{
							list3 = list5;
							index2 = 1;
							List<StringBuilder> list6 = new List<StringBuilder>(index2);
							CollectionsMarshal.SetCount(list6, index2);
							Span<StringBuilder> span4 = CollectionsMarshal.AsSpan(list6);
							num4 = 0;
							span4[num4] = new StringBuilder();
							list5 = list6;
						}
						else
						{
							list3.AddRange(list5);
							list5.Clear();
							list5.Add(new StringBuilder());
						}
					}
					if (results.Count == 1 && results[0].Length == 0)
					{
						results.Clear();
						results.AddRange(list3);
					}
					else
					{
						int count = results.Count;
						for (int num6 = 1; num6 < list3.Count; num6++)
						{
							StringBuilder value = list3[num6];
							for (int num7 = 0; num7 < count; num7++)
							{
								StringBuilder stringBuilder = new StringBuilder().Append(results[num7]);
								stringBuilder.Append(value);
								results.Add(stringBuilder);
							}
						}
						for (int num8 = 0; num8 < count; num8++)
						{
							results[num8].Append(list3[0]);
						}
					}
					return false;
				}
				}
				break;
				IL_0116:
				regexNode = regexNode.Child(0);
			}
			return false;
		}
	}

	public static string FindPrefix(RegexNode node)
	{
		Span<char> initialBuffer = stackalloc char[64];
		System.Text.ValueStringBuilder vsb = new System.Text.ValueStringBuilder(initialBuffer);
		Process(node, ref vsb);
		return vsb.ToString();
		static bool Process(RegexNode regexNode, ref System.Text.ValueStringBuilder reference)
		{
			if (!StackHelper.TryEnsureSufficientExecutionStack())
			{
				return false;
			}
			bool flag = (regexNode.Options & RegexOptions.RightToLeft) != 0;
			switch (regexNode.Kind)
			{
			case RegexNodeKind.Concatenate:
			{
				int num5 = regexNode.ChildCount();
				for (int k = 0; k < num5; k++)
				{
					if (!Process(regexNode.Child(k), ref reference))
					{
						return false;
					}
				}
				return !flag;
			}
			case RegexNodeKind.Alternate:
				if (!flag)
				{
					int num3 = regexNode.ChildCount();
					int length = reference.Length;
					Process(regexNode.Child(0), ref reference);
					int num4 = reference.Length - length;
					if (num4 != 0)
					{
						System.Text.ValueStringBuilder vsb2 = new System.Text.ValueStringBuilder(64);
						for (int j = 1; j < num3; j++)
						{
							if (num4 == 0)
							{
								break;
							}
							vsb2.Length = 0;
							Process(regexNode.Child(j), ref vsb2);
							num4 = reference.AsSpan(length, num4).CommonPrefixLength(vsb2.AsSpan());
						}
						vsb2.Dispose();
						reference.Length = length + num4;
					}
					return false;
				}
				break;
			case RegexNodeKind.One:
				reference.Append(regexNode.Ch);
				return !flag;
			case RegexNodeKind.Multi:
				reference.Append(regexNode.Str);
				return !flag;
			case RegexNodeKind.Oneloop:
			case RegexNodeKind.Onelazy:
			case RegexNodeKind.Oneloopatomic:
				if (regexNode.M > 0)
				{
					int num2 = Math.Min(regexNode.M, 32);
					reference.Append(regexNode.Ch, num2);
					if (num2 == regexNode.N)
					{
						return !flag;
					}
					return false;
				}
				break;
			case RegexNodeKind.Loop:
			case RegexNodeKind.Lazyloop:
				if (regexNode.M > 0)
				{
					int num = Math.Min(regexNode.M, 4);
					for (int i = 0; i < num; i++)
					{
						if (!Process(regexNode.Child(0), ref reference))
						{
							return false;
						}
					}
					if (num == regexNode.N)
					{
						return !flag;
					}
					return false;
				}
				break;
			case RegexNodeKind.Capture:
			case RegexNodeKind.Atomic:
				return Process(regexNode.Child(0), ref reference);
			case RegexNodeKind.Bol:
			case RegexNodeKind.Eol:
			case RegexNodeKind.Boundary:
			case RegexNodeKind.NonBoundary:
			case RegexNodeKind.Beginning:
			case RegexNodeKind.Start:
			case RegexNodeKind.EndZ:
			case RegexNodeKind.End:
			case RegexNodeKind.Empty:
			case RegexNodeKind.PositiveLookaround:
			case RegexNodeKind.NegativeLookaround:
			case RegexNodeKind.ECMABoundary:
			case RegexNodeKind.NonECMABoundary:
			case RegexNodeKind.UpdateBumpalong:
				return true;
			}
			return false;
		}
	}

	public static string FindPrefixOrdinalCaseInsensitive(RegexNode node)
	{
		while (true)
		{
			switch (node.Kind)
			{
			case RegexNodeKind.Loop:
			case RegexNodeKind.Lazyloop:
				if (node.M > 0)
				{
					goto IL_003b;
				}
				break;
			case RegexNodeKind.Capture:
			case RegexNodeKind.Atomic:
				goto IL_003b;
			case RegexNodeKind.Concatenate:
			{
				node.TryGetOrdinalCaseInsensitiveString(0, node.ChildCount(), out var _, out var caseInsensitiveString, consumeZeroWidthNodes: true);
				return caseInsensitiveString;
			}
			}
			break;
			IL_003b:
			node = node.Child(0);
		}
		return null;
	}

	public static List<RegexFindOptimizations.FixedDistanceSet> FindFixedDistanceSets(RegexNode root, bool thorough)
	{
		List<RegexFindOptimizations.FixedDistanceSet> list = new List<RegexFindOptimizations.FixedDistanceSet>();
		int distance = 0;
		TryFindRawFixedSets(root, list, ref distance, thorough);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].Set == "\0\u0001\0\0")
			{
				list.RemoveAll((RegexFindOptimizations.FixedDistanceSet s) => s.Set == "\0\u0001\0\0");
				break;
			}
		}
		if (list.Count == 0)
		{
			string text = FindFirstCharClass(root);
			if (text == null || text == "\0\u0001\0\0")
			{
				return null;
			}
			list.Add(new RegexFindOptimizations.FixedDistanceSet(null, text, 0));
		}
		Span<char> chars = stackalloc char[128];
		for (int num = 0; num < list.Count; num++)
		{
			RegexFindOptimizations.FixedDistanceSet value = list[num];
			value.Negated = RegexCharClass.IsNegated(value.Set);
			if (RegexCharClass.TryGetSingleRange(value.Set, out var lowInclusive, out var highInclusive) && highInclusive - lowInclusive > 1)
			{
				value.Range = (lowInclusive, highInclusive);
			}
			else
			{
				int setChars = RegexCharClass.GetSetChars(value.Set, chars);
				if (setChars > 0)
				{
					value.Chars = chars.Slice(0, setChars).ToArray();
				}
			}
			list[num] = value;
		}
		return list;
		static bool TryFindRawFixedSets(RegexNode node, List<RegexFindOptimizations.FixedDistanceSet> results, ref int reference, bool flag)
		{
			if (!StackHelper.TryEnsureSufficientExecutionStack())
			{
				return false;
			}
			if ((node.Options & RegexOptions.RightToLeft) != RegexOptions.None)
			{
				return false;
			}
			switch (node.Kind)
			{
			case RegexNodeKind.One:
				if (results.Count < 50)
				{
					string set = RegexCharClass.OneToStringClass(node.Ch);
					results.Add(new RegexFindOptimizations.FixedDistanceSet(null, set, reference++));
					return true;
				}
				return false;
			case RegexNodeKind.Oneloop:
			case RegexNodeKind.Onelazy:
			case RegexNodeKind.Oneloopatomic:
				if (node.M > 0)
				{
					string set3 = RegexCharClass.OneToStringClass(node.Ch);
					int num6 = Math.Min(node.M, 20);
					int n;
					for (n = 0; n < num6; n++)
					{
						if (results.Count >= 50)
						{
							break;
						}
						results.Add(new RegexFindOptimizations.FixedDistanceSet(null, set3, reference++));
					}
					if (n == node.M)
					{
						return n == node.N;
					}
					return false;
				}
				break;
			case RegexNodeKind.Multi:
			{
				string str = node.Str;
				int m;
				for (m = 0; m < str.Length; m++)
				{
					if (results.Count >= 50)
					{
						break;
					}
					string set2 = RegexCharClass.OneToStringClass(str[m]);
					results.Add(new RegexFindOptimizations.FixedDistanceSet(null, set2, reference++));
				}
				return m == str.Length;
			}
			case RegexNodeKind.Set:
				if (results.Count < 50)
				{
					results.Add(new RegexFindOptimizations.FixedDistanceSet(null, node.Str, reference++));
					return true;
				}
				return false;
			case RegexNodeKind.Setloop:
			case RegexNodeKind.Setlazy:
			case RegexNodeKind.Setloopatomic:
				if (node.M > 0)
				{
					int num4 = Math.Min(node.M, 20);
					int k;
					for (k = 0; k < num4; k++)
					{
						if (results.Count >= 50)
						{
							break;
						}
						results.Add(new RegexFindOptimizations.FixedDistanceSet(null, node.Str, reference++));
					}
					if (k == node.M)
					{
						return k == node.N;
					}
					return false;
				}
				break;
			case RegexNodeKind.Notone:
				reference++;
				return true;
			case RegexNodeKind.Notoneloop:
			case RegexNodeKind.Notonelazy:
			case RegexNodeKind.Notoneloopatomic:
				if (node.M == node.N)
				{
					reference += node.M;
					return true;
				}
				break;
			case RegexNodeKind.Bol:
			case RegexNodeKind.Eol:
			case RegexNodeKind.Boundary:
			case RegexNodeKind.NonBoundary:
			case RegexNodeKind.Beginning:
			case RegexNodeKind.Start:
			case RegexNodeKind.EndZ:
			case RegexNodeKind.End:
			case RegexNodeKind.Empty:
			case RegexNodeKind.PositiveLookaround:
			case RegexNodeKind.NegativeLookaround:
			case RegexNodeKind.ECMABoundary:
			case RegexNodeKind.NonECMABoundary:
			case RegexNodeKind.UpdateBumpalong:
				return true;
			case RegexNodeKind.Capture:
			case RegexNodeKind.Group:
			case RegexNodeKind.Atomic:
				return TryFindRawFixedSets(node.Child(0), results, ref reference, flag);
			case RegexNodeKind.Loop:
			case RegexNodeKind.Lazyloop:
				if (node.M > 0)
				{
					TryFindRawFixedSets(node.Child(0), results, ref reference, flag);
					return false;
				}
				break;
			case RegexNodeKind.Concatenate:
			{
				int num5 = node.ChildCount();
				for (int l = 0; l < num5; l++)
				{
					if (!TryFindRawFixedSets(node.Child(l), results, ref reference, flag))
					{
						return false;
					}
				}
				return true;
			}
			case RegexNodeKind.Alternate:
				if (flag)
				{
					int num2 = node.ChildCount();
					bool flag2 = true;
					int? num3 = null;
					Dictionary<int, (RegexCharClass, int)> dictionary = new Dictionary<int, (RegexCharClass, int)>();
					List<RegexFindOptimizations.FixedDistanceSet> list2 = new List<RegexFindOptimizations.FixedDistanceSet>();
					for (int j = 0; j < num2; j++)
					{
						list2.Clear();
						int distance2 = 0;
						flag2 &= TryFindRawFixedSets(node.Child(j), list2, ref distance2, flag);
						if (list2.Count == 0)
						{
							return false;
						}
						if (flag2)
						{
							if (!num3.HasValue)
							{
								num3 = distance2;
							}
							else if (num3.Value != distance2)
							{
								flag2 = false;
							}
						}
						foreach (RegexFindOptimizations.FixedDistanceSet item in list2)
						{
							if (dictionary.TryGetValue(item.Distance, out var value2))
							{
								if (value2.Item1.TryAddCharClass(RegexCharClass.Parse(item.Set)))
								{
									value2.Item2++;
									dictionary[item.Distance] = value2;
								}
							}
							else
							{
								dictionary[item.Distance] = (RegexCharClass.Parse(item.Set), 1);
							}
						}
					}
					foreach (KeyValuePair<int, (RegexCharClass, int)> item2 in dictionary)
					{
						if (results.Count >= 50)
						{
							flag2 = false;
							break;
						}
						if (item2.Value.Item2 == num2)
						{
							results.Add(new RegexFindOptimizations.FixedDistanceSet(null, item2.Value.Item1.ToStringClass(), item2.Key + reference));
						}
					}
					if (flag2)
					{
						reference += num3.Value;
						return true;
					}
					return false;
				}
				break;
			}
			return false;
		}
	}

	public static void SortFixedDistanceSetsByQuality(List<RegexFindOptimizations.FixedDistanceSet> results)
	{
		results.Sort(delegate(RegexFindOptimizations.FixedDistanceSet s1, RegexFindOptimizations.FixedDistanceSet s2)
		{
			char[] chars = s1.Chars;
			char[] chars2 = s2.Chars;
			int num = ((chars != null) ? chars.Length : 0);
			int num2 = ((chars2 != null) ? chars2.Length : 0);
			bool negated = s1.Negated;
			bool negated2 = s2.Negated;
			(char, char)? range = s1.Range;
			int num3 = (range.HasValue ? GetRangeLength(s1.Range.Value, negated) : 0);
			range = s2.Range;
			int num4 = (range.HasValue ? GetRangeLength(s2.Range.Value, negated2) : 0);
			if (negated != negated2)
			{
				if (!negated2)
				{
					return 1;
				}
				return -1;
			}
			if (!negated)
			{
				if (chars != null && chars2 != null)
				{
					float num5 = SumFrequencies(chars);
					float num6 = SumFrequencies(chars2);
					if (num5 != num6)
					{
						return num5.CompareTo(num6);
					}
					if (!RegexCharClass.IsAscii(chars) && !RegexCharClass.IsAscii(chars2))
					{
						return num.CompareTo(num2);
					}
				}
				if ((num > 0 && num4 > 0) || (num3 > 0 && num2 > 0))
				{
					int num7 = Math.Max(num, num3).CompareTo(Math.Max(num2, num4));
					if (num7 != 0)
					{
						return num7;
					}
					if (num <= 0)
					{
						return 1;
					}
					return -1;
				}
				if (num > 0 != num2 > 0)
				{
					if (num <= 0)
					{
						return 1;
					}
					return -1;
				}
			}
			if (num3 > 0 != num4 > 0)
			{
				if (num3 <= 0)
				{
					return 1;
				}
				return -1;
			}
			return (num3 > 0) ? num3.CompareTo(num4) : s1.Distance.CompareTo(s2.Distance);
		});
		static int GetRangeLength((char LowInclusive, char HighInclusive) range, bool negated)
		{
			int num = range.HighInclusive - range.LowInclusive + 1;
			if (!negated)
			{
				return num;
			}
			return 65536 - num;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float SumFrequencies(char[] chars)
		{
			float num = 0f;
			foreach (char c in chars)
			{
				if (c < '\u0080')
				{
					num += Frequency[c];
				}
			}
			return num;
		}
	}

	public static string FindFirstCharClass(RegexNode root)
	{
		return FindFirstOrLastCharClass(root, findFirst: true);
	}

	public static string FindLastCharClass(RegexNode root)
	{
		return FindFirstOrLastCharClass(root, findFirst: false);
	}

	public static string FindFirstOrLastCharClass(RegexNode root, bool findFirst)
	{
		RegexCharClass cc = null;
		if (TryFindFirstOrLastCharClass(root, findFirst, ref cc) != true)
		{
			return null;
		}
		return cc.ToStringClass();
		static bool? TryFindFirstOrLastCharClass(RegexNode node, bool flag2, ref RegexCharClass reference)
		{
			if (!StackHelper.TryEnsureSufficientExecutionStack())
			{
				return false;
			}
			switch (node.Kind)
			{
			case RegexNodeKind.Oneloop:
			case RegexNodeKind.Onelazy:
			case RegexNodeKind.One:
			case RegexNodeKind.Oneloopatomic:
				if (reference == null || reference.CanMerge)
				{
					if (reference == null)
					{
						reference = new RegexCharClass();
					}
					reference.AddChar(node.Ch);
					if (node.Kind != RegexNodeKind.One && node.M <= 0)
					{
						return null;
					}
					return true;
				}
				return false;
			case RegexNodeKind.Notoneloop:
			case RegexNodeKind.Notonelazy:
			case RegexNodeKind.Notone:
			case RegexNodeKind.Notoneloopatomic:
				if (reference == null || reference.CanMerge)
				{
					if (reference == null)
					{
						reference = new RegexCharClass();
					}
					if (node.Ch > '\0')
					{
						reference.AddRange('\0', (char)(node.Ch - 1));
					}
					if (node.Ch < '\uffff')
					{
						reference.AddRange((char)(node.Ch + 1), '\uffff');
					}
					if (node.Kind != RegexNodeKind.Notone && node.M <= 0)
					{
						return null;
					}
					return true;
				}
				return false;
			case RegexNodeKind.Setloop:
			case RegexNodeKind.Setlazy:
			case RegexNodeKind.Set:
			case RegexNodeKind.Setloopatomic:
			{
				bool flag6 = false;
				if (reference == null)
				{
					reference = RegexCharClass.Parse(node.Str);
					flag6 = true;
				}
				else if (reference.CanMerge)
				{
					RegexCharClass regexCharClass = RegexCharClass.Parse(node.Str);
					if (regexCharClass != null && regexCharClass.CanMerge)
					{
						reference.AddCharClass(regexCharClass);
						flag6 = true;
					}
				}
				if (flag6)
				{
					if (node.Kind != RegexNodeKind.Set && node.M <= 0)
					{
						return null;
					}
					return true;
				}
				return false;
			}
			case RegexNodeKind.Multi:
				if (reference == null || reference.CanMerge)
				{
					if (reference == null)
					{
						reference = new RegexCharClass();
					}
					bool flag5 = flag2 == ((node.Options & RegexOptions.RightToLeft) == 0);
					reference.AddChar(node.Str[(!flag5) ? (node.Str.Length - 1) : 0]);
					return true;
				}
				return false;
			case RegexNodeKind.Bol:
			case RegexNodeKind.Eol:
			case RegexNodeKind.Boundary:
			case RegexNodeKind.NonBoundary:
			case RegexNodeKind.Beginning:
			case RegexNodeKind.Start:
			case RegexNodeKind.EndZ:
			case RegexNodeKind.End:
			case RegexNodeKind.Nothing:
			case RegexNodeKind.Empty:
			case RegexNodeKind.PositiveLookaround:
			case RegexNodeKind.NegativeLookaround:
			case RegexNodeKind.ECMABoundary:
			case RegexNodeKind.NonECMABoundary:
			case RegexNodeKind.UpdateBumpalong:
				return null;
			case RegexNodeKind.Capture:
			case RegexNodeKind.Atomic:
				return TryFindFirstOrLastCharClass(node.Child(0), flag2, ref reference);
			case RegexNodeKind.Loop:
			case RegexNodeKind.Lazyloop:
			{
				bool? flag4 = TryFindFirstOrLastCharClass(node.Child(0), flag2, ref reference);
				if (!flag4.HasValue)
				{
					return null;
				}
				if (flag4 != true)
				{
					return false;
				}
				return (node.M == 0) ? ((bool?)null) : new bool?(true);
			}
			case RegexNodeKind.Concatenate:
			{
				int num2 = node.ChildCount();
				if (flag2)
				{
					for (int i = 0; i < num2; i++)
					{
						bool? result = TryFindFirstOrLastCharClass(node.Child(i), flag2, ref reference);
						if (result.HasValue)
						{
							return result;
						}
					}
				}
				else
				{
					for (int num3 = num2 - 1; num3 >= 0; num3--)
					{
						bool? result2 = TryFindFirstOrLastCharClass(node.Child(num3), flag2, ref reference);
						if (result2.HasValue)
						{
							return result2;
						}
					}
				}
				return null;
			}
			case RegexNodeKind.Alternate:
			{
				int num4 = node.ChildCount();
				bool flag7 = false;
				for (int j = 0; j < num4; j++)
				{
					bool? flag8 = TryFindFirstOrLastCharClass(node.Child(j), flag2, ref reference);
					if (!flag8.HasValue)
					{
						flag7 = true;
					}
					else if (flag8 == false)
					{
						return false;
					}
				}
				if (!flag7)
				{
					return true;
				}
				return null;
			}
			case RegexNodeKind.BackreferenceConditional:
			case RegexNodeKind.ExpressionConditional:
			{
				int num = ((node.Kind != RegexNodeKind.BackreferenceConditional) ? 1 : 0);
				bool? flag = TryFindFirstOrLastCharClass(node.Child(num), flag2, ref reference);
				bool? flag3 = TryFindFirstOrLastCharClass(node.Child(num + 1), flag2, ref reference);
				if (flag.HasValue)
				{
					if (flag == true)
					{
						if (!flag3.HasValue)
						{
							goto IL_043a;
						}
						if (flag3 == true)
						{
							return true;
						}
					}
				}
				else if (!((!flag3) ?? false))
				{
					goto IL_043a;
				}
				return false;
			}
			case RegexNodeKind.Backreference:
				return false;
			default:
				{
					return false;
				}
				IL_043a:
				return null;
			}
		}
	}

	public static (RegexNode LoopNode, (char Char, string String, StringComparison StringComparison, char[] Chars) Literal)? FindLiteralFollowingLeadingLoop(RegexNode node)
	{
		if ((node.Options & RegexOptions.RightToLeft) != RegexOptions.None)
		{
			return null;
		}
		RegexNodeKind kind;
		while (true)
		{
			kind = node.Kind;
			if ((kind != RegexNodeKind.Capture && kind != RegexNodeKind.Atomic) || 1 == 0)
			{
				break;
			}
			node = node.Child(0);
		}
		if (node.Kind != RegexNodeKind.Concatenate)
		{
			return null;
		}
		RegexNode regexNode = node.Child(0);
		while (true)
		{
			kind = regexNode.Kind;
			if ((kind != RegexNodeKind.Capture && kind != RegexNodeKind.Atomic) || 1 == 0)
			{
				break;
			}
			regexNode = regexNode.Child(0);
		}
		kind = regexNode.Kind;
		bool flag = ((kind == RegexNodeKind.Setloop || kind == RegexNodeKind.Setlazy || kind == RegexNodeKind.Setloopatomic) ? true : false);
		if (!flag || regexNode.N != int.MaxValue)
		{
			return null;
		}
		RegexNode regexNode2 = node.Child(1);
		if (regexNode2.Kind == RegexNodeKind.UpdateBumpalong)
		{
			if (node.ChildCount() == 2)
			{
				return null;
			}
			regexNode2 = node.Child(2);
		}
		string text = FindPrefix(regexNode2);
		if (text != null && text.Length >= 1)
		{
			if (!RegexCharClass.CharInClass(text[0], regexNode.Str))
			{
				if (text.Length != 1)
				{
					return (regexNode, ('\0', text, StringComparison.Ordinal, null));
				}
				return (regexNode, (text[0], null, StringComparison.Ordinal, null));
			}
			return null;
		}
		string text2 = FindPrefixOrdinalCaseInsensitive(regexNode2);
		if (text2 != null && text2.Length >= 2)
		{
			if (RegexCharClass.ParticipatesInCaseConversion(text2[0]))
			{
				if (RegexCharClass.CharInClass((char)(text2[0] | 0x20), regexNode.Str) || RegexCharClass.CharInClass((char)(text2[0] & -33), regexNode.Str))
				{
					return null;
				}
			}
			else if (RegexCharClass.CharInClass(text2[0], regexNode.Str))
			{
				return null;
			}
			return (regexNode, ('\0', text2, StringComparison.OrdinalIgnoreCase, null));
		}
		while (true)
		{
			kind = regexNode2.Kind;
			flag = ((kind == RegexNodeKind.Concatenate || kind == RegexNodeKind.Capture || kind == RegexNodeKind.Atomic) ? true : false);
			bool flag2 = flag;
			if (!flag2)
			{
				RegexNodeKind kind2 = regexNode2.Kind;
				bool flag3 = kind2 - 26 <= (RegexNodeKind)1;
				flag2 = flag3 && regexNode2.M >= 1;
			}
			if (!flag2)
			{
				break;
			}
			regexNode2 = regexNode2.Child(0);
		}
		if (regexNode2.IsSetFamily && !RegexCharClass.IsNegated(regexNode2.Str) && (regexNode2.Kind == RegexNodeKind.Set || regexNode2.M >= 1))
		{
			Span<char> span = stackalloc char[5];
			span = span.Slice(0, RegexCharClass.GetSetChars(regexNode2.Str, span));
			if (!span.IsEmpty)
			{
				Span<char> span2 = span;
				for (int i = 0; i < span2.Length; i++)
				{
					if (RegexCharClass.CharInClass(span2[i], regexNode.Str))
					{
						return null;
					}
				}
				return (regexNode, ('\0', null, StringComparison.Ordinal, span.ToArray()));
			}
		}
		return null;
	}

	public static RegexNode FindLeadingPositiveLookahead(RegexNode node)
	{
		RegexNode positiveLookahead = null;
		FindLeadingPositiveLookahead(node, ref positiveLookahead);
		return positiveLookahead;
		static bool FindLeadingPositiveLookahead(RegexNode regexNode, ref RegexNode reference)
		{
			if (!StackHelper.TryEnsureSufficientExecutionStack())
			{
				return false;
			}
			while (true)
			{
				if ((regexNode.Options & RegexOptions.RightToLeft) == 0)
				{
					switch (regexNode.Kind)
					{
					case RegexNodeKind.PositiveLookaround:
						reference = regexNode;
						return false;
					case RegexNodeKind.Bol:
					case RegexNodeKind.Eol:
					case RegexNodeKind.Boundary:
					case RegexNodeKind.Beginning:
					case RegexNodeKind.Start:
					case RegexNodeKind.EndZ:
					case RegexNodeKind.End:
					case RegexNodeKind.Empty:
					case RegexNodeKind.NegativeLookaround:
					case RegexNodeKind.ECMABoundary:
						return true;
					case RegexNodeKind.Capture:
					case RegexNodeKind.Atomic:
						goto IL_009f;
					case RegexNodeKind.Loop:
					case RegexNodeKind.Lazyloop:
						if (regexNode.M >= 1)
						{
							FindLeadingPositiveLookahead(regexNode.Child(0), ref reference);
							return false;
						}
						break;
					case RegexNodeKind.Concatenate:
					{
						int num = regexNode.ChildCount();
						for (int i = 0; i < num; i++)
						{
							if (!FindLeadingPositiveLookahead(regexNode.Child(i), ref reference))
							{
								return false;
							}
						}
						return true;
					}
					}
					break;
				}
				return false;
				IL_009f:
				regexNode = regexNode.Child(0);
			}
			return false;
		}
	}

	public static RegexNodeKind FindLeadingAnchor(RegexNode node)
	{
		return FindLeadingOrTrailingAnchor(node, leading: true);
	}

	public static RegexNodeKind FindTrailingAnchor(RegexNode node)
	{
		return FindLeadingOrTrailingAnchor(node, leading: false);
	}

	private static RegexNodeKind FindLeadingOrTrailingAnchor(RegexNode node, bool leading)
	{
		if (!StackHelper.TryEnsureSufficientExecutionStack())
		{
			return RegexNodeKind.Unknown;
		}
		while (true)
		{
			switch (node.Kind)
			{
			case RegexNodeKind.Bol:
			case RegexNodeKind.Eol:
			case RegexNodeKind.Beginning:
			case RegexNodeKind.Start:
			case RegexNodeKind.EndZ:
			case RegexNodeKind.End:
				return node.Kind;
			case RegexNodeKind.Loop:
			case RegexNodeKind.Lazyloop:
				if (!leading || node.M < 1)
				{
					break;
				}
				goto case RegexNodeKind.Capture;
			case RegexNodeKind.PositiveLookaround:
				if (!leading || (node.Options & RegexOptions.RightToLeft) != RegexOptions.None)
				{
					break;
				}
				goto case RegexNodeKind.Capture;
			case RegexNodeKind.Capture:
			case RegexNodeKind.Atomic:
				node = node.Child(0);
				continue;
			case RegexNodeKind.Concatenate:
			{
				int num2 = node.ChildCount();
				RegexNode regexNode = null;
				RegexNodeKind regexNodeKind2 = RegexNodeKind.Unknown;
				if (leading)
				{
					for (int j = 0; j < num2; j++)
					{
						RegexNode regexNode2 = node.Child(j);
						switch (regexNode2.Kind)
						{
						case RegexNodeKind.PositiveLookaround:
							if (((node.Options | regexNode2.Options) & RegexOptions.RightToLeft) == 0)
							{
								regexNodeKind2 = ChooseBetterAnchor(regexNodeKind2, FindLeadingOrTrailingAnchor(regexNode2, leading));
								if (IsBestAnchor(regexNodeKind2))
								{
									return regexNodeKind2;
								}
							}
							continue;
						case RegexNodeKind.Boundary:
						case RegexNodeKind.NonBoundary:
						case RegexNodeKind.Empty:
						case RegexNodeKind.NegativeLookaround:
						case RegexNodeKind.ECMABoundary:
						case RegexNodeKind.NonECMABoundary:
							continue;
						}
						regexNode = regexNode2;
						break;
					}
				}
				else
				{
					for (int num3 = num2 - 1; num3 >= 0; num3--)
					{
						RegexNodeKind kind = node.Child(num3).Kind;
						if ((kind != RegexNodeKind.Empty && kind - 30 > (RegexNodeKind)1) || 1 == 0)
						{
							regexNode = node.Child(num3);
							break;
						}
					}
				}
				if (regexNodeKind2 != RegexNodeKind.Unknown)
				{
					if (regexNode == null)
					{
						return regexNodeKind2;
					}
					return ChooseBetterAnchor(regexNodeKind2, FindLeadingAnchor(regexNode));
				}
				if (regexNode != null)
				{
					node = regexNode;
					continue;
				}
				break;
			}
			case RegexNodeKind.Alternate:
			{
				RegexNodeKind regexNodeKind = FindLeadingOrTrailingAnchor(node.Child(0), leading);
				if (regexNodeKind == RegexNodeKind.Unknown)
				{
					return RegexNodeKind.Unknown;
				}
				int num = node.ChildCount();
				for (int i = 1; i < num; i++)
				{
					if (FindLeadingOrTrailingAnchor(node.Child(i), leading) != regexNodeKind)
					{
						return RegexNodeKind.Unknown;
					}
				}
				return regexNodeKind;
			}
			}
			break;
		}
		return RegexNodeKind.Unknown;
		static RegexNodeKind ChooseBetterAnchor(RegexNodeKind anchor1, RegexNodeKind anchor2)
		{
			if (anchor1 != RegexNodeKind.Unknown)
			{
				if (anchor2 != RegexNodeKind.Unknown)
				{
					if (RankAnchorQuality(anchor1) < RankAnchorQuality(anchor2))
					{
						return anchor2;
					}
					return anchor1;
				}
				return anchor1;
			}
			return anchor2;
		}
		static bool IsBestAnchor(RegexNodeKind anchor)
		{
			if (anchor - 18 <= RegexNodeKind.Oneloop)
			{
				return true;
			}
			return false;
		}
		static int RankAnchorQuality(RegexNodeKind regexNodeKind3)
		{
			return regexNodeKind3 switch
			{
				RegexNodeKind.Beginning => 3, 
				RegexNodeKind.Start => 3, 
				RegexNodeKind.End => 3, 
				RegexNodeKind.EndZ => 3, 
				RegexNodeKind.Bol => 2, 
				RegexNodeKind.Eol => 2, 
				RegexNodeKind.Boundary => 1, 
				RegexNodeKind.ECMABoundary => 1, 
				_ => 0, 
			};
		}
	}
}
