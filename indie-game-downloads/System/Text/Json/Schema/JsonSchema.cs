using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace System.Text.Json.Schema;

internal sealed class JsonSchema
{
	private readonly bool? _trueOrFalse;

	private string _ref;

	private string _comment;

	private JsonSchemaType _type;

	private string _format;

	private string _pattern;

	private JsonNode _constant;

	private List<KeyValuePair<string, JsonSchema>> _properties;

	private List<string> _required;

	private JsonSchema _items;

	private JsonSchema _additionalProperties;

	private JsonArray _enum;

	private JsonSchema _not;

	private List<JsonSchema> _anyOf;

	private bool _hasDefaultValue;

	private JsonNode _defaultValue;

	private int? _minLength;

	private int? _maxLength;

	public bool IsTrue => _trueOrFalse ?? false;

	public string Ref
	{
		get
		{
			return _ref;
		}
		set
		{
			VerifyMutable();
			_ref = value;
		}
	}

	public string Comment
	{
		get
		{
			return _comment;
		}
		set
		{
			VerifyMutable();
			_comment = value;
		}
	}

	public JsonSchemaType Type
	{
		get
		{
			return _type;
		}
		set
		{
			VerifyMutable();
			_type = value;
		}
	}

	public string Format
	{
		get
		{
			return _format;
		}
		set
		{
			VerifyMutable();
			_format = value;
		}
	}

	public string Pattern
	{
		get
		{
			return _pattern;
		}
		set
		{
			VerifyMutable();
			_pattern = value;
		}
	}

	public JsonNode Constant
	{
		get
		{
			return _constant;
		}
		set
		{
			VerifyMutable();
			_constant = value;
		}
	}

	public List<KeyValuePair<string, JsonSchema>> Properties
	{
		get
		{
			return _properties;
		}
		set
		{
			VerifyMutable();
			_properties = value;
		}
	}

	public List<string> Required
	{
		get
		{
			return _required;
		}
		set
		{
			VerifyMutable();
			_required = value;
		}
	}

	public JsonSchema Items
	{
		get
		{
			return _items;
		}
		set
		{
			VerifyMutable();
			_items = value;
		}
	}

	public JsonSchema AdditionalProperties
	{
		get
		{
			return _additionalProperties;
		}
		set
		{
			VerifyMutable();
			_additionalProperties = value;
		}
	}

	public JsonArray Enum
	{
		get
		{
			return _enum;
		}
		set
		{
			VerifyMutable();
			_enum = value;
		}
	}

	public JsonSchema Not
	{
		get
		{
			return _not;
		}
		set
		{
			VerifyMutable();
			_not = value;
		}
	}

	public List<JsonSchema> AnyOf
	{
		get
		{
			return _anyOf;
		}
		set
		{
			VerifyMutable();
			_anyOf = value;
		}
	}

	public bool HasDefaultValue
	{
		get
		{
			return _hasDefaultValue;
		}
		set
		{
			VerifyMutable();
			_hasDefaultValue = value;
		}
	}

	public JsonNode DefaultValue
	{
		get
		{
			return _defaultValue;
		}
		set
		{
			VerifyMutable();
			_defaultValue = value;
		}
	}

	public int? MinLength
	{
		get
		{
			return _minLength;
		}
		set
		{
			VerifyMutable();
			_minLength = value;
		}
	}

	public int? MaxLength
	{
		get
		{
			return _maxLength;
		}
		set
		{
			VerifyMutable();
			_maxLength = value;
		}
	}

	public JsonSchemaExporterContext? ExporterContext { get; set; }

	public int KeywordCount
	{
		get
		{
			if (_trueOrFalse.HasValue)
			{
				return 0;
			}
			int count = 0;
			Count(Ref != null);
			Count(Comment != null);
			Count(Type != JsonSchemaType.Any);
			Count(Format != null);
			Count(Pattern != null);
			Count(Constant != null);
			Count(Properties != null);
			Count(Required != null);
			Count(Items != null);
			Count(AdditionalProperties != null);
			Count(Enum != null);
			Count(Not != null);
			Count(AnyOf != null);
			Count(HasDefaultValue);
			Count(MinLength.HasValue);
			Count(MaxLength.HasValue);
			return count;
			void Count(bool isKeywordSpecified)
			{
				count += (isKeywordSpecified ? 1 : 0);
			}
		}
	}

	private static ReadOnlySpan<JsonSchemaType> s_schemaValues => new JsonSchemaType[7]
	{
		JsonSchemaType.String,
		JsonSchemaType.Integer,
		JsonSchemaType.Number,
		JsonSchemaType.Boolean,
		JsonSchemaType.Array,
		JsonSchemaType.Object,
		JsonSchemaType.Null
	};

	public static JsonSchema CreateFalseSchema()
	{
		return new JsonSchema(trueOrFalse: false);
	}

	public static JsonSchema CreateTrueSchema()
	{
		return new JsonSchema(trueOrFalse: true);
	}

	public JsonSchema()
	{
	}

	private JsonSchema(bool trueOrFalse)
	{
		_trueOrFalse = trueOrFalse;
	}

