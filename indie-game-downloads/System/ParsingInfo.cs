using System.Globalization;

namespace System;

internal struct ParsingInfo(Calendar calendar)
{
	internal Calendar calendar = calendar;

	internal int dayOfWeek = -1;

	internal DateTimeParse.TM timeMark = DateTimeParse.TM.NotSet;

	internal bool fUseHour12 = false;

	internal bool fUseTwoDigitYear = false;

	internal bool fAllowInnerWhite = false;

	internal bool fAllowTrailingWhite = false;

	internal bool fUseHebrewNumberParser = false;
}
