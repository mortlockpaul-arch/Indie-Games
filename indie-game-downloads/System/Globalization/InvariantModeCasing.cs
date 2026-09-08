using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Globalization;

internal static class InvariantModeCasing
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static char ToLower(char c)
	{
		return CharUnicodeInfo.ToLower(c);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static char ToUpper(char c)
	{
		return CharUnicodeInfo.ToUpper(c);
	}

	internal static string ToLower(string s)
	{
		if (s.Length == 0)
		{
			return string.Empty;
		}
		ReadOnlySpan<char> readOnlySpan = s.AsSpan();
		int num = 0;
		while (num < s.Length)
		{
			if (char.IsHighSurrogate(readOnlySpan[num]) && num < s.Length - 1 && char.IsLowSurrogate(readOnlySpan[num + 1]))
			{
				SurrogateCasing.ToLower(readOnlySpan[num], readOnlySpan[num + 1], out var hr, out var lr);
				if (readOnlySpan[num] != hr || readOnlySpan[num + 1] != lr)
				{
					break;
				}
				num += 2;
			}
			else
			{
				if (ToLower(readOnlySpan[num]) != readOnlySpan[num])
				{
					break;
				}
				num++;
			}
		}
		if (num >= s.Length)
		{
			return s;
		}
		string text = string.FastAllocateString(s.Length);
		Span<char> destination = new Span<char>(ref text.GetRawStringData(), text.Length);
		ReadOnlySpan<char> readOnlySpan2 = s.AsSpan();
		readOnlySpan2.Slice(0, num).CopyTo(destination);
		ToLower(readOnlySpan2.Slice(num), destination.Slice(num));
		return text;
	}

	internal static string ToUpper(string s)
	{
		if (s.Length == 0)
		{
			return string.Empty;
		}
		ReadOnlySpan<char> readOnlySpan = s.AsSpan();
		int num = 0;
		while (num < s.Length)
		{
			if (char.IsHighSurrogate(readOnlySpan[num]) && num < s.Length - 1 && char.IsLowSurrogate(readOnlySpan[num + 1]))
			{
				SurrogateCasing.ToUpper(readOnlySpan[num], readOnlySpan[num + 1], out var hr, out var lr);
				if (readOnlySpan[num] != hr || readOnlySpan[num + 1] != lr)
				{
					break;
				}
				num += 2;
			}
			else
			{
				if (ToUpper(readOnlySpan[num]) != readOnlySpan[num])
				{
					break;
				}
				num++;
			}
		}
		if (num >= s.Length)
		{
			return s;
		}
		string text = string.FastAllocateString(s.Length);
		Span<char> destination = new Span<char>(ref text.GetRawStringData(), text.Length);
		ReadOnlySpan<char> readOnlySpan2 = s.AsSpan();
		readOnlySpan2.Slice(0, num).CopyTo(destination);
		ToUpper(readOnlySpan2.Slice(num), destination.Slice(num));
		return text;
	}

	internal static void ToUpper(ReadOnlySpan<char> source, Span<char> destination)
	{
		for (int i = 0; i < source.Length; i++)
		{
			char c = source[i];
			if (char.IsHighSurrogate(c) && i < source.Length - 1)
			{
				char c2 = source[i + 1];
				if (char.IsLowSurrogate(c2))
				{
					SurrogateCasing.ToUpper(c, c2, out var hr, out var lr);
					destination[i] = hr;
					destination[i + 1] = lr;
					i++;
					continue;
				}
			}
			destination[i] = ToUpper(c);
		}
	}

	internal static void ToLower(ReadOnlySpan<char> source, Span<char> destination)
	{
		for (int i = 0; i < source.Length; i++)
		{
			char c = source[i];
			if (char.IsHighSurrogate(c) && i < source.Length - 1)
			{
				char c2 = source[i + 1];
				if (char.IsLowSurrogate(c2))
				{
					SurrogateCasing.ToLower(c, c2, out var hr, out var lr);
					destination[i] = hr;
					destination[i + 1] = lr;
					i++;
					continue;
				}
			}
			destination[i] = ToLower(c);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static (uint, int) GetScalar(ref char source, int index, int length)
	{
		char c = source;
		if (!char.IsHighSurrogate(c) || index >= length - 1)
		{
			return (c, 1);
		}
		char c2 = Unsafe.Add(ref source, 1);
		if (!char.IsLowSurrogate(c2))
		{
			return (c, 1);
		}
		return (UnicodeUtility.GetScalarFromUtf16SurrogatePair(c, c2), 2);
	}

	internal static int CompareStringIgnoreCase(ref char strA, int lengthA, ref char strB, int lengthB)
	{
		int num = Math.Min(lengthA, lengthB);
		ref char source = ref strA;
		ref char source2 = ref strB;
		int num2 = 0;
		while (num2 < num)
		{
			var (num3, num4) = GetScalar(ref source, num2, lengthA);
			var (num5, elementOffset) = GetScalar(ref source2, num2, lengthB);
			if (num3 == num5)
			{
				num2 += num4;
				source = ref Unsafe.Add(ref source, num4);
				source2 = ref Unsafe.Add(ref source2, elementOffset);
				continue;
			}
			uint num6 = CharUnicodeInfo.ToUpper(num3);
			uint num7 = CharUnicodeInfo.ToUpper(num5);
			if (num6 == num7)
			{
				num2 += num4;
				source = ref Unsafe.Add(ref source, num4);
				source2 = ref Unsafe.Add(ref source2, elementOffset);
				continue;
			}
			return (int)(num6 - num7);
		}
		return lengthA - lengthB;
	}

	internal unsafe static int IndexOfIgnoreCase(ReadOnlySpan<char> source, ReadOnlySpan<char> value)
	{
		fixed (char* reference = &MemoryMarshal.GetReference(source))
		{
			fixed (char* reference2 = &MemoryMarshal.GetReference(value))
			{
				char* ptr = reference + (source.Length - value.Length);
				char* ptr2 = reference2 + value.Length - 1;
				for (char* ptr3 = reference; ptr3 <= ptr; ptr3++)
				{
					char* ptr4 = reference2;
					char* ptr5 = ptr3;
					while (ptr4 <= ptr2)
					{
						if (!char.IsHighSurrogate(*ptr4) || ptr4 == ptr2)
						{
							if (*ptr4 != *ptr5 && ToUpper(*ptr4) != ToUpper(*ptr5))
							{
								break;
							}
							ptr4++;
							ptr5++;
						}
						else if (char.IsHighSurrogate(*ptr5) && char.IsLowSurrogate(ptr5[1]) && char.IsLowSurrogate(ptr4[1]))
						{
							if (!SurrogateCasing.Equal(*ptr5, ptr5[1], *ptr4, ptr4[1]))
							{
								break;
							}
							ptr5 += 2;
							ptr4 += 2;
						}
						else
						{
							if (*ptr4 != *ptr5)
							{
								break;
							}
							ptr5++;
							ptr4++;
						}
					}
					if (ptr4 > ptr2)
					{
						return (int)(ptr3 - reference);
					}
				}
				return -1;
			}
		}
	}

	internal unsafe static int LastIndexOfIgnoreCase(ReadOnlySpan<char> source, ReadOnlySpan<char> value)
	{
		fixed (char* reference = &MemoryMarshal.GetReference(source))
		{
			fixed (char* reference2 = &MemoryMarshal.GetReference(value))
			{
				char* ptr = reference2 + value.Length - 1;
				for (char* ptr2 = reference + (source.Length - value.Length); ptr2 >= reference; ptr2--)
				{
					char* ptr3 = reference2;
					char* ptr4 = ptr2;
					while (ptr3 <= ptr)
					{
						if (!char.IsHighSurrogate(*ptr3) || ptr3 == ptr)
						{
							if (*ptr3 != *ptr4 && ToUpper(*ptr3) != ToUpper(*ptr4))
							{
								break;
							}
							ptr3++;
							ptr4++;
						}
						else if (char.IsHighSurrogate(*ptr4) && char.IsLowSurrogate(ptr4[1]) && char.IsLowSurrogate(ptr3[1]))
						{
							if (!SurrogateCasing.Equal(*ptr4, ptr4[1], *ptr3, ptr3[1]))
							{
								break;
							}
							ptr4 += 2;
							ptr3 += 2;
						}
						else
						{
							if (*ptr3 != *ptr4)
							{
								break;
							}
							ptr4++;
							ptr3++;
						}
					}
					if (ptr3 > ptr)
					{
						return (int)(ptr2 - reference);
					}
				}
				return -1;
			}
		}
	}
}
