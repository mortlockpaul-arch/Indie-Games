using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;

namespace Microsoft.VisualBasic.CompilerServices;

internal sealed class VBSetComplexBinder : SetMemberBinder
{
	private readonly bool _optimisticSet;

	private readonly bool _rValueBase;

	private static readonly int s_hash = typeof(VBSetComplexBinder).GetHashCode();

	[RequiresUnreferencedCode("This subclass is unsafe. Marking ctor unsafe in order to suppress warnings for overridden methods as unsafe.")]
	public VBSetComplexBinder(string memberName, bool optimisticSet, bool rValueBase)
		: base(memberName, ignoreCase: true)
	{
		_optimisticSet = optimisticSet;
		_rValueBase = rValueBase;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The constructor of this subclass has been annotated.")]
	public override DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject errorSuggestion)
	{
		if (IDOUtils.NeedsDeferral(target, null, value))
		{
			return Defer(target, value);
		}
		if (errorSuggestion != null && !NewLateBinding.CanBindSet(target.Value, base.Name, value.Value, _optimisticSet, _rValueBase))
		{
			return errorSuggestion;
		}
		Expression expression = IDOUtils.ConvertToObject(value.Expression);
		Expression[] initializers = new Expression[1] { expression };
		Expression arg = Expression.Call(typeof(NewLateBinding).GetMethod("FallbackSetComplex"), target.Expression, Expression.Constant(base.Name), Expression.NewArrayInit(typeof(object), initializers), Expression.Constant(_optimisticSet), Expression.Constant(_rValueBase));
		return new DynamicMetaObject(Expression.Block(arg, expression), IDOUtils.CreateRestrictions(target, null, value));
	}

	public override bool Equals(object _other)
	{
		if (_other is VBSetComplexBinder vBSetComplexBinder && string.Equals(base.Name, vBSetComplexBinder.Name) && _optimisticSet == vBSetComplexBinder._optimisticSet)
		{
			return _rValueBase == vBSetComplexBinder._rValueBase;
		}
		return false;
	}

	public override int GetHashCode()
	{
		int num = s_hash ^ base.Name.GetHashCode();
		bool optimisticSet = _optimisticSet;
		int num2 = num ^ optimisticSet.GetHashCode();
		optimisticSet = _rValueBase;
		return num2 ^ optimisticSet.GetHashCode();
	}
}
