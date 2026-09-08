using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Reflection.Metadata;

internal static class RuntimeTypeMetadataUpdateHandler
{
	internal static bool FilterDeletedMembers { get; private set; }

	internal static bool IsMetadataUpdateDeleted(RuntimeModule module, int memberToken)
	{
		return CustomAttribute.IsCustomAttributeDefined(module, memberToken, (RuntimeType)typeof(MetadataUpdateDeletedAttribute));
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "Clearing the caches on a Type isn't affected if a Type is trimmed, or has any of its members trimmed.")]
	public static void ClearCache(Type[] types)
	{
		FilterDeletedMembers = true;
		if (RequiresClearingAllTypes(types))
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				if (SkipAssembly(assembly))
				{
					continue;
				}
				try
				{
					Type[] types2 = assembly.GetTypes();
					for (int j = 0; j < types2.Length; j++)
					{
						ClearCache(types2[j]);
					}
				}
				catch (ReflectionTypeLoadException)
				{
				}
			}
		}
		else
		{
			Type[] types2 = types;
			for (int i = 0; i < types2.Length; i++)
			{
				ClearCache(types2[i]);
			}
		}
	}

	private static bool SkipAssembly(Assembly assembly)
	{
		return typeof(object).Assembly == assembly;
	}

	private static void ClearCache(Type type)
	{
		(type as RuntimeType)?.ClearCache();
	}

	private static bool RequiresClearingAllTypes([NotNullWhen(false)] Type[] types)
	{
		if (types == null)
		{
			return true;
		}
		for (int i = 0; i < types.Length; i++)
		{
			if (!types[i].IsSealed)
			{
				return true;
			}
		}
		return false;
	}
}
