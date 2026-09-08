using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal static class TypeUtils
{
	internal static Type GetNonNullableType(Type type)
	{
		if (type.IsNullableType())
		{
			return type.GetGenericArguments()[0];
		}
		return type;
	}

	internal static bool IsNullableType(this Type type)
	{
		if (type.IsGenericType)
		{
			return type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}
		return false;
	}

	internal static bool AreReferenceAssignable(Type dest, Type src)
	{
		if (dest == src)
		{
			return true;
		}
		if (!dest.IsValueType && !src.IsValueType && AreAssignable(dest, src))
		{
			return true;
		}
		return false;
	}

	internal static bool AreAssignable(Type dest, Type src)
	{
		if (dest == src)
		{
			return true;
		}
		if (dest.IsAssignableFrom(src))
		{
			return true;
		}
		if (dest.IsArray && src.IsArray && dest.GetArrayRank() == src.GetArrayRank() && AreReferenceAssignable(dest.GetElementType(), src.GetElementType()))
		{
			return true;
		}
		if (src.IsArray && dest.IsGenericType && (dest.GetGenericTypeDefinition() == typeof(IEnumerable<>) || dest.GetGenericTypeDefinition() == typeof(IList<>) || dest.GetGenericTypeDefinition() == typeof(ICollection<>)) && dest.GetGenericArguments()[0] == src.GetElementType())
		{
			return true;
		}
		return false;
	}

	internal static bool IsImplicitlyConvertible(Type source, Type destination)
	{
		if (!IsIdentityConversion(source, destination) && !IsImplicitNumericConversion(source, destination) && !IsImplicitReferenceConversion(source, destination))
		{
			return IsImplicitBoxingConversion(source, destination);
		}
		return true;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal static bool IsImplicitlyConvertible(Type source, Type destination, bool considerUserDefined)
	{
		if (!IsImplicitlyConvertible(source, destination))
		{
			if (considerUserDefined)
			{
				return GetUserDefinedCoercionMethod(source, destination, implicitOnly: true) != null;
			}
			return false;
		}
		return true;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal static MethodInfo GetUserDefinedCoercionMethod(Type convertFrom, Type convertToType, bool implicitOnly)
	{
		Type nonNullableType = GetNonNullableType(convertFrom);
		Type nonNullableType2 = GetNonNullableType(convertToType);
		MethodInfo[] methods = nonNullableType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		MethodInfo methodInfo = FindConversionOperator(methods, convertFrom, convertToType, implicitOnly);
		if (methodInfo != null)
		{
			return methodInfo;
		}
		MethodInfo[] methods2 = nonNullableType2.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		methodInfo = FindConversionOperator(methods2, convertFrom, convertToType, implicitOnly);
		if (methodInfo != null)
		{
			return methodInfo;
		}
		if (nonNullableType != convertFrom || nonNullableType2 != convertToType)
		{
			methodInfo = FindConversionOperator(methods, nonNullableType, nonNullableType2, implicitOnly) ?? FindConversionOperator(methods2, nonNullableType, nonNullableType2, implicitOnly);
			if (methodInfo != null)
			{
				return methodInfo;
			}
		}
		return null;
	}

	internal static MethodInfo FindConversionOperator(MethodInfo[] methods, Type typeFrom, Type typeTo, bool implicitOnly)
	{
		foreach (MethodInfo methodInfo in methods)
		{
			if ((!(methodInfo.Name != "op_Implicit") || (!implicitOnly && !(methodInfo.Name != "op_Explicit"))) && !(methodInfo.ReturnType != typeTo) && !(methodInfo.GetParameters()[0].ParameterType != typeFrom))
			{
				return methodInfo;
			}
		}
		return null;
	}

	private static bool IsIdentityConversion(Type source, Type destination)
	{
		return source == destination;
	}

	private static bool IsImplicitNumericConversion(Type source, Type destination)
	{
		TypeCode typeCode = Type.GetTypeCode(source);
		TypeCode typeCode2 = Type.GetTypeCode(destination);
		switch (typeCode)
		{
		case TypeCode.SByte:
			switch (typeCode2)
			{
			case TypeCode.Int16:
			case TypeCode.Int32:
			case TypeCode.Int64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				return true;
			default:
				return false;
			}
		case TypeCode.Byte:
			if ((uint)(typeCode2 - 7) <= 8u)
			{
				return true;
			}
			return false;
		case TypeCode.Int16:
			switch (typeCode2)
			{
			case TypeCode.Int32:
			case TypeCode.Int64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				return true;
			default:
				return false;
			}
		case TypeCode.UInt16:
			if ((uint)(typeCode2 - 9) <= 6u)
			{
				return true;
			}
			return false;
		case TypeCode.Int32:
			if (typeCode2 == TypeCode.Int64 || (uint)(typeCode2 - 13) <= 2u)
			{
				return true;
			}
			return false;
		case TypeCode.UInt32:
			if (typeCode2 == TypeCode.UInt32 || (uint)(typeCode2 - 12) <= 3u)
			{
				return true;
			}
			return false;
		case TypeCode.Int64:
		case TypeCode.UInt64:
			if ((uint)(typeCode2 - 13) <= 2u)
			{
				return true;
			}
			return false;
		case TypeCode.Char:
			if ((uint)(typeCode2 - 8) <= 7u)
			{
				return true;
			}
			return false;
		case TypeCode.Single:
			return typeCode2 == TypeCode.Double;
		default:
			return false;
		}
	}

	private static bool IsImplicitReferenceConversion(Type source, Type destination)
	{
		return AreAssignable(destination, source);
	}

	private static bool IsImplicitBoxingConversion(Type source, Type destination)
	{
		if (source.IsValueType && (destination == typeof(object) || destination == typeof(ValueType)))
		{
			return true;
		}
		if (source.IsEnum && destination == typeof(Enum))
		{
			return true;
		}
		return false;
	}
}
