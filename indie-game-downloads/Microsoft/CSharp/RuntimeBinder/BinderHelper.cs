using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics.Hashing;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.CSharp.RuntimeBinder;

internal static class BinderHelper
{
	private static MethodInfo s_DoubleIsNaN;

	private static MethodInfo s_SingleIsNaN;

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal static DynamicMetaObject Bind(ICSharpBinder action, RuntimeBinder binder, DynamicMetaObject[] args, IEnumerable<CSharpArgumentInfo> arginfos, DynamicMetaObject onBindingError)
	{
		Expression[] array = new Expression[args.Length];
		BindingRestrictions bindingRestrictions = BindingRestrictions.Empty;
		ICSharpInvokeOrInvokeMemberBinder callPayload = action as ICSharpInvokeOrInvokeMemberBinder;
		ParameterExpression parameterExpression = null;
		IEnumerator<CSharpArgumentInfo> enumerator = (arginfos ?? Array.Empty<CSharpArgumentInfo>()).GetEnumerator();
		for (int i = 0; i < args.Length; i++)
		{
			DynamicMetaObject dynamicMetaObject = args[i];
			CSharpArgumentInfo cSharpArgumentInfo = (enumerator.MoveNext() ? enumerator.Current : null);
			if (i == 0 && IsIncrementOrDecrementActionOnLocal(action))
			{
				object value = dynamicMetaObject.Value;
				parameterExpression = (ParameterExpression)(array[0] = Expression.Variable((value != null) ? value.GetType() : typeof(object), "t0"));
			}
			else
			{
				array[i] = dynamicMetaObject.Expression;
			}
			BindingRestrictions restrictions = DeduceArgumentRestriction(i, callPayload, dynamicMetaObject, cSharpArgumentInfo);
			bindingRestrictions = bindingRestrictions.Merge(restrictions);
			if (cSharpArgumentInfo != null && cSharpArgumentInfo.LiteralConstant)
			{
				if (dynamicMetaObject.Value is double && double.IsNaN((double)dynamicMetaObject.Value))
				{
					MethodInfo method = s_DoubleIsNaN ?? (s_DoubleIsNaN = typeof(double).GetMethod("IsNaN"));
					Expression expression = Expression.Call(null, method, dynamicMetaObject.Expression);
					bindingRestrictions = bindingRestrictions.Merge(BindingRestrictions.GetExpressionRestriction(expression));
				}
				else if (dynamicMetaObject.Value is float && float.IsNaN((float)dynamicMetaObject.Value))
				{
					MethodInfo method2 = s_SingleIsNaN ?? (s_SingleIsNaN = typeof(float).GetMethod("IsNaN"));
					Expression expression2 = Expression.Call(null, method2, dynamicMetaObject.Expression);
					bindingRestrictions = bindingRestrictions.Merge(BindingRestrictions.GetExpressionRestriction(expression2));
				}
				else
				{
					restrictions = BindingRestrictions.GetExpressionRestriction(Expression.Equal(dynamicMetaObject.Expression, Expression.Constant(dynamicMetaObject.Value, dynamicMetaObject.Expression.Type)));
					bindingRestrictions = bindingRestrictions.Merge(restrictions);
				}
			}
		}
		try
		{
			Expression expression3 = binder.Bind(action, array, args, out var deferredBinding);
			if (deferredBinding != null)
			{
				expression3 = ConvertResult(deferredBinding.Expression, action);
				bindingRestrictions = deferredBinding.Restrictions.Merge(bindingRestrictions);
				return new DynamicMetaObject(expression3, bindingRestrictions);
			}
			if (parameterExpression != null)
			{
				DynamicMetaObject dynamicMetaObject2 = args[0];
				expression3 = Expression.Block(new ParameterExpression[1] { parameterExpression }, Expression.Assign(parameterExpression, Expression.Convert(dynamicMetaObject2.Expression, dynamicMetaObject2.Value.GetType())), expression3, Expression.Assign(dynamicMetaObject2.Expression, Expression.Convert(parameterExpression, dynamicMetaObject2.Expression.Type)));
			}
			expression3 = ConvertResult(expression3, action);
			return new DynamicMetaObject(expression3, bindingRestrictions);
		}
		catch (RuntimeBinderException ex)
		{
			if (onBindingError != null)
			{
				return onBindingError;
			}
			return new DynamicMetaObject(Expression.Throw(Expression.New(typeof(RuntimeBinderException).GetConstructor(new Type[1] { typeof(string) }), Expression.Constant(ex.Message)), GetTypeForErrorMetaObject(action, args)), bindingRestrictions);
		}
	}

