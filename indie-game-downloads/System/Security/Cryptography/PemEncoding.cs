using System.Buffers;
using System.Buffers.Text;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Unicode;

namespace System.Security.Cryptography;

public static class PemEncoding
{
	private interface IPemEncoder<TChar> where TChar : IEquatable<TChar>
	{
		static abstract ReadOnlySpan<TChar> PreEBPrefix { get; }

		static abstract ReadOnlySpan<TChar> PostEBPrefix { get; }

		static abstract ReadOnlySpan<TChar> Ending { get; }

		static abstract ReadOnlySpan<TChar> Whitespace { get; }

		static abstract ReadOnlySpan<TChar> NewLine { get; }

		static abstract bool IsValidBase64(ReadOnlySpan<TChar> base64Text, out int decodedLength);

		static abstract int WriteBase64(ReadOnlySpan<byte> bytes, Span<TChar> destination, int offset);
	}

	private sealed class Utf16PemEncoder : IPemEncoder<char>
	{
		public static ReadOnlySpan<char> PreEBPrefix => "-----BEGIN ".AsSpan();

		public static ReadOnlySpan<char> PostEBPrefix => "-----END ".AsSpan();

		public static ReadOnlySpan<char> Ending => "-----".AsSpan();

		public static ReadOnlySpan<char> Whitespace => " \t\n\r".AsSpan();

		public static ReadOnlySpan<char> NewLine => "\n".AsSpan();

		public static bool IsValidBase64(ReadOnlySpan<char> base64Text, out int decodedLength)
		{
			return Base64.IsValid(base64Text, out decodedLength);
		}

		public static int WriteBase64(ReadOnlySpan<byte> bytes, Span<char> destination, int offset)
		{
			if (!Convert.TryToBase64Chars(bytes, destination.Slice(offset), out var charsWritten))
			{
				throw new ArgumentException(null, "destination");
			}
			return charsWritten;
		}
	}

	private sealed class Utf8PemEncoder : IPemEncoder<byte>
	{
		public static ReadOnlySpan<byte> PreEBPrefix => "-----BEGIN "u8;

		public static ReadOnlySpan<byte> PostEBPrefix => "-----END "u8;

		public static ReadOnlySpan<byte> Ending => "-----"u8;

		public static ReadOnlySpan<byte> Whitespace => " \t\n\r"u8;

		public static ReadOnlySpan<byte> NewLine => "\n"u8;

		public static bool IsValidBase64(ReadOnlySpan<byte> base64Text, out int decodedLength)
		{
			return Base64.IsValid(base64Text, out decodedLength);
		}

		public static int WriteBase64(ReadOnlySpan<byte> bytes, Span<byte> destination, int offset)
		{
			if (Base64.EncodeToUtf8(bytes, destination.Slice(offset), out var _, out var bytesWritten) != OperationStatus.Done)
			{
				throw new ArgumentException(null, "destination");
			}
			return bytesWritten;
		}
	}

	public static PemFields Find(ReadOnlySpan<char> pemData)
	{
		if (!TryFind(pemData, out var fields))
		{
			throw new ArgumentException(System.SR.Argument_PemEncoding_NoPemFound, "pemData");
		}
		return fields;
	}

	public static PemFields FindUtf8(ReadOnlySpan<byte> pemData)
	{
		if (!TryFindUtf8(pemData, out var fields))
		{
			throw new ArgumentException(System.SR.Argument_PemEncoding_NoPemFound, "pemData");
		}
		return fields;
	}

	public static bool TryFind(ReadOnlySpan<char> pemData, out PemFields fields)
	{
		return TryFindCore<char, Utf16PemEncoder>(pemData, out fields);
	}

	public static bool TryFindUtf8(ReadOnlySpan<byte> pemData, out PemFields fields)
	{
		return TryFindCore<byte, Utf8PemEncoder>(pemData, out fields);
	}

