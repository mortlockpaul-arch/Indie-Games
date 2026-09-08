using System.Text.Json.Schema;

namespace System.Text.Json.Serialization.Converters;

internal sealed class VersionConverter : JsonPrimitiveConverter<Version>
{
	public override Version Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.Null)
		{
			return null;
		}
		if (reader.TokenType != JsonTokenType.String)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedString(reader.TokenType);
		}
		return ReadCore(ref reader);
	}

	private static Version ReadCore(ref Utf8JsonReader reader)
	{
		if (!JsonHelpers.IsInRangeInclusive(reader.ValueLength, 3, 258))
		{
			ThrowHelper.ThrowFormatException(DataType.Version);
		}
		Span<char> destination = stackalloc char[258];
		ReadOnlySpan<char> input = destination[..reader.CopyString(destination)];
		if (char.IsDigit(input[0]))
		{
			if (char.IsDigit(input[input.Length - 1]))
			{
				goto IL_0070;
			}
		}
		ThrowHelper.ThrowFormatException(DataType.Version);
		goto IL_0070;
		IL_0070:
		if (Version.TryParse(input, out Version result))
		{
			return result;
		}
		ThrowHelper.ThrowJsonException();
		return null;
	}

	public override void Write(Utf8JsonWriter writer, Version value, JsonSerializerOptions options)
	{
		if ((object)value == null)
		{
			writer.WriteNullValue();
			return;
		}
		Span<byte> utf8Destination = stackalloc byte[43];
		value.TryFormat(utf8Destination, out var bytesWritten);
		writer.WriteStringValue(utf8Destination.Slice(0, bytesWritten));
	}

	internal override Version ReadAsPropertyNameCore(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return ReadCore(ref reader);
	}

	internal override void WriteAsPropertyNameCore(Utf8JsonWriter writer, Version value, JsonSerializerOptions options, bool isWritingExtensionDataProperty)
	{
		ArgumentNullException.ThrowIfNull(value, "value");
		Span<byte> utf8Destination = stackalloc byte[43];
		value.TryFormat(utf8Destination, out var bytesWritten);
		writer.WritePropertyName(utf8Destination.Slice(0, bytesWritten));
	}

	internal override JsonSchema GetSchema(JsonNumberHandling _)
	{
		return new JsonSchema
		{
			Type = JsonSchemaType.String,
			Comment = "Represents a version string.",
			Pattern = "^\\d+(\\.\\d+){1,3}$"
		};
	}
}
