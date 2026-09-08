using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;

namespace Microsoft.VisualBasic.CompilerServices;

internal sealed class VBInvokeDefaultFallbackBinder : GetIndexBinder
{
	private readonly bool _reportErrors;

	private static readonly int s_hash = typeof(VBInvokeDefaultFallbackBinder).GetHashCode();

	[RequiresUnreferencedCode("This subclass is unsafe. Marking ctor unsafe in order to suppress warnings for overridden methods as unsafe.")]
	public VBInvokeDefaultFallbackBinder(CallInfo callInfo, bool reportErrors)
		: base(callInfo)
	{
		_reportErrors = reportErrors;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The constructor of this subclass has been annotated.")]
	public override DynamicMetaObject FallbackGetIndex(DynamicMetaObject target, DynamicMetaObject[] packedArgs, DynamicMetaObject errorSuggestion)
	{
		if (IDOUtils.NeedsDeferral(target, packedArgs))
		{
			return Defer(target, packedArgs);
		}
		Expression[] args = null;
		string[] argNames = null;
		object[] argValues = null;
		IDOUtils.UnpackArguments(packedArgs, base.CallInfo, ref args, ref argNames, ref argValues);
		if (errorSuggestion != null && !NewLateBinding.CanBindInvokeDefault(target.Value, argValues, argNames, _reportErrors))
		{
			return errorSuggestion;
		}
		ParameterExpression parameterExpression = Expression.Variable(typeof(object), "result");
		ParameterExpression parameterExpression2 = Expression.Variable(typeof(object[]), "array");
		Expression right = Expression.Call(typeof(NewLateBinding).GetMethod("FallbackInvokeDefault2"), target.Expression, Expression.Assign(parameterExpression2, Expression.NewArrayInit(typeof(object), args)), Expression.Constant(argNames, typeof(string[])), Expression.Constant(_reportErrors));
		return new DynamicMetaObject(Expression.Block(new ParameterExpression[2] { parameterExpression, parameterExpression2 }, Expression.Assign(parameterExpression, right), IDOUtils.GetWriteBack(args, parameterExpression2), parameterExpression), IDOUtils.CreateRestrictions(target, packedArgs));
	}

	public override bool Equals(object _other)
	{
		if (_other is VBInvokeDefaultFallbackBinder vBInvokeDefaultFallbackBinder && base.CallInfo.Equals(vBInvokeDefaultFallbackBinder.CallInfo))
		{
			return _reportErrors == vBInvokeDefaultFallbackBinder._reportErrors;
		}
		return false;
	}

	public override int GetHashCode()
	{
		int num = s_hash ^ base.CallInfo.GetHashCode();
		bool reportErrors = _reportErrors;
		return num ^ reportErrors.GetHashCode();
	}
}
