using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal sealed class TypeLibMetaObject : DynamicMetaObject
{
	private readonly ComTypeLibDesc _lib;

	internal TypeLibMetaObject(Expression expression, ComTypeLibDesc lib)
		: base(expression, BindingRestrictions.Empty, lib)
	{
		_lib = lib;
	}

	private DynamicMetaObject TryBindGetMember(string name)
	{
		if (_lib.HasMember(name))
		{
			BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(base.Expression, typeof(ComTypeLibDesc)).Merge(BindingRestrictions.GetExpressionRestriction(System.Linq.Expressions.Expression.Equal(System.Linq.Expressions.Expression.Property(Helpers.Convert(base.Expression, typeof(ComTypeLibDesc)), typeof(ComTypeLibDesc).GetProperty("Guid")), System.Linq.Expressions.Expression.Constant(_lib.Guid))));
			return new DynamicMetaObject(System.Linq.Expressions.Expression.Constant(((ComTypeLibDesc)base.Value).GetTypeLibObjectDesc(name)), restrictions);
		}
		return null;
	}

	public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
	{
		return TryBindGetMember(binder.Name) ?? base.BindGetMember(binder);
	}

	public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
	{
		DynamicMetaObject dynamicMetaObject = TryBindGetMember(binder.Name);
		if (dynamicMetaObject != null)
		{
			return binder.FallbackInvoke(dynamicMetaObject, args, null);
		}
		return base.BindInvokeMember(binder, args);
	}

	public override IEnumerable<string> GetDynamicMemberNames()
	{
		return _lib.GetMemberNames();
	}
}
