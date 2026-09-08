using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text.Encodings.Web;
using System.Text.Json.Nodes;
using System.Text.Json.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Converters;
using System.Text.Json.Serialization.Metadata;
using System.Threading;

namespace System.Text.Json;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class JsonSerializerOptions
{
	internal sealed class CachingContext
	{
		private sealed class CacheEntry
		{
			public readonly bool HasResult;

			public readonly JsonTypeInfo TypeInfo;

			public readonly ExceptionDispatchInfo ExceptionDispatchInfo;

			public volatile bool IsNearestAncestorResolved;

			public CacheEntry NearestAncestor;

			public CacheEntry(JsonTypeInfo typeInfo)
			{
				TypeInfo = typeInfo;
				HasResult = typeInfo != null;
			}

			public CacheEntry(ExceptionDispatchInfo exception)
			{
				ExceptionDispatchInfo = exception;
				HasResult = true;
			}

			public JsonTypeInfo GetResult()
			{
				ExceptionDispatchInfo?.Throw();
				return TypeInfo;
			}
		}

		private readonly ConcurrentDictionary<Type, CacheEntry> _cache = new ConcurrentDictionary<Type, CacheEntry>();

		public JsonSerializerOptions Options { get; }

		public int HashCode { get; }

		public int Count => _cache.Count;

		public CachingContext(JsonSerializerOptions options, int hashCode)
		{
			Options = options;
			HashCode = hashCode;
		}

		public JsonTypeInfo GetOrAddTypeInfo(Type type, bool fallBackToNearestAncestorType = false)
		{
			CacheEntry orAddCacheEntry = GetOrAddCacheEntry(type);
			if (!fallBackToNearestAncestorType || orAddCacheEntry.HasResult)
			{
				return orAddCacheEntry.GetResult();
			}
			return FallBackToNearestAncestor(type, orAddCacheEntry);
		}

		public bool TryGetTypeInfo(Type type, [NotNullWhen(true)] out JsonTypeInfo typeInfo)
		{
			_cache.TryGetValue(type, out var value);
			typeInfo = value?.TypeInfo;
			return typeInfo != null;
		}

		public void Clear()
		{
			_cache.Clear();
		}

		private CacheEntry GetOrAddCacheEntry(Type type)
		{
			return _cache.GetOrAdd(type, CreateCacheEntry, this);
		}

		private static CacheEntry CreateCacheEntry(Type type, CachingContext context)
		{
			try
			{
				return new CacheEntry(context.Options.GetTypeInfoNoCaching(type));
			}
			catch (Exception source)
			{
				return new CacheEntry(ExceptionDispatchInfo.Capture(source));
			}
		}

		private JsonTypeInfo FallBackToNearestAncestor(Type type, CacheEntry entry)
		{
			return (entry.IsNearestAncestorResolved ? entry.NearestAncestor : DetermineNearestAncestor(type, entry))?.GetResult();
		}

		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2070:UnrecognizedReflectionPattern", Justification = "We only need to examine the interface types that are supported by the underlying resolver.")]
		private CacheEntry DetermineNearestAncestor(Type type, CacheEntry entry)
		{
			CacheEntry cacheEntry = null;
			Type type2 = null;
			Type baseType = type.BaseType;
			while (baseType != null && !(baseType == JsonTypeInfo.ObjectType))
			{
				cacheEntry = GetOrAddCacheEntry(baseType);
				if (cacheEntry.HasResult)
				{
					type2 = baseType;
					break;
				}
				baseType = baseType.BaseType;
			}
			Type[] interfaces = type.GetInterfaces();
			foreach (Type type3 in interfaces)
			{
				CacheEntry orAddCacheEntry = GetOrAddCacheEntry(type3);
				if (!orAddCacheEntry.HasResult)
				{
					continue;
				}
				if (type2 != null)
				{
					if (type3.IsAssignableFrom(type2))
					{
						continue;
					}
					if (!type2.IsAssignableFrom(type3))
					{
						cacheEntry = new CacheEntry(ExceptionDispatchInfo.Capture(ThrowHelper.GetNotSupportedException_AmbiguousMetadataForType(type, type2, type3)));
						break;
					}
				}
				cacheEntry = orAddCacheEntry;
				type2 = type3;
			}
			entry.NearestAncestor = cacheEntry;
			entry.IsNearestAncestorResolved = true;
			return cacheEntry;
		}
	}

	internal static class TrackedCachingContexts
	{
		private static readonly WeakReference<CachingContext>[] s_trackedContexts = new WeakReference<CachingContext>[64];

		private static readonly EqualityComparer s_optionsComparer = new EqualityComparer();

