using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using System.Numerics.Hashing;
using Microsoft.CSharp.RuntimeBinder.Semantics;

namespace Microsoft.CSharp.RuntimeBinder;

[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class CSharpUnaryOperationBinder : UnaryOperationBinder, ICSharpBinder
{
	private readonly CSharpArgumentInfo[] _argumentInfo;

	private readonly RuntimeBinder _binder;

	private readonly Type _callingContext;

	[ExcludeFromCodeCoverage(Justification = "Name should not be called for this binder")]
	public string Name => null;

	public BindingFlag BindingFlags => (BindingFlag)0;

	public bool IsBinderThatCanHaveRefReceiver => false;

	private bool IsChecked => _binder.IsChecked;

	public Expr DispatchPayload(RuntimeBinder runtimeBinder, ArgumentObject[] arguments, LocalVariableSymbol[] locals)
	{
		return runtimeBinder.BindUnaryOperation(this, arguments, locals);
	}

	public void PopulateSymbolTableWithName(Type callingType, ArgumentObject[] arguments)
	{
		SymbolTable.PopulateSymbolTableWithName(base.Operation.GetCLROperatorName(), null, arguments[0].Type);
	}

	CSharpArgumentInfo ICSharpBinder.GetArgumentInfo(int index)
	{
		return _argumentInfo[index];
	}

	public CSharpUnaryOperationBinder(ExpressionType operation, bool isChecked, Type callingContext, IEnumerable<CSharpArgumentInfo> argumentInfo)
		: base(operation)
	{
		_argumentInfo = BinderHelper.ToArray(argumentInfo);
		_callingContext = callingContext;
		_binder = new RuntimeBinder(callingContext, isChecked);
	}

	public int GetGetBinderEquivalenceHash()
	{
		int h = _callingContext?.GetHashCode() ?? 0;
		h = HashHelpers.Combine(h, (int)base.Operation);
		if (IsChecked)
		{
			h = HashHelpers.Combine(h, 1);
		}
		return BinderHelper.AddArgHashes(h, _argumentInfo);
	}

	public bool IsEquivalentTo(ICSharpBinder other)
	{
		if (!(other is CSharpUnaryOperationBinder cSharpUnaryOperationBinder))
		{
			return false;
		}
		if (base.Operation != cSharpUnaryOperationBinder.Operation || IsChecked != cSharpUnaryOperationBinder.IsChecked || _callingContext != cSharpUnaryOperationBinder._callingContext)
		{
			return false;
		}
		return BinderHelper.CompareArgInfos(_argumentInfo, cSharpUnaryOperationBinder._argumentInfo);
	}

	public override DynamicMetaObject FallbackUnaryOperation(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
	{
		BinderHelper.ValidateBindArgument(target, "target");
		return BinderHelper.Bind(this, _binder, new DynamicMetaObject[1] { target }, _argumentInfo, errorSuggestion);
	}
}
