using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using System.Text.Json.Serialization.Metadata;

namespace System.Text.Json.Serialization.Converters;

internal sealed class JsonObjectConverter : JsonConverter<JsonObject>
{
	internal override void ConfigureJsonTypeInfo(JsonTypeInfo jsonTypeInfo, JsonSerializerOptions options)
	{
		jsonTypeInfo.CreateObjectForExtensionDataProperty = () => new JsonObject(options.GetNodeOptions());
	}

	internal override void ReadElementAndSetProperty(object obj, string propertyName, ref Utf8JsonReader reader, JsonSerializerOptions options, scoped ref ReadStack state)
	{
		JsonNodeConverter.Instance.TryRead(ref reader, typeof(JsonNode), options, ref state, out var value, out var _);
		JsonObject jsonObject = (JsonObject)obj;
		JsonNode value2 = value;
		if (options.AllowDuplicateProperties)
		{
			jsonObject[propertyName] = value2;
		}
		else if (!jsonObject.TryAdd(propertyName, value2))
		{
			ThrowHelper.ThrowJsonException_DuplicatePropertyNotAllowed(propertyName);
		}
	}

	public override void Write(Utf8JsonWriter writer, JsonObject value, JsonSerializerOptions options)
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

	public override JsonObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		switch (reader.TokenType)
		{
		case JsonTokenType.StartObject:
			if (!options.AllowDuplicateProperties)
			{
				return ReadAsJsonNode(ref reader, options.GetNodeOptions());
			}
			return ReadAsJsonElement(ref reader, options.GetNodeOptions());
		case JsonTokenType.Null:
			return null;
		default:
			throw ThrowHelper.GetInvalidOperationException_ExpectedObject(reader.TokenType);
		}
	}

	internal static JsonObject ReadAsJsonElement(ref Utf8JsonReader reader, JsonNodeOptions options)
	{
		return new JsonObject(JsonElement.ParseValue(ref reader), options);
	}

	internal static JsonObject ReadAsJsonNode(ref Utf8JsonReader reader, JsonNodeOptions options)
	{
		JsonObject jsonObject = new JsonObject(options);
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
			{
				return jsonObject;
			}
			if (reader.TokenType != JsonTokenType.PropertyName)
			{
				ThrowHelper.ThrowJsonException();
			}
			string propertyName = reader.GetString();
			reader.Read();
			JsonNode value = JsonNodeConverter.ReadAsJsonNode(ref reader, options);
			jsonObject.Add(propertyName, value);
		}
		ThrowHelper.ThrowJsonException();
		return null;
	}

	internal override JsonSchema GetSchema(JsonNumberHandling _)
	{
		return new JsonSchema
		{
			Type = JsonSchemaType.Object
		};
	}
}
