using System.Text.Json.Schema;

namespace System.Text.Json.Serialization.Converters;

internal sealed class Int16Converter : JsonPrimitiveConverter<short>
{
	public Int16Converter()
	{
		base.IsInternalConverterForNumberType = true;
	}

	public override short Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return reader.GetInt16();
	}

	public override void Write(Utf8JsonWriter writer, short value, JsonSerializerOptions options)
	{
		writer.WriteNumberValue((long)value);
	}

	internal override short ReadAsPropertyNameCore(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return reader.GetInt16WithQuotes();
	}

	internal override void WriteAsPropertyNameCore(Utf8JsonWriter writer, short value, JsonSerializerOptions options, bool isWritingExtensionDataProperty)
	{
		writer.WritePropertyName(value);
	}

	internal override short ReadNumberWithCustomHandling(ref Utf8JsonReader reader, JsonNumberHandling handling, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.String && (JsonNumberHandling.AllowReadingFromString & handling) != JsonNumberHandling.Strict)
		{
			return reader.GetInt16WithQuotes();
		}
		return reader.GetInt16();
	}

	internal override void WriteNumberWithCustomHandling(Utf8JsonWriter writer, short value, JsonNumberHandling handling)
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
		return JsonPrimitiveConverter<short>.GetSchemaForNumericType(JsonSchemaType.Integer, numberHandling);
	}
}
