using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json.Serialization.Converters;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace System.Text.Json.Nodes;

public abstract class JsonNode
{
	private protected static readonly JsonSerializerOptions s_defaultOptions = new JsonSerializerOptions();

	private JsonNode _parent;

	private JsonNodeOptions? _options;

	internal virtual JsonElement? UnderlyingElement => null;

	public JsonNodeOptions? Options
	{
		get
		{
			if (!_options.HasValue && Parent != null)
			{
				_options = Parent.Options;
			}
			return _options;
		}
	}

	public JsonNode? Parent
	{
		get
		{
			return _parent;
		}
		internal set
		{
			_parent = value;
		}
	}

	public JsonNode Root
	{
		get
		{
			JsonNode parent = Parent;
			if (parent == null)
			{
				return this;
			}
			while (parent.Parent != null)
			{
				parent = parent.Parent;
			}
			return parent;
		}
	}

	public JsonNode? this[int index]
	{
		get
		{
			return GetItem(index);
		}
		set
		{
			SetItem(index, value);
		}
	}

	public JsonNode? this[string propertyName]
	{
		get
		{
			return AsObject().GetItem(propertyName);
		}
		set
		{
			AsObject().SetItem(propertyName, value);
		}
	}

	internal JsonNode(JsonNodeOptions? options = null)
	{
		_options = options;
	}

	public JsonArray AsArray()
	{
		JsonArray obj = this as JsonArray;
		if (obj == null)
		{
			ThrowHelper.ThrowInvalidOperationException_NodeWrongType("JsonArray");
		}
		return obj;
	}

	public JsonObject AsObject()
	{
		JsonObject obj = this as JsonObject;
		if (obj == null)
		{
			ThrowHelper.ThrowInvalidOperationException_NodeWrongType("JsonObject");
		}
		return obj;
	}

	public JsonValue AsValue()
	{
		JsonValue obj = this as JsonValue;
		if (obj == null)
		{
			ThrowHelper.ThrowInvalidOperationException_NodeWrongType("JsonValue");
		}
		return obj;
	}

	public string GetPath()
	{
		if (Parent == null)
		{
			return "$";
		}
		Span<char> initialBuffer = stackalloc char[128];
		System.Text.ValueStringBuilder path = new System.Text.ValueStringBuilder(initialBuffer);
		path.Append('$');
		GetPath(ref path, null);
		return path.ToString();
	}

	internal abstract void GetPath(ref System.Text.ValueStringBuilder path, JsonNode child);

	public virtual T GetValue<T>()
	{
		throw new InvalidOperationException(System.SR.Format(System.SR.NodeWrongType, "JsonValue"));
	}

	private protected virtual JsonNode GetItem(int index)
	{
		global::_003C_003Ey__InlineArray2<string> buffer = default(global::_003C_003Ey__InlineArray2<string>);
		buffer[0] = "JsonArray";
		buffer[1] = "JsonObject";
		ThrowHelper.ThrowInvalidOperationException_NodeWrongType(buffer);
		return null;
	}

	private protected virtual void SetItem(int index, JsonNode node)
	{
		global::_003C_003Ey__InlineArray2<string> buffer = default(global::_003C_003Ey__InlineArray2<string>);
		buffer[0] = "JsonArray";
		buffer[1] = "JsonObject";
		ThrowHelper.ThrowInvalidOperationException_NodeWrongType(buffer);
	}

	public JsonNode DeepClone()
	{
		return DeepCloneCore();
	}

	internal abstract JsonNode DeepCloneCore();

	public JsonValueKind GetValueKind()
	{
		return GetValueKindCore();
	}

	private protected abstract JsonValueKind GetValueKindCore();

	public string GetPropertyName()
	{
		JsonObject obj = _parent as JsonObject;
		if (obj == null)
		{
			ThrowHelper.ThrowInvalidOperationException_NodeParentWrongType("JsonObject");
		}
		return obj.GetPropertyName(this);
	}

	public int GetElementIndex()
	{
		JsonArray obj = _parent as JsonArray;
		if (obj == null)
		{
			ThrowHelper.ThrowInvalidOperationException_NodeParentWrongType("JsonArray");
		}
		return obj.GetElementIndex(this);
	}

	public static bool DeepEquals(JsonNode? node1, JsonNode? node2)
	{
		if (node1 == null)
		{
			return node2 == null;
		}
		if (node2 == null)
		{
			return false;
		}
		return node1.DeepEqualsCore(node2);
	}