	private static bool TryFindCore<TChar, T>(ReadOnlySpan<TChar> pemData, out PemFields fields) where TChar : unmanaged, IEquatable<TChar>, INumber<TChar> where T : IPemEncoder<TChar>
	{
		if (pemData.Length < T.PreEBPrefix.Length + T.Ending.Length * 2 + T.PostEBPrefix.Length)
		{
			fields = default(PemFields);
			return false;
		}
		Span<TChar> span = stackalloc TChar[256];
		int num = 0;
		int num2;
		while ((num2 = pemData.IndexOfByOffset(T.PreEBPrefix, num)) >= 0)
		{
			int num3 = num2 + T.PreEBPrefix.Length;
			if (num2 > 0 && !IsWhiteSpaceCharacter(pemData[num2 - 1], T.Whitespace))
			{
				num = num3;
				continue;
			}
			int num4 = pemData.IndexOfByOffset(T.Ending, num3);
			if (num4 < 0)
			{
				fields = default(PemFields);
				return false;
			}
			Range range = num3..num4;
			Range range2 = range;
			ReadOnlySpan<TChar> readOnlySpan = pemData[range2.Start..range2.End];
			if (IsValidLabel(readOnlySpan))
			{
				int num5 = num4 + T.Ending.Length;
				int num6 = T.PostEBPrefix.Length + readOnlySpan.Length + T.Ending.Length;
				Span<TChar> destination = ((num6 > 256) ? ((Span<TChar>)new TChar[num6]) : span);
				ReadOnlySpan<TChar> value = WritePostEB(readOnlySpan, destination);
				int num7 = pemData.IndexOfByOffset(value, num5);
				if (num7 >= 0)
				{
					int num8 = num7 + num6;
					if (num8 >= pemData.Length - 1 || IsWhiteSpaceCharacter(pemData[num8], T.Whitespace))
					{
						Range range3 = num5..num7;
						range2 = range3;
						if (TryCountBase64<TChar, T>(pemData[range2.Start..range2.End], out var base64Start, out var base64End, out var base64DecodedSize))
						{
							Range location = num2..num8;
							Range base64data = (num5 + base64Start)..(num5 + base64End);
							fields = new PemFields(range, base64data, location, base64DecodedSize);
							return true;
						}
					}
				}
			}
			if (num4 <= num)
			{
				fields = default(PemFields);
				return false;
			}
			num = num4;
		}
		fields = default(PemFields);
		return false;
		static ReadOnlySpan<TChar> WritePostEB(ReadOnlySpan<TChar> label, Span<TChar> destination2)
		{
			int length = T.PostEBPrefix.Length + label.Length + T.Ending.Length;
			T.PostEBPrefix.CopyTo(destination2);
			label.CopyTo(destination2.Slice(T.PostEBPrefix.Length));
			T.Ending.CopyTo(destination2.Slice(T.PostEBPrefix.Length + label.Length));
			return destination2.Slice(0, length);
		}
	}

	private static int IndexOfByOffset<TChar>(this ReadOnlySpan<TChar> str, ReadOnlySpan<TChar> value, int startPosition) where TChar : IEquatable<TChar>
	{
		int num = str.Slice(startPosition).IndexOf(value);
		if (num != -1)
		{
			return num + startPosition;
		}
		return -1;
	}

	private static bool IsValidLabel<TChar>(ReadOnlySpan<TChar> data) where TChar : IEquatable<TChar>, INumber<TChar>
	{
		if (data.IsEmpty)
		{
			return true;
		}
		bool flag = false;
		for (int i = 0; i < data.Length; i++)
		{
			TChar val = data[i];
			if (IsLabelChar(val))
			{
				flag = true;
				continue;
			}
			if ((!(val == TChar.CreateTruncating(' ')) && !(val == TChar.CreateTruncating('-'))) || !flag)
			{
				return false;
			}
			flag = false;
		}
		return flag;
		static bool IsLabelChar(TChar c)
		{
			if (c - TChar.CreateTruncating(33u) <= TChar.CreateTruncating(93u))
			{
				return c != TChar.CreateTruncating('-');
			}
			return false;
		}
	}

	private static bool TryCountBase64<TChar, T>(ReadOnlySpan<TChar> str, out int base64Start, out int base64End, out int base64DecodedSize) where TChar : IEquatable<TChar> where T : IPemEncoder<TChar>
	{
		int i = 0;
		int num = str.Length - 1;
		for (; i < str.Length && IsWhiteSpaceCharacter(str[i], T.Whitespace); i++)
		{
		}
		while (num > i && IsWhiteSpaceCharacter(str[num], T.Whitespace))
		{
			num--;
		}
		if (T.IsValidBase64(str.Slice(i, num + 1 - i), out base64DecodedSize))
		{
			base64Start = i;
			base64End = num + 1;
			return true;
		}
		base64Start = 0;
		base64End = 0;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsWhiteSpaceCharacter<TChar>(TChar ch, ReadOnlySpan<TChar> whitespace) where TChar : IEquatable<TChar>
	{
		return whitespace.Contains(ch);
	}

	public static int GetEncodedSize(int labelLength, int dataLength)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(labelLength, "labelLength");
		ArgumentOutOfRangeException.ThrowIfNegative(dataLength, "dataLength");
		if (labelLength > 1073741808)
		{
			throw new ArgumentOutOfRangeException("labelLength", System.SR.Argument_PemEncoding_EncodedSizeTooLarge);
		}
		if (dataLength > 1585834053)
		{
			throw new ArgumentOutOfRangeException("dataLength", System.SR.Argument_PemEncoding_EncodedSizeTooLarge);
		}
		int num = Utf16PemEncoder.PreEBPrefix.Length + labelLength + Utf16PemEncoder.Ending.Length;
		int num2 = Utf16PemEncoder.PostEBPrefix.Length + labelLength + Utf16PemEncoder.Ending.Length;
		int num3 = num + num2 + 1;
		int num4 = (dataLength + 2) / 3 << 2;
		int num5 = Math.DivRem(num4, 64, out var result);
		if (result > 0)
		{
			num5++;
		}
		int num6 = num4 + num5;
		if (int.MaxValue - num6 < num3)
		{
			throw new ArgumentException(System.SR.Argument_PemEncoding_EncodedSizeTooLarge);
		}
		return num6 + num3;
	}

