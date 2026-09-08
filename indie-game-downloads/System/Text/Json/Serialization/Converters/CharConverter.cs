using System.Text.Json.Schema;

namespace System.Text.Json.Serialization.Converters;

internal sealed class CharConverter : JsonPrimitiveConverter<char>
{
	public override char Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		JsonTokenType tokenType = reader.TokenType;
		if ((tokenType != JsonTokenType.PropertyName && tokenType != JsonTokenType.String) || 1 == 0)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedString(reader.TokenType);
		}
		if (!JsonHelpers.IsInRangeInclusive(reader.ValueLength, 1, 6))
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedChar(reader.TokenType);
		}
		Span<char> destination = stackalloc char[6];
		if (reader.CopyString(destination) != 1)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedChar(reader.TokenType);
		}
		return destination[0];
	}

	public override void Write(Utf8JsonWriter writer, char value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(new ReadOnlySpan<char>(in value));
	}

	internal override char ReadAsPropertyNameCore(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return Read(ref reader, typeToConvert, options);
	}

	internal override void WriteAsPropertyNameCore(Utf8JsonWriter writer, char value, JsonSerializerOptions options, bool isWritingExtensionDataProperty)
	{
		writer.WritePropertyName(new ReadOnlySpan<char>(in value));
	}

	internal override JsonSchema GetSchema(JsonNumberHandling _)
	{
		return new JsonSchema
		{
			Type = JsonSchemaType.String,
			MinLength = 1,
			MaxLength = 1
		};
	}
}
