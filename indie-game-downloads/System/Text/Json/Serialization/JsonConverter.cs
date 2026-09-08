using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using System.Reflection;
using System.Text.Json.Schema;
using System.Text.Json.Serialization.Converters;
using System.Text.Json.Serialization.Metadata;

namespace System.Text.Json.Serialization;

public abstract class JsonConverter
{
	private ConverterStrategy _converterStrategy;

	public abstract Type? Type { get; }

	internal ConverterStrategy ConverterStrategy
	{
		get
		{
			return _converterStrategy;
		}
		init
		{
			CanUseDirectReadOrWrite = value == ConverterStrategy.Value && IsInternalConverter;
			RequiresReadAhead = value == ConverterStrategy.Value;
			_converterStrategy = value;
		}
	}

	internal virtual bool SupportsCreateObjectDelegate => false;

	internal virtual bool CanPopulate => false;

	internal bool CanUseDirectReadOrWrite { get; set; }

	internal virtual bool CanHaveMetadata => false;

	internal bool CanBePolymorphic { get; init; }

	internal bool RequiresReadAhead { get; private protected set; }

	internal bool IsRootLevelMultiContentStreamingConverter { get; init; }

	internal bool UsesDefaultHandleNull { get; private protected set; }

	internal bool HandleNullOnRead { get; private protected init; }

	internal bool HandleNullOnWrite { get; private protected init; }

	internal virtual JsonConverter? SourceConverterForCastingConverter => null;

	internal virtual Type? ElementType => null;

	internal virtual Type? KeyType => null;

	internal virtual JsonConverter? NullableElementConverter => null;

	internal bool IsValueType { get; init; }

	internal bool IsInternalConverter { get; init; }

	internal bool IsInternalConverterForNumberType { get; init; }

	internal virtual bool IsConvertibleCollection => false;

	internal virtual bool ConstructorIsParameterized { get; }

	internal ConstructorInfo? ConstructorInfo { get; set; }

	internal JsonConverter()
	{
		IsInternalConverter = GetType().Assembly == typeof(JsonConverter).Assembly;
		ConverterStrategy = GetDefaultConverterStrategy();
	}

	public abstract bool CanConvert(Type typeToConvert);

	private protected abstract ConverterStrategy GetDefaultConverterStrategy();

	internal virtual void ReadElementAndSetProperty(object obj, string propertyName, ref Utf8JsonReader reader, JsonSerializerOptions options, scoped ref ReadStack state)
	{
		throw new InvalidOperationException();
	}

	internal virtual JsonTypeInfo CreateJsonTypeInfo(JsonSerializerOptions options)
	{
		throw new InvalidOperationException();
	}

	internal JsonConverter<TTarget> CreateCastingConverter<TTarget>()
	{
		if (this is JsonConverter<TTarget> result)
		{
			return result;
		}
		JsonSerializerOptions.CheckConverterNullabilityIsSameAsPropertyType(this, typeof(TTarget));
		return SourceConverterForCastingConverter?.CreateCastingConverter<TTarget>() ?? new CastingConverter<TTarget>(this);
	}

	internal static bool ShouldFlush(ref WriteStack state, Utf8JsonWriter writer)
	{
		PipeWriter pipeWriter = state.PipeWriter;
		if (pipeWriter != null)
		{
			if (state.FlushThreshold > 0)
			{
				return pipeWriter.UnflushedBytes > state.FlushThreshold - writer.BytesPending;
			}
			return false;
		}
		return false;
	}

	internal abstract object ReadAsObject(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options);

	internal abstract bool OnTryReadAsObject(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options, scoped ref ReadStack state, out object value);

	internal abstract bool TryReadAsObject(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options, scoped ref ReadStack state, out object value);

	internal abstract object ReadAsPropertyNameAsObject(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options);

	internal abstract object ReadAsPropertyNameCoreAsObject(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options);

	internal abstract object ReadNumberWithCustomHandlingAsObject(ref Utf8JsonReader reader, JsonNumberHandling handling, JsonSerializerOptions options);

