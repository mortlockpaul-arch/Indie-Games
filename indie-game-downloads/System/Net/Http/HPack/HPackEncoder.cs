using System.Buffers;
using System.Text;

namespace System.Net.Http.HPack;

internal static class HPackEncoder
{
	public static bool EncodeIndexedHeaderField(int index, Span<byte> destination, out int bytesWritten)
	{
		if (destination.Length != 0)
		{
			destination[0] = 128;
			return IntegerEncoder.Encode(index, 7, destination, out bytesWritten);
		}
		bytesWritten = 0;
		return false;
	}

	public static bool EncodeLiteralHeaderFieldWithoutIndexing(int index, string value, Encoding valueEncoding, Span<byte> destination, out int bytesWritten)
	{
		if ((uint)destination.Length >= 2u)
		{
			destination[0] = 0;
			if (IntegerEncoder.Encode(index, 4, destination, out var bytesWritten2) && EncodeStringLiteral(value, valueEncoding, destination.Slice(bytesWritten2), out var bytesWritten3))
			{
				bytesWritten = bytesWritten2 + bytesWritten3;
				return true;
			}
		}
		bytesWritten = 0;
		return false;
	}

	public static bool EncodeLiteralHeaderFieldWithoutIndexing(int index, Span<byte> destination, out int bytesWritten)
	{
		if (destination.Length != 0)
		{
			destination[0] = 0;
			if (IntegerEncoder.Encode(index, 4, destination, out var bytesWritten2))
			{
				bytesWritten = bytesWritten2;
				return true;
			}
		}
		bytesWritten = 0;
		return false;
	}

	public static bool EncodeLiteralHeaderFieldWithoutIndexingNewName(string name, ReadOnlySpan<string> values, byte[] separator, Encoding valueEncoding, Span<byte> destination, out int bytesWritten)
	{
		if ((uint)destination.Length >= 3u)
		{
			destination[0] = 0;
			if (EncodeLiteralHeaderName(name, destination.Slice(1), out var bytesWritten2) && EncodeStringLiterals(values, separator, valueEncoding, destination.Slice(1 + bytesWritten2), out var bytesWritten3))
			{
				bytesWritten = 1 + bytesWritten2 + bytesWritten3;
				return true;
			}
		}
		bytesWritten = 0;
		return false;
	}

	public static bool EncodeLiteralHeaderFieldWithoutIndexingNewName(string name, Span<byte> destination, out int bytesWritten)
	{
		if ((uint)destination.Length >= 2u)
		{
			destination[0] = 0;
			if (EncodeLiteralHeaderName(name, destination.Slice(1), out var bytesWritten2))
			{
				bytesWritten = 1 + bytesWritten2;
				return true;
			}
		}
		bytesWritten = 0;
		return false;
	}

	private static bool EncodeLiteralHeaderName(string value, Span<byte> destination, out int bytesWritten)
	{
		if (destination.Length != 0)
		{
			destination[0] = 0;
			if (IntegerEncoder.Encode(value.Length, 7, destination, out var bytesWritten2))
			{
				destination = destination.Slice(bytesWritten2);
				if (value.Length <= destination.Length)
				{
					Ascii.ToLower(value.AsSpan(), destination, out var _);
					bytesWritten = bytesWritten2 + value.Length;
					return true;
				}
			}
		}
		bytesWritten = 0;
		return false;
	}

	private static void EncodeValueStringPart(string value, Span<byte> destination)
	{
		if (Ascii.FromUtf16(value.AsSpan(), destination, out var _) == OperationStatus.InvalidData)
		{
			throw new HttpRequestException(System.SR.net_http_request_invalid_char_encoding);
		}
	}