	internal abstract bool DeepEqualsCore(JsonNode node);

	[RequiresUnreferencedCode("Creating JsonValue instances with non-primitive types is not compatible with trimming. It can result in non-primitive types being serialized, which may have their members trimmed.")]
	[RequiresDynamicCode("Creating JsonValue instances with non-primitive types requires generating code at runtime.")]
	public void ReplaceWith<T>(T value)
	{
		JsonNode parent = _parent;
		if (!(parent is JsonObject jsonObject))
		{
			if (parent is JsonArray jsonArray)
			{
				JsonNode node = ConvertFromValue(value);
				jsonArray.SetItem(GetElementIndex(), node);
			}
		}
		else
		{
			JsonNode node = ConvertFromValue(value);
			jsonObject.SetItem(GetPropertyName(), node);
		}
	}

	internal void AssignParent(JsonNode parent)
	{
		if (Parent != null)
		{
			ThrowHelper.ThrowInvalidOperationException_NodeAlreadyHasParent();
		}
		for (JsonNode jsonNode = parent; jsonNode != null; jsonNode = jsonNode.Parent)
		{
			if (jsonNode == this)
			{
				ThrowHelper.ThrowInvalidOperationException_NodeCycleDetected();
			}
		}
		Parent = parent;
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	internal static JsonNode ConvertFromValue<T>(T value, JsonNodeOptions? options = null)
	{
		if (value == null)
		{
			return null;
		}
		if (value is JsonNode result)
		{
			return result;
		}
		if (value is JsonElement element)
		{
			return JsonNodeConverter.Create(element, options);
		}
		JsonTypeInfo<T> jsonTypeInfo = (JsonTypeInfo<T>)JsonSerializerOptions.Default.GetTypeInfo(typeof(T));
		return JsonValue.CreateFromTypeInfo(value, jsonTypeInfo, options);
	}

	public static implicit operator JsonNode(bool value)
	{
		return JsonValue.Create(value);
	}

	public static implicit operator JsonNode?(bool? value)
	{
		return JsonValue.Create(value);
	}

	public static implicit operator JsonNode(byte value)
	{
		return JsonValue.Create(value);
	}

	public static implicit operator JsonNode?(byte? value)
	{
		return JsonValue.Create(value);
	}

	public static implicit operator JsonNode(char value)
	{
		return JsonValue.Create(value);
	}

	public static implicit operator JsonNode?(char? value)
	{
		return JsonValue.Create(value);
	}

	public static implicit operator JsonNode(DateTime value)
	{
		return JsonValue.Create(value);
	}

	public static implicit operator JsonNode?(DateTime? value)
	{
		return JsonValue.Create(value);
	}

	public static implicit operator JsonNode(DateTimeOffset value)
	{
		return JsonValue.Create(value);
	}

	public static implicit operator JsonNode?(DateTimeOffset? value)
	{
		return JsonValue.Create(value);
	}

	public static implicit operator JsonNode(decimal value)
	{
		return JsonValue.Create(value);
	}

	public static implicit operator JsonNode?(decimal? value)
	{
		return JsonValue.Create(value);
	}

	public static implicit operator JsonNode(double value)
	{
		return JsonValue.Create(value);
	}

	public static implicit operator JsonNode?(double? value)
	{
		return JsonValue.Create(value);
	}

	public static implicit operator JsonNode(Guid value)
	{
		return JsonValue.Create(value);
	}

	public static implicit operator JsonNode?(Guid? value)
	{
		return JsonValue.Create(value);
	}

	public static implicit operator JsonNode(short value)
	{
		return JsonValue.Create(value);
	}

	public static implicit operator JsonNode?(short? value)
	{
		return JsonValue.Create(value);
	}

	public static implicit operator JsonNode(int value)
	{
		return JsonValue.Create(value);
	}

	public static implicit operator JsonNode?(int? value)
	{
		return JsonValue.Create(value);
	}

	public static implicit operator JsonNode(long value)
	{
		return JsonValue.Create(value);
	}

	public static implicit operator JsonNode?(long? value)
	{
		return JsonValue.Create(value);
	}

	[CLSCompliant(false)]
	public static implicit operator JsonNode(sbyte value)
	{
		return JsonValue.Create(value);
	}

	[CLSCompliant(false)]
	public static implicit operator JsonNode?(sbyte? value)
	{
		return JsonValue.Create(value);
	}

	public static implicit operator JsonNode(float value)
	{
		return JsonValue.Create(value);
	}

	public static implicit operator JsonNode?(float? value)
	{
		return JsonValue.Create(value);
	}

	[return: NotNullIfNotNull("value")]
	public static implicit operator JsonNode?(string? value)
	{
		return JsonValue.Create(value);
	}

	[CLSCompliant(false)]
	public static implicit operator JsonNode(ushort value)
	{
		return JsonValue.Create(value);
	}

	[CLSCompliant(false)]
	public static implicit operator JsonNode?(ushort? value)
	{
		return JsonValue.Create(value);
	}

	[CLSCompliant(false)]
	public static implicit operator JsonNode(uint value)
	{
		return JsonValue.Create(value);
	}

	[CLSCompliant(false)]
	public static implicit operator JsonNode?(uint? value)
	{
		return JsonValue.Create(value);
	}

	[CLSCompliant(false)]
	public static implicit operator JsonNode(ulong value)
	{
		return JsonValue.Create(value);
	}

	[CLSCompliant(false)]
	public static implicit operator JsonNode?(ulong? value)
	{
		return JsonValue.Create(value);
	}

	public static explicit operator bool(JsonNode value)
	{
		return value.GetValue<bool>();
	}

	public static explicit operator bool?(JsonNode? value)
	{
		return value?.GetValue<bool>();
	}

	public static explicit operator byte(JsonNode value)
	{
		return value.GetValue<byte>();
	}

	public static explicit operator byte?(JsonNode? value)
	{
		return value?.GetValue<byte>();
	}

	public static explicit operator char(JsonNode value)
	{
		return value.GetValue<char>();
	}

	public static explicit operator char?(JsonNode? value)
	{
		return value?.GetValue<char>();
	}

	public static explicit operator DateTime(JsonNode value)
	{
		return value.GetValue<DateTime>();
	}

	public static explicit operator DateTime?(JsonNode? value)
	{
		return value?.GetValue<DateTime>();
	}

	public static explicit operator DateTimeOffset(JsonNode value)
	{
		return value.GetValue<DateTimeOffset>();
	}

	public static explicit operator DateTimeOffset?(JsonNode? value)
	{
		return value?.GetValue<DateTimeOffset>();
	}

	public static explicit operator decimal(JsonNode value)
	{
		return value.GetValue<decimal>();
	}

	public static explicit operator decimal?(JsonNode? value)
	{
		return value?.GetValue<decimal>();
	}

	public static explicit operator double(JsonNode value)
	{
		return value.GetValue<double>();
	}

	public static explicit operator double?(JsonNode? value)
	{
		return value?.GetValue<double>();
	}

	public static explicit operator Guid(JsonNode value)
	{
		return value.GetValue<Guid>();
	}

	public static explicit operator Guid?(JsonNode? value)
	{
		return value?.GetValue<Guid>();
	}

	public static explicit operator short(JsonNode value)
	{
		return value.GetValue<short>();
	}

	public static explicit operator short?(JsonNode? value)
	{
		return value?.GetValue<short>();
	}

	public static explicit operator int(JsonNode value)
	{
		return value.GetValue<int>();
	}

	public static explicit operator int?(JsonNode? value)
	{
		return value?.GetValue<int>();
	}

	public static explicit operator long(JsonNode value)
	{
		return value.GetValue<long>();
	}

	public static explicit operator long?(JsonNode? value)
	{
		return value?.GetValue<long>();
	}

	[CLSCompliant(false)]
	public static explicit operator sbyte(JsonNode value)
	{
		return value.GetValue<sbyte>();
	}

	[CLSCompliant(false)]
	public static explicit operator sbyte?(JsonNode? value)
	{
		return value?.GetValue<sbyte>();
	}

	public static explicit operator float(JsonNode value)
	{
		return value.GetValue<float>();
	}

	public static explicit operator float?(JsonNode? value)
	{
		return value?.GetValue<float>();
	}

	public static explicit operator string?(JsonNode? value)
	{
		return value?.GetValue<string>();
	}

	[CLSCompliant(false)]
	public static explicit operator ushort(JsonNode value)
	{
		return value.GetValue<ushort>();
	}

	[CLSCompliant(false)]
	public static explicit operator ushort?(JsonNode? value)
	{
		return value?.GetValue<ushort>();
	}

	[CLSCompliant(false)]
	public static explicit operator uint(JsonNode value)
	{
		return value.GetValue<uint>();
	}

	[CLSCompliant(false)]
	public static explicit operator uint?(JsonNode? value)
	{
		return value?.GetValue<uint>();
	}

	[CLSCompliant(false)]
	public static explicit operator ulong(JsonNode value)
	{
		return value.GetValue<ulong>();
	}

	[CLSCompliant(false)]
	public static explicit operator ulong?(JsonNode? value)
	{
		return value?.GetValue<ulong>();
	}

	public static JsonNode? Parse(ref Utf8JsonReader reader, JsonNodeOptions? nodeOptions = null)
	{
		return JsonNodeConverter.Create(JsonElement.ParseValue(ref reader), nodeOptions);
	}

	public static JsonNode? Parse([StringSyntax("Json")] string json, JsonNodeOptions? nodeOptions = null, JsonDocumentOptions documentOptions = default(JsonDocumentOptions))
	{
		ArgumentNullException.ThrowIfNull(json, "json");
		return JsonNodeConverter.Create(JsonElement.Parse(json, documentOptions), nodeOptions);
	}

	public static JsonNode? Parse(ReadOnlySpan<byte> utf8Json, JsonNodeOptions? nodeOptions = null, JsonDocumentOptions documentOptions = default(JsonDocumentOptions))
	{
		return JsonNodeConverter.Create(JsonElement.Parse(utf8Json, documentOptions), nodeOptions);
	}

	public static JsonNode? Parse(Stream utf8Json, JsonNodeOptions? nodeOptions = null, JsonDocumentOptions documentOptions = default(JsonDocumentOptions))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		return JsonNodeConverter.Create(JsonElement.ParseValue(utf8Json, documentOptions), nodeOptions);
	}

