using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace System.Net.Http.Json;

internal static class JsonHelpers
{
	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	internal static JsonTypeInfo GetJsonTypeInfo(Type type, JsonSerializerOptions options)
	{
		if (options == null)
		{
			options = JsonSerializerOptions.Web;
		}
		options.MakeReadOnly(populateMissingResolver: true);
		return options.GetTypeInfo(type);
	}

	internal static Encoding GetEncoding(HttpContent content)
	{
		Encoding result = null;
		string text = content.Headers.ContentType?.CharSet;
		if (text != null)
		{
			try
			{
				result = ((text.Length <= 2 || text[0] != '"' || text[text.Length - 1] != '"') ? Encoding.GetEncoding(text) : Encoding.GetEncoding(text.Substring(1, text.Length - 2)));
			}
			catch (ArgumentException innerException)
			{
				throw new InvalidOperationException(System.SR.CharSetInvalid, innerException);
			}
		}
		return result;
	}
}
