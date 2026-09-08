using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace System.ComponentModel;

[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2070:UnrecognizedReflectionPattern", Justification = "Class is only called with types are registered or protected with [DynamicallyAccessedMemberTypes].")]
internal static class TrimSafeReflectionHelper
{
	public static PropertyInfo[] GetProperties(Type type, BindingFlags bindingAttr)
	{
		return type.GetProperties(bindingAttr);
	}

	public static PropertyInfo GetProperty(Type type, string name, BindingFlags bindingAttr)
	{
		return type.GetProperty(name, bindingAttr);
	}

	public static EventInfo[] GetEvents(Type type, BindingFlags bindingAttr)
	{
		return type.GetEvents(bindingAttr);
	}

	public static Type[] GetInterfaces(Type type)
	{
		return type.GetInterfaces();
	}
}
