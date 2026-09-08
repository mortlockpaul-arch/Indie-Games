using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;

namespace Microsoft.VisualBasic.CompilerServices;

internal sealed class VBGetBinder : InvokeMemberBinder
{
	private static readonly int s_hash = typeof(VBGetBinder).GetHashCode();

	[RequiresUnreferencedCode("This subclass is unsafe. Marking ctor unsafe in order to suppress warnings for overridden methods as unsafe.")]
	public VBGetBinder(string memberName, CallInfo callInfo)
		: base(memberName, ignoreCase: true, callInfo)
	{
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
		if (errorSuggestion != null && !NewLateBinding.CanBindGet(target.Value, base.Name, argValues, argNames))
		{
			return errorSuggestion;
		}
		ParameterExpression parameterExpression = Expression.Variable(typeof(object), "result");
		ParameterExpression parameterExpression2 = Expression.Variable(typeof(object[]), "array");
		Expression right = Expression.Call(typeof(NewLateBinding).GetMethod("FallbackGet"), target.Expression, Expression.Constant(base.Name), Expression.Assign(parameterExpression2, Expression.NewArrayInit(typeof(object), args)), Expression.Constant(argNames, typeof(string[])));
		return new DynamicMetaObject(Expression.Block(new ParameterExpression[2] { parameterExpression, parameterExpression2 }, Expression.Assign(parameterExpression, right), IDOUtils.GetWriteBack(args, parameterExpression2), parameterExpression), IDOUtils.CreateRestrictions(target, packedArgs));
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The constructor of this subclass has been annotated.")]
	public override DynamicMetaObject FallbackInvoke(DynamicMetaObject target, DynamicMetaObject[] packedArgs, DynamicMetaObject errorSuggestion)
	{
		return new VBInvokeBinder(base.CallInfo, lateCall: false).FallbackInvoke(target, packedArgs, errorSuggestion);
	}

	public override bool Equals(object _other)
	{
		if (_other is VBGetBinder vBGetBinder && string.Equals(base.Name, vBGetBinder.Name))
		{
			return base.CallInfo.Equals(vBGetBinder.CallInfo);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return s_hash ^ base.Name.GetHashCode() ^ base.CallInfo.GetHashCode();
	}
}
