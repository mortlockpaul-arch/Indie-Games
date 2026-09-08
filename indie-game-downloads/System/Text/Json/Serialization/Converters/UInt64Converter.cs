using System.Text.Json.Schema;

namespace System.Text.Json.Serialization.Converters;

internal sealed class UInt64Converter : JsonPrimitiveConverter<ulong>
{
	public UInt64Converter()
	{
		base.IsInternalConverterForNumberType = true;
	}

	public override ulong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return reader.GetUInt64();
	}

	public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
	{
		writer.WriteNumberValue(value);
	}

	internal override ulong ReadAsPropertyNameCore(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return reader.GetUInt64WithQuotes();
	}

	internal override void WriteAsPropertyNameCore(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options, bool isWritingExtensionDataProperty)
	{
		writer.WritePropertyName(value);
	}

	internal override ulong ReadNumberWithCustomHandling(ref Utf8JsonReader reader, JsonNumberHandling handling, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.String && (JsonNumberHandling.AllowReadingFromString & handling) != JsonNumberHandling.Strict)
		{
			return reader.GetUInt64WithQuotes();
		}
		return reader.GetUInt64();
	}

	internal override void WriteNumberWithCustomHandling(Utf8JsonWriter writer, ulong value, JsonNumberHandling handling)
	{
		if ((JsonNumberHandling.WriteAsString & handling) != JsonNumberHandling.Strict)
		{
			writer.WriteNumberValueAsString(value);
		}
		else
		{
			writer.WriteNumberValue(value);
		}
	}

	internal override JsonSchema GetSchema(JsonNumberHandling numberHandling)
	{
		return JsonPrimitiveConverter<ulong>.GetSchemaForNumericType(JsonSchemaType.Integer, numberHandling);
	}
}
