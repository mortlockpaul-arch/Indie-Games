using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
internal sealed class ComMetaObject : DynamicMetaObject
{
	internal ComMetaObject(Expression expression, BindingRestrictions restrictions, object arg)
		: base(expression, restrictions, arg)
	{
	}

	public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
	{
		return binder.Defer(args.AddFirst(WrapSelf()));
	}

	public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
	{
		return binder.Defer(args.AddFirst(WrapSelf()));
	}

	public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
	{
		return binder.Defer(WrapSelf());
	}

	public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
	{
		return binder.Defer(WrapSelf(), value);
	}

	public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
	{
		return binder.Defer(WrapSelf(), indexes);
	}

	public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
	{
		return binder.Defer(WrapSelf(), indexes.AddLast(value));
	}

	private DynamicMetaObject WrapSelf()
	{
		return new DynamicMetaObject(ComObject.RcwToComObject(base.Expression), BindingRestrictions.GetExpressionRestriction(System.Linq.Expressions.Expression.Call(typeof(ComBinder).GetMethod("IsComObject", BindingFlags.Static | BindingFlags.Public), Helpers.Convert(base.Expression, typeof(object)))));
	}
}
