using System.Diagnostics.CodeAnalysis;

namespace System.Text.Json.Nodes;

internal sealed class JsonValueOfJsonBool : JsonValue
{
	private readonly bool _value;

	private JsonValueKind ValueKind
	{
		get
		{
			if (!_value)
			{
				return JsonValueKind.False;
			}
			return JsonValueKind.True;
		}
	}

	internal JsonValueOfJsonBool(bool value, JsonNodeOptions? options)
		: base(options)
	{
		_value = value;
	}

	public override void WriteTo(Utf8JsonWriter writer, JsonSerializerOptions options = null)
	{
		writer.WriteBooleanValue(_value);
	}

	internal override JsonNode DeepCloneCore()
	{
		return new JsonValueOfJsonBool(_value, base.Options);
	}

	private protected override JsonValueKind GetValueKindCore()
	{
		return ValueKind;
	}

	public override T GetValue<T>()
	{
		if (!TryGetValue<T>(out var value))
		{
			ThrowHelper.ThrowInvalidOperationException_NodeUnableToConvertElement(_value ? JsonValueKind.True : JsonValueKind.False, typeof(T));
		}
		return value;
	}

	public override bool TryGetValue<T>([NotNullWhen(true)] out T value)
	{
		if (typeof(T) == typeof(JsonElement))
		{
			value = (T)(object)JsonElement.Parse(_value ? JsonConstants.TrueValue : JsonConstants.FalseValue);
			return true;
		}
		if (typeof(T) == typeof(bool) || typeof(T) == typeof(bool?))
		{
			value = (T)(object)_value;
			return true;
		}
		value = default(T);
		return false;
	}
}
