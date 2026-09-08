using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Text.Json.Serialization.Metadata;

internal sealed class PolymorphicTypeResolver
{
	private sealed class DerivedJsonTypeInfo
	{
		public object TypeDiscriminator { get; }

		public JsonTypeInfo JsonTypeInfo { get; }

		public DerivedJsonTypeInfo(object typeDiscriminator, JsonTypeInfo derivedTypeInfo)
		{
			TypeDiscriminator = typeDiscriminator;
			JsonTypeInfo = derivedTypeInfo;
		}
	}

	private readonly ConcurrentDictionary<Type, DerivedJsonTypeInfo> _typeToDiscriminatorId = new ConcurrentDictionary<Type, DerivedJsonTypeInfo>();

	private readonly Dictionary<object, DerivedJsonTypeInfo> _discriminatorIdtoType;

	private readonly JsonSerializerOptions _options;

	public Type BaseType { get; }

	public JsonUnknownDerivedTypeHandling UnknownDerivedTypeHandling { get; }

	public bool UsesTypeDiscriminators { get; }

	public bool IgnoreUnrecognizedTypeDiscriminators { get; }

	public byte[] CustomTypeDiscriminatorPropertyNameUtf8 { get; }

	public JsonEncodedText? CustomTypeDiscriminatorPropertyNameJsonEncoded { get; }

	public PolymorphicTypeResolver(JsonSerializerOptions options, JsonPolymorphismOptions polymorphismOptions, Type baseType, bool converterCanHaveMetadata)
	{
		UnknownDerivedTypeHandling = polymorphismOptions.UnknownDerivedTypeHandling;
		IgnoreUnrecognizedTypeDiscriminators = polymorphismOptions.IgnoreUnrecognizedTypeDiscriminators;
		BaseType = baseType;
		_options = options;
		if (!IsSupportedPolymorphicBaseType(BaseType))
		{
			ThrowHelper.ThrowInvalidOperationException_TypeDoesNotSupportPolymorphism(BaseType);
		}
		bool flag = false;
		foreach (var (type2, obj2) in polymorphismOptions.DerivedTypes)
		{
			if (!IsSupportedDerivedType(BaseType, type2) || (type2.IsAbstract && UnknownDerivedTypeHandling != JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor))
			{
				ThrowHelper.ThrowInvalidOperationException_DerivedTypeNotSupported(BaseType, type2);
			}
			JsonTypeInfo typeInfoInternal = options.GetTypeInfoInternal(type2, ensureConfigured: true, true);
			DerivedJsonTypeInfo value = new DerivedJsonTypeInfo(obj2, typeInfoInternal);
			if (!_typeToDiscriminatorId.TryAdd(type2, value))
			{
				ThrowHelper.ThrowInvalidOperationException_DerivedTypeIsAlreadySpecified(BaseType, type2);
			}
			if (obj2 != null)
			{
				if (!(_discriminatorIdtoType ?? (_discriminatorIdtoType = new Dictionary<object, DerivedJsonTypeInfo>())).TryAdd(obj2, value))
				{
					ThrowHelper.ThrowInvalidOperationException_TypeDicriminatorIdIsAlreadySpecified(BaseType, obj2);
				}
				UsesTypeDiscriminators = true;
			}
			flag = true;
		}
		if (!flag)
		{
			ThrowHelper.ThrowInvalidOperationException_PolymorphicTypeConfigurationDoesNotSpecifyDerivedTypes(BaseType);
		}
		if (!UsesTypeDiscriminators)
		{
			return;
		}
		if (!converterCanHaveMetadata)
		{
			ThrowHelper.ThrowNotSupportedException_BaseConverterDoesNotSupportMetadata(BaseType);
		}
		string typeDiscriminatorPropertyName = polymorphismOptions.TypeDiscriminatorPropertyName;
		if (!typeDiscriminatorPropertyName.Equals("$type", StringComparison.Ordinal))
		{
			byte[] bytes = Encoding.UTF8.GetBytes(typeDiscriminatorPropertyName);
			if ((JsonSerializer.GetMetadataPropertyName(bytes, null) & ~MetadataPropertyName.Type) != MetadataPropertyName.None)
			{
				ThrowHelper.ThrowInvalidOperationException_InvalidCustomTypeDiscriminatorPropertyName();
			}
			CustomTypeDiscriminatorPropertyNameUtf8 = bytes;
			CustomTypeDiscriminatorPropertyNameJsonEncoded = JsonEncodedText.Encode(typeDiscriminatorPropertyName, options.Encoder);
		}
		foreach (DerivedJsonTypeInfo value2 in _discriminatorIdtoType.Values)
		{
			if (value2.JsonTypeInfo.Kind != JsonTypeInfoKind.Object)
			{
				continue;
			}
			foreach (JsonPropertyInfo property in value2.JsonTypeInfo.Properties)
			{
				if (property != null && !property.IsIgnored && !property.IsExtensionData && property.Name == typeDiscriminatorPropertyName)
				{
					ThrowHelper.ThrowInvalidOperationException_PropertyConflictsWithMetadataPropertyName(value2.JsonTypeInfo.Type, typeDiscriminatorPropertyName);
				}
			}
		}
	}

