using System.Buffers.Text;
using System.Text.Json.Schema;

namespace System.Text.Json.Serialization.Converters;

internal sealed class BooleanConverter : JsonPrimitiveConverter<bool>
{
	public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return reader.GetBoolean();
	}

	public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
	{
		writer.WriteBooleanValue(value);
	}

	internal override bool ReadAsPropertyNameCore(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		ReadOnlySpan<byte> unescapedSpan = reader.GetUnescapedSpan();
		if (!Utf8Parser.TryParse(unescapedSpan, out bool value, out int bytesConsumed, '\0') || unescapedSpan.Length != bytesConsumed)
		{
			ThrowHelper.ThrowFormatException(DataType.Boolean);
		}
		return value;
	}

	internal override void WriteAsPropertyNameCore(Utf8JsonWriter writer, bool value, JsonSerializerOptions options, bool isWritingExtensionDataProperty)
	{
		writer.WritePropertyName(value);
	}

	internal override JsonSchema GetSchema(JsonNumberHandling _)
	{
		return new JsonSchema
		{
			Type = JsonSchemaType.Boolean
		};
	}
}
