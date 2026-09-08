using System.Text.Json.Schema;

namespace System.Text.Json.Serialization.Converters;

internal sealed class JsonElementConverter : JsonConverter<JsonElement>
{
	public override JsonElement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return JsonElement.ParseValue(ref reader, options.AllowDuplicateProperties);
	}

	public override void Write(Utf8JsonWriter writer, JsonElement value, JsonSerializerOptions options)
	{
		value.WriteTo(writer);
	}

	internal override JsonSchema GetSchema(JsonNumberHandling _)
	{
		return JsonSchema.CreateTrueSchema();
	}
}
