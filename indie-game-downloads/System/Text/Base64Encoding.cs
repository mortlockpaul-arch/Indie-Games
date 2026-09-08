namespace System.Text;

internal sealed class Base64Encoding : Encoding
{
	private const string Val2Char = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

	private static ReadOnlySpan<byte> Char2val => new byte[128]
	{
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 62, 255, 255, 255, 63, 52, 53,
		54, 55, 56, 57, 58, 59, 60, 61, 255, 255,
		255, 64, 255, 255, 255, 0, 1, 2, 3, 4,
		5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
		15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
		25, 255, 255, 255, 255, 255, 255, 26, 27, 28,
		29, 30, 31, 32, 33, 34, 35, 36, 37, 38,
		39, 40, 41, 42, 43, 44, 45, 46, 47, 48,
		49, 50, 51, 255, 255, 255, 255, 255
	};

	private static ReadOnlySpan<byte> Val2byte => "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"u8;

	public override int GetMaxByteCount(int charCount)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(charCount, "charCount");
		if (charCount % 4 != 0)
		{
			throw new FormatException(System.SR.Format(System.SR.XmlInvalidBase64Length, charCount.ToString()));
		}
		return charCount / 4 * 3;
	}

	private static bool IsValidLeadBytes(int v1, int v2, int v3, int v4)
	{
		if ((v1 | v2) < 64)
		{
			return (v3 | v4) != 255;
		}
		return false;
	}

	private static bool IsValidTailBytes(int v3, int v4)
	{
		if (v3 == 64)
		{
			return v4 == 64;
		}
		return true;
	}

	public unsafe override int GetByteCount(char[] chars, int index, int count)
	{
		ArgumentNullException.ThrowIfNull(chars, "chars");
		ArgumentOutOfRangeException.ThrowIfNegative(index, "index");
		if (index > chars.Length)
		{
			throw new ArgumentOutOfRangeException("index", System.SR.Format(System.SR.OffsetExceedsBufferSize, chars.Length));
		}
		ArgumentOutOfRangeException.ThrowIfNegative(count, "count");
		if (count > chars.Length - index)
		{
			throw new ArgumentOutOfRangeException("count", System.SR.Format(System.SR.SizeExceedsRemainingBufferSpace, chars.Length - index));
		}
		if (count == 0)
		{
			return 0;
		}
		if (count % 4 != 0)
		{
			throw new FormatException(System.SR.Format(System.SR.XmlInvalidBase64Length, count.ToString()));
		}
		fixed (byte* ptr = &Char2val[0])
		{
			fixed (char* ptr2 = &chars[index])
			{
				int num = 0;
				char* ptr3 = ptr2;
				for (char* ptr4 = ptr2 + count; ptr3 < ptr4; ptr3 += 4)
				{
					char c = *ptr3;
					char c2 = ptr3[1];
					char c3 = ptr3[2];
					char c4 = ptr3[3];
					if ((c | c2 | c3 | c4) >= 128)
					{
						throw new FormatException(System.SR.Format(System.SR.XmlInvalidBase64Sequence, new string(ptr3, 0, 4), index + (int)(ptr3 - ptr2)));
					}
					int v = ptr[(int)c];
					int v2 = ptr[(int)c2];
					int num2 = ptr[(int)c3];
					int num3 = ptr[(int)c4];
					if (!IsValidLeadBytes(v, v2, num2, num3) || !IsValidTailBytes(num2, num3))
					{
						throw new FormatException(System.SR.Format(System.SR.XmlInvalidBase64Sequence, new string(ptr3, 0, 4), index + (int)(ptr3 - ptr2)));
					}
					int num4 = ((num3 != 64) ? 3 : ((num2 == 64) ? 1 : 2));
					num += num4;
				}
				return num;
			}
		}
	}

	public unsafe override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
	{
		ArgumentNullException.ThrowIfNull(chars, "chars");
		ArgumentOutOfRangeException.ThrowIfNegative(charIndex, "charIndex");
		if (charIndex > chars.Length)
		{
			throw new ArgumentOutOfRangeException("charIndex", System.SR.Format(System.SR.OffsetExceedsBufferSize, chars.Length));
		}
		ArgumentOutOfRangeException.ThrowIfNegative(charCount, "charCount");
		if (charCount > chars.Length - charIndex)
		{
			throw new ArgumentOutOfRangeException("charCount", System.SR.Format(System.SR.SizeExceedsRemainingBufferSpace, chars.Length - charIndex));
		}
		ArgumentNullException.ThrowIfNull(bytes, "bytes");
		ArgumentOutOfRangeException.ThrowIfNegative(byteIndex, "byteIndex");
		if (byteIndex > bytes.Length)
		{
			throw new ArgumentOutOfRangeException("byteIndex", System.SR.Format(System.SR.OffsetExceedsBufferSize, bytes.Length));
		}
		if (charCount == 0)
		{
			return 0;
		}
		if (charCount % 4 != 0)
		{
			throw new FormatException(System.SR.Format(System.SR.XmlInvalidBase64Length, charCount.ToString()));
		}
		fixed (byte* ptr = &Char2val[0])
		{
			fixed (char* ptr2 = &chars[charIndex])
			{
				fixed (byte* ptr3 = &bytes[byteIndex])
				{
					char* ptr4 = ptr2;
					char* ptr5 = ptr2 + charCount;
					byte* ptr6 = ptr3;
					byte* ptr7 = ptr3 + bytes.Length - byteIndex;
					for (; ptr4 < ptr5; ptr4 += 4)
					{
						char c = *ptr4;
						char c2 = ptr4[1];
						char c3 = ptr4[2];
						char c4 = ptr4[3];
						if ((c | c2 | c3 | c4) >= 128)
						{
							throw new FormatException(System.SR.Format(System.SR.XmlInvalidBase64Sequence, new string(ptr4, 0, 4), charIndex + (int)(ptr4 - ptr2)));
						}
						int num = ptr[(int)c];
						int num2 = ptr[(int)c2];
						int num3 = ptr[(int)c3];
						int num4 = ptr[(int)c4];
						if (!IsValidLeadBytes(num, num2, num3, num4) || !IsValidTailBytes(num3, num4))
						{
							throw new FormatException(System.SR.Format(System.SR.XmlInvalidBase64Sequence, new string(ptr4, 0, 4), charIndex + (int)(ptr4 - ptr2)));
						}
						int num5 = ((num4 != 64) ? 3 : ((num3 == 64) ? 1 : 2));
						if (ptr6 + num5 > ptr7)
						{
							throw new ArgumentException(System.SR.XmlArrayTooSmall, "bytes");
						}
						*ptr6 = (byte)((num << 2) | ((num2 >> 4) & 3));
						if (num5 > 1)
						{
							ptr6[1] = (byte)((num2 << 4) | ((num3 >> 2) & 0xF));
							if (num5 > 2)
							{
								ptr6[2] = (byte)((num3 << 6) | (num4 & 0x3F));
							}
						}
						ptr6 += num5;
					}
					return (int)(ptr6 - ptr3);
				}
			}
		}
	}

	public unsafe int GetBytes(byte[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
	{
		ArgumentNullException.ThrowIfNull(chars, "chars");
		ArgumentOutOfRangeException.ThrowIfNegative(charIndex, "charIndex");
		if (charIndex > chars.Length)
		{
			throw new ArgumentOutOfRangeException("charIndex", System.SR.Format(System.SR.OffsetExceedsBufferSize, chars.Length));
		}
		ArgumentOutOfRangeException.ThrowIfNegative(charCount, "charCount");
		if (charCount > chars.Length - charIndex)
		{
			throw new ArgumentOutOfRangeException("charCount", System.SR.Format(System.SR.SizeExceedsRemainingBufferSpace, chars.Length - charIndex));
		}
		ArgumentNullException.ThrowIfNull(bytes, "bytes");
		ArgumentOutOfRangeException.ThrowIfNegative(byteIndex, "byteIndex");
		if (byteIndex > bytes.Length)
		{
			throw new ArgumentOutOfRangeException("byteIndex", System.SR.Format(System.SR.OffsetExceedsBufferSize, bytes.Length));
		}
		if (charCount == 0)
		{
			return 0;
		}
		if (charCount % 4 != 0)
		{
			throw new FormatException(System.SR.Format(System.SR.XmlInvalidBase64Length, charCount.ToString()));
		}
		fixed (byte* ptr = &Char2val[0])
		{
			fixed (byte* ptr2 = &chars[charIndex])
			{
				fixed (byte* ptr3 = &bytes[byteIndex])
				{
					byte* ptr4 = ptr2;
					byte* ptr5 = ptr2 + charCount;
					byte* ptr6 = ptr3;
					byte* ptr7 = ptr3 + bytes.Length - byteIndex;
					for (; ptr4 < ptr5; ptr4 += 4)
					{
						byte b = *ptr4;
						byte b2 = ptr4[1];
						byte b3 = ptr4[2];
						byte b4 = ptr4[3];
						if ((b | b2 | b3 | b4) >= 128)
						{
							throw new FormatException(System.SR.Format(System.SR.XmlInvalidBase64Sequence, "?", charIndex + (int)(ptr4 - ptr2)));
						}
						int num = ptr[(int)b];
						int num2 = ptr[(int)b2];
						int num3 = ptr[(int)b3];
						int num4 = ptr[(int)b4];
						if (!IsValidLeadBytes(num, num2, num3, num4) || !IsValidTailBytes(num3, num4))
						{
							throw new FormatException(System.SR.Format(System.SR.XmlInvalidBase64Sequence, "?", charIndex + (int)(ptr4 - ptr2)));
						}
						int num5 = ((num4 != 64) ? 3 : ((num3 == 64) ? 1 : 2));
						if (ptr6 + num5 > ptr7)
						{
							throw new ArgumentException(System.SR.XmlArrayTooSmall, "bytes");
						}
						*ptr6 = (byte)((num << 2) | ((num2 >> 4) & 3));
						if (num5 > 1)
						{
							ptr6[1] = (byte)((num2 << 4) | ((num3 >> 2) & 0xF));
							if (num5 > 2)
							{
								ptr6[2] = (byte)((num3 << 6) | (num4 & 0x3F));
							}
						}
						ptr6 += num5;
					}
					return (int)(ptr6 - ptr3);
				}
			}
		}
	}

	public override int GetMaxCharCount(int byteCount)
	{
		if (byteCount < 0 || byteCount > 1610612731)
		{
			throw new ArgumentOutOfRangeException("byteCount", System.SR.Format(System.SR.ValueMustBeInRange, 0, 1610612731));
		}
		return (byteCount + 2) / 3 * 4;
	}

	public override int GetCharCount(byte[] bytes, int index, int count)
	{
		return GetMaxCharCount(count);
	}

	public unsafe override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
	{
		ArgumentNullException.ThrowIfNull(bytes, "bytes");
		ArgumentOutOfRangeException.ThrowIfNegative(byteIndex, "byteIndex");
		if (byteIndex > bytes.Length)
		{
			throw new ArgumentOutOfRangeException("byteIndex", System.SR.Format(System.SR.OffsetExceedsBufferSize, bytes.Length));
		}
		ArgumentOutOfRangeException.ThrowIfNegative(byteCount, "byteCount");
		if (byteCount > bytes.Length - byteIndex)
		{
			throw new ArgumentOutOfRangeException("byteCount", System.SR.Format(System.SR.SizeExceedsRemainingBufferSpace, bytes.Length - byteIndex));
		}
		int charCount = GetCharCount(bytes, byteIndex, byteCount);
		ArgumentNullException.ThrowIfNull(chars, "chars");
		ArgumentOutOfRangeException.ThrowIfNegative(charIndex, "charIndex");
		if (charIndex > chars.Length)
		{
			throw new ArgumentOutOfRangeException("charIndex", System.SR.Format(System.SR.OffsetExceedsBufferSize, chars.Length));
		}
		if (charCount < 0 || charCount > chars.Length - charIndex)
		{
			throw new ArgumentException(System.SR.XmlArrayTooSmall, "chars");
		}
		if (byteCount > 0)
		{
			fixed (char* ptr = &"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/".GetPinnableReference())
			{
				fixed (byte* ptr2 = &bytes[byteIndex])
				{
					fixed (char* ptr3 = &chars[charIndex])
					{
						byte* ptr4 = ptr2;
						byte* ptr5 = ptr4 + byteCount - 3;
						char* ptr6 = ptr3;
						while (ptr4 <= ptr5)
						{
							*ptr6 = ptr[*ptr4 >> 2];
							ptr6[1] = ptr[((*ptr4 & 3) << 4) | (ptr4[1] >> 4)];
							ptr6[2] = ptr[((ptr4[1] & 0xF) << 2) | (ptr4[2] >> 6)];
							ptr6[3] = ptr[ptr4[2] & 0x3F];
							ptr4 += 3;
							ptr6 += 4;
						}
						if (ptr4 - ptr5 == 2)
						{
							*ptr6 = ptr[*ptr4 >> 2];
							ptr6[1] = ptr[(*ptr4 & 3) << 4];
							ptr6[2] = '=';
							ptr6[3] = '=';
						}
						else if (ptr4 - ptr5 == 1)
						{
							*ptr6 = ptr[*ptr4 >> 2];
							ptr6[1] = ptr[((*ptr4 & 3) << 4) | (ptr4[1] >> 4)];
							ptr6[2] = ptr[(ptr4[1] & 0xF) << 2];
							ptr6[3] = '=';
						}
					}
				}
			}
		}
		return charCount;
	}

	public unsafe int GetChars(byte[] bytes, int byteIndex, int byteCount, byte[] chars, int charIndex)
	{
		ArgumentNullException.ThrowIfNull(bytes, "bytes");
		ArgumentOutOfRangeException.ThrowIfNegative(byteIndex, "byteIndex");
		if (byteIndex > bytes.Length)
		{
			throw new ArgumentOutOfRangeException("byteIndex", System.SR.Format(System.SR.OffsetExceedsBufferSize, bytes.Length));
		}
		ArgumentOutOfRangeException.ThrowIfNegative(byteCount, "byteCount");
		if (byteCount > bytes.Length - byteIndex)
		{
			throw new ArgumentOutOfRangeException("byteCount", System.SR.Format(System.SR.SizeExceedsRemainingBufferSpace, bytes.Length - byteIndex));
		}
		int charCount = GetCharCount(bytes, byteIndex, byteCount);
		ArgumentNullException.ThrowIfNull(chars, "chars");
		ArgumentOutOfRangeException.ThrowIfNegative(charIndex, "charIndex");
		if (charIndex > chars.Length)
		{
			throw new ArgumentOutOfRangeException("charIndex", System.SR.Format(System.SR.OffsetExceedsBufferSize, chars.Length));
		}
		if (charCount < 0 || charCount > chars.Length - charIndex)
		{
			throw new ArgumentException(System.SR.XmlArrayTooSmall, "chars");
		}
		if (byteCount > 0)
		{
			fixed (byte* ptr = &Val2byte[0])
			{
				fixed (byte* ptr2 = &bytes[byteIndex])
				{
					fixed (byte* ptr3 = &chars[charIndex])
					{
						byte* ptr4 = ptr2;
						byte* ptr5 = ptr4 + byteCount - 3;
						byte* ptr6 = ptr3;
						while (ptr4 <= ptr5)
						{
							*ptr6 = ptr[*ptr4 >> 2];
							ptr6[1] = ptr[((*ptr4 & 3) << 4) | (ptr4[1] >> 4)];
							ptr6[2] = ptr[((ptr4[1] & 0xF) << 2) | (ptr4[2] >> 6)];
							ptr6[3] = ptr[ptr4[2] & 0x3F];
							ptr4 += 3;
							ptr6 += 4;
						}
						if (ptr4 - ptr5 == 2)
						{
							*ptr6 = ptr[*ptr4 >> 2];
							ptr6[1] = ptr[(*ptr4 & 3) << 4];
							ptr6[2] = 61;
							ptr6[3] = 61;
						}
						else if (ptr4 - ptr5 == 1)
						{
							*ptr6 = ptr[*ptr4 >> 2];
							ptr6[1] = ptr[((*ptr4 & 3) << 4) | (ptr4[1] >> 4)];
							ptr6[2] = ptr[(ptr4[1] & 0xF) << 2];
							ptr6[3] = 61;
						}
					}
				}
			}
		}
		return charCount;
	}
}
