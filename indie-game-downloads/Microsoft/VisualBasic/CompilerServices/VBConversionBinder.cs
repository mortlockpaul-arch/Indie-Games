using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;

namespace Microsoft.VisualBasic.CompilerServices;

internal sealed class VBConversionBinder : ConvertBinder
{
	private static readonly int s_hash = typeof(VBConversionBinder).GetHashCode();

	[RequiresUnreferencedCode("This subclass is unsafe. Marking ctor unsafe in order to suppress warnings for overridden methods as unsafe.")]
	public VBConversionBinder(Type t)
		: base(t, @explicit: true)
	{
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The constructor of this subclass has been annotated.")]
	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2111:ReflectionToDynamicallyAccessedMembers", Justification = "The constructor of this subclass has been annotated.")]
	public override DynamicMetaObject FallbackConvert(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
	{
		if (IDOUtils.NeedsDeferral(target))
		{
			return Defer(target);
		}
		if (errorSuggestion != null && !Conversions.CanUserDefinedConvert(target.Value, base.Type))
		{
			return errorSuggestion;
		}
		Expression expression = Expression.Call(typeof(Conversions).GetMethod("FallbackUserDefinedConversion"), target.Expression, Expression.Constant(base.Type, typeof(Type)));
		return new DynamicMetaObject(Expression.Convert(expression, ReturnType), IDOUtils.CreateRestrictions(target));
	}

	public override bool Equals(object _other)
	{
		if (_other is VBConversionBinder vBConversionBinder)
		{
			return base.Type.Equals(vBConversionBinder.Type);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return s_hash ^ base.Type.GetHashCode();
	}
}
