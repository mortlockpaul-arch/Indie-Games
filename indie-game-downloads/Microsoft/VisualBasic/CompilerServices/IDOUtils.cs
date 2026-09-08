using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualBasic.CompilerServices;

internal sealed class IDOUtils
{
	private static CacheSet<CallSiteBinder> s_binderCache = new CacheSet<CallSiteBinder>(64);

	private static CacheDict<int, Func<CallSiteBinder, object, object[], object>> Invokers = new CacheDict<int, Func<CallSiteBinder, object, object[], object>>(16);

	private static CallSiteBinder GetCachedBinder(CallSiteBinder action)
	{
		return s_binderCache.GetExistingOrAdd(action);
	}

	internal static IDynamicMetaObjectProvider TryCastToIDMOP(object o)
	{
		if (o is IDynamicMetaObjectProvider result)
		{
			return result;
		}
		return null;
	}

	internal static ExpressionType? LinqOperator(Symbols.UserDefinedOperator vbOperator)
	{
		return vbOperator switch
		{
			Symbols.UserDefinedOperator.Negate => ExpressionType.Negate, 
			Symbols.UserDefinedOperator.Not => ExpressionType.Not, 
			Symbols.UserDefinedOperator.UnaryPlus => ExpressionType.UnaryPlus, 
			Symbols.UserDefinedOperator.Plus => ExpressionType.Add, 
			Symbols.UserDefinedOperator.Minus => ExpressionType.Subtract, 
			Symbols.UserDefinedOperator.Multiply => ExpressionType.Multiply, 
			Symbols.UserDefinedOperator.Divide => ExpressionType.Divide, 
			Symbols.UserDefinedOperator.Power => ExpressionType.Power, 
			Symbols.UserDefinedOperator.ShiftLeft => ExpressionType.LeftShift, 
			Symbols.UserDefinedOperator.ShiftRight => ExpressionType.RightShift, 
			Symbols.UserDefinedOperator.Modulus => ExpressionType.Modulo, 
			Symbols.UserDefinedOperator.Or => ExpressionType.Or, 
			Symbols.UserDefinedOperator.Xor => ExpressionType.ExclusiveOr, 
			Symbols.UserDefinedOperator.And => ExpressionType.And, 
			Symbols.UserDefinedOperator.Equal => ExpressionType.Equal, 
			Symbols.UserDefinedOperator.NotEqual => ExpressionType.NotEqual, 
			Symbols.UserDefinedOperator.Less => ExpressionType.LessThan, 
			Symbols.UserDefinedOperator.LessEqual => ExpressionType.LessThanOrEqual, 
			Symbols.UserDefinedOperator.GreaterEqual => ExpressionType.GreaterThanOrEqual, 
			Symbols.UserDefinedOperator.Greater => ExpressionType.GreaterThan, 
			_ => null, 
		};
	}

	public static void CopyBackArguments(CallInfo callInfo, object[] packedArgs, object[] args)
	{
		if (packedArgs != args)
		{
			int num = packedArgs.Length;
			int argumentCount = callInfo.ArgumentCount;
			int num2;
			int num3;
			checked
			{
				num2 = num - callInfo.ArgumentNames.Count;
				num3 = num - 1;
			}
			for (int i = 0; i <= num3; i = checked(i + 1))
			{
				args[i] = packedArgs[(i < argumentCount) ? (checked(i + num2) % argumentCount) : i];
			}
		}
	}

	public static void PackArguments(int valueArgs, string[] argNames, object[] args, ref object[] packedArgs, ref CallInfo callInfo)
	{
		if (argNames == null)
		{
			argNames = Array.Empty<string>();
		}
		checked
		{
			callInfo = new CallInfo(args.Length - valueArgs, argNames);
			if (argNames.Length > 0)
			{
				packedArgs = new object[args.Length - 1 + 1];
				int num = args.Length - valueArgs;
				int num2 = num - 1;
				for (int i = 0; i <= num2; i++)
				{
					unchecked
					{
						packedArgs[i] = args[checked(i + argNames.Length) % num];
					}
				}
				int num3 = args.Length - 1;
				for (int j = num; j <= num3; j++)
				{
					packedArgs[j] = args[j];
				}
			}
			else
			{
				packedArgs = args;
			}
		}
	}