		public static CachingContext GetOrCreate(JsonSerializerOptions options)
		{
			int hashCode = s_optionsComparer.GetHashCode(options);
			if (TryGetContext(options, hashCode, out var firstUnpopulatedIndex, out var result))
			{
				return result;
			}
			if (firstUnpopulatedIndex < 0)
			{
				return new CachingContext(options, hashCode);
			}
			lock (s_trackedContexts)
			{
				if (TryGetContext(options, hashCode, out firstUnpopulatedIndex, out result))
				{
					return result;
				}
				CachingContext cachingContext = new CachingContext(options, hashCode);
				if (firstUnpopulatedIndex >= 0)
				{
					ref WeakReference<CachingContext> reference = ref s_trackedContexts[firstUnpopulatedIndex];
					if (reference == null)
					{
						reference = new WeakReference<CachingContext>(cachingContext);
					}
					else
					{
						reference.SetTarget(cachingContext);
					}
				}
				return cachingContext;
			}
		}

		private static bool TryGetContext(JsonSerializerOptions options, int hashCode, out int firstUnpopulatedIndex, [NotNullWhen(true)] out CachingContext result)
		{
			WeakReference<CachingContext>[] array = s_trackedContexts;
			firstUnpopulatedIndex = -1;
			for (int i = 0; i < array.Length; i++)
			{
				WeakReference<CachingContext> weakReference = array[i];
				if (weakReference == null || !weakReference.TryGetTarget(out var target))
				{
					if (firstUnpopulatedIndex < 0)
					{
						firstUnpopulatedIndex = i;
					}
				}
				else if (hashCode == target.HashCode && s_optionsComparer.Equals(options, target.Options))
				{
					result = target;
					return true;
				}
			}
			result = null;
			return false;
		}
	}

	private sealed class EqualityComparer : IEqualityComparer<JsonSerializerOptions>
	{
		public bool Equals(JsonSerializerOptions left, JsonSerializerOptions right)
		{
			if (left._dictionaryKeyPolicy == right._dictionaryKeyPolicy && left._jsonPropertyNamingPolicy == right._jsonPropertyNamingPolicy && left._readCommentHandling == right._readCommentHandling && left._referenceHandler == right._referenceHandler && left._encoder == right._encoder && left._defaultIgnoreCondition == right._defaultIgnoreCondition && left._numberHandling == right._numberHandling && left._preferredObjectCreationHandling == right._preferredObjectCreationHandling && left._unknownTypeHandling == right._unknownTypeHandling && left._unmappedMemberHandling == right._unmappedMemberHandling && left._defaultBufferSize == right._defaultBufferSize && left._maxDepth == right._maxDepth && left.NewLine == right.NewLine && left._allowOutOfOrderMetadataProperties == right._allowOutOfOrderMetadataProperties && left._allowTrailingCommas == right._allowTrailingCommas && left._respectNullableAnnotations == right._respectNullableAnnotations && left._respectRequiredConstructorParameters == right._respectRequiredConstructorParameters && left._ignoreNullValues == right._ignoreNullValues && left._ignoreReadOnlyProperties == right._ignoreReadOnlyProperties && left._ignoreReadonlyFields == right._ignoreReadonlyFields && left._includeFields == right._includeFields && left._propertyNameCaseInsensitive == right._propertyNameCaseInsensitive && left._writeIndented == right._writeIndented && left._indentCharacter == right._indentCharacter && left._indentSize == right._indentSize && left._typeInfoResolver == right._typeInfoResolver && left._allowDuplicateProperties == right._allowDuplicateProperties)
			{
				return CompareLists<JsonConverter>(left._converters, right._converters);
			}
			return false;
			static bool CompareLists<TValue>(ConfigurationList<TValue> configurationList, ConfigurationList<TValue> configurationList2) where TValue : class
			{
				if (configurationList == null)
				{
					if (configurationList2 != null)
					{
						return configurationList2.Count == 0;
					}
					return true;
				}
				if (configurationList2 == null)
				{
					return configurationList.Count == 0;
				}
				int count;
				if ((count = configurationList.Count) != configurationList2.Count)
				{
					return false;
				}
				for (int i = 0; i < count; i++)
				{
					if (configurationList[i] != configurationList2[i])
					{
						return false;
					}
				}
				return true;
			}
		}

