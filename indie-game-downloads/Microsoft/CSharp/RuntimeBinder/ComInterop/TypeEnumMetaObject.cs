using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal sealed class TypeEnumMetaObject : DynamicMetaObject
{
	private readonly ComTypeEnumDesc _desc;

	internal TypeEnumMetaObject(ComTypeEnumDesc desc, Expression expression)
		: base(expression, BindingRestrictions.Empty, desc)
	{
		_desc = desc;
	}

	public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
	{
		if (_desc.HasMember(binder.Name))
		{
			return new DynamicMetaObject(System.Linq.Expressions.Expression.Constant(((ComTypeEnumDesc)base.Value).GetValue(binder.Name), typeof(object)), EnumRestrictions());
		}
		throw new NotImplementedException();
	}

	public override IEnumerable<string> GetDynamicMemberNames()
	{
		return _desc.GetMemberNames();
	}

	private BindingRestrictions EnumRestrictions()
	{
		return BindingRestrictions.GetTypeRestriction(base.Expression, typeof(ComTypeEnumDesc)).Merge(BindingRestrictions.GetExpressionRestriction(System.Linq.Expressions.Expression.Equal(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Property(Helpers.Convert(base.Expression, typeof(ComTypeEnumDesc)), typeof(ComTypeDesc).GetProperty("TypeLib")), typeof(ComTypeLibDesc).GetProperty("Guid")), System.Linq.Expressions.Expression.Constant(_desc.TypeLib.Guid)))).Merge(BindingRestrictions.GetExpressionRestriction(System.Linq.Expressions.Expression.Equal(System.Linq.Expressions.Expression.Property(Helpers.Convert(base.Expression, typeof(ComTypeEnumDesc)), typeof(ComTypeEnumDesc).GetProperty("TypeName")), System.Linq.Expressions.Expression.Constant(_desc.TypeName))));
	}
}
