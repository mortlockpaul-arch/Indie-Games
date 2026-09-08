using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace System.Text.Json.Nodes;

public abstract class JsonValue : JsonNode
{
	internal const string CreateUnreferencedCodeMessage = "Creating JsonValue instances with non-primitive types is not compatible with trimming. It can result in non-primitive types being serialized, which may have their members trimmed.";

	internal const string CreateDynamicCodeMessage = "Creating JsonValue instances with non-primitive types requires generating code at runtime.";

	public static JsonValue Create(bool value, JsonNodeOptions? options = null)
	{
		return new JsonValuePrimitive<bool>(value, JsonMetadataServices.BooleanConverter, options);
	}

	public static JsonValue? Create(bool? value, JsonNodeOptions? options = null)
	{
		if (!value.HasValue)
		{
			return null;
		}
		return new JsonValuePrimitive<bool>(value.Value, JsonMetadataServices.BooleanConverter, options);
	}

	public static JsonValue Create(byte value, JsonNodeOptions? options = null)
	{
		return new JsonValuePrimitive<byte>(value, JsonMetadataServices.ByteConverter, options);
	}

	public static JsonValue? Create(byte? value, JsonNodeOptions? options = null)
	{
		if (!value.HasValue)
		{
			return null;
		}
		return new JsonValuePrimitive<byte>(value.Value, JsonMetadataServices.ByteConverter, options);
	}

	public static JsonValue Create(char value, JsonNodeOptions? options = null)
	{
		return new JsonValuePrimitive<char>(value, JsonMetadataServices.CharConverter, options);
	}

	public static JsonValue? Create(char? value, JsonNodeOptions? options = null)
	{
		if (!value.HasValue)
		{
			return null;
		}
		return new JsonValuePrimitive<char>(value.Value, JsonMetadataServices.CharConverter, options);
	}

	public static JsonValue Create(DateTime value, JsonNodeOptions? options = null)
	{
		return new JsonValuePrimitive<DateTime>(value, JsonMetadataServices.DateTimeConverter, options);
	}

	public static JsonValue? Create(DateTime? value, JsonNodeOptions? options = null)
	{
		if (!value.HasValue)
		{
			return null;
		}
		return new JsonValuePrimitive<DateTime>(value.Value, JsonMetadataServices.DateTimeConverter, options);
	}

	public static JsonValue Create(DateTimeOffset value, JsonNodeOptions? options = null)
	{
		return new JsonValuePrimitive<DateTimeOffset>(value, JsonMetadataServices.DateTimeOffsetConverter, options);
	}

	public static JsonValue? Create(DateTimeOffset? value, JsonNodeOptions? options = null)
	{
		if (!value.HasValue)
		{
			return null;
		}
		return new JsonValuePrimitive<DateTimeOffset>(value.Value, JsonMetadataServices.DateTimeOffsetConverter, options);
	}

	public static JsonValue Create(decimal value, JsonNodeOptions? options = null)
	{
		return new JsonValuePrimitive<decimal>(value, JsonMetadataServices.DecimalConverter, options);
	}

	public static JsonValue? Create(decimal? value, JsonNodeOptions? options = null)
	{
		if (!value.HasValue)
		{
			return null;
		}
		return new JsonValuePrimitive<decimal>(value.Value, JsonMetadataServices.DecimalConverter, options);
	}

	public static JsonValue Create(double value, JsonNodeOptions? options = null)
	{
		return new JsonValuePrimitive<double>(value, JsonMetadataServices.DoubleConverter, options);
	}

	public static JsonValue? Create(double? value, JsonNodeOptions? options = null)
	{
		if (!value.HasValue)
		{
			return null;
		}
		return new JsonValuePrimitive<double>(value.Value, JsonMetadataServices.DoubleConverter, options);
	}

	public static JsonValue Create(Guid value, JsonNodeOptions? options = null)
	{
		return new JsonValuePrimitive<Guid>(value, JsonMetadataServices.GuidConverter, options);
	}

	public static JsonValue? Create(Guid? value, JsonNodeOptions? options = null)
	{
		if (!value.HasValue)
		{
			return null;
		}
		return new JsonValuePrimitive<Guid>(value.Value, JsonMetadataServices.GuidConverter, options);
	}