	public static bool TryWrite(ReadOnlySpan<char> label, ReadOnlySpan<byte> data, Span<char> destination, out int charsWritten)
	{
		if (!IsValidLabel(label))
		{
			throw new ArgumentException(System.SR.Argument_PemEncoding_InvalidLabel, "label");
		}
		int encodedSize = GetEncodedSize(label.Length, data.Length);
		if (destination.Length < encodedSize)
		{
			charsWritten = 0;
			return false;
		}
		charsWritten = WriteCore<char, Utf16PemEncoder>(label, data, destination);
		return true;
	}

	public static bool TryWriteUtf8(ReadOnlySpan<byte> utf8Label, ReadOnlySpan<byte> data, Span<byte> destination, out int bytesWritten)
	{
		if (!Utf8.IsValid(utf8Label) || !IsValidLabel(utf8Label))
		{
			throw new ArgumentException(System.SR.Argument_PemEncoding_InvalidLabel, "utf8Label");
		}
		int encodedSize = GetEncodedSize(utf8Label.Length, data.Length);
		if (destination.Length < encodedSize)
		{
			bytesWritten = 0;
			return false;
		}
		bytesWritten = WriteCore<byte, Utf8PemEncoder>(utf8Label, data, destination);
		return true;
	}

	public static byte[] WriteUtf8(ReadOnlySpan<byte> utf8Label, ReadOnlySpan<byte> data)
	{
		if (!Utf8.IsValid(utf8Label) || !IsValidLabel(utf8Label))
		{
			throw new ArgumentException(System.SR.Argument_PemEncoding_InvalidLabel, "utf8Label");
		}
		byte[] array = new byte[GetEncodedSize(utf8Label.Length, data.Length)];
		WriteCore<byte, Utf8PemEncoder>(utf8Label, data, array);
		return array;
	}

	private static int WriteCore<TChar, T>(ReadOnlySpan<TChar> label, ReadOnlySpan<byte> data, Span<TChar> destination) where TChar : IEquatable<TChar> where T : IPemEncoder<TChar>
	{
		int num = 0;
		num += Write(T.PreEBPrefix, destination, num);
		num += Write(label, destination, num);
		num += Write(T.Ending, destination, num);
		num += Write(T.NewLine, destination, num);
		ReadOnlySpan<byte> bytes = data;
		while (bytes.Length >= 48)
		{
			num += T.WriteBase64(bytes.Slice(0, 48), destination, num);
			num += Write(T.NewLine, destination, num);
			bytes = bytes.Slice(48);
		}
		if (bytes.Length > 0)
		{
			num += T.WriteBase64(bytes, destination, num);
			num += Write(T.NewLine, destination, num);
		}
		num += Write(T.PostEBPrefix, destination, num);
		num += Write(label, destination, num);
		return num + Write(T.Ending, destination, num);
		static int Write(ReadOnlySpan<TChar> str, Span<TChar> dest, int offset)
		{
			str.CopyTo(dest.Slice(offset));
			return str.Length;
		}
	}

	public static char[] Write(ReadOnlySpan<char> label, ReadOnlySpan<byte> data)
	{
		if (!IsValidLabel(label))
		{
			throw new ArgumentException(System.SR.Argument_PemEncoding_InvalidLabel, "label");
		}
		char[] array = new char[GetEncodedSize(label.Length, data.Length)];
		WriteCore<char, Utf16PemEncoder>(label, data, array);
		return array;
	}

	public unsafe static string WriteString(ReadOnlySpan<char> label, ReadOnlySpan<byte> data)
	{
		if (!IsValidLabel(label))
		{
			throw new ArgumentException(System.SR.Argument_PemEncoding_InvalidLabel, "label");
		}
		return string.Create(GetEncodedSize(label.Length, data.Length), ((nint)(&label), (nint)(&data)), delegate(Span<char> destination, (nint LabelPointer, nint DataPointer) state)
		{
			ReadOnlySpan<char> item = Unsafe.Read<ReadOnlySpan<char>>((void*)state.LabelPointer);
			ReadOnlySpan<byte> item2 = Unsafe.Read<ReadOnlySpan<byte>>((void*)state.DataPointer);
			if (WriteCore<char, Utf16PemEncoder>(item, item2, destination) != destination.Length)
			{
				throw new CryptographicException();
			}
		});
	}
}
