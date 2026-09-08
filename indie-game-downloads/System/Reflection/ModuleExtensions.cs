using System.ComponentModel;

namespace System.Reflection;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class ModuleExtensions
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static bool HasModuleVersionId(this Module module)
	{
		ArgumentNullException.ThrowIfNull(module, "module");
		return true;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static Guid GetModuleVersionId(this Module module)
	{
		ArgumentNullException.ThrowIfNull(module, "module");
		return module.ModuleVersionId;
	}
}
