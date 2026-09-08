using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;

namespace Microsoft.VisualBasic.CompilerServices;

internal sealed class VBUnaryOperatorBinder : UnaryOperationBinder
{
	private readonly Symbols.UserDefinedOperator _Op;

	private static readonly int s_hash = typeof(VBUnaryOperatorBinder).GetHashCode();

	[RequiresUnreferencedCode("This subclass of BinaryOperationBinder is unsafe. Marking ctor unsafe in order to suppress warnings for overridden methods as unsafe.")]
	public VBUnaryOperatorBinder(Symbols.UserDefinedOperator op, ExpressionType linqOp)
		: base(linqOp)
	{
		_Op = op;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The constructor of this subclass has been annotated.")]
	public override DynamicMetaObject FallbackUnaryOperation(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
	{
		if (IDOUtils.NeedsDeferral(target))
		{
			return Defer(target);
		}
		if (errorSuggestion != null && (object)Operators.GetCallableUserDefinedOperator(_Op, target.Value) == null)
		{
			return errorSuggestion;
		}
		Expression expression = Expression.Call(typeof(Operators).GetMethod("FallbackInvokeUserDefinedOperator"), Expression.Constant(_Op, typeof(object)), Expression.NewArrayInit(typeof(object), IDOUtils.ConvertToObject(target.Expression)));
		return new DynamicMetaObject(expression, IDOUtils.CreateRestrictions(target));
	}

	public override bool Equals(object _other)
	{
		if (_other is VBUnaryOperatorBinder vBUnaryOperatorBinder && _Op == vBUnaryOperatorBinder._Op)
		{
			return base.Operation == vBUnaryOperatorBinder.Operation;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return s_hash ^ _Op.GetHashCode() ^ base.Operation.GetHashCode();
	}
}
