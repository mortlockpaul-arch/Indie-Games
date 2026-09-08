using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace System;

[Serializable]
public readonly struct TimeSpan : IComparable, IComparable<TimeSpan>, IEquatable<TimeSpan>, ISpanFormattable, IFormattable, ISpanParsable<TimeSpan>, IParsable<TimeSpan>, IUtf8SpanFormattable
{
	public const long NanosecondsPerTick = 100L;

	public const long TicksPerMicrosecond = 10L;

	public const long TicksPerMillisecond = 10000L;

	public const long TicksPerSecond = 10000000L;

	public const long TicksPerMinute = 600000000L;

	public const long TicksPerHour = 36000000000L;

	public const long TicksPerDay = 864000000000L;

	public const long MicrosecondsPerMillisecond = 1000L;

	public const long MicrosecondsPerSecond = 1000000L;

	public const long MicrosecondsPerMinute = 60000000L;

	public const long MicrosecondsPerHour = 3600000000L;

	public const long MicrosecondsPerDay = 86400000000L;

	public const long MillisecondsPerSecond = 1000L;

	public const long MillisecondsPerMinute = 60000L;

	public const long MillisecondsPerHour = 3600000L;

	public const long MillisecondsPerDay = 86400000L;

	public const long SecondsPerMinute = 60L;

	public const long SecondsPerHour = 3600L;

	public const long SecondsPerDay = 86400L;

	public const long MinutesPerHour = 60L;

	public const long MinutesPerDay = 1440L;

	public const int HoursPerDay = 24;

	public static readonly TimeSpan Zero = new TimeSpan(0L);

	public static readonly TimeSpan MaxValue = new TimeSpan(long.MaxValue);

	public static readonly TimeSpan MinValue = new TimeSpan(long.MinValue);

	internal readonly long _ticks;

	public long Ticks => _ticks;

	public int Days => (int)(_ticks / 864000000000L);

	public int Hours => (int)(_ticks / 36000000000L % 24);

	public int Milliseconds => (int)(_ticks / 10000 % 1000);

	public int Microseconds => (int)(_ticks / 10 % 1000);

	public int Nanoseconds => (int)(_ticks % 10 * 100);

	public int Minutes => (int)(_ticks / 600000000 % 60);

	public int Seconds => (int)(_ticks / 10000000 % 60);

	public double TotalDays => (double)_ticks / 864000000000.0;

	public double TotalHours => (double)_ticks / 36000000000.0;

	public double TotalMilliseconds
	{
		get
		{
			double num = (double)_ticks / 10000.0;
			if (num > 922337203685477.0)
			{
				return 922337203685477.0;
			}
			if (num < -922337203685477.0)
			{
				return -922337203685477.0;
			}
			return num;
		}
	}

	public double TotalMicroseconds => (double)_ticks / 10.0;

	public double TotalNanoseconds => (double)_ticks * 100.0;

	public double TotalMinutes => (double)_ticks / 600000000.0;

	public double TotalSeconds => (double)_ticks / 10000000.0;

	public TimeSpan(long ticks)
	{
		_ticks = ticks;
	}

	public TimeSpan(int hours, int minutes, int seconds)
	{
		_ticks = TimeToTicks(hours, minutes, seconds);
	}

	public TimeSpan(int days, int hours, int minutes, int seconds)
		: this(days, hours, minutes, seconds, 0)
	{
	}

	public TimeSpan(int days, int hours, int minutes, int seconds, int milliseconds)
		: this(days, hours, minutes, seconds, milliseconds, 0)
	{
	}

	public TimeSpan(int days, int hours, int minutes, int seconds, int milliseconds, int microseconds)
	{
		long num = days * 86400000000L + hours * 3600000000u + (long)minutes * 60000000L + (long)seconds * 1000000L + (long)milliseconds * 1000L + microseconds;
		if (num > 922337203685477580L || num < -922337203685477580L)
		{
			ThrowHelper.ThrowArgumentOutOfRange_TimeSpanTooLong();
		}
		_ticks = num * 10;
	}

	public TimeSpan Add(TimeSpan ts)
	{
		return this + ts;
	}

	public static int Compare(TimeSpan t1, TimeSpan t2)
	{
		return t1._ticks.CompareTo(t2._ticks);
	}

	public int CompareTo(object? value)
	{
		if (value == null)
		{
			return 1;
		}
		if (value is TimeSpan value2)
		{
			return CompareTo(value2);
		}
		throw new ArgumentException(SR.Arg_MustBeTimeSpan);
	}

	public int CompareTo(TimeSpan value)
	{
		return Compare(this, value);
	}

	public static TimeSpan FromDays(double value)
	{
		return Interval(value, 864000000000.0);
	}

	public TimeSpan Duration()
	{
		if (_ticks == long.MinValue)
		{
			ThrowHelper.ThrowOverflowException_TimeSpanDuration();
		}
		return new TimeSpan((_ticks >= 0) ? _ticks : (-_ticks));
	}

	public override bool Equals([NotNullWhen(true)] object? value)
	{
		if (value is TimeSpan obj)
		{
			return Equals(obj);
		}
		return false;
	}

	public bool Equals(TimeSpan obj)
	{
		return Equals(this, obj);
	}

	public static bool Equals(TimeSpan t1, TimeSpan t2)
	{
		return t1 == t2;
	}

	public override int GetHashCode()
	{
		return _ticks.GetHashCode();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TimeSpan FromUnits(long units, long ticksPerUnit, long minUnits, long maxUnits)
	{
		if (units > maxUnits || units < minUnits)
		{
			ThrowHelper.ThrowArgumentOutOfRange_TimeSpanTooLong();
		}
		return FromTicks(units * ticksPerUnit);
	}

	public static TimeSpan FromDays(int days)
	{
		return FromUnits(days, 864000000000L, -10675199L, 10675199L);
	}

	public static TimeSpan FromDays(int days, int hours = 0, long minutes = 0L, long seconds = 0L, long milliseconds = 0L, long microseconds = 0L)
	{
		return FromMicroseconds(Math.BigMul(days, 86400000000L) + Math.BigMul(hours, 3600000000L) + Math.BigMul(minutes, 60000000L) + Math.BigMul(seconds, 1000000L) + Math.BigMul(milliseconds, 1000L) + microseconds);
	}

	public static TimeSpan FromHours(int hours)
	{
		return FromUnits(hours, 36000000000L, -256204778L, 256204778L);
	}

	public static TimeSpan FromHours(int hours, long minutes = 0L, long seconds = 0L, long milliseconds = 0L, long microseconds = 0L)
	{
		return FromMicroseconds(Math.BigMul(hours, 3600000000L) + Math.BigMul(minutes, 60000000L) + Math.BigMul(seconds, 1000000L) + Math.BigMul(milliseconds, 1000L) + microseconds);
	}

	public static TimeSpan FromMinutes(long minutes)
	{
		return FromUnits(minutes, 600000000L, -15372286728L, 15372286728L);
	}

	public static TimeSpan FromMinutes(long minutes, long seconds = 0L, long milliseconds = 0L, long microseconds = 0L)
	{
		return FromMicroseconds(Math.BigMul(minutes, 60000000L) + Math.BigMul(seconds, 1000000L) + Math.BigMul(milliseconds, 1000L) + microseconds);
	}

	public static TimeSpan FromSeconds(long seconds)
	{
		return FromUnits(seconds, 10000000L, -922337203685L, 922337203685L);
	}

	public static TimeSpan FromSeconds(long seconds, long milliseconds = 0L, long microseconds = 0L)
	{
		return FromMicroseconds(Math.BigMul(seconds, 1000000L) + Math.BigMul(milliseconds, 1000L) + microseconds);
	}

	public static TimeSpan FromMilliseconds(long milliseconds)
	{
		return FromUnits(milliseconds, 10000L, -922337203685477L, 922337203685477L);
	}

	public static TimeSpan FromMilliseconds(long milliseconds, long microseconds)
	{
		return FromMicroseconds(Math.BigMul(milliseconds, 1000L) + microseconds);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TimeSpan FromMicroseconds(Int128 microseconds)
	{
		if (microseconds > 922337203685477580L || microseconds < -922337203685477580L)
		{
			ThrowHelper.ThrowArgumentOutOfRange_TimeSpanTooLong();
		}
		return FromTicks((long)microseconds * 10);
	}

	public static TimeSpan FromMicroseconds(long microseconds)
	{
		return FromUnits(microseconds, 10L, -922337203685477580L, 922337203685477580L);
	}

	public static TimeSpan FromHours(double value)
	{
		return Interval(value, 36000000000.0);
	}

	private static TimeSpan Interval(double value, double scale)
	{
		if (double.IsNaN(value))
		{
			ThrowHelper.ThrowArgumentException_Arg_CannotBeNaN();
		}
		return IntervalFromDoubleTicks(value * scale);
	}

	private static TimeSpan IntervalFromDoubleTicks(double ticks)
	{
		if (ticks > 9.223372036854776E+18 || ticks < -9.223372036854776E+18 || double.IsNaN(ticks))
		{
			ThrowHelper.ThrowOverflowException_TimeSpanTooLong();
		}
		if (ticks == 9.223372036854776E+18)
		{
			return MaxValue;
		}
		return new TimeSpan((long)ticks);
	}

	public static TimeSpan FromMilliseconds(double value)
	{
		return Interval(value, 10000.0);
	}

	public static TimeSpan FromMicroseconds(double value)
	{
		return Interval(value, 10.0);
	}

	public static TimeSpan FromMinutes(double value)
	{
		return Interval(value, 600000000.0);
	}

	public TimeSpan Negate()
	{
		return -this;
	}

	public static TimeSpan FromSeconds(double value)
	{
		return Interval(value, 10000000.0);
	}

	public TimeSpan Subtract(TimeSpan ts)
	{
		return this - ts;
	}

	public TimeSpan Multiply(double factor)
	{
		return this * factor;
	}

	public TimeSpan Divide(double divisor)
	{
		return this / divisor;
	}

	public double Divide(TimeSpan ts)
	{
		return this / ts;
	}

	public static TimeSpan FromTicks(long value)
	{
		return new TimeSpan(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static long TimeToTicks(int hour, int minute, int second)
	{
		long num = (long)hour * 3600L + (long)minute * 60L + second;
		if (num > 922337203685L || num < -922337203685L)
		{
			ThrowHelper.ThrowArgumentOutOfRange_TimeSpanTooLong();
		}
		return num * 10000000;
	}

	private static void ValidateStyles(TimeSpanStyles style)
	{
		if (style != TimeSpanStyles.None && style != TimeSpanStyles.AssumeNegative)
		{
			ThrowHelper.ThrowArgumentException_InvalidTimeSpanStyles();
		}
	}

	public static TimeSpan Parse(string s)
	{
		ArgumentNullException.ThrowIfNull(s, ExceptionArgument.input);
		return TimeSpanParse.Parse(s.AsSpan(), null);
	}

	public static TimeSpan Parse(string input, IFormatProvider? formatProvider)
	{
		if (input == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.input);
		}
		return TimeSpanParse.Parse(input.AsSpan(), formatProvider);
	}

	public static TimeSpan Parse(ReadOnlySpan<char> input, IFormatProvider? formatProvider = null)
	{
		return TimeSpanParse.Parse(input, formatProvider);
	}

	public static TimeSpan ParseExact(string input, [StringSyntax("TimeSpanFormat")] string format, IFormatProvider? formatProvider)
	{
		ArgumentNullException.ThrowIfNull(input, ExceptionArgument.input);
		ArgumentNullException.ThrowIfNull(format, ExceptionArgument.format);
		return TimeSpanParse.ParseExact(input.AsSpan(), format.AsSpan(), formatProvider, TimeSpanStyles.None);
	}

	public static TimeSpan ParseExact(string input, [StringSyntax("TimeSpanFormat")] string[] formats, IFormatProvider? formatProvider)
	{
		if (input == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.input);
		}
		return TimeSpanParse.ParseExactMultiple(input.AsSpan(), formats, formatProvider, TimeSpanStyles.None);
	}

	public static TimeSpan ParseExact(string input, [StringSyntax("TimeSpanFormat")] string format, IFormatProvider? formatProvider, TimeSpanStyles styles)
	{
		ValidateStyles(styles);
		ArgumentNullException.ThrowIfNull(input, ExceptionArgument.input);
		ArgumentNullException.ThrowIfNull(format, ExceptionArgument.format);
		return TimeSpanParse.ParseExact(input.AsSpan(), format.AsSpan(), formatProvider, styles);
	}

	public static TimeSpan ParseExact(ReadOnlySpan<char> input, [StringSyntax("TimeSpanFormat")] ReadOnlySpan<char> format, IFormatProvider? formatProvider, TimeSpanStyles styles = TimeSpanStyles.None)
	{
		ValidateStyles(styles);
		return TimeSpanParse.ParseExact(input, format, formatProvider, styles);
	}

	public static TimeSpan ParseExact(string input, [StringSyntax("TimeSpanFormat")] string[] formats, IFormatProvider? formatProvider, TimeSpanStyles styles)
	{
		ValidateStyles(styles);
		ArgumentNullException.ThrowIfNull(input, ExceptionArgument.input);
		return TimeSpanParse.ParseExactMultiple(input.AsSpan(), formats, formatProvider, styles);
	}

	public static TimeSpan ParseExact(ReadOnlySpan<char> input, [StringSyntax("TimeSpanFormat")] string[] formats, IFormatProvider? formatProvider, TimeSpanStyles styles = TimeSpanStyles.None)
	{
		ValidateStyles(styles);
		return TimeSpanParse.ParseExactMultiple(input, formats, formatProvider, styles);
	}

	public static bool TryParse([NotNullWhen(true)] string? s, out TimeSpan result)
	{
		if (s == null)
		{
			result = default(TimeSpan);
			return false;
		}
		return TimeSpanParse.TryParse(s.AsSpan(), null, out result);
	}

	public static bool TryParse(ReadOnlySpan<char> s, out TimeSpan result)
	{
		return TimeSpanParse.TryParse(s, null, out result);
	}

	public static bool TryParse([NotNullWhen(true)] string? input, IFormatProvider? formatProvider, out TimeSpan result)
	{
		if (input == null)
		{
			result = default(TimeSpan);
			return false;
		}
		return TimeSpanParse.TryParse(input.AsSpan(), formatProvider, out result);
	}

	public static bool TryParse(ReadOnlySpan<char> input, IFormatProvider? formatProvider, out TimeSpan result)
	{
		return TimeSpanParse.TryParse(input, formatProvider, out result);
	}

	public static bool TryParseExact([NotNullWhen(true)] string? input, [NotNullWhen(true)][StringSyntax("TimeSpanFormat")] string? format, IFormatProvider? formatProvider, out TimeSpan result)
	{
		if (input == null || format == null)
		{
			result = default(TimeSpan);
			return false;
		}
		return TimeSpanParse.TryParseExact(input.AsSpan(), format.AsSpan(), formatProvider, TimeSpanStyles.None, out result);
	}

	public static bool TryParseExact(ReadOnlySpan<char> input, [StringSyntax("TimeSpanFormat")] ReadOnlySpan<char> format, IFormatProvider? formatProvider, out TimeSpan result)
	{
		return TimeSpanParse.TryParseExact(input, format, formatProvider, TimeSpanStyles.None, out result);
	}

	public static bool TryParseExact([NotNullWhen(true)] string? input, [NotNullWhen(true)][StringSyntax("TimeSpanFormat")] string?[]? formats, IFormatProvider? formatProvider, out TimeSpan result)
	{
		if (input == null)
		{
			result = default(TimeSpan);
			return false;
		}
		return TimeSpanParse.TryParseExactMultiple(input.AsSpan(), formats, formatProvider, TimeSpanStyles.None, out result);
	}

	public static bool TryParseExact(ReadOnlySpan<char> input, [NotNullWhen(true)][StringSyntax("TimeSpanFormat")] string?[]? formats, IFormatProvider? formatProvider, out TimeSpan result)
	{
		return TimeSpanParse.TryParseExactMultiple(input, formats, formatProvider, TimeSpanStyles.None, out result);
	}

	public static bool TryParseExact([NotNullWhen(true)] string? input, [NotNullWhen(true)][StringSyntax("TimeSpanFormat")] string? format, IFormatProvider? formatProvider, TimeSpanStyles styles, out TimeSpan result)
	{
		ValidateStyles(styles);
		if (input == null || format == null)
		{
			result = default(TimeSpan);
			return false;
		}
		return TimeSpanParse.TryParseExact(input.AsSpan(), format.AsSpan(), formatProvider, styles, out result);
	}

	public static bool TryParseExact(ReadOnlySpan<char> input, [StringSyntax("TimeSpanFormat")] ReadOnlySpan<char> format, IFormatProvider? formatProvider, TimeSpanStyles styles, out TimeSpan result)
	{
		ValidateStyles(styles);
		return TimeSpanParse.TryParseExact(input, format, formatProvider, styles, out result);
	}

	public static bool TryParseExact([NotNullWhen(true)] string? input, [NotNullWhen(true)][StringSyntax("TimeSpanFormat")] string?[]? formats, IFormatProvider? formatProvider, TimeSpanStyles styles, out TimeSpan result)
	{
		ValidateStyles(styles);
		if (input == null)
		{
			result = default(TimeSpan);
			return false;
		}
		return TimeSpanParse.TryParseExactMultiple(input.AsSpan(), formats, formatProvider, styles, out result);
	}

	public static bool TryParseExact(ReadOnlySpan<char> input, [NotNullWhen(true)][StringSyntax("TimeSpanFormat")] string?[]? formats, IFormatProvider? formatProvider, TimeSpanStyles styles, out TimeSpan result)
	{
		ValidateStyles(styles);
		return TimeSpanParse.TryParseExactMultiple(input, formats, formatProvider, styles, out result);
	}

	public override string ToString()
	{
		return TimeSpanFormat.FormatC(this);
	}

	public string ToString([StringSyntax("TimeSpanFormat")] string? format)
	{
		return TimeSpanFormat.Format(this, format, null);
	}

	public string ToString([StringSyntax("TimeSpanFormat")] string? format, IFormatProvider? formatProvider)
	{
		return TimeSpanFormat.Format(this, format, formatProvider);
	}

	public bool TryFormat(Span<char> destination, out int charsWritten, [StringSyntax("TimeSpanFormat")] ReadOnlySpan<char> format = default(ReadOnlySpan<char>), IFormatProvider? formatProvider = null)
	{
		return TimeSpanFormat.TryFormat(this, destination, out charsWritten, format, formatProvider);
	}

	public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, [StringSyntax("TimeSpanFormat")] ReadOnlySpan<char> format = default(ReadOnlySpan<char>), IFormatProvider? formatProvider = null)
	{
		return TimeSpanFormat.TryFormat(this, utf8Destination, out bytesWritten, format, formatProvider);
	}

	public static TimeSpan operator -(TimeSpan t)
	{
		if (t._ticks == long.MinValue)
		{
			ThrowHelper.ThrowOverflowException_NegateTwosCompNum();
		}
		return new TimeSpan(-t._ticks);
	}

	public static TimeSpan operator -(TimeSpan t1, TimeSpan t2)
	{
		long num = t1._ticks - t2._ticks;
		long num2 = t1._ticks >> 63;
		if (num2 != t2._ticks >> 63 && num2 != num >> 63)
		{
			ThrowHelper.ThrowOverflowException_TimeSpanTooLong();
		}
		return new TimeSpan(num);
	}

	public static TimeSpan operator +(TimeSpan t)
	{
		return t;
	}

	public static TimeSpan operator +(TimeSpan t1, TimeSpan t2)
	{
		long num = t1._ticks + t2._ticks;
		long num2 = t1._ticks >> 63;
		if (num2 == t2._ticks >> 63 && num2 != num >> 63)
		{
			ThrowHelper.ThrowOverflowException_TimeSpanTooLong();
		}
		return new TimeSpan(num);
	}

	public static TimeSpan operator *(TimeSpan timeSpan, double factor)
	{
		if (double.IsNaN(factor))
		{
			ThrowHelper.ThrowArgumentException_Arg_CannotBeNaN(ExceptionArgument.factor);
		}
		return IntervalFromDoubleTicks(Math.Round((double)timeSpan.Ticks * factor));
	}

	public static TimeSpan operator *(double factor, TimeSpan timeSpan)
	{
		return timeSpan * factor;
	}

	public static TimeSpan operator /(TimeSpan timeSpan, double divisor)
	{
		if (double.IsNaN(divisor))
		{
			ThrowHelper.ThrowArgumentException_Arg_CannotBeNaN(ExceptionArgument.divisor);
		}
		return IntervalFromDoubleTicks(Math.Round((double)timeSpan.Ticks / divisor));
	}

	public static double operator /(TimeSpan t1, TimeSpan t2)
	{
		return (double)t1.Ticks / (double)t2.Ticks;
	}

	public static bool operator ==(TimeSpan t1, TimeSpan t2)
	{
		return t1._ticks == t2._ticks;
	}

	public static bool operator !=(TimeSpan t1, TimeSpan t2)
	{
		return t1._ticks != t2._ticks;
	}

	public static bool operator <(TimeSpan t1, TimeSpan t2)
	{
		return t1._ticks < t2._ticks;
	}

	public static bool operator <=(TimeSpan t1, TimeSpan t2)
	{
		return t1._ticks <= t2._ticks;
	}

	public static bool operator >(TimeSpan t1, TimeSpan t2)
	{
		return t1._ticks > t2._ticks;
	}

	public static bool operator >=(TimeSpan t1, TimeSpan t2)
	{
		return t1._ticks >= t2._ticks;
	}
}
