using System.Diagnostics.CodeAnalysis;

namespace System.Text.Json.Nodes;

internal sealed class JsonValueOfJsonString : JsonValue
{
	private readonly ReadOnlyMemory<byte> _value;

	internal JsonValueOfJsonString(ReadOnlyMemory<byte> utf8String, JsonNodeOptions? options)
		: base(options)
	{
		_value = utf8String;
	}

	internal override JsonNode DeepCloneCore()
	{
		return new JsonValueOfJsonString(_value, base.Options);
	}

	private protected override JsonValueKind GetValueKindCore()
	{
		return JsonValueKind.String;
	}

	public override void WriteTo(Utf8JsonWriter writer, JsonSerializerOptions options = null)
	{
		ArgumentNullException.ThrowIfNull(writer, "writer");
		writer.WriteStringValue(_value.Span);
	}

	public override T GetValue<T>()
	{
		if (!TryGetValue<T>(out var value))
		{
			ThrowHelper.ThrowInvalidOperationException_NodeUnableToConvertElement(JsonValueKind.String, typeof(T));
		}
		return value;
	}

	public override bool TryGetValue<T>([NotNullWhen(true)] out T value)
	{
		if (typeof(T) == typeof(JsonElement))
		{
			value = (T)(object)JsonWriterHelper.WriteString(_value.Span, (ReadOnlySpan<byte> serialized) => JsonElement.Parse(serialized));
			return true;
		}
		if (typeof(T) == typeof(string))
		{
			string text = JsonReaderHelper.TranscodeHelper(_value.Span);
			value = (T)(object)text;
			return true;
		}
		if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?))
		{
			bool result = JsonReaderHelper.TryGetValue(_value.Span, false, out DateTime value2);
			value = (T)(object)value2;
			return result;
		}
		if (typeof(T) == typeof(DateTimeOffset) || typeof(T) == typeof(DateTimeOffset?))
		{
			bool result2 = JsonReaderHelper.TryGetValue(_value.Span, false, out DateTimeOffset value3);
			value = (T)(object)value3;
			return result2;
		}
		if (typeof(T) == typeof(Guid) || typeof(T) == typeof(Guid?))
		{
			bool result3 = JsonReaderHelper.TryGetValue(_value.Span, false, out Guid value4);
			value = (T)(object)value4;
			return result3;
		}
		if (typeof(T) == typeof(char) || typeof(T) == typeof(char?))
		{
			string text2 = JsonReaderHelper.TranscodeHelper(_value.Span);
			if (text2.Length == 1)
			{
				value = (T)(object)text2[0];
				return true;
			}
		}
		value = default(T);
		return false;
	}
}
