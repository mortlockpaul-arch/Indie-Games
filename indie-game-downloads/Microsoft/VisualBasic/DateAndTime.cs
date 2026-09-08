using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.VisualBasic;

[StandardModule]
public sealed class DateAndTime
{
	private static string[] AcceptedDateFormatsDBCS = new string[4] { "yyyy-M-d", "y-M-d", "yyyy/M/d", "y/M/d" };

	private static string[] AcceptedDateFormatsSBCS = new string[4] { "M-d-yyyy", "M-d-y", "M/d/yyyy", "M/d/y" };

	public static DateTime Today
	{
		get
		{
			return DateTime.Today;
		}
		[SupportedOSPlatform("windows")]
		set
		{
			Utils.SetDate(value);
		}
	}

	public static DateTime Now => DateTime.Now;

	public static DateTime TimeOfDay
	{
		get
		{
			long ticks = DateTime.Now.TimeOfDay.Ticks;
			checked
			{
				return new DateTime(ticks - unchecked(ticks % 10000000));
			}
		}
		[SupportedOSPlatform("windows")]
		set
		{
			Utils.SetTime(value);
		}
	}

	public static string TimeString
	{
		get
		{
			return new DateTime(DateTime.Now.TimeOfDay.Ticks).ToString("HH:mm:ss", Utils.GetInvariantCultureInfo());
		}
		[SupportedOSPlatform("windows")]
		set
		{
			DateTime time;
			try
			{
				time = DateType.FromString(value, Utils.GetInvariantCultureInfo());
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
				throw ExceptionUtils.VbMakeException(new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromStringTo, Strings.Left(value, 32), "Date")), 5);
			}
			Utils.SetTime(time);
		}
	}

	public static string DateString
	{
		get
		{
			if (IsDBCSCulture())
			{
				return DateTime.Today.ToString("yyyy\\-MM\\-dd", Utils.GetInvariantCultureInfo());
			}
			return DateTime.Today.ToString("MM\\-dd\\-yyyy", Utils.GetInvariantCultureInfo());
		}
		[SupportedOSPlatform("windows")]
		set
		{
			DateTime date;
			try
			{
				string s = Utils.ToHalfwidthNumbers(value, Utils.GetCultureInfo());
				date = ((!IsDBCSCulture()) ? DateTime.ParseExact(s, AcceptedDateFormatsSBCS, Utils.GetInvariantCultureInfo(), DateTimeStyles.AllowWhiteSpaces) : DateTime.ParseExact(s, AcceptedDateFormatsDBCS, Utils.GetInvariantCultureInfo(), DateTimeStyles.AllowWhiteSpaces));
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
				throw ExceptionUtils.VbMakeException(new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromStringTo, Strings.Left(value, 32), "Date")), 5);
			}
			Utils.SetDate(date);
		}
	}

	public static double Timer => (double)(DateTime.Now.Ticks % 864000000000L) / 10000000.0;

	private static Calendar CurrentCalendar => Thread.CurrentThread.CurrentCulture.Calendar;

	private static bool IsDBCSCulture()
	{
		if (Marshal.SystemMaxDBCSCharSize == 1)
		{
			return false;
		}
		return true;
	}

	public static DateTime DateAdd(DateInterval Interval, double Number, DateTime DateValue)
	{
		checked
		{
			int num = (int)Number;
			switch (Interval)
			{
			case DateInterval.Year:
				return CurrentCalendar.AddYears(DateValue, num);
			case DateInterval.Month:
				return CurrentCalendar.AddMonths(DateValue, num);
			case DateInterval.DayOfYear:
			case DateInterval.Day:
			case DateInterval.Weekday:
				return DateValue.AddDays(num);
			case DateInterval.WeekOfYear:
				return DateValue.AddDays((double)num * 7.0);
			case DateInterval.Hour:
				return DateValue.AddHours(num);
			case DateInterval.Minute:
				return DateValue.AddMinutes(num);
			case DateInterval.Second:
				return DateValue.AddSeconds(num);
			case DateInterval.Quarter:
				return DateValue.AddMonths(num * 3);
			default:
				throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Interval"));
			}
		}
	}

	public static long DateDiff(DateInterval Interval, DateTime Date1, DateTime Date2, FirstDayOfWeek DayOfWeek = FirstDayOfWeek.Sunday, FirstWeekOfYear WeekOfYear = FirstWeekOfYear.Jan1)
	{
		TimeSpan timeSpan = Date2.Subtract(Date1);
		checked
		{
			switch (Interval)
			{
			case DateInterval.Year:
			{
				Calendar currentCalendar = CurrentCalendar;
				return currentCalendar.GetYear(Date2) - currentCalendar.GetYear(Date1);
			}
			case DateInterval.Month:
			{
				Calendar currentCalendar = CurrentCalendar;
				return (currentCalendar.GetYear(Date2) - currentCalendar.GetYear(Date1)) * 12 + currentCalendar.GetMonth(Date2) - currentCalendar.GetMonth(Date1);
			}
			case DateInterval.DayOfYear:
			case DateInterval.Day:
				return (long)timeSpan.TotalDays;
			case DateInterval.Hour:
				return (long)timeSpan.TotalHours;
			case DateInterval.Minute:
				return (long)timeSpan.TotalMinutes;
			case DateInterval.Second:
				return (long)timeSpan.TotalSeconds;
			case DateInterval.WeekOfYear:
				Date1 = Date1.AddDays(-GetDayOfWeek(Date1, DayOfWeek));
				Date2 = Date2.AddDays(-GetDayOfWeek(Date2, DayOfWeek));
				unchecked
				{
					return checked((long)Date2.Subtract(Date1).TotalDays) / 7;
				}
			case DateInterval.Weekday:
				unchecked
				{
					return checked((long)timeSpan.TotalDays) / 7;
				}
			case DateInterval.Quarter:
			{
				Calendar currentCalendar = CurrentCalendar;
				return (currentCalendar.GetYear(Date2) - currentCalendar.GetYear(Date1)) * 4 + unchecked(checked(currentCalendar.GetMonth(Date2) - 1) / 3) - unchecked(checked(currentCalendar.GetMonth(Date1) - 1) / 3);
			}
			default:
				throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Interval"));
			}
		}
	}

	private static int GetDayOfWeek(DateTime dt, FirstDayOfWeek weekdayFirst)
	{
		switch (weekdayFirst)
		{
		default:
			throw ExceptionUtils.VbMakeException(5);
		case FirstDayOfWeek.System:
			weekdayFirst = (FirstDayOfWeek)checked(Utils.GetDateTimeFormatInfo().FirstDayOfWeek + 1);
			break;
		case FirstDayOfWeek.Sunday:
		case FirstDayOfWeek.Monday:
		case FirstDayOfWeek.Tuesday:
		case FirstDayOfWeek.Wednesday:
		case FirstDayOfWeek.Thursday:
		case FirstDayOfWeek.Friday:
		case FirstDayOfWeek.Saturday:
			break;
		}
		checked
		{
			return unchecked(checked(unchecked((int)dt.DayOfWeek) - unchecked((int)weekdayFirst) + 8) % 7) + 1;
		}
	}

	public static int DatePart(DateInterval Interval, DateTime DateValue, FirstDayOfWeek FirstDayOfWeekValue = FirstDayOfWeek.Sunday, FirstWeekOfYear FirstWeekOfYearValue = FirstWeekOfYear.Jan1)
	{
		switch (Interval)
		{
		case DateInterval.Year:
			return CurrentCalendar.GetYear(DateValue);
		case DateInterval.Month:
			return CurrentCalendar.GetMonth(DateValue);
		case DateInterval.Day:
			return CurrentCalendar.GetDayOfMonth(DateValue);
		case DateInterval.Hour:
			return CurrentCalendar.GetHour(DateValue);
		case DateInterval.Minute:
			return CurrentCalendar.GetMinute(DateValue);
		case DateInterval.Second:
			return CurrentCalendar.GetSecond(DateValue);
		case DateInterval.Weekday:
			return Weekday(DateValue, FirstDayOfWeekValue);
		case DateInterval.WeekOfYear:
		{
			DayOfWeek firstDayOfWeek = ((FirstDayOfWeekValue != FirstDayOfWeek.System) ? ((DayOfWeek)checked(FirstDayOfWeekValue - 1)) : Utils.GetCultureInfo().DateTimeFormat.FirstDayOfWeek);
			CalendarWeekRule rule = default(CalendarWeekRule);
			switch (FirstWeekOfYearValue)
			{
			case FirstWeekOfYear.System:
				rule = Utils.GetCultureInfo().DateTimeFormat.CalendarWeekRule;
				break;
			case FirstWeekOfYear.Jan1:
				rule = CalendarWeekRule.FirstDay;
				break;
			case FirstWeekOfYear.FirstFourDays:
				rule = CalendarWeekRule.FirstFourDayWeek;
				break;
			case FirstWeekOfYear.FirstFullWeek:
				rule = CalendarWeekRule.FirstFullWeek;
				break;
			}
			return CurrentCalendar.GetWeekOfYear(DateValue, rule, firstDayOfWeek);
		}
		case DateInterval.Quarter:
			checked
			{
				return unchecked(checked(DateValue.Month - 1) / 3) + 1;
			}
		case DateInterval.DayOfYear:
			return CurrentCalendar.GetDayOfYear(DateValue);
		default:
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Interval"));
		}
	}

	public static DateTime DateAdd(string Interval, double Number, object DateValue)
	{
		DateTime dateValue;
		try
		{
			dateValue = Conversions.ToDate(DateValue);
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
			throw new InvalidCastException(System.SR.Format(System.SR.Argument_InvalidDateValue1, "DateValue"));
		}
		return DateAdd(DateIntervalFromString(Interval), Number, dateValue);
	}

	public static long DateDiff(string Interval, object Date1, object Date2, FirstDayOfWeek DayOfWeek = FirstDayOfWeek.Sunday, FirstWeekOfYear WeekOfYear = FirstWeekOfYear.Jan1)
	{
		DateTime date;
		try
		{
			date = Conversions.ToDate(Date1);
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
			throw new InvalidCastException(System.SR.Format(System.SR.Argument_InvalidDateValue1, "Date1"));
		}
		DateTime date2;
		try
		{
			date2 = Conversions.ToDate(Date2);
		}
		catch (StackOverflowException ex4)
		{
			throw ex4;
		}
		catch (OutOfMemoryException ex5)
		{
			throw ex5;
		}
		catch (Exception)
		{
			throw new InvalidCastException(System.SR.Format(System.SR.Argument_InvalidDateValue1, "Date2"));
		}
		return DateDiff(DateIntervalFromString(Interval), date, date2, DayOfWeek, WeekOfYear);
	}

	public static int DatePart(string Interval, object DateValue, FirstDayOfWeek DayOfWeek = FirstDayOfWeek.Sunday, FirstWeekOfYear WeekOfYear = FirstWeekOfYear.Jan1)
	{
		DateTime dateValue;
		try
		{
			dateValue = Conversions.ToDate(DateValue);
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
			throw new InvalidCastException(System.SR.Format(System.SR.Argument_InvalidDateValue1, "DateValue"));
		}
		return DatePart(DateIntervalFromString(Interval), dateValue, DayOfWeek, WeekOfYear);
	}

	private static DateInterval DateIntervalFromString(string Interval)
	{
		if (Interval != null)
		{
			Interval = Interval.ToUpperInvariant();
		}
		return Interval switch
		{
			"YYYY" => DateInterval.Year, 
			"Y" => DateInterval.DayOfYear, 
			"M" => DateInterval.Month, 
			"D" => DateInterval.Day, 
			"H" => DateInterval.Hour, 
			"N" => DateInterval.Minute, 
			"S" => DateInterval.Second, 
			"WW" => DateInterval.WeekOfYear, 
			"W" => DateInterval.Weekday, 
			"Q" => DateInterval.Quarter, 
			_ => throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Interval")), 
		};
	}

	public static DateTime DateSerial(int Year, int Month, int Day)
	{
		Calendar currentCalendar = CurrentCalendar;
		checked
		{
			if (Year < 0)
			{
				Year = currentCalendar.GetYear(DateTime.Today) + Year;
			}
			else if (Year < 100)
			{
				Year = currentCalendar.ToFourDigitYear(Year);
			}
			if (currentCalendar is GregorianCalendar && Month >= 1 && Month <= 12 && Day >= 1 && Day <= 28)
			{
				return new DateTime(Year, Month, Day);
			}
			DateTime time;
			try
			{
				time = currentCalendar.ToDateTime(Year, 1, 1, 0, 0, 0, 0);
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
				throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Year")), 5);
			}
			try
			{
				time = currentCalendar.AddMonths(time, Month - 1);
			}
			catch (StackOverflowException ex4)
			{
				throw ex4;
			}
			catch (OutOfMemoryException ex5)
			{
				throw ex5;
			}
			catch (Exception)
			{
				throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Month")), 5);
			}
			try
			{
				time = currentCalendar.AddDays(time, Day - 1);
			}
			catch (StackOverflowException ex7)
			{
				throw ex7;
			}
			catch (OutOfMemoryException ex8)
			{
				throw ex8;
			}
			catch (Exception)
			{
				throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Day")), 5);
			}
			return time;
		}
	}

	public static DateTime TimeSerial(int Hour, int Minute, int Second)
	{
		checked
		{
			int num = Hour * 60 * 60 + Minute * 60 + Second;
			if (num < 0)
			{
				num += 86400;
			}
			return new DateTime(unchecked((long)num) * 10000000L);
		}
	}

	public static DateTime DateValue(string StringDate)
	{
		return Conversions.ToDate(StringDate).Date;
	}

	public static DateTime TimeValue(string StringTime)
	{
		return new DateTime(Conversions.ToDate(StringTime).Ticks % 864000000000L);
	}

	public static int Year(DateTime DateValue)
	{
		return CurrentCalendar.GetYear(DateValue);
	}

	public static int Month(DateTime DateValue)
	{
		return CurrentCalendar.GetMonth(DateValue);
	}

	public static int Day(DateTime DateValue)
	{
		return CurrentCalendar.GetDayOfMonth(DateValue);
	}

	public static int Hour(DateTime TimeValue)
	{
		return CurrentCalendar.GetHour(TimeValue);
	}

	public static int Minute(DateTime TimeValue)
	{
		return CurrentCalendar.GetMinute(TimeValue);
	}

	public static int Second(DateTime TimeValue)
	{
		return CurrentCalendar.GetSecond(TimeValue);
	}

	public static int Weekday(DateTime DateValue, FirstDayOfWeek DayOfWeek = FirstDayOfWeek.Sunday)
	{
		switch (DayOfWeek)
		{
		case FirstDayOfWeek.System:
			DayOfWeek = (FirstDayOfWeek)checked(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek + 1);
			break;
		default:
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "DayOfWeek"));
		case FirstDayOfWeek.Sunday:
		case FirstDayOfWeek.Monday:
		case FirstDayOfWeek.Tuesday:
		case FirstDayOfWeek.Wednesday:
		case FirstDayOfWeek.Thursday:
		case FirstDayOfWeek.Friday:
		case FirstDayOfWeek.Saturday:
			break;
		}
		checked
		{
			return unchecked(checked(unchecked((int)checked(CurrentCalendar.GetDayOfWeek(DateValue) + 1)) - unchecked((int)DayOfWeek) + 7) % 7) + 1;
		}
	}

	public static string MonthName(int Month, bool Abbreviate = false)
	{
		if (Month < 1 || Month > 13)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Month"));
		}
		string text = ((!Abbreviate) ? Utils.GetDateTimeFormatInfo().GetMonthName(Month) : Utils.GetDateTimeFormatInfo().GetAbbreviatedMonthName(Month));
		if (text.Length == 0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Month"));
		}
		return text;
	}

	public static string WeekdayName(int Weekday, bool Abbreviate = false, FirstDayOfWeek FirstDayOfWeekValue = FirstDayOfWeek.System)
	{
		if (Weekday < 1 || Weekday > 7)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Weekday"));
		}
		if (FirstDayOfWeekValue < FirstDayOfWeek.System || FirstDayOfWeekValue > FirstDayOfWeek.Saturday)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "FirstDayOfWeekValue"));
		}
		DateTimeFormatInfo dateTimeFormatInfo = (DateTimeFormatInfo)Utils.GetCultureInfo().GetFormat(typeof(DateTimeFormatInfo));
		if (FirstDayOfWeekValue == FirstDayOfWeek.System)
		{
			FirstDayOfWeekValue = (FirstDayOfWeek)checked(dateTimeFormatInfo.FirstDayOfWeek + 1);
		}
		string text;
		try
		{
			text = ((!Abbreviate) ? dateTimeFormatInfo.GetDayName((DayOfWeek)((int)checked(Weekday + FirstDayOfWeekValue - 2) % 7)) : dateTimeFormatInfo.GetAbbreviatedDayName((DayOfWeek)((int)checked(Weekday + FirstDayOfWeekValue - 2) % 7)));
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
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Weekday"));
		}
		if (text.Length == 0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Weekday"));
		}
		return text;
	}
}
