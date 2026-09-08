using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class DispCallableMetaObject : DynamicMetaObject
{
	private readonly DispCallable _callable;

	internal DispCallableMetaObject(Expression expression, DispCallable callable)
		: base(expression, BindingRestrictions.Empty, callable)
	{
		_callable = callable;
	}

	public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
	{
		return BindGetOrInvoke(indexes, binder.CallInfo) ?? base.BindGetIndex(binder, indexes);
	}

	public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
	{
		return BindGetOrInvoke(args, binder.CallInfo) ?? base.BindInvoke(binder, args);
	}

	private DynamicMetaObject BindGetOrInvoke(DynamicMetaObject[] args, CallInfo callInfo)
	{
		IDispatchComObject dispatchComObject = _callable.DispatchComObject;
		string memberName = _callable.MemberName;
		if (dispatchComObject.TryGetMemberMethod(memberName, out var method) || dispatchComObject.TryGetMemberMethodExplicit(memberName, out method))
		{
			bool[] isByRef = ComBinderHelpers.ProcessArgumentsForCom(ref args);
			return BindComInvoke(method, args, callInfo, isByRef);
		}
		return null;
	}

	public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
	{
		IDispatchComObject dispatchComObject = _callable.DispatchComObject;
		string memberName = _callable.MemberName;
		bool holdsNull = value.Value == null && value.HasValue;
		if (dispatchComObject.TryGetPropertySetter(memberName, out var method, value.LimitType, holdsNull) || dispatchComObject.TryGetPropertySetterExplicit(memberName, out method, value.LimitType, holdsNull))
		{
			bool[] list = ComBinderHelpers.ProcessArgumentsForCom(ref indexes);
			list = list.AddLast(item: false);
			DynamicMetaObject dynamicMetaObject = BindComInvoke(method, indexes.AddLast(value), binder.CallInfo, list);
			return new DynamicMetaObject(System.Linq.Expressions.Expression.Block(dynamicMetaObject.Expression, System.Linq.Expressions.Expression.Convert(value.Expression, typeof(object))), dynamicMetaObject.Restrictions);
		}
		return base.BindSetIndex(binder, indexes, value);
	}

	private DynamicMetaObject BindComInvoke(ComMethodDesc method, DynamicMetaObject[] indexes, CallInfo callInfo, bool[] isByRef)
	{
		Expression expression = Helpers.Convert(base.Expression, typeof(DispCallable));
		return new ComInvokeBinder(callInfo, indexes, isByRef, DispCallableRestrictions(), System.Linq.Expressions.Expression.Constant(method), System.Linq.Expressions.Expression.Property(expression, typeof(DispCallable).GetProperty("DispatchObject")), method).Invoke();
	}

	private BindingRestrictions DispCallableRestrictions()
	{
		Expression expression = base.Expression;
		BindingRestrictions typeRestriction = BindingRestrictions.GetTypeRestriction(expression, typeof(DispCallable));
		Expression expression2 = Helpers.Convert(expression, typeof(DispCallable));
		MemberExpression expr = System.Linq.Expressions.Expression.Property(expression2, typeof(DispCallable).GetProperty("DispatchComObject"));
		MemberExpression left = System.Linq.Expressions.Expression.Property(expression2, typeof(DispCallable).GetProperty("DispId"));
		BindingRestrictions restrictions = IDispatchMetaObject.IDispatchRestriction(expr, _callable.DispatchComObject.ComTypeDesc);
		BindingRestrictions expressionRestriction = BindingRestrictions.GetExpressionRestriction(System.Linq.Expressions.Expression.Equal(left, System.Linq.Expressions.Expression.Constant(_callable.DispId)));
		return typeRestriction.Merge(restrictions).Merge(expressionRestriction);
	}
}
