using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class StringType
{
	public static string FromBoolean(bool Value)
	{
		if (Value)
		{
			return bool.TrueString;
		}
		return bool.FalseString;
	}

	public static string FromByte(byte Value)
	{
		return Value.ToString(null, null);
	}

	public static string FromChar(char Value)
	{
		return Value.ToString();
	}

	public static string FromShort(short Value)
	{
		return Value.ToString(null, null);
	}

	public static string FromInteger(int Value)
	{
		return Value.ToString(null, null);
	}

	public static string FromLong(long Value)
	{
		return Value.ToString(null, null);
	}

	public static string FromSingle(float Value)
	{
		return FromSingle(Value, null);
	}

	public static string FromDouble(double Value)
	{
		return FromDouble(Value, null);
	}

	public static string FromSingle(float Value, NumberFormatInfo NumberFormat)
	{
		return Value.ToString(null, NumberFormat);
	}

	public static string FromDouble(double Value, NumberFormatInfo NumberFormat)
	{
		return Value.ToString("G", NumberFormat);
	}

	public static string FromDate(DateTime Value)
	{
		long ticks = Value.TimeOfDay.Ticks;
		if (ticks == Value.Ticks || (Value.Year == 1899 && Value.Month == 12 && Value.Day == 30))
		{
			return Value.ToString("T", null);
		}
		if (ticks == 0L)
		{
			return Value.ToString("d", null);
		}
		return Value.ToString("G", null);
	}

	public static string FromDecimal(decimal Value)
	{
		return FromDecimal(Value, null);
	}

	public static string FromDecimal(decimal Value, NumberFormatInfo NumberFormat)
	{
		return Value.ToString("G", NumberFormat);
	}

	public static string FromObject(object Value)
	{
		if (Value == null)
		{
			return null;
		}
		if (Value is string result)
		{
			return result;
		}
		if (Value is IConvertible convertible)
		{
			switch (convertible.GetTypeCode())
			{
			case TypeCode.Boolean:
				return FromBoolean(convertible.ToBoolean(null));
			case TypeCode.Byte:
				return FromByte(convertible.ToByte(null));
			case TypeCode.Int16:
				return FromShort(convertible.ToInt16(null));
			case TypeCode.Int32:
				return FromInteger(convertible.ToInt32(null));
			case TypeCode.Int64:
				return FromLong(convertible.ToInt64(null));
			case TypeCode.Single:
				return FromSingle(convertible.ToSingle(null));
			case TypeCode.Double:
				return FromDouble(convertible.ToDouble(null));
			case TypeCode.Decimal:
				return FromDecimal(convertible.ToDecimal(null));
			case TypeCode.String:
				return convertible.ToString(null);
			case TypeCode.Char:
				return FromChar(convertible.ToChar(null));
			case TypeCode.DateTime:
				return FromDate(convertible.ToDateTime(null));
			}
		}
		else if (Value is char[] { Rank: 1 })
		{
			return new string(CharArrayType.FromObject(Value));
		}
		throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "String"));
	}

	public static int StrCmp(string sLeft, string sRight, bool TextCompare)
	{
		if ((object)sLeft == sRight)
		{
			return 0;
		}
		if (sLeft == null)
		{
			if (sRight.Length == 0)
			{
				return 0;
			}
			return -1;
		}
		if (sRight == null)
		{
			if (sLeft.Length == 0)
			{
				return 0;
			}
			return 1;
		}
		if (TextCompare)
		{
			return Utils.GetCultureInfo().CompareInfo.Compare(sLeft, sRight, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
		}
		return string.CompareOrdinal(sLeft, sRight);
	}

	public static bool StrLike(string Source, string Pattern, CompareMethod CompareOption)
	{
		if (CompareOption == CompareMethod.Binary)
		{
			return StrLikeBinary(Source, Pattern);
		}
		return StrLikeText(Source, Pattern);
	}

	public static bool StrLikeBinary(string Source, string Pattern)
	{
		bool flag = false;
		int num = Pattern?.Length ?? 0;
		int num2 = Source?.Length ?? 0;
		int num3 = default(int);
		char c = default(char);
		if (num3 < num2)
		{
			c = Source[num3];
		}
		checked
		{
			int i = default(int);
			bool flag2 = default(bool);
			bool flag3 = default(bool);
			bool flag4 = default(bool);
			bool flag5 = default(bool);
			bool flag6 = default(bool);
			char c3 = default(char);
			for (; i < num; i++)
			{
				char c2 = Pattern[i];
				if (c2 == '*' && !flag2)
				{
					int num4 = AsteriskSkip(Pattern.Substring(i + 1), Source.Substring(num3), num2 - num3, CompareMethod.Binary, Strings.m_InvariantCompareInfo);
					if (num4 < 0)
					{
						return false;
					}
					if (num4 > 0)
					{
						num3 += num4;
						if (num3 < num2)
						{
							c = Source[num3];
						}
					}
					continue;
				}
				if (c2 == '?' && !flag2)
				{
					num3++;
					if (num3 < num2)
					{
						c = Source[num3];
					}
					continue;
				}
				if (c2 == '#' && !flag2)
				{
					if (!char.IsDigit(c))
					{
						break;
					}
					num3++;
					if (num3 < num2)
					{
						c = Source[num3];
					}
					continue;
				}
				if (c2 == '-' && flag2 && flag3 && !flag && !flag4 && (i + 1 >= num || Pattern[i + 1] != ']'))
				{
					flag4 = true;
					continue;
				}
				if (c2 == '!' && flag2 && !flag5)
				{
					flag5 = true;
					flag6 = true;
					continue;
				}
				if (c2 == '[' && !flag2)
				{
					flag2 = true;
					c3 = '\0';
					char c4 = '\0';
					flag3 = false;
					continue;
				}
				if (c2 == ']' && flag2)
				{
					flag2 = false;
					if (flag3)
					{
						if (!flag6)
						{
							break;
						}
						num3++;
						if (num3 < num2)
						{
							c = Source[num3];
						}
					}
					else if (flag4)
					{
						if (!flag6)
						{
							break;
						}
					}
					else if (flag5)
					{
						if ('!' != c)
						{
							break;
						}
						num3++;
						if (num3 < num2)
						{
							c = Source[num3];
						}
					}
					flag6 = false;
					flag3 = false;
					flag5 = false;
					flag4 = false;
					continue;
				}
				flag3 = true;
				flag = false;
				if (flag2)
				{
					if (flag4)
					{
						flag4 = false;
						flag = true;
						char c4 = c2;
						if (c3 > c4)
						{
							throw ExceptionUtils.VbMakeException(93);
						}
						if ((flag5 && flag6) || (!flag5 && !flag6))
						{
							flag6 = c > c3 && c <= c4;
							if (flag5)
							{
								flag6 = !flag6;
							}
						}
					}
					else
					{
						c3 = c2;
						flag6 = StrLikeCompareBinary(flag5, flag6, c2, c);
					}
				}
				else
				{
					if (c2 != c && !flag5)
					{
						break;
					}
					flag5 = false;
					num3++;
					if (num3 < num2)
					{
						c = Source[num3];
					}
					else if (num3 > num2)
					{
						return false;
					}
				}
			}
			if (flag2)
			{
				if (num2 == 0)
				{
					return false;
				}
				throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Pattern"));
			}
			return i == num && num3 == num2;
		}
	}

	public static bool StrLikeText(string Source, string Pattern)
	{
		bool flag = false;
		int num = Pattern?.Length ?? 0;
		int num2 = Source?.Length ?? 0;
		int i = default(int);
		char c = default(char);
		if (i < num2)
		{
			c = Source[i];
		}
		CompareInfo compareInfo = Utils.GetCultureInfo().CompareInfo;
		CompareOptions compareOptions = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth;
		checked
		{
			int j = default(int);
			bool flag2 = default(bool);
			bool flag3 = default(bool);
			bool flag4 = default(bool);
			bool flag5 = default(bool);
			bool flag6 = default(bool);
			char c3 = default(char);
			for (; j < num; j++)
			{
				char c2 = Pattern[j];
				if (c2 == '*' && !flag2)
				{
					int num3 = AsteriskSkip(Pattern.Substring(j + 1), Source.Substring(i), num2 - i, CompareMethod.Text, compareInfo);
					if (num3 < 0)
					{
						return false;
					}
					if (num3 > 0)
					{
						i += num3;
						if (i < num2)
						{
							c = Source[i];
						}
					}
					continue;
				}
				if (c2 == '?' && !flag2)
				{
					i++;
					if (i < num2)
					{
						c = Source[i];
					}
					continue;
				}
				if (c2 == '#' && !flag2)
				{
					if (!char.IsDigit(c))
					{
						break;
					}
					i++;
					if (i < num2)
					{
						c = Source[i];
					}
					continue;
				}
				if (c2 == '-' && flag2 && flag3 && !flag && !flag4 && (j + 1 >= num || Pattern[j + 1] != ']'))
				{
					flag4 = true;
					continue;
				}
				if (c2 == '!' && flag2 && !flag5)
				{
					flag5 = true;
					flag6 = true;
					continue;
				}
				if (c2 == '[' && !flag2)
				{
					flag2 = true;
					c3 = '\0';
					char c4 = '\0';
					flag3 = false;
					continue;
				}
				if (c2 == ']' && flag2)
				{
					flag2 = false;
					if (flag3)
					{
						if (!flag6)
						{
							break;
						}
						i++;
						if (i < num2)
						{
							c = Source[i];
						}
					}
					else if (flag4)
					{
						if (!flag6)
						{
							break;
						}
					}
					else if (flag5)
					{
						if (compareInfo.Compare("!", Conversions.ToString(c)) != 0)
						{
							break;
						}
						i++;
						if (i < num2)
						{
							c = Source[i];
						}
					}
					flag6 = false;
					flag3 = false;
					flag5 = false;
					flag4 = false;
					continue;
				}
				flag3 = true;
				flag = false;
				if (flag2)
				{
					if (flag4)
					{
						flag4 = false;
						flag = true;
						char c4 = c2;
						if (c3 > c4)
						{
							throw ExceptionUtils.VbMakeException(93);
						}
						if ((flag5 && flag6) || (!flag5 && !flag6))
						{
							flag6 = ((compareOptions != CompareOptions.Ordinal) ? (compareInfo.Compare(Conversions.ToString(c3), Conversions.ToString(c), compareOptions) < 0 && compareInfo.Compare(Conversions.ToString(c4), Conversions.ToString(c), compareOptions) >= 0) : (c > c3 && c <= c4));
							if (flag5)
							{
								flag6 = !flag6;
							}
						}
					}
					else
					{
						c3 = c2;
						flag6 = StrLikeCompare(compareInfo, flag5, flag6, c2, c, compareOptions);
					}
					continue;
				}
				if (compareOptions == CompareOptions.Ordinal)
				{
					if (c2 != c && !flag5)
					{
						break;
					}
				}
				else
				{
					string text = Conversions.ToString(c2);
					string text2 = Conversions.ToString(c);
					for (; j + 1 < num && (UnicodeCategory.ModifierSymbol == char.GetUnicodeCategory(Pattern[j + 1]) || UnicodeCategory.NonSpacingMark == char.GetUnicodeCategory(Pattern[j + 1])); j++)
					{
						text += Conversions.ToString(Pattern[j + 1]);
					}
					for (; i + 1 < num2 && (UnicodeCategory.ModifierSymbol == char.GetUnicodeCategory(Source[i + 1]) || UnicodeCategory.NonSpacingMark == char.GetUnicodeCategory(Source[i + 1])); i++)
					{
						text2 += Conversions.ToString(Source[i + 1]);
					}
					if (compareInfo.Compare(text, text2, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) != 0 && !flag5)
					{
						break;
					}
				}
				flag5 = false;
				i++;
				if (i < num2)
				{
					c = Source[i];
				}
				else if (i > num2)
				{
					return false;
				}
			}
			if (flag2)
			{
				if (num2 == 0)
				{
					return false;
				}
				throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Pattern"));
			}
			return j == num && i == num2;
		}
	}

	private static bool StrLikeCompareBinary(bool SeenNot, bool Match, char p, char s)
	{
		if (SeenNot && Match)
		{
			return p != s;
		}
		if (!SeenNot && !Match)
		{
			return p == s;
		}
		return Match;
	}

	private static bool StrLikeCompare(CompareInfo ci, bool SeenNot, bool Match, char p, char s, CompareOptions Options)
	{
		if (SeenNot && Match)
		{
			if (Options == CompareOptions.Ordinal)
			{
				return p != s;
			}
			return ci.Compare(Conversions.ToString(p), Conversions.ToString(s), Options) != 0;
		}
		if (!SeenNot && !Match)
		{
			if (Options == CompareOptions.Ordinal)
			{
				return p == s;
			}
			return ci.Compare(Conversions.ToString(p), Conversions.ToString(s), Options) == 0;
		}
		return Match;
	}

	private static int AsteriskSkip(string Pattern, string Source, int SourceEndIndex, CompareMethod CompareOption, CompareInfo ci)
	{
		checked
		{
			int i = default(int);
			bool flag3 = default(bool);
			int num2 = default(int);
			bool flag2 = default(bool);
			bool flag = default(bool);
			for (int num = Strings.Len(Pattern); i < num; i++)
			{
				switch (Pattern[i])
				{
				case '*':
					if (num2 > 0)
					{
						if (flag3)
						{
							num2 = MultipleAsteriskSkip(Pattern, Source, num2, CompareOption);
							return SourceEndIndex - num2;
						}
						string value = Pattern.Substring(0, i);
						CompareOptions options = ((CompareOption != CompareMethod.Binary) ? (CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) : CompareOptions.Ordinal);
						return ci.LastIndexOf(Source, value, options);
					}
					break;
				case '-':
					if (Pattern[i + 1] == ']')
					{
						flag2 = true;
					}
					break;
				case '!':
					if (Pattern[i + 1] == ']')
					{
						flag2 = true;
					}
					else
					{
						flag3 = true;
					}
					break;
				case '[':
					if (flag)
					{
						flag2 = true;
					}
					else
					{
						flag = true;
					}
					break;
				case ']':
					if (flag2 || !flag)
					{
						num2++;
						flag3 = true;
					}
					flag2 = false;
					flag = false;
					break;
				case '#':
				case '?':
					if (flag)
					{
						flag2 = true;
						break;
					}
					num2++;
					flag3 = true;
					break;
				default:
					if (flag)
					{
						flag2 = true;
					}
					else
					{
						num2++;
					}
					break;
				}
			}
			return SourceEndIndex - num2;
		}
	}

	private static int MultipleAsteriskSkip(string Pattern, string Source, int Count, CompareMethod CompareOption)
	{
		int num = Strings.Len(Source);
		checked
		{
			while (Count < num)
			{
				string source = Source.Substring(num - Count);
				bool flag;
				try
				{
					flag = StrLike(source, Pattern, CompareOption);
				}
				catch (StackOverflowException ex)
				{
					throw ex;
				}
				catch (OutOfMemoryException ex2)
				{
					throw ex2;
				}
				catch (Exception)
				{
					flag = false;
				}
				if (flag)
				{
					break;
				}
				Count++;
			}
			return Count;
		}
	}

	public static void MidStmtStr(ref string sDest, int StartPosition, int MaxInsertLength, string sInsert)
	{
		int length = default(int);
		if (sDest != null)
		{
			length = sDest.Length;
		}
		int num = default(int);
		if (sInsert != null)
		{
			num = sInsert.Length;
		}
		checked
		{
			StartPosition--;
			if (StartPosition < 0 || StartPosition >= length)
			{
				throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Start"));
			}
			if (MaxInsertLength < 0)
			{
				throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Length"));
			}
			if (num > MaxInsertLength)
			{
				num = MaxInsertLength;
			}
			if (num > length - StartPosition)
			{
				num = length - StartPosition;
			}
			if (num != 0)
			{
				StringBuilder stringBuilder = new StringBuilder(length);
				if (StartPosition > 0)
				{
					stringBuilder.Append(sDest, 0, StartPosition);
				}
				stringBuilder.Append(sInsert, 0, num);
				int num2 = length - (StartPosition + num);
				if (num2 > 0)
				{
					stringBuilder.Append(sDest, StartPosition + num, num2);
				}
				sDest = stringBuilder.ToString();
			}
		}
	}
}
