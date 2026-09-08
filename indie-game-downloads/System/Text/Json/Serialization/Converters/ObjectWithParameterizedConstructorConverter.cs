using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization.Metadata;

namespace System.Text.Json.Serialization.Converters;

internal abstract class ObjectWithParameterizedConstructorConverter<T> : ObjectDefaultConverter<T>
{
	internal sealed override bool ConstructorIsParameterized => true;

	internal sealed override bool OnTryRead(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options, scoped ref ReadStack state, [MaybeNullWhen(false)] out T value)
	{
		JsonTypeInfo jsonTypeInfo = state.Current.JsonTypeInfo;
		if (!jsonTypeInfo.UsesParameterizedConstructor || state.Current.IsPopulating)
		{
			return base.OnTryRead(ref reader, typeToConvert, options, ref state, out value);
		}
		ArgumentState ctorArgumentState = state.Current.CtorArgumentState;
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
				object returnValue = state.Current.ReturnValue;
				ObjectDefaultConverter<T>.PopulatePropertiesFastPath(returnValue, jsonTypeInfo, options, ref reader, ref state);
				value = (T)returnValue;
				return true;
			}
			ReadOnlySpan<byte> originalSpan = reader.OriginalSpan;
			ReadOnlySequence<byte> originalSequence = reader.OriginalSequence;
			ReadConstructorArguments(ref state, ref reader, options);
			state.Current.ValidateAllRequiredPropertiesAreRead(jsonTypeInfo);
			obj = (T)CreateObject(ref state.Current);
			jsonTypeInfo.OnDeserializing?.Invoke(obj);
			if (ctorArgumentState.FoundPropertyCount > 0)
			{
				(JsonPropertyInfo, JsonReaderState, long, byte[], string)[] foundProperties = ctorArgumentState.FoundProperties;
				for (int i = 0; i < ctorArgumentState.FoundPropertyCount; i++)
				{
					JsonPropertyInfo item = foundProperties[i].Item1;
					long item2 = foundProperties[i].Item3;
					byte[] item3 = foundProperties[i].Item4;
					string item4 = foundProperties[i].Item5;
					Utf8JsonReader reader2 = (originalSequence.IsEmpty ? new Utf8JsonReader(originalSpan.Slice(checked((int)item2)), isFinalBlock: true, foundProperties[i].Item2) : new Utf8JsonReader(originalSequence.Slice(item2), isFinalBlock: true, foundProperties[i].Item2));
					state.Current.JsonPropertyName = item3;
					state.Current.JsonPropertyInfo = item;
					state.Current.NumberHandling = item.EffectiveNumberHandling;
					bool flag = item4 != null;
					if (flag)
					{
						state.Current.JsonPropertyNameAsString = item4;
						JsonSerializer.CreateExtensionDataProperty(obj, item, options);
					}
					ObjectDefaultConverter<T>.ReadPropertyValue(obj, ref state, ref reader2, item, flag);
				}
				(JsonPropertyInfo, JsonReaderState, long, byte[], string)[] foundProperties2 = ctorArgumentState.FoundProperties;
				ctorArgumentState.FoundProperties = null;
				ArrayPool<(JsonPropertyInfo, JsonReaderState, long, byte[], string)>.Shared.Return(foundProperties2, clearArray: true);
			}
		}
		else
		{
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
					bool flag2 = jsonConverter.OnTryReadAsObject(ref reader, jsonConverter.Type, options, ref state, out var value2);
					value = (T)value2;
					state.ExitPolymorphicConverter(flag2);
					return flag2;
				}
			}
			JsonPropertyInfo parentProperty2 = state.ParentProperty;
			if (parentProperty2 != null && parentProperty2.TryGetPrePopulatedValue(ref state))
			{
				object returnValue2 = state.Current.ReturnValue;
				jsonTypeInfo.OnDeserializing?.Invoke(returnValue2);
				state.Current.ObjectState = StackFrameObjectState.CreatedObject;
				state.Current.InitializePropertiesValidationState(jsonTypeInfo);
				return base.OnTryRead(ref reader, typeToConvert, options, ref state, out value);
			}
			if ((int)state.Current.ObjectState < 3)
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
				BeginRead(ref state, options);
				state.Current.ObjectState = StackFrameObjectState.ConstructorArguments;
			}
			if (!ReadConstructorArgumentsWithContinuation(ref state, ref reader, options))
			{
				value = default(T);
				return false;
			}
			state.Current.ValidateAllRequiredPropertiesAreRead(jsonTypeInfo);
			obj = (T)CreateObject(ref state.Current);
			if ((state.Current.MetadataPropertyNames & MetadataPropertyName.Id) != MetadataPropertyName.None)
			{
				state.ReferenceResolver.AddReference(state.ReferenceId, obj);
				state.ReferenceId = null;
			}
			jsonTypeInfo.OnDeserializing?.Invoke(obj);
			if (ctorArgumentState.FoundPropertyCount > 0)
			{
				for (int j = 0; j < ctorArgumentState.FoundPropertyCount; j++)
				{
					JsonPropertyInfo item5 = ctorArgumentState.FoundPropertiesAsync[j].Item1;
					object item6 = ctorArgumentState.FoundPropertiesAsync[j].Item2;
					string item7 = ctorArgumentState.FoundPropertiesAsync[j].Item3;
					if (item7 == null)
					{
						if (item6 != null || !item5.IgnoreNullTokensOnRead || default(T) != null)
						{
							item5.Set(obj, item6);
						}
						continue;
					}
					JsonSerializer.CreateExtensionDataProperty(obj, item5, options);
					object valueAsObject = item5.GetValueAsObject(obj);
					if (valueAsObject is IDictionary<string, JsonElement> dictionary)
					{
						if (options.AllowDuplicateProperties)
						{
							dictionary[item7] = (JsonElement)item6;
						}
						else if (!dictionary.TryAdd(item7, (JsonElement)item6))
						{
							ThrowHelper.ThrowJsonException_DuplicatePropertyNotAllowed(item7);
						}
						continue;
					}
					IDictionary<string, object> dictionary2 = (IDictionary<string, object>)valueAsObject;
					if (options.AllowDuplicateProperties)
					{
						dictionary2[item7] = item6;
					}
					else if (!dictionary2.TryAdd(item7, item6))
					{
						ThrowHelper.ThrowJsonException_DuplicatePropertyNotAllowed(item7);
					}
				}
				(JsonPropertyInfo, object, string)[] foundPropertiesAsync = ctorArgumentState.FoundPropertiesAsync;
				ctorArgumentState.FoundPropertiesAsync = null;
				ArrayPool<(JsonPropertyInfo, object, string)>.Shared.Return(foundPropertiesAsync, clearArray: true);
			}
		}
		jsonTypeInfo.OnDeserialized?.Invoke(obj);
		value = (T)obj;
		if (state.Current.PropertyRefCacheBuilder != null)
		{
			jsonTypeInfo.UpdateUtf8PropertyCache(ref state.Current);
		}
		return true;
	}

	protected abstract void InitializeConstructorArgumentCaches(ref ReadStack state, JsonSerializerOptions options);

	protected abstract bool ReadAndCacheConstructorArgument(scoped ref ReadStack state, ref Utf8JsonReader reader, JsonParameterInfo jsonParameterInfo);

	protected abstract object CreateObject(ref ReadStackFrame frame);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ReadConstructorArguments(scoped ref ReadStack state, ref Utf8JsonReader reader, JsonSerializerOptions options)
	{
		BeginRead(ref state, options);
		while (true)
		{
			reader.ReadWithVerify();
			if (reader.TokenType == JsonTokenType.EndObject)
			{
				break;
			}
			ReadOnlySpan<byte> propertyName = JsonSerializer.GetPropertyName(ref state, ref reader, options, out var isAlreadyReadMetadataProperty);
			if (isAlreadyReadMetadataProperty)
			{
				reader.SkipWithVerify();
				state.Current.EndProperty();
				continue;
			}
			if (TryLookupConstructorParameter(propertyName, ref state, options, out var jsonPropertyInfo, out var jsonParameterInfo))
			{
				reader.ReadWithVerify();
				if (!jsonParameterInfo.ShouldDeserialize)
				{
					reader.TrySkip();
					state.Current.EndConstructorParameter();
				}
				else
				{
					ReadAndCacheConstructorArgument(ref state, ref reader, jsonParameterInfo);
					state.Current.EndConstructorParameter();
				}
				continue;
			}
			if (jsonPropertyInfo.CanDeserialize)
			{
				ArgumentState ctorArgumentState = state.Current.CtorArgumentState;
				if (ctorArgumentState.FoundProperties == null)
				{
					ctorArgumentState.FoundProperties = ArrayPool<(JsonPropertyInfo, JsonReaderState, long, byte[], string)>.Shared.Rent(Math.Max(1, state.Current.JsonTypeInfo.PropertyCache.Length));
				}
				else if (ctorArgumentState.FoundPropertyCount == ctorArgumentState.FoundProperties.Length)
				{
					(JsonPropertyInfo, JsonReaderState, long, byte[], string)[] array = ArrayPool<(JsonPropertyInfo, JsonReaderState, long, byte[], string)>.Shared.Rent(ctorArgumentState.FoundProperties.Length * 2);
					ctorArgumentState.FoundProperties.CopyTo(array, 0);
					(JsonPropertyInfo, JsonReaderState, long, byte[], string)[] foundProperties = ctorArgumentState.FoundProperties;
					ctorArgumentState.FoundProperties = array;
					ArrayPool<(JsonPropertyInfo, JsonReaderState, long, byte[], string)>.Shared.Return(foundProperties, clearArray: true);
				}
				ctorArgumentState.FoundProperties[ctorArgumentState.FoundPropertyCount++] = (jsonPropertyInfo, reader.CurrentState, reader.BytesConsumed, state.Current.JsonPropertyName, state.Current.JsonPropertyNameAsString);
			}
			reader.SkipWithVerify();
			state.Current.EndProperty();
		}
	}

	private bool ReadConstructorArgumentsWithContinuation(scoped ref ReadStack state, ref Utf8JsonReader reader, JsonSerializerOptions options)
	{
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
			JsonPropertyInfo jsonPropertyInfo;
			JsonParameterInfo jsonParameterInfo;
			if ((int)state.Current.PropertyState < 2)
			{
				if (reader.TokenType == JsonTokenType.EndObject)
				{
					return true;
				}
				ReadOnlySpan<byte> propertyName = JsonSerializer.GetPropertyName(ref state, ref reader, options, out var isAlreadyReadMetadataProperty);
				if (isAlreadyReadMetadataProperty)
				{
					reader.SkipWithVerify();
					state.Current.EndProperty();
					continue;
				}
				if (TryLookupConstructorParameter(propertyName, ref state, options, out jsonPropertyInfo, out jsonParameterInfo))
				{
					jsonPropertyInfo = null;
				}
				state.Current.PropertyState = StackFramePropertyState.Name;
			}
			else
			{
				jsonParameterInfo = state.Current.CtorArgumentState.JsonParameterInfo;
				jsonPropertyInfo = state.Current.JsonPropertyInfo;
			}
			if (jsonParameterInfo != null)
			{
				if (!HandleConstructorArgumentWithContinuation(ref state, ref reader, jsonParameterInfo))
				{
					return false;
				}
			}
			else if (!HandlePropertyWithContinuation(ref state, ref reader, jsonPropertyInfo))
			{
				break;
			}
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool HandleConstructorArgumentWithContinuation(scoped ref ReadStack state, ref Utf8JsonReader reader, JsonParameterInfo jsonParameterInfo)
	{
		if ((int)state.Current.PropertyState < 3)
		{
			if (!jsonParameterInfo.ShouldDeserialize)
			{
				if (!reader.TrySkipPartial(state.Current.OriginalDepth + 1))
				{
					return false;
				}
				state.Current.EndConstructorParameter();
				return true;
			}
			if (!reader.TryAdvanceWithOptionalReadAhead(jsonParameterInfo.EffectiveConverter.RequiresReadAhead))
			{
				return false;
			}
			state.Current.PropertyState = StackFramePropertyState.ReadValue;
		}
		if (!ReadAndCacheConstructorArgument(ref state, ref reader, jsonParameterInfo))
		{
			return false;
		}
		state.Current.EndConstructorParameter();
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool HandlePropertyWithContinuation(scoped ref ReadStack state, ref Utf8JsonReader reader, JsonPropertyInfo jsonPropertyInfo)
	{
		if ((int)state.Current.PropertyState < 3)
		{
			if (!jsonPropertyInfo.CanDeserialize)
			{
				if (!reader.TrySkipPartial(state.Current.OriginalDepth + 1))
				{
					return false;
				}
				state.Current.EndProperty();
				return true;
			}
			if (!ObjectDefaultConverter<T>.ReadAheadPropertyValue(ref state, ref reader, jsonPropertyInfo))
			{
				return false;
			}
			state.Current.PropertyState = StackFramePropertyState.ReadValue;
		}
		object value;
		if (state.Current.UseExtensionProperty)
		{
			if (!jsonPropertyInfo.ReadJsonExtensionDataValue(ref state, ref reader, out value))
			{
				return false;
			}
		}
		else if (!jsonPropertyInfo.ReadJsonAsObject(ref state, ref reader, out value))
		{
			return false;
		}
		ArgumentState ctorArgumentState = state.Current.CtorArgumentState;
		if (ctorArgumentState.FoundPropertiesAsync == null)
		{
			ctorArgumentState.FoundPropertiesAsync = ArrayPool<(JsonPropertyInfo, object, string)>.Shared.Rent(Math.Max(1, state.Current.JsonTypeInfo.PropertyCache.Length));
		}
		else if (ctorArgumentState.FoundPropertyCount == ctorArgumentState.FoundPropertiesAsync.Length)
		{
			(JsonPropertyInfo, object, string)[] array = ArrayPool<(JsonPropertyInfo, object, string)>.Shared.Rent(ctorArgumentState.FoundPropertiesAsync.Length * 2);
			ctorArgumentState.FoundPropertiesAsync.CopyTo(array, 0);
			(JsonPropertyInfo, object, string)[] foundPropertiesAsync = ctorArgumentState.FoundPropertiesAsync;
			ctorArgumentState.FoundPropertiesAsync = array;
			ArrayPool<(JsonPropertyInfo, object, string)>.Shared.Return(foundPropertiesAsync, clearArray: true);
		}
		ctorArgumentState.FoundPropertiesAsync[ctorArgumentState.FoundPropertyCount++] = (jsonPropertyInfo, value, state.Current.JsonPropertyNameAsString);
		state.Current.EndProperty();
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void BeginRead(scoped ref ReadStack state, JsonSerializerOptions options)
	{
		JsonTypeInfo jsonTypeInfo = state.Current.JsonTypeInfo;
		jsonTypeInfo.ValidateCanBeUsedForPropertyMetadataSerialization();
		if (jsonTypeInfo.ParameterCount != jsonTypeInfo.ParameterCache.Length)
		{
			ThrowHelper.ThrowInvalidOperationException_ConstructorParameterIncompleteBinding(Type);
		}
		state.Current.InitializePropertiesValidationState(jsonTypeInfo);
		state.Current.JsonPropertyInfo = null;
		InitializeConstructorArgumentCaches(ref state, options);
	}

	protected static bool TryLookupConstructorParameter(scoped ReadOnlySpan<byte> unescapedPropertyName, scoped ref ReadStack state, JsonSerializerOptions options, out JsonPropertyInfo jsonPropertyInfo, [NotNullWhen(true)] out JsonParameterInfo jsonParameterInfo)
	{
		jsonPropertyInfo = JsonSerializer.LookupProperty(null, unescapedPropertyName, ref state, options, out var useExtensionProperty, createExtensionProperty: false);
		if (!useExtensionProperty && jsonPropertyInfo != JsonPropertyInfo.s_missingProperty)
		{
			state.Current.MarkPropertyAsRead(jsonPropertyInfo);
		}
		jsonParameterInfo = jsonPropertyInfo.AssociatedParameter;
		if (jsonParameterInfo != null)
		{
			state.Current.JsonPropertyInfo = null;
			state.Current.CtorArgumentState.JsonParameterInfo = jsonParameterInfo;
			state.Current.NumberHandling = jsonParameterInfo.NumberHandling;
			return true;
		}
		state.Current.UseExtensionProperty = useExtensionProperty;
		return false;
	}
}
