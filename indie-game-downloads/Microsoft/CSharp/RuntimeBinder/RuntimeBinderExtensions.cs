using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.CSharp.RuntimeBinder;

internal static class RuntimeBinderExtensions
{
	public static bool IsNullableType(this Type type)
	{
		if (type.IsConstructedGenericType)
		{
			return type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}
		return false;
	}

	public static bool IsEquivalentTo(this MemberInfo mi1, MemberInfo mi2)
	{
		if (mi1 == null || mi2 == null)
		{
			if (mi1 == null)
			{
				return mi2 == null;
			}
			return false;
		}
		if (mi1.Equals(mi2))
		{
			return true;
		}
		MethodInfo methodInfo = mi1 as MethodInfo;
		if ((object)methodInfo != null)
		{
			MethodInfo methodInfo2 = mi2 as MethodInfo;
			if ((object)methodInfo2 == null || methodInfo.IsGenericMethod != methodInfo2.IsGenericMethod)
			{
				return false;
			}
			if (methodInfo.IsGenericMethod)
			{
				methodInfo = methodInfo.GetGenericMethodDefinition();
				methodInfo2 = methodInfo2.GetGenericMethodDefinition();
				if (methodInfo.GetGenericArguments().Length != methodInfo2.GetGenericArguments().Length)
				{
					return false;
				}
			}
			if (methodInfo != methodInfo2 && methodInfo.CallingConvention == methodInfo2.CallingConvention && methodInfo.Name == methodInfo2.Name && methodInfo.DeclaringType.IsGenericallyEqual(methodInfo2.DeclaringType) && methodInfo.ReturnType.IsGenericallyEquivalentTo(methodInfo2.ReturnType, methodInfo, methodInfo2))
			{
				return methodInfo.AreParametersEquivalent(methodInfo2);
			}
			return false;
		}
		if (mi1 is ConstructorInfo constructorInfo)
		{
			if (mi2 is ConstructorInfo constructorInfo2 && constructorInfo != constructorInfo2 && constructorInfo.CallingConvention == constructorInfo2.CallingConvention && constructorInfo.DeclaringType.IsGenericallyEqual(constructorInfo2.DeclaringType))
			{
				return constructorInfo.AreParametersEquivalent(constructorInfo2);
			}
			return false;
		}
		if (mi1 is PropertyInfo propertyInfo && mi2 is PropertyInfo propertyInfo2 && propertyInfo != propertyInfo2 && propertyInfo.Name == propertyInfo2.Name && propertyInfo.DeclaringType.IsGenericallyEqual(propertyInfo2.DeclaringType) && propertyInfo.PropertyType.IsGenericallyEquivalentTo(propertyInfo2.PropertyType, propertyInfo, propertyInfo2) && propertyInfo.GetGetMethod(nonPublic: true).IsEquivalentTo(propertyInfo2.GetGetMethod(nonPublic: true)))
		{
			return propertyInfo.GetSetMethod(nonPublic: true).IsEquivalentTo(propertyInfo2.GetSetMethod(nonPublic: true));
		}
		return false;
	}

	private static bool AreParametersEquivalent(this MethodBase method1, MethodBase method2)
	{
		ParameterInfo[] parameters = method1.GetParameters();
		ParameterInfo[] parameters2 = method2.GetParameters();
		if (parameters.Length != parameters2.Length)
		{
			return false;
		}
		for (int i = 0; i < parameters.Length; i++)
		{
			if (!parameters[i].IsEquivalentTo(parameters2[i], method1, method2))
			{
				return false;
			}
		}
		return true;
	}

	private static bool IsEquivalentTo(this ParameterInfo pi1, ParameterInfo pi2, MethodBase method1, MethodBase method2)
	{
		if (pi1 == null || pi2 == null)
		{
			if (pi1 == null)
			{
				return pi2 == null;
			}
			return false;
		}
		if (pi1.Equals(pi2))
		{
			return true;
		}
		return pi1.ParameterType.IsGenericallyEquivalentTo(pi2.ParameterType, method1, method2);
	}

