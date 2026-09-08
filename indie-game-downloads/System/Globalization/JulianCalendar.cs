namespace System.Globalization;

public class JulianCalendar : Calendar
{
	public static readonly int JulianEra = 1;

	internal int MaxYear = 9999;

	public override DateTime MinSupportedDateTime => DateTime.MinValue;

	public override DateTime MaxSupportedDateTime => DateTime.MaxValue;

	public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.SolarCalendar;

	internal override CalendarId ID => CalendarId.JULIAN;

	public override int[] Eras => new int[1] { JulianEra };

	public override int TwoDigitYearMax
	{
		get
		{
			return _twoDigitYearMax;
		}
		set
		{
			VerifyWritable();
			if (value < 99 || value > MaxYear)
			{
				throw new ArgumentOutOfRangeException("value", value, SR.Format(SR.ArgumentOutOfRange_Range, 99, MaxYear));
			}
			_twoDigitYearMax = value;
		}
	}

	public JulianCalendar()
	{
		_twoDigitYearMax = 2049;
	}

	internal static void CheckEraRange(int era)
	{
		if (era != 0 && era != JulianEra)
		{
			throw new ArgumentOutOfRangeException("era", era, SR.ArgumentOutOfRange_InvalidEraValue);
		}
	}

	internal void CheckYearEraRange(int year, int era)
	{
		CheckEraRange(era);
		if (year <= 0 || year > MaxYear)
		{
			throw new ArgumentOutOfRangeException("year", year, SR.Format(SR.ArgumentOutOfRange_Range, 1, MaxYear));
		}
	}

	internal static void CheckMonthRange(int month)
	{
		if (month < 1 || month > 12)
		{
			ThrowHelper.ThrowArgumentOutOfRange_Month(month);
		}
	}

	internal static void CheckDayRange(int year, int month, int day)
	{
		if (year == 1 && month == 1 && day < 3)
		{
			throw new ArgumentOutOfRangeException(null, SR.ArgumentOutOfRange_BadYearMonthDay);
		}
		ReadOnlySpan<int> readOnlySpan = ((year % 4 == 0) ? GregorianCalendar.DaysToMonth366 : GregorianCalendar.DaysToMonth365);
		int num = readOnlySpan[month] - readOnlySpan[month - 1];
		if (day < 1 || day > num)
		{
			throw new ArgumentOutOfRangeException("day", day, SR.Format(SR.ArgumentOutOfRange_Range, 1, num));
		}
	}

	private static int GetDatePart(long ticks, int part)
	{
		int num = (int)((ticks + 1728000000000L) / 864000000000L);
		int num2 = num / 1461;
		num -= num2 * 1461;
		int num3 = num / 365;
		if (num3 == 4)
		{
			num3 = 3;
		}
		if (part == 0)
		{
			return num2 * 4 + num3 + 1;
		}
		num -= num3 * 365;
		if (part == 1)
		{
			return num + 1;
		}
		ReadOnlySpan<int> readOnlySpan = ((num3 == 3) ? GregorianCalendar.DaysToMonth366 : GregorianCalendar.DaysToMonth365);
		int i;
		for (i = (num >> 5) + 1; num >= readOnlySpan[i]; i++)
		{
		}
		if (part == 2)
		{
			return i;
		}
		return num - readOnlySpan[i - 1] + 1;
	}

	internal static long DateToTicks(int year, int month, int day)
	{
		ReadOnlySpan<int> readOnlySpan = ((year % 4 == 0) ? GregorianCalendar.DaysToMonth366 : GregorianCalendar.DaysToMonth365);
		int num = year - 1;
		return (num * 365 + num / 4 + readOnlySpan[month - 1] + day - 1 - 2) * 864000000000L;
	}

