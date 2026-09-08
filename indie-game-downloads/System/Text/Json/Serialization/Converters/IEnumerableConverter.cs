using System.Collections;
using System.Collections.Generic;

namespace System.Text.Json.Serialization.Converters;

internal sealed class IEnumerableConverter<TCollection> : JsonCollectionConverter<TCollection, object> where TCollection : IEnumerable
{
	private readonly bool _isDeserializable = typeof(TCollection).IsAssignableFrom(typeof(List<object>));

	internal override bool SupportsCreateObjectDelegate => false;

	protected override void Add(in object value, ref ReadStack state)
	{
		((List<object>)state.Current.ReturnValue).Add(value);
	}

	protected override void CreateCollection(ref Utf8JsonReader reader, scoped ref ReadStack state, JsonSerializerOptions options)
	{
		if (!_isDeserializable)
		{
			ThrowHelper.ThrowNotSupportedException_CannotPopulateCollection(Type, ref reader, ref state);
		}
		state.Current.ReturnValue = new List<object>();
	}

	protected override bool OnWriteResume(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, ref WriteStack state)
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
