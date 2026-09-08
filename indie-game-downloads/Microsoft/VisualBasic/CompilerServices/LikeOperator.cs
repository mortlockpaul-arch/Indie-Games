using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class LikeOperator
{
	private enum CharKind
	{
		None,
		ExpandedChar1,
		ExpandedChar2
	}

	private struct LigatureInfo
	{
		internal CharKind Kind;

		internal char CharBeforeExpansion;
	}

	private enum PatternType
	{
		STRING,
		EXCLIST,
		INCLIST,
		DIGIT,
		ANYCHAR,
		STAR,
		NONE
	}

	private struct PatternGroup
	{
		internal PatternType PatType;

		internal int MaxSourceIndex;

		internal int CharCount;

		internal int StringPatternStart;

		internal int StringPatternEnd;

		internal int MinSourceIndex;

		internal List<Range> RangeList;

		public int StartIndexOfPossibleMatch;
	}

	private struct Range
	{
		internal int Start;

		internal int StartLength;

		internal int End;

		internal int EndLength;
	}

	private static string[] LigatureExpansions;

	private static byte[] LigatureMap;

	static LikeOperator()
	{
		LigatureExpansions = new string[9] { "", "ss", "sz", "AE", "ae", "TH", "th", "OE", "oe" };
		LigatureMap = new byte[142];
		LigatureMap[25] = 1;
		LigatureMap[25] = 2;
		LigatureMap[0] = 3;
		LigatureMap[32] = 4;
		LigatureMap[24] = 5;
		LigatureMap[56] = 6;
		LigatureMap[140] = 7;
		LigatureMap[141] = 8;
	}

	private static byte LigatureIndex(char ch)
	{
		if (ch < 'Æ' || ch > 'œ')
		{
			return 0;
		}
		return LigatureMap[checked(ch - 198)];
	}

	private static int CanCharExpand(char ch, byte[] LocaleSpecificLigatureTable, CompareInfo Comparer, CompareOptions Options)
	{
		byte b = LigatureIndex(ch);
		if (b == 0)
		{
			return 0;
		}
		if (LocaleSpecificLigatureTable[b] == 0)
		{
			if (Comparer.Compare(Conversions.ToString(ch), LigatureExpansions[b]) == 0)
			{
				LocaleSpecificLigatureTable[b] = 1;
			}
			else
			{
				LocaleSpecificLigatureTable[b] = 2;
			}
		}
		if (LocaleSpecificLigatureTable[b] == 1)
		{
			return b;
		}
		return 0;
	}

	private static string GetCharExpansion(char ch, byte[] LocaleSpecificLigatureTable, CompareInfo Comparer, CompareOptions Options)
	{
		int num = CanCharExpand(ch, LocaleSpecificLigatureTable, Comparer, Options);
		if (num == 0)
		{
			return Conversions.ToString(ch);
		}
		return LigatureExpansions[num];
	}

	private static void ExpandString(ref string Input, ref int Length, ref LigatureInfo[] InputLigatureInfo, byte[] LocaleSpecificLigatureTable, CompareInfo Comparer, CompareOptions Options, ref bool WidthChanged, bool UseFullWidth)
	{
		WidthChanged = false;
		if (Length == 0)
		{
			return;
		}
		Input = Input.ToLowerInvariant();
		checked
		{
			int num = Length - 1;
			int num2 = default(int);
			for (int i = 0; i <= num; i++)
			{
				if (CanCharExpand(Input[i], LocaleSpecificLigatureTable, Comparer, Options) != 0)
				{
					num2++;
				}
			}
			if (num2 <= 0)
			{
				return;
			}
			InputLigatureInfo = new LigatureInfo[Length + num2 - 1 + 1];
			StringBuilder stringBuilder = new StringBuilder(Length + num2 - 1);
			int num3 = 0;
			int num4 = Length - 1;
			for (int j = 0; j <= num4; j++)
			{
				char c = Input[j];
				if (CanCharExpand(c, LocaleSpecificLigatureTable, Comparer, Options) != 0)
				{
					string charExpansion = GetCharExpansion(c, LocaleSpecificLigatureTable, Comparer, Options);
					stringBuilder.Append(charExpansion);
					InputLigatureInfo[num3].Kind = CharKind.ExpandedChar1;
					InputLigatureInfo[num3].CharBeforeExpansion = c;
					num3++;
					InputLigatureInfo[num3].Kind = CharKind.ExpandedChar2;
					InputLigatureInfo[num3].CharBeforeExpansion = c;
				}
				else
				{
					stringBuilder.Append(c);
				}
				num3++;
			}
			Input = stringBuilder.ToString();
			Length = stringBuilder.Length;
		}
	}

	[RequiresUnreferencedCode("The types of source and pattern cannot be statically analyzed so the like operator may be trimmed")]
	public static object LikeObject(object Source, object Pattern, CompareMethod CompareOption)
	{
		TypeCode typeCode = ((Source is IConvertible convertible) ? convertible.GetTypeCode() : ((Source != null) ? TypeCode.Object : TypeCode.Empty));
		TypeCode typeCode2 = ((Pattern is IConvertible convertible2) ? convertible2.GetTypeCode() : ((Pattern != null) ? TypeCode.Object : TypeCode.Empty));
		if (typeCode == TypeCode.Object && Source is char[])
		{
			typeCode = TypeCode.String;
		}
		if (typeCode2 == TypeCode.Object && Pattern is char[])
		{
			typeCode2 = TypeCode.String;
		}
		if (typeCode == TypeCode.Object || typeCode2 == TypeCode.Object)
		{
			return Operators.InvokeUserDefinedOperator(Symbols.UserDefinedOperator.Like, Source, Pattern);
		}
		return LikeString(Conversions.ToString(Source), Conversions.ToString(Pattern), CompareOption);
	}

	public static bool LikeString(string Source, string Pattern, CompareMethod CompareOption)
	{
		LigatureInfo[] InputLigatureInfo = null;
		LigatureInfo[] InputLigatureInfo2 = null;
		int Length = Pattern?.Length ?? 0;
		int Length2 = Source?.Length ?? 0;
		checked
		{
			CompareOptions compareOptions;
			CompareInfo compareInfo;
			if (CompareOption == CompareMethod.Binary)
			{
				compareOptions = CompareOptions.Ordinal;
				compareInfo = null;
			}
			else
			{
				compareInfo = Utils.GetCultureInfo().CompareInfo;
				compareOptions = CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth;
				byte[] localeSpecificLigatureTable = new byte[LigatureExpansions.Length - 1 + 1];
				CompareInfo comparer = compareInfo;
				CompareOptions options = compareOptions;
				bool WidthChanged = false;
				ExpandString(ref Source, ref Length2, ref InputLigatureInfo, localeSpecificLigatureTable, comparer, options, ref WidthChanged, UseFullWidth: false);
				CompareInfo comparer2 = compareInfo;
				CompareOptions options2 = compareOptions;
				WidthChanged = false;
				ExpandString(ref Pattern, ref Length, ref InputLigatureInfo2, localeSpecificLigatureTable, comparer2, options2, ref WidthChanged, UseFullWidth: false);
			}
			int PatternIndex = default(int);
			int Current = default(int);
			bool RangePatternEmpty = default(bool);
			bool Mismatch = default(bool);
			bool PatternError = default(bool);
			bool Mismatch2 = default(bool);
			bool PatternError2 = default(bool);
			while (PatternIndex < Length && Current < Length2)
			{
				switch (Pattern[PatternIndex])
				{
				case '?':
				case '？':
					SkipToEndOfExpandedChar(InputLigatureInfo, Length2, ref Current);
					break;
				case '#':
				case '＃':
					if (!char.IsDigit(Source[Current]))
					{
						return false;
					}
					break;
				case '[':
				case '［':
				{
					string source = Source;
					int sourceLength = Length2;
					LigatureInfo[] sourceLigatureInfo = InputLigatureInfo;
					string pattern = Pattern;
					int patternLength = Length;
					LigatureInfo[] patternLigatureInfo = InputLigatureInfo2;
					CompareInfo comparer3 = compareInfo;
					CompareOptions options3 = compareOptions;
					bool WidthChanged = false;
					MatchRange(source, sourceLength, ref Current, sourceLigatureInfo, pattern, patternLength, ref PatternIndex, patternLigatureInfo, ref RangePatternEmpty, ref Mismatch, ref PatternError, comparer3, options3, ref WidthChanged);
					if (PatternError)
					{
						throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Pattern"));
					}
					if (Mismatch)
					{
						return false;
					}
					if (RangePatternEmpty)
					{
						PatternIndex++;
						continue;
					}
					break;
				}
				case '*':
				case '＊':
					MatchAsterisk(Source, Length2, Current, InputLigatureInfo, Pattern, Length, PatternIndex, InputLigatureInfo2, ref Mismatch2, ref PatternError2, compareInfo, compareOptions);
					if (PatternError2)
					{
						throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Pattern"));
					}
					return !Mismatch2;
				default:
					if (CompareChars(Source, Length2, Current, ref Current, InputLigatureInfo, Pattern, Length, PatternIndex, ref PatternIndex, InputLigatureInfo2, compareInfo, compareOptions) != 0)
					{
						return false;
					}
					break;
				}
				PatternIndex++;
				Current++;
			}
			while (PatternIndex < Length)
			{
				char c = Pattern[PatternIndex];
				if (c == '*' || c == '＊')
				{
					PatternIndex++;
					continue;
				}
				if (PatternIndex + 1 >= Length || ((c != '[' || Pattern[PatternIndex + 1] != ']') && (c != '［' || Pattern[PatternIndex + 1] != '］')))
				{
					break;
				}
				PatternIndex += 2;
			}
			return PatternIndex >= Length && Current >= Length2;
		}
	}

	private static void SkipToEndOfExpandedChar(LigatureInfo[] InputLigatureInfo, int Length, ref int Current)
	{
		checked
		{
			if (InputLigatureInfo != null && Current < Length && InputLigatureInfo[Current].Kind == CharKind.ExpandedChar1)
			{
				Current++;
			}
		}
	}

	private static int CompareChars(string Left, int LeftLength, int LeftStart, ref int LeftEnd, LigatureInfo[] LeftLigatureInfo, string Right, int RightLength, int RightStart, ref int RightEnd, LigatureInfo[] RightLigatureInfo, CompareInfo Comparer, CompareOptions Options, bool MatchBothCharsOfExpandedCharInRight = false, bool UseUnexpandedCharForRight = false)
	{
		LeftEnd = LeftStart;
		RightEnd = RightStart;
		checked
		{
			if (Options == CompareOptions.Ordinal)
			{
				return Left[LeftStart] - Right[RightStart];
			}
			if (UseUnexpandedCharForRight)
			{
				if (RightLigatureInfo != null && RightLigatureInfo[RightEnd].Kind == CharKind.ExpandedChar1)
				{
					Right = Right.Substring(RightStart, RightEnd - RightStart);
					Right += Conversions.ToString(RightLigatureInfo[RightEnd].CharBeforeExpansion);
					RightEnd++;
					return CompareChars(Left.Substring(LeftStart, LeftEnd - LeftStart + 1), Right, Comparer, Options);
				}
			}
			else if (MatchBothCharsOfExpandedCharInRight)
			{
				int num = RightEnd;
				SkipToEndOfExpandedChar(RightLigatureInfo, RightLength, ref RightEnd);
				if (num < RightEnd)
				{
					int num2 = 0;
					if (LeftEnd + 1 < LeftLength)
					{
						num2 = 1;
					}
					int num3 = CompareChars(Left.Substring(LeftStart, LeftEnd - LeftStart + 1 + num2), Right.Substring(RightStart, RightEnd - RightStart + 1), Comparer, Options);
					if (num3 == 0)
					{
						LeftEnd += num2;
					}
					return num3;
				}
			}
			if (LeftEnd == LeftStart && RightEnd == RightStart)
			{
				return Comparer.Compare(Conversions.ToString(Left[LeftStart]), Conversions.ToString(Right[RightStart]), Options);
			}
			return CompareChars(Left.Substring(LeftStart, LeftEnd - LeftStart + 1), Right.Substring(RightStart, RightEnd - RightStart + 1), Comparer, Options);
		}
	}

	private static int CompareChars(string Left, string Right, CompareInfo Comparer, CompareOptions Options)
	{
		if (Options == CompareOptions.Ordinal)
		{
			return checked(Left[0] - Right[0]);
		}
		return Comparer.Compare(Left, Right, Options);
	}

	private static int CompareChars(char Left, char Right, CompareInfo Comparer, CompareOptions Options)
	{
		if (Options == CompareOptions.Ordinal)
		{
			return checked(Left - Right);
		}
		return Comparer.Compare(Conversions.ToString(Left), Conversions.ToString(Right), Options);
	}

	private static void MatchRange(string Source, int SourceLength, ref int SourceIndex, LigatureInfo[] SourceLigatureInfo, string Pattern, int PatternLength, ref int PatternIndex, LigatureInfo[] PatternLigatureInfo, ref bool RangePatternEmpty, ref bool Mismatch, ref bool PatternError, CompareInfo Comparer, CompareOptions Options, [Optional][DefaultParameterValue(false)] ref bool SeenNot, List<Range> RangeList = null, bool ValidatePatternWithoutMatching = false)
	{
		RangePatternEmpty = false;
		Mismatch = false;
		PatternError = false;
		SeenNot = false;
		checked
		{
			PatternIndex++;
			if (PatternIndex >= PatternLength)
			{
				PatternError = true;
				return;
			}
			char c = Pattern[PatternIndex];
			if (c == '!' || c == '！')
			{
				SeenNot = true;
				PatternIndex++;
				if (PatternIndex >= PatternLength)
				{
					Mismatch = true;
					return;
				}
				c = Pattern[PatternIndex];
			}
			Range item = default(Range);
			if (c == ']' || c == '］')
			{
				if (SeenNot)
				{
					SeenNot = false;
					if (!ValidatePatternWithoutMatching)
					{
						Mismatch = CompareChars(Source[SourceIndex], '!', Comparer, Options) != 0;
					}
					if (RangeList != null)
					{
						item.Start = PatternIndex - 1;
						item.StartLength = 1;
						item.End = -1;
						item.EndLength = 0;
						RangeList.Add(item);
					}
				}
				else
				{
					RangePatternEmpty = true;
				}
				return;
			}
			int LeftEnd = default(int);
			int Current = default(int);
			while (true)
			{
				string text = null;
				string text2 = null;
				if (c == ']' || c == '］')
				{
					Mismatch = !SeenNot;
					return;
				}
				if (!ValidatePatternWithoutMatching && PatternLigatureInfo != null && PatternLigatureInfo[PatternIndex].Kind == CharKind.ExpandedChar1)
				{
					if (CompareChars(Source, SourceLength, SourceIndex, ref LeftEnd, SourceLigatureInfo, Pattern, PatternLength, PatternIndex, ref Current, PatternLigatureInfo, Comparer, Options, MatchBothCharsOfExpandedCharInRight: true) == 0)
					{
						SourceIndex = LeftEnd;
						PatternIndex = Current;
						goto IL_036c;
					}
				}
				else
				{
					Current = PatternIndex;
					SkipToEndOfExpandedChar(PatternLigatureInfo, PatternLength, ref Current);
				}
				item.Start = PatternIndex;
				item.StartLength = Current - PatternIndex + 1;
				if (Options == CompareOptions.Ordinal)
				{
					text = Conversions.ToString(Pattern[PatternIndex]);
				}
				else if (PatternLigatureInfo != null && PatternLigatureInfo[PatternIndex].Kind == CharKind.ExpandedChar1)
				{
					text = Conversions.ToString(PatternLigatureInfo[PatternIndex].CharBeforeExpansion);
					PatternIndex = Current;
				}
				else
				{
					text = Pattern.Substring(PatternIndex, Current - PatternIndex + 1);
					PatternIndex = Current;
				}
				if (Current + 2 < PatternLength && (Pattern[Current + 1] == '-' || Pattern[Current + 1] == '－') && Pattern[Current + 2] != ']' && Pattern[Current + 2] != '］')
				{
					PatternIndex += 2;
					if (!ValidatePatternWithoutMatching && PatternLigatureInfo != null && PatternLigatureInfo[PatternIndex].Kind == CharKind.ExpandedChar1)
					{
						if (CompareChars(Source, SourceLength, SourceIndex, ref LeftEnd, SourceLigatureInfo, Pattern, PatternLength, PatternIndex, ref Current, PatternLigatureInfo, Comparer, Options, MatchBothCharsOfExpandedCharInRight: true) == 0)
						{
							PatternIndex = Current;
							goto IL_036c;
						}
					}
					else
					{
						Current = PatternIndex;
						SkipToEndOfExpandedChar(PatternLigatureInfo, PatternLength, ref Current);
					}
					item.End = PatternIndex;
					item.EndLength = Current - PatternIndex + 1;
					if (Options == CompareOptions.Ordinal)
					{
						text2 = Conversions.ToString(Pattern[PatternIndex]);
					}
					else if (PatternLigatureInfo != null && PatternLigatureInfo[PatternIndex].Kind == CharKind.ExpandedChar1)
					{
						text2 = Conversions.ToString(PatternLigatureInfo[PatternIndex].CharBeforeExpansion);
						PatternIndex = Current;
					}
					else
					{
						text2 = Pattern.Substring(PatternIndex, Current - PatternIndex + 1);
						PatternIndex = Current;
					}
					if (CompareChars(text, text2, Comparer, Options) > 0)
					{
						PatternError = true;
						return;
					}
					if (!ValidatePatternWithoutMatching)
					{
						int leftStart = SourceIndex;
						int rightLength = item.Start + item.StartLength;
						int start = item.Start;
						int RightEnd = 0;
						if (CompareChars(Source, SourceLength, leftStart, ref LeftEnd, SourceLigatureInfo, Pattern, rightLength, start, ref RightEnd, PatternLigatureInfo, Comparer, Options, MatchBothCharsOfExpandedCharInRight: false, UseUnexpandedCharForRight: true) >= 0)
						{
							int leftStart2 = SourceIndex;
							int rightLength2 = item.End + item.EndLength;
							int end = item.End;
							RightEnd = 0;
							if (CompareChars(Source, SourceLength, leftStart2, ref LeftEnd, SourceLigatureInfo, Pattern, rightLength2, end, ref RightEnd, PatternLigatureInfo, Comparer, Options, MatchBothCharsOfExpandedCharInRight: false, UseUnexpandedCharForRight: true) <= 0)
							{
								goto IL_036c;
							}
						}
					}
				}
				else
				{
					if (!ValidatePatternWithoutMatching)
					{
						int leftStart3 = SourceIndex;
						int rightLength3 = item.Start + item.StartLength;
						int start2 = item.Start;
						int RightEnd = 0;
						if (CompareChars(Source, SourceLength, leftStart3, ref LeftEnd, SourceLigatureInfo, Pattern, rightLength3, start2, ref RightEnd, PatternLigatureInfo, Comparer, Options, MatchBothCharsOfExpandedCharInRight: false, UseUnexpandedCharForRight: true) == 0)
						{
							goto IL_036c;
						}
					}
					item.End = -1;
					item.EndLength = 0;
				}
				RangeList?.Add(item);
				PatternIndex++;
				if (PatternIndex >= PatternLength)
				{
					break;
				}
				c = Pattern[PatternIndex];
				continue;
				IL_036c:
				if (SeenNot)
				{
					Mismatch = true;
					return;
				}
				do
				{
					PatternIndex++;
					if (PatternIndex >= PatternLength)
					{
						PatternError = true;
						return;
					}
				}
				while (Pattern[PatternIndex] != ']' && Pattern[PatternIndex] != '］');
				SourceIndex = LeftEnd;
				return;
			}
			PatternError = true;
		}
	}

	private static bool ValidateRangePattern(string Pattern, int PatternLength, ref int PatternIndex, LigatureInfo[] PatternLigatureInfo, CompareInfo Comparer, CompareOptions Options, ref bool SeenNot, ref List<Range> RangeList)
	{
		int SourceIndex = -1;
		bool RangePatternEmpty = false;
		bool Mismatch = false;
		bool PatternError = default(bool);
		MatchRange(null, -1, ref SourceIndex, null, Pattern, PatternLength, ref PatternIndex, PatternLigatureInfo, ref RangePatternEmpty, ref Mismatch, ref PatternError, Comparer, Options, ref SeenNot, RangeList, ValidatePatternWithoutMatching: true);
		return !PatternError;
	}

	private static void BuildPatternGroups(string Source, int SourceLength, ref int SourceIndex, LigatureInfo[] SourceLigatureInfo, string Pattern, int PatternLength, ref int PatternIndex, LigatureInfo[] PatternLigatureInfo, ref bool PatternError, ref int PGIndexForLastAsterisk, CompareInfo Comparer, CompareOptions Options, ref PatternGroup[] PatternGroups)
	{
		PatternError = false;
		PGIndexForLastAsterisk = 0;
		PatternGroups = new PatternGroup[16];
		int num = 15;
		PatternType patternType = PatternType.NONE;
		int num2 = 0;
		checked
		{
			do
			{
				if (num2 >= num)
				{
					PatternGroup[] array = new PatternGroup[num + 16 + 1];
					PatternGroups.CopyTo(array, 0);
					PatternGroups = array;
					num += 16;
				}
				switch (Pattern[PatternIndex])
				{
				case '*':
				case '＊':
					if (patternType != PatternType.STAR)
					{
						patternType = PatternType.STAR;
						PatternGroups[num2].PatType = PatternType.STAR;
						PGIndexForLastAsterisk = num2;
						num2++;
					}
					break;
				case '[':
				case '［':
				{
					bool SeenNot = false;
					List<Range> RangeList = new List<Range>();
					if (!ValidateRangePattern(Pattern, PatternLength, ref PatternIndex, PatternLigatureInfo, Comparer, Options, ref SeenNot, ref RangeList))
					{
						PatternError = true;
						return;
					}
					if (RangeList.Count != 0)
					{
						patternType = (SeenNot ? PatternType.EXCLIST : PatternType.INCLIST);
						PatternGroups[num2].PatType = patternType;
						PatternGroups[num2].CharCount = 1;
						PatternGroups[num2].RangeList = RangeList;
						num2++;
					}
					break;
				}
				case '#':
				case '＃':
					if (patternType == PatternType.DIGIT)
					{
						PatternGroups[num2 - 1].CharCount++;
						break;
					}
					PatternGroups[num2].PatType = PatternType.DIGIT;
					PatternGroups[num2].CharCount = 1;
					num2++;
					patternType = PatternType.DIGIT;
					break;
				case '?':
				case '？':
					if (patternType == PatternType.ANYCHAR)
					{
						PatternGroups[num2 - 1].CharCount++;
						break;
					}
					PatternGroups[num2].PatType = PatternType.ANYCHAR;
					PatternGroups[num2].CharCount = 1;
					num2++;
					patternType = PatternType.ANYCHAR;
					break;
				default:
				{
					int stringPatternStart = PatternIndex;
					int num3 = PatternIndex;
					if (num3 >= PatternLength)
					{
						num3 = PatternLength - 1;
					}
					if (patternType == PatternType.STRING)
					{
						PatternGroups[num2 - 1].CharCount++;
						PatternGroups[num2 - 1].StringPatternEnd = num3;
						break;
					}
					PatternGroups[num2].PatType = PatternType.STRING;
					PatternGroups[num2].CharCount = 1;
					PatternGroups[num2].StringPatternStart = stringPatternStart;
					PatternGroups[num2].StringPatternEnd = num3;
					num2++;
					patternType = PatternType.STRING;
					break;
				}
				}
				PatternIndex++;
			}
			while (PatternIndex < PatternLength);
			PatternGroups[num2].PatType = PatternType.NONE;
			PatternGroups[num2].MinSourceIndex = SourceLength;
			int num4 = SourceLength;
			while (num2 > 0)
			{
				switch (PatternGroups[num2].PatType)
				{
				case PatternType.STRING:
					num4 -= PatternGroups[num2].CharCount;
					break;
				case PatternType.DIGIT:
				case PatternType.ANYCHAR:
					num4 -= PatternGroups[num2].CharCount;
					break;
				case PatternType.EXCLIST:
				case PatternType.INCLIST:
					num4--;
					break;
				}
				PatternGroups[num2].MaxSourceIndex = num4;
				num2--;
			}
		}
	}

	private static void MatchAsterisk(string Source, int SourceLength, int SourceIndex, LigatureInfo[] SourceLigatureInfo, string Pattern, int PatternLength, int PatternIndex, LigatureInfo[] PattternLigatureInfo, ref bool Mismatch, ref bool PatternError, CompareInfo Comparer, CompareOptions Options)
	{
		Mismatch = false;
		PatternError = false;
		if (PatternIndex >= PatternLength)
		{
			return;
		}
		PatternGroup[] PatternGroups = null;
		int PGIndexForLastAsterisk = default(int);
		BuildPatternGroups(Source, SourceLength, ref SourceIndex, SourceLigatureInfo, Pattern, PatternLength, ref PatternIndex, PattternLigatureInfo, ref PatternError, ref PGIndexForLastAsterisk, Comparer, Options, ref PatternGroups);
		if (PatternError)
		{
			return;
		}
		checked
		{
			if (PatternGroups[PGIndexForLastAsterisk + 1].PatType != PatternType.NONE)
			{
				int num = SourceIndex;
				int num2 = PGIndexForLastAsterisk + 1;
				int num3 = default(int);
				do
				{
					num3 += PatternGroups[num2].CharCount;
					num2++;
				}
				while (PatternGroups[num2].PatType != PatternType.NONE);
				SourceIndex = SourceLength;
				SubtractChars(Source, SourceLength, ref SourceIndex, num3, SourceLigatureInfo, Options);
				MatchAsterisk(Source, SourceLength, SourceIndex, SourceLigatureInfo, Pattern, PattternLigatureInfo, PatternGroups, PGIndexForLastAsterisk, ref Mismatch, ref PatternError, Comparer, Options);
				if (PatternError || Mismatch)
				{
					return;
				}
				SourceLength = PatternGroups[PGIndexForLastAsterisk + 1].StartIndexOfPossibleMatch;
				if (SourceLength <= 0)
				{
					return;
				}
				PatternGroups[num2].MaxSourceIndex = SourceLength;
				PatternGroups[num2].MinSourceIndex = SourceLength;
				PatternGroups[num2].StartIndexOfPossibleMatch = 0;
				PatternGroups[PGIndexForLastAsterisk + 1] = PatternGroups[num2];
				PatternGroups[PGIndexForLastAsterisk].MinSourceIndex = 0;
				PatternGroups[PGIndexForLastAsterisk].StartIndexOfPossibleMatch = 0;
				num2 = PGIndexForLastAsterisk + 1;
				int num4 = SourceLength;
				while (num2 > 0)
				{
					switch (PatternGroups[num2].PatType)
					{
					case PatternType.STRING:
						num4 -= PatternGroups[num2].CharCount;
						break;
					case PatternType.DIGIT:
					case PatternType.ANYCHAR:
						num4 -= PatternGroups[num2].CharCount;
						break;
					case PatternType.EXCLIST:
					case PatternType.INCLIST:
						num4--;
						break;
					}
					PatternGroups[num2].MaxSourceIndex = num4;
					num2--;
				}
				SourceIndex = num;
			}
			MatchAsterisk(Source, SourceLength, SourceIndex, SourceLigatureInfo, Pattern, PattternLigatureInfo, PatternGroups, 0, ref Mismatch, ref PatternError, Comparer, Options);
		}
	}

	private static void MatchAsterisk(string Source, int SourceLength, int SourceIndex, LigatureInfo[] SourceLigatureInfo, string Pattern, LigatureInfo[] PatternLigatureInfo, PatternGroup[] PatternGroups, int PGIndex, ref bool Mismatch, ref bool PatternError, CompareInfo Comparer, CompareOptions Options)
	{
		int num = PGIndex;
		int num2 = SourceIndex;
		int num3 = -1;
		int num4 = -1;
		PatternGroups[PGIndex].MinSourceIndex = SourceIndex;
		PatternGroups[PGIndex].StartIndexOfPossibleMatch = SourceIndex;
		checked
		{
			PGIndex++;
			while (true)
			{
				PatternGroup pG = PatternGroups[PGIndex];
				switch (pG.PatType)
				{
				case PatternType.STRING:
				{
					int LeftEnd;
					while (true)
					{
						if (SourceIndex > pG.MaxSourceIndex)
						{
							Mismatch = true;
							return;
						}
						PatternGroups[PGIndex].StartIndexOfPossibleMatch = SourceIndex;
						int RightEnd = pG.StringPatternStart;
						int num5 = 0;
						LeftEnd = SourceIndex;
						bool flag = true;
						while (true)
						{
							int num6 = CompareChars(Source, SourceLength, LeftEnd, ref LeftEnd, SourceLigatureInfo, Pattern, pG.StringPatternEnd + 1, RightEnd, ref RightEnd, PatternLigatureInfo, Comparer, Options);
							if (flag)
							{
								flag = false;
								num5 = LeftEnd + 1;
							}
							if (num6 != 0)
							{
								break;
							}
							RightEnd++;
							LeftEnd++;
							if (RightEnd > pG.StringPatternEnd)
							{
								goto end_IL_0069;
							}
							if (LeftEnd >= SourceLength)
							{
								Mismatch = true;
								return;
							}
						}
						SourceIndex = num5;
						num = PGIndex - 1;
						num2 = SourceIndex;
						continue;
						end_IL_0069:
						break;
					}
					SourceIndex = LeftEnd;
					goto default;
				}
				case PatternType.DIGIT:
					while (true)
					{
						if (SourceIndex > pG.MaxSourceIndex)
						{
							Mismatch = true;
							return;
						}
						PatternGroups[PGIndex].StartIndexOfPossibleMatch = SourceIndex;
						int charCount = pG.CharCount;
						for (int i = 1; i <= charCount; i++)
						{
							char c = Source[SourceIndex];
							SourceIndex++;
							if (!char.IsDigit(c))
							{
								goto IL_0141;
							}
						}
						break;
						IL_0141:
						num = PGIndex - 1;
						num2 = SourceIndex;
					}
					goto default;
				case PatternType.EXCLIST:
				case PatternType.INCLIST:
					while (true)
					{
						if (SourceIndex > pG.MaxSourceIndex)
						{
							Mismatch = true;
							return;
						}
						PatternGroups[PGIndex].StartIndexOfPossibleMatch = SourceIndex;
						if (MatchRangeAfterAsterisk(Source, SourceLength, ref SourceIndex, SourceLigatureInfo, Pattern, PatternLigatureInfo, pG, Comparer, Options))
						{
							break;
						}
						num = PGIndex - 1;
						num2 = SourceIndex;
					}
					goto default;
				case PatternType.ANYCHAR:
				{
					if (SourceIndex > pG.MaxSourceIndex)
					{
						Mismatch = true;
						return;
					}
					PatternGroups[PGIndex].StartIndexOfPossibleMatch = SourceIndex;
					int charCount2 = pG.CharCount;
					for (int j = 1; j <= charCount2; j++)
					{
						if (SourceIndex >= SourceLength)
						{
							Mismatch = true;
							return;
						}
						SkipToEndOfExpandedChar(SourceLigatureInfo, SourceLength, ref SourceIndex);
						SourceIndex++;
					}
					goto default;
				}
				case PatternType.NONE:
					PatternGroups[PGIndex].StartIndexOfPossibleMatch = pG.MaxSourceIndex;
					if (SourceIndex < pG.MaxSourceIndex)
					{
						num = PGIndex - 1;
						num2 = pG.MaxSourceIndex;
					}
					if (PatternGroups[num].PatType != PatternType.STAR && PatternGroups[num].PatType != PatternType.NONE)
					{
						goto IL_0279;
					}
					return;
				case PatternType.STAR:
					PatternGroups[PGIndex].StartIndexOfPossibleMatch = SourceIndex;
					pG.MinSourceIndex = SourceIndex;
					if (PatternGroups[num].PatType != PatternType.STAR)
					{
						if (SourceIndex > pG.MaxSourceIndex)
						{
							Mismatch = true;
							return;
						}
						goto IL_0279;
					}
					goto IL_02dd;
				default:
					{
						if (PGIndex == num)
						{
							if (SourceIndex == num2)
							{
								SourceIndex = PatternGroups[num3].MinSourceIndex;
								PGIndex = num3;
								num = num3;
							}
							else if (SourceIndex < num2)
							{
								PatternGroups[num4].MinSourceIndex++;
								SourceIndex = PatternGroups[num4].MinSourceIndex;
								PGIndex = num4 + 1;
							}
							else
							{
								PGIndex++;
								num = num4;
							}
						}
						else
						{
							PGIndex++;
						}
						break;
					}
					IL_02dd:
					PGIndex++;
					break;
					IL_0279:
					num3 = PGIndex;
					SourceIndex = num2;
					PGIndex = num;
					do
					{
						SubtractChars(Source, SourceLength, ref SourceIndex, PatternGroups[PGIndex].CharCount, SourceLigatureInfo, Options);
						PGIndex--;
					}
					while (PatternGroups[PGIndex].PatType != PatternType.STAR);
					SourceIndex = Math.Max(SourceIndex, PatternGroups[PGIndex].MinSourceIndex + 1);
					PatternGroups[PGIndex].MinSourceIndex = SourceIndex;
					num4 = PGIndex;
					goto IL_02dd;
				}
			}
		}
	}

	private static bool MatchRangeAfterAsterisk(string Source, int SourceLength, ref int SourceIndex, LigatureInfo[] SourceLigatureInfo, string Pattern, LigatureInfo[] PatternLigatureInfo, PatternGroup PG, CompareInfo Comparer, CompareOptions Options)
	{
		List<Range> rangeList = PG.RangeList;
		int LeftEnd = SourceIndex;
		bool flag = false;
		checked
		{
			foreach (Range item in rangeList)
			{
				int num = 1;
				int RightEnd;
				if (PatternLigatureInfo != null && PatternLigatureInfo[item.Start].Kind == CharKind.ExpandedChar1)
				{
					int leftStart = SourceIndex;
					int rightLength = item.Start + item.StartLength;
					int start = item.Start;
					RightEnd = 0;
					if (CompareChars(Source, SourceLength, leftStart, ref LeftEnd, SourceLigatureInfo, Pattern, rightLength, start, ref RightEnd, PatternLigatureInfo, Comparer, Options, MatchBothCharsOfExpandedCharInRight: true) == 0)
					{
						flag = true;
						break;
					}
				}
				int leftStart2 = SourceIndex;
				int rightLength2 = item.Start + item.StartLength;
				int start2 = item.Start;
				RightEnd = 0;
				int num2 = CompareChars(Source, SourceLength, leftStart2, ref LeftEnd, SourceLigatureInfo, Pattern, rightLength2, start2, ref RightEnd, PatternLigatureInfo, Comparer, Options, MatchBothCharsOfExpandedCharInRight: false, UseUnexpandedCharForRight: true);
				if (num2 > 0 && item.End >= 0)
				{
					int leftStart3 = SourceIndex;
					int rightLength3 = item.End + item.EndLength;
					int end = item.End;
					RightEnd = 0;
					num = CompareChars(Source, SourceLength, leftStart3, ref LeftEnd, SourceLigatureInfo, Pattern, rightLength3, end, ref RightEnd, PatternLigatureInfo, Comparer, Options, MatchBothCharsOfExpandedCharInRight: false, UseUnexpandedCharForRight: true);
				}
				if (num2 == 0 || (num2 > 0 && num <= 0))
				{
					flag = true;
					break;
				}
			}
			if (PG.PatType == PatternType.EXCLIST)
			{
				flag = !flag;
			}
			SourceIndex = LeftEnd + 1;
			return flag;
		}
	}

	private static void SubtractChars(string Input, int InputLength, ref int Current, int CharsToSubtract, LigatureInfo[] InputLigatureInfo, CompareOptions Options)
	{
		checked
		{
			if (Options == CompareOptions.Ordinal)
			{
				Current -= CharsToSubtract;
				if (Current < 0)
				{
					Current = 0;
				}
				return;
			}
			for (int i = 1; i <= CharsToSubtract; i++)
			{
				SubtractOneCharInTextCompareMode(Input, InputLength, ref Current, InputLigatureInfo, Options);
				if (Current < 0)
				{
					Current = 0;
					break;
				}
			}
		}
	}

	private static void SubtractOneCharInTextCompareMode(string Input, int InputLength, ref int Current, LigatureInfo[] InputLigatureInfo, CompareOptions Options)
	{
		checked
		{
			if (Current >= InputLength)
			{
				Current--;
			}
			else if (InputLigatureInfo != null && InputLigatureInfo[Current].Kind == CharKind.ExpandedChar2)
			{
				Current -= 2;
			}
			else
			{
				Current--;
			}
		}
	}
}
