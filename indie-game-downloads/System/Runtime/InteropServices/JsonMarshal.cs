using System.Text.Json;

namespace System.Runtime.InteropServices;

public static class JsonMarshal
{
	public static ReadOnlySpan<byte> GetRawUtf8Value(JsonElement element)
	{
		return element.GetRawValue().Span;
	}

	public static ReadOnlySpan<byte> GetRawUtf8PropertyName(JsonProperty property)
	{
		return property.NameSpan;
	}
}
