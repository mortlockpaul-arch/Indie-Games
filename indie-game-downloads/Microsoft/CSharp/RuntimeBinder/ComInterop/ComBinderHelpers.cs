using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal static class ComBinderHelpers
{
	internal static bool PreferPut(Type type, bool holdsNull)
	{
		if (((type.IsValueType || type.IsArray || type == typeof(string) || type == typeof(DBNull)) | holdsNull) || type == typeof(Missing) || type == typeof(CurrencyWrapper))
		{
			return true;
		}
		return false;
	}

	internal static bool IsByRef(DynamicMetaObject mo)
	{
		if (mo.Expression is ParameterExpression parameterExpression)
		{
			return parameterExpression.IsByRef;
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal static bool IsStrongBoxArg(DynamicMetaObject o)
	{
		Type limitType = o.LimitType;
		if (limitType.IsGenericType)
		{
			return limitType.GetGenericTypeDefinition() == typeof(StrongBox<>);
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal static bool[] ProcessArgumentsForCom(ref DynamicMetaObject[] args)
	{
		DynamicMetaObject[] array = new DynamicMetaObject[args.Length];
		bool[] array2 = new bool[args.Length];
		for (int i = 0; i < args.Length; i++)
		{
			DynamicMetaObject dynamicMetaObject = args[i];
			if (IsByRef(dynamicMetaObject))
			{
				array[i] = dynamicMetaObject;
				array2[i] = true;
			}
			else if (IsStrongBoxArg(dynamicMetaObject))
			{
				BindingRestrictions restrictions = dynamicMetaObject.Restrictions.Merge(GetTypeRestrictionForDynamicMetaObject(dynamicMetaObject));
				Expression expression = Expression.Field(Helpers.Convert(dynamicMetaObject.Expression, dynamicMetaObject.LimitType), dynamicMetaObject.LimitType.GetField("Value"));
				object value = (dynamicMetaObject.Value as IStrongBox)?.Value;
				array[i] = new DynamicMetaObject(expression, restrictions, value);
				array2[i] = true;
			}
			else
			{
				array[i] = dynamicMetaObject;
				array2[i] = false;
			}
		}
		args = array;
		return array2;
	}

	internal static BindingRestrictions GetTypeRestrictionForDynamicMetaObject(DynamicMetaObject obj)
	{
		if (obj.Value == null && obj.HasValue)
		{
			return BindingRestrictions.GetInstanceRestriction(obj.Expression, null);
		}
		return BindingRestrictions.GetTypeRestriction(obj.Expression, obj.LimitType);
	}
}
