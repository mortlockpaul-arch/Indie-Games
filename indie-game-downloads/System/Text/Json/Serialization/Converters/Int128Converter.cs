using System.Buffers;
using System.Globalization;
using System.Text.Json.Schema;

namespace System.Text.Json.Serialization.Converters;

internal sealed class Int128Converter : JsonPrimitiveConverter<Int128>
{
	public Int128Converter()
	{
		base.IsInternalConverterForNumberType = true;
	}

	public override Int128 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.Number)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedNumber(reader.TokenType);
		}
		return ReadCore(ref reader);
	}

	public override void Write(Utf8JsonWriter writer, Int128 value, JsonSerializerOptions options)
	{
		WriteCore(writer, value);
	}

	private static Int128 ReadCore(ref Utf8JsonReader reader)
	{
		int valueLength = reader.ValueLength;
		byte[] array = null;
		Span<byte> span = ((valueLength > 256) ? ((Span<byte>)(array = ArrayPool<byte>.Shared.Rent(valueLength))) : stackalloc byte[256]);
		Span<byte> utf8Destination = span;
		if (!Int128.TryParse(utf8Destination[..reader.CopyValue(utf8Destination)], CultureInfo.InvariantCulture, out var result))
		{
			ThrowHelper.ThrowFormatException(NumericType.Int128);
		}
		if (array != null)
		{
			ArrayPool<byte>.Shared.Return(array);
		}
		return result;
	}

	private static void WriteCore(Utf8JsonWriter writer, Int128 value)
	{
		Span<byte> destination = stackalloc byte[40];
		Format(destination, value, out var written);
		writer.WriteRawValue(destination.Slice(0, written));
	}

	internal override Int128 ReadAsPropertyNameCore(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return ReadCore(ref reader);
	}

	internal override void WriteAsPropertyNameCore(Utf8JsonWriter writer, Int128 value, JsonSerializerOptions options, bool isWritingExtensionDataProperty)
	{
		Span<byte> destination = stackalloc byte[40];
		Format(destination, value, out var written);
		writer.WritePropertyName(destination.Slice(0, written));
	}

	internal override Int128 ReadNumberWithCustomHandling(ref Utf8JsonReader reader, JsonNumberHandling handling, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.String && (JsonNumberHandling.AllowReadingFromString & handling) != JsonNumberHandling.Strict)
		{
			return ReadCore(ref reader);
		}
		return Read(ref reader, Type, options);
	}

	internal override void WriteNumberWithCustomHandling(Utf8JsonWriter writer, Int128 value, JsonNumberHandling handling)
	{
		if ((JsonNumberHandling.WriteAsString & handling) != JsonNumberHandling.Strict)
		{
			Span<byte> span = stackalloc byte[42];
			span[0] = 34;
			Format(span.Slice(1), value, out var written);
			int num = written + 2;
			span[num - 1] = 34;
			writer.WriteRawValue(span.Slice(0, num));
		}
		else
		{
			WriteCore(writer, value);
		}
	}

	internal override JsonSchema GetSchema(JsonNumberHandling numberHandling)
	{
		return JsonPrimitiveConverter<Int128>.GetSchemaForNumericType(JsonSchemaType.Integer, numberHandling);
	}

	private static void Format(Span<byte> destination, Int128 value, out int written)
	{
		IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
		value.TryFormat(destination, out written, default(ReadOnlySpan<char>), invariantCulture);
	}
}
