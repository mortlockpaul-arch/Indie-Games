using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace System.Text.Json.Nodes;

internal sealed class JsonValuePrimitive<TValue> : JsonValue<TValue>
{
	private readonly JsonConverter<TValue> _converter;

	private readonly JsonValueKind _valueKind;

	public JsonValuePrimitive(TValue value, JsonConverter<TValue> converter, JsonNodeOptions? options)
		: base(value, options)
	{
		_converter = converter;
		_valueKind = JsonValue<TValue>.DetermineValueKind(value);
	}

	private protected override JsonValueKind GetValueKindCore()
	{
		return _valueKind;
	}

	internal override JsonNode DeepCloneCore()
	{
		return new JsonValuePrimitive<TValue>(Value, _converter, base.Options);
	}

	internal override bool DeepEqualsCore(JsonNode otherNode)
	{
		if (otherNode is JsonValue jsonValue && jsonValue.TryGetValue<TValue>(out TValue value))
		{
			return EqualityComparer<TValue>.Default.Equals(Value, value);
		}
		return base.DeepEqualsCore(otherNode);
	}

	public override void WriteTo(Utf8JsonWriter writer, JsonSerializerOptions options = null)
	{
		ArgumentNullException.ThrowIfNull(writer, "writer");
		JsonConverter<TValue> converter = _converter;
		if (options == null)
		{
			options = JsonNode.s_defaultOptions;
		}
		if (converter.IsInternalConverterForNumberType)
		{
			converter.WriteNumberWithCustomHandling(writer, Value, options.NumberHandling);
		}
		else
		{
			converter.Write(writer, Value, options);
		}
	}
}