	public override DateTime AddMonths(DateTime time, int months)
	{
		if (months < -120000 || months > 120000)
		{
			throw new ArgumentOutOfRangeException("months", months, SR.Format(SR.ArgumentOutOfRange_Range, -120000, 120000));
		}
		int datePart = GetDatePart(time.Ticks, 0);
		int datePart2 = GetDatePart(time.Ticks, 2);
		int num = GetDatePart(time.Ticks, 3);
		int num2 = datePart2 - 1 + months;
		if (num2 >= 0)
		{
			datePart2 = num2 % 12 + 1;
			datePart += num2 / 12;
		}
		else
		{
			datePart2 = 12 + (num2 + 1) % 12;
			datePart += (num2 - 11) / 12;
		}
		ReadOnlySpan<int> readOnlySpan = ((datePart % 4 == 0 && (datePart % 100 != 0 || datePart % 400 == 0)) ? GregorianCalendar.DaysToMonth366 : GregorianCalendar.DaysToMonth365);
		int num3 = readOnlySpan[datePart2] - readOnlySpan[datePart2 - 1];
		if (num > num3)
		{
			num = num3;
		}
		long ticks = DateToTicks(datePart, datePart2, num) + time.Ticks % 864000000000L;
		Calendar.CheckAddResult(ticks, MinSupportedDateTime, MaxSupportedDateTime);
		return new DateTime(ticks);
	}

	public override DateTime AddYears(DateTime time, int years)
	{
		return AddMonths(time, years * 12);
	}

	public override int GetDayOfMonth(DateTime time)
	{
		return GetDatePart(time.Ticks, 3);
	}

	public override DayOfWeek GetDayOfWeek(DateTime time)
	{
		return time.DayOfWeek;
	}

	public override int GetDayOfYear(DateTime time)
	{
		return GetDatePart(time.Ticks, 1);
	}

	public override int GetDaysInMonth(int year, int month, int era)
	{
		CheckYearEraRange(year, era);
		CheckMonthRange(month);
		ReadOnlySpan<int> readOnlySpan = ((year % 4 == 0) ? GregorianCalendar.DaysToMonth366 : GregorianCalendar.DaysToMonth365);
		return readOnlySpan[month] - readOnlySpan[month - 1];
	}

	public override int GetDaysInYear(int year, int era)
	{
		if (!IsLeapYear(year, era))
		{
			return 365;
		}
		return 366;
	}

	public override int GetEra(DateTime time)
	{
		return JulianEra;
	}

	public override int GetMonth(DateTime time)
	{
		return GetDatePart(time.Ticks, 2);
	}

	public override int GetMonthsInYear(int year, int era)
	{
		CheckYearEraRange(year, era);
		return 12;
	}

	public override int GetYear(DateTime time)
	{
		return GetDatePart(time.Ticks, 0);
	}

	public override bool IsLeapDay(int year, int month, int day, int era)
	{
		CheckMonthRange(month);
		if (IsLeapYear(year, era))
		{
			CheckDayRange(year, month, day);
			if (month == 2)
			{
				return day == 29;
			}
			return false;
		}
		CheckDayRange(year, month, day);
		return false;
	}

	public override int GetLeapMonth(int year, int era)
	{
		CheckYearEraRange(year, era);
		return 0;
	}

	public override bool IsLeapMonth(int year, int month, int era)
	{
		CheckYearEraRange(year, era);
		CheckMonthRange(month);
		return false;
	}

	public override bool IsLeapYear(int year, int era)
	{
		CheckYearEraRange(year, era);
		return year % 4 == 0;
	}

	public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
	{
		CheckYearEraRange(year, era);
		CheckMonthRange(month);
		CheckDayRange(year, month, day);
		if (millisecond < 0 || (long)millisecond >= 1000L)
		{
			throw new ArgumentOutOfRangeException("millisecond", millisecond, SR.Format(SR.ArgumentOutOfRange_Range, 0, 999L));
		}
		if (hour < 0 || hour >= 24 || minute < 0 || minute >= 60 || second < 0 || second >= 60)
		{
			throw new ArgumentOutOfRangeException(null, SR.ArgumentOutOfRange_BadHourMinuteSecond);
		}
		return new DateTime(DateToTicks(year, month, day) + new TimeSpan(0, hour, minute, second, millisecond).Ticks);
	}

	public override int ToFourDigitYear(int year)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(year, "year");
		if (year > MaxYear)
		{
			throw new ArgumentOutOfRangeException("year", year, SR.Format(SR.ArgumentOutOfRange_Bounds_Lower_Upper, 1, MaxYear));
		}
		return base.ToFourDigitYear(year);
	}
}
