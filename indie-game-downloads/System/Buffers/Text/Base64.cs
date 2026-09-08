using System.Runtime.CompilerServices;

namespace System.Buffers.Text;

public static class Base64
{
	public static OperationStatus EncodeToUtf8(ReadOnlySpan<byte> bytes, Span<byte> utf8, out int bytesConsumed, out int bytesWritten, bool isFinalBlock = true)
	{
		return Base64Helper.EncodeTo(default(Base64Helper.Base64EncoderByte), bytes, utf8, out bytesConsumed, out bytesWritten, isFinalBlock);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetMaxEncodedToUtf8Length(int length)
	{
		ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)length, 1610612733u, "(uint)length");
		return (length + 2) / 3 * 4;
	}

	public static OperationStatus EncodeToUtf8InPlace(Span<byte> buffer, int dataLength, out int bytesWritten)
	{
		return Base64Helper.EncodeToUtf8InPlace(default(Base64Helper.Base64EncoderByte), buffer, dataLength, out bytesWritten);
	}

	public static OperationStatus DecodeFromUtf8(ReadOnlySpan<byte> utf8, Span<byte> bytes, out int bytesConsumed, out int bytesWritten, bool isFinalBlock = true)
	{
		return Base64Helper.DecodeFrom(default(Base64Helper.Base64DecoderByte), utf8, bytes, out bytesConsumed, out bytesWritten, isFinalBlock, ignoreWhiteSpace: true);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetMaxDecodedFromUtf8Length(int length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length, "length");
		return (length >> 2) * 3;
	}

	public static OperationStatus DecodeFromUtf8InPlace(Span<byte> buffer, out int bytesWritten)
	{
		return Base64Helper.DecodeFromUtf8InPlace(default(Base64Helper.Base64DecoderByte), buffer, out bytesWritten, ignoreWhiteSpace: true);
	}

	public static bool IsValid(ReadOnlySpan<char> base64Text)
	{
		int decodedLength;
		return Base64Helper.IsValid(default(Base64Helper.Base64CharValidatable), base64Text, out decodedLength);
	}

	public static bool IsValid(ReadOnlySpan<char> base64Text, out int decodedLength)
	{
		return Base64Helper.IsValid(default(Base64Helper.Base64CharValidatable), base64Text, out decodedLength);
	}

	public static bool IsValid(ReadOnlySpan<byte> base64TextUtf8)
	{
		int decodedLength;
		return Base64Helper.IsValid(default(Base64Helper.Base64ByteValidatable), base64TextUtf8, out decodedLength);
	}

	public static bool IsValid(ReadOnlySpan<byte> base64TextUtf8, out int decodedLength)
	{
		return Base64Helper.IsValid(default(Base64Helper.Base64ByteValidatable), base64TextUtf8, out decodedLength);
	}
}
