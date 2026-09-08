using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Pipelines;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text.Json.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace System.Text.Json.Serialization.Metadata;

public sealed class JsonTypeInfo<T> : JsonTypeInfo
{
	internal JsonTypeInfo _asyncEnumerableArrayTypeInfo;

	internal JsonTypeInfo _asyncEnumerableRootLevelValueTypeInfo;

	private volatile int _canUseSerializeHandlerInStreamingState;

	private volatile int _serializationCount;

	private Action<Utf8JsonWriter, T> _serialize;

	private Func<T> _typedCreateObject;

	private bool CanUseSerializeHandlerInStreaming => _canUseSerializeHandlerInStreamingState == 1;

	internal JsonConverter<T> EffectiveConverter { get; }

	public new Func<T>? CreateObject
	{
		get
		{
			return _typedCreateObject;
		}
		set
		{
			SetCreateObject(value);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public Action<Utf8JsonWriter, T>? SerializeHandler
	{
		get
		{
			return _serialize;
		}
		internal set
		{
			_serialize = value;
			base.HasSerializeHandler = value != null;
		}
	}

	internal T Deserialize(ref Utf8JsonReader reader, ref ReadStack state)
	{
		EffectiveConverter.ReadCore(ref reader, out var value, base.Options, ref state);
		return value;
	}

	internal async ValueTask<T> DeserializeAsync<TReadBufferState, TStream>(TStream utf8Json, TReadBufferState bufferState, CancellationToken cancellationToken) where TReadBufferState : struct, IReadBufferState<TReadBufferState, TStream>
	{
		JsonSerializerOptions options = base.Options;
		ReadStack readStack = default(ReadStack);
		readStack.Initialize(this, supportContinuation: true);
		JsonReaderState jsonReaderState = new JsonReaderState(options.GetReaderOptions());
		try
		{
			T value;
			do
			{
				bufferState = await bufferState.ReadAsync(utf8Json, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			while (!ContinueDeserialize<TReadBufferState, TStream>(ref bufferState, ref jsonReaderState, ref readStack, out value));
			return value;
		}
		finally
		{
			bufferState.Dispose();
		}
	}

	internal ValueTask<T> DeserializeAsync(Stream utf8Json, CancellationToken cancellationToken)
	{
		StreamReadBufferState bufferState = new StreamReadBufferState(base.Options.DefaultBufferSize);
		return DeserializeAsync(utf8Json, bufferState, cancellationToken);
	}

	internal ValueTask<T> DeserializeAsync(PipeReader utf8Json, CancellationToken cancellationToken)
	{
		PipeReadBufferState bufferState = new PipeReadBufferState(utf8Json);
		return DeserializeAsync(utf8Json, bufferState, cancellationToken);
	}

	internal T Deserialize(Stream utf8Json)
	{
		JsonSerializerOptions options = base.Options;
		ReadStack readStack = default(ReadStack);
		readStack.Initialize(this, supportContinuation: true);
		JsonReaderState jsonReaderState = new JsonReaderState(options.GetReaderOptions());
		StreamReadBufferState bufferState = new StreamReadBufferState(options.DefaultBufferSize);
		try
		{
			T value;
			do
			{
				bufferState.Read(utf8Json);
			}
			while (!ContinueDeserialize<StreamReadBufferState, Stream>(ref bufferState, ref jsonReaderState, ref readStack, out value));
			return value;
		}
		finally
		{
			bufferState.Dispose();
		}
	}

	internal sealed override object DeserializeAsObject(ref Utf8JsonReader reader, ref ReadStack state)
	{
		return Deserialize(ref reader, ref state);
	}

	internal sealed override async ValueTask<object> DeserializeAsObjectAsync(Stream utf8Json, CancellationToken cancellationToken)
	{
		StreamReadBufferState bufferState = new StreamReadBufferState(base.Options.DefaultBufferSize);
		return await DeserializeAsync(utf8Json, bufferState, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	internal sealed override async ValueTask<object> DeserializeAsObjectAsync(PipeReader utf8Json, CancellationToken cancellationToken)
	{
		return await DeserializeAsync(utf8Json, new PipeReadBufferState(utf8Json), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	internal sealed override object DeserializeAsObject(Stream utf8Json)
	{
		return Deserialize(utf8Json);
	}

	internal bool ContinueDeserialize<TReadBufferState, TStream>(ref TReadBufferState bufferState, ref JsonReaderState jsonReaderState, ref ReadStack readStack, out T value) where TReadBufferState : struct, IReadBufferState<TReadBufferState, TStream>
	{
		bufferState.GetReader(jsonReaderState, out var reader);
		try
		{
			bool result = EffectiveConverter.ReadCore(ref reader, out value, base.Options, ref readStack);
			jsonReaderState = reader.CurrentState;
			return result;
		}
		finally
		{
			bufferState.Advance(reader.BytesConsumed);
		}
	}

	internal void Serialize(Utf8JsonWriter writer, in T rootValue, object rootValueBoxed = null)
	{
		if (base.CanUseSerializeHandler)
		{
			SerializeHandler(writer, rootValue);
			writer.Flush();
			return;
		}
		if (!typeof(T).IsValueType && base.Converter.CanBePolymorphic && rootValue != null && base.Options.TryGetPolymorphicTypeInfoForRootType(rootValue, out var polymorphicTypeInfo))
		{
			polymorphicTypeInfo.SerializeAsObject(writer, rootValue);
			return;
		}
		WriteStack state = default(WriteStack);
		state.Initialize(this, rootValueBoxed);
		EffectiveConverter.WriteCore(writer, in rootValue, base.Options, ref state);
		writer.Flush();
	}

	internal Task SerializeAsync(Stream utf8Json, T rootValue, CancellationToken cancellationToken, object rootValueBoxed = null)
	{
		PooledByteBufferWriter pooledByteBufferWriter = new PooledByteBufferWriter(base.Options.DefaultBufferSize, utf8Json);
		int flushThreshold = (int)((float)pooledByteBufferWriter.Capacity * 0.9f);
		return SerializeAsync(pooledByteBufferWriter, rootValue, flushThreshold, cancellationToken, rootValueBoxed);
	}

	internal Task SerializeAsync(PipeWriter utf8Json, T rootValue, CancellationToken cancellationToken, object rootValueBoxed = null)
	{
		int flushThreshold = (int)((float)(4 * PipeOptions.Default.MinimumSegmentSize) * 0.9f);
		return SerializeAsync(utf8Json, rootValue, flushThreshold, cancellationToken, rootValueBoxed);
	}

	private async Task SerializeAsync(PipeWriter pipeWriter, T rootValue, int flushThreshold, CancellationToken cancellationToken, object rootValueBoxed = null)
	{
		if (CanUseSerializeHandlerInStreaming)
		{
			Utf8JsonWriter utf8JsonWriter = Utf8JsonWriterCache.RentWriter(base.Options, pipeWriter);
			try
			{
				try
				{
					SerializeHandler(utf8JsonWriter, rootValue);
					utf8JsonWriter.Flush();
				}
				finally
				{
					OnRootLevelAsyncSerializationCompleted(utf8JsonWriter.BytesCommitted + utf8JsonWriter.BytesPending);
					Utf8JsonWriterCache.ReturnWriter(utf8JsonWriter);
				}
				if ((await pipeWriter.FlushAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false)).IsCanceled)
				{
					ThrowHelper.ThrowOperationCanceledException_PipeWriteCanceled();
				}
				return;
			}
			finally
			{
				if (pipeWriter is PooledByteBufferWriter pooledByteBufferWriter)
				{
					pooledByteBufferWriter.Dispose();
				}
			}
		}
		if (!typeof(T).IsValueType && base.Converter.CanBePolymorphic && rootValue != null && base.Options.TryGetPolymorphicTypeInfoForRootType(rootValue, out var polymorphicTypeInfo))
		{
			await polymorphicTypeInfo.SerializeAsObjectAsync(pipeWriter, rootValue, flushThreshold, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			return;
		}
		WriteStack state = default(WriteStack);
		state.Initialize(this, rootValueBoxed, supportContinuation: true, supportAsync: true);
		if (!pipeWriter.CanGetUnflushedBytes)
		{
			ThrowHelper.ThrowInvalidOperationException_PipeWriterDoesNotImplementUnflushedBytes(pipeWriter);
		}
		state.PipeWriter = pipeWriter;
		state.CancellationToken = cancellationToken;
		Utf8JsonWriter writer = new Utf8JsonWriter(pipeWriter, base.Options.GetWriterOptions());
		try
		{
			state.FlushThreshold = flushThreshold;
			bool isFinalBlock;
			do
			{
				try
				{
					isFinalBlock = EffectiveConverter.WriteCore(writer, in rootValue, base.Options, ref state);
					if (state.SuppressFlush)
					{
						state.SuppressFlush = false;
						continue;
					}
					writer.Flush();
					FlushResult flushResult = await pipeWriter.FlushAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					if (flushResult.IsCanceled || flushResult.IsCompleted)
					{
						if (flushResult.IsCanceled)
						{
							ThrowHelper.ThrowOperationCanceledException_PipeWriteCanceled();
						}
						return;
					}
				}
				finally
				{
					if (state.PendingTask != null)
					{
						await state.PendingTask.ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
					}
					List<IAsyncDisposable> completedAsyncDisposables = state.CompletedAsyncDisposables;
					if (completedAsyncDisposables != null && completedAsyncDisposables.Count > 0)
					{
						await state.DisposeCompletedAsyncDisposables().ConfigureAwait(continueOnCapturedContext: false);
					}
				}
			}
			while (!isFinalBlock);
			if (base.CanUseSerializeHandler)
			{
				OnRootLevelAsyncSerializationCompleted(writer.BytesCommitted);
			}
		}
		catch
		{
			writer.Reset();
			writer.Dispose();
			await state.DisposePendingDisposablesOnExceptionAsync().ConfigureAwait(continueOnCapturedContext: false);
			throw;
		}
		finally
		{
			writer.Dispose();
			if (pipeWriter is PooledByteBufferWriter pooledByteBufferWriter2)
			{
				pooledByteBufferWriter2.Dispose();
			}
		}
	}

	internal void Serialize(Stream utf8Json, in T rootValue, object rootValueBoxed = null)
	{
		if (CanUseSerializeHandlerInStreaming)
		{
			Utf8JsonWriter utf8JsonWriter = Utf8JsonWriterCache.RentWriterAndBuffer(base.Options, out var bufferWriter);
			try
			{
				SerializeHandler(utf8JsonWriter, rootValue);
				utf8JsonWriter.Flush();
				bufferWriter.WriteToStream(utf8Json);
				return;
			}
			finally
			{
				OnRootLevelAsyncSerializationCompleted(utf8JsonWriter.BytesCommitted + utf8JsonWriter.BytesPending);
				Utf8JsonWriterCache.ReturnWriterAndBuffer(utf8JsonWriter, bufferWriter);
			}
		}
		if (!typeof(T).IsValueType && base.Converter.CanBePolymorphic && rootValue != null && base.Options.TryGetPolymorphicTypeInfoForRootType(rootValue, out var polymorphicTypeInfo))
		{
			polymorphicTypeInfo.SerializeAsObject(utf8Json, rootValue);
			return;
		}
		WriteStack state = default(WriteStack);
		state.Initialize(this, rootValueBoxed, supportContinuation: true);
		Utf8JsonWriter utf8JsonWriter2 = Utf8JsonWriterCache.RentWriterAndBuffer(base.Options, out var bufferWriter2);
		try
		{
			state.PipeWriter = bufferWriter2;
			state.FlushThreshold = (int)((float)bufferWriter2.Capacity * 0.9f);
			bool flag;
			do
			{
				flag = EffectiveConverter.WriteCore(utf8JsonWriter2, in rootValue, base.Options, ref state);
				utf8JsonWriter2.Flush();
				bufferWriter2.WriteToStream(utf8Json);
				bufferWriter2.Clear();
			}
			while (!flag);
			if (base.CanUseSerializeHandler)
			{
				OnRootLevelAsyncSerializationCompleted(utf8JsonWriter2.BytesCommitted);
			}
		}
		finally
		{
			Utf8JsonWriterCache.ReturnWriterAndBuffer(utf8JsonWriter2, bufferWriter2);
		}
	}

	internal sealed override void SerializeAsObject(Utf8JsonWriter writer, object rootValue)
	{
		Serialize(writer, JsonSerializer.UnboxOnWrite<T>(rootValue), rootValue);
	}

	internal sealed override Task SerializeAsObjectAsync(PipeWriter pipeWriter, object rootValue, int flushThreshold, CancellationToken cancellationToken)
	{
		return SerializeAsync(pipeWriter, JsonSerializer.UnboxOnWrite<T>(rootValue), flushThreshold, cancellationToken, rootValue);
	}

	internal sealed override Task SerializeAsObjectAsync(Stream utf8Json, object rootValue, CancellationToken cancellationToken)
	{
		return SerializeAsync(utf8Json, JsonSerializer.UnboxOnWrite<T>(rootValue), cancellationToken, rootValue);
	}

	internal sealed override Task SerializeAsObjectAsync(PipeWriter utf8Json, object rootValue, CancellationToken cancellationToken)
	{
		return SerializeAsync(utf8Json, JsonSerializer.UnboxOnWrite<T>(rootValue), cancellationToken, rootValue);
	}

	internal sealed override void SerializeAsObject(Stream utf8Json, object rootValue)
	{
		Serialize(utf8Json, JsonSerializer.UnboxOnWrite<T>(rootValue), rootValue);
	}

	private void OnRootLevelAsyncSerializationCompleted(long serializationSize)
	{
		if (_canUseSerializeHandlerInStreamingState != 2)
		{
			if ((ulong)serializationSize > (ulong)(base.Options.DefaultBufferSize / 2))
			{
				_canUseSerializeHandlerInStreamingState = 2;
			}
			else if ((uint)_serializationCount < 10u && Interlocked.Increment(ref _serializationCount) == 10)
			{
				Interlocked.CompareExchange(ref _canUseSerializeHandlerInStreamingState, 1, 0);
			}
		}
	}

	internal JsonTypeInfo(JsonConverter converter, JsonSerializerOptions options)
		: base(typeof(T), converter, options)
	{
		EffectiveConverter = converter.CreateCastingConverter<T>();
	}

	private protected override void SetCreateObject(Delegate createObject)
	{
		VerifyMutable();
		if (base.Kind == JsonTypeInfoKind.None)
		{
			ThrowHelper.ThrowInvalidOperationException_JsonTypeInfoOperationNotPossibleForKind(base.Kind);
		}
		if (!base.Converter.SupportsCreateObjectDelegate)
		{
			ThrowHelper.ThrowInvalidOperationException_CreateObjectConverterNotCompatible(base.Type);
		}
		Func<object> untypedCreateObject;
		Func<T> typedCreateObject;
		if ((object)createObject == null)
		{
			untypedCreateObject = null;
			typedCreateObject = null;
		}
		else
		{
			Func<T> typedDelegate = createObject as Func<T>;
			if (typedDelegate != null)
			{
				typedCreateObject = typedDelegate;
				untypedCreateObject = ((createObject is Func<object> func) ? func : ((Func<object>)(() => typedDelegate())));
			}
			else
			{
				untypedCreateObject = (Func<object>)createObject;
				typedCreateObject = () => (T)untypedCreateObject();
			}
		}
		_createObject = untypedCreateObject;
		_typedCreateObject = typedCreateObject;
		ConstructorAttributeProviderFactory = null;
		base.ConstructorAttributeProvider = null;
		if (base.CreateObjectWithArgs == null)
		{
			return;
		}
		_parameterInfoValuesIndex = null;
		base.CreateObjectWithArgs = null;
		base.ParameterCount = 0;
		foreach (JsonPropertyInfo property in base.PropertyList)
		{
			property.AssociatedParameter = null;
		}
	}

	private protected override JsonPropertyInfo CreatePropertyInfoForTypeInfo()
	{
		return new JsonPropertyInfo<T>(typeof(T), this, base.Options)
		{
			JsonTypeInfo = this,
			IsForTypeInfo = true
		};
	}

	private protected override JsonPropertyInfo CreateJsonPropertyInfo(JsonTypeInfo declaringTypeInfo, Type declaringType, JsonSerializerOptions options)
	{
		return new JsonPropertyInfo<T>(declaringType ?? declaringTypeInfo.Type, declaringTypeInfo, options)
		{
			JsonTypeInfo = this
		};
	}
}
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public abstract class JsonTypeInfo
{
	internal delegate T ParameterizedConstructorDelegate<T, TArg0, TArg1, TArg2, TArg3>(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3);

	private enum ConfigurationState : byte
	{
		NotConfigured,
		Configuring,
		Configured
	}

	internal ref struct PropertyHierarchyResolutionState(JsonSerializerOptions options)
	{
		public Dictionary<string, (JsonPropertyInfo, int index)> AddedProperties = new Dictionary<string, (JsonPropertyInfo, int)>(options.PropertyNameCaseInsensitive ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);

		public Dictionary<string, JsonPropertyInfo> IgnoredProperties = null;

		public bool IsPropertyOrderSpecified = false;
	}

	private protected readonly struct ParameterLookupKey(Type type, string name) : IEquatable<ParameterLookupKey>
	{
		public Type Type { get; } = type;

		public string Name { get; } = name;

		public override int GetHashCode()
		{
			return StringComparer.OrdinalIgnoreCase.GetHashCode(Name);
		}

		public bool Equals(ParameterLookupKey other)
		{
			if (Type == other.Type)
			{
				return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		public override bool Equals([NotNullWhen(true)] object obj)
		{
			if (obj is ParameterLookupKey other)
			{
				return Equals(other);
			}
			return false;
		}
	}

	internal sealed class JsonPropertyInfoList : ConfigurationList<JsonPropertyInfo>
	{
		private readonly JsonTypeInfo _jsonTypeInfo;

		public override bool IsReadOnly
		{
			get
			{
				if (_jsonTypeInfo._properties != this || !_jsonTypeInfo.IsReadOnly)
				{
					return _jsonTypeInfo.Kind != JsonTypeInfoKind.Object;
				}
				return true;
			}
		}

		public JsonPropertyInfoList(JsonTypeInfo jsonTypeInfo)
			: base((IEnumerable<JsonPropertyInfo>)null)
		{
			_jsonTypeInfo = jsonTypeInfo;
		}

		protected override void OnCollectionModifying()
		{
			if (_jsonTypeInfo._properties == this)
			{
				_jsonTypeInfo.VerifyMutable();
			}
			if (_jsonTypeInfo.Kind != JsonTypeInfoKind.Object)
			{
				ThrowHelper.ThrowInvalidOperationException_JsonTypeInfoOperationNotPossibleForKind(_jsonTypeInfo.Kind);
			}
		}

		protected override void ValidateAddedValue(JsonPropertyInfo item)
		{
			item.EnsureChildOf(_jsonTypeInfo);
		}

		public void SortProperties()
		{
			_list.StableSortByKey((JsonPropertyInfo propInfo) => propInfo.Order);
		}

		public void AddPropertyWithConflictResolution(JsonPropertyInfo jsonPropertyInfo, ref PropertyHierarchyResolutionState state)
		{
			string memberName = jsonPropertyInfo.MemberName;
			if (state.AddedProperties.TryAdd(jsonPropertyInfo.Name, (jsonPropertyInfo, base.Count)))
			{
				Add(jsonPropertyInfo);
				state.IsPropertyOrderSpecified |= jsonPropertyInfo.Order != 0;
			}
			else
			{
				var (jsonPropertyInfo2, num) = state.AddedProperties[jsonPropertyInfo.Name];
				if (jsonPropertyInfo2.IsIgnored)
				{
					state.AddedProperties[jsonPropertyInfo.Name] = (jsonPropertyInfo, num);
					base[num] = jsonPropertyInfo;
					state.IsPropertyOrderSpecified |= jsonPropertyInfo.Order != 0;
				}
				else if (!jsonPropertyInfo.IsIgnored && !jsonPropertyInfo.IsOverriddenOrShadowedBy(jsonPropertyInfo2))
				{
					Dictionary<string, JsonPropertyInfo> ignoredProperties = state.IgnoredProperties;
					if (ignoredProperties == null || !ignoredProperties.TryGetValue(memberName, out var value) || !jsonPropertyInfo.IsOverriddenOrShadowedBy(value))
					{
						ThrowHelper.ThrowInvalidOperationException_SerializerPropertyNameConflict(_jsonTypeInfo.Type, jsonPropertyInfo.Name);
					}
				}
			}
			if (jsonPropertyInfo.IsIgnored)
			{
				ref Dictionary<string, JsonPropertyInfo> ignoredProperties2 = ref state.IgnoredProperties;
				(ignoredProperties2 ?? (ignoredProperties2 = new Dictionary<string, JsonPropertyInfo>()))[memberName] = jsonPropertyInfo;
			}
		}
	}

	internal static readonly Type ObjectType = typeof(object);

	private JsonParameterInfo[] _parameterCache;

	private JsonPropertyInfo[] _propertyCache;

	private Dictionary<string, JsonPropertyInfo> _propertyIndex;

	private PropertyRef[] _utf8PropertyCache = Array.Empty<PropertyRef>();

	private Action<object> _onSerializing;

	private Action<object> _onSerialized;

	private Action<object> _onDeserializing;

	private Action<object> _onDeserialized;

	private protected Func<object> _createObject;

	private Func<JsonSerializerContext, JsonPropertyInfo[]> _sourceGenDelayedPropertyInitializer;

	private JsonPropertyInfoList _properties;

	private protected JsonPolymorphismOptions _polymorphismOptions;

	private JsonTypeInfo _elementTypeInfo;

	private JsonTypeInfo _keyTypeInfo;

	private JsonNumberHandling? _numberHandling;

	private JsonUnmappedMemberHandling? _unmappedMemberHandling;

	private JsonObjectCreationHandling? _preferredPropertyObjectCreationHandling;

	private IJsonTypeInfoResolver _originatingResolver;

	internal Func<ICustomAttributeProvider> ConstructorAttributeProviderFactory;

	private ICustomAttributeProvider _constructorAttributeProvider;

	private volatile ConfigurationState _configurationState;

	private ExceptionDispatchInfo _cachedConfigureError;

	private JsonTypeInfo _ancestorPolymorhicType;

	private volatile bool _isAncestorPolymorphicTypeResolved;

	private protected Dictionary<ParameterLookupKey, JsonParameterInfoValues> _parameterInfoValuesIndex;

	internal int ParameterCount { get; private protected set; }

	internal ReadOnlySpan<JsonParameterInfo> ParameterCache => _parameterCache;

	internal bool UsesParameterizedConstructor => _parameterCache != null;

	internal ReadOnlySpan<JsonPropertyInfo> PropertyCache => _propertyCache;

	internal Dictionary<string, JsonPropertyInfo> PropertyIndex => _propertyIndex;

	internal BitArray? OptionalPropertiesMask { get; private set; }

	internal bool ShouldTrackRequiredProperties => OptionalPropertiesMask != null;

	public Type? ElementType { get; }

	public Type? KeyType { get; }

	public Func<object>? CreateObject
	{
		get
		{
			return _createObject;
		}
		set
		{
			SetCreateObject(value);
		}
	}

	internal Func<object>? CreateObjectForExtensionDataProperty { get; set; }

	public Action<object>? OnSerializing
	{
		get
		{
			return _onSerializing;
		}
		set
		{
			VerifyMutable();
			JsonTypeInfoKind kind = Kind;
			if ((uint)(kind - 1) > 2u)
			{
				ThrowHelper.ThrowInvalidOperationException_JsonTypeInfoOperationNotPossibleForKind(Kind);
			}
			_onSerializing = value;
		}
	}

	public Action<object>? OnSerialized
	{
		get
		{
			return _onSerialized;
		}
		set
		{
			VerifyMutable();
			JsonTypeInfoKind kind = Kind;
			if ((uint)(kind - 1) > 2u)
			{
				ThrowHelper.ThrowInvalidOperationException_JsonTypeInfoOperationNotPossibleForKind(Kind);
			}
			_onSerialized = value;
		}
	}

	public Action<object>? OnDeserializing
	{
		get
		{
			return _onDeserializing;
		}
		set
		{
			VerifyMutable();
			JsonTypeInfoKind kind = Kind;
			if ((uint)(kind - 1) > 2u)
			{
				ThrowHelper.ThrowInvalidOperationException_JsonTypeInfoOperationNotPossibleForKind(Kind);
			}
			if (Converter.IsConvertibleCollection)
			{
				ThrowHelper.ThrowInvalidOperationException_JsonTypeInfoOnDeserializingCallbacksNotSupported(Type);
			}
			_onDeserializing = value;
		}
	}

	public Action<object>? OnDeserialized
	{
		get
		{
			return _onDeserialized;
		}
		set
		{
			VerifyMutable();
			JsonTypeInfoKind kind = Kind;
			if ((uint)(kind - 1) > 2u)
			{
				ThrowHelper.ThrowInvalidOperationException_JsonTypeInfoOperationNotPossibleForKind(Kind);
			}
			_onDeserialized = value;
		}
	}

	public IList<JsonPropertyInfo> Properties => PropertyList;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal JsonPropertyInfoList PropertyList
	{
		get
		{
			return _properties ?? CreatePropertyList();
			JsonPropertyInfoList CreatePropertyList()
			{
				JsonPropertyInfoList jsonPropertyInfoList = new JsonPropertyInfoList(this);
				Func<JsonSerializerContext, JsonPropertyInfo[]> sourceGenDelayedPropertyInitializer = _sourceGenDelayedPropertyInitializer;
				if (sourceGenDelayedPropertyInitializer != null)
				{
					JsonMetadataServices.PopulateProperties(this, jsonPropertyInfoList, sourceGenDelayedPropertyInitializer);
				}
				JsonPropertyInfoList jsonPropertyInfoList2 = Interlocked.CompareExchange(ref _properties, jsonPropertyInfoList, null);
				_sourceGenDelayedPropertyInitializer = null;
				if (jsonPropertyInfoList2 == null)
				{
					jsonPropertyInfoList2 = jsonPropertyInfoList;
				}
				return jsonPropertyInfoList2;
			}
		}
	}

	internal Func<JsonSerializerContext, JsonPropertyInfo[]>? SourceGenDelayedPropertyInitializer
	{
		set
		{
			_sourceGenDelayedPropertyInitializer = value;
		}
	}

	public JsonPolymorphismOptions? PolymorphismOptions
	{
		get
		{
			return _polymorphismOptions;
		}
		set
		{
			VerifyMutable();
			if (value != null)
			{
				if (Kind == JsonTypeInfoKind.None)
				{
					ThrowHelper.ThrowInvalidOperationException_JsonTypeInfoOperationNotPossibleForKind(Kind);
				}
				if (value.DeclaringTypeInfo != null && value.DeclaringTypeInfo != this)
				{
					ThrowHelper.ThrowArgumentException_JsonPolymorphismOptionsAssociatedWithDifferentJsonTypeInfo("value");
				}
				value.DeclaringTypeInfo = this;
			}
			_polymorphismOptions = value;
		}
	}

	public bool IsReadOnly { get; private set; }

	internal object? CreateObjectWithArgs { get; set; }

	internal object? AddMethodDelegate { get; set; }

	internal JsonPropertyInfo? ExtensionDataProperty { get; private set; }

	internal PolymorphicTypeResolver? PolymorphicTypeResolver { get; private set; }

	internal bool HasSerializeHandler { get; private protected set; }

	internal bool CanUseSerializeHandler { get; private set; }

	internal bool PropertyMetadataSerializationNotSupported { get; set; }

	internal bool IsNullable => Converter.NullableElementConverter != null;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal JsonTypeInfo? ElementTypeInfo
	{
		get
		{
			JsonTypeInfo elementTypeInfo = _elementTypeInfo;
			if (elementTypeInfo != null)
			{
				elementTypeInfo.EnsureConfigured();
				return elementTypeInfo;
			}
			return elementTypeInfo;
		}
		set
		{
			_elementTypeInfo = value;
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal JsonTypeInfo? KeyTypeInfo
	{
		get
		{
			JsonTypeInfo keyTypeInfo = _keyTypeInfo;
			if (keyTypeInfo != null)
			{
				keyTypeInfo.EnsureConfigured();
				return keyTypeInfo;
			}
			return keyTypeInfo;
		}
		set
		{
			_keyTypeInfo = value;
		}
	}

	public JsonSerializerOptions Options { get; }

	public Type Type { get; }

	public JsonConverter Converter { get; }

	public JsonTypeInfoKind Kind { get; }

	internal JsonPropertyInfo PropertyInfoForTypeInfo { get; }

	public JsonNumberHandling? NumberHandling
	{
		get
		{
			return _numberHandling;
		}
		set
		{
			VerifyMutable();
			if (value.HasValue && !JsonSerializer.IsValidNumberHandlingValue(value.Value))
			{
				throw new ArgumentOutOfRangeException("value");
			}
			_numberHandling = value;
		}
	}

	internal JsonNumberHandling EffectiveNumberHandling => _numberHandling ?? Options.NumberHandling;

	public JsonUnmappedMemberHandling? UnmappedMemberHandling
	{
		get
		{
			return _unmappedMemberHandling;
		}
		set
		{
			VerifyMutable();
			if (Kind != JsonTypeInfoKind.Object)
			{
				ThrowHelper.ThrowInvalidOperationException_JsonTypeInfoOperationNotPossibleForKind(Kind);
			}
			if (value.HasValue && !JsonSerializer.IsValidUnmappedMemberHandlingValue(value.Value))
			{
				throw new ArgumentOutOfRangeException("value");
			}
			_unmappedMemberHandling = value;
		}
	}

	internal JsonUnmappedMemberHandling EffectiveUnmappedMemberHandling { get; private set; }

	public JsonObjectCreationHandling? PreferredPropertyObjectCreationHandling
	{
		get
		{
			return _preferredPropertyObjectCreationHandling;
		}
		set
		{
			VerifyMutable();
			if (Kind != JsonTypeInfoKind.Object)
			{
				ThrowHelper.ThrowInvalidOperationException_JsonTypeInfoOperationNotPossibleForKind(Kind);
			}
			if (value.HasValue && !JsonSerializer.IsValidCreationHandlingValue(value.Value))
			{
				throw new ArgumentOutOfRangeException("value");
			}
			_preferredPropertyObjectCreationHandling = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public IJsonTypeInfoResolver? OriginatingResolver
	{
		get
		{
			return _originatingResolver;
		}
		set
		{
			VerifyMutable();
			if (value is JsonSerializerContext)
			{
				IsCustomized = false;
			}
			_originatingResolver = value;
		}
	}

	public ICustomAttributeProvider? ConstructorAttributeProvider
	{
		get
		{
			Func<ICustomAttributeProvider> func = Volatile.Read(in ConstructorAttributeProviderFactory);
			ICustomAttributeProvider customAttributeProvider = _constructorAttributeProvider;
			if (customAttributeProvider == null && func != null)
			{
				customAttributeProvider = (_constructorAttributeProvider = func());
				Volatile.Write(ref ConstructorAttributeProviderFactory, null);
			}
			return customAttributeProvider;
		}
		internal set
		{
			_constructorAttributeProvider = value;
			Volatile.Write(ref ConstructorAttributeProviderFactory, null);
		}
	}

	internal bool IsCustomized { get; set; } = true;

	internal bool IsConfigured => _configurationState == ConfigurationState.Configured;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal JsonTypeInfo? AncestorPolymorphicType
	{
		get
		{
			if (!_isAncestorPolymorphicTypeResolved)
			{
				_ancestorPolymorhicType = System.Text.Json.Serialization.Metadata.PolymorphicTypeResolver.FindNearestPolymorphicBaseType(this);
				_isAncestorPolymorphicTypeResolved = true;
			}
			return _ancestorPolymorhicType;
		}
	}

	private bool IsCompatibleWithCurrentOptions { get; set; } = true;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal bool SupportsPolymorphicDeserialization => PolymorphicTypeResolver?.UsesTypeDiscriminators ?? false;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string DebuggerDisplay => $"Type = {Type.Name}, Kind = {Kind}";

	internal JsonPropertyInfo GetProperty(ReadOnlySpan<byte> propertyName, ref ReadStackFrame frame, out byte[] utf8PropertyName)
	{
		PropertyRef[] utf8PropertyCache = _utf8PropertyCache;
		ReadOnlySpan<PropertyRef> readOnlySpan = utf8PropertyCache;
		ulong key = PropertyRef.GetKey(propertyName);
		if (!readOnlySpan.IsEmpty)
		{
			int propertyIndex = frame.PropertyIndex;
			int length = readOnlySpan.Length;
			int num = Math.Min(propertyIndex, length);
			int num2 = num - 1;
			while (true)
			{
				if (num < length)
				{
					PropertyRef propertyRef = readOnlySpan[num];
					if (propertyRef.Equals(propertyName, key))
					{
						utf8PropertyName = propertyRef.Utf8PropertyName;
						return propertyRef.Info;
					}
					num++;
					if (num2 >= 0)
					{
						propertyRef = readOnlySpan[num2];
						if (propertyRef.Equals(propertyName, key))
						{
							utf8PropertyName = propertyRef.Utf8PropertyName;
							return propertyRef.Info;
						}
						num2--;
					}
				}
				else
				{
					if (num2 < 0)
					{
						break;
					}
					PropertyRef propertyRef = readOnlySpan[num2];
					if (propertyRef.Equals(propertyName, key))
					{
						utf8PropertyName = propertyRef.Utf8PropertyName;
						return propertyRef.Info;
					}
					num2--;
				}
			}
		}
		if (PropertyIndex.TryLookupUtf8Key<JsonPropertyInfo>(propertyName, out var result) && (!Options.PropertyNameCaseInsensitive || propertyName.SequenceEqual(result.NameAsUtf8Bytes)))
		{
			utf8PropertyName = result.NameAsUtf8Bytes;
		}
		else
		{
			utf8PropertyName = propertyName.ToArray();
		}
		ref PropertyRefCacheBuilder propertyRefCacheBuilder = ref frame.PropertyRefCacheBuilder;
		if ((propertyRefCacheBuilder?.TotalCount ?? utf8PropertyCache.Length) < 64)
		{
			(propertyRefCacheBuilder ?? (propertyRefCacheBuilder = new PropertyRefCacheBuilder(utf8PropertyCache))).TryAdd(new PropertyRef(key, result, utf8PropertyName));
		}
		return result;
	}

	internal void UpdateUtf8PropertyCache(ref ReadStackFrame frame)
	{
		PropertyRef[] utf8PropertyCache = _utf8PropertyCache;
		PropertyRefCacheBuilder propertyRefCacheBuilder = frame.PropertyRefCacheBuilder;
		if (utf8PropertyCache == propertyRefCacheBuilder.OriginalCache)
		{
			propertyRefCacheBuilder.ToArray();
			_utf8PropertyCache = propertyRefCacheBuilder.ToArray();
		}
		frame.PropertyRefCacheBuilder = null;
	}

	internal JsonTypeInfo(Type type, JsonConverter converter, JsonSerializerOptions options)
	{
		Type = type;
		Options = options;
		Converter = converter;
		Kind = GetTypeInfoKind(type, converter);
		PropertyInfoForTypeInfo = CreatePropertyInfoForTypeInfo();
		ElementType = converter.ElementType;
		KeyType = converter.KeyType;
	}

	private protected abstract void SetCreateObject(Delegate createObject);

	public void MakeReadOnly()
	{
		IsReadOnly = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void ValidateCanBeUsedForPropertyMetadataSerialization()
	{
		if (PropertyMetadataSerializationNotSupported)
		{
			ThrowHelper.ThrowInvalidOperationException_NoMetadataForTypeProperties(Options.TypeInfoResolver, Type);
		}
	}

	private protected abstract JsonPropertyInfo CreatePropertyInfoForTypeInfo();

	internal void VerifyMutable()
	{
		if (IsReadOnly)
		{
			ThrowHelper.ThrowInvalidOperationException_TypeInfoImmutable();
		}
		IsCustomized = true;
	}

	internal void EnsureConfigured()
	{
		if (!IsConfigured)
		{
			ConfigureSynchronized();
		}
		void ConfigureSynchronized()
		{
			Options.MakeReadOnly();
			MakeReadOnly();
			_cachedConfigureError?.Throw();
			lock (Options.CacheContext)
			{
				if (_configurationState != ConfigurationState.NotConfigured)
				{
					return;
				}
				_cachedConfigureError?.Throw();
				try
				{
					_configurationState = ConfigurationState.Configuring;
					Configure();
					_configurationState = ConfigurationState.Configured;
				}
				catch (Exception source)
				{
					_cachedConfigureError = ExceptionDispatchInfo.Capture(source);
					_configurationState = ConfigurationState.NotConfigured;
					throw;
				}
			}
		}
	}

	private void Configure()
	{
		PropertyInfoForTypeInfo.Configure();
		if (PolymorphismOptions != null)
		{
			PolymorphicTypeResolver = new PolymorphicTypeResolver(Options, PolymorphismOptions, Type, Converter.CanHaveMetadata);
		}
		if (Kind == JsonTypeInfoKind.Object)
		{
			ConfigureProperties();
			if (DetermineUsesParameterizedConstructor())
			{
				ConfigureConstructorParameters();
			}
		}
		if (ElementType != null)
		{
			if (_elementTypeInfo == null)
			{
				_elementTypeInfo = Options.GetTypeInfoInternal(ElementType, ensureConfigured: true, true);
			}
			_elementTypeInfo.EnsureConfigured();
		}
		if (KeyType != null)
		{
			if (_keyTypeInfo == null)
			{
				_keyTypeInfo = Options.GetTypeInfoInternal(KeyType, ensureConfigured: true, true);
			}
			_keyTypeInfo.EnsureConfigured();
		}
		DetermineIsCompatibleWithCurrentOptions();
		CanUseSerializeHandler = HasSerializeHandler && IsCompatibleWithCurrentOptions;
	}

	private void DetermineIsCompatibleWithCurrentOptions()
	{
		if (!IsCurrentNodeCompatible())
		{
			IsCompatibleWithCurrentOptions = false;
			return;
		}
		if (_properties != null)
		{
			foreach (JsonPropertyInfo property in _properties)
			{
				if (property.IsPropertyTypeInfoConfigured && !property.JsonTypeInfo.IsCompatibleWithCurrentOptions)
				{
					IsCompatibleWithCurrentOptions = false;
					return;
				}
			}
		}
		JsonTypeInfo elementTypeInfo = _elementTypeInfo;
		if (elementTypeInfo == null || elementTypeInfo.IsCompatibleWithCurrentOptions)
		{
			JsonTypeInfo keyTypeInfo = _keyTypeInfo;
			if (keyTypeInfo == null || keyTypeInfo.IsCompatibleWithCurrentOptions)
			{
				return;
			}
		}
		IsCompatibleWithCurrentOptions = false;
		bool IsCurrentNodeCompatible()
		{
			if (IsCustomized)
			{
				return false;
			}
			if (Options.CanUseFastPathSerializationLogic)
			{
				return true;
			}
			return OriginatingResolver.IsCompatibleWithOptions(Options);
		}
	}

	internal bool DetermineUsesParameterizedConstructor()
	{
		if (Converter.ConstructorIsParameterized)
		{
			return CreateObject == null;
		}
		return false;
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static JsonTypeInfo<T> CreateJsonTypeInfo<T>(JsonSerializerOptions options)
	{
		ArgumentNullException.ThrowIfNull(options, "options");
		return new JsonTypeInfo<T>(DefaultJsonTypeInfoResolver.GetConverterForType(typeof(T), options, resolveJsonConverterAttribute: false), options);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public static JsonTypeInfo CreateJsonTypeInfo(Type type, JsonSerializerOptions options)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		ArgumentNullException.ThrowIfNull(options, "options");
		if (IsInvalidForSerialization(type))
		{
			ThrowHelper.ThrowArgumentException_CannotSerializeInvalidType("type", type, null, null);
		}
		JsonConverter converterForType = DefaultJsonTypeInfoResolver.GetConverterForType(type, options, resolveJsonConverterAttribute: false);
		return CreateJsonTypeInfo(type, converterForType, options);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	internal static JsonTypeInfo CreateJsonTypeInfo(Type type, JsonConverter converter, JsonSerializerOptions options)
	{
		if (converter.Type == type)
		{
			return converter.CreateJsonTypeInfo(options);
		}
		return (JsonTypeInfo)typeof(JsonTypeInfo<>).MakeGenericType(type).CreateInstanceNoWrapExceptions(new Type[2]
		{
			typeof(JsonConverter),
			typeof(JsonSerializerOptions)
		}, new object[2] { converter, options });
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public JsonPropertyInfo CreateJsonPropertyInfo(Type propertyType, string name)
	{
		ArgumentNullException.ThrowIfNull(propertyType, "propertyType");
		ArgumentNullException.ThrowIfNull(name, "name");
		if (IsInvalidForSerialization(propertyType))
		{
			ThrowHelper.ThrowArgumentException_CannotSerializeInvalidType("propertyType", propertyType, Type, name);
		}
		VerifyMutable();
		JsonPropertyInfo jsonPropertyInfo = CreatePropertyUsingReflection(propertyType, null);
		jsonPropertyInfo.Name = name;
		return jsonPropertyInfo;
	}

	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	internal JsonPropertyInfo CreatePropertyUsingReflection(Type propertyType, Type declaringType)
	{
		if (Options.TryGetTypeInfoCached(propertyType, out var typeInfo))
		{
			return typeInfo.CreateJsonPropertyInfo(this, declaringType, Options);
		}
		return (JsonPropertyInfo)typeof(JsonPropertyInfo<>).MakeGenericType(propertyType).CreateInstanceNoWrapExceptions(new Type[3]
		{
			typeof(Type),
			typeof(JsonTypeInfo),
			typeof(JsonSerializerOptions)
		}, new object[3]
		{
			declaringType ?? Type,
			this,
			Options
		});
	}

	private protected abstract JsonPropertyInfo CreateJsonPropertyInfo(JsonTypeInfo declaringTypeInfo, Type declaringType, JsonSerializerOptions options);

	internal abstract void SerializeAsObject(Utf8JsonWriter writer, object rootValue);

	internal abstract Task SerializeAsObjectAsync(PipeWriter pipeWriter, object rootValue, int flushThreshold, CancellationToken cancellationToken);

	internal abstract Task SerializeAsObjectAsync(Stream utf8Json, object rootValue, CancellationToken cancellationToken);

	internal abstract Task SerializeAsObjectAsync(PipeWriter utf8Json, object rootValue, CancellationToken cancellationToken);

	internal abstract void SerializeAsObject(Stream utf8Json, object rootValue);

	internal abstract object DeserializeAsObject(ref Utf8JsonReader reader, ref ReadStack state);

	internal abstract ValueTask<object> DeserializeAsObjectAsync(PipeReader utf8Json, CancellationToken cancellationToken);

	internal abstract ValueTask<object> DeserializeAsObjectAsync(Stream utf8Json, CancellationToken cancellationToken);

	internal abstract object DeserializeAsObject(Stream utf8Json);

	internal void ConfigureProperties()
	{
		JsonPropertyInfoList propertyList = PropertyList;
		StringComparer comparer = (Options.PropertyNameCaseInsensitive ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
		Dictionary<string, JsonPropertyInfo> dictionary = new Dictionary<string, JsonPropertyInfo>(propertyList.Count, comparer);
		List<JsonPropertyInfo> list = new List<JsonPropertyInfo>(propertyList.Count);
		bool flag = true;
		int num = int.MinValue;
		BitArray bitArray = null;
		for (int i = 0; i < propertyList.Count; i++)
		{
			JsonPropertyInfo jsonPropertyInfo = propertyList[i];
			if (jsonPropertyInfo.IsExtensionData)
			{
				JsonUnmappedMemberHandling? unmappedMemberHandling = UnmappedMemberHandling;
				if (unmappedMemberHandling.HasValue && unmappedMemberHandling == JsonUnmappedMemberHandling.Disallow)
				{
					ThrowHelper.ThrowInvalidOperationException_ExtensionDataConflictsWithUnmappedMemberHandling(Type, jsonPropertyInfo);
				}
				if (ExtensionDataProperty != null)
				{
					ThrowHelper.ThrowInvalidOperationException_SerializationDuplicateTypeAttribute(Type, typeof(JsonExtensionDataAttribute));
				}
				ExtensionDataProperty = jsonPropertyInfo;
			}
			else
			{
				jsonPropertyInfo.PropertyIndex = i;
				if (jsonPropertyInfo.IsRequired)
				{
					(bitArray ?? (bitArray = new BitArray(propertyList.Count)))[i] = true;
				}
				if (flag)
				{
					flag = num <= jsonPropertyInfo.Order;
					num = jsonPropertyInfo.Order;
				}
				if (!dictionary.TryAdd(jsonPropertyInfo.Name, jsonPropertyInfo))
				{
					ThrowHelper.ThrowInvalidOperationException_SerializerPropertyNameConflict(Type, jsonPropertyInfo.Name);
				}
				list.Add(jsonPropertyInfo);
			}
			jsonPropertyInfo.Configure();
		}
		if (!flag)
		{
			propertyList.SortProperties();
			list.StableSortByKey((JsonPropertyInfo propInfo) => propInfo.Order);
		}
		OptionalPropertiesMask = bitArray?.Not();
		_propertyCache = list.ToArray();
		_propertyIndex = dictionary;
		EffectiveUnmappedMemberHandling = UnmappedMemberHandling ?? ((ExtensionDataProperty == null) ? Options.UnmappedMemberHandling : JsonUnmappedMemberHandling.Skip);
	}

	internal void PopulateParameterInfoValues(JsonParameterInfoValues[] parameterInfoValues)
	{
		if (parameterInfoValues.Length != 0)
		{
			Dictionary<ParameterLookupKey, JsonParameterInfoValues> dictionary = new Dictionary<ParameterLookupKey, JsonParameterInfoValues>(parameterInfoValues.Length);
			foreach (JsonParameterInfoValues jsonParameterInfoValues in parameterInfoValues)
			{
				ParameterLookupKey key = new ParameterLookupKey(jsonParameterInfoValues.ParameterType, jsonParameterInfoValues.Name);
				dictionary.TryAdd(key, jsonParameterInfoValues);
			}
			ParameterCount = parameterInfoValues.Length;
			_parameterInfoValuesIndex = dictionary;
		}
	}

	internal void ResolveMatchingParameterInfo(JsonPropertyInfo propertyInfo)
	{
		Dictionary<ParameterLookupKey, JsonParameterInfoValues> parameterInfoValuesIndex = _parameterInfoValuesIndex;
		if (parameterInfoValuesIndex != null)
		{
			string name = propertyInfo.MemberName ?? propertyInfo.Name;
			ParameterLookupKey key = new ParameterLookupKey(propertyInfo.PropertyType, name);
			if (parameterInfoValuesIndex.TryGetValue(key, out var value))
			{
				propertyInfo.AddJsonParameterInfo(value);
			}
		}
	}

	internal void ConfigureConstructorParameters()
	{
		List<JsonParameterInfo> list = new List<JsonParameterInfo>(ParameterCount);
		Dictionary<ParameterLookupKey, JsonParameterInfo> dictionary = new Dictionary<ParameterLookupKey, JsonParameterInfo>(ParameterCount);
		JsonPropertyInfo[] propertyCache = _propertyCache;
		foreach (JsonPropertyInfo jsonPropertyInfo in propertyCache)
		{
			JsonParameterInfo associatedParameter = jsonPropertyInfo.AssociatedParameter;
			if (associatedParameter != null)
			{
				string name = jsonPropertyInfo.MemberName ?? jsonPropertyInfo.Name;
				ParameterLookupKey key = new ParameterLookupKey(jsonPropertyInfo.PropertyType, name);
				if (!dictionary.TryAdd(key, associatedParameter))
				{
					ThrowHelper.ThrowInvalidOperationException_MultiplePropertiesBindToConstructorParameters(Type, associatedParameter.Name, jsonPropertyInfo.Name, dictionary[key].MatchingProperty.Name);
				}
				list.Add(associatedParameter);
			}
		}
		JsonPropertyInfo extensionDataProperty = ExtensionDataProperty;
		if (extensionDataProperty != null && extensionDataProperty.AssociatedParameter != null)
		{
			ThrowHelper.ThrowInvalidOperationException_ExtensionDataCannotBindToCtorParam(ExtensionDataProperty.MemberName, ExtensionDataProperty);
		}
		_parameterCache = list.ToArray();
		_parameterInfoValuesIndex = null;
	}

	internal static void ValidateType(Type type)
	{
		if (IsInvalidForSerialization(type))
		{
			ThrowHelper.ThrowInvalidOperationException_CannotSerializeInvalidType(type, null, null);
		}
	}

	internal static bool IsInvalidForSerialization(Type type)
	{
		if (!(type == typeof(void)) && !type.IsPointer && !type.IsByRef && !IsByRefLike(type))
		{
			return type.ContainsGenericParameters;
		}
		return true;
	}

	internal void PopulatePolymorphismMetadata()
	{
		JsonPolymorphismOptions jsonPolymorphismOptions = JsonPolymorphismOptions.CreateFromAttributeDeclarations(Type);
		if (jsonPolymorphismOptions != null)
		{
			jsonPolymorphismOptions.DeclaringTypeInfo = this;
			_polymorphismOptions = jsonPolymorphismOptions;
		}
	}

	internal void MapInterfaceTypesToCallbacks()
	{
		JsonTypeInfoKind kind = Kind;
		if ((uint)(kind - 1) > 2u)
		{
			return;
		}
		if (typeof(IJsonOnSerializing).IsAssignableFrom(Type))
		{
			OnSerializing = delegate(object obj)
			{
				((IJsonOnSerializing)obj).OnSerializing();
			};
		}
		if (typeof(IJsonOnSerialized).IsAssignableFrom(Type))
		{
			OnSerialized = delegate(object obj)
			{
				((IJsonOnSerialized)obj).OnSerialized();
			};
		}
		if (typeof(IJsonOnDeserializing).IsAssignableFrom(Type))
		{
			OnDeserializing = delegate(object obj)
			{
				((IJsonOnDeserializing)obj).OnDeserializing();
			};
		}
		if (typeof(IJsonOnDeserialized).IsAssignableFrom(Type))
		{
			OnDeserialized = delegate(object obj)
			{
				((IJsonOnDeserialized)obj).OnDeserialized();
			};
		}
	}

	internal void SetCreateObjectIfCompatible(Delegate createObject)
	{
		if (Converter.SupportsCreateObjectDelegate && !Converter.ConstructorIsParameterized)
		{
			SetCreateObject(createObject);
		}
	}

	private static bool IsByRefLike(Type type)
	{
		return type.IsByRefLike;
	}

	internal static bool IsValidExtensionDataProperty(Type propertyType)
	{
		if (!typeof(IDictionary<string, object>).IsAssignableFrom(propertyType) && !typeof(IDictionary<string, JsonElement>).IsAssignableFrom(propertyType))
		{
			if (propertyType.FullName == "System.Text.Json.Nodes.JsonObject")
			{
				return (object)propertyType.Assembly == typeof(JsonTypeInfo).Assembly;
			}
			return false;
		}
		return true;
	}

	private static JsonTypeInfoKind GetTypeInfoKind(Type type, JsonConverter converter)
	{
		if (type == typeof(object) && converter.CanBePolymorphic)
		{
			return JsonTypeInfoKind.None;
		}
		switch (converter.ConverterStrategy)
		{
		case ConverterStrategy.Value:
			return JsonTypeInfoKind.None;
		case ConverterStrategy.Object:
			return JsonTypeInfoKind.Object;
		case ConverterStrategy.Enumerable:
			return JsonTypeInfoKind.Enumerable;
		case ConverterStrategy.Dictionary:
			return JsonTypeInfoKind.Dictionary;
		case ConverterStrategy.None:
			ThrowHelper.ThrowNotSupportedException_SerializationNotSupported(type);
			return JsonTypeInfoKind.None;
		default:
			throw new InvalidOperationException();
		}
	}
}
