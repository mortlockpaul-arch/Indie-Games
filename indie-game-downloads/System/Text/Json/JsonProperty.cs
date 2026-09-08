using System.Diagnostics;

namespace System.Text.Json;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct JsonProperty
{
	public JsonElement Value { get; }

	public string Name => Value.GetPropertyName();

	internal bool NameIsEscaped => Value.ValueIsEscapedHelper(isPropertyName: true);

	internal ReadOnlySpan<byte> NameSpan => Value.GetPropertyNameRaw();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string DebuggerDisplay
	{
		get
		{
			if (Value.ValueKind != JsonValueKind.Undefined)
			{
				return "\"" + ToString() + "\"";
			}
			return "<Undefined>";
		}
	}

	internal JsonProperty(JsonElement value)
	{
		Value = value;
	}

	public bool NameEquals(string? text)
	{
		return NameEquals(text.AsSpan());
	}

	public bool NameEquals(ReadOnlySpan<byte> utf8Text)
	{
		return Value.TextEqualsHelper(utf8Text, isPropertyName: true, shouldUnescape: true);
	}

	public bool NameEquals(ReadOnlySpan<char> text)
	{
		return Value.TextEqualsHelper(text, isPropertyName: true);
	}

	internal bool EscapedNameEquals(ReadOnlySpan<byte> utf8Text)
	{
		return Value.TextEqualsHelper(utf8Text, isPropertyName: true, shouldUnescape: false);
	}

	public void WriteTo(Utf8JsonWriter writer)
	{
		ArgumentNullException.ThrowIfNull(writer, "writer");
		Value.WritePropertyNameTo(writer);
		Value.WriteTo(writer);
	}

	public override string ToString()
	{
		return Value.GetPropertyRawText();
	}
}
