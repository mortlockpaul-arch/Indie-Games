using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace System.Web.Util;

internal static class HttpEncoder
{
	private ref struct UrlDecoder
	{
		private int _numChars;

		private readonly Span<char> _charBuffer;

		private int _numBytes;

		private readonly Span<byte> _byteBuffer;

		private readonly Encoding _encoding;

		private void FlushBytes()
		{
			if (_numBytes > 0)
			{
				_numChars += _encoding.GetChars(_byteBuffer.Slice(0, _numBytes), _charBuffer.Slice(_numChars));
				_numBytes = 0;
			}
		}

		internal UrlDecoder(Span<char> charBuffer, Span<byte> byteBuffer, Encoding encoding)
		{
			_numChars = 0;
			_numBytes = 0;
			_charBuffer = charBuffer;
			_byteBuffer = byteBuffer;
			_encoding = encoding;
		}

		internal void AddChar(char ch)
		{
			if (_numBytes > 0)
			{
				FlushBytes();
			}
			_charBuffer[_numChars++] = ch;
		}

		internal void AddByte(byte b)
		{
			_byteBuffer[_numBytes++] = b;
		}

		internal string GetString()
		{
			if (_numBytes > 0)
			{
				FlushBytes();
			}
			Span<char> span = _charBuffer.Slice(0, _numChars);
			for (int i = ((ReadOnlySpan<char>)span).IndexOfAnyInRange('\ud800', '\udfff'); (uint)i < (uint)span.Length; i++)
			{
				if (char.IsHighSurrogate(span[i]))
				{
					if ((uint)(i + 1) >= (uint)span.Length || !char.IsLowSurrogate(span[i + 1]))
					{
						span[i] = (char)Rune.ReplacementChar.Value;
					}
					else
					{
						i++;
					}
				}
				else if (char.IsLowSurrogate(span[i]))
				{
					span[i] = (char)Rune.ReplacementChar.Value;
				}
			}
			return span.ToString();
		}
	}

	private static readonly SearchValues<byte> s_urlSafeBytes = SearchValues.Create("!()*-.0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz"u8);

	private static readonly SearchValues<char> s_invalidJavaScriptChars = SearchValues.Create("\0\u0001\u0002\u0003\u0004\u0005\u0006\a\b\t\n\v\f\r\u000e\u000f\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001a\u001b\u001c\u001d\u001e\u001f\"&'<>\\\u0085\u2028\u2029".AsSpan());