	public static void UnpackArguments(DynamicMetaObject[] packedArgs, CallInfo callInfo, ref Expression[] args, ref string[] argNames, ref object[] argValues)
	{
		int num = packedArgs.Length;
		int argumentCount = callInfo.ArgumentCount;
		checked
		{
			args = new Expression[num - 1 + 1];
			argValues = new object[num - 1 + 1];
			int count = callInfo.ArgumentNames.Count;
			int num2 = num - count;
			int num3 = argumentCount - 1;
			for (int i = 0; i <= num3; i++)
			{
				unchecked
				{
					DynamicMetaObject dynamicMetaObject = packedArgs[checked(i + num2) % argumentCount];
					args[i] = dynamicMetaObject.Expression;
					argValues[i] = dynamicMetaObject.Value;
				}
			}
			int num4 = num - 1;
			for (int j = argumentCount; j <= num4; j++)
			{
				DynamicMetaObject dynamicMetaObject2 = packedArgs[j];
				args[j] = dynamicMetaObject2.Expression;
				argValues[j] = dynamicMetaObject2.Value;
			}
			argNames = new string[count - 1 + 1];
			callInfo.ArgumentNames.CopyTo(argNames, 0);
		}
	}

	public static Expression GetWriteBack(Expression[] arguments, ParameterExpression array)
	{
		List<Expression> list = new List<Expression>();
		checked
		{
			int num = arguments.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				if (arguments[i] is ParameterExpression { IsByRef: not false } parameterExpression)
				{
					list.Add(Expression.Assign(parameterExpression, Expression.ArrayIndex(array, Expression.Constant(i))));
				}
			}
			return list.Count switch
			{
				0 => Expression.Empty(), 
				1 => list[0], 
				_ => Expression.Block(list), 
			};
		}
	}

	public static Expression ConvertToObject(Expression valueExpression)
	{
		if (!valueExpression.Type.Equals(typeof(object)))
		{
			return Expression.Convert(valueExpression, typeof(object));
		}
		return valueExpression;
	}

	[RequiresUnreferencedCode("Calls CreateInvoker")]
	public static object CreateRefCallSiteAndInvoke(CallSiteBinder action, object instance, object[] arguments)
	{
		action = GetCachedBinder(action);
		Func<CallSiteBinder, object, object[], object> value = null;
		lock (Invokers)
		{
			if (!Invokers.TryGetValue(arguments.Length, out value))
			{
				value = CreateInvoker(arguments.Length);
				Invokers.Add(arguments.Length, value);
			}
		}
		return value(action, instance, arguments);
	}

	[RequiresUnreferencedCode("Calls Type.GetMethod() that cannot be statically analyzed")]
	private static Func<CallSiteBinder, object, object[], object> CreateInvoker(int ArgLength)
	{
		Type typeFromHandle = typeof(object);
		Type type = typeFromHandle.MakeByRefType();
		Type typeFromHandle2 = typeof(CallSiteBinder);
		checked
		{
			Type[] array = new Type[ArgLength + 2 + 1];
			array[0] = typeof(CallSite);
			array[1] = typeFromHandle;
			int num = array.Length - 2;
			for (int i = 2; i <= num; i++)
			{
				array[i] = type;
			}
			array[array.Length - 1] = typeFromHandle;
			Type delegateType = Expression.GetDelegateType(array);
			Type type2 = typeof(CallSite<>).MakeGenericType(delegateType);
			DynamicMethod dynamicMethod = new DynamicMethod("Invoker", typeFromHandle, new Type[3]
			{
				typeFromHandle2,
				typeFromHandle,
				typeof(object[])
			}, restrictedSkipVisibility: true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			LocalBuilder local = iLGenerator.DeclareLocal(type2);
			iLGenerator.Emit(OpCodes.Ldarg_0);
			iLGenerator.Emit(OpCodes.Call, type2.GetMethod("Create", new Type[1] { typeFromHandle2 }));
			iLGenerator.Emit(OpCodes.Stloc, local);
			iLGenerator.Emit(OpCodes.Ldloc, local);
			iLGenerator.Emit(OpCodes.Ldfld, type2.GetField("Target"));
			iLGenerator.Emit(OpCodes.Ldloc, local);
			iLGenerator.Emit(OpCodes.Ldarg_1);
			int num2 = ArgLength - 1;
			for (int j = 0; j <= num2; j++)
			{
				iLGenerator.Emit(OpCodes.Ldarg_2);
				iLGenerator.Emit(OpCodes.Ldc_I4, j);
				iLGenerator.Emit(OpCodes.Ldelema, typeFromHandle);
			}
			iLGenerator.Emit(OpCodes.Callvirt, delegateType.GetMethod("Invoke"));
			iLGenerator.Emit(OpCodes.Ret);
			return (Func<CallSiteBinder, object, object[], object>)dynamicMethod.CreateDelegate(typeof(Func<CallSiteBinder, object, object[], object>));
		}
	}

	[RequiresUnreferencedCode("Calls Object.GetType().GetField()")]
	public static object CreateFuncCallSiteAndInvoke(CallSiteBinder action, object instance, object[] arguments)
	{
		action = GetCachedBinder(action);
		checked
		{
			switch (arguments.Length)
			{
			case 0:
			{
				CallSite<Func<CallSite, object, object>> callSite3 = CallSite<Func<CallSite, object, object>>.Create(action);
				return callSite3.Target(callSite3, instance);
			}
			case 1:
			{
				CallSite<Func<CallSite, object, object, object>> callSite2 = CallSite<Func<CallSite, object, object, object>>.Create(action);
				return callSite2.Target(callSite2, instance, arguments[0]);
			}
			case 2:
			{
				CallSite<Func<CallSite, object, object, object, object>> callSite9 = CallSite<Func<CallSite, object, object, object, object>>.Create(action);
				return callSite9.Target(callSite9, instance, arguments[0], arguments[1]);
			}
			case 3:
			{
				CallSite<Func<CallSite, object, object, object, object, object>> callSite8 = CallSite<Func<CallSite, object, object, object, object, object>>.Create(action);
				return callSite8.Target(callSite8, instance, arguments[0], arguments[1], arguments[2]);
			}
			case 4:
			{
				CallSite<Func<CallSite, object, object, object, object, object, object>> callSite7 = CallSite<Func<CallSite, object, object, object, object, object, object>>.Create(action);
				return callSite7.Target(callSite7, instance, arguments[0], arguments[1], arguments[2], arguments[3]);
			}
			case 5:
			{
				CallSite<Func<CallSite, object, object, object, object, object, object, object>> callSite6 = CallSite<Func<CallSite, object, object, object, object, object, object, object>>.Create(action);
				return callSite6.Target(callSite6, instance, arguments[0], arguments[1], arguments[2], arguments[3], arguments[4]);
			}
			case 6:
			{
				CallSite<Func<CallSite, object, object, object, object, object, object, object, object>> callSite5 = CallSite<Func<CallSite, object, object, object, object, object, object, object, object>>.Create(action);
				return callSite5.Target(callSite5, instance, arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5]);
			}
			case 7:
			{
				CallSite<Func<CallSite, object, object, object, object, object, object, object, object, object>> callSite4 = CallSite<Func<CallSite, object, object, object, object, object, object, object, object, object>>.Create(action);
				return callSite4.Target(callSite4, instance, arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6]);
			}
			default:
			{
				Type[] array = new Type[arguments.Length + 2 + 1];
				array[0] = typeof(CallSite);
				int num = array.Length - 1;
				for (int i = 1; i <= num; i++)
				{
					array[i] = typeof(object);
				}
				CallSite callSite = CallSite.Create(Expression.GetDelegateType(array), action);
				object[] array2 = new object[arguments.Length + 1 + 1];
				array2[0] = callSite;
				array2[1] = instance;
				arguments.CopyTo(array2, 2);
				Delegate obj = (Delegate)callSite.GetType().GetField("Target").GetValue(callSite);
				try
				{
					return obj.DynamicInvoke(array2);
				}
				catch (TargetInvocationException ex)
				{
					throw ex.InnerException;
				}
			}
			}
		}
	}

	[RequiresUnreferencedCode("Calls Object.GetType().GetField()")]
	public static object CreateConvertCallSiteAndInvoke(ConvertBinder action, object instance)
	{
		CallSite callSite = CallSite.Create(Expression.GetFuncType(typeof(CallSite), typeof(object), action.Type), GetCachedBinder(action));
		object[] args = new object[2] { callSite, instance };
		Delegate obj = (Delegate)callSite.GetType().GetField("Target").GetValue(callSite);
		try
		{
			return obj.DynamicInvoke(args);
		}
		catch (TargetInvocationException ex)
		{
			throw ex.InnerException;
		}
	}

	internal static BindingRestrictions CreateRestrictions(DynamicMetaObject target, DynamicMetaObject[] args = null, DynamicMetaObject value = null)
	{
		BindingRestrictions bindingRestrictions = CreateRestriction(target);
		if (args != null)
		{
			foreach (DynamicMetaObject metaObject in args)
			{
				bindingRestrictions = bindingRestrictions.Merge(CreateRestriction(metaObject));
			}
		}
		if (value != null)
		{
			bindingRestrictions = bindingRestrictions.Merge(CreateRestriction(value));
		}
		return bindingRestrictions;
	}

	private static BindingRestrictions CreateRestriction(DynamicMetaObject metaObject)
	{
		if (metaObject.Value == null)
		{
			return metaObject.Restrictions.Merge(BindingRestrictions.GetInstanceRestriction(metaObject.Expression, null));
		}
		return metaObject.Restrictions.Merge(BindingRestrictions.GetTypeRestriction(metaObject.Expression, metaObject.LimitType));
	}

	internal static bool NeedsDeferral(DynamicMetaObject target, DynamicMetaObject[] args = null, DynamicMetaObject value = null)
	{
		if (!target.HasValue)
		{
			return true;
		}
		if (value != null && !value.HasValue)
		{
			return true;
		}
		if (args != null)
		{
			for (int i = 0; i < args.Length; i = checked(i + 1))
			{
				if (!args[i].HasValue)
				{
					return true;
				}
			}
		}
		return false;
	}
}
