using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal static class PredefinedTypeFacts
{
	private sealed class PredefinedTypeInfo
	{
		public readonly string Name;

		public readonly FUNDTYPE FundType;

		public readonly Type AssociatedSystemType;

		internal PredefinedTypeInfo(PredefinedType type, Type associatedSystemType, string name, FUNDTYPE fundType)
		{
			Name = name;
			FundType = fundType;
			AssociatedSystemType = associatedSystemType;
		}

		internal PredefinedTypeInfo(PredefinedType type, Type associatedSystemType, string name)
			: this(type, associatedSystemType, name, FUNDTYPE.FT_REF)
		{
		}
	}

	private static readonly PredefinedTypeInfo[] s_types = new PredefinedTypeInfo[49]
	{
		new PredefinedTypeInfo(PredefinedType.PT_BYTE, typeof(byte), "System.Byte", FUNDTYPE.FT_U1),
		new PredefinedTypeInfo(PredefinedType.PT_SHORT, typeof(short), "System.Int16", FUNDTYPE.FT_I2),
		new PredefinedTypeInfo(PredefinedType.PT_INT, typeof(int), "System.Int32", FUNDTYPE.FT_I4),
		new PredefinedTypeInfo(PredefinedType.PT_LONG, typeof(long), "System.Int64", FUNDTYPE.FT_I8),
		new PredefinedTypeInfo(PredefinedType.PT_FLOAT, typeof(float), "System.Single", FUNDTYPE.FT_R4),
		new PredefinedTypeInfo(PredefinedType.PT_DOUBLE, typeof(double), "System.Double", FUNDTYPE.FT_R8),
		new PredefinedTypeInfo(PredefinedType.PT_DECIMAL, typeof(decimal), "System.Decimal", FUNDTYPE.FT_STRUCT),
		new PredefinedTypeInfo(PredefinedType.PT_CHAR, typeof(char), "System.Char", FUNDTYPE.FT_U2),
		new PredefinedTypeInfo(PredefinedType.PT_BOOL, typeof(bool), "System.Boolean", FUNDTYPE.FT_I1),
		new PredefinedTypeInfo(PredefinedType.PT_SBYTE, typeof(sbyte), "System.SByte", FUNDTYPE.FT_I1),
		new PredefinedTypeInfo(PredefinedType.PT_USHORT, typeof(ushort), "System.UInt16", FUNDTYPE.FT_U2),
		new PredefinedTypeInfo(PredefinedType.PT_UINT, typeof(uint), "System.UInt32", FUNDTYPE.FT_U4),
		new PredefinedTypeInfo(PredefinedType.PT_ULONG, typeof(ulong), "System.UInt64", FUNDTYPE.FT_U8),
		new PredefinedTypeInfo(PredefinedType.FirstNonSimpleType, typeof(nint), "System.IntPtr", FUNDTYPE.FT_STRUCT),
		new PredefinedTypeInfo(PredefinedType.PT_UINTPTR, typeof(nuint), "System.UIntPtr", FUNDTYPE.FT_STRUCT),
		new PredefinedTypeInfo(PredefinedType.PT_OBJECT, typeof(object), "System.Object"),
		new PredefinedTypeInfo(PredefinedType.PT_STRING, typeof(string), "System.String"),
		new PredefinedTypeInfo(PredefinedType.PT_DELEGATE, typeof(Delegate), "System.Delegate"),
		new PredefinedTypeInfo(PredefinedType.PT_MULTIDEL, typeof(MulticastDelegate), "System.MulticastDelegate"),
		new PredefinedTypeInfo(PredefinedType.PT_ARRAY, typeof(Array), "System.Array"),
		new PredefinedTypeInfo(PredefinedType.PT_TYPE, typeof(Type), "System.Type"),
		new PredefinedTypeInfo(PredefinedType.PT_VALUE, typeof(ValueType), "System.ValueType"),
		new PredefinedTypeInfo(PredefinedType.PT_ENUM, typeof(Enum), "System.Enum"),
		new PredefinedTypeInfo(PredefinedType.PT_DATETIME, typeof(DateTime), "System.DateTime", FUNDTYPE.FT_STRUCT),
		new PredefinedTypeInfo(PredefinedType.PT_IENUMERABLE, typeof(IEnumerable), "System.Collections.IEnumerable"),
		new PredefinedTypeInfo(PredefinedType.PT_G_IENUMERABLE, typeof(IEnumerable<>), "System.Collections.Generic.IEnumerable`1"),
		new PredefinedTypeInfo(PredefinedType.PT_G_OPTIONAL, typeof(Nullable<>), "System.Nullable`1", FUNDTYPE.FT_STRUCT),
		new PredefinedTypeInfo(PredefinedType.PT_G_IQUERYABLE, typeof(IQueryable<>), "System.Linq.IQueryable`1"),
		new PredefinedTypeInfo(PredefinedType.PT_G_ICOLLECTION, typeof(ICollection<>), "System.Collections.Generic.ICollection`1"),
		new PredefinedTypeInfo(PredefinedType.PT_G_ILIST, typeof(IList<>), "System.Collections.Generic.IList`1"),
		new PredefinedTypeInfo(PredefinedType.PT_G_EXPRESSION, typeof(Expression<>), "System.Linq.Expressions.Expression`1"),
		new PredefinedTypeInfo(PredefinedType.PT_EXPRESSION, typeof(Expression), "System.Linq.Expressions.Expression"),
		new PredefinedTypeInfo(PredefinedType.PT_BINARYEXPRESSION, typeof(BinaryExpression), "System.Linq.Expressions.BinaryExpression"),
		new PredefinedTypeInfo(PredefinedType.PT_UNARYEXPRESSION, typeof(UnaryExpression), "System.Linq.Expressions.UnaryExpression"),
		new PredefinedTypeInfo(PredefinedType.PT_CONSTANTEXPRESSION, typeof(ConstantExpression), "System.Linq.Expressions.ConstantExpression"),
		new PredefinedTypeInfo(PredefinedType.PT_PARAMETEREXPRESSION, typeof(ParameterExpression), "System.Linq.Expressions.ParameterExpression"),
		new PredefinedTypeInfo(PredefinedType.PT_MEMBEREXPRESSION, typeof(MemberExpression), "System.Linq.Expressions.MemberExpression"),
		new PredefinedTypeInfo(PredefinedType.PT_METHODCALLEXPRESSION, typeof(MethodCallExpression), "System.Linq.Expressions.MethodCallExpression"),
		new PredefinedTypeInfo(PredefinedType.PT_NEWEXPRESSION, typeof(NewExpression), "System.Linq.Expressions.NewExpression"),
		new PredefinedTypeInfo(PredefinedType.PT_NEWARRAYEXPRESSION, typeof(NewArrayExpression), "System.Linq.Expressions.NewArrayExpression"),
		new PredefinedTypeInfo(PredefinedType.PT_INVOCATIONEXPRESSION, typeof(InvocationExpression), "System.Linq.Expressions.InvocationExpression"),
		new PredefinedTypeInfo(PredefinedType.PT_FIELDINFO, typeof(FieldInfo), "System.Reflection.FieldInfo"),
		new PredefinedTypeInfo(PredefinedType.PT_METHODINFO, typeof(MethodInfo), "System.Reflection.MethodInfo"),
		new PredefinedTypeInfo(PredefinedType.PT_CONSTRUCTORINFO, typeof(ConstructorInfo), "System.Reflection.ConstructorInfo"),
		new PredefinedTypeInfo(PredefinedType.PT_PROPERTYINFO, typeof(PropertyInfo), "System.Reflection.PropertyInfo"),
		new PredefinedTypeInfo(PredefinedType.PT_MISSING, typeof(Missing), "System.Reflection.Missing"),
		new PredefinedTypeInfo(PredefinedType.PT_G_IREADONLYLIST, typeof(IReadOnlyList<>), "System.Collections.Generic.IReadOnlyList`1"),
		new PredefinedTypeInfo(PredefinedType.PT_G_IREADONLYCOLLECTION, typeof(IReadOnlyCollection<>), "System.Collections.Generic.IReadOnlyCollection`1"),
		new PredefinedTypeInfo(PredefinedType.PT_FUNC, typeof(Func<>), "System.Func`1")
	};

	private static readonly Dictionary<string, PredefinedType> s_typesByName = CreatePredefinedTypeFacts();

	internal static FUNDTYPE GetFundType(PredefinedType type)
	{
		return s_types[(uint)type].FundType;
	}

	internal static Type GetAssociatedSystemType(PredefinedType type)
	{
		return s_types[(uint)type].AssociatedSystemType;
	}

	internal static bool IsSimpleType(PredefinedType type)
	{
		return type < PredefinedType.FirstNonSimpleType;
	}

	internal static bool IsNumericType(PredefinedType type)
	{
		if (type <= PredefinedType.PT_DECIMAL || type - 9 <= PredefinedType.PT_LONG)
		{
			return true;
		}
		return false;
	}

	internal static string GetNiceName(PredefinedType type)
	{
		return type switch
		{
			PredefinedType.PT_BYTE => "byte", 
			PredefinedType.PT_SHORT => "short", 
			PredefinedType.PT_INT => "int", 
			PredefinedType.PT_LONG => "long", 
			PredefinedType.PT_FLOAT => "float", 
			PredefinedType.PT_DOUBLE => "double", 
			PredefinedType.PT_DECIMAL => "decimal", 
			PredefinedType.PT_CHAR => "char", 
			PredefinedType.PT_BOOL => "bool", 
			PredefinedType.PT_SBYTE => "sbyte", 
			PredefinedType.PT_USHORT => "ushort", 
			PredefinedType.PT_UINT => "uint", 
			PredefinedType.PT_ULONG => "ulong", 
			PredefinedType.PT_OBJECT => "object", 
			PredefinedType.PT_STRING => "string", 
			_ => null, 
		};
	}

	public static PredefinedType TryGetPredefTypeIndex(string name)
	{
		if (!s_typesByName.TryGetValue(name, out var value))
		{
			return PredefinedType.PT_UNDEFINEDINDEX;
		}
		return value;
	}

	private static Dictionary<string, PredefinedType> CreatePredefinedTypeFacts()
	{
		Dictionary<string, PredefinedType> dictionary = new Dictionary<string, PredefinedType>(49);
		for (int i = 0; i < 49; i++)
		{
			dictionary.Add(s_types[i].Name, (PredefinedType)i);
		}
		return dictionary;
	}
}