	public static void ValidateBindArgument(DynamicMetaObject argument, string paramName)
	{
		ArgumentNullException.ThrowIfNull(argument, paramName);
		if (!argument.HasValue)
		{
			throw Error.DynamicArgumentNeedsValue(paramName);
		}
	}

	public static void ValidateBindArgument(DynamicMetaObject[] arguments, string paramName)
	{
		if (arguments != null)
		{
			for (int i = 0; i < arguments.Length; i++)
			{
				ValidateBindArgument(arguments[i], $"{paramName}[{i}]");
			}
		}
	}

	private static bool IsTypeOfStaticCall(int parameterIndex, ICSharpInvokeOrInvokeMemberBinder callPayload)
	{
		if (parameterIndex == 0 && callPayload != null)
		{
			return callPayload.StaticCall;
		}
		return false;
	}

	private static bool IsComObject(object obj)
	{
		if (obj != null)
		{
			return Marshal.IsComObject(obj);
		}
		return false;
	}

	private static bool IsDynamicallyTypedRuntimeProxy(DynamicMetaObject argument, CSharpArgumentInfo info)
	{
		if (info != null && !info.UseCompileTimeType)
		{
			return IsComObject(argument.Value);
		}
		return false;
	}

	private static BindingRestrictions DeduceArgumentRestriction(int parameterIndex, ICSharpInvokeOrInvokeMemberBinder callPayload, DynamicMetaObject argument, CSharpArgumentInfo info)
	{
		if (argument.Value != null && !IsTypeOfStaticCall(parameterIndex, callPayload) && !IsDynamicallyTypedRuntimeProxy(argument, info))
		{
			return BindingRestrictions.GetTypeRestriction(argument.Expression, argument.RuntimeType);
		}
		return BindingRestrictions.GetInstanceRestriction(argument.Expression, argument.Value);
	}

	private static Expression ConvertResult(Expression binding, ICSharpBinder action)
	{
		if (action is CSharpInvokeConstructorBinder)
		{
			return binding;
		}
		if (binding.Type == typeof(void))
		{
			if (action is ICSharpInvokeOrInvokeMemberBinder { ResultDiscarded: not false })
			{
				return Expression.Block(binding, Expression.Default(action.ReturnType));
			}
			throw Error.BindToVoidMethodButExpectResult();
		}
		if (binding.Type.IsValueType && !action.ReturnType.IsValueType)
		{
			return Expression.Convert(binding, action.ReturnType);
		}
		return binding;
	}

	private static Type GetTypeForErrorMetaObject(ICSharpBinder action, DynamicMetaObject[] args)
	{
		if (action is CSharpInvokeConstructorBinder)
		{
			return args[0].Value as Type;
		}
		return action.ReturnType;
	}

	private static bool IsIncrementOrDecrementActionOnLocal(ICSharpBinder action)
	{
		if (action is CSharpUnaryOperationBinder cSharpUnaryOperationBinder)
		{
			if (cSharpUnaryOperationBinder.Operation != ExpressionType.Increment)
			{
				return cSharpUnaryOperationBinder.Operation == ExpressionType.Decrement;
			}
			return true;
		}
		return false;
	}

	internal static T[] Cons<T>(T sourceHead, T[] sourceTail)
	{
		if (sourceTail == null || sourceTail.Length != 0)
		{
			T[] array = new T[sourceTail.Length + 1];
			array[0] = sourceHead;
			sourceTail.CopyTo(array, 1);
			return array;
		}
		return new T[1] { sourceHead };
	}

	internal static T[] Cons<T>(T sourceHead, T[] sourceMiddle, T sourceLast)
	{
		if (sourceMiddle == null || sourceMiddle.Length != 0)
		{
			T[] array = new T[sourceMiddle.Length + 2];
			array[0] = sourceHead;
			array[^1] = sourceLast;
			sourceMiddle.CopyTo(array, 1);
			return array;
		}
		return new T[2] { sourceHead, sourceLast };
	}

	internal static T[] ToArray<T>(IEnumerable<T> source)
	{
		if (source != null)
		{
			return source.ToArray();
		}
		return Array.Empty<T>();
	}

