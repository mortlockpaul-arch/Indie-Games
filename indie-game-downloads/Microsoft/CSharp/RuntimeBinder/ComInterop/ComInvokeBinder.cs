using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.Marshalling;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class ComInvokeBinder
{
	private readonly ComMethodDesc _methodDesc;

	private readonly Expression _method;

	private readonly Expression _dispatch;

	private readonly CallInfo _callInfo;

	private readonly DynamicMetaObject[] _args;

	private readonly bool[] _isByRef;

	private readonly Expression _instance;

	private BindingRestrictions _restrictions;

	private VarEnumSelector _varEnumSelector;

	private string[] _keywordArgNames;

	private int _totalExplicitArgs;

	private ParameterExpression _dispatchObject;

	private ParameterExpression _dispatchPointer;

	private ParameterExpression _dispId;

	private ParameterExpression _dispParams;

	private ParameterExpression _paramVariants;

	private ParameterExpression _invokeResult;

	private ParameterExpression _returnValue;

	private ParameterExpression _dispIdsOfKeywordArgsPinned;

	private ParameterExpression _propertyPutDispId;

	private ParameterExpression DispatchObjectVariable => EnsureVariable(ref _dispatchObject, typeof(IDispatch), "dispatchObject");

	private ParameterExpression DispatchPointerVariable => EnsureVariable(ref _dispatchPointer, typeof(nint), "dispatchPointer");

	private ParameterExpression DispIdVariable => EnsureVariable(ref _dispId, typeof(int), "dispId");

	private ParameterExpression DispParamsVariable => EnsureVariable(ref _dispParams, typeof(DISPPARAMS), "dispParams");

	private ParameterExpression InvokeResultVariable => EnsureVariable(ref _invokeResult, typeof(ComVariant), "invokeResult");

	private ParameterExpression ReturnValueVariable => EnsureVariable(ref _returnValue, typeof(object), "returnValue");

	private ParameterExpression DispIdsOfKeywordArgsPinnedVariable => EnsureVariable(ref _dispIdsOfKeywordArgsPinned, typeof(GCHandle), "dispIdsOfKeywordArgsPinned");

	private ParameterExpression PropertyPutDispIdVariable => EnsureVariable(ref _propertyPutDispId, typeof(int), "propertyPutDispId");

	private ParameterExpression ParamVariantsVariable => _paramVariants ?? (_paramVariants = Expression.Variable(VariantArray.GetStructType(_args.Length), "paramVariants"));

	internal ComInvokeBinder(CallInfo callInfo, DynamicMetaObject[] args, bool[] isByRef, BindingRestrictions restrictions, Expression method, Expression dispatch, ComMethodDesc methodDesc)
	{
		_method = method;
		_dispatch = dispatch;
		_methodDesc = methodDesc;
		_callInfo = callInfo;
		_args = args;
		_isByRef = isByRef;
		_restrictions = restrictions;
		_instance = dispatch;
	}

	private static ParameterExpression EnsureVariable(ref ParameterExpression var, Type type, string name)
	{
		if (var != null)
		{
			return var;
		}
		return var = Expression.Variable(type, name);
	}

	private static Type MarshalType(DynamicMetaObject mo, bool isByRef)
	{
		Type type = ((mo.Value == null && mo.HasValue && !mo.LimitType.IsValueType) ? null : mo.LimitType);
		if (isByRef)
		{
			if ((object)type == null)
			{
				type = mo.Expression.Type;
			}
			type = type.MakeByRefType();
		}
		return type;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal DynamicMetaObject Invoke()
	{
		_keywordArgNames = _callInfo.ArgumentNames.ToArray();
		_totalExplicitArgs = _args.Length;
		Type[] array = new Type[_args.Length];
		for (int i = 0; i < _args.Length; i++)
		{
			DynamicMetaObject dynamicMetaObject = _args[i];
			_restrictions = _restrictions.Merge(ComBinderHelpers.GetTypeRestrictionForDynamicMetaObject(dynamicMetaObject));
			array[i] = MarshalType(dynamicMetaObject, _isByRef[i]);
		}
		_varEnumSelector = new VarEnumSelector(array);
		return new DynamicMetaObject(CreateScope(MakeIDispatchInvokeTarget()), BindingRestrictions.Combine(_args).Merge(_restrictions));
	}

	private static void AddNotNull(List<ParameterExpression> list, ParameterExpression var)
	{
		if (var != null)
		{
			list.Add(var);
		}
	}

	private Expression CreateScope(Expression expression)
	{
		List<ParameterExpression> list = new List<ParameterExpression>();
		AddNotNull(list, _dispatchObject);
		AddNotNull(list, _dispatchPointer);
		AddNotNull(list, _dispId);
		AddNotNull(list, _dispParams);
		AddNotNull(list, _paramVariants);
		AddNotNull(list, _invokeResult);
		AddNotNull(list, _returnValue);
		AddNotNull(list, _dispIdsOfKeywordArgsPinned);
		AddNotNull(list, _propertyPutDispId);
		if (list.Count <= 0)
		{
			return expression;
		}
		return Expression.Block(list, expression);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expression GenerateTryBlock()
	{
		ParameterExpression parameterExpression = Expression.Variable(typeof(ExcepInfo), "excepInfo");
		ParameterExpression parameterExpression2 = Expression.Variable(typeof(uint), "argErr");
		ParameterExpression parameterExpression3 = Expression.Variable(typeof(int), "hresult");
		List<Expression> list = new List<Expression>();
		if (_keywordArgNames.Length != 0)
		{
			string[] value = _keywordArgNames.AddFirst(_methodDesc.Name);
			list.Add(Expression.Assign(Expression.Field(DispParamsVariable, typeof(DISPPARAMS).GetField("rgdispidNamedArgs")), Expression.Call(typeof(UnsafeMethods).GetMethod("GetIdsOfNamedParameters"), DispatchObjectVariable, Expression.Constant(value), DispIdVariable, DispIdsOfKeywordArgsPinnedVariable)));
		}
		Expression[] array = MakeArgumentExpressions();
		int num = _varEnumSelector.VariantBuilders.Length - 1;
		int num2 = _varEnumSelector.VariantBuilders.Length - _keywordArgNames.Length;
		int num3 = 0;
		while (num3 < _varEnumSelector.VariantBuilders.Length)
		{
			int field = ((num3 < num2) ? num : (num3 - num2));
			Expression expression = _varEnumSelector.VariantBuilders[num3].InitializeArgumentVariant(VariantArray.GetStructField(ParamVariantsVariable, field), array[num3 + 1]);
			if (expression != null)
			{
				list.Add(expression);
			}
			num3++;
			num--;
		}
		INVOKEKIND iNVOKEKIND = ((!_methodDesc.IsPropertyPut) ? (INVOKEKIND.INVOKE_FUNC | INVOKEKIND.INVOKE_PROPERTYGET) : ((!_methodDesc.IsPropertyPutRef) ? INVOKEKIND.INVOKE_PROPERTYPUT : INVOKEKIND.INVOKE_PROPERTYPUTREF));
		MethodCallExpression right = Expression.Call(typeof(UnsafeMethods).GetMethod("IDispatchInvoke"), DispatchPointerVariable, DispIdVariable, Expression.Constant(iNVOKEKIND), DispParamsVariable, InvokeResultVariable, parameterExpression, parameterExpression2);
		Expression item = Expression.Assign(parameterExpression3, right);
		list.Add(item);
		item = Expression.Call(typeof(ComRuntimeHelpers).GetMethod("CheckThrowException"), parameterExpression3, parameterExpression, parameterExpression2, Expression.Constant(_methodDesc.Name, typeof(string)));
		list.Add(item);
		Expression right2 = Expression.Call(typeof(System.Runtime.InteropServices.BuiltInInteropVariantExtensions).GetMethod("ToObject"), InvokeResultVariable);
		VariantBuilder[] variantBuilders = _varEnumSelector.VariantBuilders;
		Expression[] array2 = MakeArgumentExpressions();
		list.Add(Expression.Assign(ReturnValueVariable, right2));
		int i = 0;
		for (int num4 = variantBuilders.Length; i < num4; i++)
		{
			Expression expression2 = variantBuilders[i].UpdateFromReturn(array2[i + 1]);
			if (expression2 != null)
			{
				list.Add(expression2);
			}
		}
		list.Add(Expression.Empty());
		return Expression.Block(new ParameterExpression[3] { parameterExpression, parameterExpression2, parameterExpression3 }, list);
	}

	private Expression GenerateFinallyBlock()
	{
		List<Expression> list = new List<Expression> { Expression.Call(typeof(UnsafeMethods).GetMethod("IUnknownRelease"), DispatchPointerVariable) };
		int i = 0;
		for (int num = _varEnumSelector.VariantBuilders.Length; i < num; i++)
		{
			Expression expression = _varEnumSelector.VariantBuilders[i].Clear();
			if (expression != null)
			{
				list.Add(expression);
			}
		}
		list.Add(Expression.Call(InvokeResultVariable, typeof(ComVariant).GetMethod("Dispose")));
		if (_dispIdsOfKeywordArgsPinned != null)
		{
			list.Add(Expression.Call(DispIdsOfKeywordArgsPinnedVariable, typeof(GCHandle).GetMethod("Free")));
		}
		list.Add(Expression.Empty());
		return Expression.Block(list);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expression MakeIDispatchInvokeTarget()
	{
		List<Expression> list = new List<Expression> { Expression.Assign(DispIdVariable, Expression.Property(_method, typeof(ComMethodDesc).GetProperty("DispId"))) };
		if (_totalExplicitArgs != 0)
		{
			list.Add(Expression.Assign(Expression.Field(DispParamsVariable, typeof(DISPPARAMS).GetField("rgvarg")), Expression.Call(typeof(UnsafeMethods).GetMethod("ConvertVariantByrefToPtr"), VariantArray.GetStructField(ParamVariantsVariable, 0))));
		}
		list.Add(Expression.Assign(Expression.Field(DispParamsVariable, typeof(DISPPARAMS).GetField("cArgs")), Expression.Constant(_totalExplicitArgs)));
		if (_methodDesc.IsPropertyPut)
		{
			list.Add(Expression.Assign(Expression.Field(DispParamsVariable, typeof(DISPPARAMS).GetField("cNamedArgs")), Expression.Constant(1)));
			list.Add(Expression.Assign(PropertyPutDispIdVariable, Expression.Constant(-3)));
			list.Add(Expression.Assign(Expression.Field(DispParamsVariable, typeof(DISPPARAMS).GetField("rgdispidNamedArgs")), Expression.Call(typeof(UnsafeMethods).GetMethod("ConvertInt32ByrefToPtr"), PropertyPutDispIdVariable)));
		}
		else
		{
			list.Add(Expression.Assign(Expression.Field(DispParamsVariable, typeof(DISPPARAMS).GetField("cNamedArgs")), Expression.Constant(_keywordArgNames.Length)));
		}
		list.Add(Expression.Assign(DispatchObjectVariable, _dispatch));
		list.Add(Expression.Assign(DispatchPointerVariable, Expression.Call(typeof(Marshal).GetMethod("GetIDispatchForObject"), DispatchObjectVariable)));
		Expression body = GenerateTryBlock();
		Expression expression = GenerateFinallyBlock();
		list.Add(Expression.TryFinally(body, expression));
		list.Add(ReturnValueVariable);
		List<ParameterExpression> list2 = new List<ParameterExpression>();
		VariantBuilder[] variantBuilders = _varEnumSelector.VariantBuilders;
		foreach (VariantBuilder variantBuilder in variantBuilders)
		{
			if (variantBuilder.TempVariable != null)
			{
				list2.Add(variantBuilder.TempVariable);
			}
		}
		return Expression.Block(list2, list);
	}

	private Expression[] MakeArgumentExpressions()
	{
		int num = 0;
		Expression[] array;
		if (_instance != null)
		{
			array = new Expression[_args.Length + 1];
			array[num++] = _instance;
		}
		else
		{
			array = new Expression[_args.Length];
		}
		for (int i = 0; i < _args.Length; i++)
		{
			array[num++] = _args[i].Expression;
		}
		return array;
	}
}
