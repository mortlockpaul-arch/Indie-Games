using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.VisualBasic;

[StandardModule]
public sealed class Strings
{
	internal enum FormatType
	{
		Number,
		Percent,
		Currency
	}

	private static readonly string[] CurrencyPositiveFormatStrings = new string[4] { "'$'n", "n'$'", "'$' n", "n '$'" };

	private static readonly string[] CurrencyNegativeFormatStrings = new string[16]
	{
		"('$'n)", "-'$'n", "'$'-n", "'$'n-", "(n'$')", "-n'$'", "n-'$'", "n'$'-", "-n '$'", "-'$' n",
		"n '$'-", "'$' n-", "'$'- n", "n- '$'", "('$' n)", "(n '$')"
	};

	private static readonly string[] NumberNegativeFormatStrings = new string[5] { "(n)", "-n", "- n", "n-", "n -" };

	internal static readonly CompareInfo m_InvariantCompareInfo = CultureInfo.InvariantCulture.CompareInfo;

	private static object m_SyncObject = new object();

	private static CultureInfo m_LastUsedYesNoCulture;

	private static string m_CachedYesNoFormatStyle;

	private static CultureInfo m_LastUsedOnOffCulture;

	private static string m_CachedOnOffFormatStyle;

	private static CultureInfo m_LastUsedTrueFalseCulture;

	private static string m_CachedTrueFalseFormatStyle;

	private static string CachedYesNoFormatStyle
	{
		get
		{
			CultureInfo cultureInfo = Utils.GetCultureInfo();
			object syncObject = m_SyncObject;
			ObjectFlowControl.CheckForSyncLockOnValueType(syncObject);
			bool lockTaken = false;
			try
			{
				Monitor.Enter(syncObject, ref lockTaken);
				if (m_LastUsedYesNoCulture != cultureInfo)
				{
					m_LastUsedYesNoCulture = cultureInfo;
					m_CachedYesNoFormatStyle = System.SR.YesNoFormatStyle;
				}
				return m_CachedYesNoFormatStyle;
			}
			finally
			{
				if (lockTaken)
				{
					Monitor.Exit(syncObject);
				}
			}
		}
	}

	private static string CachedOnOffFormatStyle
	{
		get
		{
			CultureInfo cultureInfo = Utils.GetCultureInfo();
			object syncObject = m_SyncObject;
			ObjectFlowControl.CheckForSyncLockOnValueType(syncObject);
			bool lockTaken = false;
			try
			{
				Monitor.Enter(syncObject, ref lockTaken);
				if (m_LastUsedOnOffCulture != cultureInfo)
				{
					m_LastUsedOnOffCulture = cultureInfo;
					m_CachedOnOffFormatStyle = System.SR.OnOffFormatStyle;
				}
				return m_CachedOnOffFormatStyle;
			}
			finally
			{
				if (lockTaken)
				{
					Monitor.Exit(syncObject);
				}
			}
		}
	}

	private static string CachedTrueFalseFormatStyle
	{
		get
		{
			CultureInfo cultureInfo = Utils.GetCultureInfo();
			object syncObject = m_SyncObject;
			ObjectFlowControl.CheckForSyncLockOnValueType(syncObject);
			bool lockTaken = false;
			try
			{
				Monitor.Enter(syncObject, ref lockTaken);
				if (m_LastUsedTrueFalseCulture != cultureInfo)
				{
					m_LastUsedTrueFalseCulture = cultureInfo;
					m_CachedTrueFalseFormatStyle = System.SR.TrueFalseFormatStyle;
				}
				return m_CachedTrueFalseFormatStyle;
			}
			finally
			{
				if (lockTaken)
				{
					Monitor.Exit(syncObject);
				}
			}
		}
	}

	private static int PRIMARYLANGID(int lcid)
	{
		return lcid & 0x3FF;
	}

	private static Encoding GetAscChrEncoding()
	{
		return Encoding.GetEncoding(Utils.GetLocaleCodePage());
	}

	public static int Asc(char String)
	{
		int num = Convert.ToInt32(String);
		if (num < 128)
		{
			return num;
		}
		try
		{
			Encoding ascChrEncoding = GetAscChrEncoding();
			char[] chars = new char[1] { String };
			byte[] array;
			if (ascChrEncoding.IsSingleByte)
			{
				array = new byte[1];
				ascChrEncoding.GetBytes(chars, 0, 1, array, 0);
				return array[0];
			}
			array = new byte[2];
			if (ascChrEncoding.GetBytes(chars, 0, 1, array, 0) == 1)
			{
				return array[0];
			}
			if (BitConverter.IsLittleEndian)
			{
				byte b = array[0];
				array[0] = array[1];
				array[1] = b;
			}
			return BitConverter.ToInt16(array, 0);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static int Asc(string String)
	{
		if (String == null || String.Length == 0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_LengthGTZero1, "String"), "String");
		}
		return Asc(String[0]);
	}

