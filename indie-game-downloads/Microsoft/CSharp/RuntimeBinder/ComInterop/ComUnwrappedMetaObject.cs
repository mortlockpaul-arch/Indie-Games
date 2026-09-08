using System.Dynamic;
using System.Linq.Expressions;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal sealed class ComUnwrappedMetaObject : DynamicMetaObject
{
	internal ComUnwrappedMetaObject(Expression expression, BindingRestrictions restrictions, object value)
		: base(expression, restrictions, value)
	{
	}
}
