using System;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Internal;

internal static class VersionResilientHashCode
{
	[DllImport("QCall", EntryPoint = "VersionResilientHashCode_TypeHashCode", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "VersionResilientHashCode_TypeHashCode")]
	private static extern int TypeHashCode(QCallTypeHandle typeHandle);

	public static int TypeHashCode(RuntimeType type)
	{
		return TypeHashCode(new QCallTypeHandle(ref type));
	}

	public static int TypeHashCode(TypeName type)
	{
		if (type.IsSimple || type.IsConstructedGenericType)
		{
			int num = NameHashCode(type.IsNested ? string.Empty : type.Namespace, type.Name);
			if (type.IsNested)
			{
				num = NestedTypeHashCode(TypeHashCode(type.DeclaringType), num);
			}
			if (type.IsConstructedGenericType)
			{
				return GenericInstanceHashCode(num, type.GetGenericArguments());
			}
			return num;
		}
		if (type.IsArray)
		{
			return ArrayTypeHashCode(TypeHashCode(type.GetElementType()), type.GetArrayRank());
		}
		if (type.IsPointer)
		{
			return PointerTypeHashCode(TypeHashCode(type.GetElementType()));
		}
		if (type.IsByRef)
		{
			return ByrefTypeHashCode(TypeHashCode(type.GetElementType()));
		}
		throw new NotImplementedException();
	}

	private static int GenericInstanceHashCode(int hashcode, ReadOnlySpan<TypeName> instantiation)
	{
		for (int i = 0; i < instantiation.Length; i++)
		{
			int num = TypeHashCode(instantiation[i]);
			hashcode = (hashcode + RotateLeft(hashcode, 13)) ^ num;
		}
		return hashcode + RotateLeft(hashcode, 15);
	}

	public static int NameHashCode(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return 0;
		}
		int num = 1839446340;
		int num2 = 0;
		byte[] bytes = Encoding.UTF8.GetBytes(name);
		for (int i = 0; i < bytes.Length; i += 2)
		{
			num = (num + RotateLeft(num, 5)) ^ (sbyte)bytes[i];
			if (i + 1 >= bytes.Length)
			{
				break;
			}
			num2 = (num2 + RotateLeft(num2, 5)) ^ (sbyte)bytes[i + 1];
		}
		num += RotateLeft(num, 8);
		num2 += RotateLeft(num2, 8);
		return num ^ num2;
	}

	public static int NameHashCode(string namespacePart, string namePart)
	{
		return NameHashCode(namespacePart) ^ NameHashCode(namePart);
	}

	private static int NestedTypeHashCode(int enclosingTypeHashcode, int nestedTypeNameHash)
	{
		return (enclosingTypeHashcode + RotateLeft(enclosingTypeHashcode, 11)) ^ nestedTypeNameHash;
	}

	private static int ArrayTypeHashCode(int elementTypeHashcode, int rank)
	{
		int num = -718195370 + rank;
		_ = 1;
		int num2 = (num + RotateLeft(num, 13)) ^ elementTypeHashcode;
		return num2 + RotateLeft(num2, 15);
	}

	private static int PointerTypeHashCode(int pointeeTypeHashcode)
	{
		return (pointeeTypeHashcode + RotateLeft(pointeeTypeHashcode, 5)) ^ 0x12D0;
	}

	private static int ByrefTypeHashCode(int parameterTypeHashcode)
	{
		return (parameterTypeHashcode + RotateLeft(parameterTypeHashcode, 7)) ^ 0x4C85;
	}

	private static int RotateLeft(int value, int bitCount)
	{
		return (int)BitOperations.RotateLeft((uint)value, bitCount);
	}
}