	internal abstract void WriteAsObject(Utf8JsonWriter writer, object value, JsonSerializerOptions options);

	internal abstract bool OnTryWriteAsObject(Utf8JsonWriter writer, object value, JsonSerializerOptions options, ref WriteStack state);

	internal abstract bool TryWriteAsObject(Utf8JsonWriter writer, object value, JsonSerializerOptions options, ref WriteStack state);

	internal abstract void WriteAsPropertyNameAsObject(Utf8JsonWriter writer, object value, JsonSerializerOptions options);

	internal abstract void WriteAsPropertyNameCoreAsObject(Utf8JsonWriter writer, object value, JsonSerializerOptions options, bool isWritingExtensionDataProperty);

	internal abstract void WriteNumberWithCustomHandlingAsObject(Utf8JsonWriter writer, object value, JsonNumberHandling handling);

	internal virtual JsonSchema GetSchema(JsonNumberHandling numberHandling)
	{
		return null;
	}

	internal virtual void ConfigureJsonTypeInfo(JsonTypeInfo jsonTypeInfo, JsonSerializerOptions options)
	{
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	internal virtual void ConfigureJsonTypeInfoUsingReflection(JsonTypeInfo jsonTypeInfo, JsonSerializerOptions options)
	{
	}

	internal JsonConverter ResolvePolymorphicConverter(JsonTypeInfo jsonTypeInfo, ref ReadStack state)
	{
		JsonConverter jsonConverter = null;
		switch (state.Current.PolymorphicSerializationState)
		{
		case PolymorphicSerializationState.None:
		{
			if (jsonTypeInfo.PolymorphicTypeResolver.TryGetDerivedJsonTypeInfo(state.PolymorphicTypeDiscriminator, out var jsonTypeInfo2))
			{
				jsonConverter = state.InitializePolymorphicReEntry(jsonTypeInfo2);
				if (!jsonConverter.CanHaveMetadata)
				{
					ThrowHelper.ThrowNotSupportedException_DerivedConverterDoesNotSupportMetadata(jsonTypeInfo2.Type);
				}
			}
			else
			{
				state.Current.PolymorphicSerializationState = PolymorphicSerializationState.PolymorphicReEntryNotFound;
			}
			state.PolymorphicTypeDiscriminator = null;
			break;
		}
		case PolymorphicSerializationState.PolymorphicReEntrySuspended:
			jsonConverter = state.ResumePolymorphicReEntry();
			break;
		}
		return jsonConverter;
	}

	internal JsonConverter ResolvePolymorphicConverter(object value, JsonTypeInfo jsonTypeInfo, JsonSerializerOptions options, ref WriteStack state)
	{
		JsonConverter jsonConverter = null;
		switch (state.Current.PolymorphicSerializationState)
		{
		case PolymorphicSerializationState.None:
		{
			Type type = value.GetType();
			if (CanBePolymorphic && type != Type)
			{
				jsonTypeInfo = state.Current.InitializePolymorphicReEntry(type, options);
				jsonConverter = jsonTypeInfo.Converter;
			}
			PolymorphicTypeResolver polymorphicTypeResolver = jsonTypeInfo.PolymorphicTypeResolver;
			if (polymorphicTypeResolver != null && polymorphicTypeResolver.TryGetDerivedJsonTypeInfo(type, out var jsonTypeInfo2, out var typeDiscriminator))
			{
				jsonConverter = state.Current.InitializePolymorphicReEntry(jsonTypeInfo2);
				if (typeDiscriminator != null)
				{
					if (!jsonConverter.CanHaveMetadata)
					{
						ThrowHelper.ThrowNotSupportedException_DerivedConverterDoesNotSupportMetadata(jsonTypeInfo2.Type);
					}
					state.PolymorphicTypeDiscriminator = typeDiscriminator;
					state.PolymorphicTypeResolver = polymorphicTypeResolver;
				}
			}
			if (jsonConverter == null)
			{
				state.Current.PolymorphicSerializationState = PolymorphicSerializationState.PolymorphicReEntryNotFound;
			}
			break;
		}
		case PolymorphicSerializationState.PolymorphicReEntrySuspended:
			jsonConverter = state.Current.ResumePolymorphicReEntry();
			break;
		}
		return jsonConverter;
	}

	internal bool TryHandleSerializedObjectReference(Utf8JsonWriter writer, object value, JsonSerializerOptions options, JsonConverter polymorphicConverter, ref WriteStack state)
	{
		switch (options.ReferenceHandlingStrategy)
		{
		case JsonKnownReferenceHandler.IgnoreCycles:
		{
			ReferenceResolver referenceResolver = state.ReferenceResolver;
			if (referenceResolver.ContainsReferenceForCycleDetection(value))
			{
				writer.WriteNullValue();
				if (polymorphicConverter != null)
				{
					state.PolymorphicTypeDiscriminator = null;
					state.PolymorphicTypeResolver = null;
				}
				return true;
			}
			referenceResolver.PushReferenceForCycleDetection(value);
			state.Current.IsPushedReferenceForCycleDetection = state.CurrentDepth > 0;
			break;
		}
		case JsonKnownReferenceHandler.Preserve:
			if ((polymorphicConverter?.CanHaveMetadata ?? CanHaveMetadata) && JsonSerializer.TryGetReferenceForValue(value, ref state, writer))
			{
				return true;
			}
			break;
		}
		return false;
	}
}
public abstract class JsonConverter<T> : JsonConverter
{
	private JsonConverter<T> _fallbackConverterForPropertyNameSerialization;