		public int GetHashCode(JsonSerializerOptions options)
		{
			HashCode hc = default(HashCode);
			AddHashCode<JsonNamingPolicy>(ref hc, options._dictionaryKeyPolicy);
			AddHashCode<JsonNamingPolicy>(ref hc, options._jsonPropertyNamingPolicy);
			AddHashCode<JsonCommentHandling>(ref hc, options._readCommentHandling);
			AddHashCode<ReferenceHandler>(ref hc, options._referenceHandler);
			AddHashCode<JavaScriptEncoder>(ref hc, options._encoder);
			AddHashCode<JsonIgnoreCondition>(ref hc, options._defaultIgnoreCondition);
			AddHashCode<JsonNumberHandling>(ref hc, options._numberHandling);
			AddHashCode<JsonObjectCreationHandling>(ref hc, options._preferredObjectCreationHandling);
			AddHashCode<JsonUnknownTypeHandling>(ref hc, options._unknownTypeHandling);
			AddHashCode<JsonUnmappedMemberHandling>(ref hc, options._unmappedMemberHandling);
			AddHashCode<int>(ref hc, options._defaultBufferSize);
			AddHashCode<int>(ref hc, options._maxDepth);
			AddHashCode<string>(ref hc, options.NewLine);
			AddHashCode<bool>(ref hc, options._allowOutOfOrderMetadataProperties);
			AddHashCode<bool>(ref hc, options._allowTrailingCommas);
			AddHashCode<bool>(ref hc, options._respectNullableAnnotations);
			AddHashCode<bool>(ref hc, options._respectRequiredConstructorParameters);
			AddHashCode<bool>(ref hc, options._ignoreNullValues);
			AddHashCode<bool>(ref hc, options._ignoreReadOnlyProperties);
			AddHashCode<bool>(ref hc, options._ignoreReadonlyFields);
			AddHashCode<bool>(ref hc, options._includeFields);
			AddHashCode<bool>(ref hc, options._propertyNameCaseInsensitive);
			AddHashCode<bool>(ref hc, options._writeIndented);
			AddHashCode<char>(ref hc, options._indentCharacter);
			AddHashCode<int>(ref hc, options._indentSize);
			AddHashCode<IJsonTypeInfoResolver>(ref hc, options._typeInfoResolver);
			AddHashCode<bool>(ref hc, options._allowDuplicateProperties);
			AddListHashCode<JsonConverter>(ref hc, options._converters);
			return hc.ToHashCode();
			static void AddHashCode<TValue>(ref HashCode reference, TValue value)
			{
				if (typeof(TValue).IsSealed)
				{
					reference.Add(value);
				}
				else
				{
					reference.Add(RuntimeHelpers.GetHashCode(value));
				}
			}
			static void AddListHashCode<TValue>(ref HashCode hc2, ConfigurationList<TValue> list)
			{
				if (list != null)
				{
					int count = list.Count;
					for (int i = 0; i < count; i++)
					{
						AddHashCode<TValue>(ref hc2, list[i]);
					}
				}
			}
		}
	}

	internal static class TrackedOptionsInstances
	{
		public static ConditionalWeakTable<JsonSerializerOptions, object> All { get; } = new ConditionalWeakTable<JsonSerializerOptions, object>();
	}

	private sealed class ConverterList : ConfigurationList<JsonConverter>
	{
		private readonly JsonSerializerOptions _options;

		public override bool IsReadOnly => _options.IsReadOnly;

		public ConverterList(JsonSerializerOptions options, IList<JsonConverter> source = null)
			: base((IEnumerable<JsonConverter>)source)
		{
			_options = options;
		}

		protected override void OnCollectionModifying()
		{
			_options.VerifyMutable();
		}
	}

	private sealed class OptionsBoundJsonTypeInfoResolverChain : JsonTypeInfoResolverChain
	{
		private JsonSerializerOptions _options;

		public override bool IsReadOnly => _options?.IsReadOnly ?? false;

		public OptionsBoundJsonTypeInfoResolverChain(JsonSerializerOptions options)
		{
			_options = options;
			AddFlattened(options._typeInfoResolver);
		}

		public void DetachFromOptions()
		{
			_options = null;
		}

		protected override void ValidateAddedValue(IJsonTypeInfoResolver item)
		{
			if (item == this || item == _options?._typeInfoResolver)
			{
				ThrowHelper.ThrowInvalidOperationException_InvalidChainedResolver();
			}
		}

		protected override void OnCollectionModifying()
		{
			_options?.VerifyMutable();
		}

		protected override void OnCollectionModified()
		{
			if (_options != null)
			{
				_options._typeInfoResolver = this;
			}
		}
	}

	private CachingContext _cachingContext;

	private volatile JsonTypeInfo _lastTypeInfo;

	private JsonTypeInfo _objectTypeInfo;

	[CompilerGenerated]
	private static JsonSerializerOptions _003CDefault_003Ek__BackingField;

	[CompilerGenerated]
	private static JsonSerializerOptions _003CWeb_003Ek__BackingField;

	[CompilerGenerated]
	private static JsonSerializerOptions _003CStrict_003Ek__BackingField;

	private IJsonTypeInfoResolver _typeInfoResolver;

	private JsonNamingPolicy _dictionaryKeyPolicy;

	private JsonNamingPolicy _jsonPropertyNamingPolicy;

	private JsonCommentHandling _readCommentHandling;

	private ReferenceHandler _referenceHandler;

	private JavaScriptEncoder _encoder;

	private ConverterList _converters;

	private JsonIgnoreCondition _defaultIgnoreCondition;

	private JsonNumberHandling _numberHandling;

	private JsonObjectCreationHandling _preferredObjectCreationHandling;

	private JsonUnknownTypeHandling _unknownTypeHandling;

	private JsonUnmappedMemberHandling _unmappedMemberHandling;

	private int _defaultBufferSize = 16384;

	private int _maxDepth;

	private bool _allowOutOfOrderMetadataProperties;

	private bool _allowTrailingCommas;

	private bool _respectNullableAnnotations = AppContextSwitchHelper.RespectNullableAnnotationsDefault;

	private bool _respectRequiredConstructorParameters = AppContextSwitchHelper.RespectRequiredConstructorParametersDefault;

	private bool _ignoreNullValues;

	private bool _ignoreReadOnlyProperties;

