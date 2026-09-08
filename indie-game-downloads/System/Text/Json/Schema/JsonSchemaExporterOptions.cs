using System.Text.Json.Nodes;

namespace System.Text.Json.Schema;

public sealed class JsonSchemaExporterOptions
{
	public static JsonSchemaExporterOptions Default { get; } = new JsonSchemaExporterOptions();

	public bool TreatNullObliviousAsNonNullable { get; init; }

	public Func<JsonSchemaExporterContext, JsonNode, JsonNode>? TransformSchemaNode { get; init; }
}
