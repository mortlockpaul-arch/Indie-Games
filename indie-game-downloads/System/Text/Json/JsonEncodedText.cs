using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;

namespace System.Text.Json;

public readonly struct JsonEncodedText : IEquatable<JsonEncodedText>
{
	internal readonly byte[] _utf8Value;

	internal readonly string _value;

	public ReadOnlySpan<byte> EncodedUtf8Bytes => _utf8Value;

	public string Value => _value ?? string.Empty;

	private JsonEncodedText(byte[] utf8Value)
	{
		_value = JsonReaderHelper.GetTextFromUtf8(utf8Value);
		_utf8Value = utf8Value;
	}

	public static JsonEncodedText Encode(string value, JavaScriptEncoder? encoder = null)
	{
		ArgumentNullException.ThrowIfNull(value, "value");
		return Encode(value.AsSpan(), encoder);
	}

	public static JsonEncodedText Encode(ReadOnlySpan<char> value, JavaScriptEncoder? encoder = null)
	{
		if (value.Length == 0)
		{
			return new JsonEncodedText(Array.Empty<byte>());
		}
		return TranscodeAndEncode(value, encoder);
	}

	private static JsonEncodedText TranscodeAndEncode(ReadOnlySpan<char> value, JavaScriptEncoder encoder)
	{
		JsonWriterHelper.ValidateValue(value);
		int utf8ByteCount = JsonReaderHelper.GetUtf8ByteCount(value);
		byte[] array = null;
		Span<byte> span = ((utf8ByteCount > 256) ? ((Span<byte>)(array = ArrayPool<byte>.Shared.Rent(utf8ByteCount))) : stackalloc byte[256]);
		Span<byte> span2 = span;
		span2 = span2[..JsonReaderHelper.GetUtf8FromText(value, span2)];
		JsonEncodedText result = EncodeHelper(span2, encoder);
		if (array != null)
		{
			span2.Clear();
			ArrayPool<byte>.Shared.Return(array);
		}
		return result;
	}

	public static JsonEncodedText Encode(ReadOnlySpan<byte> utf8Value, JavaScriptEncoder? encoder = null)
	{
		if (utf8Value.Length == 0)
		{
			return new JsonEncodedText(Array.Empty<byte>());
		}
		JsonWriterHelper.ValidateValue(utf8Value);
		return EncodeHelper(utf8Value, encoder);
	}

	private static JsonEncodedText EncodeHelper(ReadOnlySpan<byte> utf8Value, JavaScriptEncoder encoder)
	{
		int num = JsonWriterHelper.NeedsEscaping(utf8Value, encoder);
		if (num != -1)
		{
			return new JsonEncodedText(JsonHelpers.EscapeValue(utf8Value, num, encoder));
		}
		return new JsonEncodedText(utf8Value.ToArray());
	}

	public bool Equals(JsonEncodedText other)
	{
		if (_value == null)
		{
			return other._value == null;
		}
		return _value.Equals(other._value);
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is JsonEncodedText other)
		{
			return Equals(other);
		}
		return false;
	}

	public override string ToString()
	{
		return _value ?? string.Empty;
	}

	public override int GetHashCode()
	{
		if (_value != null)
		{
			return _value.GetHashCode();
		}
		return 0;
	}
}