	public virtual bool HandleNull
	{
		get
		{
			base.UsesDefaultHandleNull = true;
			return false;
		}
	}

	public sealed override Type Type { get; } = typeof(T);

	internal bool ReadCore(ref Utf8JsonReader reader, out T value, JsonSerializerOptions options, ref ReadStack state)
	{
		try
		{
			if (!state.IsContinuation && !base.IsRootLevelMultiContentStreamingConverter && !reader.TryAdvanceWithOptionalReadAhead(base.RequiresReadAhead))
			{
				if (state.SupportContinuation)
				{
					object returnValue = state.Current.ReturnValue;
					if (returnValue != null)
					{
						value = (T)returnValue;
						goto IL_004d;
					}
				}
				value = default(T);
				goto IL_004d;
			}
			bool flag = TryRead(ref reader, state.Current.JsonTypeInfo.Type, options, ref state, out value, out var _);
			if (flag && !reader.AllowMultipleValues && !reader.Read() && !reader.IsFinalBlock)
			{
				state.Current.ReturnValue = value;
				flag = false;
			}
			return flag;
			IL_004d:
			return reader.IsFinalBlock;
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			if (!(ex2 is JsonReaderException ex3))
			{
				if (!(ex2 is FormatException))
				{
					if (!(ex2 is InvalidOperationException))
					{
						if (!(ex2 is JsonException ex4))
						{
							if (ex2 is NotSupportedException && !ex.Message.Contains(" Path: "))
							{
								ThrowHelper.ThrowNotSupportedException(ref state, in reader, ex);
							}
						}
						else if (ex4.Path == null)
						{
							ThrowHelper.AddJsonExceptionInformation(ref state, in reader, ex4);
						}
					}
					else if (ex.Source == "System.Text.Json.Rethrowable")
					{
						ThrowHelper.ReThrowWithPath(ref state, in reader, ex);
					}
				}
				else if (ex.Source == "System.Text.Json.Rethrowable")
				{
					ThrowHelper.ReThrowWithPath(ref state, in reader, ex);
				}
			}
			else
			{
				ThrowHelper.ReThrowWithPath(ref state, ex3);
			}
			throw;
		}
	}