	public static async Task<JsonNode?> ParseAsync(Stream utf8Json, JsonNodeOptions? nodeOptions = null, JsonDocumentOptions documentOptions = default(JsonDocumentOptions), CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		return JsonNodeConverter.Create((await JsonDocument.ParseAsyncCoreUnrented(utf8Json, documentOptions, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)).RootElement, nodeOptions);
	}

	public string ToJsonString(JsonSerializerOptions? options = null)
	{
		JsonWriterOptions options2 = default(JsonWriterOptions);
		int defaultBufferSize = 16384;
		if (options != null)
		{
			options2 = options.GetWriterOptions();
			defaultBufferSize = options.DefaultBufferSize;
		}
		Utf8JsonWriter utf8JsonWriter = Utf8JsonWriterCache.RentWriterAndBuffer(options2, defaultBufferSize, out var bufferWriter);
		try
		{
			WriteTo(utf8JsonWriter, options);
			utf8JsonWriter.Flush();
			return JsonHelpers.Utf8GetString(bufferWriter.WrittenSpan);
		}
		finally
		{
			Utf8JsonWriterCache.ReturnWriterAndBuffer(utf8JsonWriter, bufferWriter);
		}
	}

	public override string ToString()
	{
		if (this is JsonValue)
		{
			if (this is JsonValuePrimitive<string> jsonValuePrimitive)
			{
				return jsonValuePrimitive.Value;
			}
			if (this is JsonValueOfElement { Value: var value } jsonValueOfElement)
			{
				if (value.ValueKind == JsonValueKind.String)
				{
					return jsonValueOfElement.Value.GetString();
				}
			}
			else if (this is JsonValueOfJsonString jsonValueOfJsonString)
			{
				return jsonValueOfJsonString.GetValue<string>();
			}
		}
		Utf8JsonWriter utf8JsonWriter = Utf8JsonWriterCache.RentWriterAndBuffer(new JsonWriterOptions
		{
			Indented = true
		}, 16384, out var bufferWriter);
		try
		{
			WriteTo(utf8JsonWriter);
			utf8JsonWriter.Flush();
			return JsonHelpers.Utf8GetString(bufferWriter.WrittenSpan);
		}
		finally
		{
			Utf8JsonWriterCache.ReturnWriterAndBuffer(utf8JsonWriter, bufferWriter);
		}
	}

	public abstract void WriteTo(Utf8JsonWriter writer, JsonSerializerOptions? options = null);
}
