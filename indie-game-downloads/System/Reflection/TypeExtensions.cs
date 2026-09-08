using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace System.Reflection;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class TypeExtensions
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static ConstructorInfo? GetConstructor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] this Type type, Type[] types)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetConstructor(types);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static ConstructorInfo[] GetConstructors([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] this Type type)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetConstructors();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static ConstructorInfo[] GetConstructors([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] this Type type, BindingFlags bindingAttr)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetConstructors(bindingAttr);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static MemberInfo[] GetDefaultMembers([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicEvents)] this Type type)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetDefaultMembers();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static EventInfo? GetEvent([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents)] this Type type, string name)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetEvent(name);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static EventInfo? GetEvent([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)] this Type type, string name, BindingFlags bindingAttr)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetEvent(name, bindingAttr);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static EventInfo[] GetEvents([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents)] this Type type)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetEvents();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static EventInfo[] GetEvents([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)] this Type type, BindingFlags bindingAttr)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetEvents(bindingAttr);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static FieldInfo? GetField([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] this Type type, string name)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetField(name);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static FieldInfo? GetField([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)] this Type type, string name, BindingFlags bindingAttr)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetField(name, bindingAttr);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static FieldInfo[] GetFields([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] this Type type)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetFields();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static FieldInfo[] GetFields([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)] this Type type, BindingFlags bindingAttr)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetFields(bindingAttr);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static Type[] GetGenericArguments(this Type type)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetGenericArguments();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static Type[] GetInterfaces([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] this Type type)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetInterfaces();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static MemberInfo[] GetMember([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicEvents)] this Type type, string name)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetMember(name);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static MemberInfo[] GetMember([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields | DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.NonPublicNestedTypes | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties | DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)] this Type type, string name, BindingFlags bindingAttr)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetMember(name, bindingAttr);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static MemberInfo[] GetMembers([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicEvents)] this Type type)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetMembers();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static MemberInfo[] GetMembers([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields | DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.NonPublicNestedTypes | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties | DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)] this Type type, BindingFlags bindingAttr)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetMembers(bindingAttr);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static MethodInfo? GetMethod([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] this Type type, string name)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetMethod(name);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static MethodInfo? GetMethod([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)] this Type type, string name, BindingFlags bindingAttr)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetMethod(name, bindingAttr);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static MethodInfo? GetMethod([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] this Type type, string name, Type[] types)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetMethod(name, types);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static MethodInfo[] GetMethods([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] this Type type)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetMethods();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static MethodInfo[] GetMethods([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)] this Type type, BindingFlags bindingAttr)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetMethods(bindingAttr);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static Type? GetNestedType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.NonPublicNestedTypes)] this Type type, string name, BindingFlags bindingAttr)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetNestedType(name, bindingAttr);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static Type[] GetNestedTypes([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.NonPublicNestedTypes)] this Type type, BindingFlags bindingAttr)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetNestedTypes(bindingAttr);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static PropertyInfo[] GetProperties([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] this Type type)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetProperties();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static PropertyInfo[] GetProperties([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)] this Type type, BindingFlags bindingAttr)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetProperties(bindingAttr);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static PropertyInfo? GetProperty([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] this Type type, string name)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetProperty(name);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static PropertyInfo? GetProperty([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)] this Type type, string name, BindingFlags bindingAttr)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetProperty(name, bindingAttr);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static PropertyInfo? GetProperty([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] this Type type, string name, Type? returnType)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetProperty(name, returnType);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static PropertyInfo? GetProperty([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] this Type type, string name, Type? returnType, Type[] types)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.GetProperty(name, returnType, types);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static bool IsAssignableFrom(this Type type, [NotNullWhen(true)] Type? c)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.IsAssignableFrom(c);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static bool IsInstanceOfType(this Type type, [NotNullWhen(true)] object? o)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return type.IsInstanceOfType(o);
	}
}