	private static bool IsGenericallyEqual(this Type t1, Type t2)
	{
		if (t1 == null || t2 == null)
		{
			if (t1 == null)
			{
				return t2 == null;
			}
			return false;
		}
		if (t1.Equals(t2))
		{
			return true;
		}
		if (t1.IsConstructedGenericType || t2.IsConstructedGenericType)
		{
			Type obj = (t1.IsConstructedGenericType ? t1.GetGenericTypeDefinition() : t1);
			Type o = (t2.IsConstructedGenericType ? t2.GetGenericTypeDefinition() : t2);
			return obj.Equals(o);
		}
		return false;
	}

	private static bool IsGenericallyEquivalentTo(this Type t1, Type t2, MemberInfo member1, MemberInfo member2)
	{
		if (t1.Equals(t2))
		{
			return true;
		}
		if (t1.IsGenericParameter)
		{
			if (t2.IsGenericParameter)
			{
				if (t1.DeclaringMethod == null && member1.DeclaringType.Equals(t1.DeclaringType))
				{
					if (!(t2.DeclaringMethod == null) || !member2.DeclaringType.Equals(t2.DeclaringType))
					{
						return t1.IsTypeParameterEquivalentToTypeInst(t2, member2);
					}
				}
				else if (t2.DeclaringMethod == null && member2.DeclaringType.Equals(t2.DeclaringType))
				{
					return t2.IsTypeParameterEquivalentToTypeInst(t1, member1);
				}
				return false;
			}
			return t1.IsTypeParameterEquivalentToTypeInst(t2, member2);
		}
		if (t2.IsGenericParameter)
		{
			return t2.IsTypeParameterEquivalentToTypeInst(t1, member1);
		}
		if (t1.IsGenericType && t2.IsGenericType)
		{
			Type[] genericArguments = t1.GetGenericArguments();
			Type[] genericArguments2 = t2.GetGenericArguments();
			if (genericArguments.Length == genericArguments2.Length)
			{
				if (!t1.IsGenericallyEqual(t2))
				{
					return false;
				}
				for (int i = 0; i < genericArguments.Length; i++)
				{
					if (!genericArguments[i].IsGenericallyEquivalentTo(genericArguments2[i], member1, member2))
					{
						return false;
					}
				}
				return true;
			}
		}
		if (t1.IsArray && t2.IsArray)
		{
			if (t1.GetArrayRank() == t2.GetArrayRank())
			{
				return t1.GetElementType().IsGenericallyEquivalentTo(t2.GetElementType(), member1, member2);
			}
			return false;
		}
		if ((t1.IsByRef && t2.IsByRef) || (t1.IsPointer && t2.IsPointer))
		{
			return t1.GetElementType().IsGenericallyEquivalentTo(t2.GetElementType(), member1, member2);
		}
		return false;
	}

	private static bool IsTypeParameterEquivalentToTypeInst(this Type typeParam, Type typeInst, MemberInfo member)
	{
		if (typeParam.DeclaringMethod != null)
		{
			if (!(member is MethodBase))
			{
				return false;
			}
			MethodBase methodBase = (MethodBase)member;
			int genericParameterPosition = typeParam.GenericParameterPosition;
			Type[] array = (methodBase.IsGenericMethod ? methodBase.GetGenericArguments() : null);
			if (array != null && array.Length > genericParameterPosition)
			{
				return array[genericParameterPosition].Equals(typeInst);
			}
			return false;
		}
		return member.DeclaringType.GetGenericArguments()[typeParam.GenericParameterPosition].Equals(typeInst);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	public static string GetIndexerName(this Type type)
	{
		string typeIndexerName = GetTypeIndexerName(type);
		if (typeIndexerName == null && type.IsInterface)
		{
			Type[] interfaces = type.GetInterfaces();
			for (int i = 0; i < interfaces.Length; i++)
			{
				typeIndexerName = GetTypeIndexerName(interfaces[i]);
				if (typeIndexerName != null)
				{
					break;
				}
			}
		}
		return typeIndexerName;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private static string GetTypeIndexerName(Type type)
	{
		string text = type.GetCustomAttribute<DefaultMemberAttribute>()?.MemberName;
		if (text != null)
		{
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo.Name == text && propertyInfo.GetIndexParameters().Length != 0)
				{
					return text;
				}
			}
		}
		return null;
	}
}
