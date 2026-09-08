using System.Text.Json.Nodes;

namespace System.Text.Json.Serialization.Converters;

internal sealed class JsonNodeConverterFactory : JsonConverterFactory
{
	private static readonly JsonArrayConverter s_arrayConverter = new JsonArrayConverter();

	private static readonly JsonObjectConverter s_objectConverter = new JsonObjectConverter();

	private static readonly JsonValueConverter s_valueConverter = new JsonValueConverter();

	public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
	{
		if (typeof(JsonValue).IsAssignableFrom(typeToConvert))
		{
			return s_valueConverter;
		}
		if (typeof(JsonObject) == typeToConvert)
		{
			return s_objectConverter;
		}
		if (typeof(JsonArray) == typeToConvert)
		{
			return s_arrayConverter;
		}
		return JsonNodeConverter.Instance;
	}

	public override bool CanConvert(Type typeToConvert)
	{
		return typeof(JsonNode).IsAssignableFrom(typeToConvert);
	}
}
