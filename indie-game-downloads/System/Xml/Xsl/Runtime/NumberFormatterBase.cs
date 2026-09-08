using System.Text;

namespace System.Xml.Xsl.Runtime;

internal abstract class NumberFormatterBase
{
	public static void ConvertToAlphabetic(StringBuilder sb, double val, char firstChar, int totalChars)
	{
		Span<char> span = stackalloc char[7];
		int num = 7;
		int num2 = (int)val;
		while (num2 > totalChars)
		{
			int num3 = --num2 / totalChars;
			span[--num] = (char)(firstChar + (num2 - num3 * totalChars));
			num2 = num3;
		}
		span[--num] = (char)(firstChar + --num2);
		sb.Append(span.Slice(num, 7 - num));
	}

	public static void ConvertToRoman(StringBuilder sb, double val, bool upperCase)
	{
		int num = (int)val;
		string value = (upperCase ? "IIVIXXLXCCDCM" : "iivixxlxccdcm");
		ReadOnlySpan<int> readOnlySpan = new int[13]
		{
			1, 4, 5, 9, 10, 40, 50, 90, 100, 400,
			500, 900, 1000
		};
		int length = readOnlySpan.Length;
		while (length-- != 0)
		{
			while (num >= readOnlySpan[length])
			{
				num -= readOnlySpan[length];
				sb.Append(value, length, 1 + (length & 1));
			}
		}
	}
}
