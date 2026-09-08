using System.ComponentModel;

namespace System.Reflection;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class MethodInfoExtensions
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static MethodInfo GetBaseDefinition(this MethodInfo method)
	{
		ArgumentNullException.ThrowIfNull(method, "method");
		return method.GetBaseDefinition();
	}
}
