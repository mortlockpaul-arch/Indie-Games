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
internal sealed class CSharpBinaryOperationBinder : BinaryOperationBinder, ICSharpBinder
{
	private readonly CSharpBinaryOperationFlags _binopFlags;

	private readonly CSharpArgumentInfo[] _argumentInfo;

	private readonly RuntimeBinder _binder;

	private readonly Type _callingContext;

	[ExcludeFromCodeCoverage(Justification = "Name should not be called for this binder")]
	public string Name => null;

	public BindingFlag BindingFlags => (BindingFlag)0;

	public bool IsBinderThatCanHaveRefReceiver => false;

	internal bool IsLogicalOperation => (_binopFlags & CSharpBinaryOperationFlags.LogicalOperation) != 0;

	private bool IsChecked => _binder.IsChecked;

	public Expr DispatchPayload(RuntimeBinder runtimeBinder, ArgumentObject[] arguments, LocalVariableSymbol[] locals)
	{
		return runtimeBinder.BindBinaryOperation(this, arguments, locals);
	}

	public void PopulateSymbolTableWithName(Type callingType, ArgumentObject[] arguments)
	{
		string cLROperatorName = base.Operation.GetCLROperatorName();
		SymbolTable.PopulateSymbolTableWithName(cLROperatorName, null, arguments[0].Type);
		SymbolTable.PopulateSymbolTableWithName(cLROperatorName, null, arguments[1].Type);
	}

	CSharpArgumentInfo ICSharpBinder.GetArgumentInfo(int index)
	{
		return _argumentInfo[index];
	}

	public CSharpBinaryOperationBinder(ExpressionType operation, bool isChecked, CSharpBinaryOperationFlags binaryOperationFlags, Type callingContext, IEnumerable<CSharpArgumentInfo> argumentInfo)
		: base(operation)
	{
		_binopFlags = binaryOperationFlags;
		_callingContext = callingContext;
		_argumentInfo = BinderHelper.ToArray(argumentInfo);
		_binder = new RuntimeBinder(callingContext, isChecked);
	}

	public int GetGetBinderEquivalenceHash()
	{
		int h = _callingContext?.GetHashCode() ?? 0;
		h = HashHelpers.Combine(h, (int)_binopFlags);
		if (IsChecked)
		{
			h = HashHelpers.Combine(h, 1);
		}
		h = HashHelpers.Combine(h, (int)base.Operation);
		return BinderHelper.AddArgHashes(h, _argumentInfo);
	}

	public bool IsEquivalentTo(ICSharpBinder other)
	{
		if (!(other is CSharpBinaryOperationBinder cSharpBinaryOperationBinder))
		{
			return false;
		}
		if (_binopFlags != cSharpBinaryOperationBinder._binopFlags || base.Operation != cSharpBinaryOperationBinder.Operation || IsChecked != cSharpBinaryOperationBinder.IsChecked || _callingContext != cSharpBinaryOperationBinder._callingContext)
		{
			return false;
		}
		return BinderHelper.CompareArgInfos(_argumentInfo, cSharpBinaryOperationBinder._argumentInfo);
	}

	public override DynamicMetaObject FallbackBinaryOperation(DynamicMetaObject target, DynamicMetaObject arg, DynamicMetaObject errorSuggestion)
	{
		BinderHelper.ValidateBindArgument(target, "target");
		BinderHelper.ValidateBindArgument(arg, "arg");
		return BinderHelper.Bind(this, _binder, new DynamicMetaObject[2] { target, arg }, _argumentInfo, errorSuggestion);
	}
}
