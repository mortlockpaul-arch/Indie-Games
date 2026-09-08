using System.Buffers.Text;
using System.Text.Json.Schema;

namespace System.Text.Json.Serialization.Converters;

internal sealed class TimeOnlyConverter : JsonPrimitiveConverter<TimeOnly>
{
	public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.String)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedString(reader.TokenType);
		}
		return ReadCore(ref reader);
	}

	internal override TimeOnly ReadAsPropertyNameCore(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return ReadCore(ref reader);
	}

	private static TimeOnly ReadCore(ref Utf8JsonReader reader)
	{
		if (!JsonHelpers.IsInRangeInclusive(reader.ValueLength, 3, 96))
		{
			ThrowHelper.ThrowFormatException(DataType.TimeOnly);
		}
		ReadOnlySpan<byte> readOnlySpan;
		if (!reader.HasValueSequence && !reader.ValueIsEscaped)
		{
			readOnlySpan = reader.ValueSpan;
		}
		else
		{
			Span<byte> utf8Destination = stackalloc byte[96];
			readOnlySpan = utf8Destination[..reader.CopyString(utf8Destination)];
		}
		byte value = readOnlySpan[0];
		int num = readOnlySpan.IndexOfAny((byte)46, (byte)58);
		if (!JsonHelpers.IsDigit(value) || num < 0 || readOnlySpan[num] == 46)
		{
			ThrowHelper.ThrowFormatException(DataType.TimeOnly);
		}
		if (!Utf8Parser.TryParse(readOnlySpan, out TimeSpan value2, out int bytesConsumed, 'c') || readOnlySpan.Length != bytesConsumed)
		{
			ThrowHelper.ThrowFormatException(DataType.TimeOnly);
		}
		return TimeOnly.FromTimeSpan(value2);
	}

	public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
	{
		Span<byte> destination = stackalloc byte[16];
		Utf8Formatter.TryFormat(value.ToTimeSpan(), destination, out var bytesWritten, 'c');
		writer.WriteStringValue(destination.Slice(0, bytesWritten));
	}

	internal override void WriteAsPropertyNameCore(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options, bool isWritingExtensionDataProperty)
	{
		Span<byte> destination = stackalloc byte[16];
		Utf8Formatter.TryFormat(value.ToTimeSpan(), destination, out var bytesWritten, 'c');
		writer.WritePropertyName(destination.Slice(0, bytesWritten));
	}

	internal override JsonSchema GetSchema(JsonNumberHandling _)
	{
		return new JsonSchema
		{
			Type = JsonSchemaType.String,
			Format = "time"
		};
	}
}
