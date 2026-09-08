using System.Collections;
using System.Text.Json.Serialization.Metadata;

namespace System.Text.Json.Serialization.Converters;

internal class StackOrQueueConverter<TCollection> : JsonCollectionConverter<TCollection, object> where TCollection : IEnumerable
{
	internal override bool CanPopulate => true;

	protected sealed override void Add(in object value, ref ReadStack state)
	{
		((Action<TCollection, object>)state.Current.JsonTypeInfo.AddMethodDelegate)((TCollection)state.Current.ReturnValue, value);
	}

	protected sealed override void CreateCollection(ref Utf8JsonReader reader, scoped ref ReadStack state, JsonSerializerOptions options)
	{
		JsonPropertyInfo parentProperty = state.ParentProperty;
		if (parentProperty == null || !parentProperty.TryGetPrePopulatedValue(ref state))
		{
			Func<object> createObject = state.Current.JsonTypeInfo.CreateObject;
			if (createObject == null)
			{
				ThrowHelper.ThrowNotSupportedException_CannotPopulateCollection(Type, ref reader, ref state);
			}
			state.Current.ReturnValue = createObject();
		}
	}

	protected sealed override bool OnWriteResume(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, ref WriteStack state)
	{
		IEnumerator enumerator;
		if (state.Current.CollectionEnumerator == null)
		{
			enumerator = value.GetEnumerator();
			state.Current.CollectionEnumerator = enumerator;
			if (!enumerator.MoveNext())
			{
				return true;
			}
		}
		else
		{
			enumerator = state.Current.CollectionEnumerator;
		}
		JsonConverter<object> elementConverter = JsonCollectionConverter<TCollection, object>.GetElementConverter(ref state);
		do
		{
			if (JsonConverter.ShouldFlush(ref state, writer))
			{
				return false;
			}
			if (!elementConverter.TryWrite(writer, enumerator.Current, options, ref state))
			{
				return false;
			}
			state.Current.EndCollectionElement();
		}
		while (enumerator.MoveNext());
		return true;
	}
}
