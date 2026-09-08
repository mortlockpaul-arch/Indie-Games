namespace System.Buffers;

internal static class CharacterFrequencyHelper
{
	public static ReadOnlySpan<float> AsciiFrequency => new float[128]
	{
		0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0.001f,
		0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
		0.003f, 0f, 0f, 0f, 0f, 0.004f, 0f, 0f, 0.006f, 0.006f,
		0f, 0f, 8.952f, 0.065f, 0.42f, 0.01f, 0.011f, 0.005f, 0.07f, 0.05f,
		3.911f, 3.91f, 0.356f, 2.775f, 1.411f, 0.173f, 2.054f, 0.677f, 1.199f, 0.87f,
		0.729f, 0.491f, 0.335f, 0.269f, 0.435f, 0.24f, 0.234f, 0.196f, 0.144f, 0.983f,
		0.357f, 0.661f, 0.371f, 0.088f, 0.007f, 0.763f, 0.229f, 0.551f, 0.306f, 0.449f,
		0.337f, 0.162f, 0.131f, 0.489f, 0.031f, 0.035f, 0.301f, 0.205f, 0.253f, 0.228f,
		0.288f, 0.034f, 0.38f, 0.73f, 0.675f, 0.265f, 0.309f, 0.137f, 0.084f, 0.023f,
		0.023f, 0.591f, 0.085f, 0.59f, 0.013f, 0.797f, 0.001f, 4.596f, 1.296f, 2.081f,
		2.005f, 6.903f, 1.494f, 1.019f, 1.024f, 3.75f, 0.286f, 0.439f, 2.913f, 1.459f,
		3.908f, 3.23f, 1.444f, 0.231f, 4.22f, 3.924f, 5.312f, 2.112f, 0.737f, 0.573f,
		0.992f, 1.067f, 0.181f, 0.391f, 0.056f, 0.391f, 0.002f, 0f
	};

	public static void GetSingleStringMultiCharacterOffsets(string value, bool ignoreCase, out int ch2Offset, out int ch3Offset)
	{
		ch2Offset = IndexOfAsciiCharWithLowestFrequency(value.AsSpan(), ignoreCase);
		ch3Offset = 0;
		if (ch2Offset < 0)
		{
			ch2Offset = value.Length - 1;
		}
		if (value.Length > 2)
		{
			ch3Offset = IndexOfAsciiCharWithLowestFrequency(value.AsSpan(), ignoreCase, ch2Offset);
			if (ch3Offset < 0)
			{
				if (ignoreCase)
				{
					ch3Offset = 0;
				}
				else
				{
					ch3Offset = value.Length - 1;
					if (ch2Offset == ch3Offset)
					{
						ch2Offset--;
					}
				}
			}
		}
		if (ch3Offset > 0 && ch3Offset < ch2Offset)
		{
			int num = ch3Offset;
			int num2 = ch2Offset;
			ch2Offset = num;
			ch3Offset = num2;
		}
	}

	private static int IndexOfAsciiCharWithLowestFrequency(ReadOnlySpan<char> span, bool ignoreCase, int excludeIndex = -1)
	{
		float num = float.MaxValue;
		int result = -1;
		for (int i = 1; i < span.Length; i++)
		{
			if (i == excludeIndex)
			{
				continue;
			}
			char c = span[i];
			if (char.IsAscii(c))
			{
				float num2 = AsciiFrequency[c];
				if (ignoreCase)
				{
					num2 += AsciiFrequency[c ^ 0x20];
				}
				if (i <= 2)
				{
					num2 *= 1.5f;
				}
				if (num2 <= num)
				{
					num = num2;
					result = i;
				}
			}
		}
		return result;
	}
}
