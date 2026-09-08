using System.Globalization;

namespace System;

internal ref struct DateTimeResult
{
	internal int Year;

	internal int Month;

	internal int Day;

	internal int Hour;

	internal int Minute;

	internal int Second;

	internal double fraction;

	internal int era;

	internal ParseFlags flags;

	internal TimeSpan timeZoneOffset;

	internal Calendar calendar;

	internal DateTime parsedDate;

	internal ParseFailureKind failure;

	internal ReadOnlySpan<char> failureSpanArgument;

	internal int failureIntArgument;

	internal void Init(ReadOnlySpan<char> originalDateTimeString)
	{
		failureSpanArgument = originalDateTimeString;
		Year = -1;
		Month = -1;
		Day = -1;
		fraction = -1.0;
		era = -1;
	}

	internal void SetDate(int year, int month, int day)
	{
		Year = year;
		Month = month;
		Day = day;
	}

	internal void SetBadFormatSpecifierFailure()
	{
		SetBadFormatSpecifierFailure(ReadOnlySpan<char>.Empty);
	}

	internal void SetBadFormatSpecifierFailure(ReadOnlySpan<char> failedFormatSpecifier)
	{
		failure = ParseFailureKind.Format_BadFormatSpecifier;
		failureSpanArgument = failedFormatSpecifier;
	}

	internal void SetBadDateTimeFailure()
	{
		failure = ParseFailureKind.Format_BadDateTime;
	}

	internal void SetFailure(ParseFailureKind failure)
	{
		this.failure = failure;
	}

	internal void SetFailure(ParseFailureKind failure, string failureStringArgument)
	{
		this.failure = failure;
		failureSpanArgument = failureStringArgument.AsSpan();
	}

	internal void SetFailure(ParseFailureKind failure, int failureIntArgument)
	{
		this.failure = failure;
		this.failureIntArgument = failureIntArgument;
	}
}
