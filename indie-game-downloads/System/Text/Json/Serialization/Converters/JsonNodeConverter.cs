using System.Text.Json.Nodes;
using System.Text.Json.Schema;

namespace System.Text.Json.Serialization.Converters;

internal sealed class JsonNodeConverter : JsonConverter<JsonNode>
{
	internal static JsonNodeConverter Instance { get; } = new JsonNodeConverter();

	public override void Write(Utf8JsonWriter writer, JsonNode value, JsonSerializerOptions options)
	{
		if (value == null)
		{
			writer.WriteNullValue();
		}
		else
		{
			value.WriteTo(writer, options);
		}
	}

	public override JsonNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (!options.AllowDuplicateProperties)
		{
			return ReadAsJsonNode(ref reader, options.GetNodeOptions());
		}
		return ReadAsJsonElement(ref reader, options.GetNodeOptions());
	}

	internal static JsonNode ReadAsJsonElement(ref Utf8JsonReader reader, JsonNodeOptions options)
	{
		switch (reader.TokenType)
		{
		case JsonTokenType.String:
		case JsonTokenType.Number:
		case JsonTokenType.True:
		case JsonTokenType.False:
			return JsonValueConverter.ReadNonNullPrimitiveValue(ref reader, options);
		case JsonTokenType.StartObject:
			return JsonObjectConverter.ReadAsJsonElement(ref reader, options);
		case JsonTokenType.StartArray:
			return JsonArrayConverter.ReadAsJsonElement(ref reader, options);
		case JsonTokenType.Null:
			return null;
		default:
			throw new JsonException();
		}
	}

	internal static JsonNode ReadAsJsonNode(ref Utf8JsonReader reader, JsonNodeOptions options)
	{
		switch (reader.TokenType)
		{
		case JsonTokenType.String:
		case JsonTokenType.Number:
		case JsonTokenType.True:
		case JsonTokenType.False:
			return JsonValueConverter.ReadNonNullPrimitiveValue(ref reader, options);
		case JsonTokenType.StartObject:
			return JsonObjectConverter.ReadAsJsonNode(ref reader, options);
		case JsonTokenType.StartArray:
			return JsonArrayConverter.ReadAsJsonNode(ref reader, options);
		case JsonTokenType.Null:
			return null;
		default:
			throw new JsonException();
		}
	}

	public static JsonNode Create(JsonElement element, JsonNodeOptions? options)
	{
		return element.ValueKind switch
		{
			JsonValueKind.Null => null, 
			JsonValueKind.Object => new JsonObject(element, options), 
			JsonValueKind.Array => new JsonArray(element, options), 
			_ => new JsonValueOfElement(element, options), 
		};
	}

	internal override JsonSchema GetSchema(JsonNumberHandling _)
	{
		return JsonSchema.CreateTrueSchema();
	}
}
