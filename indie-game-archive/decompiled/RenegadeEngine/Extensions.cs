using System;
using System.Text;

namespace RenegadeEngine;

public static class Extensions
{
	public static void IntToString(this StringBuilder strBuilder, int value)
	{
		if (value < 0)
		{
			strBuilder.Append("-");
		}
		int num = 100000000;
		int num2 = 0;
		int num3 = 0;
		int num4 = Math.Abs(value);
		while (num >= 10)
		{
			if (num4 >= num)
			{
				num3 = num4 % (num * 10);
				num2 = num3 / num;
				strBuilder.Append((char)(num2 + 48));
			}
			num /= 10;
		}
		if (num4 >= 0)
		{
			num3 = num4 % 10;
			num2 = num3 / 1;
			strBuilder.Append((char)(num2 + 48));
		}
	}
}
