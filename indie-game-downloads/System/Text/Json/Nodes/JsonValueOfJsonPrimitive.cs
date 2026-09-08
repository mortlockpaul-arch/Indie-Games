using System.Buffers;

namespace System.Text.Json.Nodes;

internal static class JsonValueOfJsonPrimitive
{
	internal static JsonValue CreatePrimitiveValue(ref Utf8JsonReader reader, JsonNodeOptions options)
	{
		switch (reader.TokenType)
		{
		case JsonTokenType.True:
		case JsonTokenType.False:
			return new JsonValueOfJsonBool(reader.GetBoolean(), options);
		case JsonTokenType.String:
		{
			byte[] array = new byte[reader.ValueLength];
			return new JsonValueOfJsonString(array.AsMemory(0, reader.CopyString(array)), options);
		}
		case JsonTokenType.Number:
			return new JsonValueOfJsonNumber(reader.HasValueSequence ? reader.ValueSequence.ToArray<byte>() : reader.ValueSpan.ToArray(), options);
		default:
			ThrowHelper.ThrowJsonException();
			return null;
		}
	}
}
