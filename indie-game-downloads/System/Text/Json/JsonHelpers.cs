using System.Buffers;
using System.Buffers.Text;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions.Generated;

namespace System.Text.Json;

internal static class JsonHelpers
{
	[StructLayout(LayoutKind.Auto)]
	private struct DateTimeParseData
	{
		public int Year;

		public int Month;

		public int Day;

		public bool IsCalendarDateOnly;

		public int Hour;

		public int Minute;

		public int Second;

		public int Fraction;

		public int OffsetHours;

		public int OffsetMinutes;

		public byte OffsetToken;

		public bool OffsetNegative => OffsetToken == 45;
	}

	public static readonly Regex IntegerRegex = CreateIntegerRegex();

	private static ReadOnlySpan<int> DaysToMonth365 => new int[13]
	{
		0, 31, 59, 90, 120, 151, 181, 212, 243, 273,
		304, 334, 365
	};

	private static ReadOnlySpan<int> DaysToMonth366 => new int[13]
	{
		0, 31, 60, 91, 121, 152, 182, 213, 244, 274,
		305, 335, 366
	};

	internal static bool RequiresSpecialNumberHandlingOnWrite(JsonNumberHandling? handling)
	{
		if (!handling.HasValue)
		{
			return false;
		}
		return (handling.Value & (JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowNamedFloatingPointLiterals)) != 0;
	}

	internal static void StableSortByKey<T, TKey>(this List<T> items, Func<T, TKey> keySelector) where TKey : unmanaged, IComparable<TKey>
	{
		Span<T> items2 = CollectionsMarshal.AsSpan(items);
		Span<(TKey, int)> span = ((items2.Length > 32) ? ((Span<(TKey, int)>)new(TKey, int)[items2.Length]) : stackalloc(TKey, int)[32].Slice(0, items2.Length));
		Span<(TKey, int)> keys = span;
		for (int i = 0; i < keys.Length; i++)
		{
			keys[i] = (keySelector(items2[i]), i);
		}
		keys.Sort(items2);
	}

