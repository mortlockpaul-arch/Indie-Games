using System.Runtime.CompilerServices;

namespace System.Globalization;

public static class ISOWeek
{
	public static int GetWeekOfYear(DateTime date)
	{
		int weekNumber = GetWeekNumber(date);
		if (weekNumber < 1)
		{
			return GetWeeksInYear(date.Year - 1);
		}
		if (weekNumber > 52 && GetWeeksInYear(date.Year) == 52)
		{
			return 1;
		}
		return weekNumber;
	}

	public static int GetWeekOfYear(DateOnly date)
	{
		return GetWeekOfYear(date.GetEquivalentDateTime());
	}

	public static int GetYear(DateTime date)
	{
		int weekNumber = GetWeekNumber(date);
		int num = date.Year;
		if (weekNumber < 1)
		{
			num--;
		}
		else if (weekNumber > 52 && GetWeeksInYear(num) == 52)
		{
			num++;
		}
		return num;
	}

	public static int GetYear(DateOnly date)
	{
		return GetYear(date.GetEquivalentDateTime());
	}

	public static DateTime GetYearStart(int year)
	{
		return ToDateTime(year, 1, DayOfWeek.Monday);
	}

	public static DateTime GetYearEnd(int year)
	{
		return ToDateTime(year, GetWeeksInYear(year), DayOfWeek.Sunday);
	}

	public static int GetWeeksInYear(int year)
	{
		if (year < 1 || year > 9999)
		{
			ThrowHelper.ThrowArgumentOutOfRange_Year();
		}
		if (P((uint)year) == 4 || P((uint)(year - 1)) == 3)
		{
			return 53;
		}
		return 52;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static uint P(uint y)
		{
			uint num = y / 100;
			return (y + y / 4 - num + num / 4) % 7;
		}
	}

	public static DateTime ToDateTime(int year, int week, DayOfWeek dayOfWeek)
	{
		if (year < 1 || year > 9999)
		{
			ThrowHelper.ThrowArgumentOutOfRange_Year();
		}
		if (week < 1 || week > 53)
		{
			throw new ArgumentOutOfRangeException("week", SR.ArgumentOutOfRange_Week_ISO);
		}
		if (dayOfWeek < DayOfWeek.Sunday || dayOfWeek > (DayOfWeek)7)
		{
			throw new ArgumentOutOfRangeException("dayOfWeek", SR.ArgumentOutOfRange_DayOfWeek);
		}
		DateTime dateTime = new DateTime(year, 1, 4);
		int num = GetWeekday(dateTime.DayOfWeek) + 3;
		int num2 = week * 7 + GetWeekday(dayOfWeek) - num;
		return dateTime.AddTicks((num2 - 4) * 864000000000L);
	}

	public static DateOnly ToDateOnly(int year, int week, DayOfWeek dayOfWeek)
	{
		return DateOnly.FromDateTime(ToDateTime(year, week, dayOfWeek));
	}

	private static int GetWeekNumber(DateTime date)
	{
		return (int)((uint)(date.DayOfYear - GetWeekday(date.DayOfWeek) + 10) / 7u);
	}

	private static int GetWeekday(DayOfWeek dayOfWeek)
	{
		if (dayOfWeek != DayOfWeek.Sunday)
		{
			return (int)dayOfWeek;
		}
		return 7;
	}
}
