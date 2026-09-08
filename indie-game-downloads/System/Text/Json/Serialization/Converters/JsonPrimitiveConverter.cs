using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;

namespace System.Text.Json.Serialization.Converters;

internal abstract class JsonPrimitiveConverter<T> : JsonConverter<T>
{
	public sealed override void WriteAsPropertyName(Utf8JsonWriter writer, [DisallowNull] T value, JsonSerializerOptions options)
	{
		ArgumentNullException.ThrowIfNull(value, "value");
		WriteAsPropertyNameCore(writer, value, options, isWritingExtensionDataProperty: false);
	}

	public sealed override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.PropertyName)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedPropertyName(reader.TokenType);
		}
		return ReadAsPropertyNameCore(ref reader, typeToConvert, options);
	}

	private protected static JsonSchema GetSchemaForNumericType(JsonSchemaType schemaType, JsonNumberHandling numberHandling, bool isIeeeFloatingPoint = false)
	{
		string pattern = null;
		if ((numberHandling & (JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)) != JsonNumberHandling.Strict)
		{
			pattern = ((schemaType == JsonSchemaType.Integer) ? "^-?(?:0|[1-9]\\d*)$" : (isIeeeFloatingPoint ? "^-?(?:0|[1-9]\\d*)(?:\\.\\d+)?(?:[eE][+-]?\\d+)?$" : "^-?(?:0|[1-9]\\d*)(?:\\.\\d+)?$"));
			schemaType |= JsonSchemaType.String;
		}
		if (isIeeeFloatingPoint && (numberHandling & JsonNumberHandling.AllowNamedFloatingPointLiterals) != JsonNumberHandling.Strict)
		{
			JsonSchema jsonSchema = new JsonSchema();
			int num = 2;
			List<JsonSchema> list = new List<JsonSchema>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<JsonSchema> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = new JsonSchema
			{
				Type = schemaType,
				Pattern = pattern
			};
			num2++;
			span[num2] = new JsonSchema
			{
				Enum = new JsonArray
				{
					(JsonNode?)"NaN",
					(JsonNode?)"Infinity",
					(JsonNode?)"-Infinity"
				}
			};
			jsonSchema.AnyOf = list;
			return jsonSchema;
		}
		return new JsonSchema
		{
			Type = schemaType,
			Pattern = pattern
		};
	}
}