	public static JsonValue Create(short value, JsonNodeOptions? options = null)
	{
		return new JsonValuePrimitive<short>(value, JsonMetadataServices.Int16Converter, options);
	}

	public static JsonValue? Create(short? value, JsonNodeOptions? options = null)
	{
		if (!value.HasValue)
		{
			return null;
		}
		return new JsonValuePrimitive<short>(value.Value, JsonMetadataServices.Int16Converter, options);
	}

	public static JsonValue Create(int value, JsonNodeOptions? options = null)
	{
		return new JsonValuePrimitive<int>(value, JsonMetadataServices.Int32Converter, options);
	}

	public static JsonValue? Create(int? value, JsonNodeOptions? options = null)
	{
		if (!value.HasValue)
		{
			return null;
		}
		return new JsonValuePrimitive<int>(value.Value, JsonMetadataServices.Int32Converter, options);
	}

	public static JsonValue Create(long value, JsonNodeOptions? options = null)
	{
		return new JsonValuePrimitive<long>(value, JsonMetadataServices.Int64Converter, options);
	}

	public static JsonValue? Create(long? value, JsonNodeOptions? options = null)
	{
		if (!value.HasValue)
		{
			return null;
		}
		return new JsonValuePrimitive<long>(value.Value, JsonMetadataServices.Int64Converter, options);
	}

	[CLSCompliant(false)]
	public static JsonValue Create(sbyte value, JsonNodeOptions? options = null)
	{
		return new JsonValuePrimitive<sbyte>(value, JsonMetadataServices.SByteConverter, options);
	}

	[CLSCompliant(false)]
	public static JsonValue? Create(sbyte? value, JsonNodeOptions? options = null)
	{
		if (!value.HasValue)
		{
			return null;
		}
		return new JsonValuePrimitive<sbyte>(value.Value, JsonMetadataServices.SByteConverter, options);
	}

	public static JsonValue Create(float value, JsonNodeOptions? options = null)
	{
		return new JsonValuePrimitive<float>(value, JsonMetadataServices.SingleConverter, options);
	}

	public static JsonValue? Create(float? value, JsonNodeOptions? options = null)
	{
		if (!value.HasValue)
		{
			return null;
		}
		return new JsonValuePrimitive<float>(value.Value, JsonMetadataServices.SingleConverter, options);
	}

	[return: NotNullIfNotNull("value")]
	public static JsonValue? Create(string? value, JsonNodeOptions? options = null)
	{
		if (value == null)
		{
			return null;
		}
		return new JsonValuePrimitive<string>(value, JsonMetadataServices.StringConverter, options);
	}

	[CLSCompliant(false)]
	public static JsonValue Create(ushort value, JsonNodeOptions? options = null)
	{
		return new JsonValuePrimitive<ushort>(value, JsonMetadataServices.UInt16Converter, options);
	}

	[CLSCompliant(false)]
	public static JsonValue? Create(ushort? value, JsonNodeOptions? options = null)
	{
		if (!value.HasValue)
		{
			return null;
		}
		return new JsonValuePrimitive<ushort>(value.Value, JsonMetadataServices.UInt16Converter, options);
	}

	[CLSCompliant(false)]
	public static JsonValue Create(uint value, JsonNodeOptions? options = null)
	{
		return new JsonValuePrimitive<uint>(value, JsonMetadataServices.UInt32Converter, options);
	}

	[CLSCompliant(false)]
	public static JsonValue? Create(uint? value, JsonNodeOptions? options = null)
	{
		if (!value.HasValue)
		{
			return null;
		}
		return new JsonValuePrimitive<uint>(value.Value, JsonMetadataServices.UInt32Converter, options);
	}

	[CLSCompliant(false)]
	public static JsonValue Create(ulong value, JsonNodeOptions? options = null)
	{
		return new JsonValuePrimitive<ulong>(value, JsonMetadataServices.UInt64Converter, options);
	}

	[CLSCompliant(false)]
	public static JsonValue? Create(ulong? value, JsonNodeOptions? options = null)
	{
		if (!value.HasValue)
		{
			return null;
		}
		return new JsonValuePrimitive<ulong>(value.Value, JsonMetadataServices.UInt64Converter, options);
	}

