using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices.Marshalling;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal static class VariantArray
{
	private static readonly List<Type> s_generatedTypes = new List<Type>(0);

	[DynamicDependency(DynamicallyAccessedMemberTypes.PublicFields, typeof(VariantArray1))]
	[DynamicDependency(DynamicallyAccessedMemberTypes.PublicFields, typeof(VariantArray2))]
	[DynamicDependency(DynamicallyAccessedMemberTypes.PublicFields, typeof(VariantArray4))]
	[DynamicDependency(DynamicallyAccessedMemberTypes.PublicFields, typeof(VariantArray8))]
	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "Types are either dynamically created or have dynamic dependency.")]
	internal static MemberExpression GetStructField(ParameterExpression variantArray, int field)
	{
		return Expression.Field(variantArray, "Element" + field);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal static Type GetStructType(int args)
	{
		if (args <= 1)
		{
			return typeof(VariantArray1);
		}
		if (args <= 2)
		{
			return typeof(VariantArray2);
		}
		if (args <= 4)
		{
			return typeof(VariantArray4);
		}
		if (args <= 8)
		{
			return typeof(VariantArray8);
		}
		int num = 1;
		while (args > num)
		{
			num *= 2;
		}
		lock (s_generatedTypes)
		{
			foreach (Type s_generatedType in s_generatedTypes)
			{
				int num2 = int.Parse(s_generatedType.Name.AsSpan("VariantArray".Length), CultureInfo.InvariantCulture);
				if (num == num2)
				{
					return s_generatedType;
				}
			}
			Type type = CreateCustomType(num);
			s_generatedTypes.Add(type);
			return type;
		}
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static Type CreateCustomType(int size)
	{
		TypeAttributes attr = TypeAttributes.SequentialLayout;
		TypeBuilder typeBuilder = UnsafeMethods.DynamicModule.DefineType("VariantArray" + size, attr, typeof(ValueType));
		for (int i = 0; i < size; i++)
		{
			typeBuilder.DefineField("Element" + i, typeof(ComVariant), FieldAttributes.Public);
		}
		return typeBuilder.CreateType();
	}
}
