using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;

namespace Microsoft.VisualBasic.CompilerServices;

internal sealed class VBIndexSetComplexBinder : SetIndexBinder
{
	private readonly bool _optimisticSet;

	private readonly bool _rValueBase;

	private static readonly int s_hash = typeof(VBIndexSetComplexBinder).GetHashCode();

	[RequiresUnreferencedCode("This subclass is unsafe. Marking ctor unsafe in order to suppress warnings for overridden methods as unsafe.")]
	public VBIndexSetComplexBinder(CallInfo callInfo, bool optimisticSet, bool rValueBase)
		: base(callInfo)
	{
		_optimisticSet = optimisticSet;
		_rValueBase = rValueBase;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The constructor of this subclass has been annotated.")]
	public override DynamicMetaObject FallbackSetIndex(DynamicMetaObject target, DynamicMetaObject[] packedIndexes, DynamicMetaObject value, DynamicMetaObject errorSuggestion)
	{
		checked
		{
			if (IDOUtils.NeedsDeferral(target, packedIndexes, value))
			{
				Array.Resize(ref packedIndexes, packedIndexes.Length + 1);
				packedIndexes[packedIndexes.Length - 1] = value;
				return Defer(target, packedIndexes);
			}
			string[] argNames = null;
			Expression[] args = null;
			object[] argValues = null;
			IDOUtils.UnpackArguments(packedIndexes, base.CallInfo, ref args, ref argNames, ref argValues);
			object[] array = new object[argValues.Length + 1];
			argValues.CopyTo(array, 0);
			array[argValues.Length] = value.Value;
			if (errorSuggestion != null && !NewLateBinding.CanIndexSetComplex(target.Value, array, argNames, _optimisticSet, _rValueBase))
			{
				return errorSuggestion;
			}
			Expression expression = IDOUtils.ConvertToObject(value.Expression);
			Expression[] array2 = new Expression[args.Length + 1];
			args.CopyTo(array2, 0);
			array2[args.Length] = expression;
			Expression arg = Expression.Call(typeof(NewLateBinding).GetMethod("FallbackIndexSetComplex"), target.Expression, Expression.NewArrayInit(typeof(object), array2), Expression.Constant(argNames, typeof(string[])), Expression.Constant(_optimisticSet), Expression.Constant(_rValueBase));
			return new DynamicMetaObject(Expression.Block(arg, expression), IDOUtils.CreateRestrictions(target, packedIndexes, value));
		}
	}

	public override bool Equals(object _other)
	{
		if (_other is VBIndexSetComplexBinder vBIndexSetComplexBinder && base.CallInfo.Equals(vBIndexSetComplexBinder.CallInfo) && _optimisticSet == vBIndexSetComplexBinder._optimisticSet)
		{
			return _rValueBase == vBIndexSetComplexBinder._rValueBase;
		}
		return false;
	}

	public override int GetHashCode()
	{
		int num = s_hash ^ base.CallInfo.GetHashCode();
		bool optimisticSet = _optimisticSet;
		int num2 = num ^ optimisticSet.GetHashCode();
		optimisticSet = _rValueBase;
		return num2 ^ optimisticSet.GetHashCode();
	}
}
