using System.Buffers;
using System.Globalization;
using System.Text;

namespace System.Net.ServerSentEvents;

internal static class Helpers
{
	public static void WriteUtf8Number(this IBufferWriter<byte> writer, long value)
	{
		Span<byte> span = writer.GetSpan(20);
		IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
		value.TryFormat(span, out var bytesWritten, default(ReadOnlySpan<char>), invariantCulture);
		writer.Advance(bytesWritten);
	}

	public static void WriteUtf8String(this IBufferWriter<byte> writer, ReadOnlySpan<byte> value)
	{
		if (!value.IsEmpty)
		{
			Span<byte> span = writer.GetSpan(value.Length);
			value.CopyTo(span);
			writer.Advance(value.Length);
		}
	}

	public static void WriteUtf8String(this IBufferWriter<byte> writer, ReadOnlySpan<char> value)
	{
		if (!value.IsEmpty)
		{
			int maxByteCount = Encoding.UTF8.GetMaxByteCount(value.Length);
			Span<byte> span = writer.GetSpan(maxByteCount);
			int bytes = Encoding.UTF8.GetBytes(value, span);
			writer.Advance(bytes);
		}
	}

	public static bool ContainsLineBreaks(this ReadOnlySpan<char> text)
	{
		return text.IndexOfAny('\r', '\n') >= 0;
	}
}
