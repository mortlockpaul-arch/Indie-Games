using System.Dynamic;
using System.Linq.Expressions;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal class ComFallbackMetaObject : DynamicMetaObject
{
	internal ComFallbackMetaObject(Expression expression, BindingRestrictions restrictions, object arg)
		: base(expression, restrictions, arg)
	{
	}

	public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
	{
		return binder.FallbackGetIndex(UnwrapSelf(), indexes);
	}

	public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
	{
		return binder.FallbackSetIndex(UnwrapSelf(), indexes, value);
	}

	public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
	{
		return binder.FallbackGetMember(UnwrapSelf());
	}

	public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
	{
		return binder.FallbackInvokeMember(UnwrapSelf(), args);
	}

	public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
	{
		return binder.FallbackSetMember(UnwrapSelf(), value);
	}

	protected virtual ComUnwrappedMetaObject UnwrapSelf()
	{
		return new ComUnwrappedMetaObject(ComObject.RcwFromComObject(base.Expression), base.Restrictions.Merge(ComBinderHelpers.GetTypeRestrictionForDynamicMetaObject(this)), ((ComObject)base.Value).RuntimeCallableWrapper);
	}
}
