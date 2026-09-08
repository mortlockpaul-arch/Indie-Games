using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Converters;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace System.Text.Json;

public static class JsonSerializer
{
	private static readonly byte[] s_idPropertyName = "$id"u8.ToArray();

	private static readonly byte[] s_refPropertyName = "$ref"u8.ToArray();

	private static readonly byte[] s_typePropertyName = "$type"u8.ToArray();

	private static readonly byte[] s_valuesPropertyName = "$values"u8.ToArray();

	internal static readonly JsonEncodedText s_metadataId = JsonEncodedText.Encode("$id");

	internal static readonly JsonEncodedText s_metadataRef = JsonEncodedText.Encode("$ref");

	internal static readonly JsonEncodedText s_metadataType = JsonEncodedText.Encode("$type");

	internal static readonly JsonEncodedText s_metadataValues = JsonEncodedText.Encode("$values");

	[FeatureSwitchDefinition("System.Text.Json.JsonSerializer.IsReflectionEnabledByDefault")]
	public static bool IsReflectionEnabledByDefault { get; } = !AppContext.TryGetSwitch("System.Text.Json.JsonSerializer.IsReflectionEnabledByDefault", out var isEnabled) || isEnabled;

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static TValue? Deserialize<TValue>(this JsonDocument document, JsonSerializerOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(document, "document");
		JsonTypeInfo<TValue> typeInfo = GetTypeInfo<TValue>(options);
		return ReadFromSpan(document.GetRootRawValue().Span, typeInfo);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static object? Deserialize(this JsonDocument document, Type returnType, JsonSerializerOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(document, "document");
		ArgumentNullException.ThrowIfNull(returnType, "returnType");
		JsonTypeInfo typeInfo = GetTypeInfo(options, returnType);
		return ReadFromSpanAsObject(document.GetRootRawValue().Span, typeInfo);
	}

	public static TValue? Deserialize<TValue>(this JsonDocument document, JsonTypeInfo<TValue> jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(document, "document");
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return ReadFromSpan(document.GetRootRawValue().Span, jsonTypeInfo);
	}

	public static object? Deserialize(this JsonDocument document, JsonTypeInfo jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(document, "document");
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return ReadFromSpanAsObject(document.GetRootRawValue().Span, jsonTypeInfo);
	}

	public static object? Deserialize(this JsonDocument document, Type returnType, JsonSerializerContext context)
	{
		ArgumentNullException.ThrowIfNull(document, "document");
		ArgumentNullException.ThrowIfNull(returnType, "returnType");
		ArgumentNullException.ThrowIfNull(context, "context");
		JsonTypeInfo typeInfo = GetTypeInfo(context, returnType);
		return ReadFromSpanAsObject(document.GetRootRawValue().Span, typeInfo);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static TValue? Deserialize<TValue>(this JsonElement element, JsonSerializerOptions? options = null)
	{
		JsonTypeInfo<TValue> typeInfo = GetTypeInfo<TValue>(options);
		return ReadFromSpan(element.GetRawValue().Span, typeInfo);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static object? Deserialize(this JsonElement element, Type returnType, JsonSerializerOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(returnType, "returnType");
		JsonTypeInfo typeInfo = GetTypeInfo(options, returnType);
		return ReadFromSpanAsObject(element.GetRawValue().Span, typeInfo);
	}

	public static TValue? Deserialize<TValue>(this JsonElement element, JsonTypeInfo<TValue> jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return ReadFromSpan(element.GetRawValue().Span, jsonTypeInfo);
	}

	public static object? Deserialize(this JsonElement element, JsonTypeInfo jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return ReadFromSpanAsObject(element.GetRawValue().Span, jsonTypeInfo);
	}

	public static object? Deserialize(this JsonElement element, Type returnType, JsonSerializerContext context)
	{
		ArgumentNullException.ThrowIfNull(returnType, "returnType");
		ArgumentNullException.ThrowIfNull(context, "context");
		JsonTypeInfo typeInfo = GetTypeInfo(context, returnType);
		return ReadFromSpanAsObject(element.GetRawValue().Span, typeInfo);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static TValue? Deserialize<TValue>(this JsonNode? node, JsonSerializerOptions? options = null)
	{
		JsonTypeInfo<TValue> typeInfo = GetTypeInfo<TValue>(options);
		return ReadFromNode(node, typeInfo);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static object? Deserialize(this JsonNode? node, Type returnType, JsonSerializerOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(returnType, "returnType");
		JsonTypeInfo typeInfo = GetTypeInfo(options, returnType);
		return ReadFromNodeAsObject(node, typeInfo);
	}

	public static TValue? Deserialize<TValue>(this JsonNode? node, JsonTypeInfo<TValue> jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return ReadFromNode(node, jsonTypeInfo);
	}

	public static object? Deserialize(this JsonNode? node, JsonTypeInfo jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return ReadFromNodeAsObject(node, jsonTypeInfo);
	}

	public static object? Deserialize(this JsonNode? node, Type returnType, JsonSerializerContext context)
	{
		ArgumentNullException.ThrowIfNull(returnType, "returnType");
		ArgumentNullException.ThrowIfNull(context, "context");
		JsonTypeInfo typeInfo = GetTypeInfo(context, returnType);
		return ReadFromNodeAsObject(node, typeInfo);
	}

	private static TValue ReadFromNode<TValue>(JsonNode node, JsonTypeInfo<TValue> jsonTypeInfo)
	{
		JsonSerializerOptions options = jsonTypeInfo.Options;
		using PooledByteBufferWriter pooledByteBufferWriter = new PooledByteBufferWriter(options.DefaultBufferSize);
		using (Utf8JsonWriter utf8JsonWriter = new Utf8JsonWriter(pooledByteBufferWriter, options.GetWriterOptions()))
		{
			if (node == null)
			{
				utf8JsonWriter.WriteNullValue();
			}
			else
			{
				node.WriteTo(utf8JsonWriter, options);
			}
		}
		return ReadFromSpan(pooledByteBufferWriter.WrittenSpan, jsonTypeInfo);
	}

	private static object ReadFromNodeAsObject(JsonNode node, JsonTypeInfo jsonTypeInfo)
	{
		JsonSerializerOptions options = jsonTypeInfo.Options;
		using PooledByteBufferWriter pooledByteBufferWriter = new PooledByteBufferWriter(options.DefaultBufferSize);
		using (Utf8JsonWriter utf8JsonWriter = new Utf8JsonWriter(pooledByteBufferWriter, options.GetWriterOptions()))
		{
			if (node == null)
			{
				utf8JsonWriter.WriteNullValue();
			}
			else
			{
				node.WriteTo(utf8JsonWriter, options);
			}
		}
		return ReadFromSpanAsObject(pooledByteBufferWriter.WrittenSpan, jsonTypeInfo);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static ValueTask<TValue?> DeserializeAsync<TValue>(PipeReader utf8Json, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		return GetTypeInfo<TValue>(options).DeserializeAsync(utf8Json, cancellationToken);
	}

	public static ValueTask<TValue?> DeserializeAsync<TValue>(PipeReader utf8Json, JsonTypeInfo<TValue> jsonTypeInfo, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return jsonTypeInfo.DeserializeAsync(utf8Json, cancellationToken);
	}

	public static ValueTask<object?> DeserializeAsync(PipeReader utf8Json, JsonTypeInfo jsonTypeInfo, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return jsonTypeInfo.DeserializeAsObjectAsync(utf8Json, cancellationToken);
	}

	public static ValueTask<object?> DeserializeAsync(PipeReader utf8Json, Type returnType, JsonSerializerContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArgumentNullException.ThrowIfNull(returnType, "returnType");
		ArgumentNullException.ThrowIfNull(context, "context");
		return GetTypeInfo(context, returnType).DeserializeAsObjectAsync(utf8Json, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static ValueTask<object?> DeserializeAsync(PipeReader utf8Json, Type returnType, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArgumentNullException.ThrowIfNull(returnType, "returnType");
		return GetTypeInfo(options, returnType).DeserializeAsObjectAsync(utf8Json, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static IAsyncEnumerable<TValue?> DeserializeAsyncEnumerable<TValue>(PipeReader utf8Json, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		return DeserializeAsyncEnumerable<TValue>(utf8Json, topLevelValues: false, options, cancellationToken);
	}

	public static IAsyncEnumerable<TValue?> DeserializeAsyncEnumerable<TValue>(PipeReader utf8Json, JsonTypeInfo<TValue> jsonTypeInfo, CancellationToken cancellationToken = default(CancellationToken))
	{
		return DeserializeAsyncEnumerable(utf8Json, jsonTypeInfo, topLevelValues: false, cancellationToken);
	}

	public static IAsyncEnumerable<TValue?> DeserializeAsyncEnumerable<TValue>(PipeReader utf8Json, JsonTypeInfo<TValue> jsonTypeInfo, bool topLevelValues, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return DeserializeAsyncEnumerableCore(utf8Json, jsonTypeInfo, topLevelValues, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static IAsyncEnumerable<TValue?> DeserializeAsyncEnumerable<TValue>(PipeReader utf8Json, bool topLevelValues, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		JsonTypeInfo<TValue> typeInfo = GetTypeInfo<TValue>(options);
		return DeserializeAsyncEnumerableCore(utf8Json, typeInfo, topLevelValues, cancellationToken);
	}

	private static IAsyncEnumerable<T> DeserializeAsyncEnumerableCore<T>(PipeReader utf8Json, JsonTypeInfo<T> jsonTypeInfo, bool topLevelValues, CancellationToken cancellationToken)
	{
		JsonReaderOptions readerOptions = jsonTypeInfo.Options.GetReaderOptions();
		JsonTypeInfo<List<T>> listTypeInfo;
		if (topLevelValues)
		{
			listTypeInfo = GetOrAddListTypeInfoForRootLevelValueMode(jsonTypeInfo);
			readerOptions.AllowMultipleValues = true;
		}
		else
		{
			listTypeInfo = GetOrAddListTypeInfoForArrayMode(jsonTypeInfo);
		}
		return CreateAsyncEnumerableFromArray(utf8Json, listTypeInfo, readerOptions, cancellationToken);
		static async IAsyncEnumerable<T> CreateAsyncEnumerableFromArray(PipeReader utf8Json2, JsonTypeInfo<List<T>> jsonTypeInfo2, JsonReaderOptions options, [EnumeratorCancellation] CancellationToken cancellationToken2)
		{
			ReadStack readStack = default(ReadStack);
			readStack.Initialize(jsonTypeInfo2, supportContinuation: true);
			JsonReaderState jsonReaderState = new JsonReaderState(options);
			PipeReadBufferState bufferState = new PipeReadBufferState(utf8Json2);
			try
			{
				bool success;
				do
				{
					bufferState = await bufferState.ReadAsync(utf8Json2, cancellationToken2, fillBuffer: false).ConfigureAwait(continueOnCapturedContext: false);
					success = jsonTypeInfo2.ContinueDeserialize<PipeReadBufferState, PipeReader>(ref bufferState, ref jsonReaderState, ref readStack, out var _);
					object returnValue = readStack.Current.ReturnValue;
					if (returnValue != null)
					{
						List<T> list = (List<T>)returnValue;
						foreach (T item in list)
						{
							yield return item;
						}
						list.Clear();
					}
				}
				while (!success);
			}
			finally
			{
				bufferState.Dispose();
			}
		}
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static JsonDocument SerializeToDocument<TValue>(TValue value, JsonSerializerOptions? options = null)
	{
		JsonTypeInfo<TValue> typeInfo = GetTypeInfo<TValue>(options);
		return WriteDocument<TValue>(in value, typeInfo);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static JsonDocument SerializeToDocument(object? value, Type inputType, JsonSerializerOptions? options = null)
	{
		ValidateInputType(value, inputType);
		JsonTypeInfo typeInfo = GetTypeInfo(options, inputType);
		return WriteDocumentAsObject(value, typeInfo);
	}

	public static JsonDocument SerializeToDocument<TValue>(TValue value, JsonTypeInfo<TValue> jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return WriteDocument(in value, jsonTypeInfo);
	}

	public static JsonDocument SerializeToDocument(object? value, JsonTypeInfo jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return WriteDocumentAsObject(value, jsonTypeInfo);
	}

	public static JsonDocument SerializeToDocument(object? value, Type inputType, JsonSerializerContext context)
	{
		ArgumentNullException.ThrowIfNull(context, "context");
		ValidateInputType(value, inputType);
		return WriteDocumentAsObject(value, GetTypeInfo(context, inputType));
	}

	private static JsonDocument WriteDocument<TValue>(in TValue value, JsonTypeInfo<TValue> jsonTypeInfo)
	{
		JsonSerializerOptions options = jsonTypeInfo.Options;
		PooledByteBufferWriter pooledByteBufferWriter = new PooledByteBufferWriter(options.DefaultBufferSize);
		Utf8JsonWriter writer = Utf8JsonWriterCache.RentWriter(options, pooledByteBufferWriter);
		try
		{
			jsonTypeInfo.Serialize(writer, in value);
			return JsonDocument.ParseRented(pooledByteBufferWriter, options.GetDocumentOptions());
		}
		finally
		{
			Utf8JsonWriterCache.ReturnWriter(writer);
		}
	}

	private static JsonDocument WriteDocumentAsObject(object value, JsonTypeInfo jsonTypeInfo)
	{
		JsonSerializerOptions options = jsonTypeInfo.Options;
		PooledByteBufferWriter pooledByteBufferWriter = new PooledByteBufferWriter(options.DefaultBufferSize);
		Utf8JsonWriter writer = Utf8JsonWriterCache.RentWriter(options, pooledByteBufferWriter);
		try
		{
			jsonTypeInfo.SerializeAsObject(writer, value);
			return JsonDocument.ParseRented(pooledByteBufferWriter, options.GetDocumentOptions());
		}
		finally
		{
			Utf8JsonWriterCache.ReturnWriter(writer);
		}
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static JsonElement SerializeToElement<TValue>(TValue value, JsonSerializerOptions? options = null)
	{
		JsonTypeInfo<TValue> typeInfo = GetTypeInfo<TValue>(options);
		return WriteElement<TValue>(in value, typeInfo);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static JsonElement SerializeToElement(object? value, Type inputType, JsonSerializerOptions? options = null)
	{
		ValidateInputType(value, inputType);
		JsonTypeInfo typeInfo = GetTypeInfo(options, inputType);
		return WriteElementAsObject(value, typeInfo);
	}

	public static JsonElement SerializeToElement<TValue>(TValue value, JsonTypeInfo<TValue> jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return WriteElement(in value, jsonTypeInfo);
	}

	public static JsonElement SerializeToElement(object? value, JsonTypeInfo jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return WriteElementAsObject(value, jsonTypeInfo);
	}

	public static JsonElement SerializeToElement(object? value, Type inputType, JsonSerializerContext context)
	{
		ArgumentNullException.ThrowIfNull(context, "context");
		ValidateInputType(value, inputType);
		JsonTypeInfo typeInfo = GetTypeInfo(context, inputType);
		return WriteElementAsObject(value, typeInfo);
	}

	private static JsonElement WriteElement<TValue>(in TValue value, JsonTypeInfo<TValue> jsonTypeInfo)
	{
		JsonSerializerOptions options = jsonTypeInfo.Options;
		Utf8JsonWriter writer = Utf8JsonWriterCache.RentWriterAndBuffer(jsonTypeInfo.Options, out var bufferWriter);
		try
		{
			jsonTypeInfo.Serialize(writer, in value);
			return JsonElement.Parse(bufferWriter.WrittenSpan, options.GetDocumentOptions());
		}
		finally
		{
			Utf8JsonWriterCache.ReturnWriterAndBuffer(writer, bufferWriter);
		}
	}

	private static JsonElement WriteElementAsObject(object value, JsonTypeInfo jsonTypeInfo)
	{
		JsonSerializerOptions options = jsonTypeInfo.Options;
		Utf8JsonWriter writer = Utf8JsonWriterCache.RentWriterAndBuffer(jsonTypeInfo.Options, out var bufferWriter);
		try
		{
			jsonTypeInfo.SerializeAsObject(writer, value);
			return JsonElement.Parse(bufferWriter.WrittenSpan, options.GetDocumentOptions());
		}
		finally
		{
			Utf8JsonWriterCache.ReturnWriterAndBuffer(writer, bufferWriter);
		}
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static JsonNode? SerializeToNode<TValue>(TValue value, JsonSerializerOptions? options = null)
	{
		JsonTypeInfo<TValue> typeInfo = GetTypeInfo<TValue>(options);
		return WriteNode<TValue>(in value, typeInfo);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static JsonNode? SerializeToNode(object? value, Type inputType, JsonSerializerOptions? options = null)
	{
		ValidateInputType(value, inputType);
		JsonTypeInfo typeInfo = GetTypeInfo(options, inputType);
		return WriteNodeAsObject(value, typeInfo);
	}

	public static JsonNode? SerializeToNode<TValue>(TValue value, JsonTypeInfo<TValue> jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return WriteNode(in value, jsonTypeInfo);
	}

	public static JsonNode? SerializeToNode(object? value, JsonTypeInfo jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return WriteNodeAsObject(value, jsonTypeInfo);
	}

	public static JsonNode? SerializeToNode(object? value, Type inputType, JsonSerializerContext context)
	{
		ArgumentNullException.ThrowIfNull(context, "context");
		ValidateInputType(value, inputType);
		JsonTypeInfo typeInfo = GetTypeInfo(context, inputType);
		return WriteNodeAsObject(value, typeInfo);
	}

	private static JsonNode WriteNode<TValue>(in TValue value, JsonTypeInfo<TValue> jsonTypeInfo)
	{
		JsonSerializerOptions options = jsonTypeInfo.Options;
		Utf8JsonWriter writer = Utf8JsonWriterCache.RentWriterAndBuffer(jsonTypeInfo.Options, out var bufferWriter);
		try
		{
			jsonTypeInfo.Serialize(writer, in value);
			return JsonNode.Parse(bufferWriter.WrittenSpan, options.GetNodeOptions(), options.GetDocumentOptions());
		}
		finally
		{
			Utf8JsonWriterCache.ReturnWriterAndBuffer(writer, bufferWriter);
		}
	}

	private static JsonNode WriteNodeAsObject(object value, JsonTypeInfo jsonTypeInfo)
	{
		JsonSerializerOptions options = jsonTypeInfo.Options;
		Utf8JsonWriter writer = Utf8JsonWriterCache.RentWriterAndBuffer(jsonTypeInfo.Options, out var bufferWriter);
		try
		{
			jsonTypeInfo.SerializeAsObject(writer, value);
			return JsonNode.Parse(bufferWriter.WrittenSpan, options.GetNodeOptions(), options.GetDocumentOptions());
		}
		finally
		{
			Utf8JsonWriterCache.ReturnWriterAndBuffer(writer, bufferWriter);
		}
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	private static JsonTypeInfo GetTypeInfo(JsonSerializerOptions options, Type inputType)
	{
		if (options == null)
		{
			options = JsonSerializerOptions.Default;
		}
		options.MakeReadOnly(populateMissingResolver: true);
		if (!(inputType == JsonTypeInfo.ObjectType))
		{
			return options.GetTypeInfoForRootType(inputType);
		}
		return options.ObjectTypeInfo;
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	private static JsonTypeInfo<T> GetTypeInfo<T>(JsonSerializerOptions options)
	{
		return (JsonTypeInfo<T>)GetTypeInfo(options, typeof(T));
	}

	private static JsonTypeInfo GetTypeInfo(JsonSerializerContext context, Type inputType)
	{
		JsonTypeInfo? typeInfo = context.GetTypeInfo(inputType);
		if (typeInfo == null)
		{
			ThrowHelper.ThrowInvalidOperationException_NoMetadataForType(inputType, context);
		}
		typeInfo.EnsureConfigured();
		return typeInfo;
	}

	private static void ValidateInputType(object value, Type inputType)
	{
		ArgumentNullException.ThrowIfNull(inputType, "inputType");
		if (value != null)
		{
			Type type = value.GetType();
			if (!inputType.IsAssignableFrom(type))
			{
				ThrowHelper.ThrowArgumentException_DeserializeWrongType(inputType, value);
			}
		}
	}

	internal static bool IsValidNumberHandlingValue(JsonNumberHandling handling)
	{
		return JsonHelpers.IsInRangeInclusive((int)handling, 0, 7);
	}

	internal static bool IsValidCreationHandlingValue(JsonObjectCreationHandling handling)
	{
		if ((uint)handling <= 1u)
		{
			return true;
		}
		return false;
	}

	internal static bool IsValidUnmappedMemberHandlingValue(JsonUnmappedMemberHandling handling)
	{
		if ((uint)handling <= 1u)
		{
			return true;
		}
		return false;
	}

	[return: NotNullIfNotNull("value")]
	internal static T UnboxOnRead<T>(object value)
	{
		if (value == null)
		{
			if (default(T) != null)
			{
				ThrowUnableToCastValue(value);
			}
			return default(T);
		}
		if (value is T)
		{
			return (T)value;
		}
		ThrowUnableToCastValue(value);
		return default(T);
		static void ThrowUnableToCastValue(object obj)
		{
			if (obj == null)
			{
				ThrowHelper.ThrowInvalidOperationException_DeserializeUnableToAssignNull(typeof(T));
			}
			else
			{
				ThrowHelper.ThrowInvalidCastException_DeserializeUnableToAssignValue(obj.GetType(), typeof(T));
			}
		}
	}

	[return: NotNullIfNotNull("value")]
	internal static T UnboxOnWrite<T>(object value)
	{
		if (default(T) != null && value == null)
		{
			ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(typeof(T));
		}
		return (T)value;
	}

	private static JsonTypeInfo<List<T>> GetOrAddListTypeInfoForRootLevelValueMode<T>(JsonTypeInfo<T> elementTypeInfo)
	{
		if (elementTypeInfo._asyncEnumerableRootLevelValueTypeInfo != null)
		{
			return (JsonTypeInfo<List<T>>)elementTypeInfo._asyncEnumerableRootLevelValueTypeInfo;
		}
		JsonTypeInfo<List<T>> jsonTypeInfo = new JsonTypeInfo<List<T>>(new RootLevelListConverter<T>(elementTypeInfo), elementTypeInfo.Options)
		{
			ElementTypeInfo = elementTypeInfo
		};
		jsonTypeInfo.EnsureConfigured();
		elementTypeInfo._asyncEnumerableRootLevelValueTypeInfo = jsonTypeInfo;
		return jsonTypeInfo;
	}

	private static JsonTypeInfo<List<T>> GetOrAddListTypeInfoForArrayMode<T>(JsonTypeInfo<T> elementTypeInfo)
	{
		if (elementTypeInfo._asyncEnumerableArrayTypeInfo != null)
		{
			return (JsonTypeInfo<List<T>>)elementTypeInfo._asyncEnumerableArrayTypeInfo;
		}
		JsonTypeInfo<List<T>> jsonTypeInfo = new JsonTypeInfo<List<T>>(new ListOfTConverter<List<T>, T>(), elementTypeInfo.Options)
		{
			CreateObject = () => new List<T>(),
			ElementTypeInfo = elementTypeInfo
		};
		jsonTypeInfo.EnsureConfigured();
		elementTypeInfo._asyncEnumerableArrayTypeInfo = jsonTypeInfo;
		return jsonTypeInfo;
	}

	internal static bool TryReadMetadata(JsonConverter converter, JsonTypeInfo jsonTypeInfo, ref Utf8JsonReader reader, scoped ref ReadStack state)
	{
		bool allowOutOfOrderMetadataProperties = jsonTypeInfo.Options.AllowOutOfOrderMetadataProperties;
		bool flag = false;
		Utf8JsonReader reader2;
		if (allowOutOfOrderMetadataProperties && !reader.IsFinalBlock)
		{
			reader2 = reader;
			if (!reader2.TrySkipPartial())
			{
				return false;
			}
		}
		else
		{
			reader2 = default(Utf8JsonReader);
		}
		while (true)
		{
			if (state.Current.PropertyState == StackFramePropertyState.None)
			{
				if (!reader.Read())
				{
					return false;
				}
				state.Current.PropertyState = StackFramePropertyState.ReadName;
			}
			if ((int)state.Current.PropertyState < 2)
			{
				if (reader.TokenType == JsonTokenType.EndObject)
				{
					break;
				}
				if ((state.Current.MetadataPropertyNames & MetadataPropertyName.Ref) != MetadataPropertyName.None)
				{
					ThrowHelper.ThrowJsonException_MetadataReferenceObjectCannotContainOtherProperties(reader.GetUnescapedSpan(), ref state);
				}
				ReadOnlySpan<byte> unescapedSpan = reader.GetUnescapedSpan();
				switch (state.Current.LatestMetadataPropertyName = GetMetadataPropertyName(unescapedSpan, jsonTypeInfo.PolymorphicTypeResolver))
				{
				case MetadataPropertyName.Id:
					state.Current.JsonPropertyName = s_idPropertyName;
					if (state.ReferenceResolver == null)
					{
						ThrowHelper.ThrowJsonException_MetadataUnexpectedProperty(unescapedSpan, ref state);
					}
					if ((state.Current.MetadataPropertyNames & (MetadataPropertyName.Id | MetadataPropertyName.Ref)) != MetadataPropertyName.None)
					{
						ThrowHelper.ThrowJsonException_MetadataIdCannotBeCombinedWithRef(unescapedSpan, ref state);
					}
					if (!converter.CanHaveMetadata)
					{
						ThrowHelper.ThrowJsonException_MetadataCannotParsePreservedObjectIntoImmutable(converter.Type);
					}
					goto IL_0257;
				case MetadataPropertyName.Ref:
					state.Current.JsonPropertyName = s_refPropertyName;
					if (state.ReferenceResolver == null)
					{
						ThrowHelper.ThrowJsonException_MetadataUnexpectedProperty(unescapedSpan, ref state);
					}
					if (converter.IsValueType)
					{
						ThrowHelper.ThrowJsonException_MetadataInvalidReferenceToValueType(converter.Type);
					}
					if ((state.Current.MetadataPropertyNames != MetadataPropertyName.None) | flag)
					{
						ThrowHelper.ThrowJsonException_MetadataReferenceObjectCannotContainOtherProperties(reader.GetUnescapedSpan(), ref state);
					}
					goto IL_0257;
				case MetadataPropertyName.Type:
					state.Current.JsonPropertyName = jsonTypeInfo.PolymorphicTypeResolver?.CustomTypeDiscriminatorPropertyNameUtf8 ?? s_typePropertyName;
					if (jsonTypeInfo.PolymorphicTypeResolver == null)
					{
						ThrowHelper.ThrowJsonException_MetadataUnexpectedProperty(unescapedSpan, ref state);
					}
					if (state.PolymorphicTypeDiscriminator != null)
					{
						ThrowHelper.ThrowJsonException_DuplicateMetadataProperty(state.Current.JsonPropertyName);
					}
					goto IL_0257;
				case MetadataPropertyName.Values:
					state.Current.JsonPropertyName = s_valuesPropertyName;
					if ((state.Current.MetadataPropertyNames & MetadataPropertyName.Values) != MetadataPropertyName.None)
					{
						ThrowHelper.ThrowJsonException_DuplicateMetadataProperty(s_valuesPropertyName);
					}
					if (flag)
					{
						ThrowHelper.ThrowJsonException_MetadataInvalidPropertyInArrayMetadata(ref state, jsonTypeInfo.Type, in reader);
					}
					goto IL_0257;
				default:
					{
						if (!allowOutOfOrderMetadataProperties)
						{
							break;
						}
						if (!flag)
						{
							flag = true;
							reader2 = reader;
						}
						if ((state.Current.MetadataPropertyNames & MetadataPropertyName.Values) != MetadataPropertyName.None)
						{
							ThrowHelper.ThrowJsonException_MetadataInvalidPropertyInArrayMetadata(ref state, jsonTypeInfo.Type, in reader);
						}
						if (IsMetadataPropertyName(unescapedSpan, null))
						{
							ThrowHelper.ThrowUnexpectedMetadataException(unescapedSpan, ref reader, ref state);
						}
						goto IL_0257;
					}
					IL_0257:
					state.Current.PropertyState = StackFramePropertyState.Name;
					goto IL_0263;
				}
				break;
			}
			goto IL_0263;
			IL_0263:
			if ((int)state.Current.PropertyState < 3)
			{
				if (!reader.Read())
				{
					return false;
				}
				state.Current.PropertyState = StackFramePropertyState.ReadValue;
			}
			switch (state.Current.LatestMetadataPropertyName)
			{
			case MetadataPropertyName.Id:
				if (reader.TokenType != JsonTokenType.String)
				{
					ThrowHelper.ThrowJsonException_MetadataValueWasNotString(reader.TokenType);
				}
				if (state.ReferenceId != null)
				{
					ThrowHelper.ThrowNotSupportedException_ObjectWithParameterizedCtorRefMetadataNotSupported(s_refPropertyName, ref reader, ref state);
				}
				state.ReferenceId = reader.GetString();
				goto IL_03c4;
			case MetadataPropertyName.Ref:
				if (reader.TokenType != JsonTokenType.String)
				{
					ThrowHelper.ThrowJsonException_MetadataValueWasNotString(reader.TokenType);
				}
				if (state.ReferenceId != null)
				{
					ThrowHelper.ThrowNotSupportedException_ObjectWithParameterizedCtorRefMetadataNotSupported(s_refPropertyName, ref reader, ref state);
				}
				state.ReferenceId = reader.GetString();
				goto IL_03c4;
			case MetadataPropertyName.Type:
				switch (reader.TokenType)
				{
				case JsonTokenType.String:
					state.PolymorphicTypeDiscriminator = reader.GetString();
					break;
				case JsonTokenType.Number:
					state.PolymorphicTypeDiscriminator = reader.GetInt32();
					break;
				default:
					ThrowHelper.ThrowJsonException_MetadataValueWasNotString(reader.TokenType);
					break;
				}
				goto IL_03c4;
			case MetadataPropertyName.Values:
				if (reader.TokenType != JsonTokenType.StartArray)
				{
					ThrowHelper.ThrowJsonException_MetadataValuesInvalidToken(reader.TokenType);
				}
				if (allowOutOfOrderMetadataProperties)
				{
					flag = true;
					reader2 = reader;
					reader.SkipWithVerify();
					goto IL_03c4;
				}
				break;
			default:
				reader.SkipWithVerify();
				goto IL_03c4;
			}
			state.Current.MetadataPropertyNames |= MetadataPropertyName.Values;
			state.Current.PropertyState = StackFramePropertyState.None;
			break;
			IL_03c4:
			state.Current.MetadataPropertyNames |= state.Current.LatestMetadataPropertyName;
			state.Current.PropertyState = StackFramePropertyState.None;
			state.Current.JsonPropertyName = null;
		}
		if (state.Current.MetadataPropertyNames == MetadataPropertyName.Values)
		{
			ThrowHelper.ThrowJsonException_MetadataStandaloneValuesProperty(ref state, s_valuesPropertyName);
		}
		if (flag)
		{
			reader = reader2;
		}
		return true;
	}

	internal static bool IsMetadataPropertyName(ReadOnlySpan<byte> propertyName, PolymorphicTypeResolver resolver)
	{
		if (propertyName.Length <= 0 || propertyName[0] != 36)
		{
			if (resolver == null)
			{
				return false;
			}
			byte[] customTypeDiscriminatorPropertyNameUtf = resolver.CustomTypeDiscriminatorPropertyNameUtf8;
			return ((customTypeDiscriminatorPropertyNameUtf != null) ? new bool?(((ReadOnlySpan<byte>)customTypeDiscriminatorPropertyNameUtf.AsSpan()).SequenceEqual(propertyName)) : ((bool?)null)) == true;
		}
		return true;
	}

	internal static MetadataPropertyName GetMetadataPropertyName(ReadOnlySpan<byte> propertyName, PolymorphicTypeResolver resolver)
	{
		if (propertyName.Length > 0 && propertyName[0] == 36)
		{
			switch (propertyName.Length)
			{
			case 3:
				if (propertyName.SequenceEqual("$id"u8))
				{
					return MetadataPropertyName.Id;
				}
				break;
			case 4:
				if (propertyName.SequenceEqual("$ref"u8))
				{
					return MetadataPropertyName.Ref;
				}
				break;
			case 5:
				if (resolver?.CustomTypeDiscriminatorPropertyNameUtf8 == null && propertyName.SequenceEqual("$type"u8))
				{
					return MetadataPropertyName.Type;
				}
				break;
			case 7:
				if (propertyName.SequenceEqual("$values"u8))
				{
					return MetadataPropertyName.Values;
				}
				break;
			}
		}
		byte[] array = resolver?.CustomTypeDiscriminatorPropertyNameUtf8;
		if (array != null && propertyName.SequenceEqual(array))
		{
			return MetadataPropertyName.Type;
		}
		return MetadataPropertyName.None;
	}

	internal static bool TryHandleReferenceFromJsonElement(ref Utf8JsonReader reader, scoped ref ReadStack state, JsonElement element, [NotNullWhen(true)] out object referenceValue)
	{
		bool flag = false;
		referenceValue = null;
		if (element.ValueKind == JsonValueKind.Object)
		{
			int num = 0;
			foreach (JsonProperty item in element.EnumerateObject())
			{
				num++;
				if (flag)
				{
					ThrowHelper.ThrowJsonException_MetadataReferenceObjectCannotContainOtherProperties();
					continue;
				}
				if (item.EscapedNameEquals(s_idPropertyName))
				{
					if (state.ReferenceId != null)
					{
						ThrowHelper.ThrowNotSupportedException_ObjectWithParameterizedCtorRefMetadataNotSupported(s_refPropertyName, ref reader, ref state);
					}
					if (item.Value.ValueKind != JsonValueKind.String)
					{
						ThrowHelper.ThrowJsonException_MetadataValueWasNotString(item.Value.ValueKind);
					}
					object obj = element;
					state.ReferenceResolver.AddReference(item.Value.GetString(), obj);
					referenceValue = obj;
					return true;
				}
				if (item.EscapedNameEquals(s_refPropertyName))
				{
					if (state.ReferenceId != null)
					{
						ThrowHelper.ThrowNotSupportedException_ObjectWithParameterizedCtorRefMetadataNotSupported(s_refPropertyName, ref reader, ref state);
					}
					if (num > 1)
					{
						ThrowHelper.ThrowJsonException_MetadataReferenceObjectCannotContainOtherProperties();
					}
					if (item.Value.ValueKind != JsonValueKind.String)
					{
						ThrowHelper.ThrowJsonException_MetadataValueWasNotString(item.Value.ValueKind);
					}
					referenceValue = state.ReferenceResolver.ResolveReference(item.Value.GetString());
					flag = true;
				}
			}
		}
		return flag;
	}

	internal static bool TryHandleReferenceFromJsonNode(ref Utf8JsonReader reader, scoped ref ReadStack state, JsonNode jsonNode, [NotNullWhen(true)] out object referenceValue)
	{
		bool flag = false;
		referenceValue = null;
		if (jsonNode is JsonObject jsonObject)
		{
			int num = 0;
			foreach (KeyValuePair<string, JsonNode> item in jsonObject)
			{
				num++;
				if (flag)
				{
					ThrowHelper.ThrowJsonException_MetadataReferenceObjectCannotContainOtherProperties();
					continue;
				}
				if (item.Key == "$id")
				{
					if (state.ReferenceId != null)
					{
						ThrowHelper.ThrowNotSupportedException_ObjectWithParameterizedCtorRefMetadataNotSupported(s_refPropertyName, ref reader, ref state);
					}
					string referenceId = ReadAsStringMetadataValue(item.Value);
					state.ReferenceResolver.AddReference(referenceId, jsonNode);
					referenceValue = jsonNode;
					return true;
				}
				if (item.Key == "$ref")
				{
					if (state.ReferenceId != null)
					{
						ThrowHelper.ThrowNotSupportedException_ObjectWithParameterizedCtorRefMetadataNotSupported(s_refPropertyName, ref reader, ref state);
					}
					if (num > 1)
					{
						ThrowHelper.ThrowJsonException_MetadataReferenceObjectCannotContainOtherProperties();
					}
					string referenceId2 = ReadAsStringMetadataValue(item.Value);
					referenceValue = state.ReferenceResolver.ResolveReference(referenceId2);
					flag = true;
				}
			}
		}
		return flag;
		static string ReadAsStringMetadataValue(JsonNode jsonNode2)
		{
			if (jsonNode2 is JsonValue jsonValue && jsonValue.TryGetValue<string>(out string value) && value != null)
			{
				return value;
			}
			ThrowHelper.ThrowJsonException_MetadataValueWasNotString(jsonNode2?.GetValueKind() ?? JsonValueKind.Null);
			return null;
		}
	}

	internal static void ValidateMetadataForObjectConverter(ref ReadStack state)
	{
		if ((state.Current.MetadataPropertyNames & MetadataPropertyName.Values) != MetadataPropertyName.None)
		{
			ThrowHelper.ThrowJsonException_MetadataUnexpectedProperty(s_valuesPropertyName, ref state);
		}
	}

	internal static void ValidateMetadataForArrayConverter(JsonConverter converter, ref Utf8JsonReader reader, scoped ref ReadStack state)
	{
		switch (reader.TokenType)
		{
		case JsonTokenType.EndObject:
			if (state.Current.MetadataPropertyNames != MetadataPropertyName.Ref)
			{
				ThrowHelper.ThrowJsonException_MetadataPreservedArrayValuesNotFound(ref state, converter.Type);
			}
			break;
		default:
			ThrowHelper.ThrowJsonException_MetadataInvalidPropertyInArrayMetadata(ref state, converter.Type, in reader);
			break;
		case JsonTokenType.StartArray:
			break;
		}
	}

	internal static T ResolveReferenceId<T>(ref ReadStack state)
	{
		string referenceId = state.ReferenceId;
		object obj = state.ReferenceResolver.ResolveReference(referenceId);
		state.ReferenceId = null;
		try
		{
			return (T)obj;
		}
		catch (InvalidCastException)
		{
			ThrowHelper.ThrowInvalidOperationException_MetadataReferenceOfTypeCannotBeAssignedToType(referenceId, obj.GetType(), typeof(T));
			return default(T);
		}
	}

	internal static JsonPropertyInfo LookupProperty(object obj, ReadOnlySpan<byte> unescapedPropertyName, ref ReadStack state, JsonSerializerOptions options, out bool useExtensionProperty, bool createExtensionProperty = true)
	{
		JsonTypeInfo jsonTypeInfo = state.Current.JsonTypeInfo;
		useExtensionProperty = false;
		JsonPropertyInfo jsonPropertyInfo = jsonTypeInfo.GetProperty(unescapedPropertyName, ref state.Current, out var utf8PropertyName);
		state.Current.PropertyIndex++;
		state.Current.JsonPropertyName = utf8PropertyName;
		if (jsonPropertyInfo == null)
		{
			if (jsonTypeInfo.EffectiveUnmappedMemberHandling == JsonUnmappedMemberHandling.Disallow)
			{
				string unmappedPropertyName = JsonHelpers.Utf8GetString(unescapedPropertyName);
				ThrowHelper.ThrowJsonException_UnmappedJsonProperty(jsonTypeInfo.Type, unmappedPropertyName);
			}
			JsonPropertyInfo extensionDataProperty = jsonTypeInfo.ExtensionDataProperty;
			if (extensionDataProperty != null && extensionDataProperty.HasGetter && extensionDataProperty.HasSetter)
			{
				state.Current.JsonPropertyNameAsString = JsonHelpers.Utf8GetString(unescapedPropertyName);
				if (createExtensionProperty)
				{
					CreateExtensionDataProperty(obj, extensionDataProperty, options);
				}
				jsonPropertyInfo = extensionDataProperty;
				useExtensionProperty = true;
			}
			else
			{
				jsonPropertyInfo = JsonPropertyInfo.s_missingProperty;
			}
		}
		state.Current.JsonPropertyInfo = jsonPropertyInfo;
		state.Current.NumberHandling = jsonPropertyInfo.EffectiveNumberHandling;
		return jsonPropertyInfo;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ReadOnlySpan<byte> GetPropertyName(scoped ref ReadStack state, ref Utf8JsonReader reader, JsonSerializerOptions options, out bool isAlreadyReadMetadataProperty)
	{
		ReadOnlySpan<byte> unescapedSpan = reader.GetUnescapedSpan();
		isAlreadyReadMetadataProperty = false;
		if (state.Current.CanContainMetadata && IsMetadataPropertyName(unescapedSpan, state.Current.BaseJsonTypeInfo.PolymorphicTypeResolver))
		{
			if (options.AllowOutOfOrderMetadataProperties)
			{
				isAlreadyReadMetadataProperty = true;
			}
			else
			{
				ThrowHelper.ThrowUnexpectedMetadataException(unescapedSpan, ref reader, ref state);
			}
		}
		return unescapedSpan;
	}

	internal static void CreateExtensionDataProperty(object obj, JsonPropertyInfo jsonPropertyInfo, JsonSerializerOptions options)
	{
		object valueAsObject = jsonPropertyInfo.GetValueAsObject(obj);
		if (valueAsObject != null)
		{
			return;
		}
		Func<object>? obj2 = jsonPropertyInfo.JsonTypeInfo.CreateObject ?? jsonPropertyInfo.JsonTypeInfo.CreateObjectForExtensionDataProperty;
		if (obj2 == null)
		{
			if (jsonPropertyInfo.PropertyType.FullName == "System.Text.Json.Nodes.JsonObject")
			{
				ThrowHelper.ThrowInvalidOperationException_NodeJsonObjectCustomConverterNotAllowedOnExtensionProperty();
			}
			else
			{
				ThrowHelper.ThrowNotSupportedException_SerializationNotSupported(jsonPropertyInfo.PropertyType);
			}
		}
		valueAsObject = obj2();
		jsonPropertyInfo.Set(obj, valueAsObject);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static TValue? Deserialize<TValue>(ReadOnlySpan<byte> utf8Json, JsonSerializerOptions? options = null)
	{
		JsonTypeInfo<TValue> typeInfo = GetTypeInfo<TValue>(options);
		return ReadFromSpan(utf8Json, typeInfo);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static object? Deserialize(ReadOnlySpan<byte> utf8Json, Type returnType, JsonSerializerOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(returnType, "returnType");
		JsonTypeInfo typeInfo = GetTypeInfo(options, returnType);
		return ReadFromSpanAsObject(utf8Json, typeInfo);
	}

	public static TValue? Deserialize<TValue>(ReadOnlySpan<byte> utf8Json, JsonTypeInfo<TValue> jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return ReadFromSpan(utf8Json, jsonTypeInfo);
	}

	public static object? Deserialize(ReadOnlySpan<byte> utf8Json, JsonTypeInfo jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return ReadFromSpanAsObject(utf8Json, jsonTypeInfo);
	}

	public static object? Deserialize(ReadOnlySpan<byte> utf8Json, Type returnType, JsonSerializerContext context)
	{
		ArgumentNullException.ThrowIfNull(returnType, "returnType");
		ArgumentNullException.ThrowIfNull(context, "context");
		return ReadFromSpanAsObject(utf8Json, GetTypeInfo(context, returnType));
	}

	private static TValue ReadFromSpan<TValue>(ReadOnlySpan<byte> utf8Json, JsonTypeInfo<TValue> jsonTypeInfo, int? actualByteCount = null)
	{
		JsonReaderState state = new JsonReaderState(jsonTypeInfo.Options.GetReaderOptions());
		Utf8JsonReader reader = new Utf8JsonReader(utf8Json, isFinalBlock: true, state);
		ReadStack state2 = default(ReadStack);
		state2.Initialize(jsonTypeInfo);
		return jsonTypeInfo.Deserialize(ref reader, ref state2);
	}

	private static object ReadFromSpanAsObject(ReadOnlySpan<byte> utf8Json, JsonTypeInfo jsonTypeInfo, int? actualByteCount = null)
	{
		JsonReaderState state = new JsonReaderState(jsonTypeInfo.Options.GetReaderOptions());
		Utf8JsonReader reader = new Utf8JsonReader(utf8Json, isFinalBlock: true, state);
		ReadStack state2 = default(ReadStack);
		state2.Initialize(jsonTypeInfo);
		return jsonTypeInfo.DeserializeAsObject(ref reader, ref state2);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static ValueTask<TValue?> DeserializeAsync<TValue>(Stream utf8Json, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		return GetTypeInfo<TValue>(options).DeserializeAsync(utf8Json, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static TValue? Deserialize<TValue>(Stream utf8Json, JsonSerializerOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		return GetTypeInfo<TValue>(options).Deserialize(utf8Json);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static ValueTask<object?> DeserializeAsync(Stream utf8Json, Type returnType, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArgumentNullException.ThrowIfNull(returnType, "returnType");
		return GetTypeInfo(options, returnType).DeserializeAsObjectAsync(utf8Json, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static object? Deserialize(Stream utf8Json, Type returnType, JsonSerializerOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArgumentNullException.ThrowIfNull(returnType, "returnType");
		return GetTypeInfo(options, returnType).DeserializeAsObject(utf8Json);
	}

	public static ValueTask<TValue?> DeserializeAsync<TValue>(Stream utf8Json, JsonTypeInfo<TValue> jsonTypeInfo, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return jsonTypeInfo.DeserializeAsync(utf8Json, cancellationToken);
	}

	public static ValueTask<object?> DeserializeAsync(Stream utf8Json, JsonTypeInfo jsonTypeInfo, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return jsonTypeInfo.DeserializeAsObjectAsync(utf8Json, cancellationToken);
	}

	public static TValue? Deserialize<TValue>(Stream utf8Json, JsonTypeInfo<TValue> jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return jsonTypeInfo.Deserialize(utf8Json);
	}

	public static object? Deserialize(Stream utf8Json, JsonTypeInfo jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return jsonTypeInfo.DeserializeAsObject(utf8Json);
	}

	public static ValueTask<object?> DeserializeAsync(Stream utf8Json, Type returnType, JsonSerializerContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArgumentNullException.ThrowIfNull(returnType, "returnType");
		ArgumentNullException.ThrowIfNull(context, "context");
		return GetTypeInfo(context, returnType).DeserializeAsObjectAsync(utf8Json, cancellationToken);
	}

	public static object? Deserialize(Stream utf8Json, Type returnType, JsonSerializerContext context)
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArgumentNullException.ThrowIfNull(returnType, "returnType");
		ArgumentNullException.ThrowIfNull(context, "context");
		return GetTypeInfo(context, returnType).DeserializeAsObject(utf8Json);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static IAsyncEnumerable<TValue?> DeserializeAsyncEnumerable<TValue>(Stream utf8Json, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		return DeserializeAsyncEnumerable<TValue>(utf8Json, topLevelValues: false, options, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static IAsyncEnumerable<TValue?> DeserializeAsyncEnumerable<TValue>(Stream utf8Json, bool topLevelValues, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		JsonTypeInfo<TValue> typeInfo = GetTypeInfo<TValue>(options);
		return DeserializeAsyncEnumerableCore(utf8Json, typeInfo, topLevelValues, cancellationToken);
	}

	public static IAsyncEnumerable<TValue?> DeserializeAsyncEnumerable<TValue>(Stream utf8Json, JsonTypeInfo<TValue> jsonTypeInfo, CancellationToken cancellationToken = default(CancellationToken))
	{
		return DeserializeAsyncEnumerable(utf8Json, jsonTypeInfo, topLevelValues: false, cancellationToken);
	}

	public static IAsyncEnumerable<TValue?> DeserializeAsyncEnumerable<TValue>(Stream utf8Json, JsonTypeInfo<TValue> jsonTypeInfo, bool topLevelValues, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return DeserializeAsyncEnumerableCore(utf8Json, jsonTypeInfo, topLevelValues, cancellationToken);
	}

	private static IAsyncEnumerable<T> DeserializeAsyncEnumerableCore<T>(Stream utf8Json, JsonTypeInfo<T> jsonTypeInfo, bool topLevelValues, CancellationToken cancellationToken)
	{
		JsonReaderOptions readerOptions = jsonTypeInfo.Options.GetReaderOptions();
		JsonTypeInfo<List<T>> listTypeInfo;
		if (topLevelValues)
		{
			listTypeInfo = GetOrAddListTypeInfoForRootLevelValueMode(jsonTypeInfo);
			readerOptions.AllowMultipleValues = true;
		}
		else
		{
			listTypeInfo = GetOrAddListTypeInfoForArrayMode(jsonTypeInfo);
		}
		return CreateAsyncEnumerableFromArray(utf8Json, listTypeInfo, readerOptions, cancellationToken);
		static async IAsyncEnumerable<T> CreateAsyncEnumerableFromArray(Stream stream, JsonTypeInfo<List<T>> jsonTypeInfo2, JsonReaderOptions options, [EnumeratorCancellation] CancellationToken cancellationToken2)
		{
			ReadStack readStack = default(ReadStack);
			readStack.Initialize(jsonTypeInfo2, supportContinuation: true);
			JsonReaderState jsonReaderState = new JsonReaderState(options);
			StreamReadBufferState bufferState = new StreamReadBufferState(jsonTypeInfo2.Options.DefaultBufferSize);
			try
			{
				bool success;
				do
				{
					bufferState = await bufferState.ReadAsync(stream, cancellationToken2, fillBuffer: false).ConfigureAwait(continueOnCapturedContext: false);
					success = jsonTypeInfo2.ContinueDeserialize<StreamReadBufferState, Stream>(ref bufferState, ref jsonReaderState, ref readStack, out var _);
					object returnValue = readStack.Current.ReturnValue;
					if (returnValue != null)
					{
						List<T> list = (List<T>)returnValue;
						foreach (T item in list)
						{
							yield return item;
						}
						list.Clear();
					}
				}
				while (!success);
			}
			finally
			{
				bufferState.Dispose();
			}
		}
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static TValue? Deserialize<TValue>([StringSyntax("Json")] string json, JsonSerializerOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(json, "json");
		JsonTypeInfo<TValue> typeInfo = GetTypeInfo<TValue>(options);
		return ReadFromSpan(json.AsSpan(), typeInfo);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static TValue? Deserialize<TValue>([StringSyntax("Json")] ReadOnlySpan<char> json, JsonSerializerOptions? options = null)
	{
		JsonTypeInfo<TValue> typeInfo = GetTypeInfo<TValue>(options);
		return ReadFromSpan(json, typeInfo);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static object? Deserialize([StringSyntax("Json")] string json, Type returnType, JsonSerializerOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(json, "json");
		ArgumentNullException.ThrowIfNull(returnType, "returnType");
		JsonTypeInfo typeInfo = GetTypeInfo(options, returnType);
		return ReadFromSpanAsObject(json.AsSpan(), typeInfo);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static object? Deserialize([StringSyntax("Json")] ReadOnlySpan<char> json, Type returnType, JsonSerializerOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(returnType, "returnType");
		JsonTypeInfo typeInfo = GetTypeInfo(options, returnType);
		return ReadFromSpanAsObject(json, typeInfo);
	}

	public static TValue? Deserialize<TValue>([StringSyntax("Json")] string json, JsonTypeInfo<TValue> jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(json, "json");
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return ReadFromSpan(json.AsSpan(), jsonTypeInfo);
	}

	public static TValue? Deserialize<TValue>([StringSyntax("Json")] ReadOnlySpan<char> json, JsonTypeInfo<TValue> jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return ReadFromSpan(json, jsonTypeInfo);
	}

	public static object? Deserialize([StringSyntax("Json")] string json, JsonTypeInfo jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(json, "json");
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return ReadFromSpanAsObject(json.AsSpan(), jsonTypeInfo);
	}

	public static object? Deserialize([StringSyntax("Json")] ReadOnlySpan<char> json, JsonTypeInfo jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return ReadFromSpanAsObject(json, jsonTypeInfo);
	}

	public static object? Deserialize([StringSyntax("Json")] string json, Type returnType, JsonSerializerContext context)
	{
		ArgumentNullException.ThrowIfNull(json, "json");
		ArgumentNullException.ThrowIfNull(returnType, "returnType");
		ArgumentNullException.ThrowIfNull(context, "context");
		JsonTypeInfo typeInfo = GetTypeInfo(context, returnType);
		return ReadFromSpanAsObject(json.AsSpan(), typeInfo);
	}

	public static object? Deserialize([StringSyntax("Json")] ReadOnlySpan<char> json, Type returnType, JsonSerializerContext context)
	{
		ArgumentNullException.ThrowIfNull(returnType, "returnType");
		ArgumentNullException.ThrowIfNull(context, "context");
		JsonTypeInfo typeInfo = GetTypeInfo(context, returnType);
		return ReadFromSpanAsObject(json, typeInfo);
	}

	private static TValue ReadFromSpan<TValue>(ReadOnlySpan<char> json, JsonTypeInfo<TValue> jsonTypeInfo)
	{
		byte[] array = null;
		Span<byte> span = ((json.Length > 85) ? ((Span<byte>)(((long)json.Length > 357913941L) ? new byte[JsonReaderHelper.GetUtf8ByteCount(json)] : (array = ArrayPool<byte>.Shared.Rent(json.Length * 3)))) : stackalloc byte[256]);
		Span<byte> span2 = span;
		try
		{
			int utf8FromText = JsonReaderHelper.GetUtf8FromText(json, span2);
			span2 = span2.Slice(0, utf8FromText);
			return ReadFromSpan(span2, jsonTypeInfo, utf8FromText);
		}
		finally
		{
			if (array != null)
			{
				span2.Clear();
				ArrayPool<byte>.Shared.Return(array);
			}
		}
	}

	private static object ReadFromSpanAsObject(ReadOnlySpan<char> json, JsonTypeInfo jsonTypeInfo)
	{
		byte[] array = null;
		Span<byte> span = ((json.Length > 85) ? ((Span<byte>)(((long)json.Length > 357913941L) ? new byte[JsonReaderHelper.GetUtf8ByteCount(json)] : (array = ArrayPool<byte>.Shared.Rent(json.Length * 3)))) : stackalloc byte[256]);
		Span<byte> span2 = span;
		try
		{
			int utf8FromText = JsonReaderHelper.GetUtf8FromText(json, span2);
			span2 = span2.Slice(0, utf8FromText);
			return ReadFromSpanAsObject(span2, jsonTypeInfo, utf8FromText);
		}
		finally
		{
			if (array != null)
			{
				span2.Clear();
				ArrayPool<byte>.Shared.Return(array);
			}
		}
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static TValue? Deserialize<TValue>(ref Utf8JsonReader reader, JsonSerializerOptions? options = null)
	{
		JsonTypeInfo<TValue> typeInfo = GetTypeInfo<TValue>(options);
		return Read(ref reader, typeInfo);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static object? Deserialize(ref Utf8JsonReader reader, Type returnType, JsonSerializerOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(returnType, "returnType");
		JsonTypeInfo typeInfo = GetTypeInfo(options, returnType);
		return ReadAsObject(ref reader, typeInfo);
	}

	public static TValue? Deserialize<TValue>(ref Utf8JsonReader reader, JsonTypeInfo<TValue> jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return Read(ref reader, jsonTypeInfo);
	}

	public static object? Deserialize(ref Utf8JsonReader reader, JsonTypeInfo jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return ReadAsObject(ref reader, jsonTypeInfo);
	}

	public static object? Deserialize(ref Utf8JsonReader reader, Type returnType, JsonSerializerContext context)
	{
		ArgumentNullException.ThrowIfNull(returnType, "returnType");
		ArgumentNullException.ThrowIfNull(context, "context");
		return ReadAsObject(ref reader, GetTypeInfo(context, returnType));
	}

	private static TValue Read<TValue>(ref Utf8JsonReader reader, JsonTypeInfo<TValue> jsonTypeInfo)
	{
		if (reader.CurrentState.Options.CommentHandling == JsonCommentHandling.Allow)
		{
			ThrowHelper.ThrowArgumentException_SerializerDoesNotSupportComments("reader");
		}
		ReadStack state = default(ReadStack);
		state.Initialize(jsonTypeInfo);
		Utf8JsonReader utf8JsonReader = reader;
		try
		{
			Utf8JsonReader reader2 = GetReaderScopedToNextValue(ref reader, ref state);
			return jsonTypeInfo.Deserialize(ref reader2, ref state);
		}
		catch (JsonException)
		{
			reader = utf8JsonReader;
			throw;
		}
	}

	private static object ReadAsObject(ref Utf8JsonReader reader, JsonTypeInfo jsonTypeInfo)
	{
		if (reader.CurrentState.Options.CommentHandling == JsonCommentHandling.Allow)
		{
			ThrowHelper.ThrowArgumentException_SerializerDoesNotSupportComments("reader");
		}
		ReadStack state = default(ReadStack);
		state.Initialize(jsonTypeInfo);
		Utf8JsonReader utf8JsonReader = reader;
		try
		{
			Utf8JsonReader reader2 = GetReaderScopedToNextValue(ref reader, ref state);
			return jsonTypeInfo.DeserializeAsObject(ref reader2, ref state);
		}
		catch (JsonException)
		{
			reader = utf8JsonReader;
			throw;
		}
	}

	private static Utf8JsonReader GetReaderScopedToNextValue(ref Utf8JsonReader reader, scoped ref ReadStack state)
	{
		ReadOnlySpan<byte> jsonData = default(ReadOnlySpan<byte>);
		ReadOnlySequence<byte> jsonData2 = default(ReadOnlySequence<byte>);
		try
		{
			JsonTokenType tokenType = reader.TokenType;
			if ((tokenType == JsonTokenType.None || tokenType == JsonTokenType.PropertyName) && !reader.Read())
			{
				ThrowHelper.ThrowJsonReaderException(ref reader, ExceptionResource.ExpectedOneCompleteToken, 0);
			}
			switch (reader.TokenType)
			{
			case JsonTokenType.StartObject:
			case JsonTokenType.StartArray:
			{
				long tokenStartIndex = reader.TokenStartIndex;
				if (!reader.TrySkip())
				{
					ThrowHelper.ThrowJsonReaderException(ref reader, ExceptionResource.NotEnoughData, 0);
				}
				long num = reader.BytesConsumed - tokenStartIndex;
				ReadOnlySequence<byte> originalSequence = reader.OriginalSequence;
				if (originalSequence.IsEmpty)
				{
					jsonData = checked(reader.OriginalSpan.Slice((int)tokenStartIndex, (int)num));
				}
				else
				{
					jsonData2 = originalSequence.Slice(tokenStartIndex, num);
				}
				break;
			}
			case JsonTokenType.Number:
			case JsonTokenType.True:
			case JsonTokenType.False:
			case JsonTokenType.Null:
				if (reader.HasValueSequence)
				{
					jsonData2 = reader.ValueSequence;
				}
				else
				{
					jsonData = reader.ValueSpan;
				}
				break;
			case JsonTokenType.String:
			{
				ReadOnlySequence<byte> originalSequence2 = reader.OriginalSequence;
				if (originalSequence2.IsEmpty)
				{
					int length = reader.ValueSpan.Length + 2;
					jsonData = reader.OriginalSpan.Slice((int)reader.TokenStartIndex, length);
				}
				else
				{
					long length2 = (reader.HasValueSequence ? (reader.ValueSequence.Length + 2) : (reader.ValueSpan.Length + 2));
					jsonData2 = originalSequence2.Slice(reader.TokenStartIndex, length2);
				}
				break;
			}
			default:
			{
				byte nextByte = (reader.HasValueSequence ? reader.ValueSequence.First.Span[0] : reader.ValueSpan[0]);
				ThrowHelper.ThrowJsonReaderException(ref reader, ExceptionResource.ExpectedStartOfValueNotFound, nextByte);
				break;
			}
			}
		}
		catch (JsonReaderException ex)
		{
			ThrowHelper.ReThrowWithPath(ref state, ex);
		}
		if (!jsonData.IsEmpty)
		{
			return new Utf8JsonReader(jsonData, reader.CurrentState.Options);
		}
		return new Utf8JsonReader(jsonData2, reader.CurrentState.Options);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static byte[] SerializeToUtf8Bytes<TValue>(TValue value, JsonSerializerOptions? options = null)
	{
		JsonTypeInfo<TValue> typeInfo = GetTypeInfo<TValue>(options);
		return WriteBytes<TValue>(in value, typeInfo);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static byte[] SerializeToUtf8Bytes(object? value, Type inputType, JsonSerializerOptions? options = null)
	{
		ValidateInputType(value, inputType);
		JsonTypeInfo typeInfo = GetTypeInfo(options, inputType);
		return WriteBytesAsObject(value, typeInfo);
	}

	public static byte[] SerializeToUtf8Bytes<TValue>(TValue value, JsonTypeInfo<TValue> jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return WriteBytes(in value, jsonTypeInfo);
	}

	public static byte[] SerializeToUtf8Bytes(object? value, JsonTypeInfo jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return WriteBytesAsObject(value, jsonTypeInfo);
	}

	public static byte[] SerializeToUtf8Bytes(object? value, Type inputType, JsonSerializerContext context)
	{
		ArgumentNullException.ThrowIfNull(context, "context");
		ValidateInputType(value, inputType);
		JsonTypeInfo typeInfo = GetTypeInfo(context, inputType);
		return WriteBytesAsObject(value, typeInfo);
	}

	private static byte[] WriteBytes<TValue>(in TValue value, JsonTypeInfo<TValue> jsonTypeInfo)
	{
		Utf8JsonWriter writer = Utf8JsonWriterCache.RentWriterAndBuffer(jsonTypeInfo.Options, out var bufferWriter);
		try
		{
			jsonTypeInfo.Serialize(writer, in value);
			return bufferWriter.WrittenSpan.ToArray();
		}
		finally
		{
			Utf8JsonWriterCache.ReturnWriterAndBuffer(writer, bufferWriter);
		}
	}

	private static byte[] WriteBytesAsObject(object value, JsonTypeInfo jsonTypeInfo)
	{
		Utf8JsonWriter writer = Utf8JsonWriterCache.RentWriterAndBuffer(jsonTypeInfo.Options, out var bufferWriter);
		try
		{
			jsonTypeInfo.SerializeAsObject(writer, value);
			return bufferWriter.WrittenSpan.ToArray();
		}
		finally
		{
			Utf8JsonWriterCache.ReturnWriterAndBuffer(writer, bufferWriter);
		}
	}

	internal static MetadataPropertyName WriteMetadataForObject(JsonConverter jsonConverter, ref WriteStack state, Utf8JsonWriter writer)
	{
		MetadataPropertyName metadataPropertyName = MetadataPropertyName.None;
		if (state.NewReferenceId != null)
		{
			writer.WriteString(s_metadataId, state.NewReferenceId);
			metadataPropertyName |= MetadataPropertyName.Id;
			state.NewReferenceId = null;
		}
		object polymorphicTypeDiscriminator = state.PolymorphicTypeDiscriminator;
		if (polymorphicTypeDiscriminator != null)
		{
			JsonEncodedText? customTypeDiscriminatorPropertyNameJsonEncoded = state.PolymorphicTypeResolver.CustomTypeDiscriminatorPropertyNameJsonEncoded;
			JsonEncodedText jsonEncodedText;
			if (customTypeDiscriminatorPropertyNameJsonEncoded.HasValue)
			{
				JsonEncodedText valueOrDefault = customTypeDiscriminatorPropertyNameJsonEncoded.GetValueOrDefault();
				jsonEncodedText = valueOrDefault;
			}
			else
			{
				jsonEncodedText = s_metadataType;
			}
			JsonEncodedText propertyName = jsonEncodedText;
			if (polymorphicTypeDiscriminator is string value)
			{
				writer.WriteString(propertyName, value);
			}
			else
			{
				writer.WriteNumber(propertyName, (int)polymorphicTypeDiscriminator);
			}
			metadataPropertyName |= MetadataPropertyName.Type;
			state.PolymorphicTypeDiscriminator = null;
		}
		return metadataPropertyName;
	}

	internal static MetadataPropertyName WriteMetadataForCollection(JsonConverter jsonConverter, ref WriteStack state, Utf8JsonWriter writer)
	{
		writer.WriteStartObject();
		MetadataPropertyName result = WriteMetadataForObject(jsonConverter, ref state, writer);
		writer.WritePropertyName(s_metadataValues);
		return result;
	}

	internal static bool TryGetReferenceForValue(object currentValue, ref WriteStack state, Utf8JsonWriter writer)
	{
		string reference = state.ReferenceResolver.GetReference(currentValue, out var alreadyExists);
		if (alreadyExists)
		{
			writer.WriteStartObject();
			writer.WriteString(s_metadataRef, reference);
			writer.WriteEndObject();
			state.PolymorphicTypeDiscriminator = null;
			state.PolymorphicTypeResolver = null;
		}
		else
		{
			state.NewReferenceId = reference;
		}
		return alreadyExists;
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static Task SerializeAsync<TValue>(Stream utf8Json, TValue value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		return GetTypeInfo<TValue>(options).SerializeAsync(utf8Json, value, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static void Serialize<TValue>(Stream utf8Json, TValue value, JsonSerializerOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		GetTypeInfo<TValue>(options).Serialize(utf8Json, in value);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static Task SerializeAsync(Stream utf8Json, object? value, Type inputType, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ValidateInputType(value, inputType);
		return GetTypeInfo(options, inputType).SerializeAsObjectAsync(utf8Json, value, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static void Serialize(Stream utf8Json, object? value, Type inputType, JsonSerializerOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ValidateInputType(value, inputType);
		GetTypeInfo(options, inputType).SerializeAsObject(utf8Json, value);
	}

	public static Task SerializeAsync<TValue>(Stream utf8Json, TValue value, JsonTypeInfo<TValue> jsonTypeInfo, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return jsonTypeInfo.SerializeAsync(utf8Json, value, cancellationToken);
	}

	public static void Serialize<TValue>(Stream utf8Json, TValue value, JsonTypeInfo<TValue> jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		jsonTypeInfo.Serialize(utf8Json, in value);
	}

	public static Task SerializeAsync(Stream utf8Json, object? value, JsonTypeInfo jsonTypeInfo, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return jsonTypeInfo.SerializeAsObjectAsync(utf8Json, value, cancellationToken);
	}

	public static void Serialize(Stream utf8Json, object? value, JsonTypeInfo jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		jsonTypeInfo.SerializeAsObject(utf8Json, value);
	}

	public static Task SerializeAsync(Stream utf8Json, object? value, Type inputType, JsonSerializerContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArgumentNullException.ThrowIfNull(context, "context");
		ValidateInputType(value, inputType);
		return GetTypeInfo(context, inputType).SerializeAsObjectAsync(utf8Json, value, cancellationToken);
	}

	public static void Serialize(Stream utf8Json, object? value, Type inputType, JsonSerializerContext context)
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArgumentNullException.ThrowIfNull(context, "context");
		ValidateInputType(value, inputType);
		GetTypeInfo(context, inputType).SerializeAsObject(utf8Json, value);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static string Serialize<TValue>(TValue value, JsonSerializerOptions? options = null)
	{
		JsonTypeInfo<TValue> typeInfo = GetTypeInfo<TValue>(options);
		return WriteString<TValue>(in value, typeInfo);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static string Serialize(object? value, Type inputType, JsonSerializerOptions? options = null)
	{
		ValidateInputType(value, inputType);
		JsonTypeInfo typeInfo = GetTypeInfo(options, inputType);
		return WriteStringAsObject(value, typeInfo);
	}

	public static string Serialize<TValue>(TValue value, JsonTypeInfo<TValue> jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return WriteString(in value, jsonTypeInfo);
	}

	public static string Serialize(object? value, JsonTypeInfo jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return WriteStringAsObject(value, jsonTypeInfo);
	}

	public static string Serialize(object? value, Type inputType, JsonSerializerContext context)
	{
		ArgumentNullException.ThrowIfNull(context, "context");
		ValidateInputType(value, inputType);
		JsonTypeInfo typeInfo = GetTypeInfo(context, inputType);
		return WriteStringAsObject(value, typeInfo);
	}

	private static string WriteString<TValue>(in TValue value, JsonTypeInfo<TValue> jsonTypeInfo)
	{
		Utf8JsonWriter writer = Utf8JsonWriterCache.RentWriterAndBuffer(jsonTypeInfo.Options, out var bufferWriter);
		try
		{
			jsonTypeInfo.Serialize(writer, in value);
			return JsonReaderHelper.TranscodeHelper(bufferWriter.WrittenSpan);
		}
		finally
		{
			Utf8JsonWriterCache.ReturnWriterAndBuffer(writer, bufferWriter);
		}
	}

	private static string WriteStringAsObject(object value, JsonTypeInfo jsonTypeInfo)
	{
		Utf8JsonWriter writer = Utf8JsonWriterCache.RentWriterAndBuffer(jsonTypeInfo.Options, out var bufferWriter);
		try
		{
			jsonTypeInfo.SerializeAsObject(writer, value);
			return JsonReaderHelper.TranscodeHelper(bufferWriter.WrittenSpan);
		}
		finally
		{
			Utf8JsonWriterCache.ReturnWriterAndBuffer(writer, bufferWriter);
		}
	}

	public static Task SerializeAsync<TValue>(PipeWriter utf8Json, TValue value, JsonTypeInfo<TValue> jsonTypeInfo, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return jsonTypeInfo.SerializeAsync(utf8Json, value, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static Task SerializeAsync<TValue>(PipeWriter utf8Json, TValue value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		return GetTypeInfo<TValue>(options).SerializeAsync(utf8Json, value, cancellationToken);
	}

	public static Task SerializeAsync(PipeWriter utf8Json, object? value, JsonTypeInfo jsonTypeInfo, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		return jsonTypeInfo.SerializeAsObjectAsync(utf8Json, value, cancellationToken);
	}

	public static Task SerializeAsync(PipeWriter utf8Json, object? value, Type inputType, JsonSerializerContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArgumentNullException.ThrowIfNull(context, "context");
		ValidateInputType(value, inputType);
		return GetTypeInfo(context, inputType).SerializeAsObjectAsync(utf8Json, value, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static Task SerializeAsync(PipeWriter utf8Json, object? value, Type inputType, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ValidateInputType(value, inputType);
		return GetTypeInfo(options, inputType).SerializeAsObjectAsync(utf8Json, value, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static void Serialize<TValue>(Utf8JsonWriter writer, TValue value, JsonSerializerOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(writer, "writer");
		GetTypeInfo<TValue>(options).Serialize(writer, in value);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static void Serialize(Utf8JsonWriter writer, object? value, Type inputType, JsonSerializerOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(writer, "writer");
		ValidateInputType(value, inputType);
		GetTypeInfo(options, inputType).SerializeAsObject(writer, value);
	}

	public static void Serialize<TValue>(Utf8JsonWriter writer, TValue value, JsonTypeInfo<TValue> jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(writer, "writer");
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		jsonTypeInfo.Serialize(writer, in value);
	}

	public static void Serialize(Utf8JsonWriter writer, object? value, JsonTypeInfo jsonTypeInfo)
	{
		ArgumentNullException.ThrowIfNull(writer, "writer");
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		jsonTypeInfo.EnsureConfigured();
		jsonTypeInfo.SerializeAsObject(writer, value);
	}

	public static void Serialize(Utf8JsonWriter writer, object? value, Type inputType, JsonSerializerContext context)
	{
		ArgumentNullException.ThrowIfNull(writer, "writer");
		ArgumentNullException.ThrowIfNull(context, "context");
		ValidateInputType(value, inputType);
		GetTypeInfo(context, inputType).SerializeAsObject(writer, value);
	}
}
