using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace System.Reflection;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class AssemblyExtensions
{
	[RequiresUnreferencedCode("Types might be removed")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static Type[] GetExportedTypes(this Assembly assembly)
	{
		ArgumentNullException.ThrowIfNull(assembly, "assembly");
		return assembly.GetExportedTypes();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static Module[] GetModules(this Assembly assembly)
	{
		ArgumentNullException.ThrowIfNull(assembly, "assembly");
		return assembly.GetModules();
	}

	[RequiresUnreferencedCode("Types might be removed")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static Type[] GetTypes(this Assembly assembly)
	{
		ArgumentNullException.ThrowIfNull(assembly, "assembly");
		return assembly.GetTypes();
	}
}
