using System.Text.Json.Nodes;
using System.Text.Json.Schema;

namespace System.Text.Json.Serialization.Converters;

internal sealed class JsonArrayConverter : JsonConverter<JsonArray>
{
	public override void Write(Utf8JsonWriter writer, JsonArray value, JsonSerializerOptions options)
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

	public override JsonArray Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		switch (reader.TokenType)
		{
		case JsonTokenType.StartArray:
			if (!options.AllowDuplicateProperties)
			{
				return ReadAsJsonNode(ref reader, options.GetNodeOptions());
			}
			return ReadAsJsonElement(ref reader, options.GetNodeOptions());
		case JsonTokenType.Null:
			return null;
		default:
			throw ThrowHelper.GetInvalidOperationException_ExpectedArray(reader.TokenType);
		}
	}

	internal static JsonArray ReadAsJsonElement(ref Utf8JsonReader reader, JsonNodeOptions options)
	{
		return new JsonArray(JsonElement.ParseValue(ref reader), options);
	}

	internal static JsonArray ReadAsJsonNode(ref Utf8JsonReader reader, JsonNodeOptions options)
	{
		JsonArray jsonArray = new JsonArray(options, default(ReadOnlySpan<JsonNode>));
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndArray)
			{
				return jsonArray;
			}
			JsonNode item = JsonNodeConverter.ReadAsJsonNode(ref reader, options);
			jsonArray.Add(item);
		}
		ThrowHelper.ThrowJsonException();
		return null;
	}

	internal override JsonSchema GetSchema(JsonNumberHandling _)
	{
		return new JsonSchema
		{
			Type = JsonSchemaType.Array
		};
	}
}