	public bool TryGetDerivedJsonTypeInfo(Type runtimeType, [NotNullWhen(true)] out JsonTypeInfo jsonTypeInfo, out object typeDiscriminator)
	{
		if (!_typeToDiscriminatorId.TryGetValue(runtimeType, out var value))
		{
			switch (UnknownDerivedTypeHandling)
			{
			case JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor:
				value = CalculateNearestAncestor(runtimeType);
				_typeToDiscriminatorId[runtimeType] = value;
				break;
			case JsonUnknownDerivedTypeHandling.FallBackToBaseType:
				_typeToDiscriminatorId.TryGetValue(BaseType, out value);
				_typeToDiscriminatorId[runtimeType] = value;
				break;
			default:
				if (runtimeType != BaseType)
				{
					ThrowHelper.ThrowNotSupportedException_RuntimeTypeNotSupported(BaseType, runtimeType);
				}
				break;
			}
		}
		if (value == null)
		{
			jsonTypeInfo = null;
			typeDiscriminator = null;
			return false;
		}
		jsonTypeInfo = value.JsonTypeInfo;
		typeDiscriminator = value.TypeDiscriminator;
		return true;
	}

	public bool TryGetDerivedJsonTypeInfo(object typeDiscriminator, [NotNullWhen(true)] out JsonTypeInfo jsonTypeInfo)
	{
		if (_discriminatorIdtoType.TryGetValue(typeDiscriminator, out var value))
		{
			jsonTypeInfo = value.JsonTypeInfo;
			return true;
		}
		if (!IgnoreUnrecognizedTypeDiscriminators)
		{
			ThrowHelper.ThrowJsonException_UnrecognizedTypeDiscriminator(typeDiscriminator);
		}
		jsonTypeInfo = null;
		return false;
	}

	public static bool IsSupportedPolymorphicBaseType(Type type)
	{
		if (type != null && (type.IsClass || type.IsInterface) && !type.IsSealed && !type.IsGenericTypeDefinition && !type.IsPointer)
		{
			return type != JsonTypeInfo.ObjectType;
		}
		return false;
	}

	public static bool IsSupportedDerivedType(Type baseType, Type derivedType)
	{
		if (baseType.IsAssignableFrom(derivedType))
		{
			return !derivedType.IsGenericTypeDefinition;
		}
		return false;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2070:UnrecognizedReflectionPattern", Justification = "The call to GetInterfaces will cross-reference results with interface types already declared as derived types of the polymorphic base type.")]
	private DerivedJsonTypeInfo CalculateNearestAncestor(Type type)
	{
		if (type == BaseType)
		{
			return null;
		}
		DerivedJsonTypeInfo value = null;
		Type baseType = type.BaseType;
		while (BaseType.IsAssignableFrom(baseType) && !_typeToDiscriminatorId.TryGetValue(baseType, out value))
		{
			baseType = baseType.BaseType;
		}
		if (BaseType.IsInterface)
		{
			Type[] interfaces = type.GetInterfaces();
			foreach (Type type2 in interfaces)
			{
				if (type2 != BaseType && BaseType.IsAssignableFrom(type2) && _typeToDiscriminatorId.TryGetValue(type2, out var value2) && value2 != null)
				{
					if (value == null)
					{
						value = value2;
					}
					else
					{
						ThrowHelper.ThrowNotSupportedException_RuntimeTypeDiamondAmbiguity(BaseType, type, value.JsonTypeInfo.Type, value2.JsonTypeInfo.Type);
					}
				}
			}
		}
		return value;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2075:UnrecognizedReflectionPattern", Justification = "The call to GetInterfaces will cross-reference results with interface types already declared as derived types of the polymorphic base type.")]
	internal static JsonTypeInfo FindNearestPolymorphicBaseType(JsonTypeInfo typeInfo)
	{
		if (typeInfo.PolymorphismOptions != null)
		{
			return null;
		}
		JsonTypeInfo jsonTypeInfo = null;
		Type baseType = typeInfo.Type.BaseType;
		while (baseType != null)
		{
			JsonTypeInfo jsonTypeInfo2 = ResolveAncestorTypeInfo(baseType, typeInfo.Options);
			if (jsonTypeInfo2?.PolymorphismOptions != null)
			{
				jsonTypeInfo = jsonTypeInfo2;
				break;
			}
			baseType = baseType.BaseType;
		}
		Type[] interfaces = typeInfo.Type.GetInterfaces();
		foreach (Type type in interfaces)
		{
			JsonTypeInfo jsonTypeInfo3 = ResolveAncestorTypeInfo(type, typeInfo.Options);
			if (jsonTypeInfo3?.PolymorphismOptions == null)
			{
				continue;
			}
			if (jsonTypeInfo != null)
			{
				if (jsonTypeInfo.Type.IsAssignableFrom(type))
				{
					jsonTypeInfo = jsonTypeInfo3;
				}
				else if (!type.IsAssignableFrom(jsonTypeInfo.Type))
				{
					return null;
				}
			}
			else
			{
				jsonTypeInfo = jsonTypeInfo3;
			}
		}
		return jsonTypeInfo;
		static JsonTypeInfo ResolveAncestorTypeInfo(Type type2, JsonSerializerOptions options)
		{
			try
			{
				return options.GetTypeInfoInternal(type2, ensureConfigured: true, null);
			}
			catch
			{
				return null;
			}
		}
	}
}