	private bool _ignoreReadonlyFields;

	private bool _includeFields;

	private string _newLine;

	private bool _propertyNameCaseInsensitive;

	private bool _writeIndented;

	private char _indentCharacter = ' ';

	private int _indentSize = 2;

	private bool _allowDuplicateProperties = true;

	private OptionsBoundJsonTypeInfoResolverChain _typeInfoResolverChain;

	private bool? _canUseFastPathSerializationLogic;

	internal JsonKnownReferenceHandler ReferenceHandlingStrategy;

	private volatile bool _isReadOnly;

	private volatile bool _isConfiguredForJsonSerializer;

	private IJsonTypeInfoResolver _effectiveJsonTypeInfoResolver;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal CachingContext CacheContext
	{
		get
		{
			return _cachingContext ?? GetOrCreate();
			CachingContext GetOrCreate()
			{
				CachingContext orCreate = TrackedCachingContexts.GetOrCreate(this);
				return Interlocked.CompareExchange(ref _cachingContext, orCreate, null) ?? orCreate;
			}
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal JsonTypeInfo ObjectTypeInfo => _objectTypeInfo ?? (_objectTypeInfo = GetTypeInfoInternal(JsonTypeInfo.ObjectType, ensureConfigured: true, true));

	public IList<JsonConverter> Converters => _converters ?? (_converters = new ConverterList(this));

	public static JsonSerializerOptions Default
	{
		[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
		[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
		get
		{
			return _003CDefault_003Ek__BackingField ?? GetOrCreateSingleton(ref _003CDefault_003Ek__BackingField, JsonSerializerDefaults.General);
		}
	}

	public static JsonSerializerOptions Web
	{
		[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
		[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
		get
		{
			return _003CWeb_003Ek__BackingField ?? GetOrCreateSingleton(ref _003CWeb_003Ek__BackingField, JsonSerializerDefaults.Web);
		}
	}

	public static JsonSerializerOptions Strict
	{
		[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
		[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
		get
		{
			return _003CStrict_003Ek__BackingField ?? GetOrCreateSingleton(ref _003CStrict_003Ek__BackingField, JsonSerializerDefaults.Strict);
		}
	}

	public IJsonTypeInfoResolver? TypeInfoResolver
	{
		get
		{
			return _typeInfoResolver;
		}
		set
		{
			VerifyMutable();
			OptionsBoundJsonTypeInfoResolverChain typeInfoResolverChain = _typeInfoResolverChain;
			if (typeInfoResolverChain != null && typeInfoResolverChain != value)
			{
				typeInfoResolverChain.DetachFromOptions();
				_typeInfoResolverChain = null;
			}
			_typeInfoResolver = value;
		}
	}

	public IList<IJsonTypeInfoResolver> TypeInfoResolverChain => _typeInfoResolverChain ?? (_typeInfoResolverChain = new OptionsBoundJsonTypeInfoResolverChain(this));

	public bool AllowOutOfOrderMetadataProperties
	{
		get
		{
			return _allowOutOfOrderMetadataProperties;
		}
		set
		{
			VerifyMutable();
			_allowOutOfOrderMetadataProperties = value;
		}
	}

	public bool AllowTrailingCommas
	{
		get
		{
			return _allowTrailingCommas;
		}
		set
		{
			VerifyMutable();
			_allowTrailingCommas = value;
		}
	}

	public int DefaultBufferSize
	{
		get
		{
			return _defaultBufferSize;
		}
		set
		{
			VerifyMutable();
			if (value < 1)
			{
				throw new ArgumentException(System.SR.SerializationInvalidBufferSize);
			}
			_defaultBufferSize = value;
		}
	}

	public JavaScriptEncoder? Encoder
	{
		get
		{
			return _encoder;
		}
		set
		{
			VerifyMutable();
			_encoder = value;
		}
	}

	public JsonNamingPolicy? DictionaryKeyPolicy
	{
		get
		{
			return _dictionaryKeyPolicy;
		}
		set
		{
			VerifyMutable();
			_dictionaryKeyPolicy = value;
		}
	}

	[Obsolete("JsonSerializerOptions.IgnoreNullValues is obsolete. To ignore null values when serializing, set DefaultIgnoreCondition to JsonIgnoreCondition.WhenWritingNull.", DiagnosticId = "SYSLIB0020", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool IgnoreNullValues
	{
		get
		{
			return _ignoreNullValues;
		}
		set
		{
			VerifyMutable();
			if (value && _defaultIgnoreCondition != JsonIgnoreCondition.Never)
			{
				throw new InvalidOperationException(System.SR.DefaultIgnoreConditionAlreadySpecified);
			}
			_ignoreNullValues = value;
		}
	}

	public JsonIgnoreCondition DefaultIgnoreCondition
	{
		get
		{
			return _defaultIgnoreCondition;
		}
		set
		{
			VerifyMutable();
			switch (value)
			{
			case JsonIgnoreCondition.Always:
				throw new ArgumentException(System.SR.DefaultIgnoreConditionInvalid);
			default:
				if (_ignoreNullValues)
				{
					throw new InvalidOperationException(System.SR.DefaultIgnoreConditionAlreadySpecified);
				}
				break;
			case JsonIgnoreCondition.Never:
				break;
			}
			_defaultIgnoreCondition = value;
		}
	}

	public JsonNumberHandling NumberHandling
	{
		get
		{
			return _numberHandling;
		}
		set
		{
			VerifyMutable();
			if (!JsonSerializer.IsValidNumberHandlingValue(value))
			{
				throw new ArgumentOutOfRangeException("value");
			}
			_numberHandling = value;
		}
	}

	public JsonObjectCreationHandling PreferredObjectCreationHandling
	{
		get
		{
			return _preferredObjectCreationHandling;
		}
		set
		{
			VerifyMutable();
			if (!JsonSerializer.IsValidCreationHandlingValue(value))
			{
				throw new ArgumentOutOfRangeException("value");
			}
			_preferredObjectCreationHandling = value;
		}
	}

	public bool IgnoreReadOnlyProperties
	{
		get
		{
			return _ignoreReadOnlyProperties;
		}
		set
		{
			VerifyMutable();
			_ignoreReadOnlyProperties = value;
		}
	}

	public bool IgnoreReadOnlyFields
	{
		get
		{
			return _ignoreReadonlyFields;
		}
		set
		{
			VerifyMutable();
			_ignoreReadonlyFields = value;
		}
	}

	public bool IncludeFields
	{
		get
		{
			return _includeFields;
		}
		set
		{
			VerifyMutable();
			_includeFields = value;
		}
	}

	public int MaxDepth
	{
		get
		{
			return _maxDepth;
		}
		set
		{
			VerifyMutable();
			if (value < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_MaxDepthMustBePositive("value");
			}
			_maxDepth = value;
			EffectiveMaxDepth = ((value == 0) ? 64 : value);
		}
	}

	internal int EffectiveMaxDepth { get; private set; } = 64;

	public JsonNamingPolicy? PropertyNamingPolicy
	{
		get
		{
			return _jsonPropertyNamingPolicy;
		}
		set
		{
			VerifyMutable();
			_jsonPropertyNamingPolicy = value;
		}
	}

	public bool PropertyNameCaseInsensitive
	{
		get
		{
			return _propertyNameCaseInsensitive;
		}
		set
		{
			VerifyMutable();
			_propertyNameCaseInsensitive = value;
		}
	}

	public JsonCommentHandling ReadCommentHandling
	{
		get
		{
			return _readCommentHandling;
		}
		set
		{
			VerifyMutable();
			if ((int)value > 1)
			{
				throw new ArgumentOutOfRangeException("value", System.SR.JsonSerializerDoesNotSupportComments);
			}
			_readCommentHandling = value;
		}
	}

	public JsonUnknownTypeHandling UnknownTypeHandling
	{
		get
		{
			return _unknownTypeHandling;
		}
		set
		{
			VerifyMutable();
			_unknownTypeHandling = value;
		}
	}

	public JsonUnmappedMemberHandling UnmappedMemberHandling
	{
		get
		{
			return _unmappedMemberHandling;
		}
		set
		{
			VerifyMutable();
			_unmappedMemberHandling = value;
		}
	}

	public bool WriteIndented
	{
		get
		{
			return _writeIndented;
		}
		set
		{
			VerifyMutable();
			_writeIndented = value;
		}
	}

	public char IndentCharacter
	{
		get
		{
			return _indentCharacter;
		}
		set
		{
			JsonWriterHelper.ValidateIndentCharacter(value);
			VerifyMutable();
			_indentCharacter = value;
		}
	}

	public int IndentSize
	{
		get
		{
			return _indentSize;
		}
		set
		{
			JsonWriterHelper.ValidateIndentSize(value);
			VerifyMutable();
			_indentSize = value;
		}
	}

	public ReferenceHandler? ReferenceHandler
	{
		get
		{
			return _referenceHandler;
		}
		set
		{
			VerifyMutable();
			_referenceHandler = value;
			ReferenceHandlingStrategy = value?.HandlingStrategy ?? JsonKnownReferenceHandler.Unspecified;
		}
	}

	public string NewLine
	{
		get
		{
			return _newLine ?? (_newLine = Environment.NewLine);
		}
		set
		{
			JsonWriterHelper.ValidateNewLine(value);
			VerifyMutable();
			_newLine = value;
		}
	}

	public bool RespectNullableAnnotations
	{
		get
		{
			return _respectNullableAnnotations;
		}
		set
		{
			VerifyMutable();
			_respectNullableAnnotations = value;
		}
	}

	public bool RespectRequiredConstructorParameters
	{
		get
		{
			return _respectRequiredConstructorParameters;
		}
		set
		{
			VerifyMutable();
			_respectRequiredConstructorParameters = value;
		}
	}

	public bool AllowDuplicateProperties
	{
		get
		{
			return _allowDuplicateProperties;
		}
		set
		{
			VerifyMutable();
			_allowDuplicateProperties = value;
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal bool CanUseFastPathSerializationLogic
	{
		get
		{
			bool valueOrDefault = _canUseFastPathSerializationLogic == true;
			if (!_canUseFastPathSerializationLogic.HasValue)
			{
				valueOrDefault = TypeInfoResolver.IsCompatibleWithOptions(this);
				_canUseFastPathSerializationLogic = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}

	public bool IsReadOnly => _isReadOnly;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string DebuggerDisplay => $"TypeInfoResolver = {TypeInfoResolver?.ToString() ?? "<null>"}, IsReadOnly = {IsReadOnly}";

	public JsonTypeInfo GetTypeInfo(Type type)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		if (JsonTypeInfo.IsInvalidForSerialization(type))
		{
			ThrowHelper.ThrowArgumentException_CannotSerializeInvalidType("type", type, null, null);
		}
		return GetTypeInfoInternal(type, ensureConfigured: true, true, resolveIfMutable: true);
	}

	public bool TryGetTypeInfo(Type type, [NotNullWhen(true)] out JsonTypeInfo? typeInfo)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		if (JsonTypeInfo.IsInvalidForSerialization(type))
		{
			ThrowHelper.ThrowArgumentException_CannotSerializeInvalidType("type", type, null, null);
		}
		typeInfo = GetTypeInfoInternal(type, ensureConfigured: true, null, resolveIfMutable: true);
		return typeInfo != null;
	}

	[return: NotNullIfNotNull("ensureNotNull")]
	internal JsonTypeInfo GetTypeInfoInternal(Type type, bool ensureConfigured = true, bool? ensureNotNull = true, bool resolveIfMutable = false, bool fallBackToNearestAncestorType = false)
	{
		JsonTypeInfo jsonTypeInfo = null;
		if (IsReadOnly)
		{
			jsonTypeInfo = CacheContext.GetOrAddTypeInfo(type, fallBackToNearestAncestorType);
			if (ensureConfigured)
			{
				jsonTypeInfo?.EnsureConfigured();
			}
		}
		else if (resolveIfMutable)
		{
			jsonTypeInfo = GetTypeInfoNoCaching(type);
		}
		if (jsonTypeInfo == null && ensureNotNull == true)
		{
			ThrowHelper.ThrowNotSupportedException_NoMetadataForType(type, TypeInfoResolver);
		}
		return jsonTypeInfo;
	}

	internal bool TryGetTypeInfoCached(Type type, [NotNullWhen(true)] out JsonTypeInfo typeInfo)
	{
		if (_cachingContext == null)
		{
			typeInfo = null;
			return false;
		}
		return _cachingContext.TryGetTypeInfo(type, out typeInfo);
	}

	internal JsonTypeInfo GetTypeInfoForRootType(Type type, bool fallBackToNearestAncestorType = false)
	{
		JsonTypeInfo jsonTypeInfo = _lastTypeInfo;
		if (jsonTypeInfo?.Type != type)
		{
			bool fallBackToNearestAncestorType2 = fallBackToNearestAncestorType;
			jsonTypeInfo = (_lastTypeInfo = GetTypeInfoInternal(type, ensureConfigured: true, true, resolveIfMutable: false, fallBackToNearestAncestorType2));
		}
		return jsonTypeInfo;
	}

	internal bool TryGetPolymorphicTypeInfoForRootType(object rootValue, [NotNullWhen(true)] out JsonTypeInfo polymorphicTypeInfo)
	{
		Type type = rootValue.GetType();
		if (type != JsonTypeInfo.ObjectType)
		{
			polymorphicTypeInfo = GetTypeInfoForRootType(type, fallBackToNearestAncestorType: true);
			JsonTypeInfo ancestorPolymorphicType = polymorphicTypeInfo.AncestorPolymorphicType;
			if (ancestorPolymorphicType != null)
			{
				polymorphicTypeInfo = ancestorPolymorphicType;
			}
			return true;
		}
		polymorphicTypeInfo = null;
		return false;
	}

	internal void ClearCaches()
	{
		_cachingContext?.Clear();
		_lastTypeInfo = null;
		_objectTypeInfo = null;
	}

	[RequiresUnreferencedCode("Getting a converter for a type may require reflection which depends on unreferenced code.")]
	[RequiresDynamicCode("Getting a converter for a type may require reflection which depends on runtime code generation.")]
	public JsonConverter GetConverter(Type typeToConvert)
	{
		ArgumentNullException.ThrowIfNull(typeToConvert, "typeToConvert");
		if (JsonSerializer.IsReflectionEnabledByDefault && _typeInfoResolver == null)
		{
			return DefaultJsonTypeInfoResolver.GetConverterForType(typeToConvert, this);
		}
		return GetConverterInternal(typeToConvert);
	}

	internal JsonConverter GetConverterInternal(Type typeToConvert)
	{
		return GetTypeInfoInternal(typeToConvert, ensureConfigured: false, true, resolveIfMutable: true).Converter;
	}

	internal JsonConverter GetConverterFromList(Type typeToConvert)
	{
		ConverterList converters = _converters;
		if (converters != null)
		{
			foreach (JsonConverter item in converters)
			{
				if (item.CanConvert(typeToConvert))
				{
					return item;
				}
			}
		}
		return null;
	}

	[return: NotNullIfNotNull("converter")]
	internal JsonConverter ExpandConverterFactory(JsonConverter converter, Type typeToConvert)
	{
		if (converter is JsonConverterFactory jsonConverterFactory)
		{
			converter = jsonConverterFactory.GetConverterInternal(typeToConvert, this);
		}
		return converter;
	}

	internal static void CheckConverterNullabilityIsSameAsPropertyType(JsonConverter converter, Type propertyType)
	{
		if (propertyType.IsValueType && converter.IsValueType && (propertyType.IsNullableOfT() ^ converter.Type.IsNullableOfT()))
		{
			ThrowHelper.ThrowInvalidOperationException_ConverterCanConvertMultipleTypes(propertyType, converter);
		}
	}

	public JsonSerializerOptions()
	{
		TrackOptionsInstance(this);
	}

	public JsonSerializerOptions(JsonSerializerOptions options)
	{
		ArgumentNullException.ThrowIfNull(options, "options");
		_dictionaryKeyPolicy = options._dictionaryKeyPolicy;
		_jsonPropertyNamingPolicy = options._jsonPropertyNamingPolicy;
		_readCommentHandling = options._readCommentHandling;
		_referenceHandler = options._referenceHandler;
		ConverterList converters = options._converters;
		_converters = ((converters != null) ? new ConverterList(this, converters) : null);
		_encoder = options._encoder;
		_defaultIgnoreCondition = options._defaultIgnoreCondition;
		_numberHandling = options._numberHandling;
		_preferredObjectCreationHandling = options._preferredObjectCreationHandling;
		_unknownTypeHandling = options._unknownTypeHandling;
		_unmappedMemberHandling = options._unmappedMemberHandling;
		_defaultBufferSize = options._defaultBufferSize;
		_maxDepth = options._maxDepth;
		_allowOutOfOrderMetadataProperties = options._allowOutOfOrderMetadataProperties;
		_allowTrailingCommas = options._allowTrailingCommas;
		_respectNullableAnnotations = options._respectNullableAnnotations;
		_respectRequiredConstructorParameters = options._respectRequiredConstructorParameters;
		_ignoreNullValues = options._ignoreNullValues;
		_ignoreReadOnlyProperties = options._ignoreReadOnlyProperties;
		_ignoreReadonlyFields = options._ignoreReadonlyFields;
		_includeFields = options._includeFields;
		_newLine = options._newLine;
		_propertyNameCaseInsensitive = options._propertyNameCaseInsensitive;
		_writeIndented = options._writeIndented;
		_indentCharacter = options._indentCharacter;
		_indentSize = options._indentSize;
		_allowDuplicateProperties = options._allowDuplicateProperties;
		_typeInfoResolver = options._typeInfoResolver;
		EffectiveMaxDepth = options.EffectiveMaxDepth;
		ReferenceHandlingStrategy = options.ReferenceHandlingStrategy;
		TrackOptionsInstance(this);
	}

	public JsonSerializerOptions(JsonSerializerDefaults defaults)
		: this()
	{
		switch (defaults)
		{
		case JsonSerializerDefaults.Web:
			_propertyNameCaseInsensitive = true;
			_jsonPropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			_numberHandling = JsonNumberHandling.AllowReadingFromString;
			break;
		case JsonSerializerDefaults.Strict:
			_unmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
			_allowDuplicateProperties = false;
			_respectNullableAnnotations = true;
			_respectRequiredConstructorParameters = true;
			break;
		default:
			throw new ArgumentOutOfRangeException("defaults");
		case JsonSerializerDefaults.General:
			break;
		}
	}

	private static void TrackOptionsInstance(JsonSerializerOptions options)
	{
		TrackedOptionsInstances.All.Add(options, null);
	}

	[Obsolete("JsonSerializerOptions.AddContext is obsolete. To register a JsonSerializerContext, use either the TypeInfoResolver or TypeInfoResolverChain properties.", DiagnosticId = "SYSLIB0049", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void AddContext<TContext>() where TContext : JsonSerializerContext, new()
	{
		VerifyMutable();
		new TContext().AssociateWithOptions(this);
	}

	public void MakeReadOnly()
	{
		if (_typeInfoResolver == null)
		{
			ThrowHelper.ThrowInvalidOperationException_JsonSerializerOptionsNoTypeInfoResolverSpecified();
		}
		_isReadOnly = true;
	}

	[RequiresUnreferencedCode("Populating unconfigured TypeInfoResolver properties with the reflection resolver requires unreferenced code.")]
	[RequiresDynamicCode("Populating unconfigured TypeInfoResolver properties with the reflection resolver requires runtime code generation.")]
	public void MakeReadOnly(bool populateMissingResolver)
	{
		if (populateMissingResolver)
		{
			if (!_isConfiguredForJsonSerializer)
			{
				ConfigureForJsonSerializer();
			}
		}
		else
		{
			MakeReadOnly();
		}
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	private void ConfigureForJsonSerializer()
	{
		if (JsonSerializer.IsReflectionEnabledByDefault)
		{
			DefaultJsonTypeInfoResolver defaultInstance = DefaultJsonTypeInfoResolver.DefaultInstance;
			IJsonTypeInfoResolver typeInfoResolver = _typeInfoResolver;
			if (typeInfoResolver != null)
			{
				if (typeInfoResolver is JsonSerializerContext jsonSerializerContext && AppContextSwitchHelper.IsSourceGenReflectionFallbackEnabled)
				{
					global::_003C_003Ey__InlineArray2<IJsonTypeInfoResolver> buffer = default(global::_003C_003Ey__InlineArray2<IJsonTypeInfoResolver>);
					buffer[0] = jsonSerializerContext;
					buffer[1] = defaultInstance;
					_effectiveJsonTypeInfoResolver = JsonTypeInfoResolver.Combine(buffer);
					CachingContext cachingContext = _cachingContext;
					if (cachingContext != null)
					{
						if (cachingContext.Options != this && !cachingContext.Options._isConfiguredForJsonSerializer)
						{
							cachingContext.Options.ConfigureForJsonSerializer();
						}
						else
						{
							cachingContext.Clear();
						}
					}
				}
			}
			else
			{
				_typeInfoResolver = defaultInstance;
			}
		}
		else
		{
			IJsonTypeInfoResolver typeInfoResolver = _typeInfoResolver;
			if ((typeInfoResolver == null || typeInfoResolver is EmptyJsonTypeInfoResolver) ? true : false)
			{
				ThrowHelper.ThrowInvalidOperationException_JsonSerializerIsReflectionDisabled();
			}
		}
		_isReadOnly = true;
		_isConfiguredForJsonSerializer = true;
	}

	private JsonTypeInfo GetTypeInfoNoCaching(Type type)
	{
		IJsonTypeInfoResolver jsonTypeInfoResolver = _effectiveJsonTypeInfoResolver ?? _typeInfoResolver;
		if (jsonTypeInfoResolver == null)
		{
			return null;
		}
		JsonTypeInfo jsonTypeInfo = jsonTypeInfoResolver.GetTypeInfo(type, this);
		if (jsonTypeInfo != null)
		{
			if (jsonTypeInfo.Type != type)
			{
				ThrowHelper.ThrowInvalidOperationException_ResolverTypeNotCompatible(type, jsonTypeInfo.Type);
			}
			if (jsonTypeInfo.Options != this)
			{
				ThrowHelper.ThrowInvalidOperationException_ResolverTypeInfoOptionsNotCompatible();
			}
		}
		else if (type == JsonTypeInfo.ObjectType)
		{
			jsonTypeInfo = new JsonTypeInfo<object>(new SlimObjectConverter(jsonTypeInfoResolver), this);
		}
		return jsonTypeInfo;
	}

	internal JsonDocumentOptions GetDocumentOptions()
	{
		return new JsonDocumentOptions
		{
			AllowDuplicateProperties = AllowDuplicateProperties,
			AllowTrailingCommas = AllowTrailingCommas,
			CommentHandling = ReadCommentHandling,
			MaxDepth = MaxDepth
		};
	}

	internal JsonNodeOptions GetNodeOptions()
	{
		return new JsonNodeOptions
		{
			PropertyNameCaseInsensitive = PropertyNameCaseInsensitive
		};
	}

	internal JsonReaderOptions GetReaderOptions()
	{
		return new JsonReaderOptions
		{
			AllowTrailingCommas = AllowTrailingCommas,
			CommentHandling = ReadCommentHandling,
			MaxDepth = EffectiveMaxDepth
		};
	}

	internal JsonWriterOptions GetWriterOptions()
	{
		return new JsonWriterOptions
		{
			Encoder = Encoder,
			Indented = WriteIndented,
			IndentCharacter = IndentCharacter,
			IndentSize = IndentSize,
			MaxDepth = EffectiveMaxDepth,
			NewLine = NewLine,
			SkipValidation = true
		};
	}

	internal void VerifyMutable()
	{
		if (_isReadOnly)
		{
			ThrowHelper.ThrowInvalidOperationException_SerializerOptionsReadOnly(_typeInfoResolver as JsonSerializerContext);
		}
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	private static JsonSerializerOptions GetOrCreateSingleton(ref JsonSerializerOptions location, JsonSerializerDefaults defaults)
	{
		JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions(defaults);
		IJsonTypeInfoResolver typeInfoResolver;
		if (!JsonSerializer.IsReflectionEnabledByDefault)
		{
			typeInfoResolver = JsonTypeInfoResolver.Empty;
		}
		else
		{
			IJsonTypeInfoResolver defaultInstance = DefaultJsonTypeInfoResolver.DefaultInstance;
			typeInfoResolver = defaultInstance;
		}
		jsonSerializerOptions.TypeInfoResolver = typeInfoResolver;
		jsonSerializerOptions._isReadOnly = true;
		JsonSerializerOptions jsonSerializerOptions2 = jsonSerializerOptions;
		return Interlocked.CompareExchange(ref location, jsonSerializerOptions2, null) ?? jsonSerializerOptions2;
	}
}