	public static bool EncodeStringLiteral(string value, Encoding valueEncoding, Span<byte> destination, out int bytesWritten)
	{
		if (destination.Length != 0)
		{
			destination[0] = 0;
			int num = ((valueEncoding == null || valueEncoding == Encoding.Latin1) ? value.Length : valueEncoding.GetByteCount(value));
			if (IntegerEncoder.Encode(num, 7, destination, out var bytesWritten2))
			{
				destination = destination.Slice(bytesWritten2);
				if (num <= destination.Length)
				{
					if (valueEncoding == null)
					{
						EncodeValueStringPart(value, destination);
					}
					else
					{
						valueEncoding.GetBytes(value.AsSpan(), destination);
					}
					bytesWritten = bytesWritten2 + num;
					return true;
				}
			}
		}
		bytesWritten = 0;
		return false;
	}

	public static bool EncodeStringLiterals(ReadOnlySpan<string> values, byte[] separator, Encoding valueEncoding, Span<byte> destination, out int bytesWritten)
	{
		bytesWritten = 0;
		if (values.Length == 0)
		{
			return EncodeStringLiteral("", null, destination, out bytesWritten);
		}
		if (values.Length == 1)
		{
			return EncodeStringLiteral(values[0], valueEncoding, destination, out bytesWritten);
		}
		if (destination.Length != 0)
		{
			int num = checked((values.Length - 1) * separator.Length);
			if (valueEncoding == null || valueEncoding == Encoding.Latin1)
			{
				ReadOnlySpan<string> readOnlySpan = values;
				for (int i = 0; i < readOnlySpan.Length; i++)
				{
					string text = readOnlySpan[i];
					num = checked(num + text.Length);
				}
			}
			else
			{
				ReadOnlySpan<string> readOnlySpan2 = values;
				for (int i = 0; i < readOnlySpan2.Length; i++)
				{
					string s = readOnlySpan2[i];
					num = checked(num + valueEncoding.GetByteCount(s));
				}
			}
			destination[0] = 0;
			if (IntegerEncoder.Encode(num, 7, destination, out var bytesWritten2))
			{
				destination = destination.Slice(bytesWritten2);
				if (destination.Length >= num)
				{
					if (valueEncoding == null)
					{
						string text2 = values[0];
						EncodeValueStringPart(text2, destination);
						destination = destination.Slice(text2.Length);
						for (int j = 1; j < values.Length; j++)
						{
							separator.CopyTo(destination);
							destination = destination.Slice(separator.Length);
							text2 = values[j];
							EncodeValueStringPart(text2, destination);
							destination = destination.Slice(text2.Length);
						}
					}
					else
					{
						int bytes = valueEncoding.GetBytes(values[0].AsSpan(), destination);
						destination = destination.Slice(bytes);
						for (int k = 1; k < values.Length; k++)
						{
							separator.CopyTo(destination);
							destination = destination.Slice(separator.Length);
							bytes = valueEncoding.GetBytes(values[k].AsSpan(), destination);
							destination = destination.Slice(bytes);
						}
					}
					bytesWritten = bytesWritten2 + num;
					return true;
				}
			}
		}
		return false;
	}

	public static byte[] EncodeLiteralHeaderFieldWithoutIndexingToAllocatedArray(int index)
	{
		Span<byte> destination = stackalloc byte[256];
		EncodeLiteralHeaderFieldWithoutIndexing(index, destination, out var bytesWritten);
		return destination.Slice(0, bytesWritten).ToArray();
	}

	public static byte[] EncodeLiteralHeaderFieldWithoutIndexingNewNameToAllocatedArray(string name)
	{
		Span<byte> destination = stackalloc byte[256];
		EncodeLiteralHeaderFieldWithoutIndexingNewName(name, destination, out var bytesWritten);
		return destination.Slice(0, bytesWritten).ToArray();
	}

	public static byte[] EncodeLiteralHeaderFieldWithoutIndexingToAllocatedArray(int index, string value)
	{
		Span<byte> destination = stackalloc byte[512];
		int bytesWritten;
		while (!EncodeLiteralHeaderFieldWithoutIndexing(index, value, null, destination, out bytesWritten))
		{
			destination = new byte[destination.Length * 2];
		}
		return destination.Slice(0, bytesWritten).ToArray();
	}
}
