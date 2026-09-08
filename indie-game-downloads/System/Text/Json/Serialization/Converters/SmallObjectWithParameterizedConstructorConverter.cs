using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Metadata;

namespace System.Text.Json.Serialization.Converters;

internal sealed class SmallObjectWithParameterizedConstructorConverter<T, TArg0, TArg1, TArg2, TArg3> : ObjectWithParameterizedConstructorConverter<T>
{
	protected override object CreateObject(ref ReadStackFrame frame)
	{
		JsonTypeInfo.ParameterizedConstructorDelegate<T, TArg0, TArg1, TArg2, TArg3> obj = (JsonTypeInfo.ParameterizedConstructorDelegate<T, TArg0, TArg1, TArg2, TArg3>)frame.JsonTypeInfo.CreateObjectWithArgs;
		Arguments<TArg0, TArg1, TArg2, TArg3> arguments = (Arguments<TArg0, TArg1, TArg2, TArg3>)frame.CtorArgumentState.Arguments;
		return obj(arguments.Arg0, arguments.Arg1, arguments.Arg2, arguments.Arg3);
	}

	protected override bool ReadAndCacheConstructorArgument(scoped ref ReadStack state, ref Utf8JsonReader reader, JsonParameterInfo jsonParameterInfo)
	{
		Arguments<TArg0, TArg1, TArg2, TArg3> arguments = (Arguments<TArg0, TArg1, TArg2, TArg3>)state.Current.CtorArgumentState.Arguments;
		return jsonParameterInfo.Position switch
		{
			0 => TryRead<TArg0>(ref state, ref reader, jsonParameterInfo, out arguments.Arg0), 
			1 => TryRead<TArg1>(ref state, ref reader, jsonParameterInfo, out arguments.Arg1), 
			2 => TryRead<TArg2>(ref state, ref reader, jsonParameterInfo, out arguments.Arg2), 
			3 => TryRead<TArg3>(ref state, ref reader, jsonParameterInfo, out arguments.Arg3), 
			_ => throw new InvalidOperationException(), 
		};
	}

	private static bool TryRead<TArg>(scoped ref ReadStack state, ref Utf8JsonReader reader, JsonParameterInfo jsonParameterInfo, out TArg arg)
	{
		JsonParameterInfo<TArg> jsonParameterInfo2 = (JsonParameterInfo<TArg>)jsonParameterInfo;
		bool num = jsonParameterInfo2.EffectiveConverter.TryRead(ref reader, jsonParameterInfo2.ParameterType, jsonParameterInfo2.Options, ref state, out var value, out var _);
		if (num && value == null)
		{
			if (jsonParameterInfo2.IgnoreNullTokensOnRead)
			{
				value = jsonParameterInfo2.EffectiveDefaultValue;
			}
			else if (!jsonParameterInfo2.IsNullable && jsonParameterInfo2.Options.RespectNullableAnnotations)
			{
				ThrowHelper.ThrowJsonException_ConstructorParameterDisallowNull(jsonParameterInfo2.Name, state.Current.JsonTypeInfo.Type);
			}
		}
		arg = value;
		return num;
	}

	protected override void InitializeConstructorArgumentCaches(ref ReadStack state, JsonSerializerOptions options)
	{
		JsonTypeInfo jsonTypeInfo = state.Current.JsonTypeInfo;
		Arguments<TArg0, TArg1, TArg2, TArg3> arguments = new Arguments<TArg0, TArg1, TArg2, TArg3>();
		ReadOnlySpan<JsonParameterInfo> parameterCache = jsonTypeInfo.ParameterCache;
		for (int i = 0; i < parameterCache.Length; i++)
		{
			JsonParameterInfo jsonParameterInfo = parameterCache[i];
			switch (jsonParameterInfo.Position)
			{
			case 0:
				arguments.Arg0 = ((JsonParameterInfo<TArg0>)jsonParameterInfo).EffectiveDefaultValue;
				break;
			case 1:
				arguments.Arg1 = ((JsonParameterInfo<TArg1>)jsonParameterInfo).EffectiveDefaultValue;
				break;
			case 2:
				arguments.Arg2 = ((JsonParameterInfo<TArg2>)jsonParameterInfo).EffectiveDefaultValue;
				break;
			case 3:
				arguments.Arg3 = ((JsonParameterInfo<TArg3>)jsonParameterInfo).EffectiveDefaultValue;
				break;
			}
		}
		state.Current.CtorArgumentState.Arguments = arguments;
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	internal override void ConfigureJsonTypeInfoUsingReflection(JsonTypeInfo jsonTypeInfo, JsonSerializerOptions options)
	{
		jsonTypeInfo.CreateObjectWithArgs = DefaultJsonTypeInfoResolver.MemberAccessor.CreateParameterizedConstructor<T, TArg0, TArg1, TArg2, TArg3>(base.ConstructorInfo);
	}
}