	public static int AscW(string String)
	{
		if (String == null || String.Length == 0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_LengthGTZero1, "String"), "String");
		}
		return String[0];
	}

	public static int AscW(char String)
	{
		return String;
	}

	public static char Chr(int CharCode)
	{
		if (CharCode < -32768 || CharCode > 65535)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_RangeTwoBytes1, "CharCode"), "CharCode");
		}
		if (CharCode >= 0 && CharCode <= 127)
		{
			return Convert.ToChar(CharCode);
		}
		checked
		{
			try
			{
				Encoding ascChrEncoding = GetAscChrEncoding();
				if (ascChrEncoding.IsSingleByte && (CharCode < 0 || CharCode > 255))
				{
					throw ExceptionUtils.VbMakeException(5);
				}
				char[] array = new char[2];
				byte[] array2 = new byte[2];
				Decoder decoder = ascChrEncoding.GetDecoder();
				if (CharCode >= 0 && CharCode <= 255)
				{
					array2[0] = (byte)(CharCode & 0xFF);
					decoder.GetChars(array2, 0, 1, array, 0);
				}
				else
				{
					array2[0] = (byte)((CharCode & 0xFF00) >> 8);
					array2[1] = (byte)(CharCode & 0xFF);
					decoder.GetChars(array2, 0, 2, array, 0);
				}
				return array[0];
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}

	public static char ChrW(int CharCode)
	{
		if (CharCode < -32768 || CharCode > 65535)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_RangeTwoBytes1, "CharCode"), "CharCode");
		}
		return Convert.ToChar(CharCode & 0xFFFF);
	}

	public static string[] Filter(object[] Source, string Match, bool Include = true, [OptionCompare] CompareMethod Compare = CompareMethod.Binary)
	{
		int num = Information.UBound(Source);
		checked
		{
			string[] array = new string[num + 1];
			try
			{
				int num2 = num;
				for (int i = 0; i <= num2; i++)
				{
					array[i] = Conversions.ToString(Source[i]);
				}
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
				throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValueType2, "Source", "String"), "Source");
			}
			return Filter(array, Match, Include, Compare);
		}
	}

	public static string[] Filter(string[] Source, string Match, bool Include = true, [OptionCompare] CompareMethod Compare = CompareMethod.Binary)
	{
		checked
		{
			try
			{
				if (Source.Rank != 1)
				{
					throw new ArgumentException(System.SR.Argument_RankEQOne1, "Source");
				}
				if (Match == null || Match.Length == 0)
				{
					return null;
				}
				int num = Source.Length;
				CompareInfo compareInfo = Utils.GetCultureInfo().CompareInfo;
				CompareOptions options = default(CompareOptions);
				if (Compare == CompareMethod.Text)
				{
					options = CompareOptions.IgnoreCase;
				}
				string[] array = new string[num - 1 + 1];
				int num2 = num - 1;
				int num3 = default(int);
				for (int i = 0; i <= num2; i++)
				{
					string text = Source[i];
					if (text != null && compareInfo.IndexOf(text, Match, options) >= 0 == Include)
					{
						array[num3] = text;
						num3++;
					}
				}
				if (num3 == 0)
				{
					return new string[0];
				}
				if (num3 == array.Length)
				{
					return array;
				}
				return (string[])Utils.CopyArray(array, new string[num3 - 1 + 1]);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}

	public static int InStr(string String1, string String2, [OptionCompare] CompareMethod Compare = CompareMethod.Binary)
	{
		checked
		{
			if (Compare == CompareMethod.Binary)
			{
				return InternalInStrBinary(0, String1, String2) + 1;
			}
			return InternalInStrText(0, String1, String2) + 1;
		}
	}

	public static int InStr(int Start, string String1, string String2, [OptionCompare] CompareMethod Compare = CompareMethod.Binary)
	{
		if (Start < 1)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_GTZero1, "Start"), "Start");
		}
		checked
		{
			if (Compare == CompareMethod.Binary)
			{
				return InternalInStrBinary(Start - 1, String1, String2) + 1;
			}
			return InternalInStrText(Start - 1, String1, String2) + 1;
		}
	}

	private static int InternalInStrBinary(int StartPos, string sSrc, string sFind)
	{
		int num = sSrc?.Length ?? 0;
		if (StartPos > num || num == 0)
		{
			return -1;
		}
		if (sFind == null || sFind.Length == 0)
		{
			return StartPos;
		}
		return m_InvariantCompareInfo.IndexOf(sSrc, sFind, StartPos, CompareOptions.Ordinal);
	}

	private static int InternalInStrText(int lStartPos, string sSrc, string sFind)
	{
		int num = sSrc?.Length ?? 0;
		if (lStartPos > num || num == 0)
		{
			return -1;
		}
		if (sFind == null || sFind.Length == 0)
		{
			return lStartPos;
		}
		return Utils.GetCultureInfo().CompareInfo.IndexOf(sSrc, sFind, lStartPos, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
	}

	public static int InStrRev(string StringCheck, string StringMatch, int Start = -1, [OptionCompare] CompareMethod Compare = CompareMethod.Binary)
	{
		checked
		{
			try
			{
				if (Start == 0 || Start < -1)
				{
					throw new ArgumentException(System.SR.Format(System.SR.Argument_MinusOneOrGTZero1, "Start"), "Start");
				}
				int num = StringCheck?.Length ?? 0;
				if (Start == -1)
				{
					Start = num;
				}
				if (Start > num || num == 0)
				{
					return 0;
				}
				if (StringMatch == null || StringMatch.Length == 0)
				{
					return Start;
				}
				if (Compare == CompareMethod.Binary)
				{
					return m_InvariantCompareInfo.LastIndexOf(StringCheck, StringMatch, Start - 1, Start, CompareOptions.Ordinal) + 1;
				}
				return Utils.GetCultureInfo().CompareInfo.LastIndexOf(StringCheck, StringMatch, Start - 1, Start, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) + 1;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}

	public static string Join(object[] SourceArray, string Delimiter = " ")
	{
		int num = Information.UBound(SourceArray);
		checked
		{
			string[] array = new string[num + 1];
			try
			{
				int num2 = num;
				for (int i = 0; i <= num2; i++)
				{
					array[i] = Conversions.ToString(SourceArray[i]);
				}
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
				throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValueType2, "SourceArray", "String"));
			}
			return Join(array, Delimiter);
		}
	}

	public static string Join(string[] SourceArray, string Delimiter = " ")
	{
		try
		{
			if (IsArrayEmpty(SourceArray))
			{
				return null;
			}
			if (SourceArray.Rank != 1)
			{
				throw new ArgumentException(System.SR.Format(System.SR.Argument_RankEQOne1));
			}
			return string.Join(Delimiter, SourceArray);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static string LCase(string Value)
	{
		try
		{
			if (Value == null)
			{
				return null;
			}
			return Thread.CurrentThread.CurrentCulture.TextInfo.ToLower(Value);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static char LCase(char Value)
	{
		try
		{
			return Thread.CurrentThread.CurrentCulture.TextInfo.ToLower(Value);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static int Len(bool Expression)
	{
		return 2;
	}

	[CLSCompliant(false)]
	public static int Len(sbyte Expression)
	{
		return 1;
	}

	public static int Len(byte Expression)
	{
		return 1;
	}

	public static int Len(short Expression)
	{
		return 2;
	}

	[CLSCompliant(false)]
	public static int Len(ushort Expression)
	{
		return 2;
	}

	public static int Len(int Expression)
	{
		return 4;
	}

	[CLSCompliant(false)]
	public static int Len(uint Expression)
	{
		return 4;
	}

	public static int Len(long Expression)
	{
		return 8;
	}

	[CLSCompliant(false)]
	public static int Len(ulong Expression)
	{
		return 8;
	}

	public static int Len(decimal Expression)
	{
		return 8;
	}

	public static int Len(float Expression)
	{
		return 4;
	}

	public static int Len(double Expression)
	{
		return 8;
	}

	public static int Len(DateTime Expression)
	{
		return 8;
	}

	public static int Len(char Expression)
	{
		return 2;
	}

	public static int Len(string Expression)
	{
		return Expression?.Length ?? 0;
	}

	[RequiresUnreferencedCode("The object's type cannot be statically analyzed and its members may be trimmed")]
	public static int Len(object Expression)
	{
		if (Expression == null)
		{
			return 0;
		}
		if (Expression is IConvertible convertible)
		{
			switch (convertible.GetTypeCode())
			{
			case TypeCode.Boolean:
				return 2;
			case TypeCode.SByte:
				return 1;
			case TypeCode.Byte:
				return 1;
			case TypeCode.Int16:
				return 2;
			case TypeCode.UInt16:
				return 2;
			case TypeCode.Int32:
				return 4;
			case TypeCode.UInt32:
				return 4;
			case TypeCode.Int64:
				return 8;
			case TypeCode.UInt64:
				return 8;
			case TypeCode.Decimal:
				return 16;
			case TypeCode.Single:
				return 4;
			case TypeCode.Double:
				return 8;
			case TypeCode.DateTime:
				return 8;
			case TypeCode.Char:
				return 2;
			case TypeCode.String:
				return Expression.ToString().Length;
			}
		}
		else if (Expression is char[] array)
		{
			return array.Length;
		}
		if (Expression is ValueType)
		{
			return StructUtils.GetRecordLength(Expression, 1);
		}
		throw ExceptionUtils.VbMakeException(13);
	}

	public static string Replace(string Expression, string Find, string Replacement, int Start = 1, int Count = -1, [OptionCompare] CompareMethod Compare = CompareMethod.Binary)
	{
		try
		{
			if (Count < -1)
			{
				throw new ArgumentException(System.SR.Format(System.SR.Argument_GEMinusOne1, "Count"));
			}
			if (Start <= 0)
			{
				throw new ArgumentException(System.SR.Format(System.SR.Argument_GTZero1, "Start"));
			}
			if (Expression == null || Start > Expression.Length)
			{
				return null;
			}
			if (Start != 1)
			{
				Expression = Expression.Substring(checked(Start - 1));
			}
			if (Find == null || Find.Length == 0 || Count == 0)
			{
				return Expression;
			}
			if (Count == -1)
			{
				Count = Expression.Length;
			}
			return ReplaceInternal(Expression, Find, Replacement, Count, Compare);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	private static string ReplaceInternal(string Expression, string Find, string Replacement, int Count, CompareMethod Compare)
	{
		int length = Expression.Length;
		int length2 = Find.Length;
		StringBuilder stringBuilder = new StringBuilder(length);
		CompareInfo compareInfo;
		CompareOptions options;
		if (Compare == CompareMethod.Text)
		{
			compareInfo = Utils.GetCultureInfo().CompareInfo;
			options = CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth;
		}
		else
		{
			compareInfo = m_InvariantCompareInfo;
			options = CompareOptions.Ordinal;
		}
		checked
		{
			int num = default(int);
			int num2 = default(int);
			while (num < length)
			{
				if (num2 == Count)
				{
					stringBuilder.Append(Expression.Substring(num));
					break;
				}
				int num3 = compareInfo.IndexOf(Expression, Find, num, options);
				if (num3 < 0)
				{
					stringBuilder.Append(Expression.Substring(num));
					break;
				}
				stringBuilder.Append(Expression.Substring(num, num3 - num));
				stringBuilder.Append(Replacement);
				num2++;
				num = num3 + length2;
			}
			return stringBuilder.ToString();
		}
	}

	public static string Space(int Number)
	{
		if (Number >= 0)
		{
			return new string(' ', Number);
		}
		throw new ArgumentException(System.SR.Format(System.SR.Argument_GEZero1, "Number"));
	}

	public static string[] Split(string Expression, string Delimiter = " ", int Limit = -1, [OptionCompare] CompareMethod Compare = CompareMethod.Binary)
	{
		try
		{
			if (Expression == null || Expression.Length == 0)
			{
				return new string[1] { "" };
			}
			if (Limit == -1)
			{
				Limit = checked(Expression.Length + 1);
			}
			if ((Delimiter?.Length ?? 0) == 0)
			{
				return new string[1] { Expression };
			}
			return SplitHelper(Expression, Delimiter, Limit, (int)Compare);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	private static string[] SplitHelper(string sSrc, string sFind, int cMaxSubStrings, int Compare)
	{
		int num = sFind?.Length ?? 0;
		int num2 = sSrc?.Length ?? 0;
		if (num == 0)
		{
			return new string[1] { sSrc };
		}
		if (num2 == 0)
		{
			return new string[1] { sSrc };
		}
		int num3 = 20;
		if (num3 > cMaxSubStrings)
		{
			num3 = cMaxSubStrings;
		}
		checked
		{
			string[] array = new string[num3 + 1];
			CompareOptions options;
			CompareInfo compareInfo;
			if (Compare == 0)
			{
				options = CompareOptions.Ordinal;
				compareInfo = m_InvariantCompareInfo;
			}
			else
			{
				compareInfo = Utils.GetCultureInfo().CompareInfo;
				options = CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth;
			}
			int num4 = default(int);
			int num6 = default(int);
			while (num4 < num2)
			{
				int num5 = compareInfo.IndexOf(sSrc, sFind, num4, num2 - num4, options);
				string text;
				if (num5 == -1 || num6 + 1 == cMaxSubStrings)
				{
					text = sSrc.Substring(num4);
					if (text == null)
					{
						text = "";
					}
					array[num6] = text;
					break;
				}
				text = sSrc.Substring(num4, num5 - num4);
				if (text == null)
				{
					text = "";
				}
				array[num6] = text;
				num4 = num5 + num;
				num6++;
				if (num6 > num3)
				{
					num3 += 20;
					if (num3 > cMaxSubStrings)
					{
						num3 = cMaxSubStrings + 1;
					}
					array = (string[])Utils.CopyArray(array, new string[num3 + 1]);
				}
				array[num6] = "";
				if (num6 == cMaxSubStrings)
				{
					text = sSrc.Substring(num4);
					if (text == null)
					{
						text = "";
					}
					array[num6] = text;
					break;
				}
			}
			if (num6 + 1 == array.Length)
			{
				return array;
			}
			return (string[])Utils.CopyArray(array, new string[num6 + 1]);
		}
	}

	public static string LSet(string Source, int Length)
	{
		if (Length == 0)
		{
			return "";
		}
		if (Source == null)
		{
			return new string(' ', Length);
		}
		if (Length > Source.Length)
		{
			return Source.PadRight(Length);
		}
		return Source.Substring(0, Length);
	}

	public static string RSet(string Source, int Length)
	{
		if (Length == 0)
		{
			return "";
		}
		if (Source == null)
		{
			return new string(' ', Length);
		}
		if (Length > Source.Length)
		{
			return Source.PadLeft(Length);
		}
		return Source.Substring(0, Length);
	}

	public static object StrDup(int Number, object Character)
	{
		if (Number < 0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Number"));
		}
		if (Character == null)
		{
			throw new ArgumentNullException(System.SR.Format(System.SR.Argument_InvalidNullValue1, "Character"));
		}
		char c;
		if (Character is string text)
		{
			if (text.Length == 0)
			{
				throw new ArgumentException(System.SR.Format(System.SR.Argument_LengthGTZero1, "Character"));
			}
			c = text[0];
		}
		else
		{
			try
			{
				c = Conversions.ToChar(Character);
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
				throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Character"));
			}
		}
		return new string(c, Number);
	}

	public static string StrDup(int Number, char Character)
	{
		if (Number < 0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_GEZero1, "Number"));
		}
		return new string(Character, Number);
	}

	public static string StrDup(int Number, string Character)
	{
		if (Number < 0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_GEZero1, "Number"));
		}
		if (Character == null || Character.Length == 0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_LengthGTZero1, "Character"));
		}
		return new string(Character[0], Number);
	}

	public static string StrReverse(string Expression)
	{
		if (Expression == null)
		{
			return "";
		}
		if (Expression.Length <= 1)
		{
			return Expression;
		}
		TextElementEnumerator textElementEnumerator = StringInfo.GetTextElementEnumerator(Expression);
		checked
		{
			char[] array = new char[Expression.Length - 1 + 1];
			textElementEnumerator.MoveNext();
			int num = 0;
			while (textElementEnumerator.MoveNext())
			{
				Expression.CopyTo(num, array, array.Length - textElementEnumerator.ElementIndex, textElementEnumerator.ElementIndex - num);
				num = textElementEnumerator.ElementIndex;
			}
			Expression.CopyTo(num, array, 0, Expression.Length - num);
			return new string(array);
		}
	}

	public static string UCase(string Value)
	{
		try
		{
			if (Value == null)
			{
				return "";
			}
			return Thread.CurrentThread.CurrentCulture.TextInfo.ToUpper(Value);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static char UCase(char Value)
	{
		try
		{
			return Thread.CurrentThread.CurrentCulture.TextInfo.ToUpper(Value);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	private static bool FormatNamed(object Expression, string Style, ref string ReturnValue)
	{
		int length = Style.Length;
		ReturnValue = null;
		switch (length)
		{
		case 5:
		{
			char c4 = Style[0];
			if ((c4 == 'F' || c4 == 'f') && string.Equals(Style, "fixed", StringComparison.OrdinalIgnoreCase))
			{
				ReturnValue = Conversions.ToDouble(Expression).ToString("0.00", null);
				return true;
			}
			break;
		}
		case 6:
			switch (Style[0])
			{
			case 'Y':
			case 'y':
				if (string.Equals(Style, "yes/no", StringComparison.OrdinalIgnoreCase))
				{
					ReturnValue = (0 - (Conversions.ToBoolean(Expression) ? 1 : 0)).ToString(CachedYesNoFormatStyle, null);
					return true;
				}
				break;
			case 'O':
			case 'o':
				if (string.Equals(Style, "on/off", StringComparison.OrdinalIgnoreCase))
				{
					ReturnValue = (0 - (Conversions.ToBoolean(Expression) ? 1 : 0)).ToString(CachedOnOffFormatStyle, null);
					return true;
				}
				break;
			}
			break;
		case 7:
		{
			char c3 = Style[0];
			if ((c3 == 'P' || c3 == 'p') && string.Equals(Style, "percent", StringComparison.OrdinalIgnoreCase))
			{
				ReturnValue = Conversions.ToDouble(Expression).ToString("0.00%", null);
				return true;
			}
			break;
		}
		case 8:
			switch (Style[0])
			{
			case 'S':
			case 's':
				if (string.Equals(Style, "standard", StringComparison.OrdinalIgnoreCase))
				{
					ReturnValue = Conversions.ToDouble(Expression).ToString("N2", null);
					return true;
				}
				break;
			case 'C':
			case 'c':
				if (string.Equals(Style, "currency", StringComparison.OrdinalIgnoreCase))
				{
					ReturnValue = Conversions.ToDouble(Expression).ToString("C", null);
					return true;
				}
				break;
			}
			break;
		case 9:
			switch (Style[5])
			{
			case 'T':
			case 't':
				if (string.Equals(Style, "long time", StringComparison.OrdinalIgnoreCase))
				{
					ReturnValue = Conversions.ToDate(Expression).ToString("T", null);
					return true;
				}
				break;
			case 'D':
			case 'd':
				if (string.Equals(Style, "long date", StringComparison.OrdinalIgnoreCase))
				{
					ReturnValue = Conversions.ToDate(Expression).ToString("D", null);
					return true;
				}
				break;
			}
			break;
		case 10:
			switch (Style[6])
			{
			case 'A':
			case 'a':
				if (string.Equals(Style, "true/false", StringComparison.OrdinalIgnoreCase))
				{
					ReturnValue = (0 - (Conversions.ToBoolean(Expression) ? 1 : 0)).ToString(CachedTrueFalseFormatStyle, null);
					return true;
				}
				break;
			case 'T':
			case 't':
				if (string.Equals(Style, "short time", StringComparison.OrdinalIgnoreCase))
				{
					ReturnValue = Conversions.ToDate(Expression).ToString("t", null);
					return true;
				}
				break;
			case 'D':
			case 'd':
				if (string.Equals(Style, "short date", StringComparison.OrdinalIgnoreCase))
				{
					ReturnValue = Conversions.ToDate(Expression).ToString("d", null);
					return true;
				}
				break;
			case 'I':
			case 'i':
				if (string.Equals(Style, "scientific", StringComparison.OrdinalIgnoreCase))
				{
					double d = Conversions.ToDouble(Expression);
					if (double.IsNaN(d) || double.IsInfinity(d))
					{
						ReturnValue = d.ToString("G", null);
					}
					else
					{
						ReturnValue = d.ToString("0.00E+00", null);
					}
					return true;
				}
				break;
			}
			break;
		case 11:
			switch (Style[7])
			{
			case 'T':
			case 't':
				if (string.Equals(Style, "medium time", StringComparison.OrdinalIgnoreCase))
				{
					ReturnValue = Conversions.ToDate(Expression).ToString("T", null);
					return true;
				}
				break;
			case 'D':
			case 'd':
				if (string.Equals(Style, "medium date", StringComparison.OrdinalIgnoreCase))
				{
					ReturnValue = Conversions.ToDate(Expression).ToString("D", null);
					return true;
				}
				break;
			}
			break;
		case 12:
		{
			char c2 = Style[0];
			if ((c2 == 'G' || c2 == 'g') && string.Equals(Style, "general date", StringComparison.OrdinalIgnoreCase))
			{
				ReturnValue = Conversions.ToDate(Expression).ToString("G", null);
				return true;
			}
			break;
		}
		case 14:
		{
			char c = Style[0];
			if ((c == 'G' || c == 'g') && string.Equals(Style, "general number", StringComparison.OrdinalIgnoreCase))
			{
				ReturnValue = Conversions.ToDouble(Expression).ToString("G", null);
				return true;
			}
			break;
		}
		}
		return false;
	}

	public static string Format(object Expression, string Style = "")
	{
		try
		{
			IFormatProvider formatProvider = null;
			IFormattable formattable = null;
			if (Expression == null || (object)Expression.GetType() == null)
			{
				return "";
			}
			if (Style == null || Style.Length == 0)
			{
				return Conversions.ToString(Expression);
			}
			IConvertible convertible = (IConvertible)Expression;
			TypeCode typeCode = convertible.GetTypeCode();
			if (Style.Length > 0)
			{
				try
				{
					string ReturnValue = null;
					if (FormatNamed(Expression, Style, ref ReturnValue))
					{
						return ReturnValue;
					}
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
					return Conversions.ToString(Expression);
				}
			}
			formattable = Expression as IFormattable;
			if (formattable == null)
			{
				typeCode = Convert.GetTypeCode(Expression);
				if (typeCode != TypeCode.String && typeCode != TypeCode.Boolean)
				{
					throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Expression"));
				}
			}
			switch (typeCode)
			{
			case TypeCode.Boolean:
				return string.Format(formatProvider, Style, Conversions.ToString(convertible.ToBoolean(null)));
			case TypeCode.Object:
			case TypeCode.Char:
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
			case TypeCode.Decimal:
			case TypeCode.DateTime:
				return formattable.ToString(Style, formatProvider);
			case TypeCode.DBNull:
				return "";
			case TypeCode.Double:
			{
				double num2 = convertible.ToDouble(null);
				if (Style == null || Style.Length == 0)
				{
					return Conversions.ToString(num2);
				}
				if (num2 == 0.0)
				{
					num2 = 0.0;
				}
				return num2.ToString(Style, formatProvider);
			}
			case TypeCode.Empty:
				return "";
			case TypeCode.Single:
			{
				float num = convertible.ToSingle(null);
				if (Style == null || Style.Length == 0)
				{
					return Conversions.ToString(num);
				}
				if (num == 0f)
				{
					num = 0f;
				}
				return num.ToString(Style, formatProvider);
			}
			case TypeCode.String:
				return string.Format(formatProvider, Style, Expression);
			default:
				return formattable.ToString(Style, formatProvider);
			}
		}
		catch (Exception ex4)
		{
			throw ex4;
		}
	}

	public static string FormatCurrency(object Expression, int NumDigitsAfterDecimal = -1, TriState IncludeLeadingDigit = TriState.UseDefault, TriState UseParensForNegativeNumbers = TriState.UseDefault, TriState GroupDigits = TriState.UseDefault)
	{
		IFormatProvider formatProvider = null;
		try
		{
			ValidateTriState(IncludeLeadingDigit);
			ValidateTriState(UseParensForNegativeNumbers);
			ValidateTriState(GroupDigits);
			if (NumDigitsAfterDecimal > 99)
			{
				throw new ArgumentException(System.SR.Format(System.SR.Argument_Range0to99_1, "NumDigitsAfterDecimal"));
			}
			if (Expression == null)
			{
				return "";
			}
			Type type = Expression.GetType();
			if ((object)type == typeof(string))
			{
				Expression = Conversions.ToDouble(Expression);
			}
			else if (!Symbols.IsNumericType(type))
			{
				throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(type), "Currency"));
			}
			IFormattable obj = (IFormattable)Expression;
			if (IncludeLeadingDigit == TriState.False)
			{
				double num = Conversions.ToDouble(Expression);
				if (num >= 1.0 || num <= -1.0)
				{
					IncludeLeadingDigit = TriState.True;
				}
			}
			string currencyFormatString = GetCurrencyFormatString(IncludeLeadingDigit, NumDigitsAfterDecimal, UseParensForNegativeNumbers, GroupDigits, ref formatProvider);
			return obj.ToString(currencyFormatString, formatProvider);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static string FormatDateTime(DateTime Expression, DateFormat NamedFormat = DateFormat.GeneralDate)
	{
		try
		{
			return Expression.ToString(NamedFormat switch
			{
				DateFormat.LongDate => "D", 
				DateFormat.ShortDate => "d", 
				DateFormat.LongTime => "T", 
				DateFormat.ShortTime => "HH:mm", 
				DateFormat.GeneralDate => (Expression.TimeOfDay.Ticks != Expression.Ticks) ? ((Expression.TimeOfDay.Ticks != 0L) ? "G" : "d") : "T", 
				_ => throw ExceptionUtils.VbMakeException(5), 
			}, null);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static string FormatNumber(object Expression, int NumDigitsAfterDecimal = -1, TriState IncludeLeadingDigit = TriState.UseDefault, TriState UseParensForNegativeNumbers = TriState.UseDefault, TriState GroupDigits = TriState.UseDefault)
	{
		try
		{
			ValidateTriState(IncludeLeadingDigit);
			ValidateTriState(UseParensForNegativeNumbers);
			ValidateTriState(GroupDigits);
			if (Expression == null)
			{
				return "";
			}
			Type type = Expression.GetType();
			if ((object)type == typeof(string))
			{
				Expression = Conversions.ToDouble(Expression);
			}
			else if ((object)type == typeof(bool))
			{
				Expression = ((!Conversions.ToBoolean(Expression)) ? ((object)0.0) : ((object)(-1.0)));
			}
			else if (!Symbols.IsNumericType(type))
			{
				throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(type), "Currency"));
			}
			return ((IFormattable)Expression).ToString(GetNumberFormatString(NumDigitsAfterDecimal, IncludeLeadingDigit, UseParensForNegativeNumbers, GroupDigits), null);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	internal static string GetFormatString(int NumDigitsAfterDecimal, TriState IncludeLeadingDigit, TriState UseParensForNegativeNumbers, TriState GroupDigits, FormatType FormatTypeValue)
	{
		StringBuilder stringBuilder = new StringBuilder(30);
		NumberFormatInfo numberFormatInfo = (NumberFormatInfo)Utils.GetCultureInfo().GetFormat(typeof(NumberFormatInfo));
		if (NumDigitsAfterDecimal < -1)
		{
			throw ExceptionUtils.VbMakeException(5);
		}
		if (NumDigitsAfterDecimal == -1)
		{
			switch (FormatTypeValue)
			{
			case FormatType.Percent:
				NumDigitsAfterDecimal = numberFormatInfo.NumberDecimalDigits;
				break;
			case FormatType.Number:
				NumDigitsAfterDecimal = numberFormatInfo.NumberDecimalDigits;
				break;
			case FormatType.Currency:
				NumDigitsAfterDecimal = numberFormatInfo.CurrencyDecimalDigits;
				break;
			}
		}
		if (GroupDigits == TriState.UseDefault)
		{
			GroupDigits = TriState.True;
			switch (FormatTypeValue)
			{
			case FormatType.Percent:
				if (IsArrayEmpty(numberFormatInfo.PercentGroupSizes))
				{
					GroupDigits = TriState.False;
				}
				break;
			case FormatType.Number:
				if (IsArrayEmpty(numberFormatInfo.NumberGroupSizes))
				{
					GroupDigits = TriState.False;
				}
				break;
			case FormatType.Currency:
				if (IsArrayEmpty(numberFormatInfo.CurrencyGroupSizes))
				{
					GroupDigits = TriState.False;
				}
				break;
			}
		}
		if (UseParensForNegativeNumbers == TriState.UseDefault)
		{
			UseParensForNegativeNumbers = TriState.False;
			switch (FormatTypeValue)
			{
			case FormatType.Number:
				if (numberFormatInfo.NumberNegativePattern == 0)
				{
					UseParensForNegativeNumbers = TriState.True;
				}
				break;
			case FormatType.Currency:
				if (numberFormatInfo.CurrencyNegativePattern == 0)
				{
					UseParensForNegativeNumbers = TriState.True;
				}
				break;
			}
		}
		string value = ((GroupDigits != TriState.True) ? "" : "#,##");
		string value2 = ((IncludeLeadingDigit == TriState.False) ? "#" : "0");
		string value3 = ((NumDigitsAfterDecimal <= 0) ? "" : ("." + new string('0', NumDigitsAfterDecimal)));
		if (FormatTypeValue == FormatType.Currency)
		{
			stringBuilder.Append(numberFormatInfo.CurrencySymbol);
		}
		stringBuilder.Append(value);
		stringBuilder.Append(value2);
		stringBuilder.Append(value3);
		if (FormatTypeValue == FormatType.Percent)
		{
			stringBuilder.Append(numberFormatInfo.PercentSymbol);
		}
		if (UseParensForNegativeNumbers == TriState.True)
		{
			string value4 = stringBuilder.ToString();
			stringBuilder.Append(";(");
			stringBuilder.Append(value4);
			stringBuilder.Append(')');
		}
		return stringBuilder.ToString();
	}

	internal static string GetCurrencyFormatString(TriState IncludeLeadingDigit, int NumDigitsAfterDecimal, TriState UseParensForNegativeNumbers, TriState GroupDigits, ref IFormatProvider formatProvider)
	{
		string result = "C";
		NumberFormatInfo numberFormatInfo = (NumberFormatInfo)Utils.GetCultureInfo().GetFormat(typeof(NumberFormatInfo));
		numberFormatInfo = (NumberFormatInfo)numberFormatInfo.Clone();
		if (GroupDigits == TriState.False)
		{
			numberFormatInfo.CurrencyGroupSizes = new int[1];
		}
		int currencyPositivePattern = numberFormatInfo.CurrencyPositivePattern;
		int num = numberFormatInfo.CurrencyNegativePattern;
		switch (UseParensForNegativeNumbers)
		{
		case TriState.UseDefault:
			UseParensForNegativeNumbers = ((num == 0 || num == 4 || (uint)(num - 14) <= 1u) ? TriState.True : TriState.False);
			break;
		case TriState.False:
			switch (num)
			{
			case 0:
				num = 1;
				break;
			case 4:
				num = 5;
				break;
			case 14:
				num = 9;
				break;
			case 15:
				num = 10;
				break;
			}
			break;
		default:
			UseParensForNegativeNumbers = TriState.True;
			switch (num)
			{
			case 1:
			case 2:
			case 3:
				num = 0;
				break;
			case 5:
			case 6:
			case 7:
				num = 4;
				break;
			case 8:
			case 10:
			case 13:
				num = 15;
				break;
			case 9:
			case 11:
			case 12:
				num = 14;
				break;
			}
			break;
		}
		numberFormatInfo.CurrencyNegativePattern = num;
		if (NumDigitsAfterDecimal == -1)
		{
			NumDigitsAfterDecimal = numberFormatInfo.CurrencyDecimalDigits;
		}
		numberFormatInfo.CurrencyDecimalDigits = NumDigitsAfterDecimal;
		formatProvider = new FormatInfoHolder(numberFormatInfo);
		if (IncludeLeadingDigit == TriState.False)
		{
			numberFormatInfo.NumberGroupSizes = numberFormatInfo.CurrencyGroupSizes;
			string text = CurrencyPositiveFormatStrings[currencyPositivePattern] + ";" + CurrencyNegativeFormatStrings[num];
			string text2 = ((GroupDigits == TriState.False) ? ((IncludeLeadingDigit != TriState.False) ? "0" : "#") : ((IncludeLeadingDigit != TriState.False) ? "#,##0" : "#,###"));
			if (NumDigitsAfterDecimal > 0)
			{
				text2 = text2 + "." + new string('0', NumDigitsAfterDecimal);
			}
			if (!string.Equals("$", numberFormatInfo.CurrencySymbol, StringComparison.Ordinal))
			{
				text = text.Replace("$", numberFormatInfo.CurrencySymbol.Replace("'", "''"));
			}
			result = text.Replace("n", text2);
		}
		return result;
	}

	internal static string GetNumberFormatString(int NumDigitsAfterDecimal, TriState IncludeLeadingDigit, TriState UseParensForNegativeNumbers, TriState GroupDigits)
	{
		NumberFormatInfo numberFormatInfo = (NumberFormatInfo)Utils.GetCultureInfo().GetFormat(typeof(NumberFormatInfo));
		switch (NumDigitsAfterDecimal)
		{
		case -1:
			NumDigitsAfterDecimal = numberFormatInfo.NumberDecimalDigits;
			break;
		default:
			throw new ArgumentException(System.SR.Format(System.SR.Argument_Range0to99_1, "NumDigitsAfterDecimal"));
		case 0:
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
		case 10:
		case 11:
		case 12:
		case 13:
		case 14:
		case 15:
		case 16:
		case 17:
		case 18:
		case 19:
		case 20:
		case 21:
		case 22:
		case 23:
		case 24:
		case 25:
		case 26:
		case 27:
		case 28:
		case 29:
		case 30:
		case 31:
		case 32:
		case 33:
		case 34:
		case 35:
		case 36:
		case 37:
		case 38:
		case 39:
		case 40:
		case 41:
		case 42:
		case 43:
		case 44:
		case 45:
		case 46:
		case 47:
		case 48:
		case 49:
		case 50:
		case 51:
		case 52:
		case 53:
		case 54:
		case 55:
		case 56:
		case 57:
		case 58:
		case 59:
		case 60:
		case 61:
		case 62:
		case 63:
		case 64:
		case 65:
		case 66:
		case 67:
		case 68:
		case 69:
		case 70:
		case 71:
		case 72:
		case 73:
		case 74:
		case 75:
		case 76:
		case 77:
		case 78:
		case 79:
		case 80:
		case 81:
		case 82:
		case 83:
		case 84:
		case 85:
		case 86:
		case 87:
		case 88:
		case 89:
		case 90:
		case 91:
		case 92:
		case 93:
		case 94:
		case 95:
		case 96:
		case 97:
		case 98:
		case 99:
			break;
		}
		if (GroupDigits == TriState.UseDefault)
		{
			GroupDigits = ((numberFormatInfo.NumberGroupSizes != null && numberFormatInfo.NumberGroupSizes.Length != 0) ? TriState.True : TriState.False);
		}
		int num = numberFormatInfo.NumberNegativePattern;
		switch (UseParensForNegativeNumbers)
		{
		case TriState.UseDefault:
			UseParensForNegativeNumbers = ((num == 0) ? TriState.True : TriState.False);
			break;
		case TriState.False:
			if (num == 0)
			{
				num = 1;
			}
			break;
		default:
			UseParensForNegativeNumbers = TriState.True;
			if ((uint)(num - 1) <= 3u)
			{
				num = 0;
			}
			break;
		}
		if (UseParensForNegativeNumbers == TriState.UseDefault)
		{
			UseParensForNegativeNumbers = TriState.True;
		}
		string text = "n;" + NumberNegativeFormatStrings[num];
		if (!string.Equals("-", numberFormatInfo.NegativeSign, StringComparison.Ordinal))
		{
			text = text.Replace("-", "\"" + numberFormatInfo.NegativeSign + "\"");
		}
		string text2 = ((IncludeLeadingDigit == TriState.False) ? "#" : "0");
		checked
		{
			if (GroupDigits != TriState.False && numberFormatInfo.NumberGroupSizes.Length != 0)
			{
				if (numberFormatInfo.NumberGroupSizes.Length == 1)
				{
					text2 = "#," + new string('#', numberFormatInfo.NumberGroupSizes[0]) + text2;
				}
				else
				{
					text2 = new string('#', numberFormatInfo.NumberGroupSizes[0] - 1) + text2;
					int upperBound = numberFormatInfo.NumberGroupSizes.GetUpperBound(0);
					for (int i = 1; i <= upperBound; i++)
					{
						text2 = "," + new string('#', numberFormatInfo.NumberGroupSizes[i]) + "," + text2;
					}
				}
			}
			if (NumDigitsAfterDecimal > 0)
			{
				text2 = text2 + "." + new string('0', NumDigitsAfterDecimal);
			}
			return Replace(text, "n", text2);
		}
	}

	public static string FormatPercent(object Expression, int NumDigitsAfterDecimal = -1, TriState IncludeLeadingDigit = TriState.UseDefault, TriState UseParensForNegativeNumbers = TriState.UseDefault, TriState GroupDigits = TriState.UseDefault)
	{
		ValidateTriState(IncludeLeadingDigit);
		ValidateTriState(UseParensForNegativeNumbers);
		ValidateTriState(GroupDigits);
		if (Expression == null)
		{
			return "";
		}
		Type type = Expression.GetType();
		if ((object)type == typeof(string))
		{
			Expression = Conversions.ToDouble(Expression);
		}
		else if (!Symbols.IsNumericType(type))
		{
			throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(type), "numeric"));
		}
		IFormattable obj = (IFormattable)Expression;
		string formatString = GetFormatString(NumDigitsAfterDecimal, IncludeLeadingDigit, UseParensForNegativeNumbers, GroupDigits, FormatType.Percent);
		return obj.ToString(formatString, null);
	}

	public static char GetChar(string str, int Index)
	{
		if (str == null)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_LengthGTZero1, "String"));
		}
		if (Index < 1)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_GEOne1, "Index"));
		}
		if (Index > str.Length)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_IndexLELength2, "Index", "String"));
		}
		return str[checked(Index - 1)];
	}

	public static string Left(string str, int Length)
	{
		if (Length < 0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_GEZero1, "Length"), "Length");
		}
		if (Length == 0 || str == null)
		{
			return "";
		}
		if (Length >= str.Length)
		{
			return str;
		}
		return str.Substring(0, Length);
	}

	public static string LTrim(string str)
	{
		if (str == null || str.Length == 0)
		{
			return "";
		}
		char c = str[0];
		if (c == ' ' || c == '\u3000')
		{
			return str.TrimStart(Utils.m_achIntlSpace);
		}
		return str;
	}

	public static string Mid(string str, int Start)
	{
		try
		{
			if (str == null)
			{
				return null;
			}
			return Mid(str, Start, str.Length);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static string Mid(string str, int Start, int Length)
	{
		if (Start <= 0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_GTZero1, "Start"), "Start");
		}
		if (Length < 0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_GEZero1, "Length"), "Length");
		}
		if (Length == 0 || str == null)
		{
			return "";
		}
		int length = str.Length;
		if (Start > length)
		{
			return "";
		}
		checked
		{
			if (Start + Length > length)
			{
				return str.Substring(Start - 1);
			}
			return str.Substring(Start - 1, Length);
		}
	}

	public static string Right(string str, int Length)
	{
		if (Length < 0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_GEZero1, "Length"), "Length");
		}
		if (Length == 0 || str == null)
		{
			return "";
		}
		int length = str.Length;
		if (Length >= length)
		{
			return str;
		}
		return str.Substring(checked(length - Length), Length);
	}

	public static string RTrim(string str)
	{
		try
		{
			if (str == null || str.Length == 0)
			{
				return "";
			}
			char c = str[checked(str.Length - 1)];
			if (c == ' ' || c == '\u3000')
			{
				return str.TrimEnd(Utils.m_achIntlSpace);
			}
			return str;
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static string Trim(string str)
	{
		try
		{
			if (str == null || str.Length == 0)
			{
				return "";
			}
			char c = str[0];
			if (c == ' ' || c == '\u3000')
			{
				return str.Trim(Utils.m_achIntlSpace);
			}
			c = str[checked(str.Length - 1)];
			if (c == ' ' || c == '\u3000')
			{
				return str.Trim(Utils.m_achIntlSpace);
			}
			return str;
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static int StrComp(string String1, string String2, [OptionCompare] CompareMethod Compare = CompareMethod.Binary)
	{
		try
		{
			return Compare switch
			{
				CompareMethod.Binary => Operators.CompareString(String1, String2, TextCompare: false), 
				CompareMethod.Text => Operators.CompareString(String1, String2, TextCompare: true), 
				_ => throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Compare")), 
			};
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	internal static bool IsValidCodePage(int codepage)
	{
		bool result = false;
		try
		{
			if (Encoding.GetEncoding(codepage) != null)
			{
				result = true;
			}
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
		}
		return result;
	}

	[SupportedOSPlatform("windows")]
	public static string StrConv(string str, VbStrConv Conversion, int LocaleID = 0)
	{
		try
		{
			CultureInfo cultureInfo;
			if (LocaleID == 0 || LocaleID == 1)
			{
				cultureInfo = Utils.GetCultureInfo();
				LocaleID = cultureInfo.LCID;
			}
			else
			{
				try
				{
					cultureInfo = new CultureInfo(LocaleID & 0xFFFF);
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
					throw new ArgumentException(System.SR.Format(System.SR.Argument_LCIDNotSupported1, Conversions.ToString(LocaleID)));
				}
			}
			int num = PRIMARYLANGID(LocaleID);
			if ((Conversion & ~(VbStrConv.ProperCase | VbStrConv.Wide | VbStrConv.Narrow | VbStrConv.Katakana | VbStrConv.Hiragana | VbStrConv.SimplifiedChinese | VbStrConv.TraditionalChinese | VbStrConv.LinguisticCasing)) != VbStrConv.None)
			{
				throw new ArgumentException(System.SR.Argument_InvalidVbStrConv);
			}
			int num2 = default(int);
			switch ((int)(Conversion & (VbStrConv.SimplifiedChinese | VbStrConv.TraditionalChinese)))
			{
			case 768:
				throw new ArgumentException(System.SR.Argument_StrConvSCandTC);
			case 256:
				if (IsValidCodePage(936) && IsValidCodePage(950))
				{
					num2 |= 0x2000000;
					break;
				}
				throw new ArgumentException(System.SR.Argument_SCNotSupported);
			case 512:
				if (IsValidCodePage(936) && IsValidCodePage(950))
				{
					num2 |= 0x4000000;
					break;
				}
				throw new ArgumentException(System.SR.Argument_TCNotSupported);
			}
			switch (Conversion & VbStrConv.ProperCase)
			{
			case VbStrConv.None:
				if ((Conversion & VbStrConv.LinguisticCasing) != VbStrConv.None)
				{
					throw new ArgumentException(System.SR.LinguisticRequirements);
				}
				break;
			case VbStrConv.ProperCase:
				num2 = 0;
				break;
			case VbStrConv.Uppercase:
				if (Conversion == VbStrConv.Uppercase)
				{
					return cultureInfo.TextInfo.ToUpper(str);
				}
				num2 |= 0x200;
				break;
			case VbStrConv.Lowercase:
				if (Conversion == VbStrConv.Lowercase)
				{
					return cultureInfo.TextInfo.ToLower(str);
				}
				num2 |= 0x100;
				break;
			}
			if ((Conversion & (VbStrConv.Katakana | VbStrConv.Hiragana)) != VbStrConv.None && (num != 17 || !ValidLCID(LocaleID)))
			{
				throw new ArgumentException(System.SR.Argument_JPNNotSupported);
			}
			if ((Conversion & (VbStrConv.Wide | VbStrConv.Narrow)) != VbStrConv.None)
			{
				if (num != 17 && num != 18 && num != 4)
				{
					throw new ArgumentException(System.SR.Argument_WideNarrowNotApplicable);
				}
				if (!ValidLCID(LocaleID))
				{
					throw new ArgumentException(System.SR.Argument_LocalNotSupported);
				}
			}
			switch (Conversion & (VbStrConv.Wide | VbStrConv.Narrow))
			{
			case VbStrConv.Wide | VbStrConv.Narrow:
				throw new ArgumentException(System.SR.Argument_IllegalWideNarrow);
			case VbStrConv.Wide:
				num2 |= 0x800000;
				break;
			case VbStrConv.Narrow:
				num2 |= 0x400000;
				break;
			}
			switch (Conversion & (VbStrConv.Katakana | VbStrConv.Hiragana))
			{
			case VbStrConv.Katakana | VbStrConv.Hiragana:
				throw new ArgumentException(System.SR.Argument_IllegalKataHira);
			case VbStrConv.Katakana:
				num2 |= 0x200000;
				break;
			case VbStrConv.Hiragana:
				num2 |= 0x100000;
				break;
			}
			if ((Conversion & VbStrConv.ProperCase) == VbStrConv.ProperCase)
			{
				return ProperCaseString(cultureInfo, num2, str);
			}
			if (num2 != 0)
			{
				return vbLCMapString(cultureInfo, num2, str);
			}
			return str;
		}
		catch (Exception ex4)
		{
			throw ex4;
		}
	}

	internal static bool ValidLCID(int LocaleID)
	{
		try
		{
			new CultureInfo(LocaleID);
			return true;
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
			return false;
		}
	}

	private static string ProperCaseString(CultureInfo loc, int dwMapFlags, string sSrc)
	{
		if ((sSrc?.Length ?? 0) == 0)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder(vbLCMapString(loc, dwMapFlags | 0x100, sSrc));
		return loc.TextInfo.ToTitleCase(stringBuilder.ToString());
	}

	internal static string vbLCMapString(CultureInfo loc, int dwMapFlags, string sSrc)
	{
		int num = sSrc?.Length ?? 0;
		if (num == 0)
		{
			return "";
		}
		int lCID = loc.LCID;
		Encoding encoding = Encoding.GetEncoding(loc.TextInfo.ANSICodePage);
		int num2;
		if (!encoding.IsSingleByte)
		{
			string s = sSrc;
			byte[] bytes = encoding.GetBytes(s);
			num2 = UnsafeNativeMethods.LCMapStringA(lCID, dwMapFlags, bytes, bytes.Length, null, 0);
			byte[] array = new byte[checked(num2 - 1 + 1)];
			num2 = UnsafeNativeMethods.LCMapStringA(lCID, dwMapFlags, bytes, bytes.Length, array, num2);
			return encoding.GetString(array);
		}
		string lpDestStr = new string(' ', num);
		num2 = UnsafeNativeMethods.LCMapString(lCID, dwMapFlags, ref sSrc, num, ref lpDestStr, num);
		return lpDestStr;
	}

	private static void ValidateTriState(TriState Param)
	{
		if (Param != TriState.True && Param != TriState.False && Param != TriState.UseDefault)
		{
			throw ExceptionUtils.VbMakeException(5);
		}
	}

	private static bool IsArrayEmpty(Array array)
	{
		if (array == null)
		{
			return true;
		}
		return array.Length == 0;
	}
}
