using System.Text.Json.Schema;

namespace System.Text.Json.Serialization.Converters;

internal sealed class UInt32Converter : JsonPrimitiveConverter<uint>
{
	public UInt32Converter()
	{
		base.IsInternalConverterForNumberType = true;
	}

	public override uint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return reader.GetUInt32();
	}

	public override void Write(Utf8JsonWriter writer, uint value, JsonSerializerOptions options)
	{
		writer.WriteNumberValue((ulong)value);
	}

	internal override uint ReadAsPropertyNameCore(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return reader.GetUInt32WithQuotes();
	}

	internal override void WriteAsPropertyNameCore(Utf8JsonWriter writer, uint value, JsonSerializerOptions options, bool isWritingExtensionDataProperty)
	{
		writer.WritePropertyName(value);
	}

	internal override uint ReadNumberWithCustomHandling(ref Utf8JsonReader reader, JsonNumberHandling handling, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.String && (JsonNumberHandling.AllowReadingFromString & handling) != JsonNumberHandling.Strict)
		{
			return reader.GetUInt32WithQuotes();
		}
		return reader.GetUInt32();
	}

	internal override void WriteNumberWithCustomHandling(Utf8JsonWriter writer, uint value, JsonNumberHandling handling)
	{
		if ((JsonNumberHandling.WriteAsString & handling) != JsonNumberHandling.Strict)
		{
			writer.WriteNumberValueAsString(value);
		}
		else
		{
			writer.WriteNumberValue((ulong)value);
		}
	}

	internal override JsonSchema GetSchema(JsonNumberHandling numberHandling)
	{
		return JsonPrimitiveConverter<uint>.GetSchemaForNumericType(JsonSchemaType.Integer, numberHandling);
	}
}