	[return: NotNullIfNotNull("value")]
	internal static string HtmlAttributeEncode(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return value;
		}
		int num = IndexOfHtmlAttributeEncodingChars(value);
		if (num < 0)
		{
			return value;
		}
		StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
		HtmlAttributeEncodeInternal(value, num, stringWriter);
		return stringWriter.ToString();
	}

	internal static void HtmlAttributeEncode(string value, TextWriter output)
	{
		if (value != null)
		{
			ArgumentNullException.ThrowIfNull(output, "output");
			int num = IndexOfHtmlAttributeEncodingChars(value);
			if (num < 0)
			{
				output.Write(value);
			}
			else
			{
				HtmlAttributeEncodeInternal(value, num, output);
			}
		}
	}

	private static void HtmlAttributeEncodeInternal(string s, int index, TextWriter output)
	{
		output.Write(s.AsSpan(0, index));
		ReadOnlySpan<char> readOnlySpan = s.AsSpan(index);
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			char c = readOnlySpan[i];
			if (c <= '<')
			{
				switch (c)
				{
				case '<':
					output.Write("&lt;");
					break;
				case '"':
					output.Write("&quot;");
					break;
				case '\'':
					output.Write("&#39;");
					break;
				case '&':
					output.Write("&amp;");
					break;
				default:
					output.Write(c);
					break;
				}
			}
			else
			{
				output.Write(c);
			}
		}
	}

	[return: NotNullIfNotNull("value")]
	internal static string HtmlDecode(string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			return WebUtility.HtmlDecode(value);
		}
		return value;
	}

	internal static void HtmlDecode(string value, TextWriter output)
	{
		ArgumentNullException.ThrowIfNull(output, "output");
		output.Write(WebUtility.HtmlDecode(value));
	}

	[return: NotNullIfNotNull("value")]
	internal static string HtmlEncode(string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			return WebUtility.HtmlEncode(value);
		}
		return value;
	}

	internal static void HtmlEncode(string value, TextWriter output)
	{
		ArgumentNullException.ThrowIfNull(output, "output");
		output.Write(WebUtility.HtmlEncode(value));
	}

	private static int IndexOfHtmlAttributeEncodingChars(string s)
	{
		return s.AsSpan().IndexOfAny("<\"'&".AsSpan());
	}

	internal static string JavaScriptStringEncode(string value, bool addDoubleQuotes)
	{
		int num = value.AsSpan().IndexOfAny(s_invalidJavaScriptChars);
		if (num < 0)
		{
			string text;
			if (!addDoubleQuotes)
			{
				text = value;
				if (text == null)
				{
					return string.Empty;
				}
			}
			else
			{
				text = "\"" + value + "\"";
			}
			return text;
		}
		return EncodeCore(value.AsSpan(), num, addDoubleQuotes);
		static string EncodeCore(ReadOnlySpan<char> readOnlySpan2, int i, bool flag)
		{
			Span<char> initialBuffer = stackalloc char[512];
			System.Text.ValueStringBuilder valueStringBuilder = new System.Text.ValueStringBuilder(initialBuffer);
			if (flag)
			{
				valueStringBuilder.Append('"');
			}
			ReadOnlySpan<char> readOnlySpan = readOnlySpan2;
			do
			{
				valueStringBuilder.Append(readOnlySpan.Slice(0, i));
				char c = readOnlySpan[i];
				readOnlySpan = readOnlySpan.Slice(i + 1);
				switch (c)
				{
				case '\r':
					valueStringBuilder.Append("\\r");
					break;
				case '\t':
					valueStringBuilder.Append("\\t");
					break;
				case '"':
					valueStringBuilder.Append("\\\"");
					break;
				case '\\':
					valueStringBuilder.Append("\\\\");
					break;
				case '\n':
					valueStringBuilder.Append("\\n");
					break;
				case '\b':
					valueStringBuilder.Append("\\b");
					break;
				case '\f':
					valueStringBuilder.Append("\\f");
					break;
				default:
					valueStringBuilder.Append("\\u");
					valueStringBuilder.AppendSpanFormattable((int)c, "x4", (IFormatProvider)null);
					break;
				}
				i = readOnlySpan.IndexOfAny(s_invalidJavaScriptChars);
			}
			while (i >= 0);
			valueStringBuilder.Append(readOnlySpan);
			if (flag)
			{
				valueStringBuilder.Append('"');
			}
			return valueStringBuilder.ToString();
		}
	}

	[return: NotNullIfNotNull("bytes")]
	internal static byte[] UrlDecode(byte[] bytes, int offset, int count)
	{
		if (!ValidateUrlEncodingParameters(bytes, offset, count))
		{
			return null;
		}
		return UrlDecode(bytes.AsSpan(offset, count));
	}

	internal static byte[] UrlDecode(ReadOnlySpan<byte> bytes)
	{
		int length = 0;
		int length2 = bytes.Length;
		Span<byte> span = ((length2 > 512) ? ((Span<byte>)new byte[length2]) : stackalloc byte[512]);
		Span<byte> span2 = span;
		for (int i = 0; i < length2; i++)
		{
			byte b = bytes[i];
			switch (b)
			{
			case 43:
				b = 32;
				break;
			case 37:
				if (i < length2 - 2)
				{
					int num = System.HexConverter.FromChar(bytes[i + 1]);
					int num2 = System.HexConverter.FromChar(bytes[i + 2]);
					if ((num | num2) != 255)
					{
						b = (byte)((num << 4) | num2);
						i += 2;
					}
				}
				break;
			}
			span2[length++] = b;
		}
		return span2.Slice(0, length).ToArray();
	}

	[return: NotNullIfNotNull("bytes")]
	internal static string UrlDecode(byte[] bytes, int offset, int count, Encoding encoding)
	{
		if (!ValidateUrlEncodingParameters(bytes, offset, count))
		{
			return null;
		}
		UrlDecoder urlDecoder;
		if (count <= 256)
		{
			Span<char> charBuffer = stackalloc char[256];
			Span<byte> byteBuffer = stackalloc byte[256];
			urlDecoder = new UrlDecoder(charBuffer, byteBuffer, encoding);
		}
		else
		{
			urlDecoder = new UrlDecoder(new char[count], new byte[count], encoding);
		}
		UrlDecoder urlDecoder2 = urlDecoder;
		for (int i = 0; i < count; i++)
		{
			int num = offset + i;
			byte b = bytes[num];
			switch (b)
			{
			case 43:
				b = 32;
				break;
			case 37:
				if (i >= count - 2)
				{
					break;
				}
				if (bytes[num + 1] == 117 && i < count - 5)
				{
					int num2 = System.HexConverter.FromChar(bytes[num + 2]);
					int num3 = System.HexConverter.FromChar(bytes[num + 3]);
					int num4 = System.HexConverter.FromChar(bytes[num + 4]);
					int num5 = System.HexConverter.FromChar(bytes[num + 5]);
					if ((num2 | num3 | num4 | num5) != 255)
					{
						char ch = (char)((num2 << 12) | (num3 << 8) | (num4 << 4) | num5);
						i += 5;
						urlDecoder2.AddChar(ch);
						continue;
					}
				}
				else
				{
					int num6 = System.HexConverter.FromChar(bytes[num + 1]);
					int num7 = System.HexConverter.FromChar(bytes[num + 2]);
					if ((num6 | num7) != 255)
					{
						b = (byte)((num6 << 4) | num7);
						i += 2;
					}
				}
				break;
			}
			urlDecoder2.AddByte(b);
		}
		return urlDecoder2.GetString();
	}

	[return: NotNullIfNotNull("value")]
	internal static string UrlDecode(string value, Encoding encoding)
	{
		if (value == null)
		{
			return null;
		}
		return UrlDecode(value.AsSpan(), encoding);
	}

	internal static string UrlDecode(ReadOnlySpan<char> value, Encoding encoding)
	{
		if (value.IsEmpty)
		{
			return string.Empty;
		}
		int length = value.Length;
		UrlDecoder urlDecoder;
		if (length <= 256)
		{
			Span<char> charBuffer = stackalloc char[256];
			Span<byte> byteBuffer = stackalloc byte[256];
			urlDecoder = new UrlDecoder(charBuffer, byteBuffer, encoding);
		}
		else
		{
			urlDecoder = new UrlDecoder(new char[length], new byte[length], encoding);
		}
		UrlDecoder urlDecoder2 = urlDecoder;
		for (int i = 0; i < length; i++)
		{
			char c = value[i];
			switch (c)
			{
			case '+':
				c = ' ';
				break;
			case '%':
				if (i >= length - 2)
				{
					break;
				}
				if (value[i + 1] == 'u' && i < length - 5)
				{
					int num = System.HexConverter.FromChar(value[i + 2]);
					int num2 = System.HexConverter.FromChar(value[i + 3]);
					int num3 = System.HexConverter.FromChar(value[i + 4]);
					int num4 = System.HexConverter.FromChar(value[i + 5]);
					if ((num | num2 | num3 | num4) != 255)
					{
						c = (char)((num << 12) | (num2 << 8) | (num3 << 4) | num4);
						i += 5;
						urlDecoder2.AddChar(c);
						continue;
					}
				}
				else
				{
					int num5 = System.HexConverter.FromChar(value[i + 1]);
					int num6 = System.HexConverter.FromChar(value[i + 2]);
					if ((num5 | num6) != 255)
					{
						byte b = (byte)((num5 << 4) | num6);
						i += 2;
						urlDecoder2.AddByte(b);
						continue;
					}
				}
				break;
			}
			if ((c & 0xFF80) == 0)
			{
				urlDecoder2.AddByte((byte)c);
			}
			else
			{
				urlDecoder2.AddChar(c);
			}
		}
		return urlDecoder2.GetString();
	}

	[return: NotNullIfNotNull("bytes")]
	internal static byte[] UrlEncode(byte[] bytes, int offset, int count)
	{
		if (!ValidateUrlEncodingParameters(bytes, offset, count))
		{
			return null;
		}
		return UrlEncode(bytes.AsSpan(offset, count));
	}

	private static byte[] UrlEncode(ReadOnlySpan<byte> bytes)
	{
		if (!NeedsEncoding(bytes, out var cUnsafe))
		{
			return bytes.ToArray();
		}
		return UrlEncode(bytes, cUnsafe);
	}

	private static byte[] UrlEncode(ReadOnlySpan<byte> bytes, int cUnsafe)
	{
		byte[] array = new byte[bytes.Length + cUnsafe * 2];
		int num = 0;
		ReadOnlySpan<byte> readOnlySpan = bytes;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			byte b = readOnlySpan[i];
			if (s_urlSafeBytes.Contains(b))
			{
				array[num++] = b;
				continue;
			}
			if (b == 32)
			{
				array[num++] = 43;
				continue;
			}
			array[num++] = 37;
			array[num++] = (byte)System.HexConverter.ToCharLower(b >> 4);
			array[num++] = (byte)System.HexConverter.ToCharLower(b);
		}
		return array;
	}

	private static bool NeedsEncoding(ReadOnlySpan<byte> bytes, out int cUnsafe)
	{
		cUnsafe = 0;
		int num = bytes.IndexOfAnyExcept(s_urlSafeBytes);
		if (num < 0)
		{
			return false;
		}
		ReadOnlySpan<byte> readOnlySpan = bytes.Slice(num);
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			byte b = readOnlySpan[i];
			if (!s_urlSafeBytes.Contains(b) && b != 32)
			{
				cUnsafe++;
			}
		}
		return true;
	}

	internal static byte[] UrlEncode(string str, Encoding e)
	{
		if (e.GetMaxByteCount(str.Length) <= 512)
		{
			Span<byte> bytes = stackalloc byte[512];
			return UrlEncode(bytes[..e.GetBytes(str.AsSpan(), bytes)]);
		}
		byte[] bytes2 = e.GetBytes(str);
		if (!NeedsEncoding(bytes2, out var cUnsafe))
		{
			return bytes2;
		}
		return UrlEncode(bytes2, cUnsafe);
	}

	[Obsolete("This method produces non-standards-compliant output and has interoperability issues. The preferred alternative is UrlEncode(*).")]
	[return: NotNullIfNotNull("value")]
	internal static string UrlEncodeUnicode(string value)
	{
		if (value == null)
		{
			return null;
		}
		int length = value.Length;
		StringBuilder stringBuilder = new StringBuilder(length);
		for (int i = 0; i < length; i++)
		{
			char c = value[i];
			if ((c & 0xFF80) == 0)
			{
				if (s_urlSafeBytes.Contains((byte)c))
				{
					stringBuilder.Append(c);
					continue;
				}
				if (c == ' ')
				{
					stringBuilder.Append('+');
					continue;
				}
				stringBuilder.Append('%');
				stringBuilder.Append(System.HexConverter.ToCharLower((int)c >> 4));
				stringBuilder.Append(System.HexConverter.ToCharLower(c));
			}
			else
			{
				stringBuilder.Append("%u");
				stringBuilder.Append(System.HexConverter.ToCharLower((int)c >> 12));
				stringBuilder.Append(System.HexConverter.ToCharLower((int)c >> 8));
				stringBuilder.Append(System.HexConverter.ToCharLower((int)c >> 4));
				stringBuilder.Append(System.HexConverter.ToCharLower(c));
			}
		}
		return stringBuilder.ToString();
	}

	[return: NotNullIfNotNull("value")]
	internal static string UrlPathEncode(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return value;
		}
		if (!UriUtil.TrySplitUriForPathEncode(value, out var schemeAndAuthority, out var path, out var queryAndFragment))
		{
			return UrlPathEncodeImpl(value);
		}
		return string.Concat(schemeAndAuthority, UrlPathEncodeImpl(path).AsSpan(), queryAndFragment);
	}

	private static string UrlPathEncodeImpl(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return value;
		}
		int num = value.AsSpan().IndexOfAnyExceptInRange('!', '\u007f');
		if (num < 0)
		{
			return value;
		}
		int num2 = value.IndexOf('?');
		if ((uint)num2 < (uint)num)
		{
			return value;
		}
		ReadOnlySpan<char> chars = ((num2 >= 0) ? value.AsSpan(num, num2 - num) : value.AsSpan(num));
		byte[] array = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetMaxByteCount(chars.Length));
		int bytes = Encoding.UTF8.GetBytes(chars, array);
		char[] array2 = ArrayPool<char>.Shared.Rent(bytes * 3);
		int length = 0;
		Span<byte> span = array.AsSpan(0, bytes);
		for (int i = 0; i < span.Length; i++)
		{
			byte b = span[i];
			if (!char.IsBetween((char)b, '!', '\u007f'))
			{
				array2[length++] = '%';
				array2[length++] = System.HexConverter.ToCharLower(b >> 4);
				array2[length++] = System.HexConverter.ToCharLower(b);
			}
			else
			{
				array2[length++] = (char)b;
			}
		}
		ArrayPool<byte>.Shared.Return(array);
		string result = string.Concat(value.AsSpan(0, num), array2.AsSpan(0, length), (num2 >= 0) ? value.AsSpan(num2) : ReadOnlySpan<char>.Empty);
		ArrayPool<char>.Shared.Return(array2);
		return result;
	}

	private static bool ValidateUrlEncodingParameters([NotNullWhen(true)] byte[] bytes, int offset, int count)
	{
		if (bytes == null && count == 0)
		{
			return false;
		}
		ArgumentNullException.ThrowIfNull(bytes, "bytes");
		ArgumentOutOfRangeException.ThrowIfNegative(offset, "offset");
		ArgumentOutOfRangeException.ThrowIfGreaterThan(offset, bytes.Length, "offset");
		ArgumentOutOfRangeException.ThrowIfNegative(count, "count");
		ArgumentOutOfRangeException.ThrowIfGreaterThan(count, bytes.Length - offset, "count");
		return true;
	}
}
