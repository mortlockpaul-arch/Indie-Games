using System.Text.Json.Serialization.Metadata;

namespace System.Text.Json.Nodes;

internal sealed class JsonValueCustomized<TValue> : JsonValue<TValue>
{
	private readonly JsonTypeInfo<TValue> _jsonTypeInfo;

	private JsonValueKind? _valueKind;

	public JsonValueCustomized(TValue value, JsonTypeInfo<TValue> jsonTypeInfo, JsonNodeOptions? options = null)
		: base(value, options)
	{
		_jsonTypeInfo = jsonTypeInfo;
	}

	private protected override JsonValueKind GetValueKindCore()
	{
		JsonValueKind valueOrDefault = _valueKind.GetValueOrDefault();
		if (!_valueKind.HasValue)
		{
			valueOrDefault = ComputeValueKind();
			_valueKind = valueOrDefault;
			return valueOrDefault;
		}
		return valueOrDefault;
	}

	internal override JsonNode DeepCloneCore()
	{
		return JsonSerializer.SerializeToNode(Value, _jsonTypeInfo);
	}

	public override void WriteTo(Utf8JsonWriter writer, JsonSerializerOptions options = null)
	{
		ArgumentNullException.ThrowIfNull(writer, "writer");
		JsonTypeInfo<TValue> jsonTypeInfo = _jsonTypeInfo;
		if (options != null && options != jsonTypeInfo.Options)
		{
			options.MakeReadOnly();
			jsonTypeInfo = (JsonTypeInfo<TValue>)options.GetTypeInfoInternal(typeof(TValue), ensureConfigured: true, true);
		}
		jsonTypeInfo.Serialize(writer, in Value);
	}

	private JsonValueKind ComputeValueKind()
	{
		Utf8JsonWriter utf8JsonWriter = Utf8JsonWriterCache.RentWriterAndBuffer(default(JsonWriterOptions), 16384, out var bufferWriter);
		try
		{
			WriteTo(utf8JsonWriter);
			utf8JsonWriter.Flush();
			Utf8JsonReader utf8JsonReader = new Utf8JsonReader(bufferWriter.WrittenSpan);
			utf8JsonReader.Read();
			return utf8JsonReader.TokenType.ToValueKind();
		}
		finally
		{
			Utf8JsonWriterCache.ReturnWriterAndBuffer(utf8JsonWriter, bufferWriter);
		}
	}
}
