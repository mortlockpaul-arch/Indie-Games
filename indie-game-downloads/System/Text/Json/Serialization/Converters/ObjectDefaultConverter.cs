using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization.Metadata;

namespace System.Text.Json.Serialization.Converters;

internal class ObjectDefaultConverter<T> : JsonObjectConverter<T>
{
	internal override bool CanHaveMetadata => true;

	internal override bool SupportsCreateObjectDelegate => true;

	internal override bool OnTryRead(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options, scoped ref ReadStack state, [MaybeNullWhen(false)] out T value)
	{
		JsonTypeInfo jsonTypeInfo = state.Current.JsonTypeInfo;
		object obj;
		if (!state.SupportContinuation && !state.Current.CanContainMetadata)
		{
			if (reader.TokenType != JsonTokenType.StartObject)
			{
				ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(Type);
			}
			JsonPropertyInfo parentProperty = state.ParentProperty;
			if (parentProperty != null && parentProperty.TryGetPrePopulatedValue(ref state))
			{
				obj = state.Current.ReturnValue;
			}
			else
			{
				if (jsonTypeInfo.CreateObject == null)
				{
					ThrowHelper.ThrowNotSupportedException_DeserializeNoConstructor(jsonTypeInfo, ref reader, ref state);
				}
				obj = jsonTypeInfo.CreateObject();
			}
			PopulatePropertiesFastPath(obj, jsonTypeInfo, options, ref reader, ref state);
			value = (T)obj;
			return true;
		}
		if (state.Current.ObjectState == StackFrameObjectState.None)
		{
			if (reader.TokenType != JsonTokenType.StartObject)
			{
				ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(Type);
			}
			state.Current.ObjectState = StackFrameObjectState.StartToken;
		}
		if (state.Current.CanContainMetadata && (int)state.Current.ObjectState < 2)
		{
			if (!JsonSerializer.TryReadMetadata(this, jsonTypeInfo, ref reader, ref state))
			{
				value = default(T);
				return false;
			}
			if (state.Current.MetadataPropertyNames == MetadataPropertyName.Ref)
			{
				value = JsonSerializer.ResolveReferenceId<T>(ref state);
				return true;
			}
			state.Current.ObjectState = StackFrameObjectState.ReadMetadata;
		}
		if ((state.Current.MetadataPropertyNames & MetadataPropertyName.Type) != MetadataPropertyName.None && state.Current.PolymorphicSerializationState != PolymorphicSerializationState.PolymorphicReEntryStarted)
		{
			JsonConverter jsonConverter = ResolvePolymorphicConverter(jsonTypeInfo, ref state);
			if (jsonConverter != null)
			{
				bool flag = jsonConverter.OnTryReadAsObject(ref reader, jsonConverter.Type, options, ref state, out var value2);
				value = (T)value2;
				state.ExitPolymorphicConverter(flag);
				return flag;
			}
		}
		if ((int)state.Current.ObjectState < 4)
		{
			if (state.Current.CanContainMetadata)
			{
				JsonSerializer.ValidateMetadataForObjectConverter(ref state);
			}
			if (state.Current.MetadataPropertyNames == MetadataPropertyName.Ref)
			{
				value = JsonSerializer.ResolveReferenceId<T>(ref state);
				return true;
			}
			JsonPropertyInfo parentProperty2 = state.ParentProperty;
			if (parentProperty2 != null && parentProperty2.TryGetPrePopulatedValue(ref state))
			{
				obj = state.Current.ReturnValue;
			}
			else
			{
				if (jsonTypeInfo.CreateObject == null)
				{
					ThrowHelper.ThrowNotSupportedException_DeserializeNoConstructor(jsonTypeInfo, ref reader, ref state);
				}
				obj = jsonTypeInfo.CreateObject();
			}
			if ((state.Current.MetadataPropertyNames & MetadataPropertyName.Id) != MetadataPropertyName.None)
			{
				state.ReferenceResolver.AddReference(state.ReferenceId, obj);
				state.ReferenceId = null;
			}
			jsonTypeInfo.OnDeserializing?.Invoke(obj);
			state.Current.ReturnValue = obj;
			state.Current.ObjectState = StackFrameObjectState.CreatedObject;
			state.Current.InitializePropertiesValidationState(jsonTypeInfo);
		}
		else
		{
			obj = state.Current.ReturnValue;
		}
		while (true)
		{
			if (state.Current.PropertyState == StackFramePropertyState.None)
			{
				if (!reader.Read())
				{
					state.Current.ReturnValue = obj;
					value = default(T);
					return false;
				}
				state.Current.PropertyState = StackFramePropertyState.ReadName;
			}
			JsonPropertyInfo jsonPropertyInfo;
			if ((int)state.Current.PropertyState < 2)
			{
				if (reader.TokenType == JsonTokenType.EndObject)
				{
					break;
				}
				jsonTypeInfo.ValidateCanBeUsedForPropertyMetadataSerialization();
				ReadOnlySpan<byte> propertyName = JsonSerializer.GetPropertyName(ref state, ref reader, options, out var isAlreadyReadMetadataProperty);
				if (isAlreadyReadMetadataProperty)
				{
					reader.SkipWithVerify();
					state.Current.EndProperty();
					continue;
				}
				jsonPropertyInfo = JsonSerializer.LookupProperty(obj, propertyName, ref state, options, out var useExtensionProperty);
				state.Current.UseExtensionProperty = useExtensionProperty;
				state.Current.PropertyState = StackFramePropertyState.Name;
			}
			else
			{
				jsonPropertyInfo = state.Current.JsonPropertyInfo;
			}
			if ((int)state.Current.PropertyState < 3)
			{
				if (!jsonPropertyInfo.CanDeserializeOrPopulate)
				{
					if (!reader.TrySkipPartial(state.Current.OriginalDepth + 1))
					{
						state.Current.ReturnValue = obj;
						value = default(T);
						return false;
					}
					state.Current.EndProperty();
					continue;
				}
				if (!ReadAheadPropertyValue(ref state, ref reader, jsonPropertyInfo))
				{
					state.Current.ReturnValue = obj;
					value = default(T);
					return false;
				}
				state.Current.PropertyState = StackFramePropertyState.ReadValue;
			}
			if ((int)state.Current.PropertyState >= 5)
			{
				continue;
			}
			if (!state.Current.UseExtensionProperty)
			{
				if (!jsonPropertyInfo.ReadJsonAndSetMember(obj, ref state, ref reader))
				{
					state.Current.ReturnValue = obj;
					value = default(T);
					return false;
				}
			}
			else if (!jsonPropertyInfo.ReadJsonAndAddExtensionProperty(obj, ref state, ref reader))
			{
				state.Current.ReturnValue = obj;
				value = default(T);
				return false;
			}
			state.Current.EndProperty();
		}
		jsonTypeInfo.OnDeserialized?.Invoke(obj);
		state.Current.ValidateAllRequiredPropertiesAreRead(jsonTypeInfo);
		value = (T)obj;
		if (state.Current.PropertyRefCacheBuilder != null)
		{
			jsonTypeInfo.UpdateUtf8PropertyCache(ref state.Current);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void PopulatePropertiesFastPath(object obj, JsonTypeInfo jsonTypeInfo, JsonSerializerOptions options, ref Utf8JsonReader reader, scoped ref ReadStack state)
	{
		jsonTypeInfo.OnDeserializing?.Invoke(obj);
		state.Current.InitializePropertiesValidationState(jsonTypeInfo);
		while (true)
		{
			reader.ReadWithVerify();
			if (reader.TokenType == JsonTokenType.EndObject)
			{
				break;
			}
			ReadOnlySpan<byte> propertyName = JsonSerializer.GetPropertyName(ref state, ref reader, options, out var _);
			jsonTypeInfo.ValidateCanBeUsedForPropertyMetadataSerialization();
			JsonPropertyInfo jsonPropertyInfo = JsonSerializer.LookupProperty(obj, propertyName, ref state, options, out var useExtensionProperty);
			ReadPropertyValue(obj, ref state, ref reader, jsonPropertyInfo, useExtensionProperty);
		}
		jsonTypeInfo.OnDeserialized?.Invoke(obj);
		state.Current.ValidateAllRequiredPropertiesAreRead(jsonTypeInfo);
		if (state.Current.PropertyRefCacheBuilder != null)
		{
			jsonTypeInfo.UpdateUtf8PropertyCache(ref state.Current);
		}
	}

	internal sealed override bool OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, ref WriteStack state)
	{
		JsonTypeInfo jsonTypeInfo = state.Current.JsonTypeInfo;
		jsonTypeInfo.ValidateCanBeUsedForPropertyMetadataSerialization();
		object obj = value;
		if (!state.SupportContinuation)
		{
			jsonTypeInfo.OnSerializing?.Invoke(obj);
			writer.WriteStartObject();
			if (state.CurrentContainsMetadata && CanHaveMetadata)
			{
				JsonSerializer.WriteMetadataForObject(this, ref state, writer);
			}
			ReadOnlySpan<JsonPropertyInfo> propertyCache = jsonTypeInfo.PropertyCache;
			for (int i = 0; i < propertyCache.Length; i++)
			{
				JsonPropertyInfo jsonPropertyInfo = propertyCache[i];
				if (jsonPropertyInfo.CanSerialize)
				{
					state.Current.JsonPropertyInfo = jsonPropertyInfo;
					state.Current.NumberHandling = jsonPropertyInfo.EffectiveNumberHandling;
					jsonPropertyInfo.GetMemberAndWriteJson(obj, ref state, writer);
					state.Current.EndProperty();
				}
			}
			JsonPropertyInfo extensionDataProperty = jsonTypeInfo.ExtensionDataProperty;
			if (extensionDataProperty != null && extensionDataProperty.CanSerialize)
			{
				state.Current.JsonPropertyInfo = extensionDataProperty;
				state.Current.NumberHandling = extensionDataProperty.EffectiveNumberHandling;
				extensionDataProperty.GetMemberAndWriteJsonExtensionData(obj, ref state, writer);
				state.Current.EndProperty();
			}
			writer.WriteEndObject();
		}
		else
		{
			if (!state.Current.ProcessedStartToken)
			{
				writer.WriteStartObject();
				if (state.CurrentContainsMetadata && CanHaveMetadata)
				{
					JsonSerializer.WriteMetadataForObject(this, ref state, writer);
				}
				jsonTypeInfo.OnSerializing?.Invoke(obj);
				state.Current.ProcessedStartToken = true;
			}
			ReadOnlySpan<JsonPropertyInfo> propertyCache2 = jsonTypeInfo.PropertyCache;
			while (state.Current.EnumeratorIndex < propertyCache2.Length)
			{
				JsonPropertyInfo jsonPropertyInfo2 = propertyCache2[state.Current.EnumeratorIndex];
				if (jsonPropertyInfo2.CanSerialize)
				{
					state.Current.JsonPropertyInfo = jsonPropertyInfo2;
					state.Current.NumberHandling = jsonPropertyInfo2.EffectiveNumberHandling;
					if (!jsonPropertyInfo2.GetMemberAndWriteJson(obj, ref state, writer))
					{
						return false;
					}
					state.Current.EndProperty();
					state.Current.EnumeratorIndex++;
					if (JsonConverter.ShouldFlush(ref state, writer))
					{
						return false;
					}
				}
				else
				{
					state.Current.EnumeratorIndex++;
				}
			}
			if (state.Current.EnumeratorIndex == propertyCache2.Length)
			{
				JsonPropertyInfo extensionDataProperty2 = jsonTypeInfo.ExtensionDataProperty;
				if (extensionDataProperty2 != null && extensionDataProperty2.CanSerialize)
				{
					state.Current.JsonPropertyInfo = extensionDataProperty2;
					state.Current.NumberHandling = extensionDataProperty2.EffectiveNumberHandling;
					if (!extensionDataProperty2.GetMemberAndWriteJsonExtensionData(obj, ref state, writer))
					{
						return false;
					}
					state.Current.EndProperty();
					state.Current.EnumeratorIndex++;
					if (JsonConverter.ShouldFlush(ref state, writer))
					{
						return false;
					}
				}
				else
				{
					state.Current.EnumeratorIndex++;
				}
			}
			if (!state.Current.ProcessedEndToken)
			{
				state.Current.ProcessedEndToken = true;
				writer.WriteEndObject();
			}
		}
		jsonTypeInfo.OnSerialized?.Invoke(obj);
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void ReadPropertyValue(object obj, scoped ref ReadStack state, ref Utf8JsonReader reader, JsonPropertyInfo jsonPropertyInfo, bool useExtensionProperty)
	{
		if (!jsonPropertyInfo.CanDeserializeOrPopulate)
		{
			reader.TrySkip();
		}
		else
		{
			reader.ReadWithVerify();
			if (!useExtensionProperty)
			{
				jsonPropertyInfo.ReadJsonAndSetMember(obj, ref state, ref reader);
			}
			else
			{
				jsonPropertyInfo.ReadJsonAndAddExtensionProperty(obj, ref state, ref reader);
			}
		}
		state.Current.EndProperty();
	}

	protected static bool ReadAheadPropertyValue(scoped ref ReadStack state, ref Utf8JsonReader reader, JsonPropertyInfo jsonPropertyInfo)
	{
		bool requiresReadAhead = jsonPropertyInfo.EffectiveConverter.RequiresReadAhead || state.Current.UseExtensionProperty;
		return reader.TryAdvanceWithOptionalReadAhead(requiresReadAhead);
	}
}