	internal bool WriteCore(Utf8JsonWriter writer, in T value, JsonSerializerOptions options, ref WriteStack state)
	{
		try
		{
			return TryWrite(writer, in value, options, ref state);
		}
		catch (Exception ex)
		{
			if (!state.SupportAsync)
			{
				state.DisposePendingDisposablesOnException();
			}
			Exception ex2 = ex;
			if (!(ex2 is InvalidOperationException))
			{
				if (ex2 is JsonException ex3)
				{
					if (ex3.Path == null)
					{
						ThrowHelper.AddJsonExceptionInformation(ref state, ex3);
					}
				}
				else if (ex2 is NotSupportedException && !ex.Message.Contains(" Path: "))
				{
					ThrowHelper.ThrowNotSupportedException(ref state, ex);
				}
			}
			else if (ex.Source == "System.Text.Json.Rethrowable")
			{
				ThrowHelper.ReThrowWithPath(ref state, ex);
			}
			throw;
		}
	}

	protected internal JsonConverter()
	{
		base.IsValueType = typeof(T).IsValueType;
		if (HandleNull)
		{
			base.HandleNullOnRead = true;
			base.HandleNullOnWrite = true;
		}
		else if (base.UsesDefaultHandleNull)
		{
			base.HandleNullOnRead = default(T) != null;
			base.HandleNullOnWrite = false;
		}
	}

	public override bool CanConvert(Type typeToConvert)
	{
		return typeToConvert == typeof(T);
	}

	private protected override ConverterStrategy GetDefaultConverterStrategy()
	{
		return ConverterStrategy.Value;
	}

	internal sealed override JsonTypeInfo CreateJsonTypeInfo(JsonSerializerOptions options)
	{
		return new JsonTypeInfo<T>(this, options);
	}

	internal sealed override void WriteAsObject(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
	{
		T value2 = JsonSerializer.UnboxOnWrite<T>(value);
		Write(writer, value2, options);
	}

	internal sealed override bool OnTryWriteAsObject(Utf8JsonWriter writer, object value, JsonSerializerOptions options, ref WriteStack state)
	{
		T value2 = JsonSerializer.UnboxOnWrite<T>(value);
		return OnTryWrite(writer, value2, options, ref state);
	}

	internal sealed override void WriteAsPropertyNameAsObject(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
	{
		T value2 = JsonSerializer.UnboxOnWrite<T>(value);
		WriteAsPropertyName(writer, value2, options);
	}

	internal sealed override void WriteAsPropertyNameCoreAsObject(Utf8JsonWriter writer, object value, JsonSerializerOptions options, bool isWritingExtensionDataProperty)
	{
		T value2 = JsonSerializer.UnboxOnWrite<T>(value);
		WriteAsPropertyNameCore(writer, value2, options, isWritingExtensionDataProperty);
	}

	internal sealed override void WriteNumberWithCustomHandlingAsObject(Utf8JsonWriter writer, object value, JsonNumberHandling handling)
	{
		T value2 = JsonSerializer.UnboxOnWrite<T>(value);
		WriteNumberWithCustomHandling(writer, value2, handling);
	}

	internal sealed override bool TryWriteAsObject(Utf8JsonWriter writer, object value, JsonSerializerOptions options, ref WriteStack state)
	{
		return TryWrite(writer, JsonSerializer.UnboxOnWrite<T>(value), options, ref state);
	}

	internal virtual bool OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, ref WriteStack state)
	{
		Write(writer, value, options);
		return true;
	}

	internal virtual bool OnTryRead(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options, scoped ref ReadStack state, out T value)
	{
		value = Read(ref reader, typeToConvert, options);
		return true;
	}

	public abstract T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options);

