using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
internal sealed class ComClassMetaObject : DynamicMetaObject
{
	internal ComClassMetaObject(Expression expression, ComTypeClassDesc cls)
		: base(expression, BindingRestrictions.Empty, cls)
	{
	}

	public override DynamicMetaObject BindCreateInstance(CreateInstanceBinder binder, DynamicMetaObject[] args)
	{
		return new DynamicMetaObject(System.Linq.Expressions.Expression.Call(Helpers.Convert(base.Expression, typeof(ComTypeClassDesc)), typeof(ComTypeClassDesc).GetMethod("CreateInstance")), BindingRestrictions.Combine(args).Merge(BindingRestrictions.GetTypeRestriction(base.Expression, typeof(ComTypeClassDesc))));
	}
}
