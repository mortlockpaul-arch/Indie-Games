using System.Buffers.Text;
using System.Diagnostics.CodeAnalysis;

namespace System.Text.Json.Nodes;

internal sealed class JsonValueOfJsonNumber : JsonValue
{
	private readonly byte[] _value;

	internal JsonValueOfJsonNumber(byte[] number, JsonNodeOptions? options)
		: base(options)
	{
		_value = number;
	}

	internal override JsonNode DeepCloneCore()
	{
		return new JsonValueOfJsonNumber(_value, base.Options);
	}

	private protected override JsonValueKind GetValueKindCore()
	{
		return JsonValueKind.Number;
	}

	public override T GetValue<T>()
	{
		if (!TryGetValue<T>(out var value))
		{
			ThrowHelper.ThrowInvalidOperationException_NodeUnableToConvertElement(JsonValueKind.Number, typeof(T));
		}
		return value;
	}

	public override bool TryGetValue<T>([NotNullWhen(true)] out T value)
	{
		if (typeof(T) == typeof(JsonElement))
		{
			value = (T)(object)JsonElement.Parse(_value);
			return true;
		}
		if (typeof(T) == typeof(int) || typeof(T) == typeof(int?))
		{
			bool result = Utf8Parser.TryParse((ReadOnlySpan<byte>)_value, out int value2, out int bytesConsumed, '\0') && bytesConsumed == _value.Length;
			value = (T)(object)value2;
			return result;
		}
		if (typeof(T) == typeof(long) || typeof(T) == typeof(long?))
		{
			bool result2 = Utf8Parser.TryParse((ReadOnlySpan<byte>)_value, out long value3, out int bytesConsumed2, '\0') && bytesConsumed2 == _value.Length;
			value = (T)(object)value3;
			return result2;
		}
		if (typeof(T) == typeof(double) || typeof(T) == typeof(double?))
		{
			bool result3 = Utf8Parser.TryParse((ReadOnlySpan<byte>)_value, out double value4, out int bytesConsumed3, '\0') && bytesConsumed3 == _value.Length;
			value = (T)(object)value4;
			return result3;
		}
		if (typeof(T) == typeof(short) || typeof(T) == typeof(short?))
		{
			bool result4 = Utf8Parser.TryParse((ReadOnlySpan<byte>)_value, out short value5, out int bytesConsumed4, '\0') && bytesConsumed4 == _value.Length;
			value = (T)(object)value5;
			return result4;
		}
		if (typeof(T) == typeof(decimal) || typeof(T) == typeof(decimal?))
		{
			bool result5 = Utf8Parser.TryParse((ReadOnlySpan<byte>)_value, out decimal value6, out int bytesConsumed5, '\0') && bytesConsumed5 == _value.Length;
			value = (T)(object)value6;
			return result5;
		}
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(byte?))
		{
			bool result6 = Utf8Parser.TryParse((ReadOnlySpan<byte>)_value, out byte value7, out int bytesConsumed6, '\0') && bytesConsumed6 == _value.Length;
			value = (T)(object)value7;
			return result6;
		}
		if (typeof(T) == typeof(float) || typeof(T) == typeof(float?))
		{
			bool result7 = Utf8Parser.TryParse((ReadOnlySpan<byte>)_value, out float value8, out int bytesConsumed7, '\0') && bytesConsumed7 == _value.Length;
			value = (T)(object)value8;
			return result7;
		}
		if (typeof(T) == typeof(uint) || typeof(T) == typeof(uint?))
		{
			bool result8 = Utf8Parser.TryParse((ReadOnlySpan<byte>)_value, out uint value9, out int bytesConsumed8, '\0') && bytesConsumed8 == _value.Length;
			value = (T)(object)value9;
			return result8;
		}
		if (typeof(T) == typeof(ushort) || typeof(T) == typeof(ushort?))
		{
			bool result9 = Utf8Parser.TryParse((ReadOnlySpan<byte>)_value, out ushort value10, out int bytesConsumed9, '\0') && bytesConsumed9 == _value.Length;
			value = (T)(object)value10;
			return result9;
		}
		if (typeof(T) == typeof(ulong) || typeof(T) == typeof(ulong?))
		{
			bool result10 = Utf8Parser.TryParse((ReadOnlySpan<byte>)_value, out ulong value11, out int bytesConsumed10, '\0') && bytesConsumed10 == _value.Length;
			value = (T)(object)value11;
			return result10;
		}
		if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(sbyte?))
		{
			bool result11 = Utf8Parser.TryParse((ReadOnlySpan<byte>)_value, out sbyte value12, out int bytesConsumed11, '\0') && bytesConsumed11 == _value.Length;
			value = (T)(object)value12;
			return result11;
		}
		value = default(T);
		return false;
	}

	public override void WriteTo(Utf8JsonWriter writer, JsonSerializerOptions options = null)
	{
		ArgumentNullException.ThrowIfNull(writer, "writer");
		writer.WriteNumberValue(_value);
	}
}
