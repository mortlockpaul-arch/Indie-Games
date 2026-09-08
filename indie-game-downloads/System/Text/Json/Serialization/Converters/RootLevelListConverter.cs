using System.Collections.Generic;
using System.Text.Json.Serialization.Metadata;

namespace System.Text.Json.Serialization.Converters;

internal sealed class RootLevelListConverter<T> : JsonResumableConverter<List<T>>
{
	private readonly JsonTypeInfo<T> _elementTypeInfo;

	internal override Type ElementType => typeof(T);

	private protected sealed override ConverterStrategy GetDefaultConverterStrategy()
	{
		return ConverterStrategy.Enumerable;
	}

	public RootLevelListConverter(JsonTypeInfo<T> elementTypeInfo)
	{
		base.IsRootLevelMultiContentStreamingConverter = true;
		_elementTypeInfo = elementTypeInfo;
	}

	internal override bool OnTryRead(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options, scoped ref ReadStack state, out List<T> value)
	{
		JsonConverter<T> effectiveConverter = _elementTypeInfo.EffectiveConverter;
		state.Current.JsonPropertyInfo = _elementTypeInfo.PropertyInfoForTypeInfo;
		List<T> list = (List<T>)state.Current.ReturnValue;
		while (true)
		{
			if ((int)state.Current.PropertyState < 3)
			{
				if (!reader.TryAdvanceToNextRootLevelValueWithOptionalReadAhead(effectiveConverter.RequiresReadAhead, out var isAtEndOfStream))
				{
					if (isAtEndOfStream)
					{
						value = list;
						return true;
					}
					value = null;
					return false;
				}
				state.Current.PropertyState = StackFramePropertyState.ReadValue;
			}
			if (!effectiveConverter.TryRead(ref reader, typeof(T), options, ref state, out var value2, out var _))
			{
				break;
			}
			if (list == null)
			{
				list = (List<T>)(state.Current.ReturnValue = new List<T>());
			}
			list.Add(value2);
			state.Current.EndElement();
		}
		value = null;
		return false;
	}
}
