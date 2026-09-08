using System.Buffers;
using System.Text.Json.Serialization.Metadata;

namespace System.Text.Json.Serialization.Converters;

internal class LargeObjectWithParameterizedConstructorConverter<T> : ObjectWithParameterizedConstructorConverter<T>
{
	protected sealed override bool ReadAndCacheConstructorArgument(scoped ref ReadStack state, ref Utf8JsonReader reader, JsonParameterInfo jsonParameterInfo)
	{
		bool num = jsonParameterInfo.EffectiveConverter.TryReadAsObject(ref reader, jsonParameterInfo.ParameterType, jsonParameterInfo.Options, ref state, out var value);
		if (num && (value != null || !jsonParameterInfo.IgnoreNullTokensOnRead))
		{
			if (value == null && !jsonParameterInfo.IsNullable && jsonParameterInfo.Options.RespectNullableAnnotations)
			{
				ThrowHelper.ThrowJsonException_ConstructorParameterDisallowNull(jsonParameterInfo.Name, state.Current.JsonTypeInfo.Type);
			}
			((object[])state.Current.CtorArgumentState.Arguments)[jsonParameterInfo.Position] = value;
		}
		return num;
	}

	protected sealed override object CreateObject(ref ReadStackFrame frame)
	{
		object[] array = (object[])frame.CtorArgumentState.Arguments;
		frame.CtorArgumentState.Arguments = null;
		object result = ((Func<object[], T>)frame.JsonTypeInfo.CreateObjectWithArgs)(array);
		ArrayPool<object>.Shared.Return(array, clearArray: true);
		return result;
	}

	protected sealed override void InitializeConstructorArgumentCaches(ref ReadStack state, JsonSerializerOptions options)
	{
		JsonTypeInfo jsonTypeInfo = state.Current.JsonTypeInfo;
		object[] array = ArrayPool<object>.Shared.Rent(jsonTypeInfo.ParameterCache.Length);
		ReadOnlySpan<JsonParameterInfo> parameterCache = jsonTypeInfo.ParameterCache;
		for (int i = 0; i < parameterCache.Length; i++)
		{
			JsonParameterInfo jsonParameterInfo = parameterCache[i];
			array[jsonParameterInfo.Position] = jsonParameterInfo.EffectiveDefaultValue;
		}
		state.Current.CtorArgumentState.Arguments = array;
	}
}
