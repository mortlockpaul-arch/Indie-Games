using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace Quasar.Global;

public static class StringBuilderExtensions
{
	private static int[] numberGroupSizes = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSizes;

	private static Dictionary<int, char[]> numberStrings = new Dictionary<int, char[]>(4);

	public static void AppendDate(this StringBuilder builder, DateTime when)
	{
		builder.AppendNumber(when.Month, 2, AppendNumberOptions.FixedSize);
		builder.Append("/");
		builder.AppendNumber(when.Day, 2, AppendNumberOptions.FixedSize);
		builder.Append("/");
		builder.AppendNumber(when.Year, 4, AppendNumberOptions.FixedSize);
	}

	public static void AppendNumber(this StringBuilder builder, long number)
	{
		AppendNumbernternal(builder, number, 0, AppendNumberOptions.None);
	}

	public static void AppendNumber(this StringBuilder builder, int number)
	{
		AppendNumbernternal(builder, number, 0, AppendNumberOptions.None);
	}

	public static void AppendNumber(this StringBuilder builder, int number, AppendNumberOptions options)
	{
		AppendNumbernternal(builder, number, 0, options);
	}

	public static void AppendNumber(this StringBuilder builder, long number, AppendNumberOptions options)
	{
		AppendNumbernternal(builder, number, 0, options);
	}

	public static void AppendNumber(this StringBuilder builder, float number)
	{
		builder.AppendNumber(number, 2, AppendNumberOptions.None);
	}

	public static void AppendNumber(this StringBuilder builder, float number, AppendNumberOptions options)
	{
		builder.AppendNumber(number, 2, options);
	}

	public static void AppendNumber(this StringBuilder builder, float number, int decimalCount, AppendNumberOptions options)
	{
		if (float.IsNaN(number))
		{
			builder.Append("NaN");
			return;
		}
		if (float.IsNegativeInfinity(number))
		{
			builder.Append("-Infinity");
			return;
		}
		if (float.IsPositiveInfinity(number))
		{
			builder.Append("+Infinity");
			return;
		}
		bool flag = (options & AppendNumberOptions.FixedSize) != 0;
		int num = (int)(number * (flag ? 1f : ((float)Math.Pow(10.0, decimalCount))) + 0.5f);
		AppendNumbernternal(builder, num, decimalCount, options);
	}

	private static char[] GetNumberString()
	{
		int managedThreadId = Thread.CurrentThread.ManagedThreadId;
		if (!numberStrings.TryGetValue(managedThreadId, out var value))
		{
			value = new char[48];
			numberStrings.Add(managedThreadId, value);
		}
		return value;
	}

	private static void AppendNumbernternal(StringBuilder builder, long number, int decimalCount, AppendNumberOptions options)
	{
		char[] numberString = GetNumberString();
		NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;
		int num = numberString.Length;
		bool flag = (options & AppendNumberOptions.FixedSize) != 0;
		int num2 = decimalCount;
		int num3 = num - decimalCount;
		if (num3 == num)
		{
			num3 = num + 1;
		}
		int num4 = 0;
		int num5 = numberGroupSizes[num4] + ((!flag) ? decimalCount : 0);
		bool flag2 = (options & AppendNumberOptions.NumberGroup) != 0;
		bool flag3 = (options & AppendNumberOptions.PositiveSign) != 0;
		bool flag4 = number < 0;
		number = Math.Abs(number);
		bool flag5 = number == 0;
		do
		{
			if (!flag && num == num3)
			{
				numberString[--num] = numberFormat.NumberDecimalSeparator[0];
			}
			if (--num5 < 0 && flag2)
			{
				numberString[--num] = numberFormat.NumberGroupSeparator[0];
				if (num4 < numberGroupSizes.Length - 1)
				{
					num4++;
				}
				num5 = numberGroupSizes[num4] - 1;
			}
			if ((options & AppendNumberOptions.EmptySpaces) != AppendNumberOptions.None && number == 0)
			{
				numberString[--num] = ' ';
			}
			else
			{
				numberString[--num] = (char)(48 + number % 10);
			}
			number /= 10;
			num2--;
		}
		while (number > 0 || (flag ? (num2 > 0) : (num3 <= num)));
		if (flag5)
		{
			numberString[numberString.Length - 1] = '0';
		}
		if (flag4)
		{
			numberString[--num] = numberFormat.NegativeSign[0];
		}
		else if (flag3)
		{
			numberString[--num] = numberFormat.PositiveSign[0];
		}
		builder.Append(numberString, num, numberString.Length - num);
	}
}