	public static T[] TraverseGraphWithTopologicalSort<T>(T entryNode, Func<T, ICollection<T>> getChildren, IEqualityComparer<T> comparer = null)
	{
		if (comparer == null)
		{
			comparer = EqualityComparer<T>.Default;
		}
		List<T> list = new List<T> { entryNode };
		Dictionary<T, int> dictionary = new Dictionary<T, int>(comparer) { [entryNode] = 0 };
		List<bool[]> list2 = new List<bool[]>();
		Queue<int> queue = new Queue<int>();
		for (int i = 0; i < list.Count; i++)
		{
			T arg = list[i];
			ICollection<T> collection = getChildren(arg);
			int count = collection.Count;
			if (count == 0)
			{
				list2.Add(null);
				queue.Enqueue(i);
				continue;
			}
			bool[] array = new bool[Math.Max(list.Count, count)];
			foreach (T item in collection)
			{
				if (!dictionary.TryGetValue(item, out var value))
				{
					value = list.Count;
					dictionary.Add(item, value);
					list.Add(item);
				}
				if (value >= array.Length)
				{
					Array.Resize(ref array, value + 1);
				}
				array[value] = true;
			}
			list2.Add(array);
		}
		T[] array2 = new T[list.Count];
		int num = array2.Length;
		do
		{
			int num2 = queue.Dequeue();
			array2[--num] = list[num2];
			for (int j = 0; j < list2.Count; j++)
			{
				bool[] array3 = list2[j];
				if (array3 != null && num2 < array3.Length && array3[num2])
				{
					array3[num2] = false;
					if (((ReadOnlySpan<bool>)array3.AsSpan()).IndexOf(true) == -1)
					{
						queue.Enqueue(j);
					}
				}
			}
		}
		while (queue.Count > 0);
		return array2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<byte> GetUnescapedSpan(this scoped ref Utf8JsonReader reader)
	{
		ReadOnlySpan<byte> readOnlySpan = (reader.HasValueSequence ? ((ReadOnlySpan<byte>)reader.ValueSequence.ToArray<byte>()) : reader.ValueSpan);
		if (!reader.ValueIsEscaped)
		{
			return readOnlySpan;
		}
		return JsonReaderHelper.GetUnescaped(readOnlySpan);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryAdvanceWithOptionalReadAhead(this scoped ref Utf8JsonReader reader, bool requiresReadAhead)
	{
		if (!requiresReadAhead || reader.IsFinalBlock)
		{
			return reader.Read();
		}
		return TryAdvanceWithReadAhead(ref reader);
	}

	public static bool TryAdvanceToNextRootLevelValueWithOptionalReadAhead(this scoped ref Utf8JsonReader reader, bool requiresReadAhead, out bool isAtEndOfStream)
	{
		Utf8JsonReader utf8JsonReader = reader;
		if (!reader.Read())
		{
			isAtEndOfStream = reader.IsFinalBlock;
			reader = utf8JsonReader;
			return false;
		}
		isAtEndOfStream = false;
		if (requiresReadAhead && !reader.IsFinalBlock)
		{
			reader = utf8JsonReader;
			return TryAdvanceWithReadAhead(ref reader);
		}
		return true;
	}

	private static bool TryAdvanceWithReadAhead(scoped ref Utf8JsonReader reader)
	{
		Utf8JsonReader utf8JsonReader = reader;
		if (!reader.Read())
		{
			return false;
		}
		JsonTokenType tokenType = reader.TokenType;
		if ((tokenType == JsonTokenType.StartObject || tokenType == JsonTokenType.StartArray) ? true : false)
		{
			bool num = reader.TrySkipPartial();
			reader = utf8JsonReader;
			if (!num)
			{
				return false;
			}
			reader.ReadWithVerify();
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsInRangeInclusive(uint value, uint lowerBound, uint upperBound)
	{
		return value - lowerBound <= upperBound - lowerBound;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsInRangeInclusive(int value, int lowerBound, int upperBound)
	{
		return (uint)(value - lowerBound) <= (uint)(upperBound - lowerBound);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsInRangeInclusive(long value, long lowerBound, long upperBound)
	{
		return (ulong)(value - lowerBound) <= (ulong)(upperBound - lowerBound);
	}

	public static bool IsDigit(byte value)
	{
		return (uint)(value - 48) <= 9u;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ReadWithVerify(this ref Utf8JsonReader reader)
	{
		reader.Read();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SkipWithVerify(this ref Utf8JsonReader reader)
	{
		reader.TrySkipPartial(reader.CurrentDepth);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TrySkipPartial(this ref Utf8JsonReader reader)
	{
		return reader.TrySkipPartial(reader.CurrentDepth);
	}

	public static string Utf8GetString(ReadOnlySpan<byte> bytes)
	{
		return Encoding.UTF8.GetString(bytes);
	}

	public static bool TryLookupUtf8Key<TValue>(this Dictionary<string, TValue> dictionary, ReadOnlySpan<byte> utf8Key, [MaybeNullWhen(false)] out TValue result)
	{
		Dictionary<string, TValue>.AlternateLookup<ReadOnlySpan<char>> alternateLookup = dictionary.GetAlternateLookup<ReadOnlySpan<char>>();
		char[] array = null;
		Span<char> span = ((utf8Key.Length > 128) ? ((Span<char>)(array = ArrayPool<char>.Shared.Rent(utf8Key.Length))) : stackalloc char[128]);
		Span<char> chars = span;
		Span<char> span2 = chars[..Encoding.UTF8.GetChars(utf8Key, chars)];
		bool result2 = alternateLookup.TryGetValue(span2, out result);
		if (array != null)
		{
			span2.Clear();
			ArrayPool<char>.Shared.Return(array);
		}
		return result2;
	}

	public static bool IsFinite(double value)
	{
		return double.IsFinite(value);
	}

	public static bool IsFinite(float value)
	{
		return float.IsFinite(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ValidateInt32MaxArrayLength(uint length)
	{
		if (length > 2146435071)
		{
			ThrowHelper.ThrowOutOfMemoryException(length);
		}
	}

	[GeneratedRegex("^\\s*(?:\\+|\\-)?[0-9]+\\s*$", RegexOptions.None, 200)]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.37416")]
	private static Regex CreateIntegerRegex()
	{
		return _003CRegexGenerator_g_003EFEBA907BFC8DA5891021ECFC4ACF51500CA89E8FDCD0895B025EB5D8CE91E77E9__CreateIntegerRegex_0.Instance;
	}

	public static bool AreEqualJsonNumbers(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
	{
		ParseNumber(left, out var isNegative, out var integral, out var fractional, out var exponent);
		ParseNumber(right, out var isNegative2, out var integral2, out var fractional2, out var exponent2);
		if (isNegative != isNegative2 || exponent != exponent2 || integral.Length + fractional.Length != integral2.Length + fractional2.Length)
		{
			return false;
		}
		int num = integral.Length - integral2.Length;
		ReadOnlySpan<byte> span;
		ReadOnlySpan<byte> span2;
		ReadOnlySpan<byte> span3;
		ReadOnlySpan<byte> other;
		ReadOnlySpan<byte> other2;
		ReadOnlySpan<byte> other3;
		if (num >= 0)
		{
			if (num == 0)
			{
				span = integral;
				span2 = default(ReadOnlySpan<byte>);
				span3 = fractional;
				other = integral2;
				other2 = default(ReadOnlySpan<byte>);
				other3 = fractional2;
			}
			else
			{
				int num2 = integral.Length - num;
				span = integral.Slice(0, num2);
				span2 = integral.Slice(num2);
				span3 = fractional;
				other = integral2;
				other2 = fractional2.Slice(0, num);
				other3 = fractional2.Slice(num);
			}
		}
		else
		{
			span = integral;
			span2 = fractional.Slice(0, -num);
			span3 = fractional.Slice(-num);
			int num3 = integral2.Length + num;
			other = integral2.Slice(0, num3);
			other2 = integral2.Slice(num3);
			other3 = fractional2;
		}
		if (span.SequenceEqual(other) && span2.SequenceEqual(other2))
		{
			return span3.SequenceEqual(other3);
		}
		return false;
		static int IndexOfFirstTrailingZero(ReadOnlySpan<byte> span4)
		{
			int num4 = span4.LastIndexOfAnyExcept((byte)48);
			if (num4 != span4.Length - 1)
			{
				return num4 + 1;
			}
			return -1;
		}
		static int IndexOfLastLeadingZero(ReadOnlySpan<byte> span4)
		{
			int num4 = span4.IndexOfAnyExcept((byte)48);
			if (num4 >= 0)
			{
				return num4 - 1;
			}
			return span4.Length - 1;
		}
		static void ParseNumber(ReadOnlySpan<byte> readOnlySpan, out bool reference, out ReadOnlySpan<byte> reference2, out ReadOnlySpan<byte> reference3, out int reference4)
		{
			bool flag;
			if (readOnlySpan[0] == 45)
			{
				flag = true;
				readOnlySpan = readOnlySpan.Slice(1);
			}
			else
			{
				flag = false;
			}
			int num4 = readOnlySpan.IndexOfAny((byte)46, (byte)101, (byte)69);
			ReadOnlySpan<byte> readOnlySpan2;
			ReadOnlySpan<byte> readOnlySpan3;
			int value;
			if (num4 < 0)
			{
				readOnlySpan2 = readOnlySpan;
				readOnlySpan3 = default(ReadOnlySpan<byte>);
				value = 0;
			}
			else
			{
				readOnlySpan2 = readOnlySpan.Slice(0, num4);
				if (readOnlySpan[num4] == 46)
				{
					readOnlySpan = readOnlySpan.Slice(num4 + 1);
					num4 = readOnlySpan.IndexOfAny((byte)101, (byte)69);
					if (num4 < 0)
					{
						readOnlySpan3 = readOnlySpan;
						value = 0;
						goto IL_00b1;
					}
					readOnlySpan3 = readOnlySpan.Slice(0, num4);
				}
				else
				{
					readOnlySpan3 = default(ReadOnlySpan<byte>);
				}
				if (!Utf8Parser.TryParse(readOnlySpan.Slice(num4 + 1), out value, out int _, '\0'))
				{
					ThrowHelper.ThrowArgumentOutOfRangeException_JsonNumberExponentTooLarge("exponent");
				}
			}
			goto IL_00b1;
			IL_00b1:
			int num5 = IndexOfFirstTrailingZero(readOnlySpan3);
			if (num5 >= 0)
			{
				readOnlySpan3 = readOnlySpan3.Slice(0, num5);
			}
			if (readOnlySpan2[0] == 48)
			{
				int num6 = IndexOfLastLeadingZero(readOnlySpan3);
				if (num6 >= 0)
				{
					readOnlySpan3 = readOnlySpan3.Slice(num6 + 1);
					value -= num6 + 1;
				}
				readOnlySpan2 = default(ReadOnlySpan<byte>);
			}
			if (readOnlySpan3.IsEmpty)
			{
				int num7 = IndexOfFirstTrailingZero(readOnlySpan2);
				if (num7 >= 0)
				{
					value += readOnlySpan2.Length - num7;
					readOnlySpan2 = readOnlySpan2.Slice(0, num7);
				}
			}
			value -= readOnlySpan3.Length;
			if (readOnlySpan2.IsEmpty && readOnlySpan3.IsEmpty)
			{
				flag = false;
				value = 0;
			}
			reference = flag;
			reference2 = readOnlySpan2;
			reference3 = readOnlySpan3;
			reference4 = value;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsValidDateTimeOffsetParseLength(int length)
	{
		return IsInRangeInclusive(length, 10, 252);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsValidUnescapedDateTimeOffsetParseLength(int length)
	{
		return IsInRangeInclusive(length, 10, 42);
	}

	public static bool TryParseAsISO(ReadOnlySpan<byte> source, out DateTime value)
	{
		if (!TryParseDateTimeOffset(source, out var parseData))
		{
			value = default(DateTime);
			return false;
		}
		if (parseData.OffsetToken == 90)
		{
			return TryCreateDateTime(parseData, DateTimeKind.Utc, out value);
		}
		if (parseData.OffsetToken == 43 || parseData.OffsetToken == 45)
		{
			if (!TryCreateDateTimeOffset(ref parseData, out var value2))
			{
				value = default(DateTime);
				return false;
			}
			value = value2.LocalDateTime;
			return true;
		}
		return TryCreateDateTime(parseData, DateTimeKind.Unspecified, out value);
	}

	public static bool TryParseAsISO(ReadOnlySpan<byte> source, out DateTimeOffset value)
	{
		if (!TryParseDateTimeOffset(source, out var parseData))
		{
			value = default(DateTimeOffset);
			return false;
		}
		if (parseData.OffsetToken == 90 || parseData.OffsetToken == 43 || parseData.OffsetToken == 45)
		{
			return TryCreateDateTimeOffset(ref parseData, out value);
		}
		return TryCreateDateTimeOffsetInterpretingDataAsLocalTime(parseData, out value);
	}

	public static bool TryParseAsIso(ReadOnlySpan<byte> source, out DateOnly value)
	{
		if (TryParseDateTimeOffset(source, out var parseData) && parseData.IsCalendarDateOnly && TryCreateDateTime(parseData, DateTimeKind.Unspecified, out var value2))
		{
			value = DateOnly.FromDateTime(value2);
			return true;
		}
		value = default(DateOnly);
		return false;
	}

	private static bool TryParseDateTimeOffset(ReadOnlySpan<byte> source, out DateTimeParseData parseData)
	{
		parseData = default(DateTimeParseData);
		uint num = (uint)(source[0] - 48);
		uint num2 = (uint)(source[1] - 48);
		uint num3 = (uint)(source[2] - 48);
		uint num4 = (uint)(source[3] - 48);
		if (num > 9 || num2 > 9 || num3 > 9 || num4 > 9)
		{
			return false;
		}
		parseData.Year = (int)(num * 1000 + num2 * 100 + num3 * 10 + num4);
		if (source[4] != 45 || !TryGetNextTwoDigits(source.Slice(5, 2), ref parseData.Month) || source[7] != 45 || !TryGetNextTwoDigits(source.Slice(8, 2), ref parseData.Day))
		{
			return false;
		}
		if (source.Length == 10)
		{
			parseData.IsCalendarDateOnly = true;
			return true;
		}
		if (source.Length < 16)
		{
			return false;
		}
		if (source[10] != 84 || source[13] != 58 || !TryGetNextTwoDigits(source.Slice(11, 2), ref parseData.Hour) || !TryGetNextTwoDigits(source.Slice(14, 2), ref parseData.Minute))
		{
			return false;
		}
		if (source.Length == 16)
		{
			return true;
		}
		byte b = source[16];
		int num5 = 17;
		switch (b)
		{
		case 90:
			parseData.OffsetToken = 90;
			return num5 == source.Length;
		case 43:
		case 45:
			parseData.OffsetToken = b;
			return ParseOffset(ref parseData, source.Slice(num5));
		default:
			return false;
		case 58:
			if (source.Length < 19 || !TryGetNextTwoDigits(source.Slice(17, 2), ref parseData.Second))
			{
				return false;
			}
			if (source.Length == 19)
			{
				return true;
			}
			b = source[19];
			num5 = 20;
			switch (b)
			{
			case 90:
				parseData.OffsetToken = 90;
				return num5 == source.Length;
			case 43:
			case 45:
				parseData.OffsetToken = b;
				return ParseOffset(ref parseData, source.Slice(num5));
			default:
				return false;
			case 46:
			{
				if (source.Length < 21)
				{
					return false;
				}
				int i = 0;
				for (int num6 = Math.Min(num5 + 16, source.Length); num5 < num6; num5++)
				{
					if (!IsDigit(b = source[num5]))
					{
						break;
					}
					if (i < 7)
					{
						parseData.Fraction = parseData.Fraction * 10 + (b - 48);
						i++;
					}
				}
				if (parseData.Fraction != 0)
				{
					for (; i < 7; i++)
					{
						parseData.Fraction *= 10;
					}
				}
				if (num5 == source.Length)
				{
					return true;
				}
				b = source[num5++];
				switch (b)
				{
				case 90:
					parseData.OffsetToken = 90;
					return num5 == source.Length;
				case 43:
				case 45:
					parseData.OffsetToken = b;
					return ParseOffset(ref parseData, source.Slice(num5));
				default:
					return false;
				}
			}
			}
		}
		static bool ParseOffset(ref DateTimeParseData reference, ReadOnlySpan<byte> offsetData)
		{
			if (offsetData.Length < 2 || !TryGetNextTwoDigits(offsetData.Slice(0, 2), ref reference.OffsetHours))
			{
				return false;
			}
			if (offsetData.Length == 2)
			{
				return true;
			}
			if (offsetData.Length != 5 || offsetData[2] != 58 || !TryGetNextTwoDigits(offsetData.Slice(3), ref reference.OffsetMinutes))
			{
				return false;
			}
			return true;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool TryGetNextTwoDigits(ReadOnlySpan<byte> source, ref int value)
	{
		uint num = (uint)(source[0] - 48);
		uint num2 = (uint)(source[1] - 48);
		if (num > 9 || num2 > 9)
		{
			value = 0;
			return false;
		}
		value = (int)(num * 10 + num2);
		return true;
	}

	private static bool TryCreateDateTimeOffset(DateTime dateTime, ref DateTimeParseData parseData, out DateTimeOffset value)
	{
		if ((uint)parseData.OffsetHours > 14u)
		{
			value = default(DateTimeOffset);
			return false;
		}
		if ((uint)parseData.OffsetMinutes > 59u)
		{
			value = default(DateTimeOffset);
			return false;
		}
		if (parseData.OffsetHours == 14 && parseData.OffsetMinutes != 0)
		{
			value = default(DateTimeOffset);
			return false;
		}
		long num = ((long)parseData.OffsetHours * 3600L + (long)parseData.OffsetMinutes * 60L) * 10000000;
		if (parseData.OffsetNegative)
		{
			num = -num;
		}
		try
		{
			value = new DateTimeOffset(dateTime.Ticks, new TimeSpan(num));
		}
		catch (ArgumentOutOfRangeException)
		{
			value = default(DateTimeOffset);
			return false;
		}
		return true;
	}

	private static bool TryCreateDateTimeOffset(ref DateTimeParseData parseData, out DateTimeOffset value)
	{
		if (!TryCreateDateTime(parseData, DateTimeKind.Unspecified, out var value2))
		{
			value = default(DateTimeOffset);
			return false;
		}
		if (!TryCreateDateTimeOffset(value2, ref parseData, out value))
		{
			value = default(DateTimeOffset);
			return false;
		}
		return true;
	}

	private static bool TryCreateDateTimeOffsetInterpretingDataAsLocalTime(DateTimeParseData parseData, out DateTimeOffset value)
	{
		if (!TryCreateDateTime(parseData, DateTimeKind.Local, out var value2))
		{
			value = default(DateTimeOffset);
			return false;
		}
		try
		{
			value = new DateTimeOffset(value2);
		}
		catch (ArgumentOutOfRangeException)
		{
			value = default(DateTimeOffset);
			return false;
		}
		return true;
	}

	private static bool TryCreateDateTime(DateTimeParseData parseData, DateTimeKind kind, out DateTime value)
	{
		if (parseData.Year == 0)
		{
			value = default(DateTime);
			return false;
		}
		if ((uint)(parseData.Month - 1) >= 12u)
		{
			value = default(DateTime);
			return false;
		}
		uint num = (uint)(parseData.Day - 1);
		if (num >= 28 && num >= DateTime.DaysInMonth(parseData.Year, parseData.Month))
		{
			value = default(DateTime);
			return false;
		}
		if ((uint)parseData.Hour > 23u)
		{
			value = default(DateTime);
			return false;
		}
		if ((uint)parseData.Minute > 59u)
		{
			value = default(DateTime);
			return false;
		}
		if ((uint)parseData.Second > 59u)
		{
			value = default(DateTime);
			return false;
		}
		ReadOnlySpan<int> readOnlySpan = (DateTime.IsLeapYear(parseData.Year) ? DaysToMonth366 : DaysToMonth365);
		int num2 = parseData.Year - 1;
		long num3 = (num2 * 365 + num2 / 4 - num2 / 100 + num2 / 400 + readOnlySpan[parseData.Month - 1] + parseData.Day - 1) * 864000000000L;
		int num4 = parseData.Hour * 3600 + parseData.Minute * 60 + parseData.Second;
		num3 += (long)num4 * 10000000L;
		num3 += parseData.Fraction;
		value = new DateTime(num3, kind);
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte[] GetEscapedPropertyNameSection(ReadOnlySpan<byte> utf8Value, JavaScriptEncoder encoder)
	{
		int num = JsonWriterHelper.NeedsEscaping(utf8Value, encoder);
		if (num != -1)
		{
			return GetEscapedPropertyNameSection(utf8Value, num, encoder);
		}
		return GetPropertyNameSection(utf8Value);
	}

	public static byte[] EscapeValue(ReadOnlySpan<byte> utf8Value, int firstEscapeIndexVal, JavaScriptEncoder encoder)
	{
		byte[] array = null;
		int maxEscapedLength = JsonWriterHelper.GetMaxEscapedLength(utf8Value.Length, firstEscapeIndexVal);
		Span<byte> span = ((maxEscapedLength > 256) ? ((Span<byte>)(array = ArrayPool<byte>.Shared.Rent(maxEscapedLength))) : stackalloc byte[256]);
		Span<byte> destination = span;
		JsonWriterHelper.EscapeString(utf8Value, destination, firstEscapeIndexVal, encoder, out var written);
		byte[] result = destination.Slice(0, written).ToArray();
		if (array != null)
		{
			ArrayPool<byte>.Shared.Return(array);
		}
		return result;
	}

	private static byte[] GetEscapedPropertyNameSection(ReadOnlySpan<byte> utf8Value, int firstEscapeIndexVal, JavaScriptEncoder encoder)
	{
		byte[] array = null;
		int maxEscapedLength = JsonWriterHelper.GetMaxEscapedLength(utf8Value.Length, firstEscapeIndexVal);
		Span<byte> span = ((maxEscapedLength > 256) ? ((Span<byte>)(array = ArrayPool<byte>.Shared.Rent(maxEscapedLength))) : stackalloc byte[256]);
		Span<byte> destination = span;
		JsonWriterHelper.EscapeString(utf8Value, destination, firstEscapeIndexVal, encoder, out var written);
		byte[] propertyNameSection = GetPropertyNameSection(destination.Slice(0, written));
		if (array != null)
		{
			ArrayPool<byte>.Shared.Return(array);
		}
		return propertyNameSection;
	}

	private static byte[] GetPropertyNameSection(ReadOnlySpan<byte> utf8Value)
	{
		int length = utf8Value.Length;
		byte[] array = new byte[length + 3];
		array[0] = 34;
		utf8Value.CopyTo(array.AsSpan(1, length));
		array[++length] = 34;
		array[++length] = 58;
		return array;
	}
}