	internal bool TryRead(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options, scoped ref ReadStack state, out T value, out bool isPopulatedValue)
	{
		if (reader.TokenType == JsonTokenType.Null && !base.HandleNullOnRead && !state.IsContinuation)
		{
			if (default(T) != null)
			{
				ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(Type);
			}
			value = default(T);
			isPopulatedValue = false;
			return true;
		}
		if (base.ConverterStrategy == ConverterStrategy.Value)
		{
			if (base.IsInternalConverter)
			{
				if (state.Current.NumberHandling.HasValue && base.IsInternalConverterForNumberType)
				{
					value = ReadNumberWithCustomHandling(ref reader, state.Current.NumberHandling.Value, options);
				}
				else
				{
					value = Read(ref reader, typeToConvert, options);
				}
			}
			else
			{
				JsonTokenType tokenType = reader.TokenType;
				int currentDepth = reader.CurrentDepth;
				long bytesConsumed = reader.BytesConsumed;
				if (state.Current.NumberHandling.HasValue && base.IsInternalConverterForNumberType)
				{
					value = ReadNumberWithCustomHandling(ref reader, state.Current.NumberHandling.Value, options);
				}
				else
				{
					value = Read(ref reader, typeToConvert, options);
				}
				VerifyRead(tokenType, currentDepth, bytesConsumed, isValueConverter: true, ref reader);
			}
			isPopulatedValue = false;
			return true;
		}
		bool isContinuation = state.IsContinuation;
		bool flag;
		if (!typeof(T).IsValueType && base.CanBePolymorphic)
		{
			flag = OnTryRead(ref reader, typeToConvert, options, ref state, out value);
			isPopulatedValue = false;
			return true;
		}
		JsonPropertyInfo jsonPropertyInfo = state.Current.JsonPropertyInfo;
		object returnValue = state.Current.ReturnValue;
		state.Push();
		if (!isContinuation)
		{
			state.Current.OriginalDepth = reader.CurrentDepth;
		}
		if (returnValue != null && jsonPropertyInfo != null && !jsonPropertyInfo.IsForTypeInfo)
		{
			state.Current.HasParentObject = true;
		}
		flag = OnTryRead(ref reader, typeToConvert, options, ref state, out value);
		isPopulatedValue = state.Current.IsPopulating;
		state.Pop(flag);
		return flag;
	}

	internal sealed override bool OnTryReadAsObject(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options, scoped ref ReadStack state, out object value)
	{
		bool result = OnTryRead(ref reader, typeToConvert, options, ref state, out var value2);
		value = value2;
		return result;
	}

	internal sealed override bool TryReadAsObject(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options, scoped ref ReadStack state, out object value)
	{
		bool result = TryRead(ref reader, typeToConvert, options, ref state, out var value2, out var _);
		value = value2;
		return result;
	}

	internal sealed override object ReadAsObject(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return Read(ref reader, typeToConvert, options);
	}

	internal sealed override object ReadAsPropertyNameAsObject(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return ReadAsPropertyName(ref reader, typeToConvert, options);
	}

	internal sealed override object ReadAsPropertyNameCoreAsObject(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return ReadAsPropertyNameCore(ref reader, typeToConvert, options);
	}

	internal sealed override object ReadNumberWithCustomHandlingAsObject(ref Utf8JsonReader reader, JsonNumberHandling handling, JsonSerializerOptions options)
	{
		return ReadNumberWithCustomHandling(ref reader, handling, options);
	}

	private static bool IsNull(T value)
	{
		return value == null;
	}

