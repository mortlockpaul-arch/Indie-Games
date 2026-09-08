using System.ComponentModel;

namespace System.Reflection;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class PropertyInfoExtensions
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static MethodInfo[] GetAccessors(this PropertyInfo property)
	{
		ArgumentNullException.ThrowIfNull(property, "property");
		return property.GetAccessors();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static MethodInfo[] GetAccessors(this PropertyInfo property, bool nonPublic)
	{
		ArgumentNullException.ThrowIfNull(property, "property");
		return property.GetAccessors(nonPublic);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static MethodInfo? GetGetMethod(this PropertyInfo property)
	{
		ArgumentNullException.ThrowIfNull(property, "property");
		return property.GetGetMethod();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static MethodInfo? GetGetMethod(this PropertyInfo property, bool nonPublic)
	{
		ArgumentNullException.ThrowIfNull(property, "property");
		return property.GetGetMethod(nonPublic);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static MethodInfo? GetSetMethod(this PropertyInfo property)
	{
		ArgumentNullException.ThrowIfNull(property, "property");
		return property.GetSetMethod();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static MethodInfo? GetSetMethod(this PropertyInfo property, bool nonPublic)
	{
		ArgumentNullException.ThrowIfNull(property, "property");
		return property.GetSetMethod(nonPublic);
	}
}
