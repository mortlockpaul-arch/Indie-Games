using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Metadata;

namespace System.Text.Json.Serialization;

internal abstract class JsonDictionaryConverter<TDictionary> : JsonResumableConverter<TDictionary>
{
	internal override bool SupportsCreateObjectDelegate => true;

	private protected sealed override ConverterStrategy GetDefaultConverterStrategy()
	{
		return ConverterStrategy.Dictionary;
	}

	protected internal abstract bool OnWriteResume(Utf8JsonWriter writer, TDictionary dictionary, JsonSerializerOptions options, ref WriteStack state);
}
internal abstract class JsonDictionaryConverter<TDictionary, TKey, TValue> : JsonDictionaryConverter<TDictionary>
{
	protected JsonConverter<TKey> _keyConverter;

	protected JsonConverter<TValue> _valueConverter;

	internal override Type ElementType => typeof(TValue);

	internal override Type KeyType => typeof(TKey);

	protected abstract void Add(TKey key, in TValue value, JsonSerializerOptions options, ref ReadStack state);

	protected virtual void ConvertCollection(ref ReadStack state, JsonSerializerOptions options)
	{
	}

	protected virtual void CreateCollection(ref Utf8JsonReader reader, scoped ref ReadStack state)
	{
		JsonPropertyInfo parentProperty = state.ParentProperty;
		if (parentProperty == null || !parentProperty.TryGetPrePopulatedValue(ref state))
		{
			JsonTypeInfo jsonTypeInfo = state.Current.JsonTypeInfo;
			if (jsonTypeInfo.CreateObject == null)
			{
				ThrowHelper.ThrowNotSupportedException_DeserializeNoConstructor(jsonTypeInfo, ref reader, ref state);
			}
			state.Current.ReturnValue = jsonTypeInfo.CreateObject();
		}
	}

	protected static JsonConverter<T> GetConverter<T>(JsonTypeInfo typeInfo)
	{
		return ((JsonTypeInfo<T>)typeInfo).EffectiveConverter;
	}

