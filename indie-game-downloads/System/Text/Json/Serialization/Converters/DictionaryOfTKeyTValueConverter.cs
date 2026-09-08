using System.Collections.Generic;
using System.Text.Json.Serialization.Metadata;

namespace System.Text.Json.Serialization.Converters;

internal sealed class DictionaryOfTKeyTValueConverter<TCollection, TKey, TValue> : DictionaryDefaultConverter<TCollection, TKey, TValue> where TCollection : Dictionary<TKey, TValue>
{
	internal override bool CanPopulate => true;

	protected override void Add(TKey key, in TValue value, JsonSerializerOptions options, ref ReadStack state)
	{
		TCollection val = (TCollection)state.Current.ReturnValue;
		if (options.AllowDuplicateProperties)
		{
			val[key] = value;
		}
		else if (!val.TryAdd(key, value))
		{
			ThrowHelper.ThrowJsonException_DuplicatePropertyNotAllowed();
		}
	}

	protected internal override bool OnWriteResume(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, ref WriteStack state)
	{
		Dictionary<TKey, TValue>.Enumerator enumerator;
		if (state.Current.CollectionEnumerator == null)
		{
			enumerator = value.GetEnumerator();
			if (!enumerator.MoveNext())
			{
				enumerator.Dispose();
				return true;
			}
		}
		else
		{
			enumerator = (Dictionary<TKey, TValue>.Enumerator)(object)state.Current.CollectionEnumerator;
		}
		JsonTypeInfo jsonTypeInfo = state.Current.JsonTypeInfo;
		if (_keyConverter == null)
		{
			_keyConverter = JsonDictionaryConverter<TCollection, TKey, TValue>.GetConverter<TKey>(jsonTypeInfo.KeyTypeInfo);
		}
		if (_valueConverter == null)
		{
			_valueConverter = JsonDictionaryConverter<TCollection, TKey, TValue>.GetConverter<TValue>(jsonTypeInfo.ElementTypeInfo);
		}
		if (!state.SupportContinuation && _valueConverter.CanUseDirectReadOrWrite && !state.Current.NumberHandling.HasValue)
		{
			do
			{
				TKey key = enumerator.Current.Key;
				_keyConverter.WriteAsPropertyNameCore(writer, key, options, state.Current.IsWritingExtensionDataProperty);
				_valueConverter.Write(writer, enumerator.Current.Value, options);
			}
			while (enumerator.MoveNext());
		}
		else
		{
			do
			{
				if (JsonConverter.ShouldFlush(ref state, writer))
				{
					state.Current.CollectionEnumerator = enumerator;
					return false;
				}
				if ((int)state.Current.PropertyState < 2)
				{
					state.Current.PropertyState = StackFramePropertyState.Name;
					TKey key2 = enumerator.Current.Key;
					_keyConverter.WriteAsPropertyNameCore(writer, key2, options, state.Current.IsWritingExtensionDataProperty);
				}
				TValue value2 = enumerator.Current.Value;
				if (!_valueConverter.TryWrite(writer, in value2, options, ref state))
				{
					state.Current.CollectionEnumerator = enumerator;
					return false;
				}
				state.Current.EndDictionaryEntry();
			}
			while (enumerator.MoveNext());
		}
		enumerator.Dispose();
		return true;
	}
}
