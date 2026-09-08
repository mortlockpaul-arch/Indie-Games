using System.Text.Json.Schema;

namespace System.Text.Json.Serialization.Converters;

internal sealed class JsonDocumentConverter : JsonConverter<JsonDocument>
{
	public override bool HandleNull => true;

	public override JsonDocument Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return JsonDocument.ParseValue(ref reader, options.AllowDuplicateProperties);
	}

	public override void Write(Utf8JsonWriter writer, JsonDocument value, JsonSerializerOptions options)
	{
		if (value == null)
		{
			writer.WriteNullValue();
		}
		else
		{
			value.WriteTo(writer);
		}
	}

	internal override JsonSchema GetSchema(JsonNumberHandling _)
	{
		return JsonSchema.CreateTrueSchema();
	}
}
