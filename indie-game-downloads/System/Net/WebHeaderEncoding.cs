namespace System.Net;

internal static class WebHeaderEncoding
{
	internal unsafe static string GetString(byte[] bytes, int byteIndex, int byteCount)
	{
		if (byteCount < 1)
		{
			return string.Empty;
		}
		return string.Create(byteCount, (bytes, byteIndex), delegate(Span<char> buffer, (byte[] bytes, int byteIndex) state)
		{
			fixed (byte* ptr = &state.bytes[state.byteIndex])
			{
				fixed (char* ptr2 = buffer)
				{
					byte* ptr3 = ptr;
					char* ptr4 = ptr2;
					int num;
					for (num = buffer.Length; num >= 8; num -= 8)
					{
						*ptr4 = (char)(*ptr3);
						ptr4[1] = (char)ptr3[1];
						ptr4[2] = (char)ptr3[2];
						ptr4[3] = (char)ptr3[3];
						ptr4[4] = (char)ptr3[4];
						ptr4[5] = (char)ptr3[5];
						ptr4[6] = (char)ptr3[6];
						ptr4[7] = (char)ptr3[7];
						ptr4 += 8;
						ptr3 += 8;
					}
					for (int i = 0; i < num; i++)
					{
						ptr4[i] = (char)ptr3[i];
					}
				}
			}
		});
	}

	internal static int GetByteCount(string myString)
	{
		return myString.Length;
	}

	internal unsafe static void GetBytes(string myString, int charIndex, int charCount, byte[] bytes, int byteIndex)
	{
		if (myString.Length == 0)
		{
			return;
		}
		fixed (byte* ptr = bytes)
		{
			byte* ptr2 = ptr + byteIndex;
			int num = charIndex + charCount;
			while (charIndex < num)
			{
				*(ptr2++) = (byte)myString[charIndex++];
			}
		}
	}
}
