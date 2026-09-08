using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class IDispatchMetaObject : ComFallbackMetaObject
{
	private readonly IDispatchComObject _self;

	private static readonly bool[] s_false = new bool[1];

	internal IDispatchMetaObject(Expression expression, IDispatchComObject self)
		: base(expression, BindingRestrictions.Empty, self)
	{
		_self = self;
	}

	public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
	{
		if (_self.TryGetMemberMethod(binder.Name, out var method) || _self.TryGetMemberMethodExplicit(binder.Name, out method))
		{
			bool[] isByRef = ComBinderHelpers.ProcessArgumentsForCom(ref args);
			return BindComInvoke(args, method, binder.CallInfo, isByRef);
		}
		return base.BindInvokeMember(binder, args);
	}

	public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
	{
		if (_self.TryGetGetItem(out var value))
		{
			bool[] isByRef = ComBinderHelpers.ProcessArgumentsForCom(ref args);
			return BindComInvoke(args, value, binder.CallInfo, isByRef);
		}
		return base.BindInvoke(binder, args);
	}

	private DynamicMetaObject BindComInvoke(DynamicMetaObject[] args, ComMethodDesc method, CallInfo callInfo, bool[] isByRef)
	{
		return new ComInvokeBinder(callInfo, args, isByRef, IDispatchRestriction(), System.Linq.Expressions.Expression.Constant(method), System.Linq.Expressions.Expression.Property(Helpers.Convert(base.Expression, typeof(IDispatchComObject)), typeof(IDispatchComObject).GetProperty("DispatchObject")), method).Invoke();
	}

	public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
	{
		bool canReturnCallables = (binder as ComBinder.ComGetMemberBinder)?._canReturnCallables ?? false;
		if (_self.TryGetMemberMethod(binder.Name, out var method))
		{
			return BindGetMember(method, canReturnCallables);
		}
		if (_self.TryGetMemberEvent(binder.Name, out var @event))
		{
			return BindEvent(@event);
		}
		if (_self.TryGetMemberMethodExplicit(binder.Name, out method))
		{
			return BindGetMember(method, canReturnCallables);
		}
		return base.BindGetMember(binder);
	}

	private DynamicMetaObject BindGetMember(ComMethodDesc method, bool canReturnCallables)
	{
		if (method.IsDataMember && method.ParamCount == 0)
		{
			return BindComInvoke(DynamicMetaObject.EmptyMetaObjects, method, new CallInfo(0), Array.Empty<bool>());
		}
		if (!canReturnCallables)
		{
			return BindComInvoke(DynamicMetaObject.EmptyMetaObjects, method, new CallInfo(0), Array.Empty<bool>());
		}
		return new DynamicMetaObject(System.Linq.Expressions.Expression.Call(typeof(ComRuntimeHelpers).GetMethod("CreateDispCallable"), Helpers.Convert(base.Expression, typeof(IDispatchComObject)), System.Linq.Expressions.Expression.Constant(method)), IDispatchRestriction());
	}

	private DynamicMetaObject BindEvent(ComEventDesc eventDesc)
	{
		return new DynamicMetaObject(System.Linq.Expressions.Expression.Call(typeof(ComRuntimeHelpers).GetMethod("CreateComEvent"), ComObject.RcwFromComObject(base.Expression), System.Linq.Expressions.Expression.Constant(eventDesc.SourceIID), System.Linq.Expressions.Expression.Constant(eventDesc.Dispid)), IDispatchRestriction());
	}

	public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
	{
		if (_self.TryGetGetItem(out var value))
		{
			bool[] isByRef = ComBinderHelpers.ProcessArgumentsForCom(ref indexes);
			return BindComInvoke(indexes, value, binder.CallInfo, isByRef);
		}
		return base.BindGetIndex(binder, indexes);
	}

	public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
	{
		if (_self.TryGetSetItem(out var value2))
		{
			bool[] list = ComBinderHelpers.ProcessArgumentsForCom(ref indexes);
			list = list.AddLast(item: false);
			DynamicMetaObject dynamicMetaObject = BindComInvoke(indexes.AddLast(value), value2, binder.CallInfo, list);
			return new DynamicMetaObject(System.Linq.Expressions.Expression.Block(dynamicMetaObject.Expression, System.Linq.Expressions.Expression.Convert(value.Expression, typeof(object))), dynamicMetaObject.Restrictions);
		}
		return base.BindSetIndex(binder, indexes, value);
	}

	public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
	{
		return TryPropertyPut(binder, value) ?? TryEventHandlerNoop(binder, value) ?? base.BindSetMember(binder, value);
	}

	private DynamicMetaObject TryPropertyPut(SetMemberBinder binder, DynamicMetaObject value)
	{
		bool holdsNull = value.Value == null && value.HasValue;
		if (_self.TryGetPropertySetter(binder.Name, out var method, value.LimitType, holdsNull) || _self.TryGetPropertySetterExplicit(binder.Name, out method, value.LimitType, holdsNull))
		{
			BindingRestrictions restrictions = IDispatchRestriction();
			Expression dispatch = System.Linq.Expressions.Expression.Property(Helpers.Convert(base.Expression, typeof(IDispatchComObject)), typeof(IDispatchComObject).GetProperty("DispatchObject"));
			DynamicMetaObject dynamicMetaObject = new ComInvokeBinder(new CallInfo(1), new DynamicMetaObject[1] { value }, s_false, restrictions, System.Linq.Expressions.Expression.Constant(method), dispatch, method).Invoke();
			return new DynamicMetaObject(System.Linq.Expressions.Expression.Block(dynamicMetaObject.Expression, System.Linq.Expressions.Expression.Convert(value.Expression, typeof(object))), dynamicMetaObject.Restrictions);
		}
		return null;
	}

	private DynamicMetaObject TryEventHandlerNoop(SetMemberBinder binder, DynamicMetaObject value)
	{
		if (_self.TryGetMemberEvent(binder.Name, out var _) && value.LimitType == typeof(BoundDispEvent))
		{
			return new DynamicMetaObject(System.Linq.Expressions.Expression.Constant(null), value.Restrictions.Merge(IDispatchRestriction()).Merge(BindingRestrictions.GetTypeRestriction(value.Expression, typeof(BoundDispEvent))));
		}
		return null;
	}

	private BindingRestrictions IDispatchRestriction()
	{
		return IDispatchRestriction(base.Expression, _self.ComTypeDesc);
	}

	internal static BindingRestrictions IDispatchRestriction(Expression expr, ComTypeDesc typeDesc)
	{
		return BindingRestrictions.GetTypeRestriction(expr, typeof(IDispatchComObject)).Merge(BindingRestrictions.GetExpressionRestriction(System.Linq.Expressions.Expression.Equal(System.Linq.Expressions.Expression.Property(Helpers.Convert(expr, typeof(IDispatchComObject)), typeof(IDispatchComObject).GetProperty("ComTypeDesc")), System.Linq.Expressions.Expression.Constant(typeDesc))));
	}

	protected override ComUnwrappedMetaObject UnwrapSelf()
	{
		return new ComUnwrappedMetaObject(ComObject.RcwFromComObject(base.Expression), IDispatchRestriction(), _self.RuntimeCallableWrapper);
	}
}