	public static JsonValue? Create(JsonElement value, JsonNodeOptions? options = null)
	{
		return CreateFromElement(in value, options);
	}

	public static JsonValue? Create(JsonElement? value, JsonNodeOptions? options = null)
	{
		if (value.HasValue)
		{
			JsonElement element = value.GetValueOrDefault();
			return CreateFromElement(in element, options);
		}
		return null;
	}

	private protected JsonValue(JsonNodeOptions? options)
		: base(options)
	{
	}

	public abstract bool TryGetValue<T>([NotNullWhen(true)] out T? value);

	[RequiresUnreferencedCode("Creating JsonValue instances with non-primitive types is not compatible with trimming. It can result in non-primitive types being serialized, which may have their members trimmed. Use the overload that takes a JsonTypeInfo, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("Creating JsonValue instances with non-primitive types requires generating code at runtime.")]
	public static JsonValue? Create<T>(T? value, JsonNodeOptions? options = null)
	{
		if (value == null)
		{
			return null;
		}
		if (value is JsonNode)
		{
			ThrowHelper.ThrowArgumentException_NodeValueNotAllowed("value");
		}
		if (value is JsonElement)
		{
			JsonElement element = (JsonElement)((((object)value) is JsonElement) ? ((object)value) : null);
			return CreateFromElement(in element, options);
		}
		JsonTypeInfo<T> jsonTypeInfo = (JsonTypeInfo<T>)JsonSerializerOptions.Default.GetTypeInfo(typeof(T));
		return CreateFromTypeInfo(value, jsonTypeInfo, options);
	}

	public static JsonValue? Create<T>(T? value, JsonTypeInfo<T> jsonTypeInfo, JsonNodeOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		if (value == null)
		{
			return null;
		}
		if (value is JsonNode)
		{
			ThrowHelper.ThrowArgumentException_NodeValueNotAllowed("value");
		}
		jsonTypeInfo.EnsureConfigured();
		if (value is JsonElement)
		{
			JsonElement element = (JsonElement)((((object)value) is JsonElement) ? ((object)value) : null);
			if (jsonTypeInfo.EffectiveConverter.IsInternalConverter)
			{
				return CreateFromElement(in element, options);
			}
		}
		return CreateFromTypeInfo(value, jsonTypeInfo, options);
	}

	internal override bool DeepEqualsCore(JsonNode otherNode)
	{
		if (GetValueKind() != otherNode.GetValueKind())
		{
			return false;
		}
		JsonElement element = ToJsonElement(this, out var backingDocument);
		JsonElement element2 = ToJsonElement(otherNode, out var backingDocument2);
		try
		{
			return JsonElement.DeepEquals(element, element2);
		}
		finally
		{
			backingDocument?.Dispose();
			backingDocument2?.Dispose();
		}
		static JsonElement ToJsonElement(JsonNode node, out JsonDocument reference)
		{
			JsonElement? underlyingElement = node.UnderlyingElement;
			if (underlyingElement.HasValue)
			{
				JsonElement valueOrDefault = underlyingElement.GetValueOrDefault();
				reference = null;
				return valueOrDefault;
			}
			Utf8JsonWriter utf8JsonWriter = Utf8JsonWriterCache.RentWriterAndBuffer(default(JsonWriterOptions), 16384, out var bufferWriter);
			try
			{
				node.WriteTo(utf8JsonWriter);
				utf8JsonWriter.Flush();
				Utf8JsonReader reader = new Utf8JsonReader(bufferWriter.WrittenSpan);
				reference = JsonDocument.ParseValue(ref reader);
				return reference.RootElement;
			}
			finally
			{
				Utf8JsonWriterCache.ReturnWriterAndBuffer(utf8JsonWriter, bufferWriter);
			}
		}
	}

	internal sealed override void GetPath(ref System.Text.ValueStringBuilder path, JsonNode child)
	{
		base.Parent?.GetPath(ref path, this);
	}

	internal static JsonValue CreateFromTypeInfo<T>(T value, JsonTypeInfo<T> jsonTypeInfo, JsonNodeOptions? options = null)
	{
		if (JsonValue<T>.TypeIsSupportedPrimitive && jsonTypeInfo != null)
		{
			JsonConverter<T> effectiveConverter = jsonTypeInfo.EffectiveConverter;
			if (effectiveConverter != null && effectiveConverter.IsInternalConverter && (jsonTypeInfo.EffectiveNumberHandling & JsonNumberHandling.WriteAsString) == 0)
			{
				return new JsonValuePrimitive<T>(value, jsonTypeInfo.EffectiveConverter, options);
			}
		}
		return new JsonValueCustomized<T>(value, jsonTypeInfo, options);
	}

	internal static JsonValue CreateFromElement(ref readonly JsonElement element, JsonNodeOptions? options = null)
	{
		switch (element.ValueKind)
		{
		case JsonValueKind.Null:
			return null;
		case JsonValueKind.Object:
		case JsonValueKind.Array:
			ThrowHelper.ThrowInvalidOperationException_NodeElementCannotBeObjectOrArray();
			return null;
		default:
			return new JsonValueOfElement(element, options);
		}
	}
}
[DebuggerDisplay("{ToJsonString(),nq}")]
[DebuggerTypeProxy(typeof(JsonValue<>.DebugView))]
internal abstract class JsonValue<TValue> : JsonValue
{
	[ExcludeFromCodeCoverage]
	[DebuggerDisplay("{Json,nq}")]
	private sealed class DebugView
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public JsonValue<TValue> _node;

		public string Json => _node.ToJsonString();

		public string Path => _node.GetPath();

		public TValue Value => _node.Value;

		public DebugView(JsonValue<TValue> node)
		{
			_node = node;
		}
	}

	internal readonly TValue Value;

	private static readonly JsonValueKind? s_valueKind = DetermineValueKindForType(typeof(TValue));

	internal static bool TypeIsSupportedPrimitive => s_valueKind.HasValue;

	protected JsonValue(TValue value, JsonNodeOptions? options)
		: base(options)
	{
		Value = value;
	}

	public override T GetValue<T>()
	{
		TValue value = Value;
		if (value is T)
		{
			return (T)((((object)value) is T) ? ((object)value) : null);
		}
		ThrowHelper.ThrowInvalidOperationException_NodeUnableToConvert(typeof(TValue), typeof(T));
		return default(T);
	}

	public override bool TryGetValue<T>([NotNullWhen(true)] out T value)
	{
		TValue value2 = Value;
		if (value2 is T val)
		{
			value = val;
			return true;
		}
		value = default(T);
		return false;
	}

	private protected static JsonValueKind DetermineValueKind(TValue value)
	{
		if (value is bool)
		{
			if (!(bool)((((object)value) is bool) ? ((object)value) : null))
			{
				return JsonValueKind.False;
			}
			return JsonValueKind.True;
		}
		return s_valueKind.Value;
	}

	private static JsonValueKind? DetermineValueKindForType(Type type)
	{
		if (type.IsEnum)
		{
			return null;
		}
		Type underlyingType = Nullable.GetUnderlyingType(type);
		if ((object)underlyingType != null)
		{
			return DetermineValueKindForType(underlyingType);
		}
		if (type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(TimeSpan) || type == typeof(DateOnly) || type == typeof(TimeOnly) || type == typeof(Guid) || type == typeof(Uri) || type == typeof(Version))
		{
			return JsonValueKind.String;
		}
		if (type == typeof(Half) || type == typeof(UInt128) || type == typeof(Int128))
		{
			return JsonValueKind.Number;
		}
		return Type.GetTypeCode(type) switch
		{
			TypeCode.Boolean => JsonValueKind.Undefined, 
			TypeCode.SByte => JsonValueKind.Number, 
			TypeCode.Byte => JsonValueKind.Number, 
			TypeCode.Int16 => JsonValueKind.Number, 
			TypeCode.UInt16 => JsonValueKind.Number, 
			TypeCode.Int32 => JsonValueKind.Number, 
			TypeCode.UInt32 => JsonValueKind.Number, 
			TypeCode.Int64 => JsonValueKind.Number, 
			TypeCode.UInt64 => JsonValueKind.Number, 
			TypeCode.Single => JsonValueKind.Number, 
			TypeCode.Double => JsonValueKind.Number, 
			TypeCode.Decimal => JsonValueKind.Number, 
			TypeCode.String => JsonValueKind.String, 
			TypeCode.Char => JsonValueKind.String, 
			_ => null, 
		};
	}
}
