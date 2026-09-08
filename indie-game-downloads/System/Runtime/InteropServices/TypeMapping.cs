using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Runtime.InteropServices;

public static class TypeMapping
{
	[RequiresUnreferencedCode("Interop types may be removed by trimming")]
	public static IReadOnlyDictionary<string, Type> GetOrCreateExternalTypeMapping<TTypeMapGroup>()
	{
		return TypeMapLazyDictionary.CreateExternalTypeMap((RuntimeType)typeof(TTypeMapGroup));
	}

	[RequiresUnreferencedCode("Interop types may be removed by trimming")]
	public static IReadOnlyDictionary<Type, Type> GetOrCreateProxyTypeMapping<TTypeMapGroup>()
	{
		return TypeMapLazyDictionary.CreateProxyTypeMap((RuntimeType)typeof(TTypeMapGroup));
	}
}