	internal bool TryWrite(Utf8JsonWriter writer, in T value, JsonSerializerOptions options, ref WriteStack state)
	{
		if (writer.CurrentDepth >= options.EffectiveMaxDepth)
		{
			ThrowHelper.ThrowJsonException_SerializerCycleDetected(options.EffectiveMaxDepth);
		}
		if (default(T) == null && !base.HandleNullOnWrite && IsNull(value))
		{
			writer.WriteNullValue();
			return true;
		}
		if (base.ConverterStrategy == ConverterStrategy.Value)
		{
			int currentDepth = writer.CurrentDepth;
			if (state.Current.NumberHandling.HasValue && base.IsInternalConverterForNumberType)
			{
				WriteNumberWithCustomHandling(writer, value, state.Current.NumberHandling.Value);
			}
			else
			{
				Write(writer, value, options);
			}
			VerifyWrite(currentDepth, writer);
			return true;
		}
		bool isContinuation = state.IsContinuation;
		bool flag;
		if (!typeof(T).IsValueType && value != null && state.Current.PolymorphicSerializationState != PolymorphicSerializationState.PolymorphicReEntryStarted)
		{
			JsonTypeInfo jsonTypeInfo = state.PeekNestedJsonTypeInfo();
			JsonConverter jsonConverter = ((base.CanBePolymorphic || jsonTypeInfo.PolymorphicTypeResolver != null) ? ResolvePolymorphicConverter(value, jsonTypeInfo, options, ref state) : null);
			if (!isContinuation && options.ReferenceHandlingStrategy != JsonKnownReferenceHandler.Unspecified && TryHandleSerializedObjectReference(writer, value, options, jsonConverter, ref state))
			{
				return true;
			}
			if (jsonConverter != null)
			{
				flag = jsonConverter.TryWriteAsObject(writer, value, options, ref state);
				state.Current.ExitPolymorphicConverter(flag);
				if (flag && state.Current.IsPushedReferenceForCycleDetection)
				{
					state.ReferenceResolver.PopReferenceForCycleDetection();
					state.Current.IsPushedReferenceForCycleDetection = false;
				}
				return flag;
			}
		}
		state.Push();
		flag = OnTryWrite(writer, value, options, ref state);
		state.Pop(flag);
		if (flag && state.Current.IsPushedReferenceForCycleDetection)
		{
			state.ReferenceResolver.PopReferenceForCycleDetection();
			state.Current.IsPushedReferenceForCycleDetection = false;
		}
		return flag;
	}

	internal bool TryWriteDataExtensionProperty(Utf8JsonWriter writer, T value, JsonSerializerOptions options, ref WriteStack state)
	{
		if (!base.IsInternalConverter)
		{
			return TryWrite(writer, in value, options, ref state);
		}
		JsonDictionaryConverter<T> jsonDictionaryConverter = (this as JsonDictionaryConverter<T>) ?? ((this as JsonMetadataServicesConverter<T>)?.Converter as JsonDictionaryConverter<T>);
		if (jsonDictionaryConverter == null)
		{
			return TryWrite(writer, in value, options, ref state);
		}
		if (writer.CurrentDepth >= options.EffectiveMaxDepth)
		{
			ThrowHelper.ThrowJsonException_SerializerCycleDetected(options.EffectiveMaxDepth);
		}
		bool isContinuation = state.IsContinuation;
		state.Push();
		if (!isContinuation)
		{
			state.Current.OriginalDepth = writer.CurrentDepth;
		}
		state.Current.IsWritingExtensionDataProperty = true;
		state.Current.JsonPropertyInfo = state.Current.JsonTypeInfo.ElementTypeInfo.PropertyInfoForTypeInfo;
		bool flag = jsonDictionaryConverter.OnWriteResume(writer, value, options, ref state);
		if (flag)
		{
			VerifyWrite(state.Current.OriginalDepth, writer);
		}
		state.Pop(flag);
		return flag;
	}

	internal void VerifyRead(JsonTokenType tokenType, int depth, long bytesConsumed, bool isValueConverter, ref Utf8JsonReader reader)
	{
		switch (tokenType)
		{
		case JsonTokenType.StartArray:
			if (reader.TokenType != JsonTokenType.EndArray)
			{
				ThrowHelper.ThrowJsonException_SerializationConverterRead(this);
			}
			else if (depth != reader.CurrentDepth)
			{
				ThrowHelper.ThrowJsonException_SerializationConverterRead(this);
			}
			return;
		case JsonTokenType.StartObject:
			if (reader.TokenType != JsonTokenType.EndObject)
			{
				ThrowHelper.ThrowJsonException_SerializationConverterRead(this);
			}
			else if (depth != reader.CurrentDepth)
			{
				ThrowHelper.ThrowJsonException_SerializationConverterRead(this);
			}
			return;
		case JsonTokenType.None:
			return;
		}
		if (isValueConverter)
		{
			if (reader.BytesConsumed != bytesConsumed)
			{
				ThrowHelper.ThrowJsonException_SerializationConverterRead(this);
			}
		}
		else if (!base.CanBePolymorphic && (!base.HandleNullOnRead || tokenType != JsonTokenType.Null))
		{
			ThrowHelper.ThrowJsonException_SerializationConverterRead(this);
		}
	}

