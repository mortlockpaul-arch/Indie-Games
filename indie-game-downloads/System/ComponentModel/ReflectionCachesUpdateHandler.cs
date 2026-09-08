using System.Reflection;

namespace System.ComponentModel;

internal static class ReflectionCachesUpdateHandler
{
	public static void ClearCache(Type[] types)
	{
		ReflectTypeDescriptionProvider.ClearReflectionCaches();
		if (types != null)
		{
			for (int i = 0; i < types.Length; i++)
			{
				TypeDescriptor.Refresh(types[i]);
			}
			return;
		}
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		for (int i = 0; i < assemblies.Length; i++)
		{
			TypeDescriptor.Refresh(assemblies[i]);
		}
	}
}
