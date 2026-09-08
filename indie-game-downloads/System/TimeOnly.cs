using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System;

public readonly struct TimeOnly : IComparable, IComparable<TimeOnly>, IEquatable<TimeOnly>, ISpanFormattable, IFormattable, ISpanParsable<TimeOnly>, IParsable<TimeOnly>, IUtf8SpanFormattable
{
	private readonly ulong _ticks;

	public static TimeOnly MinValue => new TimeOnly(0uL);

	public static TimeOnly MaxValue => new TimeOnly(863999999999uL);

	public int Hour => (int)(_ticks / 36000000000L);

	public int Minute => (int)((uint)(_ticks / 600000000) % 60);

	public int Second => (int)((uint)(_ticks / 10000000) % 60);

	public int Millisecond => (int)((uint)(_ticks / 10000) % 1000);

	public int Microsecond => (int)(_ticks / 10 % 1000);

	public int Nanosecond => (int)(_ticks % 10 * 100);

	public long Ticks => (long)_ticks;

	public TimeOnly(int hour, int minute)
		: this(DateTime.TimeToTicks(hour, minute, 0, 0))
	{
	}

	public TimeOnly(int hour, int minute, int second)
		: this(DateTime.TimeToTicks(hour, minute, second, 0))
	{
	}

	public TimeOnly(int hour, int minute, int second, int millisecond)
		: this(DateTime.TimeToTicks(hour, minute, second, millisecond))
	{
	}

	public TimeOnly(int hour, int minute, int second, int millisecond, int microsecond)
		: this(DateTime.TimeToTicks(hour, minute, second, millisecond, microsecond))
	{
	}

	public TimeOnly(long ticks)
	{
		if ((ulong)ticks > 863999999999uL)
		{
			throw new ArgumentOutOfRangeException("ticks", SR.ArgumentOutOfRange_TimeOnlyBadTicks);
		}
		_ticks = (ulong)ticks;
	}

	internal TimeOnly(ulong ticks)
	{
		_ticks = ticks;
	}

	private TimeOnly AddTicks(long ticks)
	{
		return new TimeOnly((ulong)((long)(_ticks + 864000000000L) + ticks % 864000000000L) % 864000000000uL);
	}

	private TimeOnly AddTicks(long ticks, out int wrappedDays)
	{
		(long Quotient, long Remainder) tuple = Math.DivRem(ticks, 864000000000L);
		long num = tuple.Quotient;
		long item = tuple.Remainder;
		item += (long)_ticks;
		if (item < 0)
		{
			num--;
			item += 864000000000L;
		}
		else if (item >= 864000000000L)
		{
			num++;
			item -= 864000000000L;
		}
		wrappedDays = (int)num;
		return new TimeOnly((ulong)item);
	}

	public TimeOnly Add(TimeSpan value)
	{
		return AddTicks(value.Ticks);
	}

	public TimeOnly Add(TimeSpan value, out int wrappedDays)
	{
		return AddTicks(value.Ticks, out wrappedDays);
	}

	public TimeOnly AddHours(double value)
	{
		return AddTicks((long)(value * 36000000000.0));
	}

	public TimeOnly AddHours(double value, out int wrappedDays)
	{
		return AddTicks((long)(value * 36000000000.0), out wrappedDays);
	}

	public TimeOnly AddMinutes(double value)
	{
		return AddTicks((long)(value * 600000000.0));
	}

	public TimeOnly AddMinutes(double value, out int wrappedDays)
	{
		return AddTicks((long)(value * 600000000.0), out wrappedDays);
	}

	public bool IsBetween(TimeOnly start, TimeOnly end)
	{
		ulong ticks = _ticks;
		ulong ticks2 = start._ticks;
		ulong ticks3 = end._ticks;
		if (ticks2 > ticks3)
		{
			return ticks - ticks3 >= ticks2 - ticks3;
		}
		return ticks - ticks2 < ticks3 - ticks2;
	}

	public static bool operator ==(TimeOnly left, TimeOnly right)
	{
		return left._ticks == right._ticks;
	}

	public static bool operator !=(TimeOnly left, TimeOnly right)
	{
		return left._ticks != right._ticks;
	}

	public static bool operator >(TimeOnly left, TimeOnly right)
	{
		return left._ticks > right._ticks;
	}

	public static bool operator >=(TimeOnly left, TimeOnly right)
	{
		return left._ticks >= right._ticks;
	}

	public static bool operator <(TimeOnly left, TimeOnly right)
	{
		return left._ticks < right._ticks;
	}

	public static bool operator <=(TimeOnly left, TimeOnly right)
	{
		return left._ticks <= right._ticks;
	}

	public static TimeSpan operator -(TimeOnly t1, TimeOnly t2)
	{
		ulong num = t1._ticks - t2._ticks;
		return new TimeSpan((long)num + (((long)num >> 63) & 0xC92A69C000L));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Deconstruct(out int hour, out int minute)
	{
		hour = Hour;
		minute = Minute;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Deconstruct(out int hour, out int minute, out int second)
	{
		ToDateTime().GetTime(out hour, out minute, out second);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Deconstruct(out int hour, out int minute, out int second, out int millisecond)
	{
		ToDateTime().GetTime(out hour, out minute, out second, out millisecond);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Deconstruct(out int hour, out int minute, out int second, out int millisecond, out int microsecond)
	{
		TimeOnly timeOnly = this;
		(hour, minute, second, millisecond) = timeOnly;
		microsecond = Microsecond;
	}

	public static TimeOnly FromTimeSpan(TimeSpan timeSpan)
	{
		return new TimeOnly(timeSpan._ticks);
	}

	public static TimeOnly FromDateTime(DateTime dateTime)
	{
		return new TimeOnly((ulong)dateTime.TimeOfDay.Ticks);
	}

	public TimeSpan ToTimeSpan()
	{
		return new TimeSpan((long)_ticks);
	}

	internal DateTime ToDateTime()
	{
		return DateTime.CreateUnchecked((long)_ticks);
	}

	public int CompareTo(TimeOnly value)
	{
		return _ticks.CompareTo(value._ticks);
	}

	public int CompareTo(object? value)
	{
		if (value == null)
		{
			return 1;
		}
		if (!(value is TimeOnly value2))
		{
			throw new ArgumentException(SR.Arg_MustBeTimeOnly);
		}
		return CompareTo(value2);
	}

	public bool Equals(TimeOnly value)
	{
		return _ticks == value._ticks;
	}

	public override bool Equals([NotNullWhen(true)] object? value)
	{
		if (value is TimeOnly timeOnly)
		{
			return _ticks == timeOnly._ticks;
		}
		return false;
	}

	public override int GetHashCode()
	{
		ulong ticks = _ticks;
		return (int)ticks ^ (int)(ticks >> 32);
	}

	public static TimeOnly Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null, DateTimeStyles style = DateTimeStyles.None)
	{
		ParseFailureKind parseFailureKind = TryParseInternal(s, provider, style, out var result);
		if (parseFailureKind != ParseFailureKind.None)
		{
			ThrowOnError(parseFailureKind, s);
		}
		return result;
	}

	public static TimeOnly ParseExact(ReadOnlySpan<char> s, [StringSyntax("TimeOnlyFormat")] ReadOnlySpan<char> format, IFormatProvider? provider = null, DateTimeStyles style = DateTimeStyles.None)
	{
		ParseFailureKind parseFailureKind = TryParseExactInternal(s, format, provider, style, out var result);
		if (parseFailureKind != ParseFailureKind.None)
		{
			ThrowOnError(parseFailureKind, s);
		}
		return result;
	}

	public static TimeOnly ParseExact(ReadOnlySpan<char> s, [StringSyntax("TimeOnlyFormat")] string[] formats)
	{
		return ParseExact(s, formats, null);
	}

	public static TimeOnly ParseExact(ReadOnlySpan<char> s, [StringSyntax("TimeOnlyFormat")] string[] formats, IFormatProvider? provider, DateTimeStyles style = DateTimeStyles.None)
	{
		ParseFailureKind parseFailureKind = TryParseExactInternal(s, formats, provider, style, out var result);
		if (parseFailureKind != ParseFailureKind.None)
		{
			ThrowOnError(parseFailureKind, s);
		}
		return result;
	}

	public static TimeOnly Parse(string s)
	{
		return Parse(s, null, DateTimeStyles.None);
	}

	public static TimeOnly Parse(string s, IFormatProvider? provider, DateTimeStyles style = DateTimeStyles.None)
	{
		if (s == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
		}
		return Parse(s.AsSpan(), provider, style);
	}

	public static TimeOnly ParseExact(string s, [StringSyntax("TimeOnlyFormat")] string format)
	{
		return ParseExact(s, format, null);
	}

	public static TimeOnly ParseExact(string s, [StringSyntax("TimeOnlyFormat")] string format, IFormatProvider? provider, DateTimeStyles style = DateTimeStyles.None)
	{
		if (s == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
		}
		if (format == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.format);
		}
		return ParseExact(s.AsSpan(), format.AsSpan(), provider, style);
	}

	public static TimeOnly ParseExact(string s, [StringSyntax("TimeOnlyFormat")] string[] formats)
	{
		return ParseExact(s, formats, null);
	}

	public static TimeOnly ParseExact(string s, [StringSyntax("TimeOnlyFormat")] string[] formats, IFormatProvider? provider, DateTimeStyles style = DateTimeStyles.None)
	{
		if (s == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
		}
		return ParseExact(s.AsSpan(), formats, provider, style);
	}

	public static bool TryParse(ReadOnlySpan<char> s, out TimeOnly result)
	{
		return TryParse(s, null, DateTimeStyles.None, out result);
	}

	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, DateTimeStyles style, out TimeOnly result)
	{
		return TryParseInternal(s, provider, style, out result) == ParseFailureKind.None;
	}

	private static ParseFailureKind TryParseInternal(ReadOnlySpan<char> s, IFormatProvider provider, DateTimeStyles style, out TimeOnly result)
	{
		if ((style & ~DateTimeStyles.AllowWhiteSpaces) != DateTimeStyles.None)
		{
			result = default(TimeOnly);
			return ParseFailureKind.Argument_InvalidDateStyles;
		}
		DateTimeResult result2 = default(DateTimeResult);
		result2.Init(s);
		if (!DateTimeParse.TryParse(s, DateTimeFormatInfo.GetInstance(provider), style, ref result2))
		{
			result = default(TimeOnly);
			return ParseFailureKind.Format_BadTimeOnly;
		}
		if ((result2.flags & (ParseFlags.HaveYear | ParseFlags.HaveMonth | ParseFlags.HaveDay | ParseFlags.HaveDate | ParseFlags.TimeZoneUsed | ParseFlags.TimeZoneUtc | ParseFlags.ParsedMonthName | ParseFlags.CaptureOffset | ParseFlags.UtcSortPattern)) != 0)
		{
			result = default(TimeOnly);
			return ParseFailureKind.Format_DateTimeOnlyContainsNoneDateParts;
		}
		result = FromDateTime(result2.parsedDate);
		return ParseFailureKind.None;
	}

	public static bool TryParseExact(ReadOnlySpan<char> s, [StringSyntax("TimeOnlyFormat")] ReadOnlySpan<char> format, out TimeOnly result)
	{
		return TryParseExact(s, format, null, DateTimeStyles.None, out result);
	}

	public static bool TryParseExact(ReadOnlySpan<char> s, [StringSyntax("TimeOnlyFormat")] ReadOnlySpan<char> format, IFormatProvider? provider, DateTimeStyles style, out TimeOnly result)
	{
		return TryParseExactInternal(s, format, provider, style, out result) == ParseFailureKind.None;
	}

	private static ParseFailureKind TryParseExactInternal(ReadOnlySpan<char> s, ReadOnlySpan<char> format, IFormatProvider provider, DateTimeStyles style, out TimeOnly result)
	{
		if ((style & ~DateTimeStyles.AllowWhiteSpaces) != DateTimeStyles.None)
		{
			result = default(TimeOnly);
			return ParseFailureKind.Argument_InvalidDateStyles;
		}
		if (format.Length == 1)
		{
			switch (format[0] | 0x20)
			{
			case 111:
				format = "HH':'mm':'ss'.'fffffff".AsSpan();
				provider = DateTimeFormat.InvariantFormatInfo;
				break;
			case 114:
				format = "HH':'mm':'ss".AsSpan();
				provider = DateTimeFormat.InvariantFormatInfo;
				break;
			}
		}
		DateTimeResult result2 = default(DateTimeResult);
		result2.Init(s);
		if (!DateTimeParse.TryParseExact(s, format, DateTimeFormatInfo.GetInstance(provider), style, ref result2))
		{
			result = default(TimeOnly);
			return ParseFailureKind.Format_BadTimeOnly;
		}
		if ((result2.flags & (ParseFlags.HaveYear | ParseFlags.HaveMonth | ParseFlags.HaveDay | ParseFlags.HaveDate | ParseFlags.TimeZoneUsed | ParseFlags.TimeZoneUtc | ParseFlags.ParsedMonthName | ParseFlags.CaptureOffset | ParseFlags.UtcSortPattern)) != 0)
		{
			result = default(TimeOnly);
			return ParseFailureKind.Format_DateTimeOnlyContainsNoneDateParts;
		}
		result = FromDateTime(result2.parsedDate);
		return ParseFailureKind.None;
	}

	public static bool TryParseExact(ReadOnlySpan<char> s, [NotNullWhen(true)][StringSyntax("TimeOnlyFormat")] string?[]? formats, out TimeOnly result)
	{
		return TryParseExact(s, formats, null, DateTimeStyles.None, out result);
	}

	public static bool TryParseExact(ReadOnlySpan<char> s, [NotNullWhen(true)][StringSyntax("TimeOnlyFormat")] string?[]? formats, IFormatProvider? provider, DateTimeStyles style, out TimeOnly result)
	{
		return TryParseExactInternal(s, formats, provider, style, out result) == ParseFailureKind.None;
	}

	private static ParseFailureKind TryParseExactInternal(ReadOnlySpan<char> s, string[] formats, IFormatProvider provider, DateTimeStyles style, out TimeOnly result)
	{
		if ((style & ~DateTimeStyles.AllowWhiteSpaces) != DateTimeStyles.None || formats == null)
		{
			result = default(TimeOnly);
			return ParseFailureKind.Argument_InvalidDateStyles;
		}
		DateTimeFormatInfo instance = DateTimeFormatInfo.GetInstance(provider);
		for (int i = 0; i < formats.Length; i++)
		{
			DateTimeFormatInfo dtfi = instance;
			string text = formats[i];
			if (string.IsNullOrEmpty(text))
			{
				result = default(TimeOnly);
				return ParseFailureKind.Argument_BadFormatSpecifier;
			}
			if (text.Length == 1)
			{
				switch (text[0] | 0x20)
				{
				case 111:
					text = "HH':'mm':'ss'.'fffffff";
					dtfi = DateTimeFormat.InvariantFormatInfo;
					break;
				case 114:
					text = "HH':'mm':'ss";
					dtfi = DateTimeFormat.InvariantFormatInfo;
					break;
				}
			}
			DateTimeResult result2 = default(DateTimeResult);
			result2.Init(s);
			if (DateTimeParse.TryParseExact(s, text.AsSpan(), dtfi, style, ref result2) && (result2.flags & (ParseFlags.HaveYear | ParseFlags.HaveMonth | ParseFlags.HaveDay | ParseFlags.HaveDate | ParseFlags.TimeZoneUsed | ParseFlags.TimeZoneUtc | ParseFlags.ParsedMonthName | ParseFlags.CaptureOffset | ParseFlags.UtcSortPattern)) == 0)
			{
				result = FromDateTime(result2.parsedDate);
				return ParseFailureKind.None;
			}
		}
		result = default(TimeOnly);
		return ParseFailureKind.Format_BadTimeOnly;
	}

	public static bool TryParse([NotNullWhen(true)] string? s, out TimeOnly result)
	{
		return TryParse(s, null, DateTimeStyles.None, out result);
	}

	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, DateTimeStyles style, out TimeOnly result)
	{
		if (s == null)
		{
			result = default(TimeOnly);
			return false;
		}
		return TryParse(s.AsSpan(), provider, style, out result);
	}

	public static bool TryParseExact([NotNullWhen(true)] string? s, [NotNullWhen(true)][StringSyntax("TimeOnlyFormat")] string? format, out TimeOnly result)
	{
		return TryParseExact(s, format, null, DateTimeStyles.None, out result);
	}

	public static bool TryParseExact([NotNullWhen(true)] string? s, [NotNullWhen(true)][StringSyntax("TimeOnlyFormat")] string? format, IFormatProvider? provider, DateTimeStyles style, out TimeOnly result)
	{
		if (s == null || format == null)
		{
			result = default(TimeOnly);
			return false;
		}
		return TryParseExact(s.AsSpan(), format.AsSpan(), provider, style, out result);
	}

	public static bool TryParseExact([NotNullWhen(true)] string? s, [NotNullWhen(true)][StringSyntax("TimeOnlyFormat")] string?[]? formats, out TimeOnly result)
	{
		return TryParseExact(s, formats, null, DateTimeStyles.None, out result);
	}

	public static bool TryParseExact([NotNullWhen(true)] string? s, [NotNullWhen(true)][StringSyntax("TimeOnlyFormat")] string?[]? formats, IFormatProvider? provider, DateTimeStyles style, out TimeOnly result)
	{
		if (s == null)
		{
			result = default(TimeOnly);
			return false;
		}
		return TryParseExact(s.AsSpan(), formats, provider, style, out result);
	}

	private static void ThrowOnError(ParseFailureKind result, ReadOnlySpan<char> s)
	{
		switch (result)
		{
		case ParseFailureKind.Argument_InvalidDateStyles:
			throw new ArgumentException(SR.Argument_InvalidDateStyles, "style");
		case ParseFailureKind.Argument_BadFormatSpecifier:
			throw new FormatException(SR.Argument_BadFormatSpecifier);
		case ParseFailureKind.Format_BadTimeOnly:
			throw new FormatException(SR.Format(SR.Format_BadTimeOnly, s.ToString()));
		default:
			throw new FormatException(SR.Format(SR.Format_DateTimeOnlyContainsNoneDateParts, s.ToString(), "TimeOnly"));
		}
	}

	public string ToLongTimeString()
	{
		return ToString("T");
	}

	public string ToShortTimeString()
	{
		return ToString();
	}

	public override string ToString()
	{
		return DateTimeFormat.Format(ToDateTime(), "t", null);
	}

	public string ToString([StringSyntax("TimeOnlyFormat")] string? format)
	{
		return ToString(format, null);
	}

	public string ToString(IFormatProvider? provider)
	{
		return DateTimeFormat.Format(ToDateTime(), "t", provider);
	}

	public string ToString([StringSyntax("TimeOnlyFormat")] string? format, IFormatProvider? provider)
	{
		if (string.IsNullOrEmpty(format))
		{
			format = "t";
		}
		if (format.Length == 1)
		{
			return (format[0] | 0x20) switch
			{
				111 => string.Create(16, this, delegate(Span<char> destination, TimeOnly value)
				{
					DateTimeFormat.TryFormatTimeOnlyO(value, destination, out var _);
				}), 
				114 => string.Create(8, this, delegate(Span<char> destination, TimeOnly value)
				{
					DateTimeFormat.TryFormatTimeOnlyR(value, destination, out var _);
				}), 
				116 => DateTimeFormat.Format(ToDateTime(), format, provider), 
				_ => throw new FormatException(SR.Format_InvalidString), 
			};
		}
		DateTimeFormat.IsValidCustomTimeOnlyFormat(format.AsSpan(), throwOnError: true);
		return DateTimeFormat.Format(ToDateTime(), format, provider);
	}

	public bool TryFormat(Span<char> destination, out int charsWritten, [StringSyntax("TimeOnlyFormat")] ReadOnlySpan<char> format = default(ReadOnlySpan<char>), IFormatProvider? provider = null)
	{
		return TryFormatCore(destination, out charsWritten, format, provider);
	}

	public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, [StringSyntax("TimeOnlyFormat")] ReadOnlySpan<char> format = default(ReadOnlySpan<char>), IFormatProvider? provider = null)
	{
		return TryFormatCore(utf8Destination, out bytesWritten, format, provider);
	}

	private bool TryFormatCore<TChar>(Span<TChar> destination, out int written, [StringSyntax("TimeOnlyFormat")] ReadOnlySpan<char> format, IFormatProvider provider) where TChar : unmanaged, IUtfChar<TChar>
	{
		if (format.Length == 0)
		{
			format = "t".AsSpan();
		}
		if (format.Length == 1)
		{
			switch (format[0] | 0x20)
			{
			case 111:
				return DateTimeFormat.TryFormatTimeOnlyO(this, destination, out written);
			case 114:
				return DateTimeFormat.TryFormatTimeOnlyR(this, destination, out written);
			case 116:
				return DateTimeFormat.TryFormat(ToDateTime(), destination, out written, format, provider);
			}
			ThrowHelper.ThrowFormatException_BadFormatSpecifier();
		}
		if (!DateTimeFormat.IsValidCustomTimeOnlyFormat(format, throwOnError: false))
		{
			throw new FormatException(SR.Format(SR.Format_DateTimeOnlyContainsNoneDateParts, format.ToString(), "TimeOnly"));
		}
		return DateTimeFormat.TryFormat(ToDateTime(), destination, out written, format, provider);
	}

	public static TimeOnly Parse(string s, IFormatProvider? provider)
	{
		return Parse(s, provider, DateTimeStyles.None);
	}

	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out TimeOnly result)
	{
		return TryParse(s, provider, DateTimeStyles.None, out result);
	}

	public static TimeOnly Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
	{
		return Parse(s, provider, DateTimeStyles.None);
	}

	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out TimeOnly result)
	{
		return TryParse(s, provider, DateTimeStyles.None, out result);
	}
}
