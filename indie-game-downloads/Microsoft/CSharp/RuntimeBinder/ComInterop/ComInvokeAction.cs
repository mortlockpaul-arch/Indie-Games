using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
internal sealed class ComInvokeAction : InvokeBinder
{
	internal ComInvokeAction(CallInfo callInfo)
		: base(callInfo)
	{
	}

	public override DynamicMetaObject FallbackInvoke(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject errorSuggestion)
	{
		if (ComBinder.TryBindInvoke(this, target, args, out var result))
		{
			return result;
		}
		return errorSuggestion ?? new DynamicMetaObject(Expression.Throw(Expression.New(typeof(NotSupportedException).GetConstructor(new Type[1] { typeof(string) }), Expression.Constant(System.SR.COMCannotPerformCall)), typeof(object)), target.Restrictions.Merge(BindingRestrictions.Combine(args)));
	}
}
