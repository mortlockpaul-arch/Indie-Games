using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System;

[Serializable]
[StructLayout(LayoutKind.Auto)]
[TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
public readonly struct DateTimeOffset : IComparable, ISpanFormattable, IFormattable, IComparable<DateTimeOffset>, IEquatable<DateTimeOffset>, ISerializable, IDeserializationCallback, ISpanParsable<DateTimeOffset>, IParsable<DateTimeOffset>, IUtf8SpanFormattable
{
	public static readonly DateTimeOffset MinValue;

	public static readonly DateTimeOffset MaxValue = new DateTimeOffset(0, DateTime.CreateUnchecked(3155378975999999999L));

	public static readonly DateTimeOffset UnixEpoch = new DateTimeOffset(0, DateTime.CreateUnchecked(621355968000000000L));

	private readonly DateTime _dateTime;

	private readonly int _offsetMinutes;

	public static DateTimeOffset UtcNow => new DateTimeOffset(0, DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified));

	public DateTime DateTime => ClockDateTime;

	public DateTime UtcDateTime => DateTime.CreateUnchecked((long)(_dateTime._dateData | 0x4000000000000000L));

	public DateTime LocalDateTime => UtcDateTime.ToLocalTime();

	private DateTime ClockDateTime => DateTime.CreateUnchecked(UtcTicks + (long)_offsetMinutes * 600000000L);

	public DateTime Date => ClockDateTime.Date;

	public int Day => ClockDateTime.Day;

	public DayOfWeek DayOfWeek => ClockDateTime.DayOfWeek;

	public int DayOfYear => ClockDateTime.DayOfYear;

	public int Hour => ClockDateTime.Hour;

	public int Millisecond => UtcDateTime.Millisecond;

	public int Microsecond => UtcDateTime.Microsecond;

	public int Nanosecond => UtcDateTime.Nanosecond;

	public int Minute => ClockDateTime.Minute;

	public int Month => ClockDateTime.Month;

	public TimeSpan Offset => new TimeSpan((long)_offsetMinutes * 600000000L);

	public int TotalOffsetMinutes => _offsetMinutes;

	public int Second => UtcDateTime.Second;

	public long Ticks => ClockDateTime.Ticks;

	public long UtcTicks => (long)_dateTime._dateData;

	public TimeSpan TimeOfDay => ClockDateTime.TimeOfDay;

	public int Year => ClockDateTime.Year;

	public static DateTimeOffset Now => ToLocalTime(DateTime.UtcNow, throwOnOverflow: true);

	private DateTimeOffset(int validOffsetMinutes, DateTime validDateTime)
	{
		_dateTime = validDateTime;
		_offsetMinutes = validOffsetMinutes;
	}

	public DateTimeOffset(long ticks, TimeSpan offset)
		: this(ValidateOffset(offset), ValidateDate(new DateTime(ticks), offset))
	{
	}

	private static DateTimeOffset CreateValidateOffset(DateTime dateTime, TimeSpan offset)
	{
		return new DateTimeOffset(ValidateOffset(offset), ValidateDate(dateTime, offset));
	}

	public DateTimeOffset(DateTime dateTime)
	{
		if (dateTime.Kind != DateTimeKind.Utc)
		{
			TimeSpan localUtcOffset = TimeZoneInfo.GetLocalUtcOffset(dateTime, TimeZoneInfoOptions.NoThrowOnInvalidTime);
			_offsetMinutes = ValidateOffset(localUtcOffset);
			_dateTime = ValidateDate(dateTime, localUtcOffset);
		}
		else
		{
			_offsetMinutes = 0;
			_dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
		}
	}

	public DateTimeOffset(DateTime dateTime, TimeSpan offset)
	{
		if (dateTime.Kind == DateTimeKind.Local)
		{
			if (offset != TimeZoneInfo.GetLocalUtcOffset(dateTime, TimeZoneInfoOptions.NoThrowOnInvalidTime))
			{
				throw new ArgumentException(SR.Argument_OffsetLocalMismatch, "offset");
			}
		}
		else if (dateTime.Kind == DateTimeKind.Utc && offset.Ticks != 0L)
		{
			throw new ArgumentException(SR.Argument_OffsetUtcMismatch, "offset");
		}
		_offsetMinutes = ValidateOffset(offset);
		_dateTime = ValidateDate(dateTime, offset);
	}

	public DateTimeOffset(DateOnly date, TimeOnly time, TimeSpan offset)
		: this(new DateTime(date, time), offset)
	{
	}

	public DateTimeOffset(int year, int month, int day, int hour, int minute, int second, TimeSpan offset)
	{
		_offsetMinutes = ValidateOffset(offset);
		if (second != 60 || !DateTime.SystemSupportsLeapSeconds)
		{
			_dateTime = ValidateDate(new DateTime(year, month, day, hour, minute, second), offset);
		}
		else
		{
			_dateTime = WithLeapSecond(year, month, day, hour, minute, offset);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static DateTime WithLeapSecond(int year, int month, int day, int hour, int minute, TimeSpan offset)
	{
		DateTimeOffset dateTimeOffset = new DateTimeOffset(year, month, day, hour, minute, 59, offset);
		DateTime.ValidateLeapSecond(dateTimeOffset.UtcDateTime);
		return dateTimeOffset._dateTime;
	}

	public DateTimeOffset(int year, int month, int day, int hour, int minute, int second, int millisecond, TimeSpan offset)
		: this(year, month, day, hour, minute, second, offset)
	{
		if ((long)(uint)millisecond >= 1000L)
		{
			DateTime.ThrowMillisecondOutOfRange();
		}
		_dateTime = DateTime.CreateUnchecked(UtcTicks + (uint)(millisecond * 10000));
	}

	public DateTimeOffset(int year, int month, int day, int hour, int minute, int second, int millisecond, Calendar calendar, TimeSpan offset)
	{
		ArgumentNullException.ThrowIfNull(calendar, "calendar");
		_offsetMinutes = ValidateOffset(offset);
		if (second != 60 || !DateTime.SystemSupportsLeapSeconds)
		{
			_dateTime = ValidateDate(calendar.ToDateTime(year, month, day, hour, minute, second, millisecond), offset);
		}
		else
		{
			_dateTime = WithLeapSecond(calendar, year, month, day, hour, minute, millisecond, offset);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static DateTime WithLeapSecond(Calendar calendar, int year, int month, int day, int hour, int minute, int millisecond, TimeSpan offset)
	{
		DateTimeOffset dateTimeOffset = new DateTimeOffset(year, month, day, hour, minute, 59, millisecond, calendar, offset);
		DateTime.ValidateLeapSecond(dateTimeOffset.UtcDateTime);
		return dateTimeOffset._dateTime;
	}

	public DateTimeOffset(int year, int month, int day, int hour, int minute, int second, int millisecond, int microsecond, TimeSpan offset)
		: this(year, month, day, hour, minute, second, millisecond, offset)
	{
		if ((long)(uint)microsecond >= 1000L)
		{
			DateTime.ThrowMicrosecondOutOfRange();
		}
		_dateTime = DateTime.CreateUnchecked(UtcTicks + (uint)(microsecond * 10));
	}

	public DateTimeOffset(int year, int month, int day, int hour, int minute, int second, int millisecond, int microsecond, Calendar calendar, TimeSpan offset)
		: this(year, month, day, hour, minute, second, millisecond, calendar, offset)
	{
		if ((long)(uint)microsecond >= 1000L)
		{
			DateTime.ThrowMicrosecondOutOfRange();
		}
		_dateTime = DateTime.CreateUnchecked(UtcTicks + (uint)(microsecond * 10));
	}

	public DateTimeOffset ToOffset(TimeSpan offset)
	{
		return CreateValidateOffset(_dateTime + offset, offset);
	}

	public DateTimeOffset Add(TimeSpan timeSpan)
	{
		return Add(ClockDateTime.Add(timeSpan));
	}

	public DateTimeOffset AddDays(double days)
	{
		return Add(ClockDateTime.AddDays(days));
	}

	public DateTimeOffset AddHours(double hours)
	{
		return Add(ClockDateTime.AddHours(hours));
	}

	public DateTimeOffset AddMilliseconds(double milliseconds)
	{
		return Add(ClockDateTime.AddMilliseconds(milliseconds));
	}

	public DateTimeOffset AddMicroseconds(double microseconds)
	{
		return Add(ClockDateTime.AddMicroseconds(microseconds));
	}

	public DateTimeOffset AddMinutes(double minutes)
	{
		return Add(ClockDateTime.AddMinutes(minutes));
	}

	public DateTimeOffset AddMonths(int months)
	{
		return Add(ClockDateTime.AddMonths(months));
	}

	public DateTimeOffset AddSeconds(double seconds)
	{
		return Add(ClockDateTime.AddSeconds(seconds));
	}

	public DateTimeOffset AddTicks(long ticks)
	{
		return Add(ClockDateTime.AddTicks(ticks));
	}

	public DateTimeOffset AddYears(int years)
	{
		return Add(ClockDateTime.AddYears(years));
	}

	private DateTimeOffset Add(DateTime dateTime)
	{
		return new DateTimeOffset(_offsetMinutes, ValidateDate(dateTime, Offset));
	}

	public static int Compare(DateTimeOffset first, DateTimeOffset second)
	{
		return first.UtcTicks.CompareTo(second.UtcTicks);
	}

	int IComparable.CompareTo(object obj)
	{
		if (obj == null)
		{
			return 1;
		}
		if (!(obj is DateTimeOffset dateTimeOffset))
		{
			throw new ArgumentException(SR.Arg_MustBeDateTimeOffset);
		}
		return UtcTicks.CompareTo(dateTimeOffset.UtcTicks);
	}

	public int CompareTo(DateTimeOffset other)
	{
		return UtcTicks.CompareTo(other.UtcTicks);
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is DateTimeOffset)
		{
			return UtcTicks == ((DateTimeOffset)obj).UtcTicks;
		}
		return false;
	}

	public bool Equals(DateTimeOffset other)
	{
		return UtcTicks == other.UtcTicks;
	}

	public bool EqualsExact(DateTimeOffset other)
	{
		if (UtcTicks == other.UtcTicks)
		{
			return _offsetMinutes == other._offsetMinutes;
		}
		return false;
	}

	public static bool Equals(DateTimeOffset first, DateTimeOffset second)
	{
		return first.UtcTicks == second.UtcTicks;
	}

	public static DateTimeOffset FromFileTime(long fileTime)
	{
		return ToLocalTime(DateTime.FromFileTimeUtc(fileTime), throwOnOverflow: true);
	}

	public static DateTimeOffset FromUnixTimeSeconds(long seconds)
	{
		if (seconds < -62135596800L || seconds > 253402300799L)
		{
			ThrowHelper.ThrowArgumentOutOfRange_Range("seconds", seconds, -62135596800L, 253402300799L);
		}
		long ticks = seconds * 10000000 + 621355968000000000L;
		return new DateTimeOffset(0, DateTime.CreateUnchecked(ticks));
	}

	public static DateTimeOffset FromUnixTimeMilliseconds(long milliseconds)
	{
		if (milliseconds < -62135596800000L || milliseconds > 253402300799999L)
		{
			ThrowHelper.ThrowArgumentOutOfRange_Range("milliseconds", milliseconds, -62135596800000L, 253402300799999L);
		}
		long ticks = milliseconds * 10000 + 621355968000000000L;
		return new DateTimeOffset(0, DateTime.CreateUnchecked(ticks));
	}

	void IDeserializationCallback.OnDeserialization(object sender)
	{
		try
		{
			ValidateOffset(Offset);
			ValidateDate(ClockDateTime, Offset);
		}
		catch (ArgumentException innerException)
		{
			throw new SerializationException(SR.Serialization_InvalidData, innerException);
		}
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		ArgumentNullException.ThrowIfNull(info, "info");
		info.AddValue("DateTime", _dateTime);
		info.AddValue("OffsetMinutes", (short)_offsetMinutes);
	}

	private DateTimeOffset(SerializationInfo info, StreamingContext context)
	{
		ArgumentNullException.ThrowIfNull(info, "info");
		_dateTime = (DateTime)info.GetValue("DateTime", typeof(DateTime));
		_offsetMinutes = (short)info.GetValue("OffsetMinutes", typeof(short));
	}

	public override int GetHashCode()
	{
		return UtcTicks.GetHashCode();
	}

	public static DateTimeOffset Parse(string input)
	{
		if (input == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.input);
		}
		TimeSpan offset;
		return CreateValidateOffset(DateTimeParse.Parse(input.AsSpan(), DateTimeFormatInfo.CurrentInfo, DateTimeStyles.None, out offset), offset);
	}

	public static DateTimeOffset Parse(string input, IFormatProvider? formatProvider)
	{
		return Parse(input, formatProvider, DateTimeStyles.None);
	}

	public static DateTimeOffset Parse(string input, IFormatProvider? formatProvider, DateTimeStyles styles)
	{
		styles = ValidateStyles(styles);
		if (input == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.input);
		}
		TimeSpan offset;
		return CreateValidateOffset(DateTimeParse.Parse(input.AsSpan(), DateTimeFormatInfo.GetInstance(formatProvider), styles, out offset), offset);
	}

	public static DateTimeOffset Parse(ReadOnlySpan<char> input, IFormatProvider? formatProvider = null, DateTimeStyles styles = DateTimeStyles.None)
	{
		styles = ValidateStyles(styles);
		TimeSpan offset;
		return CreateValidateOffset(DateTimeParse.Parse(input, DateTimeFormatInfo.GetInstance(formatProvider), styles, out offset), offset);
	}

	public static DateTimeOffset ParseExact(string input, [StringSyntax("DateTimeFormat")] string format, IFormatProvider? formatProvider)
	{
		return ParseExact(input, format, formatProvider, DateTimeStyles.None);
	}

	public static DateTimeOffset ParseExact(string input, [StringSyntax("DateTimeFormat")] string format, IFormatProvider? formatProvider, DateTimeStyles styles)
	{
		styles = ValidateStyles(styles);
		if (input == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.input);
		}
		if (format == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.format);
		}
		TimeSpan offset;
		return CreateValidateOffset(DateTimeParse.ParseExact(input.AsSpan(), format.AsSpan(), DateTimeFormatInfo.GetInstance(formatProvider), styles, out offset), offset);
	}

	public static DateTimeOffset ParseExact(ReadOnlySpan<char> input, [StringSyntax("DateTimeFormat")] ReadOnlySpan<char> format, IFormatProvider? formatProvider, DateTimeStyles styles = DateTimeStyles.None)
	{
		styles = ValidateStyles(styles);
		TimeSpan offset;
		return CreateValidateOffset(DateTimeParse.ParseExact(input, format, DateTimeFormatInfo.GetInstance(formatProvider), styles, out offset), offset);
	}

	public static DateTimeOffset ParseExact(string input, [StringSyntax("DateTimeFormat")] string[] formats, IFormatProvider? formatProvider, DateTimeStyles styles)
	{
		styles = ValidateStyles(styles);
		if (input == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.input);
		}
		TimeSpan offset;
		return CreateValidateOffset(DateTimeParse.ParseExactMultiple(input.AsSpan(), formats, DateTimeFormatInfo.GetInstance(formatProvider), styles, out offset), offset);
	}

	public static DateTimeOffset ParseExact(ReadOnlySpan<char> input, [StringSyntax("DateTimeFormat")] string[] formats, IFormatProvider? formatProvider, DateTimeStyles styles = DateTimeStyles.None)
	{
		styles = ValidateStyles(styles);
		TimeSpan offset;
		return CreateValidateOffset(DateTimeParse.ParseExactMultiple(input, formats, DateTimeFormatInfo.GetInstance(formatProvider), styles, out offset), offset);
	}

	public TimeSpan Subtract(DateTimeOffset value)
	{
		return new TimeSpan(UtcTicks - value.UtcTicks);
	}

	public DateTimeOffset Subtract(TimeSpan value)
	{
		return Add(ClockDateTime.Subtract(value));
	}

	public long ToFileTime()
	{
		return UtcDateTime.ToFileTimeUtc();
	}

	public long ToUnixTimeSeconds()
	{
		return (long)((ulong)UtcTicks / 10000000uL - 62135596800L);
	}

	public long ToUnixTimeMilliseconds()
	{
		return (long)((ulong)UtcTicks / 10000uL - 62135596800000L);
	}

	public DateTimeOffset ToLocalTime()
	{
		return ToLocalTime(UtcDateTime, throwOnOverflow: false);
	}

	private static DateTimeOffset ToLocalTime(DateTime utcDateTime, bool throwOnOverflow)
	{
		TimeSpan localUtcOffset = TimeZoneInfo.GetLocalUtcOffset(utcDateTime, TimeZoneInfoOptions.NoThrowOnInvalidTime);
		long num = utcDateTime.Ticks + localUtcOffset.Ticks;
		if ((ulong)num > 3155378975999999999uL)
		{
			if (throwOnOverflow)
			{
				throw new ArgumentException(SR.Arg_ArgumentOutOfRangeException);
			}
			num = ((num < 0) ? 0 : 3155378975999999999L);
		}
		return CreateValidateOffset(DateTime.CreateUnchecked(num), localUtcOffset);
	}

	public override string ToString()
	{
		return DateTimeFormat.Format(ClockDateTime, null, null, Offset);
	}

	public string ToString([StringSyntax("DateTimeFormat")] string? format)
	{
		return DateTimeFormat.Format(ClockDateTime, format, null, Offset);
	}

	public string ToString(IFormatProvider? formatProvider)
	{
		return DateTimeFormat.Format(ClockDateTime, null, formatProvider, Offset);
	}

	public string ToString([StringSyntax("DateTimeFormat")] string? format, IFormatProvider? formatProvider)
	{
		return DateTimeFormat.Format(ClockDateTime, format, formatProvider, Offset);
	}

	public bool TryFormat(Span<char> destination, out int charsWritten, [StringSyntax("DateTimeFormat")] ReadOnlySpan<char> format = default(ReadOnlySpan<char>), IFormatProvider? formatProvider = null)
	{
		return DateTimeFormat.TryFormat(ClockDateTime, destination, out charsWritten, format, formatProvider, Offset);
	}

	public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, [StringSyntax("DateTimeFormat")] ReadOnlySpan<char> format = default(ReadOnlySpan<char>), IFormatProvider? formatProvider = null)
	{
		return DateTimeFormat.TryFormat(ClockDateTime, utf8Destination, out bytesWritten, format, formatProvider, Offset);
	}

	public DateTimeOffset ToUniversalTime()
	{
		return new DateTimeOffset(0, _dateTime);
	}

	public static bool TryParse([NotNullWhen(true)] string? input, out DateTimeOffset result)
	{
		bool result2 = DateTimeParse.TryParse(input.AsSpan(), DateTimeFormatInfo.CurrentInfo, DateTimeStyles.None, out var result3, out var offset);
		result = CreateValidateOffset(result3, offset);
		return result2;
	}

	public static bool TryParse(ReadOnlySpan<char> input, out DateTimeOffset result)
	{
		bool result2 = DateTimeParse.TryParse(input, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.None, out var result3, out var offset);
		result = CreateValidateOffset(result3, offset);
		return result2;
	}

	public static bool TryParse([NotNullWhen(true)] string? input, IFormatProvider? formatProvider, DateTimeStyles styles, out DateTimeOffset result)
	{
		styles = ValidateStyles(styles);
		if (input == null)
		{
			result = default(DateTimeOffset);
			return false;
		}
		bool result2 = DateTimeParse.TryParse(input.AsSpan(), DateTimeFormatInfo.GetInstance(formatProvider), styles, out var result3, out var offset);
		result = CreateValidateOffset(result3, offset);
		return result2;
	}

	public static bool TryParse(ReadOnlySpan<char> input, IFormatProvider? formatProvider, DateTimeStyles styles, out DateTimeOffset result)
	{
		styles = ValidateStyles(styles);
		bool result2 = DateTimeParse.TryParse(input, DateTimeFormatInfo.GetInstance(formatProvider), styles, out var result3, out var offset);
		result = CreateValidateOffset(result3, offset);
		return result2;
	}

	public static bool TryParseExact([NotNullWhen(true)] string? input, [NotNullWhen(true)][StringSyntax("DateTimeFormat")] string? format, IFormatProvider? formatProvider, DateTimeStyles styles, out DateTimeOffset result)
	{
		styles = ValidateStyles(styles);
		if (input == null || format == null)
		{
			result = default(DateTimeOffset);
			return false;
		}
		bool result2 = DateTimeParse.TryParseExact(input.AsSpan(), format.AsSpan(), DateTimeFormatInfo.GetInstance(formatProvider), styles, out var result3, out var offset);
		result = CreateValidateOffset(result3, offset);
		return result2;
	}

	public static bool TryParseExact(ReadOnlySpan<char> input, [StringSyntax("DateTimeFormat")] ReadOnlySpan<char> format, IFormatProvider? formatProvider, DateTimeStyles styles, out DateTimeOffset result)
	{
		styles = ValidateStyles(styles);
		bool result2 = DateTimeParse.TryParseExact(input, format, DateTimeFormatInfo.GetInstance(formatProvider), styles, out var result3, out var offset);
		result = CreateValidateOffset(result3, offset);
		return result2;
	}

	public static bool TryParseExact([NotNullWhen(true)] string? input, [NotNullWhen(true)][StringSyntax("DateTimeFormat")] string?[]? formats, IFormatProvider? formatProvider, DateTimeStyles styles, out DateTimeOffset result)
	{
		styles = ValidateStyles(styles);
		if (input == null)
		{
			result = default(DateTimeOffset);
			return false;
		}
		bool result2 = DateTimeParse.TryParseExactMultiple(input.AsSpan(), formats, DateTimeFormatInfo.GetInstance(formatProvider), styles, out var result3, out var offset);
		result = CreateValidateOffset(result3, offset);
		return result2;
	}

	public static bool TryParseExact(ReadOnlySpan<char> input, [NotNullWhen(true)][StringSyntax("DateTimeFormat")] string?[]? formats, IFormatProvider? formatProvider, DateTimeStyles styles, out DateTimeOffset result)
	{
		styles = ValidateStyles(styles);
		bool result2 = DateTimeParse.TryParseExactMultiple(input, formats, DateTimeFormatInfo.GetInstance(formatProvider), styles, out var result3, out var offset);
		result = CreateValidateOffset(result3, offset);
		return result2;
	}

	private static int ValidateOffset(TimeSpan offset)
	{
		long num = offset.Ticks / 600000000;
		if (offset.Ticks != num * 600000000)
		{
			ThrowOffsetPrecision();
		}
		if (num < -840 || num > 840)
		{
			ThrowOffsetOutOfRange();
		}
		return (int)num;
		static void ThrowOffsetOutOfRange()
		{
			throw new ArgumentOutOfRangeException("offset", SR.Argument_OffsetOutOfRange);
		}
		static void ThrowOffsetPrecision()
		{
			throw new ArgumentException(SR.Argument_OffsetPrecision, "offset");
		}
	}

	private static DateTime ValidateDate(DateTime dateTime, TimeSpan offset)
	{
		long num = dateTime.Ticks - offset.Ticks;
		if ((ulong)num > 3155378975999999999uL)
		{
			ThrowOutOfRange();
		}
		return DateTime.CreateUnchecked(num);
		static void ThrowOutOfRange()
		{
			throw new ArgumentOutOfRangeException("offset", SR.Argument_UTCOutOfRange);
		}
	}

	private static DateTimeStyles ValidateStyles(DateTimeStyles styles)
	{
		if ((styles & ~(DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeLocal | DateTimeStyles.AssumeUniversal | DateTimeStyles.RoundtripKind)) != DateTimeStyles.None || (styles & (DateTimeStyles.AssumeLocal | DateTimeStyles.AssumeUniversal)) == (DateTimeStyles.AssumeLocal | DateTimeStyles.AssumeUniversal))
		{
			ThrowInvalid(styles);
		}
		return styles & ~(DateTimeStyles.AssumeLocal | DateTimeStyles.RoundtripKind);
		static void ThrowInvalid(DateTimeStyles dateTimeStyles)
		{
			throw new ArgumentException(((dateTimeStyles & ~(DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeLocal | DateTimeStyles.AssumeUniversal | DateTimeStyles.RoundtripKind)) != DateTimeStyles.None) ? SR.Argument_InvalidDateTimeStyles : (((dateTimeStyles & (DateTimeStyles.AssumeLocal | DateTimeStyles.AssumeUniversal)) == (DateTimeStyles.AssumeLocal | DateTimeStyles.AssumeUniversal)) ? SR.Argument_ConflictingDateTimeStyles : SR.Argument_DateTimeOffsetInvalidDateTimeStyles), "styles");
		}
	}

	public static implicit operator DateTimeOffset(DateTime dateTime)
	{
		return new DateTimeOffset(dateTime);
	}

	public static DateTimeOffset operator +(DateTimeOffset dateTimeOffset, TimeSpan timeSpan)
	{
		return dateTimeOffset.Add(dateTimeOffset.ClockDateTime + timeSpan);
	}

	public static DateTimeOffset operator -(DateTimeOffset dateTimeOffset, TimeSpan timeSpan)
	{
		return dateTimeOffset.Add(dateTimeOffset.ClockDateTime - timeSpan);
	}

	public static TimeSpan operator -(DateTimeOffset left, DateTimeOffset right)
	{
		return new TimeSpan(left.UtcTicks - right.UtcTicks);
	}

	public static bool operator ==(DateTimeOffset left, DateTimeOffset right)
	{
		return left.UtcTicks == right.UtcTicks;
	}

	public static bool operator !=(DateTimeOffset left, DateTimeOffset right)
	{
		return left.UtcTicks != right.UtcTicks;
	}

	public static bool operator <(DateTimeOffset left, DateTimeOffset right)
	{
		return left.UtcTicks < right.UtcTicks;
	}

	public static bool operator <=(DateTimeOffset left, DateTimeOffset right)
	{
		return left.UtcTicks <= right.UtcTicks;
	}

	public static bool operator >(DateTimeOffset left, DateTimeOffset right)
	{
		return left.UtcTicks > right.UtcTicks;
	}

	public static bool operator >=(DateTimeOffset left, DateTimeOffset right)
	{
		return left.UtcTicks >= right.UtcTicks;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Deconstruct(out DateOnly date, out TimeOnly time, out TimeSpan offset)
	{
		(date, time) = ClockDateTime;
		offset = Offset;
	}

	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out DateTimeOffset result)
	{
		return TryParse(s, provider, DateTimeStyles.None, out result);
	}

	public static DateTimeOffset Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
	{
		return Parse(s, provider, DateTimeStyles.None);
	}

	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out DateTimeOffset result)
	{
		return TryParse(s, provider, DateTimeStyles.None, out result);
	}
}
