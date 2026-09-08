using System.Buffers;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace System.Text.Json;

internal static class JsonWriterHelper
{
	internal delegate T WriteCallback<T>(ReadOnlySpan<byte> serializedValue);

	private static readonly StandardFormat s_dateTimeStandardFormat = new StandardFormat('O');

	private static readonly StandardFormat s_hexStandardFormat = new StandardFormat('X', 4);

	private static ReadOnlySpan<byte> AllowList => new byte[256]
	{
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 1, 1, 0, 1, 1, 1, 0, 0,
		1, 1, 1, 0, 1, 1, 1, 1, 1, 1,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		0, 1, 0, 1, 1, 1, 1, 1, 1, 1,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		1, 1, 0, 1, 1, 1, 0, 1, 1, 1,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		1, 1, 1, 1, 1, 1, 1, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0
	};

	public static void WriteIndentation(Span<byte> buffer, int indent, byte indentByte)
	{
		if (indent < 8)
		{
			int num = 0;
			while (num + 1 < indent)
			{
				buffer[num++] = indentByte;
				buffer[num++] = indentByte;
			}
			if (num < indent)
			{
				buffer[num] = indentByte;
			}
		}
		else
		{
			buffer.Slice(0, indent).Fill(indentByte);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ValidateNewLine(string value)
	{
		if (value == null)
		{
			ThrowHelper.ThrowArgumentNullException("value");
		}
		if (!(value == "\n") && !(value == "\r\n"))
		{
			ThrowHelper.ThrowArgumentOutOfRangeException_NewLine("value");
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ValidateIndentCharacter(char value)
	{
		if (value != ' ' && value != '\t')
		{
			ThrowHelper.ThrowArgumentOutOfRangeException_IndentCharacter("value");
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ValidateIndentSize(int value)
	{
		if ((value < 0 || value > 127) ? true : false)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException_IndentSize("value", 0, 127);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ValidateProperty(ReadOnlySpan<byte> propertyName)
	{
		if (propertyName.Length > 166666666)
		{
			ThrowHelper.ThrowArgumentException_PropertyNameTooLarge(propertyName.Length);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ValidateValue(ReadOnlySpan<byte> value)
	{
		if (value.Length > 166666666)
		{
			ThrowHelper.ThrowArgumentException_ValueTooLarge(value.Length);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ValidateDouble(double value)
	{
		if (!JsonHelpers.IsFinite(value))
		{
			ThrowHelper.ThrowArgumentException_ValueNotSupported();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ValidateSingle(float value)
	{
		if (!JsonHelpers.IsFinite(value))
		{
			ThrowHelper.ThrowArgumentException_ValueNotSupported();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ValidateProperty(ReadOnlySpan<char> propertyName)
	{
		if (propertyName.Length > 166666666)
		{
			ThrowHelper.ThrowArgumentException_PropertyNameTooLarge(propertyName.Length);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ValidateValue(ReadOnlySpan<char> value)
	{
		if (value.Length > 166666666)
		{
			ThrowHelper.ThrowArgumentException_ValueTooLarge(value.Length);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ValidatePropertyAndValue(ReadOnlySpan<char> propertyName, ReadOnlySpan<byte> value)
	{
		if (propertyName.Length > 166666666 || value.Length > 166666666)
		{
			ThrowHelper.ThrowArgumentException(propertyName, value);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ValidatePropertyAndValue(ReadOnlySpan<byte> propertyName, ReadOnlySpan<char> value)
	{
		if (propertyName.Length > 166666666 || value.Length > 166666666)
		{
			ThrowHelper.ThrowArgumentException(propertyName, value);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ValidatePropertyAndValue(ReadOnlySpan<byte> propertyName, ReadOnlySpan<byte> value)
	{
		if (propertyName.Length > 166666666 || value.Length > 166666666)
		{
			ThrowHelper.ThrowArgumentException(propertyName, value);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ValidatePropertyAndValue(ReadOnlySpan<char> propertyName, ReadOnlySpan<char> value)
	{
		if (propertyName.Length > 166666666 || value.Length > 166666666)
		{
			ThrowHelper.ThrowArgumentException(propertyName, value);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ValidatePropertyNameLength(ReadOnlySpan<char> propertyName)
	{
		if (propertyName.Length > 166666666)
		{
			ThrowHelper.ThrowPropertyNameTooLargeArgumentException(propertyName.Length);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ValidatePropertyNameLength(ReadOnlySpan<byte> propertyName)
	{
		if (propertyName.Length > 166666666)
		{
			ThrowHelper.ThrowPropertyNameTooLargeArgumentException(propertyName.Length);
		}
	}

	internal static void ValidateNumber(ReadOnlySpan<byte> utf8FormattedNumber)
	{
		int i = 0;
		if (utf8FormattedNumber[i] == 45)
		{
			i++;
			if (utf8FormattedNumber.Length <= i)
			{
				throw new ArgumentException(System.SR.RequiredDigitNotFoundEndOfData, "utf8FormattedNumber");
			}
		}
		if (utf8FormattedNumber[i] == 48)
		{
			i++;
		}
		else
		{
			for (; i < utf8FormattedNumber.Length && JsonHelpers.IsDigit(utf8FormattedNumber[i]); i++)
			{
			}
		}
		if (i == utf8FormattedNumber.Length)
		{
			return;
		}
		byte b = utf8FormattedNumber[i];
		if (b == 46)
		{
			i++;
			if (utf8FormattedNumber.Length <= i)
			{
				throw new ArgumentException(System.SR.RequiredDigitNotFoundEndOfData, "utf8FormattedNumber");
			}
			for (; i < utf8FormattedNumber.Length && JsonHelpers.IsDigit(utf8FormattedNumber[i]); i++)
			{
			}
			if (i == utf8FormattedNumber.Length)
			{
				return;
			}
			b = utf8FormattedNumber[i];
		}
		if (b == 101 || b == 69)
		{
			i++;
			if (utf8FormattedNumber.Length <= i)
			{
				throw new ArgumentException(System.SR.RequiredDigitNotFoundEndOfData, "utf8FormattedNumber");
			}
			b = utf8FormattedNumber[i];
			if (b == 43 || b == 45)
			{
				i++;
			}
			if (utf8FormattedNumber.Length <= i)
			{
				throw new ArgumentException(System.SR.RequiredDigitNotFoundEndOfData, "utf8FormattedNumber");
			}
			for (; i < utf8FormattedNumber.Length && JsonHelpers.IsDigit(utf8FormattedNumber[i]); i++)
			{
			}
			if (i == utf8FormattedNumber.Length)
			{
				return;
			}
			throw new ArgumentException(System.SR.Format(System.SR.ExpectedEndOfDigitNotFound, ThrowHelper.GetPrintableString(utf8FormattedNumber[i])), "utf8FormattedNumber");
		}
		throw new ArgumentException(System.SR.Format(System.SR.ExpectedEndOfDigitNotFound, ThrowHelper.GetPrintableString(b)), "utf8FormattedNumber");
	}

	public static bool IsValidUtf8String(ReadOnlySpan<byte> bytes)
	{
		return Utf8.IsValid(bytes);
	}

	internal static OperationStatus ToUtf8(ReadOnlySpan<char> source, Span<byte> destination, out int written)
	{
		int charsRead;
		return Utf8.FromUtf16(source, destination, out charsRead, out written, replaceInvalidSequences: false);
	}

	internal static T WriteString<T>(ReadOnlySpan<byte> utf8Value, WriteCallback<T> writeCallback)
	{
		int num = NeedsEscaping(utf8Value, JavaScriptEncoder.Default);
		if (num == -1)
		{
			int num2 = utf8Value.Length + 2;
			byte[] array = null;
			try
			{
				Span<byte> span = ((num2 <= 256) ? stackalloc byte[256].Slice(0, num2) : (array = ArrayPool<byte>.Shared.Rent(num2)).AsSpan(0, num2));
				Span<byte> span2 = span;
				span2[0] = 34;
				utf8Value.CopyTo(span2.Slice(1));
				span2[span2.Length - 1] = 34;
				return writeCallback(span2);
			}
			finally
			{
				if (array != null)
				{
					ArrayPool<byte>.Shared.Return(array);
				}
			}
		}
		int num3 = checked(2 + GetMaxEscapedLength(utf8Value.Length, num));
		byte[] array2 = null;
		try
		{
			Span<byte> span3;
			if (num3 > 256)
			{
				array2 = ArrayPool<byte>.Shared.Rent(num3);
				span3 = array2;
			}
			else
			{
				span3 = stackalloc byte[256];
			}
			span3[0] = 34;
			EscapeString(utf8Value, span3.Slice(1), num, JavaScriptEncoder.Default, out var written);
			span3[1 + written] = 34;
			return writeCallback(span3.Slice(0, written + 2));
		}
		finally
		{
			if (array2 != null)
			{
				ArrayPool<byte>.Shared.Return(array2);
			}
		}
	}

	public static void WriteDateTimeTrimmed(Span<byte> buffer, DateTime value, out int bytesWritten)
	{
		Span<byte> destination = stackalloc byte[33];
		Utf8Formatter.TryFormat(value, destination, out bytesWritten, s_dateTimeStandardFormat);
		TrimDateTimeOffset(destination.Slice(0, bytesWritten), out bytesWritten);
		destination.Slice(0, bytesWritten).CopyTo(buffer);
	}

	public static void WriteDateTimeOffsetTrimmed(Span<byte> buffer, DateTimeOffset value, out int bytesWritten)
	{
		Span<byte> destination = stackalloc byte[33];
		Utf8Formatter.TryFormat(value, destination, out bytesWritten, s_dateTimeStandardFormat);
		TrimDateTimeOffset(destination.Slice(0, bytesWritten), out bytesWritten);
		destination.Slice(0, bytesWritten).CopyTo(buffer);
	}

	public static void TrimDateTimeOffset(Span<byte> buffer, out int bytesWritten)
	{
		if (buffer[26] != 48)
		{
			bytesWritten = buffer.Length;
			return;
		}
		int num = ((buffer[25] != 48) ? 26 : ((buffer[24] != 48) ? 25 : ((buffer[23] != 48) ? 24 : ((buffer[22] != 48) ? 23 : ((buffer[21] != 48) ? 22 : ((buffer[20] != 48) ? 21 : 19))))));
		if (buffer.Length == 27)
		{
			bytesWritten = num;
		}
		else if (buffer.Length == 33)
		{
			buffer[num] = buffer[27];
			buffer[num + 1] = buffer[28];
			buffer[num + 2] = buffer[29];
			buffer[num + 3] = buffer[30];
			buffer[num + 4] = buffer[31];
			buffer[num + 5] = buffer[32];
			bytesWritten = num + 6;
		}
		else
		{
			buffer[num] = 90;
			bytesWritten = num + 1;
		}
	}

	private static bool NeedsEscaping(byte value)
	{
		return AllowList[value] == 0;
	}

	private static bool NeedsEscapingNoBoundsCheck(char value)
	{
		return AllowList[value] == 0;
	}

	public static int NeedsEscaping(ReadOnlySpan<byte> value, JavaScriptEncoder encoder)
	{
		return (encoder ?? JavaScriptEncoder.Default).FindFirstCharacterToEncodeUtf8(value);
	}

	public unsafe static int NeedsEscaping(ReadOnlySpan<char> value, JavaScriptEncoder encoder)
	{
		if (value.IsEmpty)
		{
			return -1;
		}
		fixed (char* text = value)
		{
			return (encoder ?? JavaScriptEncoder.Default).FindFirstCharacterToEncode(text, value.Length);
		}
	}

	public static int GetMaxEscapedLength(int textLength, int firstIndexToEscape)
	{
		return firstIndexToEscape + 6 * (textLength - firstIndexToEscape);
	}

	private static void EscapeString(ReadOnlySpan<byte> value, Span<byte> destination, JavaScriptEncoder encoder, ref int consumed, ref int written, bool isFinalBlock)
	{
		OperationStatus operationStatus = encoder.EncodeUtf8(value, destination, out var bytesConsumed, out var bytesWritten, isFinalBlock);
		if (operationStatus != OperationStatus.Done && (operationStatus != OperationStatus.NeedMoreData || isFinalBlock))
		{
			ThrowHelper.ThrowArgumentException_InvalidUTF8(value.Slice(bytesWritten));
		}
		written += bytesWritten;
		consumed += bytesConsumed;
	}

	public static void EscapeString(ReadOnlySpan<byte> value, Span<byte> destination, int indexOfFirstByteToEscape, JavaScriptEncoder encoder, out int written)
	{
		EscapeString(value, destination, indexOfFirstByteToEscape, encoder, out var _, out written);
	}

	public static void EscapeString(ReadOnlySpan<byte> value, Span<byte> destination, int indexOfFirstByteToEscape, JavaScriptEncoder encoder, out int consumed, out int written, bool isFinalBlock = true)
	{
		value.Slice(0, indexOfFirstByteToEscape).CopyTo(destination);
		written = indexOfFirstByteToEscape;
		consumed = indexOfFirstByteToEscape;
		if (encoder != null)
		{
			destination = destination.Slice(indexOfFirstByteToEscape);
			value = value.Slice(indexOfFirstByteToEscape);
			EscapeString(value, destination, encoder, ref consumed, ref written, isFinalBlock);
			return;
		}
		while (indexOfFirstByteToEscape < value.Length)
		{
			byte b = value[indexOfFirstByteToEscape];
			if (IsAsciiValue(b))
			{
				if (NeedsEscaping(b))
				{
					EscapeNextBytes(b, destination, ref written);
					indexOfFirstByteToEscape++;
					consumed++;
				}
				else
				{
					destination[written] = b;
					written++;
					indexOfFirstByteToEscape++;
					consumed++;
				}
				continue;
			}
			destination = destination.Slice(written);
			value = value.Slice(indexOfFirstByteToEscape);
			EscapeString(value, destination, JavaScriptEncoder.Default, ref consumed, ref written, isFinalBlock);
			break;
		}
	}

	private static void EscapeNextBytes(byte value, Span<byte> destination, ref int written)
	{
		destination[written++] = 92;
		switch (value)
		{
		case 34:
			destination[written++] = 117;
			destination[written++] = 48;
			destination[written++] = 48;
			destination[written++] = 50;
			destination[written++] = 50;
			break;
		case 10:
			destination[written++] = 110;
			break;
		case 13:
			destination[written++] = 114;
			break;
		case 9:
			destination[written++] = 116;
			break;
		case 92:
			destination[written++] = 92;
			break;
		case 8:
			destination[written++] = 98;
			break;
		case 12:
			destination[written++] = 102;
			break;
		default:
		{
			destination[written++] = 117;
			Utf8Formatter.TryFormat(value, destination.Slice(written), out var bytesWritten, s_hexStandardFormat);
			written += bytesWritten;
			break;
		}
		}
	}

	private static bool IsAsciiValue(byte value)
	{
		return value <= 127;
	}

	private static bool IsAsciiValue(char value)
	{
		return value <= '\u007f';
	}

	private static void EscapeString(ReadOnlySpan<char> value, Span<char> destination, JavaScriptEncoder encoder, ref int consumed, ref int written, bool isFinalBlock)
	{
		OperationStatus operationStatus = encoder.Encode(value, destination, out var charsConsumed, out var charsWritten, isFinalBlock);
		if (operationStatus != OperationStatus.Done && (operationStatus != OperationStatus.NeedMoreData || isFinalBlock))
		{
			ThrowHelper.ThrowArgumentException_InvalidUTF16(value[charsWritten]);
		}
		written += charsWritten;
		consumed += charsConsumed;
	}

	public static void EscapeString(ReadOnlySpan<char> value, Span<char> destination, int indexOfFirstByteToEscape, JavaScriptEncoder encoder, out int written)
	{
		EscapeString(value, destination, indexOfFirstByteToEscape, encoder, out var _, out written);
	}

	public static void EscapeString(ReadOnlySpan<char> value, Span<char> destination, int indexOfFirstByteToEscape, JavaScriptEncoder encoder, out int consumed, out int written, bool isFinalBlock = true)
	{
		value.Slice(0, indexOfFirstByteToEscape).CopyTo(destination);
		written = indexOfFirstByteToEscape;
		consumed = indexOfFirstByteToEscape;
		if (encoder != null)
		{
			destination = destination.Slice(indexOfFirstByteToEscape);
			value = value.Slice(indexOfFirstByteToEscape);
			EscapeString(value, destination, encoder, ref consumed, ref written, isFinalBlock);
			return;
		}
		while (indexOfFirstByteToEscape < value.Length)
		{
			char c = value[indexOfFirstByteToEscape];
			if (IsAsciiValue(c))
			{
				if (NeedsEscapingNoBoundsCheck(c))
				{
					EscapeNextChars(c, destination, ref written);
					indexOfFirstByteToEscape++;
					consumed++;
				}
				else
				{
					destination[written] = c;
					written++;
					indexOfFirstByteToEscape++;
					consumed++;
				}
				continue;
			}
			destination = destination.Slice(written);
			value = value.Slice(indexOfFirstByteToEscape);
			EscapeString(value, destination, JavaScriptEncoder.Default, ref consumed, ref written, isFinalBlock);
			break;
		}
	}

	private static void EscapeNextChars(char value, Span<char> destination, ref int written)
	{
		destination[written++] = '\\';
		switch ((byte)value)
		{
		case 34:
			destination[written++] = 'u';
			destination[written++] = '0';
			destination[written++] = '0';
			destination[written++] = '2';
			destination[written++] = '2';
			break;
		case 10:
			destination[written++] = 'n';
			break;
		case 13:
			destination[written++] = 'r';
			break;
		case 9:
			destination[written++] = 't';
			break;
		case 92:
			destination[written++] = '\\';
			break;
		case 8:
			destination[written++] = 'b';
			break;
		case 12:
			destination[written++] = 'f';
			break;
		default:
		{
			destination[written++] = 'u';
			int num = value;
			num.TryFormat(destination.Slice(written), out var charsWritten, "X4".AsSpan());
			written += charsWritten;
			break;
		}
		}
	}
}