	internal sealed override bool OnTryRead(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options, scoped ref ReadStack state, [MaybeNullWhen(false)] out TDictionary value)
	{
		JsonTypeInfo jsonTypeInfo = state.Current.JsonTypeInfo;
		JsonTypeInfo keyTypeInfo = jsonTypeInfo.KeyTypeInfo;
		JsonTypeInfo elementTypeInfo = jsonTypeInfo.ElementTypeInfo;
		if (!state.SupportContinuation && !state.Current.CanContainMetadata)
		{
			if (reader.TokenType != JsonTokenType.StartObject)
			{
				ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(Type);
			}
			CreateCollection(ref reader, ref state);
			jsonTypeInfo.OnDeserializing?.Invoke(state.Current.ReturnValue);
			if (_keyConverter == null)
			{
				_keyConverter = GetConverter<TKey>(keyTypeInfo);
			}
			if (_valueConverter == null)
			{
				_valueConverter = GetConverter<TValue>(elementTypeInfo);
			}
			if (_valueConverter.CanUseDirectReadOrWrite && !state.Current.NumberHandling.HasValue)
			{
				while (true)
				{
					reader.ReadWithVerify();
					if (reader.TokenType == JsonTokenType.EndObject)
					{
						break;
					}
					state.Current.JsonPropertyInfo = keyTypeInfo.PropertyInfoForTypeInfo;
					TKey key = ReadDictionaryKey(_keyConverter, ref reader, ref state, options);
					reader.ReadWithVerify();
					state.Current.JsonPropertyInfo = elementTypeInfo.PropertyInfoForTypeInfo;
					Add(key, _valueConverter.Read(ref reader, ElementType, options), options, ref state);
				}
			}
			else
			{
				while (true)
				{
					reader.ReadWithVerify();
					if (reader.TokenType == JsonTokenType.EndObject)
					{
						break;
					}
					state.Current.JsonPropertyInfo = keyTypeInfo.PropertyInfoForTypeInfo;
					TKey key2 = ReadDictionaryKey(_keyConverter, ref reader, ref state, options);
					reader.ReadWithVerify();
					state.Current.JsonPropertyInfo = elementTypeInfo.PropertyInfoForTypeInfo;
					_valueConverter.TryRead(ref reader, ElementType, options, ref state, out var value2, out var _);
					Add(key2, in value2, options, ref state);
				}
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
					value = default(TDictionary);
					return false;
				}
				if (state.Current.MetadataPropertyNames == MetadataPropertyName.Ref)
				{
					value = JsonSerializer.ResolveReferenceId<TDictionary>(ref state);
					return true;
				}
				state.Current.ObjectState = StackFrameObjectState.ReadMetadata;
			}
			if ((state.Current.MetadataPropertyNames & MetadataPropertyName.Type) != MetadataPropertyName.None && state.Current.PolymorphicSerializationState != PolymorphicSerializationState.PolymorphicReEntryStarted)
			{
				JsonConverter jsonConverter = ResolvePolymorphicConverter(jsonTypeInfo, ref state);
				if (jsonConverter != null)
				{
					bool flag = jsonConverter.OnTryReadAsObject(ref reader, jsonConverter.Type, options, ref state, out var value3);
					value = (TDictionary)value3;
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
				CreateCollection(ref reader, ref state);
				if ((state.Current.MetadataPropertyNames & MetadataPropertyName.Id) != MetadataPropertyName.None)
				{
					state.ReferenceResolver.AddReference(state.ReferenceId, state.Current.ReturnValue);
					state.ReferenceId = null;
				}
				jsonTypeInfo.OnDeserializing?.Invoke(state.Current.ReturnValue);
				state.Current.ObjectState = StackFrameObjectState.CreatedObject;
			}
			if (_keyConverter == null)
			{
				_keyConverter = GetConverter<TKey>(keyTypeInfo);
			}
			if (_valueConverter == null)
			{
				_valueConverter = GetConverter<TValue>(elementTypeInfo);
			}
			while (true)
			{
				if (state.Current.PropertyState == StackFramePropertyState.None)
				{
					if (!reader.Read())
					{
						value = default(TDictionary);
						return false;
					}
					state.Current.PropertyState = StackFramePropertyState.ReadName;
				}
				TKey val;
				if ((int)state.Current.PropertyState < 2)
				{
					if (reader.TokenType == JsonTokenType.EndObject)
					{
						break;
					}
					state.Current.PropertyState = StackFramePropertyState.Name;
					if (state.Current.CanContainMetadata)
					{
						ReadOnlySpan<byte> unescapedSpan = reader.GetUnescapedSpan();
						if (JsonSerializer.IsMetadataPropertyName(unescapedSpan, state.Current.BaseJsonTypeInfo.PolymorphicTypeResolver))
						{
							if (options.AllowOutOfOrderMetadataProperties)
							{
								reader.SkipWithVerify();
								state.Current.EndElement();
								continue;
							}
							ThrowHelper.ThrowUnexpectedMetadataException(unescapedSpan, ref reader, ref state);
						}
					}
					state.Current.JsonPropertyInfo = keyTypeInfo.PropertyInfoForTypeInfo;
					val = ReadDictionaryKey(_keyConverter, ref reader, ref state, options);
				}
				else
				{
					val = (TKey)state.Current.DictionaryKey;
				}
				if ((int)state.Current.PropertyState < 3)
				{
					if (!reader.TryAdvanceWithOptionalReadAhead(_valueConverter.RequiresReadAhead))
					{
						state.Current.DictionaryKey = val;
						value = default(TDictionary);
						return false;
					}
					state.Current.PropertyState = StackFramePropertyState.ReadValue;
				}
				if ((int)state.Current.PropertyState < 5)
				{
					state.Current.JsonPropertyInfo = elementTypeInfo.PropertyInfoForTypeInfo;
					if (!_valueConverter.TryRead(ref reader, typeof(TValue), options, ref state, out var value4, out var _))
					{
						state.Current.DictionaryKey = val;
						value = default(TDictionary);
						return false;
					}
					Add(val, in value4, options, ref state);
					state.Current.EndElement();
				}
			}
		}
		ConvertCollection(ref state, options);
		object returnValue = state.Current.ReturnValue;
		jsonTypeInfo.OnDeserialized?.Invoke(returnValue);
		value = (TDictionary)returnValue;
		return true;
		static TKey ReadDictionaryKey(JsonConverter<TKey> keyConverter, ref Utf8JsonReader reference, scoped ref ReadStack reference2, JsonSerializerOptions options2)
		{
			string text = reference.GetString();
			reference2.Current.JsonPropertyNameAsString = text;
			if (keyConverter.IsInternalConverter && keyConverter.Type == typeof(string))
			{
				return (TKey)(object)text;
			}
			return keyConverter.ReadAsPropertyNameCore(ref reference, keyConverter.Type, options2);
		}
	}

	internal sealed override bool OnTryWrite(Utf8JsonWriter writer, TDictionary dictionary, JsonSerializerOptions options, ref WriteStack state)
	{
		if (dictionary == null)
		{
			writer.WriteNullValue();
			return true;
		}
		JsonTypeInfo jsonTypeInfo = state.Current.JsonTypeInfo;
		if (!state.Current.ProcessedStartToken)
		{
			state.Current.ProcessedStartToken = true;
			jsonTypeInfo.OnSerializing?.Invoke(dictionary);
			writer.WriteStartObject();
			if (state.CurrentContainsMetadata && CanHaveMetadata)
			{
				JsonSerializer.WriteMetadataForObject(this, ref state, writer);
			}
			state.Current.JsonPropertyInfo = jsonTypeInfo.ElementTypeInfo.PropertyInfoForTypeInfo;
		}
		bool num = OnWriteResume(writer, dictionary, options, ref state);
		if (num)
		{
			if (!state.Current.ProcessedEndToken)
			{
				state.Current.ProcessedEndToken = true;
				writer.WriteEndObject();
			}
			Action<object>? onSerialized = jsonTypeInfo.OnSerialized;
			if (onSerialized == null)
			{
				return num;
			}
			onSerialized(dictionary);
		}
		return num;
	}
}
