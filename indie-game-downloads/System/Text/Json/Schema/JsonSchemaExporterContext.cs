using System.Text.Json.Serialization.Metadata;

namespace System.Text.Json.Schema;

public readonly struct JsonSchemaExporterContext
{
	internal readonly string[] _path;

	public JsonTypeInfo TypeInfo { get; }

	public JsonPropertyInfo? PropertyInfo { get; }

	public JsonTypeInfo? BaseTypeInfo { get; }

	public ReadOnlySpan<string> Path => _path;

	internal JsonSchemaExporterContext(JsonTypeInfo typeInfo, JsonPropertyInfo propertyInfo, JsonTypeInfo baseTypeInfo, string[] path)
	{
		TypeInfo = typeInfo;
		PropertyInfo = propertyInfo;
		BaseTypeInfo = baseTypeInfo;
		_path = path;
	}
}
