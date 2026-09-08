using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Harmony;

internal static class AccessTools
{
	public static BindingFlags all = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty;

	public static Type TypeByName(string name)
	{
		Type type = Type.GetType(name, throwOnError: false);
		if (type == null)
		{
			type = AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly x) => x.GetTypes()).FirstOrDefault((Type x) => x.FullName == name);
		}
		if (type == null)
		{
			type = AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly x) => x.GetTypes()).FirstOrDefault((Type x) => x.Name == name);
		}
		return type;
	}

	public static T FindIncludingBaseTypes<T>(Type type, Func<Type, T> action)
	{
		while (true)
		{
			T val = action(type);
			if (val != null)
			{
				return val;
			}
			if (type == typeof(object))
			{
				break;
			}
			type = type.BaseType;
		}
		return default(T);
	}

	public static T FindIncludingInnerTypes<T>(Type type, Func<Type, T> action)
	{
		T val = action(type);
		if (val != null)
		{
			return val;
		}
		Type[] nestedTypes = type.GetNestedTypes(all);
		for (int i = 0; i < nestedTypes.Length; i++)
		{
			val = FindIncludingInnerTypes(nestedTypes[i], action);
			if (val != null)
			{
				break;
			}
		}
		return val;
	}

	public static FieldInfo Field(Type type, string name)
	{
		if (type == null || name == null)
		{
			return null;
		}
		return FindIncludingBaseTypes(type, (Type t) => t.GetField(name, all));
	}

	public static PropertyInfo Property(Type type, string name)
	{
		if (type == null || name == null)
		{
			return null;
		}
		return FindIncludingBaseTypes(type, (Type t) => t.GetProperty(name, all));
	}

	public static MethodInfo Method(Type type, string name, Type[] parameters = null, Type[] generics = null)
	{
		if (type == null || name == null)
		{
			return null;
		}
		ParameterModifier[] modifiers = new ParameterModifier[0];
		MethodInfo methodInfo;
		if (parameters == null)
		{
			try
			{
				methodInfo = FindIncludingBaseTypes(type, (Type t) => t.GetMethod(name, all));
			}
			catch (AmbiguousMatchException)
			{
				methodInfo = FindIncludingBaseTypes(type, (Type t) => t.GetMethod(name, all, null, new Type[0], modifiers));
			}
		}
		else
		{
			methodInfo = FindIncludingBaseTypes(type, (Type t) => t.GetMethod(name, all, null, parameters, modifiers));
		}
		if (methodInfo == null)
		{
			return null;
		}
		if (generics != null)
		{
			methodInfo = methodInfo.MakeGenericMethod(generics);
		}
		return methodInfo;
	}

	public static List<string> GetMethodNames(Type type)
	{
		if (type == null)
		{
			return new List<string>();
		}
		return (from m in type.GetMethods(all)
			select m.Name).ToList();
	}

	public static List<string> GetMethodNames(object instance)
	{
		if (instance == null)
		{
			return new List<string>();
		}
		return GetMethodNames(instance.GetType());
	}

	public static ConstructorInfo Constructor(Type type, Type[] parameters = null)
	{
		if (type == null)
		{
			return null;
		}
		if (parameters == null)
		{
			parameters = new Type[0];
		}
		return FindIncludingBaseTypes(type, (Type t) => t.GetConstructor(all, null, parameters, new ParameterModifier[0]));
	}

	public static List<ConstructorInfo> GetDeclaredConstructors(Type type)
	{
		return (from method in type.GetConstructors(all)
			where method.DeclaringType == type
			select method).ToList();
	}

	public static List<MethodInfo> GetDeclaredMethods(Type type)
	{
		return (from method in type.GetMethods(all)
			where method.DeclaringType == type
			select method).ToList();
	}

	public static List<PropertyInfo> GetDeclaredProperties(Type type)
	{
		return (from property in type.GetProperties(all)
			where property.DeclaringType == type
			select property).ToList();
	}

	public static List<FieldInfo> GetDeclaredFields(Type type)
	{
		return (from field in type.GetFields(all)
			where field.DeclaringType == type
			select field).ToList();
	}

	public static Type GetReturnedType(MethodBase method)
	{
		if (method is ConstructorInfo)
		{
			return typeof(void);
		}
		return ((MethodInfo)method).ReturnType;
	}

	public static Type Inner(Type type, string name)
	{
		if (type == null || name == null)
		{
			return null;
		}
		return FindIncludingBaseTypes(type, (Type t) => t.GetNestedType(name, all));
	}

	public static Type FirstInner(Type type, Func<Type, bool> predicate)
	{
		if (type == null || predicate == null)
		{
			return null;
		}
		return type.GetNestedTypes(all).FirstOrDefault((Type subType) => predicate(subType));
	}

	public static MethodInfo FirstMethod(Type type, Func<MethodInfo, bool> predicate)
	{
		if (type == null || predicate == null)
		{
			return null;
		}
		return type.GetMethods(all).FirstOrDefault((MethodInfo method) => predicate(method));
	}

	public static ConstructorInfo FirstConstructor(Type type, Func<ConstructorInfo, bool> predicate)
	{
		if (type == null || predicate == null)
		{
			return null;
		}
		return type.GetConstructors(all).FirstOrDefault((ConstructorInfo constructor) => predicate(constructor));
	}

	public static PropertyInfo FirstProperty(Type type, Func<PropertyInfo, bool> predicate)
	{
		if (type == null || predicate == null)
		{
			return null;
		}
		return type.GetProperties(all).FirstOrDefault((PropertyInfo property) => predicate(property));
	}

	public static Type[] GetTypes(object[] parameters)
	{
		if (parameters == null)
		{
			return new Type[0];
		}
		return parameters.Select((object p) => (p != null) ? p.GetType() : typeof(object)).ToArray();
	}

	public static List<string> GetFieldNames(Type type)
	{
		if (type == null)
		{
			return new List<string>();
		}
		return (from f in type.GetFields(all)
			select f.Name).ToList();
	}

	public static List<string> GetFieldNames(object instance)
	{
		if (instance == null)
		{
			return new List<string>();
		}
		return GetFieldNames(instance.GetType());
	}

	public static List<string> GetPropertyNames(Type type)
	{
		if (type == null)
		{
			return new List<string>();
		}
		return (from f in type.GetProperties(all)
			select f.Name).ToList();
	}

	public static List<string> GetPropertyNames(object instance)
	{
		if (instance == null)
		{
			return new List<string>();
		}
		return GetPropertyNames(instance.GetType());
	}

	public static void ThrowMissingMemberException(Type type, params string[] names)
	{
		string text = string.Join(",", GetFieldNames(type).ToArray());
		string text2 = string.Join(",", GetPropertyNames(type).ToArray());
		throw new MissingMemberException(string.Join(",", names) + "; available fields: " + text + "; available properties: " + text2);
	}

	public static object GetDefaultValue(Type type)
	{
		if (type == null)
		{
			return null;
		}
		if (type == typeof(void))
		{
			return null;
		}
		if (type.IsValueType)
		{
			return Activator.CreateInstance(type);
		}
		return null;
	}

	public static bool IsStruct(Type type)
	{
		if (type.IsValueType && !IsValue(type))
		{
			return !IsVoid(type);
		}
		return false;
	}

	public static bool IsClass(Type type)
	{
		return !type.IsValueType;
	}

	public static bool IsValue(Type type)
	{
		if (!type.IsPrimitive)
		{
			return type.IsEnum;
		}
		return true;
	}

	public static bool IsVoid(Type type)
	{
		return type == typeof(void);
	}
}
