using System.Text.Json.Schema;

namespace System.Text.Json.Serialization.Converters;

internal sealed class DoubleConverter : JsonPrimitiveConverter<double>
{
	public DoubleConverter()
	{
		base.IsInternalConverterForNumberType = true;
	}

	public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return reader.GetDouble();
	}

	public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
	{
		writer.WriteNumberValue(value);
	}

	internal override double ReadAsPropertyNameCore(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return reader.GetDoubleWithQuotes();
	}

	internal override void WriteAsPropertyNameCore(Utf8JsonWriter writer, double value, JsonSerializerOptions options, bool isWritingExtensionDataProperty)
	{
		writer.WritePropertyName(value);
	}

	internal override double ReadNumberWithCustomHandling(ref Utf8JsonReader reader, JsonNumberHandling handling, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.String)
		{
			if ((JsonNumberHandling.AllowReadingFromString & handling) != JsonNumberHandling.Strict)
			{
				return reader.GetDoubleWithQuotes();
			}
			if ((JsonNumberHandling.AllowNamedFloatingPointLiterals & handling) != JsonNumberHandling.Strict)
			{
				return reader.GetDoubleFloatingPointConstant();
			}
		}
		return reader.GetDouble();
	}

	internal override void WriteNumberWithCustomHandling(Utf8JsonWriter writer, double value, JsonNumberHandling handling)
	{
		if ((JsonNumberHandling.WriteAsString & handling) != JsonNumberHandling.Strict)
		{
			writer.WriteNumberValueAsString(value);
		}
		else if ((JsonNumberHandling.AllowNamedFloatingPointLiterals & handling) != JsonNumberHandling.Strict)
		{
			writer.WriteFloatingPointConstant(value);
		}
		else
		{
			writer.WriteNumberValue(value);
		}
	}

	internal override JsonSchema GetSchema(JsonNumberHandling numberHandling)
	{
		return JsonPrimitiveConverter<double>.GetSchemaForNumericType(JsonSchemaType.Number, numberHandling, isIeeeFloatingPoint: true);
	}
}
