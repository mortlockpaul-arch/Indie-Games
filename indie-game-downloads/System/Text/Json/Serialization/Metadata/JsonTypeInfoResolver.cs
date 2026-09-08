namespace System.Text.Json.Serialization.Metadata;

public static class JsonTypeInfoResolver
{
	internal static IJsonTypeInfoResolver Empty { get; } = new EmptyJsonTypeInfoResolver();

	public static IJsonTypeInfoResolver Combine(params IJsonTypeInfoResolver?[] resolvers)
	{
		ArgumentNullException.ThrowIfNull(resolvers, "resolvers");
		return Combine((ReadOnlySpan<IJsonTypeInfoResolver?>)resolvers);
	}

	public static IJsonTypeInfoResolver Combine(params ReadOnlySpan<IJsonTypeInfoResolver?> resolvers)
	{
		JsonTypeInfoResolverChain jsonTypeInfoResolverChain = new JsonTypeInfoResolverChain();
		ReadOnlySpan<IJsonTypeInfoResolver> readOnlySpan = resolvers;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			IJsonTypeInfoResolver resolver = readOnlySpan[i];
			jsonTypeInfoResolverChain.AddFlattened(resolver);
		}
		if (jsonTypeInfoResolverChain.Count != 1)
		{
			return jsonTypeInfoResolverChain;
		}
		return jsonTypeInfoResolverChain[0];
	}

	public static IJsonTypeInfoResolver WithAddedModifier(this IJsonTypeInfoResolver resolver, Action<JsonTypeInfo> modifier)
	{
		ArgumentNullException.ThrowIfNull(resolver, "resolver");
		ArgumentNullException.ThrowIfNull(modifier, "modifier");
		if (!(resolver is JsonTypeInfoResolverWithAddedModifiers jsonTypeInfoResolverWithAddedModifiers))
		{
			return new JsonTypeInfoResolverWithAddedModifiers(resolver, new Action<JsonTypeInfo>[1] { modifier });
		}
		return jsonTypeInfoResolverWithAddedModifiers.WithAddedModifier(modifier);
	}

	internal static bool IsCompatibleWithOptions(this IJsonTypeInfoResolver resolver, JsonSerializerOptions options)
	{
		if (resolver is IBuiltInJsonTypeInfoResolver builtInJsonTypeInfoResolver)
		{
			return builtInJsonTypeInfoResolver.IsCompatibleWithOptions(options);
		}
		return false;
	}
}
