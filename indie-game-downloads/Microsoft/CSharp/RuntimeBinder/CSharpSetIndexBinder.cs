using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Numerics.Hashing;
using Microsoft.CSharp.RuntimeBinder.ComInterop;
using Microsoft.CSharp.RuntimeBinder.Semantics;

namespace Microsoft.CSharp.RuntimeBinder;

[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class CSharpSetIndexBinder : SetIndexBinder, ICSharpBinder
{
	private readonly CSharpArgumentInfo[] _argumentInfo;

	private readonly RuntimeBinder _binder;

	private readonly Type _callingContext;

	public string Name => "$Item$";

	public BindingFlag BindingFlags => (BindingFlag)0;

	public bool IsBinderThatCanHaveRefReceiver => true;

	internal bool IsCompoundAssignment { get; }

	private bool IsChecked => _binder.IsChecked;

	public Expr DispatchPayload(RuntimeBinder runtimeBinder, ArgumentObject[] arguments, LocalVariableSymbol[] locals)
	{
		return runtimeBinder.BindAssignment(this, arguments, locals);
	}

	public void PopulateSymbolTableWithName(Type callingType, ArgumentObject[] arguments)
	{
		SymbolTable.PopulateSymbolTableWithName("$Item$", null, arguments[0].Type);
	}

	CSharpArgumentInfo ICSharpBinder.GetArgumentInfo(int index)
	{
		return _argumentInfo[index];
	}

	public CSharpSetIndexBinder(bool isCompoundAssignment, bool isChecked, Type callingContext, IEnumerable<CSharpArgumentInfo> argumentInfo)
		: base(BinderHelper.CreateCallInfo(ref argumentInfo, 2))
	{
		IsCompoundAssignment = isCompoundAssignment;
		_argumentInfo = argumentInfo as CSharpArgumentInfo[];
		_callingContext = callingContext;
		_binder = new RuntimeBinder(callingContext, isChecked);
	}

	public int GetGetBinderEquivalenceHash()
	{
		int num = _callingContext?.GetHashCode() ?? 0;
		if (IsChecked)
		{
			num = HashHelpers.Combine(num, 1);
		}
		if (IsCompoundAssignment)
		{
			num = HashHelpers.Combine(num, 1);
		}
		return BinderHelper.AddArgHashes(num, _argumentInfo);
	}

	public bool IsEquivalentTo(ICSharpBinder other)
	{
		if (!(other is CSharpSetIndexBinder cSharpSetIndexBinder))
		{
			return false;
		}
		if (_callingContext != cSharpSetIndexBinder._callingContext || IsChecked != cSharpSetIndexBinder.IsChecked || IsCompoundAssignment != cSharpSetIndexBinder.IsCompoundAssignment || _argumentInfo.Length != cSharpSetIndexBinder._argumentInfo.Length)
		{
			return false;
		}
		return BinderHelper.CompareArgInfos(_argumentInfo, cSharpSetIndexBinder._argumentInfo);
	}

	public override DynamicMetaObject FallbackSetIndex(DynamicMetaObject target, DynamicMetaObject[] indexes, DynamicMetaObject value, DynamicMetaObject errorSuggestion)
	{
		if (ComBinder.TryBindSetIndex(this, target, indexes, value, out var result))
		{
			return result;
		}
		BinderHelper.ValidateBindArgument(target, "target");
		BinderHelper.ValidateBindArgument(indexes, "indexes");
		BinderHelper.ValidateBindArgument(value, "value");
		return BinderHelper.Bind(this, _binder, BinderHelper.Cons(target, indexes, value), _argumentInfo, errorSuggestion);
	}
}