	internal static CallInfo CreateCallInfo(ref IEnumerable<CSharpArgumentInfo> argInfos, int discard)
	{
		int num = 0;
		List<string> list = new List<string>();
		CSharpArgumentInfo[] array = (CSharpArgumentInfo[])(argInfos = ToArray(argInfos));
		foreach (CSharpArgumentInfo cSharpArgumentInfo in array)
		{
			if (cSharpArgumentInfo.NamedArgument)
			{
				list.Add(cSharpArgumentInfo.Name);
			}
			num++;
		}
		return new CallInfo(num - discard, list);
	}

	internal static string GetCLROperatorName(this ExpressionType p)
	{
		return p switch
		{
			ExpressionType.Add => "op_Addition", 
			ExpressionType.Subtract => "op_Subtraction", 
			ExpressionType.Multiply => "op_Multiply", 
			ExpressionType.Divide => "op_Division", 
			ExpressionType.Modulo => "op_Modulus", 
			ExpressionType.LeftShift => "op_LeftShift", 
			ExpressionType.RightShift => "op_RightShift", 
			ExpressionType.LessThan => "op_LessThan", 
			ExpressionType.GreaterThan => "op_GreaterThan", 
			ExpressionType.LessThanOrEqual => "op_LessThanOrEqual", 
			ExpressionType.GreaterThanOrEqual => "op_GreaterThanOrEqual", 
			ExpressionType.Equal => "op_Equality", 
			ExpressionType.NotEqual => "op_Inequality", 
			ExpressionType.And => "op_BitwiseAnd", 
			ExpressionType.ExclusiveOr => "op_ExclusiveOr", 
			ExpressionType.Or => "op_BitwiseOr", 
			ExpressionType.AddAssign => "op_Addition", 
			ExpressionType.SubtractAssign => "op_Subtraction", 
			ExpressionType.MultiplyAssign => "op_Multiply", 
			ExpressionType.DivideAssign => "op_Division", 
			ExpressionType.ModuloAssign => "op_Modulus", 
			ExpressionType.AndAssign => "op_BitwiseAnd", 
			ExpressionType.ExclusiveOrAssign => "op_ExclusiveOr", 
			ExpressionType.OrAssign => "op_BitwiseOr", 
			ExpressionType.LeftShiftAssign => "op_LeftShift", 
			ExpressionType.RightShiftAssign => "op_RightShift", 
			ExpressionType.Negate => "op_UnaryNegation", 
			ExpressionType.UnaryPlus => "op_UnaryPlus", 
			ExpressionType.Not => "op_LogicalNot", 
			ExpressionType.OnesComplement => "op_OnesComplement", 
			ExpressionType.IsTrue => "op_True", 
			ExpressionType.IsFalse => "op_False", 
			ExpressionType.Increment => "op_Increment", 
			ExpressionType.Decrement => "op_Decrement", 
			_ => null, 
		};
	}

	internal static int AddArgHashes(int hash, Type[] typeArguments, CSharpArgumentInfo[] argInfos)
	{
		foreach (Type type in typeArguments)
		{
			hash = HashHelpers.Combine(hash, type.GetHashCode());
		}
		return AddArgHashes(hash, argInfos);
	}

	internal static int AddArgHashes(int hash, CSharpArgumentInfo[] argInfos)
	{
		foreach (CSharpArgumentInfo cSharpArgumentInfo in argInfos)
		{
			hash = HashHelpers.Combine(hash, (int)cSharpArgumentInfo.Flags);
			string name = cSharpArgumentInfo.Name;
			if (!string.IsNullOrEmpty(name))
			{
				hash = HashHelpers.Combine(hash, name.GetHashCode());
			}
		}
		return hash;
	}

	internal static bool CompareArgInfos(Type[] typeArgs, Type[] otherTypeArgs, CSharpArgumentInfo[] argInfos, CSharpArgumentInfo[] otherArgInfos)
	{
		for (int i = 0; i < typeArgs.Length; i++)
		{
			if (typeArgs[i] != otherTypeArgs[i])
			{
				return false;
			}
		}
		return CompareArgInfos(argInfos, otherArgInfos);
	}

	internal static bool CompareArgInfos(CSharpArgumentInfo[] argInfos, CSharpArgumentInfo[] otherArgInfos)
	{
		for (int i = 0; i < argInfos.Length; i++)
		{
			CSharpArgumentInfo cSharpArgumentInfo = argInfos[i];
			CSharpArgumentInfo cSharpArgumentInfo2 = otherArgInfos[i];
			if (cSharpArgumentInfo.Flags != cSharpArgumentInfo2.Flags || cSharpArgumentInfo.Name != cSharpArgumentInfo2.Name)
			{
				return false;
			}
		}
		return true;
	}
}
