using System.Globalization;
using System.Text.Json.Schema;

namespace System.Text.Json.Serialization.Converters;

internal sealed class DateOnlyConverter : JsonPrimitiveConverter<DateOnly>
{
	public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.String)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedString(reader.TokenType);
		}
		return ReadCore(ref reader);
	}

	internal override DateOnly ReadAsPropertyNameCore(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return ReadCore(ref reader);
	}

	private static DateOnly ReadCore(ref Utf8JsonReader reader)
	{
		if (!JsonHelpers.IsInRangeInclusive(reader.ValueLength, 10, 60))
		{
			ThrowHelper.ThrowFormatException(DataType.DateOnly);
		}
		ReadOnlySpan<byte> source;
		if (!reader.HasValueSequence && !reader.ValueIsEscaped)
		{
			source = reader.ValueSpan;
		}
		else
		{
			Span<byte> utf8Destination = stackalloc byte[60];
			int num = reader.CopyString(utf8Destination);
			if (num < 10)
			{
				ThrowHelper.ThrowFormatException(DataType.DateOnly);
			}
			source = utf8Destination.Slice(0, num);
		}
		if (!JsonHelpers.TryParseAsIso(source, out var value))
		{
			ThrowHelper.ThrowFormatException(DataType.DateOnly);
		}
		return value;
	}

	public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
	{
		Span<byte> span = stackalloc byte[10];
		value.TryFormat(span, out var _, "O".AsSpan(), CultureInfo.InvariantCulture);
		writer.WriteStringValue(span);
	}

	internal override void WriteAsPropertyNameCore(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options, bool isWritingExtensionDataProperty)
	{
		Span<byte> span = stackalloc byte[10];
		value.TryFormat(span, out var _, "O".AsSpan(), CultureInfo.InvariantCulture);
		writer.WritePropertyName(span);
	}

	internal override JsonSchema GetSchema(JsonNumberHandling _)
	{
		return new JsonSchema
		{
			Type = JsonSchemaType.String,
			Format = "date"
		};
	}
}
