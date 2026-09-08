using System.Buffers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Compression;

internal static class ZipHelper
{
	private static readonly DateTime s_invalidDateIndicator = new DateTime(1980, 1, 1, 0, 0, 0);

	internal static Encoding GetEncoding(string text)
	{
		if (text.AsSpan().ContainsAnyExceptInRange(' ', '~'))
		{
			return Encoding.UTF8;
		}
		return Encoding.ASCII;
	}

	internal static DateTime DosTimeToDateTime(uint dateTime)
	{
		if (dateTime == 0)
		{
			return s_invalidDateIndicator;
		}
		int year = (int)(1980 + (dateTime >> 25));
		int month = (int)((dateTime >> 21) & 0xF);
		int day = (int)((dateTime >> 16) & 0x1F);
		int hour = (int)((dateTime >> 11) & 0x1F);
		int minute = (int)((dateTime >> 5) & 0x3F);
		int second = (int)((dateTime & 0x1F) * 2);
		try
		{
			return new DateTime(year, month, day, hour, minute, second, 0);
		}
		catch (ArgumentOutOfRangeException)
		{
			return s_invalidDateIndicator;
		}
		catch (ArgumentException)
		{
			return s_invalidDateIndicator;
		}
	}

	internal static uint DateTimeToDosTime(DateTime dateTime)
	{
		return (uint)((((((((dateTime.Year - 1980) & 0x7F) << 4) + dateTime.Month << 5) + dateTime.Day << 5) + dateTime.Hour << 6) + dateTime.Minute << 5) + dateTime.Second / 2);
	}

	internal static bool SeekBackwardsToSignature(Stream stream, ReadOnlySpan<byte> signatureToFind, int maxBytesToRead)
	{
		int num = 0;
		byte[] array = ArrayPool<byte>.Shared.Rent(4096);
		Span<byte> span = array.AsSpan(0, 4096);
		try
		{
			bool flag = false;
			bool flag2 = false;
			int num2 = 0;
			while (!flag2 && !flag && num2 < maxBytesToRead)
			{
				int num3 = ((num2 != 0) ? signatureToFind.Length : 0);
				if (maxBytesToRead - num2 + num3 < span.Length)
				{
					span = span.Slice(0, maxBytesToRead - num2 + num3);
				}
				int num4 = SeekBackwardsAndRead(stream, span, num3);
				flag = num4 < span.Length;
				if (num4 < span.Length)
				{
					span = span.Slice(0, num4);
				}
				num = ((ReadOnlySpan<byte>)span).LastIndexOf(signatureToFind);
				num2 += num4 - num3;
				if (num != -1)
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				return false;
			}
			stream.Seek(num, SeekOrigin.Current);
			return true;
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(array);
		}
	}

	private static int SeekBackwardsAndRead(Stream stream, Span<byte> buffer, int overlap)
	{
		int result;
		if (stream.Position >= buffer.Length)
		{
			stream.Seek(-(buffer.Length - overlap), SeekOrigin.Current);
			result = stream.ReadAtLeast(buffer, buffer.Length);
			stream.Seek(-buffer.Length, SeekOrigin.Current);
		}
		else
		{
			int minimumBytes = (int)stream.Position;
			stream.Seek(0L, SeekOrigin.Begin);
			result = stream.ReadAtLeast(buffer, minimumBytes);
			stream.Seek(0L, SeekOrigin.Begin);
		}
		return result;
	}

	internal static byte[] GetEncodedTruncatedBytesFromString(string text, Encoding encoding, int maxBytes, out bool isUTF8)
	{
		if (string.IsNullOrEmpty(text))
		{
			isUTF8 = false;
			return Array.Empty<byte>();
		}
		if (encoding == null)
		{
			encoding = GetEncoding(text);
		}
		isUTF8 = encoding.CodePage == 65001;
		if (maxBytes == 0)
		{
			return encoding.GetBytes(text);
		}
		byte[] bytes;
		if (isUTF8 && encoding.GetMaxByteCount(text.Length) > maxBytes)
		{
			int num = 0;
			foreach (Rune item in text.EnumerateRunes())
			{
				if (num + item.Utf8SequenceLength > maxBytes)
				{
					break;
				}
				num += item.Utf8SequenceLength;
			}
			bytes = encoding.GetBytes(text);
			return bytes[0..num];
		}
		bytes = encoding.GetBytes(text);
		if (maxBytes >= bytes.Length)
		{
			return bytes;
		}
		return bytes[0..maxBytes];
	}

	internal static async Task<bool> SeekBackwardsToSignatureAsync(Stream stream, ReadOnlyMemory<byte> signatureToFind, int maxBytesToRead, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		int num = 0;
		byte[] buffer = ArrayPool<byte>.Shared.Rent(4096);
		Memory<byte> bufferMemory = buffer.AsMemory(0, 4096);
		try
		{
			bool flag = false;
			bool signatureFound = false;
			int totalBytesRead = 0;
			while (!signatureFound && !flag && totalBytesRead < maxBytesToRead)
			{
				int overlap = ((totalBytesRead != 0) ? signatureToFind.Length : 0);
				if (maxBytesToRead - totalBytesRead + overlap < bufferMemory.Length)
				{
					bufferMemory = bufferMemory.Slice(0, maxBytesToRead - totalBytesRead + overlap);
				}
				int num2 = await SeekBackwardsAndReadAsync(stream, bufferMemory, overlap, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				flag = num2 < bufferMemory.Length;
				if (num2 < bufferMemory.Length)
				{
					bufferMemory = bufferMemory.Slice(0, num2);
				}
				num = ((ReadOnlySpan<byte>)bufferMemory.Span).LastIndexOf(signatureToFind.Span);
				totalBytesRead += num2 - overlap;
				if (num != -1)
				{
					signatureFound = true;
					break;
				}
			}
			if (!signatureFound)
			{
				return false;
			}
			stream.Seek(num, SeekOrigin.Current);
			return true;
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(buffer);
		}
	}

	private static async Task<int> SeekBackwardsAndReadAsync(Stream stream, Memory<byte> buffer, int overlap, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		int result;
		if (stream.Position >= buffer.Length)
		{
			stream.Seek(-(buffer.Length - overlap), SeekOrigin.Current);
			result = await stream.ReadAtLeastAsync(buffer, buffer.Length, throwOnEndOfStream: true, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			stream.Seek(-buffer.Length, SeekOrigin.Current);
		}
		else
		{
			int minimumBytes = (int)stream.Position;
			stream.Seek(0L, SeekOrigin.Begin);
			result = await stream.ReadAtLeastAsync(buffer, minimumBytes, throwOnEndOfStream: true, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			stream.Seek(0L, SeekOrigin.Begin);
		}
		return result;
	}
}