	public void MakeNullable()
	{
		if (!_trueOrFalse.HasValue && Type != JsonSchemaType.Any)
		{
			Type |= JsonSchemaType.Null;
		}
	}

	public JsonNode ToJsonNode(JsonSchemaExporterOptions options)
	{
		bool? trueOrFalse = _trueOrFalse;
		if (trueOrFalse.HasValue)
		{
			bool valueOrDefault = trueOrFalse == true;
			return CompleteSchema(valueOrDefault);
		}
		JsonObject jsonObject = new JsonObject();
		if (Ref != null)
		{
			jsonObject.Add("$ref", Ref);
		}
		if (Comment != null)
		{
			jsonObject.Add("$comment", Comment);
		}
		JsonNode jsonNode = MapSchemaType(Type);
		if (jsonNode != null)
		{
			jsonObject.Add("type", jsonNode);
		}
		if (Format != null)
		{
			jsonObject.Add("format", Format);
		}
		if (Pattern != null)
		{
			jsonObject.Add("pattern", Pattern);
		}
		if (Constant != null)
		{
			jsonObject.Add("const", Constant);
		}
		if (Properties != null)
		{
			JsonObject jsonObject2 = new JsonObject();
			foreach (KeyValuePair<string, JsonSchema> property in Properties)
			{
				jsonObject2.Add(property.Key, property.Value.ToJsonNode(options));
			}
			jsonObject.Add("properties", jsonObject2);
		}
		if (Required != null)
		{
			JsonArray jsonArray = new JsonArray();
			foreach (string item in Required)
			{
				jsonArray.Add((JsonNode?)item);
			}
			jsonObject.Add("required", jsonArray);
		}
		if (Items != null)
		{
			jsonObject.Add("items", Items.ToJsonNode(options));
		}
		if (AdditionalProperties != null)
		{
			jsonObject.Add("additionalProperties", AdditionalProperties.ToJsonNode(options));
		}
		if (Enum != null)
		{
			jsonObject.Add("enum", Enum);
		}
		if (Not != null)
		{
			jsonObject.Add("not", Not.ToJsonNode(options));
		}
		if (AnyOf != null)
		{
			JsonArray jsonArray2 = new JsonArray();
			foreach (JsonSchema item2 in AnyOf)
			{
				jsonArray2.Add(item2.ToJsonNode(options));
			}
			jsonObject.Add("anyOf", jsonArray2);
		}
		if (HasDefaultValue)
		{
			jsonObject.Add("default", DefaultValue);
		}
		int? minLength = MinLength;
		if (minLength.HasValue)
		{
			int valueOrDefault2 = minLength.GetValueOrDefault();
			jsonObject.Add("minLength", (JsonNode)valueOrDefault2);
		}
		minLength = MaxLength;
		if (minLength.HasValue)
		{
			int valueOrDefault3 = minLength.GetValueOrDefault();
			jsonObject.Add("maxLength", (JsonNode)valueOrDefault3);
		}
		return CompleteSchema(jsonObject);
		JsonNode CompleteSchema(JsonNode schema)
		{
			JsonSchemaExporterContext? exporterContext = ExporterContext;
			if (exporterContext.HasValue)
			{
				JsonSchemaExporterContext valueOrDefault4 = exporterContext.GetValueOrDefault();
				return options.TransformSchemaNode(valueOrDefault4, schema);
			}
			return schema;
		}
	}

	public static void EnsureMutable(ref JsonSchema schema)
	{
		bool? trueOrFalse = schema._trueOrFalse;
		if (trueOrFalse.HasValue)
		{
			if (trueOrFalse != true)
			{
				schema = new JsonSchema
				{
					Not = CreateTrueSchema()
				};
			}
			else
			{
				schema = new JsonSchema();
			}
		}
	}

	private void VerifyMutable()
	{
		bool? trueOrFalse = _trueOrFalse;
		if (trueOrFalse.HasValue)
		{
			Throw();
		}
		static void Throw()
		{
			throw new InvalidOperationException();
		}
	}

	public static JsonNode MapSchemaType(JsonSchemaType schemaType)
	{
		if (schemaType == JsonSchemaType.Any)
		{
			return null;
		}
		string text = ToIdentifier(schemaType);
		if (text != null)
		{
			return text;
		}
		JsonArray jsonArray = new JsonArray();
		ReadOnlySpan<JsonSchemaType> readOnlySpan = s_schemaValues;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			JsonSchemaType jsonSchemaType = readOnlySpan[i];
			if ((schemaType & jsonSchemaType) != JsonSchemaType.Any)
			{
				jsonArray.Add((JsonNode?)ToIdentifier(jsonSchemaType));
			}
		}
		return jsonArray;
		static string ToIdentifier(JsonSchemaType jsonSchemaType2)
		{
			return jsonSchemaType2 switch
			{
				JsonSchemaType.Null => "null", 
				JsonSchemaType.Boolean => "boolean", 
				JsonSchemaType.Integer => "integer", 
				JsonSchemaType.Number => "number", 
				JsonSchemaType.String => "string", 
				JsonSchemaType.Array => "array", 
				JsonSchemaType.Object => "object", 
				_ => null, 
			};
		}
	}
}
