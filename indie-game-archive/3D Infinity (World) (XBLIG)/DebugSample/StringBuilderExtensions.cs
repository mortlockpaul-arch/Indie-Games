using System;
using System.Globalization;
using System.Text;

namespace DebugSample;

public static class StringBuilderExtensions
{
	private static int[] numberGroupSizes = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSizes;

	private static char[] numberString = new char[32];

	public static void AppendNumber(this StringBuilder builder, int number)
	{
		AppendNumbernternal(builder, number, 0, AppendNumberOptions.None);
	}

	public static void AppendNumber(this StringBuilder builder, int number, AppendNumberOptions options)
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
		int number2 = (int)(number * (float)Math.Pow(10.0, decimalCount) + 0.5f);
		AppendNumbernternal(builder, number2, decimalCount, options);
	}

	private static void AppendNumbernternal(StringBuilder builder, int number, int decimalCount, AppendNumberOptions options)
	{
		NumberFormatInfo numberFormat = CultureInfo.CurrentCulture.NumberFormat;
		int num = numberString.Length;
		int num2 = num - decimalCount;
		if (num2 == num)
		{
			num2 = num + 1;
		}
		int num3 = 0;
		int num4 = numberGroupSizes[num3] + decimalCount;
		bool flag = (options & AppendNumberOptions.NumberGroup) != 0;
		bool flag2 = (options & AppendNumberOptions.PositiveSign) != 0;
		bool flag3 = number < 0;
		number = Math.Abs(number);
		do
		{
			if (num == num2)
			{
				numberString[--num] = numberFormat.NumberDecimalSeparator[0];
			}
			if (--num4 < 0 && flag)
			{
				numberString[--num] = numberFormat.NumberGroupSeparator[0];
				if (num3 < numberGroupSizes.Length - 1)
				{
					num3++;
				}
				num4 = numberGroupSizes[num3++];
			}
			numberString[--num] = (char)(48 + number % 10);
			number /= 10;
		}
		while (number > 0 || num2 <= num);
		if (flag3)
		{
			numberString[--num] = numberFormat.NegativeSign[0];
		}
		else if (flag2)
		{
			numberString[--num] = numberFormat.PositiveSign[0];
		}
		builder.Append(numberString, num, numberString.Length - num);
	}

	public static void Remove(this StringBuilder builder)
	{
		builder.Remove(0, builder.Length);
	}

	public static void SetString(this StringBuilder builder, string text)
	{
		builder.Remove();
		builder.Append(text);
	}
}
