using System.Formats.Asn1;
using System.Numerics;

namespace System.Security.Cryptography;

internal static class KeyBlobHelpers
{
	internal static byte[] ToUnsignedIntegerBytes(this ReadOnlyMemory<byte> memory)
	{
		if (memory.Length > 1 && memory.Span[0] == 0)
		{
			return memory.Slice(1).ToArray();
		}
		return memory.ToArray();
	}

	internal static void ToUnsignedIntegerBytes(this ReadOnlyMemory<byte> memory, Span<byte> destination)
	{
		int length = destination.Length;
		if (memory.Length == length)
		{
			memory.Span.CopyTo(destination);
			return;
		}
		if (memory.Length == length + 1 && memory.Span[0] == 0)
		{
			memory.Span.Slice(1).CopyTo(destination);
			return;
		}
		if (memory.Length > length)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
		}
		destination.Slice(0, destination.Length - memory.Length).Clear();
		memory.Span.CopyTo(destination.Slice(length - memory.Length));
	}

	internal static void WriteKeyParameterInteger(this AsnWriter writer, ReadOnlySpan<byte> integer)
	{
		if (integer[0] == 0)
		{
			int i;
			for (i = 1; i < integer.Length; i++)
			{
				if (integer[i] >= 128)
				{
					i--;
					break;
				}
				if (integer[i] != 0)
				{
					break;
				}
			}
			if (i == integer.Length)
			{
				i--;
			}
			integer = integer.Slice(i);
		}
		writer.WriteIntegerUnsigned(integer);
	}

	internal static byte[] ToUnsignedIntegerBytes(this ReadOnlyMemory<byte> memory, int length)
	{
		if (memory.Length == length)
		{
			return memory.ToArray();
		}
		ReadOnlySpan<byte> span = memory.Span;
		if (memory.Length == length + 1 && span[0] == 0)
		{
			return span.Slice(1).ToArray();
		}
		if (span.Length > length)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
		}
		byte[] array = new byte[length];
		span.CopyTo(array.AsSpan(length - span.Length));
		return array;
	}

	internal static byte[] ExportKeyParameter(this BigInteger value, int length)
	{
		byte[] array = new byte[length];
		if (value.TryWriteBytes(array, out var bytesWritten, isUnsigned: true, isBigEndian: true))
		{
			if (bytesWritten < length)
			{
				Buffer.BlockCopy(array, 0, array, length - bytesWritten, bytesWritten);
				array.AsSpan(0, length - bytesWritten).Clear();
			}
			return array;
		}
		throw new CryptographicException(System.SR.Cryptography_NotValidPublicOrPrivateKey);
	}
}
