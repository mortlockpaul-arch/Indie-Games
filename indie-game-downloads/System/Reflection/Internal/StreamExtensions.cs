using System.IO;

namespace System.Reflection.Internal;

internal static class StreamExtensions
{
	internal unsafe static void ReadExactly(this Stream stream, byte* buffer, int size)
	{
		stream.ReadExactly(new Span<byte>(buffer, size));
	}

	internal static int TryReadAll(this Stream stream, byte[] buffer, int offset, int count)
	{
		int i;
		int num;
		for (i = 0; i < count; i += num)
		{
			num = stream.Read(buffer, offset + i, count - i);
			if (num == 0)
			{
				break;
			}
		}
		return i;
	}

	internal static int TryReadAll(this Stream stream, Span<byte> buffer)
	{
		return stream.ReadAtLeast(buffer, buffer.Length, throwOnEndOfStream: false);
	}

	internal static int GetAndValidateSize(Stream stream, int size, string streamParameterName)
	{
		long num = stream.Length - stream.Position;
		if (size < 0 || size > num)
		{
			throw new ArgumentOutOfRangeException("size");
		}
		if (size != 0)
		{
			return size;
		}
		if (num > int.MaxValue)
		{
			throw new ArgumentException(System.SR.StreamTooLarge, streamParameterName);
		}
		return (int)num;
	}
}
