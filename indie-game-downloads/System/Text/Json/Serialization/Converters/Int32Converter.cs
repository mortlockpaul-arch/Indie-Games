using System.Text.Json.Schema;

namespace System.Text.Json.Serialization.Converters;

internal sealed class Int32Converter : JsonPrimitiveConverter<int>
{
	public Int32Converter()
	{
		base.IsInternalConverterForNumberType = true;
	}

	public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return reader.GetInt32();
	}

	public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
	{
		writer.WriteNumberValue((long)value);
	}

	internal override int ReadAsPropertyNameCore(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return reader.GetInt32WithQuotes();
	}

	internal override void WriteAsPropertyNameCore(Utf8JsonWriter writer, int value, JsonSerializerOptions options, bool isWritingExtensionDataProperty)
	{
		writer.WritePropertyName(value);
	}

	internal override int ReadNumberWithCustomHandling(ref Utf8JsonReader reader, JsonNumberHandling handling, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.String && (JsonNumberHandling.AllowReadingFromString & handling) != JsonNumberHandling.Strict)
		{
			return reader.GetInt32WithQuotes();
		}
		return reader.GetInt32();
	}

	internal override void WriteNumberWithCustomHandling(Utf8JsonWriter writer, int value, JsonNumberHandling handling)
	{
		if ((JsonNumberHandling.WriteAsString & handling) != JsonNumberHandling.Strict)
		{
			writer.WriteNumberValueAsString(value);
		}
		else
		{
			writer.WriteNumberValue((long)value);
		}
	}

	internal override JsonSchema GetSchema(JsonNumberHandling numberHandling)
	{
		return JsonPrimitiveConverter<int>.GetSchemaForNumericType(JsonSchemaType.Integer, numberHandling);
	}
}
