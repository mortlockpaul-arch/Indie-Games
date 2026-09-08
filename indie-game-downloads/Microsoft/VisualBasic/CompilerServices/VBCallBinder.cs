using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;

namespace Microsoft.VisualBasic.CompilerServices;

internal sealed class VBCallBinder : InvokeMemberBinder
{
	private readonly bool _ignoreReturn;

	private static readonly int s_hash = typeof(VBCallBinder).GetHashCode();

	[RequiresUnreferencedCode("This subclass is unsafe. Marking ctor unsafe in order to suppress warnings for overridden methods as unsafe.")]
	public VBCallBinder(string memberName, CallInfo callInfo, bool ignoreReturn)
		: base(memberName, ignoreCase: true, callInfo)
	{
		_ignoreReturn = ignoreReturn;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The constructor of this subclass has been annotated.")]
	public override DynamicMetaObject FallbackInvokeMember(DynamicMetaObject target, DynamicMetaObject[] packedArgs, DynamicMetaObject errorSuggestion)
	{
		if (IDOUtils.NeedsDeferral(target, packedArgs))
		{
			return Defer(target, packedArgs);
		}
		Expression[] args = null;
		string[] argNames = null;
		object[] argValues = null;
		IDOUtils.UnpackArguments(packedArgs, base.CallInfo, ref args, ref argNames, ref argValues);
		if (errorSuggestion != null && !NewLateBinding.CanBindCall(target.Value, base.Name, argValues, argNames, _ignoreReturn))
		{
			return errorSuggestion;
		}
		ParameterExpression parameterExpression = Expression.Variable(typeof(object), "result");
		ParameterExpression parameterExpression2 = Expression.Variable(typeof(object[]), "array");
		Expression right = Expression.Call(typeof(NewLateBinding).GetMethod("FallbackCall"), target.Expression, Expression.Constant(base.Name, typeof(string)), Expression.Assign(parameterExpression2, Expression.NewArrayInit(typeof(object), args)), Expression.Constant(argNames, typeof(string[])), Expression.Constant(_ignoreReturn, typeof(bool)));
		return new DynamicMetaObject(Expression.Block(new ParameterExpression[2] { parameterExpression, parameterExpression2 }, Expression.Assign(parameterExpression, right), IDOUtils.GetWriteBack(args, parameterExpression2), parameterExpression), IDOUtils.CreateRestrictions(target, packedArgs));
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The constructor of this subclass has been annotated.")]
	public override DynamicMetaObject FallbackInvoke(DynamicMetaObject target, DynamicMetaObject[] packedArgs, DynamicMetaObject errorSuggestion)
	{
		return new VBInvokeBinder(base.CallInfo, lateCall: true).FallbackInvoke(target, packedArgs, errorSuggestion);
	}

	public override bool Equals(object _other)
	{
		if (_other is VBCallBinder vBCallBinder && string.Equals(base.Name, vBCallBinder.Name) && base.CallInfo.Equals(vBCallBinder.CallInfo))
		{
			return _ignoreReturn == vBCallBinder._ignoreReturn;
		}
		return false;
	}

	public override int GetHashCode()
	{
		int num = s_hash ^ base.Name.GetHashCode() ^ base.CallInfo.GetHashCode();
		bool ignoreReturn = _ignoreReturn;
		return num ^ ignoreReturn.GetHashCode();
	}
}