	internal void VerifyWrite(int originalDepth, Utf8JsonWriter writer)
	{
		if (originalDepth != writer.CurrentDepth)
		{
			ThrowHelper.ThrowJsonException_SerializationConverterWrite(this);
		}
	}

	public abstract void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options);

	public virtual T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		JsonConverter<T> fallbackConverterForPropertyNameSerialization = GetFallbackConverterForPropertyNameSerialization(options);
		if (fallbackConverterForPropertyNameSerialization == null)
		{
			ThrowHelper.ThrowNotSupportedException_DictionaryKeyTypeNotSupported(Type, this);
		}
		return fallbackConverterForPropertyNameSerialization.ReadAsPropertyNameCore(ref reader, typeToConvert, options);
	}

	internal virtual T ReadAsPropertyNameCore(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		long bytesConsumed = reader.BytesConsumed;
		T result = ReadAsPropertyName(ref reader, typeToConvert, options);
		if (reader.BytesConsumed != bytesConsumed)
		{
			ThrowHelper.ThrowJsonException_SerializationConverterRead(this);
		}
		return result;
	}

	public virtual void WriteAsPropertyName(Utf8JsonWriter writer, [DisallowNull] T value, JsonSerializerOptions options)
	{
		JsonConverter<T> fallbackConverterForPropertyNameSerialization = GetFallbackConverterForPropertyNameSerialization(options);
		if (fallbackConverterForPropertyNameSerialization == null)
		{
			ThrowHelper.ThrowNotSupportedException_DictionaryKeyTypeNotSupported(Type, this);
		}
		fallbackConverterForPropertyNameSerialization.WriteAsPropertyNameCore(writer, value, options, isWritingExtensionDataProperty: false);
	}

	internal virtual void WriteAsPropertyNameCore(Utf8JsonWriter writer, [DisallowNull] T value, JsonSerializerOptions options, bool isWritingExtensionDataProperty)
	{
		ArgumentNullException.ThrowIfNull(value, "value");
		if (isWritingExtensionDataProperty)
		{
			writer.WritePropertyName((string)(object)value);
			return;
		}
		int currentDepth = writer.CurrentDepth;
		WriteAsPropertyName(writer, value, options);
		if (currentDepth != writer.CurrentDepth || writer.TokenType != JsonTokenType.PropertyName)
		{
			ThrowHelper.ThrowJsonException_SerializationConverterWrite(this);
		}
	}

	private JsonConverter<T> GetFallbackConverterForPropertyNameSerialization(JsonSerializerOptions options)
	{
		JsonConverter<T> jsonConverter = null;
		if (!base.IsInternalConverter && !(options.TypeInfoResolver is JsonSerializerContext))
		{
			jsonConverter = _fallbackConverterForPropertyNameSerialization;
			if (jsonConverter == null && DefaultJsonTypeInfoResolver.TryGetDefaultSimpleConverter(Type, out var converter))
			{
				jsonConverter = (_fallbackConverterForPropertyNameSerialization = (JsonConverter<T>)converter);
			}
		}
		return jsonConverter;
	}

	internal virtual T ReadNumberWithCustomHandling(ref Utf8JsonReader reader, JsonNumberHandling handling, JsonSerializerOptions options)
	{
		throw new InvalidOperationException();
	}

	internal virtual void WriteNumberWithCustomHandling(Utf8JsonWriter writer, T value, JsonNumberHandling handling)
	{
		throw new InvalidOperationException();
	}
}
