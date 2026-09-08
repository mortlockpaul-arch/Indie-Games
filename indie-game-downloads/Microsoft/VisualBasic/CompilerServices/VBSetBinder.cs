using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;

namespace Microsoft.VisualBasic.CompilerServices;

internal sealed class VBSetBinder : SetMemberBinder
{
	private static readonly int s_hash = typeof(VBSetBinder).GetHashCode();

	[RequiresUnreferencedCode("This subclass is unsafe. Marking ctor unsafe in order to suppress warnings for overridden methods as unsafe.")]
	public VBSetBinder(string memberName)
		: base(memberName, ignoreCase: true)
	{
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The constructor of this subclass has been annotated.")]
	public override DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject errorSuggestion)
	{
		if (IDOUtils.NeedsDeferral(target, null, value))
		{
			return Defer(target, value);
		}
		if (errorSuggestion != null && !NewLateBinding.CanBindSet(target.Value, base.Name, value.Value, optimisticSet: false, rValueBase: false))
		{
			return errorSuggestion;
		}
		Expression expression = IDOUtils.ConvertToObject(value.Expression);
		Expression[] initializers = new Expression[1] { expression };
		Expression arg = Expression.Call(typeof(NewLateBinding).GetMethod("FallbackSet"), target.Expression, Expression.Constant(base.Name), Expression.NewArrayInit(typeof(object), initializers));
		return new DynamicMetaObject(Expression.Block(arg, expression), IDOUtils.CreateRestrictions(target, null, value));
	}

	public override bool Equals(object _other)
	{
		if (_other is VBSetBinder vBSetBinder)
		{
			return string.Equals(base.Name, vBSetBinder.Name);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return s_hash ^ base.Name.GetHashCode();
	}
}
